using System.CodeDom;
using Sunnet.Cli.Business.Common;
using Sunnet.Cli.Business.Communities;
using Sunnet.Cli.Business.Log;
using Sunnet.Cli.Business.StatusTracking;
using Sunnet.Cli.Business.Users;
using Sunnet.Cli.Core.Communities.Entities;
using Sunnet.Cli.Core.Log.Entities;
using Sunnet.Cli.Core.StatusTracking.Entities;
using Sunnet.Cli.Core.StatusTracking.Enums;
using Sunnet.Cli.Core.Users.Entities;
using Sunnet.Cli.Core.Users.Enums;
using Sunnet.Cli.MainSite.Controllers;
using Sunnet.Cli.MainSite.Models;
using Sunnet.Cli.UIBase.Models;
using Sunnet.Framework.Core.Tool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sunnet.Framework.Extensions;
using Sunnet.Framework.Helpers;
using StructureMap;
using Sunnet.Framework.Encrypt;
using Sunnet.Framework;
using Sunnet.Cli.Business.Permission;
using Sunnet.Cli.Business.Permission.Models;
using Sunnet.Cli.Core.Permission.Entities;
using Sunnet.Framework.Permission;
using Sunnet.Cli.UIBase.Filters;
using Sunnet.Cli.Business.Users.Models;
using Sunnet.Cli.Core.MasterData.Entities;
using Sunnet.Cli.Business.MasterData;
using Sunnet.Framework.Resources;
using Sunnet.Cli.Core.Common.Enums;

namespace Sunnet.Cli.MainSite.Areas.Invitation.Controllers
{
    public class CommunitySpecialistController : BaseController
    {
        private readonly UserBusiness userBusiness;
        private readonly PermissionBusiness permissionBusiness;
        private readonly MasterDataBusiness masterDataBusiness;
        private readonly StatusTrackingBusiness statusTrackingBusiness;
        private readonly OperationLogBusiness operationLogBusiness;
        private readonly CommunityBusiness communityBusiness;
        public CommunitySpecialistController()
        {
            userBusiness = new UserBusiness(UnitWorkContext);
            permissionBusiness = new PermissionBusiness(UnitWorkContext);
            masterDataBusiness = new MasterDataBusiness(UnitWorkContext);
            statusTrackingBusiness = new StatusTrackingBusiness(UnitWorkContext);
            operationLogBusiness = new OperationLogBusiness(UnitWorkContext);
            communityBusiness = new CommunityBusiness(UnitWorkContext);
        }
        //
        // GET: /Invitation/CommunitySpecialist/
        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.Community_Specialist_List, Anonymity = Anonymous.Verified)]
        public ActionResult Index()
        {
            InitAccessOperation();
            ViewBag.RoleType = Role.District_Community_Specialist;
            UserModel userModel = new UserModel();
            return View(userModel);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.Community_Specialist_List, Anonymity = Anonymous.Verified)]
        public ActionResult MyDelegate()
        {
            InitAccessOperation();
            ViewBag.RoleType = Role.Community_Specialist_Delegate;
            return View();
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.Community_Specialist_List, Anonymity = Anonymous.Verified)]
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

            var list = userBusiness.SearchCommunityUsers(UserInfo, (Role)roleType, expression, sort, order, first, count, out total, false);
            var result = new { total = total, data = list };
            return JsonHelper.SerializeObject(result);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.Add, PageId = PagesModel.Community_Specialist_List, Anonymity = Anonymous.Verified)]
        public ActionResult New(int roleType)
        {
            InternalAccess();
            UserBaseEntity userEntity = new UserBaseEntity();
            CommunityUserEntity communityUserEntity = new CommunityUserEntity();
            communityUserEntity.UserInfo = userEntity;
            ViewBag.Language = userBusiness.GetLanguages().ToSelectList(ViewTextHelper.DefaultPleaseSelectText, "0");
            ViewBag.State = userBusiness.GetStates().ToSelectList(ViewTextHelper.DefaultStateText, "");
            ViewBag.County = userBusiness.GetCountries().ToSelectList(ViewTextHelper.DefaultCountyText, "");
            ViewBag.PositionOptions = userBusiness.GetPositions((int)Role.District_Community_Specialist).ToSelectList(ViewTextHelper.DefaultPleaseSelectText, "");
            ViewBag.PDList = userBusiness.GetPDs();
            ViewBag.CertificateList = userBusiness.GetCertificates();

            ViewBag.RoleType = (Role)roleType;
            communityUserEntity.UserInfo.Role = (Role)roleType;
            if ((Role)roleType == Role.Community_Specialist_Delegate)
                ViewBag.ParentCommunityNames =
                    UserInfo.UserCommunitySchools.Where(e => e.CommunityId > 0).Select(x => x.Community.Name).ToArray();
            return View(communityUserEntity);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.Edit, PageId = PagesModel.Community_Specialist_List, Anonymity = Anonymous.Verified)]
        public ActionResult Edit(int id, int roleType)
        {
            InternalAccess();
            CommunityUserEntity communityUserEntity = userBusiness.GetCommunity(id, UserInfo, (Role)roleType, false);
            string certificateText = "";
            foreach (var item in communityUserEntity.UserInfo.Certificates)
            {
                certificateText += item.ID + ",";
            }
            ViewBag.CertificateText = certificateText;
            ViewBag.Language = userBusiness.GetLanguages().ToSelectList(ViewTextHelper.DefaultPleaseSelectText, "0");
            ViewBag.State = userBusiness.GetStates().ToSelectList(ViewTextHelper.DefaultStateText, "");
            ViewBag.County = userBusiness.GetCountries().ToSelectList(ViewTextHelper.DefaultCountyText, "");
            ViewBag.PositionOptions = userBusiness.GetPositions((int)Role.District_Community_Specialist).ToSelectList(ViewTextHelper.DefaultPleaseSelectText, "");
            ViewBag.CertificateList = userBusiness.GetCertificates();

            ViewBag.GroupPackages = permissionBusiness.GetAssignedPackages(
                (int)Role.District_Community_Specialist, null,
                communityUserEntity.UserInfo.UserCommunitySchools.Select(e => e.CommunityId).ToList());
            ViewBag.GroupPackageSelected = communityUserEntity.UserInfo.PermissionRoles.Select(e =>
               new GroupPackageModel()
               {
                   PackageId = e.ID,
                   PackageName = e.Name,
                   PackageDescription = e.Descriptions
               }).ToList();
            ViewBag.RoleType = (Role)roleType;
            ViewBag.CurrentRoleType = UserInfo.Role;
            if ((Role)roleType == Role.Community_Specialist_Delegate)
            {
                var delegateUser = userBusiness.GetUser(communityUserEntity.ParentId);
                ViewBag.ParentCommunityNames =
                    delegateUser.UserCommunitySchools.Where(e => e.CommunityId > 0).Select(x => x.Community.Name).ToArray();
            }
            return View(communityUserEntity);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.View, PageId = PagesModel.Community_Specialist_List, Anonymity = Anonymous.Verified)]
        public ActionResult View(int id, int roleType)
        {
            InternalAccess();
            CommunityUserEntity communityUserEntity = userBusiness.GetCommunity(id, UserInfo, (Role)roleType, false);
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
            if ((Role)roleType == Role.Community_Specialist_Delegate)
            {
                var delegateUser = userBusiness.GetUser(communityUserEntity.ParentId);
                ViewBag.ParentCommunityNames =
                    delegateUser.UserCommunitySchools.Where(e => e.CommunityId > 0).Select(x => x.Community.Name).ToArray();
            }
            return View(communityUserEntity);
        }

        [System.Web.Mvc.HttpPost]
        [ValidateAntiForgeryToken]
        [CLIUrlAuthorizeAttribute(Account = Authority.Edit, PageId = PagesModel.Community_Specialist_List, Anonymity = Anonymous.Verified)]
        public string SaveInvitation(CommunityUserEntity communityUser, int communityId, string certificates,
            bool? isInvite, int[] chkPackages, bool confirm = false)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            PostFormResponse response = new PostFormResponse();
            if (confirm == false && communityUser.UserInfo.Role == Role.District_Community_Specialist)
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
            if (communityUser.UserInfo.Role == Role.Community_Specialist_Delegate)
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
                if (communityUser.UserInfo.Role == Role.District_Community_Specialist)
                {
                    communityUser.UserInfo.Role = Role.District_Community_Specialist;
                    communityUser.CommunityUserId = userBusiness.CommunitySpecialistCode();
                }
                else
                {
                    communityUser.UserInfo.Role = Role.Community_Specialist_Delegate;
                    communityUser.ParentId = UserInfo.ID;
                    communityUser.CommunityUserId = userBusiness.CommunitySpecialistDelegateCode();
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
                    .Replace("{InviteLink}", "<a style='text-decoration: underline; cursor:pointer; color: #008000;' href='" + link + "'>Click here</a>")
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
                communityUser.UserInfo.LastName, communityUser.UserInfo.PrimaryEmailAddress,
                communityUser.UserInfo.Role, communityId, out existedUserId, out existedUserRole);
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
                if (existedUserRole == Role.District_Community_Specialist)
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
                    else if (UserInfo.Role == Role.District_Community_Delegate)
                    {
                        UserBaseEntity parentUser = userBusiness.GetUser(UserInfo.ID);
                        //如果当前用户所属的Community包含查询出的用户所属的Community，则直接跳转到分配页面，否则需要发送邮件请求
                        if (
                            parentUser.UserCommunitySchools.Where(e => e.CommunityId > 0).Any(
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
                        // 已存在Community Specialist 角色的并且FirstName,LastName,Email相同的记录,但是操作者没有权限(意外勾选了不属于操作者的权限)
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
        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.Community_Specialist_List, Anonymity = Anonymous.Verified)]
        public string SendEmail(int communityId, string txtCommunity, string email, int userId)
        {
            EmailTemplete template = XmlHelper.GetEmailTemplete("NoPermission_Invite_Template.xml");
            var user = userBusiness.GetUser(userId);
            var recipient = user.FirstName + " " + user.LastName;

            var approveLink = new LinkModel()
            {
                RoleType = Role.District_Community_Specialist,
                Host = SFConfig.MainSiteDomain,
                Path = "Approve/",
                Sender = UserInfo.ID,
                Recipient = userId
            };
            approveLink.Others.Add("CommunityId", communityId);

            var denyLink = new LinkModel()
            {
                RoleType = Role.District_Community_Specialist,
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

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.Administrative, Anonymity = Anonymous.Logined)]
        public void InternalAccess()
        {
            switch (UserInfo.Role)
            {
                case Role.District_Community_Specialist:
                case Role.Community_Specialist_Delegate:
                    ViewBag.CommunitySpecialistNote = "X";
                    break;
                case Role.Community:
                case Role.District_Community_Delegate:
                    ViewBag.CommunitySpecialistNote = "X";
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
            bool accessAssignCommunity = false;
            bool accessAssignClass = false;
            bool accessBES = false;
            if (UserInfo != null)
            {
                UserAuthorityModel userAuthority = new PermissionBusiness().GetUserAuthority(UserInfo, (int)PagesModel.Community_Specialist_List);

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
                if (UserInfo.IsCLIUser || UserInfo.Role == Role.Community || UserInfo.Role == Role.Statewide)
                {
                    accessAssignCommunity = true;
                }
                if (UserInfo.Role == Role.Principal || UserInfo.Role == Role.Principal_Delegate
                    || UserInfo.Role == Role.Super_admin)
                {
                    accessAssignClass = true;
                }
            }
            ViewBag.accessView = accessView;
            ViewBag.accessAdd = accessAdd;
            ViewBag.accessEdit = accessEdit;
            ViewBag.accessPermission = accessPermission;
            ViewBag.accessAssignCommunity = accessAssignCommunity;
            ViewBag.accessAssignClass = accessAssignClass;
            ViewBag.accessBES = accessBES;
        }

        #region BES
        [CLIUrlAuthorizeAttribute(Account = Authority.Bes, PageId = PagesModel.Community_Specialist_List, Anonymity = Anonymous.Verified)]
        public ActionResult CommunitySpecialistBES()
        {

            InitAccessOperation();
            ViewBag.StatusJson = JsonHelper.SerializeObject(EntityStatus.Active.ToList());
            ViewBag.PositionOptions = JsonHelper.SerializeObject(userBusiness.GetPositions((int)Role.District_Community_Specialist).ToSelectList(ViewTextHelper.DefaultPleaseSelectText, ""));
            ViewBag.GendersJson = JsonHelper.SerializeObject(Gender.Female.ToList());
            ViewBag.LanguageJson = JsonHelper.SerializeObject(userBusiness.GetLanguages().ToSelectList(ViewTextHelper.DefaultPleaseSelectText, "0"));

            ViewBag.PhoneTypeOptions = JsonHelper.SerializeObject(PhoneType.HomeNumber.ToList());
            CommunitySpecialistUserModel userModel = new CommunitySpecialistUserModel();
            return View(userModel);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.Bes, PageId = PagesModel.Community_Specialist_List, Anonymity = Anonymous.Verified)]
        public string BESSearch(string txtCommunity, string communitySpecialistCode, string firstName, string lastName, int communityId = 0,
              int status = -1, string sort = "LastName", string order = "Asc", int first = 0, int count = 10)
        {
            var total = 0;
            var expression = PredicateHelper.True<CommunityUserEntity>();
            if (communityId > 0)
                expression = expression.And(r => r.UserInfo.UserCommunitySchools.
                    Any(m => m.CommunityId == communityId));
            else if (txtCommunity.Trim() != string.Empty)
                expression = expression.And(r => r.UserInfo.UserCommunitySchools.
                    Any(m => m.Community.Name.Contains(txtCommunity)));
            if (status > 0)
                expression = expression.And(r => (int)r.UserInfo.Status == status);
            if (communitySpecialistCode.Trim() != string.Empty)
                expression = expression.And(r => r.CommunityUserId.Contains(communitySpecialistCode));
            if (firstName.Trim() != string.Empty)
                expression = expression.And(r => r.UserInfo.FirstName.Contains(firstName));
            if (lastName.Trim() != string.Empty)
                expression = expression.And(r => r.UserInfo.LastName.Contains(lastName));
            expression = expression.And(r => r.UserInfo.Role == Role.District_Community_Specialist);
            if (!UserInfo.IsCLIUser)
            {
                var communityIds = UserInfo.UserCommunitySchools.Where(o => o.CommunityId > 0).Select(o => o.CommunityId).ToList();
                expression = expression.And(r => r.UserInfo.UserCommunitySchools.
                Any(m => communityIds.Contains(m.CommunityId)));
            }
            //if (sort == "CommunityId")
            //    sort = "FirstName";
            var list = userBusiness.SearchCommunitySpecialistsForBES(UserInfo, expression, sort, order, first, count, out total);
            var result = new { total = total, data = list };
            return JsonHelper.SerializeObject(result);
        }

        [HttpPost]
        [CLIUrlAuthorizeAttribute(Account = Authority.Bes, PageId = PagesModel.Community_Specialist_List,
            Anonymity = Anonymous.Verified)]
        public string BESSave(string CommunitySpecialists, bool confirm = false)
        {
            var response = new PostFormResponse();
            OperationResult res = new OperationResult(OperationResultType.Success);
            List<CommunitySpecialistUserModel> listCommunitySpecialist = JsonHelper.DeserializeObject<List<CommunitySpecialistUserModel>>(CommunitySpecialists);
            if (!confirm)
            {
                if (CheckUsersList(listCommunitySpecialist, response))
                    return JsonHelper.SerializeObject(response);
            }
            foreach (CommunitySpecialistUserModel model in listCommunitySpecialist)
            {
                CommunityUserEntity item = null;
                if (model.ID <= 0)
                {
                    item = new CommunityUserEntity();
                    item.UserInfo = new UserBaseEntity();
                    item.UserInfo.Role = Role.District_Community_Specialist;
                    item.CommunityUserId = userBusiness.CommunitySpecialistCode();
                }
                else
                {
                    item = userBusiness.GetCommunity(model.ID, UserInfo, Role.District_Community_Specialist, false);
                    if (item == null)
                    {
                        res.Message = "User with name:" + model.FirstName + " " + model.LastName + " does not exist.";
                    }
                }

                int communityId = model.CommunityId;

                item.PositionId = model.PositionId; 
                item.UserInfo.FirstName = model.FirstName;
                item.UserInfo.MiddleName = model.MiddleName ?? "";
                item.UserInfo.LastName = model.LastName;
                item.UserInfo.PreviousLastName ="";
                item.BirthDate =  CommonAgent.MinDate;
                item.Gender = (Gender)0;
                item.PrimaryLanguageId =0;
                item.UserInfo.Status = model.Status;
                item.IsSameAddress =0;
                item.Address = "";
                item.UserInfo.PrimaryPhoneNumber = model.PrimaryPhoneNumber;
                item.UserInfo.PrimaryNumberType =(PhoneType)(0);
                item.UserInfo.SecondaryPhoneNumber ="";
                item.UserInfo.SecondaryNumberType = (PhoneType)(0);
                item.UserInfo.FaxNumber ="";
                item.UserInfo.PrimaryEmailAddress = model.PrimaryEmail;
                item.UserInfo.SecondaryEmailAddress = "";

                if (item.ID <= 0)
                {
                    res = userBusiness.InsertCommunityUser(item, communityId, new List<int>(), new int[0], UserInfo.ID);
                }
                else
                {
                    res = userBusiness.UpdateCommunityUser(item, new List<int>(), new int[0]);
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


        private void SendInvitation(CommunityUserEntity communityUser)
        {
            EmailTemplete template = XmlHelper.GetEmailTemplete("TeacherInvitation_Template.xml");
            string param = communityUser.UserInfo.ID.ToString() + "," + DateTime.Now.ToString();
            string encryptParam = ObjectFactory.GetInstance<IEncrypt>().Encrypt(param);
            string link = SFConfig.MainSiteDomain + "Home/InviteVerification/"
                + System.Web.HttpContext.Current.Server.UrlEncode(encryptParam);
            string emailBody = template.Body.Replace("{FirstName}", communityUser.UserInfo.FirstName)
            .Replace("{LastName}", communityUser.UserInfo.LastName)
            .Replace("{InviteLink}", "<a style='text-decoration: underline; cursor:pointer; color: #008000;' href='" + link + "'>Click here</a>")
            .Replace("{StaticDomain}", SFConfig.StaticDomain)
            .Replace("{CliEngageUrl}", SFConfig.MainSiteDomain);
            userBusiness.SendEmail(communityUser.UserInfo.PrimaryEmailAddress, template.Subject, emailBody);
            EmailLogEntity emailLog = new EmailLogEntity(communityUser.UserInfo.ID,
                communityUser.UserInfo.PrimaryEmailAddress, EmailLogType.Invitation);
            userBusiness.InsertEmailLog(emailLog);
            operationLogBusiness.InsertLog(OperationEnum.ResetSendInvitation,
                communityUser.UserInfo.Role.ToDescription(), "Send Invitation",
                CommonHelper.GetIPAddress(Request), UserInfo);
        }

        private bool CheckUsersList(List<CommunitySpecialistUserModel> listUser, PostFormResponse response)
        {
            var userList = listUser.Where(t => t.ID == 0).GroupBy(o => new { o.FirstName, o.LastName, o.PrimaryEmail }).ToList();
            userList = userList.Where(o => o.Count() > 1).ToList();

            CommunityUserEntity communityUser = new CommunityUserEntity();
            communityUser.UserInfo = new UserBaseEntity();

            if (userList.Count > 0)
            {
                communityUser.UserInfo.Role = Role.District_Community_Specialist;
                communityUser.UserInfo.FirstName = userList[0].Key.FirstName;
                communityUser.UserInfo.LastName = userList[0].Key.LastName;
                communityUser.UserInfo.PrimaryEmailAddress = userList[0].Key.PrimaryEmail;
                if (CheckExisted3(communityUser, UserExistedStatus.UserExisted, response))
                {
                    return true;
                }
            }

            foreach (CommunitySpecialistUserModel model in listUser)
            {
                CommunityUserEntity item = null;
                if (model.ID <= 0)
                {
                    item = new CommunityUserEntity();
                    item.UserInfo = new UserBaseEntity();
                    item.UserInfo.Role = Role.District_Community_Specialist;
                }
                else
                {
                    item = userBusiness.GetCommunity(model.ID, UserInfo, Role.District_Community_Specialist, false);
                }
                int communityId = model.CommunityId;
                item.PositionId = model.PositionId;
                item.UserInfo.FirstName = model.FirstName;
                item.UserInfo.LastName = model.LastName;
                item.UserInfo.Status = model.Status;
                item.UserInfo.PrimaryEmailAddress = model.PrimaryEmail;

                if (CheckExisted2(item, communityId, response))
                {
                    return true;
                }
            }
            return false;
        }

        private bool CheckExisted2(CommunityUserEntity communityUser, int communityId, PostFormResponse response)
        {
            var existedUserId = 0;
            Role existedUserRole;
            var existedStatus = userBusiness.CheckCommunityUserExistedStatus(communityUser.UserInfo.ID, communityUser.UserInfo.FirstName,
                communityUser.UserInfo.LastName, communityUser.UserInfo.PrimaryEmailAddress,
                communityUser.UserInfo.Role, communityId, out existedUserId, out existedUserRole);
            if (existedStatus == UserExistedStatus.ExistedInCommunity)
            {
                // do nothing
                response.Success = true;
                response.Message = "More than one community specialist with the same first name, last name, and primary email already exists in the same community: "
                   + communityUser.UserInfo.FirstName + " " + communityUser.UserInfo.LastName + " " + communityUser.UserInfo.PrimaryEmailAddress;

                response.Data = "waiting";
                return true;
            }
            else if (existedStatus == UserExistedStatus.UserExisted)
            {
                if (existedUserRole == Role.District_Community_Specialist)
                {
                    // do nothing
                    response.Success = true;
                    response.Message = "More than one community specialist with the same first name, last name, and primary email already exists in Cli Engage: "
                       + communityUser.UserInfo.FirstName + " " + communityUser.UserInfo.LastName + " " + communityUser.UserInfo.PrimaryEmailAddress;

                    response.Data = "waiting";
                    return true;
                }
                else
                {
                    // 存在其他角色的用户提醒 Continue
                    response.Success = true;
                    response.Message = "More than one user with the same first name, last name, and primary email already exists in Cli Engage: "
                    + communityUser.UserInfo.FirstName + " " +
                    communityUser.UserInfo.LastName
                    + " (" + existedUserRole.ToDescription() + ") " + communityUser.UserInfo.PrimaryEmailAddress + ", Continue?";

                    response.Data = new
                    {
                        type = "continue"
                    };
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 判断当前页面中有可能会重复的记录
        /// </summary>
        /// <param name="teacher"></param>
        /// <param name="existedStatus"></param>
        /// <param name="schoolId"></param>
        /// <param name="response"></param>
        /// <returns></returns>
        private bool CheckExisted3(CommunityUserEntity communityUser, UserExistedStatus existedStatus, PostFormResponse response)
        {
            if (existedStatus == UserExistedStatus.UserExisted)
            {
                // 已存在Teacher 角色的并且FirstName,LastName,Email相同的记录,但是操作者没有权限(意外勾选了不属于操作者的权限)
                response.Success = true;
                response.Message = "More than one community specialist with the same first name, last name, and primary email already exists in this list: "
                    + communityUser.UserInfo.FirstName + " " + communityUser.UserInfo.LastName + " " + communityUser.UserInfo.PrimaryEmailAddress;
                response.Data = "waiting";
                return true;
            }
            return false;
        }
        #endregion
    }
}