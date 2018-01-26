using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sunnet.Framework.Extensions;

namespace Sunnet.Cli.Core.Cpalls.Models.Report
{
    public class SchoolPercentileRankModel
    {
        public int SchoolId { get; set; }
        public int MeasureId { get; set; }
        public Wave Wave { get; set; }
        public decimal TotalScore { get; set; }
        public string PercentileRank { get; set; }

    }
}
