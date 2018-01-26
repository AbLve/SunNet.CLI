using Sunnet.Cli.Core.Users.Enums;
using Sunnet.Cli.MainSite.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Sunnet.Framework.Extensions;
using Sunnet.Framework.Helpers;
using Sunnet.Cli.Core.Users.Entities;
using Sunnet.Cli.Business.Users;
using Sunnet.Cli.UIBase.Models;
using Sunnet.Framework.Core.Tool;
using Sunnet.Cli.Business.Schools;
using Sunnet.Cli.Business.Schools.Models;
using Sunnet.Cli.Business.Permission;
using Sunnet.Cli.Business.Communities;
using Sunnet.Cli.Core.Communities.Entities;
using StructureMap;
using Sunnet.Framework.Encrypt;
using Sunnet.Framework;
using Sunnet.Cli.Business.Common;
using Sunnet.Cli.UIBase.Filters;
using Sunnet.Framework.Permission;
using Sunnet.Cli.Business.MasterData;
using Sunnet.Cli.Core.MasterData.Entities;
using Sunnet.Cli.Business.Permission.Models;
using Sunnet.Cli.Business.Communities.Models;
using Sunnet.Framework.Resources;

namespace Sunnet.Cli.MainSite.Areas.UserVerification.Controllers
{
    public class VerificationRequestController : BaseController
    {
        private readonly UserBusiness userBusiness;
        private readonly CommunityBusiness communityBusiness;
        private readonly SchoolBusiness schoolBusiness;
        private readonly PermissionBusiness permissionBusiness;
        private readonly MasterDataBusiness masterDataBusiness;
        public VerificationRequestController()
        {
            userBusiness = new UserBusiness(UnitWorkContext);
            communityBusiness = new CommunityBusiness(UnitWorkContext);
            schoolBusiness = new SchoolBusiness(UnitWorkContext);
            permissionBusiness = new PermissionBusiness(UnitWorkContext);
            masterDataBusiness = new MasterDataBusiness(UnitWorkContext);
        }
        //
        // GET: /UserVerification/VerificationRequest/
        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.User_Verification, Anonymity = Anonymous.Verified)]
        public ActionResult Index()
        {
            InitAccessOperation();
            List<string> listRole = new List<string>();
            if (UserInfo.Role == Role.Principal)
            {
                listRole.Add(Role.Teacher.ToDescription());
            }
            else if (UserInfo.Role == Role.Community)
            {
                listRole.Add(Role.District_Community_Specialist.ToDescription());
                listRole.Add(Role.Principal.ToDescription());
                listRole.Add(Role.School_Specialist.ToDescription());
                listRole.Add(Role.TRS_Specialist.ToDescription());
                listRole.Add(Role.Teacher.ToDescription());
            }
            else if (UserInfo.IsCLIUser)
            {
                listRole.Add(Role.Community.ToDescription());
                listRole.Add(Role.District_Community_Specialist.ToDescription());
                listRole.Add(Role.Principal.ToDescription());
                listRole.Add(Role.School_Specialist.ToDescription());
                listRole.Add(Role.TRS_Specialist.ToDescription());
                listRole.Add(Role.Teacher.ToDescription());
            }
            ViewBag.RoleTypeOptions = Role.Administrative_personnel.ToSelectList().Where(e => listRole.Contains(e.Text))
                .AddDefaultItem(ViewTextHelper.DefaultAllText, "-1", 0);
            return View();
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.User_Verification, Anonymity = Anonymous.Verified)]
        public string Search(string keyword, int roleType = -1, int status = -1,
            string sort = "LastName", string order = "Asc", int first = 0, int count = 10)
        {
            var total = 0;
            var expression = PredicateHelper.True<ApplicantEntity>();

            if (roleType > 0)
                expression = expression.And(r => r.RoleType == (Role)roleType);
            if (status > 0)
                expression = expression.And(r => (int)r.Status == status);
            if (keyword.Trim() != string.Empty)
                expression = expression.And(r => r.FirstName.Contains(keyword)
                    || r.LastName.Contains(keyword) || r.Email.Contains(keyword));
            expression = expression.And(r => r.IsDeleted == false);
            //当用户选择的community或者school未认证时，则需要进入确认页面1,但如果用户未填写确认页面1的信息，直接离开，则该用户请求不会显示
            //expression = expression.And(r => r.ApplicantContacts.FirstOrDefault() != null || r.CommunityId > 0);

            var list = userBusiness.SearchVertificationRequests(UserInfo, expression, sort, order, first, count, out total);
            var result = new { total = total, data = list };
            return JsonHelper.SerializeObject(result);
        }

        [System.Web.Mvc.HttpPost]
        [CLIUrlAuthorizeAttribute(Account = Authority.Delete, PageId = PagesModel.User_Verification, Anonymity = Anonymous.Verified)]
        public string Delete(int id)
        {
            var response = new PostFormResponse();
            var result = userBusiness.DeleteApplicant(id);
            response.Success = result.ResultType == OperationResultType.Success;
            response.Message = result.Message;
            return JsonHelper.SerializeObject(response);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.Edit, PageId = PagesModel.User_Verification, Anonymity = Anonymous.Verified)]
        public ActionResult CommunityVerification(int id)
        {
            CommunityEntity community = new CommunityEntity();
            ApplicantEntity applicant = userBusiness.GetApplicant(id);
            string stateName = "";
            if (applicant.StateId > 0)
            {
                StateEntity state = masterDataBusiness.GetState(applicant.StateId);
                if (state != null)
                {
                    stateName = state.Name;
                }
            }
            ViewBag.CommunityInfo = applicant.Address + ", " + applicant.City
                                    + ", " + stateName + " " + applicant.Zip;
            ApplicantContactEntity applicantContact = applicant.ApplicantContacts.FirstOrDefault();
            if (applicantContact != null)
            {
                ViewBag.BasicCommunityId = applicantContact.CommunityId;
                CommunityModel communityModel = communityBusiness.IsVerified(applicantContact.CommunityId.Value);
                if (communityModel != null)
                {
                    applicant.CommunityId = communityModel.ID;
                }
                else
                {
                    ViewBag.CommunityName = applicantContact.CommunityName;
                }
            }
            if (applicant.CommunityId > 0)
            {
                community = communityBusiness.GetCommunity(applicant.CommunityId);
                if (community != null)
                {
                    applicant.CommunityId = applicant.CommunityId;
                    ViewBag.CommunityName = community.BasicCommunity.Name;
                    ViewBag.CommunityInfo = community.PhysicalAddress1 + ", " + community.City + ", "
                                            + community.State.Name + " " + community.Zip;
                    ViewBag.ShowType = "disabled";
                }
            }
            if (applicant.CommunityId > 0)
                ViewBag.GroupPackageModels = permissionBusiness.GetAssignedPackages((int)applicant.RoleType, null,
                    applicant.CommunityId);
            if (applicant.InviteeId > 0)
            {
                UsernameModel userNameModel = userBusiness.GetUsername(applicant.InviteeId);
                ViewBag.InviteeUserName = userNameModel.Firstname + " " + userNameModel.Lastname;
            }
            if (applicant.SponsorId > 0)
            {
                UsernameModel userNameModel = userBusiness.GetUsername(applicant.SponsorId);
                ViewBag.SponsorUserName = userNameModel.Firstname + " " + userNameModel.Lastname;
            }
            return View(applicant);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.Edit, PageId = PagesModel.User_Verification, Anonymity = Anonymous.Verified)]
        public ActionResult PrincipalVerification(int id)
        {
            CommunityEntity community = new CommunityEntity();
            ApplicantEntity applicant = userBusiness.GetApplicant(id);
            ApplicantContactEntity applicantContact = applicant.ApplicantContacts.FirstOrDefault();
            if (applicantContact != null)
            {
                ViewBag.CommunityName = applicantContact.CommunityName;
                string stateName = "";
                if (applicantContact.StateId > 0)
                {
                    StateEntity state = masterDataBusiness.GetState(applicantContact.StateId);
                    if (state != null)
                    {
                        stateName = state.Name;
                    }
                }
                ViewBag.CommunityInfo = applicantContact.Address + ", " + applicantContact.City
                    + ", " + stateName + " " + applicantContact.Zip;
                ViewBag.SchoolName = applicantContact.SchoolName;
                applicant.CommunityId = 0;
            }
            else
            {
                if (applicant.CommunityId > 0)
                {
                    community = communityBusiness.GetCommunity(applicant.CommunityId);
                    if (community != null)
                    {
                        applicant.CommunityId = applicant.CommunityId;
                        ViewBag.CommunityName = community.BasicCommunity.Name;
                        ViewBag.CommunityInfo = community.PhysicalAddress1 + ", " + community.City + ", "
                            + community.State.Name + " " + community.Zip;
                        ViewBag.ShowType = "disabled";
                    }
                }
            }

            if (applicant.CommunityId > 0)
                ViewBag.GroupPackageModels = permissionBusiness.GetAssignedPackages((int)applicant.RoleType, null, applicant.CommunityId);
            if (applicant.InviteeId > 0)
            {
                UsernameModel userNameModel = userBusiness.GetUsername(applicant.InviteeId);
                ViewBag.InviteeUserName = userNameModel.Firstname + " " + userNameModel.Lastname;
            }
            if (applicant.SponsorId > 0)
            {
                UsernameModel userNameModel = userBusiness.GetUsername(applicant.SponsorId);
                ViewBag.SponsorUserName = userNameModel.Firstname + " " + userNameModel.Lastname;
            }
            if (applicant.PositionId > 0)
            {
                PositionEntity position = userBusiness.GetPosition(applicant.PositionId);
                if (position != null)
                    ViewBag.JobTitle = position.Title == "Other" ? applicant.PositionOther : position.Title;
            }
            return View(applicant);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.Edit, PageId = PagesModel.User_Verification, Anonymity = Anonymous.Verified)]
        public ActionResult TRSSchoolVerification(int id)
        {
            CommunityEntity community = new CommunityEntity();
            ApplicantEntity applicant = userBusiness.GetApplicant(id);
            SchoolModel school = schoolBusiness.GetActiveSchoolEntity(applicant.SchoolId);
            if (school != null)
            {
                ViewBag.SchoolName = school.SchoolName;
                applicant.SchoolId = school.ID;
            }
            else
            {
                if (applicant.ApplicantContacts.FirstOrDefault() != null)
                    ViewBag.SchoolName = applicant.ApplicantContacts.FirstOrDefault().SchoolName;
                applicant.SchoolId = 0;
            }
            if (applicant.SchoolId == 0)
                ViewBag.GroupPackageModels = permissionBusiness.GetAssignedPackages((int)applicant.RoleType, null, applicant.CommunityId);
            else
                ViewBag.GroupPackageModels = permissionBusiness.GetAssignedPackages((int)applicant.RoleType, applicant.SchoolId, applicant.CommunityId);
            if (applicant.CommunityId > 0)
            {
                community = communityBusiness.GetCommunity(applicant.CommunityId);
                ViewBag.CommunityName = community.BasicCommunity.Name;
                ViewBag.CommunityInfo = community.PhysicalAddress1 + ", " + community.City + ", "
                    + community.State.Name + " " + community.Zip;
            }

            if (applicant.InviteeId > 0)
            {
                UsernameModel userNameModel = userBusiness.GetUsername(applicant.InviteeId);
                ViewBag.InviteeUserName = userNameModel.Firstname + " " + userNameModel.Lastname;
            }
            if (applicant.SponsorId > 0)
            {
                UsernameModel userNameModel = userBusiness.GetUsername(applicant.SponsorId);
                ViewBag.SponsorUserName = userNameModel.Firstname + " " + userNameModel.Lastname;
            }
            return View(applicant);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.Edit, PageId = PagesModel.User_Verification, Anonymity = Anonymous.Verified)]
        public ActionResult TeacherVerification(int id)
        {
            CommunityEntity community = new CommunityEntity();
            ApplicantEntity applicant = userBusiness.GetApplicant(id);
            SchoolModel school = schoolBusiness.GetActiveSchoolEntity(applicant.SchoolId);
            if (school != null)
            {
                ViewBag.SchoolName = school.SchoolName;
                applicant.SchoolId = school.ID;
            }
            else
            {
                if (applicant.ApplicantContacts.FirstOrDefault() != null)
                    ViewBag.SchoolName = applicant.ApplicantContacts.FirstOrDefault().SchoolName;
                applicant.SchoolId = 0;
            }

            if (UserInfo.Role == Role.Principal || UserInfo.Role == Role.Principal_Delegate)
            {
                // todo:User 与 Schools关系改为了UserCommunitySchools
                applicant.CommunityId = UserInfo.UserCommunitySchools.FirstOrDefault().School.CommunitySchoolRelations.FirstOrDefault().CommunityId;
                applicant.SchoolId = UserInfo.UserCommunitySchools.FirstOrDefault().SchoolId;
                ViewBag.ShowType = "disabled";
            }
            if (applicant.SchoolId == 0)
                ViewBag.GroupPackageModels = permissionBusiness.GetAssignedPackages((int)applicant.RoleType, null, applicant.CommunityId);
            else
                ViewBag.GroupPackageModels = permissionBusiness.GetAssignedPackages((int)applicant.RoleType, applicant.SchoolId, applicant.CommunityId);
            if (applicant.CommunityId > 0)
            {
                community = communityBusiness.GetCommunity(applicant.CommunityId);
                ViewBag.CommunityName = community.BasicCommunity.Name;
                ViewBag.CommunityInfo = community.PhysicalAddress1 + ", " + community.City + ", "
                    + community.State.Name + " " + community.Zip;
            }

            if (applicant.InviteeId > 0)
            {
                UsernameModel userNameModel = userBusiness.GetUsername(applicant.InviteeId);
                ViewBag.InviteeUserName = userNameModel.Firstname + " " + userNameModel.Lastname;
            }
            if (applicant.SponsorId > 0)
            {
                UsernameModel userNameModel = userBusiness.GetUsername(applicant.SponsorId);
                ViewBag.SponsorUserName = userNameModel.Firstname + " " + userNameModel.Lastname;
            }
            return View(applicant);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.Edit, PageId = PagesModel.User_Verification, Anonymity = Anonymous.Verified)]
        public string SaveUserVerification(ApplicantEntity applicant, int[] packageIds, bool confirm = false)
        {
            var response = new PostFormResponse();
            ApplicantEntity applicantEntity = userBusiness.GetApplicant(applicant.ID);
            applicantEntity.Status = ApplicantStatus.Invited;
            applicantEntity.CommunityId = applicant.CommunityId;
            applicantEntity.SchoolId = applicant.SchoolId;
            applicantEntity.SponsorId = UserInfo.ID;
            applicantEntity.InvitedOn = DateTime.Now;
            if (confirm == false)
            {
                switch (applicantEntity.RoleType)
                {
                    case Role.Community:
                    case Role.District_Community_Specialist:
                        if (CheckCommunityExisted(applicantEntity, response))
                            return JsonHelper.SerializeObject(response);
                        break;
                    case Role.Principal:
                    case Role.School_Specialist:
                    case Role.TRS_Specialist:
                        if (CheckPrincipalExisted(applicantEntity, response))
                            return JsonHelper.SerializeObject(response);
                        break;
                    case Role.Teacher:
                        if (CheckTeacherExisted(applicantEntity, response))
                            return JsonHelper.SerializeObject(response);
                        break;
                }
            }
            if (response.Success = ModelState.IsValid)
            {
                OperationResult result = new OperationResult(OperationResultType.Success);
                if (applicantEntity.InviteeId == 0)
                {
                    result = userBusiness.UserVerification(applicantEntity, packageIds, UserInfo.ID);
                }
                response.Success = result.ResultType == OperationResultType.Success;
                response.Message = result.Message;
            }
            EmailTemplete template = XmlHelper.GetEmailTemplete("UserVerification_3A_Template.xml");
            string param = applicantEntity.InviteeId.ToString() + "," + DateTime.Now.ToString();
            string encryptParam = ObjectFactory.GetInstance<IEncrypt>().Encrypt(param);
            string link = SFConfig.MainSiteDomain + "Home/InviteVerification/"
                + System.Web.HttpContext.Current.Server.UrlEncode(encryptParam);
            string emailBody = template.Body.Replace("{FirstName}", applicantEntity.FirstName)
                .Replace("{LastName}", applicantEntity.LastName)
                .Replace("{InviteLink}", "<a style='text-decoration: underline; cursor:pointer; color: #008000;' href='" + link + "'>Click here</a>")
                .Replace("{StaticDomain}", SFConfig.StaticDomain)
                .Replace("{CliEngageUrl}", SFConfig.MainSiteDomain);
            userBusiness.SendEmail(applicantEntity.Email, template.Subject, emailBody);
            EmailLogEntity emailLog = new EmailLogEntity(applicantEntity.InviteeId, applicantEntity.Email, EmailLogType.Notification);
            userBusiness.InsertEmailLog(emailLog);
            response.ModelState = ModelState;
            return JsonHelper.SerializeObject(response);
        }


        #region user exists in engage?

        private bool CheckCommunityExisted(ApplicantEntity applicant, PostFormResponse response)
        {
            var existedUserId = 0;
            Role existedUserRole;
            var existedStatus = userBusiness.CheckCommunityUserExistedStatus(applicant.ID,applicant.FirstName,
                applicant.LastName, applicant.Email, applicant.RoleType,
                applicant.CommunityId, out existedUserId, out existedUserRole);
            if (existedStatus == UserExistedStatus.ExistedInCommunity)
            {
                // do nothing
                response.Success = true;
                response.Message = ResourceHelper.GetRM().GetInformation("UserExistedInCommunity")
                    .Replace("{Role}", existedUserRole.ToDescription().ToLower());
                response.Data = "waiting";
                return true;
            }
            else if (existedStatus == UserExistedStatus.UserExisted)
            {
                if (existedUserRole == applicant.RoleType)
                {
                    response.Success = true;
                    response.Message = ResourceHelper.GetRM().GetInformation("UserExistedInCurrentRole")
                        .Replace("{Role}", existedUserRole.ToDescription().ToLower());
                    response.Data = new
                    {
                        type = "confirmAssign",
                        url =
                            Url.Action("AssignCommunity", "Public",
                                new { userId = existedUserId, roleType = applicant.RoleType, Area = "Invitation" })
                    };
                    return true;
                }
                else
                {
                    // 存在其他角色的用户提醒 Continue
                    response.Success = true;
                    response.Message = ResourceHelper.GetRM()
                        .GetInformation("UserExistedInSystem")
                        .Replace("{Role}", existedUserRole.ToDescription().ToLower());
                    response.Data = new
                    {
                        type = "continue"
                    };
                    return true;
                }
            }
            return false;
        }

        private bool CheckPrincipalExisted(ApplicantEntity applicant, PostFormResponse response)
        {
            var existedUserId = 0;
            Role existedUserRole;
            var existedStatus = userBusiness.CheckPrincipalUserExistedStatus(0, applicant.FirstName,
                applicant.LastName, applicant.Email, applicant.RoleType,
                applicant.SchoolId, out existedUserId, out existedUserRole);
            if (existedStatus == UserExistedStatus.ExistedInSchool)
            {
                // do nothing
                response.Success = true;
                response.Message = ResourceHelper.GetRM().GetInformation("UserExistedInSchool")
                    .Replace("{Role}", existedUserRole.ToDescription().ToLower());
                response.Data = "waiting";
                return true;
            }
            else if (existedStatus == UserExistedStatus.UserExisted)
            {
                if (existedUserRole == applicant.RoleType)
                {
                    response.Success = true;
                    response.Message = ResourceHelper.GetRM().GetInformation("UserExistedInCurrentRole")
                        .Replace("{Role}", existedUserRole.ToDescription().ToLower());
                    response.Data = new
                    {
                        type = "confirmAssign",
                        url =
                            Url.Action("AssignSchool", "Public",
                                new { userId = existedUserId, roleType = applicant.RoleType, Area = "Invitation" })
                    };
                    return true;
                }
                else
                {
                    // 存在其他角色的用户提醒 Continue
                    response.Success = true;
                    response.Message = ResourceHelper.GetRM()
                        .GetInformation("UserExistedInSystem")
                        .Replace("{Role}", existedUserRole.ToDescription().ToLower());
                    response.Data = new
                    {
                        type = "continue"
                    };
                    return true;
                }
            }
            return false;
        }

        private bool CheckTeacherExisted(ApplicantEntity applicant, PostFormResponse response)
        {
            var existedUserId = 0;
            Role existedUserRole;
            var existedStatus = userBusiness.CheckTeacherUserExistedStatus(0, applicant.FirstName,
                applicant.LastName, applicant.Email, applicant.RoleType,
                applicant.CommunityId, applicant.SchoolId, out existedUserId, out existedUserRole);
            if (existedStatus == UserExistedStatus.ExistedInSchool)
            {
                // do nothing
                response.Success = true;
                response.Message = ResourceHelper.GetRM().GetInformation("UserExistedInSchool")
                    .Replace("{Role}", existedUserRole.ToDescription().ToLower());
                response.Data = "waiting";
                return true;
            }
            else if (existedStatus == UserExistedStatus.ExistedInCommunity)
            {
                response.Success = true;
                response.Message = ResourceHelper.GetRM().GetInformation("UserExistedInCommunity")
                    .Replace("{Role}", existedUserRole.ToDescription().ToLower());
                response.Data = "waiting";
                return true;
            }
            else if (existedStatus == UserExistedStatus.UserExisted)
            {
                if (existedUserRole == applicant.RoleType)
                {
                    response.Success = true;
                    response.Message = ResourceHelper.GetRM().GetInformation("UserExistedInCurrentRole")
                        .Replace("{Role}", existedUserRole.ToDescription().ToLower());
                    response.Data = new
                    {
                        type = "confirmAssign",
                        url =
                            Url.Action("TeacherAssignSchool", "Public",
                                new { userId = existedUserId, roleType = applicant.RoleType, Area = "Invitation" })
                    };
                    return true;
                }
                else
                {
                    // 存在其他角色的用户提醒 Continue
                    response.Success = true;
                    response.Message = ResourceHelper.GetRM()
                        .GetInformation("UserExistedInSystem")
                        .Replace("{Role}", existedUserRole.ToDescription().ToLower());
                    response.Data = new
                    {
                        type = "continue"
                    };
                    return true;
                }
            }
            return false;
        }

        #endregion

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.Administrative, Anonymity = Anonymous.Logined)]
        public string GetAssignedPackages(int userType, int? communityId, int? schoolId)
        {
            return JsonHelper.SerializeObject(permissionBusiness.GetAssignedPackages(userType, schoolId, communityId));
        }

        private void InitAccessOperation()
        {
            bool accessDelete = false;
            bool accessEdit = false;
            if (UserInfo != null)
            {
                UserAuthorityModel userAuthority = new PermissionBusiness().GetUserAuthority(UserInfo, (int)PagesModel.User_Verification);

                if (userAuthority != null)
                {
                    if ((userAuthority.Authority & (int)Authority.Delete) == (int)Authority.Delete)
                    {
                        accessDelete = true;
                    }
                    if ((userAuthority.Authority & (int)Authority.Edit) == (int)Authority.Edit)
                    {
                        accessEdit = true;
                    }
                }
            }
            ViewBag.accessEdit = accessEdit;
            ViewBag.accessDelete = accessDelete;
        }
    }
}