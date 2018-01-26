using System.Web.Optimization;
using Sunnet.Cli.Assessment.Controllers;
using Sunnet.Cli.Assessment.Models;
using Sunnet.Cli.Business.Ade;
using Sunnet.Cli.Business.Cec;
using Sunnet.Cli.Business.Communities;
using Sunnet.Cli.Business.Cpalls;
using Sunnet.Cli.Business.Schools;
using Sunnet.Cli.Business.Users;
using Sunnet.Cli.Core.Cec.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sunnet.Cli.Business.Common;
using Sunnet.Cli.Core.Common.Enums;
using Sunnet.Cli.Core.Users.Enums;
using Sunnet.Cli.Business.Cec.Model;
using Sunnet.Cli.Core.Cpalls;
using Sunnet.Cli.Business.Ade.Models;
using Sunnet.Cli.Business.Cpalls.Models;
using System.Linq.Expressions;
using Sunnet.Cli.Core.Cec.Entities;
using Sunnet.Framework.Helpers;
using Sunnet.Cli.Core.Ade.Entities;
using Sunnet.Cli.UIBase.Models;
using Sunnet.Cli.UIBase.Filters;
using Sunnet.Framework.Permission;
using Sunnet.Cli.Business;
using Sunnet.Cli.Core.Users.Entities;
using Sunnet.Framework.Core.Tool;
using Sunnet.Cli.UIBase;
using System.Text;
using CecItemModel = Sunnet.Cli.Business.Cec.Model.CecItemModel;

namespace Sunnet.Cli.Assessment.Areas.Cec.Controllers
{
    public class OfflineController : BaseController
    {

        private readonly UserBusiness _userBusiness;
        private readonly CommunityBusiness _communityBusiness;
        private readonly SchoolBusiness _schoolBusiness;
        private readonly AdeBusiness _adeBusiness;
        private readonly CecBusiness _cecBusiness;
        private readonly CpallsBusiness _cpallsBusiness;

        public OfflineController()
        {
            _userBusiness = new UserBusiness(UnitWorkContext);
            _communityBusiness = new CommunityBusiness(UnitWorkContext);
            _schoolBusiness = new SchoolBusiness(UnitWorkContext);

            _adeBusiness = new AdeBusiness(AdeUnitWorkContext);
            _cecBusiness = new CecBusiness(AdeUnitWorkContext);
            _cpallsBusiness = new CpallsBusiness(AdeUnitWorkContext);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.CEC, Anonymity = Anonymous.Verified)]
        public ActionResult Index()
        {
            ViewBag.LoginUrl = BuilderLoginUrl(LoginUserType.GOOGLEACCOUNT, LoginIASID.CEC_OFFLINE);
            ViewBag.CLIUserLogin = BuilderLoginUrl(LoginUserType.UTACCESSMANAGER, LoginIASID.CEC_OFFLINE);
            OfflineUrlModel model = Session["_CEC_Offline_URL"] as OfflineUrlModel;

            return View();
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.CEC, Anonymity = Anonymous.Verified)]
        public ActionResult Preparing()
        {
            OfflineUrlModel model = Session["_CEC_Offline_URL"] as OfflineUrlModel;
            if (model != null)
            {
                int assessmentId = model.assessmentId;
                int communityId = model.communityId;
                string communityName = model.communityName;
                string schoolName = model.schoolName;
                int schoolId = model.schoolId;

                AssessmentEntity assessmentEntity = _adeBusiness.GetAssessment(assessmentId);
                if (assessmentEntity == null || assessmentEntity.Status != Core.Common.Enums.EntityStatus.Active || assessmentEntity.IsDeleted)
                    return RedirectToAction("Index", "Dashboard", new { Area = "", showMessage = "assessment_unavaiable" });

                _adeBusiness.LockAssessment(assessmentId);
                ViewBag.AssessmentName = assessmentEntity.Name;
                ViewBag.AssessmentID = assessmentEntity.ID;

                if (communityId == 0)
                    ViewBag.CommunityName = "ALL";
                else
                    ViewBag.CommunityName = communityName;

                if (schoolId == 0)
                    ViewBag.SchoolName = "ALL";
                else
                    ViewBag.SchoolName = schoolName;

                ViewBag.SchoolYear = CommonAgent.SchoolYear;
            }

            return View();
        }

        private static string ManifestContent
        {
            get
            {
                var hash = BundleConfig.UpdateKey;

                var fileList = new List<string>();
                ProcessStaticFiles(fileList);

                var cacheList = string.Join("\n", fileList);
                var fallback = "/ /Offline/Index/Offline";
                var network = "*";
                var content = string.Format("CACHE MANIFEST\n# hash:{0}\nCACHE:\n{1}\nFALLBACK:\n{2}\nNETWORK:\n{3}", hash,
                    cacheList, fallback, network);

                return content;
            }
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.CEC, Anonymity = Anonymous.Verified)]
        public FileResult Manifest()
        {
            byte[] by = Encoding.UTF8.GetBytes(ManifestContent);
            return File(by, "text/cache-manifest");
        }

        private static void ProcessStaticFiles(List<string> fileList)
        {
            fileList.Add("/Cec/Offline/Measure");
            fileList.Add("/Cec/Offline/CECReport");

            fileList.AddRange(OfflineHelper.GlobalResources);
            fileList.AddRange(OfflineHelper.DatetimeBoxResources);

            var cecList = new List<string>()
            {
                "~/scripts/modernizr/offline",
                "~/scripts/jquery/offline",
                "~/scripts/bootstrap/offline",
                "~/scripts/knockout/offline",
                "~/scripts/cli/offline",
                "~/scripts/cec/offline",
                "~/scripts/format/offline",
                "~/scripts/jquery_val/offline",
                "~/css/basic/offline"
            };
#if DEBUG
            cecList.ForEach(resource =>
            {
                var content = resource.StartsWith("~/scripts")
                    ? Scripts.Render(resource).ToString()
                    : Styles.Render(resource).ToString();
                fileList.AddRange(OfflineHelper.SplitResources(content));
            });
#endif
#if !DEBUG
            fileList.AddRange(cecList.Select(resource => resource.Replace("~", "") + "?v=" + BundleConfig.UpdateKey));    
#endif

        }

        protected List<CecSchoolTeacherModel> GetTeacherModels(string teacherCode, string firstName, string lastName,
            int year, int assessmentId,
            int communityId, int schoolId, string sort, string order, int first, int count,
            out int total)
        {
            int? coach = null;
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

            return list;
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.CEC, Anonymity = Anonymous.Verified)]
        public string GetOfflineData()
        {
            var total = 0;
            int coachId = 0;

            if (UserInfo.Role == Role.Mentor_coach || UserInfo.Role == Role.Coordinator)
                coachId = UserInfo.ID;

            CECOfflineModel offlineModel = new CECOfflineModel();
            offlineModel.CECItemList = new List<CecHistoryModel>();
            offlineModel.TeacherList = new List<CECOfflineTeacherModel>();

            OfflineUrlModel model = Session["_CEC_Offline_URL"] as OfflineUrlModel;
            if (model != null)
            {
                int assessmentId = model.assessmentId;
                string teacherCode = model.teacherCode;
                string firstName = model.firstName;
                string lastName = model.lastName;
                int year = 2014;
                int communityId = model.communityId;
                string communityName = model.communityName;
                string schoolName = model.schoolName;
                int schoolId = model.schoolId;
                string sort = model.sort;
                string order = model.order;
                int first = model.first; int count = model.count;

                ///目前cec 各个Wave是一样的数据
                List<CecItemModel> itemList = _cecBusiness.GetCecItemModel(assessmentId, Wave.BOY);

                if (itemList != null && itemList.Any())
                {
                    //表头
                    List<MeasureHeaderModel> MeasureList;
                    List<MeasureHeaderModel> ParentMeasureList;

                    _cpallsBusiness.BuilderHeader(assessmentId
                        , CommonAgent.Year, Wave.BOY
                        , out MeasureList, out ParentMeasureList);

                    ///获取Measure 的 Links
                    List<int> tmpMeasureIds = MeasureList.Select(r => r.MeasureId).ToList();
                    tmpMeasureIds.AddRange(ParentMeasureList.Select(r => r.MeasureId).ToList());
                    int[] ids = tmpMeasureIds.Distinct().ToArray();
                    List<AdeLinkEntity> measureLinks = _adeBusiness.GetLinks<MeasureEntity>(ids);
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
                                offlineModel.CECItemList.Add(history);
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
                                offlineModel.CECItemList.Add(history);
                        }
                    }

                    //获取Items 的Links
                    ids = itemList.Select(r => r.ItemId).Distinct().ToArray();
                    List<AdeLinkEntity> itemLinks = _adeBusiness.GetLinks<ItemBaseEntity>(ids);

                    foreach (CecItemModel item in itemList)
                    {
                        item.Links = new List<AdeLinkEntity>();
                        item.Links.AddRange(itemLinks.FindAll(r => r.HostId == item.ItemId));
                    }

                    List<CecSchoolTeacherModel> list = GetTeacherModels(teacherCode, firstName, lastName, year, assessmentId, communityId, schoolId, sort, order, first, count, out total);

                    //遍历老师，绑定 Items 数据
                    foreach (var teacher in list)
                    {
                        CECOfflineTeacherModel offLineTeacher = new CECOfflineTeacherModel(teacher);

                        offLineTeacher.BOYHistory = _cecBusiness.GetCecResultModels(r => r.CecHistory.AssessmentId == assessmentId && r.CecHistory.SchoolYear == CommonAgent.SchoolYear
                      && r.CecHistory.Wave == Wave.BOY && r.CecHistory.TeacherId == teacher.ID);
                        if (offLineTeacher.BOYHistory != null && offLineTeacher.BOYHistory.Any())
                            offLineTeacher.BOYStatus = (int)CECStatus.Complete;

                        offLineTeacher.MOYHistory = _cecBusiness.GetCecResultModels(r => r.CecHistory.AssessmentId == assessmentId && r.CecHistory.SchoolYear == CommonAgent.SchoolYear
                            && r.CecHistory.Wave == Wave.MOY && r.CecHistory.TeacherId == teacher.ID);
                        if (offLineTeacher.MOYHistory != null && offLineTeacher.MOYHistory.Any())
                            offLineTeacher.MOYStatus = (int)CECStatus.Complete;

                        offLineTeacher.EOYHistory = _cecBusiness.GetCecResultModels(r => r.CecHistory.AssessmentId == assessmentId && r.CecHistory.SchoolYear == CommonAgent.SchoolYear
                            && r.CecHistory.Wave == Wave.EOY && r.CecHistory.TeacherId == teacher.ID);
                        if (offLineTeacher.EOYHistory != null && offLineTeacher.EOYHistory.Any())
                            offLineTeacher.EOYStatus = (int)CECStatus.Complete;

                        offlineModel.TeacherList.Add(offLineTeacher);
                    }
                }
            }
            return JsonHelper.SerializeObject(offlineModel);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.CEC, Anonymity = Anonymous.Verified)]
        public ActionResult Measure()
        {
            OfflineUrlModel model = Session["_CEC_Offline_URL"] as OfflineUrlModel;
            return View();
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.CEC, Anonymity = Anonymous.Verified)]
        public ActionResult CECReport()
        {
            OfflineUrlModel model = Session["_CEC_Offline_URL"] as OfflineUrlModel;
            return View();
        }


        private DateTime GetMaxDate(params DateTime[] args)
        {
            var max = args.Max();
            if (max < CommonAgent.MinDate.AddDays(1)) max = DateTime.Now;
            if (max > DateTime.Now) max = DateTime.Now;
            return max;
        }

        private DateTime GetMinDate(params DateTime[] args)
        {
            var min = args.Min();
            if (min < CommonAgent.MinDate.AddDays(1)) min = DateTime.Now;
            return min;
        }


        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.CEC, Anonymity = Anonymous.Verified)]
        public string Sync(int teacherId, int assessmentId, string boyhistory, string boydate, int boyStatus
            , string moyhistory, string moydate, int moyStatus, string eoyhistory, string eoydate, int eoyStatus)
        {

            List<CecResultModel> boyhistoryList = JsonHelper.DeserializeObject<List<CecResultModel>>(boyhistory);
            List<CecResultModel> moyhistoryList = JsonHelper.DeserializeObject<List<CecResultModel>>(moyhistory);
            List<CecResultModel> eoyhistoryList = JsonHelper.DeserializeObject<List<CecResultModel>>(eoyhistory);

            CECofflineResult result = new CECofflineResult();
            List<CecHistoryEntity> list = new List<CecHistoryEntity>();
            if (boyStatus == (int)CECStatus.OfflineComplete)
            {
                result.boy = 1;
                bool history = _cecBusiness.CheckCecHistory(assessmentId, Wave.BOY, teacherId);
                if (history) result.boy = 0;
                else
                {
                    CecHistoryEntity entityBOY = new CecHistoryEntity();
                    entityBOY.TeacherId = teacherId;
                    entityBOY.AssessmentId = assessmentId;
                    entityBOY.AssessmentDate = DateTime.Parse(boydate);
                    entityBOY.Wave = Wave.BOY;
                    entityBOY.SchoolYear = CommonAgent.SchoolYear;
                    entityBOY.CecResults = new List<CecResultEntity>();
                    entityBOY.CreatedOn = GetMinDate(boyhistoryList.Select(x => x.CreatedOn).FirstOrDefault());
                    entityBOY.UpdatedOn = GetMaxDate(boyhistoryList.Select(x => x.UpdatedOn).FirstOrDefault());
                    foreach (CecResultModel item in boyhistoryList)
                    {
                        entityBOY.CecResults.Add(new CecResultEntity()
                        {
                            ItemId = item.ItemId,
                            AnswerId = item.AnswerId,
                            Score = item.Score,
                            CreatedBy = UserInfo.ID,
                            UpdatedBy = UserInfo.ID,
                            CreatedOn = item.CreatedOn,
                            UpdatedOn = item.UpdatedOn
                        });
                    }

                    entityBOY.CreatedBy = entityBOY.UpdatedBy = UserInfo.ID;
                    list.Add(entityBOY);
                }
            }

            if (moyStatus == (int)CECStatus.OfflineComplete)
            {
                result.moy = 1;
                bool history = _cecBusiness.CheckCecHistory(assessmentId, Wave.MOY, teacherId);
                if (history) result.moy = 0;
                else
                {
                    CecHistoryEntity entityMOY = new CecHistoryEntity();
                    entityMOY.TeacherId = teacherId;
                    entityMOY.AssessmentId = assessmentId;
                    entityMOY.AssessmentDate = DateTime.Parse(moydate);
                    entityMOY.Wave = Wave.MOY;
                    entityMOY.SchoolYear = CommonAgent.SchoolYear;
                    entityMOY.CecResults = new List<CecResultEntity>();
                    entityMOY.CreatedOn = GetMinDate(moyhistoryList.Select(x => x.CreatedOn).FirstOrDefault());
                    entityMOY.UpdatedOn = GetMaxDate(moyhistoryList.Select(x => x.UpdatedOn).FirstOrDefault());
                    foreach (CecResultModel item in moyhistoryList)
                    {
                        entityMOY.CecResults.Add(new CecResultEntity()
                        {
                            ItemId = item.ItemId,
                            AnswerId = item.AnswerId,
                            Score = item.Score,
                            CreatedBy = UserInfo.ID,
                            UpdatedBy = UserInfo.ID,
                            CreatedOn = item.CreatedOn,
                            UpdatedOn = item.UpdatedOn
                        });
                    }

                    entityMOY.CreatedBy = entityMOY.UpdatedBy = UserInfo.ID;
                    list.Add(entityMOY);
                }
            }

            if (eoyStatus == (int)CECStatus.OfflineComplete)
            {
                result.eoy = 1;
                bool history = _cecBusiness.CheckCecHistory(assessmentId, Wave.EOY, teacherId);
                if (history) result.eoy = 0;
                else
                {
                    CecHistoryEntity entityEOY = new CecHistoryEntity();
                    entityEOY.TeacherId = teacherId;
                    entityEOY.AssessmentId = assessmentId;
                    entityEOY.AssessmentDate = DateTime.Parse(eoydate);
                    entityEOY.Wave = Wave.EOY;
                    entityEOY.SchoolYear = CommonAgent.SchoolYear;
                    entityEOY.CecResults = new List<CecResultEntity>();
                    entityEOY.CreatedOn = GetMinDate(eoyhistoryList.Select(x => x.CreatedOn).FirstOrDefault());
                    entityEOY.UpdatedOn = GetMaxDate(eoyhistoryList.Select(x => x.UpdatedOn).FirstOrDefault());
                    foreach (CecResultModel item in eoyhistoryList)
                    {
                        entityEOY.CecResults.Add(new CecResultEntity()
                        {
                            ItemId = item.ItemId,
                            AnswerId = item.AnswerId,
                            Score = item.Score,
                            CreatedBy = UserInfo.ID,
                            UpdatedBy = UserInfo.ID,
                            CreatedOn = item.CreatedOn,
                            UpdatedOn = item.UpdatedOn
                        });
                    }

                    entityEOY.TotalScore = entityEOY.CecResults.Sum(r => r.Score);

                    entityEOY.CreatedBy = entityEOY.UpdatedBy = UserInfo.ID;
                    list.Add(entityEOY);
                }
            }

            if (list.Count == 0)
                result.Success = 0;
            else
            {
                OperationResult historyResult = _cecBusiness.InsertCecHistory(list);
                result.Success = historyResult.ResultType == OperationResultType.Success ? 1 : 0;
            }
            return JsonHelper.SerializeObject(result);
        }

        public string Online()
        {
            var online = new
            {
                online = true,
                date = DateTime.Now.ToString("HH:mm:ss"),
                logged = UserInfo != null
            };
            return JsonHelper.SerializeObject(online);
        }

        public string Offline()
        {
            return "false";
        }


    }

    class CECofflineResult
    {
        public int Success { get; set; }
        public int boy { get; set; }
        public int moy { get; set; }
        public int eoy { get; set; }
    }

}