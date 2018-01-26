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
 * CreatedOn:		2014/8/19 16:17:41
 * Description:		Please input class summary
 * Version History:	Created,2014/8/19 16:17:41
 * 
 * 
 **************************************************************************/
namespace Sunnet.Cli.Core.Common.Enums
{
    public enum InternetType:byte
    {
        Wireless=1,

        Wired=2,

        None=3,

        Unknow=4,

        [Description("Non Applicable")]
        NonApplicable = 5
    }
}
