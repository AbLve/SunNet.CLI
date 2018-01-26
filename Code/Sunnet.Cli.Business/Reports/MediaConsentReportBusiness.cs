using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Sunnet.Cli.Core.Reports.Models;
using Sunnet.Cli.Core.Students.Enums;
using Sunnet.Framework.Excel;
using Sunnet.Framework.Extensions;

namespace Sunnet.Cli.Business.Reports
{
    public partial class ReportBusiness
    {
        #region Media Consent

        public void GetMediaConsentPercent(out string path, List<int> communityIds, List<int> schoolIds, string teacher)
        {
            List<MediaConsentPercentModel> list = _reportService.GetMediaConsentPercent(communityIds, schoolIds, teacher);

            path = CreateReportName("MediaConsent");

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
                    "Community Name",
                    "School Name",
                    "Teacher Name",
                    "Teacher ID",
                    "Coaching Model",
                    "Years in Project",
                    "Coach",
                    "Teacher Media Release",
                    "Class Name",
                    "Class Code",
                    "Media Release: Yes",
                    "Media Release: Refused",
                    "Media Release: No (missing)",
                    "Percent of Consented Students (Yes + Refused)",
                    "Total Students in Class"
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

                #region 填充MediaConsentPercent数据

                SheetData sheetData = new SheetData();
                int index = 1;
                var row = new Row { RowIndex = (uint)index };
                row.Append(new MediaBICell("0", "Media Consent Percent Report", index));
                sheetData.Append(row);

                index = index + 1;
                row = new Row { RowIndex = (uint)index };
                row.Append(new MediaBICell("0", DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss"), index));
                sheetData.Append(row);

                index = index + 1;
                row = new Row { RowIndex = (uint)index };
                row.Append(new TextCell("0",
                    "*report showing the percent of students in a teacher's class with Yes, No, or Refused selected for the CLI entered 'Media Release?' field on the Student page.",
                    index));
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
                    MediaConsentPercentModel model = list[i];
                    index++;
                    row = new Row { RowIndex = (uint)index };
                    TextCell cell1 = new TextCell(headers[0].ToString(), model.CommunityName, index);
                    row.Append(cell1);

                    TextCell cell2 = new TextCell(headers[1].ToString(), model.SchoolName, index);
                    row.Append(cell2);

                    TextCell cell3 = new TextCell(headers[2].ToString(), model.TeacherName, index);
                    row.Append(cell3);

                    TextCell cell4 = new TextCell(headers[3].ToString(), model.TeacherCode, index);
                    row.Append(cell4);

                    TextCell cell5 = new TextCell(headers[4].ToString(), model.CoachingModel, index);
                    row.Append(cell5);

                    TextCell cell6 = new TextCell(headers[5].ToString(), model.YearsinProject, index);
                    row.Append(cell6);

                    TextCell cell7 = new TextCell(headers[6].ToString(), model.CoachName, index);
                    row.Append(cell7);

                    if (model.TeacherMediaRelease == MediaRelease.Yes)
                    {
                        TextCell cell8 = new TextCell(headers[7].ToString(), model.TeacherMediaRelease.ToDescription(),
                            index);
                        row.Append(cell8);
                    }
                    else
                    {
                        MediaTextCell cell8 = new MediaTextCell(headers[7].ToString(),
                            model.TeacherMediaRelease.ToDescription() == "0"
                                ? "No"
                                : model.TeacherMediaRelease.ToDescription(), index);
                        row.Append(cell8);
                    }

                    TextCell cell9 = new TextCell(headers[8].ToString(), model.ClassName, index);
                    row.Append(cell9);

                    TextCell cell10 = new TextCell(headers[9].ToString(), model.ClassCode, index);
                    row.Append(cell10);

                    NumberCell cell11 = new NumberCell(headers[10].ToString(),
                        model.MediaReleaseYes == 0 ? "" : model.MediaReleaseYes.ToString(), index);
                    row.Append(cell11);

                    NumberCell cell12 = new NumberCell(headers[11].ToString(),
                        model.MediaReleaseRefused == 0 ? "" : model.MediaReleaseRefused.ToString(), index);
                    row.Append(cell12);

                    NumberCell cell13 = new NumberCell(headers[12].ToString(),
                        model.MediaReleaseNo == 0 ? "" : model.MediaReleaseNo.ToString(), index);
                    row.Append(cell13);

                    if (model.PercentofConsented == "100.0")
                    {
                        MediaTestCell cell14 = new MediaTestCell(headers[13].ToString(), model.PercentofConsented,
                            index);
                        row.Append(cell14);
                    }
                    else
                    {
                        TextCell cell14 = new TextCell(headers[13].ToString(), model.PercentofConsented, index);
                        row.Append(cell14);
                    }

                    NumberCell cell15 = new NumberCell(headers[14].ToString(), model.TotalStudentsinClass.ToString(),
                        index);
                    row.Append(cell15);

                    sheetData.Append(row);
                }

                #endregion

                worksheetPart.Worksheet = new Worksheet();
                worksheetPart.Worksheet.Append(columns);
                worksheetPart.Worksheet.Append(sheetData);

                #region Create Student Sheet
                var worksheetPart1 = workbookPart.AddNewPart<WorksheetPart>();
                string relId1 = workbookPart.GetIdOfPart(worksheetPart1);
                #endregion

                List<MediaConsentDetailModel> listMediaConsentDetail = _reportService.GetMediaConsentDetail(communityIds, schoolIds, teacher);
                List<string> headerNamesMediaDetail = new List<string>
                {
                    "Community Name",
                    "School Name",
                    "Teacher Name",
                    "Teacher ID",
                    "Coach Name",
                    "Student Name",
                    "Student Code",
                    "Student Status",
                    "Student Media Release"
                };
                int numRows1 = listMediaConsentDetail.Count;
                int numCols1 = headerNamesMediaDetail.Count;
                var columns1 = new Columns();
                for (int col = 0; col < numCols1; col++)
                {
                    int width = headerNamesMediaDetail[col].Length + 5;
                    Column c = new CustomColumn((UInt32)col + 1,
                        (UInt32)numCols1 + 1, width);
                    columns1.Append(c);
                }

                SheetData sheetData1 = new SheetData();
                int index1 = 1;
                row = new Row { RowIndex = (uint)index1 };
                row.Append(new MediaBICell("0", "Class list with Student Media Release", index1));
                sheetData1.Append(row);

                index1 = index1 + 1;
                row = new Row { RowIndex = (uint)index1 };
                row.Append(new MediaBICell("0", DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss"), index1));
                sheetData1.Append(row);

                index1 = index1 + 2;
                int numRows11 = listMediaConsentDetail.Count;

                #region 填充Detail数据

                List<Char> headers1 = az.GetRange(0, headerNamesMediaDetail.Count);
                Row header1 = new Row { RowIndex = (uint)index1 };
                for (int col = 0; col < headerNamesMediaDetail.Count; col++)
                {
                    HeaderCell cell = new HeaderCell(headers1[col].ToString(), headerNamesMediaDetail[col], index1);
                    header1.Append(cell);
                }
                sheetData1.Append(header1);
                for (int i = 0; i < numRows11; i++)
                {
                    MediaConsentDetailModel model = listMediaConsentDetail[i];
                    index1++;
                    row = new Row { RowIndex = (uint)index1 };
                    TextCell cell1 = new TextCell(headers1[0].ToString(), model.CommunityName, index1);
                    row.Append(cell1);

                    TextCell cell2 = new TextCell(headers1[1].ToString(), model.SchoolName, index1);
                    row.Append(cell2);

                    TextCell cell3 = new TextCell(headers1[2].ToString(), model.TeacherName, index1);
                    row.Append(cell3);

                    TextCell cell4 = new TextCell(headers1[3].ToString(), model.TeacherCode, index1);
                    row.Append(cell4);

                    TextCell cell5 = new TextCell(headers1[4].ToString(), model.CoachName, index1);
                    row.Append(cell5);

                    TextCell cell6 = new TextCell(headers1[5].ToString(), model.StudentName, index1);
                    row.Append(cell6);

                    TextCell cell7 = new TextCell(headers1[6].ToString(), model.StudentCode, index1);
                    row.Append(cell7);

                    TextCell cell8 = new TextCell(headers1[7].ToString(), model.StudentStatus.ToDescription(), index1);
                    row.Append(cell8);

                    if (model.StudentMediaRelease == MediaRelease.Yes ||
                        model.StudentMediaRelease == MediaRelease.Refused)
                    {
                        TextCell cell9 = new TextCell(headers1[8].ToString(), model.StudentMediaRelease.ToDescription(),
                            index1);
                        row.Append(cell9);
                    }
                    else
                    {
                        MediaTextCell cell9 = new MediaTextCell(headers[8].ToString(),
                            model.StudentMediaRelease.ToDescription() == "0"
                                ? "No"
                                : model.StudentMediaRelease.ToDescription(), index1);
                        row.Append(cell9);
                    }
                    sheetData1.Append(row);
                }

                #endregion

                worksheetPart1.Worksheet = new Worksheet();
                worksheetPart1.Worksheet.Append(columns1);
                worksheetPart1.Worksheet.Append(sheetData1);

                var sheets = myWorkbook.WorkbookPart.Workbook.AppendChild<Sheets>(new Sheets());
                var sheet = new Sheet { Name = "Media Consent", SheetId = 1, Id = relId };
                sheets.Append(sheet);
                var sheet1 = new Sheet { Name = "Student List", SheetId = 2, Id = relId1 };
                sheets.Append(sheet1);
                workbookPart.Workbook.Save();
                myWorkbook.Close();
            }
        }
        #endregion
    }
}
