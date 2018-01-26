using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sunnet.Cli.Core.Ade;

namespace Sunnet.Cli.UIBase.Models
{
    public static class ReportHelper
    {
        public static int GetChartWidth(int count, int barWidth = 80, int minWidth = 800, int maxWidth = 3250)
        {
            var chartWidth = 160 + count * barWidth;
            if (chartWidth > maxWidth)
            {
                chartWidth = maxWidth;
            }
            if (chartWidth < minWidth)
            {
                chartWidth = minWidth;
            }
            return chartWidth;
        }

        public static string GetReportTitle(AssessmentType type, string title)
        {
            return title;
        }

        /// <summary>
        /// 格式化系统报表名字.
        /// </summary>
        /// <param name="reportName">Name of the report.</param>
        /// <param name="date">The date.</param>
        /// <returns></returns>
        public static string FormatReportName(this string reportName, DateTime date)
        {
            var nameAndExt = reportName.Split(".".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            var name = string.Join(".", nameAndExt.Take(nameAndExt.Length - 1));
            return string.Format("{0}_{1}.{2}", name, date.ToString("yyyyMMdd_HHmmss"), nameAndExt.Last());
        }
    }
}