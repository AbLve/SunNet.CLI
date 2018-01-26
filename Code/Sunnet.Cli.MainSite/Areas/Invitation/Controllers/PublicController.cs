using StructureMap;
using Sunnet.Cli.Business.Classes;
using Sunnet.Cli.Business.Communities;
using Sunnet.Cli.Business.Log;
using Sunnet.Cli.Business.Permission;
using Sunnet.Cli.Business.Permission.Models;
using Sunnet.Cli.Business.Schools;
using Sunnet.Cli.Business.Users;
using Sunnet.Cli.Core.Classes.Entites;
using Sunnet.Cli.Core.Common.Enums;
using Sunnet.Cli.Core.Communities.Entities;
using Sunnet.Cli.Core.Log.Entities;
using Sunnet.Cli.Core.Schools.Entities;
using Sunnet.Cli.Core.StatusTracking.Enums;
using Sunnet.Cli.Core.Users.Entities;
using Sunnet.Cli.Core.Users.Enums;
using Sunnet.Cli.MainSite.Controllers;
using Sunnet.Cli.UIBase.Models;
using Sunnet.Framework;
using Sunnet.Framework.Core.Tool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sunnet.Framework.Encrypt;
using Sunnet.Framework.Extensions;
using Sunnet.Cli.UIBase.Filters;
using Sunnet.Framework.Helpers;
using Sunnet.Framework.Permission;
using Sunnet.Cli.Core.Permission.Entities;
using Sunnet.Cli.Business.MasterData;

namespace Sunnet.Cli.MainSite.Areas.Invitation.Controllers
{
    public class PublicController : BaseController
    {
        private readonly UserBusiness userBusiness;
        private readonly PermissionBusiness permissionBusiness;
        private readonly MasterDataBusiness masterDataBusiness;
        private readonly CommunityBusiness communityBusiness;
        private readonly SchoolBusiness schoolBusiness;
        private readonly ClassBusiness classBusiness;
        private readonly OperationLogBusiness operationLogBusiness;
        public PublicController()
        {
            userBusiness = new UserBusiness(UnitWorkContext);
            permissionBusiness = new PermissionBusiness(UnitWorkContext);
            masterDataBusiness = new MasterDataBusiness(UnitWorkContext);
            communityBusiness = new CommunityBusiness(UnitWorkContext);
            schoolBusiness = new SchoolBusiness(UnitWorkContext);
            classBusiness = new ClassBusiness(UnitWorkContext);
            operationLogBusiness = new OperationLogBusiness(UnitWorkContext);
        }
        //
        // GET: /Invitation/Home/ 
        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.Administrative, Anonymity = Anonymous.Logined)]
        public ActionResult Index()
        {
            return View();
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.Administrative, Anonymity = Anonymous.Logined)]
        public ActionResult Dashboard()
        {
            ViewBag.RoleType = UserInfo.Role;
            ViewBag.IsCLIUser = UserInfo.IsCLIUser;
            ViewBag.PageList = permissionBusiness.CheckPage(UserInfo);
            return View();
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.Delegate, Anonymity = Anonymous.Verified)]
        public ActionResult Delegate()
        {
            InitAccessOperation();
            List<string> listRole = new List<string>();
            listRole.Add(Role.District_Community_Delegate.ToDescription());
            listRole.Add(Role.Community_Specialist_Delegate.ToDescription());
            listRole.Add(Role.Principal_Delegate.ToDescription());
            listRole.Add(Role.School_Specialist_Delegate.ToDescription());
            listRole.Add(Role.TRS_Specialist_Delegate.ToDescription());
            ViewBag.RoleTypeOptions = Role.Administrative_personnel.ToSelectList().Where(e => listRole.Contains(e.Text))
                .AddDefaultItem(ViewTextHelper.DefaultAllText, "-1", 0);
            return View();
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.Delegate, Anonymity = Anonymous.Verified)]
        public string SearchDelegates(string keyword, int roleType = -1, int status = -1,
            string sort = "LastName", string order = "Asc", int first = 0, int count = 10)
        {
            var total = 0;

            var expression = PredicateHelper.True<UserBaseEntity>();
            if (roleType > 0)
                expression = expression.And(r => r.Role == (Role)roleType);
            if (status > 0)
                expression = expression.And(r => (int)r.Status == status);
            if (keyword.Trim() != string.Empty)
                expression = expression.And(r => r.FirstName.Contains(keyword)
                    || r.LastName.Contains(keyword) || r.PrimaryEmailAddress.Contains(keyword));
            expression = expression.And(r => r.IsDeleted == false);
            var list = userBusiness.SearchDelegates(UserInfo, expression, sort, order, first, count, out total);
            var result = new { total = total, data = list };
            return JsonHelper.SerializeObject(result);
        }

        #region Assign Permission
        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.Administrative, Anonymity = Anonymous.Logined)]
        public ActionResult AssignPermission(int userId)
        {
            TeacherRoleEntity roleEntity = userBusiness.GetTeacherRoleEntity(UserInfo.Role);
            if (roleEntity == null)
                return RedirectToAction("Index");
            ViewBag.Role = JsonHelper.SerializeObject(roleEntity);

            UserBaseEntity user = userBusiness.GetUser(userId);
            ViewBag.UserId = user.ID;
            ViewBag.Name = user.FirstName + " " + user.LastName;
            ViewBag.UserType = user.Role.ToDescription();
            ViewBag.GroupPackageSelected = user.PermissionRoles.Select(e =>
                new GroupPackageModel()
                {
                    PackageId = e.ID,
                    PackageName = e.Name,
                    PackageDescription = e.Descriptions
                }).ToList();
            ViewBag.IsAdmin = UserInfo.Role == Role.Super_admin;
            ViewBag.GroupPackageDisabled = user.DisabledUsrRoles.Select(r => r.RoleId).ToList();
            List<GroupPackageModel> groupPackage = new List<GroupPackageModel>();
            switch (user.Role)
            {
                case Role.Parent:
                    groupPackage = permissionBusiness.GetAssignedPackages((int)user.Role,
                        user.Parent.UserInfo.UserCommunitySchools.Select(x => x.SchoolId).ToList(),
                        user.Parent.UserInfo.UserCommunitySchools.Select(x => x.CommunityId).ToList());
                    break;
                case Role.Teacher:
                    groupPackage = permissionBusiness.GetAssignedPackages((int)user.Role,
                        user.TeacherInfo.UserInfo.UserCommunitySchools.Select(x => x.SchoolId).ToList(),
                        user.TeacherInfo.UserInfo.UserCommunitySchools.Select(x => x.CommunityId).ToList());
                    break;
                case Role.Principal:
                case Role.Principal_Delegate:
                    groupPackage = permissionBusiness.GetAssignedPackages((int)Role.Principal,
                        user.UserCommunitySchools.Select(x => x.SchoolId).ToList(),
                        user.UserCommunitySchools.Select(x => x.CommunityId).ToList());
                    break;
                case Role.TRS_Specialist:
                case Role.TRS_Specialist_Delegate:
                    groupPackage = permissionBusiness.GetAssignedPackages((int)Role.TRS_Specialist,
                        user.UserCommunitySchools.Select(x => x.SchoolId).ToList(),
                        user.UserCommunitySchools.Select(x => x.CommunityId).ToList());
                    break;
                case Role.School_Specialist:
                case Role.School_Specialist_Delegate:
                    groupPackage = permissionBusiness.GetAssignedPackages((int)Role.School_Specialist,
                        user.UserCommunitySchools.Select(x => x.SchoolId).ToList(),
                        user.UserCommunitySchools.Select(x => x.CommunityId).ToList());
                    break;
                case Role.Community:
                case Role.District_Community_Delegate:
                    groupPackage = permissionBusiness.GetAssignedPackages((int)Role.Community, null,
                        user.UserCommunitySchools.Select(e => e.CommunityId).ToList());
                    break;
                case Role.District_Community_Specialist:
                case Role.Community_Specialist_Delegate:
                    groupPackage = permissionBusiness.GetAssignedPackages((int)Role.District_Community_Specialist, null,
                        user.UserCommunitySchools.Select(e => e.CommunityId).ToList());
                    break;
                case Role.Auditor:
                case Role.Statewide:
                    groupPackage = permissionBusiness.GetCustomPackages(user.Role);
                    break;
            }
            ViewBag.GroupPackages = groupPackage;
            return View();
        }

        [System.Web.Mvc.HttpPost]
        [ValidateAntiForgeryToken]
        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.Administrative, Anonymity = Anonymous.Logined)]
        public string AssignPermission(int userId, int[] chkPackages, int[] chkDisablePackages)
        {
            var response = new PostFormResponse();
            if (response.Success = ModelState.IsValid)
            {
                OperationResult result = new OperationResult(OperationResultType.Success);
                result = userBusiness.AssignPermission(userId, chkPackages, chkDisablePackages, UserInfo.Role == Role.Super_admin);
                response.Success = result.ResultType == OperationResultType.Success;
                response.Message = result.Message;
            }
            response.ModelState = ModelState;
            return JsonHelper.SerializeObject(response);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.Administrative, Anonymity = Anonymous.Logined)]
        public string GetAssignedPackages(int userType, int? communityId, int? schoolId)
        {
            return JsonHelper.SerializeObject(permissionBusiness.GetAssignedPackages(userType, schoolId, communityId));
        }
        #endregion

        #region Public Method
        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.Administrative, Anonymity = Anonymous.Logined)]
        public string GetCountiesByStateId(int stateId = 0)
        {
            var classList = masterDataBusiness.GetCountySelectList(stateId).ToSelectList(ViewTextHelper.DefaultCountyText, "");
            return JsonHelper.SerializeObject(classList);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.Administrative, Anonymity = Anonymous.Logined)]
        public string GetNullCountiesByStateId(int stateId = 0)
        {
            var classList = masterDataBusiness.GetCountySelectList(stateId).ToSelectList("County", "0");
            return JsonHelper.SerializeObject(classList);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.Administrative, Anonymity = Anonymous.Logined)]
        public string ReInvite(int userId)
        {
            UserBaseEntity user = userBusiness.GetUser((userId));
            user.GoogleId = "";
            user.Gmail = "";//Ticket 2685 可能有风险
            userBusiness.UpdateUser(user);
            EmailTemplete template = XmlHelper.GetEmailTemplete("TeacherInvitation_Template.xml");
            string param = user.ID.ToString() + "," + DateTime.Now.ToString();
            string encryptParam = ObjectFactory.GetInstance<IEncrypt>().Encrypt(param);
            string link = SFConfig.MainSiteDomain + "Home/InviteVerification/"
                + System.Web.HttpContext.Current.Server.UrlEncode(encryptParam);
            string emailBody = template.Body.Replace("{FirstName}", user.FirstName)
            .Replace("{LastName}", user.LastName)
            .Replace("{InviteLink}", "<a style='text-decoration: underline; cursor:pointer; color: #008000;' href='" + link + "'>Click here</a>")
            .Replace("{StaticDomain}", SFConfig.StaticDomain)
            .Replace("{CliEngageUrl}", SFConfig.MainSiteDomain);
            userBusiness.SendEmail(user.PrimaryEmailAddress, template.Subject, emailBody);
            EmailLogEntity emailLog = new EmailLogEntity(user.ID, user.PrimaryEmailAddress, EmailLogType.Invitation);
            userBusiness.InsertEmailLog(emailLog);
            operationLogBusiness.InsertLog(OperationEnum.ResetSendInvitation, user.Role.ToDescription(),
                "Reset Invitation,UserId:" + user.ID, CommonHelper.GetIPAddress(Request), UserInfo);
            return user.GoogleId;
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.Administrative, Anonymity = Anonymous.Logined)]
        public bool Inactive(int id)
        {
            UserBaseEntity user = userBusiness.GetUser(id);
            OperationResult result = userBusiness.ChangeStatusInactive(user);
            operationLogBusiness.InsertLog(OperationEnum.StatusChanged, user.Role.ToDescription(),
                "Status Changed," + (user.Status == EntityStatus.Inactive ? "Active to Inactive" : "Inactive to Active") +
                ",UserId:" + user.ID,
                CommonHelper.GetIPAddress(Request), UserInfo);
            return result.ResultType == OperationResultType.Success;
        }
        #endregion

        #region User Community Relations

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.Administrative, Anonymity = Anonymous.Logined)]
        public ActionResult AssignCommunity(int userId)
        {
            UserBaseEntity user = userBusiness.GetUser(userId);
            ViewBag.NavigationText = user.Role.ToDescription();
            return View(user);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.Administrative, Anonymity = Anonymous.Logined)]
        public string SearchUnassigedCommunities(int userId, int communityId = 0, string name = "",
            string sort = "CommunityName",
            string order = "Asc", int first = 0, int count = 10)
        {
            int total = 0;
            var expression = PredicateHelper.True<CommunityEntity>();
            expression = expression.And(r => !(r.UserCommunitySchools.Any(s => s.UserId == userId)));
            if (communityId > 0)
                expression = expression.And(r => r.ID == communityId);
            else if (name.Trim() != string.Empty)
                expression = expression.And(r => r.BasicCommunity.Name.Contains(name));
            expression = expression.And(r => r.Status == EntityStatus.Active);

            var list = communityBusiness.SearchCommunities(UserInfo, expression, sort, order, first, count, out total);
            var result = new { total = total, data = list };
            return JsonHelper.SerializeObject(result);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.Administrative,
            Anonymity = Anonymous.Logined)]
        public string SearchAssignedCommunities(int userId = 0, int communityId = 0, string name = "",
            string sort = "CommunityName",
            string order = "Asc", int first = 0, int count = 10)
        {
            int total = 0;
            var expression = PredicateHelper.True<UserComSchRelationEntity>();
            if (UserInfo.Role == Role.Content_personnel || UserInfo.Role == Role.Statisticians ||
                UserInfo.Role == Role.Administrative_personnel || UserInfo.Role == Role.Intervention_manager ||
                UserInfo.Role == Role.Video_coding_analyst || UserInfo.Role == Role.Intervention_support_personnel ||
                UserInfo.Role == Role.Coordinator || UserInfo.Role == Role.Mentor_coach)
                expression = expression.And(r => r.Community.UserCommunitySchools.Any(s => s.UserId == UserInfo.ID));
            else if (UserInfo.Role == Role.Community || UserInfo.Role == Role.District_Community_Specialist ||
                     UserInfo.Role == Role.Statewide)
                expression = expression.And(r => r.Community.UserCommunitySchools.Any(s => s.UserId == UserInfo.ID));
            else if (UserInfo.Role == Role.District_Community_Delegate ||
                     UserInfo.Role == Role.Community_Specialist_Delegate)
                expression =
                    expression.And(
                        r => r.Community.UserCommunitySchools.Any(s => s.UserId == UserInfo.CommunityUser.ParentId));
            if (communityId > 0)
                expression = expression.And(r => r.CommunityId == communityId);
            else if (name.Trim() != string.Empty)
                expression = expression.And(r => r.Community.BasicCommunity.Name.Contains(name));
            if (userId > 0)
                expression = expression.And(r => r.UserId == userId);
            var list = userBusiness.GetAssignedCommunities(expression, sort, order, first, count, out total);
            var result = new { total = total, data = list };
            return JsonHelper.SerializeObject(result);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.Administrative, Anonymity = Anonymous.Logined)]
        public string UserCommRelations(int userId, int[] comIds)
        {
            var response = new PostFormResponse();
            OperationResult result = new OperationResult(OperationResultType.Success);
            result = userBusiness.InsertUserCommunityRelations(userId, UserInfo.ID, comIds);
            response.Success = result.ResultType == OperationResultType.Success;
            response.Data = "";
            response.Message = result.Message;
            return JsonHelper.SerializeObject(response);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.Administrative, Anonymity = Anonymous.Logined)]
        public string DeleteUserCommRelations(int userId, int[] comIds)
        {
            var response = new PostFormResponse();
            OperationResult result = new OperationResult(OperationResultType.Success);
            result = userBusiness.DeleteUserCommunityRelations(userId, comIds);
            response.Success = result.ResultType == OperationResultType.Success;
            response.Data = "";
            response.Message = result.Message;
            return JsonHelper.SerializeObject(response);
        }
        #endregion

        #region assign user to schools

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.Administrative, Anonymity = Anonymous.Logined)]
        public ActionResult AssignSchool(int userId)
        {
            UserBaseEntity user = userBusiness.GetUser(userId);
            ViewBag.NavigationText = user.Role.ToDescription();
            return View(user);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.Administrative, Anonymity = Anonymous.Logined)]
        public string SearchUnassigedSchools(int userId, string name = "", string communityname = "",
            int communityId = 0,
            int schoolId = 0, string sort = "SchoolName", string order = "Asc", int first = 0, int count = 10)
        {
            int total = 0;
            var expression = PredicateHelper.True<SchoolEntity>();
            expression = expression.And(r => !(r.UserCommunitySchools.Any(s => s.UserId == userId)));
            if (communityId > 0)
                expression =
                    expression.And(r => r.CommunitySchoolRelations.Any(e => e.CommunityId == communityId));
            else if (communityname.Trim() != string.Empty)
                expression =
                    expression.And(r => r.CommunitySchoolRelations.Any(e => e.Community.Name.Contains(communityname)));
            if (schoolId > 0)
                expression = expression.And(r => r.ID == schoolId);
            else if (name.Trim() != string.Empty)
                expression = expression.And(r => r.Name.Contains(name.Trim()));
            expression = expression.And(r => r.Status == SchoolStatus.Active);

            var list = schoolBusiness.SearchUnassigedSchools(UserInfo, expression, sort, order, first, count, out total);
            var result = new { total = total, data = list };
            return JsonHelper.SerializeObject(result);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.Administrative,
            Anonymity = Anonymous.Logined)]
        public string SearchAssignedSchools(int userId = 0, string name = "", string communityname = "",
            int communityId = 0
            , int schoolId = 0, string sort = "SchoolName", string order = "Asc", int first = 0, int count = 10)
        {
            int total = 0;

            var expression = PredicateHelper.True<UserComSchRelationEntity>();
            if (UserInfo.Role == Role.Content_personnel || UserInfo.Role == Role.Statisticians ||
                UserInfo.Role == Role.Administrative_personnel || UserInfo.Role == Role.Intervention_manager ||
                UserInfo.Role == Role.Video_coding_analyst || UserInfo.Role == Role.Intervention_support_personnel ||
                UserInfo.Role == Role.Coordinator || UserInfo.Role == Role.Mentor_coach)
                expression = expression.And(r => r.School.CommunitySchoolRelations.Any(
                    s => s.Community.UserCommunitySchools.Any(t => t.UserId == UserInfo.ID)));
            else if (UserInfo.Role == Role.Statewide || UserInfo.Role == Role.Community ||
                     UserInfo.Role == Role.District_Community_Specialist)
            {
                var primarySchoolIds =
                    schoolBusiness.GetPrimarySchoolsByComId(
                        UserInfo.UserCommunitySchools.Select(x => x.CommunityId).ToList()).Select(x => x.ID);
                expression = expression.And(r => primarySchoolIds.Contains(r.SchoolId));
            }
            else if (UserInfo.Role == Role.District_Community_Delegate ||
                     UserInfo.Role == Role.Community_Specialist_Delegate)
            {
                var parentCommunityUser = userBusiness.GetUser(UserInfo.CommunityUser.ParentId);
                var primarySchoolIds =
                    schoolBusiness.GetPrimarySchoolsByComId(
                        parentCommunityUser.UserCommunitySchools.Select(x => x.CommunityId).ToList()).Select(x => x.ID);
                expression = expression.And(r => r.UserId == UserInfo.CommunityUser.ParentId);
            }
            else if (UserInfo.Role == Role.Principal || UserInfo.Role == Role.TRS_Specialist ||
                     UserInfo.Role == Role.School_Specialist)
            {
                expression = expression.And(r => r.School.UserCommunitySchools.Any(s => s.UserId == UserInfo.ID));
            }
            else if (UserInfo.Role == Role.Principal_Delegate || UserInfo.Role == Role.TRS_Specialist_Delegate ||
                     UserInfo.Role == Role.School_Specialist_Delegate)
                expression =
                    expression.And(
                        r => r.School.UserCommunitySchools.Any(s => s.UserId == UserInfo.Principal.ParentId));
            if (userId > 0)
                expression = expression.And(r => r.UserId == userId);
            if (communityId > 0)
                expression =
                    expression.And(r => r.School.CommunitySchoolRelations.Any(e => e.CommunityId == communityId));
            else if (communityname.Trim() != string.Empty)
                expression =
                    expression.And(
                        r => r.School.CommunitySchoolRelations.Any(e => e.Community.Name.Contains(communityname)));
            if (schoolId > 0)
                expression = expression.And(r => r.SchoolId == schoolId);
            else if (name.Trim() != string.Empty)
                expression = expression.And(r => r.School.Name.Contains(name.Trim()));

            var list = userBusiness.GetAssignedSchools(expression, sort, order, first, count, out total);
            var result = new { total = total, data = list };
            return JsonHelper.SerializeObject(result);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.Administrative, Anonymity = Anonymous.Logined)]
        public string InsertUserSchoolRelations(int userId, int[] schoolIds)
        {
            var response = new PostFormResponse();
            OperationResult result = new OperationResult(OperationResultType.Success);
            result = userBusiness.InsertUserSchoolRelations(userId, UserInfo.ID, schoolIds);
            response.Success = result.ResultType == OperationResultType.Success;
            response.Data = "";
            response.Message = result.Message;
            return JsonHelper.SerializeObject(response);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.Administrative, Anonymity = Anonymous.Logined)]
        public string DeleteUserSchoolRelations(int userId, int[] schoolIds)
        {
            var response = new PostFormResponse();
            OperationResult result = new OperationResult(OperationResultType.Success);
            result = userBusiness.DeleteUserSchoolRelations(userId, schoolIds);
            response.Success = result.ResultType == OperationResultType.Success;
            response.Data = "";
            response.Message = result.Message;
            return JsonHelper.SerializeObject(response);
        }
        #endregion

        #region assign teacher to schools and community

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.Administrative, Anonymity = Anonymous.Logined)]
        public ActionResult TeacherAssignSchool(int userId)
        {
            UserBaseEntity user = userBusiness.GetUser(userId);
            ViewBag.NavigationText = user.Role.ToDescription();
            return View(user);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.Administrative, Anonymity = Anonymous.Logined)]
        public string SearchTeacherUnassigedSchools(int userId, string name = "", string communityname = "",
            int communityId = 0,
            int schoolId = 0, string sort = "SchoolName", string order = "Asc", int first = 0, int count = 10)
        {
            int total = 0;
            var expression = PredicateHelper.True<CommunitySchoolRelationsEntity>();
            expression =
                expression.And(
                    r =>
                        !(r.School.UserCommunitySchools.Any(s => s.UserId == userId) &&
                          r.Community.UserCommunitySchools.Any(t => t.UserId == userId && t.SchoolId > 0)));
            if (communityId > 0)
                expression =
                    expression.And(r => r.CommunityId == communityId);
            else if (communityname.Trim() != string.Empty)
                expression =
                    expression.And(r => r.Community.Name.Contains(communityname));
            if (schoolId > 0)
                expression = expression.And(r => r.SchoolId == schoolId);
            else if (name.Trim() != string.Empty)
                expression = expression.And(r => r.School.Name.Contains(name.Trim()));
            expression = expression.And(r => r.Status == EntityStatus.Active);

            var list = schoolBusiness.GetSchoolCommunitiy(UserInfo, expression, sort, order, first, count, out total);
            var result = new { total = total, data = list };
            return JsonHelper.SerializeObject(result);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.Administrative, Anonymity = Anonymous.Logined)]
        public string SearchTeacherAssignedSchools(int userId = 0, string name = "", string communityname = "", int communityId = 0
            , int schoolId = 0, string sort = "SchoolName", string order = "Asc", int first = 0, int count = 10)
        {
            int total = 0;

            var expression = PredicateHelper.True<UserComSchRelationEntity>();
            if (UserInfo.Role == Role.Content_personnel || UserInfo.Role == Role.Statisticians ||
                UserInfo.Role == Role.Administrative_personnel || UserInfo.Role == Role.Intervention_manager ||
                UserInfo.Role == Role.Video_coding_analyst || UserInfo.Role == Role.Intervention_support_personnel ||
                UserInfo.Role == Role.Coordinator || UserInfo.Role == Role.Mentor_coach)
                expression = expression.And(r => r.School.CommunitySchoolRelations.Any(
                    s => s.Community.UserCommunitySchools.Any(t => t.UserId == UserInfo.ID)));
            else if (UserInfo.Role == Role.Statewide || UserInfo.Role == Role.Community ||
                     UserInfo.Role == Role.District_Community_Specialist)
            {
                var primarySchoolIds =
                    schoolBusiness.GetPrimarySchoolsByComId(
                        UserInfo.UserCommunitySchools.Select(x => x.CommunityId).ToList()).Select(x => x.ID);
                expression = expression.And(r => primarySchoolIds.Contains(r.SchoolId));
            }
            else if (UserInfo.Role == Role.District_Community_Delegate ||
                     UserInfo.Role == Role.Community_Specialist_Delegate)
            {
                var parentCommunityUser = userBusiness.GetUser(UserInfo.CommunityUser.ParentId);
                var primarySchoolIds =
                    schoolBusiness.GetPrimarySchoolsByComId(
                        parentCommunityUser.UserCommunitySchools.Select(x => x.CommunityId).ToList()).Select(x => x.ID);
                expression = expression.And(r => r.UserId == UserInfo.CommunityUser.ParentId);
            }
            else if (UserInfo.Role == Role.Principal
                     || UserInfo.Role == Role.TRS_Specialist || UserInfo.Role == Role.School_Specialist)
            {
                expression = expression.And(r => r.School.UserCommunitySchools.Any(s => s.UserId == UserInfo.ID));
            }
            else if (UserInfo.Role == Role.Principal_Delegate || UserInfo.Role == Role.TRS_Specialist_Delegate ||
                     UserInfo.Role == Role.School_Specialist_Delegate)
            {
                expression =
                    expression.And(
                        r => r.School.UserCommunitySchools.Any(s => s.UserId == UserInfo.Principal.ParentId));
            }
            if (userId > 0)
                expression = expression.And(r => r.UserId == userId);
            if (communityId > 0)
                expression =
                    expression.And(r => r.School.CommunitySchoolRelations.Any(e => e.CommunityId == communityId));
            else if (communityname.Trim() != string.Empty)
                expression =
                    expression.And(r => r.School.CommunitySchoolRelations.Any(e => e.Community.Name.Contains(communityname)));
            if (schoolId > 0)
                expression = expression.And(r => r.SchoolId == schoolId);
            else if (name.Trim() != string.Empty)
                expression = expression.And(r => r.School.Name.Contains(name.Trim()));

            var list = userBusiness.GetTeacherAssignedSchools(expression, sort, order, first, count, out total);
            var result = new { total = total, data = list };
            return JsonHelper.SerializeObject(result);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.Administrative, Anonymity = Anonymous.Logined)]
        public string InsertTeacherSchoolRelations(int userId, string[] schoolIds)
        {
            var response = new PostFormResponse();
            OperationResult result = new OperationResult(OperationResultType.Success);
            result = userBusiness.InsertTeacherSchoolRelations(userId, UserInfo.ID, schoolIds);
            response.Success = result.ResultType == OperationResultType.Success;
            response.Data = "";
            response.Message = result.Message;
            return JsonHelper.SerializeObject(response);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.Administrative, Anonymity = Anonymous.Logined)]
        public string DeleteTeacherSchoolRelations(int userId, string[] schoolids)
        {
            var response = new PostFormResponse();
            OperationResult result = new OperationResult(OperationResultType.Success);
            result = userBusiness.DeleteTeacherSchoolRelations(userId, schoolids);
            response.Success = result.ResultType == OperationResultType.Success;
            response.Data = "";
            response.Message = result.Message;
            return JsonHelper.SerializeObject(response);
        }

        #endregion

        #region assign user to schools

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.Administrative, Anonymity = Anonymous.Logined)]
        public ActionResult AssignTrsSpecialistSchool(int userId)
        {
            UserBaseEntity user = userBusiness.GetUser(userId);
            ViewBag.NavigationText = user.Role.ToDescription();
            return View(user);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.Administrative, Anonymity = Anonymous.Logined)]
        public string SearchUnassigedTrsSpecialistSchools(int userId, string name = "", string communityname = "",
            int communityId = 0,
            int schoolId = 0, string sort = "SchoolName", string order = "Asc", int first = 0, int count = 10)
        {
            int total = 0;
            var expression = PredicateHelper.True<SchoolEntity>();
            expression = expression.And(r => !(r.UserCommunitySchools.Any(s => s.UserId == userId)));
            if (communityId > 0)
                expression =
                    expression.And(r => r.CommunitySchoolRelations.Any(e => e.CommunityId == communityId));
            else if (communityname.Trim() != string.Empty)
                expression =
                    expression.And(r => r.CommunitySchoolRelations.Any(e => e.Community.Name.Contains(communityname)));
            if (schoolId > 0)
                expression = expression.And(r => r.ID == schoolId);
            else if (name.Trim() != string.Empty)
                expression = expression.And(r => r.Name.Contains(name.Trim()));
            expression = expression.And(r => r.Status == SchoolStatus.Active);

            var list = schoolBusiness.SearchTrsSpecialistUnassigedSchools(UserInfo, expression, sort, order, first, count, out total);
            var result = new { total = total, data = list };
            return JsonHelper.SerializeObject(result);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.Administrative,
            Anonymity = Anonymous.Logined)]
        public string SearchAssignedTrsSpecialistSchools(int userId = 0, string name = "", string communityname = "",
            int communityId = 0
            , int schoolId = 0, string sort = "SchoolName", string order = "Asc", int first = 0, int count = 10)
        {
            int total = 0;

            var expression = PredicateHelper.True<UserComSchRelationEntity>();
            if (UserInfo.Role == Role.Content_personnel || UserInfo.Role == Role.Statisticians ||
                UserInfo.Role == Role.Administrative_personnel || UserInfo.Role == Role.Intervention_manager ||
                UserInfo.Role == Role.Video_coding_analyst || UserInfo.Role == Role.Intervention_support_personnel ||
                UserInfo.Role == Role.Coordinator || UserInfo.Role == Role.Mentor_coach)
                expression = expression.And(r => r.School.CommunitySchoolRelations.Any(
                    s => s.Community.UserCommunitySchools.Any(t => t.UserId == UserInfo.ID)));
            else if (UserInfo.Role == Role.Statewide)
            {
                var primarySchoolIds =
                    schoolBusiness.GetPrimarySchoolsByComId(
                        UserInfo.UserCommunitySchools.Select(x => x.CommunityId).ToList()).Select(x => x.ID);
                expression = expression.And(r => primarySchoolIds.Contains(r.SchoolId));
            }
            else if (UserInfo.Role == Role.Community ||
                     UserInfo.Role == Role.District_Community_Specialist)
            {
                expression =
                    expression.And(
                        r =>
                            r.School.CommunitySchoolRelations.Any(
                                s => s.Community.UserCommunitySchools.Any(t => t.UserId == UserInfo.ID)));
            }
            else if (UserInfo.Role == Role.District_Community_Delegate ||
                     UserInfo.Role == Role.Community_Specialist_Delegate)
            {
                var parentCommunityUser = userBusiness.GetUser(UserInfo.CommunityUser.ParentId);
                var primarySchoolIds =
                    schoolBusiness.GetPrimarySchoolsByComId(
                        parentCommunityUser.UserCommunitySchools.Select(x => x.CommunityId).ToList()).Select(x => x.ID);
                expression = expression.And(r => r.UserId == UserInfo.CommunityUser.ParentId);
            }
            else if (UserInfo.Role == Role.Principal || UserInfo.Role == Role.TRS_Specialist ||
                     UserInfo.Role == Role.School_Specialist)
            {
                expression = expression.And(r => r.School.UserCommunitySchools.Any(s => s.UserId == UserInfo.ID));
            }
            else if (UserInfo.Role == Role.Principal_Delegate || UserInfo.Role == Role.TRS_Specialist_Delegate ||
                     UserInfo.Role == Role.School_Specialist_Delegate)
                expression =
                    expression.And(
                        r => r.School.UserCommunitySchools.Any(s => s.UserId == UserInfo.Principal.ParentId));
            if (userId > 0)
                expression = expression.And(r => r.UserId == userId);
            if (communityId > 0)
                expression =
                    expression.And(r => r.School.CommunitySchoolRelations.Any(e => e.CommunityId == communityId));
            else if (communityname.Trim() != string.Empty)
                expression =
                    expression.And(
                        r => r.School.CommunitySchoolRelations.Any(e => e.Community.Name.Contains(communityname)));
            if (schoolId > 0)
                expression = expression.And(r => r.SchoolId == schoolId);
            else if (name.Trim() != string.Empty)
                expression = expression.And(r => r.School.Name.Contains(name.Trim()));

            var list = userBusiness.GetAssignedTrsSpecialistSchools(expression, sort, order, first, count, out total);
            var result = new { total = total, data = list };
            return JsonHelper.SerializeObject(result);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.Administrative, Anonymity = Anonymous.Logined)]
        public string InsertTrsSpecialistSchoolRelations(int userId, int[] schoolIds)
        {
            var response = new PostFormResponse();
            OperationResult result = new OperationResult(OperationResultType.Success);
            result = userBusiness.InsertUserSchoolRelations(userId, UserInfo.ID, schoolIds);
            response.Success = result.ResultType == OperationResultType.Success;
            response.Data = "";
            response.Message = result.Message;
            return JsonHelper.SerializeObject(response);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.Administrative, Anonymity = Anonymous.Logined)]
        public string DeleteTrsSpecialistSchoolRelations(int userId, int[] schoolIds)
        {
            var response = new PostFormResponse();
            OperationResult result = new OperationResult(OperationResultType.Success);
            result = userBusiness.DeleteUserSchoolRelations(userId, schoolIds);
            response.Success = result.ResultType == OperationResultType.Success;
            response.Data = "";
            response.Message = result.Message;
            return JsonHelper.SerializeObject(response);
        }
        #endregion

        #region assign user to classes

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.Administrative, Anonymity = Anonymous.Logined)]
        public ActionResult AssignClass(int userId)
        {
            UserBaseEntity user = userBusiness.GetUser(userId);
            ViewBag.NavigationText = user.Role.ToDescription();
            return View(user);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.Administrative,
            Anonymity = Anonymous.Logined)]
        public string SearchUnassigedClasses(int userId, int communityId = 0, string communityName = "",
            int schoolId = 0, string schoolName = "", string name = "",
            string sort = "SchoolName", string order = "Asc", int first = 0, int count = 10)
        {
            var user = userBusiness.GetUser(userId);
            List<int> schoolIds = user.UserCommunitySchools.Select(e => e.SchoolId).ToList();
            int total = 0;
            var expression = PredicateHelper.True<ClassEntity>();
            if (userId > 0)
            {
                expression = expression.And(o =>
                    !(o.UserClasses.Any(p => p.UserId == userId
                                             && p.Status == EntityStatus.Active))
                    && schoolIds.Contains(o.SchoolId));
            }
            if (communityId > 0)
                expression =
                    expression.And(r => r.School.CommunitySchoolRelations.Any(e => e.CommunityId == communityId));
            else if (communityName.Trim() != string.Empty)
                expression =
                    expression.And(r => r.School.CommunitySchoolRelations.Any(e => e.Community.Name.Contains(communityName)));
            if (schoolId > 0)
                expression = expression.And(r => r.SchoolId == schoolId);
            else if (name.Trim() != string.Empty)
                expression = expression.And(r => r.School.Name.Contains(name.Trim()));
            if (name.Trim() != string.Empty)
                expression = expression.And(r => r.Name.Contains(name.Trim()));
            var list = classBusiness.AssignClass(UserInfo, expression, sort, order, first, count, out total);
            var result = new { total = total, data = list };
            return JsonHelper.SerializeObject(result);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.Administrative, Anonymity = Anonymous.Logined)]
        public string SearchAssignedClasses(int userId = 0, int communityId = 0, string communityName = "",
            int schoolId = 0, string schoolName = "", string name = "", string sort = "ClassName", string order = "Asc", int first = 0, int count = 10)
        {
            int total = 0;
            var expression = PredicateHelper.True<UserClassRelationEntity>();
            expression = expression.And(r => r.Status == EntityStatus.Active);
            if (UserInfo.Role == Role.Content_personnel || UserInfo.Role == Role.Statisticians ||
                UserInfo.Role == Role.Administrative_personnel || UserInfo.Role == Role.Intervention_manager ||
                UserInfo.Role == Role.Video_coding_analyst || UserInfo.Role == Role.Intervention_support_personnel ||
                UserInfo.Role == Role.Coordinator || UserInfo.Role == Role.Mentor_coach)
            {
                expression = expression.And(r => r.Class.School.CommunitySchoolRelations.Any
                    (s => s.Community.UserCommunitySchools.Any(t => t.UserId == UserInfo.ID)));
            }
            else if (UserInfo.Role == Role.Community || UserInfo.Role == Role.District_Community_Specialist || UserInfo.Role==Role.Statewide)
            {
                var primarySchoolIds =
                    schoolBusiness.GetPrimarySchoolsByComId(
                        UserInfo.UserCommunitySchools.Select(x => x.CommunityId).ToList()).Select(x => x.ID);
                expression = expression.And(r => primarySchoolIds.Contains(r.Class.SchoolId));
            }
            else if (UserInfo.Role == Role.District_Community_Delegate ||
                     UserInfo.Role == Role.Community_Specialist_Delegate)
            {
                var parentCommunityUser = userBusiness.GetUser(UserInfo.CommunityUser.ParentId);
                var primarySchoolIds =
                    schoolBusiness.GetPrimarySchoolsByComId(
                        parentCommunityUser.UserCommunitySchools.Select(x => x.CommunityId).ToList()).Select(x => x.ID);
                expression = expression.And(r => primarySchoolIds.Contains(r.Class.SchoolId));
            }
            else if (UserInfo.Role == Role.Principal || UserInfo.Role == Role.TRS_Specialist
                     || UserInfo.Role == Role.School_Specialist)
            {
                expression =
                    expression.And(r => r.Class.School.UserCommunitySchools.Any(s => s.UserId == UserInfo.ID));
            }
            else if (UserInfo.Role == Role.Principal || UserInfo.Role == Role.TRS_Specialist_Delegate
                     || UserInfo.Role == Role.School_Specialist_Delegate)
            {
                expression =
                    expression.And(
                        r => r.Class.School.UserCommunitySchools.Any(s => s.UserId == UserInfo.Principal.ParentId));
            }
            if (userId > 0)
                expression = expression.And(r => r.UserId == userId);
            if (communityId > 0)
                expression =
                    expression.And(r => r.Class.School.CommunitySchoolRelations.Any(e => e.CommunityId == communityId));
            else if (communityName.Trim() != string.Empty)
                expression =
                    expression.And(r => r.Class.School.CommunitySchoolRelations.Any(e => e.Community.Name.Contains(communityName)));
            if (schoolId > 0)
                expression = expression.And(r => r.Class.SchoolId == schoolId);
            else if (name.Trim() != string.Empty)
                expression = expression.And(r => r.Class.School.Name.Contains(name.Trim()));
            if (name.Trim() != string.Empty)
                expression = expression.And(r => r.Class.Name.Contains(name.Trim()));
            var list = userBusiness.GetAssignedClasses(expression, sort, order, first, count, out total);
            var result = new { total = total, data = list };
            return JsonHelper.SerializeObject(result);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.Administrative, Anonymity = Anonymous.Logined)]
        public string InsertUserClassRelations(int userId, int[] classIds)
        {
            var response = new PostFormResponse();
            OperationResult result = new OperationResult(OperationResultType.Success);
            result = userBusiness.InsertUserClassRelationsMoreClass(userId, UserInfo.ID, classIds);
            response.Success = result.ResultType == OperationResultType.Success;
            response.Data = "";
            response.Message = result.Message;
            return JsonHelper.SerializeObject(response);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.Administrative, Anonymity = Anonymous.Logined)]
        public string DeleteUserClassRelations(int userId, int[] classIds)
        {
            var response = new PostFormResponse();
            OperationResult result = new OperationResult(OperationResultType.Success);
            result = userBusiness.DeleteUserClassRelationsMoreClass(classIds, userId);
            response.Success = result.ResultType == OperationResultType.Success;
            response.Data = "";
            response.Message = result.Message;
            return JsonHelper.SerializeObject(response);
        }
        #endregion

        private void InitAccessOperation()
        {
            bool accessView = false;
            bool accessEdit = false;
            if (UserInfo != null)
            {
                UserAuthorityModel userAuthority = new PermissionBusiness().GetUserAuthority(UserInfo, (int)PagesModel.Delegate);

                if (userAuthority != null)
                {
                    if ((userAuthority.Authority & (int)Authority.View) == (int)Authority.View)
                    {
                        accessView = true;
                    }
                    if ((userAuthority.Authority & (int)Authority.Edit) == (int)Authority.Edit)
                    {
                        accessEdit = true;
                    }
                }
            }
            ViewBag.accessView = accessView;
            ViewBag.accessEdit = accessEdit;
        }
    }
}