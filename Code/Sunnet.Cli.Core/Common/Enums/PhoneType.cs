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
 * CreatedOn:		2014/8/19 14:36:55
 * Description:		Please input class summary
 * Version History:	Created,2014/8/19 14:36:55
 * 
 * 
 **************************************************************************/
namespace Sunnet.Cli.Core.Common.Enums
{
    public enum PhoneType:byte
    {

        [Description("Home Number")]
        HomeNumber = 1,

        [Description("Cell Number")]
        CellNumber = 2,

        [Description("Work Number")]
        WorkNumber = 3,

        [Description("Non Applicable")]
        NonApplicable=4
    }
}
