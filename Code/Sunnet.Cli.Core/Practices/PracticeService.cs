using Sunnet.Cli.Core.Practices.Entites;
using Sunnet.Cli.Core.Practices.Entities;
using Sunnet.Cli.Core.Practices.Interfaces;
using Sunnet.Framework.Core.Base;
using Sunnet.Framework.Core.Tool;
using Sunnet.Framework.EmailSender;
using Sunnet.Framework.Encrypt;
using Sunnet.Framework.File;
using Sunnet.Framework.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sunnet.Cli.Core.Cpalls;
using Sunnet.Cli.Core.Cpalls.Models.Report;

namespace Sunnet.Cli.Core.Practices
{
    class PracticeService : CoreServiceBase, IPracticeContract
    {
        #region PracticeService
        private readonly IPracticeStudentAssessmentRpst _studentAssRpst;
        private readonly IPracticeStudentItemRpst _studentItemRpst;
        private readonly IPracticeStudentMeasureRpst _studentMeasureRpst;
        private readonly IDemoStudentRpst _studentRpst;
        private readonly IPracticeMeasureGroupRpst _practiceMeasureGroupRpst;
        private readonly IPracticeStudentGroupRpst _practiceStudentGroupRpst;
        private readonly IPracticeGroupMyActivityRpst _practiceGroupMyActivityRpst;

        public PracticeService(ISunnetLog log, IFile fileHelper, IEmailSender emailSender, IEncrypt encrypt,
            IPracticeStudentAssessmentRpst studentAssRpst,
            IPracticeStudentItemRpst studentItemRpst,
            IPracticeStudentMeasureRpst studentMeasureRpst,
            IDemoStudentRpst studentRpst,
                 IPracticeMeasureGroupRpst practiceMeasureGroupRpst,
                   IPracticeStudentGroupRpst practiceStudentGroupRpst,
                   IPracticeGroupMyActivityRpst practiceGroupMyActivityRpst,
            IUnitOfWork unit)
            : base(log, fileHelper, emailSender, encrypt)
        {
            _studentAssRpst = studentAssRpst;
            _studentItemRpst = studentItemRpst;
            _studentMeasureRpst = studentMeasureRpst;
            _studentRpst = studentRpst;
            _practiceMeasureGroupRpst = practiceMeasureGroupRpst;
            _practiceStudentGroupRpst = practiceStudentGroupRpst;
            _practiceGroupMyActivityRpst = practiceGroupMyActivityRpst;
            UnitOfWork = unit;
        }
        #endregion

        #region IQueryables

        public IQueryable<PracticeStudentAssessmentEntity> Assessments
        {
            get { return _studentAssRpst.Entities; }
        }

        public IQueryable<PracticeStudentMeasureEntity> Measures
        {
            get { return _studentMeasureRpst.Entities; }
        }

        public IQueryable<PracticeStudentItemEntity> Items
        {
            get { return _studentItemRpst.Entities; }
        }
        public IQueryable<DemoStudentEntity> Students
        {
            get { return _studentRpst.Entities; }
        }

        public IQueryable<PracticeMeasureGroupEntity> MeasureGroups
        {
            get { return _practiceMeasureGroupRpst.Entities; }
        }
        public IQueryable<PracticeStudentGroupEntity> StudentGroups
        {
            get { return _practiceStudentGroupRpst.Entities; }
        }
        #endregion 

        #region Student Assessments
        public PracticeStudentAssessmentEntity GetStudentAssessment(int id)
        {
            return _studentAssRpst.GetByKey(id);
        }
        public OperationResult InsertStudentAssessment(PracticeStudentAssessmentEntity entity)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                _studentAssRpst.Insert(entity);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }
        public OperationResult UpdateStudentAssessment(PracticeStudentAssessmentEntity entity)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                _studentAssRpst.Update(entity);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }
        public OperationResult DeleteStudentAssessment(int id)
        {
            throw new NotImplementedException();
        }

        public PracticeStudentAssessmentEntity NewStudentAssessmentEntity()
        {
            return _studentAssRpst.Create();
        }


        #endregion

        #region Student Measures
        public void RecalculateParentGoal(int saId, int parentMeasureId = 0)
        {
            try
            {
                _studentMeasureRpst.RecalculateParentGoal(saId, parentMeasureId);
            }
            catch (Exception ex)
            {
                ResultError(ex);
            }
        }
        public OperationResult RefreshClassroom(int assessmentId,Wave wave,int userId)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                _studentMeasureRpst.RefreshClassroom(assessmentId, wave, userId);
            }
            catch (Exception ex)
            {
                result.ResultType = OperationResultType.Error;
                result.Message = "Reset Class Error.";
                ResultError(ex);
            }
            return result;
        }
        public OperationResult CleanClassroom(int assessmentId)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                _studentMeasureRpst.CleanClassroom(assessmentId);
            }
            catch (Exception ex)
            {
                result.ResultType = OperationResultType.Error;
                result.Message = "Clean Classroom Error.";
                ResultError(ex);
            }
            return result;
        }
        public PracticeStudentMeasureEntity GetStudentMeasure(int id)
        {
            return _studentMeasureRpst.GetByKey(id);
        }
        public OperationResult InsertStudentMeasure(PracticeStudentMeasureEntity entity)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                _studentMeasureRpst.Insert(entity);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }
        public OperationResult UpdateStudentMeasure(PracticeStudentMeasureEntity entity)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                _studentMeasureRpst.Update(entity);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult UpdateStudentMeasures(List<PracticeStudentMeasureEntity> entities)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                entities.ForEach(x => _studentMeasureRpst.Update(x, false));
                UnitOfWork.Commit();
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public PracticeStudentMeasureEntity NewStudentMeasureEntity()
        {
            return _studentMeasureRpst.Create();
        }
        #endregion

        #region Student Items
        public PracticeStudentItemEntity NewStudentItemEntity()
        {
            return _studentItemRpst.Create();
        }
        public OperationResult UpdateStudentItems(List<PracticeStudentItemEntity> entities)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                entities.ForEach(x => _studentItemRpst.Update(x, false));
                UnitOfWork.Commit();
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }
        #endregion

        #region Demo students
        public DemoStudentEntity NewDemoStudentEntity()
        {
            return _studentRpst.Create();
        }
        public OperationResult InsertDemoStudent(DemoStudentEntity entity)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                _studentRpst.Insert(entity);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult InsertDemoStudents(IList<DemoStudentEntity> entityList)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                _studentRpst.Insert(entityList);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }
        #endregion

        public OperationResult InitMeasures(int userId, int assessmentId, string schoolYear, int studentId,
          DateTime studentBirthday, Wave wave, IEnumerable<int> measureIds)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            var stuAssID = _studentMeasureRpst.InitMeasures(userId, assessmentId, schoolYear, studentId,
                studentBirthday, wave, measureIds);
            if (stuAssID < 1)
                result.Message = "Something is wrong.";
            result.AppendData = stuAssID;
            return result;
        }

        #region Student Measure Group 
        public PracticeMeasureGroupEntity GetMeasureGroup(int id)
        {
            return _practiceMeasureGroupRpst.GetByKey(id);
        }
        public OperationResult InsertMeasureGroup(PracticeMeasureGroupEntity entity)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                _practiceMeasureGroupRpst.Insert(entity);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }
        public OperationResult UpdateMeasureGroup(PracticeMeasureGroupEntity entity)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                _practiceMeasureGroupRpst.Update(entity);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }
      

        public PracticeMeasureGroupEntity NewMeasureGroupEntity()
        {
            return _practiceMeasureGroupRpst.Create();
        }
        #endregion

        #region Practice Student Group

        public OperationResult InsertCpallsStudentGroup(PracticeStudentGroupEntity entity)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                _practiceStudentGroupRpst.Insert(entity);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult UpdateCpallsStudentGroup(PracticeStudentGroupEntity entity)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                _practiceStudentGroupRpst.Update(entity);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult UpdateCpallsStudentGroup(List<PracticeStudentGroupEntity> list)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                _practiceStudentGroupRpst.Insert(list);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult DeleteCpallsStudentGroup(int id)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                _practiceStudentGroupRpst.Delete(id);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public PracticeStudentGroupEntity GetCpallsStudentGroupEntity(int id)
        {
            return _practiceStudentGroupRpst.GetByKey(id);
        }

        #endregion

        #region Measure Class Group

        public OperationResult InsertPracticeMeasureGroup(PracticeMeasureGroupEntity entity)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                _practiceMeasureGroupRpst.Insert(entity);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult UpdatePracticeMeasureGroup(PracticeMeasureGroupEntity entity)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                _practiceMeasureGroupRpst.Update(entity);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public IQueryable<PracticeMeasureGroupEntity> PracticeMeasureGroups
        {
            get { return _practiceMeasureGroupRpst.Entities; }
        }

        #endregion

        #region Practice Report

        public List<ReportMeasureHeaderModel> GetReportMeasureHeaders(int assessmentId)
        {
            try
            {
                return _studentMeasureRpst.GetReportMeasureHeaders(assessmentId);
            }
            catch (Exception ex)
            {
                LoggerHelper.Debug(ex);
                return null;
            }
        }
        public List<StudentRecordModel> GetReportStudentRecords(int assessmentId, string schoolYear, DateTime startDate,
         DateTime endDate, IEnumerable<int> studentIds,int userId)
        {
            try
            {
                return _studentMeasureRpst.GetReportStudentRecords(assessmentId, schoolYear, startDate, endDate, studentIds,userId);
            }
            catch (Exception ex)
            {
                LoggerHelper.Debug(ex);
                return null;
            }
        }

        public List<WaveFinishedOnModel> GetWaveFinishedDate(int assessmentId, int userId)
        {
            try
            {
                return _studentMeasureRpst.GetWaveFinishedDate(assessmentId, userId);
            }
            catch (Exception ex)
            {
                LoggerHelper.Debug(ex);
                return null;
            }
        }
        #endregion

        #region Custom Group My Activity
        public IQueryable<PracticeGroupMyActivityEntity> PracticeGroupActivities
        {
            get { return _practiceGroupMyActivityRpst.Entities; }
        }



        public OperationResult InsertPracticeGroupActivity(PracticeGroupMyActivityEntity entity)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                _practiceGroupMyActivityRpst.Insert(entity);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult InsertPracticeGroupActivity(IList<PracticeGroupMyActivityEntity> list)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                _practiceGroupMyActivityRpst.Insert(list);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult DeletePracticeGroupActivity(int groupId)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                _practiceGroupMyActivityRpst.Delete(c => c.GroupId == groupId);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult DeletePracticeGroupActivity(int activityId, int userId)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                _practiceGroupMyActivityRpst.Delete(c => c.MyActivityId == activityId && c.CreatedBy == userId);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        #endregion
    }
}
