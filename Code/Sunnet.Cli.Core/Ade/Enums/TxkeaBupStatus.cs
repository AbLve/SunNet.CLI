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
 * CreatedOn:		2015/12/16
 * Description:		
 * Version History:	Created,2015/12/16
 * 
 * 
 **************************************************************************/

namespace Sunnet.Cli.Core.Ade.Enums
{
    public enum TxkeaBupStatus : byte
    {
        [Description("In progress")]
        Inprogress = 1,
        Completed = 2,
        Error = 3
    }
}
