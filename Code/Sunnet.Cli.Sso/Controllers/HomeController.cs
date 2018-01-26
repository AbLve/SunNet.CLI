using System;
using System.Web;
using System.Web.Mvc;
using System.Text;

using Sunnet.Cli.Business.Users;
using Sunnet.Cli.Core.Users.Entities;
using Sunnet.Framework.SSO;
using Sunnet.Cli.UIBase.Models;
using Sunnet.Framework;
using Sunnet.Cli.Core.Common.Enums;
using Sunnet.Cli.Business.Log;
using Sunnet.Cli.Core.Log.Entities;
using Sunnet.Cli.UIBase;
using Sunnet.Framework.Log;
using StructureMap;
using Sunnet.Framework.Helpers;
using System.Net;
using System.IO;
using Newtonsoft.Json;

namespace Sunnet.Cli.Sso.Controllers
{
    public class HomeController : BaseController
    {
        ISunnetLog _log;
        public HomeController()
        {
            _log = ObjectFactory.GetInstance<ISunnetLog>();
        }
        // GET: /Home/TimeStamp={0}&IASID={1}&Authenticato
        public ActionResult Index(string IASID = "", string TimeStamp = "", string Authenticator = "" ,int Admin=2)
        {
            DateTime beginDate = DateTime.Parse("2016/12/06 17:00"); 
            DateTime endDate = DateTime.Parse("2016/12/06 21:00"); 
            DateTime now = DateTime.Now;

            switch (Admin)
            {
                case LoginUserType.UTACCESSMANAGER: //Login as UT Account
                    if (now > beginDate && now < endDate)
                    {
                        //Internal user cannot log in.
                        //The system is currently under scheduled maintenance. Please try back after 9:00PM
                        return new RedirectResult(DomainHelper.MainSiteDomain.ToString() + "/maintenance?isInternal=1");
                    }
                    break;
                case LoginUserType.GOOGLEACCOUNT: //Login as Google Account
                    if (now > beginDate && now < endDate)
                    {
                        //External user cannot log in.
                        //The system is currently under scheduled maintenance. Please try back after 9:00PM
                        return new RedirectResult(DomainHelper.MainSiteDomain.ToString() + "/maintenance?isInternal=0");
                    }
                    break;
            }
            if (UserAuthentication == AuthenticationStatus.Login)
            {


                SSORequest ssoRequest = new SSORequest();
                ssoRequest.IASID = IASID.Trim();
                ssoRequest.TimeStamp = TimeStamp.Trim();
                ssoRequest.Authenticator = Authenticator.Trim();
                switch (IASID)
                {
                    case LoginIASID.MAIN:  // Main Site  
                    case LoginIASID.ParentSign:// parent 注册
                    case LoginIASID.LostSession: //登陆后，返回原网页
                        //验证从分站发过来的Token
                        ssoRequest.AppUrl = DomainHelper.Main_SITE_Backurl;

                        if (Authentication.ValidateAppToken(ssoRequest))
                        {
                            ssoRequest.UserAccount = userAccount;

                         //创建认证中心发往各分站的 Token
                            if (Authentication.CreateCenterToken(ssoRequest))
                            {
                                return Content(Post(ssoRequest));
                            }
                        }
                        break;
                    case LoginIASID.ADE: //跳转到 Assessment/ADE Site
                    case LoginIASID.ASSESSMENT: //跳转到 Assessment Site
                    case LoginIASID.CPALLS_OFFLINE:  //cpalls离线 /Offline
                    case LoginIASID.COT_OFFLINE:  //cot 离线 /Cot/Offline
                    case LoginIASID.CEC_OFFLINE:  //cec 离线  /Cec/Offline
                    case LoginIASID.TRS:
                    case LoginIASID.TRS_OFFLINE: //trs 离线 /Trs/Offline
                        //验证从分站发过来的Token
                        ssoRequest.AppUrl = DomainHelper.Assessment_SITE_Backurl;

                        if (Authentication.ValidateAppToken(ssoRequest))
                        {
                            ssoRequest.UserAccount = userAccount;

                            //创建认证中心发往各分站的 Token
                            if (Authentication.CreateCenterToken(ssoRequest))
                            {
                                return Content(Post(ssoRequest));
                            }
                        }
                        break;
                    case LoginIASID.VCW: //跳转到 Vcw Site
                        //验证从分站发过来的Token
                        ssoRequest.AppUrl = DomainHelper.Vcw_SITE_Backurl;

                        if (Authentication.ValidateAppToken(ssoRequest))
                        {
                            ssoRequest.UserAccount = userAccount;

                            //创建认证中心发往各分站的 Token
                            if (Authentication.CreateCenterToken(ssoRequest))
                            {
                                return Content(Post(ssoRequest));
                            }
                        }
                        break;
                    default:
                        return new RedirectResult(DomainHelper.MainSiteDomain.ToString());
                }
                return new RedirectResult(DomainHelper.MainSiteDomain.ToString());
            }
            else
            {
                /* David 2016/08/03 for the CLI Engage Archive 15-16 (Rollover 16-17)
              1.Access to Engage is removed from all Internal and External Users on 08/04/2016 4:00PM 
              2. Access to Internal Users is restored on 08/04/2016 7:00PM
              3. If go or no-go decision, access to all Users is restored on 08/04/2016 9:00PM
              */
                //DateTime dat1 = DateTime.Parse("2016/08/04 10:00"); //08/04/2016 4:00PM
                ////DateTime dat1 = new DateTime(2016,08,04,16,0,0); //08/04/2016 4:00PM
                //DateTime dat2 = DateTime.Parse("2016/08/04 19:00"); //08/04/2016 9:00PM
                //DateTime dat3 = DateTime.Parse("2016/08/04 21:00"); //08/04/2016 9:00PM
                //DateTime now = DateTime.Now;



                if (IASID == string.Empty)
                    IASID = LoginIASID.MAIN;
                SignIASID(IASID);
                if (SFConfig.EnableAccessManager == "1")
                {
                    switch (Admin)
                    {
                        case LoginUserType.UTACCESSMANAGER: //Login as UT Account
                           
                            return new RedirectResult(SFConfig.AccessManagerAdminUrl + HttpUtility.UrlEncode(DomainHelper.SSO_SITE_CLIBackurl));
                        case LoginUserType.GOOGLEACCOUNT: //Login as Google Account
                            

                            var url = "https://accounts.google.com/o/oauth2/auth?" +
         "scope=openid+email{0}&state={1}&redirect_uri={2}&response_type=code&client_id={3}&approval_prompt=auto&access_type=online";
        //userinfo.email表示获取用户的email
                            var scope = ""; //HttpUtility.UrlEncode("https://www.googleapis.com/auth/drive.file"); openid and then include profile or email
        //对应于userinfo.email
                            var state = "profile";
                            var redirectUri = HttpUtility.UrlEncode(SFConfig.Google_RedirectUri);
                            var cilentId = HttpUtility.UrlEncode(SFConfig.Google_ClientID);
                            return Redirect(string.Format(url, scope, state, redirectUri, cilentId));

                        default:
                            return new RedirectResult(DomainHelper.MainSiteDomain.ToString());
                    }                   
                }
            }
            return new RedirectResult(DomainHelper.MainSiteDomain.ToString());
        }
        public ActionResult Index2(string IASID = "", string TimeStamp = "", string Authenticator = "", int Admin = 2)
        {
            DateTime beginDate = DateTime.Parse("2016/12/06 17:00");
            DateTime endDate = DateTime.Parse("2016/12/06 21:00");
            DateTime now = DateTime.Now;

            switch (Admin)
            {
                case LoginUserType.UTACCESSMANAGER: //Login as UT Account
                    //if (now > beginDate && now < endDate)
                    //{
                    //    //Internal user cannot log in.
                    //    //The system is currently under scheduled maintenance. Please try back after 9:00PM
                    //    return new RedirectResult(DomainHelper.MainSiteDomain.ToString() + "/maintenance?isInternal=1");
                    //}
                    break;
                case LoginUserType.GOOGLEACCOUNT: //Login as Google Account
                    //if (now > beginDate && now < endDate)
                    //{
                    //    //External user cannot log in.
                    //    //The system is currently under scheduled maintenance. Please try back after 9:00PM
                    //    return new RedirectResult(DomainHelper.MainSiteDomain.ToString() + "/maintenance?isInternal=0");
                    //}
                    break;
            }
            if (UserAuthentication == AuthenticationStatus.Login)
            {


                SSORequest ssoRequest = new SSORequest();
                ssoRequest.IASID = IASID.Trim();
                ssoRequest.TimeStamp = TimeStamp.Trim();
                ssoRequest.Authenticator = Authenticator.Trim();
                switch (IASID)
                {
                    case LoginIASID.MAIN:  // Main Site  
                    case LoginIASID.ParentSign:// parent 注册
                    case LoginIASID.LostSession: //登陆后，返回原网页
                        //验证从分站发过来的Token
                        ssoRequest.AppUrl = DomainHelper.Main_SITE_Backurl;

                        if (Authentication.ValidateAppToken(ssoRequest))
                        {
                            ssoRequest.UserAccount = userAccount;

                            //创建认证中心发往各分站的 Token
                            if (Authentication.CreateCenterToken(ssoRequest))
                            {
                                return Content(Post(ssoRequest));
                            }
                        }
                        break;
                    case LoginIASID.ADE: //跳转到 Assessment/ADE Site
                    case LoginIASID.ASSESSMENT: //跳转到 Assessment Site
                    case LoginIASID.CPALLS_OFFLINE:  //cpalls离线 /Offline
                    case LoginIASID.COT_OFFLINE:  //cot 离线 /Cot/Offline
                    case LoginIASID.CEC_OFFLINE:  //cec 离线  /Cec/Offline
                    case LoginIASID.TRS:
                    case LoginIASID.TRS_OFFLINE: //trs 离线 /Trs/Offline
                        //验证从分站发过来的Token
                        ssoRequest.AppUrl = DomainHelper.Assessment_SITE_Backurl;

                        if (Authentication.ValidateAppToken(ssoRequest))
                        {
                            ssoRequest.UserAccount = userAccount;

                            //创建认证中心发往各分站的 Token
                            if (Authentication.CreateCenterToken(ssoRequest))
                            {
                                return Content(Post(ssoRequest));
                            }
                        }
                        break;
                    case LoginIASID.VCW: //跳转到 Vcw Site
                        //验证从分站发过来的Token
                        ssoRequest.AppUrl = DomainHelper.Vcw_SITE_Backurl;

                        if (Authentication.ValidateAppToken(ssoRequest))
                        {
                            ssoRequest.UserAccount = userAccount;

                            //创建认证中心发往各分站的 Token
                            if (Authentication.CreateCenterToken(ssoRequest))
                            {
                                return Content(Post(ssoRequest));
                            }
                        }
                        break;
                    default:
                        return new RedirectResult(DomainHelper.MainSiteDomain.ToString());
                }
                return new RedirectResult(DomainHelper.MainSiteDomain.ToString());
            }
            else
            {
                /* David 2016/08/03 for the CLI Engage Archive 15-16 (Rollover 16-17)
              1.Access to Engage is removed from all Internal and External Users on 08/04/2016 4:00PM 
              2. Access to Internal Users is restored on 08/04/2016 7:00PM
              3. If go or no-go decision, access to all Users is restored on 08/04/2016 9:00PM
              */
                //DateTime dat1 = DateTime.Parse("2016/08/04 10:00"); //08/04/2016 4:00PM
                ////DateTime dat1 = new DateTime(2016,08,04,16,0,0); //08/04/2016 4:00PM
                //DateTime dat2 = DateTime.Parse("2016/08/04 19:00"); //08/04/2016 9:00PM
                //DateTime dat3 = DateTime.Parse("2016/08/04 21:00"); //08/04/2016 9:00PM
                //DateTime now = DateTime.Now;



                if (IASID == string.Empty)
                    IASID = LoginIASID.MAIN;
                SignIASID(IASID);
                if (SFConfig.EnableAccessManager == "1")
                {
                    switch (Admin)
                    {
                        case LoginUserType.UTACCESSMANAGER: //Login as UT Account

                            return new RedirectResult(SFConfig.AccessManagerAdminUrl + HttpUtility.UrlEncode(DomainHelper.SSO_SITE_CLIBackurl));
                        case LoginUserType.GOOGLEACCOUNT: //Login as Google Account


                            var url = "https://accounts.google.com/o/oauth2/auth?" +
         "scope=openid+email{0}&state={1}&redirect_uri={2}&response_type=code&client_id={3}&approval_prompt=auto&access_type=online";
                            //userinfo.email表示获取用户的email
                            var scope = ""; //HttpUtility.UrlEncode("https://www.googleapis.com/auth/drive.file"); openid and then include profile or email
                                            //对应于userinfo.email
                            var state = "profile";
                            var redirectUri = HttpUtility.UrlEncode(SFConfig.Google_RedirectUri);
                            var cilentId = HttpUtility.UrlEncode(SFConfig.Google_ClientID);
                            return Redirect(string.Format(url, scope, state, redirectUri, cilentId));

                        default:
                            return new RedirectResult(DomainHelper.MainSiteDomain.ToString());
                    }
                }
            }
            return new RedirectResult(DomainHelper.MainSiteDomain.ToString());
        }
        string Post(SSORequest ssoRequest)
        {
            PostService ps = new PostService();
            ps.Url = ssoRequest.AppUrl;
            ps.Add("UserAccount", ssoRequest.UserAccount);
            ps.Add("e", ssoRequest.Email);
            ps.Add("IASID", ssoRequest.IASID);
            ps.Add("TimeStamp", System.Web.HttpContext.Current.Server.UrlEncode(ssoRequest.TimeStamp));
            ps.Add("Authenticator", System.Web.HttpContext.Current.Server.UrlEncode(ssoRequest.Authenticator));
            return ps.Post();
        }

        public ActionResult LogOut(string forward,string moodleForward)
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
            if (string.IsNullOrEmpty(moodleForward) && (SFConfig.EnableLMS == "1") )
            {
                return new RedirectResult(string.Format("{0}login/logout.php?auth=cli", SFConfig.LMSDomain));
            }

            return View();
        }
                

        /// <summary>
        /// QA专用，仿google登陆
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult LoginUser(string key)
        {
            string tmpIp = CommonHelper.GetIPAddress(Request);
           // if (tmpIp == "58.32.209.110" || tmpIp == "98.197.44.82" || tmpIp == "127.0.0.1" || tmpIp.StartsWith("129.106.") || tmpIp == "99.162.197.214" || tmpIp.StartsWith("192.168.1."))
            {
                UserBaseEntity user = new UserBusiness().UserLoginQA(key);
                if (user != null && (user.Status == EntityStatus.Inactive || user.IsDeleted))
                    return new RedirectResult("/home/Guide");

                if (user == null)
                    user = new UserBaseEntity();

                Authentication.CreatGlobalCookie(user.ID, key, user.FirstName);
                return new RedirectResult(string.Format("{0}home/Index?{1}", DomainHelper.SsoSiteDomain.ToString(), BuilderLoginUrl()));
            }
            return View();
        }
        public ActionResult LoginUser2(string key)
        {
            string tmpIp = CommonHelper.GetIPAddress(Request);
            // if (tmpIp == "58.32.209.110" || tmpIp == "98.197.44.82" || tmpIp == "127.0.0.1" || tmpIp.StartsWith("129.106.") || tmpIp == "99.162.197.214" || tmpIp.StartsWith("192.168.1."))
            {
                UserBaseEntity user = new UserBusiness().UserLoginQA(key);
                if (user != null && (user.Status == EntityStatus.Inactive || user.IsDeleted))
                    return new RedirectResult("/home/Guide");

                if (user == null)
                    user = new UserBaseEntity();

                Authentication.CreatGlobalCookie(user.ID, key, user.FirstName);
                return new RedirectResult(string.Format("{0}home/Index2?{1}", DomainHelper.SsoSiteDomain.ToString(), BuilderLoginUrl()));
            }
            return View();
        }

        /// <summary>
        /// QA专用，仿google 登陆 ,用来测试注册 /home/Guide
        /// </summary>
        public string LoginGoogle(string key)
        {
            if (SFConfig.TestMode)
            {
                string tmpIp = CommonHelper.GetIPAddress(Request);
                if (tmpIp == "58.32.209.110" || tmpIp == "98.197.44.82" || tmpIp == "127.0.0.1" || tmpIp.StartsWith("129.106.") || tmpIp == "99.162.197.214" || tmpIp.StartsWith("192.168.1."))
                {
                    SignIASID("01");
                    UserBaseEntity user = new UserBaseEntity();
                    user.GoogleId = key;
                    return Push(user, key);
                }
                return "unauthorized access";
            }
            else
                return "unauthorized access";
        }

        /// <summary>
        /// QA专用
        /// </summary>
        /// <returns></returns>
        protected string BuilderLoginUrl()
        {
            SSORequest ssoRequest = new SSORequest();
            ssoRequest.IASID = "01";
            ssoRequest.TimeStamp = DateTime.Now.Ticks.ToString();  //DateTime.Now.ToString("MM/dd/yyyy HH:mm");
            ssoRequest.AppUrl = DomainHelper.Main_SITE_Backurl;
            Authentication.CreateAppToken(ssoRequest);

            PostService ps = new PostService();

            return string.Format("TimeStamp={0}&IASID={1}&Authenticator={2}"
                , System.Web.HttpContext.Current.Server.UrlEncode(ssoRequest.TimeStamp), ssoRequest.IASID
                , System.Web.HttpContext.Current.Server.UrlEncode(ssoRequest.Authenticator));
        }

        /// <summary>
        /// CLI用户
        /// </summary>
        /// <returns></returns>
        public string CallBackCLI()
        {
            StringBuilder sb = new StringBuilder();
            string email = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_EPPN"] + "";
            email = email.Trim();
            string dispLAYNAME = this.Request.ServerVariables["HTTP_DISPLAYNAME"] +"";

            dispLAYNAME = dispLAYNAME.Trim();
            //_log.Log(string.Format("CLI Inter User Return HTTP_EPPN:{0}  ;HTTP_DISPLAYNAME:{1}", email, dispLAYNAME));
            if (string.IsNullOrEmpty(email))
                return "<script type='text/javascript'>location='/home/Guide';</script>";
            if (email.IndexOf("@") > -1)
            {
                email = email.Substring(0, email.IndexOf("@"));
                UserBaseEntity user = new UserBusiness().UserLogin(email);
                if (user == null)
                    return "<script type='text/javascript'>location='/home/Guide';</script>";

                ///登陆日志
                new OperationLogBusiness().InsertLogingLog(new OperationLogEntity( CommonHelper.GetIPAddress(Request), email, user.ID));
                return Push(user,email);
            }
            return "<script type='text/javascript'>location='/home/Guide';</script>";
        }

        /// <summary>
        /// 外部用户回调（如 Teacher,parent,校长，专家)
        /// </summary>
        /// <returns></returns>
        public string CallBackExtend()
        {
            var id = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_GOOGLEID"] + "";
            id = id.Trim();

            if (!string.IsNullOrEmpty(id))
                id = id.Replace("https://plus.google.com/", "");

            var email = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_MAIL"] + "";
            email = email.Trim();

            //_log.Log(string.Format("External User Return HTTP_GOOGLEID:{0}  ;HTTP_MAIL:{1}", id, email));
            if (!string.IsNullOrEmpty(email))
            {
                UserBaseEntity user = new UserBaseEntity();
                user.GoogleId = id;                
                ///登陆日志
                new OperationLogBusiness().InsertLogingLog(new OperationLogEntity(CommonHelper.GetIPAddress(Request), email, user.ID, id));
                return Push(user,email);
            }
            return "<script type='text/javascript'>location='/home/Guide';</script>";
        }

        /// <summary>
        /// 外部用户回调 （google api)
        /// </summary>
        /// <returns></returns>
        public string CallBack()
        {
            if (Request.QueryString["code"] == null || Request.QueryString["code"] == string.Empty) //CallBack?error=access_denied&state=email#
                return string.Format("<script type='text/javascript'>location='{0}';</script>", DomainHelper.MainSiteDomain.ToString());

            //由于是https，这里必须要转换为HttpWebRequest
            var webRequest = WebRequest.Create("https://accounts.google.com/o/oauth2/token") as HttpWebRequest;
            webRequest.Method = "POST";
            webRequest.ContentType = "application/x-www-form-urlencoded";

            //参考https://developers.google.com/accounts/docs/OAuth2WebServer
            var postData = string.Format("code={0}&client_id={1}&client_secret={2}&redirect_uri={3}" +
                "&grant_type=authorization_code",
                Request.QueryString["code"],
                 SFConfig.Google_ClientID,
                 SFConfig.Google_ClientSecret,
                 SFConfig.Google_RedirectUri);

            //在HTTP POST请求中传递参数
            using (var sw = new StreamWriter(webRequest.GetRequestStream()))
            {
                sw.Write(postData);
            }

            //发送请求，并获取服务器响应
            var resonseJson = "";
            using (var response = webRequest.GetResponse())
            {
                using (var sr = new StreamReader(response.GetResponseStream()))
                {
                    resonseJson = sr.ReadToEnd();
                }
            }


            //通过Json.NET对服务器返回的json字符串进行反序列化，得到access_token
            var accessToken = JsonConvert.DeserializeAnonymousType(resonseJson, new { access_token = "" }).access_token;

            webRequest = WebRequest.Create("https://www.googleapis.com/oauth2/v1/userinfo") as HttpWebRequest;
            webRequest.Method = "GET";
            webRequest.Headers.Add("Authorization", "Bearer " + accessToken);

            using (var response = webRequest.GetResponse())
            {
                using (var sr = new StreamReader(response.GetResponseStream()))
                {
                   // resonseJson = sr.ReadToEnd();
                   var userInfo =  JsonConvert.DeserializeAnonymousType(sr.ReadToEnd(), new {id="", Email = "" });
                   if (!string.IsNullOrEmpty(userInfo.Email))
                   {
                       UserBaseEntity user = new UserBaseEntity();
                       user.GoogleId = userInfo.id;
                       ///登陆日志
                       new OperationLogBusiness().InsertLogingLog(new OperationLogEntity(CommonHelper.GetIPAddress(Request), userInfo.Email, user.ID, userInfo.id));
                       return Push(user, userInfo.Email);
                   }
                   return "<script type='text/javascript'>location='/home/Guide';</script>";
                }
            }
        }

        private string Push(UserBaseEntity user, string eamil)
        {
            string iasid = GetIASID();

            SSORequest ssoRequest = new SSORequest();

            ssoRequest = new SSORequest();
            ssoRequest.UserAccount = user.GoogleId;
            ssoRequest.Email = eamil;
            ssoRequest.IASID = iasid;
            ssoRequest.TimeStamp = DateTime.Now.Ticks.ToString();//DateTime.Now.ToString("MM/dd/yyyy HH:mm");
            ssoRequest.Authenticator = string.Empty;
            string userIP = CommonHelper.GetIPAddress(Request);
            switch (iasid)
            {
                case LoginIASID.MAIN:  //Main Site    
                case LoginIASID.ParentSign: // parent 注册
                case LoginIASID.LostSession: //登陆后返回原网页

                    ssoRequest.AppUrl = DomainHelper.Main_SITE_Backurl;
                    if (Authentication.CreateCenterToken(ssoRequest))
                    {
                        Authentication.CreatGlobalCookie(user.ID, user.GoogleId, user.FirstName);
                        return Post(ssoRequest);
                    }
                    else
                        return string.Format("<script type='text/javascript'>location='{0}';</script>", DomainHelper.MainSiteDomain);
                case LoginIASID.ADE: //Assessment ADE Site
                case LoginIASID.ASSESSMENT: //Assessment 
                case LoginIASID.CPALLS_OFFLINE:  //cpalls离线 /Offline
                case LoginIASID.COT_OFFLINE:  //cot 离线 /Cot/Offline
                case LoginIASID.CEC_OFFLINE:  //cec 离线  /Cec/Offline
                case LoginIASID.TRS:
                case LoginIASID.TRS_OFFLINE:  // trs 离线 /Trs/Offline
                    ssoRequest.AppUrl = DomainHelper.Assessment_SITE_Backurl;
                    if (Authentication.CreateCenterToken(ssoRequest))
                    {
                        Authentication.CreatGlobalCookie(user.ID, user.GoogleId, user.FirstName);
                        return Post(ssoRequest);
                    }
                    else
                        return string.Format("<script type='text/javascript'>location='{0}';</script>", DomainHelper.MainSiteDomain);
                case LoginIASID.VCW: //Vcw 
                    ssoRequest.AppUrl = DomainHelper.Vcw_SITE_Backurl;
                    if (Authentication.CreateCenterToken(ssoRequest))
                    {
                        Authentication.CreatGlobalCookie(user.ID, user.GoogleId, user.FirstName);
                        return Post(ssoRequest);
                    }
                    else
                        return string.Format("<script type='text/javascript'>location='{0}';</script>", DomainHelper.AssessmentDomain);
                default:
                    return string.Format("<script type='text/javascript'>location='{0}';</script>", DomainHelper.MainSiteDomain);
            }
        }

        /// <summary>
        /// The user does not exist. 用户不存在时，会跳到该页面
        /// </summary>
        /// <returns></returns>
        public ActionResult Guide()
        {
            return View();
        }

        /// <summary>
        /// 申请家长时，会判断当前的google是否有绑定其他的账号，如果有，见提示用户
        /// </summary>
        /// <returns></returns>
        public ActionResult Switches()
        {
            return View();
        }

        /// <summary>
        /// 通过邀请链接注册后，发现绑定的google 已被使用，提示用户
        /// </summary>
        /// <returns></returns>
        public ActionResult SwitchesUser()
        {
            return View();
        }

        public ActionResult LostSession(string IASID = "")
        {
            if (false == LoginIASID.CheckIASID(IASID))
                IASID = LoginIASID.MAIN;
         
            ViewBag.LoginUrl_Google = BuilderLoginUrl(LoginUserType.GOOGLEACCOUNT, IASID);
            ViewBag.LoginUrl_UT = BuilderLoginUrl(LoginUserType.UTACCESSMANAGER, IASID);
            return View();
        }

        // 每次访问Moodle页面的时候会访问此方法得到延长的Cookie值，然后在Moodle中写入此Cookie
        public string MoodleGlobalCookie(int userid)
        {
            UserBaseEntity user = new UserBusiness().GetUser(userid);
            return Authentication.GlobalCookieForMoodle(user.ID, user.GoogleId);
        }

        public ActionResult TRSLogin()
        {
            ViewBag.TRS_LoginUrl_Google = BuilderLoginUrl(LoginUserType.GOOGLEACCOUNT, LoginIASID.MAIN); 
            return View();
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        { }
    }
}