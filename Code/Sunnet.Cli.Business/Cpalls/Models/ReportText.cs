using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


/**************************************************************************
 * Developer: 		JackZhang
 * Computer:		JACKZ
 * Domain:			Jackz
 * CreatedOn:		2014/11/9 19:11:29
 * Description:		Please input class summary
 * Version History:	Created,2014/11/9 19:11:29
 * 
 * 
 **************************************************************************/

namespace Sunnet.Cli.Business.Cpalls.Models
{
    public static class ReportText
    {
        /// <summary>
        /// 没有记录
        /// </summary>
        public static string No_Record
        {
            get { return "-"; }
        }

        /// <summary>
        /// 选中的Wave里面没有选择Measure
        /// </summary>
        public static string No_Choosed
        {
            get { return "-"; }
        }

        /// <summary>
        /// 不计分的Measure
        /// </summary>
        public static string No_TotalScored
        {
            // 对勾 √
            get { return Encoding.Unicode.GetString(new byte[] { 26, 34 }); }
        }

        /// <summary>
        /// 没有学生符合条件. 
        /// </summary>
        public static string No_Benchmark
        {
            get { return "*"; }
        }

        public const string TotalColumnName = "Total";

        public const string TotalColumnNameForSatisfactory= "Overall Measure";

        public static string TotalScore = "Maximum Score";

        public static string AveragePerSource = "Average *";

        public static string SatisfactoryColumnName = "% Meeting Benchmark";

        public static string PercentileRankNA = "N/A";

        private static List<char> illegalChars = new List<char>() { ':', '/', '?', '*', '[', ']' };
        /// <summary>
        /// Clears the illegal chars of the sheet.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        public static string ClearSheetName(this string source)
        {
            var target = string.Empty;
            illegalChars.ForEach(c => target = source.Replace(c, ' '));
            if (target.Length > 31)
                target = target.Substring(0, 31);
            return target;
        }
    }
}
