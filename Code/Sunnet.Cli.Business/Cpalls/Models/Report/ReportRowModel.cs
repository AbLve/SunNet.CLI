using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


/**************************************************************************
 * Developer: 		JackZhang
 * Computer:		JACKZ
 * Domain:			Jackz
 * CreatedOn:		2014/10/9 3:39:18
 * Description:		Please input class summary
 * Version History:	Created,2014/10/9 3:39:18
 * 
 * 
 **************************************************************************/

namespace Sunnet.Cli.Business.Cpalls.Models.Report
{
    public class ReportRowModel
    {
        public void Add(ReportCellModel model)
        {
            this.Cells.Add(model);
        }
        public void Add(object text)
        {
            this.Add(new ReportCellModel(text));
        }
        public void Add(object text, string description)
        {
            this.Add(new ReportCellModel(text, description));
        }
        public void Add(object text, string alertText, string color)
        {
            this.Add(new ReportCellModel(text, alertText, color));
        }
        public void Add(object text, int colspan)
        {
            this.Add(new ReportCellModel(text, colspan));
        }
        public void Add(object text, int colspan, int rowspan, string des = "")
        {
            this.Add(new ReportCellModel(text, colspan, rowspan) { Description = des });
        }
        public void Add(object text, int colspan, int rowspan, bool isParent)
        {
            this.Add(new ReportCellModel(text, colspan, rowspan) { IsParent = isParent });
        }
        public void Add(object text, int colspan, int rowspan, bool isParent, string des)
        {
            this.Add(new ReportCellModel(text, colspan, rowspan) { IsParent = isParent, Description = des });
        }
        public void SetBGColorofLastCell(string background)
        {
            if (_cells.Any())
                _cells.Last().Background = background;
        }

        private List<ReportCellModel> _cells;

        public List<ReportCellModel> Cells
        {
            get { return _cells ?? (_cells = new List<ReportCellModel>()); }
            set { _cells = value; }
        }

        public int Columns
        {
            get
            {
                var columns = 0;
                this.Cells.ForEach(x => columns += x.Colspan);
                return columns;
            }
        }

        public ReportCellModel this[int index]
        {
            get
            {
                if (Cells.Count >= index + 1)
                    return Cells[index];
                return null;
            }
        }

        public int MeasureId { get; set; }
        public int ParentMeasureId { get; set; }

    }
}
