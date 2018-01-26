using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		Lee
 * Computer:		Lee-PC
 * Domain:			Lee-pc
 * CreatedOn:		2014/10/21
 * Description:		Create AuthorityEntity
 * Version History:	Created,2014/10/21
 * 
 * 
 **************************************************************************/

namespace Sunnet.Cli.Core.Vcw.Enums
{
    public enum AssignmentStatus : byte
    {
        [Description("New")]
        New = 1,

        [Description("Pending")]
        Pending = 2,

        [Description("Completed")]
        Completed = 3,

        [Description("Rejected")]
        Rejected = 4,
    }
}
