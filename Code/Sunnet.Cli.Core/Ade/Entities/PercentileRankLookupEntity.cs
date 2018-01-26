using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sunnet.Framework.Core.Base;

namespace Sunnet.Cli.Core.Ade.Entities
{
    public class PercentileRankLookupEntity : EntityBase<int>
    {
        public string MeasureLabel { get; set; }

        public int AgeMin { get; set; }

        public int AgeMax { get; set; }

        public int RawScore { get; set; }

        public string PercentileRank { get; set; }

        public string AlternateScore { get; set; }
    }
}
