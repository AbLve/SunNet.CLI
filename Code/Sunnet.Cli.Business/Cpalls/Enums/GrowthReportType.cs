using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		JackZhang
 * Computer:		JACKZ
 * Domain:			Jackz
 * CreatedOn:		2014/10/20 3:00:39
 * Description:		Please input class summary
 * Version History:	Created,2014/10/20 3:00:39
 * 
 * 
 **************************************************************************/

namespace Sunnet.Cli.Business.Cpalls
{
    /// <summary>
    /// 请谨慎修改此类型，因为有视图文件名与类型相关联
    /// </summary>
    public enum GrowthReportType
    {
        [Description("Average Scores")]
        Average = 10,

        [Description("Benchmark Report")] 
        MeetingBenchmark=20
    }
}
