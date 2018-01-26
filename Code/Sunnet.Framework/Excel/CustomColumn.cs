using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Framework.Excel
{
    public class CustomColumn : Column
    {
        public CustomColumn(UInt32 startColumnIndex,
              UInt32 endColumnIndex, double columnWidth)
        {
            this.Min = startColumnIndex;
            this.Max = endColumnIndex;
            this.Width = columnWidth;
            this.CustomWidth = true;
        }
    }
}
