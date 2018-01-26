using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sunnet.Cli.Core.Ade.Entities;

namespace Sunnet.Cli.Business.Cpalls.Models
{
    public class CpallsHeaderModel
    {
        public CpallsHeaderModel()
        {
            Measures = new List<MeasureHeaderModel>();
        }
        public int ID { get; set; }
        public string Name { get; set; }

        public IEnumerable<MeasureHeaderModel> Measures { get; set; }
    }

    public class MeasureHeaderModel
    {
        private decimal? _totalScore;
        /// <summary>
        /// StudentMeasureID.
        /// </summary>
        public int ID { get; set; }

        public int MeasureId { get; set; }
        public string Name { get; set; }
        public int ParentId { get; set; }

        public bool TotalScored { get; set; }
        public decimal? TotalScore
        {
            get
            {
                return _totalScore;
            }
            set { _totalScore = value; }
        }

        public string ParentMeasureName { get; set; }
        public int Subs { get; set; }
        public int Sort { get; set; }

        public string ApplyToWave { get; set; }

        //For Completion RelatedMeasure
        public int RelatedMeasureId { get; set; }

        /// <summary>
        /// 目前专为 Group服务, 是在 GroupController 中赋值的
        /// </summary>
        public List<AdeLinkEntity> Links { get; set; }

        public bool IsFirstOfParent { get; set; }
        public bool IsLastOfParent { get; set; }

        /// <summary>
        /// 关联项的Name
        /// </summary>
        public string TheOtherLanguageName { get; set; }

        public string Note { get; set; }

        public bool LightColor { get; set; }

        public bool PercentileRank { get; set; }

        public bool GroupByLabel { get; set; }
    }
}
