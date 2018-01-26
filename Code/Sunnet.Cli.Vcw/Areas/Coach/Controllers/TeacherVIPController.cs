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
using Sunnet.Cli.Business.Vcw;
using Sunnet.Cli.Business.Users;
using Sunnet.Cli.Business.Schools;
using Sunnet.Cli.Business.Vcw.Models;
using Sunnet.Framework.Helpers;
using Sunnet.Cli.Core.Vcw.Entities;
using Sunnet.Cli.Core.Vcw.Enums;
using Sunnet.Cli.UIBase.Models;
using Sunnet.Framework.Extensions;
using Sunnet.Framework.Core.Tool;
using Sunnet.Cli.UIBase;
using System.Web.Script.Serialization;
using Sunnet.Framework;
using Sunnet.Cli.Business.Common;
using Sunnet.Cli.UIBase.Filters;
using Sunnet.Framework.Permission;
using Sunnet.Cli.Vcw.Models;
using Sunnet.Cli.Business;
using Sunnet.Cli.Core.Users.Enums;

namespace Sunnet.Cli.Vcw.Areas.Coach.Controllers
{
    public class TeacherVIPController : BaseController
    {
        VcwBusiness _vcwBusiness;
        UserBusiness _userBusiness;
        SchoolBusiness _schoolBusiness;
        VCW_MasterDataBusiness _vcwMasterDataBusiness;

        public TeacherVIPController()
        {
            _vcwBusiness = new VcwBusiness(VcwUnitWorkContext);
            _vcwMasterDataBusiness = new VCW_MasterDataBusiness(VcwUnitWorkContext);
            _userBusiness = new UserBusiness();
            _schoolBusiness = new SchoolBusiness();
        }


        //
        // GET: /Coach/TeacherVIP/
        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.CoachTeachers, Anonymity = Anonymous.Verified)]
        public ActionResult Index()
        {
            if (UserInfo.Role == Role.Coordinator)  //Coordinator角色可查看所分配的Community下的所有Teacher
            {
                List<object> DropdownItems = GetDropDownItems.GetItemsByPM_Teacher(UserInfo.ID);
                if (DropdownItems.Count >= 4)
                {
                    ViewBag.Communities = DropdownItems[0];
                    ViewBag.Schools = DropdownItems[2];
                    ViewBag.Teachers = DropdownItems[3];
                }
            }
            else if (UserInfo.Role == Role.Mentor_coach)
            {
                List<object> dropdownData = GetDropDownItems.GetItems(UserInfo.ID);

                if (dropdownData.Count >= 3)
                {
                    ViewBag.Teachers = dropdownData[0];
                    ViewBag.Schools = dropdownData[1];
                    ViewBag.Communities = dropdownData[2];
                }
            }
            else
            {
                List<object> dropdownData = GetDropDownItems.GetExternalItems(UserInfo);

                if (dropdownData.Count >= 3)
                {
                    ViewBag.Teachers = dropdownData[0];
                    ViewBag.Schools = dropdownData[1];
                    ViewBag.Communities = dropdownData[2];
                }
            }
            return View();
        }


        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.CoachTeachers, Anonymity = Anonymous.Verified)]
        public ActionResult View(int id)
        {
            AssignmentModel Assignment = _vcwBusiness.GetTeacherAssignment(id);
            ViewBag.DueDate = Assignment.DueDate.FormatDateString();
            ViewBag.Wave = Assignment.WaveText;
            ViewBag.Status = Assignment.Status.ToDescription();
            ViewBag.Content = Assignment.Content;
            ViewBag.Context = Assignment.Context;
            ViewBag.AssignmentId = Assignment.ID;
            ViewBag.Description = Assignment.Description;
            ViewBag.VIPDicList = _vcwBusiness.GetVIPDocuments();
            TeacherListModel teacher = _userBusiness.GetTeacherInfoByUserId(Assignment.ReceiveUserId);
            if (teacher != null)
            {
                ViewBag.Teacher = teacher;
            }
            return View();
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.Coach, Anonymity = Anonymous.Verified)]
        public ActionResult ViewFile(int id, string redirect = "")
        {
            ViewBag.Redirect = redirect;
            TeacherVIPFileModel model = _vcwBusiness.GetTeacherVIPFileModelByCoach(id);
            ViewBag.IsCoach = model.UploadUserId == UserInfo.ID;//用于判断是否为自身上传，是否可编辑基本信息
            ViewBag.IsVideo = true;
            TeacherListModel teacher = _userBusiness.GetTeacherInfoByUserId(model.OwnerId);
            if (teacher != null)
            {
                ViewBag.Teacher = teacher;
            }
            if (model.UploadUserId == UserInfo.ID)
            {
                ViewBag.Languages = _vcwMasterDataBusiness.GetActiveVideo_Language_Datas();
            }
            ViewBag.SelectionList = _vcwMasterDataBusiness.GetActiveVideo_SelectionList_Datas();
            return View(model);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.CoachTeachers, Anonymity = Anonymous.Verified)]
        public ActionResult UploadVideo(int AssignmentID)
        {
            ViewBag.Title = "Upload Video";
            AssignmentEntity assignmentEntity = _vcwBusiness.GetAssignment(AssignmentID);
            if (assignmentEntity != null)
            {
                TeacherVIPFileModel model = new TeacherVIPFileModel();
                model.AssignmentID = AssignmentID;
                model.DateRecorded = DateTime.Now;
                model.OwnerId = assignmentEntity.ReceiveUserId;
                ViewBag.Languages = _vcwMasterDataBusiness.GetActiveVideo_Language_Datas();
                return View(model);
            }
            else
                return View();
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.Coach, Anonymity = Anonymous.Verified)]
        public ActionResult LinkToAssignment(string fileId, int teacherId, string redirect = "")
        {
            ViewBag.TeacherId = teacherId;
            ViewBag.Redirect = redirect;
            ViewBag.FileId = fileId;
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
        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.Coach, Anonymity = Anonymous.Verified)]
        public string SearchAssignment(int community, int school, int teacher, int status,
            string sort = "ID", string order = "Asc", int first = 0, int count = 10)
        {
            var total = 0;
            var expression = GetExpression.GetAssignmentExpression(community, school, teacher, status,
                 AssignmentTypeEnum.TeacherVIP, UserInfo);

            var list = _vcwBusiness.GetTeacherAssignments(expression, sort, order, first, count, out total);

            if (list.Count > 0)
                list = _vcwBusiness.FormatTeacherAssignments(list);

            var result = new { total = total, data = list };
            return JsonHelper.SerializeObject(result);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.Coach, Anonymity = Anonymous.Verified)]
        public string EditFile(TeacherVIPFileModel model, int[] language, int[] screening, int[] selectionlist, string uploadfiles, bool isCoach = false)
        {
            var response = new PostFormResponse();
            Vcw_FileEntity entity = _vcwBusiness.GetFileEntity(model.ID);
            if (entity != null)
            {
                if (entity.FileSelections != null)
                {
                    _vcwBusiness.DeleteFileSelection(entity.FileSelections.ToList(), false);
                }

                if (isCoach)//Coach查看自己上传的文件时可编辑以下选项
                {
                    entity.IdentifyFileName = model.IdentifyFileName;
                    entity.DateRecorded = model.DateRecorded.Value;
                    entity.LanguageId = language == null ? 0 : language[0];
                    entity.Description = model.Description;
                    if (!string.IsNullOrEmpty(uploadfiles))
                    {
                        string[] files = uploadfiles.Split('|');
                        if (files.Length == 2)
                        {
                            entity.FileName = files[0];
                            entity.FilePath = files[1];
                        }
                    }
                }

                entity.UpdatedOn = DateTime.Now;
                entity.UpdatedBy = UserInfo.ID;
                entity.TBRSDate = model.TBRSDate.Value;

                if (screening != null)
                    //选中Accept，则将Status改为Completed，否则改为Rejected
                    entity.Status = screening[0] == 1 ? FileStatus.Completed : FileStatus.Rejected;
                else
                    entity.Status = FileStatus.Submitted;

                if (selectionlist != null)
                {
                    foreach (int item in selectionlist)
                    {
                        FileSelectionEntity FileSelection = new FileSelectionEntity();
                        FileSelection.SelectionId = item;
                        entity.FileSelections.Add(FileSelection);
                    }
                }
                OperationResult result = _vcwBusiness.UpdateFile(entity);
                response.Success = result.ResultType == OperationResultType.Success;
                response.Message = result.Message;
                if (response.Success)//更新Assignment的Status
                {
                    _vcwBusiness.ChangeStatus(ChangeStatusEnum.UpdateFile, entity.AssignmentId.Value, entity.Status);
                }
                return JsonHelper.SerializeObject(response);
            }
            else
            {
                response.Success = false;
                return JsonHelper.SerializeObject(response);
            }
        }


        /// <summary>
        /// 查找对应Assignment下的Files
        /// </summary>
        /// <param name="assignmentId"></param>
        /// <param name="sort"></param>
        /// <param name="order"></param>
        /// <param name="first"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.CoachTeachers, Anonymity = Anonymous.Verified)]
        public string SearchFiles(int assignmentId, string sort = "", string order = "Asc", int first = 0, int count = 10)
        {
            if (assignmentId > 0)
            {
                var total = 0;
                var expression = PredicateHelper.True<Vcw_FileEntity>();
                expression = expression.And(o => o.AssignmentId == assignmentId && o.IsDelete == false && o.VideoType == FileTypeEnum.TeacherVIP);
                var list = _vcwBusiness.GetSummaryList(expression, sort, order, first, count, out total);
                var result = new { total = total, data = list };
                return JsonHelper.SerializeObject(result);
            }
            else
            {
                var result = new { total = 0 };
                return JsonHelper.SerializeObject(result);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.CoachTeachers, Anonymity = Anonymous.Verified)]
        public string Delete(int[] video_select, int AssignmentId)
        {
            var response = new PostFormResponse();
            if (video_select != null)
            {
                List<int> deleteids = video_select.ToList();
                OperationResult result = _vcwBusiness.DeleteFile(deleteids);
                response.Success = result.ResultType == OperationResultType.Success;
                response.Message = result.Message;
                if (response.Success)//更新Assignment的Status
                {
                    _vcwBusiness.ChangeStatus(ChangeStatusEnum.DeleteFile, AssignmentId);
                }
                return JsonHelper.SerializeObject(response);
            }
            else
            {
                response.Success = false;
                return JsonHelper.SerializeObject(response);
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.CoachTeachers, Anonymity = Anonymous.Verified)]
        public string UploadVideo(TeacherVIPFileModel model, int[] language, string uploadfiles)
        {
            var response = new PostFormResponse();
            if (ModelState.IsValid)
            {
                model.IdentifyFileName = model.IdentifyFileName;
                if (!string.IsNullOrEmpty(uploadfiles))
                {
                    string[] files = uploadfiles.Split('|');
                    if (files.Length == 2)
                    {
                        model.FileName = files[0];
                        model.FilePath = files[1];
                    }
                }
                if (string.IsNullOrEmpty(model.FileName))
                {
                    response.Success = false;
                    response.Message = GetInformation("Vcw_File_Noupload");
                    return JsonHelper.SerializeObject(response);
                }
                model.LanguageId = language == null ? 0 : language[0];
                model.Status = FileStatus.Submitted;
                model.UploadUserType = UploadUserTypeEnum.Coach;
                OperationResult result = _vcwBusiness.InsertFileEntity(model, UserInfo);
                response.Success = result.ResultType == OperationResultType.Success;
                response.Message = result.Message;
                response.Data = new FileListModel()
                {
                    ID = model.ID,
                    IdentifyFileName = model.IdentifyFileName,
                    Status = model.Status,
                    UploadDate = DateTime.Now,
                    DateRecorded = model.DateRecorded == null ? CommonAgent.MinDate : model.DateRecorded.Value,
                    LanguageText = _vcwBusiness.GetLanguageText(model.LanguageId),
                    FilePath = model.FilePath,
                    FileName = model.FileName
                };

                if (response.Success)//更新Assignment的Status
                {
                    _vcwBusiness.ChangeStatus(ChangeStatusEnum.AddFile, model.AssignmentID);
                }
            }
            response.ModelState = ModelState;
            return JsonHelper.SerializeObject(response);
        }


        [HttpPost]
        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.CoachTeachers, Anonymity = Anonymous.Verified)]
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