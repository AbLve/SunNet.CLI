using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Sunnet.Cli.Business.Cpalls.Growth;
using Sunnet.Cli.Business.Cpalls.Models;
using Sunnet.Cli.Business.Cpalls.Models.Report;
using LinqKit;
using Sunnet.Cli.Business.Ade.Models;
using Sunnet.Cli.Core.Ade;
using Sunnet.Cli.Core.Cpalls;
using Sunnet.Cli.Core.Cpalls.Models.Report;
using Sunnet.Cli.Core.Schools.Entities;
using Sunnet.Cli.Core.Users.Entities;
using Sunnet.Cli.Business.Common;
using Sunnet.Cli.Core.Common.Enums;


namespace Sunnet.Cli.Business.Cpalls
{
    public partial class CpallsBusiness
    {
        // Community Growth
        public GrowthReportModel GetCommunityGrowthPdf(int assessmentId, StudentAssessmentLanguage language, GrowthReportType type,
            UserBaseEntity user, int year, Dictionary<Wave, List<int>> waves, int districtId, DateTime startDate, DateTime endDate,
            DateTime dobStartDate, DateTime dobEndDate, IEnumerable<BenchmarkModel> benchmarks)
        {
            var schoolYear = year.ToSchoolYearString();
            int t;
            var communities = CommunityBusiness.GetCommunities(x => x.ID == districtId);
            if (communities == null)
                return null;
            var headers = _cpallsContract.GetReportMeasureHeaders(assessmentId);
            var actualWaves = new Dictionary<Wave, IEnumerable<int>>();
            waves.ForEach(x =>
            {
                if (x.Value != null && x.Value.Any())
                    actualWaves.Add(x.Key, x.Value);
            });

            var report = new GrowthReportModel()
            {
                Title = "Comm " + (type == GrowthReportType.Average ? "Average" : "Benchmark") + " Growth Report",
                Class = "All",
                Classes = 0,
                Community = communities.First().Name,
                Language = language,
                School = "All",
                Schools = 0,
                Teacher = "All",
                Year = year,
                Type = type
            };
            var waveDates = _cpallsContract.GetWaveFinishedDate(QueryLevel.Community, districtId);
            report.Wave1 = waves.ContainsKey(Wave.BOY) && waveDates.Any(x => x.Wave == Wave.BOY)
                ? waveDates.Find(x => x.Wave == Wave.BOY).FinishedOn
                : DateTime.MinValue;
            report.Wave2 = waves.ContainsKey(Wave.MOY) && waveDates.Any(x => x.Wave == Wave.MOY)
                ? waveDates.Find(x => x.Wave == Wave.MOY).FinishedOn
                : DateTime.MinValue;
            report.Wave3 = waves.ContainsKey(Wave.EOY) && waveDates.Any(x => x.Wave == Wave.EOY)
                ? waveDates.Find(x => x.Wave == Wave.EOY).FinishedOn
                : DateTime.MinValue;
            var schoolIds = SchoolBusiness.GetAssignedSchoolIdsWithoutDemo(districtId);

            IList<int> classIds = ClassBusiness.GetClassIdsForReport(districtId, assessmentId);
            var records = _cpallsContract.GetReportStudentRecords(assessmentId, schoolYear, schoolIds, startDate, endDate,
            dobStartDate, dobEndDate, classIds);
            if (records == null)
                return null;
            records.ForEach(stuScore =>
            {
                stuScore.ParentId = headers.Where(x => x.ID == stuScore.MeasureId).Select(x => x.ParentId).FirstOrDefault();
            });
            var studentIds = records.Select(x => x.StudentId).Distinct().ToList();
            var actualSchoolIds = new List<int>();
            records.ForEach(record =>
            {
                actualSchoolIds = actualSchoolIds.Union(record.SchoolIds).Distinct().ToList();
            });
            schoolIds = schoolIds.Intersect(actualSchoolIds).Distinct().ToList();

            report.Schools = schoolIds.Count();
            var classes = ClassBusiness.GetClassList(x => schoolIds.Contains(x.SchoolId) && classIds.Contains(x.ID),
                user, "ID", "Asc", 0, int.MaxValue, out t).Where(c => c.StudentIds.Intersect(studentIds).Any());
            report.Classes = classes.Count(c => c.StudentIds.Any(studentIds.Contains));
            try
            {
                if (type == GrowthReportType.Average)
                {
                    var generator = new AverageBySourcePdfCommunityGenerator(communities, headers, records, actualWaves, benchmarks);
                    generator.ShowTotalForMeasure = true;
                    generator.ShowTotalScore = true;
                    generator.Generate();
                    report.Report = generator.Reports[communities.First().ID];
                }
                else if (type == GrowthReportType.MeetingBenchmark)
                {
                    var generator = new SatisfactoryBySourcePdfCommunityGenerator(communities, headers, records, actualWaves, benchmarks);
                    generator.ShowTotalForMeasure = true;
                    generator.Generate();
                    report.Report = generator.Reports[communities.First().ID];
                }
            }
            catch (Exception ex)
            {
                _logger.Debug(ex);
                return null;
            }
            finally
            {
                GC.Collect();
            }
            return report;
        }

        // School(s) Growth
        public List<GrowthReportModel> GetSchoolGrowthPdf(int assessmentId, StudentAssessmentLanguage language, UserBaseEntity user, int year,
            GrowthReportType type, Dictionary<Wave, List<int>> waves, int schoolId, int districtId, bool allSchools,
            DateTime startDate, DateTime endDate,
            DateTime dobStartDate, DateTime dobEndDate, IEnumerable<BenchmarkModel> benchmarks)
        {
            var schoolYear = year.ToSchoolYearString();
            int t;
            Expression<Func<SchoolEntity, bool>> condition = s => s.Status == SchoolStatus.Active;
            condition = allSchools
                ? condition.And(s => s.CommunitySchoolRelations.Any(c => c.CommunityId == districtId))
                : condition.And(s => s.ID == schoolId);
            var schools = SchoolBusiness.GetSchoolList(condition, user, "Name", "ASC", 0, int.MaxValue, out t);
            var schoolIds = schools.Select(x => x.ID).ToList();
            var headers = _cpallsContract.GetReportMeasureHeaders(assessmentId);

            List<GrowthReportModel> reports = null;

            var actualWaves = new Dictionary<Wave, IEnumerable<int>>();
            waves.ForEach(x =>
            {
                if (x.Value != null && x.Value.Any())
                    actualWaves.Add(x.Key, x.Value);
            });
            IList<int> classIds = ClassBusiness.GetClassIdsForReport(districtId, assessmentId);
            var records = _cpallsContract.GetReportStudentRecords(assessmentId, schoolYear, schoolIds, startDate, endDate,
            dobStartDate, dobEndDate, classIds);
            if (records == null)
                return null;
            records.ForEach(stuScore =>
            {
                stuScore.ParentId = headers.Where(x => x.ID == stuScore.MeasureId).Select(x => x.ParentId).FirstOrDefault();
            });
            schoolIds = records.Select(x => x.SchoolId).Distinct().ToList();
            var classes = ClassBusiness.GetClassList(x => schoolIds.Contains(x.SchoolId) && classIds.Contains(x.ID),
                user, "ID", "Asc", 0, int.MaxValue, out t);

            try
            {
                var waveDates = _cpallsContract.GetWaveFinishedDate(QueryLevel.School, districtId); ;
                IReportGenerator generator = null;
                if (type == GrowthReportType.Average)
                {
                    generator = new AverageBySourcePdfSchoolGenerator(schools, headers, records, actualWaves, benchmarks);
                    var g1 = generator as AverageGenerator<CpallsSchoolModel, ReportMeasureHeaderModel, StudentRecordModel>;
                    g1.ShowTotalForMeasure = true;
                    g1.ShowTotalScore = true;
                }
                else
                {
                    generator = new SatisfactoryBySourcePdfSchoolGenerator(schools, headers, records, actualWaves, benchmarks);
                    var g1 = generator as SatisfactoryGenerator<CpallsSchoolModel, ReportMeasureHeaderModel, StudentRecordModel>;
                    g1.ShowTotalForMeasure = true;
                }

                generator.Generate();
                reports = schools.Select(sch => new GrowthReportModel
                {
                    Title = "School " + (type == GrowthReportType.Average ? "Average" : "Benchmark") + " Growth Report",
                    Class = "All",
                    Classes = classes.Count(c => c.StudentIds.Any(sId => records.Any(r => r.SchoolId == sch.ID && r.StudentId == sId))),
                    Community = sch.CommunitiesText,
                    Language = language,
                    School = sch.Name,
                    Teacher = "All",
                    Year = year,
                    Type = type,
                    Wave1 = waves.ContainsKey(Wave.BOY) && waveDates.Any(x => x.Wave == Wave.BOY
                        && x.SchoolId == sch.ID
                        )
                    ? waveDates.Find(x => x.Wave == Wave.BOY
                        && x.SchoolId == sch.ID
                        ).FinishedOn
                    : DateTime.MinValue,
                    Wave2 = waves.ContainsKey(Wave.MOY) && waveDates.Any(x => x.Wave == Wave.MOY
                        && x.SchoolId == sch.ID
                        )
                    ? waveDates.Find(x => x.Wave == Wave.MOY
                        && x.SchoolId == sch.ID
                        ).FinishedOn
                    : DateTime.MinValue,
                    Wave3 = waves.ContainsKey(Wave.EOY) && waveDates.Any(x => x.Wave == Wave.EOY
                        && x.SchoolId == sch.ID
                        )
                    ? waveDates.Find(x => x.Wave == Wave.EOY
                        && x.SchoolId == sch.ID
                        ).FinishedOn
                    : DateTime.MinValue,
                    Report = generator.Reports.ContainsKey(sch.ID) ? generator.Reports[sch.ID] : new List<ReportRowModel>()
                }).ToList();
            }
            catch (Exception ex)
            {
                _logger.Debug(ex);
                return null;
            }
            finally
            {
                GC.Collect();
            }
            return reports;
        }

        // Class(es) Growth
        public List<GrowthReportModel> GetClassGrowthPdf(int assessmentId, StudentAssessmentLanguage language, UserBaseEntity user, int year, GrowthReportType type, Dictionary<Wave, List<int>> waves,
             int classId, int schoolId, bool allClasses, DateTime startDate, DateTime endDate, IEnumerable<BenchmarkModel> benchmarks, DateTime dobStart, DateTime dobEnd)
        {
            var schoolYear = year.ToSchoolYearString();
            int t;

            List<CpallsClassModel> classes = null;
            IList<int> classIds = ClassBusiness.GetClassIdsForReport(schoolId, user, assessmentId);
            if (allClasses)
            {
                classes = ClassBusiness.GetClassList(x => classIds.Contains(x.ID) && x.SchoolId == schoolId && x.Status == EntityStatus.Active,
                    user, dobStart, dobEnd, language);
            }
            else
            {
                classes = ClassBusiness.GetClassList(x => classIds.Contains(x.ID) && x.ID == classId && x.Status == EntityStatus.Active,
                user, dobStart, dobEnd, language);
            }
            var studentIds = new List<int>();
            classes.ForEach(x => studentIds.AddRange(x.StudentIds));
            var headers = _cpallsContract.GetReportMeasureHeaders(assessmentId);
            var school = SchoolBusiness.GetCpallsSchoolModel(schoolId);
            List<GrowthReportModel> reports = null;

            var actualWaves = new Dictionary<Wave, IEnumerable<int>>();
            waves.ForEach(x =>
            {
                if (x.Value != null && x.Value.Any())
                    actualWaves.Add(x.Key, x.Value);
            });

            var records = _cpallsContract.GetReportStudentRecords(assessmentId, schoolYear, schoolId, startDate, endDate, classIds, studentIds);
            if (records == null)
                return null;
            records.ForEach(stuScore =>
            {
                stuScore.ParentId = headers.Where(x => x.ID == stuScore.MeasureId).Select(x => x.ParentId).FirstOrDefault();
            });
            try
            {
                var waveDates = _cpallsContract.GetWaveFinishedDate(QueryLevel.Class, schoolId);
                IReportGenerator generator = null;
                if (type == GrowthReportType.Average)
                {
                    generator = new AverageBySourcePdfClassGenerator(classes, headers, records, actualWaves, benchmarks);
                    var g1 = generator as AverageGenerator<CpallsClassModel, ReportMeasureHeaderModel, StudentRecordModel>;
                    g1.ShowTotalForMeasure = true;
                    g1.ShowTotalScore = true;
                }
                else
                {

                    generator = new SatisfactoryBySourcePdfClassGenerator(classes, headers, records, actualWaves, benchmarks);
                    var g1 = generator as SatisfactoryGenerator<CpallsClassModel, ReportMeasureHeaderModel, StudentRecordModel>;
                    g1.ShowTotalForMeasure = true;
                }

                generator.Generate();
                reports = classes.Select(class1 => new GrowthReportModel
                {
                    Title = "Class " + (type == GrowthReportType.Average ? "Average" : "Benchmark") + " Growth Report",
                    Community = school.CommunitiesText,
                    Class = class1.Name,
                    Language = language,
                    School = school.Name,
                    Teacher = "All",
                    Year = year,
                    Type = type,
                    //下面wave1，wave2，wave3的结果是空，原因是waveDates用的是QueryLevel.Class查询的，没有为studentId赋值，是个bug
                    Wave1 = waves.ContainsKey(Wave.BOY) && waveDates.Any(x => x.Wave == Wave.BOY && class1.StudentIds.Contains(x.StudentId))
                    ? waveDates.Find(x => x.Wave == Wave.BOY && class1.StudentIds.Contains(x.StudentId)).FinishedOn
                    : DateTime.MinValue,
                    Wave2 = waves.ContainsKey(Wave.MOY) && waveDates.Any(x => x.Wave == Wave.MOY && class1.StudentIds.Contains(x.StudentId))
                    ? waveDates.Find(x => x.Wave == Wave.MOY && class1.StudentIds.Contains(x.StudentId)).FinishedOn
                    : DateTime.MinValue,
                    Wave3 = waves.ContainsKey(Wave.EOY) && waveDates.Any(x => x.Wave == Wave.EOY && class1.StudentIds.Contains(x.StudentId))
                    ? waveDates.Find(x => x.Wave == Wave.EOY && class1.StudentIds.Contains(x.StudentId)).FinishedOn
                    : DateTime.MinValue,
                    Report = generator.Reports.ContainsKey(class1.ID) ? generator.Reports[class1.ID] : new List<ReportRowModel>()
                }).ToList();
            }
            catch (Exception ex)
            {
                _logger.Debug(ex);
                return null;
            }
            finally
            {
                GC.Collect();
            }
            return reports;
        }

    }
}