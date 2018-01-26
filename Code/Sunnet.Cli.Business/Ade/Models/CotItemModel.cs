using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		JackZhang
 * Computer:		JACKZ
 * Domain:			Jackz
 * CreatedOn:		2014/8/28 20:22:12
 * Description:		Please input class summary
 * Version History:	Created,2014/8/28 20:22:12
 * 
 * 
 **************************************************************************/
using Sunnet.Cli.Business.Cpalls.Models;
using Sunnet.Cli.Core.Ade;
using Sunnet.Cli.Core.Ade.Entities;
using Sunnet.Framework.Mvc;

namespace Sunnet.Cli.Business.Ade.Models
{
    public class CotItemModel : ItemModelBase<CotItemEntity>
    {
        public override ItemType Type
        {
            get
            {
                return ItemType.Cot;
            }
            set { }
        }

        public override AnswerType AnswerType
        {
            get
            {
                return AnswerType.None;
            }
            set { }
        }

        public CotLevel Level { get; set; }

        [StringLength(1000)]
        [Required]
        [DisplayName("Short Target Text")]
        public string ShortTargetText { get; set; }

        [StringLength(4000)]
        [Required]
        [DisplayName("Full Target Text")]
        public string FullTargetText { get; set; }

        [StringLength(4000)]
        [EensureEmptyIfNull]
        [DisplayName("Prekindergarten Guidelines")]
        public string PrekindergartenGuidelines { get; set; }

        [StringLength(4000)]
        [EensureEmptyIfNull]
        [DisplayName("Circle Manual")]
        public string CircleManual { get; set; }

        [StringLength(4000)]
        [EensureEmptyIfNull]
        [DisplayName("Mentoring Guide")]
        public string MentoringGuide { get; set; }

        [Required]
        [DisplayName("Cot Item")]
        [StringLength(10)]
        public string CotItemId { get; set; }

        public CotItemModel()
            : base()
        {

        }

        public CotItemModel(CotItemEntity entity)
            : base(entity)
        {
            
            this.Level = entity.Level;
            this.ShortTargetText = entity.ShortTargetText;
            this.FullTargetText = entity.FullTargetText;
            this.PrekindergartenGuidelines = entity.PrekindergartenGuidelines;
            this.MentoringGuide = entity.MentoringGuide;
            this.CircleManual = entity.CircleManual;
            this.CotItemId = entity.CotItemId;
        }

        public override void UpdateEntity(CotItemEntity entity)
        {
            base.UpdateEntity(entity);

            entity.Level = this.Level;
            entity.ShortTargetText = this.ShortTargetText;
            entity.FullTargetText = this.FullTargetText;
            entity.PrekindergartenGuidelines = this.PrekindergartenGuidelines;
            entity.MentoringGuide = this.MentoringGuide;
            entity.CircleManual = this.CircleManual;
            entity.Scored = true;
            entity.Timed = false;
            entity.CotItemId = this.CotItemId;
        }

        public override ExecCpallsItemModel ExecCpallsItemModel
        {
            get
            {
                return null;
                //throw new NotImplementedException("COT Item does not implement this property");
            }
        }
    }
}
