using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Framework.Excel
{
    public class EmptyCell : Cell
    {
        public EmptyCell()
        {
            this.StyleIndex = 7;
        }
    }


    public class TextCell : Cell
    {
        public TextCell(string header, string text, int index)
        {
            this.DataType = CellValues.InlineString;
            this.CellReference = header + index;
            //Add text to the text cell.
            this.InlineString = new InlineString { Text = new Text { Text = text } };
            this.StyleIndex = 7;
        }
    }

    public class NumberCell : Cell
    {
        public NumberCell(string header, string text, int index)
        {
            this.DataType = CellValues.Number;
            this.CellReference = header + index;
            this.CellValue = new CellValue(text);
            this.StyleIndex = 7;
        }
    }

    public class PercentageCell : Cell
    {
        public PercentageCell(string header, object text, int index)
        {
            this.DataType = CellValues.Number;
            this.CellReference = header + index;
            this.CellValue = new CellValue(text.ToString());
            this.StyleIndex = 10;
        }
    }

    //Cell 2 decimal custom format
    public class NumberD2Cell : Cell
    {
        public NumberD2Cell(string header, string text, int index)
        {
            this.DataType = CellValues.Number;
            this.CellReference = header + index;
            this.CellValue = new CellValue(text);
            this.StyleIndex = 6;
        }
    }

    public class FormatedNumberCell : NumberCell
    {
        public FormatedNumberCell(string header, string text, int index)
            : base(header, text, index)
        {
            this.StyleIndex = 3;
        }
    }

    public class DateCell : Cell
    {
        public DateCell(string header, DateTime dateTime, int index)
        {
            this.StyleIndex = 1;
            CellValue cellValue = new CellValue();
            cellValue.Text = dateTime.ToOADate().ToString();
            this.Append(cellValue);
        }
    }

    /// <summary>
    /// 粗体
    /// </summary>
    public class HeaderCell : TextCell
    {
        public HeaderCell(string header, string text, int index) :
            base(header, text, index)
        {
            this.StyleIndex = 8;
        }
    }
}
