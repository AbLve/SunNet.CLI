using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Antlr.Runtime;
using Microsoft.Ajax.Utilities;
using Sunnet.Cli.Assessment.Areas.Report.Models;
using Sunnet.Cli.Assessment.Controllers;
using Sunnet.Cli.Core.Common.Enums;
using Sunnet.Cli.Core.Cpalls;
using Sunnet.Cli.UIBase.Models;
using Sunnet.Framework.Core.Tool;
using Sunnet.Framework.Extensions;
using Sunnet.Framework.Helpers;
using Sunnet.Cli.Core.Communities.Entities;
using Sunnet.Cli.Business.Communities;
using Sunnet.Cli.Core.Schools.Entities;
using Sunnet.Cli.Business.Schools;
using Sunnet.Cli.Business.Common;
using Sunnet.Cli.Business.Cpalls.Models;
using Sunnet.Cli.Business.Classes;
using Sunnet.Cli.Business.Cpalls;
using Sunnet.Cli.Business.Ade;
using Sunnet.Cli.Core.Ade.Entities;
using Sunnet.Cli.Core.Ade;
using Sunnet.Cli.Core.Users.Enums;
using System.Linq.Expressions;
using Sunnet.Cli.Core.Cpalls.Entities;
using Sunnet.Cli.Core.Students.Entities;
using Sunnet.Cli.UIBase.Filters;
using Sunnet.Framework.Permission;
using Sunnet.Framework.Resources;

namespace Sunnet.Cli.Assessment.Areas.Cpalls.Controllers
{
    public class SchoolController : BaseController
    {
        CommunityBusiness _communityBusiness;
        SchoolBusiness _schoolBusiness;
        ClassBusiness _classBusiness;
        CpallsBusiness _cpallsBusiness;
        AdeBusiness _adeBusiness;
        public SchoolController()
        {
            _communityBusiness = new CommunityBusiness();
            _schoolBusiness = new SchoolBusiness();
            _classBusiness = new ClassBusiness();
            _cpallsBusiness = new CpallsBusiness(AdeUnitWorkContext);
            _adeBusiness = new AdeBusiness(AdeUnitWorkContext);
        }

        // GET: /Cpalls/School/
        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.CPALLS, Anonymity = Anonymous.Verified, Parameter = "assessmentId")]
        public ActionResult Index(int assessmentId, int year = 0, int wave = 0)
        {
            if (!_adeBusiness.CanExecuteAssessment(assessmentId))
                return RedirectToAction("Index", "Dashboard", new { Area = "", showMessage = "assessment_unavaiable" });

            var assessment = _adeBusiness.GetBaseAssessmentModel(assessmentId);
            if (assessment == null)
                return RedirectToAction("Index", "Dashboard", new { Area = "", showMessage = "assessment_unavaiable" });

            if (UserInfo.Role != Role.Super_admin)
            {
                if (!_schoolBusiness.GetSchoolIds(UserInfo).Any())
                    ViewBag.Message = ResourceHelper.GetRM().GetInformation("Assessment_No_Community");
            }

            if (year == 0)
            {
                year = CommonAgent.Year;
            }
            if (wave == 0)
            {
                //获取用户上一次记录过的Wave
                var waveLog = _adeBusiness.GetUserWaveLog(UserInfo.ID,assessmentId);
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


            // 判断处理是否有双语版本
            ViewBag.HasAnotherVersion = false;
            if (assessment.TheOtherId > 0)
            {
                ViewBag.HasAnotherVersion = true;
                ViewBag.OtherVersion = AssessmentLanguageHelper.GetButtonText(assessment.TheOtherLang);
                ViewBag.OtherAssessmentId = assessment.TheOtherId;
            }

            ViewBag.AssessmentId = assessmentId;
            ViewBag.AssessmentName = assessment.Name;
            ViewBag.AssessmentReports = _adeBusiness.GetAssessmentReports(assessmentId, ReportTypeEnum.Community);

            //当前年度，数据初始化
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
           // List<MeasureHeaderModel> MeasureList;
           // List<MeasureHeaderModel> ParentMeasureList;

           // _cpallsBusiness.BuilderHeader(assessmentId
           //     , year, (Wave)wave
           //     , out MeasureList, out ParentMeasureList);

           // //去除 Hide的measures
           //var HasHide = false;
           // var findShownMeasures = _cpallsBusiness.GetUserShownMeasure(assessmentId, UserInfo.ID, (Wave)wave, year);
           // if (findShownMeasures != null)
           // {
           //     var measures = JsonHelper.DeserializeObject<Dictionary<string, List<int>>>(findShownMeasures.Measures);
           //     if (measures[wave.ToString()].Count > 0)
           //     {
           //         foreach (var measure in MeasureList)
           //         {
           //             if(!measures[wave.ToString()].Contains(measure.MeasureId))
           //             {
           //                 HasHide = true;
           //                 break;
           //             }
           //         }
           //         MeasureList = MeasureList.Where(o => measures[wave.ToString()].Contains(o.MeasureId)).ToList();
           //         ParentMeasureList = ParentMeasureList.Where(o => measures[wave.ToString()].Contains(o.MeasureId)).ToList();
           //     }
           // }
           // foreach (var measureHeaderModel in ParentMeasureList)
           // {
           //     measureHeaderModel.Subs = MeasureList.Count(c => c.ParentId == measureHeaderModel.MeasureId);
           // }

           // ViewBag.HasHide = HasHide;
           // ViewBag.Parents = JsonHelper.SerializeObject(ParentMeasureList.Where(x => x.Subs > 0)
           //     .Select(x => new { x.MeasureId, x.Subs, x.Name }));
           // ViewBag.Measures = JsonHelper.SerializeObject(MeasureList);

          
            
           // if (MeasureList != null && MeasureList.Count > 0)
           // {
           //     ViewBag.HaveMeasure = true;

           //     ViewBag.MeasureList = MeasureList;
           //     ViewBag.ParentMeasure = ParentMeasureList;
           // }
            return View();
        }

        /// <summary>
        /// 方案备案，没有调用的地方，可删除
        /// </summary>
        /// <param name="assessmentId"></param>
        /// <param name="year"></param>
        /// <param name="wave"></param>
        /// <param name="communityId"></param>
        /// <param name="schoolId"></param>
        /// <param name="sort"></param>
        /// <param name="order"></param>
        /// <param name="first"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.CPALLS, Anonymity = Anonymous.Verified, Parameter = "assessmentId")]
        public string Search2(int assessmentId, int year, int wave, int communityId = 0, int schoolId = 0, string sort = "Name", string order = "Asc",
         int first = 0, int count = 10)
        {
            int total = 0;
            List<CpallsSchoolModel> list = new List<CpallsSchoolModel>();

            if (CommonAgent.IsCurrentSchoolYear(year))
            {
                //当前数据
                Expression<Func<SchoolEntity, bool>> schoolCondition;
                schoolCondition = PredicateHelper.True<SchoolEntity>();

                if (schoolId > 0)
                    schoolCondition = schoolCondition.And(s => s.ID == schoolId);
                else
                {
                    if (communityId > 0)
                        schoolCondition =
                            schoolCondition.And(
                                s =>
                                    s.CommunitySchoolRelations.Any(
                                        x => x.CommunityId == communityId && x.Status == EntityStatus.Active));
                }

                schoolCondition = schoolCondition.And(s => s.Status == SchoolStatus.Active);

                list = _cpallsBusiness.GetCpallsSchool(UserInfo, schoolCondition, assessmentId, (Wave)wave, sort, order, first, count, out total);

                total++;
                list.Insert(0, new CpallsSchoolModel()
                {
                    ID = 0,
                    Name = ViewTextHelper.DemoSchoolName
                });

            }
            //  var findShownMeasures = _cpallsBusiness.GetUserShownMeasure(assessmentId, UserInfo.ID, (Wave)wave, year);
            //var findMeasures = new Dictionary<string, List<int>>() ;
            //if (findShownMeasures != null)
            //{
            //      findMeasures = JsonHelper.DeserializeObject<Dictionary<string, List<int>>>(findShownMeasures.Measures);
                 
            //}
            //foreach (var cpallsSchoolModel in list)
            //{
            //    cpallsSchoolModel.MeasureList =
            //        cpallsSchoolModel.MeasureList.Where(o => findMeasures[wave.ToString()].Contains(o.MeasureId))
            //            .ToList();
            //}


            var result = new { total = total, data = list };
            return JsonHelper.SerializeObject(result);
        }



        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.CPALLS, Anonymity = Anonymous.Verified,Parameter = "assessmentId")]
        public string Search(int year, int communityId = 0, int schoolId = 0, int wave = 1, string sort = "Name", string order = "Asc", int first = 0, int count = 10)
        {
            int total = 0;
             IEnumerable<SchoolSelectItemModel>
                 schoolList = _schoolBusiness.GetSchoolsSelectListForCpalls(UserInfo, communityId, schoolId, sort, order, first,count,out total);

            var list = schoolList.ToList();
            //total++;
            //list.Insert(0, new SchoolSelectItemModel()
            //{
            //    ID = 0,
            //    Name = ViewTextHelper.DemoSchoolName
            //});

            var result = new { total = total, data = list };
            return JsonHelper.SerializeObject(result);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.CPALLS, Anonymity = Anonymous.Verified)]
        public string GetCommunitySelectListForSearch(string keyword, int communityId = -1, bool isActiveCommunity = true)
        {  
            var list = _communityBusiness.GetCommunitySelectListForCache(UserInfo, communityId, isActiveCommunity);
            return JsonHelper.SerializeObject(list);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.CPALLS, Anonymity = Anonymous.Verified)]
        public string GetSchoolSelectList(string keyword, int communityId = 0, string schoolName = "", bool isActive = true)
        { 
            var schoolList = _schoolBusiness.GetSchoolsSelectListForCache(UserInfo, communityId, schoolName, isActive);
            return JsonHelper.SerializeObject(schoolList);
        }


        //Hide or Show Measures
        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.CPALLS, Anonymity = Anonymous.Verified, Parameter = "assessmentId")]
        public ActionResult HideMeasures(int assessmentId, int year = 0, Wave wave = Wave.BOY)
        {
            List<MeasureHeaderModel> measures = new List<MeasureHeaderModel>();
            List<MeasureHeaderModel> parentMeasures = new List<MeasureHeaderModel>();
            List<MeasureHeaderModel> measuresSum = new List<MeasureHeaderModel>();
            List<MeasureHeaderModel> parentMeasuresSum = new List<MeasureHeaderModel>();
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


            //measuresSum.AddRange(measures);
            //parentMeasuresSum.AddRange(parentMeasures);


            //List<int> parentIds = measuresSum.Where(r => r.MeasureId != r.ParentId).GroupBy(r => r.ParentId).Select(r => r.Key).ToList();
            //List<int> removeIds = new List<int>();
            //foreach (MeasureHeaderModel item in parentMeasuresSum)
            //{
            //    if (!(parentIds.Contains(item.MeasureId) || (item.ParentId == 1 && item.Subs == 0)))
            //        removeIds.Add(item.MeasureId);
            //}

            //parentMeasuresSum.RemoveAll(r => removeIds.Contains(r.MeasureId));
            //measuresSum.RemoveAll(r => r.MeasureId == r.ParentId && removeIds.Contains(r.MeasureId));


            ViewBag.assessmentId = assessmentId;
            ViewBag.year = year;
            ViewBag.wave = (int)wave;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CLIUrlAuthorize(Account = Authority.Index, PageId = PagesModel.CPALLS, Anonymity = Anonymous.Verified, Parameter = "assessmentId")]
        public string SaveShownMeasures(int assessmentId, int year, Wave wave, string measures)
        {
            var response = new PostFormResponse();
            OperationResult result = new OperationResult(OperationResultType.Success);
            var findEntity = _cpallsBusiness.GetUserShownMeasure(assessmentId, UserInfo.ID, wave, year);
             if (findEntity != null)
            {
                if (measures == "{\"1\":[],\"2\":[],\"3\":[]}")
                {
                    result =  _cpallsBusiness.DeleteUserShownMeasures(findEntity.ID);
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
    }

  
}