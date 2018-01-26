using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
/**************************************************************************
 * Developer: 		Joe
 * Computer:		JOE-PC
 * Domain:			Joe-pc
 * CreatedOn:		2014/12/4 12:15:10
 * Description:		Please input class summary
 * Version History:	Created,2014/12/4 12:15:10
 * 
 * 
 **************************************************************************/
using Sunnet.Cli.Vcw.Controllers;
using Sunnet.Cli.Business.Vcw;
using Sunnet.Cli.Business.Users;
using Sunnet.Cli.Business.Schools;
using Sunnet.Cli.UIBase.Filters;
using Sunnet.Framework.Permission;
using Sunnet.Cli.UIBase.Models;
using Sunnet.Cli.Vcw.Models;
using Sunnet.Cli.Business.Vcw.Models;
using Sunnet.Framework.Helpers;
using Sunnet.Cli.Core.Users.Entities;
using Sunnet.Cli.Core.Vcw.Enums;
using Sunnet.Framework.Extensions;
using Sunnet.Cli.Core.Vcw.Entities;
using Sunnet.Cli.Business.Common;
using System.Web.Script.Serialization;
using Sunnet.Framework.Core.Tool;

namespace Sunnet.Cli.Vcw.Areas.Admin.Controllers
{
    public class TeacherSendAssignmentController : BaseController
    {
        VcwBusiness _vcwBusiness;
        UserBusiness _userBusiness;
        SchoolBusiness _schoolBusiness;
        VCW_MasterDataBusiness _vcwMasterDataBusiness;

        public TeacherSendAssignmentController()
        {
            _vcwBusiness = new VcwBusiness(VcwUnitWorkContext);
            _vcwMasterDataBusiness = new VCW_MasterDataBusiness(VcwUnitWorkContext);
            _userBusiness = new UserBusiness();
            _schoolBusiness = new SchoolBusiness();
        }


        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.Admin, Anonymity = Anonymous.Verified)]
        public ActionResult TeacherIndex()
        {
            return View();
        }


        /// <summary>
        /// 查找Teacher列表
        /// </summary>
        /// <param name="community"></param>
        /// <param name="school"></param>
        /// <param name="teacher"></param>
        /// <param name="sort"></param>
        /// <param name="order"></param>
        /// <param name="first"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.Admin, Anonymity = Anonymous.Verified)]
        public string SearchTeacher(int community = -1, int school = -1, int teacher = -1,
            string sort = "ID", string order = "Asc", int first = 0, int count = 10)
        {
            var total = 0;
            var expression = PredicateHelper.True<TeacherEntity>();
            if (community > 0)
                expression = expression.And(o => o.UserInfo.UserCommunitySchools.Any(p => p.CommunityId == community));
            if (school > 0)
                expression = expression.And(o => o.UserInfo.UserCommunitySchools.Any(p => p.SchoolId == school));
            if (teacher > 0)
                expression = expression.And(o => o.UserInfo.ID == teacher);

            var list = _userBusiness.GetTeachersByAdmin(expression, sort, order, first, count, out total);

            var result = new { total = total, data = list };
            return JsonHelper.SerializeObject(result);
        }


        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.Admin, Anonymity = Anonymous.Verified)]
        public ActionResult New(int teacherId, string teacher_select, int teacherTableId = 0)
        {
            IEnumerable<SelectListItem> AssignmentTypes = AssignmentTypeEnum.TeacherAssignment.ToSelectList(true);
            AssignmentTypes = AssignmentTypes.Where(a => a.Value != ((int)AssignmentTypeEnum.CoachAssignment).ToString());  //去除CoachAssignment选项
            ViewBag.AssignmentType = AssignmentTypes;
            ViewBag.Session = _vcwMasterDataBusiness.GetActiveSessions().ToSelectList()
                .AddDefaultItem(ViewTextHelper.DefaultPleaseSelectText, "-1").ToList();
            ViewBag.Wave = _vcwMasterDataBusiness.GetActiveWaves().ToSelectList()
                .AddDefaultItem(ViewTextHelper.DefaultPleaseSelectText, "-1").ToList();
            ViewBag.EncryptUserID = UserInfo.ID;
            ViewBag.Teachers = teacher_select;
            ViewBag.Teacher = teacherId;
            ViewBag.TeacherTableId = teacherTableId;
            ViewBag.UploadTypes = _vcwMasterDataBusiness.GetActiveUploadTypes();
            ViewBag.Contents = _vcwMasterDataBusiness.GetActiveAssignment_Content_Datas();
            ViewBag.Contexts = _vcwMasterDataBusiness.GetActiveContext_Datas();
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.Admin, Anonymity = Anonymous.Verified)]
        public string New(AssignmentModel assignmentModel, int[] Context_Assignment, int[] Content, int[] UploadType, string uploadfiles, string[] stgreports)
        {

            var response = new PostFormResponse();
            AssignmentEntity assignment = new AssignmentEntity();
            assignment.AssignmentType = assignmentModel.Type;
            assignment.DueDate = assignmentModel.DueDate;
            assignment.FeedbackCalllDate = assignmentModel.CallDate == null ? CommonAgent.MinDate : assignmentModel.CallDate.Value;
            assignment.WaveId = assignmentModel.WaveId;
            assignment.SessionId = assignmentModel.SessionId;
            assignment.Description = assignmentModel.Description;
            assignment.ContentOther = assignmentModel.ContentOther;
            assignment.ContextOther = assignmentModel.ContextOther;
            assignment.UpdatedOn = DateTime.Now;
            assignment.SendUserId = UserInfo.ID;
            assignment.CreatedBy = UserInfo.ID;
            assignment.UpdatedBy = UserInfo.ID;
            assignment.Status = AssignmentStatus.New;
            assignment.ReceiveUserId = assignmentModel.ReceiveUserId;
            assignment.Watch = assignmentModel.Watch;

            List<AssignmentFileEntity> assignmentFiles = new List<AssignmentFileEntity>();
            if (!string.IsNullOrEmpty(uploadfiles))
            {
                if (uploadfiles.EndsWith(",]"))
                {
                    //uploadfiles格式：[{'FileName1':'FileName1','FilePath1':'FilePath1'},{'FileName2':'FileName2','FilePath2':'FilePath2'},]
                    uploadfiles = uploadfiles.Replace(",]", "]");
                    //将传入的字符串解析成List<AssignmentFileEntity>
                    JavaScriptSerializer Serializer = new JavaScriptSerializer();
                    assignmentFiles =
                        Serializer.Deserialize<List<AssignmentFileEntity>>(uploadfiles);
                }
            }
            OperationResult result = _vcwBusiness.SendAssignment(assignment, Context_Assignment, Content, UploadType, assignmentFiles, "", null, stgreports);
            response.Success = result.ResultType == OperationResultType.Success;
            response.Message = result.Message;
            response.ModelState = ModelState;
            return JsonHelper.SerializeObject(response);
        }

        /// <summary>
        /// 批量创建assignment
        /// </summary>
        /// <param name="assignment"></param>
        /// <param name="Context"></param>
        /// <param name="Content"></param>
        /// <param name="UploadType"></param>
        /// <param name="uploadfiles"></param>
        /// <param name="teachers"></param>
        /// <returns></returns>
        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.Admin, Anonymity = Anonymous.Verified)]
        public string NewBatch(AssignmentModel assignmentModel, int[] Context_Assignment, int[] Content, int[] UploadType,
            string uploadfiles, string teachers)
        {
            var response = new PostFormResponse();
            if (!string.IsNullOrEmpty(teachers))
            {
                if (teachers.Trim().Length > 0)//默认值时
                {
                    AssignmentEntity assignment = new AssignmentEntity();
                    assignment.AssignmentType = assignmentModel.Type;
                    assignment.DueDate = assignmentModel.DueDate;
                    assignment.FeedbackCalllDate = assignmentModel.CallDate == null ? CommonAgent.MinDate : assignmentModel.CallDate.Value;
                    assignment.WaveId = assignmentModel.WaveId;
                    assignment.SessionId = assignmentModel.SessionId;
                    assignment.Description = assignmentModel.Description;
                    assignment.ContentOther = assignmentModel.ContentOther;
                    assignment.ContextOther = assignmentModel.ContextOther;
                    assignment.UpdatedOn = DateTime.Now;
                    assignment.SendUserId = UserInfo.ID;
                    assignment.CreatedBy = UserInfo.ID;
                    assignment.UpdatedBy = UserInfo.ID;
                    assignment.Status = AssignmentStatus.New;
                    assignment.ReceiveUserId = assignmentModel.ReceiveUserId;
                    assignment.Watch = assignmentModel.Watch;


                    if (assignment.FeedbackCalllDate == null)
                    {
                        assignment.FeedbackCalllDate = CommonAgent.MinDate;
                    }
                    else
                    {
                        if (assignment.FeedbackCalllDate < CommonAgent.MinDate)
                        {
                            assignment.FeedbackCalllDate = CommonAgent.MinDate;
                        }
                    }

                    if (!string.IsNullOrEmpty(uploadfiles))
                    {
                        if (uploadfiles.EndsWith(",]"))
                        {
                            List<AssignmentFileEntity> assignmentFiles = new List<AssignmentFileEntity>();
                            //uploadfiles格式：[{'FileName1':'FileName1','FilePath1':'FilePath1'},{'FileName2':'FileName2','FilePath2':'FilePath2'},]
                            uploadfiles = uploadfiles.Replace(",]", "]");
                            //将传入的字符串解析成List<AssignmentFileEntity>
                            JavaScriptSerializer Serializer = new JavaScriptSerializer();
                            assignmentFiles =
                                Serializer.Deserialize<List<AssignmentFileEntity>>(uploadfiles);
                            assignment.AssignmentFiles = assignmentFiles;
                        }
                    }

                    if (Context_Assignment != null)  //添加AssignmentContext
                    {
                        List<AssignmentContextEntity> contextEntities = new List<AssignmentContextEntity>();

                        foreach (int item in Context_Assignment)
                        {
                            AssignmentContextEntity AssignmentContext = new AssignmentContextEntity();
                            AssignmentContext.ContextId = item;
                            contextEntities.Add(AssignmentContext);
                        }
                        assignment.AssignmentContexts = contextEntities;
                    }

                    if (Content != null) //添加AssignmentContent
                    {
                        List<AssignmentContentEntity> contentEntities = new List<AssignmentContentEntity>();
                        foreach (int item in Content)
                        {
                            AssignmentContentEntity AssignmentContent = new AssignmentContentEntity();
                            AssignmentContent.ContentId = item;
                            contentEntities.Add(AssignmentContent);
                        }
                        assignment.AssignmentContents = contentEntities;
                    }

                    if (UploadType != null) //添加AssignmentUploadType
                    {
                        List<AssignmentUploadTypeEntity> assignmentUploadTypeEntities = new List<AssignmentUploadTypeEntity>();
                        foreach (int item in UploadType)
                        {
                            AssignmentUploadTypeEntity AssignmentUploadTypeContent = new AssignmentUploadTypeEntity();
                            AssignmentUploadTypeContent.TypeId = item;
                            assignmentUploadTypeEntities.Add(AssignmentUploadTypeContent);
                        }
                        assignment.AssignmentUploadTypes = assignmentUploadTypeEntities;
                    }

                    List<int> teacher_list = new List<int>();
                    string[] teacher_arr = teachers.Split(',');
                    foreach (string item in teacher_arr)
                    {
                        teacher_list.Add(int.Parse(item));
                    }

                    OperationResult result = _vcwBusiness.SendAssignment(assignment, teacher_list);
                    response.Success = result.ResultType == OperationResultType.Success;
                    response.Message = result.Message;
                    response.ModelState = ModelState;
                    return JsonHelper.SerializeObject(response);
                }
                else
                {
                    response.Success = false;
                    response.ModelState = ModelState;
                    return JsonHelper.SerializeObject(response);
                }
            }
            else
            {
                response.Success = false;
                return JsonHelper.SerializeObject(response);
            }
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.Admin, Anonymity = Anonymous.Verified)]
        public ActionResult Edit(int id)
        {
            AssignmentModel Assignment = _vcwBusiness.GetTeacherAssignment(id);
            int TeacherTableId = _userBusiness.GetTeacherId(Assignment.ReceiveUserId);
            IEnumerable<SelectListItem> AssignmentTypes = AssignmentTypeEnum.TeacherVIP.ToSelectList(true);
            AssignmentTypes = AssignmentTypes.Where(a => a.Value != ((int)AssignmentTypeEnum.CoachAssignment).ToString());  //去除CoachAssignment选项
            ViewBag.AssignmentType = AssignmentTypes;
            ViewBag.Sessions = _vcwMasterDataBusiness.GetActiveSessions().ToSelectList()
                .AddDefaultItem(ViewTextHelper.DefaultPleaseSelectText, "-1").ToList();
            ViewBag.Waves = _vcwMasterDataBusiness.GetActiveWaves().ToSelectList()
                .AddDefaultItem(ViewTextHelper.DefaultPleaseSelectText, "-1").ToList();
            ViewBag.TeacherTableId = TeacherTableId;
            ViewBag.UploadTypes = _vcwMasterDataBusiness.GetActiveUploadTypes();
            ViewBag.Contents = _vcwMasterDataBusiness.GetActiveAssignment_Content_Datas();
            ViewBag.Contexts = _vcwMasterDataBusiness.GetActiveContext_Datas();
            TeacherListModel teacher = _userBusiness.GetTeacherInfoByUserId(Assignment.ReceiveUserId);
            if (teacher != null)
            {
                ViewBag.Teacher = teacher;
            }
            return View(Assignment);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.Admin, Anonymity = Anonymous.Verified)]
        public string Edit(AssignmentModel assignmentModel, int[] Context_Assignment, int[] Content, int[] UploadType,
            string uploadfiles, string checkuploadedfiles, string[] stgreports)
        {
            AssignmentEntity assignment = _vcwBusiness.GetAssignment(assignmentModel.ID);
            assignment.DueDate = assignmentModel.DueDate;
            assignment.FeedbackCalllDate = assignmentModel.CallDate == null ? CommonAgent.MinDate : assignmentModel.CallDate.Value;

            assignment.WaveId = assignmentModel.WaveId;
            assignment.SessionId = assignmentModel.SessionId;
            assignment.Description = assignmentModel.Description;
            assignment.ContentOther = assignmentModel.ContentOther;
            assignment.ContextOther = assignmentModel.ContextOther;
            assignment.UpdatedOn = DateTime.Now;
            assignment.UpdatedBy = UserInfo.ID;
            assignment.Watch = assignmentModel.Watch;
            var response = new PostFormResponse();
            if (ModelState.IsValid)
            {
                List<AssignmentFileEntity> assignmentFiles = new List<AssignmentFileEntity>();
                if (!string.IsNullOrEmpty(uploadfiles))
                {
                    if (uploadfiles.EndsWith(",]"))
                    {
                        //uploadfiles格式：[{'FileName1':'FileName1','FilePath1':'FilePath1'},{'FileName2':'FileName2','FilePath2':'FilePath2'},]
                        uploadfiles = uploadfiles.Replace(",]", "]");
                        //将传入的字符串解析成List<AssignmentFileEntity>
                        JavaScriptSerializer Serializer = new JavaScriptSerializer();
                        assignmentFiles =
                            Serializer.Deserialize<List<AssignmentFileEntity>>(uploadfiles);
                    }
                }


                OperationResult result = _vcwBusiness.SendAssignment(assignment, Context_Assignment, Content,
                    UploadType, assignmentFiles, checkuploadedfiles, null, stgreports);
                response.Success = result.ResultType == OperationResultType.Success;
                response.Message = result.Message;
            }
            else
            {
                response.Success = false;
            }
            response.ModelState = ModelState;
            return JsonHelper.SerializeObject(response);
        }


        [HttpPost]
        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.Admin, Anonymity = Anonymous.Verified)]
        public string Delete(int[] assignment_select)
        {
            var response = new PostFormResponse();
            if (assignment_select != null)
            {
                List<int> deleteids = assignment_select.ToList();
                OperationResult result = _vcwBusiness.DeleteAssignment(deleteids);
                response.Success = result.ResultType == OperationResultType.Success;
                response.Message = result.Message;
                return JsonHelper.SerializeObject(response);
            }
            else
            {
                response.Success = false;
                return JsonHelper.SerializeObject(response);
            }
        }


        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.Admin, Anonymity = Anonymous.Verified)]
        public ActionResult AssignmentIndex()
        {
            AssignmentListModel AssignmentList = new AssignmentListModel();
            return View(AssignmentList);
        }

        /// <summary>
        /// 查找Assignment列表
        /// </summary>
        /// <param name="community"></param>
        /// <param name="school"></param>
        /// <param name="teacher"></param>
        /// <param name="status"></param>
        /// <param name="sort"></param>
        /// <param name="order"></param>
        /// <param name="first"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.Admin, Anonymity = Anonymous.Verified)]
        public string SearchAssignment(int community = -1, int school = -1, int teacher = -1, int status = -1,
            string sort = "ID", string order = "Asc", int first = 0, int count = 10)
        {
            var total = 0;
            var expression = PredicateHelper.True<AssignmentEntity>();

            if (status > 0)
                expression = expression.And(o => o.Status == (AssignmentStatus)status);
            if (community > 0)
            {
                List<int> teacherIds = new UserBusiness().GetTeacheridsByCommunity(community);
                expression = expression.And(o => teacherIds.Contains(o.ReceiveUserId));
            }
            if (school > 0)
            {
                List<int> teacherIds = new UserBusiness().GetTeacheridsBySchool(school);
                expression = expression.And(o => teacherIds.Contains(o.ReceiveUserId));
            }
            if (teacher > 0)
                expression = expression.And(o => o.ReceiveUserId == teacher);


            expression = expression.And(o =>
                         (o.AssignmentType == AssignmentTypeEnum.TeacherVIP
                         || o.AssignmentType == AssignmentTypeEnum.TeacherAssignment)
                          && o.IsDelete == false && o.SendUserId == UserInfo.ID);

            var list = _vcwBusiness.GetTeacherAssignments(expression, sort, order, first, count, out total);

            if (list.Count > 0)
                list = _vcwBusiness.FormatTeacherAssignments(list);

            var result = new { total = total, data = list };
            return JsonHelper.SerializeObject(result);
        }
    }
}