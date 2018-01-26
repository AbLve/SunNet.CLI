using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Spreadsheet;
using Sunnet.Framework.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Cli.Business.Reports.Model.CoachingHoursbyCommunity
{
    internal class CoachingHoursbyCommunityStylesheet : CustomStylesheet
    {
        public override void AddCellFormats(CellFormats cellFormats)
        {
            // Index 11 Center Hol
            var cellFormat11 = new CellFormat
            {
                NumberFormatId = 4,
                FontId = 1,
                FillId = 0,
                BorderId = 1, // All border
                FormatId = 0,
                ApplyNumberFormat = BooleanValue.FromBoolean(true),
                Alignment = new Alignment()
                {
                    Horizontal = HorizontalAlignmentValues.Center
                }
            };

            cellFormats.Append(cellFormat11);

            // Index 12 F2F Coaching
            var cellFormat12 = new CellFormat
            {
                NumberFormatId = 4,
                FontId = 1,
                FillId = 7,
                BorderId = 1, // All border
                FormatId = 0,
                ApplyNumberFormat = BooleanValue.FromBoolean(true),
                Alignment = new Alignment()
                {
                    Horizontal = HorizontalAlignmentValues.Center
                }
            };
            cellFormats.Append(cellFormat12);

            // Index 13  Remote Cycles
            var cellFormat13 = new CellFormat
            {
                NumberFormatId = 4,
                FontId = 1,
                FillId = 4,
                BorderId = 1, // All border
                FormatId = 0,
                ApplyNumberFormat = BooleanValue.FromBoolean(true),
                Alignment = new Alignment()
                {
                    Horizontal = HorizontalAlignmentValues.Center
                }
            };
            cellFormats.Append(cellFormat13);

            // Index 14  黄色填充 数字 两位小数
            var cellFormat14 = new CellFormat
            {
                NumberFormatId = UInt32Value.FromUInt32(166),
                FontId = 0,
                FillId = 5,
                BorderId = 1, // All border
                FormatId = 0,
                ApplyNumberFormat = BooleanValue.FromBoolean(true)
            };
            cellFormats.Append(cellFormat14);


            // Index 15  灰色文本
            var cellFormat15 = new CellFormat
            {
                NumberFormatId = UInt32Value.FromUInt32(166),
                FontId = 0,
                FillId = 3,
                BorderId = 1, // All border
                FormatId = 0,
                ApplyNumberFormat = BooleanValue.FromBoolean(true)
            };
            cellFormats.Append(cellFormat15);

            // Index 16  蓝绿色填充数字
            var cellFormat16 = new CellFormat
            {
                NumberFormatId = UInt32Value.FromUInt32(166),
                FontId = 0,
                FillId = 8,
                BorderId = 1, // All border
                FormatId = 0,
                ApplyNumberFormat = BooleanValue.FromBoolean(true)
            };
            cellFormats.Append(cellFormat16);

            // Index 17  蓝绿色填充数字
            var cellFormat17 = new CellFormat
            {
                NumberFormatId = UInt32Value.FromUInt32(166),
                FontId = 0,
                FillId = 3,
                BorderId = 1, // All border
                FormatId = 0,
                ApplyNumberFormat = BooleanValue.FromBoolean(true),
                Alignment = new Alignment()
                {
                    Horizontal = HorizontalAlignmentValues.Center
                }
            };
            cellFormats.Append(cellFormat17);


            // Index 18  数据居中显示
            var cellFormat18 = new CellFormat
            {
                NumberFormatId = UInt32Value.FromUInt32(166),
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
            cellFormats.Append(cellFormat18);

            // Index 19  数据居中显示
            var cellFormat19 = new CellFormat
            {
                NumberFormatId = UInt32Value.FromUInt32(167),
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
            cellFormats.Append(cellFormat19);

        }

        public override void AddFills(Fills fills)
        {
            //Fill index  7 淡紫色 F2F Coaching 表头
            Fill fill = new Fill();
            var patternFill = new PatternFill
            {
                PatternType = PatternValues.Solid,
                ForegroundColor = new ForegroundColor()
            };
            patternFill.ForegroundColor =
     CustomStylesheet.TranslateForeground(System.Drawing.Color.FromArgb(227, 224, 238));
            patternFill.BackgroundColor =
                new BackgroundColor { Rgb = patternFill.ForegroundColor.Rgb };
            fill.PatternFill = patternFill;
            fills.Append(fill);

            //Fill index  8 蓝色偏绿 Read from the Teacher pages
            fill = new Fill();
            patternFill = new PatternFill
            {
                PatternType = PatternValues.Solid,
                ForegroundColor = new ForegroundColor()
            };
            patternFill.ForegroundColor =
     CustomStylesheet.TranslateForeground(System.Drawing.Color.FromArgb(144, 208, 217));
            patternFill.BackgroundColor =
                new BackgroundColor { Rgb = patternFill.ForegroundColor.Rgb };
            fill.PatternFill = patternFill;
            fills.Append(fill);
        }



    }

    internal class CenterHeaderCell : TextCell
    {
        public CenterHeaderCell(string header, string text, int index) :
            base(header, text, index)
        {
            this.StyleIndex = 11;
        }
    }

    internal class F2FHeaderCell : TextCell
    {
        public F2FHeaderCell(string header, string text, int index) :
            base(header, text, index)
        {
            this.StyleIndex = 12;
        }
    }

    internal class RemoteHeaderCell : TextCell
    {
        public RemoteHeaderCell(string header, string text, int index) :
            base(header, text, index)
        {
            this.StyleIndex = 13;
        }
    }

    /// <summary>
    /// Read from the Coaches pages
    /// </summary>
    internal class NumberYellowCell : Cell
    {
        public NumberYellowCell(string header, string text, int index)
        {
            this.DataType = CellValues.Number;
            this.CellReference = header + index;
            this.CellValue = new CellValue(text);
            this.StyleIndex = 14;
        }
    }

    /// <summary>
    /// Read from the Teacher pages
    /// </summary>
    internal class NumberBlueCell : Cell
    {
        public NumberBlueCell(string header, string text, int index)
        {
            this.DataType = CellValues.Number;
            this.CellReference = header + index;
            this.CellValue = new CellValue(text);
            this.StyleIndex = 16;
        }
    }

    /// <summary>
    /// Grey out box if no calculation for F2F Coaching or Remote Cycles
    /// </summary>
    internal class EmptyFillCell : Cell
    {
        public EmptyFillCell()
        {
            this.StyleIndex = 15;
        }
    }

    /// <summary>
    /// Read from the Teacher pages
    /// </summary>
    internal class NumberGreyCell : Cell
    {
        public NumberGreyCell(string header, string text, int index)
        {
            this.DataType = CellValues.Number;
            this.CellReference = header + index;
            this.CellValue = new CellValue(text);
            this.StyleIndex = 17;
        }
    }


    //Cell 2 decimal custom format 居中显示
    internal class NumberD2CenterCell : Cell
    {
        public NumberD2CenterCell(string header, string text, int index)
        {
            this.DataType = CellValues.Number;
            this.CellReference = header + index;
            this.CellValue = new CellValue(text);
            this.StyleIndex = 18;
        }
    }

    //Cell 2 decimal custom format 居中显示
    internal class NumberCenterCell : Cell
    {
        public NumberCenterCell(string header, string text, int index)
        {
            this.DataType = CellValues.Number;
            this.CellReference = header + index;
            this.CellValue = new CellValue(text);
            this.StyleIndex = 19;
        }
    }
}