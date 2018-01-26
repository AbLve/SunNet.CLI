using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Sunnet.Cli.Business.Common;
using Sunnet.Cli.Core.Cec.Models;
using Sunnet.Cli.Core.Cot.Entities;
using Sunnet.Cli.Core.Cot.Models;
using Sunnet.Cli.Core.Cpalls;
using Sunnet.Cli.Core.Reports.Models;
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
        public void COTCECCompletion(out string path)
        {
            path = string.Empty;

            int cotAssessmentId = _adeService.Assessments.Where(r => r.Status == Core.Common.Enums.EntityStatus.Active && r.Type == Core.Ade.AssessmentType.Cot).Select(r => r.ID).FirstOrDefault();
            int cecAssessmentId = _adeService.Assessments.Where(r => r.Status == Core.Common.Enums.EntityStatus.Active && r.Type == Core.Ade.AssessmentType.Cec).Select(r => r.ID).FirstOrDefault();

            List<COTCECCompletionModel> listCOTCECCompletion = _cotService.GetCOTCECCompletion(cotAssessmentId, cecAssessmentId, CommonAgent.SchoolYear);

            List<TeacherMissingCOTModel> listMissingCOT = _cotService.GetTeacherMissingMOYCOT(cotAssessmentId, CommonAgent.SchoolYear);

            List<TeacherMissingCECModel> listMissingCECMOY = _cecService.GetTeacherMissingCEC(cecAssessmentId, CommonAgent.SchoolYear, Wave.MOY);

            List<CECCompletionModel> listCECCompletion = _cecService.GetEOYCECCompletion(cecAssessmentId, CommonAgent.SchoolYear);

            List<TeacherMissingCECModel> listMissingCECEOY = _cecService.GetTeacherMissingCEC(cecAssessmentId, CommonAgent.SchoolYear, Wave.EOY);

            path = CreateReportName("COTCECCompletion");

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

                List<string> headerNameCECCOTCompletion = new List<string>
                {
                    "Coach",
                    "Total",
                    "CEC",
                    "COT"
                };

                #region 填充数据
                SheetData sheetData = new SheetData();
                int index = 1;

                #region //BOY & MOY - COT & CEC Assessment Completion Report
                Row r = new Row { RowIndex = (uint)index };
                r.Append(new HeaderCell("0", "BOY & MOY - COT & CEC Assessment Completion Report", index));
                sheetData.Append(r);

                index = index + 2;
                //Get a list of A to Z
                var az = new List<Char>(Enumerable.Range('A', 'Z' -
                                      'A' + 1).Select(i => (Char)i).ToArray());
                //A to E number of columns 
                List<Char> headers = az.GetRange(0, headerNameCECCOTCompletion.Count);
                Row header = new Row { RowIndex = (uint)index };
                for (int col = 0; col < headerNameCECCOTCompletion.Count; col++)
                {
                    HeaderCell cell = new HeaderCell(headers[col].ToString(), headerNameCECCOTCompletion[col], index);
                    header.Append(cell);
                }
                sheetData.AppendChild(header);

                foreach (COTCECCompletionModel model in listCOTCECCompletion)
                {
                    index++;
                    r = new Row { RowIndex = (uint)index };
                    TextCell cell1 = new TextCell(headers[0].ToString(), string.Format("{0} {1}", model.FirstName, model.LastName), index);
                    r.Append(cell1);

                    NumberCell cell2 = new NumberCell(headers[1].ToString(), model.Total.ToString(), index);
                    r.Append(cell2);

                    NumberCell cell3 = new NumberCell(headers[2].ToString(), model.CECCompletion.ToString(), index);
                    r.Append(cell3);

                    NumberCell cell4 = new NumberCell(headers[3].ToString(), model.COTCompletion.ToString(), index);
                    r.Append(cell4);
                    sheetData.Append(r);
                }
                #endregion

                #region //List of teachers with missing CEC:
                index += 3;

                r = new Row { RowIndex = (uint)index };
                r.Append(new HeaderCell("0", "List of teachers with missing CEC: ", index));
                sheetData.Append(r);

                index++;

                List<string> headerNameCECMOYMissing = new List<string>
                {
                    "Community Name",
                    "School Name",
                    "Teacher Name",
                    "Coach Name",
                    "MOY CEC Complete"
                };

                headers = az.GetRange(0, headerNameCECMOYMissing.Count);
                header = new Row { RowIndex = (uint)index };
                for (int col = 0; col < headerNameCECMOYMissing.Count; col++)
                {
                    HeaderCell cell = new HeaderCell(headers[col].ToString(), headerNameCECMOYMissing[col], index);
                    header.Append(cell);
                }
                sheetData.AppendChild(header);

                foreach (TeacherMissingCECModel model in listMissingCECMOY)
                {
                    index++;
                    r = new Row { RowIndex = (uint)index };
                    TextCell cell1 = new TextCell(headers[0].ToString(), model.CommunityName, index);
                    r.Append(cell1);

                    TextCell cell2 = new TextCell(headers[1].ToString(), model.SchoolName, index);
                    r.Append(cell2);

                    TextCell cell3 = new TextCell(headers[2].ToString(), string.Format("{0} {1}", model.Teacher_FirstName, model.Teacher_LastName), index);
                    r.Append(cell3);

                    TextCell cell4 = new TextCell(headers[3].ToString(), string.Format("{0} {1}", model.Coach_FirstName, model.Coach_LastName), index);
                    r.Append(cell4);

                    TextCell cell5 = new TextCell(headers[4].ToString(), "No", index);
                    r.Append(cell5);

                    sheetData.Append(r);
                }
                #endregion

                #region //List of teachers with missing COT:
                index += 2;

                r = new Row { RowIndex = (uint)index };
                r.Append(new HeaderCell("0", "List of teachers with missing COT: ", index));
                sheetData.Append(r);

                index++;

                List<string> headerNameCOTMOYMissing = new List<string>
                {
                    "Community Name",
                    "School Name",
                    "Teacher Name",
                    "Coach Name",
                    "MOY CEC Complete"
                };

                headers = az.GetRange(0, headerNameCOTMOYMissing.Count);
                header = new Row { RowIndex = (uint)index };
                for (int col = 0; col < headerNameCOTMOYMissing.Count; col++)
                {
                    HeaderCell cell = new HeaderCell(headers[col].ToString(), headerNameCOTMOYMissing[col], index);
                    header.Append(cell);
                }
                sheetData.AppendChild(header);


                foreach(TeacherMissingCOTModel model in listMissingCOT)
                {
                     index++;
                    r = new Row { RowIndex = (uint)index };
                    TextCell cell1 = new TextCell(headers[0].ToString(), model.CommunityName, index);
                    r.Append(cell1);

                    TextCell cell2 = new TextCell(headers[1].ToString(), model.SchoolName, index);
                    r.Append(cell2);

                    TextCell cell3 = new TextCell(headers[2].ToString(), string.Format("{0} {1}", model.Teacher_FirstName, model.Teacher_LastName), index);
                    r.Append(cell3);

                    TextCell cell4 = new TextCell(headers[3].ToString(), string.Format("{0} {1}", model.Coach_FirstName, model.Coach_LastName), index);
                    r.Append(cell4);

                    TextCell cell5 = new TextCell(headers[4].ToString(), "No", index);
                    r.Append(cell5);

                    sheetData.Append(r);
                }
                #endregion

                #region  //EOY CEC Assessment Completion Report
                index += 4;

                r = new Row { RowIndex = (uint)index };
                r.Append(new HeaderCell("0", "EOY CEC Assessment Completion Report ", index));
                sheetData.Append(r);

                index++;

                List<string> headerNameCECCompletion = new List<string>
                {
                    "Coach",
                    "Total",
                    "Completion"
                };

                headers = az.GetRange(0, headerNameCECCompletion.Count);
                header = new Row { RowIndex = (uint)index };
                for (int col = 0; col < headerNameCECCompletion.Count; col++)
                {
                    HeaderCell cell = new HeaderCell(headers[col].ToString(), headerNameCECCompletion[col], index);
                    header.Append(cell);
                }
                sheetData.AppendChild(header);
       
                foreach (CECCompletionModel model in listCECCompletion)
                {
                    index++;
                    r = new Row { RowIndex = (uint)index };
                    TextCell cell1 = new TextCell(headers[0].ToString(), string.Format("{0} {1}", model.FirstName, model.LastName), index);
                    r.Append(cell1);

                    NumberCell cell2 = new NumberCell(headers[1].ToString(), model.Total.ToString(), index);
                    r.Append(cell2);

                    NumberCell cell3 = new NumberCell(headers[2].ToString(), model.Complete.ToString(), index);
                    r.Append(cell3);

                    sheetData.Append(r);
                }
                #endregion


                //List of teachers with missing CEC: 
                index += 2;

                r = new Row { RowIndex = (uint)index };
                r.Append(new HeaderCell("0", "List of teachers with missing COT: ", index));
                sheetData.Append(r);

                index++;

                List<string> headerNameCECEOYMissing = new List<string>
                {
                    "Community Name",
                    "School Name",
                    "Teacher Name",
                    "Coach Name",
                    "EOY CEC Complete"
                };

                headers = az.GetRange(0, headerNameCECEOYMissing.Count);
                header = new Row { RowIndex = (uint)index };
                for (int col = 0; col < headerNameCECEOYMissing.Count; col++)
                {
                    HeaderCell cell = new HeaderCell(headers[col].ToString(), headerNameCECEOYMissing[col], index);
                    header.Append(cell);
                }
                sheetData.AppendChild(header);
          
                foreach (TeacherMissingCECModel model in listMissingCECEOY)
                {
                    index++;
                    r = new Row { RowIndex = (uint)index };
                    TextCell cell1 = new TextCell(headers[0].ToString(), model.CommunityName, index);
                    r.Append(cell1);

                    TextCell cell2 = new TextCell(headers[1].ToString(), model.SchoolName, index);
                    r.Append(cell2);

                    TextCell cell3 = new TextCell(headers[2].ToString(), string.Format("{0} {1}", model.Teacher_FirstName, model.Teacher_LastName), index);
                    r.Append(cell3);

                    TextCell cell4 = new TextCell(headers[3].ToString(), string.Format("{0} {1}", model.Coach_FirstName, model.Coach_LastName), index);
                    r.Append(cell4);

                    TextCell cell5 = new TextCell(headers[4].ToString(), "No", index);
                    r.Append(cell5);

                    sheetData.Append(r);
                }              

                #endregion

                int numCols = headerNameCECEOYMissing.Count;
                var columns = new Columns();
                for (int col = 0; col < numCols; col++)
                {
                    int width = headerNameCECEOYMissing[col].Length + 5;
                    Column c = new CustomColumn((UInt32)col + 1,
                        (UInt32)numCols + 1, width);
                    columns.Append(c);
                }

                worksheetPart.Worksheet = new Worksheet();
                worksheetPart.Worksheet.Append(columns);
                worksheetPart.Worksheet.Append(sheetData);


                var sheets = myWorkbook.WorkbookPart.Workbook.AppendChild<Sheets>(new Sheets());
                var sheet = new Sheet { Name = "COT & CEC Completion", SheetId = 1, Id = relId };
                sheets.Append(sheet);
                workbookPart.Workbook.Save();
                myWorkbook.Close();
            }
        }
    }
}
