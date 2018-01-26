using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		Joe
 * Computer:		JOE-PC
 * Domain:			Joe
 * CreatedOn:		2015/10/23
 * Description:		
 * Version History:	Created,2015/10/23
 * 
 * 
 **************************************************************************/

namespace Sunnet.Cli.Core.Ade.Enums
{
    public enum BreakCondition : byte
    {
        [Description("Stop Condition")]
        StopCondition = 1,
        [Description("Incorrect Response")]
        IncorrectResponse = 2,
        [Description("None")]
        None = 3
    }
}
