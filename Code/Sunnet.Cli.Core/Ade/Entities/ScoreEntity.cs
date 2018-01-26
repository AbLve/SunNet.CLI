using Sunnet.Framework.Core.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Cli.Core.Ade.Entities
{

    public class ScoreEntity : EntityBase<int>, ICutOffScoreProperties, IAdeLinkProperties
    {

        public int AssessmentId { get; set; }

        [Required]
        [StringLength(100)]
        [DisplayName("ScoreName")]
        public string ScoreName { get; set; }

        [Required]
        [StringLength(100)]
        [DisplayName("ScoreDomain")]
        public string ScoreDomain { get; set; }

        [Required]
        [DisplayName("MeanAdjustment")]
        public decimal MeanAdjustment { get; set; }

        [Required]
        [DisplayName("SDAdjustment")]
        public decimal SDAdjustment { get; set; }

        [Required]
        [DisplayName("TargetMean")]
        public decimal TargetMean { get; set; }

        [Required]
        [DisplayName("TargetSD")]
        public decimal TargetSD { get; set; }

        [Required]
        [DisplayName("TargetRound")]
        public int TargetRound { get; set; }

        public bool IsDeleted { get; set; }

        public string Description { get; set; }

        [DisplayName("Group by Label")]
        public bool GroupByLabel { get; set; }

        public virtual ICollection<ScoreAgeOrWaveBandsEntity> ScoreAgeOrWaveBands { get; set; }

        public virtual ICollection<ScoreMeasureOrDefineCoefficientsEntity> ScoreMeasureOrDefineCoefficients { get; set; }

    }
}
