using Sunnet.Cli.Business.Users;
using Sunnet.Cli.UIBase.Controllers;
using Sunnet.Cli.UIBase.Models;
using Sunnet.Framework.SSO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sunnet.Cli.Vcw.Controllers
{
    public class BaseController : GlobalController
    {
        public BaseController()
        {     
            int loginID = 0;
            string userAccount = string.Empty;
            AuthenticationStatus authenticationSatus = Authentication.ValidateGlobalCookie(out loginID, out userAccount);
            if (authenticationSatus == AuthenticationStatus.Login)
            {
                if (loginID > 0)
                {
                    UserInfo = new UserBusiness(UnitWorkContext).GetUser(loginID);
                    if (UserInfo != null)
                    {
                        UserAuthentication = AuthenticationStatus.Login;
                        LocalSignIn(loginID, UserInfo.GoogleId, UserInfo.FirstName);
                        System.Web.HttpContext.Current.Items["USERINFO"] = UserInfo;
                    }
                }
            }
            else
            {
                UserAuthentication = authenticationSatus;
                System.Web.HttpContext.Current.Items.Remove("USERINFO");
            }
        }


        /// <summary>
        /// Local Sin In 
        /// David 10/03/2014
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="userAccount"></param>
        /// <param name="userIP"></param>
        protected void LocalSignIn(int userID, string userAccount, string firstName)
        {
            Authentication.CreatGlobalCookie(userID, userAccount, firstName);
        }


        protected string BuilderLoginUrl(string IASID)
        {
            SSORequest ssoRequest = new SSORequest();
            ssoRequest.IASID = IASID;
            ssoRequest.TimeStamp = DateTime.Now.ToString("MM/dd/yyyy HH:mm");
            ssoRequest.AppUrl = string.Format("{0}home/CallBack", DomainHelper.VcwDomain.ToString());
            Authentication.CreateAppToken(ssoRequest);

            PostService ps = new PostService();

            return string.Format("TimeStamp={0}&IASID={1}&Authenticator={2}"
                , System.Web.HttpContext.Current.Server.UrlEncode(ssoRequest.TimeStamp), ssoRequest.IASID
                , System.Web.HttpContext.Current.Server.UrlEncode(ssoRequest.Authenticator));
        }
    }
}