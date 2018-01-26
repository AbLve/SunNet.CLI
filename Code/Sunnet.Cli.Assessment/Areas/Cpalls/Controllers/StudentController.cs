using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Sunnet.Cli.Assessment.Controllers;
using Sunnet.Cli.Business.Ade.Models;
using Sunnet.Cli.Business.Classes;
using Sunnet.Cli.Business.Classes.Models;
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
using Sunnet.Framework.Helpers;
using Sunnet.Cli.Core.Cpalls.Entities;
using Sunnet.Cli.Core.Classes.Entites;
using Sunnet.Cli.Core.Cpalls;
using Sunnet.Framework.Core.Tool;
using Sunnet.Cli.Business.Students;
using Sunnet.Cli.Core.Ade.Entities;
using Sunnet.Cli.Core.Ade;
using Sunnet.Cli.Core.Common.Enums;
using Sunnet.Cli.UIBase.Filters;
using Sunnet.Framework.Permission;
using Sunnet.Framework;
using Newtonsoft.Json;

namespace Sunnet.Cli.Assessment.Areas.Cpalls.Controllers
{
    public class StudentController : BaseController
    {
        //
        // GET: /Cpalls/Student/
        ClassBusiness _classBusiness;
        CpallsBusiness _cpallsBusiness;
        AdeBusiness _adeBusiness;
        private StudentBusiness _studentBusiness;
        private CommunityBusiness _communityBusiness;
        SchoolBusiness _schoolBusiness;
        public StudentController()
        {
            _classBusiness = new ClassBusiness();
            _cpallsBusiness = new CpallsBusiness(AdeUnitWorkContext);
            _adeBusiness = new AdeBusiness(AdeUnitWorkContext);
            _studentBusiness = new StudentBusiness();
            _schoolBusiness = new SchoolBusiness();
            _communityBusiness = new CommunityBusiness();
        }

        //绿： 高于分数线的
        //黄： 4岁以下低于分数线的
        //红： 4岁以上低于分数线的
        //蓝： 年龄在及格线设置年龄范围之外
        //白： Measure没有及格线   
        // GET: /Cpalls/Class/
        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.CPALLS, Anonymity = Anonymous.Verified, Parameter = "assessmentId")]
        public ActionResult Index(int classId, int assessmentId, int year = 0, int wave = 0, int schoolId = 0, string measures = "")
        {
            BaseAssessmentModel assessment = null;
            if ((int)UserInfo.Role > (int)Role.Teacher)
                return new RedirectResult("/error/nonauthorized");
            if (assessmentId < 1)
            {
                return RedirectToAction("Index", "Dashboard", new { Area = "", showMessage = "assessment_unavaiable" });
            }
            else
            {
                assessment = _adeBusiness.GetBaseAssessmentModel(assessmentId);
                if (assessment == null)
                    return RedirectToAction("Index", "Dashboard", new { Area = "", showMessage = "assessment_unavaiable" });
            }

            var assessmentLegend = _adeBusiness.GetAssessmentLegend(assessmentId, LegendTypeEnum.EngageUI);
            if (assessmentLegend == null)
                assessmentLegend = new AssessmentLegendEntity();
            ViewBag.AssessmentLegend = assessmentLegend;

            ViewBag.ShowSchoolback = true;
            var teachers = new List<string>();
            if (UserInfo.Role == Role.Teacher)
            {
                ViewBag.ShowSchoolback = false;
                teachers.Add(string.Format("{0} {1}", UserInfo.FirstName, UserInfo.LastName));
            }
            else
                teachers = _classBusiness.GetTeachers(classId);
            ViewBag.Teacher = teachers;

            if (year == 0)
            {
                year = CommonAgent.Year;
            }
            if (wave == 0)
            {
                //获取用户上一次记录过的Wave
                var waveLog = _adeBusiness.GetUserWaveLog(UserInfo.ID, assessmentId);
                if (waveLog != null)
                {
                    wave = (int)waveLog.WaveValue;
                }
                else
                {
                    wave = (int)CommonAgent.CurrentWave;
                }
            }
            ViewBag.Year = year;
            ViewBag.Wave = wave;
            ViewBag.ClassId = classId;
            ViewBag.SchoolId = schoolId;
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

            //验证class

            ClassModel classModel = _classBusiness.GetClassForCpalls(classId);
            if ((classModel == null || classModel.ID == 0) && classId == 0)
            {
                CpallsSchoolModel schoolModel = _schoolBusiness.GetCpallsSchoolModel(schoolId);
                classModel = new ClassModel()
                {
                    ID = 0,
                    ClassName = ViewTextHelper.DemoClassName,
                    School = schoolModel,
                    ClassLevel = 0
                };
                if (schoolModel == null || schoolModel.ID == 0)
                    schoolId = 0;
            }
            var classlevlIds = new List<int>();
            if ((classModel == null || classModel.ID == 0) && classId != 0)
                return new RedirectResult(DomainHelper.AssessmentDomain.ToString());
            //if (classModel != null && classModel.School != null)
            //{
            //    //犹豫性能问题，暂时去掉该条件查询
            //    //  classlevlIds = _communityBusiness.GetAssignedClassLevels(assessmentId, classModel.School.ID);
            //    //if (!(classlevlIds.Contains(classModel.ClassLevel) || classlevlIds.Contains(0) || classModel.ClassId==0))
            //    //    return new RedirectResult(DomainHelper.AssessmentDomain.ToString());
            //}
            // schoolId = classModel.School.ID;
            ViewBag.ClassModel = classModel;
            if (classId > 0)
                ViewBag.SchoolId = classModel.School.ID;

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

            // 绑定Class 下拉框

            List<SelectListItem> classList = new List<SelectListItem>();
            //if (!classlevlIds.Contains(0))//All and missing
            //{
            //    classList = _classBusiness.GetClassSelectList(UserInfo,
            //   r =>r.SchoolYear == CommonAgent.SchoolYear && r.SchoolId == schoolId && r.Status == EntityStatus.Active && classlevlIds.Contains(r.Classlevel))
            //   .ToSelectList();
            //}
            //else
            {
                classList = _classBusiness.GetClassSelectListForCache(UserInfo, classModel.School.ID).ToSelectList();
            }
            tmpLI = classList.Find(r => r.Value == classId.ToString());
            if (tmpLI != null)
                tmpLI.Selected = true;
            classList.AddDefaultItem(ViewTextHelper.DemoClassName, 0, 0);

            ViewBag.ClassOptions = classList;

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

            if (classId > 0)
                ViewBag.OfflineUrl = string.Format("/Offline/Index/Preparing?assessmentId={0}&language={1}&year={2}&wave={3}&classId={4}",
                    assessmentId, (int)assessment.Language, year, wave, classId);

            bool cpallsOffline = false;
            bool keepSelectMeasure = false;
            if (CommonAgent.IsCurrentSchoolYear(year))
            {
                cpallsOffline = CheckAssessmentPermission(assessmentId, Authority.Offline);
                if (measures.Trim() != string.Empty)
                    keepSelectMeasure = true;
            }
            ViewBag.CpallsOffline = cpallsOffline;
            ViewBag.KeepSelectMeasure = keepSelectMeasure;

            return View();
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.CPALLS, Anonymity = Anonymous.Verified, Parameter = "assessmentId")]
        public string Search(int schoolId, int classId, int assessmentId, AssessmentLanguage language, int year, Wave wave, string firstname = "", string lastname = "",
            bool isDisplayRanks = false, string sort = "Name", string order = "Asc",
            int first = 0, int count = 10, int sortMeasureId = 0)
        {
            int total = 0;
            List<CpallsStudentModel> list = new List<CpallsStudentModel>();

            string schoolYear = year.ToSchoolYearString();

            Expression<Func<StudentEntity, bool>> studentCondition = PredicateHelper.True<StudentEntity>();
            studentCondition = studentCondition.And(r => r.Classes.Where(e => e.IsDeleted == false).Select(c => c.ID).Contains(classId));
            studentCondition = studentCondition.And(r => r.Status == EntityStatus.Active);
            studentCondition = studentCondition.And(r => r.AssessmentLanguage == (StudentAssessmentLanguage)((byte)language)
                                                         || r.AssessmentLanguage == StudentAssessmentLanguage.Bilingual);
            if (firstname != string.Empty)
                studentCondition = studentCondition.And(r => r.FirstName.Contains(firstname));
            if (lastname != string.Empty)
                studentCondition = studentCondition.And(r => r.LastName.Contains(lastname));


            list = _cpallsBusiness.GetStudentList(schoolId, UserInfo, assessmentId, wave, year, studentCondition,
                sort, order, first, count, out total, sortMeasureId,isDisplayRanks);

            if (classId < 1)
            {
                //list.Insert(0, new CpallsStudentModel()
                //{
                //    BirthDate = CommonAgent.GetStartDateForAge(year).AddYears(-4),
                //    CommunityId = 0,
                //    FirstName = ViewTextHelper.DemoStudentFirstName,
                //    ID = 0,
                //    LastName = ViewTextHelper.DemoStudentLastName,
                //    SchoolId = 0,
                //    SchoolYear = year,
                //    SchoolName = ViewTextHelper.DemoSchoolName,
                //    StudentAssessmentId = 0
                //});
                //total++;
            }
            var result = new { total = total, data = list };
            return JsonHelper.SerializeObject(result);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.CPALLS, Anonymity = Anonymous.Verified, Parameter = "assessmentId")]
        public string LockMeasure(int studentId, int measureId, int assessmentId, int studentAssessmentId, int schoolId, Wave wave, int year, int classId, CpallsStatus status)
        {
            var response = new PostFormResponse();
            CpallsStudentModel student = _studentBusiness.GetStudentModel(studentId, UserInfo);
            var measureIds = new List<int>() { measureId };
            var chkRes = _cpallsBusiness.CheckStudentMeasure(UserInfo, assessmentId, studentId, schoolId, year.ToSchoolYearString(), year, wave, measureIds);
            if (chkRes.ResultType == OperationResultType.Error)
            {
                response.Update(chkRes);
                return JsonHelper.SerializeObject(response);
            }

            if (student != null)
            {
                response.Update(_cpallsBusiness.LockMeasure(student, measureId, assessmentId, studentAssessmentId, schoolId, wave, year, UserInfo, status));
            }
            else
            {
                response.Success = false;
                response.Message = "Student is required.";
            }
            return JsonHelper.SerializeObject(response);
        }

        public int IsExistMobileAudio(string measureIds)
        {
            var listMeasureId = measureIds.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList();
            int audios = _adeBusiness.GetIsExistMobileAudio(listMeasureId);
            return audios;
        }

        ///Cpalls/Execute/Index(int id, string measures, int classId, bool calc = false)
        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.CPALLS, Anonymity = Anonymous.Verified, Parameter = "assessmentId")]
        public string PlayMeasure(int studentId, int assessmentId, int studentAssessmentId, Wave wave, int year, int classId, string measureIds)
        {
            var response = new PostFormResponse();
            CpallsStudentModel student = _studentBusiness.GetStudentModelForPlayer(studentId, UserInfo);
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
            var schoolId = 0;
            if (student.SchoolId >0)
                schoolId = student.SchoolId;
            var chkRes = _cpallsBusiness.CheckStudentMeasure(UserInfo, assessmentId, studentId, schoolId, year.ToSchoolYearString(), year, wave, measureIds.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList());
            if (chkRes.ResultType == OperationResultType.Error)
            {
                response.Update(chkRes);
                return JsonHelper.SerializeObject(response);
            }

            int newStudentAssessmentId;
            OperationResult operationResult = _cpallsBusiness.PlayMeasure(student, assessmentId, measureIds.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Select(int.Parse), studentAssessmentId, year, wave,
                UserInfo, out newStudentAssessmentId);
            response.Update(operationResult);
            if (response.Success)
            {
                var listMeasureId = measureIds.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList();
                ExecCpallsAssessmentModel assessment = _cpallsBusiness.GetAssessment(newStudentAssessmentId, listMeasureId, classId, UserInfo);
                bool showLaunchPage = assessment.Measures.Any(measureEntity => measureEntity.ShowLaunchPage && measureEntity.OrderType == OrderType.Sequenced
                                                                && measureEntity.Status == CpallsStatus.Initialised);
                if (!showLaunchPage)
                {
                    response.OtherMsg = "NotShowLaunchPage";
                    assessment.Measures =
                        assessment.Measures.Where(x => x.Status == CpallsStatus.Initialised || x.Status == CpallsStatus.Paused)
                            .ToList();
                    var isAssessmentExist = _adeBusiness.IsAssessmentExist(assessment.AssessmentId);
                    if (!isAssessmentExist)
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
                    if (assessment.Class.ID == 0)
                    {
                        assessment.Class = new ExecCpallsClassModel()
                        {
                            ID = 0,
                            Name = ViewTextHelper.DemoClassName
                        };
                    }
                    response.Data = assessment;
                }
                else
                {
                    response.OtherMsg = "";
                    var executeUrl = "/Cpalls/Execute/Index";
                    response.Data = string.Format("{0}?id={1}&measures={2}&classId={3}&wave={4}",
                        executeUrl, newStudentAssessmentId, measureIds, classId, (int)wave);
                }

            }
            return JsonHelper.SerializeObject(response);
        }
        ///Cpalls/Execute/Index(int id, string measures, int classId, bool calc = false)
        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.CPALLS, Anonymity = Anonymous.Verified, Parameter = "assessmentId")]
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

            var chkRes = _cpallsBusiness.CheckStudentMeasure(UserInfo, assessmentId, studentId, student.Schools.FirstOrDefault().ID, year.ToSchoolYearString(), year, wave, measureIds.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList());
            response.Update(chkRes);
            return JsonHelper.SerializeObject(response);
        }


        //Ticket 2137 要求记录用户已经选中的wave

        public string LogUserWave(int wave, int assessmentId)
        {

            var response = new PostFormResponse();
            var result = _adeBusiness.SaveWaveLog((Wave)wave, UserInfo.ID, assessmentId);

            response.Success = result.ResultType == OperationResultType.Success;
            response.Message = result.Message;

            return JsonHelper.SerializeObject(response);
        }

    }
}