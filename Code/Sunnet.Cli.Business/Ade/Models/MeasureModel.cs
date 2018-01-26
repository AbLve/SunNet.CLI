using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Sunnet.Cli.Core.Ade;
using Sunnet.Cli.Core.Ade.Entities;
using Sunnet.Cli.Core.Ade.Models;
using Sunnet.Cli.Core.Common.Enums;
/**************************************************************************
 * Developer: 		JackZhang
 * Computer:		JACKZ
 * Domain:			Jackz
 * CreatedOn:		2014/8/20 22:09:50
 * Description:		Please input class summary
 * Version History:	Created,2014/8/20 22:09:50
 * 
 * 
 **************************************************************************/
using Sunnet.Cli.Core.Cpalls;
using Sunnet.Framework.Mvc;

namespace Sunnet.Cli.Business.Ade.Models
{
    public class MeasureModel
    {

        public MeasureModel()
        {
            Status = EntityStatus.Active;
            OrderType = OrderType.Sequenced;
            ItemType = ItemShowType.Sequenced;
            Note = "";
            ShowLaunchPage = false;
            ShowFinalizePage = true;
        }

        public int ID { get; set; }
        public int AssessmentId { get; set; }

        [DisplayName("Assessment Label")]
        public string AssessmentLabel { get; set; }

        public AssessmentType AssessmentType { get; set; }

        [StringLength(100)]
        [Required]
        [DisplayName("Measure Name")]
        public string Name { get; set; }

        [StringLength(10)]
        [EensureEmptyIfNull]
        [DisplayName("Short Name")]
        public string ShortName
        {
            get
            {
                return _shortName;
            }
            set
            {
                _shortName = value;
            }
        }


        [StringLength(4000)]
        [EensureEmptyIfNull]
        [DisplayName("Description")]
        public string Description { get; set; }

        /// <summary>
        /// 获取或设置显示Measure时用于表示分级的前缀.
        /// </summary>
        public string NamePrefix { get; set; }

        [StringLength(100)]
        [Required]
        [DisplayName("Measure Label")]
        public string Label { get; set; }

        [DisplayName("Administration Order")]
        public OrderType OrderType { get; set; }

        [DisplayName("Items Type")]
        public ItemShowType ItemType { get; set; }

        public int Sort { get; set; }

        [DisplayName("Total Scored")]
        public bool TotalScored { get; set; }
        /// <summary>
        /// Total score of items [Active, Not deleted, calculated by PROC]
        /// </summary>
        public decimal TotalScore { get; set; }

        [DisplayName("Interval")]
        public int InnerTime { get; set; }

        [DisplayName("Time Out")]
        public int Timeout { get; set; }

        [DisplayName("Start Page")]
        [EensureEmptyIfNull]
        public string StartPageHtml { get; set; }

        [EensureEmptyIfNull]
        [DisplayName("End Page")]
        public string EndPageHtml { get; set; }

        [DisplayName("Parent Measure")]
        public int ParentId { get; set; }

        public EntityStatus Status { get; set; }

        public bool IsDeleted { get; set; }

        [DisplayName("SubMeasure Count")]
        public int SubMeasureCount { get; set; }

        public List<MeasureModel> SubMeasures { get; set; }

        [JsonIgnore]
        public MeasureModel Parent { get; set; }

        [DisplayName("Item Count")]
        public int ItemCount { get; set; }

        private List<ItemModel> _items;
        public List<ItemModel> Items
        {
            get { return _items ?? (_items = new List<ItemModel>()); }
            set { _items = value; }
        }

        private List<CutOffScoreEntity> _cutOffScores;
        private string _createdByName;
        private List<AdeLinkEntity> _links;
        private string _shortName;

        /// <summary>
        /// 添加时临时存储添加的值
        /// </summary>
        [DisplayName("Cutoff Scores")]
        public List<CutOffScoreEntity> CutOffScores
        {
            get { return _cutOffScores ?? (_cutOffScores = new List<CutOffScoreEntity>()); }
            set { _cutOffScores = value; }
        }

        [DisplayName("Created By")]
        public int CreatedBy { get; set; }

        [DisplayName("Created By")]
        public string CreatedByName
        {
            get { return _createdByName ?? (_createdByName = ""); }
            set { _createdByName = value; }
        }

        [DisplayName("Updated By")]
        public int UpdatedBy { get; set; }
        [DisplayName("Created")]
        public DateTime CreatedOn { get; set; }
        [DisplayName("Updated")]
        public DateTime UpdatedOn { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="MeasureModel"/> is locked by assessment.
        /// </summary>
        public bool Locked { get; set; }

        public List<AdeLinkEntity> Links
        {
            get { return _links ?? (_links = new List<AdeLinkEntity>()); }
            set { _links = value; }
        }

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

        [DisplayName("Relate Measure")]
        public int RelatedMeasureId { get; set; }

        [DisplayName("Stop/Restart Button")]
        public bool StopButton { get; set; }

        [DisplayName("Next Button")]
        public bool NextButton { get; set; }

        [DisplayName("Previous Button")]
        public bool PreviousButton { get; set; }

        [DisplayName("Note")]
        public string Note { get; set; }

        [DisplayName("Light Score Color")]
        public bool LightColor { get; set; }

        /// <summary>
        /// Measure有没有设置及格线
        /// </summary>
        public bool HasCutOffScores { get; set; }

        /// <summary>
        /// 表示wave1下这个Measure是否有Score
        /// </summary>
        public bool BOYHasCutOffScores { get; set; }

        /// <summary>
        /// 表示wave2下这个Measure是否有Score
        /// </summary>
        public bool MOYHasCutOffScores { get; set; }

        /// <summary>
        /// 表示wave3下这个Measure是否有Score
        /// </summary>
        public bool EOYHasCutOffScores { get; set; }

        [DisplayName("Group By Parent Measure")]
        public bool GroupByParentMeasure { get; set; }

        [DisplayName("Launch Page")]
        public bool ShowLaunchPage { get; set; }

        [DisplayName("Finalize Page")]
        public bool ShowFinalizePage { get; set; }

        [DisplayName("Percentile Rank")]
        public bool PercentileRank { get; set; }

        [DisplayName("Group by Label")]
        public bool GroupByLabel { get; set; }
    }
}
