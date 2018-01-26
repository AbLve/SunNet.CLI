using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


/**************************************************************************
 * Developer: 		JackZhang
 * Computer:		JACKZ
 * Domain:			JackZ
 * CreatedOn:		1/15 2015 14:02:05
 * Description:		Please input class summary
 * Version History:	Created,1/15 2015 14:02:05
 * 
 * 
 **************************************************************************/

namespace Sunnet.Cli.Core.Trs
{
    public enum TrsTaStatus
    {
        [Description("TA Plan")]
        TaPlan = 1,
        [Description("SIA")]
        Sia = 2,
        [Description("TRS Probation")]
        TrsProbation = 3,
        [Description("N/A")]
        None = 4
    }
}
