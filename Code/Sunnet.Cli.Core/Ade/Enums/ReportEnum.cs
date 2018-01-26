using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Sunnet.Cli.Core.Ade
{
    public enum ReportEnum : byte
    {
        Completion = 1,
        Summary = 2,
        Growth = 3,
        [Description("Percentile Rank")]
        PercentileRank = 4,
        [Description("Comparision of Growth")]
        ComparisionofGrowth = 5,
        Parent = 6,
        [Description("Custom Score Report")]
        CustomScoreReport = 7,
        [Description("School Custom Score Report")]
        SchoolCustomScoreReport = 8,
        [Description("Community Custom Score Report")]
        CommunityCustomScoreReport = 9
    }
}
