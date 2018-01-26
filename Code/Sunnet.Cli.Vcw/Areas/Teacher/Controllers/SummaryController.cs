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
using Sunnet.Cli.Core.Users.Enums;
using Sunnet.Cli.Vcw.Controllers;
using Sunnet.Cli.Business.Users;
using Sunnet.Cli.Core.Users.Entities;
using Sunnet.Cli.Core.Vcw.Enums;
using Sunnet.Framework.Extensions;
using Sunnet.Framework.Helpers;
using Sunnet.Cli.UIBase.Models;
using Sunnet.Cli.Business.Users.Models;
using Sunnet.Cli.Business.Vcw.Models;
using Sunnet.Cli.Core.Vcw.Entities;
using Sunnet.Cli.Business.Vcw;
using System.Linq.Expressions;
using System.Text;
using System.Globalization;
using System.IO;
using Sunnet.Framework.PDF;
using Sunnet.Cli.UIBase.Filters;
using Sunnet.Framework.Permission;
using System.Data.Entity.Core.Objects.SqlClient;
using Sunnet.Cli.Vcw.Models;
using Sunnet.Cli.Business.Common;
using Sunnet.Cli.Business.Cot;
using Sunnet.Cli.Business.Cot.Models;
using Sunnet.Cli.Business.Cec;
using Sunnet.Cli.Core.Cpalls;
using Sunnet.Cli.Business.Ade;
using Sunnet.Cli.Core.Cec.Entities;
using Sunnet.Cli.Business.Cec.Model;
using System.Web.Caching;

namespace Sunnet.Cli.Vcw.Areas.Teacher.Controllers
{
    public class SummaryController : BaseController
    {
        UserBusiness _userBusiness;
        VcwBusiness _vcwBusiness;
        VcwReport _vcwReport;
        CotBusiness _cotBusiness;
        CecBusiness _cecBusiness;
        AdeBusiness _adeBusiness;
        int year = CommonAgent.Year;
        public SummaryController()
        {
            _userBusiness = new UserBusiness();
            _vcwBusiness = new VcwBusiness(VcwUnitWorkContext);
            _vcwReport = new VcwReport(VcwUnitWorkContext);
            _cotBusiness = new CotBusiness();
            _cecBusiness = new CecBusiness();
            _adeBusiness = new AdeBusiness(AdeUnitWorkContext);
        }

        //Teahcer UserId 342
        //Coach   UserId 397
        //PM      UserId 516
        // GET: /Teacher/Summary/
        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.TeacherSummary, Anonymity = Anonymous.Verified)]
        public ActionResult Index()
        {
            if (UserInfo.Role == Role.Teacher)
            {
                List<SelectListItem> UploadbyList = new List<SelectListItem>();

                UploadbyList.Add(new SelectListItem() { Text = "All", Value = "-1" });
                UploadbyList.Add(new SelectListItem() { Text = UploadUserTypeEnum.Teacher.ToDescription(), Value = UploadUserTypeEnum.Teacher.GetValue().ToString() });
                UploadbyList.Add(new SelectListItem() { Text = UploadUserTypeEnum.Coach.ToDescription(), Value = UploadUserTypeEnum.Coach.GetValue().ToString() });
                ViewBag.UploadbyOptions = UploadbyList;


                List<SelectListItem> VideoTypeOptions = new List<SelectListItem>();
                VideoTypeOptions.Add(new SelectListItem { Text = ViewTextHelper.DefaultAllText, Value = "-1" });
                VideoTypeOptions.Add(new SelectListItem { Text = FileTypeEnum.TeacherVIP.ToDescription(), Value = FileTypeEnum.TeacherVIP.GetValue().ToString() });
                VideoTypeOptions.Add(new SelectListItem { Text = FileTypeEnum.TeacherGeneral.ToDescription(), Value = FileTypeEnum.TeacherGeneral.GetValue().ToString() });
                VideoTypeOptions.Add(new SelectListItem { Text = FileTypeEnum.TeacherAssignment.ToDescription(), Value = FileTypeEnum.TeacherAssignment.GetValue().ToString() });
                ViewBag.VideoTypeOptions = VideoTypeOptions;
            }
            ViewBag.UserId = UserInfo.ID;

            return View();
        }


        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.TeacherSummary, Anonymity = Anonymous.Verified)]
        public string Search(int uploadby = 0, int videotype = 0, string number = "",
            string sort = "ID", string order = "Desc", int first = 0, int count = 10)
        {
            int total = 0;

            List<FileListModel> list = new List<FileListModel>();

            Expression<Func<Vcw_FileEntity, bool>> fileContition = PredicateHelper.True<Vcw_FileEntity>();
            fileContition = fileContition.And(r => r.OwnerId == UserInfo.ID && r.IsDelete == false);
            if (uploadby == UploadUserTypeEnum.Teacher.GetValue()) //Teacher上传
                fileContition = fileContition.And(r => r.UploadUserType == UploadUserTypeEnum.Teacher);
            if (uploadby == UploadUserTypeEnum.Coach.GetValue()) //Coach上传
                fileContition = fileContition.And(r => (r.UploadUserType == UploadUserTypeEnum.Coach
                    || r.UploadUserType == UploadUserTypeEnum.PM || r.UploadUserType == UploadUserTypeEnum.Admin));
            if (videotype > 0)
                fileContition = fileContition.And(r => r.VideoType == (FileTypeEnum)videotype);

            if (!string.IsNullOrEmpty(number))
                fileContition = fileContition.And(GetDropDownItems.GetNumberExpression(number));


            list = _vcwBusiness.GetSummaryListWithDueDate(fileContition, sort, order, first, count, out total);

            var result = new { total = total, data = list };
            return JsonHelper.SerializeObject(result);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.TeacherSummary, Anonymity = Anonymous.Verified)]
        public ActionResult Feedback(int id)
        {
            ViewBag.Title = "Coach Feedback";
            TeacherGeneralFileModel model = _vcwBusiness.GetTeacherGeneralFileModel(id);
            return View(model);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.TeacherSummary, Anonymity = Anonymous.Verified)]
        public void ExportExcel(int uploadby = 0, int videotype = 0, string number = "")
        {
            Expression<Func<Vcw_FileEntity, bool>> fileContition = PredicateHelper.True<Vcw_FileEntity>();
            fileContition = fileContition.And(r => r.OwnerId == UserInfo.ID && r.IsDelete == false);
            if (uploadby == UploadUserTypeEnum.Teacher.GetValue()) //Teacher上传
                fileContition = fileContition.And(r => r.UploadUserType == UploadUserTypeEnum.Teacher);
            if (uploadby == UploadUserTypeEnum.Coach.GetValue()) //Coach上传
                fileContition = fileContition.And(r => (r.UploadUserType == UploadUserTypeEnum.Coach
                    || r.UploadUserType == UploadUserTypeEnum.PM || r.UploadUserType == UploadUserTypeEnum.Admin));
            if (videotype > 0)
                fileContition = fileContition.And(r => r.VideoType == (FileTypeEnum)videotype);
            if (!string.IsNullOrEmpty(number))
                fileContition = fileContition.And(GetDropDownItems.GetNumberExpression(number));
            string filepath = _vcwReport.TeacherSummaryToExcel(fileContition);

            FileInfo fileinfo = new FileInfo(filepath);

            FileStream fs = new FileStream(filepath, FileMode.Open);
            byte[] bytes = new byte[(int)fs.Length];
            fs.Read(bytes, 0, bytes.Length);
            fs.Close();
            Response.ContentType = "application/octet-stream";
            Response.AddHeader("Content-Disposition", "attachment;filename=TeacherFiles" + filepath.Substring(filepath.LastIndexOf(".")));
            Response.BinaryWrite(bytes);
            Response.Flush();
            Response.End();
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.TeacherSummary, Anonymity = Anonymous.Verified)]
        public ActionResult ExportPDF(int uploadby = 0, int videotype = 0, string number = "", bool export = true)
        {
            List<FileListModel> list = new List<FileListModel>();

            Expression<Func<Vcw_FileEntity, bool>> fileContition = PredicateHelper.True<Vcw_FileEntity>();
            fileContition = fileContition.And(r => r.OwnerId == UserInfo.ID && r.IsDelete == false);
            if (uploadby == UploadUserTypeEnum.Teacher.GetValue()) //Teacher上传
                fileContition = fileContition.And(r => r.UploadUserType == UploadUserTypeEnum.Teacher);
            if (uploadby == UploadUserTypeEnum.Coach.GetValue()) //Coach上传
                fileContition = fileContition.And(r => (r.UploadUserType == UploadUserTypeEnum.Coach
                    || r.UploadUserType == UploadUserTypeEnum.PM || r.UploadUserType == UploadUserTypeEnum.Admin));
            if (videotype > 0)
                fileContition = fileContition.And(r => r.VideoType == (FileTypeEnum)videotype);
            if (!string.IsNullOrEmpty(number))
                fileContition = fileContition.And(GetDropDownItems.GetNumberExpression(number));
            int total;
            list = _vcwBusiness.GetSummaryList(fileContition, "ID", "DESC", 0, int.MaxValue, out total);
            ViewBag.List = list;
            if (export)
            {
                GetPdf(GetViewHtml("ExportPDF"), "TeacherFiles.pdf");
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
                throw new InvalidOperationException(FormatErrorMessage(viewName, result.SearchedLocations));
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

        private string FormatErrorMessage(string viewName, IEnumerable<string> searchedLocations)
        {
            string format = "The view '{0}' or its master was not found or no view engine supports the searched locations. The following locations were searched:{1}";
            StringBuilder builder = new StringBuilder();
            foreach (string str in searchedLocations)
            {
                builder.AppendLine();
                builder.Append(str);
            }
            return string.Format(CultureInfo.CurrentCulture, format, viewName, builder);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.STGReport, Anonymity = Anonymous.Verified)]
        public ActionResult STG()
        {
            ViewBag.Community = string.Join(", ", UserInfo.UserCommunitySchools
                .Where(r => r.CommunityId > 0)
                .DistinctBy(r => r.CommunityId).Select(x => x.Community.Name));
            ViewBag.School = string.Join(", ", UserInfo.UserCommunitySchools
                .Where(r => r.SchoolId > 0)
                .DistinctBy(r => r.SchoolId).Select(x => x.School.Name));

            if (UserInfo.TeacherInfo.CoachId > 0)
            {
                UserBaseModel userModel = _userBusiness.GetUserBaseModel(UserInfo.TeacherInfo.CoachId);
                ViewBag.Coach = userModel == null ? "" : userModel.FirstName + " " + UserInfo.LastName;
            }
            else
                ViewBag.Coach = "";

            ViewBag.SchoolYear = CommonAgent.SchoolYear;
            ViewBag.Teacher = string.Format("{0} {1}", UserInfo.FirstName, UserInfo.LastName);

            ViewBag.SchoolYear = year.ToSchoolYearString();

            ViewBag.Reports = _cotBusiness.GetReports(year, UserInfo.TeacherInfo.ID);
            ViewBag.CotReports = _cotBusiness.GetAssessments(year, UserInfo.TeacherInfo.ID);
            ViewBag.CecReports = _cecBusiness.GetCecReports(year, UserInfo.TeacherInfo.ID);

            return View();
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.STGReport, Anonymity = Anonymous.Verified)]
        public ActionResult CotPdf(int assessmentId, int teacherId, bool export = true)
        {

            var baseAssessment = _adeBusiness.GetBaseAssessmentModel(assessmentId);
            if (baseAssessment == null) return new EmptyResult();
            ViewBag.AssessmentName = baseAssessment.Name;

            var status = _cotBusiness.GetTeacherStatus(assessmentId, year, teacherId);
            if (!status.HasCotReport)
            {
                return RedirectToAction("Index", "Dashboard", new { Area = "" });
            }
            TeacherEntity teacher = _userBusiness.GetTeacher(teacherId, UserInfo);
            ViewBag.teacher = string.Format("{0} {1}", teacher.UserInfo.FirstName, teacher.UserInfo.LastName);
            ViewBag.schoolYear = year.ToSchoolYearString();
            var assessment = _cotBusiness.GetAssessment(assessmentId, year, teacherId);
            ViewBag.model = assessment;
            if (export)
            {
                GetPdf(GetViewHtml("CotPdf"), "COT");
            }
            return View();
        }

        string CacheKey(int teacherId, int assessmentId, Wave wave, int year)
        {
            return string.Format("CEC_{0}_{1}_{2}_{3}", teacherId, assessmentId, (int)wave, year);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.STGReport, Anonymity = Anonymous.Verified)]
        public ActionResult CECReport(int teacherId, int assessmentId, Wave wave, bool export = true)
        {
            if (!_adeBusiness.CanExecuteAssessment(assessmentId))
                return RedirectToAction("Index", "Dashboard", new { Area = "" });

            var baseAssessment = _adeBusiness.GetBaseAssessmentModel(assessmentId);
            if (baseAssessment == null) return new EmptyResult();
            ViewBag.AssessmentName = baseAssessment.Name;

            string schoolYear = year.ToSchoolYearString();
            CecHistoryEntity history = _cecBusiness.GetCecHistoryEntity(r => r.AssessmentId == assessmentId &&
                r.TeacherId == teacherId && r.SchoolYear == schoolYear && r.Wave == wave);

            ViewBag.NoData = false;
            if (history == null)
            {
                ViewBag.NoData = true;
                return View();
            }

            string[] waves = { "", "BOY", "MOY", "EOY" };
            ViewBag.Wave = waves[(int)wave];
            TeacherEntity teacherEntity = _userBusiness.GetTeacher(teacherId, null);
            ViewBag.Teacher = string.Format("{0} {1}", teacherEntity.UserInfo.FirstName, teacherEntity.UserInfo.LastName);
            ViewBag.SchoolYear = year.ToFullSchoolYearString();

            ViewBag.CommunityName = string.Join(", ", UserInfo.UserCommunitySchools.Select(x => x.Community.Name));
            ViewBag.SchoolName = string.Join(", ", UserInfo.UserCommunitySchools.Select(x => x.School.Name));

            if (teacherEntity.CoachId > 0)
            {
                UserBaseModel userModel = _userBusiness.GetUserBaseModel(UserInfo.TeacherInfo.CoachId);
                ViewBag.Mentor = userModel == null ? "" : userModel.FirstName + " " + UserInfo.LastName;
            }
            else
                ViewBag.Mentor = "";

            ViewBag.Date = history.AssessmentDate.ToString("MM/dd/yyyy");

            CecReportModel cecReportModel = HttpContext.Cache[CacheKey(teacherId, assessmentId, wave, year)] as CecReportModel;
            if (cecReportModel == null)
                cecReportModel = _cecBusiness.GetCECReport(history.ID, assessmentId, wave);


            if (cecReportModel == null)
            {
                ViewBag.NoData = true;
                return View();
            }
            HttpContext.Cache.Insert(CacheKey(teacherId, assessmentId, wave, year), cecReportModel, null, DateTime.Now.AddMinutes(30), Cache.NoSlidingExpiration);

            ViewBag.List = cecReportModel.Items;
            ViewBag.ParentMeasure = cecReportModel.ParentMeasureList;
            ViewBag.MeasureList = cecReportModel.MeasureList;

            ViewBag.Pdf = false;
            if (export)
            {
                ViewBag.Pdf = true;
                GetPdf(GetViewHtml("CECReport"), "CECReport.pdf");
            }

            return View();
        }
    }
}