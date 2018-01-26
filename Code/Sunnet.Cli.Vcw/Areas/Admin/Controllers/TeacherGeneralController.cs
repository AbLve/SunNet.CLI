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
using Sunnet.Cli.Core.Vcw.Entities;
using Sunnet.Cli.Core.Vcw.Enums;
using Sunnet.Framework.Core.Tool;
using Sunnet.Framework.Extensions;
using Sunnet.Cli.Business.Common;
using System.Collections;
using Sunnet.Cli.Business.Users.Models.VCW;
using System.Linq.Expressions;
using Sunnet.Cli.Core.Users.Entities;

namespace Sunnet.Cli.Vcw.Areas.Admin.Controllers
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

        //
        // GET: /Admin/TeacherGeneral/
        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.Admin, Anonymity = Anonymous.Verified)]
        public ActionResult Index()
        {
            ViewBag.Coaches = _userBusiness.GetCoach().ToSelectList().AddDefaultItem(ViewTextHelper.DefaultAllText, "-1").ToList();
            return View();
        }


        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.Admin, Anonymity = Anonymous.Verified)]
        public string Search(int community = -1, int school = -1, int teacher = -1, int coach = -1, int status = -1, string number = "",
            string sort = "ID", string order = "Asc", int first = 0, int count = 10)
        {
            List<int> teacherIds = _userBusiness.GetAllTeachers().Select(a => a.ID).ToList();
            var total = 0;
            List<FileListModel> list = new List<FileListModel>();
            var expression = PredicateHelper.True<Vcw_FileEntity>();
            if (community > 0)
            {
                List<int> comuser_Ids = new UserBusiness().GetAssignedUsersByCommunity(community).Select(a => a.UserId).ToList();
                expression = expression.And(r => comuser_Ids.Contains(r.OwnerId));
            }
            if (school > 0)
            {
                List<int> schooluser_Ids = new UserBusiness().GetTeacheridsBySchool(school);
                expression = expression.And(r => schooluser_Ids.Contains(r.OwnerId));
            }
            if (teacher > 0)
                expression = expression.And(o => o.OwnerId == teacher);
            if (coach > 0)
                expression = expression.And(o => o.UserInfo.TeacherInfo.CoachId == coach);
            if (status > 0)
                expression = expression.And(o => o.Status == (FileStatus)status);
            if (!string.IsNullOrEmpty(number))
                expression = expression.And(GetDropDownItems.GetNumberExpression(number));


            expression = expression.And(o => teacherIds.Contains(o.OwnerId)
                && o.VideoType == FileTypeEnum.TeacherGeneral && o.IsDelete == false);

            list = _vcwBusiness.GetSummaryListByCoach(expression, sort, order, first, count, out total);

            if (list.Count > 0)
                list = _vcwBusiness.FormatTeacherGenerals(list);

            var result = new { total = total, data = list };
            return JsonHelper.SerializeObject(result);
        }

        [HttpPost]
        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.Admin, Anonymity = Anonymous.Verified)]
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


        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.Admin, Anonymity = Anonymous.Verified)]
        public ActionResult View(int id, string redirect = "")
        {
            ViewBag.Redirect = redirect;
            TeacherGeneralFileModel model = _vcwBusiness.GetTeacherGeneralFileModel(id);
            TeacherListModel teacher = _userBusiness.GetTeacherInfoByUserId(model.OwnerId);
            if (teacher != null)
            {
                ViewBag.Teacher = teacher;
            }
            ViewBag.Contents = _vcwMasterDataBusiness.GetActiveVideo_Content_Datas();
            ViewBag.Contexts = _vcwMasterDataBusiness.GetActiveContext_Datas();
            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.Admin, Anonymity = Anonymous.Verified)]
        public string Edit(TeacherGeneralFileModel model, int[] Content, int[] Context, string uploadfile_feedback, string uploadfile, bool isPM = false)
        {
            var response = new PostFormResponse();
            OperationResult result = new OperationResult(OperationResultType.Success);
            Vcw_FileEntity File = _vcwBusiness.GetFileEntity(model.ID);
            if (File.FileContents != null)
            {
                _vcwBusiness.DeleteFileContent(File.FileContents.ToList(), false);
            }
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
            if (!string.IsNullOrEmpty(uploadfile_feedback))
            {
                string[] file = uploadfile_feedback.Split('|');
                File.FeedbackFileName = file[0];
                File.FeedbackFilePath = file[1];
            }
            if (Content != null)
            {
                List<FileContentEntity> fileContents = new List<FileContentEntity>();
                foreach (int item in Content)
                {
                    FileContentEntity FileContent = new FileContentEntity();
                    FileContent.ContentId = item;
                    fileContents.Add(FileContent);
                }
                File.FileContents = fileContents;
            }

            File.FeedbackText = model.FeedbackText;
            File.UpdatedBy = UserInfo.ID;
            File.UpdatedOn = DateTime.Now;
            result = _vcwBusiness.UpdateFile(File);
            response.Success = result.ResultType == OperationResultType.Success;

            response.Message = result.Message;
            return JsonHelper.SerializeObject(response);
        }


        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.Admin, Anonymity = Anonymous.Verified)]
        public ActionResult Upload()
        {
            ViewBag.Contents = _vcwMasterDataBusiness.GetActiveVideo_Content_Datas();
            ViewBag.Contexts = _vcwMasterDataBusiness.GetActiveContext_Datas();
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.Admin, Anonymity = Anonymous.Verified)]
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
                        if (!string.IsNullOrEmpty(uploadfiles) && model.FilePath != "undefined")
                        {
                            model.UploadUserType = UploadUserTypeEnum.Admin;
                            result = _vcwBusiness.InsertFileEntity(model, Content, Context, teacher_select, UserInfo);
                        }
                        else
                        {
                            result.ResultType = OperationResultType.Error;
                            result.Message = "File uploaded error.";
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


        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.Admin, Anonymity = Anonymous.Verified)]
        public ActionResult LinkToAssignment(string fileId, int teacherId, string redirect = "")
        {
            ViewBag.TeacherId = teacherId;
            ViewBag.Redirect = redirect;
            ViewBag.FileId = fileId;
            AssignmentListModel model = new AssignmentListModel();
            return View(model);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.Admin, Anonymity = Anonymous.Verified)]
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