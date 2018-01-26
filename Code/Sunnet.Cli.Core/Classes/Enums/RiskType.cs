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
 * Description:		Create ClassType
 * Version History:	Created,2014/8/27 9:40:20
 * 
 * 
 **************************************************************************/
namespace Sunnet.Cli.Core.Classes.Enums
{
    public enum RiskType:byte
    {
        [Description("Same as School")]
        SameasSchool=1,
        [Description("Different from School")]
        DifferentfromSchool
    }
}
