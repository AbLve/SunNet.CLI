using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		JackZhang
 * Computer:		JACKZ
 * Domain:			JackZ
 * CreatedOn:		1/30 2015 12:05:44
 * Description:		Please input class summary
 * Version History:	Created,1/30 2015 12:05:44
 * 
 * 
 **************************************************************************/
using Sunnet.Cli.Business.Common;
using Sunnet.Cli.Core.Ade;
using Sunnet.Cli.Core.Cot;

namespace Sunnet.Cli.Business.Cot.Report
{
    public class CotItemModel
    {
        public int MeasureId { get; set; }

        /// <summary>
        /// 导出所有学校的平均报表使用
        /// </summary>
        public int SchoolId { get; set; }

        public int TeacherId { get; set; }

        public int Id { get; set; }


        public DateTime BoyDate { get; set; }

        public DateTime MoyDate { get; set; }

        public DateTime MetDate { get; set; }

        public DateTime SetDate { get; set; }

        public DateTime CotUpdatedOn { get; set; }

        public bool NeedSupport { get; set; }

        public bool IsFillColor
        {
            get
            {
                return BoyDate > CommonAgent.MinDate
                    || MoyDate > CommonAgent.MinDate
                    || CotUpdatedOn > CommonAgent.MinDate
                    || MetDate > CommonAgent.MinDate;
            }
        }

        /// <summary>
        /// 使用最小的日期作为统计日期
        /// </summary>
        public DateTime CompareDate
        {
            get
            {
                var date = DateTime.MaxValue;
                if (date >= BoyDate && BoyDate > CommonAgent.MinDate)
                    date = BoyDate;
                if (date >= MoyDate && MoyDate > CommonAgent.MinDate)
                    date = MoyDate;
                if (date >= MetDate && MetDate > CommonAgent.MinDate)
                    date = MetDate;
                if (date >= CotUpdatedOn && CotUpdatedOn > CommonAgent.MinDate)
                    date = CotUpdatedOn;
                return date;
            }
        }
    }
}
