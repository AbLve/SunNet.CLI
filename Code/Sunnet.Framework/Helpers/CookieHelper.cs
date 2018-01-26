using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Sunnet.Framework.Helpers
{
    public static class CookieHelper
    {
        #region Check Parameters
        private static void CheckKey(string key)
        {
            if (key == null || key == string.Empty)
            {
                throw new ArgumentNullException("key", "key should never be null or empty.");
            }
        }

        private static void CheckValue(string value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value", "value should never be null or empty.");
            }
        }
        #endregion

        #region Add Cookie

        /// <summary>
        /// 添加cookie
        /// </summary>
        /// <param name="key">需加密码传入</param>
        /// <param name="value">需加密码传入</param>
        /// <param name="slidingMinutes">单位分钟</param>
        /// <param name="domainName"></param>
        public static void Add(string key, string value, int slidingMinutes, string domainName)
        {
            CheckKey(key);
            CheckValue(value);

            HttpCookie co = new HttpCookie(key);
            co.Value = HttpContext.Current.Server.UrlEncode(value);
            co.Secure = SFConfig.CookieSSL;
            co.HttpOnly = true;

            if (slidingMinutes != 0)
                co.Expires = DateTime.Now.AddMinutes(slidingMinutes);

            if (!(domainName == null || domainName == string.Empty))
                co.Domain = domainName;

            HttpContext.Current.Response.Cookies.Add(co);
        }

        /// <summary>
        /// 添加cookie
        /// </summary>
        /// <param name="key">需加密码传入</param>
        /// <param name="value">需加密码传入</param>
        /// <param name="slidingMinutes">单位分钟,传入零表示关闭浏览器失效</param>
        public static void Add(string key, string value, int slidingMinutes)
        {
            //Add(key, value, slidingMinutes, string.Empty);
            //David 10/02/2014
            Add(key, value, slidingMinutes, SFConfig.CookieDomain);
        }

        #endregion

        #region Get Cookie Value
        public static string Get(string key)
        {
            CheckKey(key);

            string value = string.Empty;

            HttpCookie requestCookie = HttpContext.Current.Request.Cookies[key];
            if (requestCookie != null)
            {
                value = HttpContext.Current.Server.UrlDecode(requestCookie.Value);
            }

            return value;
        }
        #endregion

        #region Remove Cookie
        public static void Remove(string key)
        {
            CheckKey(key);

            HttpContext.Current.Response.Cookies[key].Expires = DateTime.Now.AddDays(-10);
        }

        public static void RemoveAll()
        {
            foreach (string key in HttpContext.Current.Request.Cookies.AllKeys)
            {
                Add(key, "", -1000);
               // HttpContext.Current.Response.Cookies[key].Expires = DateTime.Now.AddDays(-10);
            }
        }
        #endregion

        public static void Resume(string key, int slidingMinutes)
        {
            CheckKey(key);

            HttpCookie cookie = HttpContext.Current.Request.Cookies[key];
            if (cookie != null)
            {
                cookie.Expires = DateTime.Now.AddMinutes(slidingMinutes);
                cookie.Domain = SFConfig.CookieDomain;//David 10/02/2014
                HttpContext.Current.Response.Cookies.Add(cookie);
            }
        }
    }
}