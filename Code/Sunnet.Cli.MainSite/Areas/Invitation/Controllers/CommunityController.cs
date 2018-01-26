using Sunnet.Cli.Business.Log;
using Sunnet.Cli.Business.StatusTracking;
using Sunnet.Cli.Business.Users;
using Sunnet.Cli.Core.Log.Entities;
using Sunnet.Cli.Core.StatusTracking.Entities;
using Sunnet.Cli.Core.StatusTracking.Enums;
using Sunnet.Cli.Core.Users.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Sunnet.Cli.MainSite.Models;
using Sunnet.Framework.Extensions;
using Sunnet.Cli.UIBase.Models;
using Sunnet.Cli.Business.Common;
using Sunnet.Cli.Core.Common.Enums;
using Sunnet.Framework.Core.Tool;
using Sunnet.Cli.MainSite.Controllers;
using Sunnet.Cli.Core.Users.Enums;
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
    public class CommunityController : BaseController
    {
        private readonly UserBusiness userBusiness;
        private readonly PermissionBusiness permissionBusiness;
        private readonly MasterDataBusiness masterDataBusiness;
        private readonly StatusTrackingBusiness statusTrackingBusiness;
        private readonly OperationLogBusiness operationLogBusiness;

        public CommunityController()
        {
            userBusiness = new UserBusiness(UnitWorkContext);
            permissionBusiness = new PermissionBusiness(UnitWorkContext);
            masterDataBusiness = new MasterDataBusiness(UnitWorkContext);
            statusTrackingBusiness = new StatusTrackingBusiness(UnitWorkContext);
            operationLogBusiness = new OperationLogBusiness(UnitWorkContext);
        }
        //
        // GET: /Invitation/Community/
        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.Community, Anonymity = Anonymous.Verified)]
        public ActionResult Index()
        {
            InitAccessOperation();
            ViewBag.RoleType = Role.Community;
            UserModel userModel = new UserModel();
            return View(userModel);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.Community, Anonymity = Anonymous.Verified)]
        public ActionResult MyDelegate()
        {
            InitAccessOperation();
            ViewBag.RoleType = Role.District_Community_Delegate;
            return View();
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.Community, Anonymity = Anonymous.Verified)]
        public string Search(int roleType, string txtCommunity, string communityCode, string firstName, string lastName, int communityId = 0,
            int status = -1, string sort = "LastName", string order = "Asc", int first = 0, int count = 10)
        {
            var total = 0;

            var expression = PredicateHelper.True<CommunityUserEntity>();
            if (communityId > 0)
                expression = expression.And(r => r.UserInfo.UserCommunitySchools.Any(e => e.CommunityId == communityId));
            if (status > 0)
                expression = expression.And(r => (int)r.UserInfo.Status == status);
            if (txtCommunity.Trim() != string.Empty)
                expression = expression.And(r => r.UserInfo.UserCommunitySchools.Any(e => e.Community.BasicCommunity.Name.Contains(txtCommunity)));
            if (communityCode.Trim() != string.Empty)
                expression = expression.And(r => r.CommunityUserId.Contains(communityCode));
            if (firstName.Trim() != string.Empty)
                expression = expression.And(r => r.UserInfo.FirstName.Contains(firstName));
            if (lastName.Trim() != string.Empty)
                expression = expression.And(r => r.UserInfo.LastName.Contains(lastName));

            var list = userBusiness.SearchCommunityUsers(UserInfo, (Role)roleType, expression, sort, order, first, count, out total, true);
            var result = new { total = total, data = list };
            return JsonHelper.SerializeObject(result);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.Add, PageId = PagesModel.Community, Anonymity = Anonymous.Verified)]
        public ActionResult New(int roleType)
        {
            UserBaseEntity userEntity = new UserBaseEntity();
            CommunityUserEntity communityUserEntity = new CommunityUserEntity();
            communityUserEntity.UserInfo = userEntity;
            ViewBag.Language = userBusiness.GetLanguages().ToSelectList(ViewTextHelper.DefaultPleaseSelectText, "0");
            ViewBag.State = userBusiness.GetStates().ToSelectList(ViewTextHelper.DefaultStateText, "");
            ViewBag.County = userBusiness.GetCountries().ToSelectList(ViewTextHelper.DefaultCountyText, "");
            ViewBag.PositionOptions = userBusiness.GetPositions((int)Role.Community).ToSelectList(ViewTextHelper.DefaultPleaseSelectText, "");
            ViewBag.PDList = userBusiness.GetPDs();
            ViewBag.CertificateList = userBusiness.GetCertificates();

            ViewBag.RoleType = (Role)roleType;
            communityUserEntity.UserInfo.Role = (Role)roleType;
            if ((Role) roleType == Role.District_Community_Delegate)
                ViewBag.ParentCommunityNames =
                    UserInfo.UserCommunitySchools.Where(e => e.CommunityId > 0).Select(x => x.Community.Name).ToArray();
            return View(communityUserEntity);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.Edit, PageId = PagesModel.Community, Anonymity = Anonymous.Verified)]
        public ActionResult Edit(int id, int roleType)
        {
            CommunityUserEntity communityUserEntity = userBusiness.GetCommunity(id, UserInfo, (Role)roleType, true);
            string certificateText = "";
            foreach (var item in communityUserEntity.UserInfo.Certificates)
            {
                certificateText += item.ID + ",";
            }
            ViewBag.CertificateText = certificateText;
            ViewBag.Language = userBusiness.GetLanguages().ToSelectList(ViewTextHelper.DefaultPleaseSelectText, "0");
            ViewBag.State = userBusiness.GetStates().ToSelectList(ViewTextHelper.DefaultStateText, "");
            ViewBag.County = userBusiness.GetCountries().ToSelectList(ViewTextHelper.DefaultCountyText, "");
            ViewBag.PositionOptions = userBusiness.GetPositions((int)Role.Community).ToSelectList(ViewTextHelper.DefaultPleaseSelectText, "");
            ViewBag.CertificateList = userBusiness.GetCertificates();

            ViewBag.RoleType = (Role)roleType;
            ViewBag.CurrentRoleType = UserInfo.Role;

            ViewBag.GroupPackages = permissionBusiness.GetAssignedPackages(
                (int)Role.Community, null,
                communityUserEntity.UserInfo.UserCommunitySchools.Select(e => e.CommunityId).ToList());
            ViewBag.GroupPackageSelected = communityUserEntity.UserInfo.PermissionRoles.Select(e =>
               new GroupPackageModel()
               {
                   PackageId = e.ID,
                   PackageName = e.Name,
                   PackageDescription = e.Descriptions
               }).ToList();
            if ((Role) roleType == Role.District_Community_Delegate)
            {
                var delegateUser = userBusiness.GetUser(communityUserEntity.ParentId);
                ViewBag.ParentCommunityNames =
                    delegateUser.UserCommunitySchools.Where(e => e.CommunityId > 0).Select(x => x.Community.Name).ToArray();
            }
            return View(communityUserEntity);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.View, PageId = PagesModel.Community, Anonymity = Anonymous.Verified)]
        public ActionResult View(int id, int roleType)
        {
            CommunityUserEntity communityUserEntity = userBusiness.GetCommunity(id, UserInfo, (Role)roleType, true);
            if (communityUserEntity.StateId > 0)
            {
                StateEntity state = masterDataBusiness.GetState(communityUserEntity.StateId);
                if (state != null)
                    ViewBag.State = state.Name;
            }
            if (communityUserEntity.CountyId > 0)
            {
                CountyEntity county = masterDataBusiness.GetCounty(communityUserEntity.CountyId);
                if (county != null)
                    ViewBag.County = county.Name;
            }
            if (communityUserEntity.PrimaryLanguageId > 0)
            {
                LanguageEntity language = masterDataBusiness.GetLanguage(communityUserEntity.PrimaryLanguageId);
                if (language != null)
                    ViewBag.Language = language.Language;
            }
            if (communityUserEntity.PositionId > 0)
            {
                PositionEntity position = userBusiness.GetPosition(communityUserEntity.PositionId);
                if (position != null)
                    ViewBag.Position = position.Title;
            }
            ViewBag.CertificateList = userBusiness.GetCertificates();
            ViewBag.RoleType = (Role)roleType;
            ViewBag.CurrentRoleType = UserInfo.Role;
            ViewBag.GroupPackageSelected = communityUserEntity.UserInfo.PermissionRoles.Where(e => e.IsDefault == false).Select(e =>
                new GroupPackageModel()
                {
                    PackageId = e.ID,
                    PackageName = e.Name,
                    PackageDescription = e.Descriptions
                }).Select(x => x.PackageName).ToList();
            if ((Role)roleType == Role.District_Community_Delegate)
            {
                var delegateUser = userBusiness.GetUser(communityUserEntity.ParentId);
                ViewBag.ParentCommunityNames =
                    delegateUser.UserCommunitySchools.Where(e => e.CommunityId > 0).Select(x => x.Community.Name).ToArray();
            }
            return View(communityUserEntity);
        }

        [System.Web.Mvc.HttpPost]
        [ValidateAntiForgeryToken]
        [CLIUrlAuthorizeAttribute(Account = Authority.Edit, PageId = PagesModel.Community, Anonymity = Anonymous.Verified)]
        public string SaveInvitation(CommunityUserEntity communityUser, int communityId, string certificates,
            bool? isInvite,
            int[] chkPackages, bool confirm = false)
        {
            PostFormResponse response = new PostFormResponse();
            OperationResult result = new OperationResult(OperationResultType.Success);
            if (confirm == false && communityUser.UserInfo.Role == Role.Community)
            {
                if (communityUser.ID == 0)
                {
                    if (CheckExisted(communityUser, communityId, response))
                        return JsonHelper.SerializeObject(response);
                }
                else
                {
                    if (userBusiness.CheckUserExistedStatus(communityUser.UserInfo.ID,
                        communityUser.UserInfo.FirstName,
                        communityUser.UserInfo.LastName, communityUser.UserInfo.PrimaryEmailAddress,
                        communityUser.UserInfo.Role, out result))
                    {
                        response.Success = true;
                        response.Message = result.Message;
                        response.Data = result.AppendData;
                        return JsonHelper.SerializeObject(response);
                    }
                }
            }
            if (communityUser.UserInfo.Role == Role.District_Community_Delegate)
                communityId = 0;
            List<int> certificatesList = new List<int>();
            if (!certificates.IsNullOrEmpty())
            {
                foreach (var item in certificates.TrimEnd(',').Split(','))
                {
                    certificatesList.Add(Convert.ToInt32(item));
                }
            }
            if (communityUser.ID == 0)
            {
                communityUser.UserInfo.GoogleId = "";
                communityUser.SchoolYear = CommonAgent.SchoolYear;
                communityUser.UserInfo.StatusDate = DateTime.Now;
                if (communityUser.UserInfo.Role == Role.Community)
                {
                    communityUser.UserInfo.Role = Role.Community;
                    communityUser.CommunityUserId = userBusiness.CommunityUserCode();
                }
                else
                {
                    communityUser.UserInfo.Role = Role.District_Community_Delegate;
                    communityUser.ParentId = UserInfo.ID;
                    communityUser.CommunityUserId = userBusiness.CommunityDelegateCode();
                }
                communityUser.UserInfo.Sponsor = UserInfo.ID;
                communityUser.UserInfo.InvitationEmail = InvitationEmailEnum.NotSend;
                communityUser.UserInfo.Notes = RegisterType.Invitation.ToDescription();
            }
            if (isInvite == true)
            {
                communityUser.UserInfo.EmailExpireTime = DateTime.Now.AddDays(SFConfig.ExpirationTime);
                communityUser.UserInfo.InvitationEmail = InvitationEmailEnum.Sent;
            }

            if (response.Success = ModelState.IsValid)
            {
                if (communityUser.ID > 0)
                {
                    var user = userBusiness.GetUser(communityUser.UserInfo.ID);
                    if (user.Status == EntityStatus.Inactive && communityUser.UserInfo.Status == EntityStatus.Active)
                    {
                        operationLogBusiness.InsertLog(OperationEnum.ResetSendInvitation,
                            communityUser.UserInfo.Role.ToDescription(),
                            "Status Changed:" + "Inactive to Active" +
                            ",UserId:" + communityUser.UserInfo.ID,
                            CommonHelper.GetIPAddress(Request), UserInfo);
                    }
                    result = userBusiness.UpdateCommunityUser(communityUser, certificatesList, chkPackages);
                }
                else
                {
                    result = userBusiness.InsertCommunityUser(communityUser, communityId, certificatesList, chkPackages,
                        UserInfo.ID);
                    operationLogBusiness.InsertLog(OperationEnum.ResetSendInvitation,
                        communityUser.UserInfo.Role.ToDescription(), "Created User,UserId:" + communityUser.UserInfo.ID,
                        CommonHelper.GetIPAddress(Request), UserInfo);
                }
                if (isInvite == true)
                {
                    EmailTemplete template = XmlHelper.GetEmailTemplete("TeacherInvitation_Template.xml");
                    string param = communityUser.UserInfo.ID.ToString() + "," + DateTime.Now.ToString();
                    string encryptParam = ObjectFactory.GetInstance<IEncrypt>().Encrypt(param);
                    string link = SFConfig.MainSiteDomain + "Home/InviteVerification/"
                                  + System.Web.HttpContext.Current.Server.UrlEncode(encryptParam);
                    string emailBody = template.Body.Replace("{FirstName}", communityUser.UserInfo.FirstName)
                        .Replace("{LastName}", communityUser.UserInfo.LastName)
                        .Replace("{InviteLink}",
                            "<a style='text-decoration: underline; cursor:pointer; color: #008000;' href='" + link +
                            "'>Click here</a>")
                        .Replace("{StaticDomain}", SFConfig.StaticDomain)
                        .Replace("{CliEngageUrl}", SFConfig.MainSiteDomain);
                    userBusiness.SendEmail(communityUser.UserInfo.PrimaryEmailAddress, template.Subject, emailBody);
                    EmailLogEntity emailLog = new EmailLogEntity(communityUser.UserInfo.ID,
                        communityUser.UserInfo.PrimaryEmailAddress, EmailLogType.Invitation);
                    userBusiness.InsertEmailLog(emailLog);
                    operationLogBusiness.InsertLog(OperationEnum.ResetSendInvitation,
                        communityUser.UserInfo.Role.ToDescription(),
                        "Send Invitation,UserId:" + communityUser.UserInfo.ID,
                        CommonHelper.GetIPAddress(Request), UserInfo);
                }
                response.Success = result.ResultType == OperationResultType.Success;
                response.Message = result.Message;
            }
            response.ModelState = ModelState;
            return JsonHelper.SerializeObject(response);
        }

        private bool CheckExisted(CommunityUserEntity communityUser, int communityId, PostFormResponse response)
        {
            var existedUserId = 0;
            Role existedUserRole;
            var existedStatus = userBusiness.CheckCommunityUserExistedStatus(communityUser.UserInfo.ID, communityUser.UserInfo.FirstName,
                communityUser.UserInfo.LastName, communityUser.UserInfo.PrimaryEmailAddress, communityUser.UserInfo.Role,
                communityId, out existedUserId, out existedUserRole);
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
                if (existedUserRole == Role.Community)
                {
                    if (UserInfo.Role == Role.Community || UserInfo.Role == Role.Statewide)
                    {
                        //如果当前用户所属的Community包含查询出的用户所属的Community，则直接跳转到分配页面，否则需要发送邮件请求
                        if (
                            UserInfo.UserCommunitySchools.Where(e => e.CommunityId > 0).Any(
                                x => x.Community.UserCommunitySchools.Any(y => y.UserId == existedUserId)))
                        {
                            response.Success = true;
                            response.Message = ResourceHelper.GetRM().GetInformation("UserExistedInCurrentRole")
                                .Replace("{Role}", existedUserRole.ToDescription().ToLower());
                            response.Data = new
                            {
                                type = "confirmAssign",
                                url =
                                    Url.Action("AssignCommunity", "Public",
                                        new { userId = existedUserId, roleType = communityUser.UserInfo.Role })
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
                                Url.Action("AssignCommunity", "Public",
                                    new { userId = existedUserId, roleType = communityUser.UserInfo.Role })
                        };
                        return true;
                    }
                    else
                    {
                        // 已存在Community 角色的并且FirstName,LastName,Email相同的记录,但是操作者没有权限(意外勾选了不属于操作者的权限)
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
        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.Community, Anonymity = Anonymous.Verified)]
        public string SendEmail(int communityId, string txtCommunity, string email, int userId)
        {
            EmailTemplete template = XmlHelper.GetEmailTemplete("NoPermission_Invite_Template.xml");
            var user = userBusiness.GetUser(userId);
            var recipient = user.FirstName + " " + user.LastName;

            var approveLink = new LinkModel()
            {
                RoleType = Role.Community,
                Host = SFConfig.MainSiteDomain,
                Path = "Approve/",
                Sender = UserInfo.ID,
                Recipient = userId
            };
            approveLink.Others.Add("CommunityId", communityId);

            var denyLink = new LinkModel()
            {
                RoleType = Role.Community,
                Host = SFConfig.MainSiteDomain,
                Path = "Deny/",
                Sender = UserInfo.ID,
                Recipient = userId
            };
            denyLink.Others.Add("CommunityId", communityId);

            string emailBody = template.Body.Replace("{StaticDomain}", SFConfig.StaticDomain)
                .Replace("{Recipient}", recipient)
                .Replace("{Sender}", UserInfo.FirstName + " " + UserInfo.LastName)
                .Replace("{SystemObject}", txtCommunity)
                .Replace("{Approve}", approveLink.ToString())
                .Replace("{Deny}", denyLink.ToString());
            userBusiness.SendEmail(email, template.Subject, emailBody);

            StatusTrackingEntity statusTrackingEntity = statusTrackingBusiness.GetExistingTracking(UserInfo.ID,
                userId, communityId, 0);
            if (statusTrackingEntity == null)
            {
                statusTrackingEntity = new StatusTrackingEntity();
                statusTrackingEntity.RequestorId = UserInfo.ID;
                statusTrackingEntity.SupposedApproverId = userId;
                statusTrackingEntity.Status = StatusEnum.Pending;
                statusTrackingEntity.ExpiredTime = DateTime.Now.AddDays(Convert.ToInt32(SFConfig.ExpirationTime));
                statusTrackingEntity.CommunityId = communityId;
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

        private void InitAccessOperation()
        {
            bool accessView = false;
            bool accessAdd = false;
            bool accessEdit = false;
            bool accessPermission = false;
            bool accessAssignCommunity = false;
            if (UserInfo != null)
            {
                UserAuthorityModel userAuthority = new PermissionBusiness().GetUserAuthority(UserInfo, (int)PagesModel.Community);

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
                if (UserInfo.IsCLIUser || UserInfo.Role == Role.Community || UserInfo.Role == Role.Statewide)
                {
                    accessAssignCommunity = true;
                }
            }
            ViewBag.accessView = accessView;
            ViewBag.accessAdd = accessAdd;
            ViewBag.accessEdit = accessEdit;
            ViewBag.accessPermission = accessPermission;
            ViewBag.accessAssignCommunity = accessAssignCommunity;
        }
    }
}