using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Spreadsheet;
using Sunnet.Framework.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Cli.Business.Reports.Model
{
    /// <summary>
    /// Number 加粗 居中
    /// </summary>
    internal class NumberBCenterCell : Cell
    {
        public NumberBCenterCell(string header, string text, int index)
        {
            this.DataType = CellValues.Number;
            this.CellReference = header + index;
            this.CellValue = new CellValue(text);
            this.StyleIndex = 11;
        }
    }


    /// <summary>
    /// PercentageCell 加粗 居中
    /// </summary>
    internal class PercentageBCenterCell : PercentageCell
    {
        public PercentageBCenterCell(string header, object text, int index)
            : base(header, text, index)
        {
            this.StyleIndex = 12;
        }
    }

    /// <summary>
    /// PercentageCell 加粗 居中
    /// </summary>
    internal class NumberCenterCell : PercentageCell
    {
        public NumberCenterCell(string header, object text, int index)
            : base(header, text, index)
        {
            this.StyleIndex = 13;
        }
    }


    /// <summary>
    /// PercentageCell 居中
    /// </summary>
    internal class PercentageCenterCell : PercentageCell
    {
        public PercentageCenterCell(string header, object text, int index)
            : base(header, text, index)
        {
            this.StyleIndex = 14;
        }
    }


    public class CoordinatorCell : Cell
    {
        public CoordinatorCell(string header, string text, int index)
        {
            this.DataType = CellValues.InlineString;
            this.CellReference = header + index;
            //Add text to the text cell.
            this.InlineString = new InlineString { Text = new Text { Text = text } };
            this.StyleIndex = 15;
        }
    }


    internal class TeacherTrunoverStylesheet : CustomStylesheet
    {
        public override void AddCellFormats(CellFormats cellFormats)
        {
            // Index 11 Number Bold Center
            var cellFormat11 = new CellFormat
            {
                NumberFormatId = UInt32Value.FromUInt32(167),
                FontId = 1,
                FillId = 0,
                BorderId = 1,
                FormatId = 0,
                ApplyNumberFormat = BooleanValue.FromBoolean(true),
                Alignment = new Alignment()
                {
                    Horizontal = HorizontalAlignmentValues.Center
                }
            };
            cellFormats.Append(cellFormat11);

            // Index 12 PercentageCell Bold Center
            var cellFormat12 = new CellFormat
            {
                NumberFormatId = UInt32Value.FromUInt32(169),
                FontId = 1,
                FillId = 0,
                BorderId = 1,
                FormatId = 0,
                ApplyNumberFormat = BooleanValue.FromBoolean(true),
                Alignment = new Alignment()
                {
                    Horizontal = HorizontalAlignmentValues.Center
                }
            };
            cellFormats.Append(cellFormat12);

            // Index 13 Number Center
            var cellFormat13 = new CellFormat
            {
                NumberFormatId = UInt32Value.FromUInt32(167),
                FontId = 0,
                FillId = 0,
                BorderId = 1,
                FormatId = 0,
                ApplyNumberFormat = BooleanValue.FromBoolean(true),
                Alignment = new Alignment()
                {
                    Horizontal = HorizontalAlignmentValues.Center
                }
            };
            cellFormats.Append(cellFormat13);

            // Index 14 PercentageCell  Center
            var cellFormat14 = new CellFormat
            {
                NumberFormatId = UInt32Value.FromUInt32(169),
                FontId = 0,
                FillId = 0,
                BorderId = 1,
                FormatId = 0,
                ApplyNumberFormat = BooleanValue.FromBoolean(true),
                Alignment = new Alignment()
                {
                    Horizontal = HorizontalAlignmentValues.Center
                }
            };
            cellFormats.Append(cellFormat14);

            // Index 15 Coordinator  
            var cellFormat15= new CellFormat
            {
                NumberFormatId = UInt32Value.FromUInt32(167),
                FontId = 0,
                FillId = 0,
                BorderId = 1,
                FormatId = 0,
                ApplyNumberFormat = BooleanValue.FromBoolean(true),
                Alignment = new Alignment()
                {
                   WrapText = true
                }
            };
            cellFormats.Append(cellFormat15);
        }
    }
}