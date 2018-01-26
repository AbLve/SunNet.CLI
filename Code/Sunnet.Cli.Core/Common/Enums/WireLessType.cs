using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/**************************************************************************
 * Developer: 		Sam
 * Computer:		SAM-PC
 * Domain:			sam-pc
 * CreatedOn:		2014/8/19 16:19:52
 * Description:		Please input class summary
 * Version History:	Created,2014/8/19 16:19:52
 * 
 * 
 **************************************************************************/
namespace Sunnet.Cli.Core.Common.Enums
{
    public enum WireLessType : byte
    {
        [Description("Wireless A")]
        WirelessA = 1,
        [Description("Wireless B")]
        WirelessB = 2,
        [Description("Wireless G")]
        WirelessG = 3,
        [Description("Wireless N")]
        WirelessN = 4,
        Unknown = 5
    }
}
