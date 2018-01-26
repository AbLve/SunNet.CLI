using Sunnet.Cli.Core.Cpalls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Cli.Business.Ade.Models
{
    public  class ScoreAgeOrWaveBandsModel
    {
        public int ScoreId { get; set; }

        public Wave wave { get; set; }
        
        public int AgeMin { get; set; }

        public int AgeMax { get; set; }

        public int AgeOrWaveMean { get; set; }

        public int AgeOrWaveSD { get; set; }
    }
}
