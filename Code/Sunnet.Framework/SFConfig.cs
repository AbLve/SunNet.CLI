using Sunnet.Framework.Helpers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;

namespace Sunnet.Framework
{
    public class SFConfig
    {
        private static string GetAppSetting(string key)
        {
            return ConfigurationManager.AppSettings[key] ?? "";
        }

        private static int GetAppSettingOutInt(string key)
        {
            int result;
            if (int.TryParse(GetAppSetting(key).Trim(), out result))
                return result;
            else
                return 0;
        }

        private static bool GetAppSettingOutBool(string key)
        {
            bool result = false;
            if (GetAppSetting(key).Equals("1"))
                result = true;
            else if (GetAppSetting(key).Equals("0"))
                result = false;
            else
                bool.TryParse(GetAppSetting(key), out result);
            return result;
        }

        private static string _TestingCommunities = GetAppSetting("TestingCommunities");
        public static string TestingCommunities
        {
            get
            {
                return _TestingCommunities;
            }
        }

        private static string _AppKey = GetAppSetting("AppKey");
        private static string _AppIV = GetAppSetting("AppIV");
        public static string AppKey
        {
            get
            {
                return _AppKey;
            }
        }

        public static string AppIV
        {
            get
            {
                return _AppIV;
            }
        }

        private static string _expiredDate = GetAppSetting("ExpiredDate");
        public static string ExpiredDate
        {
            get
            {
                return _expiredDate;
            }
        }

        private static string _FromEmailAddress = GetAppSetting("FromEmailAddress");
        public static string FromEmailAddress
        {
            get
            {
                return _FromEmailAddress;
            }
        }


        private static string _EVOPDFKEY = GetAppSetting("EVOPDFKEY");
        public static string EVOPDFKEY
        {
            get
            {
                return _EVOPDFKEY;
            }
        }

        private static string _WebSite = GetAppSetting("WebSite");
        public static string WebSite
        {
            get
            {
                return _WebSite;
            }
        }


        private static int _CookieTimeOut = GetAppSettingOutInt("CookieTimeOut");
        /// <summary>
        /// Number of Minutes
        /// </summary>
        public static int CookieTimeOut
        {
            get
            {
                return _CookieTimeOut;
            }
        }


        private static string _CookieDomain = GetAppSetting("CookieDomain");

        /// <summary>
        /// Cookie's Domain --David 10/02/2014
        /// </summary>
        public static string CookieDomain
        {
            get
            {
                return _CookieDomain;
            }
        }

        private static bool _CookieSSL = GetAppSettingOutBool("CookieSSL");
        public static bool CookieSSL
        {
            get
            {
                return _CookieSSL;
            }
        }

        private static bool _TestMode = GetAppSettingOutBool("TestMode");
        public static bool TestMode
        {
            get
            {
                return _TestMode;
            }
        }

        private static string _TestModeEmail = GetAppSetting("TestModeEmail");
        public static string TestModeEmail
        {
            get
            {
                return _TestModeEmail;
            }
        }

        private static string _CLIAdministratorEmail = GetAppSetting("CLIAdministratorEmail");
        public static string CLIAdministratorEmail
        {
            get
            {
                return _CLIAdministratorEmail;
            }
        }

        private static string _EmailDisplayName = GetAppSetting("EmailDisplayName");
        public static string EmailDisplayName
        {
            get
            {
                return _EmailDisplayName;
            }
        }

        private static bool _EmailSSL = GetAppSettingOutBool("EmailSSL");
        public static bool EmailSSL
        {
            get
            {
                return _EmailSSL;
            }
        }


        private static string _SenderEmail = GetAppSetting("SenderEmail");
        public static string SenderEmail
        {
            get
            {
                return _SenderEmail;
            }
        }

        private static string _UploadFile = GetAppSetting("UploadFile");

        /// <summary>
        /// 上传文件目录, 物理地址
        /// </summary>
        public static string UploadFile
        {
            get { return _UploadFile; }
        }

        private static string _protected = GetAppSetting("ProtectedFiles");
        /// <summary>
        /// 上传文件目录(受保护的文件,不允许公开访问), 物理地址
        /// </summary>
        public static string ProtectedFiles
        {
            get { return _protected; }
        }

        private static string _txkeaResource = GetAppSetting("TxkeaResource");
        /// <summary>
        /// Txkea资源文件目录
        /// </summary>
        public static string TxkeaResource
        {
            get { return _txkeaResource; }
        }

        private static string _tempFile = GetAppSetting("TempPDF");

        /// <summary>
        /// 用户存放student邮件附件内容的临时目录
        /// </summary>
        public static string TempFile
        {
            get { return _tempFile; }
        }


        private static string _CatchFlvImgSize = GetAppSetting("CatchFlvImgSize");
        public static string CatchFlvImgSize
        {
            get
            {
                return _CatchFlvImgSize;
            }
        }


        private static string _Ffmpeg = GetAppSetting("Ffmpeg");
        /// <summary>
        /// Ffmpeg文件存放的物理地址
        /// </summary>
        public static string Ffmpeg
        {
            get
            {
                return _Ffmpeg;
            }
        }


        private static string _LDAPUrl = GetAppSetting("LDAPUrl");

        public static string LDAPUrl
        {
            get
            {
                return _LDAPUrl;
            }
        }

        private static string _LDAP = GetAppSetting("LDAP");

        public static string LDAP
        {
            get
            {
                return _LDAP;
            }
        }

        private static int _ExpirationTime = GetAppSettingOutInt("ExpirationTime");

        public static int ExpirationTime
        {
            get
            {
                return _ExpirationTime;
            }
        }

        /// <summary>
        /// Mvc.Information
        /// </summary>
        private static string _ResourceFile = GetAppSetting("ResourceFile");

        public static string ResourceFile
        {
            get { return _ResourceFile; }
        }
        /// <summary>
        /// Mvc
        /// </summary>
        private static string _ResourceAssembly = GetAppSetting("ResourceAssembly");

        public static string ResourceAssembly
        {
            get { return _ResourceAssembly; }
        }

        private static string _staticDomain = GetAppSetting("StaticDomain");
        public static string StaticDomain
        {
            get { return _staticDomain; }
        }
        private static string _mainDomain = GetAppSetting("MainSiteDomain");
        public static string MainSiteDomain
        {
            get { return _mainDomain; }
        }
        private static string _assDomain = GetAppSetting("AssessmentDomain");
        public static string AssessmentDomain
        {
            get { return _assDomain; }
        }
        private static string _ssoDomain = GetAppSetting("SsoDomain");
        public static string SsoDomain
        {
            get { return _ssoDomain; }
        }

        private static string _lmsDomain = GetAppSetting("LMSDomain");
        public static string LMSDomain { get { return _lmsDomain; } }

        private static string _vcwDomain = GetAppSetting("VcwDomain");
        public static string VcwDomain { get { return _vcwDomain; } }

        private static string _practiceDomain = GetAppSetting("PracticeDomain");
        public static string PracticeDomain { get { return _practiceDomain; } }

        private static string _cacDomain = GetAppSetting("CACDomain");
        public static string CACDomain { get { return _cacDomain; } }

        private static string _missingFunding = GetAppSetting("MissingFunding");
        public static string MissingFunding
        {
            get { return _missingFunding; }
        }

       


        private static string _twitterAppKey = GetAppSetting("TwitterKey");
        private static string _twitterAppSecret = GetAppSetting("TwitterSecret");
        private static string _twitterUrl_ApiBase = GetAppSetting("TwitterUrl_ApiBase");
        private static string _twitterUrl_Oahth2Token = GetAppSetting("TwitterUrl_Oauth2Token");
        private static string _twitterUrl_UserTimeline = GetAppSetting("TwitterUrl_UserTimeline");
        private static string _twitter_ScreenName = GetAppSetting("Twitter_ScreenName");
        public static string TwitterKey
        {
            get { return _twitterAppKey; }
        }
        public static string TwitterSecret
        {
            get { return _twitterAppSecret; }
        }
        public static string TwitterUrl_ApiBase
        {
            get { return _twitterUrl_ApiBase; }
        }
        public static string TwitterUrl_Oauth2Token
        {
            get { return _twitterUrl_Oahth2Token; }
        }
        public static string TwitterUrl_UserTimeline
        {
            get { return _twitterUrl_UserTimeline; }
        }
        public static string Twitter_ScreenName
        {
            get { return _twitter_ScreenName; }
        }


        #region login
        private static string _measureId = GetAppSetting("MeasureId");
        public static string MeasureId
        {
            get { return _measureId; }
        }

        private static string _enableAccessManager = GetAppSetting("EnableAccessManager");
        public static string EnableAccessManager
        {
            get { return _enableAccessManager; }
        }

        private static string _homeRedirectPage = GetAppSetting("HomeRecirectPage");
        public static string HomeRedirectPage
        {
            get { return _homeRedirectPage; }
        }

        private static string _enableLMS = GetAppSetting("EnableLMS");
        public static string EnableLMS
        {
            get { return _enableLMS; }
        }

        private static string _accessManagerAdminUrl = GetAppSetting("AccessManagerAdminUrl");
        public static string AccessManagerAdminUrl
        {
            get { return _accessManagerAdminUrl; }
        }

        private static string _accessManagerUrl = GetAppSetting("AccessManagerUrl");
        public static string AccessManagerUrl
        {
            get { return _accessManagerUrl; }
        }

        private static string _accessManagerLogoutUrl = GetAppSetting("AccessManagerLogoutUrl");
        public static string AccessManagerLogoutUrl
        {
            get { return _accessManagerLogoutUrl; }
        }

        private static string _google_ClientSecret = GetAppSetting("Google_ClientSecret");
        public static string Google_ClientSecret
        {
            get { return _google_ClientSecret; }
        }

        private static string _google_ClientID = GetAppSetting("Google_ClientID");
        public static string Google_ClientID
        {
            get { return _google_ClientID; }
        }

        private static string _google_RedirectUri = GetAppSetting("Google_RedirectUri");
        public static string Google_RedirectUri
        {
            get { return _google_RedirectUri; }
        }
        #endregion

        #region TRS

        /// <summary>
        /// TRS Results报表中,Strutual item 为not met 的id
        /// </summary>
        private static string _notMetId = GetAppSetting("NotMetId");
        public static string NotMetId
        {
            get { return _notMetId; }
        }
        #endregion

        #region VCW

        /// <summary>
        /// 上传文件时，需要配置输入框的选项
        /// </summary>
        private static string _contextOther = GetAppSetting("ContextOther");
        public static string ContextOther
        {
            get { return _contextOther; }
        }

        /// <summary>
        /// 上传文件时，需要配置输入框的选项
        /// </summary>
        private static string _strategyOther = GetAppSetting("StrategyOther");
        public static string StrategyOther
        {
            get { return _strategyOther; }
        }

        /// <summary>
        /// 上传文件时，需要配置输入框的选项
        /// </summary>
        private static string _videoContentOther = GetAppSetting("VideoContentOther");
        public static string VideoContentOther
        {
            get { return _videoContentOther; }
        }

        /// <summary>
        /// 发送任务时，需要配置输入框的选项
        /// </summary>
        private static string _assignmentContentOther = GetAppSetting("AssignmentContentOther");
        public static string AssignmentContentOther
        {
            get { return _assignmentContentOther; }
        }

        /// <summary>
        /// 下载文件块的大小
        /// </summary>
        private static int _chunkSize = GetAppSettingOutInt("ChunkSize");
        public static int ChunkSize
        {
            get { return _chunkSize; }
        }

        #endregion

        #region Time Periods For Sync Users Profile To LMS System

        private static string _syncTimeToLMS = GetAppSetting("SyncTimeToLms");
        public static string SyncTimeToLms
        {
            get { return _syncTimeToLMS; }
        }

        #endregion

        /// <summary>
        /// Assessment 当做Page 写入 Page表 时，加上该值
        /// </summary>
        public static int AssessmentPageStartId
        {
            get { return 10000; }
        }

        private static int _reportExpire = GetAppSettingOutInt("ReportExpire");
        /// <summary>
        /// 报表过期时间 Days
        /// </summary>
        public static int ReportExpire
        {
            get { return _reportExpire; }
        }

        private static string _cacheFileDependency_Permission = System.IO.Path.Combine(_protected, "CacheFileDependency_Permission.txt");
        public static string CacheFileDependency_Permission
        {
            get { return _cacheFileDependency_Permission; }
        }

        private static string _cacheFileDependency_VcwTool = System.IO.Path.Combine(_protected, "CacheFileDependency_VcwTool.txt");
        public static string CacheFileDependency_VcwTool
        {
            get { return _cacheFileDependency_VcwTool; }
        }

        private static string _excludedCommunityForReport = GetAppSetting("ExcludedCommunityForReport");
        public static List<int> ExcludedCommunityForReport
        {
            get
            {
                List<int> communityIds = new List<int>();
                foreach (var v in _excludedCommunityForReport.Split(','))
                {
                    int tmpId;
                    if (int.TryParse(v, out tmpId))
                        communityIds.Add(tmpId);
                }
                return communityIds;
            }
        }
    }
}