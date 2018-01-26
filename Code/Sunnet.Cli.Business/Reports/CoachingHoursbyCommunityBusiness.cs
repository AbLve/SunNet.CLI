using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Drawing.Spreadsheet;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Sunnet.Cli.Business.Reports.Model;
using Sunnet.Cli.Business.Reports.Model.CoachingHoursbyCommunity;
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
        public void CoachingHoursbyCommunity(bool isClIUser, out string path)
        {
            List<CoachingHoursbyCommunityModel> list = new List<CoachingHoursbyCommunityModel>();
            if (isClIUser)
                list = _reportService.GetCoachingHoursbyCommunityModel().OrderBy(r => r.CommunityName).ToList();

            path = CreateReportName("CoachingHoursbyCommunity");

            using (SpreadsheetDocument myWorkbook =
                   SpreadsheetDocument.Create(path,
                   SpreadsheetDocumentType.Workbook))
            {
                WorkbookPart workbookPart = myWorkbook.AddWorkbookPart();
                workbookPart.Workbook = new Workbook();
                var worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                // Create Styles and Insert into Workbook
                var stylesPart = myWorkbook.WorkbookPart.AddNewPart<WorkbookStylesPart>();
                Stylesheet styles = new CoachingHoursbyCommunityStylesheet();
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
                    "     Community         ",
                    "F2F Available Hours",
                    "Rem Available Cycles",
                    "Number of F2F Teachers",
                    "Number of Rem Teachers",
                    "Total Teachers",
                    "Requested Hours",
                    "Unused Hours",
                    "Excess Hours",
                    "Requested Cycles",
                    "Unused Cycles",
                    "Excess Cycles"
                };

                int numCols = headerNames.Count;
                var columns = new Columns();
                for (int col = 0; col < numCols; col++)
                {
                    int width = headerNames[col].Length;
                    Column c = new CustomColumn((UInt32)col + 1,
                                  (UInt32)numCols + 1, width);
                    columns.Append(c);
                }
                #region 填充数据
                SheetData sheetData = new SheetData();
                int index = 1;
                var row = new Row { RowIndex = (uint)index };
                row.Append(new MediaBICell("A", "Coaching Load by Community", index));
                sheetData.Append(row);

                index++;
                row = new Row { RowIndex = (uint)index };
                row.Append(new TextCell("A", DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"), index));
                sheetData.Append(row);

                ///记录需要合并的单元格 A4:A11
                List<string> listCells = new List<string>();

                index = index + 2;
                row = new Row { RowIndex = (uint)index };
                row.Append(new EmptyCell());
                row.Append(new CenterHeaderCell("B", "Coaches", index));
                row.Append(new EmptyCell());
                row.Append(new CenterHeaderCell("D", "Teachers", index));
                row.Append(new EmptyCell());
                row.Append(new EmptyCell());
                row.Append(new F2FHeaderCell("G", "F2F Coaching", index));
                row.Append(new EmptyCell());
                row.Append(new EmptyCell());
                row.Append(new RemoteHeaderCell("J", "Remote Cycles", index));
                row.Append(new EmptyCell());
                row.Append(new EmptyCell());

                sheetData.Append(row);

                listCells.Add(string.Format("B{0}:C{0}", index));
                listCells.Add(string.Format("D{0}:F{0}", index));
                listCells.Add(string.Format("G{0}:I{0}", index));
                listCells.Add(string.Format("J{0}:L{0}", index));

                index++;
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

                decimal totalG = 0;
                decimal totalH = 0;
                decimal totalI = 0;
                decimal totalJ = 0;
                decimal totalK = 0;
                decimal totalL = 0;

                foreach (CoachingHoursbyCommunityModel model in list)
                {
                    index++;
                    row = new Row { RowIndex = (uint)index };
                    row.Append(new HeaderCell("A", model.CommunityName, index));
                    row.Append(new NumberD2CenterCell("B", model.F2FAvailableHours.ToString(), index));
                    row.Append(new NumberD2CenterCell("C", model.RemAvailableCycle.ToString(), index));
                    row.Append(new Sunnet.Cli.Business.Reports.Model.CoachingHoursbyCommunity.NumberCenterCell("D", model.NmberofF2F.ToString(), index));
                    row.Append(new Sunnet.Cli.Business.Reports.Model.CoachingHoursbyCommunity.NumberCenterCell("E", model.NmberofRem.ToString(), index));
                    row.Append(new Sunnet.Cli.Business.Reports.Model.CoachingHoursbyCommunity.NumberCenterCell("F", model.TeacherTotal.ToString(), index));


                    if (model.F2FAvailableHours > 0)
                    {
                        row.Append(new NumberD2CenterCell("G", model.ReqHours.ToString(), index));
                        decimal h = model.F2FAvailableHours - model.ReqHours;
                        if (h < 0) h = 0;
                        row.Append(new NumberD2CenterCell("H", h.ToString(), index));
                        decimal i = model.ReqHours - model.F2FAvailableHours;
                        if (i < 0) i = 0;
                        row.Append(new NumberD2CenterCell("I", i.ToString(), index));

                        totalG += model.ReqHours;
                        totalH += h;
                        totalI += i;
                    }
                    else
                    {
                        if (model.ReqHours > 0)
                        {
                            row.Append(new NumberGreyCell("G", model.ReqHours.ToString(), index));
                            decimal h = model.F2FAvailableHours - model.ReqHours;
                            if (h < 0) h = 0;
                            row.Append(new NumberGreyCell("H", h.ToString(), index));
                            decimal i = model.ReqHours - model.F2FAvailableHours;
                            if (i < 0) i = 0;
                            row.Append(new NumberGreyCell("I", i.ToString(), index));
                            totalG += model.ReqHours;
                            totalH += h;
                            totalI += i;
                        }
                        else
                        {
                            row.Append(new EmptyFillCell());
                            row.Append(new EmptyFillCell());
                            row.Append(new EmptyFillCell());
                        }
                    }

                    if (model.RemAvailableCycle > 0)
                    {
                        row.Append(new NumberD2CenterCell("J", model.ReqCycles.ToString(), index));
                        decimal k = model.RemAvailableCycle - model.ReqCycles;
                        if (k < 0) k = 0;
                        row.Append(new NumberD2CenterCell("K", k.ToString(), index));
                        decimal L = model.ReqCycles - model.RemAvailableCycle;
                        if (L < 0) L = 0;
                        row.Append(new NumberD2CenterCell("L", L.ToString(), index));

                        totalJ += model.ReqCycles;
                        totalK += k;
                        totalL += L;
                    }
                    else
                    {
                        if (model.ReqCycles > 0)
                        {
                            row.Append(new NumberGreyCell("J", model.ReqCycles.ToString(), index));
                            decimal k = model.RemAvailableCycle - model.ReqCycles;
                            if (k < 0) k = 0;
                            row.Append(new NumberGreyCell("K", k.ToString(), index));
                            decimal L = model.ReqCycles - model.RemAvailableCycle;
                            if (L < 0) L = 0;
                            row.Append(new NumberGreyCell("L", L.ToString(), index));

                            totalJ += model.ReqCycles;
                            totalK += k;
                            totalL += L;
                        }
                        else
                        {
                            row.Append(new EmptyFillCell());
                            row.Append(new EmptyFillCell());
                            row.Append(new EmptyFillCell());
                        }
                    }
                    sheetData.Append(row);
                }
                index++;
                row = new Row { RowIndex = (uint)index };
                row.Append(new HeaderCell("A", "Grand Totals", index));
                row.Append(new NumberD2CenterCell("B", list.Sum(r => r.F2FAvailableHours).ToString(), index));
                row.Append(new NumberD2CenterCell("C", list.Sum(r => r.RemAvailableCycle).ToString(), index));
                row.Append(new Sunnet.Cli.Business.Reports.Model.CoachingHoursbyCommunity.NumberCenterCell("D", list.Sum(r => r.NmberofF2F).ToString(), index));
                row.Append(new Sunnet.Cli.Business.Reports.Model.CoachingHoursbyCommunity.NumberCenterCell("E", list.Sum(r => r.NmberofRem).ToString(), index));
                row.Append(new Sunnet.Cli.Business.Reports.Model.CoachingHoursbyCommunity.NumberCenterCell("F", list.Sum(r => r.TeacherTotal).ToString(), index));
                row.Append(new NumberD2CenterCell("G", totalG.ToString(), index));
                row.Append(new NumberD2CenterCell("H", totalH.ToString(), index));
                row.Append(new NumberD2CenterCell("I", totalI.ToString(), index));
                row.Append(new NumberD2CenterCell("J", totalJ.ToString(), index));
                row.Append(new NumberD2CenterCell("K", totalK.ToString(), index));
                row.Append(new NumberD2CenterCell("L", totalL.ToString(), index));

                sheetData.Append(row);
                #endregion


                worksheetPart.Worksheet = new Worksheet();
                worksheetPart.Worksheet.Append(columns);
                worksheetPart.Worksheet.Append(sheetData);

                worksheetPart.Worksheet.MergeCells(listCells);

                var sheets = myWorkbook.WorkbookPart.Workbook.AppendChild<Sheets>(new Sheets());
                var sheet = new Sheet { Name = "Sheet1", SheetId = 1, Id = relId };
                sheets.Append(sheet);
                workbookPart.Workbook.Save();
                myWorkbook.Close();
            }
        }
    }
}
