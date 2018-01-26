using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Sunnet.Cli.Assessment.Controllers;
using Sunnet.Cli.Business.Classes;
using Sunnet.Cli.Business.Classes.Models;
using Sunnet.Cli.Business.Communities;
using Sunnet.Cli.Business.Cpalls;
using Sunnet.Cli.Core.Common.Enums;
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
using Sunnet.Cli.Core.Ade.Entities;
using Sunnet.Cli.Core.Ade;
using Sunnet.Cli.Business.Schools;
using Sunnet.Cli.UIBase.Filters;
using Sunnet.Framework.Permission;

namespace Sunnet.Cli.Assessment.Areas.Cpalls.Controllers
{
    public class ClassController : BaseController
    {
        ClassBusiness _classBusiness;
        CpallsBusiness _cpallsBusiness;
        AdeBusiness _adeBusiness;
        SchoolBusiness _schoolBusiness;
        private CommunityBusiness _communityBusiness;
        public ClassController()
        {
            _classBusiness = new ClassBusiness();
            _cpallsBusiness = new CpallsBusiness(AdeUnitWorkContext);
            _adeBusiness = new AdeBusiness(AdeUnitWorkContext);
            _schoolBusiness = new SchoolBusiness();
            _communityBusiness = new CommunityBusiness();
        }

        // GET: /Cpalls/Class/
        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.CPALLS, Anonymity = Anonymous.Verified, Parameter = "assessmentId")]
        public ActionResult Index(int schoolId, int assessmentId, int year = 0, int wave = 0)
        {
            if ((int)UserInfo.Role > (int)Role.Teacher)
                return new RedirectResult("/error/nonauthorized");

            if (!_adeBusiness.CanExecuteAssessment(assessmentId))
                return RedirectToAction("Index", "Dashboard", new { Area = "", showMessage = "assessment_unavaiable" });
            var assessment = _adeBusiness.GetBaseAssessmentModel(assessmentId);
            if (assessment == null)
                return RedirectToAction("Index", "Dashboard", new { Area = "", showMessage = "assessment_unavaiable" });

            ViewBag.ShowSchoolback = true;
            if (UserInfo.Role == Role.Teacher)
                ViewBag.ShowSchoolback = false;

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

            //判断处理是否有双语版本
            ViewBag.HasAnotherVersion = false;

            if (assessment.TheOtherId > 0)
            {
                ViewBag.HasAnotherVersion = true;
                ViewBag.OtherVersion = AssessmentLanguageHelper.GetButtonText(assessment.TheOtherLang);
                ViewBag.OtherAssessmentId = assessment.TheOtherId;
            }

            //当前年度，数据初始化
            if (CommonAgent.IsCurrentSchoolYear(year))
            {
                _cpallsBusiness.InitializeStudentAssessmentDate(assessmentId, (Wave)wave, UserInfo);
            }
            ViewBag.AssessmentId = assessmentId;
            ViewBag.AssessmentName = assessment.Name;
            ViewBag.SchoolId = schoolId;
            ViewBag.AssessmentReports = _adeBusiness.GetAssessmentReports(assessmentId, ReportTypeEnum.School);

            CpallsSchoolModel schoolModel = _schoolBusiness.GetCpallsSchoolModel(schoolId);
            if (schoolModel == null && schoolId == 0)
            {
                schoolModel = new CpallsSchoolModel()
                {
                    ID = 0,
                    Name = ViewTextHelper.DemoSchoolName
                };
            }
            ViewBag.SchoolModel = schoolModel;

            ////绑定年度与 wave下拉框数据
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
            //List<MeasureHeaderModel> MeasureList;
            //List<MeasureHeaderModel> ParentMeasureList;

            //_cpallsBusiness.BuilderHeader(assessmentId
            //    , year, (Wave)wave
            //    , out MeasureList, out ParentMeasureList);


            ////去除 Hide的measures
            //var HasHide = false;
            //var findShownMeasures = _cpallsBusiness.GetUserShownMeasure(assessmentId, UserInfo.ID, (Wave)wave, year);
            //if (findShownMeasures != null)
            //{
            //    var measures = JsonHelper.DeserializeObject<Dictionary<string, List<int>>>(findShownMeasures.Measures);
            //    if (measures[wave.ToString()].Count > 0)
            //    {
            //        foreach (var measure in MeasureList)
            //        {
            //            if (!measures[wave.ToString()].Contains(measure.MeasureId))
            //            {
            //                HasHide = true;
            //                break;
            //            }
            //        }
            //        MeasureList = MeasureList.Where(o => measures[wave.ToString()].Contains(o.MeasureId)).ToList();
            //        ParentMeasureList = ParentMeasureList.Where(o => measures[wave.ToString()].Contains(o.MeasureId)).ToList();
            //    }

            //}

            //foreach (var measureHeaderModel in ParentMeasureList)
            //{
            //    measureHeaderModel.Subs = MeasureList.Count(c => c.ParentId == measureHeaderModel.MeasureId);
            //}
            //ViewBag.HasHide = HasHide;



            //List<MeasureHeaderModel> ViewBagMeasureList = new List<MeasureHeaderModel>();
            //foreach (MeasureHeaderModel tmpItem in ParentMeasureList.FindAll(r => r.Subs > 0))
            //    ViewBagMeasureList.Add(new MeasureHeaderModel() { MeasureId = tmpItem.MeasureId, Subs = tmpItem.Subs + 1, Name = tmpItem.Name });

            //ViewBag.Parents = JsonHelper.SerializeObject(ParentMeasureList.Where(x => x.Subs > 0).Select(x => new { x.MeasureId, x.Subs, x.Name }));
            //ViewBag.Measures = JsonHelper.SerializeObject(MeasureList);
            //if (MeasureList != null && MeasureList.Count > 0)
            //{
            //    ViewBag.HaveMeasure = true;

            //    ViewBag.MeasureList = MeasureList;
            //    ViewBag.ParentMeasure = ParentMeasureList;
            //}
            return View();
        }

        /// <summary>
        /// 方法备份，可以删除
        /// </summary>
        /// <param name="schoolId"></param>
        /// <param name="assessmentId"></param>
        /// <param name="year"></param>
        /// <param name="wave"></param>
        /// <param name="className"></param>
        /// <param name="sort"></param>
        /// <param name="order"></param>
        /// <param name="first"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.CPALLS, Anonymity = Anonymous.Verified, Parameter = "assessmentId")]
        public string Search2(int schoolId, int assessmentId, int year, int wave, string className = "", string sort = "Name", string order = "Asc",
            int first = 0, int count = 10)
        {
            int total = 0;
            List<CpallsClassModel> list = new List<CpallsClassModel>();

            if (CommonAgent.IsCurrentSchoolYear(year))
            {
                // 当前数据
                Expression<Func<ClassEntity, bool>> classCondition;
                classCondition = PredicateHelper.True<ClassEntity>();
                classCondition = classCondition.And(r => r.SchoolId == schoolId);

                if (className.Trim() != string.Empty)
                    classCondition = classCondition.And(r => r.Name.Contains(className));
                classCondition = classCondition.And(r => r.SchoolId == schoolId && r.Status == EntityStatus.Active);
                classCondition = classCondition.And(r => r.SchoolYear == CommonAgent.SchoolYear);

                //犹豫性能问题，暂时去掉该条件查询
                //var classlevlIds = _communityBusiness.GetAssignedClassLevels(assessmentId, schoolId);
                //if (!classlevlIds.Contains(0))//All and missing
                //{
                //    classCondition = classCondition.And(r => classlevlIds.Contains(r.Classlevel));
                //}
                list = _cpallsBusiness.GetCpallsClass(UserInfo, classCondition, assessmentId, (Wave)wave, sort, order, first, count, out total);
                total++;
                list.Insert(0, new CpallsClassModel()
                {
                    ID = 0,
                    Name = ViewTextHelper.DemoClassName
                });
            }
            var result = new { total = total, data = list };
            return JsonHelper.SerializeObject(result);
        }


        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.CPALLS, Anonymity = Anonymous.Verified, Parameter = "assessmentId")]
        public string Search(int schoolId, int assessmentId, int year, int wave, string className = "", string sort = "Name", string order = "Asc",
      int first = 0, int count = 10)
        {
            int total = 0;
            List<CpallsClassModel> list = new List<CpallsClassModel>();

            if (CommonAgent.IsCurrentSchoolYear(year))
            {
                // 当前数据
                Expression<Func<ClassEntity, bool>> classCondition;
                classCondition = PredicateHelper.True<ClassEntity>();
                classCondition = classCondition.And(r => r.SchoolId == schoolId);

                if (className.Trim() != string.Empty)
                    classCondition = classCondition.And(r => r.Name.Contains(className));
                classCondition = classCondition.And(r => r.SchoolId == schoolId && r.Status == EntityStatus.Active);
                classCondition = classCondition.And(r => r.SchoolYear == CommonAgent.SchoolYear);

                //由于性能问题，暂时去掉该条件查询
                //var classlevlIds = _communityBusiness.GetAssignedClassLevels(assessmentId, schoolId);
                //if (!classlevlIds.Contains(0))//All and missing
                //{
                //    classCondition = classCondition.And(r => classlevlIds.Contains(r.Classlevel));
                //}
                list = _classBusiness.GetCpallsClassForCache(UserInfo, schoolId, className, sort, order, first, count, out total);
                //total++;
                //list.Insert(0, new CpallsClassModel()
                //{
                //    ID = 0,
                //    Name = ViewTextHelper.DemoClassName
                //});
            }
            var result = new { total = total, data = list };
            return JsonHelper.SerializeObject(result);
        }
    }
}