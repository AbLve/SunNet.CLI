using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json.Converters;

namespace Sunnet.Cli.UIBase.Models
{
    /// <summary>
    /// 处理视图里面的文字显示
    /// </summary>
    public static class ViewTextHelper
    {
        public static string DefaultPleaseSelectText
        {
            get { return "Please select..."; }
        }

        public static string DefaultNAText
        {
            get { return "N/A"; }
        }

        public static string DefaultAllText
        {
            get { return "ALL"; }
        }

        public static string DefaultCountyText
        {
            get { return "County*"; }
        }

        public static string DefaultStateText
        {
            get { return "State*"; }
        }


        public static string NoRecordFound
        {
            get { return "No record found"; }
        }

        public static MvcHtmlString HtmlEmptySeparator
        {
            get { return new MvcHtmlString("&nbsp;&nbsp;&nbsp;&nbsp;"); }
        }

        public static string DemoSchoolName
        {
            get { return "Demo School"; }
        }
        public static string DemoClassName
        {
            get { return "Demo Class"; }
        }

        public static string DemoStudentFirstName
        {
            get { return "Demo"; }
        }
        public static string DemoStudentLastName
        {
            get { return "Student"; }
        }

        /// <summary>
        /// Gets the bool in texts.
        /// </summary>
        /// <value>
        public static Dictionary<bool, string> BoolInTexts
        {
            get
            {
                var bools = new Dictionary<bool, string>();
                bools.Add(true, "Yes");
                bools.Add(false, "No");
                return bools;
            }
        }

        private static DateTime _limitDate = new DateTime(1900, 1, 1);
        public static string FormatDateString(this DateTime date, string format = "MM/dd/yyyy")
        {
            if (date < _limitDate)
                return "";
            return date.ToString(format);
        }

        public static string FormatDetailDateString(this DateTime date, string format = "MM/dd/yyyy HH:mm:ss")
        {
            if (date < _limitDate)
                return " ";
            return date.ToString(format);
        }

        public static string CutString(this string source, int length, string stuffix = " ...")
        {
            if (source == null)
                return string.Empty;
            if (source.Length < length)
                return source;
            return source.Substring(0, length) + stuffix;
        }

        public static string Vcw_UploadTitle
        {
            get { return "doc,picture,video"; }
        }

        public static string Vcw_VideoUploadTitle
        {
            get { return "mp4,wmv,mov,m4v"; }
        }

        public static string Vcw_DescriptionLimit
        {
            get { return "up to 500 characters"; }
        }

        public static string Vcw_UploadLimit
        {
            get { return "up to 10 GB"; }
        }

        public static string Vcw_MinDate
        {
            get { return Sunnet.Cli.Business.Common.CommonAgent.MinDate.ToString("MM/dd/yyyy"); }
        }

        public static string Vcw_MulitipleLinks
        {
            get { return "If there are multiple links, just press enter to wrap"; }
        }
    }
}
