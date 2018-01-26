using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
using Sunnet.Cli.Core.Ade;
using Sunnet.Cli.Core.Cpalls.Entities;
using Sunnet.Cli.Core.Cpalls.Models;
using Sunnet.Cli.Core.Cpalls.Models.Report;
using Sunnet.Cli.Core.Reports.Models;
using Sunnet.Framework.Core.Tool;


namespace Sunnet.Cli.Core.Cpalls
{
    public interface ICpallsContract
    {
        #region IQueryables

        IQueryable<StudentAssessmentEntity> Assessments { get; }
        IQueryable<StudentMeasureEntity> Measures { get; }
        IQueryable<StudentItemEntity> Items { get; }

        IQueryable<CpallsSchoolEntity> CpallsShools { get; }
        IQueryable<CpallsSchoolMeasureEntity> CpallsSchoolMeasures { get; }
        IQueryable<CpallsClassEntity> CpallsClasses { get; }
        IQueryable<CpallsClassMeasureEntity> CpallsClassMeasures { get; }
        IQueryable<CpallsStudentGroupEntity> CpallsStudentGroups { get; }
        IQueryable<UserShownMeasuresEntity> UserShownMeasures { get; }

        #endregion

        StudentAssessmentEntity NewStudentAssessmentEntity();

        StudentMeasureEntity NewStudentMeasureEntity();

        StudentItemEntity NewStudentItemEntity();


        StudentAssessmentEntity GetStudentAssessment(int id);
        OperationResult InsertStudentAssessment(StudentAssessmentEntity entity);
        OperationResult UpdateStudentAssessment(StudentAssessmentEntity entity);

        OperationResult DeleteStudentAssessment(int id);

        int GetStudentAssessmentIdForPlayMeasure(int assessmentId, int studentId, string schoolYear, int wave);

        StudentMeasureEntity GetStudentMeasure(int id);

        OperationResult InsertStudentMeasure(StudentMeasureEntity entity);

        OperationResult UpdateStudentMeasure(StudentMeasureEntity entity);

        int UpdateBenchmark(int studentMeasureId, int benchmarkId, decimal lowerScore, decimal higherScore, bool ShowOnGroup, bool benchmarkChanged);
        int UpdatePercentileRank(int studentMeasureId, string percentileRank);

        int UpdateBenchmarkChangedToFalse(int measureId);

        OperationResult UpdateStudentMeasures(List<StudentMeasureEntity> entities);

        OperationResult InitMeasures(int userId, int assessmentId, string schoolYear, int studentId,
            DateTime studentBirthday, Wave wave, IEnumerable<int> measureIds);

        void RecalculateParentGoal(int saId, int parentMeasureId = 0);

        OperationResult UpdateStudentItem(StudentItemEntity entity);
        OperationResult UpdateStudentItems(List<StudentItemEntity> entities);


        List<SchoolMeasureGoalModel> GetSchoolMeasureGoal(List<int> schoolId, string schoolYear, Wave wave, int assessmentId,IList<int> classIds );
        List<StudentMeasureGoalModel> GetStudentMeasureGoal(List<int> studentId, string schoolYear, Wave wave, int assessmentId);

        #region Report
        // header

        /// <summary>
        /// 查找所有(未删除的正常状态)的Measure
        /// </summary>
        /// <param name="assessmentId">The assessment identifier.</param>
        /// <returns></returns>
        List<ReportMeasureHeaderModel> GetReportMeasureHeaders(int assessmentId);

        /// <summary>
        /// 查找所有的<paramref name="schools" />在<paramref name="schoolYear" />的Assessment的Average结果
        /// </summary>
        /// <param name="assessmentId">The assessment identifier.</param>
        /// <param name="schoolYear">The school year.</param>
        /// <param name="schools">The schools.</param>
        /// <param name="startDate">The start date.</param>
        /// <param name="endDate">The end date.</param>
        /// <returns></returns>
        List<SchoolRecordModel> GetReportSchoolRecords(int assessmentId, string schoolYear, string schools, DateTime startDate, DateTime endDate,
            DateTime dobStartDate, DateTime dobEndDate, IList<int> classIds );

        /// <summary>
        /// 查找所有的<paramref name="schools" />在<paramref name="schoolYear" />的Assessment的Average结果
        /// </summary>
        /// <param name="assessmentId">The assessment identifier.</param>
        /// <param name="schoolYear">The school year.</param>
        /// <param name="schools">The schools.</param>
        /// <param name="startDate">The start date.</param>
        /// <param name="endDate">The end date.</param>
        /// <returns></returns>
        List<SchoolPercentileRankModel> GetReportSchoolPercentileRankRecords(int assessmentId, string schoolYear, string schools, DateTime startDate, DateTime endDate,
            DateTime dobStartDate, DateTime dobEndDate, IList<int> classIds);

        /// <summary>
        /// Gets the report student records.
        /// </summary>
        /// <param name="assessmentId">The assessment identifier.</param>
        /// <param name="schoolYear">The school year.</param>
        /// <param name="schoolIds">The school ids.</param>
        /// <param name="startDate">The start date.</param>
        /// <param name="endDate">The end date.</param>
        /// <returns></returns>
        List<StudentRecordModel> GetReportStudentRecords(int assessmentId, string schoolYear, IEnumerable<int> schoolIds, DateTime startDate, DateTime endDate,
            DateTime dobStartDate, DateTime dobEndDate, IList<int> listClassId );

        /// <summary>
        /// 查找<paramref name="districtId"/>在<paramref name="schoolYear"/>的Assessment学生详细结果
        /// </summary>
        /// <param name="assessmentId">The assessment identifier.</param>
        /// <param name="schoolYear">The school year.</param>
        /// <param name="districtId">The district identifier.</param>
        /// <returns></returns>
        List<StudentRecordModel> GetReportStudentRecordsByDistrict(int assessmentId, string schoolYear, int districtId, List<Wave> waves);

        /// <summary>
        /// Gets the report student records.
        /// </summary>
        /// <param name="assessmentId">The assessment identifier.</param>
        /// <param name="schoolYear">The school year.</param>
        /// <param name="schoolId">The school identifier.</param>
        /// <param name="studentIds">学生范围.</param>
        /// <returns></returns>
        List<StudentRecordModel> GetReportStudentRecords(int assessmentId, string schoolYear, int schoolId, DateTime startDate, DateTime endDate,IList<int> classIds, IEnumerable<int> studentIds = null);

        /// <summary>
        /// communityId ==0 时，按schoolId查询
        /// </summary>
        List<CompletionMeasureModel> GetCompletionCombinedStudentMeasure(int communityId, int schoolId, int assessmentId, int otherAssessmentId, Wave wave
            , List<int> measureIdList, List<int> hasChilderMeasureId, string schoolYear,DateTime startDate,DateTime endDate, DateTime dobStartDate, DateTime dobEndDate, IList<int> classIds , out List<CompletionMeasureModel> otherList);


        /// <summary>
        /// communityId ==0 时，表示，按SchoolId 查询，又语报表，处理只是英语或者西班牙语的学生
        /// </summary>
        List<CompletionMeasureModel> GetCompletionEnglishAndSpanishStudentMeasure(int communityId, int schoolId, int assessmentId, int otherAssessmentId
            , Wave wave, List<int> measureIdList, string schoolYear,DateTime startDate,DateTime endDate, DateTime dobStartDate, DateTime dobEndDate, IList<int> classIds );

        /// <summary>
        /// communityId ==0 时，表示，按SchoolId 查寻 ,完成报表，English 版或 Spanlish
        /// </summary>
        List<CompletionMeasureModel> GetCompletionStudentMeasure(int communityId, int schoolId, int assessmentId
            , Wave wave, List<int> measureIdList, string schoolYear,DateTime startDate,DateTime endDate, DateTime dobStartDate, DateTime dobEndDate, StudentAssessmentLanguage language,IList<int> classIds );


        List<CircleDataExportStudentItemModel> GetCircleDataExportStudentItemModels(int communityId, string year,
            int schoolId, List<int> waves, List<int> measures, List<ItemType> types);

        List<CircleDataExportStudentMeasureModel> GetCircleDataExportStudentMeasureModels(int communityId, string year,
            int schoolId, List<int> waves, List<int> measures);

        List<CircleDataExportStudentMeasureModel> GetCircleDataExportStudentMeasureModelsWithItems(int communityId, string year,
            int schoolId, List<int> waves, List<int> measures, List<int> measuresIncludeItems, List<ItemType> types,DateTime startDate, DateTime endDate);

        /// <summary>
        /// Gets the wave finished date.
        /// </summary>
        /// <param name="level">The level.</param>
        /// <param name="objectId">The object identifier.</param>
        /// <returns>
        /// 所有Wave的完成时间, 没有任何数据的以MinDate表示
        /// </returns>
        /// Author : JackZhang
        /// Date   : 6/22/2015 17:52:17
       List<WaveFinishedOnModel> GetWaveFinishedDate(QueryLevel level, int objectId);

        #endregion

        #region CpallsStudentGroup

        OperationResult InsertCpallsStudentGroup(CpallsStudentGroupEntity entity);

        OperationResult UpdateCpallsStudentGroup(CpallsStudentGroupEntity entity);

        OperationResult UpdateCpallsStudentGroup(List<CpallsStudentGroupEntity> list);

        OperationResult DeleteCpallsStudentGroup(int id);

        CpallsStudentGroupEntity GetCpallsStudentGroupEntity(int id);


        #endregion


        OperationResult InsertUserShownMeasure(UserShownMeasuresEntity entity);
        OperationResult UpdateUserShownMeasure(UserShownMeasuresEntity entity);
        OperationResult DeleteUserShownMeasure(int id);

        #region MeasureClassGroup

        OperationResult InsertMeasureClassGroup(MeasureClassGroupEntity entity);

        OperationResult UpdateMeasureClassGroup(MeasureClassGroupEntity entity);

        IQueryable<MeasureClassGroupEntity> MeasureClassGroups { get; }

        #endregion


        IQueryable<CustomGroupMyActivityEntity> GroupActivities { get; }

        OperationResult InsertGroupActivity(CustomGroupMyActivityEntity entity);
        OperationResult InsertGroupActivity(IList<CustomGroupMyActivityEntity> list);
        OperationResult DeleteGroupActivity(int groupId);
        OperationResult DeleteGroupActivity(int activityId, int userId);
    }
}
