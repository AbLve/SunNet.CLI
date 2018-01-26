using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Sunnet.Cli.Business.Common;
using Sunnet.Cli.Core.Reports.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sunnet.Framework.Excel;
using Sunnet.Framework.Extensions;
using Sunnet.Cli.Core.Schools.Entities;

namespace Sunnet.Cli.Business.Reports
{
    public partial class ReportBusiness
    {
        /// <summary>
        /// 内部用户 communityIds.count ==0； 
        /// </summary>
        public void GetParticipationCountsReport(out string path, List<int> communityIds, List<int> fundingList,
            DateTime? startDate, DateTime? endDate, int status)
        {
            int communityId = 0;
            List<QuarterlyReportModel> listYearsInProjectCountBySchoolType =
                _reportService.GetYearsInProjectCountBySchoolType(communityIds, fundingList, startDate, endDate, status);
            List<CountbyFacilityTypeMode> listCountbyFacilityType = _reportService.GetCountbyFacilityTypeList(communityIds, fundingList, startDate, endDate, status);
            path = CreateReportName("QuarterlyReport");

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
                    "School Type",
                    "School Count",
                    "Classroom Count",
                    "Teacher Count",
                    "Student Count"
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
                #region 填充 Counts by Facility Type数据
                SheetData sheetData = new SheetData();
                int index = 1;
                var row = new Row { RowIndex = (uint)index };

                string statusText = string.Empty;
                switch (status)
                {
                    case 1:
                        statusText = "Active";
                        break;
                    case 2:
                        statusText = "Inactive";
                        break;
                    case 0:
                        statusText = "All";
                        break;
                }

                row.Append(new MediaBICell("0", "Participation Counts - " + statusText, index));
                sheetData.Append(row);

                index = index + 1;
                row = new Row { RowIndex = (uint)index };
                row.Append(new TextCell("0", DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss"), index));
                sheetData.Append(row);

                index = index + 3;
                row = new Row { RowIndex = (uint)index };
                row.Append(new HeaderCell("0", "Counts by Facility Type", index));
                sheetData.Append(row);

                index = index + 1;
                row = new Row { RowIndex = (uint)index };
                row.Append(new MediaBICell("0", "Time Period : " + startDate.Value.ToString("MM/dd/yyyy") + " - " + endDate.Value.ToString("MM/dd/yyyy"), index));
                sheetData.Append(row);

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

                List<SchoolTypeEntity> schoolTypeList = _schoolBusiness.GetSchoolTypeList(0);

                CountbyFacilityTypeMode missingModel = new CountbyFacilityTypeMode();

                foreach (CountbyFacilityTypeMode item in listCountbyFacilityType)
                {
                    if (item.Status == Core.Common.Enums.EntityStatus.Inactive || item.SchoolTypeId == 0)
                    {
                        missingModel.SchoolTotal += item.SchoolTotal;
                        missingModel.ClassroomTotal += item.ClassroomTotal;
                        missingModel.TeacherTotal += item.TeacherTotal;
                        missingModel.StudentTotal += item.StudentTotal;
                        continue;
                    }
                    index++;
                    row = new Row { RowIndex = (uint)index };

                    TextCell cell1 = new TextCell("A", item.SchoolType, index);
                    row.Append(cell1);

                    NumberCell cell2 = new NumberCell("B", item.SchoolTotal.ToString(), index);
                    row.Append(cell2);

                    NumberCell cell3 = new NumberCell("C", item.ClassroomTotal.ToString(), index);
                    row.Append(cell3);

                    NumberCell cell4 = new NumberCell("D", item.TeacherTotal.ToString(), index);
                    row.Append(cell4);

                    NumberCell cell5 = new NumberCell("E", item.StudentTotal.ToString(), index);
                    row.Append(cell5);

                    sheetData.Append(row);
                }

                index++;
                row = new Row { RowIndex = (uint)index };
                row.Append(new TextCell("A", "Missing", index));
                row.Append(new NumberCell("B", missingModel.SchoolTotal.ToString(), index));
                row.Append(new NumberCell("C", missingModel.ClassroomTotal.ToString(), index));
                row.Append(new NumberCell("D", missingModel.TeacherTotal.ToString(), index));
                row.Append(new NumberCell("E", missingModel.StudentTotal.ToString(), index));
                sheetData.Append(row);

                index++;
                row = new Row { RowIndex = (uint)index };
                row.Append(new HeaderCell("A", "Total", index));
                row.Append(new NumberCell("B", listCountbyFacilityType.Sum(r => r.SchoolTotal).ToString(), index));
                row.Append(new NumberCell("C", listCountbyFacilityType.Sum(r => r.ClassroomTotal).ToString(), index));
                row.Append(new NumberCell("D", listCountbyFacilityType.Sum(r => r.TeacherTotal).ToString(), index));
                row.Append(new NumberCell("E", listCountbyFacilityType.Sum(r => r.StudentTotal).ToString(), index));
                sheetData.Append(row);
                #endregion

                #region 填充Quarterly Report数据



                //Years in Project by School Type
                List<YearsInProjectModel> listYearsInProject = new List<YearsInProjectModel>();
                listYearsInProject =
                    listYearsInProjectCountBySchoolType.GroupBy(e => new { e.SchoolTypeId, e.SchoolType }).Select(e => new YearsInProjectModel()
                    {
                        SchoolTypeId = e.Key.SchoolTypeId,
                        SchoolType = e.Key.SchoolType,
                        Year1Count = e.Where(r => r.YearsInProjectId == 1).Sum(r => r.Count),
                        Year2Count = e.Where(r => r.YearsInProjectId == 2).Sum(r => r.Count),
                        Year3Count = e.Where(r => r.YearsInProjectId == 3).Sum(r => r.Count),
                        MissingCount =
                            e.Where(r => r.YearsInProjectId == 0 || r.YearsInProjectId == 4).Sum(r => r.Count)
                    }).ToList();

                //Count of Teacher Years in Project by School Type
                index = index + 3;
                row = new Row { RowIndex = (uint)index };
                row.Append(new MediaBICell("0", "Count of 'Teacher Years in Project' by Facility Type", index));
                sheetData.Append(row);
                index = index + 1;
                row = new Row { RowIndex = (uint)index };
                row.Append(new MediaBICell("0", "Time Period : " + startDate.Value.ToString("MM/dd/yyyy") + " - " + endDate.Value.ToString("MM/dd/yyyy"), index));
                sheetData.Append(row);

                index = index + 1;
                row = new Row { RowIndex = (uint)index };
                row.Append(new HeaderCell("0", "School Type", index));
                row.Append(new HeaderCell("1", "Year 1", index));
                row.Append(new HeaderCell("2", "Year 2", index));
                row.Append(new HeaderCell("3", "Year 3", index));
                row.Append(new HeaderCell("4", "Missing", index));
                row.Append(new HeaderCell("5", "Grand Total", index));
                sheetData.Append(row);
                YearsInProjectModel yearsMissingModel = new YearsInProjectModel();
                foreach (var yearsInProject in listYearsInProject)
                {
                    if (yearsInProject.SchoolTypeId == 0)
                    {
                        yearsMissingModel.SchoolType = "Missing";
                        yearsMissingModel.Year1Count += yearsInProject.Year1Count;
                        yearsMissingModel.Year2Count += yearsInProject.Year2Count;
                        yearsMissingModel.Year3Count += yearsInProject.Year3Count;
                        yearsMissingModel.MissingCount += yearsInProject.MissingCount;
                        continue;
                    }
                    index = index + 1;
                    var r = new Row { RowIndex = (uint)index };
                    r.Append(new TextCell("0", yearsInProject.SchoolType, index));
                    r.Append(new NumberCell("1", yearsInProject.Year1Count.ToString(), index));
                    r.Append(new NumberCell("2", yearsInProject.Year2Count.ToString(), index));
                    r.Append(new NumberCell("3", yearsInProject.Year3Count.ToString(), index));
                    r.Append(new NumberCell("4", yearsInProject.MissingCount.ToString(), index));
                    r.Append(new NumberCell("5",
                        (yearsInProject.Year1Count + yearsInProject.Year2Count + yearsInProject.Year3Count +
                         yearsInProject.MissingCount).ToString(), index));
                    sheetData.Append(r);
                }
                index = index + 1;
                row = new Row { RowIndex = (uint)index };
                row.Append(new TextCell("0", yearsMissingModel.SchoolType, index));
                row.Append(new NumberCell("1", yearsMissingModel.Year1Count.ToString(), index));
                row.Append(new NumberCell("2", yearsMissingModel.Year2Count.ToString(), index));
                row.Append(new NumberCell("3", yearsMissingModel.Year3Count.ToString(), index));
                row.Append(new NumberCell("4", yearsMissingModel.MissingCount.ToString(), index));
                row.Append(new NumberCell("5",
                    (yearsMissingModel.Year1Count + yearsMissingModel.Year2Count + yearsMissingModel.Year3Count +
                     yearsMissingModel.MissingCount).ToString(), index));
                sheetData.Append(row);
                index = index + 1;
                row = new Row { RowIndex = (uint)index };
                row.Append(new HeaderCell("0", "Grand Total", index));
                row.Append(new NumberCell("1", listYearsInProject.Sum(e => e.Year1Count).ToString(), index));
                row.Append(new NumberCell("2", listYearsInProject.Sum(e => e.Year2Count).ToString(), index));
                row.Append(new NumberCell("3", listYearsInProject.Sum(e => e.Year3Count).ToString(), index));
                row.Append(new NumberCell("4", listYearsInProject.Sum(e => e.MissingCount).ToString(), index));
                row.Append(new NumberCell("5", (listYearsInProject.Sum(e => e.Year1Count)
                                                + listYearsInProject.Sum(e => e.Year2Count)
                                                + listYearsInProject.Sum(e => e.Year3Count)
                                                + listYearsInProject.Sum(e => e.MissingCount)).ToString(), index));
                sheetData.Append(row);
                #endregion

                worksheetPart.Worksheet = new Worksheet();
                worksheetPart.Worksheet.Append(columns);
                worksheetPart.Worksheet.Append(sheetData);

                #region Create Community Counts Sheet
                var worksheetPart1 = workbookPart.AddNewPart<WorksheetPart>();
                string relId1 = workbookPart.GetIdOfPart(worksheetPart1);
                #endregion

                List<CountbyCommunityModel> list = _reportService.GetCountbyCommunityList(communityIds, fundingList, startDate, endDate, status);
                List<SelectItemModel> fundingModelList = _masterBusiness.GetFundingSelectList().ToList();
                fundingModelList.Insert(0, new SelectItemModel() { ID = 0, Name = "Missing Funding" });
                List<string> headerNamesCommunityCounts = new List<string>
                {
                    "       Community      "
                };
                foreach (SelectItemModel model in fundingModelList)
                    headerNamesCommunityCounts.Add(model.Name);

                if (status == 1)
                {
                    headerNamesCommunityCounts.Add("Active Classroom Count");
                    headerNamesCommunityCounts.Add("Active Teacher Count");
                    headerNamesCommunityCounts.Add("Active Student Count");
                }
                else if (status == 2)
                {
                    headerNamesCommunityCounts.Add("Inactive Classroom Count");
                    headerNamesCommunityCounts.Add("Inactive Teacher Count");
                    headerNamesCommunityCounts.Add("Inactive Student Count");
                }
                else
                {
                    headerNamesCommunityCounts.Add("Classroom Count");
                    headerNamesCommunityCounts.Add("Teacher Count");
                    headerNamesCommunityCounts.Add("Student Count");
                }


                int numCols1 = headerNamesCommunityCounts.Count;
                var columns1 = new Columns();
                for (int col = 0; col < numCols1; col++)
                {
                    int width = headerNamesCommunityCounts[col].Length + 5;
                    Column c = new CustomColumn((UInt32)col + 1,
                                  (UInt32)numCols1 + 1, width);
                    columns1.Append(c);
                }

                #region 填充Community Counts数据
                SheetData sheetData1 = new SheetData();
                int index1 = 1;
                row = new Row { RowIndex = (uint)index1 };
                row.Append(new MediaBICell("0", "Count by Community", index1));
                sheetData1.Append(row);

                index1++;
                row = new Row { RowIndex = (uint)index1 };
                row.Append(new TextCell("0", DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss"), index1));
                sheetData1.Append(row);

                index1 = index1 + 2;
                row = new Row { RowIndex = (uint)index1 };
                row.Append(new MediaBICell("0", "Time Period : " + startDate.Value.ToString("MM/dd/yyyy") + " - " + endDate.Value.ToString("MM/dd/yyyy"), index1));
                sheetData1.Append(row);

                index1 = index1 + 1;
                //Get a list of A to Z
                var az1 = new List<Char>(Enumerable.Range('A', 'Z' -
                                      'A' + 1).Select(i => (Char)i).ToArray());
                //A to E number of columns 
                List<Char> headers1 = az1.GetRange(0, headerNamesCommunityCounts.Count);

                ///记录需要合并的单元格 A4:A11
                List<string> listCells = new List<string>();
                if (fundingModelList.Count > 1)
                    listCells.Add(string.Format("B{0}:{1}{0}", index1, az1[fundingModelList.Count]));

                row = new Row { RowIndex = (uint)index1 };
                row.Append(new EmptyCell());
                row.Append(new HeaderCenterCell("B", "Classroom Funding", index1));
                for (int i = 1; i < fundingModelList.Count; i++)
                    row.Append(new EmptyCell());
                row.Append(new EmptyCell());
                row.Append(new EmptyCell());
                row.Append(new EmptyCell());
                sheetData1.Append(row);

                index1++;
                Row row1 = new Row { RowIndex = (uint)index1 };
                for (int col = 0; col < headerNamesCommunityCounts.Count; col++)
                {
                    HeaderCell cell = new HeaderCell(headers1[col].ToString(), headerNamesCommunityCounts[col], index1);
                    row1.Append(cell);
                }
                sheetData1.AppendChild(row1);


                List<int> communityIdList = list.GroupBy(r => r.CommunityId).Select(r => r.Key).ToList();
                int column = 1;
                foreach (int itemId in communityIdList)
                {
                    index1++;
                    row = new Row { RowIndex = (uint)index1 };
                    CountbyCommunityModel model = list.Find(r => r.CommunityId == itemId);
                    row.Append(new TextCell("A", model.CommunityName, index1));
                    column = 1;
                    foreach (SelectItemModel funding in fundingModelList)
                    {
                        CountbyCommunityModel fundingModel = list.Find(r => r.CommunityId == itemId && r.FundingId == funding.ID);
                        if (fundingModel != null && fundingModel.ClassroomTotal > 0)
                            row.Append(new NumberCell(headers1[column].ToString(), fundingModel.ClassroomTotal.ToString(), index1));
                        else
                            row.Append(new EmptyCell());

                        column++;
                    }
                    int classroomTotal = list.FindAll(r => r.CommunityId == itemId).Sum(r => r.ClassroomTotal);
                    row.Append(new NumberCell(headers1[column].ToString(), classroomTotal.ToString(), index1));
                    column++;
                    row.Append(new NumberCell(headers1[column].ToString(), model.TeacherTotal.ToString(), index1));
                    column++;
                    row.Append(new NumberCell(headers1[column].ToString(), model.StudentTotal.ToString(), index1));
                    sheetData1.Append(row);

                }


                index1++;
                row = new Row { RowIndex = (uint)index1 };
                row.Append(new HeaderCell("A", "Total", index1));
                column = 1;
                foreach (SelectItemModel funding in fundingModelList)
                {
                    int total = list.Where(r => r.FundingId == funding.ID).Sum(r => r.ClassroomTotal);
                    row.Append(new NumberCell(headers1[column].ToString(), total.ToString(), index1));
                    column++;
                }
                row.Append(new NumberCell(headers1[column].ToString(), list.Sum(r => r.ClassroomTotal).ToString(), index1));
                column++;
                row.Append(new NumberCell(headers1[column].ToString(), list.DistinctBy(r => r.CommunityId).Sum(r => r.TeacherTotal).ToString(), index1));
                column++;
                row.Append(new NumberCell(headers1[column].ToString(), list.DistinctBy(r => r.CommunityId).Sum(r => r.StudentTotal).ToString(), index1));
                sheetData1.Append(row);


                worksheetPart1.Worksheet = new Worksheet();
                worksheetPart1.Worksheet.Append(columns1);
                worksheetPart1.Worksheet.Append(sheetData1);

                worksheetPart1.Worksheet.MergeCells(listCells);

                var sheets = myWorkbook.WorkbookPart.Workbook.AppendChild<Sheets>(new Sheets());
                var sheet = new Sheet { Name = "Participation Counts", SheetId = 1, Id = relId };
                sheets.Append(sheet);
                var sheet1 = new Sheet { Name = "Community Counts", SheetId = 2, Id = relId1 };
                sheets.Append(sheet1);
                workbookPart.Workbook.Save();
                myWorkbook.Close();
            }
        }

                #endregion
    }
}
