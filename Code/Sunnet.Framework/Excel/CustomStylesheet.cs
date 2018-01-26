using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Framework.Excel
{
    public class CustomStylesheet : Stylesheet
    {
        /// <summary>
        /// 需要将 DocumentFormat.OpenXml.dll 复制到 web 站点的dll 目录下
        /// </summary>
        public CustomStylesheet()
        {
            var fonts = new Fonts();
            //Font Index 0 11号字
            var font = new DocumentFormat.OpenXml.Spreadsheet.Font();
            var fontName = new FontName { Val = StringValue.FromString("Calibri") };
            var fontSize = new FontSize { Val = DoubleValue.FromDouble(11) };
            font.FontName = fontName;
            font.FontSize = fontSize;
            fonts.Append(font);
            //Font Index 1  11号字加粗
            var font1 = new DocumentFormat.OpenXml.Spreadsheet.Font();
            fontName = new FontName { Val = StringValue.FromString("Calibri") };
            fontSize = new FontSize { Val = DoubleValue.FromDouble(11) };
            font1.FontName = fontName;
            font1.FontSize = fontSize;
            font1.Bold = new Bold();
            fonts.Append(font1);
            //Font Index 2 11号字斜体
            var font2 = new DocumentFormat.OpenXml.Spreadsheet.Font();
            fontName = new FontName { Val = StringValue.FromString("Calibri") };
            fontSize = new FontSize { Val = DoubleValue.FromDouble(11) };
            font2.FontName = fontName;
            font2.FontSize = fontSize;
            font2.Italic = new Italic();
            fonts.Append(font2);

            //Font Index 3  11号字斜体加粗
            var font3 = new DocumentFormat.OpenXml.Spreadsheet.Font();
            fontName = new FontName { Val = StringValue.FromString("Calibri") };
            fontSize = new FontSize { Val = DoubleValue.FromDouble(11) };
            font3.FontName = fontName;
            font3.FontSize = fontSize;
            font3.Italic = new Italic();
            font3.Bold = new Bold();
            fonts.Append(font3);

            AddFonts(fonts);

            fonts.Count = UInt32Value.FromUInt32((uint)fonts.ChildElements.Count);


            #region 填充
            //fillId,0总是None，1总是gray125，自定义的从fillid =2开始
            //http://msdn.microsoft.com/en-us/library/cc296089.aspx 颜色
            //Fill index  0
            var fills = new Fills();
            var fill = new Fill();
            var patternFill = new PatternFill { PatternType = PatternValues.None };
            fill.PatternFill = patternFill;
            fills.Append(fill);
            //Fill index  1
            fill = new Fill();
            patternFill = new PatternFill { PatternType = PatternValues.Gray125 };
            fill.PatternFill = patternFill;
            fills.Append(fill);

            //Fill index  2 浅绿色
            fill = new Fill();
            patternFill = new PatternFill
            {
                PatternType = PatternValues.Solid,
                ForegroundColor = new ForegroundColor()
            };
            patternFill.ForegroundColor =
               TranslateForeground(System.Drawing.Color.FromArgb(204, 255, 204));
            patternFill.BackgroundColor =
                new BackgroundColor { Rgb = patternFill.ForegroundColor.Rgb };
            fill.PatternFill = patternFill;
            fills.Append(fill);
            //Fill index  3  灰色
            fill = new Fill();
            patternFill = new PatternFill
            {
                PatternType = PatternValues.Solid,
                ForegroundColor = new ForegroundColor()
            };
            patternFill.ForegroundColor =
               TranslateForeground(System.Drawing.Color.FromArgb(192, 192, 192));
            patternFill.BackgroundColor =
                new BackgroundColor { Rgb = patternFill.ForegroundColor.Rgb };
            fill.PatternFill = patternFill;
            fills.Append(fill);
            //Fill index  4  浅橙色
            fill = new Fill();
            patternFill = new PatternFill
            {
                PatternType = PatternValues.Solid,
                ForegroundColor = new ForegroundColor()
            };
            patternFill.ForegroundColor =
               TranslateForeground(System.Drawing.Color.FromArgb(255, 204, 153));
            patternFill.BackgroundColor =
                new BackgroundColor { Rgb = patternFill.ForegroundColor.Rgb };
            fill.PatternFill = patternFill;
            fills.Append(fill);
            //Fill index  5 黄色   255 255 0
            fill = new Fill();
            patternFill = new PatternFill
            {
                PatternType = PatternValues.Solid,
                ForegroundColor = new ForegroundColor()
            };
            patternFill.ForegroundColor =
               TranslateForeground(System.Drawing.Color.FromArgb(255, 255, 0));
            patternFill.BackgroundColor =
                new BackgroundColor { Rgb = patternFill.ForegroundColor.Rgb };
            fill.PatternFill = patternFill;
            fills.Append(fill);
            //Fill index  6
            fill = new Fill();
            patternFill = new PatternFill
            {
                PatternType = PatternValues.Solid,
                ForegroundColor = new ForegroundColor()
            };
            patternFill.ForegroundColor =
               TranslateForeground(System.Drawing.Color.Red);
            patternFill.BackgroundColor =
                new BackgroundColor { Rgb = patternFill.ForegroundColor.Rgb };
            fill.PatternFill = patternFill;
            fills.Append(fill);

            AddFills(fills);

            fills.Count = UInt32Value.FromUInt32((uint)fills.ChildElements.Count);
            #endregion //填充


            #region 边框
            var borders = new Borders();
            //All Boarder Index 0  无边框
            var border = new Border
            {
                LeftBorder = new LeftBorder(),
                RightBorder = new RightBorder(),
                TopBorder = new TopBorder(),
                BottomBorder = new BottomBorder(),
                DiagonalBorder = new DiagonalBorder()
            };
            borders.Append(border);
            //All Boarder Index 1  有边框
            border = new Border
            {
                LeftBorder = new LeftBorder { Style = BorderStyleValues.Thin },
                RightBorder = new RightBorder { Style = BorderStyleValues.Thin },
                TopBorder = new TopBorder { Style = BorderStyleValues.Thin },
                BottomBorder = new BottomBorder { Style = BorderStyleValues.Thin },
                DiagonalBorder = new DiagonalBorder()
            };
            borders.Append(border);

            //All Boarder Index 2  上边框
            border = new Border
            {
                LeftBorder = new LeftBorder(),
                RightBorder = new RightBorder(),
                TopBorder = new TopBorder { Style = BorderStyleValues.Thin },
                BottomBorder = new BottomBorder(),
                DiagonalBorder = new DiagonalBorder()
            };
            borders.Append(border);

            //All Boarder Index 3  右边框
            border = new Border
            {
                LeftBorder = new LeftBorder(),
                RightBorder = new RightBorder { Style = BorderStyleValues.Thin },
                TopBorder = new TopBorder(),
                BottomBorder = new BottomBorder(),
                DiagonalBorder = new DiagonalBorder()
            };
            borders.Append(border);

            //All Boarder Index 4  下边框
            border = new Border
            {
                LeftBorder = new LeftBorder(),
                RightBorder = new RightBorder(),
                TopBorder = new TopBorder(),
                BottomBorder = new BottomBorder { Style = BorderStyleValues.Thin },
                DiagonalBorder = new DiagonalBorder()
            };
            borders.Append(border);

            //All Boarder Index 5  左边框
            border = new Border
            {
                LeftBorder = new LeftBorder { Style = BorderStyleValues.Thin },
                RightBorder = new RightBorder(),
                TopBorder = new TopBorder(),
                BottomBorder = new BottomBorder(),
                DiagonalBorder = new DiagonalBorder()
            };
            borders.Append(border);

            borders.Count = UInt32Value.FromUInt32((uint)borders.ChildElements.Count);
            #endregion  //边框


            #region Formats
            var cellStyleFormats = new CellStyleFormats();

            //index 0             
            var cellFormat = new CellFormat
            {
                NumberFormatId = 0,
                FontId = 0,
                FillId = 0,
                BorderId = 0
            };

            cellStyleFormats.Append(cellFormat);
            cellStyleFormats.Count =
               UInt32Value.FromUInt32((uint)cellStyleFormats.ChildElements.Count);


            uint iExcelIndex = 164;
            var numberingFormats = new NumberingFormats();

            var cellFormats = new CellFormats();
            cellFormat = new CellFormat
            {
                NumberFormatId = 0,
                FontId = 0,
                FillId = 0,
                BorderId = 0,
                FormatId = 0
            };
            cellFormats.Append(cellFormat);
            var nformatDateTime = new NumberingFormat
            {
                NumberFormatId = UInt32Value.FromUInt32(iExcelIndex++), //164
                FormatCode = StringValue.FromString("MM/dd/yyyy hh:mm:ss")
            };
            numberingFormats.Append(nformatDateTime);
            var nformat4Decimal = new NumberingFormat
            {
                NumberFormatId = UInt32Value.FromUInt32(iExcelIndex++), //165
                FormatCode = StringValue.FromString("#,##0.0000")
            };
            numberingFormats.Append(nformat4Decimal);
            var nformat2Decimal = new NumberingFormat
            {
                NumberFormatId = UInt32Value.FromUInt32(iExcelIndex++), //166
                FormatCode = StringValue.FromString("#,##0.00")
            };
            numberingFormats.Append(nformat2Decimal);
            var nformatNumber = new NumberingFormat
            {
                NumberFormatId = UInt32Value.FromUInt32(iExcelIndex++), //167
                FormatCode = StringValue.FromString("#,##0")
            };
            numberingFormats.Append(nformatNumber);
            var nformatForcedText = new NumberingFormat
            {
                NumberFormatId = UInt32Value.FromUInt32(iExcelIndex++), //168
                FormatCode = StringValue.FromString("@")
            };
            numberingFormats.Append(nformatForcedText);

            NumberingFormat nf2percentage = new NumberingFormat();
            nf2percentage.NumberFormatId = UInt32Value.FromUInt32(iExcelIndex++); //169
            nf2percentage.FormatCode = StringValue.FromString("0%");
            numberingFormats.Append(nf2percentage);

            // index 1
            // Cell Standard Date format 
            var cellFormat1 = new CellFormat
            {
                NumberFormatId = 14, //14表示为日期
                FontId = 0,
                FillId = 0,
                BorderId = 1,
                FormatId = 0,
                ApplyNumberFormat = BooleanValue.FromBoolean(true)
            };
            cellFormats.Append(cellFormat1);
            // index 2
            // Cell Standard Date format 
            var cellFormat2 = new CellFormat
            {
                NumberFormatId = 14,
                FontId = 0,
                FillId = 0,
                BorderId = 1,
                FormatId = 0,
                ApplyNumberFormat = BooleanValue.FromBoolean(true)
            };
            cellFormats.Append(cellFormat2);
            // Index 3
            // Cell Standard Number format with 2 decimal placing
            var cellFormat3 = new CellFormat
            {
                NumberFormatId = 4,
                FontId = 0,
                FillId = 0,
                BorderId = 1,
                FormatId = 0,
                ApplyNumberFormat = BooleanValue.FromBoolean(true)
            };
            cellFormats.Append(cellFormat3);
            // Index 4
            // Cell Date time custom format
            var cellFormat4 = new CellFormat
            {
                NumberFormatId = nformatDateTime.NumberFormatId,
                FontId = 0,
                FillId = 0,
                BorderId = 1,
                FormatId = 0,
                ApplyNumberFormat = BooleanValue.FromBoolean(true)
            };
            cellFormats.Append(cellFormat4);
            // Index 5
            // Cell 4 decimal custom format
            var cellFormat5 = new CellFormat
            {
                NumberFormatId = nformat4Decimal.NumberFormatId,
                FontId = 0,
                FillId = 0,
                BorderId = 1,
                FormatId = 0,
                ApplyNumberFormat = BooleanValue.FromBoolean(true)
            };
            cellFormats.Append(cellFormat5);
            // Index 6
            // Cell 2 decimal custom format
            var cellFormat6 = new CellFormat
            {
                NumberFormatId = nformat2Decimal.NumberFormatId,
                FontId = 0,
                FillId = 0,
                BorderId = 1,
                FormatId = 0,
                ApplyNumberFormat = BooleanValue.FromBoolean(true)
            };
            cellFormats.Append(cellFormat6);
            // Index 7
            // Cell forced number text custom format
            var cellFormat7 = new CellFormat
            {
                NumberFormatId = nformatNumber.NumberFormatId,
                FontId = 0,
                FillId = 0,
                BorderId = 1,
                FormatId = 0,
                ApplyNumberFormat = BooleanValue.FromBoolean(true)
            };
            cellFormats.Append(cellFormat7);
            // index 8
            // Cell Header 粗体
            var cellFormat8 = new CellFormat
            {
                NumberFormatId = nformatForcedText.NumberFormatId,
                FontId = 1, //粗体
                FillId = 0, //无填充色
                BorderId = 1, //有边框
                FormatId = 0,
                ApplyNumberFormat = BooleanValue.FromBoolean(true)
            };
            cellFormats.Append(cellFormat8);

            // index 9
            // Cell Header 粗体加斜体
            var cellFormat9 = new CellFormat
            {
                NumberFormatId = nformatForcedText.NumberFormatId,
                FontId = 3, //粗体加斜体
                FillId = 0, //无填充色
                BorderId = 1, //有边框
                FormatId = 0,
                ApplyNumberFormat = BooleanValue.FromBoolean(true)
            };
            cellFormats.Append(cellFormat9);


            // index 10
            // 百分比
            var cellFormat10 = new CellFormat
            {
                NumberFormatId = nf2percentage.NumberFormatId,
                FontId = 0,
                FillId = 0,
                BorderId = 1,
                FormatId = 0,
                ApplyNumberFormat = BooleanValue.FromBoolean(true)
            };
            cellFormats.Append(cellFormat10);


            // add all cell formats
            numberingFormats.Count =
             UInt32Value.FromUInt32((uint)numberingFormats.ChildElements.Count);
            cellFormats.Count = UInt32Value.FromUInt32((uint)cellFormats.ChildElements.Count);
            AddCellFormats(cellFormats);
            #endregion //Formats

            this.Append(numberingFormats);
            this.Append(fonts);
            this.Append(fills);
            this.Append(borders);
            this.Append(cellStyleFormats);
            this.Append(cellFormats);
            var css = new CellStyles();
            var cs = new CellStyle
            {
                Name = StringValue.FromString("Normal"),
                FormatId = 0,
                BuiltinId = 0
            };
            css.Append(cs);
            css.Count = UInt32Value.FromUInt32((uint)css.ChildElements.Count);
            this.Append(css);
            var dfs = new DifferentialFormats { Count = 0 };
            this.Append(dfs);
            var tss = new TableStyles
            {
                Count = 0,
                DefaultTableStyle = StringValue.FromString("TableStyleMedium9"),
                DefaultPivotStyle = StringValue.FromString("PivotStyleLight16")
            };
            this.Append(tss);
        }

        public static ForegroundColor TranslateForeground(System.Drawing.Color fillColor)
        {
            return new ForegroundColor()
            {
                Rgb = new HexBinaryValue()
                {
                    Value =
                        System.Drawing.ColorTranslator.ToHtml(
                        System.Drawing.Color.FromArgb(
                            fillColor.A,
                            fillColor.R,
                            fillColor.G,
                            fillColor.B)).Replace("#", "")
                }
            };
        }

        public virtual void AddFonts(Fonts fonts)
        {
        }


        /// <summary>
        /// 从8开始
        /// </summary>
        public virtual void AddFills(Fills fills)
        {
        }

        /// <summary>
        /// 新加的从11开始
        /// </summary>
        /// <param name="cellFormats"></param>
        public virtual void AddCellFormats(CellFormats cellFormats)
        {
        }
    }
}
