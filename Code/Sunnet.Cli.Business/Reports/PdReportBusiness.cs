using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Sunnet.Cli.Core.Reports.Models;
using Sunnet.Framework.Excel;

namespace Sunnet.Cli.Business.Reports
{
    public partial class ReportBusiness
    {
        #region PD Report

        public List<PDReportModel> GetPdCompletionCourse(List<int> communityIds, List<int> schoolIds, string teacher, int status)
        {
            List<PDReportModel> list = _reportService.PDCompletionCourseReport(communityIds, schoolIds, teacher, status);
            HttpRuntime.Cache.Insert("PdCompletionCourse", list);
            return (List<PDReportModel>)HttpRuntime.Cache["PdCompletionCourse"];
        }

        public List<SummaryReportModel> GetSummaryReport(List<int> communityIds, List<int> schoolIds, string teacher, int status)
        {
            if (HttpRuntime.Cache["PdCompletionCourse"] == null)
            {
                List<PDReportModel> list = _reportService.PDCompletionCourseReport(communityIds, schoolIds, teacher,
                    status);
                HttpRuntime.Cache.Insert("PdCompletionCourse", list);
            }
            List<PDReportModel> listPDReport = (List<PDReportModel>)HttpRuntime.Cache["PdCompletionCourse"];
            List<SummaryReportModel> listSummaryReport1 =
                listPDReport.GroupBy(e => new { e.CommunityDistrict, e.CircleCourse, e.CircleCourseId })
                    .Select(e => new SummaryReportModel()
                    {
                        CommunityDistrict = e.Key.CommunityDistrict,
                        CircleCourse = e.Key.CircleCourse,
                        CountofTeachersNotStarted = e.Where(r => r.Status == "Not Started").Select(r => r.TeacherID).Distinct().Count(),
                        CountofTeachersinProgress = e.Where(r => r.Status == "In Progress").Select(r => r.TeacherID).Distinct().Count(),
                        CountofTeachersComplete = e.Where(r => r.Status == "Complete").Select(r => r.TeacherID).Distinct().Count(),
                        TotalTeachers = e.Select(r => r.TeacherID).Distinct().Count(),
                        CountofPosts = e.Sum(r => r.CountofPosts),
                        CountofCourseViewed = e.Sum(r => r.CourseViewed)
                    }).OrderBy(e => e.CircleCourse).ToList();
            return listSummaryReport1;
        }

        public void GetPdReport(out string path, List<int> communityIds, List<int> schoolIds, string teacher, int status)
        {
            List<PDReportModel> list = _reportService.PDCompletionCourseReport(communityIds, schoolIds, teacher, status);

            path = CreateReportName("PDReport");

            using (SpreadsheetDocument myWorkbook =
                SpreadsheetDocument.Create(path,
                    SpreadsheetDocumentType.Workbook))
            {
                WorkbookPart workbookPart = myWorkbook.AddWorkbookPart();
                workbookPart.Workbook = new Workbook();
                var worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                // Create Styles and Insert into Workbook
                var stylesPart = myWorkbook.WorkbookPart.AddNewPart<WorkbookStylesPart>();
                Stylesheet styles = new MediaCustomStylesheet();
                styles.Save(stylesPart);
                string relId = workbookPart.GetIdOfPart(worksheetPart);
                var workbook = new Workbook();
                var fileVersion =
                    new FileVersion
                    {
                        ApplicationName = "Microsoft Office Excel"
                    };

                List<string> headerNames = new List<string>
                {
                    "Community/District",
                    "School Name",
                    "Teacher First Name",
                    "Teacher Last Name",
                    "Teacher ID",
                    "Teacher Email",
                    "Online Course",
                    "Start Date",
                    "Status",
                    "Time Spent in Course(hrs:mins)",
                    "Count of Posts",
                    "Course Viewed"
                };
                int numRows = list.Count;
                int numCols = headerNames.Count;
                var columns = new Columns();
                for (int col = 0; col < numCols; col++)
                {
                    int width = headerNames[col].Length + 5;
                    Column c = new CustomColumn((UInt32)col + 1,
                        (UInt32)numCols + 1, width);
                    columns.Append(c);
                }

                #region 填充PD Report数据

                SheetData sheetData = new SheetData();
                int index = 1;
                var row = new Row { RowIndex = (uint)index };
                row.Append(new MediaBICell("0", "Online Courses Report", index));
                sheetData.Append(row);
                index = index + 1;
                row = new Row { RowIndex = (uint)index };
                row.Append(new TextCell("0", DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss"), index));
                sheetData.Append(row);

                index = index + 2;
                row = new Row { RowIndex = (uint)index };
                row.Append(new PDTextCell("0", "PD Completion and Time in Course Report", index));
                sheetData.Append(row);

                index = index + 2;
                //Get a list of A to Z
                var az = new List<Char>(Enumerable.Range('A', 'Z' -
                                                              'A' + 1).Select(i => (Char)i).ToArray());
                //A to E number of columns 
                List<Char> headers = az.GetRange(0, headerNames.Count);
                Row header = new Row { RowIndex = (uint)index };
                for (int col = 0; col < headerNames.Count; col++)
                {
                    HeaderCell cell = new HeaderCell(headers[col].ToString(), headerNames[col], index);
                    header.Append(cell);
                }
                sheetData.AppendChild(header);
                for (int i = 0; i < numRows; i++)
                {
                    PDReportModel model = list[i];
                    index++;
                    row = new Row { RowIndex = (uint)index };
                    TextCell cell1 = new TextCell(headers[0].ToString(), model.CommunityDistrict, index);
                    row.Append(cell1);

                    TextCell cell2 = new TextCell(headers[1].ToString(), model.SchoolName, index);
                    row.Append(cell2);

                    TextCell cell3 = new TextCell(headers[2].ToString(), model.TeacherFirstName, index);
                    row.Append(cell3);

                    TextCell cell4 = new TextCell(headers[3].ToString(), model.TeacherLastName, index);
                    row.Append(cell4);

                    TextCell cell5 = new TextCell(headers[4].ToString(), model.TeacherID, index);
                    row.Append(cell5);

                    TextCell cell6 = new TextCell(headers[5].ToString(), model.TeacherEmail, index);
                    row.Append(cell6);

                    TextCell cell7 = new TextCell(headers[6].ToString(), model.CircleCourse, index);
                    row.Append(cell7);

                    TextCell cell8 = new TextCell(headers[7].ToString(),
                        model.StartDate == null ? "" : model.StartDate.Value.ToString("MM/dd/yyyy"), index);
                    row.Append(cell8);

                    TextCell cell9 = new TextCell(headers[8].ToString(), model.Status, index);
                    row.Append(cell9);

                    TextCell cell10 = new TextCell(headers[9].ToString(), model.TimeSpentInCourse, index);
                    row.Append(cell10);

                    ReportNumberCenterCell cell11 = new ReportNumberCenterCell(headers[10].ToString(), model.CountofPosts.ToString(), index);
                    row.Append(cell11);

                    ReportNumberCenterCell cell12 = new ReportNumberCenterCell(headers[11].ToString(), model.CourseViewed.ToString(), index);
                    row.Append(cell12);
                    sheetData.Append(row);

                    if (list.Count == i + 1 || list[i].TeacherEmail != list[i + 1].TeacherEmail)
                    {
                        PDReportModel model_1 = list[i];
                        index++;
                        row = new Row { RowIndex = (uint)index };
                        TextCell cell1_1 = new TextCell(headers[0].ToString(), model_1.CommunityDistrict, index);
                        row.Append(cell1_1);

                        TextCell cell2_1 = new TextCell(headers[1].ToString(), model_1.SchoolName, index);
                        row.Append(cell2_1);

                        TextCell cell3_1 = new TextCell(headers[2].ToString(), model_1.TeacherFirstName, index);
                        row.Append(cell3_1);

                        TextCell cell4_1 = new TextCell(headers[3].ToString(), model_1.TeacherLastName, index);
                        row.Append(cell4_1);

                        TextCell cell5_1 = new TextCell(headers[4].ToString(), model_1.TeacherID, index);
                        row.Append(cell5_1);

                        TextCell cell6_1 = new TextCell(headers[5].ToString(), model_1.TeacherEmail, index);
                        row.Append(cell6_1);

                        HeaderCell cell7_1 = new HeaderCell(headers[6].ToString(), "Total", index);
                        row.Append(cell7_1);

                        TextCell cell8_1 = new TextCell(headers[7].ToString(), "", index);
                        row.Append(cell8_1);

                        TextCell cell9_1 = new TextCell(headers[8].ToString(), "", index);
                        row.Append(cell9_1);

                        List<PDReportModel> timeSpentList =
                            list.Where(e =>
                                e.CommunityDistrict == model_1.CommunityDistrict &&
                                e.SchoolName == model_1.SchoolName &&
                                model_1.TeacherEmail == e.TeacherEmail).ToList();
                        TimeSpan ts = new TimeSpan();
                        foreach (var pdReport in timeSpentList)
                        {
                            if (pdReport.TimeSpentInCourse != "")
                                ts += DateTime.Parse(pdReport.TimeSpentInCourse) - DateTime.Now.Date;
                        }
                        TextCell cell10_1 = new TextCell(headers[9].ToString(), ts.ToString(), index);
                        row.Append(cell10_1);

                        ReportNumberCenterCell cell11_1 = new ReportNumberCenterCell(headers[10].ToString(),
                            list.Where(e => e.CommunityDistrict == model_1.CommunityDistrict
                                            && e.SchoolName == model_1.SchoolName &&
                                            e.TeacherEmail == model_1.TeacherEmail)
                                .Sum(e => e.CountofPosts)
                                .ToString(), index);
                        row.Append(cell11_1);

                        ReportNumberCenterCell cell12_1 = new ReportNumberCenterCell(headers[11].ToString(),
                            list.Where(e => e.CommunityDistrict == model_1.CommunityDistrict
                                            && e.SchoolName == model_1.SchoolName &&
                                            e.TeacherEmail == model_1.TeacherEmail)
                                .Sum(e => e.CourseViewed)
                                .ToString(), index);
                        row.Append(cell12_1);

                        sheetData.Append(row);
                    }
                }

                worksheetPart.Worksheet = new Worksheet();
                worksheetPart.Worksheet.Append(columns);
                worksheetPart.Worksheet.Append(sheetData);
                #endregion

                #region Create Summary Report
                var worksheetPart1 = workbookPart.AddNewPart<WorksheetPart>();
                string relId1 = workbookPart.GetIdOfPart(worksheetPart1);
                #endregion
                List<string> headerNamesSummaryReport = new List<string>
                {
                    "Community/District",
                    "CIRCLE Course",
                    "Count of Teachers Complete",
                    "Count of Teachers in Process",
                    "Count of Teachers Not Started",
                    "Total Teachers",
                    "Count of Posts",
                    "Count of Course Viewed"
                };
                var columns1 = new Columns();
                for (int col = 0; col < headerNamesSummaryReport.Count; col++)
                {
                    int width = headerNamesSummaryReport[col].Length + 5;
                    Column c = new CustomColumn((UInt32)col + 1,
                        (UInt32)headerNamesSummaryReport.Count + 1, width);
                    columns1.Append(c);
                }

                SheetData sheetData1 = new SheetData();
                int index1 = 1;
                row = new Row { RowIndex = (uint)index1 };
                row.Append(new MediaBICell("0", "Summary Reports", index1));
                sheetData1.Append(row);

                index1 = index1 + 1;
                row = new Row { RowIndex = (uint)index1 };
                row.Append(new TextCell("0", DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss"), index1));
                sheetData1.Append(row);

                index1 = index1 + 2;

                #region 填充Summary Reports数据

                List<Char> headers1 = az.GetRange(0, headerNamesSummaryReport.Count);
                Row header1 = new Row { RowIndex = (uint)index1 };
                for (int col = 0; col < headerNamesSummaryReport.Count; col++)
                {
                    HeaderCell cell = new HeaderCell(headers1[col].ToString(), headerNamesSummaryReport[col], index1);
                    header1.Append(cell);
                }
                sheetData1.Append(header1);

                List<SummaryReportModel> listSummaryReport =
                list.GroupBy(e => new { e.CommunityDistrict, e.CircleCourse, e.CircleCourseId })
                    .Select(e => new SummaryReportModel()
                    {
                        CommunityDistrict = e.Key.CommunityDistrict,
                        CircleCourse = e.Key.CircleCourse,
                        CountofTeachersNotStarted = e.Where(r => r.Status == "Not Started").Select(r => r.TeacherID).Distinct().Count(),
                        CountofTeachersinProgress = e.Where(r => r.Status == "In Progress").Select(r => r.TeacherID).Distinct().Count(),
                        CountofTeachersComplete = e.Where(r => r.Status == "Complete").Select(r => r.TeacherID).Distinct().Count(),
                        TotalTeachers = e.Select(r => r.TeacherID).Distinct().Count(),
                        CountofPosts = e.Sum(r => r.CountofPosts),
                        CountofCourseViewed = e.Sum(r => r.CourseViewed)
                    }).ToList();
                int numRows1 = listSummaryReport.Count;
                for (int i = 0; i < numRows1; i++)
                {
                    SummaryReportModel model = listSummaryReport[i];
                    index1++;
                    row = new Row { RowIndex = (uint)index1 };
                    TextCell cell1 = new TextCell(headers1[0].ToString(), model.CommunityDistrict, index1);
                    row.Append(cell1);

                    TextCell cell2 = new TextCell(headers1[1].ToString(), model.CircleCourse, index1);
                    row.Append(cell2);

                    ReportNumberCenterCell cell3 = new ReportNumberCenterCell(headers1[2].ToString(), model.CountofTeachersComplete.ToString(), index1);
                    row.Append(cell3);

                    ReportNumberCenterCell cell4 = new ReportNumberCenterCell(headers1[3].ToString(), model.CountofTeachersinProgress.ToString(), index1);
                    row.Append(cell4);

                    ReportNumberCenterCell cell5 = new ReportNumberCenterCell(headers1[4].ToString(), model.CountofTeachersNotStarted.ToString(), index1);
                    row.Append(cell5);

                    ReportNumberCenterCell cell6 = new ReportNumberCenterCell(headers1[5].ToString(), model.TotalTeachers.ToString(), index1);
                    row.Append(cell6);

                    ReportNumberCenterCell cell7 = new ReportNumberCenterCell(headers1[6].ToString(), model.CountofPosts.ToString(), index1);
                    row.Append(cell7);

                    ReportNumberCenterCell cell8 = new ReportNumberCenterCell(headers1[7].ToString(), model.CountofCourseViewed.ToString(), index1);
                    row.Append(cell8);

                    sheetData1.Append(row);
                }

                #endregion

                worksheetPart1.Worksheet = new Worksheet();
                worksheetPart1.Worksheet.Append(columns1);
                worksheetPart1.Worksheet.Append(sheetData1);

                var sheets = myWorkbook.WorkbookPart.Workbook.AppendChild<Sheets>(new Sheets());
                var sheet = new Sheet { Name = "Online Courses Report", SheetId = 1, Id = relId };
                sheets.Append(sheet);
                var sheet1 = new Sheet { Name = "Summary Report", SheetId = 2, Id = relId1 };
                sheets.Append(sheet1);
                workbookPart.Workbook.Save();
                myWorkbook.Close();
            }
        }

        #endregion
    }
}
