using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		JackZhang
 * Computer:		JACKZ
 * Domain:			Jackz
 * CreatedOn:		2014/8/11 0:58:46
 * Description:		Please input class summary
 * Version History:	Created,2014/8/11 0:58:46
 * 
 * 
 **************************************************************************/
using Sunnet.Cli.Core.Common.Enums;
using Sunnet.Framework.Core.Base;
using Sunnet.Framework.Mvc;

namespace Sunnet.Cli.Core.Ade.Entities
{
    public class ItemBaseEntity : EntityBase<int>, IAdeLinkProperties
    {
        private ICollection<AnswerEntity> _answers;
        private ICollection<BranchingScoreEntity> _branchingScores;

        public int MeasureId { get; set; }

        [StringLength(100)]
        [Required]
        [DisplayName("Item Label")]
        public string Label { get; set; }

        [StringLength(1000)]
        [EensureEmptyIfNull]
        public string Description { get; set; }

        public bool Scored { get; set; }

        public decimal Score { get; set; }

        public bool Timed { get; set; }

        public int Sort { get; set; }

        public ItemType Type { get; set; }

        public EntityStatus Status { get; set; }

        public bool IsDeleted { get; set; }

        public int CreatedBy { get; set; }

        public int UpdatedBy { get; set; }

        public AnswerType AnswerType { get; set; }

        public virtual MeasureEntity Measure { get; set; }

        public virtual ICollection<AnswerEntity> Answers
        {
            get { return _answers ?? (_answers = new Collection<AnswerEntity>()); }
            set { _answers = value; }
        }

        public virtual ICollection<BranchingScoreEntity> BranchingScores
        {
            get { return _branchingScores ?? (_branchingScores = new Collection<BranchingScoreEntity>()); }
            set { _branchingScores = value; }
        }

        /// <summary>
        /// 是否随机显示答案顺序
        /// </summary>
        public bool RandomAnswer { get; set; }

        [DisplayName("Practice Item")]
        public bool IsPractice { get; set; }

        [DisplayName("Show at test resume")]
        public bool ShowAtTestResume { get; set; }

        [DisplayName("Wait Time")]
        public int WaitTime { get; set; }
    }
}
