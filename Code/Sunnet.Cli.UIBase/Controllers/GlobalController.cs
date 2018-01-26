using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Sunnet.Cli.Core;
using Sunnet.Framework;
using Sunnet.Framework.Core.Base;
using Sunnet.Framework.Encrypt;

using StructureMap;
using Sunnet.Framework.Helpers;
using Sunnet.Cli.UIBase.Models;
using Sunnet.Cli.Core.Users.Entities;
using Sunnet.Cli.Business.Permission;
using Sunnet.Framework.SSO;
using Sunnet.Framework.Resources;

namespace Sunnet.Cli.UIBase.Controllers
{
    /// <summary>
    /// Controller基类,所有Controller引用,用来控制登录用户等公共信息
    /// </summary>
    public class GlobalController : Controller
    {
        protected AuthenticationStatus UserAuthentication = AuthenticationStatus.NotLogin;
        //protected string cookieKey = string.Empty;
        public UserBaseEntity UserInfo;
        private IEncrypt encrypt;
        public UserBaseEntity Administrator { get { return UserInfo; } }

        public string GetInformation(string key)
        {
            var r = ResourceHelper.GetRM().RmInformation.GetObject(key);
            if (r != null)
                return r.ToString();
            return "";
        }


        EFUnitOfWorkContext _unitWorkContext;
        protected EFUnitOfWorkContext UnitWorkContext
        {
            get
            {
                if (_unitWorkContext == null)
                    _unitWorkContext = new EFUnitOfWorkContext();
                return _unitWorkContext;
            }
        }

        VCWUnitOfWorkContext _vcwUnitWorkContext;
        protected VCWUnitOfWorkContext VcwUnitWorkContext
        {
            get
            {
                if (_vcwUnitWorkContext == null)
                    _vcwUnitWorkContext = new VCWUnitOfWorkContext();
                return _vcwUnitWorkContext;
            }
        }


        PracticeUnitOfWorkContext _practiceUnitWorkContext;
        protected PracticeUnitOfWorkContext PracticeUnitWorkContext
        {
            get
            {
                if (_practiceUnitWorkContext == null)
                    _practiceUnitWorkContext = new PracticeUnitOfWorkContext();
                return _practiceUnitWorkContext;
            }
        }

        AdeUnitOfWorkContext _adeUnitWorkContext;
        protected AdeUnitOfWorkContext AdeUnitWorkContext
        {
            get
            {
                if (_adeUnitWorkContext == null)
                    _adeUnitWorkContext = new AdeUnitOfWorkContext();
                return _adeUnitWorkContext;
            }
        }

        /// <summary>
        /// 生成登录URL
        /// </summary>
        /// <param name="userType">LoginUserType.UTACCESSMANAGER or  LoginUserType.GOOGLEACCOUNT</param>
        /// <param name="IASID">LoginIASID</param>
        /// <returns></returns>
        protected string BuilderLoginUrl(int userType, string IASID)
        {
            SSORequest ssoRequest = new SSORequest();
            ssoRequest.IASID = IASID;
            ssoRequest.TimeStamp = DateTime.Now.Ticks.ToString();// DateTime.Now.ToString("MM/dd/yyyy HH:mm");
            switch (IASID)
            {
                case LoginIASID.MAIN:
                case LoginIASID.ParentSign:
                case LoginIASID.LostSession:
                    ssoRequest.AppUrl = DomainHelper.Main_SITE_Backurl;
                    break;
                case LoginIASID.ASSESSMENT:
                case LoginIASID.ADE:
                case LoginIASID.CPALLS_OFFLINE:
                case LoginIASID.COT_OFFLINE:
                case LoginIASID.CEC_OFFLINE:
                case LoginIASID.TRS:
                case LoginIASID.TRS_OFFLINE:
                    ssoRequest.AppUrl = DomainHelper.Assessment_SITE_Backurl;
                    break;
                case LoginIASID.VCW:
                    ssoRequest.AppUrl = DomainHelper.Vcw_SITE_Backurl;
                    break;
                case LoginIASID.AssessmentPractice:
                    ssoRequest.AppUrl = DomainHelper.Practice_SITE_Backurl;
                    break;
            }
            Authentication.CreateAppToken(ssoRequest);

            PostService ps = new PostService();

            return string.Format("TimeStamp={0}&IASID={1}&Authenticator={2}&Admin={3}"
                , System.Web.HttpContext.Current.Server.UrlEncode(ssoRequest.TimeStamp), ssoRequest.IASID
                , System.Web.HttpContext.Current.Server.UrlEncode(ssoRequest.Authenticator), userType);
        }

        public GlobalController()
        {
            encrypt = ObjectFactory.GetInstance<IEncrypt>();
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (UserAuthentication == AuthenticationStatus.LostSession)
            {
                if (filterContext.HttpContext.Request.IsAjaxRequest())
                {
                    filterContext.HttpContext.Response.StatusCode = 403;
                    string response = string.Format("{{\"StatusCode\":403,\"ErrorMessage\":\"403-session-lost\",\"success\":false}}");
                    filterContext.Result = new ContentResult() { Content = "response" };
                }
                else
                {
                    if (filterContext.HttpContext.Request.HttpMethod.ToLower().IndexOf("post") >= 0)
                    {
                        filterContext.HttpContext.Response.StatusCode = 403;
                        string response = string.Format("{{\"StatusCode\":403,\"ErrorMessage\":\"403-session-lost\",\"success\":false}}");
                        filterContext.Result = new ContentResult() { Content = "response" };
                    }
                    else
                    {
                        filterContext.Result = Content(string.Format("<script type='text/javascript'>location='{0}home/LostSession';</script>", DomainHelper.SsoSiteDomain));
                    }
                }
            }
        }



        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
#if DEBUG
            ViewBag.Debug = true;
#endif
#if !DEBUG
            ViewBag.Debug = false;
#endif
            ViewBag.TimeoutUrl = SFConfig.SsoDomain + "home/LostSession";

            if (UserInfo != null)
            {
                ViewBag.FirstName = UserInfo.FirstName;
                ViewBag.EncryptUserID = encrypt.Encrypt(UserInfo.ID.GetHashCode().ToString());
                ///拥有的访问Page
                ViewBag.UserPages = new PermissionBusiness().CheckPage(UserInfo);
                ViewBag.UserRoleType = UserInfo.Role;
            }
            ViewBag.Login = UserAuthentication == AuthenticationStatus.Login;
            string googleId;
            int userId;
            Authentication.ValidateGlobalCookie(out userId, out googleId);
            ViewBag.GoogleID = googleId.Trim() != string.Empty;

            ViewBag.Mobile = false;
            var userAgent = filterContext.HttpContext.Request.UserAgent;
            if (userAgent != null)
            {
                userAgent = userAgent.ToLower();
                if (userAgent.ToLower().Contains("iphone"))
                {
                    ViewBag.Mobile = true;
                }
                else if (userAgent.ToLower().Contains("ipad"))
                {
                    ViewBag.Mobile = true;
                }
                else if (userAgent.ToLower().Contains("android"))
                {
                    ViewBag.Mobile = true;
                }
            }

            base.OnActionExecuted(filterContext);
        }
    }

}