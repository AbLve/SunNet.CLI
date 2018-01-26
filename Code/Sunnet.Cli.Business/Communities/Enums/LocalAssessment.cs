using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Cli.Business.Communities.Enums
{
    public enum LocalAssessment
    {
        [Description("Online Courses")]
        OnlineCourses = -1,

        [Description("Texas Rising Star")]
        TexasRisingStar = -2,

        [Description("BEECH")]
        BEECH = -3,

        [Description("CIRCLE Activity Collection")]
        CIRCLEActivityCollection = -4,

        [Description("ADE")]
        ADE = -5,

        [Description("Collaborative Tools")]
        CollaborativeTools = -6,

        [Description("Automation")]
        Automation = -7

    }
}
