using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Sunnet.Cli.Business.Users;
using Sunnet.Cli.Core.Users.Entities;
using Sunnet.Cli.UIBase.Models;
using Sunnet.Framework.Helpers;
using Sunnet.Framework.SSO;
using Sunnet.Cli.Core.Users.Enums;
using System.Collections;
using Sunnet.Cli.UIBase.Filters;
using Sunnet.Framework.Permission;
using Sunnet.Cli.Business.Vcw;
using Sunnet.Cli.Business.Permission.Models;
using Sunnet.Cli.Business.Permission;

namespace Sunnet.Cli.Vcw.Controllers
{
    public class HomeController : BaseController
    {
        VcwBusiness _vcwBusiness;
        PermissionBusiness _permissionBusiness;
        public HomeController()
        {
            _vcwBusiness = new VcwBusiness(VcwUnitWorkContext);
            _permissionBusiness = new PermissionBusiness();
        }
        //
        // GET: /Home/
        public ActionResult Index()
        {
            if (UserAuthentication != AuthenticationStatus.Login)
                return new RedirectResult(string.Format("{0}home/Index?{1}", DomainHelper.SsoSiteDomain, BuilderLoginUrl("4")));
            if (UserInfo.Role == Role.Teacher)
                return new RedirectResult("/home/dashboard");
            if (UserInfo.Role == Role.Mentor_coach || UserInfo.Role == Role.Coordinator)
                return new RedirectResult("/home/dashboard_coach");
            if (UserInfo.Role == Role.Intervention_manager)
                return new RedirectResult("/home/dashboard_pm");
            if (UserInfo.Role == Role.Super_admin)
                return new RedirectResult("/home/dashboard_admin");
            if (UserInfo.Role >= Role.Auditor)
                return new RedirectResult("/home/dashboard_coach");
            return new RedirectResult("/home/dashboard");
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.Teachers, Anonymity = Anonymous.Verified)]
        public ActionResult dashboard()
        {
            ViewBag.VIPCount = _vcwBusiness.GetTeacherVIPCount(UserInfo.ID);
            ViewBag.CoachingCount = _vcwBusiness.GetTeacherAssignmentCount(UserInfo.ID);
            ViewBag.TeacherVip = _permissionBusiness.GetUserAuthority(UserInfo, (int)PagesModel.TeacherVip) != null;
            ViewBag.TeacherAssignment = _permissionBusiness.GetUserAuthority(UserInfo, (int)PagesModel.TeacherAssignment) != null;
            ViewBag.TeacherGeneral = _permissionBusiness.GetUserAuthority(UserInfo, (int)PagesModel.TeacherGeneral) != null;
            ViewBag.TeacherSummary = _permissionBusiness.GetUserAuthority(UserInfo, (int)PagesModel.TeacherSummary) != null;
            ViewBag.STGReport = _permissionBusiness.GetUserAuthority(UserInfo, (int)PagesModel.STGReport) != null;
            return View();
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.Coach, Anonymity = Anonymous.Verified)]
        public ActionResult dashboard_coach()
        {
            ViewBag.CoachingCount = _vcwBusiness.GetCoachAssignmentCount(UserInfo.ID);
            ViewBag.CoachAssignment = _permissionBusiness.GetUserAuthority(UserInfo, (int)PagesModel.CoachAssignment) != null;
            ViewBag.CoachGeneral = _permissionBusiness.GetUserAuthority(UserInfo, (int)PagesModel.CoachGeneral) != null;
            ViewBag.CoachTeachers = _permissionBusiness.GetUserAuthority(UserInfo, (int)PagesModel.CoachTeachers) != null;
            ViewBag.CoachSummary = _permissionBusiness.GetUserAuthority(UserInfo, (int)PagesModel.CoachSummary) != null;
            return View();
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.PM, Anonymity = Anonymous.Verified)]
        public ActionResult dashboard_pm()
        {
            ViewBag.Teachers = _permissionBusiness.GetUserAuthority(UserInfo, (int)PagesModel.PMTeachers) != null;
            ViewBag.Coaches = _permissionBusiness.GetUserAuthority(UserInfo, (int)PagesModel.PMCoaches) != null;
            ViewBag.Summary = _permissionBusiness.GetUserAuthority(UserInfo, (int)PagesModel.PMSummary) != null;
            return View();
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.Admin, Anonymity = Anonymous.Verified)]
        public ActionResult dashboard_admin()
        {
            return View();
        }

        /// <summary>
        /// 专供ssol调用
        /// </summary>
        public ActionResult CallBack(string IASID = "", string TimeStamp = "", string Authenticator = "", string UserAccount = "")
        {
            if (!string.IsNullOrEmpty(IASID)
               && !string.IsNullOrEmpty(TimeStamp)
               && !string.IsNullOrEmpty(Authenticator)
               && !string.IsNullOrEmpty(UserAccount))
            {
                SSORequest ssoRequest = new SSORequest();
                ssoRequest.IASID = IASID;
                ssoRequest.TimeStamp = TimeStamp;
                ssoRequest.AppUrl = string.Format("{0}Home/CallBack", DomainHelper.VcwDomain.ToString());
                ssoRequest.UserAccount = UserAccount;//google Id
                ssoRequest.Authenticator = Authenticator;

                if (Authentication.ValidateCenterToken(ssoRequest))
                {
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


                    UserBaseEntity user = new UserBusiness(UnitWorkContext).UserLogin(UserAccount);
                    if (user != null)
                    {
                        LocalSignIn(user.ID, user.GoogleId, user.FirstName);
                        return new RedirectResult("/home/");
                    }
                }
            }
            return new RedirectResult(DomainHelper.MainSiteDomain.ToString());
        }

        /// <summary>
        /// 专供sso调用
        /// </summary>
        public ActionResult LogOut()
        {
            CookieHelper.RemoveAll();
            return View();
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        { }

    }
}