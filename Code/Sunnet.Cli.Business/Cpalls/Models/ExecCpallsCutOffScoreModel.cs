using Sunnet.Cli.Core.Ade;
using Sunnet.Cli.Core.Cpalls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Cli.Business.Cpalls.Models
{
    public class ExecCpallsCutOffScoreModel
    {
        public int ID { get; set; }
        public int MeasureId { get; set; }
        public int FromYear { get; set; }
        public int FromMonth { get; set; }
        public int ToYear { get; set; }
        public int ToMonth { get; set; }

        public double FromAge
        {
            get
            {
                int month = FromYear * 12 + FromMonth;
                return (double)((month * 10 / 12.00)) / 10.00;//年
            }
        }

        public double ToAge
        {
            get
            {
                int month = ToYear * 12 + ToMonth;
                return (double)((month * 10 / 12.00)) / 10.00;//年
            }
        }

        public Wave Wave { get; set; }

        public int BenchmarkId { get; set; }

        public decimal LowerScore { get; set; }

        public decimal HigherScore { get; set; }

        public string BenchmarkLabel { get; set; }

        public string BenchmarkColor { get; set; }

        public BlackWhiteStyle BenchmarkBW { get; set; }
    }
}
