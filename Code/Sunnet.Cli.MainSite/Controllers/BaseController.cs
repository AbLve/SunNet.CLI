using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using StructureMap;
using Sunnet.Cli.Business.Common;
using Sunnet.Cli.Business.Users;
using Sunnet.Cli.Core.Users.Entities;
using Sunnet.Cli.UIBase.Controllers;
using Sunnet.Framework.Encrypt;
using Sunnet.Framework.Helpers;
using Sunnet.Framework.SSO;
using Sunnet.Cli.UIBase.Models;
using Sunnet.Framework;
using Sunnet.Cli.Business.Permission;
using Sunnet.Cli.UIBase;

namespace Sunnet.Cli.MainSite.Controllers
{
    public class BaseController : GlobalController
    {
        public BaseController()
        {
            // cookieKey = ObjectFactory.GetInstance<IEncrypt>().Encrypt("CLI_Main");
            // string value = CookieHelper.Get(cookieKey);
            // value = ObjectFactory.GetInstance<IEncrypt>().Decrypt(value);


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


        ///// <summary>
        ///// google / cli 登陆进来，没有注册本站账号，标记cookie
        ///// </summary>
        ///// <param name="account"></param>
        //protected void SignUserAccount(string account)
        //{
        //    string cookieK = ObjectFactory.GetInstance<IEncrypt>().Encrypt("SignUserAccount");
        //    string cookieV = ObjectFactory.GetInstance<IEncrypt>().Encrypt(account);
        //    cookieV = System.Web.HttpContext.Current.Server.UrlEncode(cookieV);
        //    CookieHelper.Add(cookieK, cookieV, 0);
        //}

        ///// <summary>
        ///// 不是本站点账号，检查是否有 google /cli 登陆的cookie 标记
        ///// </summary>
        ///// <returns></returns>
        //protected bool CheckUserAccount()
        //{
        //    string inviteSignKey = ObjectFactory.GetInstance<IEncrypt>().Encrypt("SignUserAccount");
        //    string value = CookieHelper.Get(inviteSignKey);
        //    if (value != string.Empty)
        //    {
        //        value = ObjectFactory.GetInstance<IEncrypt>().Decrypt(value);
        //        if (value != string.Empty)
        //            return true;
        //    }
        //    return false;
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userType">LoginUserType.UTACCESSMANAGER or  LoginUserType.GOOGLEACCOUNT</param>
        /// <returns></returns>
        protected string BuilderLoginUrl(int userType = 2)
        {
            return base.BuilderLoginUrl(userType, LoginIASID.MAIN);
        }

        /// <summary>
        /// 关闭浏览器，cookie失效
        /// </summary>
        /// <param name="userId"></param>
        protected void InviteSign(int userId)
        {
            string inviteSignKey = ObjectFactory.GetInstance<IEncrypt>().Encrypt("InviteSign");
            string cookieV = ObjectFactory.GetInstance<IEncrypt>().Encrypt(userId.ToString());
            cookieV = System.Web.HttpContext.Current.Server.UrlEncode(cookieV);
            CookieHelper.Add(inviteSignKey, cookieV, 0);
        }

        /// <summary>
        /// 获取邀请的 对应 Id
        /// </summary>
        /// <returns></returns>
        protected int GetInviteSignId()
        {

            string inviteSignKey = ObjectFactory.GetInstance<IEncrypt>().Encrypt("InviteSign");
            string value = CookieHelper.Get(inviteSignKey);
            int tmpId = 0;
            if (value != string.Empty)
            {
                value = ObjectFactory.GetInstance<IEncrypt>().Decrypt(value);
                int.TryParse(value, out tmpId);
            }
            return tmpId;
        }
    }
}