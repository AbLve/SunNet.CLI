using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sunnet.Cli.Core.Common.Enums;
using Sunnet.Cli.Core.Reports.Entities;
using Sunnet.Framework.Core.Base;
using Sunnet.Cli.Core.Reports.Models;

namespace Sunnet.Cli.Core.Reports.Interfaces
{

    public interface IReportQueueRpst : IRepository<ReportQueueEntity, int>
    {
        #region TSR Turnover Report
        /// <summary>
        /// SchoolIds 为空 并且 communityIds 为空，表示管理员查看
        /// </summary>
        List<TeacherSchoolTypeModel> GetTeacherBySchoolType(DateTime start, DateTime end, List<int> communityIds, List<int> schoolIds);

        /// <summary>
        ///  SchoolIds 为空 并且 communityIds 为空，表示管理员查看
        /// </summary>
        List<CountTeacherbyCommunityModel> GetCountTeacherbyCommunityList(DateTime start, DateTime end, List<int> communityIds, List<int> schoolIds);
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

        List<CircleDataExportStudentModel> GetCircleDataExportStudentModels(int communityId, int schoolId, out int debugId);

        /// <summary>
        /// TSR: Coaching Hours by Community 
        /// </summary>
        /// <returns></returns>
        List<CoachingHoursbyCommunityModel> GetCoachingHoursbyCommunityModel();

        List<TurnoverTeacherModel> GetTurnoverTeacherModels(DateTime start, DateTime end, List<int> communities, List<int> schools);
    }
}
