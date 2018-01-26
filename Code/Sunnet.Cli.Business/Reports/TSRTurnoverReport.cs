using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Drawing.Spreadsheet;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using LinqKit;
using Sunnet.Cli.Business.Reports.Model;
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
using Sunnet.Framework.Extensions;
using Chart = DocumentFormat.OpenXml.Drawing.Charts.Chart;
using Path = System.IO.Path;
using ShapeProperties = DocumentFormat.OpenXml.Drawing.ShapeProperties;
using TextProperties = DocumentFormat.OpenXml.Drawing.Charts.TextProperties;
using Sunnet.Cli.Core.Schools.Entities;

namespace Sunnet.Cli.Business.Reports
{
    public partial class ReportBusiness
    {
        private int chartTop = 2;

        private void AddChart(string file, string worksheetName, int startRow, int endRow, string axisLabelColumn, string dataColumn ,string dataSourceSheetName= "")
        {
            // Open the document for editing.  
            // classes list:https://msdn.microsoft.com/zh-cn/library/documentformat.openxml.drawing.charts(v=office.14).aspx

            if (dataSourceSheetName == string.Empty)
                dataSourceSheetName = worksheetName;
            using (SpreadsheetDocument document = SpreadsheetDocument.Open(file, true))
            {
                IEnumerable<Sheet> sheets =
                    document.WorkbookPart.Workbook.Descendants<Sheet>().Where(s => s.Name == worksheetName);
                if (!sheets.Any())
                {
                    // The specified worksheet does not exist.  
                    return;
                }
                WorksheetPart worksheetPart = (WorksheetPart)document.WorkbookPart.GetPartById(sheets.First().Id);

                // Chart Global Start
                // Add a new drawing to the worksheet.  
                DrawingsPart drawingsPart = worksheetPart.GetPartsOfType<DrawingsPart>().FirstOrDefault();
                if (drawingsPart == null)
                {
                    drawingsPart = worksheetPart.AddNewPart<DrawingsPart>();

                }

                var drawing = worksheetPart.Worksheet.GetFirstChild<Drawing>();
                if (drawing == null)
                {
                    drawing = new Drawing() { Id = worksheetPart.GetIdOfPart(drawingsPart) };
                    worksheetPart.Worksheet.Append(drawing);
                }
                else
                {
                    drawing.Append();
                }
                worksheetPart.Worksheet.Save();
                // Add a new chart and set the chart language to English-US.  
                ChartPart chartPart = drawingsPart.AddNewPart<ChartPart>();
                chartPart.ChartSpace = new ChartSpace();
                chartPart.ChartSpace.Append(new EditingLanguage() { Val = new StringValue("en-US") });
                Chart chart = chartPart.ChartSpace.AppendChild<Chart>(new Chart());
                chart.DisplayBlanksAs = new DisplayBlanksAs() { Val = new EnumValue<DisplayBlanksAsValues>(DisplayBlanksAsValues.Zero) };
                chart.ShowDataLabelsOverMaximum = new ShowDataLabelsOverMaximum() { Val = new BooleanValue(true) };

                chart.AppendChild<Title>(
                    new Title(
                        new Layout(),
                        new Overlay() { Val = new BooleanValue(false) },
                        new ShapeProperties(new NoFill(), new DocumentFormat.OpenXml.Drawing.Outline(new NoFill()),
                            new EffectList()),
                        new TextProperties(
                            new BodyProperties()
                            {
                                Rotation = new Int32Value(0),
                                UseParagraphSpacing = new BooleanValue(true),
                                VerticalOverflow =
                                    new EnumValue<TextVerticalOverflowValues>(TextVerticalOverflowValues.Ellipsis),
                                Vertical = new EnumValue<TextVerticalValues>(TextVerticalValues.Horizontal),
                                Wrap = new EnumValue<TextWrappingValues>(TextWrappingValues.Square),
                                Anchor = new EnumValue<TextAnchoringTypeValues>(TextAnchoringTypeValues.Center),
                                AnchorCenter = new BooleanValue(true)
                            },
                            new ListStyle(),
                            new Paragraph(
                                new ParagraphProperties(
                                    new DefaultRunProperties(
                                        new SolidFill(
                                            new SchemeColor(
                                                new LuminanceModulation() { Val = new Int32Value(65000) },
                                                new LuminanceOffset() { Val = new Int32Value(35000) })
                                            {
                                                Val = new EnumValue<SchemeColorValues>(SchemeColorValues.Text1)
                                            },
                                            new LatinFont()
                                            {
                                                Typeface = new StringValue("+mn-lt")
                                            },
                                            new EastAsianFont()
                                            {
                                                Typeface = new StringValue("+mn-ea")
                                            },
                                            new ComplexScriptFont()
                                            {
                                                Typeface = new StringValue("+mn-cs")
                                            }))
                                    {
                                        FontSize = new Int32Value(1600),
                                        Bold = new BooleanValue(true),
                                        Italic = new BooleanValue(false),
                                        Underline = new EnumValue<TextUnderlineValues>(TextUnderlineValues.None),
                                        Strike = new EnumValue<TextStrikeValues>(TextStrikeValues.NoStrike),
                                        Kerning = new Int32Value(1200),
                                        Baseline = new Int32Value(0)
                                    }),
                                new EndParagraphRunProperties()
                                {
                                    Language = new StringValue("en-US")
                                })
                            )
                        )
                    );

                chart.AppendChild<AutoTitleDeleted>(new AutoTitleDeleted() { Val = new BooleanValue(false) });

                // Create a new clustered column chart.  
                PlotArea plotArea = chart.AppendChild<PlotArea>(new PlotArea());
                Layout layout = plotArea.AppendChild<Layout>(new Layout());

                // 图表类型根据自己的需求定义  
                // 柱状图  
                BarChart barChart = plotArea.AppendChild<BarChart>(new BarChart(
                    new BarDirection() { Val = new EnumValue<BarDirectionValues>(BarDirectionValues.Bar) },
                    new BarGrouping() { Val = new EnumValue<BarGroupingValues>(BarGroupingValues.Clustered) }
                    ));
                barChart.VaryColors = new VaryColors() { Val = new BooleanValue(true) };

                // first chart
                uint i2 = 0;

                string formulaString = string.Format("{0}!${1}${2}:${1}${3}", dataSourceSheetName, dataColumn, startRow, endRow);
                DocumentFormat.OpenXml.Drawing.Charts.Values v = new DocumentFormat.OpenXml.Drawing.Charts.Values();
                v.NumberReference = new NumberReference()
                {
                    Formula = new DocumentFormat.OpenXml.Drawing.Charts.Formula(formulaString)
                };

                formulaString = string.Format("{0}!${1}${2}", dataSourceSheetName, dataColumn, startRow - 1);
                SeriesText st = new SeriesText();
                st.StringReference = new StringReference()
                {
                    Formula = new DocumentFormat.OpenXml.Drawing.Charts.Formula(formulaString)
                };

                var ct = GetCategoryAxisData(dataSourceSheetName, startRow, endRow, axisLabelColumn);


                DataLabels dl = new DataLabels(
                    new ChartShapeProperties(new NoFill(), new DocumentFormat.OpenXml.Drawing.Outline(new NoFill()),
                        new EffectList()) { },
                    new ShowLegendKey() { Val = new BooleanValue(false) },
                    new ShowValue() { Val = new BooleanValue(true) },
                    new ShowCategoryName() { Val = new BooleanValue(false) },
                    new ShowSeriesName() { Val = new BooleanValue(false) },
                    new ShowPercent() { Val = new BooleanValue(false) },
                    new ShowBubbleSize() { Val = new BooleanValue(false) },
                    new ShowLeaderLines() { Val = new BooleanValue(false) },
                    new BarSerExtensionList(new BarChartExtension(
                        new Layout(),
                        new ShowLeaderLines() { Val = new BooleanValue(true) })
                    {
                        Uri = new StringValue(Guid.NewGuid().ToString("B").ToUpper())
                    })
                    );

                // 生成一个实例---柱状图  
                BarChartSeries barChartSeries =
                    barChart.AppendChild<BarChartSeries>(new BarChartSeries(
                        new Index() { Val = new UInt32Value(i2) },
                        new Order() { Val = new UInt32Value(i2) }
                        , st, dl, ct, v)
                        );

                // 以下代码没有做任何改动，跟sdk中的源码一致  
                barChart.Append(new AxisId() { Val = new UInt32Value(48650112u) });
                barChart.Append(new AxisId() { Val = new UInt32Value(48672768u) });
                // Add the Category Axis.  
                CategoryAxis catAx =
                    plotArea.AppendChild<CategoryAxis>(new CategoryAxis(new AxisId() { Val = new UInt32Value(48650112u) },
                        new Scaling(new Orientation()
                        {
                            Val = new EnumValue<DocumentFormat.OpenXml.Drawing.Charts.OrientationValues>(
                                DocumentFormat.OpenXml.Drawing.Charts.OrientationValues.MinMax)
                        }),
                        new Delete() { Val = new BooleanValue(false) },
                        new AxisPosition() { Val = new EnumValue<AxisPositionValues>(AxisPositionValues.Left) },
                        new TickLabelPosition()
                        {
                            Val = new EnumValue<TickLabelPositionValues>(TickLabelPositionValues.NextTo)
                        },
                        new CrossingAxis() { Val = new UInt32Value(48672768U) },
                        new Crosses() { Val = new EnumValue<CrossesValues>(CrossesValues.AutoZero) },
                        new AutoLabeled() { Val = new BooleanValue(true) },
                        new LabelAlignment() { Val = new EnumValue<LabelAlignmentValues>(LabelAlignmentValues.Center) },
                        new LabelOffset() { Val = new UInt16Value((ushort)100) },
                        new MajorTickMark() { Val = new EnumValue<TickMarkValues>(TickMarkValues.None) },
                        new MinorTickMark() { Val = new EnumValue<TickMarkValues>(TickMarkValues.None) },
                        new NoMultiLevelLabels() { Val = new BooleanValue(false) }
                        ));

                // Add the Value Axis.  
                ValueAxis valAx = plotArea.AppendChild<ValueAxis>(new ValueAxis(new AxisId() { Val = new UInt32Value(48672768u) },
                    new Scaling(new Orientation()
                    {
                        Val = new EnumValue<DocumentFormat.OpenXml.Drawing.Charts.OrientationValues>(
                            DocumentFormat.OpenXml.Drawing.Charts.OrientationValues.MinMax)
                    }),
                    new Delete() { Val = new BooleanValue(false) },
                    new AxisPosition() { Val = new EnumValue<AxisPositionValues>(AxisPositionValues.Bottom) },
                    new MajorGridlines(),
                    new DocumentFormat.OpenXml.Drawing.Charts.NumberingFormat() { FormatCode = new StringValue("General"), SourceLinked = new BooleanValue(true) },
                    new TickLabelPosition() { Val = new EnumValue<TickLabelPositionValues>(TickLabelPositionValues.NextTo) },
                    new CrossingAxis() { Val = new UInt32Value(48650112U) },
                    new Crosses() { Val = new EnumValue<CrossesValues>(CrossesValues.AutoZero) },
                    new CrossBetween() { Val = new EnumValue<CrossBetweenValues>(CrossBetweenValues.Between) },
                    new MajorTickMark() { Val = new EnumValue<TickMarkValues>(TickMarkValues.None) },
                    new MinorTickMark() { Val = new EnumValue<TickMarkValues>(TickMarkValues.None) }));

                // Add the chart Legend.  
                //Legend legend = chart.AppendChild<Legend>(new Legend(new LegendPosition() { Val = new EnumValue<LegendPositionValues>(LegendPositionValues.Bottom) },
                //    new Layout()));

                chart.Append(new PlotVisibleOnly() { Val = new BooleanValue(true) });

                // Save the chart part.  
                chartPart.ChartSpace.Save();

                // Position the chart on the worksheet using a TwoCellAnchor object. 
                if (drawingsPart.WorksheetDrawing == null)
                    drawingsPart.WorksheetDrawing = new WorksheetDrawing();
                TwoCellAnchor twoCellAnchor = drawingsPart.WorksheetDrawing.AppendChild<TwoCellAnchor>(new TwoCellAnchor());


                twoCellAnchor.Append(new DocumentFormat.OpenXml.Drawing.Spreadsheet.FromMarker(new ColumnId("6"),
                    new ColumnOffset("581025"),
                    new RowId(chartTop.ToString()),
                    new RowOffset("114300")));

                var toRow = (chartTop + 10 + (endRow - startRow) * 2).ToString();
                twoCellAnchor.Append(new DocumentFormat.OpenXml.Drawing.Spreadsheet.ToMarker(new ColumnId("16"),
                    new ColumnOffset("276225"),
                    new RowId(toRow),
                    new RowOffset("0")));

                // Append a GraphicFrame to the TwoCellAnchor object.  
                DocumentFormat.OpenXml.Drawing.Spreadsheet.GraphicFrame graphicFrame =
                    twoCellAnchor.AppendChild<DocumentFormat.OpenXml.Drawing.Spreadsheet.GraphicFrame>(
                    new DocumentFormat.OpenXml.Drawing.Spreadsheet.GraphicFrame());
                graphicFrame.Macro = "";

                graphicFrame.Append(new DocumentFormat.OpenXml.Drawing.Spreadsheet.NonVisualGraphicFrameProperties(
                    new DocumentFormat.OpenXml.Drawing.Spreadsheet.NonVisualDrawingProperties() { Id = new UInt32Value(2u), Name = "Chart 1" },
                    new DocumentFormat.OpenXml.Drawing.Spreadsheet.NonVisualGraphicFrameDrawingProperties()));

                graphicFrame.Append(new Transform(new Offset() { X = 0L, Y = 0L },
                                                                        new Extents() { Cx = 0L, Cy = 0L }));

                graphicFrame.Append(new Graphic(new GraphicData(new ChartReference() { Id = drawingsPart.GetIdOfPart(chartPart) }) { Uri = "http://schemas.openxmlformats.org/drawingml/2006/chart" }));

                twoCellAnchor.Append(new ClientData());

                // Save the WorksheetDrawing object.  
                drawingsPart.WorksheetDrawing.Save();

                chartTop += 5 + (endRow - startRow) * 2 + 5;
            }
        }

        private static CategoryAxisData GetCategoryAxisData(string worksheetName, int startRow, int endRow,
            string axisLabelColumn)
        {
            CategoryAxisData ct = new CategoryAxisData();

            string formulaString = "";
            if (axisLabelColumn.Length == 1)
            {
                formulaString = string.Format("{0}!${1}${2}:${1}${3}", worksheetName, axisLabelColumn, startRow, endRow);
                ct.StringReference = new StringReference()
                {
                    Formula = new DocumentFormat.OpenXml.Drawing.Charts.Formula(formulaString)
                };
            }
            else
            {
                formulaString = string.Format("{0}!${1}${2}:${3}${4}", worksheetName, axisLabelColumn.First(), startRow, axisLabelColumn.Last(), endRow);
                ct.MultiLevelStringReference = new MultiLevelStringReference()
                {
                    Formula = new DocumentFormat.OpenXml.Drawing.Charts.Formula(formulaString)
                };
            }
            return ct;
        }

        public void TurnoverReport(out string path, DateTime startDate, DateTime endDate, List<int> communityIds, List<int> schoolIds,
            Dictionary<string, object> searchCriteria)
        {
            var generateSchoolTypeReport = false;
            var generateCommunityReport = false;

            string directory = Path.Combine(SFConfig.ProtectedFiles, string.Format("Reports/{0}{1}/", DateTime.Now.Year, DateTime.Now.Month));
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            path = directory + "TurnoverReport" + DateTime.Now.Ticks + "_" + Guid.NewGuid() + ".xlsx".Replace("/", "\\");
            string worksheetName = "Teachers";
            int dataStartRow1 = 0, dataEndRow1 = 0, dataStartRow2 = 0, dataEndRow2 = 0;
            using (SpreadsheetDocument myWorkbook = SpreadsheetDocument.Create(path, SpreadsheetDocumentType.Workbook))
            {
                WorkbookPart workbookPart = myWorkbook.AddWorkbookPart();
                workbookPart.Workbook = new Workbook();
                var worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                // Create Styles and Insert into Workbook
                var stylesPart = myWorkbook.WorkbookPart.AddNewPart<WorkbookStylesPart>();
                Stylesheet styles = new TeacherTrunoverStylesheet();
                styles.Save(stylesPart);
                string relId = workbookPart.GetIdOfPart(worksheetPart);

                var sheets = myWorkbook.WorkbookPart.Workbook.AppendChild<Sheets>(new Sheets());
                var sheet = new Sheet { Name = worksheetName, SheetId = 1, Id = relId };
                sheets.Append(sheet);

                List<int> headerWidth = new List<int>() { 30, 15, 15, 15, 30 };
                headerWidth.Insert(1, 30);//Coach column
                var columns = new Columns();

                for (int col = 0; col < headerWidth.Count; col++)
                {
                    int width = headerWidth[col];
                    Column c = new CustomColumn((UInt32)col + 1,
                                  (UInt32)headerWidth.Count + 1, width);
                    columns.Append(c);
                }

                worksheetPart.Worksheet = new Worksheet();
                worksheetPart.Worksheet.Append(columns);

                SheetData sheetData = new SheetData();
                int rowIndex = 1;

                var r1 = new Row { RowIndex = (uint)rowIndex };
                r1.Append(new MediaBICell("0", "Teacher Turnover Report", rowIndex));
                sheetData.Append(r1);

                rowIndex = rowIndex + 1;
                r1 = new Row { RowIndex = (uint)rowIndex };
                var subTitle = DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss");
                if (searchCriteria != null)
                {
                    searchCriteria.ForEach(
                        criteria => subTitle = string.Format("{0} {1}:{2}", subTitle, criteria.Key, criteria.Value));
                }
                r1.Append(new TextCell("0", subTitle, rowIndex));
                sheetData.Append(r1);

                rowIndex = rowIndex + 2;

                r1 = new Row { RowIndex = (uint)rowIndex };
                r1.Append(new HeaderCell("0", "Teacher Turnover Rate by School Type", rowIndex));
                sheetData.Append(r1);

                rowIndex += 1;
                r1 = new Row { RowIndex = (uint)rowIndex };
                r1.Append(new MediaBICell("0", "Time Period : " + startDate.ToString("MM/dd/yyyy") + " - " + endDate.AddDays(-1).ToString("MM/dd/yyyy"), rowIndex));
                sheetData.Append(r1);

                rowIndex += 1;

                var xAxis = new XAxis();

                Row header = new Row { RowIndex = (uint)rowIndex };
                header.Append(new HeaderCell(xAxis.Next, "School Type", rowIndex));
                header.Append(new HeaderCell(xAxis.Next, "Active", rowIndex));
                header.Append(new HeaderCell(xAxis.Next, "Dropped (Inactive)", rowIndex));
                header.Append(new HeaderCell(xAxis.Next, "Teacher Turnover Rate by School Type", rowIndex));
                sheetData.AppendChild(header);

                var row = new Row();
                dataStartRow1 = rowIndex + 1;

                List<TeacherSchoolTypeModel> list = _reportService.GetTeacherBySchoolType(startDate, endDate,
                    communityIds, schoolIds);
                generateSchoolTypeReport = list != null && list.Any();

                float percentage = 0f;

                TeacherSchoolTypeModel missingModel = new TeacherSchoolTypeModel();

                foreach (TeacherSchoolTypeModel item in list)
                {
                    if (item.SchoolTypeId == 0 || item.Status == EntityStatus.Inactive)
                    {
                        missingModel.ActiveTotal += item.ActiveTotal;
                        missingModel.DroppedTotal += item.DroppedTotal;
                        missingModel.Total += item.Total;
                        continue;
                    }

                    xAxis.Reset();
                    rowIndex++;
                    row = new Row { RowIndex = (uint)rowIndex };
                    TextCell cell1 = new TextCell(xAxis.Next, item.SchoolTypeName, rowIndex);
                    row.Append(cell1);
                    row.Append(new NumberCenterCell(xAxis.Next, item.ActiveTotal.ToString(), rowIndex));
                    row.Append(new NumberCenterCell(xAxis.Next, item.DroppedTotal.ToString(), rowIndex));

                    percentage = item.Total == 0 ? 0f : (item.DroppedTotal + 0.0f) / item.Total;

                    PercentageCenterCell cell5 = new PercentageCenterCell(xAxis.Next, Math.Round(percentage, 2), rowIndex);
                    row.Append(cell5);

                    sheetData.Append(row);
                }

                rowIndex++;
                xAxis.Reset();
                row = new Row { RowIndex = (uint)rowIndex };
                row.Append(new TextCell(xAxis.Next, "Missing", rowIndex));
                row.Append(new NumberCenterCell(xAxis.Next, missingModel.ActiveTotal.ToString(), rowIndex));
                row.Append(new NumberCenterCell(xAxis.Next, missingModel.DroppedTotal.ToString(), rowIndex));

                percentage = missingModel.Total == 0 ? 0f : (missingModel.DroppedTotal + 0.0f) / missingModel.Total;
                row.Append(new PercentageCenterCell(xAxis.Next, Math.Round(percentage, 2), rowIndex));

                sheetData.Append(row);

                rowIndex++;
                xAxis.Reset();
                row = new Row { RowIndex = (uint)rowIndex };
                row.Append(new HeaderCell(xAxis.Next, "Total", rowIndex));
                row.Append(new NumberBCenterCell(xAxis.Next, list.Sum(r => r.ActiveTotal).ToString(), rowIndex));
                row.Append(new NumberBCenterCell(xAxis.Next, list.Sum(r => r.DroppedTotal).ToString(), rowIndex));
                int total = list.Sum(r => r.Total);
                int totalInactive = list.Sum(r => r.DroppedTotal);
                percentage = total == 0 ? 0 : (totalInactive + 0.0f) / total;
                row.Append(new PercentageBCenterCell(xAxis.Next, Math.Round(percentage, 2), rowIndex));

                sheetData.Append(row);
                dataEndRow1 = rowIndex;

                //Count by Community
                rowIndex += 4;
                row = new Row { RowIndex = (uint)rowIndex };
                row.Append(new HeaderCell("0", "Turnover Rate by Community/District", rowIndex));
                sheetData.Append(row);

                rowIndex += 1;
                row = new Row { RowIndex = (uint)rowIndex };
                row.Append(new MediaBICell("0", "Time Period : " + startDate.ToString("MM/dd/yyyy") + " - " + endDate.AddDays(-1).ToString("MM/dd/yyyy"), rowIndex));
                sheetData.Append(row);

                rowIndex += 1;
                row = new Row { RowIndex = (uint)rowIndex };
                xAxis.Reset();
                row.Append(new HeaderCell(xAxis.Next, " Coordinator ", rowIndex));
                row.Append(new HeaderCell(xAxis.Next, "Community Name", rowIndex));
                row.Append(new HeaderCell(xAxis.Next, "Active", rowIndex));
                row.Append(new HeaderCell(xAxis.Next, "Dropped (Inactive)", rowIndex));
                row.Append(new HeaderCell(xAxis.Next, "Turnover Rate by Community/District", rowIndex));
                sheetData.Append(row);

                ///记录需要合并的单元格 A1:AN
                List<string> listCells = new List<string>();

                List<CountTeacherbyCommunityModel> communityTeacherList =
                    _reportService.GetCountTeacherbyCommunityList(startDate, endDate, communityIds, schoolIds);
                generateCommunityReport = communityTeacherList != null && communityTeacherList.Any();

                List<int> users = communityTeacherList.Select(r => r.CoachId).Distinct().ToList();
                GetCommunityData(users, xAxis, communityTeacherList, listCells, sheetData, ref rowIndex);

              


                rowIndex += 3;
                               

                worksheetPart.Worksheet.Append(sheetData);
                if (listCells.Count > 0)
                    worksheetPart.Worksheet.MergeCells(listCells);

                #region   // sheet2
                worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                relId = workbookPart.GetIdOfPart(worksheetPart);
                sheetData = new SheetData();
                sheets.Append(new Sheet()
                {
                    Name = "List",
                    SheetId = 2,
                    Id = relId
                });
                columns = new Columns();
                var props = new List<string>() { "Community/District", "School Name", "School Type", "School Status", "Admin Email",
                    "Teacher First Name", "Teacher Last Name","Teacher ID", "Teacher Email", "Teacher Type", 
                    "Coaching Model", "Years in Project", "Teacher Status", "Inactivation Date","Creation Date" };
                for (int col = 0; col < props.Count; col++)
                {
                    int width = props[col].Length + 4;
                    Column c = new CustomColumn((UInt32)col + 1, (UInt32)col + 1, width);
                    columns.Append(c);
                }
                var headerRow = new Row();
                rowIndex = 1;
                for (int i = 0; i < props.Count; i++)
                {
                    headerRow.Append(new HeaderCell((i + 1).ToString(), props[i], rowIndex));
                }
                sheetData.Append(headerRow);

                var teachers = _reportService.GetTurnoverTeacherModels(startDate, endDate, communityIds, schoolIds);
                xAxis.Reset();
                rowIndex++;
                if (teachers != null)
                {
                    teachers.ForEach(teacher =>
                    {
                        var dataRow = new Row();
                        dataRow.Append(new TextCell(xAxis.Next, teacher.CommunityName, rowIndex));
                        dataRow.Append(new TextCell(xAxis.Next, teacher.SchoolName, rowIndex));
                        dataRow.Append(new TextCell(xAxis.Next, teacher.SchoolType, rowIndex));
                        dataRow.Append(new TextCell(xAxis.Next, teacher.SchoolStatus.ToDescription(), rowIndex));
                        dataRow.Append(new TextCell(xAxis.Next, teacher.PrimaryEmail, rowIndex));
                        dataRow.Append(new TextCell(xAxis.Next, teacher.FirstName, rowIndex));
                        dataRow.Append(new TextCell(xAxis.Next, teacher.LastName, rowIndex));
                        dataRow.Append(new TextCell(xAxis.Next, teacher.TeacherId, rowIndex));
                        dataRow.Append(new TextCell(xAxis.Next, teacher.PrimaryEmailAddress, rowIndex));
                        dataRow.Append(new TextCell(xAxis.Next,
                            teacher.TeacherType > 0 ? teacher.TeacherType.ToDescription() : teacher.TeacherTypeOther, rowIndex));
                        dataRow.Append(new TextCell(xAxis.Next,
                            teacher.CoachAssignmentWay > 0 ? teacher.CoachAssignmentWay.ToDescription() : teacher.CoachAssignmentWayOther, rowIndex));
                        dataRow.Append(new TextCell(xAxis.Next, teacher.YearsInProject, rowIndex));
                        dataRow.Append(new TextCell(xAxis.Next, teacher.TeacherStatus.ToDescription(), rowIndex));
                        dataRow.Append(new TextCell(xAxis.Next, teacher.InactiveDate.ToString("MM/dd/yyyy"), rowIndex));
                        dataRow.Append(new TextCell(xAxis.Next, teacher.CreationDate.ToString("MM/dd/yyyy"), rowIndex));
                        sheetData.Append(dataRow);
                        xAxis.Reset();
                        rowIndex++;
                    });
                }


                worksheetPart.Worksheet = new Worksheet();
                worksheetPart.Worksheet.Append(columns);

                worksheetPart.Worksheet.Append(sheetData);
                #endregion

                #region // sheet3
                worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                relId = workbookPart.GetIdOfPart(worksheetPart);
                sheetData = new SheetData();
                sheets.Append(new Sheet()
                {
                    Name = "Community",
                    SheetId = 3,
                    Id = relId,
                    State = SheetStateValues.Hidden
                });

                
                rowIndex = 1;
                ///记录需要合并的单元格 A1:AN
                List<string> listCells3 = new List<string>();

                row = new Row { RowIndex = (uint)rowIndex };

                xAxis.Reset();
                row.Append(new HeaderCell(xAxis.Next, " Coordinator ", rowIndex));
                row.Append(new HeaderCell(xAxis.Next, "Community Name", rowIndex));
                row.Append(new HeaderCell(xAxis.Next, "Active", rowIndex));
                row.Append(new HeaderCell(xAxis.Next, "Dropped (Inactive)", rowIndex));
                row.Append(new HeaderCell(xAxis.Next, "Turnover Rate by Community/District", rowIndex));
                sheetData.Append(row);

                dataStartRow2 = rowIndex + 1;
                GetCommunityData(users, xAxis, communityTeacherList, listCells3, sheetData, ref rowIndex, false);
                dataEndRow2 = rowIndex;

                worksheetPart.Worksheet = new Worksheet();

                worksheetPart.Worksheet.Append(sheetData);
                if (listCells3.Count > 0)
                    worksheetPart.Worksheet.MergeCells(listCells3);
                #endregion

                workbookPart.Workbook.Save();
                myWorkbook.Close();
            }
            if (generateSchoolTypeReport)
                AddChart(path, worksheetName, dataStartRow1, dataEndRow1, "A", "D");
            if (generateCommunityReport)
                AddChart(path, worksheetName, dataStartRow2, dataEndRow2,
                    "AB", "E", "Community");
        }

        private void GetCommunityData(List<int> users, XAxis xAxis,
         List<CountTeacherbyCommunityModel> communityTeacherList, List<string> listCells, SheetData sheetData,
         ref int rowIndex, bool addTotalRow = true)
        {
            Row row;
            int totalActive =0 ;
            int totalInactive = 0;
            int totalAll = 0;
            int active;
            int inactive;
            int total;
            float percentage;
            List<int> communityIds = new List<int>();

            if (users.Contains(0)) //有userId 为0的排在前面
            {
                communityIds = communityTeacherList.Where(r => r.CoachId == 0).Select(r => r.CommunityId).Distinct().ToList();
                if (communityIds.Count > 0)
                {
                    listCells.Add(string.Format("A{0}:A{1}", rowIndex + 1, addTotalRow ? rowIndex + communityIds.Count + 1 : rowIndex+ communityIds.Count));
                    

                    foreach (int id in communityIds)
                    {
                        xAxis.Reset();
                        xAxis.Step(1);
                        rowIndex++;
                        row = new Row() { RowIndex = new UInt32Value((uint)rowIndex) };
                        CountTeacherbyCommunityModel activeModel =
                   communityTeacherList.FirstOrDefault(
                       r => r.CoachId == 0 && r.CommunityId == id && r.Status == EntityStatus.Active);
                        CountTeacherbyCommunityModel inActiveModel =
                            communityTeacherList.FirstOrDefault(
                                r => r.CoachId == 0 && r.CommunityId == id && r.Status == EntityStatus.Inactive);

                        var communityName = activeModel == null
                   ? inActiveModel.CommunityName
                   : activeModel.CommunityName;
                        row.Append(new TextCell(xAxis.Next, communityName, rowIndex));

                        active = activeModel == null ? 0 : activeModel.Total;
                        inactive = inActiveModel == null ? 0 : inActiveModel.Total;
                        total = activeModel == null ? inActiveModel.AllTotal : activeModel.AllTotal;
                        totalActive += active;
                        totalInactive += inactive;
                        totalAll += total;

                        row.Append(new NumberCenterCell(xAxis.Next, active.ToString(), rowIndex));
                        row.Append(new NumberCenterCell(xAxis.Next, inactive.ToString(), rowIndex));

                        percentage = total == 0 ? 0f : (inactive + 0.0f) / total;
                        row.Append(new PercentageCenterCell(xAxis.Next, Math.Round(percentage, 2), rowIndex));
                        sheetData.Append(row);
                    }

                    if (addTotalRow)
                    {
                        rowIndex++;
                        xAxis.Reset();
                        xAxis.Step(1);
                        row = new Row() { RowIndex = new UInt32Value((uint)rowIndex) };
                        row.Append(new HeaderCell(xAxis.Next, "Total", rowIndex));

                        row.Append(new NumberBCenterCell(xAxis.Next, totalActive.ToString(), rowIndex));
                        row.Append(new NumberBCenterCell(xAxis.Next, totalInactive.ToString(), rowIndex));

                        total = totalAll;


                        percentage = total == 0 ? 0 : (totalInactive + 0f) / total;
                        row.Append(new PercentageBCenterCell(xAxis.Next, Math.Round(percentage, 2), rowIndex));
                        sheetData.Append(row);
                    }
                }
            }

            communityIds = communityTeacherList.Where(r => r.CoachId != 0).Select(r => r.CommunityId).Distinct().ToList();

            foreach (int id in communityIds)
            {
                xAxis.Reset();
                rowIndex++;
                row = new Row() { RowIndex = new UInt32Value((uint)rowIndex) };

                var coachNames = communityTeacherList.Where(r => r.CommunityId == id && r.CoachId != 0).Select(r => r.CoachName).Distinct();
                CountTeacherbyCommunityModel model = communityTeacherList.Find(r => r.CommunityId == id);
                totalActive = communityTeacherList.Where(r => r.CommunityId == id && r.CoachId != 0 && r.Status == EntityStatus.Active).Sum(r=>r.Total);
                totalInactive = communityTeacherList.Where(r => r.CommunityId == id && r.CoachId != 0 && r.Status == EntityStatus.Inactive).Sum(r => r.Total);

                row.Append(new CoordinatorCell(xAxis.Next, string.Join(", ", coachNames), rowIndex));
                row.Append(new TextCell(xAxis.Next, string.Join(", ", model.CommunityName), rowIndex));
                row.Append(new NumberCenterCell(xAxis.Next, totalActive.ToString(), rowIndex));
                row.Append(new NumberCenterCell(xAxis.Next, totalInactive.ToString(), rowIndex));

                total = totalAll;

                percentage = total == 0 ? 0 : (totalInactive + 0f) / total;
                row.Append(new PercentageBCenterCell(xAxis.Next, Math.Round(percentage, 2), rowIndex));
                sheetData.Append(row);
            }

            if (addTotalRow)
            {
                rowIndex++;
                xAxis.Reset();
                xAxis.Step(1);
                row = new Row() { RowIndex = new UInt32Value((uint)rowIndex) };
                row.Append(new HeaderCell(xAxis.Next, "Total", rowIndex));

                totalActive = communityTeacherList.Where(r => r.CoachId != 0 && r.Status == EntityStatus.Active).Sum(r => r.Total);
                totalInactive = communityTeacherList.Where(r => r.CoachId != 0 && r.Status == EntityStatus.Inactive).Sum(r => r.Total);

                row.Append(new NumberBCenterCell(xAxis.Next, totalActive.ToString(), rowIndex));
                row.Append(new NumberBCenterCell(xAxis.Next, totalInactive.ToString(), rowIndex));

                    total = totalAll;


                percentage = total == 0 ? 0 : (totalInactive + 0f) / total;
                row.Append(new PercentageBCenterCell(xAxis.Next, Math.Round(percentage, 2), rowIndex));
                sheetData.Append(row);
            }
        }
    }
}
