using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sunnet.Cli.Core.Ade;
using Sunnet.Cli.Core.Cpalls.Entities;
using Sunnet.Cli.Core.Reports.Models;
using Sunnet.Framework.Core.Base;
using Sunnet.Cli.Core.Cpalls.Models.Report;

/**************************************************************************
 * Developer: 		JackZhang
 * Computer:		JACKZ
 * Domain:			Jackz
 * CreatedOn:		2014/9/4 4:22:23
 * Description:		Please input class summary
 * Version History:	Created,2014/9/4 4:22:23
 * 
 * 
 **************************************************************************/


namespace Sunnet.Cli.Core.Cpalls.Interfaces
{
    public interface IStudentMeasureRpst : IRepository<StudentMeasureEntity, int>
    {

        int InitMeasures(int userId, int assessmentId, string schoolYear, int studentId,
            DateTime studentBirthday, Wave wave, IEnumerable<int> measureIds);

        void RecalculateParentGoal(int saId, int parentMeasureId = 0);

        /// <summary>
        /// communityId ==0 时，表示，按SchoolId 查寻
        /// </summary>
        List<CompletionMeasureModel> GetCompletionCombinedStudentMeasure(int communityId, int schoolId, int assessmentId, int otherAssessmentId
            , Wave wave, List<int> measureIdList, List<int> hasChilderMeasureId, string schoolYear, DateTime startDate, DateTime endDate, DateTime dobStartDate, DateTime dobEndDate, IList<int> classIds, out List<CompletionMeasureModel> otherList);

        /// <summary>
        /// communityId ==0 时，表示，按SchoolId 查询，又语报表，处理只是英语或者西班牙语的学生
        /// </summary>
        List<CompletionMeasureModel> GetCompletionEnglishAndSpanishStudentMeasure(int communityId, int schoolId, int assessmentId, int otherAssessmentId
            , Wave wave, List<int> measureIdList, string schoolYear, DateTime startDate, DateTime endDate, DateTime dobStartDate, DateTime dobEndDate, IList<int> classIds);

        /// <summary>
        /// communityId ==0 时，表示，按SchoolId 查寻 ,完成报表，English 版或 Spanlish
        /// </summary>
        List<CompletionMeasureModel> GetCompletionStudentMeasure(int communityId, int schoolId, int assessmentId
            , Wave wave, List<int> measureIdList, string schoolYear, DateTime startDate, DateTime endDate, DateTime dobStartDate, DateTime dobEndDate, StudentAssessmentLanguage language, IList<int> classIds);

        List<CircleDataExportStudentMeasureModel> GetCircleDataExportStudentMeasureModels(int communityId, string year,
            int schoolId, List<int> waves, List<int> measures);

        List<CircleDataExportStudentMeasureModel> GetCircleDataExportStudentMeasureModelsWithItems(int communityId, string year,
            int schoolId, List<int> waves, List<int> measures, List<int> measuresIncludeItems, List<ItemType> types, DateTime startDate, DateTime endDate);

        List<WaveFinishedOnModel> GetWaveFinishedDate(QueryLevel level, int objectId);

        int UpdateBenchmark(int studentMeasureId, int benchmarkId, decimal lowerScore, decimal higherScore, bool ShowOnGroup, bool benchmarkChanged);

        int UpdatePercentileRank(int studentMeasureId, string percentileRank);

        int UpdateBenchmarkChangedToFalse(int measureId);
    }
}
