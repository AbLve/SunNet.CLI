using System.Data;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections.Generic;
using System.Linq;
using Sunnet.Cli.Core.Common.Enums;
using Sunnet.Cli.Core.Reports.Models;
using Sunnet.Framework.Excel;

namespace Sunnet.Cli.Business.Reports
{
    public partial class ReportBusiness
    {
        #region Ever serviced Report

        public void GetServiceReport(out string path, List<int> communityIds, List<int> schoolIds, int serviceType)
        {
            List<ServiceReportModel> list = _reportService.GetServiceReport(communityIds, schoolIds);
            List<ServiceReportModel> listActiveSchool = list.Where(e => e.Status == EntityStatus.Active).ToList();

            path = CreateReportName("ServiceReport");

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

                #region 填充Active School数据
                List<string> headerNames = new List<string>
                {
                    "Community/District Name",
                    "ESC Name",
                    "School Name",
                    "School Code",
                    "Physical Address1",
                    "Physical Address2",
                    "School City",
                    "School County",
                    "School State",
                    "School Zip",
                    "School Type",
                    "Cohort",
                    "Classroom Count",
                    "Teacher Count",
                    "Student Count"
                };
                int numRows = listActiveSchool.Count;
                int numCols = headerNames.Count;
                var columns = new Columns();
                for (int col = 0; col < numCols; col++)
                {
                    int width = headerNames[col].Length + 5;
                    Column c = new CustomColumn((UInt32)col + 1,
                        (UInt32)numCols + 1, width);
                    columns.Append(c);
                }
                SheetData sheetData = new SheetData();
                int index = 1;
                var r1 = new Row { RowIndex = (uint)index };

                r1.Append(new MediaBICell("0", "Service Report", index));
                sheetData.Append(r1);

                index = index + 1;
                r1 = new Row { RowIndex = (uint)index };
                r1.Append(new MediaBICell("0", DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss"), index));
                sheetData.Append(r1);

                index = index + 2;
                r1 = new Row { RowIndex = (uint)index };
                r1.Append(new MediaBICell("0", "Active School", index));
                sheetData.Append(r1);

                index = index + 1;
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
                    ServiceReportModel model = listActiveSchool[i];
                    index++;
                    var r = new Row { RowIndex = (uint)index };
                    TextCell cell1 = new TextCell(headers[0].ToString(), model.CommunityName, index);
                    r.Append(cell1);

                    TextCell cell2 = new TextCell(headers[1].ToString(), model.ESCName.ToString(), index);
                    r.Append(cell2);

                    TextCell cell3 = new TextCell(headers[2].ToString(), model.SchoolName, index);
                    r.Append(cell3);

                    TextCell cell4 = new TextCell(headers[3].ToString(), model.SchoolCode, index);
                    r.Append(cell4);

                    TextCell cell5 = new TextCell(headers[4].ToString(), model.PhysicalAddress1, index);
                    r.Append(cell5);

                    TextCell cell6 = new TextCell(headers[5].ToString(), model.PhysicalAddress2, index);
                    r.Append(cell6);

                    TextCell cell7 = new TextCell(headers[6].ToString(), model.City, index);
                    r.Append(cell7);

                    TextCell cell8 = new TextCell(headers[7].ToString(), model.County, index);
                    r.Append(cell8);

                    TextCell cell9 = new TextCell(headers[8].ToString(), model.State, index);
                    r.Append(cell9);

                    TextCell cell10 = new TextCell(headers[9].ToString(), model.Zip, index);
                    r.Append(cell10);

                    TextCell cell11 = new TextCell(headers[10].ToString(), model.SchoolType, index);
                    r.Append(cell11);

                    TextCell cell12 = new TextCell(headers[11].ToString(), model.SchoolYear, index);
                    r.Append(cell12);

                    NumberCell cell13 = new NumberCell(headers[12].ToString(), model.ClassroomCount.ToString(), index);
                    r.Append(cell13);

                    NumberCell cell14 = new NumberCell(headers[13].ToString(), model.TeacherCount.ToString(), index);
                    r.Append(cell14);

                    NumberCell cell15 = new NumberCell(headers[14].ToString(), model.StudentCount.ToString(), index);
                    r.Append(cell15);

                    sheetData.Append(r);
                }

                #endregion

                if (serviceType == 1)
                {
                    #region 填充Inactive Report数据
                    List<ServiceReportModel> listInactiveSchool =
                        list.Where(e => e.Status == EntityStatus.Inactive).ToList();

                    List<string> headerNames1 = new List<string>
                    {
                        "Community/District Name",
                        "ESC Name",
                        "School Name",
                        "School Code",
                        "Physical Address1",
                        "Physical Address2",
                        "School City",
                        "School County",
                        "School State",
                        "School Zip",
                        "School Phone",
                        "School Type",
                        "Child Care License",
                        "Funding",
                        "Classroom Count",
                        "Teacher Count",
                        "Student Count"
                    };
                    int numCols1 = headerNames1.Count;
                    var columns1 = new Columns();
                    for (int col = 0; col < numCols; col++)
                    {
                        int width = headerNames1[col].Length + 5;
                        Column c = new CustomColumn((UInt32) col + 1,
                            (UInt32) numCols1 + 1, width);
                        columns1.Append(c);
                    }

                    //Count of Teacher Years in Project by School Type
                    index = index + 3;
                    r1 = new Row {RowIndex = (uint) index};
                    r1.Append(new MediaBICell("0", "Inactive School", index));
                    sheetData.Append(r1);

                    index = index + 1;
                    List<Char> headers1 = az.GetRange(0, headerNames1.Count);
                    header = new Row {RowIndex = (uint) index};
                    for (int col = 0; col < headerNames1.Count; col++)
                    {
                        HeaderCell cell = new HeaderCell(headers1[col].ToString(), headerNames1[col], index);
                        header.Append(cell);
                    }
                    sheetData.AppendChild(header);
                    for (int i = 0; i < listInactiveSchool.Count; i++)
                    {
                        ServiceReportModel model = listInactiveSchool[i];
                        index++;
                        var r = new Row {RowIndex = (uint) index};
                        TextCell cell1 = new TextCell(headers1[0].ToString(), model.CommunityName, index);
                        r.Append(cell1);

                        TextCell cell2 = new TextCell(headers1[1].ToString(), model.ESCName.ToString(), index);
                        r.Append(cell2);

                        TextCell cell3 = new TextCell(headers1[2].ToString(), model.SchoolName, index);
                        r.Append(cell3);

                        TextCell cell4 = new TextCell(headers1[3].ToString(), model.SchoolCode, index);
                        r.Append(cell4);

                        TextCell cell5 = new TextCell(headers1[4].ToString(), model.PhysicalAddress1, index);
                        r.Append(cell5);

                        TextCell cell6 = new TextCell(headers1[5].ToString(), model.PhysicalAddress2, index);
                        r.Append(cell6);

                        TextCell cell7 = new TextCell(headers1[6].ToString(), model.City, index);
                        r.Append(cell7);

                        TextCell cell8 = new TextCell(headers1[7].ToString(), model.County, index);
                        r.Append(cell8);

                        TextCell cell9 = new TextCell(headers1[8].ToString(), model.State, index);
                        r.Append(cell9);

                        TextCell cell10 = new TextCell(headers1[9].ToString(), model.Zip, index);
                        r.Append(cell10);

                        TextCell cell11 = new TextCell(headers1[10].ToString(), model.Phone, index);
                        r.Append(cell11);

                        TextCell cell12 = new TextCell(headers1[11].ToString(), model.SchoolType, index);
                        r.Append(cell12);

                        TextCell cell13 = new TextCell(headers1[12].ToString(), model.ChildCareLicense, index);
                        r.Append(cell13);

                        TextCell cell14 = new TextCell(headers1[13].ToString(), model.Funding, index);
                        r.Append(cell14);

                        NumberCell cell15 = new NumberCell(headers1[14].ToString(), model.ClassroomCount.ToString(),
                            index);
                        r.Append(cell15);

                        NumberCell cell16 = new NumberCell(headers1[15].ToString(), model.TeacherCount.ToString(), index);
                        r.Append(cell16);

                        NumberCell cell17 = new NumberCell(headers1[16].ToString(), model.StudentCount.ToString(), index);
                        r.Append(cell17);

                        sheetData.Append(r);
                    }
                    #endregion
                }
                worksheetPart.Worksheet = new Worksheet();
                worksheetPart.Worksheet.Append(columns);
                worksheetPart.Worksheet.Append(sheetData);

                var sheets = myWorkbook.WorkbookPart.Workbook.AppendChild<Sheets>(new Sheets());
                var sheet = new Sheet { Name = "Service Report", SheetId = 1, Id = relId };
                sheets.Append(sheet);
                workbookPart.Workbook.Save();
                myWorkbook.Close();
            }
        }

        #endregion
    }
}
