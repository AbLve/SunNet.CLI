using Sunnet.Cli.Core.Cpalls.Entities;
using Sunnet.Cli.Core.Cpalls.Models.Report;
using Sunnet.Framework.Core.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Cli.Core.Cpalls.Interfaces
{
    public interface ICpallsSchoolRpst : IRepository<CpallsSchoolEntity, int>
    {
        /// <summary>
        /// 查找所有(未删除的正常状态)的Measure
        /// </summary>
        /// <param name="assessmentId">The assessment identifier.</param>
        /// <returns></returns>
        List<ReportMeasureHeaderModel> GetReportMeasureHeaders(int assessmentId);

        /// <summary>
        /// 查找所有的<paramref name="schools" />在<paramref name="schoolYear" />的Assessment结果,School 统计结果
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
        /// 查找所有的<paramref name="schools" />在<paramref name="schoolYear" />的Assessment结果,School 统计结果
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
        /// 查找<paramref name="schoolId" />在<paramref name="schoolYear" />的Assessment结果,学生详细结果
        /// </summary>
        /// <param name="assessmentId">The assessment identifier.</param>
        /// <param name="schoolYear">The school year.</param>
        /// <param name="schoolId">The school.</param>
        /// <param name="startDate">The start date.</param>
        /// <param name="endDate">The end date.</param>
        /// <returns></returns>
        List<StudentRecordModel> GetReportStudentRecords(int assessmentId, string schoolYear, int schoolId, DateTime startDate, DateTime endDate, IList<int> listClassId);

        /// <summary>
        /// 查找<paramref name="districtId"/>在<paramref name="schoolYear"/>的Assessment学生详细结果
        /// </summary>
        /// <param name="assessmentId">The assessment identifier.</param>
        /// <param name="schoolYear">The school year.</param>
        /// <param name="districtId">The district identifier.</param>
        /// <returns></returns>
        List<StudentRecordModel> GetReportStudentRecordsByDistrict(int assessmentId, string schoolYear, int districtId, List<Wave> waves);

        /// <summary>
        /// 查找<paramref name="schoolId" />在<paramref name="schoolYear" />的Assessment结果,学生详细结果
        /// </summary>
        /// <param name="assessmentId">The assessment identifier.</param>
        /// <param name="schoolYear">The school year.</param>
        /// <param name="schoolIds">The school ids.</param>
        /// <param name="startDate">The start date.</param>
        /// <param name="endDate">The end date.</param>
        /// <returns></returns>
        List<StudentRecordModel> GetReportStudentRecords(int assessmentId, string schoolYear, IEnumerable<int> schoolIds, DateTime startDate, DateTime endDate,
            DateTime dobStartDate, DateTime dobEndDate, IList<int> listClassId);

        /// <summary>
        /// 查找<paramref name="schoolId" />在<paramref name="schoolYear" />的Assessment结果,学生详细结果
        /// </summary>
        /// <param name="assessmentId">The assessment identifier.</param>
        /// <param name="schoolYear">The school year.</param>
        /// <param name="schoolId">The school.</param>
        /// <param name="studentIds">The student ids.</param>
        /// <param name="startDate">The start date.</param>
        /// <param name="endDate">The end date.</param>
        /// <returns></returns>
        List<StudentRecordModel> GetReportStudentRecords(int assessmentId, string schoolYear, int schoolId, IEnumerable<int> studentIds, DateTime startDate, DateTime endDate);
    }
}
