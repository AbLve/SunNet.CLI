using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Cli.Core.Reports
{
    public enum ReportQueueType : byte
    {
        /// <summary>
        /// 需要调用 Sunnet.Cli.Business.Cpalls.CpallsBusiness.GetSchoolSummaryReport 处理
        /// 参数:
        /// int assessmentId, UserBaseEntity user, int year, int districtId, Dictionary&lt;Wave, IEnumerable&lt;int&gt;&gt; selectedWaves
        /// </summary>
        [Description("Community Summary Average Report")]
        SummaryWithAverge_Community = 1,

        /// <summary>
        /// 需要调用 Sunnet.Cli.Business.Cpalls.CpallsBusiness.GetSchoolSatisfactoryReport 处理
        /// 参数:
        /// int assessmentId, UserBaseEntity user, int year, int districtId, Dictionary&lt;Wave, IEnumerable&lt;int&gt;&gt; selectedWaves
        /// </summary>
        [Description("Community Summary Benchmark Report")]
        Percentage_Community = 2,

        /// <summary>
        /// 需要调用 Sunnet.Cli.Business.Cpalls.CpallsBusiness.GetReport_Community 处理
        /// 参数:
        /// int assessmentId, UserBaseEntity user, int year, Dictionary&lt;Wave, IEnumerable&lt;int&gt;&gt; selectedWaves
        /// </summary>
        [Description("Community Completion Report")]
        Community_Completion_Report = 3,

        /// <summary>
        /// The circl e_ data_ export
        /// </summary>
        [Description("CIRCLE Data Export")]
        CIRCLE_Data_Export = 4,

        /// <summary>
        /// 需要调用 Sunnet.Cli.Business.Cpalls.CpallsBusiness.GetSchoolSummaryReport 处理
        /// 参数:
        /// int assessmentId, UserBaseEntity user, int year, int districtId, Dictionary&lt;Wave, IEnumerable&lt;int&gt;&gt; selectedWaves
        /// </summary>
        [Description("School Percentile Rank Averages")]
        SummaryWithAvergePercentileRank_Community = 5,

        /// <summary>
        /// 需要调用 Sunnet.Cli.Business.Cpalls.CpallsBusiness.GetCommunityScoreReportPdf 处理
        /// 参数:
        /// int assessmentId, StudentAssessmentLanguage language, int communityId,
        ///Wave wave, int scoreYear, List<int> scoreIds, DateTime? startDate, DateTime? endDate, DateTime? dobStartDate, DateTime? dobEndDate
        /// </summary>
        [Description("Community Custom Score Report")]
        Community_CustomScoreReport = 6,

        /// <summary>
        /// 需要调用 Sunnet.Cli.Business.Cpalls.CpallsBusiness.GetSchoolScoreReportPdf 处理
        /// 参数:
        /// int assessmentId, StudentAssessmentLanguage language, int schoolId,
        ///Wave wave, int scoreYear, List<int> scoreIds, DateTime? startDate, DateTime? endDate, DateTime? dobStartDate, DateTime? dobEndDate
        /// </summary>
        [Description("School Custom Score Report")]
        School_CustomScoreReport = 7
    }
}
