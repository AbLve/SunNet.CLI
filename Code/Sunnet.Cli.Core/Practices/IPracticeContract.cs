using Sunnet.Cli.Core.Practices.Entites;
using Sunnet.Cli.Core.Practices.Entities;
using Sunnet.Framework.Core.Tool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sunnet.Cli.Core.Cpalls;
using Sunnet.Cli.Core.Cpalls.Models.Report;

namespace Sunnet.Cli.Core.Practices
{
    public interface IPracticeContract
    {
        #region IQueryables

        IQueryable<PracticeStudentAssessmentEntity> Assessments { get; }
        IQueryable<PracticeStudentMeasureEntity> Measures { get; }
        IQueryable<PracticeStudentItemEntity> Items { get; }
          IQueryable<DemoStudentEntity> Students { get; }
        IQueryable<PracticeMeasureGroupEntity> MeasureGroups { get; }
        IQueryable<PracticeStudentGroupEntity> StudentGroups { get; }
        IQueryable<PracticeMeasureGroupEntity> PracticeMeasureGroups { get; }

        #endregion

        PracticeStudentAssessmentEntity NewStudentAssessmentEntity();

        PracticeStudentMeasureEntity NewStudentMeasureEntity();

        PracticeStudentItemEntity NewStudentItemEntity();
        DemoStudentEntity NewDemoStudentEntity();

        PracticeStudentAssessmentEntity GetStudentAssessment(int id);
        OperationResult InsertStudentAssessment(PracticeStudentAssessmentEntity entity);
        OperationResult UpdateStudentAssessment(PracticeStudentAssessmentEntity entity);

        OperationResult DeleteStudentAssessment(int id);
        void RecalculateParentGoal(int saId, int parentMeasureId = 0);
        OperationResult RefreshClassroom(int assessmentId, Wave wave,int userId);
        OperationResult CleanClassroom(int assessmentId);
        PracticeStudentMeasureEntity GetStudentMeasure(int id);
        OperationResult InsertStudentMeasure(PracticeStudentMeasureEntity entity);
        OperationResult UpdateStudentMeasure(PracticeStudentMeasureEntity entity);
        OperationResult UpdateStudentMeasures(List<PracticeStudentMeasureEntity> entities);
        OperationResult InsertDemoStudent(DemoStudentEntity entity);
        OperationResult InsertDemoStudents(IList<DemoStudentEntity> entityList);

        OperationResult InitMeasures(int userId, int assessmentId, string schoolYear, int studentId,
          DateTime studentBirthday, Wave wave, IEnumerable<int> measureIds);


        OperationResult UpdateStudentItems(List<PracticeStudentItemEntity> entities);
        #region MeasureGroup

        PracticeMeasureGroupEntity GetMeasureGroup(int id);
        OperationResult InsertMeasureGroup(PracticeMeasureGroupEntity entity);
        OperationResult UpdateMeasureGroup(PracticeMeasureGroupEntity entity);
        PracticeMeasureGroupEntity NewMeasureGroupEntity();

        #endregion

        #region Group
        List<ReportMeasureHeaderModel> GetReportMeasureHeaders(int assessmentId);
        List<StudentRecordModel> GetReportStudentRecords(int assessmentId, string schoolYear, DateTime startDate,
          DateTime endDate, IEnumerable<int> studentIds, int userId);

        List<WaveFinishedOnModel> GetWaveFinishedDate(int assessmentId,int userId);

        #endregion

        #region Practice Student Group

        OperationResult InsertCpallsStudentGroup(PracticeStudentGroupEntity entity);
        OperationResult UpdateCpallsStudentGroup(PracticeStudentGroupEntity entity);
        OperationResult UpdateCpallsStudentGroup(List<PracticeStudentGroupEntity> list);
        OperationResult DeleteCpallsStudentGroup(int id);
        PracticeStudentGroupEntity GetCpallsStudentGroupEntity(int id);

        #endregion

        OperationResult InsertPracticeMeasureGroup(PracticeMeasureGroupEntity entity);
        OperationResult UpdatePracticeMeasureGroup(PracticeMeasureGroupEntity entity);

        #region Custom Group My Activity
        IQueryable<PracticeGroupMyActivityEntity> PracticeGroupActivities { get; }

        OperationResult InsertPracticeGroupActivity(PracticeGroupMyActivityEntity entity);

        OperationResult InsertPracticeGroupActivity(IList<PracticeGroupMyActivityEntity> list);

        OperationResult DeletePracticeGroupActivity(int groupId);

        OperationResult DeletePracticeGroupActivity(int activityId, int userId);

        #endregion
    }
}
