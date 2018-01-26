using System.ComponentModel;

namespace Sunnet.Cli.Core.Trs.Enums
{
    public enum TrsEventType : byte
    {
        [Description("SIA reconsideration")]
        SIA_reconsideration = 1,

        [Description("TRS probation")]
        TRS_probation = 2,

        [Description("TA update")]
        TA_update = 3,

        [Description("Star Level Change")]
        Star_Level_Change = 4,

        [Description("General update")]
        General_update = 5,

        [Description("Status update")]
        Status_update = 6,

        [Description("Auto Assign")]
        Auto_Assign = 7,

        //Database has the value of 8, unknow...

        [Description("Request Validation")]
        Request_Validation = 9,

        [Description("Validation Complete")]
        Validation_Complete = 10

    }
}
