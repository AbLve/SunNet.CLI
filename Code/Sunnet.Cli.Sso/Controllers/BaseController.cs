using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using StructureMap;
using Sunnet.Cli.Core;
using Sunnet.Cli.UIBase.Controllers;
using Sunnet.Framework.Core.Base;
using Sunnet.Framework.Encrypt;
using Sunnet.Framework.Helpers;
using Sunnet.Framework.SSO;
using Sunnet.Cli.Business.Common;
using Sunnet.Framework;

namespace Sunnet.Cli.Sso.Controllers
{
    public class BaseController : GlobalController
    {
        protected string userAccount;
        protected int userID;
        public BaseController()
        {
            UserAuthentication = Authentication.ValidateGlobalCookie(out userID, out userAccount);

            //测试用
            if (UserAuthentication != AuthenticationStatus.Login && SFConfig.EnableAccessManager == "0")
            {
                Authentication.CreatGlobalCookie(1, "clisunnet@gmail.com", "John");
                UserAuthentication = Authentication.ValidateGlobalCookie(out userID, out userAccount);
            }
        }


        protected void SignIASID(string iasid)
        {
            string key = ObjectFactory.GetInstance<IEncrypt>().Encrypt("IASID");
            string val = ObjectFactory.GetInstance<IEncrypt>().Encrypt(iasid);
            CookieHelper.Add(key, val, 0);
        }

        protected string GetIASID()
        {
            string key = ObjectFactory.GetInstance<IEncrypt>().Encrypt("IASID");
            string value = CookieHelper.Get(key);
            value = ObjectFactory.GetInstance<IEncrypt>().Decrypt(value);
            return value;
        }

        protected void ClearCookie()
        {
            Authentication.Logout();
            CookieHelper.RemoveAll();
        }

    }
}