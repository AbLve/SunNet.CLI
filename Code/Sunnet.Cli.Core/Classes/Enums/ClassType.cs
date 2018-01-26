using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		Leason
 * Computer:		Leason-PC
 * Domain:			Leason-pc
 * CreatedOn:		2014/8/27 14:48:00
 * Description:		Create DayType
 * Version History:	Created,2014/8/27 14:48:00
 * 
 * 
 **************************************************************************/
namespace Sunnet.Cli.Core.Classes.Enums
{
    public enum ClassType:byte
    {
        Inclusion=1,

        [Description("Non-Inclusion")]
        NonInclusion=2,

        [Description("Pre-School Programs for Children with Disabilities")]
        Ppcd=3,

        [Description("Non Applicable")]
        NonApplicable = 4
    }
}
