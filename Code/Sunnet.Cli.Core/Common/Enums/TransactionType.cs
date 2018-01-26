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
 * CreatedOn:		2014/8/19 14:26:23
 * Description:		Please input class summary
 * Version History:	Created,2014/8/19 14:26:23
 * 
 * 
 **************************************************************************/
namespace Sunnet.Cli.Core.Common.Enums
{
    public enum TransactionType : byte
    {
        [Description("Incentive")]
        Incentive = 1,
        [Description("Substitute")]
        Substitute = 2
    }
}
