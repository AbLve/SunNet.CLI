using Sunnet.Cli.Core.Ade.Entities;
using Sunnet.Cli.Core.Cpalls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Cli.Business.Ade.Models
{
    //public class ScoreModel: ScoreEntity


    public class ScoreModel 
    {
        public int AssessmentId { get; set; }

        public int ID { get; set; }

        [Required]
        [StringLength(100)]
        [DisplayName("Score Name")]
        public string ScoreName { get; set; }

        [Required]
        [StringLength(100)]
        [DisplayName("Score Domain")]
        public string ScoreDomain { get; set; }

        [Required]
        [DisplayName("Mean Adjustment")]
        public decimal MeanAdjustment { get; set; }

        [Required]
        [DisplayName("SD Adjustment")]
        public decimal SDAdjustment { get; set; }

        [Required]
        [DisplayName("Target Mean")]
        public decimal TargetMean { get; set; }

        [Required]
        [DisplayName("Target SD")]
        public decimal TargetSD { get; set; }

        [Required]
        [DisplayName("Target Round")]
        public int TargetRound { get; set; }

        public bool IsDeleted { get; set; }

        public string Description { get; set; }

        [DisplayName("Group by Label")]
        public bool GroupByLabel { get; set; }

        internal string ApplyToWaveValues { get; set; }

        [DisplayName("Apply to Wave")]
        public List<Wave> ApplyToWave
        {
            get
            {
                if (string.IsNullOrEmpty(ApplyToWaveValues))
                    return new List<Wave>();
                return ApplyToWaveValues.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                    .Select(int.Parse).Select(x => (Wave)x).ToList();
            }
            set { ApplyToWaveValues = string.Join(",", value.Select(x => (int)x)); }
        }

        private List<ScoreAgeOrWaveBandsEntity> _scoreAgeOrWave;

        public List<ScoreAgeOrWaveBandsEntity> ScoreAgeOrWave
        {
            get { return _scoreAgeOrWave ?? (_scoreAgeOrWave = new List<ScoreAgeOrWaveBandsEntity>()); }
            set { _scoreAgeOrWave = value; }
        }


        private List<ScoreMeasureOrDefineCoefficientsEntity> _scoreMeasureOrDefineCoefficients;

        public List<ScoreMeasureOrDefineCoefficientsEntity> ScoreMeasureOrDefineCoefficients
        {
            get { return _scoreMeasureOrDefineCoefficients ?? (_scoreMeasureOrDefineCoefficients = new List<ScoreMeasureOrDefineCoefficientsEntity>()); }
            set { _scoreMeasureOrDefineCoefficients = value; }
        }

        private List<CutOffScoreEntity> _cutOffScores;
        /// <summary>
        /// 添加时临时存储添加的值
        /// </summary>
        [DisplayName("Cutoff Scores")]
        public List<CutOffScoreEntity> CutOffScores
        {
            get { return _cutOffScores ?? (_cutOffScores = new List<CutOffScoreEntity>()); }
            set { _cutOffScores = value; }
        }

    }
}
