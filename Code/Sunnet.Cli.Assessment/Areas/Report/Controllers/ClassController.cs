using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Sunnet.Cli.Assessment.Areas.Cpalls.Models;
using Sunnet.Cli.Assessment.Areas.Report.Models;
using Sunnet.Cli.Assessment.Controllers;
using Sunnet.Cli.Business;
using Sunnet.Cli.Business.Ade;
using Sunnet.Cli.Business.Classes;
using Sunnet.Cli.Business.Common;
using Sunnet.Cli.Business.Communities;
using Sunnet.Cli.Business.Cpalls;
using Sunnet.Cli.Business.Cpalls.Models;
using Sunnet.Cli.Business.Cpalls.Models.Report;
using Sunnet.Cli.Business.Schools;
using Sunnet.Cli.Core.Ade;
using Sunnet.Cli.Core.Common.Enums;
using Sunnet.Cli.Core.Cpalls;
using Sunnet.Cli.UIBase;
using Sunnet.Cli.UIBase.Filters;
using Sunnet.Cli.UIBase.Models;
using Sunnet.Framework.Extensions;
using Sunnet.Framework.PDF;
using Sunnet.Framework.Permission;
using Sunnet.Framework.Resources;
using Sunnet.Cli.Business.Reports;
using Sunnet.Cli.Core.Reports;
using StructureMap;
using Sunnet.Framework.Encrypt;
using Sunnet.Framework;
using Newtonsoft.Json;
using Sunnet.Cli.Business.Ade.Models;

namespace Sunnet.Cli.Assessment.Areas.Report.Controllers
{
    public class ClassController : BaseController
    {
        private CpallsBusiness _cpallsBusiness;
        private readonly ClassBusiness _classBusiness;
        private SchoolBusiness _schoolBusiness;
        private readonly AdeBusiness _adeBusiness;
        private readonly ReportBusiness _reportBusiness;
        private readonly IEncrypt encrypter;
        private readonly string _reportFirstColumnTitle = "Class";
        private static string ReportTitle_CustomScore_Average = "School Custom Score Report";
        private static string ReportTitle_CustomScore_Benchmark = "School Custom Score Benchmark Report";
        public ClassController()
        {
            _cpallsBusiness = new CpallsBusiness(AdeUnitWorkContext);
            _schoolBusiness = new SchoolBusiness();
            _adeBusiness = new AdeBusiness(AdeUnitWorkContext);
            _classBusiness = new ClassBusiness();
            _reportBusiness = new ReportBusiness();
            encrypter = ObjectFactory.GetInstance<IEncrypt>();
        }

        private T GetFromCache<T>(string key) where T : class
        {
            key = string.Format("_{0}_{1}_{2}_", UserInfo.ID, ControllerContext.Controller, key);
            var value = CacheHelper.Get<T>(key);
            return value;
        }

        private void SetCache(string key, object value)
        {
            key = string.Format("_{0}_{1}_{2}_", UserInfo.ID, ControllerContext.Controller, key);
            CacheHelper.Add(key, value, 10 * 60);
        }

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

        private void GetPdf(string html, string fileName, PdfType type = PdfType.Assessment_Landscape)
        {
            PdfProvider pdfProvider = new PdfProvider(type);
            //pdfProvider.IsPortrait = false;
            pdfProvider.GeneratePDF(html, fileName);
        }

        public string Index()
        {
            ViewData["test"] = "Index";
            return GetViewHtml("Summary");
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

        #region Summary
        [CLIUrlAuthorize(Account = Authority.Index, PageId = PagesModel.CPALLS, Anonymity = Anonymous.Verified, Parameter = "assessmentId")]
        public ActionResult Summary(int assessmentId, int year, int schoolId)
        {
            List<MeasureHeaderModel> measures;
            List<MeasureHeaderModel> parentMeasures;
            _cpallsBusiness.BuilderHeader(assessmentId, year, Wave.BOY, out measures, out parentMeasures, true);

            var groups = MeasureGroup.GetGroupJson(measures, parentMeasures);
            ViewBag.MeasureJson = JsonHelper.SerializeObject(groups);

            var assessment = _adeBusiness.GetAssessment(assessmentId);
            ViewBag.language = assessment.Language;
            ViewBag.TheOtherLanguage = assessment.Language == AssessmentLanguage.English
                ? AssessmentLanguage.Spanish
                : AssessmentLanguage.English;

            var theOtherLanguageAssessment = _adeBusiness.GetTheOtherLanguageAssessment(assessmentId);
            if (theOtherLanguageAssessment != null)
            {
                _cpallsBusiness.BuilderHeader(theOtherLanguageAssessment.ID, year, Wave.BOY, out measures, out parentMeasures, true);
                groups = MeasureGroup.GetGroupJson(measures, parentMeasures);
                ViewBag.MeasureJson2 = JsonHelper.SerializeObject(groups);
            }
            else
            {
                ViewBag.MeasureJson2 = "false";
            }

            return View();
        }

        [CLIUrlAuthorize(Account = Authority.Index, PageId = PagesModel.CPALLS, Anonymity = Anonymous.Verified, Parameter = "assessmentId")]
        public ActionResult ClassPercentileRankAverage(int assessmentId, int year, int schoolId)
        {
            List<MeasureHeaderModel> measures;
            List<MeasureHeaderModel> parentMeasures;
            _cpallsBusiness.BuilderHeader(assessmentId, year, Wave.BOY, out measures, out parentMeasures, true);
            measures = measures.Where(e => e.PercentileRank).ToList();

            var groups = MeasureGroup.GetGroupJson(measures, parentMeasures);
            ViewBag.MeasureJson = JsonHelper.SerializeObject(groups);

            var assessment = _adeBusiness.GetAssessment(assessmentId);
            ViewBag.language = assessment.Language;
            ViewBag.TheOtherLanguage = assessment.Language == AssessmentLanguage.English
                ? AssessmentLanguage.Spanish
                : AssessmentLanguage.English;

            var theOtherLanguageAssessment = _adeBusiness.GetTheOtherLanguageAssessment(assessmentId);
            if (theOtherLanguageAssessment != null)
            {
                _cpallsBusiness.BuilderHeader(theOtherLanguageAssessment.ID, year, Wave.BOY, out measures, out parentMeasures, true);
                measures = measures.Where(e => e.PercentileRank).ToList();
                groups = MeasureGroup.GetGroupJson(measures, parentMeasures);
                ViewBag.MeasureJson2 = JsonHelper.SerializeObject(groups);
            }
            else
            {
                ViewBag.MeasureJson2 = "false";
            }

            return View();
        }

        [CLIUrlAuthorize(Account = Authority.Index, PageId = PagesModel.CPALLS, Anonymity = Anonymous.Verified, Parameter = "assessmentId")]
        public ActionResult SummaryWithAverge(int assessmentId, StudentAssessmentLanguage language, int year, int schoolId, string waves,
            DateTime? startDate, DateTime? endDate, DateTime? dobStartDate, DateTime? dobEndDate, bool export = false)
        {
            var assessment = _adeBusiness.GetBaseAssessmentModel(assessmentId);
            if (assessment == null) return View("NoData");
            ViewBag.AssessmentName = assessment.Name;

            if (assessment.TheOtherId > 0 && (byte)assessment.TheOtherLang == (byte)language)
            {
                assessmentId = assessment.TheOtherId;
            }

            var waveMeasures = JsonHelper.DeserializeObject<Dictionary<Wave, IEnumerable<int>>>(waves);
            var school = _schoolBusiness.GetCpallsSchoolModel(schoolId);

            ViewBag.Title = "School Average Scores";
            ViewBag.District = school.CommunitiesText;
            ViewBag.School = school.Name;
            ViewBag.Class = ViewTextHelper.DefaultAllText;
            ViewBag.Teacher = ViewTextHelper.DefaultAllText;
            ViewBag.Language = language.ToDescription();
            ViewBag.ScoolYear = year.ToFullSchoolYearString();
            ViewBag.ReportFirstColumn = _reportFirstColumnTitle;
            ViewBag.NoClassSummary = true;

            Dictionary<object, List<ReportRowModel>> reports = _cpallsBusiness.GetClassSummaryReport(assessmentId,
                UserInfo, year, schoolId, waveMeasures, startDate ?? StartDate, (endDate ?? EndDate).AddDays(1).Date,
                dobStartDate ?? CommonAgent.MinDate, dobEndDate ?? DateTime.Now);
            ViewBag.Waves = reports.ToDictionary(x => (Wave)x.Key, x => x.Value);
            ViewBag.Pdf = export;

            if (export)
            {
                GetPdf(GetViewHtml("SummaryWithAverge"), "Average Report");
            }

            return View();
        }

        [CLIUrlAuthorize(Account = Authority.Index, PageId = PagesModel.CPALLS, Anonymity = Anonymous.Verified, Parameter = "assessmentId")]
        public ActionResult SummaryPercentileRankAveragePdf(int assessmentId, StudentAssessmentLanguage language, int year, int schoolId, string waves,
            DateTime? startDate, DateTime? endDate, DateTime? dobStartDate, DateTime? dobEndDate, bool export = false)
        {
            var assessment = _adeBusiness.GetBaseAssessmentModel(assessmentId);
            if (assessment == null) return View("NoData");
            ViewBag.AssessmentName = assessment.Name;

            if (assessment.TheOtherId > 0 && (byte)assessment.TheOtherLang == (byte)language)
            {
                assessmentId = assessment.TheOtherId;
            }

            var waveMeasures = JsonHelper.DeserializeObject<Dictionary<Wave, IEnumerable<int>>>(waves);
            var school = _schoolBusiness.GetCpallsSchoolModel(schoolId);

            ViewBag.Title = "Class Percentile Rank Averages";
            ViewBag.District = school.CommunitiesText;
            ViewBag.School = school.Name;
            ViewBag.Class = ViewTextHelper.DefaultAllText;
            ViewBag.Teacher = ViewTextHelper.DefaultAllText;
            ViewBag.Language = language.ToDescription();
            ViewBag.ScoolYear = year.ToFullSchoolYearString();
            ViewBag.ReportFirstColumn = _reportFirstColumnTitle;
            ViewBag.NoClassSummary = true;

            Dictionary<object, List<ReportRowModel>> reports = _cpallsBusiness.ClassPercentileRankAverage(assessmentId,
                UserInfo, year, schoolId, waveMeasures, startDate ?? StartDate, (endDate ?? EndDate).AddDays(1).Date,
                dobStartDate ?? CommonAgent.MinDate, dobEndDate ?? DateTime.Now);
            ViewBag.Waves = reports.ToDictionary(x => (Wave)x.Key, x => x.Value);
            ViewBag.Pdf = export;

            if (export)
            {
                GetPdf(GetViewHtml("SummaryPercentileRankAveragePdf"), "Percentile Rank Report");
            }
            return View();
        }

        [CLIUrlAuthorize(Account = Authority.Index, PageId = PagesModel.CPALLS, Anonymity = Anonymous.Verified, Parameter = "assessmentId")]
        public ActionResult PercentageofSatisfactory(int assessmentId, StudentAssessmentLanguage language, int year, int schoolId,
            string waves, DateTime? startDate, DateTime? endDate, DateTime? dobStartDate, DateTime? dobEndDate, bool export = false)
        {
            var assessment = _adeBusiness.GetBaseAssessmentModel(assessmentId);
            if (assessment == null) return View("NoData");
            ViewBag.AssessmentName = assessment.Name;

            if (assessment.TheOtherId > 0 && (byte)assessment.TheOtherLang == (byte)language)
            {
                assessmentId = assessment.TheOtherId;
            }

            var waveMeasures = JsonHelper.DeserializeObject<Dictionary<Wave, IEnumerable<int>>>(waves);
            var school = _schoolBusiness.GetCpallsSchoolModel(schoolId);
            ViewBag.Title = "School Benchmark Report";
            ViewBag.District = school.CommunitiesText;
            ViewBag.School = school.Name;
            ViewBag.Class = ViewTextHelper.DefaultAllText;
            ViewBag.Teacher = ViewTextHelper.DefaultAllText;
            ViewBag.Language = language.ToDescription();
            ViewBag.ScoolYear = year.ToFullSchoolYearString();
            ViewBag.ReportFirstColumn = _reportFirstColumnTitle;

            var benchmarks = _adeBusiness.GetIEnumBenchmarks(assessmentId);
            Dictionary<object, List<ReportRowModel>> reportsForChart = _cpallsBusiness.GetClassSatisfactoryReport(assessmentId, UserInfo, year, schoolId, waveMeasures, startDate ?? StartDate, (endDate ?? EndDate).AddDays(1).Date,
                dobStartDate ?? CommonAgent.MinDate, dobEndDate ?? DateTime.Now, benchmarks, false);
            Dictionary<object, List<ReportRowModel>> reportsForTable = _cpallsBusiness.GetClassSatisfactoryReport(assessmentId, UserInfo, year, schoolId, waveMeasures, startDate ?? StartDate, (endDate ?? EndDate).AddDays(1).Date,
                dobStartDate ?? CommonAgent.MinDate, dobEndDate ?? DateTime.Now, benchmarks, true);

            ViewBag.Waves = reportsForTable.ToDictionary(x => (Wave)x.Key, x => x.Value);
            var jdata = reportsForChart.ToDictionary(x => (byte)x.Key, x => x.Value);
            ViewBag.JData = JsonHelper.SerializeObject(jdata);
            return View();
        }

        [CLIUrlAuthorize(Account = Authority.Index, PageId = PagesModel.CPALLS, Anonymity = Anonymous.Verified, Parameter = "assessmentId")]
        public ActionResult PercentageofSatisfactory_Pdf(int assessmentId, StudentAssessmentLanguage language, int year, int schoolId, string waves,
            List<string> imgSources, DateTime? startDate, DateTime? endDate, DateTime? dobStartDate, DateTime? dobEndDate, bool export = false)
        {
            var assessment = _adeBusiness.GetBaseAssessmentModel(assessmentId);
            if (assessment == null) return View("NoData");
            ViewBag.AssessmentName = assessment.Name;

            if (assessment.TheOtherId > 0 && (byte)assessment.TheOtherLang == (byte)language)
            {
                assessmentId = assessment.TheOtherId;
            }

            var waveMeasures = JsonHelper.DeserializeObject<Dictionary<Wave, IEnumerable<int>>>(waves);
            var school = _schoolBusiness.GetCpallsSchoolModel(schoolId);
            ViewBag.Title = "School Benchmark Report";
            ViewBag.District = school.CommunitiesText;
            ViewBag.School = school.Name;
            ViewBag.Class = ViewTextHelper.DefaultAllText;
            ViewBag.Teacher = ViewTextHelper.DefaultAllText;
            ViewBag.Language = language.ToDescription();
            ViewBag.ScoolYear = year.ToFullSchoolYearString();
            ViewBag.ReportFirstColumn = _reportFirstColumnTitle;

            var benchmarks = _adeBusiness.GetIEnumBenchmarks(assessmentId);

            Dictionary<object, List<ReportRowModel>> reportsForChart = _cpallsBusiness.GetClassSatisfactoryReport(assessmentId,
                UserInfo, year, schoolId, waveMeasures, startDate ?? StartDate, (endDate ?? EndDate).AddDays(1).Date,
                dobStartDate ?? CommonAgent.MinDate, dobEndDate ?? DateTime.Now, benchmarks, false);

            Dictionary<object, List<ReportRowModel>> reportsForTable = _cpallsBusiness.GetClassSatisfactoryReport(assessmentId,
                UserInfo, year, schoolId, waveMeasures, startDate ?? StartDate, (endDate ?? EndDate).AddDays(1).Date,
                dobStartDate ?? CommonAgent.MinDate, dobEndDate ?? DateTime.Now, benchmarks, true);

            var index = 0;
            foreach (var chart in reportsForChart)
            {
                ViewData["Image" + index] = imgSources[index++];
            }
            ViewBag.Waves = reportsForTable.ToDictionary(x => (Wave)x.Key, x => x.Value);
            ViewBag.Pdf = export;
            if (export)
            {
                GetPdf(GetViewHtml("PercentageofSatisfactory_Pdf"), "Benchmark_Report.pdf");
            }

            return View();
        }

        #endregion

        #region Class Completion
        [CLIUrlAuthorize(Account = Authority.Index, PageId = PagesModel.CPALLS, Anonymity = Anonymous.Verified, Parameter = "assessmentId")]
        public ActionResult Completion(int assessmentId, int year, int schoolId)
        {
            List<MeasureHeaderModel> measures = new List<MeasureHeaderModel>();
            List<MeasureHeaderModel> parentMeasures = new List<MeasureHeaderModel>();
            List<MeasureHeaderModel> measuresOther = new List<MeasureHeaderModel>();
            List<MeasureHeaderModel> parentMeasuresOther = new List<MeasureHeaderModel>();
            List<MeasureHeaderModel> measuresSum = new List<MeasureHeaderModel>();
            List<MeasureHeaderModel> parentMeasuresSum = new List<MeasureHeaderModel>();
            _cpallsBusiness.BuilderHeader(assessmentId, year, Wave.BOY, out measures, out parentMeasures, true);


            var assessment = _adeBusiness.GetAssessment(assessmentId);
            ViewBag.language = assessment.Language;
            var groups = MeasureGroup.GetGroupJson(measures, parentMeasures);
            ViewBag.MeasureJson = JsonHelper.SerializeObject(groups);

            ViewBag.TheOtherLanguage = assessment.Language == AssessmentLanguage.English
                ? StudentAssessmentLanguage.Spanish
                : StudentAssessmentLanguage.English;

            bool otherAssessmentStatusIsActive = false;
            var theOtherLanguageAssessment = _adeBusiness.GetTheOtherLanguageAssessment(assessmentId);
            if (theOtherLanguageAssessment != null)
            {
                _cpallsBusiness.BuilderHeader(theOtherLanguageAssessment.ID, year, Wave.BOY, out measuresOther, out parentMeasuresOther, true);
                groups = MeasureGroup.GetGroupJson(measuresOther, parentMeasuresOther);
                ViewBag.MeasureJson2 = JsonHelper.SerializeObject(groups);
                otherAssessmentStatusIsActive = theOtherLanguageAssessment.Status == EntityStatus.Active;
            }
            else
            {
                ViewBag.MeasureJson2 = "false";
            }

            ViewBag.BilingualLanguage = StudentAssessmentLanguage.Bilingual;
            if (assessment.Language == AssessmentLanguage.English)
            {
                measuresSum.AddRange(measures);
                parentMeasuresSum.AddRange(parentMeasures);
                if (otherAssessmentStatusIsActive)
                {
                    foreach (var item in measuresOther)
                    {
                        if (measures.All(o => o.MeasureId != item.RelatedMeasureId))
                        {
                            //Assessment为西班牙，改变无关联项的Measure的Name
                            measuresSum.Add(GetCloneModel(item, false));
                        }
                        //将ParentId!=1同时又没有关联项的Measure的父Measure，添加到parentMeasuresSum里
                        if (item.ParentId != 1 && parentMeasuresSum.All(o => o.MeasureId != item.ParentId))
                        {
                            parentMeasuresSum.Add(parentMeasuresOther.FirstOrDefault(o => o.MeasureId == item.ParentId));
                        }
                    }

                    //Assessment为英语，改变Measure的Name
                    foreach (var item in measures)
                    {
                        if (measuresOther.All(o => o.RelatedMeasureId != item.MeasureId))
                        {
                            int index = measures.IndexOf(item);
                            measuresSum.Remove(item);
                            measuresSum.Insert(index, GetCloneModel(item, true));
                        }
                        else
                        {
                            var spanishFirst = measuresOther.FirstOrDefault(o => o.RelatedMeasureId == item.MeasureId);
                            int index = measures.IndexOf(item);
                            measuresSum.Remove(item);
                            measuresSum.Insert(index, GetCloneModel(item, true, true, spanishFirst.Name));
                        }
                    }
                }
            }
            else
            {
                if (otherAssessmentStatusIsActive)
                {
                    measuresSum.AddRange(measuresOther);
                    parentMeasuresSum.AddRange(parentMeasuresOther);
                    foreach (var item in measures)
                    {
                        if (measuresOther.All(o => o.MeasureId != item.RelatedMeasureId))
                        {
                            //改Assessment为西班牙，改变无关联项的Measure的Name
                            measuresSum.Add(GetCloneModel(item, false));
                        }
                        if (item.ParentId != 1 && parentMeasuresSum.All(o => o.MeasureId != item.ParentId))
                        {
                            parentMeasuresSum.Add(parentMeasures.FirstOrDefault(o => o.MeasureId == item.ParentId));
                        }
                    }
                    //Assessment为英语，改变Measure的Name
                    foreach (var item in measuresOther)
                    {
                        if (measures.All(o => o.RelatedMeasureId != item.MeasureId))
                        {
                            int index = measuresOther.IndexOf(item);
                            measuresSum.Remove(item);
                            measuresSum.Insert(index, GetCloneModel(item, true));
                        }
                        else
                        {
                            var spanishFirst = measures.FirstOrDefault(o => o.RelatedMeasureId == item.MeasureId);
                            int index = measuresOther.IndexOf(item);
                            measuresSum.Remove(item);
                            measuresSum.Insert(index, GetCloneModel(item, true, true, spanishFirst.Name));
                        }
                    }
                }
                else
                {
                    measuresSum.AddRange(measures);
                    parentMeasuresSum.AddRange(parentMeasures);
                }
            }


            List<int> parentIds = measuresSum.Where(r => r.MeasureId != r.ParentId).GroupBy(r => r.ParentId).Select(r => r.Key).ToList();
            List<int> removeIds = new List<int>();
            foreach (MeasureHeaderModel item in parentMeasuresSum)
            {
                if (!(parentIds.Contains(item.MeasureId) || (item.ParentId == 1 && item.Subs == 0)))
                    removeIds.Add(item.MeasureId);
            }

            parentMeasuresSum.RemoveAll(r => removeIds.Contains(r.MeasureId));
            measuresSum.RemoveAll(r => r.MeasureId == r.ParentId && removeIds.Contains(r.MeasureId));

            groups = MeasureGroup.GetGroupJson(measuresSum, parentMeasuresSum);
            ViewBag.MeasureJson3 = JsonHelper.SerializeObject(groups);
            ViewBag.schoolName = _schoolBusiness.GetSchoolEntity(schoolId, UserInfo).SchoolName;
            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="measure"></param>
        /// <param name="isEnglish">measure所属的Assessment是否是英语</param>
        /// <param name="isCombine">是否需要将2个Name合并</param>
        /// <param name="name"></param>
        /// <returns></returns>
        private MeasureHeaderModel GetCloneModel(MeasureHeaderModel measure, bool isEnglish, bool isCombine = false, string name = "")
        {
            MeasureHeaderModel otherMeasure = new MeasureHeaderModel();
            otherMeasure.ID = measure.ID;
            otherMeasure.MeasureId = measure.MeasureId;
            otherMeasure.Name = measure.Name;
            if (!isCombine)
            {
                if (isEnglish)
                {
                    otherMeasure.TheOtherLanguageName = "Only in English";
                }
                else
                {
                    otherMeasure.TheOtherLanguageName = "Only in Spanish";
                }
            }
            else
            {
                otherMeasure.TheOtherLanguageName = name;
            }
            otherMeasure.ParentId = measure.ParentId;
            otherMeasure.TotalScored = measure.TotalScored;
            otherMeasure.TotalScore = measure.TotalScore;
            otherMeasure.ParentMeasureName = measure.ParentMeasureName;
            otherMeasure.Subs = measure.Subs;
            otherMeasure.Sort = measure.Sort;
            otherMeasure.ApplyToWave = measure.ApplyToWave;
            otherMeasure.RelatedMeasureId = measure.RelatedMeasureId;
            otherMeasure.Links = measure.Links;
            otherMeasure.IsFirstOfParent = measure.IsFirstOfParent;
            otherMeasure.IsLastOfParent = measure.IsLastOfParent;
            return otherMeasure;
        }

        [CLIUrlAuthorize(Account = Authority.Index, PageId = PagesModel.CPALLS, Anonymity = Anonymous.Verified, Parameter = "assessmentId")]
        public ActionResult CompletionPdf(int assessmentId, int year, int schoolId,
            string waves, StudentAssessmentLanguage language, DateTime? startDate, DateTime? endDate, DateTime? dobStartDate, DateTime? dobEndDate)
        {
            var assessment = _adeBusiness.GetBaseAssessmentModel(assessmentId);
            if (assessment == null) return View("NoData");
            ViewBag.AssessmentName = assessment.Name;

            DateTime dobStart = dobStartDate ?? CommonAgent.MinDate;
            DateTime dobEnd = dobEndDate ?? DateTime.Now;
            var waveMeasures = JsonHelper.DeserializeObject<Dictionary<Wave, IEnumerable<int>>>(waves);
            var school = _schoolBusiness.GetCpallsSchoolModel(schoolId);

            ReportList report = new ReportList();

            foreach (var v in waveMeasures)
            {
                if (v.Value.Any())
                {
                    report = _cpallsBusiness.GetReport_School(assessmentId, year.ToSchoolYearString(),
                  v.Value.ToList(), v.Key, language, schoolId, UserInfo, startDate ?? StartDate, (endDate ?? EndDate).AddDays(1).Date, dobStart, dobEnd);
                    break;
                }
            }

            ViewBag.report = report;

            ViewBag.communityName = school.CommunitiesText;
            ViewBag.schoolName = school.Name;
            ViewBag.Title = string.Format("School {0} Completion Report", language);
            ViewBag.language = language;
            ViewBag.year = year;
            if (language == StudentAssessmentLanguage.Bilingual)
            {
                ViewBag.Title = "School Combined Completion Report";
                ViewBag.language = "Combined";
            }

            ViewBag.report = report;
            ViewBag.json = JsonHelper.SerializeObject(report.ModelList);
            ViewBag.num = GetNum(report);
            return View();
        }

        [CLIUrlAuthorize(Account = Authority.Index, PageId = PagesModel.CPALLS, Anonymity = Anonymous.Verified, Parameter = "assessmentId")]
        public ActionResult CompletionPdf_Export(int assessmentId, int year, int schoolId, string waves,
            StudentAssessmentLanguage language, string imgSource, string imgSourcePercent, DateTime? startDate, DateTime? endDate, DateTime? dobStartDate, DateTime? dobEndDate, bool export = false)
        {
            var assessment = _adeBusiness.GetBaseAssessmentModel(assessmentId);
            if (assessment == null) return View("NoData");
            ViewBag.AssessmentName = assessment.Name;
            var waveMeasures = JsonHelper.DeserializeObject<Dictionary<Wave, IEnumerable<int>>>(waves);
            var school = _schoolBusiness.GetCpallsSchoolModel(schoolId);

            ViewBag.communityName = school.CommunitiesText;
            ViewBag.schoolName = school.Name;
            ViewBag.Title = string.Format("School {0} Completion Report", language);
            ViewBag.language = language;
            ViewBag.year = year;
            if (language == StudentAssessmentLanguage.Bilingual)
            {
                ViewBag.Title = "School Combined Completion Report";
                ViewBag.language = "Combined";
            }

            DateTime dobStart = dobStartDate ?? CommonAgent.MinDate;
            DateTime dobEnd = dobEndDate ?? DateTime.Now;
            ReportList report = new ReportList();

            foreach (var v in waveMeasures)
            {
                if (v.Value.Any())
                {
                    report = _cpallsBusiness.GetReport_School(assessmentId, year.ToSchoolYearString(),
                  v.Value.ToList(), v.Key, language, schoolId, UserInfo, startDate ?? StartDate, (endDate ?? EndDate).AddDays(1).Date, dobStart, dobEnd);
                    break;
                }
            }

            ViewBag.report = report;
            ViewBag.imgSource = imgSource;
            ViewBag.imgSourcePercent = imgSourcePercent;
            if (export)
            {
                GetPdf(GetViewHtml("CompletionPdf_Export"), "School_Combined_Completion");
            }
            return View();
        }

        private int GetNum(ReportList report)
        {
            if (report == null)
                return 0;

            int numTmp = 0;

            foreach (var wave in report.ModelList.Keys)
            {
                foreach (var item in report.ModelList[wave])
                {
                    if (item.Children == null)
                        ++numTmp;
                    else
                        numTmp += item.Children.Count;
                }
            }
            return numTmp;
        }
        #endregion

        #region Comparison Pdf
        [CLIUrlAuthorize(Account = Authority.Index, PageId = PagesModel.CPALLS, Anonymity = Anonymous.Verified, Parameter = "assessmentId")]
        public ActionResult Comparison(int assessmentId, int schoolId)
        {
            List<int> schoolYear = _cpallsBusiness.GetSchoolYear();
            ViewBag.schoolYear = schoolYear;
            List<MeasureHeaderModel> measures;
            List<MeasureHeaderModel> parentMeasures;
            _cpallsBusiness.BuilderHeader(assessmentId, schoolYear.FirstOrDefault(), Wave.BOY, out measures, out parentMeasures, true);
            ViewBag.Measures = measures;
            ViewBag.Parents = parentMeasures;
            var groups = MeasureGroup.GetGroupJson(measures, parentMeasures);
            ViewBag.MeasureJson = JsonHelper.SerializeObject(groups);
            return View();
        }

        [CLIUrlAuthorize(Account = Authority.Index, PageId = PagesModel.CPALLS, Anonymity = Anonymous.Verified, Parameter = "assessmentId")]
        public ActionResult ComparisonPdf(int assessmentId = 9, int schoolId = 67, string schoolYear = "", string waves = "")
        {
            var schoolYearArray = new string[1];
            if (schoolYear.Contains(","))
                schoolYearArray = schoolYear.Split(',');
            else
                schoolYearArray[0] = schoolYear;
            var waveMeasures = JsonHelper.DeserializeObject<Dictionary<Wave, IEnumerable<int>>>(waves);


            Dictionary<Wave, List<ReportModel>> report = _cpallsBusiness.GetReport_Class(assessmentId, schoolYearArray, waveMeasures, schoolId);

            ViewBag.schoolYear = schoolYearArray;
            ViewBag.json = JsonHelper.SerializeObject(report);
            ViewData["boyNum"] = GetNum(Wave.BOY, report);
            ViewData["moyNum"] = GetNum(Wave.MOY, report);
            ViewData["eoyNum"] = GetNum(Wave.EOY, report);
            ViewBag.report = report;

            return View();
        }

        private int GetNum(Wave wave, Dictionary<Wave, List<ReportModel>> report)
        {
            if (report == null)
                return 0;
            if (report.ContainsKey(wave) && report[wave].Any())
            {
                int numTmp = 0;
                foreach (ReportModel item in report[wave])
                {
                    if (item.Children == null)
                        ++numTmp;
                    else
                        numTmp += item.Children.Count;
                }
                return numTmp;
            }
            else
                return 0;
        }

        [CLIUrlAuthorize(Account = Authority.Index, PageId = PagesModel.CPALLS, Anonymity = Anonymous.Verified, Parameter = "assessmentId")]
        public ActionResult ComparisonPdf_Export(int assessmentId, int schoolId, string schoolYear, string waves,
            string boyImgSource, string moyImgSource, string eoyImgSource, bool export = false)
        {
            var schoolYearArray = new string[1];
            if (schoolYear.Contains(","))
                schoolYearArray = schoolYear.Split(',');
            else
                schoolYearArray[0] = schoolYear;

            var waveMeasures = JsonHelper.DeserializeObject<Dictionary<Wave, IEnumerable<int>>>(waves);
            Dictionary<Wave, List<ReportModel>> report = _cpallsBusiness.GetReport_Class(assessmentId, schoolYearArray, waveMeasures, schoolId);
            var school = _schoolBusiness.GetCpallsSchoolModel(schoolId);
            ViewBag.District = school.CommunitiesText;
            ViewBag.School = school.Name;

            ViewBag.report = report;
            ViewBag.schoolYear = schoolYearArray;
            ViewBag.boyImgSource = boyImgSource;
            ViewBag.moyImgSource = moyImgSource;
            ViewBag.eoyImgSource = eoyImgSource;
            if (export)
            {
                GetPdf(GetViewHtml("ComparisonPdf_Export"), "School Comparison Report");
            }
            return View();
        }
        #endregion

        #region Growth Report
        private string Key_Growth
        {
            get { return string.Format("CPALLS_Growth:{0}", UserInfo.ID); }
        }
        private string Key_GrowthBenchmark
        {
            get { return "CPALLS_Growth_BENCHMARK"; }
        }

        [CLIUrlAuthorize(Account = Authority.Index, PageId = PagesModel.CPALLS, Anonymity = Anonymous.Verified, Parameter = "assessmentId")]
        public ActionResult Growth(int assessmentId, int year, int schoolId, int districtId, string school)
        {
            ViewBag.Title = "School Growth Report";
            ViewBag.Source = school;
            ViewBag.AllSourceType = "Schools";

            List<MeasureHeaderModel> measures;
            List<MeasureHeaderModel> parentMeasures;
            _cpallsBusiness.BuilderHeader(assessmentId, year, Wave.BOY, out measures, out parentMeasures, true);

            var assessment = _adeBusiness.GetAssessment(assessmentId);
            ViewBag.language = assessment.Language;

            var groups = MeasureGroup.GetGroupJson(measures, parentMeasures);
            // growth report, 需要比较三个Wave的Measure 成绩
            var meaGroup = groups["groups"] as List<MeasureGroup>;
            if (meaGroup != null)
            {
                meaGroup.ForEach(mg => mg.Measures.ForEach(mea => mea.Waves = "1,2,3"));
            }
            ViewBag.MeasureJson = JsonHelper.SerializeObject(groups);

            ViewBag.TheOtherLanguage = assessment.Language == AssessmentLanguage.English
                ? AssessmentLanguage.Spanish
                : AssessmentLanguage.English;

            var schoolModel = _schoolBusiness.GetCpallsSchoolModel(schoolId);
            ViewBag.CommunityJson = JsonHelper.SerializeObject(schoolModel.Communities);

            var theOtherLanguageAssessment = _adeBusiness.GetTheOtherLanguageAssessment(assessmentId);
            if (theOtherLanguageAssessment != null)
            {
                _cpallsBusiness.BuilderHeader(theOtherLanguageAssessment.ID, year, Wave.BOY, out measures, out parentMeasures, true);
                groups = MeasureGroup.GetGroupJson(measures, parentMeasures);
                // growth report, 需要比较三个Wave的Measure 成绩
                meaGroup = groups["groups"] as List<MeasureGroup>;
                if (meaGroup != null)
                {
                    meaGroup.ForEach(mg => mg.Measures.ForEach(mea => mea.Waves = "1,2,3"));
                }
                ViewBag.MeasureJson2 = JsonHelper.SerializeObject(groups);
            }
            else
            {
                ViewBag.MeasureJson2 = "false";
            }
            return View();
        }

        [CLIUrlAuthorize(Account = Authority.Index, PageId = PagesModel.CPALLS, Anonymity = Anonymous.Verified, Parameter = "assessmentId")]
        public ActionResult Growth_Pdf(int assessmentId, StudentAssessmentLanguage language, int year, int schoolId, int districtId,
            GrowthReportType type, bool all, string waves, string measures, DateTime? startDate, DateTime? endDate,
            DateTime? dobStartDate, DateTime? dobEndDate)
        {
            var assessment = _adeBusiness.GetBaseAssessmentModel(assessmentId);
            if (assessment == null) return View("NoData");
            if (assessment.TheOtherId > 0 && (byte)assessment.TheOtherLang == (byte)language)
                assessmentId = assessment.TheOtherId;
            ViewBag.AssessmentName = assessment.Name;
            ViewBag.assessmentId = assessmentId;
            var ws = JsonHelper.DeserializeObject<List<int>>(waves).Select(x => (Wave)x).ToList();
            var measureIds = JsonHelper.DeserializeObject<List<int>>(measures);
            var benchmarks = _adeBusiness.GetIEnumBenchmarks(assessmentId);
            var waveMeasures = ws.ToDictionary(x => x, x => measureIds);
            var reports = _cpallsBusiness.GetSchoolGrowthPdf(assessmentId, language, UserInfo, year, type, waveMeasures,
                schoolId, districtId, all, startDate ?? StartDate, (endDate ?? EndDate).AddDays(1).Date,
                dobStartDate ?? CommonAgent.MinDate, dobEndDate ?? DateTime.Now, benchmarks);
            if (reports == null || !reports.Any())
            {
                return View("NoData");
            }
            SetCache(type == GrowthReportType.Average ? Key_Growth : Key_GrowthBenchmark, reports);

            ViewBag.Models = reports;
            ViewBag.JData = JsonHelper.SerializeObject(reports);
            return View("Growth_Pdf_" + type.ToString());
        }

        [CLIUrlAuthorize(Account = Authority.Index, PageId = PagesModel.CPALLS, Anonymity = Anonymous.Verified, Parameter = "assessmentId")]
        public ActionResult Growth_Pdf_Download(int assessmentId, StudentAssessmentLanguage language, GrowthReportType type,
            int year, int schoolId, int districtId, bool all, string waves, string measures, List<string> imgSources, bool export,
            DateTime? startDate, DateTime? endDate, DateTime? dobStartDate, DateTime? dobEndDate)
        {
            var assessment = _adeBusiness.GetBaseAssessmentModel(assessmentId);
            if (assessment == null) return View("NoData");
            ViewBag.AssessmentName = assessment.Name;

            var ws = JsonHelper.DeserializeObject<List<int>>(waves).Select(x => (Wave)x).ToList();
            var measureIds = JsonHelper.DeserializeObject<List<int>>(measures);
            var benchmarks = _adeBusiness.GetIEnumBenchmarks(assessmentId);
            var waveMeasures = ws.ToDictionary(x => x, x => measureIds);

            var reports = GetFromCache<List<GrowthReportModel>>(type == GrowthReportType.Average ? Key_Growth : Key_GrowthBenchmark);
            reports = reports ??
                      _cpallsBusiness.GetSchoolGrowthPdf(assessmentId, language, UserInfo, year, type, waveMeasures,
                          schoolId, districtId, all, startDate ?? StartDate, (endDate ?? EndDate).AddDays(1).Date,
                          dobStartDate ?? CommonAgent.MinDate, dobEndDate ?? DateTime.Now, benchmarks);
            if (reports == null || !reports.Any())
            {
                return View("NoData");
            }
            var index = 0;
            foreach (var img in imgSources)
            {
                ViewData["Image" + (index++).ToString()] = img;
            }
            ViewBag.Models = reports;
            if (export)
            {
                GetPdf(GetViewHtml("Growth_Pdf_Download"), reports.First().Title.Replace(" ", "_"));
            }
            return View();
        }
        #endregion

        #region Custom Score Report

        [CLIUrlAuthorize(Account = Authority.Index, PageId = PagesModel.CPALLS, Anonymity = Anonymous.Verified,
            Parameter = "assessmentId")]
        public ActionResult CustomScoreReportView(int assessmentId, int year, int schoolId, int districtId, string school = "")
        {
            var assessment = _adeBusiness.GetAssessment(assessmentId);
            ViewBag.language = assessment.Language;

            var theOtherLanguageAssessment = _adeBusiness.GetTheOtherLanguageAssessment(assessmentId);
            int theOtherAssessmentId = 0;
            ViewBag.AssessmentId = assessmentId;
            ViewBag.CommunityId = districtId;
            ViewBag.SchoolId = schoolId;
            ViewBag.Year = year;
            ViewBag.School = school;
            if (theOtherLanguageAssessment != null)
            {
                theOtherAssessmentId = theOtherLanguageAssessment.ID;
                ViewBag.TheOtherLanguage = theOtherLanguageAssessment.Language;
            }
            if (assessment.Language == AssessmentLanguage.English)
            {
                ViewBag.CustomScoresEnglish = _adeBusiness.GetScoresByAssessmentId(assessmentId);
                ViewBag.CustomScoresSpanish = _adeBusiness.GetScoresByAssessmentId(theOtherAssessmentId);
            }
            else if (assessment.Language == AssessmentLanguage.Spanish)
            {
                ViewBag.CustomScoresSpanish = _adeBusiness.GetScoresByAssessmentId(assessmentId);
                ViewBag.CustomScoresEnglish = _adeBusiness.GetScoresByAssessmentId(theOtherAssessmentId);
            }
            return View();
        }

        #region Custom Score Report not Benchmark
        [CLIUrlAuthorize(Account = Authority.Index, PageId = PagesModel.CPALLS, Anonymity = Anonymous.Verified, Parameter = "assessmentId")]
        public string CustomScoreWhere(int assessmentId, StudentAssessmentLanguage language, int schoolId,
            Wave wave, int scoreYear, List<int> scoreIds, DateTime? startDate, DateTime? endDate, DateTime? dobStartDate, DateTime? dobEndDate)
        {
            var response = new PostFormResponse();

            var assessment = _adeBusiness.GetBaseAssessmentModel(assessmentId);
            if (assessment == null)
            {
                response.Success = false;
                response.Message = "Assessment is required.";
                return JsonHelper.SerializeObject(response);
            }
            if (assessment.TheOtherId > 0 && (byte)assessment.TheOtherLang == (byte)language)
            {
                assessmentId = assessment.TheOtherId;
            }
            var dicParams = new Dictionary<string, object>();
            dicParams.Add("assessmentId", assessmentId);
            dicParams.Add("language", (int)language);
            dicParams.Add("schoolId", schoolId);
            dicParams.Add("wave", (int)wave);
            dicParams.Add("year", scoreYear);
            dicParams.Add("scoreIds", scoreIds);
            dicParams.Add("startDate", startDate ?? StartDate);
            dicParams.Add("endDate", (endDate ?? EndDate).AddDays(1).Date);
            dicParams.Add("dobStartDate", dobStartDate ?? CommonAgent.MinDate);
            dicParams.Add("dobEndDate", dobEndDate ?? DateTime.Now);
            var queryParams = JsonHelper.SerializeObject(dicParams);

            response.Update(_reportBusiness.SubmitReport(ReportTitle_CustomScore_Average, ReportQueueType.School_CustomScoreReport, queryParams, Url.Action("CustomScoreDL_Report") + "/{ID}", UserInfo));
            if (response.Success) response.Message = ResourceHelper.GetRM().GetInformation("Report_Queue_Submitted");
            return JsonHelper.SerializeObject(response);
        }

        [CLIUrlAuthorize(Account = Authority.All, PageId = PagesModel.CPALLS, Anonymity = Anonymous.Verified)]
        public ActionResult CustomScoreDL_Report(string id)
        {
            var reportId = 0;
            if (!int.TryParse(id, out reportId))
            {
                int.TryParse(encrypter.Decrypt(id), out reportId);
            }
            var reportEntity = _reportBusiness.GetReportQueue(reportId);

            if (reportEntity == null)
                throw new HttpException(404, ResourceHelper.GetRM().GetInformation("Report_Queue_Report_404"));
            if (reportEntity.Status < ReportQueueStatus.Processed)
                throw new HttpException(404, ResourceHelper.GetRM().GetInformation("Report_Queue_Report_Not_Ready"));
            if (reportEntity.CreatedBy != UserInfo.ID)
                return new RedirectResult("/error/nonauthorized");
            if (reportEntity.CreatedOn.AddDays(SFConfig.ReportExpire) < DateTime.Now.AddDays(1))
                return new RedirectResult(string.Format("{0}/expire.html", DomainHelper.SsoSiteDomain));


            ViewBag.Title = reportEntity.Title;
            ViewBag.CanDownload = reportEntity.Status >= ReportQueueStatus.Processed;
            return View();
        }

        [CLIUrlAuthorize(Account = Authority.All, PageId = PagesModel.CPALLS, Anonymity = Anonymous.Verified)]
        public ActionResult CustomScorePdf(string id, bool export = true)
        {
            var reportId = 0;
            if (!int.TryParse(id, out reportId))
            {
                int.TryParse(encrypter.Decrypt(id), out reportId);
            }
            var reportEntity = _reportBusiness.GetReportQueue(reportId);
            if (reportEntity == null)
                throw new HttpException(404, ResourceHelper.GetRM().GetInformation("Report_Queue_Report_404"));
            if (reportEntity.Status < ReportQueueStatus.Processed)
                throw new HttpException(404, ResourceHelper.GetRM().GetInformation("Report_Queue_Report_Not_Ready"));

            var queryParams = JsonHelper.DeserializeObject<Dictionary<string, object>>(reportEntity.QueryParams);
            ViewBag.Title = reportEntity.Title;

            var assessmentId = int.Parse(queryParams["assessmentId"].ToString());
            var language = 0;
            int.TryParse(queryParams["language"].ToString(), out language);
            var year = 0;
            int.TryParse(queryParams["year"].ToString(), out year);

            var assessment = _adeBusiness.GetBaseAssessmentModel(assessmentId);
            if (assessment == null) return View("NoData");
            ViewBag.AssessmentName = assessment.Name;

            ViewBag.Title = reportEntity.Type.ToDescription();
            ViewBag.School = ViewTextHelper.DefaultAllText;
            ViewBag.Class = ViewTextHelper.DefaultAllText;
            ViewBag.Teacher = ViewTextHelper.DefaultAllText;
            ViewBag.Language = ((StudentAssessmentLanguage)language).ToDescription();
            ViewBag.ScoolYear = year.ToFullSchoolYearString();
            ViewBag.ReportFirstColumn = _reportFirstColumnTitle;
            ViewBag.NoClassSummary = true;
            int wave = 0;
            int.TryParse(queryParams["wave"].ToString(), out wave);
            ViewBag.Wave = (Wave)wave;

            var reports = JsonHelper.DeserializeObject<List<CustomScoreReportModel>>(reportEntity.Result);
            ViewBag.CustomScoreReports = reports;
            var scoreIds =
                JsonConvert.DeserializeObject<List<int>>(queryParams["scoreIds"].ToString());
            ViewBag.CustomScoreInits = reports.FirstOrDefault().ScoreInits;
            var benchmarks = _adeBusiness.GetIEnumBenchmarks(assessmentId);
            ViewBag.BenchmarkInits = benchmarks.ToList();
            string filename = string.IsNullOrEmpty(reportEntity.Report)
                ? "Assessments/" + assessmentId + "/Result_" + reportEntity.UpdatedOn.ToString("yyyy_MM_dd_HH_mm_ss") + ".pdf"
                : reportEntity.Report;

            string outFilename = reportEntity.Title + ".pdf";
            var localFile = FileHelper.HasProtectedFile(filename);
            if (string.IsNullOrEmpty(localFile))
            {
                localFile = FileHelper.GetProtectedFilePhisycalPath(filename);
                GetPdf(GetViewHtml("CustomScorePdf"), localFile, PdfType.Assessment_Portrait);
                _reportBusiness.UpdateReportStatus(reportId, ReportQueueStatus.Downloaded, filename);
            }
            if (export)
            {
                FileHelper.ResponseFile(localFile, outFilename);
            }
            return new EmptyResult();
        }
        #endregion

        #region Custom Score Benchmark Report 
        [CLIUrlAuthorize(Account = Authority.Index, PageId = PagesModel.CPALLS, Anonymity = Anonymous.Verified, Parameter = "assessmentId")]
        public string CustomScoreBenchmarkWhere(int assessmentId, StudentAssessmentLanguage language, int schoolId,
            Wave wave, int scoreYear, List<int> scoreIds, DateTime? startDate, DateTime? endDate, DateTime? dobStartDate, DateTime? dobEndDate)
        {
            var response = new PostFormResponse();

            var assessment = _adeBusiness.GetBaseAssessmentModel(assessmentId);
            if (assessment == null)
            {
                response.Success = false;
                response.Message = "Assessment is required.";
                return JsonHelper.SerializeObject(response);
            }
            if (assessment.TheOtherId > 0 && (byte)assessment.TheOtherLang == (byte)language)
            {
                assessmentId = assessment.TheOtherId;
            }
            var dicParams = new Dictionary<string, object>();
            dicParams.Add("assessmentId", assessmentId);
            dicParams.Add("language", (int)language);
            dicParams.Add("schoolId", schoolId);
            dicParams.Add("wave", (int)wave);
            dicParams.Add("year", scoreYear);
            dicParams.Add("scoreIds", scoreIds);
            dicParams.Add("startDate", startDate ?? StartDate);
            dicParams.Add("endDate", (endDate ?? EndDate).AddDays(1).Date);
            dicParams.Add("dobStartDate", dobStartDate ?? CommonAgent.MinDate);
            dicParams.Add("dobEndDate", dobEndDate ?? DateTime.Now);
            var queryParams = JsonHelper.SerializeObject(dicParams);

            response.Update(_reportBusiness.SubmitReport(ReportTitle_CustomScore_Benchmark, ReportQueueType.School_CustomScoreReport, queryParams, Url.Action("CustomScoreBenchmarkDL_Report") + "/{ID}", UserInfo));
            if (response.Success) response.Message = ResourceHelper.GetRM().GetInformation("Report_Queue_Submitted");
            return JsonHelper.SerializeObject(response);
        }

        [CLIUrlAuthorize(Account = Authority.All, PageId = PagesModel.CPALLS, Anonymity = Anonymous.Verified)]
        public ActionResult CustomScoreBenchmarkDL_Report(string id)
        {
            var reportId = 0;
            if (!int.TryParse(id, out reportId))
            {
                int.TryParse(encrypter.Decrypt(id), out reportId);
            }
            var reportEntity = _reportBusiness.GetReportQueue(reportId);

            if (reportEntity == null)
                throw new HttpException(404, ResourceHelper.GetRM().GetInformation("Report_Queue_Report_404"));
            if (reportEntity.Status < ReportQueueStatus.Processed)
                throw new HttpException(404, ResourceHelper.GetRM().GetInformation("Report_Queue_Report_Not_Ready"));
            if (reportEntity.CreatedBy != UserInfo.ID)
                return new RedirectResult("/error/nonauthorized");
            if (reportEntity.CreatedOn.AddDays(SFConfig.ReportExpire) < DateTime.Now.AddDays(1))
                return new RedirectResult(string.Format("{0}/expire.html", DomainHelper.SsoSiteDomain));


            ViewBag.Title = reportEntity.Title;
            ViewBag.CanDownload = reportEntity.Status >= ReportQueueStatus.Processed;
            return View();
        }

        [CLIUrlAuthorize(Account = Authority.All, PageId = PagesModel.CPALLS, Anonymity = Anonymous.Verified)]
        public ActionResult CustomScoreBenchmarkPdf(string id, bool export = true)
        {
            var reportId = 0;
            if (!int.TryParse(id, out reportId))
            {
                int.TryParse(encrypter.Decrypt(id), out reportId);
            }
            var reportEntity = _reportBusiness.GetReportQueue(reportId);
            if (reportEntity == null)
                throw new HttpException(404, ResourceHelper.GetRM().GetInformation("Report_Queue_Report_404"));
            if (reportEntity.Status < ReportQueueStatus.Processed)
                throw new HttpException(404, ResourceHelper.GetRM().GetInformation("Report_Queue_Report_Not_Ready"));

            var queryParams = JsonHelper.DeserializeObject<Dictionary<string, object>>(reportEntity.QueryParams);
            ViewBag.Title = reportEntity.Title;

            var assessmentId = int.Parse(queryParams["assessmentId"].ToString());
            var language = 0;
            int.TryParse(queryParams["language"].ToString(), out language);
            var year = 0;
            int.TryParse(queryParams["year"].ToString(), out year);

            var assessment = _adeBusiness.GetBaseAssessmentModel(assessmentId);
            if (assessment == null) return View("NoData");
            ViewBag.AssessmentName = assessment.Name;

            ViewBag.Title = reportEntity.Type.ToDescription();
            ViewBag.School = ViewTextHelper.DefaultAllText;
            ViewBag.Class = ViewTextHelper.DefaultAllText;
            ViewBag.Teacher = ViewTextHelper.DefaultAllText;
            ViewBag.Language = ((StudentAssessmentLanguage)language).ToDescription();
            ViewBag.ScoolYear = year.ToFullSchoolYearString();
            ViewBag.ReportFirstColumn = _reportFirstColumnTitle;
            ViewBag.NoClassSummary = true;
            int wave = 0;
            int.TryParse(queryParams["wave"].ToString(), out wave);
            ViewBag.Wave = (Wave)wave;

            var reports = JsonHelper.DeserializeObject<List<CustomScoreReportModel>>(reportEntity.Result);
            ViewBag.CustomScoreReports = reports;
            var scoreIds =
                JsonConvert.DeserializeObject<List<int>>(queryParams["scoreIds"].ToString());
            ViewBag.CustomScoreInits = reports.FirstOrDefault().ScoreInits;
            var benchmarks = _adeBusiness.GetIEnumBenchmarks(assessmentId);
            ViewBag.BenchmarkInits = benchmarks.ToList();
            string filename = string.IsNullOrEmpty(reportEntity.Report)
                ? "Assessments/" + assessmentId + "/Result_" + reportEntity.UpdatedOn.ToString("yyyy_MM_dd_HH_mm_ss") + ".pdf"
                : reportEntity.Report;

            string outFilename = reportEntity.Title + ".pdf";
            var localFile = FileHelper.HasProtectedFile(filename);
            if (string.IsNullOrEmpty(localFile))
            {
                localFile = FileHelper.GetProtectedFilePhisycalPath(filename);
                GetPdf(GetViewHtml("CustomScoreBenchmarkPdf"), localFile, PdfType.Assessment_Portrait);
                _reportBusiness.UpdateReportStatus(reportId, ReportQueueStatus.Downloaded, filename);
            }
            if (export)
            {
                FileHelper.ResponseFile(localFile, outFilename);
            }
            return new EmptyResult();
        }
        #endregion
        #endregion

        private DateTime StartDate
        {
            get
            {
                return CommonAgent.GetStartDateOfSchoolYear();
            }
        }

        private DateTime EndDate
        {
            get
            {
                return DateTime.Now.Date;
            }
        }

        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            base.OnActionExecuted(filterContext);

            ViewBag.StartDate = StartDate;
            ViewBag.EndDate = EndDate;
        }
    }
}