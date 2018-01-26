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
 * CreatedOn:		2014/8/22 17:04:00
 * Description:		Please input class summary
 * Version History:	Created,2014/8/22 17:04:00
 * 
 * 
 **************************************************************************/
namespace Sunnet.Cli.Core.Common.Enums
{
    public enum InternetSpeed : byte
    {
        [Description(">1.5 Mbps")]
        LessThan15 = 1,
        [Description("1.6 - 2.5 Mbps")]
        Between16And25 = 2,
        [Description("2.6 - 5.0 Mbps")]
        Between26And50 = 3,
        [Description("5.0 - 8.0 Mbps")]
        Between50And80 = 4,
        [Description("8.0 - 25 Mbps")]
        Between80And250 = 5,
        [Description("25+ Mbps")]
        MoreThan250 = 6
    }
}
