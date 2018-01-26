using Sunnet.Cli.Business.Log;
using Sunnet.Cli.Business.Users;
using Sunnet.Cli.Core.Common.Enums;
using Sunnet.Cli.Core.Log.Entities;
using Sunnet.Cli.Core.Users.Entities;
using Sunnet.Cli.Core.Users.Enums;
using Sunnet.Cli.MainSite.Controllers;
using Sunnet.Cli.UIBase.Models;
using System;
using System.Linq;
using System.Web.Mvc;
using Sunnet.Framework.Extensions;
using Sunnet.Cli.Business.Common;
using Sunnet.Framework.Core.Tool;
using Sunnet.Framework.Helpers;
using Sunnet.Framework.Encrypt;
using StructureMap;
using Sunnet.Framework;
using Sunnet.Cli.Business.Permission.Models;
using Sunnet.Cli.Business.Permission;
using Sunnet.Framework.Permission;
using Sunnet.Cli.UIBase.Filters;

namespace Sunnet.Cli.MainSite.Areas.Invitation.Controllers
{
    public class AuditorController : BaseController
    {
        private readonly UserBusiness userBusiness;
        private readonly PermissionBusiness permissionBusiness;
        private readonly OperationLogBusiness operationLogBusiness;

        public AuditorController()
        {
            userBusiness = new UserBusiness(UnitWorkContext);
            permissionBusiness = new PermissionBusiness(UnitWorkContext);
            operationLogBusiness = new OperationLogBusiness(UnitWorkContext);
        }
        //
        // GET: /Invitation/Auditor/
        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.Auditor, Anonymity = Anonymous.Verified)]
        public ActionResult Index()
        {
            InitAccessOperation();
            return View();
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.Auditor, Anonymity = Anonymous.Verified)]
        public string Search(string auditorCode, string firstName, string lastName,
            int status = -1, string sort = "LastName", string order = "Asc", int first = 0, int count = 10)
        {
            var total = 0;
            var expression = PredicateHelper.True<AuditorEntity>();
            if (auditorCode.Trim() != string.Empty)
                expression = expression.And(r => r.AuditorId.Contains(auditorCode));
            if (firstName.Trim() != string.Empty)
                expression = expression.And(r => r.UserInfo.FirstName.Contains(firstName));
            if (lastName.Trim() != string.Empty)
                expression = expression.And(r => r.UserInfo.LastName.Contains(lastName));

            if (status > 0)
                expression = expression.And(r => (int)r.UserInfo.Status == status);
            var list = userBusiness.SearchAuditors(expression, sort, order, first, count, out total);
            var result = new { total = total, data = list };
            return JsonHelper.SerializeObject(result);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.Add, PageId = PagesModel.Auditor, Anonymity = Anonymous.Verified)]
        public ActionResult New()
        {
            UserBaseEntity userEntity = new UserBaseEntity();
            AuditorEntity auditorEntity = new AuditorEntity();
            auditorEntity.UserInfo = userEntity;
            ViewBag.PositionOptions = userBusiness.GetPositions((int)Role.Auditor).
                ToSelectList(ViewTextHelper.DefaultPleaseSelectText, "");
            ViewBag.AffiliationOptions = userBusiness.GetAffiliations().
                ToSelectList(ViewTextHelper.DefaultPleaseSelectText, "");
            ViewBag.GroupPackages = permissionBusiness.GetCustomPackages(Role.Auditor);

            return View(auditorEntity);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.Edit, PageId = PagesModel.Auditor, Anonymity = Anonymous.Verified)]
        public ActionResult Edit(int id)
        {
            AuditorEntity auditorEntity = userBusiness.GetAuditor(id);
            ViewBag.PositionOptions = userBusiness.GetPositions((int)Role.Auditor).
                ToSelectList(ViewTextHelper.DefaultPleaseSelectText, "");
            ViewBag.AffiliationOptions = userBusiness.GetAffiliations().
                ToSelectList(ViewTextHelper.DefaultPleaseSelectText, "");
            ViewBag.GroupPackages = permissionBusiness.GetCustomPackages(Role.Auditor);
            ViewBag.GroupPackageSelected = auditorEntity.UserInfo.PermissionRoles.Select(e =>
                new GroupPackageModel()
                {
                    PackageId = e.ID,
                    PackageName = e.Name,
                    PackageDescription = e.Descriptions
                }).ToList();

            return View(auditorEntity);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.View, PageId = PagesModel.Auditor, Anonymity = Anonymous.Verified)]
        public ActionResult View(int id)
        {
            AuditorEntity auditorEntity = userBusiness.GetAuditor(id);
            PositionEntity position = userBusiness.GetPosition(auditorEntity.PositionId);
            if (position != null)
                ViewBag.Position = position.Title;
            AffiliationEntity affiliation = userBusiness.GetAffiliation(auditorEntity.AffiliationId);
            if (affiliation != null)
                ViewBag.Affiliation = affiliation.Affiliation;
            ViewBag.GroupPackageSelected = auditorEntity.UserInfo.PermissionRoles.Where(e => e.IsDefault == false).Select(e =>
                new GroupPackageModel()
                {
                    PackageId = e.ID,
                    PackageName = e.Name,
                    PackageDescription = e.Descriptions
                }).Select(x => x.PackageName).ToList();

            return View(auditorEntity);
        }

        [System.Web.Mvc.HttpPost]
        [ValidateAntiForgeryToken]
        [CLIUrlAuthorizeAttribute(Account = Authority.Edit, PageId = PagesModel.Auditor, Anonymity = Anonymous.Verified)]
        public string SaveInvitation(AuditorEntity auditor, bool? isInvite, int[] chkPackages)
        {
            if (auditor.ID == 0)
            {
                auditor.UserInfo.GoogleId = "";
                auditor.AuditorId = userBusiness.AuditorCode();
                auditor.UserInfo.Role = Role.Auditor;
                auditor.SchoolYear = CommonAgent.SchoolYear;
                auditor.UserInfo.StatusDate = DateTime.Now;
                auditor.UserInfo.Sponsor = UserInfo.ID;
                auditor.UserInfo.InvitationEmail = InvitationEmailEnum.NotSend;
                auditor.UserInfo.Notes = RegisterType.Invitation.ToDescription();
            }
            if (isInvite == true)
            {
                auditor.UserInfo.EmailExpireTime = DateTime.Now.AddDays(SFConfig.ExpirationTime);
                auditor.UserInfo.InvitationEmail = InvitationEmailEnum.Sent;
            }
            var response = new PostFormResponse();
            if (response.Success = ModelState.IsValid)
            {
                OperationResult result = new OperationResult(OperationResultType.Success);
                if (auditor.ID > 0)
                {
                    var user = userBusiness.GetUser(auditor.UserInfo.ID);
                    if (user.Status == EntityStatus.Inactive && auditor.UserInfo.Status == EntityStatus.Active)
                    {
                        operationLogBusiness.InsertLog(OperationEnum.ResetSendInvitation,
                            auditor.UserInfo.Role.ToDescription(),
                            "Status Changed:" + "Inactive to Active" +
                            ",UserId:" + auditor.UserInfo.ID,
                            CommonHelper.GetIPAddress(Request), UserInfo);
                    }
                    result = userBusiness.UpdateAuditor(auditor, chkPackages);
                }
                else
                {
                    result = userBusiness.InsertAuditor(auditor, chkPackages);
                    operationLogBusiness.InsertLog(OperationEnum.ResetSendInvitation,
                        auditor.UserInfo.Role.ToDescription(), "Created User,UserId:" + auditor.UserInfo.ID,
                        CommonHelper.GetIPAddress(Request), UserInfo);
                }
                if (isInvite == true)
                {
                    EmailTemplete template = XmlHelper.GetEmailTemplete("TeacherInvitation_Template.xml");
                    string param = auditor.UserInfo.ID.ToString() + "," + DateTime.Now.ToString();
                    string encryptParam = ObjectFactory.GetInstance<IEncrypt>().Encrypt(param);
                    string link = SFConfig.MainSiteDomain + "Home/InviteVerification/"
                        + System.Web.HttpContext.Current.Server.UrlEncode(encryptParam);
                    string emailBody = template.Body.Replace("{FirstName}", auditor.UserInfo.FirstName)
                    .Replace("{LastName}", auditor.UserInfo.LastName)
                    .Replace("{InviteLink}", "<a style='text-decoration: underline; cursor:pointer; color: #008000;' href='" + link + "'>Click here</a>")
                    .Replace("{StaticDomain}", SFConfig.StaticDomain)
                    .Replace("{CliEngageUrl}", SFConfig.MainSiteDomain);
                    userBusiness.SendEmail(auditor.UserInfo.PrimaryEmailAddress, template.Subject, emailBody);
                    EmailLogEntity emailLog = new EmailLogEntity(auditor.UserInfo.ID,
                        auditor.UserInfo.PrimaryEmailAddress, EmailLogType.Invitation);
                    userBusiness.InsertEmailLog(emailLog);
                    operationLogBusiness.InsertLog(OperationEnum.ResetSendInvitation,
                        auditor.UserInfo.Role.ToDescription(), "Send Invitation,UserId:" + auditor.UserInfo.ID,
                        CommonHelper.GetIPAddress(Request), UserInfo);
                }
                response.Success = result.ResultType == OperationResultType.Success;
                response.Message = result.Message;
            }
            response.ModelState = ModelState;
            return JsonHelper.SerializeObject(response);
        }

        private void InitAccessOperation()
        {
            bool accessView = false;
            bool accessAdd = false;
            bool accessEdit = false;
            bool accessPermission = false;
            if (UserInfo != null)
            {
                UserAuthorityModel userAuthority = new PermissionBusiness().GetUserAuthority(UserInfo, (int)PagesModel.Auditor);

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
            }
            ViewBag.accessView = accessView;
            ViewBag.accessAdd = accessAdd;
            ViewBag.accessEdit = accessEdit;
            ViewBag.accessPermission = accessPermission;
        }
    }
}