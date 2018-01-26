using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using Antlr.Runtime.Tree;
using Microsoft.Ajax.Utilities;
using Sunnet.Cli.Assessment.Controllers;
using Sunnet.Cli.Business.Ade;
using Sunnet.Cli.Business.Ade.Models;
using Sunnet.Cli.Business.Cec;
using Sunnet.Cli.Business.Common;
using Sunnet.Cli.Business.Communities;
using Sunnet.Cli.Business.Cpalls.Models;
using Sunnet.Cli.Business.Permission;
using Sunnet.Cli.Business.Permission.Models;
using Sunnet.Cli.Business.Schools;
using Sunnet.Cli.Business.Users;
using Sunnet.Cli.Business.Users.Models;
using Sunnet.Cli.Core;
using Sunnet.Cli.Core.Ade;
using Sunnet.Cli.Core.Cec.Entities;
using Sunnet.Cli.Core.Common.Enums;
using Sunnet.Cli.Core.Communities.Entities;
using Sunnet.Cli.Core.Schools.Entities;
using Sunnet.Cli.Core.Users.Entities;
using Sunnet.Cli.Core.Users.Enums;
using Sunnet.Cli.UIBase.Filters;
using Sunnet.Cli.UIBase.Models;
using Sunnet.Framework.Core.Base;
using Sunnet.Framework.Core.Tool;
using Sunnet.Framework.Extensions;
using Sunnet.Framework.Helpers;
using Sunnet.Framework.Permission;
using WebGrease.Css.Extensions;
using Sunnet.Cli.Core.Cpalls;
using System.Linq.Expressions;
using Sunnet.Framework.PDF;
using System.IO;
using System.Text;
using System.Globalization;
using Sunnet.Cli.Business.Cec.Model;
using Sunnet.Cli.Business.Cpalls;
using Sunnet.Cli.Core.Ade.Entities;
using Sunnet.Cli.Core.Cec.Models;
using System.Web.Caching;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using CecItemModel = Sunnet.Cli.Business.Cec.Model.CecItemModel;

namespace Sunnet.Cli.Assessment.Areas.Cec.Controllers
{
    public class CecController : BaseController
    {
        private readonly UserBusiness _userBusiness;
        private readonly CommunityBusiness _communityBusiness;
        private readonly SchoolBusiness _schoolBusiness;
        private readonly AdeBusiness _adeBusiness;
        private readonly CecBusiness _cecBusiness;
        private readonly CpallsBusiness _cpallsBusiness;

        public CecController()
        {
            _userBusiness = new UserBusiness(UnitWorkContext);
            _communityBusiness = new CommunityBusiness(UnitWorkContext);
            _schoolBusiness = new SchoolBusiness(UnitWorkContext);

            _adeBusiness = new AdeBusiness(AdeUnitWorkContext);
            _cecBusiness = new CecBusiness(AdeUnitWorkContext);
            _cpallsBusiness = new CpallsBusiness(AdeUnitWorkContext);
        }

        //
        // GET: /Cec/Home/
        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.CEC, Anonymity = Anonymous.Verified, Parameter = "assessmentId")]
        public ActionResult Index(int assessmentId)
        {
            var assessment = _adeBusiness.GetBaseAssessmentModel(assessmentId);
            if (assessment == null) return new EmptyResult();
            ViewBag.AssessmentName = assessment.Name;

            if (!_adeBusiness.CanExecuteAssessment(assessmentId))
                return RedirectToAction("Index", "Dashboard", new { Area = "", showMessage = "assessment_unavaiable" });
            _adeBusiness.LockAssessment(assessmentId);

            ViewBag.AssessmentId = assessmentId;

            // 绑定年度与 wave下拉框数据
            List<SelectListItem> yearList = new SelectList(CommonAgent.GetYears(), "ID", "Name").ToList();
            yearList.ForEach(r => r.Selected = false);
            int year = CommonAgent.Year;

            SelectListItem tmpLI = yearList.Find(r => r.Value == year.ToString());
            if (tmpLI != null)
                tmpLI.Selected = true;
            ViewBag.Year = year.ToString();
            ViewBag.YearOptions = yearList;
            ViewBag.SchoolYear = CommonAgent.SchoolYear;

            ViewBag.IsAdmin = UserInfo.Role == Role.Super_admin;
            ViewBag.CecOffline = CheckAssessmentPermission(assessmentId, Authority.Offline);

            return View(new CECSeachModel());
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.CEC, Anonymity = Anonymous.Verified)]
        public string GetCommunitySelectListForSearch(string keyword, int communityId = -1, bool isActiveCommunity = true)
        {
            var expression = PredicateHelper.True<CommunityEntity>();
            if (communityId > 0)
                expression = expression.And(o => o.ID == communityId);
            var list = _communityBusiness.GetCommunitySelectList(UserInfo, expression, isActiveCommunity);
            return JsonHelper.SerializeObject(list);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.CEC, Anonymity = Anonymous.Verified)]
        public string GetSchoolSelectList(string keyword, int communityId = 0, string schoolName = "", bool isActive = true)
        {
            var expression = PredicateHelper.True<SchoolEntity>();
            if (communityId > 0)
                expression = expression.And(o => o.CommunitySchoolRelations.Any(c => c.CommunityId == communityId));
            if (schoolName != null && schoolName.Trim() != string.Empty)
                expression = expression.And(o => o.BasicSchool.Name.Contains(schoolName));
            var schoolList = _schoolBusiness.GetSchoolsSelectList(UserInfo, expression, isActive);
            return JsonHelper.SerializeObject(schoolList);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.CEC, Anonymity = Anonymous.Verified, Parameter = "assessmentId")]
        public string Search(int assessmentId, string teacherCode, string firstName, string lastName, int year, int communityId = 0,
            int schoolId = 0, int coachId = 0, string sort = "LastName", string order = "Asc", int first = 0, int count = 10)
        {
            int total;
            int? coach = null;
            if (coachId != 0)
                coach = coachId;
            if (year == 0) year = CommonAgent.Year;
            List<int> schoolIds = null;
            List<int> communities = null;
            if (UserInfo.Role == Role.Super_admin)
            {
                // 管理员可以看没有分配的Teacher
            }
            else if (UserInfo.Role == Role.Coordinator
                      || UserInfo.Role == Role.Intervention_manager
                      || UserInfo.Role == Role.Intervention_support_personnel
                      || UserInfo.Role == Role.Content_personnel)
            {
                communities = _userBusiness.GetCommunities(UserInfo.ID);
            }
            else if (UserInfo.Role == Role.Mentor_coach)
            {
                // Coach进来看分配给自己的Teacher
                coach = UserInfo.ID;
            }
            else
            {
                communities = new List<int>();
                schoolIds = new List<int>();
                // 其他人进来看属于自己的Teacher
                switch (UserInfo.Role)
                {
                    case Role.Community:
                    case Role.District_Community_Delegate:
                    case Role.District_Community_Specialist:
                    case Role.Community_Specialist_Delegate:
                        communities.AddRange(
                            UserInfo.UserCommunitySchools.Where(ucs => ucs.Status == EntityStatus.Active)
                                .Select(ucs => ucs.CommunityId)
                                .ToList());
                        break;
                    case Role.Principal:
                    case Role.Principal_Delegate:
                    case Role.TRS_Specialist:
                    case Role.TRS_Specialist_Delegate:
                    case Role.School_Specialist:
                    case Role.School_Specialist_Delegate:
                        schoolIds.AddRange(UserInfo.UserCommunitySchools.Select(x => x.SchoolId).ToList());
                        break;
                }
                communities.Add(communityId);
            }
            if (schoolId > 0)
                schoolIds = new List<int>() { schoolId };
            if (communityId > 0)
                communities = new List<int>() { communityId };
            List<CecSchoolTeacherModel> list = _cecBusiness.GetCECTeacherList(assessmentId, year, coach, communities, schoolIds, firstName, lastName,
                teacherCode, sort, order, first, count, out total);

            var result = new { total = total, data = list };
            return JsonHelper.SerializeObject(result);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.CEC, Anonymity = Anonymous.Verified, Parameter = "assessmentId")]
        public string Offline(int assessmentId = 19, string teacherCode = "", string firstName = "", string lastName = "", int year = 2014
            , int communityId = 0, string communityName = "", string schoolName = "", int schoolId = 0
            , string sort = "LastName", string order = "Asc", int first = 0, int count = 10)
        {
            OfflineUrlModel model = new OfflineUrlModel();
            model.assessmentId = assessmentId;
            model.communityId = communityId;
            model.communityName = communityName;
            model.schoolId = schoolId;
            model.schoolName = schoolName;
            model.teacherCode = teacherCode;
            model.firstName = firstName;
            model.lastName = lastName;
            model.year = year;
            model.sort = sort;
            model.order = order;
            model.first = first;
            model.count = count;

            Session["_CEC_Offline_URL"] = model;
            return string.Empty;
        }


        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.CEC, Anonymity = Anonymous.Verified, Parameter = "assessmentId")]
        public ActionResult Measure(int teacherId, int assessmentId, int wave)
        {
            var assessment = _adeBusiness.GetBaseAssessmentModel(assessmentId);
            if (assessment == null) return new EmptyResult();
            ViewBag.AssessmentName = assessment.Name;

            if (!_adeBusiness.CanExecuteAssessment(assessmentId))
                return RedirectToAction("Index", "Dashboard", new { Area = "", showMessage = "assessment_unavaiable" });

            List<SelectListItem> waveList = new SelectList(CommonAgent.GetWave(), "ID", "Name").ToList();
            SelectListItem tmpLI = waveList.Find(r => r.Value == wave.ToString());
            if (tmpLI != null)
                tmpLI.Selected = true;
            ViewBag.WaveOptions = waveList;
            ViewBag.assessmentId = assessmentId;
            ViewBag.teacherId = teacherId;
            ViewBag.SchoolYear = CommonAgent.SchoolYear;
            ViewBag.Year = CommonAgent.Year;

            return View();
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.CEC, Anonymity = Anonymous.Verified, Parameter = "assessmentId")]
        public string ShowItems(int teacherId, int assessmentId, Wave wave)
        {
            List<CecItemModel> itemList = _cecBusiness.GetCecItemModel(assessmentId, wave);
            var response = new PostFormResponse();
            if (itemList != null && itemList.Any())
            {
                //获取Items 的Links
                int[] ids = itemList.Select(r => r.ItemId).Distinct().ToArray();
                List<AdeLinkEntity> itemLinks = _adeBusiness.GetLinks<ItemBaseEntity>(ids);

                foreach (CecItemModel item in itemList)
                {
                    item.Links = new List<AdeLinkEntity>();
                    item.Links.AddRange(itemLinks.FindAll(r => r.HostId == item.ItemId));
                }

                //表头
                List<MeasureHeaderModel> MeasureList;
                List<MeasureHeaderModel> ParentMeasureList;

                _cpallsBusiness.BuilderHeader(assessmentId
                    , CommonAgent.Year, (Wave)wave
                    , out MeasureList, out ParentMeasureList);

                List<int> tmpMeasureIds = MeasureList.Select(r => r.MeasureId).ToList();
                tmpMeasureIds.AddRange(ParentMeasureList.Select(r => r.MeasureId).ToList());
                ids = tmpMeasureIds.Distinct().ToArray();
                List<AdeLinkEntity> measureLinks = _adeBusiness.GetLinks<MeasureEntity>(ids);

                // ViewBag.HaveMeasure = true;

                foreach (MeasureHeaderModel item in MeasureList)
                {
                    item.Links = new List<AdeLinkEntity>();
                    item.Links.AddRange(measureLinks.FindAll(r => r.HostId == item.MeasureId));
                }

                foreach (MeasureHeaderModel item in ParentMeasureList)
                {
                    item.Links = new List<AdeLinkEntity>();
                    item.Links.AddRange(measureLinks.FindAll(r => r.HostId == item.MeasureId));
                }

                response.Success = true;
                Expression<Func<CecResultEntity, bool>> condition = PredicateHelper.True<CecResultEntity>();

                condition = condition.And(r => r.CecHistory.AssessmentId == assessmentId && r.CecHistory.SchoolYear == CommonAgent.SchoolYear
                    && r.CecHistory.Wave == wave && r.CecHistory.TeacherId == teacherId);

                List<CecResultModel> resultList = _cecBusiness.GetCecResultModels(condition);

                if (resultList != null && resultList.Count > 0)
                {
                    foreach (CecResultModel item in resultList)
                    {
                        CecItemModel cecItem = itemList.Find(r => r.ItemId == item.ItemId);
                        if (cecItem != null)
                        {
                            CecAnswerModel tmpAnser = cecItem.Answer.FirstOrDefault(r => r.AnswerId == item.AnswerId);
                            if (tmpAnser != null)
                                tmpAnser.Selected = true;
                        }
                    }
                    response.Message = resultList[0].AssessmentDate.ToString("MM/dd/yyyy");
                }

                List<CecHistoryModel> list = new List<CecHistoryModel>();

                List<MeasureHeaderModel> headerList = ParentMeasureList;
                foreach (MeasureHeaderModel item in headerList)
                {
                    CecHistoryModel history = new CecHistoryModel();
                    history.MeasureId = item.MeasureId;
                    history.MeasureName = item.Name;
                    history.Links = item.Links;
                    if (item.Subs == 0)
                    {
                        history.List = new List<CecItemModel>();
                        history.List.AddRange(itemList.FindAll(r => r.MeasureId == item.MeasureId));
                        if (history.List.Any())
                            list.Add(history);
                    }
                    else
                    {
                        history.Childer = new List<CecHistoryModel>();
                        foreach (MeasureHeaderModel sub in MeasureList.FindAll(r => r.ParentId == item.MeasureId && r.MeasureId != r.ParentId))
                        {
                            CecHistoryModel subHistory = new CecHistoryModel();
                            subHistory.MeasureId = sub.MeasureId;
                            subHistory.MeasureName = sub.Name;
                            subHistory.Links = sub.Links;
                            subHistory.List = new List<CecItemModel>();
                            subHistory.List.AddRange(itemList.FindAll(r => r.MeasureId == sub.MeasureId));
                            if (subHistory.List.Any())
                                history.Childer.Add(subHistory);
                        }
                        if (history.Childer.Any())
                            list.Add(history);
                    }
                }

                response.Data = list;

            }
            else
            {
                response.Message = string.Empty;
                response.Data = string.Empty;
            }
            return JsonHelper.SerializeObject(response);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.CEC, Anonymity = Anonymous.Verified, Parameter = "assessmentId")]
        public string Save(int teacherId, int assessmentId, DateTime assessmentDate, Wave wave, string itemAnswer)
        {
            var response = new PostFormResponse();
            if (ModelState.IsValid)
            {
                List<CecAnswerModel> answerList = _cecBusiness.GetCecAnswerList(assessmentId);

                bool history = _cecBusiness.CheckCecHistory(assessmentId, wave, teacherId);
                if (history) response.Success = false;
                else
                {
                    CecHistoryEntity entity = new CecHistoryEntity();
                    entity.TeacherId = teacherId;
                    entity.AssessmentId = assessmentId;
                    entity.AssessmentDate = assessmentDate;
                    entity.Wave = wave;
                    entity.SchoolYear = CommonAgent.SchoolYear;

                    string[] itemArray = itemAnswer.Split('&');
                    List<CecResultEntity> cecResult = new List<CecResultEntity>();
                    foreach (var s in itemArray)
                    {
                        if (!string.IsNullOrEmpty(s))
                        {
                            //前台设置，第一个为ItemId，第二个为AnswerId
                            string[] tmp = s.Split(',');
                            CecAnswerModel tmpCecAnswerModel = answerList.Find(r => r.AnswerId == Convert.ToInt32(tmp[1]));
                            cecResult.Add(new CecResultEntity()
                            {
                                ItemId = Convert.ToInt32(tmp[0]),
                                AnswerId = Convert.ToInt32(tmp[1]),
                                Score = tmpCecAnswerModel.Score,
                                CreatedBy = UserInfo.ID,
                                UpdatedBy = UserInfo.ID
                            });
                        }
                    }

                    entity.CreatedBy = entity.UpdatedBy = UserInfo.ID;
                    entity.CecResults = cecResult;

                    OperationResult result = _cecBusiness.InsertCecHistory(entity);
                    response.Success = result.ResultType == OperationResultType.Success;
                    response.Message = result.Message;
                }
            }
            else
            {
                response.Success = false;
            }
            return JsonHelper.SerializeObject(response);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.CEC, Anonymity = Anonymous.Verified, Parameter = "assessmentId")]
        public string reset(int assessmentId, int teacherId, Wave wave)
        {
            var response = new PostFormResponse();
            if (!_adeBusiness.CanExecuteAssessment(assessmentId))
            {
                response.Message = GetInformation("assessment_unavaiable");
                return JsonHelper.SerializeObject(response);
            }
            response.Success = _cecBusiness.Reset(assessmentId, teacherId, wave).ResultType == OperationResultType.Success;
            HttpContext.Cache.Remove(CacheKey(teacherId, assessmentId, wave, CommonAgent.Year));
            return JsonHelper.SerializeObject(response);
        }

        string CacheKey(int teacherId, int assessmentId, Wave wave, int year)
        {
            return string.Format("CEC_{0}_{1}_{2}_{3}", teacherId, assessmentId, (int)wave, year);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.CEC, Anonymity = Anonymous.Verified, Parameter = "assessmentId")]
        public ActionResult CECReport(int teacherId, int assessmentId, Wave wave, int year, bool export = false)
        {
            var assessment = _adeBusiness.GetBaseAssessmentModel(assessmentId);
            if (assessment == null) return new EmptyResult();
            ViewBag.AssessmentName = assessment.Name;

            if (!_adeBusiness.CanExecuteAssessment(assessmentId))
                return RedirectToAction("Index", "Dashboard", new { Area = "", showMessage = "assessment_unavaiable" });

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

            ViewBag.CommunityName = string.Join(", ",
                teacherEntity.UserInfo.UserCommunitySchools.Select(x => x.Community.Name));
            var schoolNames = teacherEntity.UserInfo.UserCommunitySchools.Where(r => r.SchoolId > 0).Select(x => x.School.Name).ToList();
            ViewBag.SchoolName = string.Join(", ", schoolNames);

            ViewBag.Mentor = _userBusiness.GetCoordCoachByUserId(teacherEntity.CountyId);
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

        private void GetPdf(string html, string fileName)
        {
            PdfProvider pdfProvider = new PdfProvider(PdfType.Assessment_Portrait);
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

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.CEC, Anonymity = Anonymous.Verified)]
        public string GetCoachSelectList(int communityId = 0)
        {
            IEnumerable<SelectListItem> list = null;

            if (communityId == 0)
            {
                if (UserInfo.Role == Role.Super_admin)
                {
                    list = _userBusiness.GetMentor_Coachs().ToSelectList(ViewTextHelper.DefaultAllText, "0");
                }
                else
                {
                    list = _userBusiness.GetMentor_CoachsByUserId(UserInfo.ID)
                        .ToSelectList(ViewTextHelper.DefaultAllText, "0");
                }
            }
            else
                list = _userBusiness.GetCoachCoordByCommunity(communityId)
                .ToSelectList(ViewTextHelper.DefaultAllText, "0");

            return JsonHelper.SerializeObject(list);
        }
    }
}