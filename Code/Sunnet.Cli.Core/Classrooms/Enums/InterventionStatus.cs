using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Sunnet.Cli.Core.Classrooms.Enums
{
    public enum InterventionStatus:byte
    {
        [Description("Engage Only")]
        EngageOnly=1,

        [Description("Materials Eligible")]
        MaterialsEligible=2,

        [Description("Other")] 
        Other=3,

        [Description("Non Applicable")]
        NonApplicable = 4
    }
}
