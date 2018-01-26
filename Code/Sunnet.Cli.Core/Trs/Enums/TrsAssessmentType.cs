using System.ComponentModel;

namespace Sunnet.Cli.Core.Trs
{
    //修改此枚举时，需要修改Module_TRS_Offline.js  AssessmentType, && SchoolEntity StarAssessmentType 枚举
    public enum TrsAssessmentType : byte
    {
        Initial = 1,

        Recertification = 2,

        [Description("Facility Changes")]
        FacilityChanges = 3,

        [Description("Category Reassessment")]
        CategoryReassessment = 4,

        [Description("Star Level Evaluation")]
        StarLevelEvaluation = 5,

        /// <summary>
        /// 不需要更新School的Star
        /// </summary>
        [Description("Annual Monitoring")]
        AnnualMonitoring = 6

    }
}