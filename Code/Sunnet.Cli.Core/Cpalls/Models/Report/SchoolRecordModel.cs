using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		JackZhang
 * Computer:		JACKZ
 * Domain:			Jackz
 * CreatedOn:		2014/10/9 3:01:23
 * Description:		Please input class summary
 * Version History:	Created,2014/10/9 3:01:23
 * 
 * 
 **************************************************************************/
using Sunnet.Framework.Extensions;

namespace Sunnet.Cli.Core.Cpalls.Models.Report
{
    public class SchoolRecordModel
    {
        public int SchoolId { get; set; }
        public int MeasureId { get; set; }
        public Wave Wave { get; set; }
        public decimal TotalScore { get; set; }

        public int PercentileRankTotal { get; set; }
        /// <summary>
        /// 已完成学生数.
        /// </summary> 
        public int Count { get; set; }

        /// <summary>
        /// 及格数量.
        /// </summary> 
        public int Satisfied { get; set; }

        public string Average
        {
            get
            {
                if (Count == 0)
                    return "-";
                return (TotalScore / Count).ToPrecisionString(2);
            }
        }
        public decimal AverageForCalc
        {
            get
            {
                if (Count == 0)
                    return 0;
                return TotalScore / Count;
            }
        }

        public string Satisfactory
        {
            get
            {
                if (Count == 0)
                    return "-";
                return SatisfactoryForCalc.ToPercentage(2);
            }
        }

        public decimal SatisfactoryForCalc
        {
            get
            {
                if (Count == 0)
                    return 0;
                return Convert.ToDecimal(Satisfied) / Count;
            }
        }

        public int BenchmarkId { get; set; }

    }
}
