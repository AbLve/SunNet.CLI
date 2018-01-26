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
 * CreatedOn:		2014/8/27 9:40:20
 * Description:		Create DayType
 * Version History:	Created,2014/8/27 9:40:20
 * 
 * 
 **************************************************************************/
namespace Sunnet.Cli.Core.Classes.Enums
{
    public enum DayType:byte
    {
        [Description("A.M.")]
        Am = 1,

        [Description("P.M.")]
        Pm = 2,

        [Description("Full Day")]
        FullDay=3,

        [Description("Non Applicable")]
        NonApplicable = 4
    }
}
