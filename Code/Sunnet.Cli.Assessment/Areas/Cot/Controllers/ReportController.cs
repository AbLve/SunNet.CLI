using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;
using Sunnet.Cli.Assessment.Controllers;
using Sunnet.Cli.Business;
using Sunnet.Cli.Business.Ade;
using Sunnet.Cli.Business.Cot;
using Sunnet.Cli.Business.Cot.Cumulative;
using Sunnet.Cli.Business.Cot.Growth;
using Sunnet.Cli.Business.Cot.Report;
using Sunnet.Cli.Business.Schools;
using Sunnet.Cli.Business.Users;
using Sunnet.Cli.Core.Users.Enums;
using Sunnet.Cli.UIBase.Filters;
using Sunnet.Cli.UIBase.Models;
using Sunnet.Framework.Extensions;
using Sunnet.Framework.PDF;
using Sunnet.Framework.Permission;
using ReportModel = Sunnet.Cli.Business.Cot.Cumulative.ReportModel;
using Sunnet.Cli.Business.Cot.Enum;
using Sunnet.Cli.Business.Cot.Summary;

namespace Sunnet.Cli.Assessment.Areas.Cot.Controllers
{
    public class ReportController : BaseController
    {
        UserBusiness _userBusiness = new UserBusiness();
        private CotReportBusiness _reportBusiness;
        private AdeBusiness _adeBusiness;
        private SchoolBusiness _schoolBusiness;

        private string GetViewHtml(string viewName)
        {
            ViewBag.Pdf = true;
            var resultHtml = "";
            ViewEngineResult result = ViewEngines.Engines.FindView(ControllerContext, viewName, null);
            if (null == result.View)
            {
                throw new InvalidOperationException(FormatErrorMessage(viewName, result.SearchedLocations));
            }
            try
            {
                ViewContext viewContext = new ViewContext(ControllerContext, result.View, this.ViewData, this.TempData,
                    Response.Output);
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

        private void GetPdf(string html, string fileName, PdfType type = PdfType.COT_Landscape_No_Pager)
        {
            string userName = UserInfo.FirstName + " " + UserInfo.LastName;
            PdfProvider pdfProvider = new PdfProvider(type);
            pdfProvider.GeneratePDF(html, fileName);
        }

        private string FormatErrorMessage(string viewName, IEnumerable<string> searchedLocations)
        {
            string format =
                "The view '{0}' or its master was not found or no view engine supports the searched locations. The following locations were searched:{1}";
            StringBuilder builder = new StringBuilder();
            foreach (string str in searchedLocations)
            {
                builder.AppendLine();
                builder.Append(str);
            }
            return string.Format(CultureInfo.CurrentCulture, format, viewName, builder);
        }

        public ReportController()
        {
            _reportBusiness = new CotReportBusiness(AdeUnitWorkContext);
            _adeBusiness = new AdeBusiness(AdeUnitWorkContext);
            _schoolBusiness = new SchoolBusiness();
        }
        // GET: Cot/Report
        [CLIUrlAuthorize(Account = Authority.Index, PageId = PagesModel.COT, Anonymity = Anonymous.Verified, Parameter = "assessmentId")]
        public ActionResult Index(int teacherId, int assessmentId, int year)
        {
            var teacherModel = _userBusiness.GetTeacherModel(teacherId, UserInfo);
            var measures = _adeBusiness.GetMeasureReport(assessmentId).Where(x => x.ParentId == 1).OrderBy(x => x.Sort);
            ViewBag.Measures = JsonHelper.SerializeObject(measures);

            return View(teacherModel);
        }

        #region COT Observed Strategies Report

        private string Key_Cot
        {
            get { return string.Format("Sunnet.Cli.Assessment.Areas.Cot_{0}", UserInfo.ID); }
        }

        public ActionResult CotPdf(int teacherId, int assessmentId, int year, string schoolId, int communityId, ObservedReportType type, string measures,
            string yearsInProject = null, List<AssignmentType> coachModels = null, List<AssignmentType> eCircles = null, string selectedTeachers = "")
        {
            var baseAssessment = _adeBusiness.GetBaseAssessmentModel(assessmentId);
            if (baseAssessment == null) return new EmptyResult();
            ViewBag.AssessmentName = baseAssessment.Name;

            var measureIds = measures.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Select(int.Parse);
            List<CotReportModel> list = null;
            if (type == ObservedReportType.SingleTeacher || type == ObservedReportType.AssignedTeachers)
            {
                var teacherIds = selectedTeachers.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Select(int.Parse);
                if (teacherIds.Any() && type == ObservedReportType.AssignedTeachers)
                {
                    list = _reportBusiness.GetMutiTeacherReports(assessmentId, teacherId, schoolId, communityId, year,
                        measureIds, type, UserInfo, teacherIds);
                }
                else
                {
                    list = _reportBusiness.GetCotReports(assessmentId, teacherId, schoolId, communityId, year,
                        measureIds, type, UserInfo);
                }
            }
            else
            {
                var years = JsonHelper.DeserializeObject<Dictionary<int, string>>(yearsInProject);
                list = _reportBusiness.GetCotReports(assessmentId, teacherId, schoolId, communityId, year,
                   measureIds, type, years, coachModels, eCircles, UserInfo);
            }
            if (list == null || !list.Any())
            {
                return View("NoTeacher");
            }
            CacheHelper.Add(Key_Cot, list);
            ViewBag.Json = JsonHelper.SerializeObject(list);
            ViewBag.Models = list;
            return View();
        }

        public ActionResult CotPdf_Download(int teacherId, int assessmentId, int year, string schoolId, int communityId, ObservedReportType type,
            string measures, List<string> imgSources,
            string yearsInProject = null, List<AssignmentType> coachModels = null, List<AssignmentType> eCircles = null, string selectedTeachers = "",
            bool export = true)
        {
            var baseAssessment = _adeBusiness.GetBaseAssessmentModel(assessmentId);
            if (baseAssessment == null) return new EmptyResult();
            ViewBag.AssessmentName = baseAssessment.Name;

            var measureIds = measures.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Select(int.Parse);
            var list = CacheHelper.Get<List<CotReportModel>>(Key_Cot);
            if (list == null)
            {
                if (yearsInProject == null)
                {
                    if (selectedTeachers == "")
                        list = _reportBusiness.GetCotReports(assessmentId, teacherId, schoolId, communityId, year,
                            measureIds, type, UserInfo);
                    else
                    {
                        var teacherIds = selectedTeachers.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Select(int.Parse);
                        list = _reportBusiness.GetMutiTeacherReports(assessmentId, teacherId, schoolId, communityId, year,
                            measureIds, type, UserInfo, teacherIds);
                    }
                }
                else
                {
                    var years = JsonHelper.DeserializeObject<Dictionary<int, string>>(yearsInProject);
                    list = _reportBusiness.GetCotReports(assessmentId, teacherId, schoolId, communityId, year,
                       measureIds, type, years, coachModels, eCircles, UserInfo);
                }
            }
            var index = 0;
            ViewBag.Models = list;
            foreach (var img in imgSources)
            {
                ViewData["Image" + (index++).ToString()] = img;
            }
            if (export)
            {
                GetPdf(GetViewHtml("CotPdf_Download"), list.First().Type.ToDescription().Replace(" ", "_"));
            }
            return View();
        }

        public ActionResult Filter(int teacherId, int assessmentId, int year, int schoolId, string schoolName, int communityId,
            ObservedReportType type, string measures)
        {
            ViewBag.Type = type;
            ViewBag.SchoolName = schoolName;
            ViewBag.YearsInProject = JsonHelper.SerializeObject(_userBusiness.GetYearsInProjects().ToSelectList());
            return View();
        }

        #endregion

        #region COT Item Report

        private string Key_Cumulative
        {
            get { return string.Format("Sunnet.Cli.Assessment.Areas.Cot.Cumulative_{0}", UserInfo.ID); }
        }

        public ActionResult CumulativePdf(int teacherId, int assessmentId, int year, string schoolId, int communityId, CumulativeReportType type, string measures, string selectedTeachers = "")
        {
            var baseAssessment = _adeBusiness.GetBaseAssessmentModel(assessmentId);
            if (baseAssessment == null) return new EmptyResult();
            ViewBag.AssessmentName = baseAssessment.Name;

            var measureIds = measures.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Select(int.Parse);
            var teacherIds = selectedTeachers.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Select(int.Parse);
            var list = _reportBusiness.GetCumulativeReports(assessmentId, teacherId, schoolId, communityId, year,
            measureIds, type, UserInfo, teacherIds);
            if (list == null || !list.Any())
            {
                return View("NoTeacher");
            }
            CacheHelper.Add(Key_Cumulative, list);
            ViewBag.Json = JsonHelper.SerializeObject(list);
            ViewBag.Model = list;
            return View();
        }


        public ActionResult CumulativePdf_Download(int teacherId, int assessmentId, int year, string schoolId, int communityId,
            CumulativeReportType type, string measures, List<string> imgSources, string selectedTeachers = "", bool export = true)
        {
            var baseAssessment = _adeBusiness.GetBaseAssessmentModel(assessmentId);
            if (baseAssessment == null) return new EmptyResult();
            ViewBag.AssessmentName = baseAssessment.Name;

            var measureIds = measures.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Select(int.Parse);
            var teacherIds = selectedTeachers.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Select(int.Parse);
            var list = CacheHelper.Get<List<ReportModel>>(Key_Cumulative);
            if (list == null)
            {
                list = _reportBusiness.GetCumulativeReports(assessmentId, teacherId, schoolId, communityId, year,
                measureIds, type, UserInfo, teacherIds);
            }
            var index = 0;
            ViewBag.Model = list;
            foreach (var img in imgSources)
            {
                ViewData["Image" + (index++).ToString()] = img;
            }
            if (export)
            {
                GetPdf(GetViewHtml("CumulativePdf_Download"), list.First().Type.ToDescription().Replace(" ", "_"));
            }
            return View();
        }


        public ActionResult Filter1(int teacherId, int assessmentId, int year, int schoolId, string schoolName, int communityId,
            CumulativeReportType type, string measures)
        {
            ViewBag.Type = type;
            ViewBag.SchoolName = schoolName;
            ViewBag.YearsInProject = JsonHelper.SerializeObject(_userBusiness.GetYearsInProjects().ToSelectList());
            return View();
        }


        public ActionResult Cumulative2Pdf(int teacherId, int assessmentId, int year, int schoolId, int communityId,
            CumulativeReportType type, string measures, string yearsInProject, List<AssignmentType> coachModels, List<AssignmentType> eCircles)
        {
            var baseAssessment = _adeBusiness.GetBaseAssessmentModel(assessmentId);
            if (baseAssessment == null) return new EmptyResult();
            ViewBag.AssessmentName = baseAssessment.Name;

            coachModels = coachModels ?? new List<AssignmentType>();
            eCircles = eCircles ?? new List<AssignmentType>();

            var measureIds = measures.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Select(int.Parse);
            var years = JsonHelper.DeserializeObject<Dictionary<int, string>>(yearsInProject);
            List<CumulativeReportModel> list = _reportBusiness.GetCumulative2Reports(assessmentId, teacherId, schoolId, communityId, year, measureIds, type,
                       years, coachModels, eCircles, UserInfo);

            if (list == null || !list.Any())
            {
                return View("NoTeacher");
            }

            CacheHelper.Add(Key_Cumulative, list, 120);
            ViewBag.Json = JsonHelper.SerializeObject(list);
            ViewBag.Model = list;
            return View();
        }

        public ActionResult Cumulative2Pdf_Download(int teacherId, int assessmentId, int year, int schoolId, int communityId,
            CumulativeReportType type, string measures, string yearsInProject,
            List<AssignmentType> coachModels, List<AssignmentType> eCircles, List<string> imgSources, bool export = true)
        {
            var baseAssessment = _adeBusiness.GetBaseAssessmentModel(assessmentId);
            if (baseAssessment == null) return new EmptyResult();
            ViewBag.AssessmentName = baseAssessment.Name;

            coachModels = coachModels ?? new List<AssignmentType>();
            eCircles = eCircles ?? new List<AssignmentType>();
            var measureIds = measures.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Select(int.Parse);
            var years = JsonHelper.DeserializeObject<Dictionary<int, string>>(yearsInProject);
            var list = CacheHelper.Get<List<CumulativeReportModel>>(Key_Cumulative);
            if (list == null)
            {
                list = _reportBusiness.GetCumulative2Reports(assessmentId, teacherId, schoolId, communityId, year, measureIds, type,
                 years, coachModels, eCircles, UserInfo);
            }
            ViewBag.Model = list;
            var index = 0;
            foreach (var img in imgSources)
            {
                ViewData["Image" + (index++).ToString()] = img;
            }
            if (export)
            {
                var pdfType = type == CumulativeReportType.AssignedTeachersCumulative
                    ? PdfType.COT_Landscape
                    : PdfType.COT_Landscape_No_Pager;

                GetPdf(GetViewHtml("Cumulative2Pdf_Download"), list.First().Type.ToDescription().Replace(" ", "_"), pdfType);
            }
            return View();
        }

        #endregion

        #region COT Growth Report

        private string Key_Growth
        {
            get { return string.Format("Sunnet.Cli.Assessment.Areas.Cot.Growth_{0}", UserInfo.ID); }
        }

        public ActionResult Filter2(int teacherId, int assessmentId, int year, int schoolId, string schoolName, int communityId,
                    GrowthReportType type, string measures)
        {
            ViewBag.Type = type;
            ViewBag.SchoolName = schoolName;
            ViewBag.YearsInProject = JsonHelper.SerializeObject(_userBusiness.GetYearsInProjects().ToSelectList());
            return View();
        }

        public ActionResult Growth(int teacherId, int assessmentId, int year, string schoolId, int communityId,
            GrowthReportType type, string measures, string selectedTeachers = "")
        {
            var baseAssessment = _adeBusiness.GetBaseAssessmentModel(assessmentId);
            if (baseAssessment == null) return new EmptyResult();
            ViewBag.AssessmentName = baseAssessment.Name;

            var measureIds = measures.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Select(int.Parse);
            var teacherIds = selectedTeachers.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Select(int.Parse);
            var list = _reportBusiness.GetGrowthReports(assessmentId, teacherId, schoolId, communityId, year,
                measureIds, type, UserInfo, teacherIds);
            if (list == null || !list.Any())
            {
                return View("NoTeacher");
            }
            CacheHelper.Add(Key_Growth, list, 120);
            ViewBag.Json = JsonHelper.SerializeObject(list);
            ViewBag.Model = list;
            return View();
        }

        public ActionResult Growth_Download(int teacherId, int assessmentId, int year, string schoolId, int communityId,
            GrowthReportType type, string measures, List<string> imgSources, string selectedTeachers = "", bool export = true)
        {
            var baseAssessment = _adeBusiness.GetBaseAssessmentModel(assessmentId);
            if (baseAssessment == null) return new EmptyResult();
            ViewBag.AssessmentName = baseAssessment.Name;

            var measureIds = measures.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Select(int.Parse);
            var teacherIds = selectedTeachers.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Select(int.Parse);
            var list = CacheHelper.Get<List<GrowthReportModel>>(Key_Growth);
            if (list == null)
            {
                list = _reportBusiness.GetGrowthReports(assessmentId, teacherId, schoolId, communityId, year, measureIds, type, UserInfo, teacherIds);
            }
            ViewBag.Model = list;
            var index = 0;
            foreach (var img in imgSources)
            {
                ViewData["Image" + (index++).ToString()] = img;
            }
            if (export)
            {
                var pdfType = PdfType.COT_Landscape;
                GetPdf(GetViewHtml("Growth_Download"), list.First().Type.ToDescription().Replace(" ", "_"), pdfType);
            }
            return View();
        }

        public ActionResult Growth2(int teacherId, int assessmentId, int year, int schoolId, int communityId,
            GrowthReportType type, string measures, string yearsInProject, List<AssignmentType> coachModels,
            List<AssignmentType> eCircles)
        {
            var baseAssessment = _adeBusiness.GetBaseAssessmentModel(assessmentId);
            if (baseAssessment == null) return new EmptyResult();
            ViewBag.AssessmentName = baseAssessment.Name;


            coachModels = coachModels ?? new List<AssignmentType>();
            eCircles = eCircles ?? new List<AssignmentType>();
            var measureIds = measures.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Select(int.Parse);
            var years = JsonHelper.DeserializeObject<Dictionary<int, string>>(yearsInProject);
            var list = _reportBusiness.GetGrowthReports(assessmentId, teacherId, schoolId, communityId, year,
                measureIds, type, years, coachModels, eCircles, UserInfo);
            if (list == null || !list.Any())
            {
                return View("NoTeacher");
            }
            CacheHelper.Add(Key_Growth, list, 120);
            ViewBag.Json = JsonHelper.SerializeObject(list);
            ViewBag.Model = list;
            return View();
        }

        public ActionResult NoTeacher()
        {
            return View();
        }

        public ActionResult Growth2_Download(int teacherId, int assessmentId, int year, int schoolId, int communityId,
            GrowthReportType type, string measures, string yearsInProject, List<AssignmentType> coachModels,
            List<AssignmentType> eCircles, List<string> imgSources, bool export = true)
        {
            var baseAssessment = _adeBusiness.GetBaseAssessmentModel(assessmentId);
            if (baseAssessment == null) return new EmptyResult();
            ViewBag.AssessmentName = baseAssessment.Name;

            coachModels = coachModels ?? new List<AssignmentType>();
            eCircles = eCircles ?? new List<AssignmentType>();
            var measureIds = measures.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Select(int.Parse);
            var years = JsonHelper.DeserializeObject<Dictionary<int, string>>(yearsInProject);
            var list = CacheHelper.Get<List<GrowthReportModel>>(Key_Growth);
            if (list == null)
            {
                list = _reportBusiness.GetGrowthReports(assessmentId, teacherId, schoolId, communityId, year, measureIds, type,
                    years, coachModels, eCircles, UserInfo);
            }
            ViewBag.Model = list;
            var index = 0;
            foreach (var img in imgSources)
            {
                ViewData["Image" + (index++).ToString()] = img;
            }
            if (export)
            {
                var pdfType = PdfType.COT_Landscape;
                GetPdf(GetViewHtml("Growth2_Download"), list.First().Type.ToDescription().Replace(" ", "_"), pdfType);
            }
            return View();
        }

        #endregion

        #region COT Summary Report

        private string Key_Summary
        {
            get { return string.Format("Sunnet.Cli.Assessment.Areas.Cot.Summary{0}", UserInfo.ID); }
        }

        public ActionResult Summary(int teacherId, int assessmentId, int year, string schoolId, int communityId,
            SummaryReportType type, string measures, string selectedTeachers = "")
        {
            var baseAssessment = _adeBusiness.GetBaseAssessmentModel(assessmentId);
            if (baseAssessment == null) return new EmptyResult();
            ViewBag.AssessmentName = baseAssessment.Name;

            var measureIds = measures.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Select(int.Parse);
            var teacherIds = selectedTeachers.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Select(int.Parse);
            var list = _reportBusiness.GetSummaryReports(assessmentId, teacherId, schoolId, communityId, year,
                measureIds, type, UserInfo, teacherIds);
            if (list == null || !list.Any())
            {
                return View("NoTeacher");
            }
            CacheHelper.Add(Key_Summary, list, 120);
            ViewBag.Json = JsonHelper.SerializeObject(list);
            ViewBag.Model = list;
            return View();
        }

        public ActionResult Summary_Download(int teacherId, int assessmentId, int year, string schoolId, int communityId,
            SummaryReportType type, string measures, List<string> imgSources, string selectedTeachers = "", bool export = true)
        {
            var baseAssessment = _adeBusiness.GetBaseAssessmentModel(assessmentId);
            if (baseAssessment == null) return new EmptyResult();
            ViewBag.AssessmentName = baseAssessment.Name;

            var measureIds = measures.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Select(int.Parse);
            var teacherIds = selectedTeachers.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Select(int.Parse);
            var list = CacheHelper.Get<List<SummaryReportModel>>(Key_Summary);
            if (list == null)
            {
                list = _reportBusiness.GetSummaryReports(assessmentId, teacherId, schoolId, communityId, year, measureIds, type, UserInfo, teacherIds);
            }
            ViewBag.Model = list;
            var index = 0;
            foreach (var img in imgSources)
            {
                ViewData["Image" + (index++).ToString()] = img;
            }
            if (export)
            {
                var pdfType = PdfType.COT_Landscape;
                GetPdf(GetViewHtml("Summary_Download"), list.First().Type.ToDescription().Replace(" ", "_"), pdfType);
            }
            return View();
        }

        public ActionResult Summary2(int teacherId, int assessmentId, int year, int schoolId, int communityId,
            SummaryReportType type, string measures, string yearsInProject, List<AssignmentType> coachModels,
            List<AssignmentType> eCircles)
        {
            var baseAssessment = _adeBusiness.GetBaseAssessmentModel(assessmentId);
            if (baseAssessment == null) return new EmptyResult();
            ViewBag.AssessmentName = baseAssessment.Name;

            coachModels = coachModels ?? new List<AssignmentType>();
            eCircles = eCircles ?? new List<AssignmentType>();
            var measureIds = measures.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Select(int.Parse);
            var years = JsonHelper.DeserializeObject<Dictionary<int, string>>(yearsInProject);
            var list = _reportBusiness.GetSummaryReports(assessmentId, teacherId, schoolId, communityId, year,
                measureIds, type, years, coachModels, eCircles, UserInfo);
            if (list == null || !list.Any())
            {
                return View("NoTeacher");
            }
            CacheHelper.Add(Key_Summary, list, 120);
            ViewBag.Json = JsonHelper.SerializeObject(list);
            ViewBag.Model = list;
            return View();
        }

        public ActionResult Summary2_Download(int teacherId, int assessmentId, int year, int schoolId, int communityId,
            SummaryReportType type, string measures, string yearsInProject, List<AssignmentType> coachModels,
            List<AssignmentType> eCircles, List<string> imgSources, bool export = true)
        {
            var baseAssessment = _adeBusiness.GetBaseAssessmentModel(assessmentId);
            if (baseAssessment == null) return new EmptyResult();
            ViewBag.AssessmentName = baseAssessment.Name;

            coachModels = coachModels ?? new List<AssignmentType>();
            eCircles = eCircles ?? new List<AssignmentType>();
            var measureIds = measures.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Select(int.Parse);
            var years = JsonHelper.DeserializeObject<Dictionary<int, string>>(yearsInProject);
            var list = CacheHelper.Get<List<SummaryReportModel>>(Key_Summary);
            if (list == null)
            {
                list = _reportBusiness.GetSummaryReports(assessmentId, teacherId, schoolId, communityId, year, measureIds, type,
                    years, coachModels, eCircles, UserInfo);
            }
            ViewBag.Model = list;
            var index = 0;
            foreach (var img in imgSources)
            {
                ViewData["Image" + (index++).ToString()] = img;
            }
            if (export)
            {
                var pdfType = PdfType.COT_Landscape;
                GetPdf(GetViewHtml("Summary2_Download"), list.First().Type.ToDescription().Replace(" ", "_"), pdfType);
            }
            return View();
        }
        #endregion

        #region Teacher List 

        public ActionResult TeacherList(int teacherId, int assessmentId, int year, string schoolId, string schoolName,
       string type, int communityId, string measures, CotReportType cotReportType)
        {
            ViewBag.Type = type;
            ViewBag.cotReportType = cotReportType;
            return View();
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.COT, Anonymity = Anonymous.Verified)]
        public string GetAllAssignedTeachers(int teacherId, string schoolId, int communityId, string type, CotReportType cotReportType)
        {
            int reportType = (int)Enum.Parse(typeof(ObservedReportType), type);
            if (cotReportType == CotReportType.Cumulative)
            {
                reportType = (int)Enum.Parse(typeof(ObservedReportType), type);
            }
            else if (cotReportType == CotReportType.Grouth)
            {
                reportType = (int)Enum.Parse(typeof(ObservedReportType), type);
            }
            var teachers = _reportBusiness.GetTeacherList(teacherId, schoolId, communityId, reportType, UserInfo);
            return JsonHelper.SerializeObject(teachers);
        }

        #endregion
    }
}