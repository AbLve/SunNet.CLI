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
using Sunnet.Cli.Business.Classes;
using Sunnet.Cli.Business.Students;
using Sunnet.Cli.Core.Practices.Entites;
using Sunnet.Cli.Business.Cpalls;
using Sunnet.Framework.Helpers;
using Sunnet.Framework.PDF;

namespace Sunnet.Cli.Business.Practices
{
    public partial class PracticeBusiness
    {

        #region Student Report
        public Dictionary<object, List<ReportRowModel>> GetStudentReport(int assessmentId,
            StudentAssessmentLanguage language, bool printComments, UserBaseEntity user, int year, int studentId,
            Dictionary<Wave, IEnumerable<int>> selectedWaves, int fromMonths, int toMonths)
        {
            var schoolYear = string.Format("{0}-{1}", year.ToString().Substring(2), (year + 1).ToString().Substring(2));
            List<CpallsStudentModel> students = GetPraceticeDemoStudents(x =>
                x.Status == EntityStatus.Active
                && (x.AssessmentLanguage == language || x.AssessmentLanguage == StudentAssessmentLanguage.Bilingual)
                && ((studentId == 0 && x.AssessmentId == assessmentId) || x.ID == studentId)
                && (x.StudentAgeYear * 12 + x.StudentAgeMonth) >= fromMonths
                && (x.StudentAgeYear * 12 + x.StudentAgeMonth) <= toMonths);
            var headers = _practiceContract.GetReportMeasureHeaders(assessmentId);
            var actualWaves = new Dictionary<Wave, IEnumerable<int>>();
            selectedWaves.ForEach(x =>
            {
                if (x.Value != null && x.Value.Any())
                    actualWaves.Add(x.Key, x.Value);
            });
            List<StudentRecordModel> records = _practiceContract.GetReportStudentRecords(assessmentId, schoolYear,
                CommonAgent.MinDate, DateTime.Now.AddDays(1), students.Select(e => e.ID), user.ID)
                                               ?? new List<StudentRecordModel>();
            var benchmarks = _adeBusiness.GetIEnumBenchmarks(assessmentId);
            var generator = new AverageBySourcePdfStudentGenerator(students, headers, records, actualWaves, benchmarks);
            generator.ShowTotalForMeasure = true;
            generator.ShowTotalScore = true;
            generator.PrintComment = printComments;
            generator.Generate();
            return generator.Reports;
        }
        public Dictionary<object, List<ReportRowModel>> GetStudentPercentileRankReport(int assessmentId, StudentAssessmentLanguage language, bool printComments,
            UserBaseEntity user, int year, int studentId, Dictionary<Wave, IEnumerable<int>> selectedWaves, int fromMonths, int toMonths)
        {
            var schoolYear = string.Format("{0}-{1}", year.ToString().Substring(2), (year + 1).ToString().Substring(2));
            List<CpallsStudentModel> students = GetPraceticeDemoStudents(x =>
                x.Status == EntityStatus.Active
                && (x.AssessmentLanguage == language || x.AssessmentLanguage == StudentAssessmentLanguage.Bilingual)
                && ((studentId == 0 && x.AssessmentId == assessmentId) || x.ID == studentId)
                && (x.StudentAgeYear * 12 + x.StudentAgeMonth) >= fromMonths
                && (x.StudentAgeYear * 12 + x.StudentAgeMonth) <= toMonths);
            var headers = _practiceContract.GetReportMeasureHeaders(assessmentId);
            var actualWaves = new Dictionary<Wave, IEnumerable<int>>();
            selectedWaves.ForEach(x =>
            {
                if (x.Value != null && x.Value.Any())
                    actualWaves.Add(x.Key, x.Value);
            });
            List<StudentRecordModel> records = _practiceContract.GetReportStudentRecords(assessmentId, schoolYear,
                CommonAgent.MinDate, DateTime.Now.AddDays(1),
                students.Select(s => s.ID).Distinct().ToList(), user.ID)
                                               ?? new List<StudentRecordModel>();
            var benchmarks = _adeBusiness.GetIEnumBenchmarks(assessmentId);
            var generator = new AverageBySourcePdfStudentPercentileRankGenerator(students, headers, records, actualWaves, benchmarks);
            generator.ShowTotalForMeasure = true;
            generator.ShowTotalScore = false;
            generator.PrintComment = printComments;
            generator.Generate();
            return generator.Reports;
        }

        #region For ParentReport
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
        public Dictionary<object, List<ReportRowModel>> GetStudentParentReport(int assessmentId, StudentAssessmentLanguage language, bool printComments, UserBaseEntity user, int year, int studentId, Dictionary<Wave, IEnumerable<int>> selectedWaves)
        {
            var schoolYear = string.Format("{0}-{1}", year.ToString().Substring(2), (year + 1).ToString().Substring(2));
            List<CpallsStudentModel> students = GetPraceticeDemoStudents(x =>
                x.Status == EntityStatus.Active
                && (x.AssessmentLanguage == language || x.AssessmentLanguage == StudentAssessmentLanguage.Bilingual)
                && ((studentId == 0 && x.AssessmentId == assessmentId) || x.ID == studentId));
            var headers = _practiceContract.GetReportMeasureHeaders(assessmentId);
            var actualWaves = new Dictionary<Wave, IEnumerable<int>>();
            selectedWaves.ForEach(x =>
            {
                if (x.Value != null && x.Value.Any())
                    actualWaves.Add(x.Key, x.Value);
            });
            List<StudentRecordModel> records = _practiceContract.GetReportStudentRecords(assessmentId, schoolYear,
                CommonAgent.MinDate, DateTime.Now.AddDays(1),
                students.Select(s => s.ID).Distinct().ToList(), user.ID)
                                               ?? new List<StudentRecordModel>();
            var benchmarks = _adeBusiness.GetIEnumBenchmarks(assessmentId);
            var generator = new ParentReportPdfStudentGenerator(students, headers, records, actualWaves, benchmarks);
            generator.ShowTotalForMeasure = true;
            generator.ShowTotalScore = true;
            generator.PrintComment = printComments;
            generator.Generate();
            return generator.Reports;
        }

        public byte[] GeneratePDFForParentReport(DemoStudentEntity student)
        {
            string emailPDFTemplateName = "ParentPinPagePDF_Template.xml";
            EmailTemplete template = XmlHelper.GetEmailTemp(emailPDFTemplateName);
            string pdfContent = "";
            pdfContent = template.Body.Replace("[firstName]", student.StudentName)
                .Replace("[lastName]", "")
                .Replace("[parentCode]", "")
                .Replace("[staticdomain]", SFConfig.StaticDomain);
            return GetPdfByte(pdfContent);
        }
        public byte[] GenerateCoverReport(DemoStudentEntity student)
        {
            string emailPDFTemplateName = "ParentInvitationPDF_CoverPage.xml";
            EmailTemplete template = XmlHelper.GetEmailTemp(emailPDFTemplateName);
            string pdfContent = "";
            pdfContent = template.Body.Replace("[firstName]", student.StudentName)
                .Replace("[lastName]", "")
                .Replace("[parentCode]", "")
                .Replace("[staticdomain]", SFConfig.StaticDomain);
            return GetPdfByte(pdfContent);
        }
        private byte[] GetPdfByte(string contentHtml)
        {
            int teacherCount = 0;
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                PdfProvider pdfProvider = new PdfProvider();
                byte[] pdfBytes = pdfProvider.GetPdfBytes(contentHtml);
                return pdfBytes;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        #endregion
        #endregion

        #region Class Reports
        public Dictionary<object, List<ReportRowModel>> GetStudentSatisfactoryReport(int assessmentId, StudentAssessmentLanguage language,
            UserBaseEntity user, int year,
            Dictionary<Wave, IEnumerable<int>> selectedWaves, DateTime startDate, DateTime endDate,
            IEnumerable<BenchmarkModel> benchmarks, bool mergeSameLabel, int fromMonths, int toMonths)
        {
            var schoolYear = string.Format("{0}-{1}", year.ToString().Substring(2), (year + 1).ToString().Substring(2));

            List<CpallsStudentModel> students = GetPraceticeDemoStudents(x =>
                x.Status == EntityStatus.Active
                && (x.AssessmentLanguage == language || x.AssessmentLanguage == StudentAssessmentLanguage.Bilingual)
                && x.AssessmentId == assessmentId
                && (x.StudentAgeYear * 12 + x.StudentAgeMonth) >= fromMonths
                && (x.StudentAgeYear * 12 + x.StudentAgeMonth) <= toMonths) ?? new List<CpallsStudentModel>();
            var headers = _practiceContract.GetReportMeasureHeaders(assessmentId);

            var records = _practiceContract.GetReportStudentRecords(assessmentId, schoolYear, startDate, endDate,
                students.Select(s => s.ID).Distinct().ToList(), user.ID)
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

        public Dictionary<object, List<ReportRowModel>> GetClassSummaryReport(int assessmentId, StudentAssessmentLanguage language, int year,
            Dictionary<Wave, IEnumerable<int>> selectedWaves, DateTime startDate, DateTime endDate, int fromMonths, int toMonths, UserBaseEntity user)
        {
            var schoolYear = string.Format("{0}-{1}", year.ToString().Substring(2), (year + 1).ToString().Substring(2));
            List<CpallsStudentModel> students = GetPraceticeDemoStudents(x =>
                x.Status == EntityStatus.Active
                && (x.AssessmentLanguage == language || x.AssessmentLanguage == StudentAssessmentLanguage.Bilingual)
                && x.AssessmentId == assessmentId
                && (x.StudentAgeYear * 12 + x.StudentAgeMonth) >= fromMonths
                && (x.StudentAgeYear * 12 + x.StudentAgeMonth) <= toMonths) ?? new List<CpallsStudentModel>();
            var headers = _practiceContract.GetReportMeasureHeaders(assessmentId);
            var measureIds = headers.Select(x => x.ID).Distinct().ToList();
            measureIds.AddRange(headers.Select(x => x.ParentId).Distinct().ToList());

            var records = _practiceContract.GetReportStudentRecords(assessmentId, schoolYear, startDate, endDate, students.Select(s => s.ID).Distinct().ToList(), user.ID)
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
        public Dictionary<object, List<ReportRowModel>> GetClassPercentileRankReport(int assessmentId, StudentAssessmentLanguage language, UserBaseEntity user, int year,
            Dictionary<Wave, IEnumerable<int>> selectedWaves, DateTime startDate, DateTime endDate, int fromMonths, int toMonths)
        {
            var schoolYear = string.Format("{0}-{1}", year.ToString().Substring(2), (year + 1).ToString().Substring(2));
            List<CpallsStudentModel> students = GetPraceticeDemoStudents(x =>
                x.Status == EntityStatus.Active
                && (x.AssessmentLanguage == language || x.AssessmentLanguage == StudentAssessmentLanguage.Bilingual)
                && x.AssessmentId == assessmentId
                && (x.StudentAgeYear * 12 + x.StudentAgeMonth) >= fromMonths
                && (x.StudentAgeYear * 12 + x.StudentAgeMonth) <= toMonths) ?? new List<CpallsStudentModel>();
            var headers = _practiceContract.GetReportMeasureHeaders(assessmentId);
            var measureIds = headers.Select(x => x.ID).Distinct().ToList();
            measureIds.AddRange(headers.Select(x => x.ParentId).Distinct().ToList());

            var records = _practiceContract.GetReportStudentRecords(assessmentId, schoolYear, startDate, endDate, students.Select(s => s.ID).Distinct().ToList(), user.ID)
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
        #endregion

        public List<CpallsStudentModel> GetPraceticeDemoStudents(Expression<Func<DemoStudentEntity, bool>> condition)
        {
            var query = _practiceContract.Students.AsExpandable().Where(condition)
                .Select(SelectorStudentEntityToCpallsModel);
            return query.ToList();
        }

        private static Expression<Func<DemoStudentEntity, CpallsStudentModel>> SelectorStudentEntityToCpallsModel
        {
            get
            {
                return r => new CpallsStudentModel()
                {
                    ID = r.ID,
                    FirstName = r.StudentName,
                    LastName = "",
                    BirthDate = r.StudentDob
                };
            }
        }
    }
}