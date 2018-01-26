using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using Newtonsoft.Json;

using Sunnet.Cli.Business.Ade;
using Sunnet.Cli.Business.Ade.Models;
using Sunnet.Cli.Business.Users;
using Sunnet.Cli.Core.Ade;
using Sunnet.Cli.Core.Ade.Entities;
using Sunnet.Cli.Core.Common.Enums;
using Sunnet.Cli.Core.Users.Enums;
using Sunnet.Cli.UIBase.Controllers;
using Sunnet.Cli.UIBase.Models;
using Sunnet.Framework.Core.Tool;
using Sunnet.Framework.Extensions;
using Sunnet.Framework.Helpers;
using Sunnet.Cli.Assessment.Controllers;
using Sunnet.Cli.Business.Communities;
using Sunnet.Cli.UIBase.Filters;
using Sunnet.Framework.Permission;
using Sunnet.Cli.Business.Permission;
using Sunnet.Cli.Business.Students;
using Sunnet.Cli.Core.Communities.Entities;
using Sunnet.Framework;
using Sunnet.Framework.File;
using Sunnet.Cli.Core.Cpalls;

namespace Sunnet.Cli.Assessment.Areas.Ade.Controllers
{

    public class AssessmentController : BaseController
    {
        private AdeBusiness _adeBusiness;
        private UserBusiness _userBuss = new UserBusiness();
        private StudentBusiness _studentBusiness = new StudentBusiness();
        private CommunityBusiness _communityBusiness = new CommunityBusiness();
        public AssessmentController()
        {
            _adeBusiness = new AdeBusiness(AdeUnitWorkContext);
        }

        //
        // GET: /Ade/Assessment/
        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.ADE, Anonymity = Anonymous.Verified)]
        public ActionResult Index()
        {
            return View();
        }


        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.ADE, Anonymity = Anonymous.Verified)]
        public string Search(string label, string name, string createdby, int status = -1, string sort = "Name", string order = "Asc",
            int first = 0, int count = 10)
        {
            var users = _userBuss.SearchUserIds(createdby);
            var total = 0;
            var searchCriteria = PredicateHelper.True<AssessmentEntity>()
                .And(x => x.IsDeleted == false);
            if (!string.IsNullOrEmpty(label = label.Trim()))
                searchCriteria = searchCriteria.And(x => x.Label.Contains(label));
            if (!string.IsNullOrEmpty(name = name.Trim()))
                searchCriteria = searchCriteria.And(x => x.Name.Contains(name));
            if (users != null)
                searchCriteria = searchCriteria.And(x => users.Contains(x.CreatedBy));
            if (status > -1)
                searchCriteria = searchCriteria.And(x => (int)x.Status == status);
            var list = _adeBusiness.SearchAssessments(searchCriteria
                , sort, order, first, count, out total);
            var result = new { total = total, data = list };
            return JsonHelper.SerializeObject(result);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.ADE, Anonymity = Anonymous.Verified)]
        public ActionResult New()
        {
            var model = new AssessmentModel();

            return View(model);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.ADE, Anonymity = Anonymous.Verified)]
        public ActionResult Edit(int id)
        {
            var model = _adeBusiness.GetAssessmentModel(id);
            ViewBag.Scores = JsonHelper.SerializeObject(model.CutOffScores);
            ViewBag.Benchmarks = JsonHelper.SerializeObject(model.Benchmarks);
            ViewBag.BlackWhiteList = BlackWhiteStyle.Underline.ToSelectList();
            ViewBag.ShowUpload = (string.IsNullOrEmpty(model.ParentReportCoverPath) || string.IsNullOrEmpty(model.ParentReportCoverName));
            if (model.Locked)
            {
                if (model.Type == AssessmentType.Cpalls)
                    return View("View", model);
                else
                {
                    return View("View_CECCOT", model);
                }
            }
            if (model.Type == AssessmentType.Cpalls)
            {
                return View(model);
            }
            return View("Edit_CECCOT", model);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.ADE, Anonymity = Anonymous.Verified)]
        public ActionResult Detail(int id)
        {
            var model = _adeBusiness.GetAssessmentModel(id);
            return View(model);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.ADE, Anonymity = Anonymous.Verified)]
        public ActionResult CutoffScores(int id)
        {
            var model = _adeBusiness.GetAssessmentModel(id);
            ViewBag.Scores = JsonHelper.SerializeObject(model.CutOffScores);
            return View(model);
        }

        [System.Web.Mvc.HttpPost]
        [ValidateAntiForgeryToken]
        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.ADE, Anonymity = Anonymous.Verified)]
        public string CutoffScores(int id, string scores)
        {
            var scoreEntities = JsonHelper.DeserializeObject<List<CutOffScoreEntity>>(scores);
            var response = new PostFormResponse() { Success = ModelState.IsValid };
            if (response.Success)
            {
                var result = _adeBusiness.UpdateCutoffScores<MeasureEntity>(id, scoreEntities);
                response.Update(result);
            }
            response.ModelState = ModelState;
            return JsonHelper.SerializeObject(response);
        }

        private class CompareBenchmark : IEqualityComparer<BenchmarkEntity>
        {
            public bool Equals(BenchmarkEntity x, BenchmarkEntity y)
            {
                return x.LabelText == y.LabelText && x.Color == y.Color
                    && x.BlackWhite == y.BlackWhite;
            }

            public int GetHashCode(BenchmarkEntity obj)
            {
                var benchmark = new { obj.LabelText, obj.Color, obj.BlackWhite };
                return benchmark.GetHashCode();
            }
        }

        [System.Web.Mvc.HttpPost]
        [ValidateAntiForgeryToken]
        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.ADE, Anonymity = Anonymous.Verified)]
        public string Save(int id, AssessmentModel model, string scores = null, string adebenchmarks = null)
        {
            var response = new PostFormResponse() { Success = ModelState.IsValid };
            model.CreatedBy = UserInfo.ID;
            model.UpdatedBy = UserInfo.ID;
            //if (!string.IsNullOrEmpty(scores))
            //{
            //    var scoreEntities = JsonHelper.DeserializeObject<List<CutOffScoreEntity>>(scores);
            //    model.CutOffScores = scoreEntities;
            //}
            model.CutOffScores = null;
            if (!string.IsNullOrEmpty(adebenchmarks))
            {
                var benchmarkEntities = JsonHelper.DeserializeObject<List<BenchmarkEntity>>(adebenchmarks);
                model.Benchmarks = benchmarkEntities.Distinct(new CompareBenchmark()).ToList();
            }
            if (response.Success)
            {
                var result = _adeBusiness.SaveAssessment(model);
                response.Success = result.ResultType == OperationResultType.Success;
                response.Data = model;
                response.Message = result.Message;
            }
            else
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors);
            }

            response.ModelState = ModelState;
            return JsonHelper.SerializeObject(response);
        }

        [System.Web.Mvc.HttpPost]
        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.ADE, Anonymity = Anonymous.Verified)]
        public string Delete(int id)
        {
            var response = new PostFormResponse();
            var result = _adeBusiness.DeleteAssessment(id);
            if (result.ResultType == OperationResultType.Success)
                new PermissionBusiness().DeleteAssessmentPage(id);
            response.Success = result.ResultType == OperationResultType.Success;
            response.Message = result.Message;
            return JsonHelper.SerializeObject(response);
        }

        [System.Web.Mvc.HttpPost]
        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.ADE, Anonymity = Anonymous.Verified)]
        public string Status(int id)
        {
            var response = new PostFormResponse();
            var result = _adeBusiness.ChangeAssessmentStatus(id);
            response.Update(result);
            response.Data = result.AppendData;
            return JsonHelper.SerializeObject(response);
        }
        [System.Web.Mvc.HttpPost]
        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.ADE, Anonymity = Anonymous.Verified)]
        public string Unlock(int id)
        {
            var response = new PostFormResponse();
            var result = _adeBusiness.UnockAssessment(id, UserInfo);
            response.Update(result);
            response.Data = result.AppendData;
            return JsonHelper.SerializeObject(response);
        }

        public string DeleteParentReportCover(int assessmentId)
        {
            var response = new PostFormResponse();
            OperationResult result = new OperationResult(OperationResultType.Success);
            AssessmentEntity assessment = _adeBusiness.GetAssessment(assessmentId);
            string parentReportCoverUrl = SFConfig.UploadFile + assessment.ParentReportCoverPath;
            if (FileHelper.Delete(parentReportCoverUrl))
            {
                assessment.ParentReportCoverName = "";
                assessment.ParentReportCoverPath = "";
                result = _adeBusiness.UpdateAssessment(assessment);
            }
            else
            {
                result.ResultType = OperationResultType.Error;
                result.Message = "Delete parent report cover failed.";
            }
            response.Success = result.ResultType == OperationResultType.Success;
            response.Message = result.Message;
            return JsonHelper.SerializeObject(response);
        }

        #region Assign Report
        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.ADE, Anonymity = Anonymous.Verified)]
        public ActionResult AssignReport(int assessmentId)
        {
            List<AssessmentReportEntity> list = _adeBusiness.GetAssessmentReports(assessmentId);
            ViewBag.AssesmentReports = list;
            return View();
        }

        [System.Web.Mvc.HttpPost]
        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.ADE, Anonymity = Anonymous.Verified)]
        public string AssignReport(int assessmentId, List<int> communityReports, List<int> schoolReports,
            List<int> classReports, List<int> studentReports)
        {
            var response = new PostFormResponse();
            var assessmentReports = _adeBusiness.GetAssessmentReports(assessmentId);
            _adeBusiness.DeleteAssessmentReports(assessmentReports);
            List<AssessmentReportEntity> list = new List<AssessmentReportEntity>();
            if (communityReports != null)
            {
                foreach (var item in communityReports)
                {
                    AssessmentReportEntity entity = new AssessmentReportEntity();
                    entity.AssessmentId = assessmentId;
                    entity.ReportType = ReportTypeEnum.Community;
                    entity.Report = (ReportEnum)item;
                    list.Add(entity);
                }
            }
            if (schoolReports != null)
            {
                foreach (var item in schoolReports)
                {
                    AssessmentReportEntity entity = new AssessmentReportEntity();
                    entity.AssessmentId = assessmentId;
                    entity.ReportType = ReportTypeEnum.School;
                    entity.Report = (ReportEnum)item;
                    list.Add(entity);
                }
            }
            if (classReports != null)
            {
                foreach (var item in classReports)
                {
                    AssessmentReportEntity entity = new AssessmentReportEntity();
                    entity.AssessmentId = assessmentId;
                    entity.ReportType = ReportTypeEnum.Class;
                    entity.Report = (ReportEnum)item;
                    list.Add(entity);
                }
            }
            if (studentReports != null)
            {
                foreach (var item in studentReports)
                {
                    AssessmentReportEntity entity = new AssessmentReportEntity();
                    entity.AssessmentId = assessmentId;
                    entity.ReportType = ReportTypeEnum.Student;
                    entity.Report = (ReportEnum)item;
                    list.Add(entity);
                }
            }
            var result = _adeBusiness.InsertAssessmentReports(list);
            response.Update(result);
            response.Data = result.AppendData;
            return JsonHelper.SerializeObject(response);
        }

        #endregion

        #region Upload Legend

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.ADE, Anonymity = Anonymous.Verified)]
        public ActionResult UploadLegend(int assessmentId)
        {
            //var entity = _adeBusiness.GetAssessmentLegend(assessmentId, legendType);
            //if (entity == null)
            //{
            //    entity = new AssessmentLegendEntity();
            //    entity.AssessmentId = assessmentId;
            //    entity.LegendType = legendType;
            //}
            var entity = new AssessmentLegendEntity();
            entity.AssessmentId = assessmentId;
            var studentLegend = _adeBusiness.GetAssessmentLegend(assessmentId, LegendTypeEnum.StudentSummary);
            var classLegend = _adeBusiness.GetAssessmentLegend(assessmentId, LegendTypeEnum.ClassSummary);
            var parentLegend = _adeBusiness.GetAssessmentLegend(assessmentId, LegendTypeEnum.ParentReport);
            var engageLegend = _adeBusiness.GetAssessmentLegend(assessmentId, LegendTypeEnum.EngageUI);
            if (studentLegend == null)
                studentLegend = new AssessmentLegendEntity();
            if (classLegend == null)
                classLegend = new AssessmentLegendEntity();
            if (parentLegend == null)
                parentLegend = new AssessmentLegendEntity();
            if (engageLegend == null)
                engageLegend = new AssessmentLegendEntity();
            ViewBag.studentLegend = studentLegend;
            ViewBag.classLegend = classLegend;
            ViewBag.parentLegend = parentLegend;
            ViewBag.engageLegend = engageLegend;
            return View(entity);
        }
        [System.Web.Mvc.HttpPost]
        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.ADE, Anonymity = Anonymous.Verified)]
        public string UploadLegend(AssessmentLegendEntity assessmentLegend)
        {
            var response = new PostFormResponse();
            OperationResult result = new OperationResult(OperationResultType.Success);
            if (assessmentLegend.LegendType == LegendTypeEnum.ParentReport ||
                assessmentLegend.LegendType == LegendTypeEnum.EngageUI)
            {
                assessmentLegend.BlackWhiteFilePath = "";
                assessmentLegend.BlackWhiteFileName = "";
            }
            if (assessmentLegend.ID > 0)
            {
                result = _adeBusiness.UpdateAssessmentLegend(assessmentLegend);
            }
            else
            {
                result = _adeBusiness.InsertAssessmentLegend(assessmentLegend);
            }
            result.AppendData = assessmentLegend;
            response.Update(result);
            response.Data = result.AppendData;
            return JsonHelper.SerializeObject(response);
        }

        #endregion

        #region Assessment Score
        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.ADE, Anonymity = Anonymous.Verified)]
        public ActionResult ScoreIndex(int assessmentId)
        {
            ViewBag.assessmentId = assessmentId;
            return View();
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.ADE, Anonymity = Anonymous.Verified)]
        public string SearchScore(int assessmentId, string sort = "ID", string order = "DESC", int first = 0, int count = 10)
        {
            var total = 0;
            var searchCriteria = PredicateHelper.True<ScoreEntity>()
               .And(x => x.IsDeleted == false);
            searchCriteria = searchCriteria.And(x => x.AssessmentId == assessmentId);
            var list = _adeBusiness.GetScoreList(searchCriteria, out total, sort, order, first, count);
            var result = new { total = total, data = list };
            return JsonHelper.SerializeObject(result);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.ADE, Anonymity = Anonymous.Verified)]
        public ActionResult NewScore()
        {
            int assessmentId = Convert.ToInt32(Request.Params["assessmentId"]);
            AssessmentEntity assessmentEntity = _adeBusiness.GetAssessment(assessmentId);
            if (assessmentEntity == null) return new EmptyResult();
            var model = new ScoreModel();
            model.AssessmentId = assessmentId;
            Object measureList = _adeBusiness.GetMeasureByAssessmentId(Convert.ToInt32(assessmentId));
            ViewBag.JsonMeasures = JsonHelper.SerializeObject(measureList);
            model.ApplyToWave = new List<Wave>();
            model.ApplyToWave.Add(Wave.BOY);
            model.ApplyToWave.Add(Wave.EOY);
            model.ApplyToWave.Add(Wave.MOY);
            if (assessmentEntity.Type == AssessmentType.Cpalls)
            {
                ViewBag.Benchmarks = JsonHelper.SerializeObject(_adeBusiness.GetBenchmarksForSelect(assessmentId));
            }
            return View(model);
        }

        private class CompareCutoffScore : IEqualityComparer<CutOffScoreEntity>
        {
            public bool Equals(CutOffScoreEntity x, CutOffScoreEntity y)
            {
                return x.FromYear == y.FromYear && x.FromMonth == y.FromMonth
                       && x.ToYear == y.ToYear && x.ToMonth == y.ToMonth
                       && x.Wave == y.Wave
                       && x.BenchmarkId == y.BenchmarkId
                       && x.LowerScore == y.LowerScore
                       && x.HigherScore == y.HigherScore;
            }
            public int GetHashCode(CutOffScoreEntity obj)
            {
                var score = new { obj.FromYear, obj.FromMonth, obj.ToYear, obj.ToMonth, obj.Wave, obj.CutOffScore, obj.BenchmarkId, obj.LowerScore, obj.HigherScore };
                return score.GetHashCode();
            }
        }

        private class CompareCoefficient : IEqualityComparer<ScoreMeasureOrDefineCoefficientsEntity>
        {
            public bool Equals(ScoreMeasureOrDefineCoefficientsEntity x, ScoreMeasureOrDefineCoefficientsEntity y)
            {
                return x.Wave == y.Wave && x.Measure == y.Measure && x.Coefficient == y.Coefficient;
            }
            public int GetHashCode(ScoreMeasureOrDefineCoefficientsEntity obj)
            {
                var score = new { obj.Wave, obj.Measure, obj.Coefficient };
                return score.GetHashCode();
            }
        }

        private class CompareBand : IEqualityComparer<ScoreAgeOrWaveBandsEntity>
        {
            public bool Equals(ScoreAgeOrWaveBandsEntity x, ScoreAgeOrWaveBandsEntity y)
            {
                return x.Wave == y.Wave && x.AgeMin == y.AgeMin && x.AgeMax == y.AgeMax
                       && x.AgeOrWaveMean == y.AgeOrWaveMean && x.AgeOrWave == y.AgeOrWave;
            }
            public int GetHashCode(ScoreAgeOrWaveBandsEntity obj)
            {
                var score = new { obj.Wave, obj.AgeMin, obj.AgeMax, obj.AgeOrWaveMean, obj.AgeOrWave };
                return score.GetHashCode();
            }
        }

        [ValidateInput(false)]
        [System.Web.Mvc.HttpPost]
        [ValidateAntiForgeryToken]
        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.ADE, Anonymity = Anonymous.Verified)]
        public string NewScore(int id, ScoreModel model, string coefficients = null, string bands = null, string scores = null)
        {
            var response = new PostFormResponse() { Success = ModelState.IsValid };
            if (model.Description == null)
            {
                model.Description = "";
            }
            if (scores != null)
            {
                var scoreEntities = JsonHelper.DeserializeObject<List<CutOffScoreEntity>>(scores);
                model.CutOffScores = scoreEntities.Distinct(new CompareCutoffScore()).ToList();
            }
            if (coefficients != null)
            {
                var coefficientEntitis = JsonHelper.DeserializeObject<List<ScoreMeasureOrDefineCoefficientsEntity>>(coefficients);
                model.ScoreMeasureOrDefineCoefficients = coefficientEntitis.Distinct(new CompareCoefficient()).ToList();
            }
            if (bands != null)
            {
                var bandEntitis = JsonHelper.DeserializeObject<List<ScoreAgeOrWaveBandsEntity>>(bands);
                model.ScoreAgeOrWave = bandEntitis.Distinct(new CompareBand()).ToList();
            }

            var result = _adeBusiness.InsertScore(model);
            response.Success = result.ResultType == OperationResultType.Success;
            response.Data = model;
            response.Message = result.Message;
            response.ModelState = ModelState;
            return JsonHelper.SerializeObject(response);
        }


        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.ADE, Anonymity = Anonymous.Verified)]
        public ActionResult EditScore(int id)
        {
            var model = _adeBusiness.GetScore(id);
            AssessmentEntity assessmentEntity = _adeBusiness.GetAssessment(model.AssessmentId);
            foreach (var scoreAgeOrWaveBandsEntity in model.ScoreAgeOrWave)
            {
                scoreAgeOrWaveBandsEntity.Score = new ScoreEntity();
            }
            foreach (var scoreMeasureOrDefineCoefficients in model.ScoreMeasureOrDefineCoefficients)
            {
                scoreMeasureOrDefineCoefficients.MeasureObject = new MeasureEntity();
            }
            Object measureList = _adeBusiness.GetMeasureByAssessmentId(Convert.ToInt32(model.AssessmentId));
            ViewBag.JsonMeasures = JsonHelper.SerializeObject(measureList);
            ViewBag.JsonBands = JsonHelper.SerializeObject(model.ScoreAgeOrWave);
            ViewBag.JsonCoefficients = JsonHelper.SerializeObject(model.ScoreMeasureOrDefineCoefficients);
            ViewBag.JsonCutoffScores = JsonHelper.SerializeObject(_adeBusiness.GetCutOffScores<ScoreEntity>(id));
            model.ApplyToWave = new List<Wave>();
            model.ApplyToWave.Add(Wave.BOY);
            model.ApplyToWave.Add(Wave.EOY);
            model.ApplyToWave.Add(Wave.MOY);
            if (assessmentEntity.Type == AssessmentType.Cpalls)
            {
                ViewBag.Benchmarks = JsonHelper.SerializeObject(_adeBusiness.GetBenchmarksForSelect(model.AssessmentId));
            }
            return View(model);
        }

        [ValidateInput(false)]
        [System.Web.Mvc.HttpPost]
        [ValidateAntiForgeryToken]
        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.ADE, Anonymity = Anonymous.Verified)]
        public string EditScore(int id, ScoreModel model, string coefficients = null, string bands = null, string scores = null)
        {
            var response = new PostFormResponse() { Success = ModelState.IsValid };
            if (model.Description == null)
            {
                model.Description = "";
            }
            if (scores != null)
            {
                var scoreEntities = JsonHelper.DeserializeObject<List<CutOffScoreEntity>>(scores);
                model.CutOffScores = scoreEntities.Distinct(new CompareCutoffScore()).ToList();
            }
            if (coefficients != null)
            {
                var coefficientEntitis = JsonHelper.DeserializeObject<List<ScoreMeasureOrDefineCoefficientsEntity>>(coefficients);
                model.ScoreMeasureOrDefineCoefficients = coefficientEntitis.Distinct(new CompareCoefficient()).ToList();
            }
            if (bands != null)
            {
                var bandEntitis = JsonHelper.DeserializeObject<List<ScoreAgeOrWaveBandsEntity>>(bands);
                model.ScoreAgeOrWave = bandEntitis.Distinct(new CompareBand()).ToList();
            }

            var result = _adeBusiness.EditSaveScore(model);
            response.Success = result.ResultType == OperationResultType.Success;
            response.Data = model;
            response.Message = result.Message;
            response.ModelState = ModelState;
            return JsonHelper.SerializeObject(response);
        }


        [System.Web.Mvc.HttpPost]
        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.ADE, Anonymity = Anonymous.Verified)]
        public string DeleteScore(int id)
        {
            var response = new PostFormResponse();
            var result = _adeBusiness.DeleteScore(id);
            //if (result.ResultType == OperationResultType.Success)
            //    new PermissionBusiness().DeleteAssessmentPage(id);
            response.Success = result.ResultType == OperationResultType.Success;
            response.Message = result.Message;
            return JsonHelper.SerializeObject(response);
        }

        public decimal TestFinalScores(int said, int scoreId)
        {
            return _adeBusiness.GetFinalResult(said, scoreId);
        }

        public ActionResult FinalScore(int assessmentId, int scoreId)
        {
            ViewBag.AssessmentId = assessmentId;
            ViewBag.ScoreId = scoreId;
            return View();
        }

        public string GetFinalScore(int assessmentId, int scoreId, int studentId, int wave)
        {
            int saId = _adeBusiness.GetOneSaid(studentId, assessmentId, wave);
            var response = new PostFormResponse();
            var finalResult = _adeBusiness.GetFinalResult(saId, scoreId);
            response.Data = finalResult == 0 ? "Score could not be calculated" : finalResult.ToString("N2");
            response.Success = true;
            return JsonHelper.SerializeObject(response);
        }
        public string GetStudentSelectList(string keyword)
        {
            var studentList = _studentBusiness.GetStudentList(UserInfo);
            return JsonHelper.SerializeObject(studentList);
        }

        public ActionResult ScoreActivities(int scoreId)
        {
            ScoreEntity entity = _adeBusiness.GetScoreEntity(scoreId);
            AssessmentEntity assessment = _adeBusiness.GetAssessment(entity.AssessmentId);
            ViewBag.AssessmentName = assessment.Name;
            List<AdeLinkEntity> links = _adeBusiness.GetLinks<ScoreEntity>(scoreId);
            ViewBag.Links = JsonHelper.SerializeObject(links.Where(e => e.LinkType == 0));
            ViewBag.ScoreId = scoreId;
            return View();
        }

        [System.Web.Mvc.HttpPost]
        [ValidateAntiForgeryToken]
        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.ADE, Anonymity = Anonymous.Verified)]
        public string ScoreActivities(int ScoreId, string Links)
        {
            var response = new PostFormResponse();
            List<AdeLinkEntity> list = new List<AdeLinkEntity>();
            if (Links.Trim() != string.Empty)
            {
                string[] tmpLinks = Links.Split(';');
                if (tmpLinks.Length > 0)
                {
                    foreach (string s in tmpLinks)
                    {
                        if (s.Trim() != string.Empty)
                        {
                            string[] tmpS = s.Split('|');
                            if (tmpS.Length == 5)
                            {
                                if (tmpS[0].Trim() != string.Empty)
                                {
                                    list.Add(new AdeLinkEntity()
                                    {
                                        Link = tmpS[0],
                                        LinkType = 0,
                                        DisplayText = tmpS[1],
                                        Status = EntityStatus.Active,
                                        MeasureWave1 = tmpS[2] == "true",
                                        MeasureWave2 = tmpS[3] == "true",
                                        MeasureWave3 = tmpS[4] == "true"
                                    });
                                }
                            }
                        }
                    }
                }
            }
            if (list.Count > 0)
            {
                var result = _adeBusiness.UpdateLinks<ScoreEntity>(ScoreId, list);
                response.Update(result);
            }
            else
                response.Success = true;

            return JsonHelper.SerializeObject(response);
        }
        #endregion
    }
}


