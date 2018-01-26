using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


/**************************************************************************
 * Developer: 		JackZhang
 * Computer:		JACKZ
 * Domain:			Jackz
 * CreatedOn:		2014/11/5 2:46:29
 * Description:		Please input class summary
 * Version History:	Created,2014/11/5 2:46:29
 * 
 * 
 **************************************************************************/

namespace Sunnet.Cli.Business.Cpalls.Models
{
    //此类已映射到Module_CpallsOffline.js
    public static class ReportTheme
    {
        /// <summary>
        /// 未找到匹配的Bentchmark(html 样式类名)
        /// </summary>
        public static string Missing_Bentchmark_ClassName { get { return "cpalls_no_benchmark"; } }

        /// <summary>
        /// 未找到匹配的Bentchmark(Excel 背景色)
        /// http://msdn.microsoft.com/en-us/library/cc296089.aspx
        /// </summary>
        public static string Missing_Bentchmark_Color { get { return "23"; } }

        /// <summary>
        /// 四岁（包括）以上(html 样式类名)
        /// </summary>
        public static string GE4_ClassName { get { return "cpalls_four"; } }
        /// <summary>
        /// 四岁（包括）以上(html 样式类名)亮色
        /// </summary>
        public static string GE4_Light_ClassName { get { return "cpalls_four_light"; } }

        /// <summary>
        /// 四岁（包括）以上(Excel 背景色)
        /// </summary>
        public static string GE4_Color { get { return "3"; } }

        /// <summary>
        /// 3.5-3.9岁(html 样式类名)
        /// </summary>
        public static string TE4_ClassName { get { return "cpalls_three"; } }
        /// <summary>
        /// 3.5-3.9岁(html 样式类名)亮色
        /// </summary>
        public static string TE4_Light_ClassName { get { return "cpalls_three_light"; } }

        /// <summary>
        /// 3.5-3.9岁(Excel 背景色)
        /// </summary>
        public static string TE4_Color { get { return "45"; } }

        /// <summary>
        /// 3.5岁以下(html 样式类名)
        /// </summary>
        public static string TE3_ClassName { get { return "cpalls_three_less"; } }
        /// <summary>
        /// 3.5岁以下(html 样式类名)亮色
        /// </summary>
        public static string TE3_Light_ClassName { get { return "cpalls_three_less_light"; } }

        /// <summary>
        /// 3.5岁以下(Excel 背景色)
        /// </summary>
        public static string TE3_Color { get { return "0"; } }

        /// <summary>
        /// 通过(html 样式类名)
        /// </summary>
        public static string Passed_ClassName { get { return "cpalls_normal"; } }
        /// <summary>
        /// 通过(html 样式类名)亮色
        /// </summary>
        public static string Passed_Light_ClassName { get { return "cpalls_normal_light"; } }

        /// <summary>
        /// 通过(Excel 背景色)
        /// </summary>
        public static string Passed_Color { get { return "10"; } }

        public static string AveragePerSource_ClassName
        {
            get { return "title-strong"; }
        }

    }
}
