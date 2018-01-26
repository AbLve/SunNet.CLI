using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/**************************************************************************
 * Developer: 		Sam
 * Computer:		SAM-PC
 * Domain:			sam-pc
 * CreatedOn:		2014/11/8 12:10:16
 * Description:		Please input class summary
 * Version History:	Created,2014/11/8 12:10:16
 * 
 * 
 **************************************************************************/
namespace Sunnet.Cli.Core.Reports
{
    public enum DataExportStatus : byte
    {
        /// <summary>
        /// 等待处理
        /// </summary>
        Pending = 1,

        /// <summary>
        /// 处理中
        /// </summary>
        Processing = 2,

        /// <summary>
        /// 处理过的
        /// </summary>
        Processed = 3,

        /// <summary>
        /// 出错
        /// </summary>
        Error = 4
    }
}
