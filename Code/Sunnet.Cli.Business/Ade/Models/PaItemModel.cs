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
 * CreatedOn:		2014/8/28 20:22:59
 * Description:		Please input class summary
 * Version History:	Created,2014/8/28 20:22:59
 * 
 * 
 **************************************************************************/
using Sunnet.Cli.Business.Cpalls.Models;
using Sunnet.Cli.Core.Ade;
using Sunnet.Cli.Core.Ade.Entities;
using Sunnet.Framework.Mvc;

namespace Sunnet.Cli.Business.Ade.Models
{
    public class PaItemModel : ItemModelBase<PaItemEntity>, IMultiChoiceProperty
    {

        public override ItemType Type
        {
            get
            {
                return ItemType.Pa;
            }
            set { }
        }

        public override AnswerType AnswerType
        {
            get
            {
                return AnswerType.PaText;
            }
            set { }
        }

        [DisplayName("Target Text")]
        [StringLength(1000)]
        [EensureEmptyIfNull]
        public string TargetText { get; set; }

        [DisplayName("Target Text Time in")]
        public int TargetTextTimeout { get; set; }

        [DisplayName("Target Audio")]
        [StringLength(100)]
        [EensureEmptyIfNull]
        public string TargetAudio { get; set; }

        [DisplayName("Target Audio Time in")]
        public int TargetAudioTimeout { get; set; }

        [DisplayName("Choice type")]
        [Required(ErrorMessage = "Choose one, please.")]
        public bool IsMultiChoice { get; set; }

        public PaItemModel()
            : base()
        {
        }

        public PaItemModel(PaItemEntity entity)
            : base(entity)
        {
            this.TargetText = entity.TargetText;
            this.TargetTextTimeout = entity.TargetTextTimeout;
            this.TargetAudio = entity.TargetAudio;
            this.TargetAudioTimeout = entity.TargetAudioTimeout;
            this.IsMultiChoice = entity.IsMultiChoice;
        }

        public override void UpdateEntity(PaItemEntity entity)
        {
            base.UpdateEntity(entity);

            entity.TargetText = this.TargetText;
            entity.TargetTextTimeout = this.TargetTextTimeout;
            entity.TargetAudio = this.TargetAudio;
            entity.TargetAudioTimeout = this.TargetAudioTimeout;
            entity.IsMultiChoice = this.IsMultiChoice;

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
                base.ExecCpallsItemModel.Props.Add("TargetTextTimeout", TargetTextTimeout);
                base.ExecCpallsItemModel.Props.Add("TargetAudio", TargetAudio);
                base.ExecCpallsItemModel.Props.Add("TargetAudioTimeout", TargetAudioTimeout);
                base.ExecCpallsItemModel.Props.Add("IsMultiChoice", IsMultiChoice);

                return base.ExecCpallsItemModel;
            }
        }
    }
}
