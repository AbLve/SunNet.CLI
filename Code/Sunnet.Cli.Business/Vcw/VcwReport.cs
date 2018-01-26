using Sunnet.Cli.Business.Vcw.Models;
using Sunnet.Cli.Core;
using Sunnet.Cli.Core.Vcw;
using Sunnet.Cli.Core.Vcw.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using LinqKit;
using System.Reflection;
using System.IO;
using Sunnet.Framework;
using Sunnet.Framework.Log;
using StructureMap;
using Sunnet.Framework.Extensions;
using System.Globalization;

using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using System.Data;
using Sunnet.Framework.Excel;
using Sunnet.Cli.Business.Schools;
using Sunnet.Cli.Business.Common;
using Sunnet.Cli.Business.Users;
using Sunnet.Cli.Core.Vcw.Enums;
using Sunnet.Cli.Business.Common.Enum;
using Sunnet.Cli.Core.Common.Enums;

namespace Sunnet.Cli.Business.Vcw
{
    public class VcwReport
    {
        private readonly IVcwContract _server;
        private readonly ISunnetLog _logger;
        SchoolBusiness _schoolBusiness;
        UserBusiness _userBusiness;
        VcwBusiness _vcwBusiness;
        VCW_MasterDataBusiness _vcwMasterDataBusiness;

        public VcwReport(VCWUnitOfWorkContext unit = null)
        {
            _server = DomainFacade.CreateVcwService(unit);
            _logger = ObjectFactory.GetInstance<ISunnetLog>();
            _schoolBusiness = new SchoolBusiness();
            _userBusiness = new UserBusiness();
            _vcwBusiness = new VcwBusiness();
            _vcwMasterDataBusiness = new VCW_MasterDataBusiness();
        }

        public string TeacherSummaryToExcel(Expression<Func<Vcw_FileEntity, bool>> fileContition)
        {
            List<FileListModel> list = _server.Files.AsExpandable().Where(fileContition).Select(r => new FileListModel()
            {
                ID = r.ID,
                ContentOther = r.ContentOther,
                ContextId = r.ContextId,
                ContextOther = r.ContextOther,
                ContextText = r.Context == null ? "" : (r.Context.Status == EntityStatus.Active ? r.Context.Name : ""),
                DateRecorded = r.DateRecorded,
                FileName = r.FileName,
                FilePath = r.FilePath,
                Status = r.Status,
                UpdatedOn = r.UpdatedOn,
                UploadDate = r.UploadDate,
                VideoType = r.VideoType,
                IdentifyFileName = r.IdentifyFileName
            }).OrderByDescending(r => r.ID).ToList();


            List<string> headerNames =
               new List<string> {"File Name","ID", "Video Type", 
                   "Upload Date", "Date Recorded", 
                   "Context", "File Type","Status" };

            string path = Path.Combine(SFConfig.ProtectedFiles, "Teacher_Summary_" + DateTime.Now.Ticks + "_" + Guid.NewGuid() + ".xlsx").Replace("/", "\\");


            //Open the copied template workbook. 
            using (SpreadsheetDocument myWorkbook =
                   SpreadsheetDocument.Create(path,
                   SpreadsheetDocumentType.Workbook))
            {
                WorkbookPart workbookPart = myWorkbook.AddWorkbookPart();
                workbookPart.Workbook = new Workbook();

                var worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                // Create Styles and Insert into Workbook
                var stylesPart =
                    myWorkbook.WorkbookPart.AddNewPart<WorkbookStylesPart>();
                Stylesheet styles = new CustomStylesheet();
                styles.Save(stylesPart);
                string relId = workbookPart.GetIdOfPart(worksheetPart);
                var fileVersion =
                new FileVersion
                {
                    ApplicationName =
                        "Microsoft Office Excel"
                };
                var worksheet = new Worksheet();
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


                #region 填充数据
                SheetData sheetData = new SheetData();
                int index = 1;
                //Get a list of A to Z
                var az = new List<Char>(Enumerable.Range('A', 'Z' -
                                      'A' + 1).Select(i => (Char)i).ToArray());
                //A to E number of columns 
                List<Char> headers = az.GetRange(0, headerNames.Count);
                Row header = new Row();
                for (int col = 0; col < headerNames.Count; col++)
                {
                    HeaderCell cell = new HeaderCell(headers[col].ToString(), headerNames[col], index);
                    header.Append(cell);
                }
                sheetData.AppendChild(header);

                //Row headerRow = new Row();
                //foreach (string  s in headerNames)
                //{
                //    Cell cell = new Cell();
                //    cell.DataType = CellValues.String;
                //    cell.CellValue = new CellValue(s);
                //    headerRow.AppendChild(cell);
                //}
                //sheetData.AppendChild(headerRow);
                for (int i = 0; i < numRows; i++)
                {
                    FileListModel model = list[i];
                    index++;
                    var obj1 = list[i];
                    var r = new Row { RowIndex = (uint)index };

                    TextCell cell0 = new TextCell(headers[0].ToString(), model.IdentifyFileName, index);
                    r.Append(cell0);

                    TextCell cell1 = new TextCell(headers[1].ToString(), model.Number, index);
                    r.Append(cell1);

                    TextCell cell2 = new TextCell(headers[2].ToString(), model.VideoType.ToDescription(), index);
                    r.Append(cell2);

                    if (model.UploadDate == CommonAgent.MinDate)
                    {
                        TextCell cell3 = new TextCell(headers[3].ToString(), "", index);
                        r.Append(cell3);
                    }
                    else
                    {
                        DateCell cell3 = new DateCell(headers[3].ToString(), model.UploadDate, index);
                        r.Append(cell3);
                    }

                    if (model.DateRecorded == CommonAgent.MinDate)
                    {
                        TextCell cell4 = new TextCell(headers[4].ToString(), "", index);
                        r.Append(cell4);
                    }
                    else
                    {
                        DateCell cell4 = new DateCell(headers[4].ToString(), model.DateRecorded, index);
                        r.Append(cell4);
                    }

                    TextCell cell5 = new TextCell(headers[5].ToString(), model.Context, index);
                    r.Append(cell5);

                    TextCell cell6 = new TextCell(headers[6].ToString(), model.FileExtension, index);
                    r.Append(cell6);

                    TextCell cell7 = new TextCell(headers[7].ToString(), model.Status.ToDescription() == "0" ? "" : model.Status.ToDescription(), index);
                    r.Append(cell7);

                    sheetData.Append(r);
                }



                #endregion
                worksheetPart.Worksheet = new Worksheet();
                worksheetPart.Worksheet.Append(columns);
                worksheetPart.Worksheet.Append(sheetData);


                var sheets = myWorkbook.WorkbookPart.Workbook.AppendChild<Sheets>(new Sheets());
                var sheet = new Sheet { Name = "Teacher Summary", SheetId = 1, Id = relId };
                sheets.Append(sheet);
                //myWorkbook.WorkbookPart.Workbook.Append(fileVersion);
                //workbookPart.Workbook.Append(fileVersion);

                workbookPart.Workbook.Save();
                myWorkbook.Close();
            }









            //Console.WriteLine("Completed");
            #region 合并单元格
            //using (SpreadsheetDocument document = SpreadsheetDocument.Open(path, true))
            //{
            //    IEnumerable<Sheet> sheets = document.WorkbookPart.Workbook.Descendants<Sheet>().Where(s => s.Name == "Sheet1");
            //    WorksheetPart worksheetPart = (WorksheetPart)document.WorkbookPart.GetPartById(sheets.First().Id);
            //    Worksheet worksheet = worksheetPart.Worksheet;
            //    Cell cell = new Cell();

            //    MergeCells mergeCells;
            //    if (worksheet.Elements<MergeCells>().Count() > 0)
            //    {
            //        mergeCells = worksheet.Elements<MergeCells>().First();
            //    }
            //    else
            //    {
            //    mergeCells = new MergeCells();

            //        // Insert a MergeCells object into the specified position.
            //        if (worksheet.Elements<CustomSheetView>().Count() > 0)
            //        {
            //            worksheet.InsertAfter(mergeCells, worksheet.Elements<CustomSheetView>().First());
            //        }
            //        else if (worksheet.Elements<DataConsolidate>().Count() > 0)
            //        {
            //            worksheet.InsertAfter(mergeCells, worksheet.Elements<DataConsolidate>().First());
            //        }
            //        else if (worksheet.Elements<SortState>().Count() > 0)
            //        {
            //            worksheet.InsertAfter(mergeCells, worksheet.Elements<SortState>().First());
            //        }
            //        else if (worksheet.Elements<AutoFilter>().Count() > 0)
            //        {
            //            worksheet.InsertAfter(mergeCells, worksheet.Elements<AutoFilter>().First());
            //        }
            //        else if (worksheet.Elements<Scenarios>().Count() > 0)
            //        {
            //            worksheet.InsertAfter(mergeCells, worksheet.Elements<Scenarios>().First());
            //        }
            //        else if (worksheet.Elements<ProtectedRanges>().Count() > 0)
            //        {
            //            worksheet.InsertAfter(mergeCells, worksheet.Elements<ProtectedRanges>().First());
            //        }
            //        else if (worksheet.Elements<SheetProtection>().Count() > 0)
            //        {
            //            worksheet.InsertAfter(mergeCells, worksheet.Elements<SheetProtection>().First());
            //        }
            //        else if (worksheet.Elements<SheetCalculationProperties>().Count() > 0)
            //        {
            //    worksheet.InsertAfter(mergeCells, worksheet.Elements<SheetCalculationProperties>().First());
            //        }
            //        else
            //        {
            //    worksheet.InsertAfter(mergeCells, worksheet.Elements<SheetData>().First());
            //        }
            //    }

            //     Create the merged cell and append it to the MergeCells collection.
            //    MergeCell mergeCell = new MergeCell() { Reference = new StringValue("B2:C2") };
            //    mergeCells.Append(mergeCell);

            //}
            #endregion

            //FileStream stream = new FileStream(path, FileMode.Create);

            //document.Save(stream);
            //stream.Close();

            #region 3
            //// Create a spreadsheet document by supplying the filepath.
            //// By default, AutoSave = true, Editable = true, and Type = xlsx.
            //SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.Create(path, SpreadsheetDocumentType.Workbook);

            //// Add a WorkbookPart to the document.
            //WorkbookPart workbookpart = spreadsheetDocument.AddWorkbookPart();
            //workbookpart.Workbook = new Workbook();

            //// Add a WorksheetPart to the WorkbookPart.
            //WorksheetPart worksheetPart = workbookpart.AddNewPart<WorksheetPart>();
            //worksheetPart.Worksheet = new Worksheet(new SheetData());

            //// Add Sheets to the Workbook.
            //Sheets sheets = spreadsheetDocument.WorkbookPart.Workbook.AppendChild<Sheets>(new Sheets());

            //// Append a new worksheet and associate it with the workbook.
            //Sheet sheet = new Sheet() { Id = spreadsheetDocument.WorkbookPart.GetIdOfPart(worksheetPart), SheetId = 1, Name = "mySheet" };
            //sheets.Append(sheet);

            //workbookpart.Workbook.Save();

            //// Close the document.
            //spreadsheetDocument.Close();
            #endregion



            return path;


        }


        void test()
        {
            // Create Spreadsheet document and Wordbook
            SpreadsheetDocument spreadsheetDocument =
            SpreadsheetDocument.Create
("datetime.xlsx", SpreadsheetDocumentType.Workbook);
            WorkbookPart workbookpart = spreadsheetDocument.AddWorkbookPart();
            workbookpart.Workbook = new Workbook();
            // Worksheet
            WorksheetPart worksheetPart = workbookpart.AddNewPart<WorksheetPart>();
            SheetData sheetData = new SheetData();
            worksheetPart.Worksheet = new Worksheet();
            Sheets sheets = spreadsheetDocument.WorkbookPart.Workbook.AppendChild<Sheets>(new Sheets());
            Sheet sheet = new Sheet() { Id = spreadsheetDocument.WorkbookPart.GetIdOfPart(worksheetPart), SheetId = 1, Name = "datedemo" };
            sheets.Append(sheet);
            // Columns and Rows
            Columns columns = new Columns();
            Column column = new Column() { Min = (UInt32Value)1U, Max = (UInt32Value)1U, Width = 18D };
            columns.Append(column);
            // First Row "header"
            Row row1 = new Row();
            Cell cell1 = new Cell() { StyleIndex = (UInt32Value)0U, DataType = CellValues.InlineString };
            InlineString inlineString = new InlineString();
            Text text = new Text();
            text.Text = "date"; // <- text
            inlineString.Append(text);
            cell1.Append(inlineString);
            row1.Append(cell1);
            // Second Row "Date 1"
            Row row2 = new Row();
            Cell cell2 = new Cell() { StyleIndex = (UInt32Value)1U };
            CellValue cellValue2 = new CellValue();
            DateTime dt2 = new DateTime(2011, 3, 13);
            cellValue2.Text = dt2.ToOADate().ToString(); // <- Date Value
            cell2.Append(cellValue2);
            row2.Append(cell2);
            // Third Row "Date 2"
            Row row3 = new Row();
            Cell cell3 = new Cell() { StyleIndex = (UInt32Value)1U };
            CellValue cellValue3 = new CellValue();
            DateTime dt3 = new DateTime(2012, 3, 17);
            cellValue3.Text = dt3.ToOADate().ToString(); // <- Date Value
            cell3.Append(cellValue3);
            row3.Append(cell3);
            // Add Rows/Columns to worksheet
            sheetData.Append(row1);
            sheetData.Append(row2);
            sheetData.Append(row3);
            worksheetPart.Worksheet.Append(columns);
            worksheetPart.Worksheet.Append(sheetData);
            // Styles
            WorkbookStylesPart workbookStylesPart = workbookpart.AddNewPart<WorkbookStylesPart>("rId3");
            Stylesheet stylesheet = new Stylesheet();
            //  Date Time Display Format when s="1" is applied to cell
            NumberingFormats numberingFormats = new NumberingFormats() { Count = (UInt32Value)1U };
            NumberingFormat numberingFormat = new NumberingFormat() { NumberFormatId = (UInt32Value)164U, FormatCode = "[$-409]mmmm\\ d\\,\\ yyyy;@" };
            numberingFormats.Append(numberingFormat);
            // Cell font
            Fonts fonts = new Fonts() { Count = (UInt32Value)1U };
            Font font = new Font();
            FontSize fontSize = new FontSize() { Val = 11D };
            FontName fontName = new FontName() { Val = "Calibri" };
            font.Append(fontSize);
            font.Append(fontName);
            fonts.Append(font);
            // empty, but mandatory
            Fills fills = new Fills() { Count = (UInt32Value)1U };
            Fill fill = new Fill();
            fills.Append(fill);
            Borders borders = new Borders() { Count = (UInt32Value)1U };
            Border border = new Border();
            borders.Append(border);
            // cellFormat1 for text cell cellFormat2 for Datetime cell 
            CellFormats cellFormats = new CellFormats() { Count = (UInt32Value)2U };
            CellFormat cellFormat1 = new CellFormat() { FontId = (UInt32Value)0U };
            CellFormat cellFormat2 = new CellFormat() { NumberFormatId = (UInt32Value)164U, FontId = (UInt32Value)0U, ApplyNumberFormat = true };
            cellFormats.Append(cellFormat1);
            cellFormats.Append(cellFormat2);
            // Save as styles
            stylesheet.Append(numberingFormats);
            stylesheet.Append(fonts);
            stylesheet.Append(fills);
            stylesheet.Append(borders);
            stylesheet.Append(cellFormats);
            workbookStylesPart.Stylesheet = stylesheet;
            // Save all
            workbookpart.Workbook.Save();
            spreadsheetDocument.Close();
        }



        public string TeacherAssignmentToExcel(List<AssignmentListModel> list, bool isPM = false)
        {           
            List<string> headerNames =
               new List<string> { 
                   "Community", "School", "Teacher", "Sender",
                   "Due Date", "Call Date","Session","Content","Upload Type","Status" };

            string path = "";

            if (isPM)
            {
                path = Path.Combine(SFConfig.ProtectedFiles, "PM_Teacher_Assignment_" + DateTime.Now.Ticks + "_" + Guid.NewGuid() + ".xlsx").Replace("/", "\\");
            }
            else
            {
                path = Path.Combine(SFConfig.ProtectedFiles, "Coach_Teacher_Assignment_" + DateTime.Now.Ticks + "_" + Guid.NewGuid() + ".xlsx").Replace("/", "\\");
            }

            //Open the copied template workbook. 
            using (SpreadsheetDocument myWorkbook =
                   SpreadsheetDocument.Create(path,
                   SpreadsheetDocumentType.Workbook))
            {
                WorkbookPart workbookPart = myWorkbook.AddWorkbookPart();
                workbookPart.Workbook = new Workbook();

                var worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                // Create Styles and Insert into Workbook
                var stylesPart =
                    myWorkbook.WorkbookPart.AddNewPart<WorkbookStylesPart>();
                Stylesheet styles = new CustomStylesheet();
                styles.Save(stylesPart);
                string relId = workbookPart.GetIdOfPart(worksheetPart);
                var fileVersion =
                new FileVersion
                {
                    ApplicationName =
                        "Microsoft Office Excel"
                };
                var worksheet = new Worksheet();
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


                #region 填充数据
                SheetData sheetData = new SheetData();
                int index = 1;
                //Get a list of A to Z
                var az = new List<Char>(Enumerable.Range('A', 'Z' -
                                      'A' + 1).Select(i => (Char)i).ToArray());
                //A to E number of columns 
                List<Char> headers = az.GetRange(0, headerNames.Count);
                Row header = new Row();
                for (int col = 0; col < headerNames.Count; col++)
                {
                    HeaderCell cell = new HeaderCell(headers[col].ToString(), headerNames[col], index);
                    header.Append(cell);
                }
                sheetData.AppendChild(header);


                for (int i = 0; i < numRows; i++)
                {
                    AssignmentListModel model = list[i];
                    index++;
                    var obj1 = list[i];
                    var r = new Row { RowIndex = (uint)index };
                    TextCell cell0 = new TextCell(headers[0].ToString(), model.CommunityName, index);
                    r.Append(cell0);

                    TextCell cell1 = new TextCell(headers[1].ToString(), model.SchoolName, index);
                    r.Append(cell1);

                    TextCell cell2 = new TextCell(headers[2].ToString(), model.TeacherName, index);
                    r.Append(cell2);

                    TextCell cell3 = new TextCell(headers[3].ToString(), model.SendUserName, index);
                    r.Append(cell3);

                    if (model.DueDate == CommonAgent.MinDate)
                    {
                        TextCell cell4 = new TextCell(headers[4].ToString(), "", index);
                    }
                    else
                    {
                        DateCell cell4 = new DateCell(headers[4].ToString(), model.DueDate, index);
                        r.Append(cell4);
                    }

                    if (model.FeedbackCalllDate == CommonAgent.MinDate)
                    {
                        TextCell cell5 = new TextCell(headers[5].ToString(), "", index);
                        r.Append(cell5);
                    }
                    else
                    {
                        DateCell cell5 = new DateCell(headers[5].ToString(), model.FeedbackCalllDate, index);
                        r.Append(cell5);
                    }


                    TextCell cell6 = new TextCell(headers[6].ToString(), model.SessionText, index);
                    r.Append(cell6);

                    TextCell cell7 = new TextCell(headers[7].ToString(), model.Content, index);
                    r.Append(cell7);

                    TextCell cell8 = new TextCell(headers[8].ToString(), model.UploadType, index);
                    r.Append(cell8);

                    TextCell cell9 = new TextCell(headers[9].ToString(), model.Status.ToDescription() == "0" ? "" : model.Status.ToDescription(), index);
                    r.Append(cell9);

                    sheetData.Append(r);
                }

                #endregion
                worksheetPart.Worksheet = new Worksheet();
                worksheetPart.Worksheet.Append(columns);
                worksheetPart.Worksheet.Append(sheetData);

                var sheets = myWorkbook.WorkbookPart.Workbook.AppendChild<Sheets>(new Sheets());
                var sheet = new Sheet();
                if (isPM)
                {
                    sheet = new Sheet { Name = "PM Teacher Assignment", SheetId = 1, Id = relId };
                }
                else
                {
                    sheet = new Sheet { Name = "Coach Teacher Assignment", SheetId = 1, Id = relId };
                }

                sheets.Append(sheet);

                workbookPart.Workbook.Save();
                myWorkbook.Close();
            }

            return path;
        }


        public string CoachFilesToExcel(Expression<Func<Vcw_FileEntity, bool>> Contition)
        {

            List<FileListModel> list = _server.Files.AsExpandable().Where(Contition).Select(r => new FileListModel()
            {
                ID = r.ID,
                ContentOther = r.ContentOther,
                ContextId = r.ContextId,
                ContextOther = r.ContextOther,
                ContextText = r.Context == null ? "" : (r.Context.Status == EntityStatus.Active ? r.Context.Name : ""),
                DateRecorded = r.DateRecorded,
                FileName = r.FileName,
                FilePath = r.FilePath,
                Status = r.Status,
                UpdatedOn = r.UpdatedOn,
                UploadDate = r.UploadDate,
                VideoType = r.VideoType,
                StrategyOther = r.StrategyOther,
                IdentifyFileName = r.IdentifyFileName,
                ContentIds = r.FileContents.Select(c => c.ContentId),
                StrategyIds = r.FileStrategies.Select(c => c.StrategyId)
            }).OrderByDescending(r => r.ID).ToList();

            if (list.Count > 0)
            {
                IEnumerable<SelectItemModel> Contents = _vcwMasterDataBusiness.GetActiveVideo_Content_Datas();
                List<SelectItemModel> Strategies = _vcwMasterDataBusiness.GetActiveCoachingStrategy_Datas();

                foreach (FileListModel item in list)
                {
                    item.Contents = Contents.Where(r => item.ContentIds.Contains(r.ID));
                    item.Strategies = Strategies.Where(r => item.StrategyIds.Contains(r.ID));
                }
            }

            List<string> headerNames =
               new List<string> { 
                   "File Name","ID",
                   "Video Type", "Upload Date", "Date Recorded", 
                   "Strategies", "Context","Content","File Type","Status" };

            string path = Path.Combine(SFConfig.ProtectedFiles, "CoachSummary_CoachFiles_" + DateTime.Now.Ticks + "_" + Guid.NewGuid() + ".xlsx").Replace("/", "\\");


            //Open the copied template workbook. 
            using (SpreadsheetDocument myWorkbook =
                   SpreadsheetDocument.Create(path,
                   SpreadsheetDocumentType.Workbook))
            {
                WorkbookPart workbookPart = myWorkbook.AddWorkbookPart();
                workbookPart.Workbook = new Workbook();

                var worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                // Create Styles and Insert into Workbook
                var stylesPart =
                    myWorkbook.WorkbookPart.AddNewPart<WorkbookStylesPart>();
                Stylesheet styles = new CustomStylesheet();
                styles.Save(stylesPart);
                string relId = workbookPart.GetIdOfPart(worksheetPart);
                var fileVersion =
                new FileVersion
                {
                    ApplicationName =
                        "Microsoft Office Excel"
                };
                var worksheet = new Worksheet();
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


                #region 填充数据
                SheetData sheetData = new SheetData();
                int index = 1;
                //Get a list of A to Z
                var az = new List<Char>(Enumerable.Range('A', 'Z' -
                                      'A' + 1).Select(i => (Char)i).ToArray());
                //A to E number of columns 
                List<Char> headers = az.GetRange(0, headerNames.Count);
                Row header = new Row();
                for (int col = 0; col < headerNames.Count; col++)
                {
                    HeaderCell cell = new HeaderCell(headers[col].ToString(), headerNames[col], index);
                    header.Append(cell);
                }
                sheetData.AppendChild(header);


                for (int i = 0; i < numRows; i++)
                {
                    FileListModel model = list[i];
                    index++;
                    var obj1 = list[i];
                    var r = new Row { RowIndex = (uint)index };

                    TextCell cell0 = new TextCell(headers[0].ToString(), model.IdentifyFileName, index);
                    r.Append(cell0);

                    TextCell cell1 = new TextCell(headers[1].ToString(), model.Number, index);
                    r.Append(cell1);

                    TextCell cell2 = new TextCell(headers[2].ToString(), model.VideoType.ToDescription() == "0" ? "" : model.VideoType.ToDescription(), index);
                    r.Append(cell2);

                    if (model.UploadDate == CommonAgent.MinDate)
                    {
                        TextCell cell3 = new TextCell(headers[3].ToString(), "", index);
                        r.Append(cell3);
                    }
                    else
                    {
                        DateCell cell3 = new DateCell(headers[3].ToString(), model.UploadDate, index);
                        r.Append(cell3);
                    }

                    if (model.DateRecorded == CommonAgent.MinDate)
                    {
                        TextCell cell4 = new TextCell(headers[4].ToString(), "", index);
                        r.Append(cell4);
                    }
                    else
                    {
                        DateCell cell4 = new DateCell(headers[4].ToString(), model.DateRecorded, index);
                        r.Append(cell4);
                    }

                    TextCell cell5 = new TextCell(headers[5].ToString(), model.Strategy, index);
                    r.Append(cell5);

                    TextCell cell6 = new TextCell(headers[6].ToString(), model.Context, index);
                    r.Append(cell6);

                    TextCell cell7 = new TextCell(headers[7].ToString(), model.Content, index);
                    r.Append(cell7);

                    TextCell cell8 = new TextCell(headers[8].ToString(), model.FileExtension, index);
                    r.Append(cell8);

                    TextCell cell9 = new TextCell(headers[9].ToString(), model.Status.ToDescription() == "0" ? "" : model.Status.ToDescription(), index);
                    r.Append(cell9);

                    sheetData.Append(r);
                }

                #endregion
                worksheetPart.Worksheet = new Worksheet();
                worksheetPart.Worksheet.Append(columns);
                worksheetPart.Worksheet.Append(sheetData);

                var sheets = myWorkbook.WorkbookPart.Workbook.AppendChild<Sheets>(new Sheets());
                var sheet = new Sheet { Name = "CoachSummary_CoachFile", SheetId = 1, Id = relId };
                sheets.Append(sheet);

                workbookPart.Workbook.Save();
                myWorkbook.Close();
            }

            return path;
        }

        public string TeacherFilesToExcel(List<FileListModel> list)
        {
            List<string> headerNames =
               new List<string> { "File Name","ID",
                   "Community", "School", "Teacher", 
                   "Context","Content","Status","Recorded","Type" };

            string path = Path.Combine(SFConfig.ProtectedFiles, "CoachSummary_TeacherFiles_" + DateTime.Now.Ticks + "_" + Guid.NewGuid() + ".xlsx").Replace("/", "\\");


            //Open the copied template workbook. 
            using (SpreadsheetDocument myWorkbook =
                   SpreadsheetDocument.Create(path,
                   SpreadsheetDocumentType.Workbook))
            {
                WorkbookPart workbookPart = myWorkbook.AddWorkbookPart();
                workbookPart.Workbook = new Workbook();

                var worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                // Create Styles and Insert into Workbook
                var stylesPart =
                    myWorkbook.WorkbookPart.AddNewPart<WorkbookStylesPart>();
                Stylesheet styles = new CustomStylesheet();
                styles.Save(stylesPart);
                string relId = workbookPart.GetIdOfPart(worksheetPart);
                var fileVersion =
                new FileVersion
                {
                    ApplicationName =
                        "Microsoft Office Excel"
                };
                var worksheet = new Worksheet();
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


                #region 填充数据
                SheetData sheetData = new SheetData();
                int index = 1;
                //Get a list of A to Z
                var az = new List<Char>(Enumerable.Range('A', 'Z' -
                                      'A' + 1).Select(i => (Char)i).ToArray());
                //A to E number of columns 
                List<Char> headers = az.GetRange(0, headerNames.Count);
                Row header = new Row();
                for (int col = 0; col < headerNames.Count; col++)
                {
                    HeaderCell cell = new HeaderCell(headers[col].ToString(), headerNames[col], index);
                    header.Append(cell);
                }
                sheetData.AppendChild(header);


                for (int i = 0; i < numRows; i++)
                {
                    FileListModel model = list[i];
                    index++;
                    var obj1 = list[i];
                    var r = new Row { RowIndex = (uint)index };

                    TextCell cell0 = new TextCell(headers[0].ToString(), model.IdentifyFileName, index);
                    r.Append(cell0);

                    TextCell cell1 = new TextCell(headers[1].ToString(), model.Number, index);
                    r.Append(cell1);

                    TextCell cell2 = new TextCell(headers[2].ToString(), model.CommunityName, index);
                    r.Append(cell2);

                    TextCell cell3 = new TextCell(headers[3].ToString(), model.SchoolName, index);
                    r.Append(cell3);

                    TextCell cell4 = new TextCell(headers[4].ToString(), model.TeacherName, index);
                    r.Append(cell4);

                    TextCell cell5 = new TextCell(headers[5].ToString(), model.Context, index);
                    r.Append(cell5);

                    TextCell cell6 = new TextCell(headers[6].ToString(), model.Content, index);
                    r.Append(cell6);

                    TextCell cell7 = new TextCell(headers[7].ToString(), model.Status.ToDescription() == "0" ? "" : model.Status.ToDescription(), index);
                    r.Append(cell7);

                    if (model.DateRecorded == CommonAgent.MinDate)
                    {
                        TextCell cell8 = new TextCell(headers[8].ToString(), "", index);
                        r.Append(cell8);
                    }
                    else
                    {
                        DateCell cell8 = new DateCell(headers[8].ToString(), model.DateRecorded, index);
                        r.Append(cell8);
                    }

                    TextCell cell9 = new TextCell(headers[9].ToString(), model.FileExtension, index);
                    r.Append(cell9);

                    sheetData.Append(r);
                }

                #endregion
                worksheetPart.Worksheet = new Worksheet();
                worksheetPart.Worksheet.Append(columns);
                worksheetPart.Worksheet.Append(sheetData);

                var sheets = myWorkbook.WorkbookPart.Workbook.AppendChild<Sheets>(new Sheets());
                var sheet = new Sheet { Name = "CoachSummary_TeacherFile", SheetId = 1, Id = relId };
                sheets.Append(sheet);

                workbookPart.Workbook.Save();
                myWorkbook.Close();
            }

            return path;
        }



        public string TeacherVIPAssignmentToExcel(List<AssignmentListModel> list)
        {
            List<string> headerNames =
               new List<string> { 
                   "Community", "School", "Teacher", "Sender",
                   "Due Date", "Wave","Context","Content","Status" };

            string path = Path.Combine(SFConfig.ProtectedFiles, "PM_Teacher_VIP_" + DateTime.Now.Ticks + "_" + Guid.NewGuid() + ".xlsx").Replace("/", "\\");


            //Open the copied template workbook. 
            using (SpreadsheetDocument myWorkbook =
                   SpreadsheetDocument.Create(path,
                   SpreadsheetDocumentType.Workbook))
            {
                WorkbookPart workbookPart = myWorkbook.AddWorkbookPart();
                workbookPart.Workbook = new Workbook();

                var worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                // Create Styles and Insert into Workbook
                var stylesPart =
                    myWorkbook.WorkbookPart.AddNewPart<WorkbookStylesPart>();
                Stylesheet styles = new CustomStylesheet();
                styles.Save(stylesPart);
                string relId = workbookPart.GetIdOfPart(worksheetPart);
                var fileVersion =
                new FileVersion
                {
                    ApplicationName =
                        "Microsoft Office Excel"
                };
                var worksheet = new Worksheet();
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


                #region 填充数据
                SheetData sheetData = new SheetData();
                int index = 1;
                //Get a list of A to Z
                var az = new List<Char>(Enumerable.Range('A', 'Z' -
                                      'A' + 1).Select(i => (Char)i).ToArray());
                //A to E number of columns 
                List<Char> headers = az.GetRange(0, headerNames.Count);
                Row header = new Row();
                for (int col = 0; col < headerNames.Count; col++)
                {
                    HeaderCell cell = new HeaderCell(headers[col].ToString(), headerNames[col], index);
                    header.Append(cell);
                }
                sheetData.AppendChild(header);


                for (int i = 0; i < numRows; i++)
                {
                    AssignmentListModel model = list[i];
                    index++;
                    var obj1 = list[i];
                    var r = new Row { RowIndex = (uint)index };
                    TextCell cell0 = new TextCell(headers[0].ToString(), model.CommunityName, index);
                    r.Append(cell0);

                    TextCell cell1 = new TextCell(headers[1].ToString(), model.SchoolName, index);
                    r.Append(cell1);

                    TextCell cell2 = new TextCell(headers[2].ToString(), model.TeacherName, index);
                    r.Append(cell2);

                    TextCell cell3 = new TextCell(headers[3].ToString(), model.SendUserName, index);
                    r.Append(cell3);

                    if (model.DueDate == CommonAgent.MinDate)
                    {
                        TextCell cell4 = new TextCell(headers[4].ToString(), "", index);
                        r.Append(cell4);
                    }
                    else
                    {
                        DateCell cell4 = new DateCell(headers[4].ToString(), model.DueDate, index);
                        r.Append(cell4);
                    }

                    TextCell cell5 = new TextCell(headers[5].ToString(), model.WaveText, index);
                    r.Append(cell5);

                    TextCell cell6 = new TextCell(headers[6].ToString(), model.Context, index);
                    r.Append(cell6);

                    TextCell cell7 = new TextCell(headers[7].ToString(), model.Content, index);
                    r.Append(cell7);

                    TextCell cell8 = new TextCell(headers[8].ToString(), model.Status.ToDescription() == "0" ? "" : model.Status.ToDescription(), index);
                    r.Append(cell8);

                    sheetData.Append(r);
                }

                #endregion
                worksheetPart.Worksheet = new Worksheet();
                worksheetPart.Worksheet.Append(columns);
                worksheetPart.Worksheet.Append(sheetData);

                var sheets = myWorkbook.WorkbookPart.Workbook.AppendChild<Sheets>(new Sheets());
                var sheet = new Sheet { Name = "PM Teacher VIP", SheetId = 1, Id = relId };
                sheets.Append(sheet);

                workbookPart.Workbook.Save();
                myWorkbook.Close();
            }

            return path;
        }

        #region PM Summary

        public string PM_CoachFilesToExcel(List<FileListModel> list, bool isAdmin = false)
        {
            List<string> headerNames =
               new List<string> {"Coach", 
                   "File Name","ID",
                   "Upload Date", "Date Recorded", 
                   "Strategies", "Context","Content","File Type","Status" };

            string path = "";
            if (isAdmin)
            {
                path = Path.Combine(SFConfig.ProtectedFiles, "AdminSummary_CoachFiles_" + DateTime.Now.Ticks + "_" + Guid.NewGuid() + ".xlsx").Replace("/", "\\");
            }
            else
            {
                path = Path.Combine(SFConfig.ProtectedFiles, "PMSummary_CoachFiles_" + DateTime.Now.Ticks + "_" + Guid.NewGuid() + ".xlsx").Replace("/", "\\");
            }


            //Open the copied template workbook. 
            using (SpreadsheetDocument myWorkbook =
                   SpreadsheetDocument.Create(path,
                   SpreadsheetDocumentType.Workbook))
            {
                WorkbookPart workbookPart = myWorkbook.AddWorkbookPart();
                workbookPart.Workbook = new Workbook();

                var worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                // Create Styles and Insert into Workbook
                var stylesPart =
                    myWorkbook.WorkbookPart.AddNewPart<WorkbookStylesPart>();
                Stylesheet styles = new CustomStylesheet();
                styles.Save(stylesPart);
                string relId = workbookPart.GetIdOfPart(worksheetPart);
                var fileVersion =
                new FileVersion
                {
                    ApplicationName =
                        "Microsoft Office Excel"
                };
                var worksheet = new Worksheet();
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


                #region 填充数据
                SheetData sheetData = new SheetData();
                int index = 1;
                //Get a list of A to Z
                var az = new List<Char>(Enumerable.Range('A', 'Z' -
                                      'A' + 1).Select(i => (Char)i).ToArray());
                //A to E number of columns 
                List<Char> headers = az.GetRange(0, headerNames.Count);
                Row header = new Row();
                for (int col = 0; col < headerNames.Count; col++)
                {
                    HeaderCell cell = new HeaderCell(headers[col].ToString(), headerNames[col], index);
                    header.Append(cell);
                }
                sheetData.AppendChild(header);


                for (int i = 0; i < numRows; i++)
                {
                    FileListModel model = list[i];
                    index++;
                    var obj1 = list[i];
                    var r = new Row { RowIndex = (uint)index };

                    TextCell cell0 = new TextCell(headers[0].ToString(), model.CoachName, index);
                    r.Append(cell0);

                    TextCell cell1 = new TextCell(headers[1].ToString(), model.IdentifyFileName, index);
                    r.Append(cell1);

                    TextCell cell2 = new TextCell(headers[2].ToString(), model.Number, index);
                    r.Append(cell2);

                    if (model.UploadDate == CommonAgent.MinDate)
                    {
                        TextCell cell3 = new TextCell(headers[3].ToString(), "", index);
                        r.Append(cell3);
                    }
                    else
                    {
                        DateCell cell3 = new DateCell(headers[3].ToString(), model.UploadDate, index);
                        r.Append(cell3);
                    }

                    if (model.DateRecorded == CommonAgent.MinDate)
                    {
                        TextCell cell4 = new TextCell(headers[4].ToString(), "", index);
                        r.Append(cell4);
                    }
                    else
                    {
                        DateCell cell4 = new DateCell(headers[4].ToString(), model.DateRecorded, index);
                        r.Append(cell4);
                    }

                    TextCell cell5 = new TextCell(headers[5].ToString(), model.Strategy, index);
                    r.Append(cell5);

                    TextCell cell6 = new TextCell(headers[6].ToString(), model.Context, index);
                    r.Append(cell6);

                    TextCell cell7 = new TextCell(headers[7].ToString(), model.Content, index);
                    r.Append(cell7);

                    TextCell cell8 = new TextCell(headers[8].ToString(), model.FileExtension, index);
                    r.Append(cell8);

                    TextCell cell9 = new TextCell(headers[9].ToString(), model.Status.ToDescription() == "0" ? "" : model.Status.ToDescription(), index);
                    r.Append(cell9);

                    sheetData.Append(r);
                }

                #endregion
                worksheetPart.Worksheet = new Worksheet();
                worksheetPart.Worksheet.Append(columns);
                worksheetPart.Worksheet.Append(sheetData);

                var sheets = myWorkbook.WorkbookPart.Workbook.AppendChild<Sheets>(new Sheets());
                var sheet = new Sheet { Name = isAdmin ? "AdminSummary_CoachFiles" : "PMSummary_CoachFiles", SheetId = 1, Id = relId };
                sheets.Append(sheet);

                workbookPart.Workbook.Save();
                myWorkbook.Close();
            }

            return path;
        }

        public string PM_TeacherFilesToExcel(List<FileListModel> list, bool isAdmin = false)
        {
            List<string> headerNames =
               new List<string> {"Community", "School","Teacher",
                   "File Name","ID","Context","Content","Status",
                   "Date Recorded", "Due Date", "File Type" };

            string path = "";
            if (isAdmin)
            {
                path = Path.Combine(SFConfig.ProtectedFiles, "AdminSummary_TeacherFiles_" + DateTime.Now.Ticks + "_" + Guid.NewGuid() + ".xlsx").Replace("/", "\\");
            }
            else
            {
                path = Path.Combine(SFConfig.ProtectedFiles, "PMSummary_TeacherFiles_" + DateTime.Now.Ticks + "_" + Guid.NewGuid() + ".xlsx").Replace("/", "\\");
            }


            //Open the copied template workbook. 
            using (SpreadsheetDocument myWorkbook =
                   SpreadsheetDocument.Create(path,
                   SpreadsheetDocumentType.Workbook))
            {
                WorkbookPart workbookPart = myWorkbook.AddWorkbookPart();
                workbookPart.Workbook = new Workbook();

                var worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                // Create Styles and Insert into Workbook
                var stylesPart =
                    myWorkbook.WorkbookPart.AddNewPart<WorkbookStylesPart>();
                Stylesheet styles = new CustomStylesheet();
                styles.Save(stylesPart);
                string relId = workbookPart.GetIdOfPart(worksheetPart);
                var fileVersion =
                new FileVersion
                {
                    ApplicationName =
                        "Microsoft Office Excel"
                };
                var worksheet = new Worksheet();
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


                #region 填充数据
                SheetData sheetData = new SheetData();
                int index = 1;
                //Get a list of A to Z
                var az = new List<Char>(Enumerable.Range('A', 'Z' -
                                      'A' + 1).Select(i => (Char)i).ToArray());
                //A to E number of columns 
                List<Char> headers = az.GetRange(0, headerNames.Count);
                Row header = new Row();
                for (int col = 0; col < headerNames.Count; col++)
                {
                    HeaderCell cell = new HeaderCell(headers[col].ToString(), headerNames[col], index);
                    header.Append(cell);
                }
                sheetData.AppendChild(header);


                for (int i = 0; i < numRows; i++)
                {
                    FileListModel model = list[i];
                    index++;
                    var obj1 = list[i];
                    var r = new Row { RowIndex = (uint)index };

                    TextCell cell0 = new TextCell(headers[0].ToString(), model.CommunityName, index);
                    r.Append(cell0);

                    TextCell cell1 = new TextCell(headers[1].ToString(), model.SchoolName, index);
                    r.Append(cell1);

                    TextCell cell2 = new TextCell(headers[2].ToString(), model.TeacherName, index);
                    r.Append(cell2);

                    TextCell cell3 = new TextCell(headers[3].ToString(), model.IdentifyFileName, index);
                    r.Append(cell3);

                    TextCell cell4 = new TextCell(headers[4].ToString(), model.Number, index);
                    r.Append(cell4);

                    TextCell cell5 = new TextCell(headers[5].ToString(), model.Context, index);
                    r.Append(cell5);

                    TextCell cell6 = new TextCell(headers[6].ToString(), model.Content, index);
                    r.Append(cell6);

                    TextCell cell7 = new TextCell(headers[7].ToString(), model.Status.ToDescription() == "0" ? "" : model.Status.ToDescription(), index);
                    r.Append(cell7);

                    if (model.DateRecorded == CommonAgent.MinDate)
                    {
                        TextCell cell8 = new TextCell(headers[8].ToString(), "", index);
                        r.Append(cell8);
                    }
                    else
                    {
                        DateCell cell8 = new DateCell(headers[8].ToString(), model.DateRecorded, index);
                        r.Append(cell8);
                    }

                    if (model.AssignmentDueDate == CommonAgent.MinDate)
                    {
                        TextCell cell9 = new TextCell(headers[9].ToString(), "", index);
                        r.Append(cell9);
                    }
                    else
                    {
                        DateCell cell9 = new DateCell(headers[9].ToString(), model.AssignmentDueDate, index);
                        r.Append(cell9);
                    }

                    TextCell cell10 = new TextCell(headers[10].ToString(), model.FileExtension, index);
                    r.Append(cell10);

                    sheetData.Append(r);
                }

                #endregion
                worksheetPart.Worksheet = new Worksheet();
                worksheetPart.Worksheet.Append(columns);
                worksheetPart.Worksheet.Append(sheetData);

                var sheets = myWorkbook.WorkbookPart.Workbook.AppendChild<Sheets>(new Sheets());
                var sheet = new Sheet { Name = isAdmin ? "AdminSummary_TeacherFiles" : "PMSummary_TeacherFiles", SheetId = 1, Id = relId };
                sheets.Append(sheet);

                workbookPart.Workbook.Save();
                myWorkbook.Close();
            }

            return path;
        }

        #endregion

    }
}