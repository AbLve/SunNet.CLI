using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		JackZhang
 * Computer:		JACKZ
 * Domain:			Jackz
 * CreatedOn:		2014/10/12 23:13:21
 * Description:		Please input class summary
 * Version History:	Created,2014/10/12 23:13:21
 * 
 * 
 **************************************************************************/
using Sunnet.Framework.Extensions;

namespace Sunnet.Cli.Core.Cpalls.Models.Report
{
    public class ClassRecordModel
    {
        public int ClassId { get; set; }
        public int MeasureId { get; set; }
        public Wave Wave { get; set; }
        public decimal TotalScore { get; set; }
        public int Count { get; set; }

        public string Average
        {
            get
            {
                if (Count == 0)
                    return "-";
                return (TotalScore / Count).ToPrecisionString(2);
            }
        }
    }
}
