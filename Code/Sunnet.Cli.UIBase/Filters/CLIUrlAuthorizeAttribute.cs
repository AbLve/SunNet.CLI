using Sunnet.Cli.Business.Permission;
using Sunnet.Cli.Business.Permission.Models;
using Sunnet.Cli.Core.Permission.Entities;
using Sunnet.Cli.Core.Users.Entities;
using Sunnet.Cli.UIBase.Models;
using Sunnet.Framework;
using Sunnet.Framework.Helpers;
using Sunnet.Framework.Permission;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sunnet.Cli.UIBase.Filters
{
    public class CLIUrlAuthorizeAttribute : AuthorizeAttribute
    {
        public Authority Account { get; set; }
        public PagesModel PageId { get; set; }

        /// <summary>
        /// 为ade 专用
        /// </summary>
        public string Parameter { get; set; }

        /// <summary>
        /// 是否记住Url，登陆
        /// </summary>
        /// 
        bool _signUrl = true;
        public bool IsSignUrl
        {
            get { return _signUrl; }
            set { _signUrl = value; }
        }

        /// <summary>
        /// 是否充许匿名访问
        /// </summary>
        public Anonymous Anonymity { get; set; }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException("HttpContext");
            }

            if (!httpContext.User.Identity.IsAuthenticated)
            {
                return false;
            }
            return true;
        }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (Anonymity == Anonymous.Unconditional) return;
            bool canAccess = false;


            //if (SysSetting.GetSettings().Permission == "0") return;
            UserBaseEntity userInfo = System.Web.HttpContext.Current.Items["USERINFO"] as UserBaseEntity;
            if (userInfo != null)
            {
                if (Anonymity == Anonymous.Logined) return;
                UserAuthorityModel userAuthority = null;
                if (string.IsNullOrEmpty(Parameter) == false)
                {
                    string tmpAssessmentId = "0";

                    if (Parameter.ToLower() == "id")
                    {
                        tmpAssessmentId = filterContext.HttpContext.Request[Parameter];
                        if (tmpAssessmentId == null)
                            tmpAssessmentId = filterContext.HttpContext.Request.Url.Segments.Last();
                    }
                    else
                    {
                        tmpAssessmentId = filterContext.HttpContext.Request[Parameter];
                    }
                    int tmpId = 0;
                    int.TryParse(tmpAssessmentId, out tmpId);
                    userAuthority = new PermissionBusiness().GetUserAuthority(userInfo, tmpId + SFConfig.AssessmentPageStartId);
                }
                else
                    userAuthority = new PermissionBusiness().GetUserAuthority(userInfo, (int)PageId);
                if (userAuthority != null)
                {
                    if ((userAuthority.Authority & (int)Account) == (int)Account)
                    {
                        canAccess = true;
                    }
                }
            }

            if (false == canAccess)
            {
                if (filterContext.HttpContext.Request.IsAjaxRequest())
                {
                    filterContext.HttpContext.Response.StatusCode = 403;
                    filterContext.Result = new ContentResult() { Content = "response" };
                }
                else
                {
                    if (filterContext.HttpContext.Request.HttpMethod.ToLower().IndexOf("post") >= 0)
                        filterContext.Result = new ContentResult() { Content = string.Format("<script>top.location.href='{0}/home/LostSession';</script>", SFConfig.SsoDomain, LoginIASID.LostSession) };
                    else
                    {
                        if (userInfo == null)
                        {
                            if (_signUrl)
                            {
                                string url = filterContext.HttpContext.Request.Url.ToString();
                                CookieHelper.Add("Url", System.Web.HttpContext.Current.Server.UrlEncode(url), 0);
                            }
                            filterContext.Result = new RedirectResult(string.Format("{0}/home/LostSession?IASID={1}", SFConfig.SsoDomain, LoginIASID.LostSession));
                        }
                        else
                            filterContext.Result = new RedirectResult("/error/nonauthorized");
                    }
                }
            }
        }
    }

    public enum Anonymous
    {
        /// <summary>
        /// 需要验证的
        /// </summary>
        Verified,
        /// <summary>
        /// 需要登陆
        /// </summary>
        Logined,

        /// <summary>
        /// 无条件匿名性
        /// </summary>
        Unconditional
    }
}