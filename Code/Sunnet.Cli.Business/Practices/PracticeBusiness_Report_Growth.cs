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
using Sunnet.Cli.Business.Cpalls;
using Sunnet.Cli.Business.Practices.Models;
using Sunnet.Cli.Core.Common.Enums;
using GrowthReportModel = Sunnet.Cli.Business.Cpalls.Models.GrowthReportModel;


namespace Sunnet.Cli.Business.Practices
{
    public partial class PracticeBusiness
    {
        // Class(es) Growth
        public List<GrowthReportModel> GetClassGrowthPdf(int assessmentId, StudentAssessmentLanguage language, UserBaseEntity user, int year, GrowthReportType type, Dictionary<Wave, List<int>> waves,
             DateTime startDate, DateTime endDate, IEnumerable<BenchmarkModel> benchmarks, int fromMonths, int toMonths)
        {
            var schoolYear = year.ToSchoolYearString();
            List<CpallsClassModel> classes = new List<CpallsClassModel>();
            //var studentIds = new List<int>();
            //studentIds = GetStudentIdByAssessmentId(assessmentId);
            var studentIds = GetStudentIdByCondition(x =>
                 x.AssessmentId == assessmentId
                 && (x.StudentAgeYear * 12 + x.StudentAgeMonth) >= fromMonths
                 && (x.StudentAgeYear * 12 + x.StudentAgeMonth) <= toMonths) ?? new List<int>();
            var headers = _practiceContract.GetReportMeasureHeaders(assessmentId);
            List<GrowthReportModel> reports = null;

            var assessment = _adeBusiness.GetAssessment(assessmentId);
            List<PracticeAssessmentModel> assessmentList = new List<PracticeAssessmentModel>();
            assessmentList.Add(new PracticeAssessmentModel()
            {
                ID = assessmentId,
                Name = assessment.Name
            });
            var actualWaves = new Dictionary<Wave, IEnumerable<int>>();
            waves.ForEach(x =>
            {
                if (x.Value != null && x.Value.Any())
                    actualWaves.Add(x.Key, x.Value);
            });

            var records = _practiceContract.GetReportStudentRecords(assessmentId, schoolYear, startDate, endDate,
                studentIds, user.ID);
            if (records == null)
                return null;
            records.ForEach(stuScore =>
            {
                stuScore.ParentId = headers.Where(x => x.ID == stuScore.MeasureId).Select(x => x.ParentId).FirstOrDefault();
            });
            try
            {
                var waveDates = _practiceContract.GetWaveFinishedDate(assessmentId, user.ID);
                IReportGenerator generator = null;
                if (type == GrowthReportType.Average)
                {
                    generator = new AverageBySourcePdfPracticeClassGenerator(assessmentList, headers, records, actualWaves, benchmarks);
                    var g1 = generator as AverageGenerator<PracticeAssessmentModel, ReportMeasureHeaderModel, StudentRecordModel>;
                    g1.ShowTotalForMeasure = true;
                    g1.ShowTotalScore = true;
                }
                else
                {
                    generator = new SatisfactoryBySourcePdfPracticeClassGenerator(assessmentList, headers, records, actualWaves, benchmarks);
                    var g1 = generator as SatisfactoryGenerator<PracticeAssessmentModel, ReportMeasureHeaderModel, StudentRecordModel>;
                    g1.ShowTotalForMeasure = true;
                }

                generator.Generate();
                reports = assessmentList.Select(assesement => new GrowthReportModel
                {
                    Title = "Class " + (type == GrowthReportType.Average ? "Average" : "Benchmark") + " Growth Report",
                    Class = assesement.Name,
                    Language = language,
                    Teacher = "All",
                    Year = year,
                    Type = type,
                    Wave1 = waves.ContainsKey(Wave.BOY) && waveDates.Any(x => x.Wave == Wave.BOY && studentIds.Contains(x.StudentId))
                    ? waveDates.Find(x => x.Wave == Wave.BOY && studentIds.Contains(x.StudentId)).FinishedOn
                    : DateTime.MinValue,
                    Wave2 = waves.ContainsKey(Wave.MOY) && waveDates.Any(x => x.Wave == Wave.MOY && studentIds.Contains(x.StudentId))
                    ? waveDates.Find(x => x.Wave == Wave.MOY && studentIds.Contains(x.StudentId)).FinishedOn
                    : DateTime.MinValue,
                    Wave3 = waves.ContainsKey(Wave.EOY) && waveDates.Any(x => x.Wave == Wave.EOY && studentIds.Contains(x.StudentId))
                    ? waveDates.Find(x => x.Wave == Wave.EOY && studentIds.Contains(x.StudentId)).FinishedOn
                    : DateTime.MinValue,
                    Report = generator.Reports.ContainsKey(assesement.ID) ? generator.Reports[assesement.ID] : new List<ReportRowModel>()
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