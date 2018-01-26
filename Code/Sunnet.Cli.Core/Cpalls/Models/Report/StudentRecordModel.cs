using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		JackZhang
 * Computer:		JACKZ
 * Domain:			Jackz
 * CreatedOn:		2014/10/12 21:31:26
 * Description:		Please input class summary
 * Version History:	Created,2014/10/12 21:31:26
 * 
 * 
 **************************************************************************/

namespace Sunnet.Cli.Core.Cpalls.Models.Report
{
    public class StudentRecordModel
    {
        private decimal _goal;

        public int SchoolId { get; set; }

        public List<int> SchoolIds { get; set; }

        public int StudentId { get; set; }

        public int MeasureId { get; set; }
        /// <summary>
        /// Not Mapped
        /// </summary>
        public int ParentId { get; set; }
        public Wave Wave { get; set; }

        public decimal Goal
        {
            get
            {
                if (_goal < 0)
                    _goal = 0;
                return _goal;
            }
            set { _goal = value; }
        }

        public decimal Benchmark { get; set; }

        public string Comment { get; set; }

        public string PercentileRank { get; set; }

        public decimal LowerScore { get; set; }

        public decimal HigherScore { get; set; }

        public int BenchmarkId { get; set; }
    }
}
