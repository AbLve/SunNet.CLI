using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


/**************************************************************************
 * Developer: 		JackZhang
 * Computer:		JACKZ
 * Domain:			JackZ
 * CreatedOn:		2014/12/20 9:43:09
 * Description:		Please input class summary
 * Version History:	Created,2014/12/20 9:43:09
 * 
 * 
 **************************************************************************/

namespace Sunnet.Cli.Core.Cot
{
    public enum CotStgReportStatus : byte
    {
        Initialised = 1,

        Saved = 10,

        /// <summary>
        /// 此状态取消了
        /// </summary>
        Completed = 20,

        /// <summary>
        /// Reset Short Term Goals
        /// </summary>
        Deleted = 40
    }
}
