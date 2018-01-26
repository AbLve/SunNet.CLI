using Sunnet.Framework.Core.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sunnet.Cli.Core.Trs.Enums;

namespace Sunnet.Cli.Core.Trs.Entities
{
    public class TRSAssessmentItemEntity : EntityBase<int>
    {
        public int TRSAssessmentId { get; set; }

        public int ClassId { get; set; }

        /// <summary>
        /// TRSItem.Id
        /// </summary>
        public int ItemId { get; set; }

        /// <summary>
        /// 选中的答案 TRSAnswer.ID ，初始时为 0 
        /// </summary>
        public int? AnswerId { get; set; }

        public string Comments { get; set; }

        public int CreatedBy { get; set; }

        public int UpdatedBy { get; set; }

        public ItemAgeGroup AgeGroup { get; set; }

        public int GroupSize { get; set; }

        public int CaregiversNo { get; set; }

        public virtual TRSAssessmentEntity Assessment { get; set; }

        public virtual TRSItemEntity Item { get; set; }

        public virtual TRSAnswerEntity Answer { get; set; }
    }
}
