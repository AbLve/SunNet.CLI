using System;
using System.ComponentModel;
using Sunnet.Framework;

namespace Sunnet.Cli.UIBase.Models
{
    public enum Cli_Menus
    {
        Home = 0,
        [Description("About Us")]
        AboutUs = 1,
        [Description("Engage Overview")]
        EngageOverview = 2,
        [Description("FAQ")]
        Faq = 3,
        [Description("Contact Us")]
        ContactUs = 4,
        [Description("Parents")]
        EngageParents = 5,
        [Description("Tools")]
        Tools = 6,
        [Description("Calendar")]
        Calendar = 7,
        [Description("Help")]
        Helps = 8,
        [Description("Search")]
        Search = 9,
        [Description("Resources")]
        Resources=10,
        [Description("TakeALook")]
        TakeALook = 11,
        [Description("Eligibility")]
        Eligibility = 12,
        [Description("Dashboard")]
        Dashboard = 13

    }

    /// <summary>
    /// 各个子域的地址相关
    /// </summary>
    public static class DomainHelper
    {
        public static Uri MainSiteDomain { get; set; }
        public static Uri StaticSiteDomain { get; set; }
        public static Uri SsoSiteDomain { get; set; }
        public static Uri AssessmentDomain { get; set; }
        public static Uri LMSDomain { get; set; }
        public static Uri VcwDomain { get; set; }
        public static Uri PracticeDomain { get; set; }
        public static Uri CACDomain { get; set; }
        public static string Main_SITE_Backurl { get; set; }
        public static string Assessment_SITE_Backurl { get; set; }
        public static string Vcw_SITE_Backurl { get; set; }

        public static string Practice_SITE_Backurl { get; set; }

        public static string SSO_SITE_CLIBackurl { get; set; }
        public static string SSO_SITE_ExtendBackurl { get; set; }

        static DomainHelper()
        {
            MainSiteDomain = new Uri(SFConfig.MainSiteDomain);
            StaticSiteDomain = new Uri(SFConfig.StaticDomain);
            SsoSiteDomain = new Uri(SFConfig.SsoDomain);
            AssessmentDomain = new Uri(SFConfig.AssessmentDomain);
            LMSDomain = new Uri(SFConfig.LMSDomain);
            VcwDomain = new Uri(SFConfig.VcwDomain);
            PracticeDomain = new Uri(SFConfig.PracticeDomain);

            CACDomain = new Uri(SFConfig.CACDomain);
            Main_SITE_Backurl = string.Format("{0}home/CallBack", DomainHelper.MainSiteDomain.ToString());
            Assessment_SITE_Backurl = string.Format("{0}AssessmentHome/CallBack", DomainHelper.AssessmentDomain.ToString());
            Vcw_SITE_Backurl = string.Format("{0}home/CallBack", DomainHelper.VcwDomain.ToString());

            Practice_SITE_Backurl = string.Format("{0}home/CallBack", DomainHelper.PracticeDomain.ToString());

            SSO_SITE_CLIBackurl = string.Format("{0}home/CallBackCLI", DomainHelper.SsoSiteDomain.ToString());
            SSO_SITE_ExtendBackurl = string.Format("{0}home/CallBackExtend", DomainHelper.SsoSiteDomain.ToString());
        }
    }
}