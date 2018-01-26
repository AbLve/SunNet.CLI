using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Sunnet.Cli.Core.Reports.Models;
using Sunnet.Framework.Excel;

namespace Sunnet.Cli.Business.Reports
{
    public partial class ReportBusiness
    {
        #region Coach Report

        public void GetCoachReport(out string path, List<int> communityIds, int mentorCoach, string funding, int status)
        {
            List<CoachReportModel> list = _reportService.GetCoachReport(communityIds, mentorCoach, funding, status);

            path = CreateReportName("CoachReport");

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
                    "School Type",
                    "Teacher First Name",
                    "Teacher Last Name",
                    "Teacher Engage ID",
                    "Phone Number",
                    "Email",
                    "Coaching Model",
                    "eCIRCLE Assignment",
                    "Years in Project",
                    "CLI Funding",
                    "Teacher Type",
                    "Coaching Hrs",
                    "Rem Cycles",
                    "Coach"
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

                #region 填充Coach Report数据

                SheetData sheetData = new SheetData();
                int index = 1;
                var row = new Row { RowIndex = (uint)index };
                row.Append(new MediaBICell("0", "Coach Report", index));
                sheetData.Append(row);
                index = index + 1;
                row = new Row { RowIndex = (uint)index };
                row.Append(new TextCell("0", DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"), index));
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
                    CoachReportModel model = list[i];
                    index++;
                    row = new Row { RowIndex = (uint)index };
                    TextCell cell1 = new TextCell(headers[0].ToString(), model.CommunityName, index);
                    row.Append(cell1);

                    TextCell cell2 = new TextCell(headers[1].ToString(), model.SchoolName, index);
                    row.Append(cell2);

                    TextCell cell3 = new TextCell(headers[2].ToString(), model.SchoolType, index);
                    row.Append(cell3);

                    TextCell cell4 = new TextCell(headers[3].ToString(), model.FirstName, index);
                    row.Append(cell4);

                    TextCell cell5 = new TextCell(headers[4].ToString(), model.LastName, index);
                    row.Append(cell5);

                    TextCell cell6 = new TextCell(headers[5].ToString(), model.TeacherCode, index);
                    row.Append(cell6);

                    TextCell cell7 = new TextCell(headers[6].ToString(), model.PrimaryPhoneNumber, index);
                    row.Append(cell7);

                    TextCell cell8 = new TextCell(headers[7].ToString(), model.PrimaryEmailAddress, index);
                    row.Append(cell8);

                    TextCell cell9 = new TextCell(headers[8].ToString(), model.CoachModel, index);
                    row.Append(cell9);

                    TextCell cell10 = new TextCell(headers[9].ToString(), model.EcircleAssignment, index);
                    row.Append(cell10);

                    TextCell cell11 = new TextCell(headers[10].ToString(), model.YearsInProject, index);
                    row.Append(cell11);

                    TextCell cell12 = new TextCell(headers[11].ToString(), model.TeacherFunding, index);
                    row.Append(cell12);

                    TextCell cell13 = new TextCell(headers[12].ToString(), model.TeacherType, index);
                    row.Append(cell13);

                    ReportNumberD2CenterCell cell14 = new ReportNumberD2CenterCell(headers[13].ToString(), model.NumberofCoachingHoursReceived.ToString(), index);
                    row.Append(cell14);

                    ReportNumberD2CenterCell cell15 = new ReportNumberD2CenterCell(headers[14].ToString(), model.NumberofRemoteCoachingCyclesReceived.ToString(), index);
                    row.Append(cell15);

                    TextCell cell16 = new TextCell(headers[15].ToString(), model.Coach, index);
                    row.Append(cell16);

                    sheetData.Append(row);
                }
                index = index + 1;
                row = new Row { RowIndex = (uint)index };
                row.Append(new HeaderCell(headers[0].ToString(), "Grand Totals", index));
                row.Append(new ReportNumberCenterCell(headers[1].ToString(),
                    list.Where(e => !string.IsNullOrEmpty(e.SchoolName)).
                        Select(e => e.SchoolName).Distinct().Count().ToString(), index));
                row.Append(new NumberCell(headers[2].ToString(), "", index));
                row.Append(new NumberCell(headers[3].ToString(), "", index));
                row.Append(new NumberCell(headers[4].ToString(), "", index));
                row.Append(new ReportNumberCenterCell(headers[5].ToString(),
                    list.Where(e => !string.IsNullOrEmpty(e.TeacherCode)).
                        Select(e => e.TeacherCode).Distinct().Count().ToString(), index));
                row.Append(new NumberCell(headers[6].ToString(), "", index));
                row.Append(new NumberCell(headers[7].ToString(), "", index));
                row.Append(new NumberCell(headers[8].ToString(), "", index));
                row.Append(new NumberCell(headers[9].ToString(), "", index));
                row.Append(new NumberCell(headers[10].ToString(), "", index));
                row.Append(new NumberCell(headers[11].ToString(), "", index));
                row.Append(new NumberCell(headers[12].ToString(), "", index));
                row.Append(new ReportNumberD2CenterCell(headers[13].ToString(),
                    list.Sum(e => e.NumberofCoachingHoursReceived).ToString(), index));
                row.Append(new ReportNumberD2CenterCell(headers[14].ToString(),
                    list.Sum(e => e.NumberofRemoteCoachingCyclesReceived).ToString(), index));
                row.Append(new NumberCell(headers[15].ToString(), "", index));
                sheetData.Append(row);

                #endregion

                worksheetPart.Worksheet = new Worksheet();
                worksheetPart.Worksheet.Append(columns);
                worksheetPart.Worksheet.Append(sheetData);

                var sheets = myWorkbook.WorkbookPart.Workbook.AppendChild<Sheets>(new Sheets());
                var sheet = new Sheet { Name = "Coach", SheetId = 1, Id = relId };
                sheets.Append(sheet);
                workbookPart.Workbook.Save();
                myWorkbook.Close();
            }
        }

        #endregion
    }
}
