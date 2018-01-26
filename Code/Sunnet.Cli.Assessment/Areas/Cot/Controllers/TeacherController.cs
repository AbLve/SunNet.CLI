using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sunnet.Cli.Assessment.Controllers;
using Sunnet.Cli.Business.Ade;
using Sunnet.Cli.Business.Common;
using Sunnet.Cli.Business.Communities;
using Sunnet.Cli.Business.Cot;
using Sunnet.Cli.Business.Cot.Models;
using Sunnet.Cli.Business.Schools;
using Sunnet.Cli.Business.Users;
using Sunnet.Cli.Business.Users.Models;
using Sunnet.Cli.Core.Common.Enums;
using Sunnet.Cli.Core.Communities.Entities;
using Sunnet.Cli.Core.Cot;
using Sunnet.Cli.Core.Cot.Models;
using Sunnet.Cli.Core.Schools.Entities;
using Sunnet.Cli.Core.Users.Entities;
using Sunnet.Cli.Core.Users.Enums;
using Sunnet.Cli.UIBase.Filters;
using Sunnet.Cli.UIBase.Models;
using Sunnet.Framework.Helpers;
using Sunnet.Framework.Permission;
using Sunnet.Framework.Extensions;

namespace Sunnet.Cli.Assessment.Areas.Cot.Controllers
{
    public class TeacherController : BaseController
    {

        private readonly AdeBusiness _adeBusiness;
        private readonly UserBusiness _userBusiness;
        private readonly CommunityBusiness _communityBusiness;
        private readonly SchoolBusiness _schoolBusiness;
        private readonly CotBusiness _cotBusiness;
        public TeacherController()
        {
            _adeBusiness = new AdeBusiness(AdeUnitWorkContext);
            _userBusiness = new UserBusiness(UnitWorkContext);
            _communityBusiness = new CommunityBusiness(UnitWorkContext);
            _schoolBusiness = new SchoolBusiness(UnitWorkContext);
            _cotBusiness = new CotBusiness(AdeUnitWorkContext);
        }
        // GET: Cot/Teacher
        [CLIUrlAuthorize(Account = Authority.Index, PageId = PagesModel.COT, Anonymity = Anonymous.Verified, Parameter = "assessmentId")]
        public ActionResult Index(int id, int assessmentId, int year = 0)
        {
            var assessment = _adeBusiness.GetBaseAssessmentModel(assessmentId);
            if (assessment == null) return new EmptyResult();
            ViewBag.AssessmentName = assessment.Name;

            if (!_adeBusiness.CanExecuteAssessment(assessmentId))
                return RedirectToAction("Index", "Dashboard", new { Area = "", showMessage = "assessment_unavaiable" });

            ViewBag.teacherId = id;
            ViewBag.assessmentId = assessmentId;
            CotTeacherStatus status = _cotBusiness.GetTeacherStatus(assessmentId, year, id);

            var teacherModel = _userBusiness.GetUserModelByTeacherId(id);
            if (teacherModel == null)
            {
                return RedirectToAction("All", "Teacher", new { Area = "Cot", id = assessmentId, year = year });
            }
            ViewBag.TeacherName = string.Format("{0} {1}", teacherModel.FirstName, teacherModel.LastName);
            ViewBag.SchoolName = teacherModel.SchoolNameText;
            ViewBag.CommunityName = teacherModel.CommunityNameText;

            if (year == 0)
                year = CommonAgent.Year;
            ViewBag.SchoolYear = year.ToSchoolYearString();
            if (status.HasLastStgReport || status.StgReportReadOnly)
            {
                ViewBag.Reports = _cotBusiness.GetReports(assessmentId, year, id);
            }
            else
            {
                ViewBag.Reports = new List<CotStgReportModel>();
            }

            var rolesCanDeleteLastSTGR = new List<Role>()
            {
                Role.Super_admin,
                Role.Mentor_coach,
                Role.School_Specialist,
                Role.School_Specialist_Delegate,
                Role.Principal,
                Role.Principal_Delegate,
                Role.District_Community_Specialist,
                Role.Community_Specialist_Delegate
            };
            if (rolesCanDeleteLastSTGR.Contains(UserInfo.Role))
            {
                status.ResetShortTermGoalsVisible = true;
            }
            return View(status);
        }

        /// <summary>
        /// Teacherses the specified identifier.
        /// </summary>
        /// <param name="id">AssessmentId.</param>
        /// <returns></returns>
        [CLIUrlAuthorize(Account = Authority.Index, PageId = PagesModel.COT, Anonymity = Anonymous.Verified, Parameter = "id")]
        public ActionResult All(int id, int year = 0)
        {
            var assessment = _adeBusiness.GetBaseAssessmentModel(id);
            if (assessment == null) return new EmptyResult();
            ViewBag.AssessmentName = assessment.Name;

            if (!_adeBusiness.CanExecuteAssessment(id))
                return RedirectToAction("Index", "Dashboard", new { Area = "", showMessage = "assessment_unavaiable" });

            _adeBusiness.LockAssessment(id);
            ViewBag.assessmentId = id;
            ViewBag.CurrentYear = CommonAgent.Year;
            ViewBag.IsAdmin = UserInfo.Role == Role.Super_admin;
            if (year == 0)
                year = CommonAgent.Year;
            List<SelectListItem> yearList = new SelectList(CommonAgent.GetYears(), "ID", "Name").ToList();
            yearList.ForEach(r => r.Selected = false);
            SelectListItem currentYearItem = yearList.Find(r => r.Value == year.ToString());
            if (currentYearItem != null)
                currentYearItem.Selected = true;
            ViewBag.SchoolYearOptions = yearList;

            ViewBag.COTOffline = CheckAssessmentPermission(id, Authority.Offline);
            return View();
        }

        public string Offline(string teacherCode, string firstName, string lastName,
            int year = 0, int assessmentId = 0, bool searchExisted = false, int communityId = 0,
            int schoolId = 0, int selCoach = 0, string sort = "LastName", string order = "Asc", int first = 0, int count = 10)
        {
            var total = 0;
            var list = GetTeacherModels(teacherCode, firstName, lastName, year, assessmentId, searchExisted, communityId, schoolId, selCoach, sort, order, first, count, ref total);
            Session["_Cot_Offline_Teachers"] = list;
            var response = new PostFormResponse() { Success = list.Any() };
            return JsonHelper.SerializeObject(response);
        }

        [CLIUrlAuthorize(Account = Authority.All, PageId = PagesModel.COT, Anonymity = Anonymous.Verified)]
        public string GetCommunitySelectListForSearch(string keyword, int communityId = -1, bool isActiveCommunity = true)
        {
            var expression = PredicateHelper.True<CommunityEntity>();
            if (communityId > 0)
                expression = expression.And(o => o.ID == communityId);
            var list = _communityBusiness.GetCommunitySelectList(UserInfo, expression, isActiveCommunity);
            return JsonHelper.SerializeObject(list);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.COT, Anonymity = Anonymous.Verified)]
        public string GetSchoolSelectList(string keyword, int communityId = 0, string schoolName = "", bool isActive = true)
        {
            var expression = PredicateHelper.True<SchoolEntity>();
            if (communityId > 0)
                expression = expression.And(s => s.CommunitySchoolRelations.Any(csr => csr.CommunityId == communityId));
            if (schoolName != null && schoolName.Trim() != string.Empty)
                expression = expression.And(o => o.BasicSchool.Name.Contains(schoolName));
            var schoolList = _schoolBusiness.GetSchoolsSelectList(UserInfo, expression, isActive);
            return JsonHelper.SerializeObject(schoolList);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.COT, Anonymity = Anonymous.Verified, Parameter = "assessmentId")]
        public string Search(string teacherCode, string firstName, string lastName,
            int year = 0, int assessmentId = 0, bool searchExisted = false, int communityId = 0,
            int schoolId = 0, int coachId = 0, string sort = "LastName", string order = "Asc", int first = 0, int count = 10)
        {
            var total = 0;
            var list = GetTeacherModels(teacherCode, firstName, lastName, year, assessmentId, searchExisted, communityId, schoolId, coachId, sort, order, first, count, ref total);
            var result = new { total = total, data = list };
            return JsonHelper.SerializeObject(result);
        }

        protected List<CotSchoolTeacherModel> GetTeacherModels(string teacherCode, string firstName, string lastName, int year, int assessmentId,
            bool searchExisted, int communityId, int schoolId, int coachId, string sort, string order, int first, int count, ref int total)
        {
            int? coach = null;
            if (coachId != 0)
                coach = coachId;

            if (year == 0) year = CommonAgent.Year;
            List<int> schoolIds = null;
            List<int> communities = null;
            UserBaseEntity host = null;
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
                    case Role.District_Community_Specialist:
                        communities.AddRange(
                            UserInfo.UserCommunitySchools.Where(ucs => ucs.Status == EntityStatus.Active)
                                .Select(ucs => ucs.CommunityId)
                                .ToList());
                        schoolIds = null;
                        break;

                    case Role.District_Community_Delegate:
                    case Role.Community_Specialist_Delegate:
                        host = _userBusiness.GetUser(UserInfo.CommunityUser.ParentId);
                        communities.AddRange(
                            host.UserCommunitySchools.Where(ucs => ucs.Status == EntityStatus.Active)
                                .Select(ucs => ucs.CommunityId)
                                .ToList());
                        schoolIds = null;
                        break;
                    case Role.Principal:
                    case Role.TRS_Specialist:
                    case Role.School_Specialist:
                        communities.AddRange(_userBusiness.GetAssignedCommunityIdsForPrincipal(UserInfo.ID));
                        schoolIds.AddRange(
                            UserInfo.UserCommunitySchools.Where(x => x.SchoolId > 0).Select(x => x.SchoolId).ToList());
                        break;
                    case Role.Principal_Delegate:
                    case Role.TRS_Specialist_Delegate:
                    case Role.School_Specialist_Delegate:
                        host = _userBusiness.GetUser(UserInfo.Principal.ParentId);
                        communities.AddRange(_userBusiness.GetAssignedCommunityIdsForPrincipal(host.ID));
                        schoolIds.AddRange(
                            host.UserCommunitySchools.Where(x => x.SchoolId > 0).Select(x => x.SchoolId).ToList());
                        break;
                }
            }
            if (schoolId > 0)
                schoolIds = new List<int>() { schoolId };
            if (communityId > 0)
                communities = new List<int>() { communityId };
            var list = _cotBusiness.GetTeachers(assessmentId, year, coach, communities, schoolIds, firstName, lastName,
                teacherCode, searchExisted, sort, order, first, count, out total);
            return list;
        }

        [CLIUrlAuthorize(Account = Authority.Index, PageId = PagesModel.COT, Anonymity = Anonymous.Verified, Parameter = "assessmentId")]
        [HttpPost]
        public string ResetStgReport(int assessmentId, int teacherId, int year)
        {
            var response = new PostFormResponse();
            response.Update(_cotBusiness.ResetStgReport(assessmentId, teacherId, year, UserInfo));
            return JsonHelper.SerializeObject(response);
        }

        [CLIUrlAuthorize(Account = Authority.Index, PageId = PagesModel.COT, Anonymity = Anonymous.Verified, Parameter = "assessmentId")]
        [HttpPost]
        public string ResetCot(int assessmentId, int teacherId, int year, CotWave wave)
        {
            var response = new PostFormResponse();
            response.Update(_cotBusiness.ResetCot(assessmentId, teacherId, year, wave, UserInfo));
            return JsonHelper.SerializeObject(response);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.COT, Anonymity = Anonymous.Verified)]
        public string GetCoachSelectList(int communityId = -1)
        {
            IEnumerable<SelectListItem> list = null;

            if (communityId == -1)
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