using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/**************************************************************************
 * Developer: 		JackZhang
 * Computer:		JACKZ
 * Domain:			Jackz
 * CreatedOn:		2014/8/12 3:04:39
 * Description:		Please input class summary
 * Version History:	Created,2014/8/12 3:04:39
 * 
 * 
 **************************************************************************/
namespace Sunnet.Cli.Core.Ade
{
    /// <summary>
    /// AssessmentTemplate Type, Assessment Type
    /// </summary>
    public enum AssessmentType : byte
    {
        [Description("CPALLS+")]
        Cpalls = 1,
        [Description("COT")]
        Cot = 2,
        [Description("CEC")]
        Cec = 3,
        [Description("Observables")]
        UpdateObservables = 4 
    }
}
