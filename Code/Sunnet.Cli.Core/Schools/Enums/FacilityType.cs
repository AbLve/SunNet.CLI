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
 * CreatedOn:		2015/1/7 16:30:58
 * Description:		Please input class summary
 * Version History:	Created,2015/1/7 16:30:58
 * 
 * 
 **************************************************************************/
namespace Sunnet.Cli.Core.Schools.Enums
{
    public enum FacilityType : byte
    {
        /// <summary>
        /// Licensed Center, All Ages [LC(AA)]
        /// </summary>
        [Description("Licensed Center, All Ages [LC(AA)]")]
        LCAA = 1,
        /// <summary>
        /// Licensed Center, School Age Only [LC(SA)]
        /// </summary>
        [Description("Licensed Center, School Age Only [LC(SA)]")]
        LCSA = 2,
        /// <summary>
        /// Licensed Child Care Home [LC(CH)]
        /// </summary>
        [Description("Licensed Child Care Home [LC(CH)]")]
        LCCH = 3,
        /// <summary>
        /// Registered Child Care Home [RC(CH)]
        /// </summary>
        [Description("Registered Child Care Home [RC(CH)]")]
        RCCH = 4
    }
}
