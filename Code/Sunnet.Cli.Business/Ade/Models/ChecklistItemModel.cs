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
 * CreatedOn:		2014/8/28 20:19:15
 * Description:		Please input class summary
 * Version History:	Created,2014/8/28 20:19:15
 * 
 * 
 **************************************************************************/
using Sunnet.Cli.Business.Cpalls.Models;
using Sunnet.Cli.Core.Ade;
using Sunnet.Cli.Core.Ade.Entities;
using Sunnet.Framework.Mvc;

namespace Sunnet.Cli.Business.Ade.Models
{
    public class ChecklistItemModel : ItemModelBase<ChecklistItemEntity>, IMultiChoiceProperty
    {
        public override ItemType Type
        {
            get
            {
                return ItemType.Checklist;
            }
            set { }
        }

        public override AnswerType AnswerType
        {
            get
            {
                return AnswerType.Cec;
            }
            set { }
        }

        public ChecklistItemModel()
            : base()
        {

        }

        public ChecklistItemModel(ChecklistItemEntity entity)
            : base(entity)
        {

            this.TargetText = entity.TargetText;
            this.IsMultiChoice = entity.IsMultiChoice;
            this.Direction = entity.Direction;
            this.ResponseCount = entity.ResponseCount;
            this.IsRequired = entity.IsRequired;
        }

        [StringLength(1000)]
        [Required]
        [DisplayName("Target Text")]
        [EensureEmptyIfNull]
        public string TargetText { get; set; }

        [DisplayName("Choice type")]
        [Required(ErrorMessage = "Choose one, please.")]
        public bool IsMultiChoice { get; set; }

        [DisplayName("Required")]
        public bool IsRequired { get; set; }



        [DisplayName("Response #")]
        public int ResponseCount { get; set; }

        [Required(ErrorMessage = "Choose one, please.")]
        public CecItemsDirection Direction { get; set; }

        public override void UpdateEntity(ChecklistItemEntity entity)
        {
            base.UpdateEntity(entity);

            entity.Description = this.Description;
            entity.TargetText = this.TargetText;
            entity.IsMultiChoice = this.IsMultiChoice;
            entity.Direction = this.Direction;
            entity.ResponseCount = this.ResponseCount;
            entity.Scored = true;
            entity.Timed = false;
            entity.IsRequired = this.IsRequired;
            if (this.Answers != null && !this.IsPractice)
                entity.Score = this.IsMultiChoice ?
                    this.Answers.Sum(a => a.Score) :
                    this.Answers.Max(a => a.Score);
            else
                entity.Score = 0;
        }

        public override ExecCpallsItemModel ExecCpallsItemModel
        {
            get
            {
                base.ExecCpallsItemModel.Props.Add("TargetText", TargetText);
                base.ExecCpallsItemModel.Props.Add("IsMultiChoice", IsMultiChoice);
                base.ExecCpallsItemModel.Props.Add("Response", ResponseCount);
                base.ExecCpallsItemModel.Props.Add("Direction", Direction);
                base.ExecCpallsItemModel.Props.Add("IsRequired", IsRequired);
                return base.ExecCpallsItemModel;
            }
        }
    }
}
