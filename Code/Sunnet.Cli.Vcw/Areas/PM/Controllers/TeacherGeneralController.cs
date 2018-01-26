using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
/**************************************************************************
 * Developer: 		Joe
 * Computer:		JOE-PC
 * Domain:			Joe-pc
 * CreatedOn:		2014/11/24 12:15:10
 * Description:		Please input class summary
 * Version History:	Created,2014/11/24 12:15:10
 * 
 * 
 **************************************************************************/
using Sunnet.Cli.UIBase.Filters;
using Sunnet.Cli.UIBase.Models;
using Sunnet.Framework.Permission;
using Sunnet.Cli.Vcw.Controllers;
using System.Web.Mvc;
using Sunnet.Cli.Vcw.Models;
using Sunnet.Cli.Business.Vcw;
using Sunnet.Cli.Business.Users;
using Sunnet.Cli.Business.Schools;
using Sunnet.Cli.Business.Vcw.Models;
using Sunnet.Framework.Helpers;
using Sunnet.Cli.Core.Vcw.Entities;
using Sunnet.Cli.Core.Vcw.Enums;
using Sunnet.Framework.Core.Tool;
using Sunnet.Framework.Extensions;
using Sunnet.Cli.Business.Common;
using System.Linq.Expressions;
using Sunnet.Cli.Business.Users.Models.VCW;
using Sunnet.Cli.Core.Users.Entities;

namespace Sunnet.Cli.Vcw.Areas.PM.Controllers
{
    public class TeacherGeneralController : BaseController
    {
        VcwBusiness _vcwBusiness;
        UserBusiness _userBusiness;
        SchoolBusiness _schoolBusiness;
        VCW_MasterDataBusiness _vcwMasterDataBusiness;
        public TeacherGeneralController()
        {
            _vcwBusiness = new VcwBusiness(VcwUnitWorkContext);
            _userBusiness = new UserBusiness();
            _schoolBusiness = new SchoolBusiness();
            _vcwMasterDataBusiness = new VCW_MasterDataBusiness(VcwUnitWorkContext);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.PMTeachers, Anonymity = Anonymous.Verified)]
        public ActionResult Index()
        {
            List<object> DropdownItems = GetDropDownItems.GetItemsByPM_Teacher(UserInfo.ID);
            if (DropdownItems.Count == 4)
            {
                ViewBag.Communities = DropdownItems[0];
                ViewBag.Coaches = DropdownItems[1];
                ViewBag.Schools = DropdownItems[2];
                ViewBag.Teachers = DropdownItems[3];
            }
            return View();
        }


        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.PMTeachers, Anonymity = Anonymous.Verified)]
        public string Search(int community, int school, int teacher, int coach, int status, string number,
            string sort = "ID", string order = "Asc", int first = 0, int count = 10)
        {
            var total = 0;
            var expression = GetExpression.GetTeacherGeneralExpression(community, school, coach, teacher, number, UserInfo);
            if (status > 0)
                expression = expression.And(o => o.Status == (FileStatus)status);

            List<FileListModel> list = _vcwBusiness.GetSummaryListByCoach(expression, sort, order, first, count, out total);

            if (list.Count > 0)
                list = _vcwBusiness.FormatTeacherGenerals(list);
            var result = new { total = total, data = list };
            return JsonHelper.SerializeObject(result);
        }


        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.PM, Anonymity = Anonymous.Verified)]
        public ActionResult View(int id, string redirect = "")
        {
            ViewBag.Redirect = redirect;
            TeacherGeneralFileModel model = _vcwBusiness.GetTeacherGeneralFileModel(id);
            ViewBag.IsPM = model.UploadUserId == UserInfo.ID;//用于判断是否为自身上传，是否可编辑基本信息
            TeacherListModel teacher = _userBusiness.GetTeacherInfoByUserId(model.OwnerId);
            if (teacher != null)
            {
                ViewBag.Teacher = teacher;
            }
            if (model.UploadUserId == UserInfo.ID)
            {
                ViewBag.Contents = _vcwMasterDataBusiness.GetActiveVideo_Content_Datas();
                ViewBag.Contexts = _vcwMasterDataBusiness.GetActiveContext_Datas();
            }
            return View(model);
        }


        [HttpPost]
        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.PMTeachers, Anonymity = Anonymous.Verified)]
        public string Delete(int[] video_select)
        {
            var response = new PostFormResponse();
            if (video_select != null)
            {
                List<int> deleteids = video_select.ToList();
                OperationResult result = _vcwBusiness.DeleteFile(deleteids);
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


        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.PMTeachers, Anonymity = Anonymous.Verified)]
        public ActionResult Upload()
        {
            List<SelectItemModel> teachers = _userBusiness.GetTeacherByPM(UserInfo.ID);
            if (teachers != null)
            {
                ViewBag.Teachers = teachers;
            }
            ViewBag.Contents = _vcwMasterDataBusiness.GetActiveVideo_Content_Datas();
            ViewBag.Contexts = _vcwMasterDataBusiness.GetActiveContext_Datas();
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.PMTeachers, Anonymity = Anonymous.Verified)]
        public string Upload(TeacherGeneralFileModel model, string uploadfiles, int[] teacher_select, int[] Content, int[] Context)
        {
            var response = new PostFormResponse();
            OperationResult result = new OperationResult(OperationResultType.Error);
            if (ModelState.IsValid)
            {
                if (teacher_select != null)
                {
                    string[] files = uploadfiles.Split('|');
                    if (files.Length == 2)
                    {
                        model.FileName = files[0];
                        model.FilePath = files[1];
                    }
                    if (teacher_select.Length > 0)
                    {
                        if (!string.IsNullOrEmpty(uploadfiles))
                        {
                            model.UploadUserType = UploadUserTypeEnum.PM;
                            result = _vcwBusiness.InsertFileEntity(model, Content, Context, teacher_select, UserInfo);
                        }
                    }
                }
                else
                {
                    response.Success = false;
                    return JsonHelper.SerializeObject(response);
                }
                response.Success = result.ResultType == OperationResultType.Success;
                response.Message = result.Message;
            }
            response.ModelState = ModelState;
            return JsonHelper.SerializeObject(response);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.PM, Anonymity = Anonymous.Verified)]
        public string Edit(TeacherGeneralFileModel model, string uploadfile_feedback, string uploadfile, int[] Content, int[] Context, bool isPM = false)
        {
            var response = new PostFormResponse();
            OperationResult result = new OperationResult(OperationResultType.Success);
            Vcw_FileEntity File = _vcwBusiness.GetFileEntity(model.ID);
            if (isPM)//Coach查看自己上传的文件时可编辑以下选项
            {
                if (File.FileContents != null)
                {
                    _vcwBusiness.DeleteFileContent(File.FileContents.ToList(), false);
                }
                File.IdentifyFileName = model.IdentifyFileName;
                File.DateRecorded = model.DateVieoRecorded.Value;
                File.ContextId = Context == null ? 0 : Context[0];
                File.ContextOther = model.ContextOther;
                File.ContentOther = model.ContentOther;
                File.Description = model.Description;
                if (!string.IsNullOrEmpty(uploadfile))
                {
                    string[] files = uploadfile.Split('|');
                    if (files.Length == 2)
                    {
                        File.FileName = files[0];
                        File.FilePath = files[1];
                    }
                }
                if (Content != null)
                {
                    List<FileContentEntity> fileContents = new List<FileContentEntity>();
                    foreach (int item_content in Content)
                    {
                        FileContentEntity FileContent = new FileContentEntity();
                        FileContent.ContentId = item_content;
                        fileContents.Add(FileContent);
                    }
                    File.FileContents = fileContents;
                }
                File.UpdatedBy = UserInfo.ID;
                File.UpdatedOn = DateTime.Now;

            }
            if (!string.IsNullOrEmpty(uploadfile_feedback))
            {
                string[] file = uploadfile_feedback.Split('|');
                File.FeedbackFileName = file[0];
                File.FeedbackFilePath = file[1];
            }
            File.FeedbackText = model.FeedbackText;
            result = _vcwBusiness.UpdateFile(File);
            response.Success = result.ResultType == OperationResultType.Success;
            response.Message = result.Message;
            return JsonHelper.SerializeObject(response);
        }


        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.PM, Anonymity = Anonymous.Verified)]
        public ActionResult LinkToAssignment(string fileId, int teacherId, string redirect = "")
        {
            ViewBag.TeacherId = teacherId;
            ViewBag.Redirect = redirect;
            ViewBag.FileId = fileId;
            AssignmentListModel model = new AssignmentListModel();
            return View(model);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.PM, Anonymity = Anonymous.Verified)]
        public ActionResult LinkToVIP(string fileId, int teacherId, string redirect = "")
        {
            ViewBag.TeacherId = teacherId;
            ViewBag.Redirect = redirect;
            ViewBag.FileId = fileId;
            AssignmentListModel model = new AssignmentListModel();
            return View(model);
        }
    }
}