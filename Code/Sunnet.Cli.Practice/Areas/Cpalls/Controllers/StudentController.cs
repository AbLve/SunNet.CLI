using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sunnet.Cli.Business.Ade.Models;
using Sunnet.Cli.Business.Classes;
using Sunnet.Cli.Business.Communities;
using Sunnet.Cli.Business.Cpalls;
using Sunnet.Cli.Business.Schools;
using Sunnet.Cli.UIBase.Models;
using Sunnet.Cli.Business.Common;
using Sunnet.Framework.Extensions;
using Sunnet.Cli.Core.Users.Enums;
using Sunnet.Cli.Core.Students.Entities;
using Sunnet.Cli.Business.Cpalls.Models;
using Sunnet.Cli.Business.Ade;
using System.Linq.Expressions;
using Sunnet.Cli.Business.Practices;
using Sunnet.Cli.Business.Practices.Models;
using Sunnet.Framework.Helpers;
using Sunnet.Cli.Core.Cpalls;
using Sunnet.Framework.Core.Tool;
using Sunnet.Cli.Business.Students;
using Sunnet.Cli.Core.Ade.Entities;
using Sunnet.Cli.Core.Ade;
using Sunnet.Cli.Core.Common.Enums;
using Sunnet.Cli.Core.Cpalls.Entities;
using Sunnet.Cli.Core.Practices.Entites;
using Sunnet.Cli.Practice.Areas.Report.Models;
using Sunnet.Cli.Practice.Controllers;
using Sunnet.Cli.UIBase.Filters;
using Sunnet.Framework.Permission;

namespace Sunnet.Cli.Practice.Areas.Cpalls.Controllers
{
    public class StudentController : BaseController
    {
        //
        // GET: /Cpalls/Student/
         CpallsBusiness _cpallsBusiness;
        AdeBusiness _adeBusiness;
        private StudentBusiness _studentBusiness;
        private PracticeBusiness _practiceBusiness;  
        public StudentController()
        { 
            _cpallsBusiness = new CpallsBusiness(AdeUnitWorkContext);
            _adeBusiness = new AdeBusiness(AdeUnitWorkContext);
            _studentBusiness = new StudentBusiness();
            _practiceBusiness = new PracticeBusiness(PracticeUnitWorkContext);
        }

        //绿： 高于分数线的
        //黄： 4岁以下低于分数线的
        //红： 4岁以上低于分数线的
        //蓝： 年龄在及格线设置年龄范围之外
        //白： Measure没有及格线   
        // GET: /Cpalls/Class/
        [CLIUrlAuthorizeAttribute(Account = Authority.AssessmentPracticeArea, PageId = PagesModel.Practice, Anonymity = Anonymous.Verified, Parameter = "assessmentId")]
        public ActionResult Index(int assessmentId, int year = 0, int wave = 0,string measures = "")
        {
            BaseAssessmentModel assessment = null;
            if ((int)UserInfo.Role > (int)Role.Teacher)
                return new RedirectResult("/error/nonauthorized");
            if (assessmentId < 1)
            {
                return RedirectToAction("Index", "Home", new { Area = "", showMessage = "assessment_unavaiable" });
            }
            else
            {
                assessment = _adeBusiness.GetBaseAssessmentModel(assessmentId);
                if (assessment == null)
                    return RedirectToAction("Index", "Home", new { Area = "", showMessage = "assessment_unavaiable" });
            }

            var assessmentLegend = _adeBusiness.GetAssessmentLegend(assessmentId, LegendTypeEnum.EngageUI) ??
                                   new AssessmentLegendEntity();
            ViewBag.AssessmentLegend = assessmentLegend;

            if (year == 0)
            {
                year = CommonAgent.Year;
            }
            if (wave == 0)
            {
                //获取用户上一次记录过的Wave
                //var waveLog = _adeBusiness.GetUserWaveLog(UserInfo.ID, assessmentId);
                //if (waveLog != null)
                //{
                //    wave = (int)waveLog.WaveValue;
                //}
                //else
                //{
                //    wave = (int)CommonAgent.CurrentWave;
                //}
                //wave = (int)CommonAgent.CurrentWave;//2017.05.08 Ticket 2659 改成 wave1
                wave = (int) Wave.BOY;
            }
            ViewBag.Year = year;
            ViewBag.Wave = wave;
            ViewBag.AssessmentId = assessmentId;
            ViewBag.AssessmentName = assessment.Name;
            ViewBag.AssessmentReports = _adeBusiness.GetAssessmentReports(assessmentId, ReportTypeEnum.Class);
            ViewBag.AssessmentReportStudents = _adeBusiness.GetAssessmentReports(assessmentId, ReportTypeEnum.Student);
            ViewBag.DisplayPercentileRank = assessment.DisplayPercentileRank;

            // 是否可以进行assessment David 01/15/2015 不限制，直接设置为True
            ViewBag.Start = true;
            if (year == CommonAgent.Year)
            {
                ViewBag.Start = true;
            }
             
            // 判断处理是否有双语版本
            ViewBag.Language = assessment.Language;
            ViewBag.HasAnotherVersion = false;
            if (assessment.TheOtherId > 0)
            {
                ViewBag.HasAnotherVersion = true;
                ViewBag.OtherVersion = AssessmentLanguageHelper.GetButtonText(assessment.TheOtherLang);
                ViewBag.OtherAssessmentId = assessment.TheOtherId;
            }
            // 当前年度，数据初始化
            if (CommonAgent.IsCurrentSchoolYear(year))
            {
                _cpallsBusiness.InitializeStudentAssessmentDate(assessmentId, (Wave)wave, UserInfo);
            }
            // 绑定年度与 wave下拉框数据
            List<SelectListItem> yearList = new SelectList(CommonAgent.GetYears(), "ID", "Name").ToList();
            yearList.ForEach(r => r.Selected = false);
            SelectListItem tmpLI = yearList.Find(r => r.Value == year.ToString());
            if (tmpLI != null)
                tmpLI.Selected = true;
            ViewBag.YearOptions = yearList;

            List<SelectListItem> waveList = Wave.BOY.ToSelectList().ToList();
            tmpLI = waveList.Find(r => r.Value == wave.ToString());
            if (tmpLI != null)
                tmpLI.Selected = true;
            ViewBag.WaveOptions = waveList;
            ViewBag.HaveMeasure = false;
            List<MeasureHeaderModel> MeasureList;
            List<MeasureHeaderModel> ParentMeasureList;

            _cpallsBusiness.BuilderHeader(assessmentId
                , year, (Wave)wave
                , out MeasureList, out ParentMeasureList);

            //去除 Hide的measures
            var HasHide = false;
            var findShownMeasures = _cpallsBusiness.GetUserShownMeasure(assessmentId, UserInfo.ID, (Wave)wave, year);
            if (findShownMeasures != null)
            {
                var findmeasures = JsonHelper.DeserializeObject<Dictionary<string, List<int>>>(findShownMeasures.Measures);
                if (findmeasures[wave.ToString()].Count > 0)
                {
                    foreach (var measured in MeasureList)
                    {
                        if (!findmeasures[wave.ToString()].Contains(measured.MeasureId))
                        {
                            HasHide = true;
                            break;
                        }
                    }

                    MeasureList = MeasureList.Where(o => findmeasures[wave.ToString()].Contains(o.MeasureId)).ToList();
                    ParentMeasureList = ParentMeasureList.Where(o => findmeasures[wave.ToString()].Contains(o.MeasureId)).ToList();
                }

            }
            foreach (var measureHeaderModel in ParentMeasureList)
            {
                measureHeaderModel.Subs = MeasureList.Count(c => c.ParentId == measureHeaderModel.MeasureId);
            }
            ViewBag.HasHide = HasHide;

            ViewBag.Parents = JsonHelper.SerializeObject(ParentMeasureList.Select(x => new { x.MeasureId, x.Subs, x.Name }));
            ViewBag.Measures = JsonHelper.SerializeObject(MeasureList);
            if (MeasureList != null && MeasureList.Count > 0)
            {
                ViewBag.HaveMeasure = true;
                ViewBag.MeasureList = MeasureList;
                ViewBag.ParentMeasure = ParentMeasureList;
            }

            bool cpallsOffline = false;
            bool keepSelectMeasure = false;
            if (CommonAgent.IsCurrentSchoolYear(year))
            {
                cpallsOffline = CheckAssessmentPermission(assessmentId, Authority.Offline);
                if (measures.Trim() != string.Empty)
                    keepSelectMeasure = true;
            }
            ViewBag.KeepSelectMeasure = keepSelectMeasure;
              ViewBag.OfflineUrl = string.Format("/Offline/Index/Preparing?assessmentId={0}&language={1}&year={2}&wave={3}",
                    assessmentId, (int)assessment.Language, year, wave);


       
            ViewBag.CpallsOffline = cpallsOffline;
            return View();
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.AssessmentPracticeArea, PageId = PagesModel.Practice, Anonymity = Anonymous.Verified, Parameter = "assessmentId")]
        public string Search(int assessmentId, AssessmentLanguage language, int year, Wave wave, 
            bool isDisplayRanks = false, string sort = "Name", string order = "Asc",
            int first = 0, int count = 10, int sortMeasureId = 0)
        {
            int total = 0;
            IList<DemoStudentModel> list = new List<DemoStudentModel>();

            string schoolYear = year.ToSchoolYearString();

            Expression<Func<DemoStudentEntity, bool>> studentCondition = PredicateHelper.True<DemoStudentEntity>();
            studentCondition = studentCondition.And(c => c.AssessmentId == assessmentId);
            studentCondition = studentCondition.And(r => r.Status == EntityStatus.Active);
            studentCondition = studentCondition.And(r => r.AssessmentLanguage == (StudentAssessmentLanguage)((byte)language)
                                                         || r.AssessmentLanguage == StudentAssessmentLanguage.Bilingual);

            list = _practiceBusiness.GetStudentList(UserInfo, assessmentId, studentCondition, wave, year,
                sort, order, first, count, out total, sortMeasureId,isDisplayRanks); 

            var result = new { total = total, data = list };
            return JsonHelper.SerializeObject(result);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.Practice, Anonymity = Anonymous.Verified)]
        public ActionResult HideMeasures(int assessmentId, int year = 0, Wave wave = Wave.BOY)
        {
            List<MeasureHeaderModel> measures = new List<MeasureHeaderModel>();
            List<MeasureHeaderModel> parentMeasures = new List<MeasureHeaderModel>();
            _cpallsBusiness.BuilderHeader(assessmentId, year, wave, out measures, out parentMeasures, false);
            var findShownMeasures = _cpallsBusiness.GetUserShownMeasure(assessmentId, UserInfo.ID, wave, year);
            if (findShownMeasures != null)
            {
                ViewBag.shownMeasures = findShownMeasures.Measures;
            }
            else
            {
                ViewBag.shownMeasures = "all";
            }

            var assessment = _adeBusiness.GetAssessment(assessmentId);
            ViewBag.language = assessment.Language;
            var groups = MeasureGroup.GetGroupJson(measures, parentMeasures);
            ViewBag.MeasureJson = JsonHelper.SerializeObject(groups);
             
            ViewBag.assessmentId = assessmentId;
            ViewBag.year = year;
            ViewBag.wave = (int)wave;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.Practice, Anonymity = Anonymous.Verified)]
        public string SaveShownMeasures(int assessmentId, int year, Wave wave, string measures)
        {
            var response = new PostFormResponse();
            OperationResult result = new OperationResult(OperationResultType.Success);
            var findEntity = _cpallsBusiness.GetUserShownMeasure(assessmentId, UserInfo.ID, wave, year);
            if (findEntity != null)
            {
                if (measures == "{\"1\":[],\"2\":[],\"3\":[]}")
                {
                    result = _cpallsBusiness.DeleteUserShownMeasures(findEntity.ID);
                }
                else
                {
                    findEntity.Measures = measures;
                    result = _cpallsBusiness.SaveUserShownMeasures(findEntity);
                }
            }
            else
            {
                UserShownMeasuresEntity entity = new UserShownMeasuresEntity();
                entity.UserId = UserInfo.ID;
                entity.AssessmentId = assessmentId;
                entity.Wave = wave;
                entity.Year = year;
                entity.Measures = measures;
                result = _cpallsBusiness.SaveUserShownMeasures(entity);
            }

            response.Success = result.ResultType == OperationResultType.Success;
            response.Message = result.Message;
            return JsonHelper.SerializeObject(response);
        }

        public int IsExistMobileAudio(string measureIds)
        {
            var listMeasureId = measureIds.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList();
            int audios = _adeBusiness.GetIsExistMobileAudio(listMeasureId);
            return audios;
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.AssessmentPracticeArea, PageId = PagesModel.Practice, Anonymity = Anonymous.Verified, Parameter = "assessmentId")]
        public string PlayMeasure(int studentId, int assessmentId, int studentAssessmentId, Wave wave, int year,  string measureIds)
        {
            var response = new PostFormResponse();
            //  DemoStudentEntity student = _practiceBusiness.GetStudent(studentId);

            //    CpallsStudentModel student = _studentBusiness.GetStudentModel(studentId, UserInfo);
            var student = _practiceBusiness.GetStudentModel(studentId);
            if (student == null && studentId == 0)
            {
                return "";
            }
            var chkRes = _practiceBusiness.CheckStudentMeasure(UserInfo, assessmentId, studentId, year.ToSchoolYearString(), year, wave, measureIds.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList());
            if (chkRes.ResultType == OperationResultType.Error)
            {
                response.Update(chkRes);
                return JsonHelper.SerializeObject(response);
            }

            int newStudentAssessmentId=0;
       
            OperationResult operationResult = _practiceBusiness.PlayMeasure(student, assessmentId, measureIds.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Select(int.Parse), studentAssessmentId, year, wave,
                UserInfo, out newStudentAssessmentId);
             response.Update(operationResult);
            if (response.Success)
            {
                var listMeasureId = measureIds.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList();
                ExecCpallsAssessmentModel assessment = _practiceBusiness.GetAssessment(newStudentAssessmentId, listMeasureId,  UserInfo);
                bool showLaunchPage = assessment.Measures.Any(measureEntity => measureEntity.ShowLaunchPage && measureEntity.OrderType == OrderType.Sequenced
                                                                && measureEntity.Status == CpallsStatus.Initialised);
                if (!showLaunchPage)
                {
                    response.OtherMsg = "NotShowLaunchPage";
                    assessment.Measures =
                        assessment.Measures.Where(x => x.Status == CpallsStatus.Initialised || x.Status == CpallsStatus.Paused)
                            .ToList();
                    var assessmentModel = _adeBusiness.GetBaseAssessmentModel(assessment.AssessmentId);
                    if (assessmentModel == null)
                    {
                        response.OtherMsg = "";
                        response.Data = "/Dashboard/index";
                        return JsonHelper.SerializeObject(response);
                    }

                    Response.Cache.SetCacheability(HttpCacheability.NoCache);
                    Response.ExpiresAbsolute = DateTime.Now.AddDays(-1);
                    Response.Cache.SetExpires(DateTime.Now.AddDays(-1));
                    Response.CacheControl = "no-cache";
                    Response.Expires = 0;
                    if (assessment.StudentId == 0)
                    {
                        assessment.Student = new ExecCpallsStudentModel()
                        {
                            ID = 0,
                            Name = ViewTextHelper.DemoStudentFirstName + " " + ViewTextHelper.DemoStudentLastName
                        };
                    }
                   
                    response.Data = assessment;
                }
                else
                {
                    response.OtherMsg = "";
                    var executeUrl = "/Cpalls/Execute/Index";
                    response.Data = string.Format("{0}?id={1}&measures={2}&wave={3}",
                        executeUrl, newStudentAssessmentId, measureIds,  (int)wave);
                }

            }
            return JsonHelper.SerializeObject(response);
        }

        ///Cpalls/Execute/Index(int id, string measures, int classId, bool calc = false)
        [CLIUrlAuthorizeAttribute(Account = Authority.AssessmentPracticeArea, PageId = PagesModel.Practice, Anonymity = Anonymous.Verified, Parameter = "assessmentId")]
        public string CheckStudentMeasure(int studentId, int assessmentId, int studentAssessmentId, Wave wave, int year, int classId, string measureIds)
        {
            var response = new PostFormResponse();
            CpallsStudentModel student = _studentBusiness.GetStudentModel(studentId, UserInfo);
            if (student == null && studentId == 0)
            {
                student = new CpallsStudentModel()
                {
                    BirthDate = CommonAgent.GetStartDateForAge(year).AddYears(-4),
                    CommunityId = 0,
                    FirstName = ViewTextHelper.DemoStudentFirstName,
                    ID = 0,
                    LastName = ViewTextHelper.DemoStudentLastName,
                    SchoolId = 0,
                    SchoolName = ViewTextHelper.DemoSchoolName
                };
            }

            var chkRes = _practiceBusiness.CheckStudentMeasure(UserInfo, assessmentId, studentId, year.ToSchoolYearString(), year, wave, measureIds.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList());
            response.Update(chkRes);
            return JsonHelper.SerializeObject(response);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.AssessmentPracticeArea, PageId = PagesModel.Practice, Anonymity = Anonymous.Verified, Parameter = "assessmentId")]
        public string LockMeasure(int studentId, int measureId, int assessmentId, int studentAssessmentId, int schoolId, Wave wave, int year,  CpallsStatus status)
        {
            var response = new PostFormResponse();
            DemoStudentModel student = _practiceBusiness.GetStudentModel(studentId);
            var measureIds = new List<int>() { measureId };
            var chkRes = _practiceBusiness.CheckStudentMeasure(UserInfo, assessmentId, studentId,year.ToSchoolYearString(), year, wave, measureIds);
            if (chkRes.ResultType == OperationResultType.Error)
            {
                response.Update(chkRes);
                return JsonHelper.SerializeObject(response);
            }

            if (student != null)
            {
                response.Update(_practiceBusiness.LockMeasure(student, measureId, assessmentId, studentAssessmentId,wave, year, UserInfo, status));
            }
            else
            {
                response.Success = false;
                response.Message = "Student is required.";
            }
            return JsonHelper.SerializeObject(response);
        }
    }
}