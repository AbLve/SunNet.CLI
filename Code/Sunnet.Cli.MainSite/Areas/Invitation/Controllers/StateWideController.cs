using Sunnet.Cli.Business.Log;
using Sunnet.Cli.Business.Users;
using Sunnet.Cli.Core.Log.Entities;
using Sunnet.Cli.Core.Users.Entities;
using Sunnet.Cli.MainSite.Controllers;
using Sunnet.Cli.UIBase.Models;
using System;
using System.Linq;
using System.Web.Mvc;
using Sunnet.Framework.Extensions;
using Sunnet.Cli.Core.Users.Enums;
using Sunnet.Cli.Business.Common;
using Sunnet.Cli.Core.Common.Enums;
using Sunnet.Framework.Core.Tool;
using Sunnet.Framework.Helpers;
using StructureMap;
using Sunnet.Framework.Encrypt;
using Sunnet.Framework;
using Sunnet.Cli.Business.Permission;
using Sunnet.Cli.Business.Permission.Models;
using Sunnet.Framework.Permission;
using Sunnet.Cli.UIBase.Filters;
using Sunnet.Framework.Resources;

namespace Sunnet.Cli.MainSite.Areas.Invitation.Controllers
{
    public class StateWideController : BaseController
    {
        private readonly UserBusiness userBusiness;
        private readonly PermissionBusiness permissionBusiness;
        private readonly OperationLogBusiness operationLogBusiness;

        public StateWideController()
        {
            userBusiness = new UserBusiness(UnitWorkContext);
            permissionBusiness = new PermissionBusiness(UnitWorkContext);
            operationLogBusiness = new OperationLogBusiness(UnitWorkContext);
        }
        //
        // GET: /Invitation/StateWide/
        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.Statewide, Anonymity = Anonymous.Verified)]
        public ActionResult Index()
        {
            InitAccessOperation();
            return View();
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.Statewide, Anonymity = Anonymous.Verified)]
        public string Search(string statewideCode, string firstName, string lastName,
            int status = -1, string sort = "LastName", string order = "Asc", int first = 0, int count = 10)
        {
            var total = 0;
            var expression = PredicateHelper.True<StateWideEntity>();
            if (status > 0)
                expression = expression.And(r => (int)r.UserInfo.Status == status);
            if (statewideCode.Trim() != string.Empty)
                expression = expression.And(r => r.StateWideId.Contains(statewideCode));
            if (firstName.Trim() != string.Empty)
                expression = expression.And(r => r.UserInfo.FirstName.Contains(firstName));
            if (lastName.Trim() != string.Empty)
                expression = expression.And(r => r.UserInfo.LastName.Contains(lastName));
            switch (UserInfo.Role)
            {
                case Role.Statewide:
                case Role.Content_personnel:
                case Role.Statisticians:
                case Role.Administrative_personnel:
                case Role.Intervention_manager:
                case Role.Video_coding_analyst:
                case Role.Intervention_support_personnel:
                case Role.Coordinator:
                case Role.Mentor_coach:
                    expression =
                        expression.And(
                            r =>
                                r.UserInfo.UserCommunitySchools.Any(
                                    s => s.Community.UserCommunitySchools.Any(t => t.UserId == UserInfo.ID)));
                    break;
            }
            var list = userBusiness.SearchStatewides(expression, sort, order, first, count, out total);
            var result = new { total = total, data = list };
            return JsonHelper.SerializeObject(result);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.Add, PageId = PagesModel.Statewide, Anonymity = Anonymous.Verified)]
        public ActionResult New()
        {
            UserBaseEntity userEntity = new UserBaseEntity();
            StateWideEntity stateWideEntity = new StateWideEntity();
            ViewBag.GroupPackages = permissionBusiness.GetCustomPackages(Role.Statewide);
            stateWideEntity.UserInfo = userEntity;
            ViewBag.PositionOptions = userBusiness.GetPositions((int)Role.Statewide).
                ToSelectList(ViewTextHelper.DefaultPleaseSelectText, "");

            return View(stateWideEntity);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.Edit, PageId = PagesModel.Statewide, Anonymity = Anonymous.Verified)]
        public ActionResult Edit(int id)
        {
            StateWideEntity stateWideEntity = userBusiness.GetStateWide(id);
            ViewBag.GroupPackages = permissionBusiness.GetCustomPackages(Role.Statewide);
            ViewBag.GroupPackageSelected = stateWideEntity.UserInfo.PermissionRoles.Select(e =>
                new GroupPackageModel()
                {
                    PackageId = e.ID,
                    PackageName = e.Name,
                    PackageDescription = e.Descriptions
                }).ToList();
            ViewBag.PositionOptions = userBusiness.GetPositions((int)Role.Statewide).
                ToSelectList(ViewTextHelper.DefaultPleaseSelectText, "");

            return View(stateWideEntity);
        }


        [CLIUrlAuthorizeAttribute(Account = Authority.View, PageId = PagesModel.Statewide, Anonymity = Anonymous.Verified)]
        public ActionResult View(int id)
        {
            StateWideEntity stateWideEntity = userBusiness.GetStateWide(id);
            ViewBag.GroupPackageSelected = stateWideEntity.UserInfo.PermissionRoles.Where(e => e.IsDefault == false).Select(e =>
                new GroupPackageModel()
                {
                    PackageId = e.ID,
                    PackageName = e.Name,
                    PackageDescription = e.Descriptions
                }).Select(x => x.PackageName).ToList();
            PositionEntity position = userBusiness.GetPosition(stateWideEntity.PositionId);
            if (position != null)
                ViewBag.Position = position.Title;

            return View(stateWideEntity);
        }

        [System.Web.Mvc.HttpPost]
        [ValidateAntiForgeryToken]
        [CLIUrlAuthorizeAttribute(Account = Authority.Edit, PageId = PagesModel.Statewide, Anonymity = Anonymous.Verified)]
        public string SaveInvitation(StateWideEntity stateWide, bool? isInvite, int[] chkPackages,
            bool confirm = false)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            PostFormResponse response = new PostFormResponse();
            if (confirm == false)
            {
                if (stateWide.ID == 0)
                {
                    if (CheckExisted(stateWide, response))
                        return JsonHelper.SerializeObject(response);
                }
                else
                {
                    if (userBusiness.CheckUserExistedStatus(stateWide.UserInfo.ID,
                        stateWide.UserInfo.FirstName,
                        stateWide.UserInfo.LastName, stateWide.UserInfo.PrimaryEmailAddress,
                        stateWide.UserInfo.Role, out result))
                    {
                        response.Success = true;
                        response.Message = result.Message;
                        response.Data = result.AppendData;
                        return JsonHelper.SerializeObject(response);
                    }
                }
            }
            if (stateWide.ID == 0)
            {
                stateWide.UserInfo.GoogleId = "";
                stateWide.StateWideId = userBusiness.StatewideCode();
                stateWide.UserInfo.Role = Role.Statewide;
                stateWide.SchoolYear = CommonAgent.SchoolYear;
                stateWide.UserInfo.StatusDate = DateTime.Now;
                stateWide.UserInfo.Sponsor = UserInfo.ID;
                stateWide.UserInfo.InvitationEmail = InvitationEmailEnum.NotSend;
                stateWide.UserInfo.Notes = RegisterType.Invitation.ToDescription();
            }
            if (isInvite == true)
            {
                stateWide.UserInfo.EmailExpireTime = DateTime.Now.AddDays(SFConfig.ExpirationTime);
                stateWide.UserInfo.InvitationEmail = InvitationEmailEnum.Sent;
            }
            if (response.Success = ModelState.IsValid)
            {
                if (stateWide.ID > 0)
                {
                    var user = userBusiness.GetUser(stateWide.UserInfo.ID);
                    if (user.Status == EntityStatus.Inactive && stateWide.UserInfo.Status == EntityStatus.Active)
                    {
                        operationLogBusiness.InsertLog(OperationEnum.ResetSendInvitation,
                            stateWide.UserInfo.Role.ToDescription(),
                            "Status Changed:" + "Inactive to Active" +
                            ",UserId:" + stateWide.UserInfo.ID,
                            CommonHelper.GetIPAddress(Request), UserInfo);
                    }
                    result = userBusiness.UpdateStateWide(stateWide, chkPackages);
                }
                else
                {
                    result = userBusiness.InsertStateWide(stateWide, chkPackages);
                    operationLogBusiness.InsertLog(OperationEnum.ResetSendInvitation,
                        stateWide.UserInfo.Role.ToDescription(), "Created User,UserId:" + stateWide.UserInfo.ID,
                        CommonHelper.GetIPAddress(Request), UserInfo);
                }
                if (isInvite == true)
                {
                    EmailTemplete template = XmlHelper.GetEmailTemplete("TeacherInvitation_Template.xml");
                    string param = stateWide.UserInfo.ID.ToString() + "," + DateTime.Now.ToString();
                    string encryptParam = ObjectFactory.GetInstance<IEncrypt>().Encrypt(param);
                    string link = SFConfig.MainSiteDomain + "Home/InviteVerification/"
                        + System.Web.HttpContext.Current.Server.UrlEncode(encryptParam);
                    string emailBody = template.Body.Replace("{FirstName}", stateWide.UserInfo.FirstName)
                    .Replace("{LastName}", stateWide.UserInfo.LastName)
                    .Replace("{InviteLink}", "<a style='text-decoration: underline; cursor:pointer; color: #008000;' href='" + link + "'>Click here</a>")
                    .Replace("{StaticDomain}", SFConfig.StaticDomain)
                    .Replace("{CliEngageUrl}", SFConfig.MainSiteDomain);
                    userBusiness.SendEmail(stateWide.UserInfo.PrimaryEmailAddress, template.Subject, emailBody);
                    EmailLogEntity emailLog = new EmailLogEntity(stateWide.UserInfo.ID,
                        stateWide.UserInfo.PrimaryEmailAddress, EmailLogType.Invitation);
                    userBusiness.InsertEmailLog(emailLog);
                    operationLogBusiness.InsertLog(OperationEnum.ResetSendInvitation,
                        stateWide.UserInfo.Role.ToDescription(), "Send Invitation,UserId:" + stateWide.UserInfo.ID,
                        CommonHelper.GetIPAddress(Request), UserInfo);
                }
                response.Success = result.ResultType == OperationResultType.Success;
                response.Message = result.Message;
            }
            response.ModelState = ModelState;
            return JsonHelper.SerializeObject(response);
        }

        private bool CheckExisted(StateWideEntity statewide, PostFormResponse response)
        {
            statewide.UserInfo.Role = Role.Statewide;
            Role existedUserRole;
            var existedStatus = userBusiness.CheckStatewideExistedStatus(statewide.UserInfo.FirstName,
                statewide.UserInfo.LastName, statewide.UserInfo.PrimaryEmailAddress, statewide.UserInfo.Role,
                out existedUserRole);
            if (existedStatus == UserExistedStatus.UserExisted)
            {
                if (existedUserRole == Role.Statewide)
                {
                    // do nothing
                    response.Success = true;
                    response.Message = ResourceHelper.GetRM().GetInformation("UserExistedInStatewide")
                        .Replace("{Role}", existedUserRole.ToDescription().ToLower());
                    response.Data = "waiting";
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

        [CLIUrlAuthorizeAttribute(Account = Authority.Edit, PageId = PagesModel.Statewide, Anonymity = Anonymous.Verified)]
        public ActionResult AssignCommunity(int userId)
        {
            UserBaseEntity user = userBusiness.GetUser(userId);
            return View(user);
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
                UserAuthorityModel userAuthority = new PermissionBusiness().GetUserAuthority(UserInfo, (int)PagesModel.Statewide);

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
                if (UserInfo.IsCLIUser)
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