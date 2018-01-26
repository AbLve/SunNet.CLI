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
using Sunnet.Framework.Extensions;
using Sunnet.Cli.Business.Users;
using Sunnet.Cli.Business.Vcw;
using Sunnet.Cli.Vcw.Models;
using Sunnet.Cli.Core.Vcw.Enums;
using Sunnet.Framework.Helpers;
using Sunnet.Cli.Business.Vcw.Models;
using Sunnet.Cli.Core.Vcw.Entities;
using System.Linq.Expressions;
using Sunnet.Cli.Business.Schools;
using System.IO;
using Sunnet.Framework.PDF;

namespace Sunnet.Cli.Vcw.Areas.PM.Controllers
{
    public class SummaryController : BaseController
    {
        VcwBusiness _vcwBusiness;
        UserBusiness _userBusiness;
        SchoolBusiness _schoolBusiness;
        VCW_MasterDataBusiness _vcwMasterDataBusiness;
        VcwReport _vcwReport;
        public SummaryController()
        {
            _vcwBusiness = new VcwBusiness(VcwUnitWorkContext);
            _userBusiness = new UserBusiness();
            _schoolBusiness = new SchoolBusiness();
            _vcwReport = new VcwReport(VcwUnitWorkContext);
            _vcwMasterDataBusiness = new VCW_MasterDataBusiness(VcwUnitWorkContext);
        }

        // GET: /PM/Summary/
        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.PMSummary, Anonymity = Anonymous.Verified)]
        public ActionResult Index()
        {
            List<object> Dropdown = GetDropDownItems.GetItemsByPM(UserInfo.ID);
            ViewBag.Coaches = Dropdown[0];
            ViewBag.Communities = Dropdown[1];

            List<SelectListItem> Uploadby = new List<SelectListItem>();
            Uploadby.Add(new SelectListItem { Text = ViewTextHelper.DefaultAllText, Value = "-1" });
            Uploadby.Add(new SelectListItem { Text = UploadUserTypeEnum.Coach.ToDescription(), Value = UploadUserTypeEnum.Coach.GetValue().ToString() });
            Uploadby.Add(new SelectListItem { Text = UploadUserTypeEnum.PM.ToDescription(), Value = UploadUserTypeEnum.PM.GetValue().ToString() });
            Uploadby.Add(new SelectListItem { Text = UploadUserTypeEnum.Admin.ToDescription(), Value = UploadUserTypeEnum.Admin.GetValue().ToString() });
            ViewBag.UploadedBy = Uploadby;

            List<SelectListItem> VideoType = new List<SelectListItem>();
            VideoType.Add(new SelectListItem { Text = ViewTextHelper.DefaultAllText, Value = "-1" });
            VideoType.Add(new SelectListItem { Text = FileTypeEnum.CoachGeneral.ToDescription(), Value = FileTypeEnum.CoachGeneral.GetValue().ToString() });
            VideoType.Add(new SelectListItem { Text = FileTypeEnum.CoachAssignment.ToDescription(), Value = FileTypeEnum.CoachAssignment.GetValue().ToString() });
            ViewBag.VideoTypeOptions = VideoType;

            return View();
        }


        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.PMSummary, Anonymity = Anonymous.Verified)]
        public string Search(int community, int coach, int uploadby, int videotype, string number,
            string sort = "ID", string order = "Desc", int first = 0, int count = 10)
        {
            int total = 0;

            Expression<Func<Vcw_FileEntity, bool>> fileContition = GetExpression.GetPMCoachSummaryExpression(community,
                coach, uploadby, videotype, number, false, UserInfo.ID);

            List<FileListModel> list = _vcwBusiness.GetSummaryCoachFileList(fileContition, sort, order, first, count, out total);

            if (list.Count > 0)
                list = _vcwBusiness.FormatCoachSummary(list);

            var result = new { total = total, data = list };
            return JsonHelper.SerializeObject(result);
        }


        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.PMSummary, Anonymity = Anonymous.Verified)]
        public ActionResult TeacherFiles()
        {
            List<object> DropdownItems = GetDropDownItems.GetItemsByPM_Teacher(UserInfo.ID);
            if (DropdownItems.Count == 4)
            {
                ViewBag.Communities = DropdownItems[0];
                ViewBag.Schools = DropdownItems[2];
                ViewBag.Teachers = DropdownItems[3];
            }

            List<SelectListItem> Uploadby = new List<SelectListItem>();
            Uploadby.Add(new SelectListItem { Text = ViewTextHelper.DefaultAllText, Value = "-1" });
            Uploadby.Add(new SelectListItem { Text = UploadUserTypeEnum.Teacher.ToDescription(), Value = UploadUserTypeEnum.Teacher.GetValue().ToString() });
            Uploadby.Add(new SelectListItem { Text = UploadUserTypeEnum.Coach.ToDescription(), Value = UploadUserTypeEnum.Coach.GetValue().ToString() });
            Uploadby.Add(new SelectListItem { Text = UploadUserTypeEnum.PM.ToDescription(), Value = UploadUserTypeEnum.PM.GetValue().ToString() });
            Uploadby.Add(new SelectListItem { Text = UploadUserTypeEnum.Admin.ToDescription(), Value = UploadUserTypeEnum.Admin.GetValue().ToString() });
            ViewBag.UploadedBy = Uploadby;

            List<SelectListItem> VideoType = new List<SelectListItem>();
            VideoType.Add(new SelectListItem { Text = ViewTextHelper.DefaultAllText, Value = "-1" });
            VideoType.Add(new SelectListItem { Text = FileTypeEnum.TeacherVIP.ToDescription(), Value = FileTypeEnum.TeacherVIP.GetValue().ToString() });
            VideoType.Add(new SelectListItem { Text = FileTypeEnum.TeacherGeneral.ToDescription(), Value = FileTypeEnum.TeacherGeneral.GetValue().ToString() });
            VideoType.Add(new SelectListItem { Text = FileTypeEnum.TeacherAssignment.ToDescription(), Value = FileTypeEnum.TeacherAssignment.GetValue().ToString() });
            ViewBag.VideoTypeOptions = VideoType;

            return View();
        }


        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.PMSummary, Anonymity = Anonymous.Verified)]
        public string SearchTeacherFile(int community, int school, int teacher, int uploadby, int videotype, string number,
            string sort = "ID", string order = "Desc", int first = 0, int count = 10)
        {
            var total = 0;
            var expression = GetExpression.GetPMTeacherSummaryExpression(community, school,
                teacher, uploadby, videotype, number, UserInfo.ID);

            List<FileListModel> list = _vcwBusiness.GetSummaryListByCoach(expression, sort, order, first, count, out total);

            if (list.Count > 0)
                list = _vcwBusiness.FormatTeacherGenerals(list);

            var result = new { total = total, data = list };
            return JsonHelper.SerializeObject(result);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.PMSummary, Anonymity = Anonymous.Verified)]
        public void ExportExcel_CoachFiles(int community = -1, int coach = -1, int uploadby = -1, int videotype = -1, string number = "")
        {
            int total = 0;
            Expression<Func<Vcw_FileEntity, bool>> fileContition = GetExpression.GetPMCoachSummaryExpression(community,
                coach, uploadby, videotype, number, false, UserInfo.ID);
            List<FileListModel> list = _vcwBusiness.GetSummaryCoachFileList(fileContition, "ID", "DESC", 0, int.MaxValue, out total);

            if (list.Count > 0)
                list = _vcwBusiness.FormatCoachSummary(list);

            string filepath = _vcwReport.PM_CoachFilesToExcel(list);

            FileInfo fileinfo = new FileInfo(filepath);

            FileStream fs = new FileStream(filepath, FileMode.Open);
            byte[] bytes = new byte[(int)fs.Length];
            fs.Read(bytes, 0, bytes.Length);
            fs.Close();
            Response.ContentType = "application/octet-stream";
            Response.AddHeader("Content-Disposition", "attachment;filename=PMSummary_CoachFiles" + filepath.Substring(filepath.LastIndexOf(".")));
            Response.BinaryWrite(bytes);
            Response.Flush();
            Response.End();
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.PMSummary, Anonymity = Anonymous.Verified)]
        public ActionResult ExportPdf_CoachFiles(int community = -1, int coach = -1, int uploadby = -1,
            int videotype = -1, string number = "", bool isExport = true)
        {
            int total = 0;
            Expression<Func<Vcw_FileEntity, bool>> fileContition = GetExpression.GetPMCoachSummaryExpression(community,
                coach, uploadby, videotype, number, false, UserInfo.ID);
            List<FileListModel> list = _vcwBusiness.GetSummaryCoachFileList(fileContition, "ID", "DESC", 0, int.MaxValue, out total);

            if (list.Count > 0)
                list = _vcwBusiness.FormatCoachSummary(list);
            ViewBag.List = list;
            if (isExport)
            {
                GetPdf(GetViewHtml("ExportPdf_CoachFiles"), "PMSummary_CoachFiles.pdf");
            }
            return View();
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.PMSummary, Anonymity = Anonymous.Verified)]
        public void ExportExcel_TeacherFiles(int community = -1, int school = -1, int teacher = -1
            , int uploadby = -1, int videotype = -1, string number = "")
        {
            var total = 0;
            var expression = GetExpression.GetPMTeacherSummaryExpression(community, school,
                teacher, uploadby, videotype, number, UserInfo.ID);

            List<FileListModel> list = _vcwBusiness.GetSummaryListByCoach(expression, "ID", "DESC", 0, int.MaxValue, out total);

            if (list.Count > 0)
                list = _vcwBusiness.FormatTeacherGenerals(list);

            string filepath = _vcwReport.PM_TeacherFilesToExcel(list);

            FileInfo fileinfo = new FileInfo(filepath);

            FileStream fs = new FileStream(filepath, FileMode.Open);
            byte[] bytes = new byte[(int)fs.Length];
            fs.Read(bytes, 0, bytes.Length);
            fs.Close();
            Response.ContentType = "application/octet-stream";
            Response.AddHeader("Content-Disposition", "attachment;filename=PMSummary_TeacherFiles" + filepath.Substring(filepath.LastIndexOf(".")));
            Response.BinaryWrite(bytes);
            Response.Flush();
            Response.End();
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.PMSummary, Anonymity = Anonymous.Verified)]
        public ActionResult ExportPdf_TeacherFiles(int community = -1, int school = -1, int teacher = -1
            , int uploadby = -1, int videotype = -1, string number = "", bool isExport = true)
        {
            var total = 0;
            var expression = GetExpression.GetPMTeacherSummaryExpression(community, school,
                teacher, uploadby, videotype, number, UserInfo.ID);
            List<FileListModel> list = _vcwBusiness.GetSummaryListByCoach(expression, "ID", "DESC", 0, int.MaxValue, out total);

            if (list.Count > 0)
                list = _vcwBusiness.FormatTeacherGenerals(list);
            ViewBag.List = list;
            if (isExport)
            {
                GetPdf(GetViewHtml("ExportPdf_TeacherFiles"), "PMSummary_TeacherFiles.pdf");
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

    }
}