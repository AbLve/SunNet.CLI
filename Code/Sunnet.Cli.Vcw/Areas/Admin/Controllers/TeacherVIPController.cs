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
using Sunnet.Framework.Extensions;
using Sunnet.Framework.Core.Tool;
using System.IO;
using Sunnet.Framework.PDF;
using Sunnet.Cli.Business.Common;
using System.Web.Script.Serialization;

namespace Sunnet.Cli.Vcw.Areas.Admin.Controllers
{
    public class TeacherVIPController : BaseController
    {
        VcwBusiness _vcwBusiness;
        UserBusiness _userBusiness;
        SchoolBusiness _schoolBusiness;
        VcwReport _vcwReport;
        VCW_MasterDataBusiness _vcwMasterDataBusiness;

        public TeacherVIPController()
        {
            _vcwBusiness = new VcwBusiness(VcwUnitWorkContext);
            _userBusiness = new UserBusiness();
            _schoolBusiness = new SchoolBusiness();
            _vcwReport = new VcwReport(VcwUnitWorkContext);
            _vcwMasterDataBusiness = new VCW_MasterDataBusiness(VcwUnitWorkContext);
        }

        //
        // GET: /Admin/TeacherVIP/
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
                coach, teacher, status, AssignmentTypeEnum.TeacherVIP, true, UserInfo.ID);

            var list = _vcwBusiness.GetTeacherAssignments(expression, sort, order, first, count, out total);

            if (list.Count > 0)
                list = _vcwBusiness.FormatTeacherAssignments(list);

            var result = new { total = total, data = list };
            return JsonHelper.SerializeObject(result);
        }


        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.Admin, Anonymity = Anonymous.Verified)]
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
            TeacherListModel teacher = _userBusiness.GetTeacherInfoByUserId(Assignment.ReceiveUserId);
            if (teacher != null)
            {
                ViewBag.Teacher = teacher;
            }
            return View();
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
        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.Admin, Anonymity = Anonymous.Verified)]
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
        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.Admin, Anonymity = Anonymous.Verified)]
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

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.Admin, Anonymity = Anonymous.Verified)]
        public ActionResult UploadVideo(int AssignmentID)
        {
            ViewBag.Title = "Upload Video";
            AssignmentEntity assignmentEntity = _vcwBusiness.GetAssignment(AssignmentID);
            if (assignmentEntity != null)
            {
                TeacherVIPFileModel model = new TeacherVIPFileModel();
                model.AssignmentID = AssignmentID;
                model.OwnerId = assignmentEntity.ReceiveUserId;
                ViewBag.Languages = _vcwMasterDataBusiness.GetActiveVideo_Language_Datas();
                return View(model);
            }
            else
                return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.Admin, Anonymity = Anonymous.Verified)]
        public string UploadVideo(TeacherVIPFileModel model, int[] language, string uploadfiles)
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
            TeacherVIPFileModel model = _vcwBusiness.GetTeacherVIPFileModelByCoach(id);
            TeacherListModel teacher = _userBusiness.GetTeacherInfoByUserId(model.OwnerId);
            if (teacher != null)
            {
                ViewBag.Teacher = teacher;
            }
            ViewBag.Languages = _vcwMasterDataBusiness.GetActiveVideo_Language_Datas();
            ViewBag.SelectionList = _vcwMasterDataBusiness.GetActiveVideo_SelectionList_Datas();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.Admin, Anonymity = Anonymous.Verified)]
        public string EditFile(TeacherVIPFileModel model, int[] screening, int[] language, int[] selectionlist, string uploadfiles, bool isPM = false)
        {
            var response = new PostFormResponse();
            Vcw_FileEntity entity = _vcwBusiness.GetFileEntity(model.ID);
            if (entity != null)
            {
                if (entity.FileSelections != null)
                {
                    _vcwBusiness.DeleteFileSelection(entity.FileSelections.ToList(), false);
                }
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

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.Admin, Anonymity = Anonymous.Verified)]
        public void ExportExcel(int community = -1, int school = -1, int coach = -1, int teacher = -1, int status = -1)
        {
            var total = 0;
            var expression = GetExpression.GetPMTeacherAssignmentExpression(community, school,
                coach, teacher, status, AssignmentTypeEnum.TeacherVIP, true, UserInfo.ID);

            var list = _vcwBusiness.GetTeacherAssignments(expression, "ID", "DESC", 0, int.MaxValue, out total);

            if (list.Count > 0)
                list = _vcwBusiness.FormatTeacherAssignments(list);

            string filepath = _vcwReport.TeacherVIPAssignmentToExcel(list);

            FileInfo fileinfo = new FileInfo(filepath);

            FileStream fs = new FileStream(filepath, FileMode.Open);
            byte[] bytes = new byte[(int)fs.Length];
            fs.Read(bytes, 0, bytes.Length);
            fs.Close();
            Response.ContentType = "application/octet-stream";
            Response.AddHeader("Content-Disposition", "attachment;filename=TeacherVIP" + filepath.Substring(filepath.LastIndexOf(".")));
            Response.BinaryWrite(bytes);
            Response.Flush();
            Response.End();
        }


        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.Admin, Anonymity = Anonymous.Verified)]
        public ActionResult ExportPDF(int community = -1, int school = -1, int coach = -1, int teacher = -1, int status = -1, bool isExport = true)
        {
            var total = 0;
            var expression = GetExpression.GetPMTeacherAssignmentExpression(community, school,
                coach, teacher, status, AssignmentTypeEnum.TeacherVIP, true, UserInfo.ID);

            var list = _vcwBusiness.GetTeacherAssignments(expression, "ID", "DESC", 0, int.MaxValue, out total);

            if (list.Count > 0)
                list = _vcwBusiness.FormatTeacherAssignments(list);

            ViewBag.List = list;
            if (isExport)
            {
                GetPdf(GetViewHtml("ExportPDF"), "TeacherVIPAssignments.pdf");
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

        public ActionResult Export1(bool isExport = false)
        {
            if (isExport)
            {
                GetPdf(GetViewHtml("Export1"), "Style.pdf");
            }
            return View();
        }

        public ActionResult Export(bool isExport = false)
        {
            if (isExport)
            {
                GetPdf(GetViewHtml("Export"), "Style.pdf");
            }
            return View();
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.Admin, Anonymity = Anonymous.Verified)]
        public ActionResult VIPDocuments()
        {
            ViewBag.VIPDocuments = _vcwBusiness.GetVIPDocuments();
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.Admin, Anonymity = Anonymous.Verified)]
        public string UplaodVipDocument(string uploadfiles, string checkuploadedfiles)
        {
            var response = new PostFormResponse();
            List<VIPDocumentEntity> list_vipDocuments = new List<VIPDocumentEntity>();
            if (!string.IsNullOrEmpty(uploadfiles))
            {
                if (uploadfiles.EndsWith(",]"))
                {
                    //uploadfiles格式：[{'FileName1':'FileName1','FilePath1':'FilePath1'},{'FileName2':'FileName2','FilePath2':'FilePath2'},]
                    uploadfiles = uploadfiles.Replace(",]", "]");
                    //将传入的字符串解析成List<AssignmentFileEntity>
                    JavaScriptSerializer Serializer = new JavaScriptSerializer();
                    list_vipDocuments =
                        Serializer.Deserialize<List<VIPDocumentEntity>>(uploadfiles);
                }
            }

            List<VIPDocumentEntity> vipdocuments = _vcwBusiness.GetVIPDocuments();

            //删除VIPDocuments
            if (!string.IsNullOrEmpty(checkuploadedfiles))   //之前上传过的文件是否全部删除
            {
                string[] filestr = checkuploadedfiles.Split('|');
                if (vipdocuments.Count > 0)
                {
                    foreach (string item in filestr)
                    {
                        VIPDocumentEntity file = vipdocuments.Find(a => a.FilePath == item);
                        if (file != null)//在已上传的文件中查找是否包含该文件，若包含，则不删除
                        {
                            vipdocuments.Remove(file);
                        }
                    }
                    _vcwBusiness.DeleteVIPDocuments(vipdocuments);
                }

            }
            else//如果之前上传过的文件已全部删除，则将其对应的文件子表全部删除
            {
                _vcwBusiness.DeleteVIPDocuments(vipdocuments);
            }

            OperationResult result = _vcwBusiness.AddVIPDocuments(list_vipDocuments);
            response.Success = result.ResultType == OperationResultType.Success;
            response.Message = result.Message;
            return JsonHelper.SerializeObject(response);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.Admin, Anonymity = Anonymous.Verified)]
        public ActionResult LinkToAssignment(string fileId, int teacherId, string redirect = "")
        {
            ViewBag.TeacherId = teacherId;
            ViewBag.Redirect = redirect;
            ViewBag.FileId = fileId;
            return View();
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
    }
}