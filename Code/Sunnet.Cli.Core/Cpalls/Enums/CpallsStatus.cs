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
 * CreatedOn:		2014/9/4 4:22:23
 * Description:		Please input class summary
 * Version History:	Created,2014/9/4 4:22:23
 * 
 * 
 **************************************************************************/


namespace Sunnet.Cli.Core.Cpalls
{
    // 更新时注意更新 Sunnet.Cli.Static\Content\Scripts_cliProject\Module_Cpalls.js
    // 更新时注意更新 Sunnet.Cli.Assessment\Areas\Cpalls\Views\Student\Index.cshtml

    public enum CpallsStatus : byte
    {
        /// <summary>
        /// 初始化:未开始
        /// </summary>
        Initialised = 1,

        /// <summary>
        /// 暂停
        /// </summary>
        Paused = 2,

        /// <summary>
        /// 已执行完毕
        /// </summary>
        Finished = 3,

        /// <summary>
        /// 锁定[Excluded]
        /// </summary>
        [Description("Excluded")]
        Locked = 4
    }
}
