using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using StructureMap;
using Sunnet.Cli.Assessment.Controllers;
using Sunnet.Cli.Business.Communities;
using Sunnet.Cli.Business.Communities.Enums;
using Sunnet.Cli.Business.Cpalls.Growth;
using Sunnet.Cli.Business.Schools;
using Sunnet.Cli.Business.Schools.Models;
using Sunnet.Cli.Business.Trs;
using Sunnet.Cli.Business.Trs.Models;
using Sunnet.Cli.Business.Users;
using Sunnet.Cli.Core.Common.Enums;
using Sunnet.Cli.Core.Communities.Entities;
using Sunnet.Cli.Core.Schools.Entities;
using Sunnet.Cli.Core.Trs;
using Sunnet.Cli.Core.Trs.Entities;
using Sunnet.Cli.Core.Users.Enums;
using Sunnet.Cli.UIBase.Filters;
using Sunnet.Cli.UIBase.Models;
using Sunnet.Framework;
using Sunnet.Framework.Helpers;
using Sunnet.Framework.Log;
using Sunnet.Framework.Permission;
using Sunnet.Framework.Resources;
using Sunnet.Cli.Core.Users.Entities;
using Sunnet.Cli.Business.Permission.Models;
using Sunnet.Cli.Business.Permission;
using Sunnet.Cli.Business.Common;
using Sunnet.Cli.Business.TRSClasses;
using Sunnet.Cli.Core.TRSClasses.Enums;
using System.Text.RegularExpressions;
using Sunnet.Cli.Core.Trs.Enums;
using Sunnet.Framework.Extensions;
using Sunnet.Framework.Core.Tool;
using System.Web.Script.Serialization;
using Sunnet.Cli.UIBase;
namespace Sunnet.Cli.Assessment.Areas.Trs.Controllers
{
    public class IndexController : BaseController
    {
        private readonly CommunityBusiness _communityBusiness = null;
        private readonly SchoolBusiness _schoolBusiness;
        private TRSClassBusiness _trsClassBusiness;
        private readonly TrsBusiness _trsBusiness;
        private readonly UserBusiness _userBusiness;
        private readonly ISunnetLog _logger;

        public IndexController()
        {
            _logger = ObjectFactory.GetInstance<ISunnetLog>();
            _communityBusiness = new CommunityBusiness();
            _schoolBusiness = new SchoolBusiness();
            _trsClassBusiness = new TRSClassBusiness();
            _trsBusiness = new TrsBusiness(AdeUnitWorkContext);
            _userBusiness = new UserBusiness();
        }

        // GET: Trs/Index
        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.TRS, Anonymity = Anonymous.Verified)]
        public ActionResult Index()
        {
            InitAccessOperation();
            if (UserInfo.Role == Role.Principal || UserInfo.Role == Role.Principal_Delegate)
            {
                var principal = UserInfo.Principal;
                if (principal != null)
                {
                    return RedirectToAction("School",
                        new { id = principal.UserInfo.UserCommunitySchools.Select(x => x.SchoolId).FirstOrDefault() });
                }
            }
            if (UserInfo.Role == Role.TRS_Specialist || UserInfo.Role == Role.TRS_Specialist_Delegate)
            {
                ViewBag.SchoolUrl = SFConfig.MainSiteDomain + "School/School/SchoolProfile/";
            }
            else
            {
                ViewBag.SchoolUrl = SFConfig.MainSiteDomain + "School/School/Edit?ID=";
            }

            bool trsOffline = false;

            UserAuthorityModel userAuthority = new PermissionBusiness().GetUserAuthority(UserInfo, (int)PagesModel.TRSPage);

            if (userAuthority != null)
            {
                if ((userAuthority.Authority & (int)Authority.Offline) == (int)Authority.Offline)
                {
                    trsOffline = true;
                }
            }

            ViewBag.TRSOffline = trsOffline;
            return View();
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.TRS, Anonymity = Anonymous.Verified)]
        public string Search(string schoolName, string director, int communityId = 0, int schoolId = 0,
            string sort = "BasicSchool.Name", string order = "ASC", int first = 0, int count = 10)
        {
            var total = 0;
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
            var schools = _schoolBusiness.GetTrsSchools(condition, UserInfo, sort, order, first, count, out total);
            schools.ForEach(x => x.UpdateAction(UserInfo));

            //显示最后一条Assessment的信息,最后一条EventLog信息
            if (schools.Count > 0)
            {
                List<int> schoolIds = schools.Select(r => r.ID).Distinct().ToList();
                List<TrsAssessmentModel> assessments = _trsBusiness.GetLatestAssessmentBySchools(schoolIds);

                List<TRSEventLogEntity> eventLogs = new List<TRSEventLogEntity>();
                if (sort != "ActionRequired")
                {
                    var expression = PredicateHelper.True<TRSEventLogEntity>();
                    expression = expression.And(r => schoolIds.Contains(r.SchoolId));
                    eventLogs = _trsBusiness.GetEventLogList(expression);
                }
                foreach (TrsSchoolModel item in schools)
                {
                    TrsAssessmentModel model = assessments.Where(r => r.SchoolId == item.ID).OrderByDescending(r => r.Id).FirstOrDefault();
                    item.StarStatus = model == null ? 0 : model.Star;
                    item.VerifiedStar = model == null ? 0 : model.VerifiedStar;
                    item.RecertificationBy = model == null ? CommonAgent.MinDate : model.RecertificatedBy;

                    if (sort != "ActionRequired")
                    {
                        TRSEventLogEntity eventLog = eventLogs.Where(r => r.SchoolId == item.ID && r.ActionRequired != CommonAgent.MinDate).OrderByDescending(r => r.ID).FirstOrDefault();
                        item.ActionRequired = eventLog == null ? CommonAgent.MinDate : eventLog.ActionRequired;
                    }
                }
            }

            return JsonHelper.SerializeObject(new { total = total, data = schools });
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.TRS, Anonymity = Anonymous.Verified)]
        public string GetCommunitySelectListForSearch(string keyword, int communityId = -1, bool isActiveCommunity = true)
        {
            var expression = PredicateHelper.True<CommunityEntity>();
            if (communityId > 0)
                expression = expression.And(o => o.ID == communityId);
            var list = _communityBusiness.GetCommunitySelectList(UserInfo, expression, isActiveCommunity);
            return JsonHelper.SerializeObject(list);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.TRS, Anonymity = Anonymous.Verified)]
        public string GetSchoolSelectList(string keyword, int communityId = 0, string schoolName = "", bool isActive = true)
        {
            var expression = PredicateHelper.True<SchoolEntity>();
            if (communityId > 0)
                expression = expression.And(s => s.CommunitySchoolRelations.Any(x => x.CommunityId == communityId && x.Status == EntityStatus.Active));
            if (schoolName != null && schoolName.Trim() != string.Empty)
                expression = expression.And(o => o.BasicSchool.Name.Contains(schoolName));
            var schoolList = _schoolBusiness.GetSchoolsSelectList(UserInfo, expression, isActive);
            return JsonHelper.SerializeObject(schoolList);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.TRS, Anonymity = Anonymous.Verified)]
        public ActionResult School(int id)
        {
            var schoolModel = _schoolBusiness.GetTrsSchool(id, UserInfo);
            if (schoolModel == null)
            {
                return View("SchoolNotAvailable");
            }
            schoolModel.Classes = _trsClassBusiness.GetTrsClasses(UserInfo, schoolModel.ID);
            schoolModel.UpdateAction(UserInfo);
            ViewBag.Assessments = _trsBusiness.GetAssessments(id, UserInfo, schoolModel);
            ViewBag.IsPrincipal = UserInfo.Role == Role.Principal;
            ViewBag.TrsEventType = TrsEventType.SIA_reconsideration.ToSelectList().AddDefaultItem(ViewTextHelper.DefaultPleaseSelectText, "");
            return View(schoolModel);
        }

        private bool TrsAvailable(TrsSchoolModel schoolModel)
        {
            if (schoolModel == null)
            {
                return false;
            }
            if (schoolModel.Classes == null)
            {
                schoolModel.Classes = _trsClassBusiness.GetTrsClasses(UserInfo, schoolModel.ID);
            }
            if (schoolModel.Classes == null || !schoolModel.Classes.Any())
            {
                return false;
            }
            if (schoolModel.Classes.Any(x => x.Ages == null || !x.Ages.Any()))
            {
                return false;
            }
            return true;
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.TRS, Anonymity = Anonymous.Verified)]
        public string TrsAvailable(int schoolId)
        {
            TrsSchoolModel schoolModel = _schoolBusiness.GetTrsSchool(schoolId, UserInfo);
            if (schoolModel == null)
            {
                return PostFormResponse.GetFailResponse("The school is not existed or you have no authorization to access the school you requested.").ToString();
            }

            var response = new PostFormResponse(true);

            schoolModel.UpdateAction(UserInfo);
            schoolModel.Classes = _trsClassBusiness.GetTrsClasses(UserInfo, schoolModel.ID);
            if (schoolModel.Classes == null || !schoolModel.Classes.Any())
            {
                response.Message = ResourceHelper.GetRM().GetInformation("Trs_School_Has_No_Class");
                response.Success = false;
                return response.ToString();
            }
            var classesHasNoType = schoolModel.Classes.Where(x => x.Ages == null || !x.Ages.Any()).ToList();

            if (!classesHasNoType.Any()) // success
                return response.ToString();

            // class has un filled properties, redirect edit class
            response.Data = classesHasNoType.Select(c => new { name = c.Name, id = c.Id });
            response.Message = ResourceHelper.GetRM()
                .GetInformation("Trs_TypeOfClassroom_Blank")
                .Replace("{0}", string.Join(", ", classesHasNoType.Select(x => x.Name)));
            response.Success = false;
            return response.ToString();
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.TRS, Anonymity = Anonymous.Verified)]
        public ActionResult Assessment(int schoolId, int id = 0)
        {
            TrsAssessmentModel assessment = null;
            assessment = id == 0 ?
                _trsBusiness.GetNewAssessmentModel(schoolId, UserInfo)
                : _trsBusiness.GetAssessmentModel(id, UserInfo);

            var list_Classes = assessment.AssessmentClasses;
            foreach (var item in assessment.Classes)
            {
                if (id > 0)
                {
                    var classModel = list_Classes.Where(r => r.ClassId == item.Id).FirstOrDefault();
                    if (classModel != null)
                    {
                        item.ObservationLength = classModel.ObservationLength;
                    }
                }
                string newName = item.Name;
                if (newName.Length >= 4 && newName.Substring(0, 4) == "TRS-")
                {
                    newName = newName.Substring(4);
                }
                if (item.Ages.Count() > 0)
                {
                    newName += " - ";
                    foreach (var age in item.Ages)
                    {
                        Regex r = new Regex(@"\d");
                        int index = r.Match(age.TypeOfChildren).Index;
                        newName += age.TypeOfChildren.Substring(0, index) + "/";
                    }
                }
                item.Name = newName.TrimEnd('/');
            }

            if (assessment == null)
            {
                return RedirectToAction("Index");
            }
            if (assessment.Status == TRSStatusEnum.Completed)
            {
                return View("Finalized", assessment);
            }
            if (!TrsAvailable(assessment.School))
            {
                return RedirectToAction("School", new { id = assessment.SchoolId });
            }

            assessment.School.UpdateAction(UserInfo);
            if (assessment.School.Action != "assessment" && assessment.School.Action != "viewAssessment")
            {
                return RedirectToAction("School", new { id = assessment.SchoolId });
            }
            ViewBag.IsSpecialist = assessment.School.Assessor.UserId == UserInfo.ID || UserInfo.Role == Role.Super_admin;
            ViewBag.IfCanSave = assessment.School.Assessor.UserId == UserInfo.ID || UserInfo.Role == Role.Super_admin
                || assessment.Classes.Any(r => r.TrsAssessorId == UserInfo.ID);
            ViewBag.Json = JsonHelper.SerializeObject(assessment);
            ViewBag.ItemAgeGroup = ItemAgeGroup.m_0_11.ToSelectList().AddDefaultItem(ViewTextHelper.DefaultPleaseSelectText, "");
            return View(assessment);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.TRS, Anonymity = Anonymous.Verified)]
        [HttpPost]
        public string SaveAssessment(TRSAssessmentEntity entity, string items, List<TrsTaStatus> TaStatuses, string observations, bool hasRetained)
        {
            var response = new PostFormResponse();
            try
            {
                if (ModelState.IsValid)
                {
                    entity.TAStatus = string.Join(",", TaStatuses.Where(x => x != 0).Select(x => (int)x)) ?? "";
                    var itemEntities = JsonHelper.DeserializeObject<List<TRSAssessmentItemEntity>>(items);
                    var assessmentClasses = JsonHelper.DeserializeObject<List<TRSAssessmentClassEntity>>(observations);
                    response.Update(_trsBusiness.SaveAssessment(entity, itemEntities, UserInfo, assessmentClasses, hasRetained));
                }
                else
                {
                    response.ModelState = ModelState;
                }
            }
            catch (Exception ex)
            {
                _logger.Debug(ex);
                response.Message = ex.Message;
#if DEBUG
                response.Message += "<br/>StackTrace:" + ex.StackTrace + "<br/>Source:" + ex.Source;
#endif
                response.Success = false;
                throw;
            }
            return JsonHelper.SerializeObject(response);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.TRS, Anonymity = Anonymous.Verified)]
        [HttpGet]
        public ActionResult RetainStar(int id, int schoolId)
        {
            TrsRecentStarModel model = _trsBusiness.GetRecentStarModel(id, schoolId, UserInfo);
            ViewBag.model = model;
            ViewBag.assessmentId = id;
            ViewBag.schoolId = schoolId;
            ViewBag.verifiedStar = (int)model.RecentVerifiedStar;
            return View();
        }

        public string RetainStar(int assessmentId, int category1, int category2, int category3, int category4, int category5, int verifiedStar, int[] chk)
        {
            List<int> autoAssginCategorys = new List<int>();
            if (category1 == (int)TRSStarDisplayEnum.AutoAssign)
            {
                autoAssginCategorys.Add(1);
            }
            if (category2 == (int)TRSStarDisplayEnum.AutoAssign)
            {
                autoAssginCategorys.Add(2);
            }
            if (category3 == (int)TRSStarDisplayEnum.AutoAssign)
            {
                autoAssginCategorys.Add(3);
            }
            if (category4 == (int)TRSStarDisplayEnum.AutoAssign)
            {
                autoAssginCategorys.Add(4);
            }
            if (category5 == (int)TRSStarDisplayEnum.AutoAssign)
            {
                autoAssginCategorys.Add(5);
            }
            category1 = category1 == (int)TRSStarDisplayEnum.NA || category1 == (int)TRSStarDisplayEnum.AutoAssign ? 0 : category1;
            category2 = category2 == (int)TRSStarDisplayEnum.NA || category2 == (int)TRSStarDisplayEnum.AutoAssign ? 0 : category2;
            category3 = category3 == (int)TRSStarDisplayEnum.NA || category3 == (int)TRSStarDisplayEnum.AutoAssign ? 0 : category3;
            category4 = category4 == (int)TRSStarDisplayEnum.NA || category4 == (int)TRSStarDisplayEnum.AutoAssign ? 0 : category4;
            category5 = category5 == (int)TRSStarDisplayEnum.NA || category5 == (int)TRSStarDisplayEnum.AutoAssign ? 0 : category5;

            var response = new PostFormResponse();
            OperationResult result = new OperationResult(OperationResultType.Success);
            result = _trsBusiness.InsertTrsStars(assessmentId, category1, category2, category3, category4, category5, verifiedStar, chk, autoAssginCategorys, UserInfo);
            response.Success = result.ResultType == OperationResultType.Success;
            response.Data = "";
            response.Message = result.Message;
            return JsonHelper.SerializeObject(response);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.TRS, Anonymity = Anonymous.Verified)]
        [HttpGet]
        public ActionResult VerifiedStar(int id, bool hasRecent = false, int type = 0, bool calcStar = true)
        {
            var model = _trsBusiness.GetPreviewModel(id, UserInfo, calcStar);
            ViewBag.type = type;
            ViewBag.hasRecent = hasRecent;
            return View(model);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.TRS, Anonymity = Anonymous.Verified)]
        [HttpPost]
        public string VerifiedStar(int id, TRSStarEnum verifiedStar)
        {
            var response = new PostFormResponse();
            response.Update(_trsBusiness.UpdateVerifiedStar(id, verifiedStar, UserInfo));
            return JsonHelper.SerializeObject(response);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.TRS, Anonymity = Anonymous.Verified)]
        [HttpPost]
        public string Invalidate(int id)
        {
            var response = new PostFormResponse();
            response.Update(_trsBusiness.Invalidate(id, UserInfo));
            return JsonHelper.SerializeObject(response);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.TRS, Anonymity = Anonymous.Verified)]
        [HttpPost]
        public string Delete(int id)
        {
            var response = new PostFormResponse();
            response.Update(_trsBusiness.DeleteAssessment(id));
            return JsonHelper.SerializeObject(response);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.TRS, Anonymity = Anonymous.Verified)]
        public string Offline(string schoolName, string director, int communityId = 0, int schoolId = 0,
            string sort = "Name", string order = "ASC", int first = 0, int count = 10)
        {
            OfflineUrlModel model = new OfflineUrlModel();
            model.communityId = communityId;
            model.schoolId = schoolId;
            model.schoolName = schoolName;
            model.sort = sort;
            model.order = order;
            model.first = first;
            model.count = count;

            Session["_TRS_Offline_URL"] = model;
            return string.Empty;
        }


        private void InitAccessOperation()
        {
            bool accessSchool = false;
            bool accessClass = false;
            if (UserInfo != null)
            {
                List<UserAuthorityModel> userAuthorities = new PermissionBusiness().GetUserAuthorities(UserInfo
                    , new List<int> { (int)PagesModel.School_Management, (int)PagesModel.TRSClass_Management });
                if (userAuthorities != null && userAuthorities.Count > 0)
                {
                    foreach (UserAuthorityModel item in userAuthorities)
                    {
                        if (item.PageId == (int)PagesModel.School_Management)
                        {
                            if ((item.Authority & (int)Authority.Edit) == (int)Authority.Edit)
                            {
                                accessSchool = true;
                            }
                        }
                        if (item.PageId == (int)PagesModel.TRSClass_Management)
                        {
                            if ((item.Authority & (int)Authority.Edit) == (int)Authority.Edit)
                            {
                                accessClass = true;
                            }
                        }
                    }
                }
            }
            ViewBag.accessSchool = accessSchool;
            ViewBag.accessClass = accessClass;
        }

        #region TRS EventLog
        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.TRS, Anonymity = Anonymous.Verified)]
        [HttpGet]
        public ActionResult Accreditations(int id, int accreditation, int verifiedStar, int schoolId)
        {
            SchoolEntity school = _schoolBusiness.GetSchool(schoolId);
            ViewBag.schoolAccreditation = _trsBusiness.GetSchoolAccreditation(school);
            ViewBag.logId = id;
            ViewBag.accreditation = accreditation;
            ViewBag.verifiedStar = TRSStarEnum.Four.ToSelectList();
            ViewBag.ApproveDate = school.StarDate.FormatDateString();
            ViewBag.currentVerifiedStar = verifiedStar == 0 ? school.VSDesignation.GetValue() : verifiedStar;
            return View();
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.TRS, Anonymity = Anonymous.Verified)]
        [HttpGet]
        public ActionResult Notification(int id, int schoolId)
        {
            List<NotificationUserModel> users = _trsBusiness.GetNotificationUsers(schoolId);
            TRSEventLogEntity eventLog = _trsBusiness.GetEventLogById(id);
            List<int> existUserIdList = eventLog.Notifications.Select(n => n.UserId).ToList();
            users.FindAll(r => existUserIdList.Contains(r.UserId)).ForEach(r => r.Selected = true);
            ViewBag.users = users;
            ViewBag.logId = id;
            ViewBag.schoolId = schoolId;
            return View();
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.TRS, Anonymity = Anonymous.Verified)]
        [HttpGet]
        public string GetEventLogList(int schoolId, int eventLogId = 0)
        {
            var condition = PredicateHelper.True<TRSEventLogEntity>();
            condition = condition.And(x => true);
            if (eventLogId != 0)
            {
                condition = condition.And(r => r.ID == eventLogId);
            }
            else if (schoolId != 0)
            {
                condition = condition.And(r => r.SchoolId == schoolId);
            }
            List<TRSEventLogModel> eventLogModelList = _trsBusiness.GetEventLogModelList(condition);
            foreach (TRSEventLogModel eventLog in eventLogModelList)
            {
                if (eventLog.Documents.Any())
                {
                    foreach (TRSEventLogFileModel doc in eventLog.Documents)
                    {
                        doc.FileServerPath = FileHelper.GetPreviewPathofUploadFile(doc.FilePath);
                        if (doc.CreatedBy == UserInfo.ID)
                            doc.IsAllowDel = true;
                    }
                }
            }
            return JsonHelper.SerializeObject(eventLogModelList);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.TRS, Anonymity = Anonymous.Verified)]
        [HttpPost]
        public string NewEventLog(DateTime? dateCreated, string createdBy, int eventType, string comment,
            DateTime? actionRequired, bool notification, int accreditation, int schoolId, string files, string approvalDate, int relatedId = 0, int verifiedStar = 0)
        {
            var response = new PostFormResponse();
            OperationResult result = new OperationResult(OperationResultType.Success);
            TRSEventLogEntity entity = new TRSEventLogEntity();
            entity.SchoolId = schoolId;
            entity.DateCreated = dateCreated == null ? CommonAgent.MinDate : dateCreated.Value;
            entity.CreatedBy = createdBy;
            entity.EventType = (TrsEventType)eventType;
            entity.Comment = comment;
            entity.ActionRequired = actionRequired == null ? CommonAgent.MinDate : actionRequired.Value;
            entity.Notification = notification;
            entity.Accreditation = (TrsAccreditation)accreditation;
            entity.RelatedId = relatedId;
            entity.UpdateBy = UserInfo.ID;
            entity.CreateBy = UserInfo.ID;
            result = _trsBusiness.InsertEventLog(entity);

            if (result.ResultType == OperationResultType.Success)
            {
                if (!string.IsNullOrEmpty(files))
                {
                    AddEventLogFiles(entity.ID, files);
                }
                if (entity.EventType == TrsEventType.Auto_Assign && entity.Accreditation != 0)
                {

                    TRSStarEnum schoolVDStar = verifiedStar > 0 & verifiedStar <= TRSStarEnum.Four.GetValue() ? (TRSStarEnum)verifiedStar : TRSStarEnum.Four;
                    DateTime trsApprovalDate = approvalDate == "" ? CommonAgent.MinDate : Convert.ToDateTime(approvalDate);
                    result = _trsBusiness.UpdateSchoolAccreditation(entity.SchoolId, trsApprovalDate, entity.Accreditation, EventLogType.Auto_Assign, schoolVDStar);
                }
                else if (entity.EventType == TrsEventType.Star_Level_Change && verifiedStar != 0)
                {
                    TRSStarEnum schoolVDStar = verifiedStar > 0 & verifiedStar <= TRSStarEnum.Four.GetValue() ? (TRSStarEnum)verifiedStar : TRSStarEnum.Four;
                    DateTime trsApprovalDate = approvalDate == "" ? CommonAgent.MinDate : Convert.ToDateTime(approvalDate);
                    result = _trsBusiness.UpdateSchoolVerifiedStar(entity.SchoolId, EventLogType.Star_Level_Change, trsApprovalDate, schoolVDStar);
                }
            }
            response.Success = result.ResultType == OperationResultType.Success;
            response.Data = GetEventLogList(0, entity.ID);
            response.Message = result.Message;
            return JsonHelper.SerializeObject(response);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.TRS, Anonymity = Anonymous.Verified)]
        [HttpPost]
        public string NotificationUsers(int EventLogId, int SchoolId, int[] SelectUser)
        {
            var response = new PostFormResponse();
            OperationResult result = new OperationResult(OperationResultType.Success);
            if (SelectUser.Length > 0)
            {
                result = _trsBusiness.InsertNotifications(EventLogId, SchoolId, SelectUser);
            }
            response.Success = result.ResultType == OperationResultType.Success;
            response.Data = "";
            response.Message = result.Message;
            return JsonHelper.SerializeObject(response);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.TRS, Anonymity = Anonymous.Verified)]
        [HttpPost]
        public string AddEventLogFiles(int eventLogId, string uploadfiles)
        {
            var response = new PostFormResponse();
            OperationResult result = new OperationResult(OperationResultType.Success);
            List<TRSEventLogFileEntity> listEventLogFile = new List<TRSEventLogFileEntity>();
            if (!string.IsNullOrEmpty(uploadfiles))
            {
                if (uploadfiles.EndsWith(",]"))
                {
                    //uploadfiles格式：[{'FileName1':'FileName1','FilePath1':'FilePath1'},{'FileName2':'FileName2','FilePath2':'FilePath2'},]
                    uploadfiles = uploadfiles.Replace(",]", "]");
                    //将传入的字符串解析成List<AssignmentFileEntity>
                    JavaScriptSerializer Serializer = new JavaScriptSerializer();
                    listEventLogFile =
                        Serializer.Deserialize<List<TRSEventLogFileEntity>>(uploadfiles);
                    foreach (TRSEventLogFileEntity entity in listEventLogFile)
                    {
                        entity.EventLogId = eventLogId;
                        entity.CreatedBy = UserInfo.ID;
                        entity.UpdatedBy = UserInfo.ID;
                    }
                    result = _trsBusiness.InsertEventLogFiles(listEventLogFile);
                    response.Success = result.ResultType == OperationResultType.Success;
                    response.Data = "";
                    response.Message = result.Message;
                }
            }
            return JsonHelper.SerializeObject(response);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.TRS, Anonymity = Anonymous.Verified)]
        [HttpPost]
        public string DeleteEventLogFile(int eventLogId)
        {
            var response = new PostFormResponse();
            OperationResult result = new OperationResult(OperationResultType.Success);
            result = _trsBusiness.DeleteEventlogFile(eventLogId, UserInfo);
            response.Success = result.ResultType == OperationResultType.Success;
            response.Data = "";
            response.Message = result.Message;
            return JsonHelper.SerializeObject(response);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.TRS, Anonymity = Anonymous.Verified)]
        [HttpGet]
        public ActionResult ChangeStarLevel(int id, int verifiedStar, int schoolId, string approvalDate)
        {
            SchoolEntity school = _schoolBusiness.GetSchool(schoolId);
            ViewBag.logId = id;
            ViewBag.verifiedStar = TRSStarEnum.Four.ToSelectList();
            ViewBag.currentVerifiedStar = verifiedStar == 0 ? school.VSDesignation.GetValue() : verifiedStar;
            ViewBag.approvalDate = approvalDate == "" ? DateTime.Now.FormatDateString() : approvalDate;
            return View();
        }
        #endregion
    }
}