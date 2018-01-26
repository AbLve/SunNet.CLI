using Sunnet.Cli.Business.Log;
using Sunnet.Cli.Business.Schools;
using Sunnet.Cli.Business.StatusTracking;
using Sunnet.Cli.Business.Users;
using Sunnet.Cli.Core.Common.Enums;
using Sunnet.Cli.Core.Log.Entities;
using Sunnet.Cli.Core.StatusTracking.Entities;
using Sunnet.Cli.Core.StatusTracking.Enums;
using Sunnet.Cli.Core.Users.Enums;
using Sunnet.Cli.MainSite.Controllers;
using Sunnet.Cli.MainSite.Models;
using Sunnet.Cli.UIBase.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Sunnet.Framework.Extensions;
using Sunnet.Cli.Core.Users.Entities;
using Sunnet.Cli.Business.Common;
using Sunnet.Framework.Core.Tool;
using Sunnet.Framework.Helpers;
using StructureMap;
using Sunnet.Framework.Encrypt;
using Sunnet.Framework;
using Sunnet.Cli.Business.Permission;
using Sunnet.Cli.Business.Permission.Models;
using Sunnet.Framework.Permission;
using Sunnet.Cli.UIBase.Filters;
using Sunnet.Cli.Business.Users.Models;
using Sunnet.Cli.Core.MasterData.Entities;
using Sunnet.Cli.Business.MasterData;
using Sunnet.Framework.Resources;

namespace Sunnet.Cli.MainSite.Areas.Invitation.Controllers
{
    public class TRSSpecialistController : BaseController
    {
        private readonly UserBusiness userBusiness;
        private readonly SchoolBusiness schoolBusiness;
        private readonly PermissionBusiness permissionBusiness;
        private readonly MasterDataBusiness masterDataBusiness;
        private readonly StatusTrackingBusiness statusTrackingBusiness;
        private readonly OperationLogBusiness operationLogBusiness;
        private IEncrypt encrypter;
        public TRSSpecialistController()
        {
            userBusiness = new UserBusiness(UnitWorkContext);
            permissionBusiness = new PermissionBusiness(UnitWorkContext);
            masterDataBusiness = new MasterDataBusiness();
            schoolBusiness = new SchoolBusiness(UnitWorkContext);
            encrypter = ObjectFactory.GetInstance<IEncrypt>();
            statusTrackingBusiness = new StatusTrackingBusiness(UnitWorkContext);
            operationLogBusiness = new OperationLogBusiness(UnitWorkContext);
        }
        //
        // GET: /Invitation/TRSSpecialist/
        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.TRS_Specialist, Anonymity = Anonymous.Verified)]
        public ActionResult Index()
        {
            InitAccessOperation();
            ViewBag.RoleType = Role.TRS_Specialist;
            UserModel userModel = new UserModel();
            return View(userModel);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.TRS_Specialist, Anonymity = Anonymous.Verified)]
        public ActionResult MyDelegate()
        {
            InitAccessOperation();
            ViewBag.RoleType = Role.TRS_Specialist_Delegate;
            return View();
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.TRS_Specialist, Anonymity = Anonymous.Verified)]
        public string Search(int roleType, string txtCommunity, string txtSchool, string principalCode, string firstName, string lastName, int communityId = 0,
            int schoolId = 0, int status = -1, string sort = "LastName", string order = "Asc", int first = 0, int count = 10)
        {
            var total = 0;

            var expression = PredicateHelper.True<PrincipalEntity>();
            if (communityId > 0)
                expression =
                    expression.And(
                        r =>
                            r.UserInfo.UserCommunitySchools.Any(
                                m => m.School.CommunitySchoolRelations.Any(n => n.CommunityId == communityId)));
            else if (txtCommunity.Trim() != string.Empty)
                expression =
                    expression.And(
                        r =>
                            r.UserInfo.UserCommunitySchools.Any(
                                m => m.School.CommunitySchoolRelations.Any(n => n.Community.Name.Contains(txtCommunity))));
            if (schoolId > 0)
                expression = expression.And(r => r.UserInfo.UserCommunitySchools.Any(x => x.SchoolId == schoolId));
            else if (txtSchool.Trim() != string.Empty)
                expression =
                    expression.And(r => r.UserInfo.UserCommunitySchools.Any(x => x.School.BasicSchool.Name.Contains(txtSchool)));
            if (status > 0)
                expression = expression.And(r => (int)r.UserInfo.Status == status);

            if (principalCode.Trim() != string.Empty)
                expression = expression.And(r => r.PrincipalId.Contains(principalCode));
            if (firstName.Trim() != string.Empty)
                expression = expression.And(r => r.UserInfo.FirstName.Contains(firstName));
            if (lastName.Trim() != string.Empty)
                expression = expression.And(r => r.UserInfo.LastName.Contains(lastName));

            var list = userBusiness.SearchPrincipals(UserInfo, (Role)roleType, expression, sort, order, first, count, out total, false);
            var result = new { total = total, data = list };
            return JsonHelper.SerializeObject(result);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.Add, PageId = PagesModel.TRS_Specialist, Anonymity = Anonymous.Verified)]
        public ActionResult New(int roleType)
        {
            InternalAccess();
            UserBaseEntity userEntity = new UserBaseEntity();
            PrincipalEntity principalEntity = new PrincipalEntity();
            principalEntity.UserInfo = userEntity;
            ViewBag.Language = userBusiness.GetLanguages().ToSelectList(ViewTextHelper.DefaultPleaseSelectText, "0");
            ViewBag.State = userBusiness.GetStates().ToSelectList(ViewTextHelper.DefaultStateText, "");
            ViewBag.County = userBusiness.GetCountries().ToSelectList(ViewTextHelper.DefaultCountyText, "");
            ViewBag.PositionOptions = userBusiness.GetPositions((int)Role.TRS_Specialist).
                ToSelectList(ViewTextHelper.DefaultPleaseSelectText, "");
            ViewBag.PDList = userBusiness.GetPDs();
            ViewBag.CertificateList = userBusiness.GetCertificates();

            ViewBag.CommunityName = "";
            ViewBag.CommunityId = 0;
            ViewBag.SchoolId = 0;
            ViewBag.SchoolName = "";

            ViewBag.RoleType = (Role)roleType;
            principalEntity.UserInfo.Role = (Role)roleType;
            if ((Role)roleType == Role.TRS_Specialist_Delegate)
                ViewBag.ParentSchoolNames = UserInfo.UserCommunitySchools.Select(x => x.School.Name).ToArray();
            return View(principalEntity);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.Edit, PageId = PagesModel.TRS_Specialist, Anonymity = Anonymous.Verified)]
        public ActionResult Edit(int id, int roleType)
        {
            InternalAccess();
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
            ViewBag.PositionOptions = userBusiness.GetPositions((int)Role.TRS_Specialist).
                ToSelectList(ViewTextHelper.DefaultPleaseSelectText, "");
            ViewBag.PDList = userBusiness.GetPDs();
            ViewBag.CertificateList = userBusiness.GetCertificates();

            ViewBag.RoleType = (Role)roleType;
            ViewBag.CurrentRoleType = UserInfo.Role;

            ViewBag.GroupPackages = permissionBusiness.GetAssignedPackages(
                (int)Role.TRS_Specialist, principalEntity.UserInfo.UserCommunitySchools.Select(x => x.SchoolId).ToList(),
                principalEntity.UserInfo.UserCommunitySchools.Select(x => x.CommunityId).ToList());
            ViewBag.GroupPackageSelected = principalEntity.UserInfo.PermissionRoles.Select(e =>
               new GroupPackageModel()
               {
                   PackageId = e.ID,
                   PackageName = e.Name,
                   PackageDescription = e.Descriptions
               }).ToList();
            if ((Role)roleType == Role.TRS_Specialist_Delegate)
            {
                var delegateUser = userBusiness.GetUser(principalEntity.ParentId);
                ViewBag.ParentSchoolNames = delegateUser.UserCommunitySchools.Select(x => x.School.Name).ToArray();
            }
            return View(principalEntity);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.View, PageId = PagesModel.TRS_Specialist, Anonymity = Anonymous.Verified)]
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
                    PackageName = e.Name
                }).Select(x => x.PackageName).ToList();
            if ((Role)roleType == Role.TRS_Specialist_Delegate)
            {
                var delegateUser = userBusiness.GetUser(principalEntity.ParentId);
                ViewBag.ParentSchoolNames = delegateUser.UserCommunitySchools.Select(x => x.School.Name).ToArray();
            }
            return View(principalEntity);
        }

        /// <summary>
        /// Saves the invitation.
        /// </summary>
        /// <param name="principal">The principal.</param>
        /// <param name="chkPDs">The CHK p ds.</param>
        /// <param name="certificates">The certificates.</param>
        /// <param name="isInvite">The is invite.</param>
        /// <param name="chkPackages">The CHK packages.</param>
        /// <param name="SchoolId">The school identifier.</param>
        /// <param name="CommunityId">The community identifier.</param>
        /// <param name="txtSchool">The text school.</param>
        /// <param name="confirm">True: 提醒Continue, 用户已确认.</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CLIUrlAuthorizeAttribute(Account = Authority.Edit, PageId = PagesModel.TRS_Specialist, Anonymity = Anonymous.Verified)]
        public string SaveInvitation(PrincipalEntity principal, int[] chkPDs, string certificates,
            bool? isInvite, int[] chkPackages, int schoolId, bool confirm = false)
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
            if (principal.UserInfo.Role == Role.TRS_Specialist_Delegate)
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
                if (principal.UserInfo.Role == Role.TRS_Specialist)
                {
                    principal.UserInfo.Role = Role.TRS_Specialist;
                    principal.PrincipalId = userBusiness.SchoolSpecialistCode();
                }
                else
                {
                    principal.UserInfo.Role = Role.TRS_Specialist_Delegate;
                    principal.ParentId = UserInfo.ID;
                    principal.PrincipalId = userBusiness.SchoolSpecialistDelegateCode();
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
                if (existedUserRole == Role.TRS_Specialist)
                {
                    if (UserInfo.Role == Role.Community || UserInfo.Role == Role.District_Community_Specialist
                        || UserInfo.Role == Role.Statewide)
                    {
                        //如果当前用户所属的Community包含查询出的用户所属的Community，则直接跳转到分配页面，否则需要发送邮件请求
                        if (
                            UserInfo.UserCommunitySchools.Any(
                                m =>
                                    m.Community.CommunitySchoolRelations.Any(
                                        n => n.School.UserCommunitySchools.Any(o => o.UserId == existedUserId))))
                        {
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
                            parentUser.UserCommunitySchools.Any(
                                m =>
                                    m.Community.CommunitySchoolRelations.Any(
                                        n => n.School.UserCommunitySchools.Any(o => o.UserId == existedUserId))))
                        {
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
                    else if (UserInfo.Role == Role.Principal)
                    {
                        //如果当前用户所属的School包含查询出的用户所属的School，则直接跳转到分配页面，否则需要发送邮件请求
                        if (UserInfo.UserCommunitySchools.Any(m => m.School.UserCommunitySchools.Any(n => n.UserId == existedUserId)))
                        {
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
                    else if (UserInfo.Role == Role.Principal_Delegate)
                    {
                        UserBaseEntity parentUser = userBusiness.GetUser(UserInfo.ID);
                        //如果当前Delegate用户的父级所属的School包含查询出的用户所属的School，则直接跳转到分配页面，否则需要发送邮件请求
                        if (parentUser.UserCommunitySchools.Any(m => m.School.UserCommunitySchools.Any(n => n.UserId == existedUserId)))
                        {
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
                        // 已存在TRS Specialist 角色的并且FirstName,LastName,Email相同的记录,但是操作者没有权限(意外勾选了不属于操作者的权限)
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
        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.TRS_Specialist, Anonymity = Anonymous.Verified)]
        public string SendEmail(int schoolId, string txtSchool, string email, int userId)
        {
            EmailTemplete template = XmlHelper.GetEmailTemplete("NoPermission_Invite_Template.xml");
            var user = userBusiness.GetUser(userId);
            var recipient = user.FirstName + " " + user.LastName;

            var approveLink = new LinkModel()
            {
                RoleType = Role.TRS_Specialist,
                Host = SFConfig.MainSiteDomain,
                Path = "Approve/",
                Sender = UserInfo.ID,
                Recipient = userId
            };
            approveLink.Others.Add("SchoolId", schoolId);

            var denyLink = new LinkModel()
            {
                RoleType = Role.TRS_Specialist,
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

        public void InternalAccess()
        {
            switch (UserInfo.Role)
            {
                case Role.District_Community_Specialist:
                case Role.Community_Specialist_Delegate:
                case Role.TRS_Specialist:
                case Role.TRS_Specialist_Delegate:
                case Role.School_Specialist:
                case Role.School_Specialist_Delegate:
                    ViewBag.TRSSpecialistNote = "X";
                    break;
                case Role.Community:
                case Role.District_Community_Delegate:
                    ViewBag.TRSSpecialistNote = "X";
                    ViewBag.Readonly = "R";
                    break;
                default:
                    break;
            }
        }

        private void InitAccessOperation()
        {
            bool accessView = false;
            bool accessAdd = false;
            bool accessEdit = false;
            bool accessPermission = false;
            bool accessAssignSchool = false;
            if (UserInfo != null)
            {
                UserAuthorityModel userAuthority = new PermissionBusiness().GetUserAuthority(UserInfo, (int)PagesModel.TRS_Specialist);

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
                }
                if (UserInfo.IsCLIUser || UserInfo.Role == Role.Community ||
                    UserInfo.Role == Role.District_Community_Delegate ||
                    UserInfo.Role == Role.District_Community_Specialist ||
                    UserInfo.Role == Role.Community_Specialist_Delegate
                    || UserInfo.Role == Role.Statewide
                    || UserInfo.Role == Role.Principal || UserInfo.Role == Role.Principal_Delegate)
                {
                    accessAssignSchool = true;
                }
            }
            ViewBag.accessView = accessView;
            ViewBag.accessAdd = accessAdd;
            ViewBag.accessEdit = accessEdit;
            ViewBag.accessPermission = accessPermission;
            ViewBag.accessAssignSchool = accessAssignSchool;
        }
    }
}