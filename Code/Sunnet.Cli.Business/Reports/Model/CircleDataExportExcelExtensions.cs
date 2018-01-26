using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Spreadsheet;
using Sunnet.Framework.Excel;

namespace Sunnet.Cli.Business.Reports.Model
{

    public class NoBorderTextCell : Cell
    {
        public NoBorderTextCell(string header, string text, int index)
        {
            this.DataType = CellValues.InlineString;
            this.CellReference = header + index;
            //Add text to the text cell.
            this.InlineString = new InlineString { Text = new Text { Text = text } };
            this.StyleIndex = 15;
        }
    }

    public class TopBorderTextCell : Cell
    {
        public TopBorderTextCell(string header, string text, int index)
        {
            this.DataType = CellValues.InlineString;
            this.CellReference = header + index;
            //Add text to the text cell.
            this.InlineString = new InlineString { Text = new Text { Text = text } };
            this.StyleIndex = 11;
        }
    }

    public class RightBorderTextCell : Cell
    {
        public RightBorderTextCell(string header, string text, int index)
        {
            this.DataType = CellValues.InlineString;
            this.CellReference = header + index;
            //Add text to the text cell.
            this.InlineString = new InlineString { Text = new Text { Text = text } };
            this.StyleIndex = 12;
        }
    }

    public class BottomBorderTextCell : Cell
    {
        public BottomBorderTextCell(string header, string text, int index)
        {
            this.DataType = CellValues.InlineString;
            this.CellReference = header + index;
            //Add text to the text cell.
            this.InlineString = new InlineString { Text = new Text { Text = text } };
            this.StyleIndex = 13;
        }
    }

    public class LeftBorderTextCell : Cell
    {
        public LeftBorderTextCell(string header, string text, int index)
        {
            this.DataType = CellValues.InlineString;
            this.CellReference = header + index;
            //Add text to the text cell.
            this.InlineString = new InlineString { Text = new Text { Text = text } };
            this.StyleIndex = 14;
        }
    }

    internal class HorizontalCenterTextCell : TextCell
    {
        public HorizontalCenterTextCell(string header, string text, int index)
            : base(header, text, index)
        {
            this.StyleIndex = 16;
        }
    }

    public class LeftBorderGoalCenterTextCell : Cell
    {
        public LeftBorderGoalCenterTextCell(string header, object text, int index)
        {
            this.DataType = CellValues.Number;
            this.CellReference = header + index;
            this.CellValue = new CellValue(text.ToString());
            this.StyleIndex = 17;
        }
    }


    public class NoBorderItemResultTextCell : Cell
    {
        public NoBorderItemResultTextCell(string header, object text, int index)
        {
            this.DataType = CellValues.Number;
            this.CellReference = header + index;
            this.CellValue = new CellValue(text.ToString());
            this.StyleIndex = 18;
        }
    }


    public class RightBorderItemResultTextCell : Cell
    {
        public RightBorderItemResultTextCell(string header, object text, int index)
        {
            this.DataType = CellValues.Number;
            this.CellReference = header + index;
            this.CellValue = new CellValue(text.ToString());
            this.StyleIndex = 19;
        }
    }

    internal class CircleDataExportExcelStylesheet : CustomStylesheet
    {
        public override void AddCellFormats(CellFormats cellFormats)
        {
            var numberD2 = new NumberingFormat
            {
                NumberFormatId = UInt32Value.FromUInt32(166)
            };

            var numberD0 = new NumberingFormat
            {
                NumberFormatId = UInt32Value.FromUInt32(167)
            };

            // Index: 11, Top Border
            var cellFormat11 = new CellFormat
            {
                FontId = 0,
                FillId = 0,
                BorderId = 2,
                FormatId = 0
            };
            cellFormats.Append(cellFormat11);

            // Index: 12, Right Border
            var cellFormat12 = new CellFormat
            {
                FontId = 0,
                FillId = 0,
                BorderId = 3,
                FormatId = 0
            };
            cellFormats.Append(cellFormat12);

            // Index: 13, Bottom Border
            var cellFormat13 = new CellFormat
            {
                FontId = 0,
                FillId = 0,
                BorderId = 4,
                FormatId = 0
            };
            cellFormats.Append(cellFormat13);


            // Index: 14, Left Border
            var cellFormat14 = new CellFormat
            {
                FontId = 0,
                FillId = 0,
                BorderId = 5,
                FormatId = 0
            };
            cellFormats.Append(cellFormat14);

            // Index 15 No Border
            var cellFormat15 = new CellFormat
            {
                FontId = 0,
                FillId = 0,
                BorderId = 0,
                FormatId = 0
            };
            cellFormats.Append(cellFormat15);

            // Index 16 Center Hol
            var cellFormat16 = new CellFormat
            {
                NumberFormatId = numberD2.NumberFormatId,
                FontId = 0,
                FillId = 0,
                BorderId = 1, // All border
                FormatId = 0,
                ApplyNumberFormat = BooleanValue.FromBoolean(true),
                Alignment = new Alignment()
                {
                    Horizontal = HorizontalAlignmentValues.Center
                }
            };
            cellFormats.Append(cellFormat16);

            // Index: 17, Center, Number, Left Border
            var cellFormat17 = new CellFormat
            {
                NumberFormatId = numberD2.NumberFormatId,
                FontId = 0,
                FillId = 0,
                BorderId = 5,
                FormatId = 0,
                ApplyNumberFormat = BooleanValue.FromBoolean(true),
                Alignment = new Alignment()
                {
                    Horizontal = HorizontalAlignmentValues.Center
                }
            };
            cellFormats.Append(cellFormat17);

            // Index: 18, Center, Number, Bottom Border
            var cellFormat18 = new CellFormat
            {
                NumberFormatId = numberD0.NumberFormatId,
                FontId = 0,
                FillId = 0,
                BorderId = 0,
                FormatId = 0,
                ApplyNumberFormat = BooleanValue.FromBoolean(true),
                Alignment = new Alignment()
                {
                    Horizontal = HorizontalAlignmentValues.Center
                }
            };
            cellFormats.Append(cellFormat18);


            // Index: 19, Center, Number, Right Border
            var cellFormat19 = new CellFormat
            {
                NumberFormatId = numberD0.NumberFormatId,
                FontId = 0,
                FillId = 0,
                BorderId = 3,
                FormatId = 0,
                ApplyNumberFormat = BooleanValue.FromBoolean(true),
                Alignment = new Alignment()
                {
                    Horizontal = HorizontalAlignmentValues.Center
                }
            };
            cellFormats.Append(cellFormat19);

        }
    }
}
