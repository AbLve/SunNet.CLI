using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Cli.Core.Schools.Enums
{
    public enum StarAssessmentType : byte
    {
        Initial = 1,

        Recertification = 2,

        [Description("Facility Changes")]
        FacilityChanges = 3,

        [Description("Category Reassessment")]
        CategoryReassessment = 4,

        [Description("Star Level Evaluation")]
        StarLevelEvaluation = 5,

        [Description("Annual Monitoring")]
        AnnualMonitoring = 6,

        [Description("Star Level Change")]
        Star_Level_Change = 7,

        [Description("Auto Assign")]
        Auto_Assign = 8
    }
}
