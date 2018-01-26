using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using StructureMap;
using Sunnet.Cli.Business.Common;
using Sunnet.Cli.Business.Users;
using Sunnet.Cli.UIBase.Controllers;
using Sunnet.Framework.Encrypt;
using Sunnet.Framework.Helpers;
using Sunnet.Framework.SSO;
using Sunnet.Cli.UIBase.Models;
using Sunnet.Framework;
using Sunnet.Cli.UIBase;
using Sunnet.Framework.Permission;
using Sunnet.Cli.Business.Permission;
using Sunnet.Cli.Business.Permission.Models;

namespace Sunnet.Cli.Assessment.Controllers
{
    public class BaseController : GlobalController
    {
        public BaseController()
        {
            #region
            /*  David 10/03/2014
            cookieKey = ObjectFactory.GetInstance<IEncrypt>().Encrypt("CLI_Assessment");
            string value = CookieHelper.Get(cookieKey);
            value = ObjectFactory.GetInstance<IEncrypt>().Decrypt(value);
            int loginId = 0;
            if (int.TryParse(value, out loginId))
            {
                UserInfo = new UserBusiness(UnitWorkContext).GetUser(loginId);
                if (UserInfo != null)
                {
                    IsLogin = true;
                    SignIn(loginId);
                    System.Web.HttpContext.Current.Items["USERINFO"] = UserInfo;
                }
            }
            else
            {
                System.Web.HttpContext.Current.Items.Remove("USERINFO");
            }*/
            #endregion

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
        protected void LocalSignIn(int userID, string userAccount,string firstName)
        {
            Authentication.CreatGlobalCookie(userID, userAccount, firstName);
        }

        /// <summary>
        /// 本站登陆cookie
        /// </summary>
        /// <param name="userId"></param>
        #region
        /* David 10/03/2014
        protected void SignIn(int userId)
        {
            string cookieV = ObjectFactory.GetInstance<IEncrypt>().Encrypt(userId.ToString());
            CookieHelper.Add(cookieKey, cookieV, CommonAgent.CookieHours * 60);
        }*/
        #endregion

        protected string BuilderLoginUrl(string IASID)
        {
            return base.BuilderLoginUrl(LoginUserType.UTACCESSMANAGER, IASID);
        }

        protected bool CheckAssessmentPermission(int assessmentId, Authority authority)
        {
            UserAuthorityModel userAuthority = new PermissionBusiness().GetUserAuthority(UserInfo, assessmentId + SFConfig.AssessmentPageStartId);
            if (userAuthority == null) return false;

            return (userAuthority.Authority & (int)authority) == (int)authority;
        }
    }
}