using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Web.Mvc;
using System;
using System.Linq;
using System.Web.UI.WebControls;
using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
using Sunnet.Cli.Business.Communities.Enums;
using Sunnet.Cli.Business.StatusTracking;
using Sunnet.Cli.Core.StatusTracking.Entities;
using Sunnet.Cli.Core.StatusTracking.Enums;
using Sunnet.Cli.MainSite.Models;
using Sunnet.Cli.UIBase.Controllers;
using Sunnet.Cli.Core.Users.Entities;
using Sunnet.Cli.Business.Users;
using Sunnet.Cli.UIBase.Filters;
using Sunnet.Framework.Core.Tool;
using Sunnet.Framework.EmailSender;
using Sunnet.Framework.Extensions;
using Sunnet.Framework.Helpers;
using StructureMap;
using Sunnet.Framework.Encrypt;
using Sunnet.Cli.Business.Common;
using Sunnet.Framework.SSO;
using Sunnet.Cli.UIBase.Models;
using System.Text;
using System.Web;
using Sunnet.Cli.Core.Users.Enums;
using Sunnet.Framework;
using Sunnet.Framework.Log;
using Sunnet.Cli.Business.Permission;
using Sunnet.Cli.Core.Permission.Entities;
using Sunnet.Cli.Business.Permission.Models;
using System.Collections.Generic;
using Sunnet.Cli.Business.Ade;
using Sunnet.Cli.Core.Common.Enums;
using Sunnet.Cli.UIBase;
using Sunnet.Cli.Business.Communities;
using Sunnet.Cli.Business.Cpalls.Models;
using Sunnet.Cli.Business.UpdateCluster;
using Sunnet.Cli.Business.Users.Models;
using Sunnet.Cli.Core.Ade;
using Sunnet.Cli.Core.UpdateClusters.Entities;
using Sunnet.Framework.Permission;

namespace Sunnet.Cli.MainSite.Controllers
{
    [RoutePrefix("home")]
    public class HomeController : BaseController
    {

        private delegate void SendHandler();
        ISunnetLog _log;
        IEncrypt _encrypt;
        UserBusiness _userBusiness;
        PermissionBusiness _permissionBusiness;
        StatusTrackingBusiness statusTrackingBusiness;
        CommunityBusiness _communityBusiness;
        UpdateClusterBusiness _updateClusterBusiness;
        AdeBusiness _adeBusiness;
        public HomeController()
        {
            _userBusiness = new UserBusiness(UnitWorkContext);
            _log = ObjectFactory.GetInstance<ISunnetLog>();
            _encrypt = ObjectFactory.GetInstance<IEncrypt>();
            _permissionBusiness = new PermissionBusiness(UnitWorkContext);
            statusTrackingBusiness = new StatusTrackingBusiness(UnitWorkContext);
            _communityBusiness = new CommunityBusiness(UnitWorkContext);
            _updateClusterBusiness = new UpdateClusterBusiness(UnitWorkContext);
            _adeBusiness = new AdeBusiness(AdeUnitWorkContext);
        }

        public ActionResult Index()
        {
            /*
           David 10/10/2016 it will redirect to another server public pages via NAM.
          */
            if (SFConfig.EnableAccessManager == "1")
            {
                //Ticket 3136 12/01/2017 David, Control by the web.config  Key: HomeRecirectPage
                if (!string.IsNullOrEmpty(SFConfig.HomeRedirectPage))
                {
                    return new RedirectResult(SFConfig.HomeRedirectPage);
                }

                return new RedirectResult("/static/");
            }
                


            string ip = Request.UserHostAddress;
            string ip2 = HttpContext.Request.UserHostAddress;

            if (UserAuthentication != AuthenticationStatus.Login)
            {
                ViewBag.LoginUrl = BuilderLoginUrl();
                ViewBag.CLIUserLogin = BuilderLoginUrl(LoginUserType.UTACCESSMANAGER);
            }
            return View();



        }
        /// <summary>
        /// 专供ssol调用
        /// </summary>
        public ActionResult CallBack(string IASID = "", string TimeStamp = "", string Authenticator = "", string UserAccount = "", string e = "")
        {
            //_log.Info("CallBack-->11111->IASID=" + IASID + ",TimeStamp=" + TimeStamp + ",Authenticator=" + Authenticator + ",UserAccount=" + UserAccount + "e=" + e);
            SSORequest ssoRequest = new SSORequest();
            ssoRequest.IASID = IASID;
            ssoRequest.TimeStamp = TimeStamp;
            ssoRequest.AppUrl = string.Format("{0}home/CallBack", DomainHelper.MainSiteDomain.ToString());
            ssoRequest.UserAccount = UserAccount; //google Id
            ssoRequest.Authenticator = Authenticator;

            //检查是否过期
            if (string.IsNullOrEmpty(TimeStamp))
            {
                CookieHelper.RemoveAll();
                return new RedirectResult(string.Format("{0}home/logout", DomainHelper.SsoSiteDomain));
            }
            else
            {
                var sendTime = new DateTime(long.Parse(TimeStamp));
                if (sendTime < DateTime.Now.AddMinutes(-2) || sendTime > DateTime.Now.AddMinutes(2))
                {
                    CookieHelper.RemoveAll();
                    return new RedirectResult(string.Format("{0}home/logout", DomainHelper.SsoSiteDomain));
                } 
            }
            //_log.Info("CallBack-->222222222222");
            if (Authentication.ValidateCenterToken(ssoRequest))
            {

                //获取是否从  Invitation Email Link过来              
                int inviteUserID = GetInviteSignId();
                //_log.Info("CallBack-->333333333333333->inviteUserID(GetInviteSignId)=" + inviteUserID);

                //根据google账号获取用户信息
                UserBaseEntity user = _userBusiness.UserLogin(UserAccount);
               // _log.Info("CallBack-->4444444444444-->user(UserBaseEntity)=" + user);

                if (user != null && user.ID > 0)//存在登录
                {
                   // _log.Info("CallBack-->555555555-->user.id=" + user.ID + ",user.Gmail="+ user.Gmail);

                    LocalSignIn(user.ID, user.GoogleId, user.FirstName);

                    if (user.Gmail == string.Empty && e.Trim() != string.Empty)
                        _userBusiness.InsertUserMail(user.ID, e);

                   // _log.Info("CallBack-->666666666666-->user.ID=" + user.ID + ",inviteUserID=" + inviteUserID);

                    if (inviteUserID > 0)
                    {
                        if (inviteUserID != user.ID)
                        {
                            return new RedirectResult(string.Format("{0}home/SwitchesUser", DomainHelper.SsoSiteDomain.ToString()));
                        }
                    }

                    //_log.Info("CallBack-->77777777777777-->user.ID=" + user.ID + ",inviteUserID=" + inviteUserID);

                    if (IASID == LoginIASID.LostSession)
                    {
                        string url = CookieHelper.Get("Url");
                        if (url != string.Empty)
                        {
                            CookieHelper.Remove("Url");
                            return new RedirectResult(System.Web.HttpContext.Current.Server.UrlDecode(url));
                        }
                    }
                    //_log.Info("CallBack-->77777777777777-->user.Role=" + user.Role + " (parent is 150)");

                    if (user.Role == Role.Parent)
                    {
                        user.Parent.ParentStatus = ParentStatus.Active;
                        _userBusiness.UpdateParent(user.Parent);
                        //David all users are using same dashboard according June 2017 Rquirement  --07/12/2017
                        //return new RedirectResult(string.Format("{0}home/parentDashboard/sign-in", DomainHelper.MainSiteDomain.ToString()));
                    }
                    return new RedirectResult(string.Format("{0}home/Dashboard/sign-in", DomainHelper.MainSiteDomain.ToString()));
                }

                //_log.Info("CallBack-->88888888888888888");

                if (inviteUserID > 0)//是从Invitation过来
                {
                    //根据UserID获取用户信息
                    user = _userBusiness.GetUser(inviteUserID);
                    if (user != null && user.Status == EntityStatus.Active)
                    {
                        //用户存在，且是有效的，则绑定

                        user.GoogleId = UserAccount;
                        LocalSignIn(user.ID, user.GoogleId, user.FirstName);

                        _userBusiness.UpdateUser(user);
                        _userBusiness.InsertUserMail(user.ID, e);
                        ApplicantEntity applicant = _userBusiness.GetApplicantByInviteeId(user.ID);

                        if (applicant != null)
                        {
                            applicant.Status = ApplicantStatus.Verified;
                            applicant.VerifiedOn = DateTime.Now;
                            _userBusiness.UpdateApplicant(applicant);
                        }
                        if (user.Role == Role.Parent)
                        {
                            user.Parent.ParentStatus = ParentStatus.Active;
                            _userBusiness.UpdateParent(user.Parent);
                        }
                        return new RedirectResult("/home/Dashboard/sign-in");

                    }//if (user != null && user.Status == EntityStatus.Active)
                }// if (userId > 0)//是从Invitation过来

                //SignUserAccount(UserAccount);

                if (ssoRequest.IASID == LoginIASID.ParentSign)
                    return new RedirectResult(string.Format("{0}Signup/Parent/", DomainHelper.MainSiteDomain.ToString()));
                else
                    return new RedirectResult(string.Format("{0}home/GoogleAccount?e={1}", DomainHelper.MainSiteDomain.ToString(), _encrypt.Encrypt(e.Trim())));
            }

            //非法请求
            return new RedirectResult(DomainHelper.MainSiteDomain.ToString());
        }
        protected void ClearCookie()
        {
            Authentication.Logout();
            CookieHelper.RemoveAll();
        }
        /// <summary>
        /// 专供sso调用
        /// </summary>
        public ActionResult LogOut(string forward, string moodleForward)
        {
            ClearCookie();
            ViewBag.LoginUrl_Google = BuilderLoginUrl(LoginUserType.GOOGLEACCOUNT, LoginIASID.MAIN);
            ViewBag.LoginUrl_UT = BuilderLoginUrl(LoginUserType.UTACCESSMANAGER, LoginIASID.MAIN);

            #region David 10/01/2014
            if (forward == null || forward == string.Empty)
            {

                if (SFConfig.EnableAccessManager == "1") //AccessManager Logout first
                {

                    //return new RedirectResult(SFConfig.AccessManagerLogoutUrl);
                    return new RedirectResult(SFConfig.AccessManagerLogoutUrl + "?return=" + HttpUtility.UrlEncode(string.Format("{0}home/LogOut?forward=2", DomainHelper.SsoSiteDomain.ToString())));
                }
            }
            #endregion

            //logout lms site
            if (string.IsNullOrEmpty(moodleForward) && (SFConfig.EnableLMS == "1"))
            {
                return new RedirectResult(string.Format("{0}login/logout.php?auth=cli", SFConfig.LMSDomain));
            }

            return View();
        }

        [Route("~/about")]
        public ActionResult AboutUs()
        {
            if (UserAuthentication != AuthenticationStatus.Login)
            {
                ViewBag.LoginUrl = BuilderLoginUrl();
                ViewBag.CLIUserLogin = BuilderLoginUrl(LoginUserType.UTACCESSMANAGER);
            }
            return View();
        }


        [Route("~/monitortool")]
        public ActionResult MonitorTool()
        {
            if (UserAuthentication != AuthenticationStatus.Login)
            {
                ViewBag.LoginUrl = BuilderLoginUrl();
                ViewBag.CLIUserLogin = BuilderLoginUrl(LoginUserType.UTACCESSMANAGER);
            }
            return View();
        }

        [Route("~/MonitoringProgress")]
        public ActionResult MonitoringProgress()
        {
            if (UserAuthentication != AuthenticationStatus.Login)
            {
                ViewBag.LoginUrl = BuilderLoginUrl();
                ViewBag.CLIUserLogin = BuilderLoginUrl(LoginUserType.UTACCESSMANAGER);
            }
            return View();
        }
        [Route("~/ActivitiesKids")]
        public ActionResult ActivitiesKids()
        {
            if (UserAuthentication != AuthenticationStatus.Login)
            {
                ViewBag.LoginUrl = BuilderLoginUrl();
                ViewBag.CLIUserLogin = BuilderLoginUrl(LoginUserType.UTACCESSMANAGER);
            }
            return View();
        }
        [Route("~/OnlineLearning")]
        public ActionResult OnlineLearning()
        {
            if (UserAuthentication != AuthenticationStatus.Login)
            {
                ViewBag.LoginUrl = BuilderLoginUrl();
                ViewBag.CLIUserLogin = BuilderLoginUrl(LoginUserType.UTACCESSMANAGER);
            }
            return View();
        }
        [Route("~/UnderstandingChildDev")]
        public ActionResult UnderstandingChildDev()
        {
            if (UserAuthentication != AuthenticationStatus.Login)
            {
                ViewBag.LoginUrl = BuilderLoginUrl();
                ViewBag.CLIUserLogin = BuilderLoginUrl(LoginUserType.UTACCESSMANAGER);
            }
            return View();
        }
        [Route("~/onlinecourse")]
        public ActionResult OnlineCourse()
        {
            if (UserAuthentication != AuthenticationStatus.Login)
            {
                ViewBag.LoginUrl = BuilderLoginUrl();
                ViewBag.CLIUserLogin = BuilderLoginUrl(LoginUserType.UTACCESSMANAGER);
            }
            return View();
        }
        [Route("~/observationtool")]
        public ActionResult ObservationTool()
        {
            if (UserAuthentication != AuthenticationStatus.Login)
            {
                ViewBag.LoginUrl = BuilderLoginUrl();
                ViewBag.CLIUserLogin = BuilderLoginUrl(LoginUserType.UTACCESSMANAGER);
            }
            return View();
        }
        [Route("~/coachingtool")]
        public ActionResult CoachingTool()
        {
            if (UserAuthentication != AuthenticationStatus.Login)
            {
                ViewBag.LoginUrl = BuilderLoginUrl();
                ViewBag.CLIUserLogin = BuilderLoginUrl(LoginUserType.UTACCESSMANAGER);
            }
            return View();
        }
        [Route("~/classroomactivity")]
        public ActionResult ClassroomActivity()
        {
            if (UserAuthentication != AuthenticationStatus.Login)
            {
                ViewBag.LoginUrl = BuilderLoginUrl();
                ViewBag.CLIUserLogin = BuilderLoginUrl(LoginUserType.UTACCESSMANAGER);
            }
            return View();
        }
        [Route("~/kindergarten")]
        public ActionResult Kindergarten()
        {
            if (UserAuthentication != AuthenticationStatus.Login)
            {
                ViewBag.LoginUrl = BuilderLoginUrl();
                ViewBag.CLIUserLogin = BuilderLoginUrl(LoginUserType.UTACCESSMANAGER);
            }
            return View();
        }

        [Route("~/calendar")]
        public ActionResult Calendar()
        {
            if (UserAuthentication != AuthenticationStatus.Login)
            {
                ViewBag.LoginUrl = BuilderLoginUrl();
                ViewBag.CLIUserLogin = BuilderLoginUrl(LoginUserType.UTACCESSMANAGER);
            }
            return View();
        }

        [Route("~/Overview")]
        public ActionResult Overview()
        {
            if (UserAuthentication != AuthenticationStatus.Login)
            {
                ViewBag.FirstName = string.Empty;
                ViewBag.LoginUrl = BuilderLoginUrl();
                ViewBag.CLIUserLogin = BuilderLoginUrl(LoginUserType.UTACCESSMANAGER);
            }
            return View();
        }
        [Route("~/Parents")]
        public ActionResult Parents()
        {
            if (UserAuthentication != AuthenticationStatus.Login)
            {
                ViewBag.FirstName = string.Empty;
                ViewBag.LoginUrl = BuilderLoginUrl();
                ViewBag.CLIUserLogin = BuilderLoginUrl(LoginUserType.UTACCESSMANAGER);
            }
            return View();
        }
        [Route("~/faq")]
        public ActionResult Faq()
        {
            if (UserAuthentication != AuthenticationStatus.Login)
            {
                ViewBag.LoginUrl = BuilderLoginUrl();
                ViewBag.CLIUserLogin = BuilderLoginUrl(LoginUserType.UTACCESSMANAGER);
            }
            return View();
        }

     


        [Route("~/tools")]
        public ActionResult Tools()
        {
            if (UserAuthentication != AuthenticationStatus.Login)
            {
                ViewBag.LoginUrl = BuilderLoginUrl();
                ViewBag.CLIUserLogin = BuilderLoginUrl(LoginUserType.UTACCESSMANAGER);
            }
            return View();
        }

        [Route("~/news")]
        public ActionResult News()
        {
            if (UserAuthentication != AuthenticationStatus.Login)
            {
                ViewBag.LoginUrl = BuilderLoginUrl();
                ViewBag.CLIUserLogin = BuilderLoginUrl(LoginUserType.UTACCESSMANAGER);
            }
            return View();
        }

        [Route("~/launch2015")]
        public ActionResult Launch2015()
        {
            if (UserAuthentication != AuthenticationStatus.Login)
            {
                ViewBag.LoginUrl = BuilderLoginUrl();
                ViewBag.CLIUserLogin = BuilderLoginUrl(LoginUserType.UTACCESSMANAGER);
            }
            return View();
        }

        [Route("~/contact")]
        public ActionResult ContactUs()
        {
            if (UserAuthentication != AuthenticationStatus.Login)
            {
                ViewBag.LoginUrl = BuilderLoginUrl();
                ViewBag.CLIUserLogin = BuilderLoginUrl(LoginUserType.UTACCESSMANAGER);
            }
            return View();
        }

        [Route("~/maintenance")]
        public ActionResult Maintenance(int isInternal = 0)
        {
            if (UserAuthentication != AuthenticationStatus.Login)
            {
                ViewBag.LoginUrl = BuilderLoginUrl();
                ViewBag.CLIUserLogin = BuilderLoginUrl(LoginUserType.UTACCESSMANAGER);
            }
            ViewBag.isInternalUser = (isInternal == 1);
            return View();
        }

        [Route("~/helps")]
        public ActionResult Helps()
        {
            if (UserAuthentication != AuthenticationStatus.Login)
            {
                ViewBag.LoginUrl = BuilderLoginUrl();
                ViewBag.CLIUserLogin = BuilderLoginUrl(LoginUserType.UTACCESSMANAGER);
            }
            return View();
        }
        [Route("~/search")]
        public ActionResult Search()
        {
            if (UserAuthentication != AuthenticationStatus.Login)
            {
                ViewBag.LoginUrl = BuilderLoginUrl();
                ViewBag.CLIUserLogin = BuilderLoginUrl(LoginUserType.UTACCESSMANAGER);
            }
            return View();
        }
        [HttpPost]
        [Route("~/contact")]
        [ValidateAntiForgeryToken]
        public ActionResult ContactUs(ContactUsModel model)
        {
            EmailTemplete template = XmlHelper.GetEmailTemplete("ContactUS_Template.xml");
            var body = template.Body
                .Replace("{Name}", model.Name).Replace("{District}", model.District)
                .Replace("{Email}", model.Email).Replace("{Phone}", model.Phone)
                .Replace("{Subject}", model.Subject).Replace("{Content}", model.Content);

            var emailSender = ObjectFactory.GetInstance<IEmailSender>();
            new SendHandler(() => emailSender.SendMail(SFConfig.CLIAdministratorEmail, template.Subject, body))
                   .BeginInvoke(null, null);
            if (UserAuthentication == AuthenticationStatus.Login)
            {
                ViewBag.LoginUrl = BuilderLoginUrl();
                ViewBag.CLIUserLogin = BuilderLoginUrl(LoginUserType.UTACCESSMANAGER);
            }
            return View();
        }

        [Route("~/resources")]
        [CLIUrlAuthorize(Anonymity = Anonymous.Logined)]
        public ActionResult Resources()
        {
            if (UserAuthentication != AuthenticationStatus.Login)
            {
                ViewBag.LoginUrl = BuilderLoginUrl();
                ViewBag.CLIUserLogin = BuilderLoginUrl(LoginUserType.UTACCESSMANAGER);
            }
            return View();
        }

        [Route("~/TakeALook")]
        public ActionResult TakeALook()
        {
            if (UserAuthentication != AuthenticationStatus.Login)
            {
                ViewBag.LoginUrl = BuilderLoginUrl();
                ViewBag.CLIUserLogin = BuilderLoginUrl(LoginUserType.UTACCESSMANAGER);
            }
            return View();
        }
        [Route("~/Eligibility")]
        public ActionResult Eligibility()
        {
            if (UserAuthentication != AuthenticationStatus.Login)
            {
                ViewBag.LoginUrl = BuilderLoginUrl();
                ViewBag.CLIUserLogin = BuilderLoginUrl(LoginUserType.UTACCESSMANAGER);
            }
            return View();
        }

        public ActionResult Dashboard()
        {

            HttpContext.Response.Cache.SetNoStore();
            if (UserAuthentication == AuthenticationStatus.LostSession)
                return new RedirectResult(string.Format("{0}home/LostSession", DomainHelper.SsoSiteDomain));
            if (UserAuthentication != AuthenticationStatus.Login)
                return new RedirectResult("/home/");
            var canAccessTrs = _permissionBusiness.CheckMenu(UserInfo, (int)PagesModel.TRS);
            /*****************/
            var canAccessVCW = _permissionBusiness.CheckMenu(UserInfo, (int)PagesModel.VCW);
            var canAccessADE = _permissionBusiness.CheckMenu(UserInfo, (int)PagesModel.ADE);
            var canAccessAssessment = _permissionBusiness.CheckMenu(UserInfo, (int)PagesModel.Assessment);
            var canAccessCAC = _permissionBusiness.CheckMenu(UserInfo, (int)PagesModel.CAC);
            var canAccessLMS = _permissionBusiness.CheckMenu(UserInfo, (int)PagesModel.LMS);
            var canAccessLDE = _permissionBusiness.CheckMenu(UserInfo, (int)PagesModel.LDE);
            var canAccessBUK = _permissionBusiness.CheckMenu(UserInfo, (int)PagesModel.BulkUpload);
   
            var canAccessTCD = _permissionBusiness.CheckMenu(UserInfo, (int)PagesModel.TCD);
            ViewBag.Practice = _permissionBusiness.CheckMenu(UserInfo, (int) PagesModel.Practice);
            ViewBag.TSDS = _permissionBusiness.CheckMenu(UserInfo, (int)PagesModel.TSDS);



            if (UserInfo.Role > Role.Mentor_coach)
            {
                int trsId = (int)LocalAssessment.TexasRisingStar;
                int vcwId = (int)LocalAssessment.CollaborativeTools;
                int adeId = (int)LocalAssessment.ADE;
                int cacId = (int)LocalAssessment.CIRCLEActivityCollection;
                int lmsId = (int)LocalAssessment.OnlineCourses;

                if (UserInfo.Role == Role.Teacher)
                    canAccessVCW = canAccessVCW && _permissionBusiness.CheckMenu(UserInfo, (int)PagesModel.Teachers);
                else
                    canAccessVCW = canAccessVCW && _permissionBusiness.CheckMenu(UserInfo, (int)PagesModel.Coach);

                switch (UserInfo.Role)
                {
                    case Role.Statewide:
                        canAccessTrs = canAccessTrs && UserInfo.UserCommunitySchools.Where(o => o.CommunityId > 0)
                            .Any(r => (r.Community.CommunityAssessmentRelations.Where(a => a.Isrequest == false).Select(a => a.AssessmentId).Contains(trsId)));

                        canAccessVCW = canAccessVCW && UserInfo.UserCommunitySchools.Where(o => o.CommunityId > 0)
                            .Any(r => (r.Community.CommunityAssessmentRelations.Where(a => a.Isrequest == false).Select(a => a.AssessmentId).Contains(vcwId)));

                        canAccessADE = canAccessADE && UserInfo.UserCommunitySchools.Where(o => o.CommunityId > 0)
                            .Any(r => (r.Community.CommunityAssessmentRelations.Where(a => a.Isrequest == false).Select(a => a.AssessmentId).Contains(adeId)));

                        canAccessAssessment = canAccessAssessment &&
                                              UserInfo.UserCommunitySchools.Where(e => e.CommunityId > 0)
                                                  .Any(s => s.Community.CommunityAssessmentRelations.Any(t => t.AssessmentId > 0));

                        canAccessCAC = canAccessCAC && UserInfo.UserCommunitySchools.Where(o => o.CommunityId > 0)
                            .Any(r => (r.Community.CommunityAssessmentRelations.Where(a => a.Isrequest == false).Select(a => a.AssessmentId).Contains(cacId)));

                        canAccessLMS = canAccessLMS && UserInfo.UserCommunitySchools.Where(o => o.CommunityId > 0)
                            .Any(r => (r.Community.CommunityAssessmentRelations.Where(a => a.Isrequest == false).Select(a => a.AssessmentId).Contains(lmsId)));
                        break;
                    case Role.Community:
                    case Role.District_Community_Specialist:
                        //所属school关联的任何一个community有trs feature时，即可查看
                        canAccessTrs = canAccessTrs &&
                                       UserInfo.UserCommunitySchools.Any(
                                           e =>
                                               e.Community.CommunityAssessmentRelations.Where(a => a.Isrequest == false)
                                                   .Select(a => a.AssessmentId)
                                                   .Contains(trsId));

                        canAccessVCW = canAccessVCW && UserInfo.UserCommunitySchools.Where(o => o.CommunityId > 0)
                            .Any(r => (r.Community.CommunityAssessmentRelations.Where(a => a.Isrequest == false).Select(a => a.AssessmentId).Contains(vcwId)));

                        canAccessADE = canAccessADE && UserInfo.UserCommunitySchools.Where(o => o.CommunityId > 0)
                            .Any(r => (r.Community.CommunityAssessmentRelations.Where(a => a.Isrequest == false).Select(a => a.AssessmentId).Contains(adeId)));

                        canAccessAssessment = canAccessAssessment &&
                                              UserInfo.UserCommunitySchools.Where(e => e.CommunityId > 0)
                                                  .Any(s => s.Community.CommunityAssessmentRelations.Any(t => t.AssessmentId > 0));

                        canAccessCAC = canAccessCAC && UserInfo.UserCommunitySchools.Where(o => o.CommunityId > 0)
                            .Any(r => (r.Community.CommunityAssessmentRelations.Where(a => a.Isrequest == false).Select(a => a.AssessmentId).Contains(cacId)));

                        canAccessLMS = canAccessLMS && UserInfo.UserCommunitySchools.Where(o => o.CommunityId > 0)
                            .Any(r => (r.Community.CommunityAssessmentRelations.Where(a => a.Isrequest == false).Select(a => a.AssessmentId).Contains(lmsId)));
                        break;
                    case Role.District_Community_Delegate:
                    case Role.Community_Specialist_Delegate:
                        //所属school关联的任何一个community有trs feature时，即可查看
                        var parentCommunityUser = _userBusiness.GetUser(UserInfo.CommunityUser.ParentId);
                        canAccessTrs = canAccessTrs && parentCommunityUser.UserCommunitySchools.Where(o => o.CommunityId > 0)
                            .Any(x => x.Community.CommunitySchoolRelations.
                                Any(y => y.School.CommunitySchoolRelations.
                                    Any(z => z.Community.CommunityAssessmentRelations.Where(a => a.Isrequest == false).Select(a => a.AssessmentId).Contains(trsId))));

                        canAccessVCW = canAccessVCW && parentCommunityUser.UserCommunitySchools.Where(o => o.CommunityId > 0)
                            .Any(r => (r.Community.CommunityAssessmentRelations.Where(a => a.Isrequest == false).Select(a => a.AssessmentId).Contains(vcwId)));

                        canAccessADE = canAccessADE && parentCommunityUser.UserCommunitySchools.Where(o => o.CommunityId > 0)
                            .Any(r => (r.Community.CommunityAssessmentRelations.Where(a => a.Isrequest == false).Select(a => a.AssessmentId).Contains(adeId)));

                        canAccessAssessment = canAccessAssessment &&
                                              parentCommunityUser.UserCommunitySchools.Where(e => e.CommunityId > 0)
                                                  .Any(s => s.Community.CommunityAssessmentRelations.Any(t => t.AssessmentId > 0));

                        canAccessCAC = canAccessCAC && parentCommunityUser.UserCommunitySchools.Where(o => o.CommunityId > 0)
                            .Any(r => (r.Community.CommunityAssessmentRelations.Where(a => a.Isrequest == false).Select(a => a.AssessmentId).Contains(cacId)));

                        canAccessLMS = canAccessLMS && parentCommunityUser.UserCommunitySchools.Where(o => o.CommunityId > 0)
                            .Any(r => (r.Community.CommunityAssessmentRelations.Where(a => a.Isrequest == false).Select(a => a.AssessmentId).Contains(lmsId)));
                        break;
                    case Role.Principal:
                    case Role.School_Specialist:
                    case Role.TRS_Specialist:
                        canAccessTrs = canAccessTrs && UserInfo.UserCommunitySchools.Where(o => o.SchoolId > 0)
                            .Any(e => e.School.CommunitySchoolRelations
                                .Any(n => (n.Community.CommunityAssessmentRelations.Where(a => a.Isrequest == false).Select(o => o.AssessmentId).Contains(trsId))));

                        canAccessVCW = canAccessVCW && UserInfo.UserCommunitySchools.Where(o => o.SchoolId > 0)
                            .Any(e => e.School.CommunitySchoolRelations
                                .Any(n => (n.Community.CommunityAssessmentRelations.Where(a => a.Isrequest == false).Select(o => o.AssessmentId).Contains(vcwId))));

                        canAccessADE = canAccessADE && UserInfo.UserCommunitySchools.Where(o => o.SchoolId > 0)
                            .Any(e => e.School.CommunitySchoolRelations
                                .Any(n => (n.Community.CommunityAssessmentRelations.Where(a => a.Isrequest == false).Select(o => o.AssessmentId).Contains(adeId))));

                        canAccessAssessment = canAccessAssessment &&
                                              UserInfo.UserCommunitySchools.Where(e => e.SchoolId > 0)
                                              .Any(e => e.School.CommunitySchoolRelations
                                                  .Any(s => s.Community.CommunityAssessmentRelations.Any(t => t.AssessmentId > 0)));

                        canAccessCAC = canAccessCAC && UserInfo.UserCommunitySchools.Where(o => o.SchoolId > 0)
                            .Any(e => e.School.CommunitySchoolRelations
                                .Any(n => (n.Community.CommunityAssessmentRelations.Where(a => a.Isrequest == false).Select(o => o.AssessmentId).Contains(cacId))));

                        canAccessLMS = canAccessLMS && UserInfo.UserCommunitySchools.Where(o => o.SchoolId > 0)
                            .Any(e => e.School.CommunitySchoolRelations
                                .Any(n => (n.Community.CommunityAssessmentRelations.Where(a => a.Isrequest == false).Select(o => o.AssessmentId).Contains(lmsId))));
                        break;
                    case Role.Principal_Delegate:
                    case Role.School_Specialist_Delegate:
                    case Role.TRS_Specialist_Delegate:
                        var parentPrincipalUser = _userBusiness.GetUser(UserInfo.Principal.ParentId);
                        canAccessTrs = canAccessTrs && parentPrincipalUser.UserCommunitySchools.Where(o => o.SchoolId > 0)
                            .Any(e => e.School.CommunitySchoolRelations
                                .Any(n => (n.Community.CommunityAssessmentRelations.Where(a => a.Isrequest == false).Select(o => o.AssessmentId).Contains(trsId))));

                        canAccessVCW = canAccessVCW && parentPrincipalUser.UserCommunitySchools.Where(o => o.SchoolId > 0)
                            .Any(e => e.School.CommunitySchoolRelations
                                .Any(n => (n.Community.CommunityAssessmentRelations.Where(a => a.Isrequest == false).Select(o => o.AssessmentId).Contains(vcwId))));

                        canAccessADE = canAccessADE && parentPrincipalUser.UserCommunitySchools.Where(o => o.SchoolId > 0)
                            .Any(e => e.School.CommunitySchoolRelations
                                .Any(n => (n.Community.CommunityAssessmentRelations.Where(a => a.Isrequest == false).Select(o => o.AssessmentId).Contains(adeId))));

                        canAccessAssessment = canAccessAssessment &&
                                              parentPrincipalUser.UserCommunitySchools.Where(e => e.SchoolId > 0)
                                              .Any(e => e.School.CommunitySchoolRelations
                                                  .Any(s => s.Community.CommunityAssessmentRelations.Any(t => t.AssessmentId > 0)));

                        canAccessCAC = canAccessCAC && parentPrincipalUser.UserCommunitySchools.Where(o => o.SchoolId > 0)
                            .Any(e => e.School.CommunitySchoolRelations
                                .Any(n => (n.Community.CommunityAssessmentRelations.Where(a => a.Isrequest == false).Select(o => o.AssessmentId).Contains(cacId))));

                        canAccessLMS = canAccessLMS && parentPrincipalUser.UserCommunitySchools.Where(o => o.SchoolId > 0)
                            .Any(e => e.School.CommunitySchoolRelations
                                .Any(n => (n.Community.CommunityAssessmentRelations.Where(a => a.Isrequest == false).Select(o => o.AssessmentId).Contains(lmsId))));
                        break;
                    case Role.Teacher:
                        canAccessTrs = false;
                        canAccessVCW = canAccessVCW && UserInfo.UserCommunitySchools.Where(o => o.SchoolId > 0)
                            .Any(e => e.School.CommunitySchoolRelations
                                .Any(n => (n.Community.CommunityAssessmentRelations.Where(a => a.Isrequest == false).Select(o => o.AssessmentId).Contains(vcwId))));

                        canAccessADE = canAccessADE && UserInfo.UserCommunitySchools.Where(o => o.SchoolId > 0)
                            .Any(e => e.School.CommunitySchoolRelations
                                .Any(n => (n.Community.CommunityAssessmentRelations.Where(a => a.Isrequest == false).Select(o => o.AssessmentId).Contains(adeId))));

                        canAccessAssessment = canAccessAssessment &&
                                              (UserInfo.UserCommunitySchools.Where(e => e.SchoolId > 0)
                                              .Any(e => e.School.CommunitySchoolRelations
                                                  .Any(s => s.Community.CommunityAssessmentRelations.Any(t => t.AssessmentId > 0)))
                                                  ||
                                                  UserInfo.UserCommunitySchools.Where(e => e.CommunityId > 0)
                                                  .Any(s => s.Community.CommunityAssessmentRelations.Any(t => t.AssessmentId > 0)));

                        canAccessCAC = canAccessCAC && UserInfo.UserCommunitySchools.Where(o => o.SchoolId > 0)
                            .Any(e => e.School.CommunitySchoolRelations
                                .Any(n => (n.Community.CommunityAssessmentRelations.Where(a => a.Isrequest == false).Select(o => o.AssessmentId).Contains(cacId))));

                        canAccessLMS = canAccessLMS && UserInfo.UserCommunitySchools.Where(o => o.SchoolId > 0)
                            .Any(e => e.School.CommunitySchoolRelations
                                .Any(n => (n.Community.CommunityAssessmentRelations.Where(a => a.Isrequest == false).Select(o => o.AssessmentId).Contains(lmsId))));

                        break;
                    default:
                        canAccessTrs = false;
                        break;
                }
            }
            ViewBag.Trs = canAccessTrs;

            ViewBag.Administrative = _permissionBusiness.CheckMenu(UserInfo, (int)PagesModel.Administrative);
            ViewBag.AdministrativeUrl = "Community/Community/Index";

            ViewBag.ADE = canAccessADE;
            ViewBag.Assessment = canAccessAssessment;
            ViewBag.LMS = canAccessLMS;
            ViewBag.LMSUrl = GeneralLmsUrl();
            ViewBag.CAC = canAccessCAC;
            ViewBag.CACUrl = "?key=" + Server.UrlEncode(Encrypt("CLIENGAGECAC" + "_" + UserInfo.ID + "_" + DateTime.Now.ToString("yyyyMMddhhmmss")));
            ViewBag.Vcw = canAccessVCW;
            ViewBag.LDE = canAccessLDE;
            ViewBag.BUK = canAccessBUK;

            ViewBag.TCD = (canAccessTCD && UserInfo.Role == Role.Parent);

            /*******************/

            if (_permissionBusiness.CheckMenu(UserInfo, (int)PagesModel.Administrative))
            {
                UserAuthorityModel userAuthorityModel = _permissionBusiness.GetUserAuthority(UserInfo, (int)PagesModel.Community_Management);
                if (userAuthorityModel == null)//没有Community Management
                    GetAdminUrl();
                else
                {
                    if (!((userAuthorityModel.Authority & (int)Authority.Index) == (int)Authority.Index)) //没有Community的Index权限
                        GetAdminUrl();
                }
            }
            ViewBag.ShowCommunityNote = false;

            if (UserInfo.Role > Role.Mentor_coach && UserInfo.Role != Role.Parent)
            {
                bool show = _communityBusiness.IsShowCommunityNotes(UserInfo);
                if (show)
                {
                    string url = HttpContext.Request.Url.ToString().ToLower();
                    if (url.Contains("sign-in"))
                        show = true;
                    else
                        show = false;
                }
                ViewBag.ShowCommunityNote = show;
            }

            var total = 0;
            ViewBag.SystemUpdates = _updateClusterBusiness.SearchSystemUpdates("UpdatedOn", "DESC", 0, 3, out total);
            ViewBag.MessageCenters = _updateClusterBusiness.SearchMessageCenters("UpdatedOn", "DESC", 0, 3, out total);
            ViewBag.NewFeatureds = _updateClusterBusiness.SearchNewFeatureds("UpdatedOn", "DESC", 0, 3, out total);


            List<int> accountPageId = new PermissionBusiness().CheckPage(UserInfo);
            List<int> pageIds = accountPageId.FindAll(r => r > SFConfig.AssessmentPageStartId);
            List<int> featureAssessmentIds = _communityBusiness.GetFeatureAssessmentIds(UserInfo);
            List<CpallsAssessmentModel> list = _adeBusiness.GetAssessmentCpallsList();
            List<CpallsAssessmentModel> accessList = list.FindAll(r => pageIds.Contains(r.ID + SFConfig.AssessmentPageStartId));
            if (UserInfo.Role > Role.Mentor_coach)
                accessList = accessList.FindAll(r => featureAssessmentIds.Contains(r.ID));

            List<CpallsAssessmentModel> English = accessList.FindAll(r => r.Language == AssessmentLanguage.English);
            List<CpallsAssessmentModel> newList = new List<CpallsAssessmentModel>();

            foreach (CpallsAssessmentModel model in accessList)
            {
                if (model.Language == AssessmentLanguage.Spanish)
                {
                    if (English.Find(r => r.Name == model.Name) != null)
                        continue;
                }
                newList.Add(model);
            }
            ViewBag.List = newList.ToList();

            //cec
            List<CpallsAssessmentModel> cecList = _adeBusiness.GetAssessmentList(o => o.Type == AssessmentType.Cec);
            List<CpallsAssessmentModel> accessCECList = cecList.FindAll(r => pageIds.Contains(r.ID + SFConfig.AssessmentPageStartId));
            if (UserInfo.Role > Role.Mentor_coach)
                accessCECList = accessCECList.FindAll(r => featureAssessmentIds.Contains(r.ID));
            ViewBag.CECList = accessCECList;

            //cot
            List<CpallsAssessmentModel> cotList = _adeBusiness.GetAssessmentList(o => o.Type == AssessmentType.Cot);
            List<CpallsAssessmentModel> accessCOTList = cotList.FindAll(r => pageIds.Contains(r.ID + SFConfig.AssessmentPageStartId));
            if (UserInfo.Role > Role.Mentor_coach)
                accessCOTList = accessCOTList.FindAll(r => featureAssessmentIds.Contains(r.ID));
            ViewBag.COTList = accessCOTList;

            //Observable
            List<CpallsAssessmentModel> obsevList = _adeBusiness.GetAssessmentList(o => o.Type == AssessmentType.UpdateObservables);
            List<CpallsAssessmentModel> accessTCDList = obsevList.FindAll(r => pageIds.Contains(r.ID + SFConfig.AssessmentPageStartId));
            ViewBag.TCDList = accessTCDList;

            ViewBag.ShowCPALLS = accountPageId.Contains((int)PagesModel.CPALLS);
            ViewBag.ShowCOT = accountPageId.Contains((int)PagesModel.COT);
            ViewBag.ShowCEC = accountPageId.Contains((int)PagesModel.CEC);
            ViewBag.ShowTCD = accountPageId.Contains((int)PagesModel.TCD);
            return View();
        }

        public ActionResult ParentDashboard()
        {
            List<int> allPageIds = _permissionBusiness.CheckPage(UserInfo);
            List<PageModel> pageModel = _permissionBusiness.GetChildPagesList(allPageIds, (int)PagesModel.ParentFeature);
            ViewBag.ParentFeatures = pageModel;

            ViewBag.ShowCommunityNote = false;
            var canAccessLMS = _permissionBusiness.CheckMenu(UserInfo, (int)PagesModel.LMS);
            int lmsId = (int)LocalAssessment.OnlineCourses;
            canAccessLMS = canAccessLMS && UserInfo.UserCommunitySchools.Where(o => o.CommunityId > 0)
                .Any(r => (r.Community.CommunityAssessmentRelations.Where(a => a.Isrequest == false).Select(a => a.AssessmentId).Contains(lmsId)));
            ViewBag.LMSUrl = GeneralLmsUrl();
            ViewBag.LMS = canAccessLMS;

            if (UserInfo.Role == Role.Parent)
            {
                bool show = _communityBusiness.IsShowCommunityNotes(UserInfo);
                if (show)
                {
                    string url = HttpContext.Request.Url.ToString().ToLower();
                    if (url.Contains("sign-in"))
                        show = true;
                    else
                        show = false;
                }
                ViewBag.ShowCommunityNote = show;
            }
            return View();
        }


        private void GetAdminUrl()
        {
            List<PageModel> pageList = _permissionBusiness.GetPagesByMenuId((int)PagesModel.Administrative);
            UserAuthorityModel userDefautlAuthority =
                _permissionBusiness.GetUserAuthority(UserInfo, pageList.Where(r => r.ID != (int)PagesModel.Community_Management && r.ID != (int)PagesModel.Dashboard)
                .Select(r => r.ID).ToList());
            if (userDefautlAuthority == null)
                ViewBag.AdministrativeUrl = "/Profile/MyProfile/";
            else
            {
                PageModel defatuPageModel = pageList.Find(r => r.ID == userDefautlAuthority.PageId);
                ViewBag.AdministrativeUrl = defatuPageModel.Url;
            }
        }


        [Route("~/Signup")]
        public ActionResult Signup2()
        {
            return new RedirectResult("/public/about-us/registration/"); //David 02/24/2017 Requested by Walker, Steven T
            //if (UserAuthentication != AuthenticationStatus.Login)
            //{
            //    ViewBag.LoginUrl = BuilderLoginUrl();
            //    ViewBag.CLIUserLogin = BuilderLoginUrl(LoginUserType.UTACCESSMANAGER);
            //}
            //return View();
        }

        [Route("~/home/Signin")]
        public ActionResult Signin()
        {
            string ip = Request.UserHostAddress;
            string ip2 = HttpContext.Request.UserHostAddress;

            if (UserAuthentication != AuthenticationStatus.Login)
            {
                ViewBag.LoginUrl = BuilderLoginUrl();
                ViewBag.CLIUserLogin = BuilderLoginUrl(LoginUserType.UTACCESSMANAGER);
            }
            else
            {
                return new RedirectResult("/home/Dashboard");//David 10/14/2016
            }

            return View();
        }



        public ActionResult TestLogin(int id)
        {
            var configExpiredDate = SFConfig.ExpiredDate;
            var expiredTime = DateTime.MinValue;
            DateTime.TryParse(configExpiredDate, out expiredTime);

            if (DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd 00:00:00")) > expiredTime)
            {
                ViewBag.Show = true;
                ViewBag.Msg = "The link has been expired.";
                return View();
            }

            ViewBag.Show = false;
            UserBaseEntity user = _userBusiness.GetUser(id);
            ViewBag.Msg = string.Empty;
            ViewBag.Url = string.Empty;
            var communityIds = SFConfig.TestingCommunities;

            if (user == null)
                ViewBag.Msg = "The user doesn't exist";
            else
            {
                ViewBag.Msg = "The user is not in the target testing community.";

                if (!string.IsNullOrEmpty(communityIds))
                {
                    List<string> tempList = communityIds.Split(',').ToList();
                    List<int> listComId = new List<int>();
                    foreach (var temp in tempList)
                    {
                        var comId = -1;
                        if (int.TryParse(temp, out comId))
                        {
                            if (comId > -1)
                                listComId.Add(comId);
                        }
                    }
                    var canLogin = false;
                    switch (user.Role)
                    {
                        case Role.Principal:
                        case Role.TRS_Specialist:
                        case Role.School_Specialist:
                            canLogin =
                                user.UserCommunitySchools.Any(
                                    c => c.School.CommunitySchoolRelations.Any(e => listComId.Contains(e.CommunityId)));
                            break;
                        case Role.Principal_Delegate:
                        case Role.TRS_Specialist_Delegate:
                        case Role.School_Specialist_Delegate:
                            UserBaseEntity parentPrincipalUser = _userBusiness.GetUser(user.Principal.ParentId);
                            canLogin =
                                parentPrincipalUser.UserCommunitySchools.Any(
                                    c => c.School.CommunitySchoolRelations.Any(e => listComId.Contains(e.CommunityId)));
                            break;
                        case Role.District_Community_Delegate:
                        case Role.Community_Specialist_Delegate:
                            UserBaseEntity parentCommunityUser = _userBusiness.GetUser(user.Principal.ParentId);
                            canLogin = parentCommunityUser.UserCommunitySchools.Any(c => listComId.Contains(c.CommunityId));
                            break;
                        default:
                            canLogin = user.UserCommunitySchools.Any(c => listComId.Contains(c.CommunityId));
                            break;
                    }
                    if (canLogin)
                    {
                        if (user.GoogleId != string.Empty)
                        {
                            var returnUrl = string.Format("{0}home/LoginUser2?key={1}", DomainHelper.SsoSiteDomain.ToString(), System.Web.HttpContext.Current.Server.UrlEncode(user.GoogleId));
                            return new RedirectResult(returnUrl);
                        }
                        else
                        {
                            InviteSign(id);
                            var returnUrl = string.Format("{0}home/LoginUser2?key={1}", DomainHelper.SsoSiteDomain.ToString(), System.Web.HttpContext.Current.Server.UrlEncode("TestLogin" + user.ID));
                            return new RedirectResult(returnUrl);
                        }
                    }
                }
            }
            ViewBag.Show = true;

            return View();
        }


        /// <summary>
        /// QA专用
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Invite(int id)
        {
            ViewBag.Show = false;
            string tmpIp = CommonHelper.GetIPAddress(Request);

            if (tmpIp == "127.0.0.1" || tmpIp.StartsWith("129.106.") || tmpIp == "73.155.32.201" || tmpIp.StartsWith("192.168.1."))
            {
                UserBaseEntity user = _userBusiness.GetUser(id);
                ViewBag.Msg = string.Empty;
                ViewBag.Url = string.Empty;
                if (user == null)
                    ViewBag.Msg = "The ID  does not exist";
                else
                {
                    if (user.GoogleId != string.Empty)
                    {
                        ViewBag.Msg = "The ID has registered";
                        ViewBag.Url = string.Format("{0}home/LoginUser?key={1}", DomainHelper.SsoSiteDomain.ToString(), System.Web.HttpContext.Current.Server.UrlEncode(user.GoogleId));
                    }
                    else
                    {
                        InviteSign(id);
                        ViewBag.Url = string.Format("{0}home/LoginUser?key=", DomainHelper.SsoSiteDomain.ToString());
                    }
                }
                ViewBag.Show = true;
            }
            return View();
        }

        /// <summary>
        /// 验证用户收到的 邀请链接
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult InviteVerification(string id)
        {
            string param = ObjectFactory.GetInstance<IEncrypt>().Decrypt(id);
            int mainExpirationTime = Convert.ToInt32(SFConfig.ExpirationTime);
            string userIdValue = param.Split(',')[0];
            string expirationTimeValue = param.Split(',')[1];
            ViewBag.HasActived = false;

            if (DateTime.Now < DateTime.Parse(expirationTimeValue).AddDays(mainExpirationTime))
            {
                UserBaseEntity user = _userBusiness.GetUser(Convert.ToInt32(userIdValue));
                if (user != null)
                {
                    if (user.Status == EntityStatus.Active && !user.IsDeleted)
                    {
                        if (user.GoogleId == string.Empty)
                        {
                            InviteSign(user.ID);

                            if (SFConfig.EnableAccessManager == "1")
                                return new RedirectResult(SFConfig.SsoDomain + "Home/Index?" + BuilderLoginUrl());
                            else
                                return new RedirectResult(string.Format("/home/Invite/{0}", user.ID));
                        }
                        else
                        {
                            ///已绑定了账号，提示用户，可以直接登录了
                            ViewBag.LoginUrl = BuilderLoginUrl(LoginUserType.UTACCESSMANAGER);
                            ViewBag.HasActived = true;
                        }
                    }
                    else
                    {
                        return new RedirectResult("/home/Invalidaccount");
                    }
                }
            }
            return View();
        }

        /// <summary>
        /// Your Engage account is inactive.
        /// </summary>
        /// <returns></returns>
        public ActionResult Invalidaccount()
        {
            return View();
        }

        public ActionResult MoodlePerPageAutoLogin()
        {
            if (UserInfo == null)
                return new RedirectResult("/Home/Index");
            else
            {
                if (_permissionBusiness.CheckMenu(UserInfo, (int)PagesModel.LMS))
                    return new RedirectResult(GeneralLmsUrl());
                else
                    return new RedirectResult("/Error/nonauthorized");
            }
        }

        public string GetInfoForWordPress(string callbackparam)
        {
            string googleId;
            int userId;
            string firstName;
            Authentication.ValidateGlobalCookie(out userId, out googleId, out firstName);
            if (userId > 0)
            {

                return callbackparam + "(" + JsonHelper.SerializeObject(firstName) + ")";
            }
            return callbackparam;
        }

        [Route("~/Approve/{id}")]
        public ActionResult Approve(string id)
        {
            try
            {
                var link = LinkModel.Parse(_encrypt.Decrypt(id));
                var inviter = link.Sender;
                var recipientUserId = link.Recipient;
                if (inviter < 1 || recipientUserId < 1) return Redirect("/Error/Error");
                var result = new OperationResult(OperationResultType.Success);
                var expireTime = link.CreatedOn.AddDays(Convert.ToInt32(SFConfig.ExpirationTime));
                if (DateTime.Now > expireTime)
                {
                    ViewBag.Approve = false;
                    return View();
                }
                var communityId = 0;
                var schoolId = 0;
                var roleType = link.RoleType;
                switch (roleType)
                {
                    case Role.Community:
                    case Role.District_Community_Specialist:
                        communityId = Convert.ToInt32(link.Others["CommunityId"]);
                        break;
                    case Role.Principal:
                    case Role.TRS_Specialist:
                        schoolId = Convert.ToInt32(link.Others["SchoolId"]);
                        break;
                    case Role.Teacher:
                        communityId = Convert.ToInt32(link.Others["CommunityId"]);
                        schoolId = Convert.ToInt32(link.Others["SchoolId"]);
                        break;
                }
                StatusTrackingEntity statusTrackingEntity = statusTrackingBusiness.GetExistingTracking(inviter,
                    recipientUserId, communityId, schoolId);
                if (statusTrackingEntity == null)
                {
                    ViewBag.Approve = false;
                    return View();
                }
                else
                {
                    if (statusTrackingEntity.Status == StatusEnum.Pending)
                    {
                        statusTrackingBusiness.Approve(statusTrackingEntity.ID, recipientUserId);
                        UserBaseEntity recipientUser = _userBusiness.GetUser(recipientUserId);
                        if (
                            !(recipientUser.UserCommunitySchools.Any(
                                e => e.CommunityId == communityId && e.SchoolId == schoolId)))
                            result = _userBusiness.InsertUserCommunitySchoolRelations(recipientUserId, inviter,
                                communityId, schoolId);


                        var sender = _userBusiness.GetUser(link.Sender);
                        var recipient = _userBusiness.GetUser(link.Recipient);
                        if (result.ResultType == OperationResultType.Success && sender != null && recipient != null)
                        {
                            EmailTemplete template =
                                XmlHelper.GetEmailTemplete("NoPermission_Invite_Result_Template.xml");
                            string emailBody = template.Body.Replace("{StaticDomain}", SFConfig.StaticDomain)
                                .Replace("{Sender}", sender.FirstName)
                                .Replace("{Recipient}", recipient.FirstName + " " + recipient.LastName)
                                .Replace("{Result}", "accepted");
                            string subject = template.Subject.Replace("{Result}", "accepted");
                            _userBusiness.SendEmail(sender.PrimaryEmailAddress, subject, emailBody);
                        }
                        ViewBag.Approve = true;
                    }
                    else
                    {
                        ViewBag.Approve = false;
                    }
                    return View();
                }
            }
            catch
            {
                return Redirect("/Error/Error");
            }
        }

        [Route("~/Deny/{id}")]
        public ActionResult Deny(string id)
        {
            var link = LinkModel.Parse(_encrypt.Decrypt(id));
            var inviter = link.Sender;
            var recipientUserId = link.Recipient;
            if (inviter < 1 || recipientUserId < 1) return Redirect("/Error/Error");
            var sender = _userBusiness.GetUser(link.Sender);
            var recipient = _userBusiness.GetUser(link.Recipient);
            var result = new OperationResult(OperationResultType.Success);
            var expireTime = link.CreatedOn.AddDays(Convert.ToInt32(SFConfig.ExpirationTime));
            if (DateTime.Now > expireTime)
            {
                ViewBag.Deny = false;
                return View();
            }
            var communityId = 0;
            var schoolId = 0;
            var roleType = link.RoleType;
            switch (roleType)
            {
                case Role.Community:
                case Role.District_Community_Specialist:
                    communityId = Convert.ToInt32(link.Others["CommunityId"]);
                    break;
                case Role.Principal:
                case Role.TRS_Specialist:
                    schoolId = Convert.ToInt32(link.Others["SchoolId"]);
                    break;
                case Role.Teacher:
                    communityId = Convert.ToInt32(link.Others["CommunityId"]);
                    schoolId = Convert.ToInt32(link.Others["SchoolId"]);
                    break;
            }
            StatusTrackingEntity statusTrackingEntity = statusTrackingBusiness.GetExistingTracking(inviter,
                recipientUserId, communityId, schoolId);
            if (statusTrackingEntity == null)
            {
                ViewBag.Deny = false;
                return View();
            }
            else
            {
                if (statusTrackingEntity.Status == StatusEnum.Pending)
                {
                    statusTrackingBusiness.Deny(statusTrackingEntity.ID, recipientUserId);

                    EmailTemplete template = XmlHelper.GetEmailTemplete("NoPermission_Invite_Result_Template.xml");
                    string emailBody = template.Body.Replace("{StaticDomain}", SFConfig.StaticDomain)
                        .Replace("{Sender}", sender.FirstName)
                        .Replace("{Recipient}", recipient.FirstName + " " + recipient.LastName)
                        .Replace("{Result}", "declined");
                    string subject = template.Subject.Replace("{Result}", "declined");
                    _userBusiness.SendEmail(sender.PrimaryEmailAddress, subject, emailBody);
                    ViewBag.Deny = true;
                }
                else
                {
                    ViewBag.Deny = false;
                }
                return View();
            }
        }
        private string Encrypt(string source)
        {
            if (string.IsNullOrEmpty(source))
                return source;
            var encrypt = ObjectFactory.GetInstance<IEncrypt>();
            return encrypt.Encrypt(source);
        }

        private string GeneralLmsUrl()
        {
            string lmsurl = string.Format(
                "{0}/auth/cliauth/cli_redirect.php?clirole={1}&useremail={2}&firstname={3}&lastname={4}&userid={5}&status={6}&roletext={7}",
                DomainHelper.LMSDomain, _encrypt.Encrypt(((byte)UserInfo.Role).ToString()),
                _encrypt.Encrypt(UserInfo.PrimaryEmailAddress),
                _encrypt.Encrypt(UserInfo.FirstName),
                _encrypt.Encrypt(UserInfo.LastName),
                _encrypt.Encrypt(UserInfo.ID.ToString()),
                _encrypt.Encrypt(UserInfo.Status.ToString()),
                _encrypt.Encrypt(UserInfo.Role.ToDescription()));

            if (UserInfo.Role == Role.Community || UserInfo.Role == Role.District_Community_Delegate
                || UserInfo.Role == Role.District_Community_Specialist ||
                UserInfo.Role == Role.Community_Specialist_Delegate)
            {
                lmsurl = lmsurl +
                         string.Format("&objectId={0}", _encrypt.Encrypt(UserInfo.CommunityUser.CommunityUserId.ToLower()));
            }
            else if (UserInfo.Role == Role.Principal || UserInfo.Role == Role.Principal_Delegate
                     || UserInfo.Role == Role.TRS_Specialist || UserInfo.Role == Role.TRS_Specialist_Delegate
                     || UserInfo.Role == Role.School_Specialist || UserInfo.Role == Role.School_Specialist_Delegate)
            {
                lmsurl = lmsurl + string.Format("&objectId={0}", _encrypt.Encrypt(UserInfo.Principal.PrincipalId.ToLower()));
            }
            else if (UserInfo.Role == Role.Teacher)
            {
                lmsurl = lmsurl +
                         string.Format("&objectId={0}", _encrypt.Encrypt(UserInfo.TeacherInfo.TeacherId.ToLower()));
            }
            return lmsurl;
        }


        public string GenerateCacUrl(string targetUrl)
        {
            string urlStr = "";
            string paramStrs = "";
            CACUser cacUser = new CACUser();
            cacUser.LoginName = UserInfo.FirstName + UserInfo.ID;
            cacUser.FirstName = UserInfo.FirstName;
            cacUser.LastName = UserInfo.LastName;
            cacUser.Email = UserInfo.Gmail;
            cacUser.TimeNow = DateTime.Now;
            cacUser.CliUserId = Encrypt(UserInfo.ID.ToString());
            cacUser.RoleName = UserInfo.Role.ToString();
            paramStrs = "?key=" + Encrypt(HttpUtility.UrlEncode(JsonConvert.SerializeObject(cacUser)));
            paramStrs+= "&targetUrl=" + targetUrl;
            //paramStrs += "&targetUrl=" + DomainHelper.CACDomain.AbsoluteUri;

            urlStr = Path.Combine(DomainHelper.CACDomain.AbsoluteUri, paramStrs);
            return urlStr;
        }

        /// <summary>
        /// moodle站点使用
        /// 查找每个用户对应的Community下的所有用户
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="callbackparam"></param>
        /// <returns></returns>
        public string GetAllUserIds(int userId, string callbackparam)
        {
            List<int> userIds = _userBusiness.GetSubUserByGoogleId(UserInfo.Role, userId);
            return callbackparam + "(" + JsonHelper.SerializeObject(userIds) + ")";
        }


        public ActionResult GoogleAccount(string e = "")
        {
            string email = "example@gmail.com";
            if (e != string.Empty)
            {
                email = _encrypt.Decrypt(e);
            }
            ViewBag.Email = email == string.Empty ? "example@gmail.com" : email;

            return View();
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {

        }
    }

    internal class CACUser
    {
        public string CliUserId { get; set; }
        public string FirstName { get; set; }
        public string LoginName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string RoleName { get; set; }
        public DateTime TimeNow { get; set; }
    }
}