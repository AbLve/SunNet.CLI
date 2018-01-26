using System.Data;
using System.Globalization;
using Sunnet.Cli.Core.Common.Enums;
using Sunnet.Cli.Core.Reports.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sunnet.Framework.Core.Tool;
using Sunnet.Cli.Core.Reports.Models;

namespace Sunnet.Cli.Core.Reports
{
    public interface IReportContract
    {
        #region Media Consent Report
        List<MediaConsentPercentModel> GetMediaConsentPercent(List<int> communityIds, List<int> schoolIds, string teacher);
        List<MediaConsentDetailModel> GetMediaConsentDetail(List<int> communityIds, List<int> schoolIds, string teacher);
        #endregion

        #region Beech Report
        List<BeechReportModel> GetBeechReport(List<int> communityIds, List<int> schoolIds, string teacher);
        #endregion

        #region Coach Report
        List<CoachReportModel> GetCoachReport(List<int> communityIds, int mentorCoach, string funding, int status);
        #endregion

        #region Querterly-like reports
        List<QuarterlyReportModel> GetYearsInProjectCountBySchoolType(List<int> communityIds, List<int> fundingList,
            DateTime? startDate, DateTime? endDate, int status);
        #endregion

        #region Ever serviced Report
        List<ServiceReportModel> GetServiceReport(List<int> communityIds, List<int> schoolIds);
        #endregion

        #region Report Queue
        IQueryable<ReportQueueEntity> ReportQueues { get; }

        ReportQueueEntity NewReportQueueEntity();

        ReportQueueEntity GetReportQueue(int id);

        OperationResult InsertReportQueue(ReportQueueEntity entity);

        OperationResult UpdateReportQueue(ReportQueueEntity entity);

        OperationResult InsertReportQueueList(List<ReportQueueEntity> entityList);
        #endregion

        List<Community_Mentor_TeacherModel> GetCommunity_Mentor_Teachers();

        #region TSR Turnover Report
        /// <summary>
        ///  SchoolIds 为空 并且 communityIds 为空，表示管理员查看
        /// </summary>
        List<TeacherSchoolTypeModel> GetTeacherBySchoolType(DateTime start, DateTime end, List<int> communityIds, List<int> schoolIds);

        /// <summary>
        ///  SchoolIds 为空 并且 communityIds 为空，表示管理员查看
        /// </summary>
        List<CountTeacherbyCommunityModel> GetCountTeacherbyCommunityList(DateTime start, DateTime end, List<int> communityIds, List<int> schoolIds);

        List<TurnoverTeacherModel> GetTurnoverTeacherModels(DateTime startDate, DateTime endDate, List<int> communities, List<int> schools);

        #endregion


        /// <summary>
        /// TSR: Count by Facility Type
        /// </summary>
        /// <returns></returns>
        List<CountbyFacilityTypeMode> GetCountbyFacilityTypeList(List<int> communityIds, List<int> fundingList,
            DateTime? startDate, DateTime? endDate, int status);


        /// <summary>
        /// TSR: Count by Community
        /// </summary>
        /// <returns></returns>
        List<CountbyCommunityModel> GetCountbyCommunityList(List<int> communityIds, List<int> fundingList,
            DateTime? startDate, DateTime? endDate, int status);

        #region pd report
        List<PDReportModel> PDCompletionCourseReport(List<int> communityIds, List<int> schoolIds, string teacher, int status);
        #endregion

        List<CircleDataExportStudentModel> GetCircleDataExportStudentModels(int communityId, int schoolId);

        /// <summary>
        ///TSR: Coaching Hours by Community 
        /// </summary>
        /// <returns></returns>
        List<CoachingHoursbyCommunityModel> GetCoachingHoursbyCommunityModel();

        #region Assessment Report Template
        IQueryable<AssessmentReportTemplateEntity> AssReportTemplates { get; }

        AssessmentReportTemplateEntity GetAssReportTemplate(int id);

        OperationResult InsertAssReportTemplate(AssessmentReportTemplateEntity entity);

        OperationResult UpdateAssReportTemplate(AssessmentReportTemplateEntity entity);

        OperationResult DeleteAssReportTemplate(int id);

 

        #endregion
        #region Parent Report
        IQueryable<ParentReportEntity> ParentReports { get; }
        OperationResult InsertParentReport(ParentReportEntity entity);

        OperationResult InsertParentReportList(List<ParentReportEntity> entityList);

        OperationResult UpdateParentReport(ParentReportEntity entity);

        #endregion
    }
}
