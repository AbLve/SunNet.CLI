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
using Sunnet.Framework.Helpers;
using Sunnet.Cli.Core.Vcw.Entities;
using Sunnet.Cli.Core.Vcw.Enums;
using Sunnet.Cli.Business.Vcw.Models;
using System.IO;
using Sunnet.Framework.PDF;
using Sunnet.Framework.Core.Tool;
using Sunnet.Framework.Extensions;
using Sunnet.Cli.Business.Common;

namespace Sunnet.Cli.Vcw.Areas.Admin.Controllers
{
    public class TeacherAssignmentController : BaseController
    {
        VcwBusiness _vcwBusiness;
        UserBusiness _userBusiness;
        SchoolBusiness _schoolBusiness;
        VcwReport _vcwReport;
        VCW_MasterDataBusiness _vcwMasterDataBusiness;
        public TeacherAssignmentController()
        {
            _vcwBusiness = new VcwBusiness(VcwUnitWorkContext);
            _userBusiness = new UserBusiness();
            _schoolBusiness = new SchoolBusiness();
            _vcwReport = new VcwReport(VcwUnitWorkContext);
            _vcwMasterDataBusiness = new VCW_MasterDataBusiness(VcwUnitWorkContext);
        }

        //
        // GET: /Admin/TeacherAssignment/
        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.Admin, Anonymity = Anonymous.Verified)]
        public ActionResult Index()
        {
            ViewBag.Coaches = _userBusiness.GetCoach().ToSelectList().AddDefaultItem(ViewTextHelper.DefaultAllText, "-1").ToList();
            return View();
        }


        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.Admin, Anonymity = Anonymous.Verified)]
        public string SearchAssignment(int community = -1, int school = -1, int teacher = -1, int coach = -1, int status = -1,
            string sort = "ID", string order = "Asc", int first = 0, int count = 10)
        {
            var total = 0;
            var expression = GetExpression.GetPMTeacherAssignmentExpression(community, school,
               coach, teacher, status, AssignmentTypeEnum.TeacherAssignment, true, UserInfo.ID);

            var list = _vcwBusiness.GetTeacherAssignments(expression, sort, order, first, count, out total);

            if (list.Count > 0)
                list = _vcwBusiness.FormatTeacherAssignments(list);

            var result = new { total = total, data = list };
            return JsonHelper.SerializeObject(result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.Admin, Anonymity = Anonymous.Verified)]
        public string EditAssignment(AssignmentModel model, string uploadfiles)
        {
            AssignmentEntity Assignment = _vcwBusiness.GetAssignment(model.ID);
            Assignment.FeedbackText = model.FeedbackText;
            Assignment.UpdatedBy = UserInfo.ID;
            Assignment.UpdatedOn = DateTime.Now;
            if (!string.IsNullOrEmpty(uploadfiles))
            {
                string[] file = uploadfiles.Split('|');
                Assignment.FeedbackFileName = file[0];
                Assignment.FeedbackFilePath = file[1];
            }

            var response = new PostFormResponse();
            OperationResult result = _vcwBusiness.UpdateAssignment(Assignment);
            response.Success = result.ResultType == OperationResultType.Success;
            response.Message = result.Message;
            return JsonHelper.SerializeObject(response);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.Admin, Anonymity = Anonymous.Verified)]
        public void ExportExcel(int community = -1, int school = -1, int coach = -1, int teacher = -1, int status = -1)
        {
            var expression = GetExpression.GetPMTeacherAssignmentExpression(community, school,
               coach, teacher, status, AssignmentTypeEnum.TeacherAssignment, true, UserInfo.ID);

            var list = _vcwBusiness.GetTeacherAssignmentsList(expression);
            if (list != null && list.Count > 0)
                list = _vcwBusiness.FormatTeacherAssignments(list);

            string filepath = _vcwReport.TeacherAssignmentToExcel(list, true);

            FileInfo fileinfo = new FileInfo(filepath);

            FileStream fs = new FileStream(filepath, FileMode.Open);
            byte[] bytes = new byte[(int)fs.Length];
            fs.Read(bytes, 0, bytes.Length);
            fs.Close();
            Response.ContentType = "application/octet-stream";
            Response.AddHeader("Content-Disposition", "attachment;filename=TeacherAssignments" + filepath.Substring(filepath.LastIndexOf(".")));
            Response.BinaryWrite(bytes);
            Response.Flush();
            Response.End();
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.Admin, Anonymity = Anonymous.Verified)]
        public ActionResult ExportPDF(int community = -1, int school = -1, int coach = -1, int teacher = -1, int status = -1, bool isExport = true)
        {
            var total = 0;
            var expression = GetExpression.GetPMTeacherAssignmentExpression(community, school,
               coach, teacher, status, AssignmentTypeEnum.TeacherAssignment, true, UserInfo.ID);

            var list = _vcwBusiness.GetTeacherAssignments(expression, "ID", "DESC", 0, int.MaxValue, out total);

            if (list.Count > 0)
                list = _vcwBusiness.FormatTeacherAssignments(list);

            ViewBag.List = list;
            if (isExport)
            {
                GetPdf(GetViewHtml("ExportPDF"), "TeacherAssignments.pdf");
            }
            return View();
        }

        private void GetPdf(string html, string fileName)
        {
            PdfProvider pdfProvider = new PdfProvider();
            pdfProvider.IsPortrait = false;
            pdfProvider.GeneratePDF(html, fileName);
        }

        private string GetViewHtml(string viewName)
        {
            var resultHtml = string.Empty;
            ViewEngineResult result = ViewEngines.Engines.FindView(ControllerContext, viewName, null);
            if (null == result.View)
            {
                throw new InvalidOperationException();
            }
            try
            {
                ViewContext viewContext = new ViewContext(ControllerContext, result.View, this.ViewData, this.TempData, Response.Output);
                var textWriter = new StringWriter();
                result.View.Render(viewContext, textWriter);
                resultHtml = textWriter.ToString();
            }
            finally
            {
                result.ViewEngine.ReleaseView(ControllerContext, result.View);
            }
            return resultHtml;
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.Admin, Anonymity = Anonymous.Verified)]
        public ActionResult View(int id)
        {
            AssignmentModel Assignment = _vcwBusiness.GetTeacherAssignment(id);
            TeacherListModel teacher = _userBusiness.GetTeacherInfoByUserId(Assignment.ReceiveUserId);
            if (teacher != null)
            {
                ViewBag.Teacher = teacher;
            }
            return View(Assignment);
        }

        [HttpPost]
        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.Admin, Anonymity = Anonymous.Verified)]
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

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.Admin, Anonymity = Anonymous.Verified)]
        public ActionResult ViewAssignmentFiles(int id)
        {
            ViewBag.AssignmentId = id;
            int ReceiveUserId = _vcwBusiness.GetAssignmentReceive(id);
            TeacherListModel teacher = _userBusiness.GetTeacherInfoByUserId(ReceiveUserId);
            if (teacher != null)
            {
                ViewBag.Teacher = teacher;
            }
            return View();
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.Admin, Anonymity = Anonymous.Verified)]
        public string SearchFiles(int assignmentId, string sort = "ID", string order = "Asc", int first = 0, int count = 10)
        {
            if (assignmentId > 0)
            {
                var total = 0;
                var expression = PredicateHelper.True<Vcw_FileEntity>();
                expression = expression.And(o => o.AssignmentId == assignmentId && o.IsDelete == false && o.VideoType == FileTypeEnum.TeacherAssignment);
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
        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.Admin, Anonymity = Anonymous.Verified)]
        public string DeleteFile(int[] video_select, int AssignmentId)
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

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.Admin, Anonymity = Anonymous.Verified)]
        public ActionResult UploadVideo(int AssignmentID)
        {
            ViewBag.Title = "Upload File";
            int ReceiveUserId = _vcwBusiness.GetAssignmentReceive(AssignmentID);
            TeacherAssignmentFileModel model = new TeacherAssignmentFileModel();
            model.AssignmentID = AssignmentID;
            model.OwnerId = ReceiveUserId;
            ViewBag.Languages = _vcwMasterDataBusiness.GetActiveVideo_Language_Datas();
            ViewBag.Contexts = _vcwMasterDataBusiness.GetActiveContext_Datas();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.Admin, Anonymity = Anonymous.Verified)]
        public string UploadVideo(TeacherAssignmentFileModel model, string uploadfiles, int[] language, int[] Context)
        {
            var response = new PostFormResponse();
            if (ModelState.IsValid)
            {
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
                model.ContextId = Context == null ? 0 : Context[0];
                model.Status = FileStatus.Submitted;
                model.UploadUserType = UploadUserTypeEnum.Admin;
                OperationResult result = _vcwBusiness.InsertFileEntity(model, UserInfo);
                response.Success = result.ResultType == OperationResultType.Success;
                response.Message = result.Message;
                response.Data = new FileListModel()
                {
                    ID = model.ID,
                    Status = model.Status,
                    UploadDate = DateTime.Now,
                    DateRecorded = model.DateRecorded.Value,
                    LanguageText = _vcwBusiness.GetLanguageText(model.LanguageId),
                    FileName = model.FileName,
                    FilePath = model.FilePath,
                    IdentifyFileName = model.IdentifyFileName
                };
                if (response.Success)//更新Assignment的Status
                {
                    _vcwBusiness.ChangeStatus(ChangeStatusEnum.AddFile, model.AssignmentID);
                }
            }
            response.ModelState = ModelState;
            return JsonHelper.SerializeObject(response);
        }


        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.Admin, Anonymity = Anonymous.Verified)]
        public ActionResult ViewFile(int id, string redirect = "")
        {
            ViewBag.Redirect = redirect;
            TeacherAssignmentFileModel model = _vcwBusiness.GetTeacherAssignmentFileModelByCoach(id);
            TeacherListModel teacher = _userBusiness.GetTeacherInfoByUserId(model.OwnerId);
            if (teacher != null)
            {
                ViewBag.Teacher = teacher;
            }
            ViewBag.Languages = _vcwMasterDataBusiness.GetActiveVideo_Language_Datas();
            ViewBag.Contexts = _vcwMasterDataBusiness.GetActiveContext_Datas();
            ViewBag.SelectionList = _vcwMasterDataBusiness.GetActiveVideo_SelectionList_Datas();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.Admin, Anonymity = Anonymous.Verified)]
        public string EditFile(TeacherAssignmentFileModel model, string uploadfiles, int[] language, int[] screening, int[] selectionlist, int[] Context)
        {
            var response = new PostFormResponse();
            Vcw_FileEntity entity = _vcwBusiness.GetFileEntity(model.ID);
            if (entity != null)
            {
                OperationResult result = new OperationResult(OperationResultType.Success);

                entity.IdentifyFileName = model.IdentifyFileName;
                entity.DateRecorded = model.DateRecorded.Value;
                entity.LanguageId = language == null ? 0 : language[0];
                entity.ContextId = Context == null ? 0 : Context[0];
                entity.ContextOther = model.ContextOther;
                entity.Description = model.Description;
                entity.UpdatedOn = DateTime.Now;
                entity.UpdatedBy = UserInfo.ID;

                if (!string.IsNullOrEmpty(uploadfiles))
                {
                    string[] files = uploadfiles.Split('|');
                    if (files.Length == 2)
                    {
                        entity.FileName = files[0];
                        entity.FilePath = files[1];
                    }
                }
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
                result = _vcwBusiness.UpdateFile(entity);
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

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.Admin, Anonymity = Anonymous.Verified)]
        public ActionResult LinkToAssignment(string fileId, int teacherId, string redirect = "")
        {
            ViewBag.TeacherId = teacherId;
            ViewBag.Redirect = redirect;
            ViewBag.FileId = fileId;
            AssignmentListModel model = new AssignmentListModel();
            return View(model);
        }
    }
}