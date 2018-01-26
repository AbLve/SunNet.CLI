using Sunnet.Cli.UIBase.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sunnet.Cli.UIBase
{
    public static class LoginIASID
    {
        /// <summary>
        /// 主站点 01
        /// </summary>
        public const string MAIN = "01";

        /// <summary>
        /// Parent 注册 011
        /// </summary>
        public const string ParentSign = "011";

        /// <summary>
        /// 登陆后返回原Url 012
        /// </summary>
        public const string LostSession = "012";

        /// <summary>
        /// ADE 站点 02
        /// </summary>
        public const string ADE = "02";

        /// <summary>
        /// Assessment 03
        /// </summary>
        public const string ASSESSMENT = "03";

        /// <summary>
        /// CPALLS+ Offline 031
        /// </summary>
        public const string CPALLS_OFFLINE = "031";

        /// <summary>
        /// COT Offline 032
        /// </summary>
        public const string COT_OFFLINE = "032";

        /// <summary>
        /// CEC Offline 033
        /// </summary>
        public const string CEC_OFFLINE = "033";

        /// <summary>
        /// TRS  034
        /// </summary>
        public const string TRS = "034";

        /// <summary>
        /// TRS Offline  035
        /// </summary>
        public const string TRS_OFFLINE = "035";

        /// <summary>
        /// ADE Layout  036
        /// </summary>
        public const string ADE_LAYOUT = "036";

        /// <summary>
        /// ADE BUK  037
        /// </summary>
        public const string ADE_BUK = "037";

        /// <summary>
        /// VCW  04
        /// </summary>
        public const string VCW = "04";    

        /// <summary>
        /// Update Observables  038
        /// </summary>
        public const string  Observables = "038";

        /// <summary>
        /// TSDS 039
        /// </summary>
        public const string TSDS = "039";

        /// <summary>
        /// Assessment Practice Area 040
        /// </summary>
        public const string AssessmentPractice = "040";

        public static bool CheckIASID(string iasid)
        {
            switch (iasid)
            {
                case MAIN:
                case ParentSign:
                case LostSession:
                case ADE:
                case ASSESSMENT:
                case CPALLS_OFFLINE:
                case COT_OFFLINE:
                case CEC_OFFLINE:
                case TRS:
                case TRS_OFFLINE:
                case VCW:
                case Observables:
                case ADE_LAYOUT:
                case ADE_BUK:
                case TSDS:
                    return true;
                default:
                    return false;
            }
        }
    }

    public static class LoginUserType
    {

        /// <summary>
        /// CLI Internal User 1 David 01/12/2015
        /// </summary>
        public const int UTACCESSMANAGER = 1;

        /// <summary>
        /// CLI External User 2  David 01/12/2015
        /// </summary>
        public const int GOOGLEACCOUNT = 2;
    }
}