using System.Data;
using Sunnet.Cli.Core.Common.Enums;
using Sunnet.Cli.Core.Reports.Entities;
using Sunnet.Cli.Core.Reports.Interfaces;
using Sunnet.Cli.Core.Schools.Interfaces;
using Sunnet.Cli.Core.Students.Interfaces;
using Sunnet.Cli.Core.Users.Interfaces;
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
using Sunnet.Cli.Core.Reports.Models;
using Sunnet.Cli.Core.Export.Enums;

namespace Sunnet.Cli.Core.Reports
{
    internal class ReportService : CoreServiceBase, IReportContract
    {
        private readonly IStudentRpst _studentRpst;
        private readonly ICoordCoachRpst _coordCoachRpst;
        private readonly ISchoolRpst _schoolRpst;
        private readonly ISchoolTypeRpst _schoolTypeRpst;
        private readonly ITeacherRpst _teacherRpst;
        private readonly IReportQueueRpst _reportQueueRpst;
        private readonly IAssessmentReportTemplateRpst _assessmentReportTemplatesRpst;
        private readonly IParentReportRpst _parentReportRpst;
        internal ReportService(ISunnetLog log, IFile fileHelper, IEmailSender emailSender, IEncrypt encrypt,
            IStudentRpst studentRpst,
            ICoordCoachRpst coorCoachRpst,
            ISchoolViewRpst schoolViewRpst,
            ISchoolRpst schoolRpst,
            ISchoolTypeRpst schoolTypeRpst,
            ITeacherRpst teacherRpst,
            IReportQueueRpst reportQueueRpst,
            IAssessmentReportTemplateRpst assessementReportTemplateRpst,
            IParentReportRpst parentReportRpst,
            IUnitOfWork unit)
            : base(log, fileHelper, emailSender, encrypt)
        {
            _studentRpst = studentRpst;
            _coordCoachRpst = coorCoachRpst;
            _schoolRpst = schoolRpst;
            _schoolTypeRpst = schoolTypeRpst;
            _teacherRpst = teacherRpst;
            _reportQueueRpst = reportQueueRpst;
            _assessmentReportTemplatesRpst = assessementReportTemplateRpst;
            _parentReportRpst = parentReportRpst;
            UnitOfWork = unit;
        }

        #region Media Consent Report
        public List<MediaConsentPercentModel> GetMediaConsentPercent(List<int> communityIds, List<int> schoolIds, string teacher)
        {
            return _studentRpst.GetMediaConsentPercent(communityIds, schoolIds, teacher);
        }

        public List<MediaConsentDetailModel> GetMediaConsentDetail(List<int> communityIds, List<int> schoolIds, string teacher)
        {
            return _studentRpst.GetMediaConsentDetail(communityIds, schoolIds, teacher);
        }
        #endregion

        public List<BeechReportModel> GetBeechReport(List<int> communityIds, List<int> schoolIds, string teacher)
        {
            return _studentRpst.GetBeechReport(communityIds, schoolIds, teacher);
        }

        public List<CoachReportModel> GetCoachReport(List<int> communityIds, int mentorCoach, string funding, int status)
        {
            return _coordCoachRpst.GetCoachReport(communityIds, mentorCoach, funding, status);
        }

        public List<ServiceReportModel> GetServiceReport(List<int> communityIds, List<int> schoolIds)
        {
            return _schoolRpst.GetServiceReport(communityIds, schoolIds);
        }

        #region Querterly-like reports
        public List<QuarterlyReportModel> GetYearsInProjectCountBySchoolType(List<int> communityIds, List<int> fundingList,
            DateTime? startDate, DateTime? endDate, int status)
        {
            return _schoolTypeRpst.GetYearsInProjectCountBySchoolType(communityIds, fundingList, startDate, endDate, status);
        }
        #endregion

        public List<Community_Mentor_TeacherModel> GetCommunity_Mentor_Teachers()
        {
            return _teacherRpst.GetCommunity_Mentor_Teachers();
        }

        public List<PDReportModel> PDCompletionCourseReport(List<int> communityIds, List<int> schoolIds, string teacher, int status)
        {
            List<PDReportModel> list = _teacherRpst.PDCompletionCourseReport(communityIds, schoolIds, teacher, status);
            List<PDReportModel> list1 = new List<PDReportModel>();
            foreach (var reportModel in list)
            {
                TimeSpan sum = new TimeSpan();
                if (reportModel.TimeSpentInCourse != "" && reportModel.TimeSpentInCourse.Length > 0)
                {
                    for (int i = 0; i < reportModel.TimeSpentInCourse.Split(',').Length; i++)
                    {
                        DateTime dateNow = DateTime.Now.Date;
                        DateTime dt = DateTime.Parse(reportModel.TimeSpentInCourse.Split(',')[i].ToString());
                        TimeSpan span = dt - dateNow;
                        sum += span;
                    }
                }
                //lms value :"",failed,incomplete,completed
                //status: Not Started;In Progress;Failed;Complete
                if (reportModel.Status == "")
                {
                    reportModel.Status = "Not Started";
                }
                else if (reportModel.Status.Contains("incomplete") || reportModel.Status.Contains("failed"))
                {
                    reportModel.Status = "In Progress";
                }
                else
                {
                    reportModel.Status = "Complete";
                }
                reportModel.TimeSpentInCourse = sum.ToString();
                list1.Add(reportModel);
            }
            return list1;
        }

        #region Report Queue

        public IQueryable<ReportQueueEntity> ReportQueues { get { return _reportQueueRpst.Entities; } }

        public ReportQueueEntity NewReportQueueEntity()
        {
            var entity = _reportQueueRpst.Create();
            entity.ExcuteTime = DateTime.Parse("1753-1-1");
            entity.ReceiveFileBy = ReceiveFileBy.DownloadLink;
            entity.SFTPHostIp = string.Empty;
            entity.SFTPFilePath = string.Empty;
            entity.SFTPUserName = string.Empty;
            entity.SFTPPassword = string.Empty;
            entity.FileType = ExportFileType.Comma;
            return entity;
        }

        public ReportQueueEntity GetReportQueue(int id)
        {
            if (id < 1)
                return null;
            return _reportQueueRpst.GetByKey(id);
        }

        public OperationResult InsertReportQueue(ReportQueueEntity entity)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                _reportQueueRpst.Insert(entity);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult InsertReportQueueList(List<ReportQueueEntity> entityList)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                _reportQueueRpst.Insert(entityList);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult UpdateReportQueue(ReportQueueEntity entity)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                _reportQueueRpst.Update(entity);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        #endregion

        #region TSR Turnover Report
        /// <summary>
        ///SchoolIds 为空 并且 communityIds 为空，表示管理员查看
        /// </summary>
        public List<TeacherSchoolTypeModel> GetTeacherBySchoolType(DateTime start, DateTime end, List<int> communityIds, List<int> schoolIds)
        {
            List<TeacherSchoolTypeModel> list = new List<TeacherSchoolTypeModel>();
            try
            {
                list = _reportQueueRpst.GetTeacherBySchoolType(start, end, communityIds, schoolIds);
            }
            catch (Exception ex)
            {
                ResultError(ex);
            }
            return list;
        }


        /// <summary>
        ///  SchoolIds 为空 并且 communityIds 为空，表示管理员查看
        /// </summary>
        public List<CountTeacherbyCommunityModel> GetCountTeacherbyCommunityList(DateTime start, DateTime end, List<int> communityIds, List<int> schoolIds)
        {
            List<CountTeacherbyCommunityModel> list = new List<CountTeacherbyCommunityModel>();
            try
            {
                list = _reportQueueRpst.GetCountTeacherbyCommunityList(start, end, communityIds, schoolIds);
            }
            catch (Exception ex)
            {
                ResultError(ex);
            }
            return list;
        }

        public List<TurnoverTeacherModel> GetTurnoverTeacherModels(DateTime startDate, DateTime endDate, List<int> communities, List<int> schools)
        {
            try
            {
                return _reportQueueRpst.GetTurnoverTeacherModels(startDate, endDate, communities, schools);
            }
            catch (Exception ex)
            {
                ResultError(ex);
                return new List<TurnoverTeacherModel>();
            }
        }

        #endregion


        public List<CountbyFacilityTypeMode> GetCountbyFacilityTypeList(List<int> communityIds, List<int> fundingList,
            DateTime? startDate, DateTime? endDate, int status)
        {
            List<CountbyFacilityTypeMode> list = new List<CountbyFacilityTypeMode>();
            try
            {
                list = _reportQueueRpst.GetCountbyFacilityTypeList(communityIds, fundingList, startDate, endDate, status);
            }
            catch (Exception ex)
            {
                ResultError(ex);
            }
            return list;
        }

        public List<CountbyCommunityModel> GetCountbyCommunityList(List<int> communityIds, List<int> fundingList,
            DateTime? startDate, DateTime? endDate, int status)
        {
            List<CountbyCommunityModel> list = new List<CountbyCommunityModel>();
            try
            {
                list = _reportQueueRpst.GetCountbyCommunityList(communityIds, fundingList, startDate, endDate, status);
            }
            catch (Exception ex)
            {
                ResultError(ex);
            }
            return list;
        }

        public List<CircleDataExportStudentModel> GetCircleDataExportStudentModels(int communityId, int schoolId)
        {
            int debugId = 0;
            try
            {
                return _reportQueueRpst.GetCircleDataExportStudentModels(communityId, schoolId, out debugId);
            }
            catch (Exception ex)
            {
                LoggerHelper.Debug(ex);
                LoggerHelper.Debug("error student Id: " + debugId.ToString());
                return new List<CircleDataExportStudentModel>();
            }
        }

        /// <summary>
        ///TSR: Coaching Hours by Community 
        /// </summary>
        /// <returns></returns>
        public List<CoachingHoursbyCommunityModel> GetCoachingHoursbyCommunityModel()
        {
            try
            {
                return _reportQueueRpst.GetCoachingHoursbyCommunityModel();
            }
            catch (Exception ex)
            {
                LoggerHelper.Debug(ex);
                return null;
            }
        }

        #region Assessment Report Template
        public IQueryable<AssessmentReportTemplateEntity> AssReportTemplates
        {
            get { return _assessmentReportTemplatesRpst.Entities; }
        }

        public AssessmentReportTemplateEntity GetAssReportTemplate(int id)
        {
            if (id < 1)
                return null;
            return _assessmentReportTemplatesRpst.GetByKey(id);
        }

        public OperationResult InsertAssReportTemplate(AssessmentReportTemplateEntity entity)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                _assessmentReportTemplatesRpst.Insert(entity);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult UpdateAssReportTemplate(AssessmentReportTemplateEntity entity)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                _assessmentReportTemplatesRpst.Update(entity);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult DeleteAssReportTemplate(int id)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                _assessmentReportTemplatesRpst.Delete(id);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }
        #endregion

        #region parent report
        public IQueryable<ParentReportEntity> ParentReports
        {
            get { return _parentReportRpst.Entities; }
        }
        public OperationResult InsertParentReport(ParentReportEntity entity)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                _parentReportRpst.Insert(entity);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult InsertParentReportList(List<ParentReportEntity> entityList)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                _parentReportRpst.Insert(entityList);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult UpdateParentReport(ParentReportEntity entity)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                _parentReportRpst.Update(entity);
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
