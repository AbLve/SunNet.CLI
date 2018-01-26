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
using Sunnet.Cli.Core.Cpalls.Interfaces;
using Sunnet.Cli.Core.Cpalls.Models;
using Sunnet.Cli.Core.Cpalls.Models.Report;
using Sunnet.Cli.Core.Reports.Models;
using Sunnet.Framework.Core.Base;
using Sunnet.Framework.Core.Tool;
using Sunnet.Framework.EmailSender;
using Sunnet.Framework.Encrypt;
using Sunnet.Framework.Extensions;
using Sunnet.Framework.File;
using Sunnet.Framework.Log;


namespace Sunnet.Cli.Core.Cpalls
{
    internal class CpallsService : CoreServiceBase, ICpallsContract
    {
        #region Ctor
        private readonly IStudentAssessmentRpst _studentAssRpst;
        private readonly IStudentItemRpst _studentItemRpst;
        private readonly IStudentMeasureRpst _studentMeasure;
        private readonly ICpallsSchoolRpst _cpallsSchool;
        private readonly ICpallsSchoolMeasureRpst _cpallsSchoolMeasure;
        private readonly ICpallsClassRpst _cpallsClass;
        private readonly ICpallsClassMeasureRpst _cpallsClassMeasure;
        private readonly ICpallsStudentGroupRpst _cpallsStudentGroup;
        private readonly IUserShownMeasuresRpst _userShownMeasuresRpst;
        private readonly IMeasureClassGroupRpst _measureClassGroup;
        private readonly ICustomGroupMyActivityRpst _customGroupMyActivityRpst;

        public CpallsService(ISunnetLog log, IFile fileHelper, IEmailSender emailSender, IEncrypt encrypt,
            IStudentAssessmentRpst studentAssRpst,
            IStudentItemRpst studentItemRpst,
            IStudentMeasureRpst studentMeasure,
            ICpallsSchoolRpst cpallsSchool,
            ICpallsSchoolMeasureRpst cpasslsSchoolMeasure,
            ICpallsClassRpst cpallsClass,
            ICpallsClassMeasureRpst cpallsClassMeasure,
            ICpallsStudentGroupRpst cpallsStudentGroup,
             IUserShownMeasuresRpst userShownMeasuresRpst,
            IMeasureClassGroupRpst measureClassGroup,
            ICustomGroupMyActivityRpst customGroupMyActivityRpst,
        IUnitOfWork unit)
            : base(log, fileHelper, emailSender, encrypt)
        {
            _studentAssRpst = studentAssRpst;
            _studentItemRpst = studentItemRpst;
            _studentMeasure = studentMeasure;
            _cpallsSchool = cpallsSchool;
            _cpallsSchoolMeasure = cpasslsSchoolMeasure;
            _cpallsClassMeasure = cpallsClassMeasure;
            _cpallsClass = cpallsClass;
            _cpallsStudentGroup = cpallsStudentGroup;
            _userShownMeasuresRpst = userShownMeasuresRpst;
            _measureClassGroup = measureClassGroup;
            _customGroupMyActivityRpst = customGroupMyActivityRpst;
            UnitOfWork = unit;
        }
        #endregion

        #region IQueryables

        public IQueryable<StudentAssessmentEntity> Assessments
        {
            get { return _studentAssRpst.Entities; }
        }

        public IQueryable<StudentMeasureEntity> Measures
        {
            get { return _studentMeasure.Entities; }
        }

        public IQueryable<StudentItemEntity> Items
        {
            get { return _studentItemRpst.Entities; }
        }

        public IQueryable<CpallsSchoolEntity> CpallsShools
        {
            get { return _cpallsSchool.Entities; }
        }

        public IQueryable<CpallsSchoolMeasureEntity> CpallsSchoolMeasures
        {
            get { return _cpallsSchoolMeasure.Entities; }
        }

        public IQueryable<CpallsClassEntity> CpallsClasses
        {
            get { return _cpallsClass.Entities; }
        }

        public IQueryable<CpallsClassMeasureEntity> CpallsClassMeasures
        {
            get { return _cpallsClassMeasure.Entities; }
        }

        public IQueryable<CpallsStudentGroupEntity> CpallsStudentGroups
        {
            get { return _cpallsStudentGroup.Entities; }
        }

        public IQueryable<UserShownMeasuresEntity> UserShownMeasures
        {
            get { return _userShownMeasuresRpst.Entities; }
        }
        #endregion


        public StudentAssessmentEntity NewStudentAssessmentEntity()
        {
            return _studentAssRpst.Create();
        }

        public StudentMeasureEntity NewStudentMeasureEntity()
        {
            return _studentMeasure.Create();
        }

        public StudentItemEntity NewStudentItemEntity()
        {
            return _studentItemRpst.Create();
        }


        public StudentAssessmentEntity GetStudentAssessment(int id)
        {
            return _studentAssRpst.GetByKey(id);
        }

        public OperationResult InsertStudentAssessment(StudentAssessmentEntity entity)
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
        public OperationResult UpdateStudentAssessment(StudentAssessmentEntity entity)
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


        public StudentMeasureEntity GetStudentMeasure(int id)
        {
            return _studentMeasure.GetByKey(id);
        }
        public OperationResult InsertStudentMeasure(StudentMeasureEntity entity)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                _studentMeasure.Insert(entity);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }
        public OperationResult UpdateStudentMeasure(StudentMeasureEntity entity)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                _studentMeasure.Update(entity);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }
        public int UpdateBenchmark(int studentMeasureId, int benchmarkId, decimal lowerScore, decimal higherScore, bool ShowOnGroup, bool benchmarkChanged)
        {
            int count = 0;
            try
            {
                count = _studentMeasure.UpdateBenchmark(studentMeasureId, benchmarkId, lowerScore, higherScore, ShowOnGroup, benchmarkChanged);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return count;
        }
        public int UpdatePercentileRank(int studentMeasureId, string percentileRank)
        {
            int count = 0;
            try
            {
                count = _studentMeasure.UpdatePercentileRank(studentMeasureId, percentileRank);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return count;
        }

        public int UpdateBenchmarkChangedToFalse(int measureId)
        {
            int count = 0;
            try
            {
                count = _studentMeasure.UpdateBenchmarkChangedToFalse(measureId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return count;
        }
        public OperationResult UpdateStudentMeasures(List<StudentMeasureEntity> entities)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                entities.ForEach(x => _studentMeasure.Update(x, false));
                UnitOfWork.Commit();
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult InitMeasures(int userId, int assessmentId, string schoolYear, int studentId,
            DateTime studentBirthday, Wave wave, IEnumerable<int> measureIds)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            var stuAssID = _studentMeasure.InitMeasures(userId, assessmentId, schoolYear, studentId,
                studentBirthday, wave, measureIds);
            if (stuAssID < 1)
                result.Message = "Something is wrong.";
            result.AppendData = stuAssID;
            return result;
        }

        public void RecalculateParentGoal(int saId, int parentMeasureId = 0)
        {
            try
            {
                _studentMeasure.RecalculateParentGoal(saId, parentMeasureId);
            }
            catch (Exception ex)
            {
                ResultError(ex);
            }
        }

        public OperationResult UpdateStudentItem(StudentItemEntity entity)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                _studentItemRpst.Update(entity);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult UpdateStudentItems(List<StudentItemEntity> entities)
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

        public List<SchoolMeasureGoalModel> GetSchoolMeasureGoal(List<int> schoolId, string schoolYear, Wave wave, int assessmentId, IList<int> classIds)
        {
            return _studentAssRpst.GetSchoolMeasureGoal(schoolId, schoolYear, wave, assessmentId, classIds);
        }

        public List<StudentMeasureGoalModel> GetStudentMeasureGoal(List<int> studentIds, string schoolYear, Wave wave, int assessmentId)
        {
            return _studentAssRpst.GetStudentMeasureGoal(studentIds, schoolYear, wave, assessmentId);
        }

        public int GetStudentAssessmentIdForPlayMeasure(int assessmentId, int studentId, string schoolYear, int wave)
        {
            return _studentAssRpst.GetStudentAssessmentIdForPlayMeasure(assessmentId, studentId, schoolYear, wave);
        }

        #region Report

        /// <summary>
        /// 查找所有(未删除的正常状态)的Measure
        /// </summary>
        /// <param name="assessmentId">The assessment identifier.</param>
        /// <returns></returns>
        public List<ReportMeasureHeaderModel> GetReportMeasureHeaders(int assessmentId)
        {
            return _cpallsSchool.GetReportMeasureHeaders(assessmentId);
        }

        /// <summary>
        /// 查找所有的<paramref name="schools" />在<paramref name="schoolYear" />的Assessment的Average结果
        /// </summary>
        /// <param name="assessmentId">The assessment identifier.</param>
        /// <param name="schoolYear">The school year.</param>
        /// <param name="schools">The schools.</param>
        /// <param name="startDate">The start date.</param>
        /// <param name="endDate">The end date.</param>
        /// <returns></returns>
        public List<SchoolRecordModel> GetReportSchoolRecords(int assessmentId, string schoolYear, string schools, DateTime startDate, DateTime endDate,
            DateTime dobStartDate, DateTime dobEndDate, IList<int> classIds)
        {
            return _cpallsSchool.GetReportSchoolRecords(assessmentId, schoolYear, schools, startDate, endDate, dobStartDate, dobEndDate, classIds);
        }
        /// <summary>
         /// 查找所有的<paramref name="schools" />在<paramref name="schoolYear" />的Assessment的Average结果
         /// </summary>
         /// <param name="assessmentId">The assessment identifier.</param>
         /// <param name="schoolYear">The school year.</param>
         /// <param name="schools">The schools.</param>
         /// <param name="startDate">The start date.</param>
         /// <param name="endDate">The end date.</param>
         /// <returns></returns>
        public List<SchoolPercentileRankModel> GetReportSchoolPercentileRankRecords(int assessmentId, string schoolYear, string schools, DateTime startDate, DateTime endDate,
            DateTime dobStartDate, DateTime dobEndDate, IList<int> classIds)
        {
            return _cpallsSchool.GetReportSchoolPercentileRankRecords(assessmentId, schoolYear, schools, startDate, endDate,
            dobStartDate, dobEndDate, classIds);
        }

        public List<StudentRecordModel> GetReportStudentRecords(int assessmentId, string schoolYear, int schoolId, DateTime startDate, DateTime endDate, IList<int> classIds, IEnumerable<int> studentIds = null)
        {
            if (studentIds == null)
            {
                return _cpallsSchool.GetReportStudentRecords(assessmentId, schoolYear, schoolId, startDate, endDate, classIds);
            }

            return _cpallsSchool.GetReportStudentRecords(assessmentId, schoolYear, schoolId, studentIds, startDate, endDate);
        }

        public List<StudentRecordModel> GetReportStudentRecords(int assessmentId, string schoolYear, IEnumerable<int> schoolIds, DateTime startDate, DateTime endDate,
            DateTime dobStartDate, DateTime dobEndDate, IList<int> listClassId)
        {
            try
            {
                return _cpallsSchool.GetReportStudentRecords(assessmentId, schoolYear, schoolIds, startDate, endDate,
                dobStartDate, dobEndDate, listClassId);
            }
            catch (Exception ex)
            {
                LoggerHelper.Debug(ex);
                return null;
            }
        }

        public List<StudentRecordModel> GetReportStudentRecordsByDistrict(int assessmentId, string schoolYear, int districtId, List<Wave> waves)
        {
            try
            {
                return _cpallsSchool.GetReportStudentRecordsByDistrict(assessmentId, schoolYear, districtId, waves);
            }
            catch (Exception ex)
            {
                LoggerHelper.Debug(ex);
                return null;
            }
        }

        public List<CircleDataExportStudentItemModel> GetCircleDataExportStudentItemModels(int communityId, string year, int schoolId, List<int> waves, List<int> measures, List<ItemType> types)
        {
            try
            {
                return _studentItemRpst.GetCircleDataExportStudentItemModels(communityId, year, schoolId, waves, measures, types);
            }
            catch (Exception ex)
            {
                LoggerHelper.Debug(ex);
                return null;
            }
        }

        public List<CircleDataExportStudentMeasureModel> GetCircleDataExportStudentMeasureModels(int communityId, string year, int schoolId, List<int> waves, List<int> measures)
        {
            try
            {
                return _studentMeasure.GetCircleDataExportStudentMeasureModels(communityId, year, schoolId, waves, measures);
            }
            catch (Exception ex)
            {
                LoggerHelper.Debug(ex);
                return null;
            }
        }

        public List<CircleDataExportStudentMeasureModel> GetCircleDataExportStudentMeasureModelsWithItems(int communityId, string year, int schoolId, List<int> waves, List<int> measures, List<int> measuresIncludeItems, List<ItemType> types, DateTime startDate, DateTime endDate)
        {
            try
            {
                return _studentMeasure.GetCircleDataExportStudentMeasureModelsWithItems(
                    communityId, year, schoolId, waves, measures, measuresIncludeItems, types, startDate, endDate);
            }
            catch (Exception ex)
            {
                LoggerHelper.Debug(ex);
                return null;
            }
        }


        public List<WaveFinishedOnModel> GetWaveFinishedDate(QueryLevel level, int objectId)
        {
            var dates = _studentMeasure.GetWaveFinishedDate(level, objectId);

            return dates;
        }

        #endregion


        #region CpallsStudentGroup

        public OperationResult InsertCpallsStudentGroup(CpallsStudentGroupEntity entity)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                _cpallsStudentGroup.Insert(entity);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult UpdateCpallsStudentGroup(CpallsStudentGroupEntity entity)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                _cpallsStudentGroup.Update(entity);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult UpdateCpallsStudentGroup(List<CpallsStudentGroupEntity> list)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                _cpallsStudentGroup.Insert(list);
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
                _cpallsStudentGroup.Delete(id);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public CpallsStudentGroupEntity GetCpallsStudentGroupEntity(int id)
        {
            return _cpallsStudentGroup.GetByKey(id);
        }

        #endregion

        /// <summary>
        /// communityId ==0 时，按schoolId查询
        /// </summary>
        public List<CompletionMeasureModel> GetCompletionCombinedStudentMeasure(int communityId, int schoolId, int assessmentId, int otherAssessmentId
            , Wave wave, List<int> measureIdList, List<int> hasChilderMeasureId, string schoolYear, DateTime startDate, DateTime endDate, DateTime dobStartDate, DateTime dobEndDate, IList<int> classIds, out List<CompletionMeasureModel> otherList)
        {
            otherList = new List<CompletionMeasureModel>();
            try
            {
                return _studentMeasure.GetCompletionCombinedStudentMeasure(
                    communityId, schoolId, assessmentId, otherAssessmentId, wave, measureIdList, hasChilderMeasureId, schoolYear, startDate, endDate, dobStartDate, dobEndDate, classIds, out otherList);
            }
            catch (Exception ex)
            {
                LoggerHelper.Debug(ex);
                return new List<CompletionMeasureModel>();
            }
        }

        /// <summary>
        /// communityId ==0 时，表示，按SchoolId 查询，又语报表，处理只是英语或者西班牙语的学生
        /// </summary>
        public List<CompletionMeasureModel> GetCompletionEnglishAndSpanishStudentMeasure(int communityId, int schoolId, int assessmentId, int otherAssessmentId
            , Wave wave, List<int> measureIdList, string schoolYear, DateTime startDate, DateTime endDate, DateTime dobStartDate, DateTime dobEndDate, IList<int> classIds)
        {
            try
            {
                return _studentMeasure.GetCompletionEnglishAndSpanishStudentMeasure(
                    communityId, schoolId, assessmentId, otherAssessmentId, wave, measureIdList, schoolYear, startDate, endDate, dobStartDate, dobEndDate, classIds);
            }
            catch (Exception ex)
            {
                LoggerHelper.Debug(ex);
                return new List<CompletionMeasureModel>();
            }
        }

        /// <summary>
        /// communityId ==0 时，表示，按SchoolId 查寻 ,完成报表，English 版或 Spanlish
        /// </summary>
        public List<CompletionMeasureModel> GetCompletionStudentMeasure(int communityId, int schoolId, int assessmentId
            , Wave wave, List<int> measureIdList, string schoolYear, DateTime startDate, DateTime endDate, DateTime dobStartDate, DateTime dobEndDate, StudentAssessmentLanguage language, IList<int> classIds)
        {
            try
            {
                return _studentMeasure.GetCompletionStudentMeasure(communityId, schoolId, assessmentId, wave, measureIdList, schoolYear, startDate, endDate, dobStartDate, dobEndDate, language, classIds);
            }
            catch (Exception ex)
            {
                LoggerHelper.Debug(ex);
                return new List<CompletionMeasureModel>();
            }
        }

        #region Display/Hide Measures
        public OperationResult InsertUserShownMeasure(UserShownMeasuresEntity entity)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                _userShownMeasuresRpst.Insert(entity);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }
        public OperationResult UpdateUserShownMeasure(UserShownMeasuresEntity entity)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                _userShownMeasuresRpst.Update(entity);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }
        public OperationResult DeleteUserShownMeasure(int id)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                _userShownMeasuresRpst.Delete(i => i.ID == id);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }
        #endregion

        #region MeasureClassGroup

        public OperationResult InsertMeasureClassGroup(MeasureClassGroupEntity entity)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                _measureClassGroup.Insert(entity);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult UpdateMeasureClassGroup(MeasureClassGroupEntity entity)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                _measureClassGroup.Update(entity);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public IQueryable<MeasureClassGroupEntity> MeasureClassGroups
        {
            get { return _measureClassGroup.Entities; }
        }

        #endregion


        #region Custom Group My Activity
        public IQueryable<CustomGroupMyActivityEntity> GroupActivities
        {
            get { return _customGroupMyActivityRpst.Entities; }
        }
         
      

        public OperationResult InsertGroupActivity(CustomGroupMyActivityEntity entity)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                _customGroupMyActivityRpst.Insert(entity);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        } 

        public OperationResult InsertGroupActivity(IList<CustomGroupMyActivityEntity> list)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                _customGroupMyActivityRpst.Insert(list);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult DeleteGroupActivity(int groupId)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                _customGroupMyActivityRpst.Delete(c=>c.GroupId ==groupId);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult DeleteGroupActivity(int activityId,int userId)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                _customGroupMyActivityRpst.Delete(c => c.MyActivityId == activityId && c.CreatedBy == userId);
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