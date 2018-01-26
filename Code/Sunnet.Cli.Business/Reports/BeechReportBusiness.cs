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
        #region Beech Report

        public void GetBeechReport(out string path, List<int> communityIds, List<int> schoolIds, string teacher)
        {
            List<BeechReportModel> list = _reportService.GetBeechReport(communityIds, schoolIds, teacher);

            path = CreateReportName("BEECHReport");

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
                    "School Engage ID",
                    "Phone",
                    "Physical Address",
                    "City",
                    "Zip",
                    "Admin Name",
                    "Admin Email",
                    "Child Care License",
                    "Teacher Name",
                    "Teacher Engage ID",
                    "Teacher Primary Language",
                    "Class Name",
                    "Class Engage ID",
                    "Infants 0-17 mos",
                    "Toddler 18-35 mos",
                    "Pre School 3-5 yrs old",
                    "School Age 5-12 yrs old",
                    "Student Count Total"
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

                #region 填充Beech Report数据

                SheetData sheetData = new SheetData();
                int index = 1;
                var row = new Row { RowIndex = (uint)index };
                row.Append(new MediaBICell("0", "BEECH Report", index));
                sheetData.Append(row);
                index = index + 1;
                row = new Row { RowIndex = (uint)index };
                row.Append(new MediaBICell("0", DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss"), index));
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
                    BeechReportModel model = list[i];
                    index++;
                    row = new Row { RowIndex = (uint)index };
                    TextCell cell1 = new TextCell(headers[0].ToString(), model.CommunityDistrict, index);
                    row.Append(cell1);

                    TextCell cell2 = new TextCell(headers[1].ToString(), model.SchoolName, index);
                    row.Append(cell2);

                    TextCell cell3 = new TextCell(headers[2].ToString(), model.SchoolCode, index);
                    row.Append(cell3);

                    TextCell cell4 = new TextCell(headers[3].ToString(), model.Phone, index);
                    row.Append(cell4);

                    TextCell cell5 = new TextCell(headers[4].ToString(), model.PhysicalAddress, index);
                    row.Append(cell5);

                    TextCell cell6 = new TextCell(headers[5].ToString(), model.City, index);
                    row.Append(cell6);

                    TextCell cell7 = new TextCell(headers[6].ToString(), model.Zip, index);
                    row.Append(cell7);

                    TextCell cell8 = new TextCell(headers[7].ToString(), model.AdminName, index);
                    row.Append(cell8);

                    TextCell cell9 = new TextCell(headers[8].ToString(), model.AdminEmail, index);
                    row.Append(cell9);

                    TextCell cell10 = new TextCell(headers[9].ToString(), model.ChildCareLicense, index);
                    row.Append(cell10);

                    TextCell cell11 = new TextCell(headers[10].ToString(), model.TeacherName, index);
                    row.Append(cell11);

                    TextCell cell12 = new TextCell(headers[11].ToString(), model.TeacherCode, index);
                    row.Append(cell12);

                    TextCell cell13 = new TextCell(headers[12].ToString(), model.Language, index);
                    row.Append(cell13);

                    TextCell cell14 = new TextCell(headers[13].ToString(), model.ClassName, index);
                    row.Append(cell14);

                    TextCell cell15 = new TextCell(headers[14].ToString(), model.ClassCode, index);
                    row.Append(cell15);

                    ReportNumberCenterCell cell16 = new ReportNumberCenterCell(headers[15].ToString(),
                        model.Infant.ToString(), index);
                    row.Append(cell16);

                    ReportNumberCenterCell cell17 = new ReportNumberCenterCell(headers[16].ToString(),
                        model.Toddler.ToString(), index);
                    row.Append(cell17);

                    ReportNumberCenterCell cell18 = new ReportNumberCenterCell(headers[17].ToString(),
                        model.Preschool.ToString(), index);
                    row.Append(cell18);

                    ReportNumberCenterCell cell19 = new ReportNumberCenterCell(headers[18].ToString(),
                        model.Other.ToString(), index);
                    row.Append(cell19);

                    ReportNumberCenterCell cell20 = new ReportNumberCenterCell(headers[19].ToString(),
                        (model.Infant + model.Toddler + model.Preschool + model.Other).ToString(), index);
                    row.Append(cell20);

                    sheetData.Append(row);
                }
                index = index + 1;
                row = new Row { RowIndex = (uint)index };
                row.Append(new HeaderCell(headers[0].ToString(), "Grand Totals", index));
                row.Append(new NumberCell(headers[1].ToString(),"", index));
                row.Append(new ReportNumberCenterCell(headers[2].ToString(),
                    list.Where(e => !string.IsNullOrEmpty(e.SchoolCode)).
                        Select(e => e.SchoolCode).Distinct().Count().ToString(), index));
                row.Append(new NumberCell(headers[3].ToString(), "", index));
                row.Append(new NumberCell(headers[4].ToString(), "", index));
                row.Append(new NumberCell(headers[5].ToString(), "", index));
                row.Append(new NumberCell(headers[6].ToString(), "", index));
                row.Append(new NumberCell(headers[7].ToString(), "", index));
                row.Append(new NumberCell(headers[8].ToString(), "", index));
                row.Append(new NumberCell(headers[9].ToString(), "", index));
                row.Append(new NumberCell(headers[10].ToString(), "", index));
                row.Append(new ReportNumberCenterCell(headers[11].ToString(),
                    list.Where(e => !string.IsNullOrEmpty(e.TeacherCode)).
                        Select(e => e.TeacherCode).Distinct().Count().ToString(), index));
                row.Append(new NumberCell(headers[12].ToString(), "", index));
                row.Append(new NumberCell(headers[13].ToString(), "", index));
                row.Append(new ReportNumberCenterCell(headers[14].ToString(),
                    list.Where(e => !string.IsNullOrEmpty(e.ClassCode)).
                        Select(e => e.ClassCode).Distinct().Count().ToString(), index));
                row.Append(new ReportNumberCenterCell(headers[15].ToString(), list.Sum(e => e.Infant).ToString(), index));
                row.Append(new ReportNumberCenterCell(headers[16].ToString(), list.Sum(e => e.Toddler).ToString(), index));
                row.Append(new ReportNumberCenterCell(headers[17].ToString(), list.Sum(e => e.Preschool).ToString(), index));
                row.Append(new ReportNumberCenterCell(headers[18].ToString(), list.Sum(e => e.Other).ToString(), index));
                row.Append(new ReportNumberCenterCell(headers[19].ToString(),
                    list.Sum(e => e.Infant + e.Toddler + e.Preschool + e.Other).ToString(), index));
                sheetData.Append(row);

                #endregion

                worksheetPart.Worksheet = new Worksheet();
                worksheetPart.Worksheet.Append(columns);
                worksheetPart.Worksheet.Append(sheetData);

                var sheets = myWorkbook.WorkbookPart.Workbook.AppendChild<Sheets>(new Sheets());
                var sheet = new Sheet { Name = "FCC-BEECH", SheetId = 1, Id = relId };
                sheets.Append(sheet);
                workbookPart.Workbook.Save();
                myWorkbook.Close();
            }
        }

        #endregion
    }
}
