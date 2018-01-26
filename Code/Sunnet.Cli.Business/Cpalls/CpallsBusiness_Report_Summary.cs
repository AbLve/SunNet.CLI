using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Sunnet.Cli.Business.Cpalls.Growth;
using Sunnet.Cli.Business.Cpalls.Models;
using Sunnet.Cli.Business.Cpalls.Models.Report;
using Sunnet.Cli.Core.Cpalls.Entities;
using Sunnet.Cli.Core.Cpalls.Models;
using Sunnet.Cli.Core.Cpalls.Models.Report;
using Sunnet.Cli.Core.Students.Entities;
using LinqKit;
using Sunnet.Cli.Business.Classes.Models;
using Sunnet.Cli.Core.Users.Entities;
using Sunnet.Cli.Core.Ade.Entities;
using Sunnet.Framework;
using Sunnet.Framework.Core.Tool;
using Sunnet.Cli.Core.Classes.Entites;
using Sunnet.Cli.Business.Common;
using Sunnet.Cli.Core.Common.Enums;
using Sunnet.Cli.Core.Schools.Entities;
using Sunnet.Cli.Business.Schools;
using Sunnet.Framework.Core.Extensions;
using Sunnet.Framework.Extensions;
using Sunnet.Cli.Core.Ade;
using Sunnet.Cli.Core.Cpalls;
using Microsoft.Office.Interop.Excel;
using Sunnet.Cli.Business.Ade.Models;

namespace Sunnet.Cli.Business.Cpalls
{
    public partial class CpallsBusiness
    {
        // Summary Report
        public Dictionary<object, List<ReportRowModel>> GetSchoolSummaryReport(int assessmentId, UserBaseEntity user, int year,
            int districtId, Dictionary<Wave, IEnumerable<int>> selectedWaves, DateTime startDate, DateTime endDate, 
            DateTime dobStartDate, DateTime dobEndDate)
        {
            var schoolYear = string.Format("{0}-{1}", year.ToString().Substring(2), (year + 1).ToString().Substring(2));
            int t;
            var schools = SchoolBusiness.GetSchoolList(s => s.Status == SchoolStatus.Active
                && (districtId < 1 || s.CommunitySchoolRelations.Any(com => com.CommunityId == districtId))
                && s.SchoolType.Name.StartsWith("Demo") == false,
                user, "Name", "ASC", 0, int.MaxValue, out t);
            var schoolIds = string.Join(",", schools.Select(x => x.ID).Distinct().ToList());
            if (schoolIds.Length > 0)
                schoolIds += ",";
            var headers = _cpallsContract.GetReportMeasureHeaders(assessmentId);
            IList<int> classIds = ClassBusiness.GetClassIdsForReport(districtId, assessmentId);
            var records =
                _cpallsContract.GetReportSchoolRecords(assessmentId, schoolYear, schoolIds, startDate, endDate, dobStartDate, dobEndDate, classIds) ??
                new List<SchoolRecordModel>();
            var actualWaves = new Dictionary<Wave, IEnumerable<int>>();
            selectedWaves.ForEach(x =>
            {
                if (x.Value != null && x.Value.Any())
                    actualWaves.Add(x.Key, x.Value);
            });
            var benchmarks = _adeBusiness.GetIEnumBenchmarks(assessmentId);
            var generator = new AverageByWavePdfCommunityGenerator(schools, headers, records, actualWaves, benchmarks);
            generator.ShowTotalForMeasure = true;
            generator.ShowTotalScore = true;
            generator.ShowAveragePerSource = true;
            generator.Generate();
            return generator.Reports;
        }

        // Summary Percentile Rank Report
        public Dictionary<object, List<ReportRowModel>> GetSchoolSummaryPercentileRankReport(int assessmentId, UserBaseEntity user, int year,
            int districtId, Dictionary<Wave, IEnumerable<int>> selectedWaves, DateTime startDate, DateTime endDate,
            DateTime dobStartDate, DateTime dobEndDate)
        {
            var schoolYear = string.Format("{0}-{1}", year.ToString().Substring(2), (year + 1).ToString().Substring(2));
            int t;
            var schools = SchoolBusiness.GetSchoolList(s => s.Status == SchoolStatus.Active
                && (districtId < 1 || s.CommunitySchoolRelations.Any(com => com.CommunityId == districtId))
                && s.SchoolType.Name.StartsWith("Demo") == false,
                user, "Name", "ASC", 0, int.MaxValue, out t);
            var schoolIds = string.Join(",", schools.Select(x => x.ID).Distinct().ToList());
            if (schoolIds.Length > 0)
                schoolIds += ",";
            var headers = _cpallsContract.GetReportMeasureHeaders(assessmentId);
            IList<int> classIds = ClassBusiness.GetClassIdsForReport(districtId, assessmentId);
            var records =
                _cpallsContract.GetReportSchoolPercentileRankRecords(assessmentId, schoolYear, schoolIds, startDate, endDate,
                dobStartDate, dobEndDate, classIds) ??
                new List<SchoolPercentileRankModel>();
            var actualWaves = new Dictionary<Wave, IEnumerable<int>>();
            selectedWaves.ForEach(x =>
            {
                if (x.Value != null && x.Value.Any())
                    actualWaves.Add(x.Key, x.Value);
            });
            var benchmarks = _adeBusiness.GetIEnumBenchmarks(assessmentId);
            var generator = new AverageByWavePdfCommunityPercentileRankGenerator(schools, headers, records, actualWaves, benchmarks);
            generator.ShowTotalForMeasure = true;
            generator.ShowTotalScore = false;
            generator.ShowAveragePerSource = true;
            generator.Generate();
            return generator.Reports;
        }

        public Dictionary<object, List<ReportRowModel>> GetSchoolSatisfactoryReport(int assessmentId, UserBaseEntity user, int year,
            int districtId, Dictionary<Wave, IEnumerable<int>> selectedWaves, DateTime startDate, DateTime endDate,
            DateTime dobStartDate, DateTime dobEndDate, bool mergeSameLabel)
        {
            var schoolYear = string.Format("{0}-{1}", year.ToString().Substring(2), (year + 1).ToString().Substring(2));

            Stopwatch swSingle = new Stopwatch();
            var totalTime = 0d;
            swSingle.Start();
            _logger.Info("Start GetSchoolSatisfactoryReport");

            var headers = _cpallsContract.GetReportMeasureHeaders(assessmentId);
            swSingle.Stop();
            totalTime += swSingle.Elapsed.TotalMilliseconds;
            _logger.Info("Get measure for header: {0} ms/ {1} ms", swSingle.Elapsed.TotalMilliseconds, totalTime);
            swSingle.Restart();
            var schoolIds = SchoolBusiness.GetAssignedSchoolIdsWithoutDemo(districtId);
            IList<int> classIds = ClassBusiness.GetClassIdsForReport(districtId, assessmentId);
            List<StudentRecordModel> records = _cpallsContract.GetReportStudentRecords(assessmentId, schoolYear, schoolIds, startDate, endDate,
            dobStartDate, dobEndDate, classIds) ??
                new List<StudentRecordModel>();
            swSingle.Stop();

            totalTime += swSingle.Elapsed.TotalMilliseconds;
            _logger.Info("Get records: {0} ms/ {1} ms", swSingle.Elapsed.TotalMilliseconds, totalTime);
            swSingle.Restart();

            records.ForEach(stuScore =>
            {
                stuScore.ParentId = headers.Where(x => x.ID == stuScore.MeasureId).Select(x => x.ParentId).FirstOrDefault();
            });

            var query = from r in records
                        where r.Goal >= 0
                        && (headers.Any(child => child.ID == r.MeasureId && child.TotalScored)
                            || records.Any(child => child.ParentId == r.MeasureId && child.StudentId == r.StudentId
                                                    && child.Wave == r.Wave
                                                    && headers.Any(z => z.ID == child.MeasureId && z.TotalScored)))
                        group r by new { r.SchoolId, r.MeasureId, r.Wave, r.BenchmarkId }
                            into g
                        select new SchoolRecordModel()
                        {
                            SchoolId = g.Key.SchoolId,
                            MeasureId = g.Key.MeasureId,
                            Wave = g.Key.Wave,
                            Satisfied = g.Count(x => x.Goal >= x.LowerScore && x.Goal <= x.HigherScore),
                            Count = g.Count(x => x.Goal >= 0),
                            BenchmarkId = g.Key.BenchmarkId
                        };
            var schoolRecords = query.ToList();
            var actualWaves = new Dictionary<Wave, IEnumerable<int>>();
            selectedWaves.ForEach(x =>
            {
                if (x.Value != null && x.Value.Any())
                    actualWaves.Add(x.Key, x.Value);
            });
            swSingle.Stop();
            totalTime += swSingle.Elapsed.TotalMilliseconds;
            _logger.Info("Calc school's count: {0} ms/ {1} ms", swSingle.Elapsed.TotalMilliseconds, totalTime);
            swSingle.Restart();

            var benchmarks = _adeBusiness.GetIEnumBenchmarks(assessmentId);
            var generator = new SatisfactoryByWavePdfCommunityGenerator(SatisfactoryType.Totaly, null, headers, schoolRecords, actualWaves, benchmarks);
            generator.ShowTotalForMeasure = true;
            generator.MergeSameLabel = mergeSameLabel;
            generator.Generate();
            swSingle.Stop();
            totalTime += swSingle.Elapsed.TotalMilliseconds;
            _logger.Info("Generate report data: {0} ms/ {1} ms", swSingle.Elapsed.TotalMilliseconds, totalTime);

            _logger.Info("Over, start render html");
            return generator.Reports;
        }

        public Dictionary<object, List<ReportRowModel>> GetClassSummaryReport(int assessmentId, UserBaseEntity user, int year, int schoolId,
            Dictionary<Wave, IEnumerable<int>> selectedWaves, DateTime startDate, DateTime endDate,
            DateTime dobStartDate, DateTime dobEndDate)
        {
            var schoolYear = string.Format("{0}-{1}", year.ToString().Substring(2), (year + 1).ToString().Substring(2));
            int t;
            List<CpallsClassModel> classes = ClassBusiness.GetClassList(x => x.SchoolId == schoolId && x.Status == EntityStatus.Active,
                user, "Name", "ASC", 0, int.MaxValue, out t);
            var headers = _cpallsContract.GetReportMeasureHeaders(assessmentId);

            IList<int> classIds = ClassBusiness.GetClassIdsForReport(schoolId, user, assessmentId);
            classes = classes.Where(c => classIds.Contains(c.ID)).ToList();
            IList<int> studentIds = StudentBusiness.GetStudentIdsByClassId(classIds, dobStartDate, dobEndDate);
            var records = _cpallsContract.GetReportStudentRecords(assessmentId, schoolYear, schoolId, startDate, endDate, classIds, studentIds) ?? new List<StudentRecordModel>()
                 ?? new List<StudentRecordModel>();

            var actualWaves = new Dictionary<Wave, IEnumerable<int>>();
            selectedWaves.ForEach(x =>
            {
                if (x.Value != null && x.Value.Any())
                    actualWaves.Add(x.Key, x.Value);
            });
            var benchmarks = _adeBusiness.GetIEnumBenchmarks(assessmentId);
            var generator = new AverageByWavePdfSchoolGenerator(classes, headers, records, actualWaves, benchmarks);
            generator.ShowTotalForMeasure = true;
            generator.ShowTotalScore = true;
            generator.ShowAveragePerSource = true;
            generator.Generate();
            return generator.Reports;
        }

        public Dictionary<object, List<ReportRowModel>> ClassPercentileRankAverage(int assessmentId, UserBaseEntity user, int year, int schoolId,
            Dictionary<Wave, IEnumerable<int>> selectedWaves, DateTime startDate, DateTime endDate, DateTime dobStartDate, DateTime dobEndDate)
        {
            var schoolYear = string.Format("{0}-{1}", year.ToString().Substring(2), (year + 1).ToString().Substring(2));
            int t;
            List<CpallsClassModel> classes = ClassBusiness.GetClassList(x => x.SchoolId == schoolId && x.Status == EntityStatus.Active,
                user, "Name", "ASC", 0, int.MaxValue, out t);
            var headers = _cpallsContract.GetReportMeasureHeaders(assessmentId);

            IList<int> classIds = ClassBusiness.GetClassIdsForReport(schoolId, user, assessmentId);
            classes = classes.Where(c => classIds.Contains(c.ID)).ToList();
            IList<int> studentIds = StudentBusiness.GetStudentIdsByClassId(classIds, dobStartDate, dobEndDate);
            var records = _cpallsContract.GetReportStudentRecords(assessmentId, schoolYear, schoolId, startDate, endDate, classIds, studentIds) ?? new List<StudentRecordModel>()
                 ?? new List<StudentRecordModel>();

            var actualWaves = new Dictionary<Wave, IEnumerable<int>>();
            selectedWaves.ForEach(x =>
            {
                if (x.Value != null && x.Value.Any())
                    actualWaves.Add(x.Key, x.Value);
            });
            var benchmarks = _adeBusiness.GetIEnumBenchmarks(assessmentId);
            var generator = new AverageByWavePdfClassPercentileRankAverageGenerator(classes, headers, records, actualWaves, benchmarks);
            generator.ShowTotalForMeasure = true;
            generator.ShowTotalScore = false;
            generator.ShowAveragePerSource = true;
            generator.Generate();
            return generator.Reports;
        }

        public Dictionary<object, List<ReportRowModel>> GetClassSatisfactoryReport(int assessmentId, UserBaseEntity user, int year, int schoolId,
            Dictionary<Wave, IEnumerable<int>> selectedWaves, DateTime startDate, DateTime endDate, DateTime dobStartDate, DateTime dobEndDate, 
            IEnumerable<BenchmarkModel> benchmarks, bool mergeSameLabel)
        {
            var schoolYear = string.Format("{0}-{1}", year.ToString().Substring(2), (year + 1).ToString().Substring(2));
            int t;

            List<CpallsClassModel> classes = ClassBusiness.GetClassList(x => x.SchoolId == schoolId && x.Status == EntityStatus.Active,
                user, "Name", "ASC", 0, int.MaxValue, out t);

            var headers = _cpallsContract.GetReportMeasureHeaders(assessmentId);
            IList<int> classIds = ClassBusiness.GetClassIdsForReport(schoolId, user, assessmentId);
            IList<int> studentIds = StudentBusiness.GetStudentIdsByClassId(classIds, dobStartDate, dobEndDate);
            var records = _cpallsContract.GetReportStudentRecords(assessmentId, schoolYear, schoolId, startDate, endDate, classIds, studentIds) ?? new List<StudentRecordModel>()
                ?? new List<StudentRecordModel>();

            records.ForEach(stuScore =>
            {
                stuScore.ParentId = headers.Where(x => x.ID == stuScore.MeasureId).Select(x => x.ParentId).FirstOrDefault();
            });

            //studentIds.ForEach(stuId =>
            //{
            //    var recordsOfStu = records.Where(x => x.StudentId == stuId).ToList();
            //    var parents = recordsOfStu.Where(sm => headers.Any(x => x.ParentId == sm.MeasureId));
            //    parents.ForEach(p =>
            //    {
            //        p.Goal = recordsOfStu.Where(x => x.ParentId == p.MeasureId).Sum(record => record.Goal);
            //    });
            //});

            var actualWaves = new Dictionary<Wave, IEnumerable<int>>();
            selectedWaves.ForEach(x =>
            {
                if (x.Value != null && x.Value.Any())
                    actualWaves.Add(x.Key, x.Value);
            });
            var generator = new SatisfactoryByWavePdfSchoolGenerator(SatisfactoryType.Totaly, classes, headers, records, actualWaves, benchmarks);
            generator.ShowTotalForMeasure = true;
            generator.MergeSameLabel = mergeSameLabel;
            generator.Generate();
            return generator.Reports;
        }

        public Dictionary<object, List<ReportRowModel>> GetStudentSummaryReport(int assessmentId, StudentAssessmentLanguage language, UserBaseEntity user, int year, int schoolId, int classId,
            Dictionary<Wave, IEnumerable<int>> selectedWaves, DateTime startDate, DateTime endDate, DateTime dobStart, DateTime dobEnd)
        {
            var schoolYear = string.Format("{0}-{1}", year.ToString().Substring(2), (year + 1).ToString().Substring(2));
            int t;
            IList<int> classIds = ClassBusiness.GetClassIdsForReport(schoolId, user, assessmentId);
            List<CpallsStudentModel> students = StudentBusiness.GetClassStudents(x =>
                x.SchoolRelations.Any(s => s.SchoolId == schoolId)
                && x.Classes.Any(c => c.ID == classId && c.IsDeleted == false && classIds.Contains(c.ID))
                && x.Status == EntityStatus.Active
                && (x.AssessmentLanguage == language || x.AssessmentLanguage == StudentAssessmentLanguage.Bilingual)
                && x.BirthDate >= dobStart && x.BirthDate <= dobEnd
                , user, "FirstName", "ASC", 0, int.MaxValue, out t);
            var headers = _cpallsContract.GetReportMeasureHeaders(assessmentId);
            var measureIds = headers.Select(x => x.ID).Distinct().ToList();
            measureIds.AddRange(headers.Select(x => x.ParentId).Distinct().ToList());

            var records = _cpallsContract.GetReportStudentRecords(assessmentId, schoolYear, schoolId, startDate, endDate, classIds, students.Select(s => s.ID).Distinct().ToList())
                ?? new List<StudentRecordModel>();

            var actualWaves = new Dictionary<Wave, IEnumerable<int>>();
            selectedWaves.ForEach(x =>
            {
                if (x.Value != null && x.Value.Any())
                    actualWaves.Add(x.Key, x.Value);
            });
            var benchmarks = _adeBusiness.GetIEnumBenchmarks(assessmentId);
            var generator = new AverageByWavePdfClassGenerator(students, headers, records, actualWaves, benchmarks);
            generator.ShowTotalForMeasure = true;
            generator.ShowTotalScore = true;
            generator.Generate();
            return generator.Reports;
        }

        public Dictionary<object, List<ReportRowModel>> GetClassPercentileRankReport(int assessmentId, StudentAssessmentLanguage language, UserBaseEntity user, int year, int schoolId, int classId,
            Dictionary<Wave, IEnumerable<int>> selectedWaves, DateTime startDate, DateTime endDate, DateTime dobStart, DateTime dobEnd)
        {
            var schoolYear = string.Format("{0}-{1}", year.ToString().Substring(2), (year + 1).ToString().Substring(2));
            int t;
            IList<int> classIds = ClassBusiness.GetClassIdsForReport(schoolId, user, assessmentId);
            List<CpallsStudentModel> students = StudentBusiness.GetClassStudents(x =>
                x.SchoolRelations.Any(s => s.SchoolId == schoolId)
                && x.Classes.Any(c => c.ID == classId && c.IsDeleted == false && classIds.Contains(c.ID))
                && x.Status == EntityStatus.Active
                && (x.AssessmentLanguage == language || x.AssessmentLanguage == StudentAssessmentLanguage.Bilingual)
                && x.BirthDate >= dobStart && x.BirthDate <= dobEnd
                , user, "FirstName", "ASC", 0, int.MaxValue, out t);
            var headers = _cpallsContract.GetReportMeasureHeaders(assessmentId);
            var measureIds = headers.Select(x => x.ID).Distinct().ToList();
            measureIds.AddRange(headers.Select(x => x.ParentId).Distinct().ToList());

            var records = _cpallsContract.GetReportStudentRecords(assessmentId, schoolYear, schoolId, startDate, endDate, classIds, students.Select(s => s.ID).Distinct().ToList())
                ?? new List<StudentRecordModel>();

            var actualWaves = new Dictionary<Wave, IEnumerable<int>>();
            selectedWaves.ForEach(x =>
            {
                if (x.Value != null && x.Value.Any())
                    actualWaves.Add(x.Key, x.Value);
            });
            var benchmarks = _adeBusiness.GetIEnumBenchmarks(assessmentId);
            var generator = new AverageByWavePdfClassPercentileRankGenerator(students, headers, records, actualWaves, benchmarks);
            generator.ShowTotalForMeasure = true;
            generator.ShowTotalScore = false;
            generator.Generate();
            return generator.Reports;
        }

        public Dictionary<object, List<ReportRowModel>> GetStudentSatisfactoryReport(int assessmentId, StudentAssessmentLanguage language,
            UserBaseEntity user, int year,
            int schoolId, int classId, Dictionary<Wave, IEnumerable<int>> selectedWaves, DateTime startDate, DateTime endDate,
            IEnumerable<BenchmarkModel> benchmarks, bool mergeSameLabel, DateTime dobStart, DateTime dobEnd)
        {
            var schoolYear = string.Format("{0}-{1}", year.ToString().Substring(2), (year + 1).ToString().Substring(2));
            int t;
            IList<int> classIds = ClassBusiness.GetClassIdsForReport(schoolId, user, assessmentId);
            List<CpallsStudentModel> students = StudentBusiness.GetClassStudents(x =>
                x.SchoolRelations.Any(s => s.SchoolId == schoolId)
                && x.Classes.Any(c => c.ID == classId && c.IsDeleted == false && classIds.Contains(c.ID))
                && x.Status == EntityStatus.Active
                && (x.AssessmentLanguage == language || x.AssessmentLanguage == StudentAssessmentLanguage.Bilingual)
                && x.BirthDate >= dobStart && x.BirthDate <= dobEnd
                , user, "FirstName", "ASC", 0, int.MaxValue, out t) ?? new List<CpallsStudentModel>();
            var headers = _cpallsContract.GetReportMeasureHeaders(assessmentId);

            var records = _cpallsContract.GetReportStudentRecords(assessmentId, schoolYear, schoolId, startDate, endDate, classIds,
                students.Select(s => s.ID).Distinct().ToList())
                          ?? new List<StudentRecordModel>();

            records.ForEach(stuScore =>
            {
                stuScore.ParentId = headers.Where(x => x.ID == stuScore.MeasureId).Select(x => x.ParentId).FirstOrDefault();
            });
            var actualWaves = new Dictionary<Wave, IEnumerable<int>>();
            selectedWaves.ForEach(x =>
            {
                if (x.Value != null && x.Value.Any())
                    actualWaves.Add(x.Key, x.Value);
            });
            var generator = new SatisfactoryByWavePdfClassGenerator(SatisfactoryType.Totaly, students, headers, records, actualWaves, benchmarks);
            generator.ShowTotalForMeasure = true;
            generator.MergeSameLabel = mergeSameLabel;
            generator.Generate();
            return generator.Reports;
        }

        private static string OkMark
        {
            get { return Encoding.Unicode.GetString(new byte[] { 26, 34 }); }
        }

        /// <summary>
        /// Gets the student report.
        /// </summary>
        /// <param name="assessmentId">The assessment identifier.</param>
        /// <param name="user">The user.</param>
        /// <param name="year">The year.</param>
        /// <param name="schoolId">The school identifier.</param>
        /// <param name="classId">The class identifier.</param>
        /// <param name="studentId">0: entire class, other wise: the selected student only.</param>
        /// <param name="selectedWaves">The selected waves.</param>
        /// <returns></returns>
        public Dictionary<object, List<ReportRowModel>> GetStudentReport(int assessmentId, StudentAssessmentLanguage language, bool printComments,
            UserBaseEntity user, int year, int schoolId, int classId, int studentId, Dictionary<Wave, IEnumerable<int>> selectedWaves, DateTime? dobStartDate, DateTime? dobEndDate)
        {
            var schoolYear = string.Format("{0}-{1}", year.ToString().Substring(2), (year + 1).ToString().Substring(2));
            int t;
            DateTime dobStart = dobStartDate ?? CommonAgent.MinDate;
            DateTime dobEnd = dobEndDate ?? DateTime.Now;
            IList<int> classIds = ClassBusiness.GetClassIdsForReport(schoolId, user, assessmentId);
            List<CpallsStudentModel> students = StudentBusiness.GetClassStudents(x =>
                x.SchoolRelations.Any(s => s.SchoolId == schoolId)
                && x.Classes.Any(c => c.ID == classId && c.IsDeleted == false && classIds.Contains(c.ID))
                && x.Status == EntityStatus.Active
                && (x.AssessmentLanguage == language || x.AssessmentLanguage == StudentAssessmentLanguage.Bilingual)
                && (studentId == 0 || x.ID == studentId)
                && x.BirthDate >= dobStart && x.BirthDate <= dobEnd
                , user, "FirstName", "ASC", 0, int.MaxValue, out t);

            var headers = _cpallsContract.GetReportMeasureHeaders(assessmentId);

            var actualWaves = new Dictionary<Wave, IEnumerable<int>>();
            selectedWaves.ForEach(x =>
            {
                if (x.Value != null && x.Value.Any())
                    actualWaves.Add(x.Key, x.Value);
            });
            List<StudentRecordModel> records = _cpallsContract.GetReportStudentRecords(assessmentId, schoolYear,
                schoolId, CommonAgent.MinDate, DateTime.Now.AddDays(1), classIds,
                students.Select(s => s.ID).Distinct().ToList())
                                               ?? new List<StudentRecordModel>();
            var benchmarks = _adeBusiness.GetIEnumBenchmarks(assessmentId);
            var generator = new AverageBySourcePdfStudentGenerator(students, headers, records, actualWaves, benchmarks);
            generator.ShowTotalForMeasure = true;
            generator.ShowTotalScore = true;
            generator.PrintComment = printComments;
            generator.Generate();
            return generator.Reports;
        }

        /// <summary>
        /// Gets the student report.
        /// </summary>
        /// <param name="assessmentId">The assessment identifier.</param>
        /// <param name="user">The user.</param>
        /// <param name="year">The year.</param>
        /// <param name="schoolId">The school identifier.</param>
        /// <param name="classId">The class identifier.</param>
        /// <param name="studentId">0: entire class, other wise: the selected student only.</param>
        /// <param name="selectedWaves">The selected waves.</param>
        /// <returns></returns>
        public Dictionary<object, List<ReportRowModel>> GetStudentPercentileRankReport(int assessmentId, StudentAssessmentLanguage language, bool printComments,
            UserBaseEntity user, int year, int schoolId, int classId, int studentId, Dictionary<Wave, IEnumerable<int>> selectedWaves, DateTime dobStart, DateTime dobEnd)
        {
            var schoolYear = string.Format("{0}-{1}", year.ToString().Substring(2), (year + 1).ToString().Substring(2));
            int t;
            IList<int> classIds = ClassBusiness.GetClassIdsForReport(schoolId, user, assessmentId);
            List<CpallsStudentModel> students = StudentBusiness.GetClassStudents(x =>
                x.SchoolRelations.Any(s => s.SchoolId == schoolId)
                && x.Classes.Any(c => c.ID == classId && c.IsDeleted == false && classIds.Contains(c.ID))
                && x.Status == EntityStatus.Active
                && (x.AssessmentLanguage == language || x.AssessmentLanguage == StudentAssessmentLanguage.Bilingual)
                && (studentId == 0 || x.ID == studentId)
                && x.BirthDate >= dobStart && x.BirthDate <= dobEnd
                , user, "FirstName", "ASC", 0, int.MaxValue, out t);

            var headers = _cpallsContract.GetReportMeasureHeaders(assessmentId);

            var actualWaves = new Dictionary<Wave, IEnumerable<int>>();
            selectedWaves.ForEach(x =>
            {
                if (x.Value != null && x.Value.Any())
                    actualWaves.Add(x.Key, x.Value);
            });
            List<StudentRecordModel> records = _cpallsContract.GetReportStudentRecords(assessmentId, schoolYear,
                schoolId, CommonAgent.MinDate, DateTime.Now.AddDays(1), classIds,
                students.Select(s => s.ID).Distinct().ToList())
                                               ?? new List<StudentRecordModel>();
            var benchmarks = _adeBusiness.GetIEnumBenchmarks(assessmentId);
            var generator = new AverageBySourcePdfStudentPercentileRankGenerator(students, headers, records, actualWaves, benchmarks);
            generator.ShowTotalForMeasure = true;
            generator.ShowTotalScore = false;
            generator.PrintComment = printComments;
            generator.Generate();
            return generator.Reports;
        }

        #region

        /// <summary>
        /// Gets the student report.
        /// </summary>
        /// <param name="assessmentId">The assessment identifier.</param>
        /// <param name="user">The user.</param>
        /// <param name="year">The year.</param>
        /// <param name="schoolId">The school identifier.</param>
        /// <param name="classId">The class identifier.</param>
        /// <param name="studentId">0: entire class, other wise: the selected student only.</param>
        /// <param name="selectedWaves">The selected waves.</param>
        /// <returns></returns>
        public Dictionary<object, List<ReportRowModel>> GetStudentParentReport(int assessmentId, StudentAssessmentLanguage language, bool printComments, UserBaseEntity user, int year, int schoolId, int classId, int studentId, Dictionary<Wave, IEnumerable<int>> selectedWaves)
        {
            var schoolYear = string.Format("{0}-{1}", year.ToString().Substring(2), (year + 1).ToString().Substring(2));
            int t;
            IList<int> classIds = ClassBusiness.GetClassIdsForReport(schoolId, user, assessmentId);
            List<CpallsStudentModel> students = StudentBusiness.GetClassStudents(x =>
                x.SchoolRelations.Any(s => s.SchoolId == schoolId)
                && x.Classes.Any(c => c.ID == classId && c.IsDeleted == false && classIds.Contains(c.ID))
                && x.Status == EntityStatus.Active
                && (x.AssessmentLanguage == language || x.AssessmentLanguage == StudentAssessmentLanguage.Bilingual)
                && (studentId == 0 || x.ID == studentId)
                , user, "FirstName", "ASC", 0, int.MaxValue, out t);

            var headers = _cpallsContract.GetReportMeasureHeaders(assessmentId);

            var actualWaves = new Dictionary<Wave, IEnumerable<int>>();
            selectedWaves.ForEach(x =>
            {
                if (x.Value != null && x.Value.Any())
                    actualWaves.Add(x.Key, x.Value);
            });
            List<StudentRecordModel> records = _cpallsContract.GetReportStudentRecords(assessmentId, schoolYear,
                schoolId, CommonAgent.MinDate, DateTime.Now.AddDays(1), classIds,
                students.Select(s => s.ID).Distinct().ToList())
                                               ?? new List<StudentRecordModel>();
            var benchmarks = _adeBusiness.GetIEnumBenchmarks(assessmentId);
            var generator = new ParentReportPdfStudentGenerator(students, headers, records, actualWaves, benchmarks);
            generator.ShowTotalForMeasure = true;
            generator.ShowTotalScore = true;
            generator.PrintComment = printComments;
            generator.Generate();
            return generator.Reports;
        }


        #endregion

        /// <summary>
        /// Gets the student report.
        /// </summary>
        /// <param name="assessmentId">The assessment identifier.</param>
        /// <param name="user">The user.</param>
        /// <param name="year">The year.</param>
        /// <param name="schoolId">The school identifier.</param>
        /// <param name="classId">The class identifier.</param>
        /// <param name="studentId">0: entire class, other wise: the selected student only.</param>
        /// <param name="selectedWaves">The selected waves.</param>
        /// <returns></returns>
        public Dictionary<object, List<ReportRowModel>> GetStudentReport(int assessmentId, StudentAssessmentLanguage language, bool printComments, UserBaseEntity user, int year, int schoolId, int studentId, Dictionary<Wave, IEnumerable<int>> selectedWaves)
        {
            var schoolYear = string.Format("{0}-{1}", year.ToString().Substring(2), (year + 1).ToString().Substring(2));
            int t;
            IList<int> classIds = ClassBusiness.GetClassIdsForReport(schoolId, user, assessmentId);
            List<CpallsStudentModel> students = StudentBusiness.GetClassStudents(x =>
                x.SchoolRelations.Any(s => s.SchoolId == schoolId)
                && x.Classes.Any(c => classIds.Contains(c.ID) && c.IsDeleted == false)
                && x.Status == EntityStatus.Active
                && (x.AssessmentLanguage == language || x.AssessmentLanguage == StudentAssessmentLanguage.Bilingual)
                && (studentId == 0 || x.ID == studentId)
                , user, "FirstName", "ASC", 0, int.MaxValue, out t);

            var headers = _cpallsContract.GetReportMeasureHeaders(assessmentId);

            var actualWaves = new Dictionary<Wave, IEnumerable<int>>();
            selectedWaves.ForEach(x =>
            {
                if (x.Value != null && x.Value.Any())
                    actualWaves.Add(x.Key, x.Value);
            });
            List<StudentRecordModel> records = _cpallsContract.GetReportStudentRecords(assessmentId, schoolYear,
                schoolId, CommonAgent.MinDate, DateTime.Now.AddDays(1), classIds,
                students.Select(s => s.ID).Distinct().ToList())
                                               ?? new List<StudentRecordModel>();
            var benchmarks = _adeBusiness.GetIEnumBenchmarks(assessmentId);
            var generator = new AverageBySourcePdfStudentGenerator(students, headers, records, actualWaves, benchmarks);
            generator.ShowTotalForMeasure = true;
            generator.ShowTotalScore = true;
            generator.PrintComment = printComments;
            generator.Generate();
            return generator.Reports;
        }

    }
}