using Sunnet.Framework.Helpers;
using Sunnet.Framework.Log;
using Sunnet.Framework.Encrypt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using StructureMap;

namespace Sunnet.Framework.SSO
{
    public class Authentication
    {
        static readonly string cookieName = ObjectFactory.GetInstance<IEncrypt>().Encrypt("CLIEngageToken");
        static readonly string hashSplitter = "|";
        static ISunnetLog _log =  ObjectFactory.GetInstance<ISunnetLog>();
        public Authentication()
        {
             
        }

        public static string GetAppKey(int appID)
        {
            return string.Empty;
        }

        #region 加密的密钥 David 12/07/2017

        private static string GetAppKey()
        {
            if (string.IsNullOrEmpty(SFConfig.AppKey))
                return "22362E7A9285DD53A0BBC2932F9733C505DC04EDBFE00D70"; //Default AppKey if there is no Appkey in web.config.
            else
                return SFConfig.AppKey;
        }

        private static string GetAppIV()
        {
            if (string.IsNullOrEmpty(SFConfig.AppIV))
                return "1E7FA9231E7FA923"; //Default AppIV if there is no AppIV in web.config.
            else
                return SFConfig.AppIV;

        }
        #endregion


        /// <summary>
        /// 取得加密服务
        /// </summary>
        /// <returns></returns>
        static CryptoService GetCryptoService()
        {
            string key = GetAppKey();
            string IV = GetAppIV();

            CryptoService cs = new CryptoService(key, IV);
            return cs;
        }

        /// <summary>
        /// 创建各分站发往认证中心的 Token
        /// </summary>
        /// <param name="ssoRequest"></param>
        /// <returns></returns>
        public static bool CreateAppToken(SSORequest ssoRequest)
        {
            string OriginalAuthenticator = ssoRequest.IASID + ssoRequest.TimeStamp + ssoRequest.AppUrl;
            string AuthenticatorDigest = CryptoHelper.ComputeHashString(OriginalAuthenticator);
            string sToEncrypt = OriginalAuthenticator + AuthenticatorDigest;
            byte[] bToEncrypt = CryptoHelper.ConvertStringToByteArray(sToEncrypt);

            CryptoService cs = GetCryptoService();

            byte[] encrypted;

            if (cs.Encrypt(bToEncrypt, out encrypted))
            {
                ssoRequest.Authenticator = CryptoHelper.ToBase64String(encrypted);

                return true;
            }
            else
            {
                return false;
            }
        }


        /// <summary>
        /// 验证从各分站发送过来的 Token
        /// </summary>
        /// <param name="ssoRequest"></param>
        /// <returns></returns>
        public static bool ValidateAppToken(SSORequest ssoRequest)
        {
            string Authenticator = ssoRequest.Authenticator;

            string OriginalAuthenticator = ssoRequest.IASID + ssoRequest.TimeStamp + ssoRequest.AppUrl;
            string AuthenticatorDigest = CryptoHelper.ComputeHashString(OriginalAuthenticator);
            string sToEncrypt = OriginalAuthenticator + AuthenticatorDigest;
            byte[] bToEncrypt = CryptoHelper.ConvertStringToByteArray(sToEncrypt);

            CryptoService cs = GetCryptoService();
            byte[] encrypted;

            if (cs.Encrypt(bToEncrypt, out encrypted))
            {
                return Authenticator == CryptoHelper.ToBase64String(encrypted);
            }
            else
            {
                return false;
            }
        }


        /// <summary>
        /// 创建认证中心发往各分站的 Token
        /// </summary>
        /// <param name="ssoRequest"></param>
        /// <returns></returns>
        public static bool CreateCenterToken(SSORequest ssoRequest)
        {
            string OriginalAuthenticator = ssoRequest.UserAccount + ssoRequest.IASID + ssoRequest.TimeStamp + ssoRequest.AppUrl;
            string AuthenticatorDigest = CryptoHelper.ComputeHashString(OriginalAuthenticator);
            string sToEncrypt = OriginalAuthenticator + AuthenticatorDigest;
            byte[] bToEncrypt = CryptoHelper.ConvertStringToByteArray(sToEncrypt);

            CryptoService cs = GetCryptoService();
            byte[] encrypted;

            if (cs.Encrypt(bToEncrypt, out encrypted))
            {
                ssoRequest.Authenticator = CryptoHelper.ToBase64String(encrypted);

                return true;
            }
            else
            {
                return false;
            }
        }


        /// <summary>
        /// 验证从认证中心发送过来的 Token
        /// </summary>
        /// <param name="ssoRequest"></param>
        /// <returns></returns>
        public static bool ValidateCenterToken(SSORequest ssoRequest)
        {
            string Authenticator = ssoRequest.Authenticator;

            string OriginalAuthenticator = ssoRequest.UserAccount + ssoRequest.IASID + ssoRequest.TimeStamp + ssoRequest.AppUrl;
            string AuthenticatorDigest = CryptoHelper.ComputeHashString(OriginalAuthenticator);
            string sToEncrypt = OriginalAuthenticator + AuthenticatorDigest;
            byte[] bToEncrypt = CryptoHelper.ConvertStringToByteArray(sToEncrypt);

            string EncryCurrentAuthenticator = string.Empty;
            CryptoService cs = GetCryptoService();
            byte[] encrypted;

            if (cs.Encrypt(bToEncrypt, out encrypted))
            {
                EncryCurrentAuthenticator = CryptoHelper.ToBase64String(encrypted);

                return Authenticator == EncryCurrentAuthenticator;
            }
            else
            {
                return false;
            }
        }


       /// <summary>
       /// 创建全局的Cookie 内存Cookie，验证时注意判断CookieTimeOut时间
       /// </summary>
       /// <param name="userID">当为0时，表示接受邀请</param>
       /// <param name="userAccount"></param>
       /// <returns></returns>
        public static bool CreatGlobalCookie(int userID, string userAccount,string firstName)
        {
            long ticks = DateTime.Now.Ticks;
            //string plainText = "userID=" + userID + ";userAccount=" + userAccount + ";Ticks=" + ticks + ";userIP=" + userIP;
           string plainText = "userID=" + userID + ";userAccount=" + userAccount + ";Ticks=" + ticks + ";firstName=" +
                              firstName;
            plainText += hashSplitter + CryptoHelper.ComputeHashString(plainText);
            CryptoService cs = GetCryptoService();
            byte[] encrypted;

            if (cs.Encrypt(CryptoHelper.ConvertStringToByteArray(plainText), out encrypted))
            {
                string cookieValue = CryptoHelper.ToBase64String(encrypted);
                SetCookie(cookieValue);

                return true;
            }
            else
            {
                return false;
            }
        }

        public static string GlobalCookieForMoodle(int userID, string userAccount)
        {
            long ticks = DateTime.Now.Ticks;
            string plainText = "userID=" + userID + ";userAccount=" + userAccount + ";Ticks=" + ticks;
            plainText += hashSplitter + CryptoHelper.ComputeHashString(plainText);
            CryptoService cs = GetCryptoService();
            byte[] encrypted;
            cs.Encrypt(CryptoHelper.ConvertStringToByteArray(plainText), out encrypted);
            string cookieValue = CryptoHelper.ToBase64String(encrypted);
            return cookieValue;
        }

        /// <summary>
       ///  验证全局的 Cookie，验证通过时获取用户登录账号及ID
       /// </summary>
       /// <param name="userIP"></param>
       /// <param name="userID"></param>
       /// <param name="userAccount"></param>
       /// <returns></returns>
        public static AuthenticationStatus ValidateGlobalCookie(out int userID, out string userAccount)
        {
            userAccount = string.Empty;
            userID = 0;
            try
            {
                string cookieValue = GetCookie();
                byte[] toDecrypt = CryptoHelper.FromBase64String(cookieValue);
                CryptoService cs = GetCryptoService();

                string decrypted = string.Empty;
                if (cs.Decrypt(toDecrypt, out decrypted))
                {

                    string[] arrTemp = decrypted.Split(Convert.ToChar(hashSplitter));
                    string plainText = arrTemp[0];
                    string hashedText = arrTemp[1];
                    if (hashedText.Replace("\0", string.Empty) == CryptoHelper.ComputeHashString(plainText))
                    {
                        string[] values = plainText.Split(Convert.ToChar(";"));
                        userID = int.Parse(values[0].Split(Convert.ToChar("="))[1]);
                        userAccount = values[1].Split(Convert.ToChar("="))[1];
                        long ticks = long.Parse(values[2].Split(Convert.ToChar("="))[1]);
                        //string userIPinCookie = values[3].Split(Convert.ToChar("="))[1];

                        // Check TimeOut and IP --David 10/03/2014 
                        //if (userIP == userIPinCookie && !isCookieTimeOut(ticks))
                        if (isCookieTimeOut(ticks))
                        {
                            Logout();
                            return AuthenticationStatus.LostSession;
                        }
                        else
                        {
                            return AuthenticationStatus.Login;
                        }
                    }
                }

                return AuthenticationStatus.NotLogin;
            }
            catch (Exception e)
            {
                Console.Write(e);
                return AuthenticationStatus.NotLogin;
            }
        }

        /// <summary>
        /// WordPress read user firstname
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="userAccount"></param>
        /// <param name="firstName"></param>
        /// <returns></returns>
        public static AuthenticationStatus ValidateGlobalCookie(out int userID, out string userAccount,
            out string firstName)
        {
            userAccount = string.Empty;
            userID = 0;
            firstName = "";
            try
            {
                string cookieValue = GetCookie();
                byte[] toDecrypt = CryptoHelper.FromBase64String(cookieValue);
                CryptoService cs = GetCryptoService();

                string decrypted = string.Empty;
                if (cs.Decrypt(toDecrypt, out decrypted))
                {

                    string[] arrTemp = decrypted.Split(Convert.ToChar(hashSplitter));
                    string plainText = arrTemp[0];
                    string hashedText = arrTemp[1];
                    if (hashedText.Replace("\0", string.Empty) == CryptoHelper.ComputeHashString(plainText))
                    {
                        string[] values = plainText.Split(Convert.ToChar(";"));
                        userID = int.Parse(values[0].Split(Convert.ToChar("="))[1]);
                        userAccount = values[1].Split(Convert.ToChar("="))[1];
                        long ticks = long.Parse(values[2].Split(Convert.ToChar("="))[1]);
                        firstName = values[3].Split(Convert.ToChar("="))[1];
                        //string userIPinCookie = values[3].Split(Convert.ToChar("="))[1];

                        // Check TimeOut and IP --David 10/03/2014 
                        //if (userIP == userIPinCookie && !isCookieTimeOut(ticks))
                        if (isCookieTimeOut(ticks))
                        {
                            Logout();
                            return AuthenticationStatus.LostSession;
                        }
                        else
                        {
                            return AuthenticationStatus.Login;
                        }
                    }
                }

                return AuthenticationStatus.NotLogin;
            }
            catch (Exception e)
            {
                Console.Write(e);
                return AuthenticationStatus.NotLogin;
            }
        }


        /// <summary>
        /// 判断Cookie是否Time Out了 
        /// David 10/03/2014
        /// </summary>
        /// <param name="ticks"></param>
        /// <returns></returns>
        public static bool isCookieTimeOut(long ticks)
        {
            try
            {
                TimeSpan ts1 = new TimeSpan(ticks);
                TimeSpan ts2 = new TimeSpan(DateTime.Now.Ticks);
                TimeSpan ts = ts1.Subtract(ts2).Duration();
                //_log.Log("TotalMinutes=" + ts.TotalMinutes + " ,CokieTimeOut=" + SFConfig.CookieTimeOut);
                if (ts.TotalMinutes > SFConfig.CookieTimeOut)
                {
                    return true;
                }
            }
            catch
            {
                return true;//Error, consider time out
            }
            return false;
        }

        public static void Logout()
        {
            CookieHelper.RemoveAll();
        }

        private static void SetCookie(string cookieValue)
        {
            // CookieHelper.Add(cookieName, cookieValue, 24 * 60);
            //David kaokao 10/02/2014
            CookieHelper.Add(cookieName, cookieValue, 0);
        }

        private static string GetCookie()
        {
            return CookieHelper.Get(cookieName);
        }
    }

    public enum AuthenticationStatus
    {
        /// <summary>
        /// 未登陆
        /// </summary>
        NotLogin = 1,
        /// <summary>
        /// 超时
        /// </summary>
        LostSession = 2,
        /// <summary>
        /// 已登陆
        /// </summary>
        Login = 3
    }
}
