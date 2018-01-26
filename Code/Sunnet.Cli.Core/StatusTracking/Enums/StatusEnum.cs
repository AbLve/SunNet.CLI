using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		Joe
 * Computer:		Joe-PC
 * Domain:			Joe-pc
 * CreatedOn:		2015/06/03
 * Description:		Create AuthorityEntity
 * Version History:	Created,2015/06/03
 * 
 * 
 **************************************************************************/
using System.ComponentModel;

namespace Sunnet.Cli.Core.StatusTracking.Enums
{
    public enum StatusEnum : byte
    {
        [Description("Pending")]
        Pending = 1,

        [Description("Accepted")]
        Accepted = 2,

        [Description("Denied")]
        Denied = 3
    }

    /// <summary>
    /// 该枚举用于绑定搜索框，多出Deny状态
    /// </summary>
    public enum StatusSearchEnum : byte
    {
        [Description("Pending")]
        Pending = 1,

        [Description("Accepted")]
        Accepted = 2,

        [Description("Denied")]
        Denied = 3,

        [Description("Expired")]
        Expired = 4
    }
}
