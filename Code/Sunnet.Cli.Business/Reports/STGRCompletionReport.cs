using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Drawing.Spreadsheet;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Sunnet.Cli.Core.Cot.Models;
using Sunnet.Cli.Core.Reports.Entities;
using Sunnet.Cli.Core.Reports.Models;
using Sunnet.Cli.Core.Users.Entities;
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
        public void STGRCompletion(out string path)
        {
            int cotAssessmentId = _adeService.Assessments.Where(r => r.Status == Core.Common.Enums.EntityStatus.Active && r.Type == Core.Ade.AssessmentType.Cot).Select(r => r.ID).FirstOrDefault();

            List<STGR_CompletionModel> listSTRGCompletion = _cotService.GetSTGR_Completion_Report(cotAssessmentId);

            List<Community_Mentor_TeacherModel> listMentorTeacher = _reportService.GetCommunity_Mentor_Teachers();

            path = CreateReportName("STGRCompletion");

            using (SpreadsheetDocument myWorkbook =
                   SpreadsheetDocument.Create(path,
                   SpreadsheetDocumentType.Workbook))
            {
                WorkbookPart workbookPart = myWorkbook.AddWorkbookPart();
                workbookPart.Workbook = new Workbook();
                var worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                // Create Styles and Insert into Workbook
                var stylesPart = myWorkbook.WorkbookPart.AddNewPart<WorkbookStylesPart>();
                Stylesheet styles = new AlignmentCustomStylesheet();
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
                    "Project Manager",
                    "Community Name",
                    "    Mentor     ",
                    "    No        ",
                    "   Yes      ",
                    "   Total    ",
                    "% Yes"
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
                var r1 = new Row { RowIndex = (uint)index };
                r1.Append(new HeaderCell("0", "STGR Completion", index));
                sheetData.Append(r1);

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

                var pmList = listMentorTeacher.GroupBy(r => new { r.PMID, r.PM_FirstName, r.PM_LastName }).OrderBy(r => r.Key.PM_FirstName).ThenBy(r => r.Key.PM_LastName).ToList();

                ///记录需要合并的单元格 A4:A11
                List<string> listCells = new List<string>();

                foreach (var v in pmList)
                {
                    int pmTotal = listMentorTeacher.Count(r => r.PMID == v.Key.PMID);
                    List<Community_Mentor_TeacherModel> communityList = listMentorTeacher.FindAll(r => r.PMID == v.Key.PMID);
                    bool onePm = true;
                    listCells.Add(string.Format("A{0}:A{1}", index + 1, index + pmTotal));

                    var communityGroups = communityList.GroupBy(r => new { r.CommunityID, r.CommunityName }).OrderBy(r => r.Key.CommunityName).ToList();
                    foreach (var c in communityGroups)
                    {
                        int cTotal = listMentorTeacher.Count(r => r.PMID == v.Key.PMID && r.CommunityID == c.Key.CommunityID);

                        bool oneCommunity = true;
                        listCells.Add(string.Format("B{0}:B{1}", index + 1, index + cTotal));

                        foreach (Community_Mentor_TeacherModel model in
                            listMentorTeacher.FindAll(r => r.PMID == v.Key.PMID && r.CommunityID == c.Key.CommunityID).OrderBy(r => r.Coach_FirstName).ToList())
                        {
                            index++;
                            var r = new Row { RowIndex = (uint)index };
                            if (onePm)
                            {
                                VerticalTopTextCell cell1 = new VerticalTopTextCell(headers[0].ToString(), string.Format("{0} {1}", model.PM_FirstName, model.PM_LastName), index);

                                r.Append(cell1);
                                onePm = false;
                            }
                            else
                            {
                                r.Append(new EmptyCell());
                            }

                            if (oneCommunity)
                            {
                                VerticalTopTextCell cell2 = new VerticalTopTextCell(headers[1].ToString(), model.CommunityName, index);
                                r.Append(cell2);
                                oneCommunity = false;
                            }
                            else
                            {
                                r.Append(new EmptyCell());
                            }

                            TextCell cell3 = new TextCell(headers[2].ToString(), string.Format("{0} {1}", model.Coach_FirstName, model.Coach_lastName), index);
                            r.Append(cell3);

                            int Total = model.Total;
                            STGR_CompletionModel completionModel = listSTRGCompletion.Find(n => n.CommunityId == model.CommunityID && n.CoachId == model.CoachId);
                            int Yes = 0;
                            if (completionModel != null)
                                Yes = completionModel.Total;

                            NumberCell cell4 = new NumberCell(headers[3].ToString(), (Total - Yes).ToString(), index);
                            r.Append(cell4);

                            NumberCell cell5 = new NumberCell(headers[4].ToString(), Yes.ToString(), index);
                            r.Append(cell5);

                            NumberCell cell6 = new NumberCell(headers[5].ToString(), model.Total.ToString(), index);
                            r.Append(cell6);

                            TextCell cell7 = new TextCell(headers[6].ToString(), (Yes / (model.Total * 1.00) * 100.00).ToString("N") + "%", index);
                            r.Append(cell7);

                            sheetData.Append(r);
                        }
                    }
                }

                #endregion

                //list of teachers with missing STGR:
                index += 3;
                var row = new Row { RowIndex = (uint)index };
                TextCell missingCell = new TextCell(headers[0].ToString(), "list of teachers with missing STGR", index);
                row.Append(missingCell);
                sheetData.Append(row);
                index += 1;

                List<string> headerNameMissingSTGR = new List<string>
                {
                    "Community Name",
                    "Project Manager",
                    "School Name",
                    "Teacher First Name",
                    "Teacher Last Name",
                    "Mentor",
                    "STGR?"
                };


                //A to E number of columns 
                headers = az.GetRange(0, headerNameMissingSTGR.Count);
                header = new Row { RowIndex = (uint)index };
                for (int col = 0; col < headerNameMissingSTGR.Count; col++)
                {
                    HeaderCell cell = new HeaderCell(headers[col].ToString(), headerNameMissingSTGR[col], index);
                    header.Append(cell);
                }
                sheetData.AppendChild(header);

                List<TeacherMissingSTGRModel> missingSTRGList = _cotService.GetTeacher_Missing_STGR(cotAssessmentId);
                foreach (TeacherMissingSTGRModel model in missingSTRGList)
                {
                    index++;
                    row = new Row { RowIndex = (uint)index };
                    VerticalTopTextCell cell1 = new VerticalTopTextCell(headerNameMissingSTGR[0].ToString(), model.CommunityName, index);
                    row.Append(cell1);

                    TextCell cell2 = new TextCell(headers[1].ToString(), model.PM, index);
                    row.Append(cell2);

                    TextCell cell3 = new TextCell(headers[2].ToString(), model.SchoolName, index);
                    row.Append(cell3);

                    TextCell cell4 = new TextCell(headers[3].ToString(), model.TeacherFirstName, index);
                    row.Append(cell4);

                    TextCell cell5 = new TextCell(headers[4].ToString(), model.TeacherLastName, index);
                    row.Append(cell5);

                    TextCell cell6 = new TextCell(headers[5].ToString(), string.Format("{0} {1}", model.CoachFirstName, model.CoachLastName), index);
                    row.Append(cell6);

                    TextCell cell7 = new TextCell(headers[6].ToString(), "No", index);
                    row.Append(cell7);

                    sheetData.Append(row);
                }


                worksheetPart.Worksheet = new Worksheet();
                worksheetPart.Worksheet.Append(columns);
                worksheetPart.Worksheet.Append(sheetData);

                worksheetPart.Worksheet.MergeCells(listCells);

                var sheets = myWorkbook.WorkbookPart.Workbook.AppendChild<Sheets>(new Sheets());
                var sheet = new Sheet { Name = "STGR", SheetId = 1, Id = relId };
                sheets.Append(sheet);
                workbookPart.Workbook.Save();
                myWorkbook.Close();
            }
        }



        public class AlignmentCustomStylesheet : CustomStylesheet
        {
            public override void AddCellFormats(CellFormats cellFormats)
            {   // index 11
                // 合并单元条顶部对齐
                var cellFormat10 = new CellFormat
                {
                    NumberFormatId = 4,
                    FontId = 0,
                    FillId = 0,
                    BorderId = 1,
                    FormatId = 0,
                    ApplyNumberFormat = BooleanValue.FromBoolean(true),
                    Alignment = new Alignment() { Vertical = VerticalAlignmentValues.Top }
                };
                cellFormats.Append(cellFormat10);
            }
        }

        class VerticalTopTextCell : Cell
        {
            public VerticalTopTextCell(string header, string text, int index)
            {
                this.DataType = CellValues.InlineString;
                this.CellReference = header + index;
                //Add text to the text cell.
                this.InlineString = new InlineString { Text = new DocumentFormat.OpenXml.Spreadsheet.Text { Text = text } };
                this.StyleIndex = 11;
            }
        }
    }
}