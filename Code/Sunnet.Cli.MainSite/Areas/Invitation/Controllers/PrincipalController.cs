using System.Web.UI.WebControls;
using Sunnet.Cli.Business.Log;
using Sunnet.Cli.Business.StatusTracking;
using Sunnet.Cli.Business.Users;
using Sunnet.Cli.Core.Log.Entities;
using Sunnet.Cli.Core.Schools.Entities;
using Sunnet.Cli.Core.StatusTracking.Entities;
using Sunnet.Cli.Core.StatusTracking.Enums;
using Sunnet.Cli.Core.Users.Entities;
using Sunnet.Cli.MainSite.Models;
using Sunnet.Cli.UIBase.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sunnet.Framework.Extensions;
using Sunnet.Cli.Business.Communities;
using Sunnet.Framework.Core.Tool;
using Sunnet.Cli.Business.Common;
using Sunnet.Cli.Core.Common.Enums;
using Sunnet.Cli.Business.Schools;
using Sunnet.Cli.MainSite.Controllers;
using Sunnet.Cli.Core.Users.Enums;
using Sunnet.Framework.Helpers;
using StructureMap;
using Sunnet.Framework;
using Sunnet.Framework.Encrypt;
using Sunnet.Cli.Business.Permission.Models;
using Sunnet.Cli.Business.Permission;
using Sunnet.Cli.Core.Permission.Entities;
using Sunnet.Framework.Permission;
using Sunnet.Cli.UIBase.Filters;
using Sunnet.Cli.Business.Users.Models;
using Sunnet.Cli.Core.MasterData.Entities;
using Sunnet.Cli.Business.MasterData;
using Sunnet.Framework.Resources;
using Sunnet.Cli.Core.Ade;

namespace Sunnet.Cli.MainSite.Areas.Invitation.Controllers
{
    public class PrincipalController : BaseController
    {
        private readonly UserBusiness userBusiness;
        private readonly PermissionBusiness permissionBusiness;
        private readonly MasterDataBusiness masterDataBusiness;
        private readonly StatusTrackingBusiness statusTrackingBusiness;
        private readonly OperationLogBusiness operationLogBusiness;
        private readonly SchoolBusiness schoolBusiness;
        public PrincipalController()
        {
            userBusiness = new UserBusiness(UnitWorkContext);
            permissionBusiness = new PermissionBusiness(UnitWorkContext);
            masterDataBusiness = new MasterDataBusiness(UnitWorkContext);
            statusTrackingBusiness = new StatusTrackingBusiness(UnitWorkContext);
            operationLogBusiness = new OperationLogBusiness(UnitWorkContext);
            schoolBusiness = new SchoolBusiness(UnitWorkContext);
        }
        //
        // GET: /Invitation/Principal/
        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.Principal_Director, Anonymity = Anonymous.Verified)]
        public ActionResult Index()
        {
            InitAccessOperation();
            ViewBag.RoleType = Role.Principal;
            UserModel userModel = new UserModel();
            return View(userModel);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.Principal_Director, Anonymity = Anonymous.Verified)]
        public ActionResult MyDelegate()
        {
            InitAccessOperation();
            ViewBag.RoleType = Role.Principal_Delegate;
            return View();
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.Principal_Director, Anonymity = Anonymous.Verified)]
        public string Search(int roleType, string txtCommunity, string txtSchool, string principalCode, string firstName, string lastName, int communityId = 0,
            int schoolId = 0, int status = -1, string sort = "LastName", string order = "Asc", int first = 0, int count = 10)
        {
            var total = 0;
            var expression = PredicateHelper.True<PrincipalEntity>();
            if (communityId > 0)
                expression = expression.And(r => r.UserInfo.UserCommunitySchools.
                    Any(m => m.School.CommunitySchoolRelations.Any(n => n.CommunityId == communityId)));
            else if (txtCommunity.Trim() != string.Empty)
                expression = expression.And(r => r.UserInfo.UserCommunitySchools.
                    Any(m => m.School.CommunitySchoolRelations.Any(n => n.Community.Name.Contains(txtCommunity))));
            if (schoolId > 0)
                expression = expression.And(r => r.UserInfo.UserCommunitySchools.Any(e => e.SchoolId == schoolId));
            else if (txtSchool.Trim() != string.Empty)
                expression = expression.And(r => r.UserInfo.UserCommunitySchools.Any(e => e.School.Name.Contains(txtSchool)));
            if (status > 0)
                expression = expression.And(r => (int)r.UserInfo.Status == status);
            if (principalCode.Trim() != string.Empty)
                expression = expression.And(r => r.PrincipalId.Contains(principalCode));
            if (firstName.Trim() != string.Empty)
                expression = expression.And(r => r.UserInfo.FirstName.Contains(firstName));
            if (lastName.Trim() != string.Empty)
                expression = expression.And(r => r.UserInfo.LastName.Contains(lastName));

            var list = userBusiness.SearchPrincipals(UserInfo, (Role)roleType, expression, sort, order, first, count, out total, true);
            var result = new { total = total, data = list };
            return JsonHelper.SerializeObject(result);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.Add, PageId = PagesModel.Principal_Director, Anonymity = Anonymous.Verified)]
        public ActionResult New(int roleType)
        {
            UserBaseEntity userEntity = new UserBaseEntity();
            PrincipalEntity principalEntity = new PrincipalEntity();
            principalEntity.UserInfo = userEntity;
            ViewBag.Language = userBusiness.GetLanguages().ToSelectList(ViewTextHelper.DefaultPleaseSelectText, "0");
            ViewBag.State = userBusiness.GetStates().ToSelectList(ViewTextHelper.DefaultStateText, "");
            ViewBag.County = userBusiness.GetCountries().ToSelectList(ViewTextHelper.DefaultCountyText, "");
            ViewBag.PositionOptions = userBusiness.GetPositions((int)Role.Principal).
                ToSelectList(ViewTextHelper.DefaultPleaseSelectText, "");
            ViewBag.PDList = userBusiness.GetPDs();
            ViewBag.CertificateList = userBusiness.GetCertificates();

            ViewBag.CommunityName = "";
            ViewBag.CommunityId = 0;

            ViewBag.RoleType = (Role)roleType;
            principalEntity.UserInfo.Role = (Role)roleType;
            if ((Role)roleType == Role.Principal_Delegate)
                ViewBag.ParentSchoolNames = UserInfo.UserCommunitySchools.Select(x => x.School.Name).ToArray();
            return View(principalEntity);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.Edit, PageId = PagesModel.Principal_Director, Anonymity = Anonymous.Verified)]
        public ActionResult Edit(int id, int roleType)
        {
            PrincipalEntity principalEntity = userBusiness.GetPrincipal(id, UserInfo, (Role)roleType);
            string certificateText = "";
            foreach (var item in principalEntity.UserInfo.Certificates)
            {
                certificateText += item.ID + ",";
            }
            ViewBag.CertificateText = certificateText;
            ViewBag.Language = userBusiness.GetLanguages().ToSelectList(ViewTextHelper.DefaultPleaseSelectText, "0");
            ViewBag.State = userBusiness.GetStates().ToSelectList(ViewTextHelper.DefaultStateText, "");
            ViewBag.County = userBusiness.GetCountries().ToSelectList(ViewTextHelper.DefaultCountyText, "");
            ViewBag.PositionOptions = userBusiness.GetPositions((int)Role.Principal).
                ToSelectList(ViewTextHelper.DefaultPleaseSelectText, "");
            ViewBag.PDList = userBusiness.GetPDs();
            ViewBag.CertificateList = userBusiness.GetCertificates();

            ViewBag.RoleType = (Role)roleType;
            ViewBag.CurrentRoleType = UserInfo.Role;
            ViewBag.GroupPackages = permissionBusiness.GetAssignedPackages(
                (int)Role.Principal, principalEntity.UserInfo.UserCommunitySchools.Select(x => x.SchoolId).ToList(),
                principalEntity.UserInfo.UserCommunitySchools.Select(x => x.CommunityId).ToList()
                );
            ViewBag.GroupPackageSelected = principalEntity.UserInfo.PermissionRoles.Select(e =>
               new GroupPackageModel()
               {
                   PackageId = e.ID,
                   PackageName = e.Name,
                   PackageDescription = e.Descriptions
               }).ToList();
            if ((Role)roleType == Role.Principal_Delegate)
            {
                var delegateUser = userBusiness.GetUser(principalEntity.ParentId);
                ViewBag.ParentSchoolNames = delegateUser.UserCommunitySchools.Select(x => x.School.Name).ToArray();
            }
            return View(principalEntity);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.View, PageId = PagesModel.Principal_Director, Anonymity = Anonymous.Verified)]
        public ActionResult View(int id, int roleType)
        {
            InternalAccess();
            PrincipalEntity principalEntity = userBusiness.GetPrincipal(id, UserInfo, (Role)roleType);
            if (principalEntity.StateId > 0)
            {
                StateEntity state = masterDataBusiness.GetState(principalEntity.StateId);
                if (state != null)
                    ViewBag.State = state.Name;
            }
            if (principalEntity.CountyId > 0)
            {
                CountyEntity county = masterDataBusiness.GetCounty(principalEntity.CountyId);
                if (county != null)
                    ViewBag.County = county.Name;
            }
            if (principalEntity.PrimaryLanguageId > 0)
            {
                LanguageEntity language = masterDataBusiness.GetLanguage(principalEntity.PrimaryLanguageId);
                if (language != null)
                    ViewBag.Language = language.Language;
            }
            if (principalEntity.PositionId > 0)
            {
                PositionEntity position = userBusiness.GetPosition(principalEntity.PositionId);
                if (position != null)
                    ViewBag.Position = position.Title;
            }
            ViewBag.CertificateList = userBusiness.GetCertificates();
            ViewBag.PD = principalEntity.UserInfo.ProfessionalDevelopments.Select(e => e.ProfessionalDevelopment).ToList();
            ViewBag.RoleType = (Role)roleType;
            ViewBag.CurrentRoleType = UserInfo.Role;
            ViewBag.GroupPackageSelected = principalEntity.UserInfo.PermissionRoles.Where(e => e.IsDefault == false).Select(e =>
                new GroupPackageModel()
                {
                    PackageId = e.ID,
                    PackageName = e.Name,
                    PackageDescription = e.Descriptions
                }).Select(x => x.PackageName).ToList();
            if ((Role)roleType == Role.Principal_Delegate)
            {
                var delegateUser = userBusiness.GetUser(principalEntity.ParentId);
                ViewBag.ParentSchoolNames = delegateUser.UserCommunitySchools.Select(x => x.School.Name).ToArray();
            }
            return View(principalEntity);
        }

        [System.Web.Mvc.HttpPost]
        [ValidateAntiForgeryToken]
        [CLIUrlAuthorizeAttribute(Account = Authority.Edit, PageId = PagesModel.Principal_Director, Anonymity = Anonymous.Verified)]
        public string SaveInvitation(PrincipalEntity principal, int[] chkPDs,
            string certificates, bool? isInvite, int[] chkPackages, int schoolId, bool confirm = false)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            PostFormResponse response = new PostFormResponse();
            if (confirm == false && principal.UserInfo.Role == Role.Principal)
            {
                if (principal.ID == 0)
                {
                    if (CheckExisted(principal, schoolId, response))
                        return JsonHelper.SerializeObject(response);
                }
                else
                {
                    if (userBusiness.CheckUserExistedStatus(principal.UserInfo.ID,
                        principal.UserInfo.FirstName,
                        principal.UserInfo.LastName, principal.UserInfo.PrimaryEmailAddress,
                        principal.UserInfo.Role, out result))
                    {
                        response.Success = true;
                        response.Message = result.Message;
                        response.Data = result.AppendData;
                        return JsonHelper.SerializeObject(response);
                    }
                }
            }
            if (principal.UserInfo.Role == Role.Principal_Delegate)
                schoolId = 0;
            List<int> certificatesList = new List<int>();
            if (!certificates.IsNullOrEmpty())
            {
                foreach (var item in certificates.TrimEnd(',').Split(','))
                {
                    certificatesList.Add(Convert.ToInt32(item));
                }
            }
            if (principal.ID == 0)
            {
                principal.UserInfo.GoogleId = "";
                principal.SchoolYear = CommonAgent.SchoolYear;
                principal.UserInfo.StatusDate = DateTime.Now;
                if (principal.UserInfo.Role == Role.Principal)
                {
                    principal.UserInfo.Role = Role.Principal;
                    principal.PrincipalId = userBusiness.PrincipalCode();
                }
                else
                {
                    principal.UserInfo.Role = Role.Principal_Delegate;
                    principal.ParentId = UserInfo.ID;
                    principal.PrincipalId = userBusiness.SchoolDelegateCode();
                }
                principal.UserInfo.Sponsor = UserInfo.ID;
                principal.UserInfo.InvitationEmail = InvitationEmailEnum.NotSend;
                principal.UserInfo.Notes = RegisterType.Invitation.ToDescription();
            }
            if (isInvite == true)
            {
                principal.UserInfo.EmailExpireTime = DateTime.Now.AddDays(SFConfig.ExpirationTime);
                principal.UserInfo.InvitationEmail = InvitationEmailEnum.Sent;
            }
            if (response.Success = ModelState.IsValid)
            {
                if (principal.ID > 0)
                {
                    var user = userBusiness.GetUser(principal.UserInfo.ID);
                    if (user.Status == EntityStatus.Inactive && principal.UserInfo.Status == EntityStatus.Active)
                    {
                        operationLogBusiness.InsertLog(OperationEnum.ResetSendInvitation,
                            principal.UserInfo.Role.ToDescription(),
                            "Status Changed:" + "Inactive to Active" +
                            ",UserId:" + principal.UserInfo.ID,
                            CommonHelper.GetIPAddress(Request), UserInfo);
                    }
                    result = userBusiness.UpdatePrincipal(principal, chkPDs, certificatesList, chkPackages);
                }
                else
                {
                    result = userBusiness.InsertPrincipal(principal, chkPDs, certificatesList, chkPackages, schoolId,
                        UserInfo.ID);
                    operationLogBusiness.InsertLog(OperationEnum.ResetSendInvitation,
                        principal.UserInfo.Role.ToDescription(), "Created User,UserId:" + principal.UserInfo.ID,
                        CommonHelper.GetIPAddress(Request), UserInfo);
                }
                if (isInvite == true)
                {
                    EmailTemplete template = XmlHelper.GetEmailTemplete("TeacherInvitation_Template.xml");
                    string param = principal.UserInfo.ID.ToString() + "," + DateTime.Now.ToString();
                    string encryptParam = ObjectFactory.GetInstance<IEncrypt>().Encrypt(param);
                    string link = SFConfig.MainSiteDomain + "Home/InviteVerification/"
                        + System.Web.HttpContext.Current.Server.UrlEncode(encryptParam);
                    string emailBody = template.Body.Replace("{FirstName}", principal.UserInfo.FirstName)
                    .Replace("{LastName}", principal.UserInfo.LastName)
                    .Replace("{InviteLink}", "<a style='text-decoration: underline; cursor:pointer; color: #008000;' href='" + link + "'>Click here</a>")
                    .Replace("{StaticDomain}", SFConfig.StaticDomain)
                    .Replace("{CliEngageUrl}", SFConfig.MainSiteDomain);
                    userBusiness.SendEmail(principal.UserInfo.PrimaryEmailAddress, template.Subject, emailBody);
                    EmailLogEntity emailLog = new EmailLogEntity(principal.UserInfo.ID,
                        principal.UserInfo.PrimaryEmailAddress, EmailLogType.Invitation);
                    userBusiness.InsertEmailLog(emailLog);
                    operationLogBusiness.InsertLog(OperationEnum.ResetSendInvitation,
                        principal.UserInfo.Role.ToDescription(), "Send Invitation,UserId:" + principal.UserInfo.ID,
                        CommonHelper.GetIPAddress(Request), UserInfo);
                }
                response.Success = result.ResultType == OperationResultType.Success;
                response.Message = result.Message;
            }
            response.ModelState = ModelState;
            return JsonHelper.SerializeObject(response);
        }

        private bool CheckExisted(PrincipalEntity principal, int schoolId, PostFormResponse response)
        {
            var existedUserId = 0;
            Role existedUserRole;
            var existedStatus = userBusiness.CheckPrincipalUserExistedStatus(principal.UserInfo.ID, principal.UserInfo.FirstName,
                principal.UserInfo.LastName, principal.UserInfo.PrimaryEmailAddress, principal.UserInfo.Role, schoolId,
                out existedUserId, out existedUserRole);
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
                if (existedUserRole == Role.Principal)
                {
                    if (UserInfo.Role == Role.Community || UserInfo.Role == Role.District_Community_Specialist
                        || UserInfo.Role == Role.Statewide)
                    {
                        //如果当前用户所属的Community包含查询出的用户所属的Community，则直接跳转到分配页面，否则需要发送邮件请求
                        if (
                            UserInfo.UserCommunitySchools.Where(e => e.SchoolId > 0).Any(
                                n =>
                                    n.School.UserCommunitySchools.Any(
                                        o => o.UserId == existedUserId && o.AccessType == AccessType.Primary)))
                        {
                            response.Success = true;
                            response.Message = ResourceHelper.GetRM().GetInformation("UserExistedInCurrentRole")
                                .Replace("{Role}", existedUserRole.ToDescription().ToLower());
                            response.Data = new
                            {
                                type = "confirmAssign",
                                url =
                                    Url.Action("AssignSchool", "Public",
                                        new {userId = existedUserId, roleType = principal.UserInfo.Role})
                            };
                            return true;
                        }
                        else
                        {
                            response.Success = true;
                            response.Message =
                                ResourceHelper.GetRM().GetInformation("UserExistedInSystemSendEmail")
                                    .Replace("{Role}", existedUserRole.ToDescription().ToLower());
                            response.Data = new
                            {
                                type = "confirmEmail",
                                userId = existedUserId
                            };
                            return true;
                        }
                    }
                    else if (UserInfo.Role == Role.District_Community_Delegate || UserInfo.Role == Role.Community_Specialist_Delegate)
                    {
                        UserBaseEntity parentUser = userBusiness.GetUser(UserInfo.ID);
                        //如果当前Delegate用户的父级所属的Community包含查询出的用户所属的Community，则直接跳转到分配页面，否则需要发送邮件请求
                        if (
                            parentUser.UserCommunitySchools.Where(e => e.SchoolId > 0).Any(
                                n =>
                                    n.School.UserCommunitySchools.Any(
                                        o => o.UserId == existedUserId && o.AccessType == AccessType.Primary)))
                        {
                            response.Success = true;
                            response.Message = ResourceHelper.GetRM().GetInformation("UserExistedInCurrentRole")
                                .Replace("{Role}", existedUserRole.ToDescription().ToLower());
                            response.Data = new
                            {
                                type = "confirmAssign",
                                url =
                                    Url.Action("AssignSchool", "Public",
                                        new {userId = existedUserId, roleType = principal.UserInfo.Role})
                            };
                            return true;
                        }
                        else
                        {
                            response.Success = true;
                            response.Message =
                                ResourceHelper.GetRM().GetInformation("UserExistedInSystemSendEmail")
                                    .Replace("{Role}", existedUserRole.ToDescription().ToLower());
                            response.Data = new
                            {
                                type = "confirmEmail",
                                userId = existedUserId
                            };
                            return true;
                        }
                    }
                    else if (UserInfo.Role <= Role.Mentor_coach)
                    {
                        // 内部用户提醒分配 
                        response.Success = true;
                        response.Message = ResourceHelper.GetRM().GetInformation("UserExistedInCurrentRole")
                            .Replace("{Role}", existedUserRole.ToDescription().ToLower());
                        response.Data = new
                        {
                            type = "confirmAssign",
                            url =
                                Url.Action("AssignSchool", "Public",
                                    new { userId = existedUserId, roleType = principal.UserInfo.Role })
                        };
                        return true;
                    }
                    else
                    {
                        // 已存在Principal 角色的并且FirstName,LastName,Email相同的记录,但是操作者没有权限(意外勾选了不属于操作者的权限)
                        response.Success = true;
                        response.Message = ResourceHelper.GetRM()
                            .GetInformation("UserExistedInCurrentRole")
                            .Replace("{Role}", existedUserRole.ToDescription().ToLower());
                        response.Data = new
                        {
                            type = "noauthority"
                        };
                        return true;
                    }
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

        [HttpPost]
        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.Principal_Director, Anonymity = Anonymous.Verified)]
        public string SendEmail(int schoolId, string txtSchool, string email, int userId)
        {
            EmailTemplete template = XmlHelper.GetEmailTemplete("NoPermission_Invite_Template.xml");
            var user = userBusiness.GetUser(userId);
            var recipient = user.FirstName + " " + user.LastName;

            var approveLink = new LinkModel()
            {
                RoleType = Role.Principal,
                Host = SFConfig.MainSiteDomain,
                Path = "Approve/",
                Sender = UserInfo.ID,
                Recipient = userId
            };
            approveLink.Others.Add("SchoolId", schoolId);

            var denyLink = new LinkModel()
            {
                RoleType = Role.Principal,
                Host = SFConfig.MainSiteDomain,
                Path = "Deny/",
                Sender = UserInfo.ID,
                Recipient = userId
            };
            denyLink.Others.Add("SchoolId", schoolId);

            string emailBody = template.Body.Replace("{StaticDomain}", SFConfig.StaticDomain)
                .Replace("{Recipient}", recipient)
                .Replace("{Sender}", UserInfo.FirstName + " " + UserInfo.LastName)
                .Replace("{SystemObject}", txtSchool)
                .Replace("{Approve}", approveLink.ToString())
                .Replace("{Deny}", denyLink.ToString());
            userBusiness.SendEmail(email, template.Subject, emailBody);

            StatusTrackingEntity statusTrackingEntity = statusTrackingBusiness.GetExistingTracking(UserInfo.ID,
                userId, 0, schoolId);
            if (statusTrackingEntity == null)
            {
                statusTrackingEntity = new StatusTrackingEntity();
                statusTrackingEntity.RequestorId = UserInfo.ID;
                statusTrackingEntity.SupposedApproverId = userId;
                statusTrackingEntity.Status = StatusEnum.Pending;
                statusTrackingEntity.ExpiredTime = DateTime.Now.AddDays(Convert.ToInt32(SFConfig.ExpirationTime));
                statusTrackingEntity.SchoolId = schoolId;
                statusTrackingEntity.CreatedBy = UserInfo.ID;
                statusTrackingEntity.Type = StatusType.Invitation;
                statusTrackingBusiness.AddTracking(statusTrackingEntity);
            }
            else
            {
                statusTrackingBusiness.Resend(statusTrackingEntity.ID, UserInfo.ID);
            }
            return JsonHelper.SerializeObject(new PostFormResponse()
            {
                Success = true
            });
        }

        private void InternalAccess()
        {
            if (UserInfo.Role == Role.Coordinator || UserInfo.Role == Role.Mentor_coach)
            {
                ViewBag.Access = "X";
            }
        }

        private void InitAccessOperation()
        {
            bool accessView = false;
            bool accessAdd = false;
            bool accessEdit = false;
            bool accessPermission = false;
            bool accessAssignSchool = false;
            bool accessBES = false;
            if (UserInfo != null)
            {
                UserAuthorityModel userAuthority = new PermissionBusiness().GetUserAuthority(UserInfo, (int)PagesModel.Principal_Director);

                if (userAuthority != null)
                {
                    if ((userAuthority.Authority & (int)Authority.View) == (int)Authority.View)
                    {
                        accessView = true;
                    }
                    if ((userAuthority.Authority & (int)Authority.Add) == (int)Authority.Add)
                    {
                        accessAdd = true;
                    }
                    if ((userAuthority.Authority & (int)Authority.Edit) == (int)Authority.Edit)
                    {
                        accessEdit = true;
                    }
                    if ((userAuthority.Authority & (int)Authority.Assign) == (int)Authority.Assign)
                    {
                        accessPermission = true;
                    }
                    if ((userAuthority.Authority & (int)Authority.Bes) == (int)Authority.Bes)
                    {
                        accessBES = true;
                    }
                }
                if (UserInfo.IsCLIUser || UserInfo.Role == Role.Community ||
                    UserInfo.Role == Role.District_Community_Delegate ||
                    UserInfo.Role == Role.District_Community_Specialist ||
                    UserInfo.Role == Role.Community_Specialist_Delegate || UserInfo.Role == Role.Statewide)
                {
                    accessAssignSchool = true;
                }
            }
            ViewBag.accessView = accessView;
            ViewBag.accessAdd = accessAdd;
            ViewBag.accessEdit = accessEdit;
            ViewBag.accessPermission = accessPermission;
            ViewBag.accessAssignSchool = accessAssignSchool;
            ViewBag.accessBES = accessBES;
        }


        #region BES
        [CLIUrlAuthorizeAttribute(Account = Authority.Bes, PageId = PagesModel.Principal_Director, Anonymity = Anonymous.Verified)]
        public ActionResult PrincipalBES()
        {
            InitAccessOperation();
            ViewBag.StatusJson = JsonHelper.SerializeObject(EntityStatus.Active.ToList());
            ViewBag.PositionOptions = JsonHelper.SerializeObject(userBusiness.GetPositions((int)Role.Principal).ToSelectList(ViewTextHelper.DefaultPleaseSelectText, ""));
            ViewBag.Language = JsonHelper.SerializeObject(userBusiness.GetLanguages().ToSelectList(ViewTextHelper.DefaultPleaseSelectText, "0"));
            PrincipalUserModel userModel = new PrincipalUserModel();
            return View(userModel);
        }


        [CLIUrlAuthorizeAttribute(Account = Authority.Bes, PageId = PagesModel.Principal_Director, Anonymity = Anonymous.Verified)]
        public string BESSearch(string txtCommunity, string txtSchool, string principalCode, string firstName, string lastName, int communityId = 0,
            int schoolId = 0, int status = -1, string sort = "LastName", string order = "Asc", int first = 0, int count = 10)
        {
            var total = 0;
            var expression = PredicateHelper.True<PrincipalEntity>();
            if (communityId > 0)
                expression = expression.And(r => r.UserInfo.UserCommunitySchools.
                    Any(m => m.School.CommunitySchoolRelations.Any(n => n.CommunityId == communityId)));
            else if (txtCommunity.Trim() != string.Empty)
                expression = expression.And(r => r.UserInfo.UserCommunitySchools.
                    Any(m => m.School.CommunitySchoolRelations.Any(n => n.Community.Name.Contains(txtCommunity))));
            if (schoolId > 0)
                expression = expression.And(r => r.UserInfo.UserCommunitySchools.Any(e => e.SchoolId == schoolId));
            else if (txtSchool.Trim() != string.Empty)
                expression = expression.And(r => r.UserInfo.UserCommunitySchools.Any(e => e.School.Name.Contains(txtSchool)));
            if (status > 0)
                expression = expression.And(r => (int)r.UserInfo.Status == status);
            if (principalCode.Trim() != string.Empty)
                expression = expression.And(r => r.PrincipalId.Contains(principalCode));
            if (firstName.Trim() != string.Empty)
                expression = expression.And(r => r.UserInfo.FirstName.Contains(firstName));
            if (lastName.Trim() != string.Empty)
                expression = expression.And(r => r.UserInfo.LastName.Contains(lastName));
            if (sort == "SchoolName") sort = "LastName";
            var list = userBusiness.SearchPrincipals(UserInfo, Role.Principal, expression, sort, order, first, count, out total, true);
            var result = new { total = total, data = list };
            return JsonHelper.SerializeObject(result);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.Bes, PageId = PagesModel.Principal_Director, Anonymity = Anonymous.Verified)]
        public string BESSave(string principals, bool confirm = false)
        {
            var response = new PostFormResponse();
            OperationResult res = new OperationResult(OperationResultType.Success);
            List<PrincipalUserModel> listPrincipal = JsonHelper.DeserializeObject<List<PrincipalUserModel>>(principals);
            if (!confirm)
            {
                if (CheckUsersList(listPrincipal, response))
                    return JsonHelper.SerializeObject(response);
            }
            foreach (PrincipalUserModel model in listPrincipal)
            {
                PrincipalEntity item = null;
                if (model.ID <= 0)
                {
                    item = new PrincipalEntity();
                    item.UserInfo = new UserBaseEntity();
                    item.UserInfo.Role = Role.Principal;
                    item.SchoolYear = CommonAgent.SchoolYear;
                    item.PrincipalId = userBusiness.PrincipalCode();
                }
                else
                {
                    item = userBusiness.GetPrincipal(model.ID, UserInfo, Role.Principal);
                }

                int schoolId = model.SchoolId;
                item.PositionId = model.PositionId;
                item.UserInfo.FirstName = model.FirstName;
                item.UserInfo.MiddleName = model.MiddleName ?? "";
                item.UserInfo.LastName = model.LastName;
                item.UserInfo.PreviousLastName = "";
                item.BirthDate =  CommonAgent.MinDate;
                item.Gender = (Gender)0;
                item.PrimaryLanguageId = 0;
                item.UserInfo.Status = model.Status;
                item.IsSameAddress =  0;
                item.Address = "";
                item.UserInfo.PrimaryPhoneNumber = model.PrimaryPhoneNumber;
                item.UserInfo.PrimaryNumberType = (PhoneType)(0);
                item.UserInfo.SecondaryPhoneNumber = "";
                item.UserInfo.SecondaryNumberType = (PhoneType)(0); 
                item.UserInfo.FaxNumber = "";
                item.UserInfo.PrimaryEmailAddress = model.PrimaryEmail;
                item.UserInfo.SecondaryEmailAddress = "";

                if (item.ID <= 0)
                {
                    res = userBusiness.InsertPrincipal(item, new int[0], new List<int>(), new int[0], schoolId, UserInfo.ID);
                }
                else
                {
                    res = userBusiness.UpdatePrincipal(item);
                }
                if (res.ResultType == OperationResultType.Error)
                    break;
                if (model.IsInvitation)
                {
                    try
                    {
                        SendInvitation(item);
                    }
                    catch (Exception ex)
                    {
                        res.ResultType = OperationResultType.Error;
                        res.Message = ex.ToString();
                        break;
                    }
                }
            }
            response.Success = res.ResultType == OperationResultType.Success;
            response.Message = res.Message;
            return JsonHelper.SerializeObject(response);
        }

        private void SendInvitation(PrincipalEntity principal)
        {
            EmailTemplete template = XmlHelper.GetEmailTemplete("TeacherInvitation_Template.xml");
            string param = principal.UserInfo.ID.ToString() + "," + DateTime.Now.ToString();
            string encryptParam = ObjectFactory.GetInstance<IEncrypt>().Encrypt(param);
            string link = SFConfig.MainSiteDomain + "Home/InviteVerification/" + System.Web.HttpContext.Current.Server.UrlEncode(encryptParam);
            string emailBody = template.Body.Replace("{FirstName}", principal.UserInfo.FirstName)
            .Replace("{LastName}", principal.UserInfo.LastName)
            .Replace("{InviteLink}", "<a style='text-decoration: underline; cursor:pointer; color: #008000;' href='" + link + "'>Click here</a>")
            .Replace("{StaticDomain}", SFConfig.StaticDomain)
            .Replace("{CliEngageUrl}", SFConfig.MainSiteDomain);
            userBusiness.SendEmail(principal.UserInfo.PrimaryEmailAddress, template.Subject, emailBody);
            EmailLogEntity emailLog = new EmailLogEntity(principal.UserInfo.ID, principal.UserInfo.PrimaryEmailAddress, EmailLogType.Invitation);
            userBusiness.InsertEmailLog(emailLog);
            operationLogBusiness.InsertLog(OperationEnum.ResetSendInvitation, principal.UserInfo.Role.ToDescription(), "Send Invitation",
                                             CommonHelper.GetIPAddress(Request), UserInfo);
        }

        private bool CheckUsersList(List<PrincipalUserModel> listPrincipal, PostFormResponse response)
        {
            var userList = listPrincipal.Where(t => t.ID == 0).GroupBy(o => new { o.FirstName, o.LastName, o.PrimaryEmail }).ToList();
            userList = userList.Where(o => o.Count() > 1).ToList();

            PrincipalEntity principal = new PrincipalEntity();
            principal.UserInfo = new UserBaseEntity();

            if (userList.Count > 0)
            {
                principal.UserInfo.Role = Role.District_Community_Specialist;
                principal.UserInfo.FirstName = userList[0].Key.FirstName;
                principal.UserInfo.LastName = userList[0].Key.LastName;
                principal.UserInfo.PrimaryEmailAddress = userList[0].Key.PrimaryEmail;
                if (CheckExisted3(principal, UserExistedStatus.UserExisted, response))
                {
                    return true;
                }
            }

            foreach (PrincipalUserModel model in listPrincipal)
            {
                PrincipalEntity item = null;
                if (model.ID <= 0)
                {
                    item = new PrincipalEntity();
                    item.UserInfo = new UserBaseEntity();
                    item.UserInfo.Role = Role.Principal;
                }
                else
                {
                    item = userBusiness.GetPrincipal(model.ID, UserInfo, Role.Principal);
                }
                int schoolId = model.SchoolId;
                item.PositionId = model.PositionId;
                item.UserInfo.FirstName = model.FirstName;
                item.UserInfo.LastName = model.LastName;
                item.UserInfo.Status = model.Status;
                item.UserInfo.PrimaryEmailAddress = model.PrimaryEmail;

                if (CheckExisted2(item, schoolId, response))
                {
                    return true;
                }
            }
            return false;
        }

        private bool CheckExisted2(PrincipalEntity principal, int schoolId, PostFormResponse response)
        {
            var existedUserId = 0;
            Role existedUserRole;
            var existedStatus = userBusiness.CheckPrincipalUserExistedStatus(principal.UserInfo.ID, principal.UserInfo.FirstName,
                principal.UserInfo.LastName, principal.UserInfo.PrimaryEmailAddress, principal.UserInfo.Role, schoolId,
                out existedUserId, out existedUserRole);
            if (existedStatus == UserExistedStatus.ExistedInSchool)
            {
                // do nothing
                response.Success = true;
                response.Message = "More than one principal with the same first name, last name, and primary email already exists in the same school: "
                  + principal.UserInfo.FirstName + " " + principal.UserInfo.LastName + " "
                  + principal.UserInfo.PrimaryEmailAddress;

                response.Data = "waiting";
                return true;
            }
            else if (existedStatus == UserExistedStatus.UserExisted)
            {
                if (existedUserRole == Role.Principal)
                {
                    // do nothing
                    response.Success = true;
                    response.Message = "More than one principal with the same first name, last name, and primary email already exists in Cli Engage: "
                      + principal.UserInfo.FirstName + " " + principal.UserInfo.LastName + " "
                      + principal.UserInfo.PrimaryEmailAddress;

                    response.Data = "waiting";
                    return true;
                }
                else
                {
                    // 存在其他角色的用户提醒 Continue
                    response.Success = true;
                    response.Message = "More than one user with the same first name, last name, and primary email already exists in Cli Engage: "
                    + principal.UserInfo.FirstName + " " +
                    principal.UserInfo.LastName
                    + " (" + existedUserRole.ToDescription() + ") " + principal.UserInfo.PrimaryEmailAddress + ", Continue?";

                    response.Data = new
                    {
                        type = "continue"
                    };
                    return true;
                }
            }
            return false;
        }

        private bool CheckExisted3(PrincipalEntity principal, UserExistedStatus existedStatus, PostFormResponse response)
        {
            if (existedStatus == UserExistedStatus.UserExisted)
            {
                // 已存在Teacher 角色的并且FirstName,LastName,Email相同的记录,但是操作者没有权限(意外勾选了不属于操作者的权限)
                response.Success = true;
                response.Message = "More than one principal with the same first name, last name, and primary email already exists in this list: "
                    + principal.UserInfo.FirstName + " " + principal.UserInfo.LastName + " " + principal.UserInfo.PrimaryEmailAddress;
                response.Data = "waiting";
                return true;
            }
            return false;
        }

        #endregion
    }
}