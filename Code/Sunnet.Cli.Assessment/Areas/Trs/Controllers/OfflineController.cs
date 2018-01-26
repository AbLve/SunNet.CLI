using Sunnet.Cli.Assessment.Controllers;
using Sunnet.Cli.Assessment.Models;
using Sunnet.Cli.Business.Common;
using Sunnet.Cli.Business.Schools;
using Sunnet.Cli.Business.Trs;
using Sunnet.Cli.Business.Trs.Models;
using Sunnet.Cli.Core.Common.Enums;
using Sunnet.Cli.Core.Schools.Entities;
using Sunnet.Cli.Core.Trs;
using Sunnet.Cli.Core.Trs.Entities;
using Sunnet.Cli.Core.Users.Enums;
using Sunnet.Cli.UIBase;
using Sunnet.Cli.UIBase.Filters;
using Sunnet.Cli.UIBase.Models;
using Sunnet.Framework.Core.Tool;
using Sunnet.Framework.Helpers;
using Sunnet.Framework.Permission;
using Sunnet.Framework.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Optimization;
using Sunnet.Cli.Business.TRSClasses;

namespace Sunnet.Cli.Assessment.Areas.Trs.Controllers
{
    public class OfflineController : BaseController
    {
        private readonly SchoolBusiness _schoolBusiness;
        private readonly TrsBusiness _trsBusiness;
        private readonly TRSClassBusiness _trsClassBusiness;
        public OfflineController()
        {
            _schoolBusiness = new SchoolBusiness(UnitWorkContext);
            _trsBusiness = new TrsBusiness(AdeUnitWorkContext);
            _trsClassBusiness = new TRSClassBusiness(UnitWorkContext);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.TRS, Anonymity = Anonymous.Verified)]
        public ActionResult Index()
        {
            ViewBag.LoginUrl = BuilderLoginUrl(LoginUserType.GOOGLEACCOUNT, LoginIASID.TRS_OFFLINE);
            ViewBag.CLIUserLogin = BuilderLoginUrl(LoginUserType.UTACCESSMANAGER, LoginIASID.TRS_OFFLINE);

            return View();
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.TRS, Anonymity = Anonymous.Verified)]
        public FileResult Manifest()
        {
            byte[] by = Encoding.UTF8.GetBytes(ManifestContent);
            return File(by, "text/cache-manifest");
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.TRS, Anonymity = Anonymous.Verified)]
        public ActionResult Preparing()
        {
            return View();
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.TRS, Anonymity = Anonymous.Verified)]
        public string GetOfflineData()
        {
            int total = 0;
            TrsOfflineModel offlineModel = new TrsOfflineModel();
            OfflineUrlModel model = Session["_TRS_Offline_URL"] as OfflineUrlModel;
            if (model != null)
            {
                int communityId = model.communityId;
                int schoolId = model.schoolId;
                string schoolName = model.schoolName;
                string director = model.director;
                string sort = model.sort;
                string order = model.order;
                int first = model.first;
                int count = model.count;

                //搜索School列表

                #region 搜索条件
                var condition = PredicateHelper.True<SchoolEntity>();
                if (communityId > 0)
                    condition = condition.And(x =>
                                x.CommunitySchoolRelations.Any(
                                    c => c.CommunityId == communityId && c.Status == EntityStatus.Active));

                if (schoolId > 0) condition = condition.And(x => x.ID == schoolId);

                if (UserInfo.Role == Role.Super_admin)
                {
                    // view/ assessment for all schools 
                    // no condition
                }
                else if (UserInfo.Role == Role.Community
                    || UserInfo.Role == Role.District_Community_Specialist
                    || UserInfo.Role == Role.Statewide)
                {
                    condition = condition.And(r => r.CommunitySchoolRelations.Any(
                        x => x.Community.UserCommunitySchools.Any(
                            y => y.UserId == UserInfo.ID)));
                }
                else if (UserInfo.Role == Role.District_Community_Delegate
                    || UserInfo.Role == Role.Community_Specialist_Delegate)
                {
                    int parentId = UserInfo.CommunityUser == null ? 0 : UserInfo.CommunityUser.ParentId;
                    condition = condition.And(r => r.CommunitySchoolRelations.Any(
                        x => x.Community.UserCommunitySchools.Any(
                            y => y.UserId == parentId)));
                }
                // For a school, allow all TRS Specialist users under that school to view all TRS reports under that school. 
                else if (UserInfo.Role == Role.Principal || UserInfo.Role == Role.TRS_Specialist)
                    condition = condition.And(x => x.UserCommunitySchools.Any(p => p.UserId == UserInfo.ID));
                else if (UserInfo.Role == Role.Principal_Delegate || UserInfo.Role == Role.TRS_Specialist_Delegate)
                {
                    int parentId = UserInfo.Principal == null ? 0 : UserInfo.Principal.ParentId;
                    condition = condition.And(x => x.UserCommunitySchools.Any(p => p.UserId == parentId));
                }
                else
                    condition = condition.And(x => false);

                if (!string.IsNullOrEmpty(schoolName)) condition = condition.And(x => x.BasicSchool.Name.Contains(schoolName));
                if (!string.IsNullOrEmpty(director))
                    condition = condition.And(x => x.UserCommunitySchools.Any(p => p.User.FirstName.Contains(director))
                                                   || x.UserCommunitySchools.Any(p => p.User.LastName.Contains(director)));
                #endregion

                List<TrsOfflineSchoolModel> schools = _schoolBusiness.GetTrsOfflineSchools(condition, UserInfo, sort, order, first, count, out total);
                if (schools != null && schools.Count > 0)
                {
                    foreach (TrsOfflineSchoolModel item in schools)
                    {
                        item.UpdateAction(UserInfo);
                        item.Classes = _trsClassBusiness.GetTrsClasses(UserInfo, item.ID);

                        string minAgeGroup = "";//最小年龄段
                        string maxAgeGroup = "";//最大年龄段
                        _trsClassBusiness.GetTrsClassesReport(UserInfo, item.ID, out minAgeGroup, out maxAgeGroup);
                        item.MinAgeGroup = minAgeGroup;
                        item.MaxAgeGroup = maxAgeGroup;

                        item.AssessmentList = _trsBusiness.GetAssessmentModelsBySchool(item, UserInfo);
                        foreach (TrsAssessmentModel assessment in item.AssessmentList)
                        {
                            assessment.UpdateAction(UserInfo);
                        }
                        item.NewAssessment = _trsBusiness.GetNewOfflineAssessmentModel(item, UserInfo);
                    }

                    List<int> schoolIds = schools.Select(r => r.ID).Distinct().ToList();
                    List<TrsAssessmentModel> assessments = _trsBusiness.GetLatestAssessmentBySchools(schoolIds);
                    foreach (TrsSchoolModel item in schools)
                    {
                        TrsAssessmentModel assessmentModel = assessments.Where(r => r.SchoolId == item.ID).OrderByDescending(r => r.Id).FirstOrDefault();
                        item.StarStatus = assessmentModel == null ? 0 : assessmentModel.Star;
                        item.VerifiedStar = assessmentModel == null ? 0 : assessmentModel.VerifiedStar;
                        item.RecertificationBy = assessmentModel == null ? CommonAgent.MinDate : assessmentModel.RecertificatedBy;
                    }

                    offlineModel.Schools = schools;
                }
                var AssessmentStructure = _trsBusiness.GetOfflineAssessment();
                offlineModel.AssessmentStructure = AssessmentStructure;
            }

            return JsonHelper.SerializeObject(offlineModel);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.TRS, Anonymity = Anonymous.Verified)]
        public ActionResult School()
        {
            return View();
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.TRS, Anonymity = Anonymous.Verified)]
        public ActionResult Assessment()
        {
            return View();
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.TRS, Anonymity = Anonymous.Verified)]
        public ActionResult VerifiedStar()
        {
            return View();
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.TRS, Anonymity = Anonymous.Verified)]
        public ActionResult Preview()
        {
            ViewBag.Preview = true;
            ViewBag.StarNotAvailable = ResourceHelper.GetRM().GetInformation("Trs_CalcStar_NeedCompleted");
            return View();
        }

        private static void ProcessStaticFiles(List<string> fileList)
        {
            fileList.Add("/Trs/Offline/");
            fileList.Add("/Trs/Offline/School");
            fileList.Add("/Trs/Offline/Assessment");
            fileList.Add("/Trs/Offline/VerifiedStar");
            fileList.Add("/Trs/Offline/Preview");

            fileList.AddRange(OfflineHelper.GlobalResources);
            fileList.AddRange(OfflineHelper.DatetimeBoxResources);
            fileList.Add("/Content/images/preview_bg.png");
            fileList.Add("/Content/images/trs_logo.jpg");

            var trsList = new List<string>()
            {
                "~/scripts/modernizr/offline",
                "~/scripts/jquery/offline",
                "~/scripts/bootstrap/offline",
                "~/scripts/knockout/offline",
                "~/scripts/cli/offline",
                "~/scripts/trs/offline",
                "~/scripts/format/offline",
                "~/scripts/jquery_val/offline",
                "~/css/basic/offline",
                "~/css/assessment/offline",
                "~/css/trs/offline"
            };
#if DEBUG
            trsList.ForEach(resource =>
            {
                var content = resource.StartsWith("~/scripts")
                    ? Scripts.Render(resource).ToString()
                    : Styles.Render(resource).ToString();
                fileList.AddRange(OfflineHelper.SplitResources(content));
            });
#endif
#if !DEBUG
            fileList.AddRange(trsList.Select(resource => resource.Replace("~", "") + "?v=" + BundleConfig.UpdateKey));    
#endif

        }

        private static string ManifestContent
        {
            get
            {
                var hash = BundleConfig.UpdateKey;

                var fileList = new List<string>();
                ProcessStaticFiles(fileList);

                var cacheList = string.Join("\n", fileList);
                var fallback = "/ /Trs/Offline/Offline";
                var network = "*";
                var content = string.Format("CACHE MANIFEST\n# hash:{0}\nCACHE:\n{1}\nFALLBACK:\n{2}\nNETWORK:\n{3}", hash,
                    cacheList, fallback, network);

                return content;
            }
        }

        public bool Sync(int schoolId, string trsTaStatus, string recertificationBy,
            string starStatus, string starDate, string verifiedStar, string trsLastStatusChange,
            string addAssessments, string deleteAssessments, string updatAssessments)
        {
            bool result = false;

            //更新School信息
            SchoolEntity schoolEntity = _schoolBusiness.GetSchool(schoolId);
            schoolEntity.TrsTaStatus = trsTaStatus;
            schoolEntity.RecertificatedBy = DateTime.Parse(recertificationBy);
            schoolEntity.StarStatus = (TRSStarEnum)int.Parse(starStatus);
            schoolEntity.StarDate = DateTime.Parse(starDate);
            schoolEntity.VSDesignation = (TRSStarEnum)int.Parse(verifiedStar);
            schoolEntity.TrsLastStatusChange = DateTime.Parse(trsLastStatusChange);
            _schoolBusiness.UpdateSchool(schoolEntity);

            //更新Assessment

            //新增
            if (!string.IsNullOrEmpty(addAssessments))
            {
                if (addAssessments.Length > 2)  //默认值为[]
                {
                    result = AddAssessments(schoolId, addAssessments);
                    if (!result)
                        return result;
                }
            }
            //删除
            if (!string.IsNullOrEmpty(deleteAssessments))
            {
                if (deleteAssessments.Length > 2)  //默认值为[]
                {
                    dynamic DeleteAssessments = JsonHelper.DeserializeObject<object>(deleteAssessments);
                    List<int> assessmentIds = new List<int>();
                    foreach (var item in DeleteAssessments)
                    {
                        assessmentIds.Add((int)item.Id);
                    }
                    if (assessmentIds.Count > 0)
                    {
                        result = _trsBusiness.DeleteOfflineAssessment(assessmentIds) > 0;
                        if (!result)
                            return result;
                    }
                }
            }

            //更改
            if (!string.IsNullOrEmpty(updatAssessments))
            {
                if (updatAssessments.Length > 2)  //默认值为[]
                {
                    dynamic UpdatedAssessments = JsonHelper.DeserializeObject<object>(updatAssessments);
                    result = UpdateAssessments(schoolId, updatAssessments);
                }
            }
            return result;
        }

        private bool AddAssessments(int schoolId, string addAssessments)
        {
            List<TRSAssessmentEntity> list_AddAssessments = new List<TRSAssessmentEntity>();
            dynamic AddAssessments = JsonHelper.DeserializeObject<object>(addAssessments);
            foreach (var item in AddAssessments)
            {
                TRSAssessmentEntity entity = new TRSAssessmentEntity();
                entity.SchoolId = schoolId;
                entity.Status = (TRSStatusEnum)item.Status.value;
                entity.Star = (TRSStarEnum)item.Star.value;
                entity.CreatedOn = DateTime.Parse((string)item.CreatedOn);
                entity.CreatedBy = UserInfo.ID;
                entity.UpdatedOn = DateTime.Parse((string)item.UpdatedOn);
                entity.UpdatedBy = UserInfo.ID;
                entity.Type = (TrsAssessmentType)item.Type.value;
                entity.VisitDate = DateTime.Parse((string)item.VisitDate) < CommonAgent.MinDate ?
                    CommonAgent.MinDate : DateTime.Parse((string)item.VisitDate);
                entity.DiscussDate = DateTime.Parse((string)item.DiscussDate) < CommonAgent.MinDate ?
                    CommonAgent.MinDate : DateTime.Parse((string)item.DiscussDate);
                entity.ApproveDate = DateTime.Parse((string)item.ApproveDate) <= CommonAgent.MinDate ?
                    CommonAgent.TrsMinDate : DateTime.Parse((string)item.ApproveDate);
                entity.RecertificatedBy = DateTime.Parse((string)item.RecertificatedBy) <= CommonAgent.MinDate ?
                    CommonAgent.TrsMinDate.AddYears(3) : DateTime.Parse((string)item.RecertificatedBy);
                entity.TAStatus = item.TaStatus.ToString();
                entity.VerifiedStar = (TRSStarEnum)item.VerifiedStar.value;
                entity.IsDeleted = false;
                entity.EventLogType = 0;

                List<TrsStarEntity> stars = new List<TrsStarEntity>();
                foreach (var category in item.Categories_Offline)
                {
                    foreach (var subCategory in category)
                    {
                        foreach (var items in subCategory)
                        {
                            foreach (var item1 in items.Value)
                            {
                                TRSAssessmentItemEntity assessmentItem = new TRSAssessmentItemEntity();
                                assessmentItem.ClassId = item1.ClassId;
                                assessmentItem.ItemId = item1.ItemId;
                                assessmentItem.AnswerId = item1.AnswerId;
                                assessmentItem.Comments = item1.Comments;
                                assessmentItem.CreatedOn = DateTime.Parse((string)item1.CreatedOn);
                                assessmentItem.CreatedBy = UserInfo.ID;
                                assessmentItem.UpdatedOn = DateTime.Parse((string)item1.UpdatedOn);
                                assessmentItem.UpdatedBy = UserInfo.ID;
                                entity.AssessmentItems.Add(assessmentItem);
                            }
                        }
                    }
                }

                foreach (var class1 in item.Classes)
                {
                    foreach (var classCategory in class1.Categories_Offline)
                    {
                        foreach (var classSubCategory in classCategory)
                        {
                            foreach (var items in classSubCategory)
                            {
                                foreach (var item2 in items.Value)
                                {
                                    TRSAssessmentItemEntity assessmentItem = new TRSAssessmentItemEntity();
                                    assessmentItem.ClassId = item2.ClassId;
                                    assessmentItem.ItemId = item2.ItemId;
                                    assessmentItem.AnswerId = item2.AnswerId;
                                    assessmentItem.Comments = item2.Comments;
                                    assessmentItem.CreatedOn = DateTime.Parse((string)item2.CreatedOn);
                                    assessmentItem.CreatedBy = UserInfo.ID;
                                    assessmentItem.UpdatedOn = DateTime.Parse((string)item2.UpdatedOn);
                                    assessmentItem.UpdatedBy = UserInfo.ID;
                                    entity.AssessmentItems.Add(assessmentItem);
                                }
                            }
                        }
                    }
                    TRSAssessmentClassEntity assessmentClass = new TRSAssessmentClassEntity();
                    assessmentClass.ClassId = class1.Id;
                    assessmentClass.ObservationLength = string.IsNullOrEmpty(class1.ObservationLength.Value.ToString()) ? 0 : class1.ObservationLength;
                    entity.AssessmentClasses.Add(assessmentClass);
                }

                if (item.StarOfCategory != null)
                {
                    foreach (var star in item.StarOfCategory)
                    {
                        TrsStarEntity starEntity = new TrsStarEntity();
                        starEntity.Category = (TRSCategoryEnum)(int.Parse(star.Name));
                        starEntity.Star = (TRSStarEnum)star.Value.value;
                        entity.Stars.Add(starEntity);
                    }
                }

                list_AddAssessments.Add(entity);
            }
            if (list_AddAssessments.Count > 0)
            {
                return _trsBusiness.AddAssessments(list_AddAssessments, true).ResultType == OperationResultType.Success;
            }
            else
            {
                return false;
            }
        }

        private bool UpdateAssessments(int schoolId, string updateAssessments)
        {
            bool isSuccess = true;
            List<TRSAssessmentEntity> list_UpdateAssessments = new List<TRSAssessmentEntity>();
            dynamic UpdateAssessments = JsonHelper.DeserializeObject<object>(updateAssessments);
            foreach (var item in UpdateAssessments)
            {
                TRSAssessmentEntity entity = _trsBusiness.GetAssessment((int)item.Id);
                if (entity == null)
                {
                    return false;
                }
                else
                {
                    List<TRSAssessmentItemEntity> assessmentItems = entity.AssessmentItems.Where(r => r.Item.IsDeleted == false).ToList();
                    entity.UpdatedBy = UserInfo.ID;
                    entity.UpdatedOn = DateTime.Parse((string)item.UpdatedOn);
                    entity.TAStatus = item.TaStatus.ToString();
                    entity.VerifiedStar = (TRSStarEnum)item.VerifiedStar.value;
                    entity.IsDeleted = false;
                    entity.Type = (TrsAssessmentType)item.Type.value;
                    entity.Status = (TRSStatusEnum)item.Status.value;
                    entity.VisitDate = DateTime.Parse((string)item.VisitDate) < CommonAgent.MinDate ?
                    CommonAgent.MinDate : DateTime.Parse((string)item.VisitDate);
                    entity.DiscussDate = DateTime.Parse((string)item.DiscussDate) < CommonAgent.MinDate ?
                        CommonAgent.MinDate : DateTime.Parse((string)item.DiscussDate);
                    entity.ApproveDate = DateTime.Parse((string)item.ApproveDate) <= CommonAgent.MinDate ?
                    CommonAgent.TrsMinDate : DateTime.Parse((string)item.ApproveDate);
                    entity.RecertificatedBy = DateTime.Parse((string)item.RecertificatedBy) <= CommonAgent.MinDate ?
                        CommonAgent.TrsMinDate.AddYears(3) : DateTime.Parse((string)item.RecertificatedBy);

                    //删除AssessmentClasses
                    List<TRSAssessmentClassEntity> list_AssessmentClasses = entity.AssessmentClasses.ToList();
                    if (list_AssessmentClasses != null && list_AssessmentClasses.Count > 0)
                    {
                        _trsBusiness.DeleteAssessmentClasses(list_AssessmentClasses, false);
                    }

                    foreach (var category in item.Categories_Offline)
                    {
                        foreach (var subCategory in category)
                        {
                            foreach (var items in subCategory)
                            {
                                foreach (var item1 in items.Value)
                                {
                                    TRSAssessmentItemEntity assessmentItem = assessmentItems.Find(r => r.ItemId == (int)item1.ItemId && r.ClassId == (int)item1.ClassId);
                                    if (assessmentItem == null)
                                    {
                                        TRSAssessmentItemEntity itementity = new TRSAssessmentItemEntity();
                                        itementity.ClassId = item1.ClassId;
                                        itementity.ItemId = item1.ItemId;
                                        itementity.AnswerId = item1.AnswerId;
                                        itementity.Comments = item1.Comments;
                                        itementity.CreatedOn = DateTime.Parse((string)item1.CreatedOn);
                                        itementity.CreatedBy = UserInfo.ID;
                                        itementity.UpdatedOn = DateTime.Parse((string)item1.UpdatedOn);
                                        itementity.UpdatedBy = UserInfo.ID;
                                        entity.AssessmentItems.Add(itementity);
                                    }
                                    else
                                    {
                                        assessmentItem.UpdatedOn = DateTime.Parse((string)item1.UpdatedOn);
                                        assessmentItem.UpdatedBy = UserInfo.ID;
                                        assessmentItem.AnswerId = item1.AnswerId;
                                        assessmentItem.Comments = item1.Comments;
                                    }
                                }
                            }
                        }
                    }

                    foreach (var class1 in item.Classes)
                    {
                        foreach (var classCategory in class1.Categories_Offline)
                        {
                            foreach (var classSubCategory in classCategory)
                            {
                                foreach (var items in classSubCategory)
                                {
                                    foreach (var item2 in items.Value)
                                    {
                                        TRSAssessmentItemEntity assessmentItem = assessmentItems.Find(r => r.ItemId == (int)item2.ItemId && r.ClassId == (int)item2.ClassId);
                                        if (assessmentItem == null)
                                        {
                                            TRSAssessmentItemEntity itementity = new TRSAssessmentItemEntity();
                                            itementity.ClassId = item2.ClassId;
                                            itementity.ItemId = item2.ItemId;
                                            itementity.AnswerId = item2.AnswerId;
                                            itementity.Comments = item2.Comments;
                                            itementity.CreatedOn = DateTime.Parse((string)item2.CreatedOn);
                                            itementity.CreatedBy = UserInfo.ID;
                                            itementity.UpdatedOn = DateTime.Parse((string)item2.UpdatedOn);
                                            itementity.UpdatedBy = UserInfo.ID;
                                            entity.AssessmentItems.Add(itementity);
                                        }
                                        else
                                        {
                                            assessmentItem.UpdatedOn = DateTime.Parse((string)item2.UpdatedOn);
                                            assessmentItem.UpdatedBy = UserInfo.ID;
                                            assessmentItem.AnswerId = item2.AnswerId;
                                            assessmentItem.Comments = item2.Comments;
                                        }
                                    }
                                }
                            }
                        }
                        TRSAssessmentClassEntity assessmentClass = new TRSAssessmentClassEntity();
                        assessmentClass.ClassId = class1.Id;
                        assessmentClass.ObservationLength = string.IsNullOrEmpty(class1.ObservationLength.Value.ToString()) ? 0 : class1.ObservationLength;
                        entity.AssessmentClasses.Add(assessmentClass);
                    }
                }
                isSuccess = _trsBusiness.UpdateAssessment(entity).ResultType == OperationResultType.Success;
                if (!isSuccess)
                    break;
            }
            return isSuccess;
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
            return ResourceHelper.GetRM().GetInformation("Offline_Message");
        }

    }
}