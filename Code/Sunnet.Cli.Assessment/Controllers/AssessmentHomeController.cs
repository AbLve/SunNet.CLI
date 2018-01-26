using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Sunnet.Cli.UIBase.Controllers;
using Sunnet.Framework.Helpers;
using Sunnet.Framework.SSO;
using Sunnet.Cli.UIBase.Models;
using Sunnet.Cli.Core.Users.Entities;
using Sunnet.Cli.Business.Users;
using Sunnet.Cli.Business.Permission;
using Sunnet.Cli.Business.Permission.Models;
using Sunnet.Cli.UIBase;
using System.Collections;

namespace Sunnet.Cli.Assessment.Controllers
{
    public class AssessmentHomeController : BaseController
    {
        UserBusiness userBusiness;
        PermissionBusiness _permissionBusiness;
        public AssessmentHomeController()
        {
            userBusiness = new UserBusiness(UnitWorkContext);
              _permissionBusiness = new PermissionBusiness(UnitWorkContext);
        }


        // GET: /AssessmentHome/
        public ActionResult Index(string IASID)
        {
            if (UserAuthentication != AuthenticationStatus.Login)
                return new RedirectResult(string.Format("{0}home/LostSession", DomainHelper.SsoSiteDomain));
            else
            {
                switch (IASID)
                {
                    case LoginIASID.ADE:
                        return new RedirectResult("/Ade");
                    case LoginIASID.ASSESSMENT:///Dashboard/                      
                        return new RedirectResult("/");
                    case LoginIASID.CPALLS_OFFLINE: //cpalls+ 离线
                        return new RedirectResult("/Offline");
                    case LoginIASID.COT_OFFLINE: //cot 离线
                        return new RedirectResult("/Cot/Offline");
                    case LoginIASID.CEC_OFFLINE: //cec 离线
                        return new RedirectResult("/Cec/Offline");
                    case LoginIASID.TRS: //trs
                        return new RedirectResult("/trs");
                    case LoginIASID.TRS_OFFLINE:// TRS 离线
                        return new RedirectResult("/Trs/Offline");
                    case LoginIASID.ADE_LAYOUT://Ade Layout
                        return new RedirectResult("/Layout");
                    case LoginIASID.ADE_BUK://Ade Bulk Upload
                        return new RedirectResult("/BulkUpload");
                    case LoginIASID.Observables:// 
                        return new RedirectResult("/Observable/Observable/Index");
                    case LoginIASID.TSDS:
                        return new RedirectResult("/Tsds/Tsds/Index");
                    case LoginIASID.AssessmentPractice:
                        return new RedirectResult("/Practice/Practice/Index");

                }
                return new RedirectResult(DomainHelper.MainSiteDomain.ToString());
            }
        }

        /// <summary>
        /// 专供sso调用
        /// </summary>
        public ActionResult LogOut()
        {
            CookieHelper.RemoveAll();
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
                ssoRequest.AppUrl = string.Format("{0}AssessmentHome/CallBack", DomainHelper.AssessmentDomain.ToString());
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


                    UserBaseEntity user = userBusiness.UserLogin(UserAccount);
                    if (user != null)
                    {
                        LocalSignIn(user.ID, user.GoogleId, user.FirstName);
                        switch (IASID)
                        {
                            case LoginIASID.ADE:
                                return new RedirectResult(string.Format("{0}Ade/", DomainHelper.AssessmentDomain.ToString()));
                            case LoginIASID.ASSESSMENT: ///Dashboard/
                                {
                                    if (_permissionBusiness.CheckMenu(UserInfo, (int)PagesModel.Assessment))
                                    {
                                        UserAuthorityModel userAuthorityModel = _permissionBusiness.GetUserAuthority(UserInfo, (int)PagesModel.CPALLS);
                                        if (userAuthorityModel != null)
                                            return new RedirectResult("/");
                                        else
                                        {
                                            userAuthorityModel = _permissionBusiness.GetUserAuthority(UserInfo, (int)PagesModel.CEC);
                                            if (userAuthorityModel != null)
                                                return new RedirectResult("/Cec/Cec/Dashboard");
                                        }
                                    }
                                }
                                break;
                            case LoginIASID.CPALLS_OFFLINE: //cpalls+ 离线
                                return new RedirectResult("/Offline");
                            case LoginIASID.COT_OFFLINE: //cot 离线
                                return new RedirectResult("/Cot/Offline");
                            case LoginIASID.CEC_OFFLINE:  //cec 离线
                                return new RedirectResult("/Cec/Offline");
                            case LoginIASID.TRS:
                                return new RedirectResult("/trs");
                            case LoginIASID.TRS_OFFLINE:
                                return new RedirectResult("/Trs/Offline");
                        }
                    }
                }
            }
            return new RedirectResult(DomainHelper.MainSiteDomain.ToString());
        }
        
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        { }


        
    }
}