using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;
using System.Web;
using System.Web.Configuration;
using System.Web.Security;
using System.Web.Services.Description;
using Newtonsoft.Json;
using umbraco;
using umbraco.BusinessLogic;
using umbraco.cms.businesslogic.member;
using Umbraco.Core;
using Umbraco.Core.Configuration;
using Umbraco.Core.Security;
using Umbraco.Core.Services;

namespace Our.Umbraco.ezSearch
{
    /// <summary>
    /// EngageLoginEvent 
    /// </summary>
    public class EngageLoginEvent : ApplicationEventHandler
    {
        public EngageLoginEvent()
        {
            //
            // TODO: 
            //
        }
        protected override void ApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            //Listen for the ApplicationInit event which then allows us to bind to the
            //HttpApplication events.
            UmbracoApplicationBase.ApplicationInit += UmbracoApplicationBase_ApplicationInit;
        }
        /// <summary>
        /// Bind to the events of the HttpApplication
        /// </summary>
        void UmbracoApplicationBase_ApplicationInit(object sender, EventArgs e)
        {
            var app = (HttpApplication)sender;

            app.PreRequestHandlerExecute += UmbracoApplication_PreRequestHandlerExecute;
        }
        /// <summary>
        /// At the end of a handled request do something... 
        /// </summary>          
        void UmbracoApplication_PreRequestHandlerExecute(object sender, EventArgs e)
        {
            var app = (HttpApplication)sender;
            var requestKey = app.Request.QueryString["key"];

            var targetUrl = app.Request.QueryString["targetUrl"];

            if (targetUrl == null)
            {
                Write("UmbracoApplication_PreRequestHandlerExecute1: " + targetUrl);
                targetUrl = app.Request.RawUrl;

                Write("UmbracoApplication_PreRequestHandlerExecute2: " + targetUrl);
            }
            if (String.IsNullOrEmpty(requestKey))
            {
                Write("step1: no key.");
                KeyIsNull(app.Request, app.Response, targetUrl);
            }
            else
            {
                Write("step2: has  key.");
                HasKey(app.Context, app.Response, requestKey, targetUrl);
            }
        }

        public void Write(string text)
        {
            /*  FileStream fs = new FileStream("D:\\LogFile.txt", FileMode.Append);
              StreamWriter sw = new StreamWriter(fs, Encoding.Default);
              sw.WriteLine(text);
              sw.Close();
              fs.Close();
              fs.Dispose(); */
        }
        string hashPassword(string password)
        {
            HMACSHA1 hash = new HMACSHA1();
            hash.Key = Encoding.Unicode.GetBytes(password);

            string encodedPassword = Convert.ToBase64String(hash.ComputeHash(Encoding.Unicode.GetBytes(password)));
            return encodedPassword;
        }

        private string Decrypt(string source)
        {
            if (source == null || source.Length == 0)
            {
                return source;
            }

            string key = ".!e@0Na&";
            try
            {
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                byte[] bytes = new byte[source.Length / 2];
                for (int x = 0; x < source.Length / 2; x++)
                {
                    int i = (Convert.ToInt32(source.Substring(x * 2, 2), 16));
                    bytes[x] = (byte)i;
                }

                des.Key = ASCIIEncoding.ASCII.GetBytes(key);
                des.IV = ASCIIEncoding.ASCII.GetBytes(key);

                MemoryStream ms = new MemoryStream();
                CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write);
                cs.Write(bytes, 0, bytes.Length);
                cs.FlushFinalBlock();

                return Encoding.UTF8.GetString(ms.ToArray());
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        private void OtherLogin(HttpResponse response, string userGmail)
        {
            HttpCookie cookie = new HttpCookie(UmbracoConfig.For.UmbracoSettings().Security.AuthCookieName);
            cookie.Value = userGmail;
            response.Cookies.Add(cookie);

        }
        private void KeyIsNull(HttpRequest request, HttpResponse response, string targetUrl)
        {
            bool isSomeoneLoggedIn = HttpContext.Current.User.Identity.IsAuthenticated;

            if (isSomeoneLoggedIn)
            {
                Write("step3: isSomeoneLoggedIn.");
            }
            else if (request.Cookies[UmbracoConfig.For.UmbracoSettings().Security.AuthCookieName] != null)
            {
                Write("step4: CliLogin.");
            }
            else if (request.RawUrl.ToLower().IndexOf("/umbraco", System.StringComparison.Ordinal) == 0)
            {
                if (request.RawUrl.ToLower().IndexOf("/umbraco/", System.StringComparison.Ordinal) == -1)
                    response.Redirect("/umbraco/");
            }
            else
            {
                Write("step6: redirect CLI.");

                if (request.RawUrl.ToLower().IndexOf("favicon.ico", System.StringComparison.Ordinal) == -1 &&
                    request.RawUrl.ToLower().IndexOf("dependencyhandler.axd", System.StringComparison.Ordinal) == -1)
                    RedirectCli(response, targetUrl);
            }
        }

        private void HasKey(HttpContext context, HttpResponse response, string requestKey, string targetUrl)
        {
            CACUser cacUser = new CACUser();
            try
            {
                Write("step7: begin try HasKey.");
                requestKey = HttpUtility.UrlDecode(Decrypt(requestKey));
                cacUser = JsonConvert.DeserializeObject<CACUser>(requestKey);
            }
            catch (Exception ex)
            {
                Write("step8:" + ex.ToString());
                targetUrl = "";
                RedirectCli(response, targetUrl);
            }
            if (cacUser == null)
            {
                Write("step9:cacUser == null");
                targetUrl = "";
                RedirectCli(response, targetUrl);
            }

            if (String.IsNullOrEmpty(cacUser.LoginName) || String.IsNullOrEmpty(cacUser.RoleName))
            {
                Write("step10:cacUser:" + cacUser.LoginName + " and " + cacUser.RoleName);
                targetUrl = "";
                RedirectCli(response, targetUrl);

            }
            if (cacUser.TimeNow < DateTime.Now.AddMinutes(-5) || cacUser.TimeNow > DateTime.Now.AddMinutes(5))
            {
                Write("step11:cacUser.TimeNow:" + cacUser.TimeNow.ToString("yyyy-MM-dd hh:mm:ss"));
                targetUrl = "";
                RedirectCli(response, targetUrl);
            }
            Write("step12:OtherLogin");
            Write("targetUrl-1:" + targetUrl);
            //20170509 begin
            HttpCookie cookie = new HttpCookie("UserLoginInfo");
            cookie.Value = cacUser.CliUserId.ToString();
            response.Cookies.Add(cookie);
            //20170509 end
            string userInfo = hashPassword(cacUser.LoginName);
            Write("targetUrl-2:" + targetUrl);
            OtherLogin(response, userInfo);
            Write("targetUrl-3:" + targetUrl);
            if (String.IsNullOrWhiteSpace(targetUrl))
                response.Redirect("/");
            else
            {
                response.Redirect(targetUrl);
            }
        }

        private void RedirectCli(HttpResponse response, string targetUrl)
        {

            var cliDomain = WebConfigurationManager.AppSettings["CliEngageDomain"] ?? "";
            Write("targetUrl: :" + targetUrl);
            var toCacUrl = cliDomain + "/tocac/index?targetUrl=" + targetUrl;

            response.Redirect(toCacUrl);
        }

        internal class CACUser
        {  //20170509 begin
            public string CliUserId { get; set; }
            //20170509 end
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Email { get; set; }
            public string RoleName { get; set; }
            public DateTime TimeNow { get; set; }
            public string LoginName { get; set; }
        }
    }
}
