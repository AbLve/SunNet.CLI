using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sunnet.Cli.Core.Ade;
using Sunnet.Cli.Core.Ade.Entities;
using Sunnet.Cli.Core.Common.Enums;
using Sunnet.Framework.Core.Base;
/**************************************************************************
 * Developer: 		JackZhang
 * Computer:		JACKZ
 * Domain:			Jackz
 * CreatedOn:		2014/8/11 0:57:07
 * Description:		Please input class summary
 * Version History:	Created,2014/8/11 0:57:07
 * 
 * 
 **************************************************************************/
using Sunnet.Framework.Mvc;

namespace Sunnet.Cli.Core.Practices.Entities
{
    public class CliAdeMeasureEntity : EntityBase<int>, IAdeLinkProperties, ICutOffScoreProperties
    {
        public int AssessmentId { get; set; }

        [StringLength(100)]
        [Required]
        [DisplayName("Measure Name")]
        public string Name { get; set; }

        [StringLength(10)]
        [EensureEmptyIfNull]
        [DisplayName("Short Name")]
        public string ShortName { get; set; }

        [StringLength(100)]
        [Required]
        [DisplayName("Measure Label")]
        public string Label { get; set; }

        [StringLength(4000)]
        [EensureEmptyIfNull]
        [DisplayName("Description")]
        public string Description { get; set; }

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

        [DisplayName("Time Out")]
        public int Timeout { get; set; }

        [DisplayName("Interval")]
        public int InnerTime { get; set; }

        [DisplayName("Start Page")]
        public string StartPageHtml { get; set; }

        [DisplayName("End Page")]
        public string EndPageHtml { get; set; }

        [DisplayName("Parent Measure")]
        public int ParentId { get; set; }

        /// <summary>
        /// 包含哪些类型的Items.
        /// </summary>
        public ItemType Type { get; set; }

        public EntityStatus Status { get; set; }

        public bool IsDeleted { get; set; }

        public int CreatedBy { get; set; }

        public int UpdatedBy { get; set; }

        public virtual CliAdeMeasureEntity Parent { get; set; }
        public virtual ICollection<CliAdeMeasureEntity> SubMeasures { get; set; }

        public virtual ICollection<CliAdeItemBaseEntity> Items { get; set; }

        public virtual CliAdeAssessmentEntity Assessment { get; set; }

        /// <summary>
        /// 格式:1,2,3.
        /// </summary>
        [DisplayName("Apply to Wave")]
        public string ApplyToWave { get; set; }

        [DisplayName("Relate Measure")]
        public int RelatedMeasureId { get; set; }

        public bool StopButton { get; set; }

        public bool NextButton { get; set; }

        public bool PreviousButton { get; set; }
        [DisplayName("Note")]
        public string Note { get; set; }

        [DisplayName("Light Score Color")]
        public bool LightColor { get; set; }

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

        /// <summary>
        /// Measure的及格线是否改变
        /// </summary>
        public bool CutOffScoresChanged { get; set; }

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