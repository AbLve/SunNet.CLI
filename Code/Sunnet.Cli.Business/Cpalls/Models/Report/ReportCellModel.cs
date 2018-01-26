using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Drawing;


/**************************************************************************
 * Developer: 		JackZhang
 * Computer:		JACKZ
 * Domain:			Jackz
 * CreatedOn:		2014/10/9 3:41:38
 * Description:		Please input class summary
 * Version History:	Created,2014/10/9 3:41:38
 * 
 * 
 **************************************************************************/

namespace Sunnet.Cli.Business.Cpalls.Models.Report
{
    public class ReportCellModel
    {
        private bool _isParent;

        public ReportCellModel()
        {
            Rowspan = 1;
            Colspan = 1;
        }
        public ReportCellModel(object text)
            : this()
        {
            Text = text;
        }
        public ReportCellModel(object text, string description)
            : this()
        {
            Text = text;
            Description = description;
        }
        public ReportCellModel(object text, string alertText, string color)
            : this(text)
        {
            Color = color;
            AlertText = alertText;
        }
        public ReportCellModel(object text, int colspan)
            : this(text)
        {
            Colspan = colspan;
        }
        public ReportCellModel(object text, int colspan, int rowspan)
            : this(text, colspan)
        {
            Rowspan = rowspan;
        }

        public object Text { get; set; }
        public int Rowspan { get; set; }
        public int Colspan { get; set; }
        public string Description { get; set; }

        public bool IsParent
        {
            get { return _isParent; }
            set { _isParent = value; }
        }

        public string Background { get; set; }

        public string AlertText { get; set; }

        public string Color { get; set; }

    }
}
