using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		JackZhang
 * Computer:		JACKZ
 * Domain:			JackZ
 * CreatedOn:		6/9/2015 09:15:37
 * Description:		Please input class summary
 * Version History:	Created,6/9/2015 09:15:37
 *
 *
 **************************************************************************/
using Sunnet.Cli.Core.Ade.Entities;
using Sunnet.Cli.Core.Common.Enums;
using Sunnet.Framework.Mvc;
using Sunnet.Framework.Extensions;

namespace Sunnet.Cli.Core.Ade.Models
{
    public class ItemModel : BaseItemModel
    {
        private bool _scored;
        private decimal _score;
        private bool _showAtTestResume;

        public ItemModel()
        {

        }

        public ItemModel(ItemBaseEntity entity)
        {

        }
        public int MeasureId { get; set; }

        [StringLength(100, MinimumLength = 8)]
        [Required]
        [DisplayName("Item Label")]
        public string Label { get; set; }

        public bool Scored
        {
            get
            {
                if (IsPractice)
                    return false;
                return _scored;
            }
            set { _scored = value; }
        }

        public decimal Score
        {
            get
            {
                if (IsPractice)
                    return 0;
                return _score;
            }
            set { _score = value; }
        }

        public bool Timed { get; set; }

        [DisplayName("Wait Time")]
        public int WaitTime { get; set; }

        public int Sort { get; set; }

        public virtual ItemType Type { get; set; }

        public EntityStatus Status { get; set; }

        public bool IsDeleted { get; set; }

        public int CreatedBy { get; set; }

        public int UpdatedBy { get; set; }

        [DisplayName("Created")]
        public DateTime CreatedOn { get; set; }

        [DisplayName("Updated")]
        public DateTime UpdatedOn { get; set; }

        public virtual AnswerType AnswerType { get; set; }

        /// <summary>
        /// 是否随机显示答案顺序
        /// </summary>
        public bool RandomAnswer { get; set; }

        [DisplayName("Practice Item")]
        public bool IsPractice { get; set; }


        [DisplayName("Show at test resume")]
        public bool ShowAtTestResume
        {
            get
            {
                if (!IsPractice)
                    return false;
                return _showAtTestResume;
            }
            set { _showAtTestResume = value; }
        }

        public List<SelectItemModel> BranchingItems { get; set; }
    }
}
