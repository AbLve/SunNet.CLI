using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sunnet.Cli.Core.Cpalls;
using Sunnet.Cli.Core.Ade;

namespace Sunnet.Cli.Core.Reports.Models
{
    public class CircleDataExportStudentMeasureModel
    {
        private string _measureShortName;
        private List<CircleDataExportStudentItemModel> _items;
        private decimal _goal;
        public int AssessmentId { get; set; }

        public string SchoolYear { get; set; }

        public Wave Wave { get; set; }

        //public int CDId { get; set; }

        //public int SchoolId { get; set; }

        public int StudentId { get; set; }

        public DateTime BirthDay { get; set; }

        /// <summary>
        /// StudentMeasure Id
        /// </summary>
        public int ID { get; set; }

        public int MeasureId { get; set; }

        public string MeasureName { get; set; }

        public string MeasureShortName
        {
            get
            {
                if (string.IsNullOrEmpty(_measureShortName))
                {
                    _measureShortName = string.Join("", MeasureName.Where(x => x >= 'A' && x <= 'Z'));
                }
                return _measureShortName;
            }
            set { _measureShortName = value; }
        }

        public decimal Benchmark { get; set; }

        public decimal TotalScore { get; set; }

        public decimal Goal
        {
            get { return _goal >= 0 ? _goal : 0; }
            set { _goal = value; }
        }

        public int BenchmarkId { get; set; }

        public string LabelText { get; set; }

        public string Color { get; set; }

        public BlackWhiteStyle BW { get; set; }

        public DateTime UpdatedOn { get; set; }

        public List<CircleDataExportStudentItemModel> Items
        {
            get { return _items ?? (_items = new List<CircleDataExportStudentItemModel>()); }
            set { _items = value; }
        }
    }
}
