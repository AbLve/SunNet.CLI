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
 * CreatedOn:		2014/8/28 20:24:57
 * Description:		Please input class summary
 * Version History:	Created,2014/8/28 20:24:57
 * 
 * 
 **************************************************************************/
using Sunnet.Cli.Business.Cpalls.Models;
using Sunnet.Cli.Core.Ade;
using Sunnet.Cli.Core.Ade.Entities;
using Sunnet.Framework.Mvc;

namespace Sunnet.Cli.Business.Ade.Models
{
    public class ReceptivePromptItemModel : ItemModelBase<ReceptivePromptItemEntity>
    {
        public override ItemType Type
        {
            get
            {
                return ItemType.ReceptivePrompt;
            }
            set { }
        }

        public override AnswerType AnswerType
        {
            get
            {
                return AnswerType.PictureAudio;
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

        [StringLength(100)]
        [Required]
        [DisplayName("Prompt Picture")]
        public string PromptPicture { get; set; }
        [DisplayName("Prompt Picture Time in")]
        public int PromptPictureTimeout { get; set; }

        [StringLength(1000)]
        [DisplayName("Prompt Text")]
        [EensureEmptyIfNull]
        public string PromptText { get; set; }
        [DisplayName("Prompt Text Time in")]
        public int PromptTextTimeout { get; set; }

        [StringLength(1000)]
        [EensureEmptyIfNull]
        [DisplayName("Prompt Audio")]
        public string PromptAudio { get; set; }
        [DisplayName("Prompt Audio Time in")]
        public int PromptAudioTimeout { get; set; }

        public ReceptivePromptItemModel()
            : base()
        {
        }

        public ReceptivePromptItemModel(ReceptivePromptItemEntity entity)
            : base(entity)
        {
            TargetText = entity.TargetText;
            TargetTextTimeout = entity.TargetTextTimeout;
            TargetAudio = entity.TargetAudio;
            TargetAudioTimeout = entity.TargetAudioTimeout;
            PromptPicture = entity.PromptPicture;
            PromptPictureTimeout = entity.PromptPictureTimeout;
            PromptText = entity.PromptText;
            PromptTextTimeout = entity.PromptTextTimeout;
            PromptAudio = entity.PromptAudio;
            PromptAudioTimeout = entity.PromptAudioTimeout;
        }

        public override void UpdateEntity(ReceptivePromptItemEntity entity)
        {
            base.UpdateEntity(entity);

            entity.TargetText = TargetText;
            entity.TargetTextTimeout = TargetTextTimeout;
            entity.TargetAudio = TargetAudio;
            entity.TargetAudioTimeout = TargetAudioTimeout;
            entity.PromptPicture = PromptPicture;
            entity.PromptPictureTimeout = PromptPictureTimeout;
            entity.PromptText = PromptText;
            entity.PromptTextTimeout = PromptTextTimeout;
            entity.PromptAudio = PromptAudio;
            entity.PromptAudioTimeout = PromptAudioTimeout;
        }

        public override ExecCpallsItemModel ExecCpallsItemModel
        {
            get
            {
                base.ExecCpallsItemModel.Props.Add("TargetText", TargetText);
                base.ExecCpallsItemModel.Props.Add("TargetTextTimeout", TargetTextTimeout);
                base.ExecCpallsItemModel.Props.Add("TargetAudio", TargetAudio);
                base.ExecCpallsItemModel.Props.Add("TargetAudioTimeout", TargetAudioTimeout);
                base.ExecCpallsItemModel.Props.Add("PromptPicture", PromptPicture);
                base.ExecCpallsItemModel.Props.Add("PromptPictureTimeout", PromptPictureTimeout);
                base.ExecCpallsItemModel.Props.Add("PromptText", PromptText);
                base.ExecCpallsItemModel.Props.Add("PromptTextTimeout", PromptTextTimeout);
                base.ExecCpallsItemModel.Props.Add("PromptAudio", PromptAudio);
                base.ExecCpallsItemModel.Props.Add("PromptAudioTimeout", PromptAudioTimeout);

                return base.ExecCpallsItemModel;
            }
        }
    }
}
