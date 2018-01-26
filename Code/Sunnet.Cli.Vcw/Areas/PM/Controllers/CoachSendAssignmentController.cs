using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
/**************************************************************************
 * Developer: 		Joe
 * Computer:		JOE-PC
 * Domain:			Joe-pc
 * CreatedOn:		2014/10/23 12:15:10
 * Description:		Please input class summary
 * Version History:	Created,2014/10/23 12:15:10
 * 
 * 
 **************************************************************************/
using Sunnet.Cli.Vcw.Controllers;
using Sunnet.Cli.UIBase.Filters;
using Sunnet.Framework.Permission;
using Sunnet.Cli.UIBase.Models;
using Sunnet.Cli.Business.Vcw;
using Sunnet.Cli.Business.Users;
using Sunnet.Cli.Business.Vcw.Models;
using Sunnet.Cli.Business.Users.Models;
using Sunnet.Cli.Core.Vcw.Enums;
using Sunnet.Framework.Extensions;
using Sunnet.Framework.Helpers;
using Sunnet.Cli.Core.Vcw.Entities;
using System.Web.Script.Serialization;
using Sunnet.Framework.Core.Tool;
using Sunnet.Cli.Business.Communities.Models;
using Sunnet.Cli.Business.Communities;
using System.Linq.Expressions;
using Sunnet.Cli.Business.Common;
using Sunnet.Cli.Vcw.Models;
using Sunnet.Cli.Core.Users.Entities;

namespace Sunnet.Cli.Vcw.Areas.PM.Controllers
{
    public class CoachSendAssignmentController : BaseController
    {

        VcwBusiness _vcwBusiness;
        UserBusiness _userBusiness;
        VCW_MasterDataBusiness _vcwMasterDataBusiness;

        public CoachSendAssignmentController()
        {
            _vcwBusiness = new VcwBusiness(VcwUnitWorkContext);
            _vcwMasterDataBusiness = new VCW_MasterDataBusiness(VcwUnitWorkContext);
            _userBusiness = new UserBusiness();
        }

        //
        // GET: /PM/CoacheSendAssignment/
        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.PMCoaches, Anonymity = Anonymous.Verified)]
        public ActionResult Index()
        {
            List<object> Dropdown = GetDropDownItems.GetItemsByPM(UserInfo.ID);
            ViewBag.Coaches = Dropdown[0];
            ViewBag.Communities = Dropdown[1];
            return View();
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.PMCoaches, Anonymity = Anonymous.Verified)]
        public string SearchCoach(int community = -1, int coach = -1, string sort = "CommunityName", string order = "Asc", int first = 0, int count = 10)
        {
            int total = 0;

            var expression = PredicateHelper.True<CoordCoachEntity>();
            if (community > 0)
                expression = expression.And(r => r.User.UserCommunitySchools.Any(x => x.CommunityId == community));
            if (coach > 0)
                expression = expression.And(r => r.User.ID == coach);

            List<CoachesListModel> list = _userBusiness.GetSendCoachByPM(expression, UserInfo.ID, sort, order, first, count, out total);
            var result = new { total = total, data = list };
            return JsonHelper.SerializeObject(result);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.PMCoaches, Anonymity = Anonymous.Verified)]
        public ActionResult New(int coachId, string coach_select)
        {
            ViewBag.Coaches = coach_select;
            ViewBag.Session = _vcwMasterDataBusiness.GetActiveSessions().ToSelectList()
                .AddDefaultItem(ViewTextHelper.DefaultPleaseSelectText, "-1").ToList();
            ViewBag.Coach = coachId;
            ViewBag.UploadTypes = _vcwMasterDataBusiness.GetActiveUploadTypes();
            ViewBag.Strategies = _vcwMasterDataBusiness.GetActiveAssignmentCoachingStrategy_Datas();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.PMCoaches, Anonymity = Anonymous.Verified)]
        public string New(AssignmentModel assignmentModel, int[] Strategy, int[] UploadType, string uploadfiles)
        {
            var response = new PostFormResponse();
            AssignmentEntity assignment = new AssignmentEntity();
            assignment.DueDate = assignmentModel.DueDate;
            assignment.FeedbackCalllDate = assignmentModel.CallDate == null ? CommonAgent.MinDate : assignmentModel.CallDate.Value;
            assignment.SessionId = assignmentModel.SessionId;
            assignment.Description = assignmentModel.Description;
            assignment.UpdatedOn = DateTime.Now;
            assignment.SendUserId = UserInfo.ID;
            assignment.CreatedBy = UserInfo.ID;
            assignment.UpdatedBy = UserInfo.ID;
            assignment.Status = AssignmentStatus.New;
            assignment.ReceiveUserId = assignmentModel.ReceiveUserId;
            assignment.Watch = assignmentModel.Watch;
            assignment.AssignmentType = AssignmentTypeEnum.CoachAssignment;

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

            OperationResult result = _vcwBusiness.SendAssignment(assignment, null, null, UploadType, assignmentFiles, "", Strategy, null);
            response.Success = result.ResultType == OperationResultType.Success;
            response.Message = result.Message;
            response.ModelState = ModelState;
            return JsonHelper.SerializeObject(response);
        }


        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.PMCoaches, Anonymity = Anonymous.Verified)]
        public string NewBatch(AssignmentModel assignmentModel, int[] Strategy, int[] UploadType,
            string uploadfiles, string coaches)
        {
            var response = new PostFormResponse();
            if (!string.IsNullOrEmpty(coaches))
            {
                if (coaches.Trim().Length > 0)//默认值时
                {
                    AssignmentEntity assignment = new AssignmentEntity();
                    assignment.DueDate = assignmentModel.DueDate;
                    assignment.FeedbackCalllDate = assignmentModel.CallDate == null ? CommonAgent.MinDate : assignmentModel.CallDate.Value;
                    assignment.SessionId = assignmentModel.SessionId;
                    assignment.Description = assignmentModel.Description;
                    assignment.UpdatedOn = DateTime.Now;
                    assignment.SendUserId = UserInfo.ID;
                    assignment.CreatedBy = UserInfo.ID;
                    assignment.UpdatedBy = UserInfo.ID;
                    assignment.Status = AssignmentStatus.New;
                    assignment.ReceiveUserId = assignmentModel.ReceiveUserId;
                    assignment.Watch = assignmentModel.Watch;
                    assignment.AssignmentType = AssignmentTypeEnum.CoachAssignment;

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

                    if (Strategy != null) //添加AssignmentStrategy
                    {
                        List<AssignmentStrategyEntity> assignmentStrategyEntities = new List<AssignmentStrategyEntity>();
                        foreach (int item in Strategy)
                        {
                            AssignmentStrategyEntity AssignmentStrategy = new AssignmentStrategyEntity();
                            AssignmentStrategy.StrategyId = item;
                            assignmentStrategyEntities.Add(AssignmentStrategy);
                        }
                        assignment.AssignmentStrategies = assignmentStrategyEntities;
                    }

                    List<int> coach_list = new List<int>();
                    string[] coach_arr = coaches.Split(',');
                    foreach (string item in coach_arr)
                    {
                        coach_list.Add(int.Parse(item));
                    }

                    OperationResult result = _vcwBusiness.SendAssignment(assignment, coach_list);
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

        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.PMCoaches, Anonymity = Anonymous.Verified)]
        public ActionResult AssignmentIndex()
        {
            List<object> Dropdown = GetDropDownItems.GetItemsByPM(UserInfo.ID);
            ViewBag.Coaches = Dropdown[0];
            ViewBag.Communities = Dropdown[1];
            return View();
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
        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.PMCoaches, Anonymity = Anonymous.Verified)]
        public string SearchAssignment(int community, int coach, int status,
            string sort = "ID", string order = "Asc", int first = 0, int count = 10)
        {
            var total = 0;
            var expression = PredicateHelper.True<AssignmentEntity>();

            if (status > 0)
                expression = expression.And(o => o.Status == (AssignmentStatus)status);
            if (community > 0)
            {
                List<int> user_Ids = _userBusiness.GetAssignedUsersByCommunity(community).Select(a => a.UserId).ToList();
                expression = expression.And(r => user_Ids.Contains(r.ReceiveUserId));
            }

            if (coach > 0)
                expression = expression.And(o => o.ReceiveUserId == coach);


            expression = expression.And(o =>
                         o.AssignmentType == AssignmentTypeEnum.CoachAssignment
                         && o.SendUserId == UserInfo.ID && o.IsDelete == false);

            List<AssignmentListModel> list = new List<AssignmentListModel>();
            list = _vcwBusiness.GetCoachAssignments(expression, sort, order, first, count, out total);

            if (list.Count > 0)
            {
                List<SelectItemModel> UploadTypes = _vcwMasterDataBusiness.GetActiveUploadTypes();
                List<SelectItemModel> Strategies = _vcwMasterDataBusiness.GetActiveAssignmentCoachingStrategy_Datas();
                foreach (AssignmentListModel item in list)
                {
                    item.UploadTypes = UploadTypes.Where(r => item.UploadTypeIds.Contains(r.ID));
                    item.Strategies = Strategies.Where(r => item.StrategyIds.Contains(r.ID));
                }
            }

            var result = new { total = total, data = list };
            return JsonHelper.SerializeObject(result);
        }


        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.PMCoaches, Anonymity = Anonymous.Verified)]
        public ActionResult View(int id)
        {
            AssignmentModel Assignment = _vcwBusiness.GetTeacherAssignment(id);
            ViewBag.Sessions = _vcwMasterDataBusiness.GetActiveSessions().ToSelectList()
                .AddDefaultItem(ViewTextHelper.DefaultPleaseSelectText, "-1").ToList();
            ViewBag.UploadTypes = _vcwMasterDataBusiness.GetActiveUploadTypes();
            ViewBag.Strategies = _vcwMasterDataBusiness.GetActiveAssignmentCoachingStrategy_Datas();
            CoachesListModel coach = _userBusiness.GetCoachInfoById(Assignment.ReceiveUserId);
            if (coach != null)
            {
                ViewBag.Coach = coach;
            }
            return View(Assignment);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.PMCoaches, Anonymity = Anonymous.Verified)]
        public string Edit(AssignmentModel assignmentModel, int[] Strategy, int[] UploadType, string uploadfiles, string checkuploadedfiles)
        {
            AssignmentEntity assignment = _vcwBusiness.GetAssignment(assignmentModel.ID);
            assignment.Watch = assignmentModel.Watch;
            assignment.DueDate = assignmentModel.DueDate;
            assignment.FeedbackCalllDate = assignmentModel.CallDate.Value;
            assignment.SessionId = assignmentModel.SessionId;
            assignment.Description = assignmentModel.Description;
            assignment.UpdatedOn = DateTime.Now;
            assignment.UpdatedBy = UserInfo.ID;
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

                OperationResult result = _vcwBusiness.SendAssignment(assignment, null, null, UploadType, assignmentFiles, checkuploadedfiles, Strategy, null);
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
        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.PMCoaches, Anonymity = Anonymous.Verified)]
        public string DeleteAssignment(int[] assignment_select)
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
    }
}