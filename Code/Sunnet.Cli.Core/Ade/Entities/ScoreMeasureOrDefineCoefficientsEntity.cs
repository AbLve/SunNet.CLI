using Newtonsoft.Json;
using Sunnet.Cli.Core.Cpalls;
using Sunnet.Framework.Core.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Cli.Core.Ade.Entities
{
    public class ScoreMeasureOrDefineCoefficientsEntity : EntityBase<int>
    {

        public int ScoreId { get; set; }

        [Required]
        public Wave Wave { get; set; }

        [Required]
        public int Measure { get; set; }

        [Required]
        public decimal Coefficient { get; set; }

        [JsonIgnore]
        public virtual ScoreEntity Score { get; set; }

        [JsonIgnore]
        public virtual MeasureEntity MeasureObject { get; set; }

    }
}
