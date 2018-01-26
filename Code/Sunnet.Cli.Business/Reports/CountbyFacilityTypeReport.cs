using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Sunnet.Cli.Business.Common;
using Sunnet.Cli.Core.Common.Enums;
using Sunnet.Cli.Core.Cot.Models;
using Sunnet.Cli.Core.Reports.Entities;
using Sunnet.Cli.Core.Reports.Models;
using Sunnet.Cli.Core.Users.Entities;
using Sunnet.Cli.Core.Users.Enums;
using Sunnet.Framework;
using Sunnet.Framework.Excel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Cli.Business.Reports
{
    public partial class ReportBusiness
    {
        public void CountbyFacilityTypeReport(out string path)
        {
            path = string.Empty;

            List<CountbyFacilityTypeMode> list = new List<CountbyFacilityTypeMode>();

   
            path =  CreateReportName("CountbyFacilityType");

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
                    "      Type      ",
                    "Schools",
                    "Classrooms",
                    "Teachers",
                    "Students"
                };

                int numCols = headerNames.Count;
                var columns = new Columns();
                for (int col = 0; col < numCols; col++)
                {
                    int width = headerNames[col].Length + 5;
                    Column c = new CustomColumn((UInt32)col + 1,
                                  (UInt32)numCols + 1, width);
                    columns.Append(c);
                }
                #region 填充数据
                SheetData sheetData = new SheetData();
                int index = 1;
                var row = new Row { RowIndex = (uint)index };
                row.Append(new HeaderCell("0", string.Format("20{0} TSR Count by Facility Type", CommonAgent.SchoolYear), index));
                sheetData.Append(row);

                index++;
                row = new Row { RowIndex = (uint)index };
                row.Append(new TextCell("0", string.Format("These Numbers are based on Rosters imported to TOMS as of {0}", DateTime.Now.ToString("M"))
                    , index));
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


                foreach (CountbyFacilityTypeMode model in list)
                {
                    index++;
                    row = new Row { RowIndex = (uint)index };

                    TextCell cell1 = new TextCell("A", model.SchoolType, index);
                    row.Append(cell1);

                    NumberCell cell2 = new NumberCell("B", model.SchoolTotal.ToString(), index);
                    row.Append(cell2);

                    NumberCell cell3 = new NumberCell("C", model.ClassroomTotal.ToString(), index);
                    row.Append(cell3);

                    NumberCell cell4 = new NumberCell("D", model.TeacherTotal.ToString(), index);
                    row.Append(cell4);

                    NumberCell cell5 = new NumberCell("E", model.StudentTotal.ToString(), index);
                    row.Append(cell5);

                    sheetData.Append(row);
                }

                index++;
                row = new Row { RowIndex = (uint)index };
                row.Append(new TextCell("A", "Total", index));
                row.Append(new NumberCell("B", list.Sum(r => r.SchoolTotal).ToString(), index));
                row.Append(new NumberCell("C", list.Sum(r => r.ClassroomTotal).ToString(), index));
                row.Append(new NumberCell("D", list.Sum(r => r.TeacherTotal).ToString(), index));
                row.Append(new TextCell("E", list.Sum(r => r.StudentTotal).ToString(), index));
                sheetData.Append(row);
                #endregion

                worksheetPart.Worksheet = new Worksheet();
                worksheetPart.Worksheet.Append(columns);
                worksheetPart.Worksheet.Append(sheetData);

                var sheets = myWorkbook.WorkbookPart.Workbook.AppendChild<Sheets>(new Sheets());
                var sheet = new Sheet { Name = "Count by Facility Type", SheetId = 1, Id = relId };
                sheets.Append(sheet);
                workbookPart.Workbook.Save();
                myWorkbook.Close();
            }
        }
    }
}
