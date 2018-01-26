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
 * CreatedOn:		2014/8/28 20:23:24
 * Description:		Please input class summary
 * Version History:	Created,2014/8/28 20:23:24
 * 
 * 
 **************************************************************************/
using Sunnet.Cli.Business.Cpalls.Models;
using Sunnet.Cli.Core.Ade;
using Sunnet.Cli.Core.Ade.Entities;
using Sunnet.Framework.Mvc;

namespace Sunnet.Cli.Business.Ade.Models
{
    public class RapidExpressiveItemModel:ItemModelBase<RapidExpressiveItemEntity>
    {
        public override ItemType Type
        {
            get
            {
                return ItemType.RapidExpressive;
            }
            set { }
        }

        public override AnswerType AnswerType
        {
            get
            {
                return AnswerType.YesNo;
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

        public int? Timeout { get; set; }
        public RapidExpressiveItemModel():base()
        {
        }

        public RapidExpressiveItemModel(RapidExpressiveItemEntity entity) : base(entity)
        {
            this.Timeout = entity.Timeout;
            this.TargetText = entity.TargetText;
            this.TargetTextTimeout = entity.TargetTextTimeout;
            this.TargetAudio = entity.TargetAudio;
            this.TargetAudioTimeout = entity.TargetAudioTimeout;
        }

        public override void UpdateEntity(RapidExpressiveItemEntity entity)
        {
            base.UpdateEntity(entity);

            entity.Timeout = this.Timeout ?? 0;
            entity.TargetText = this.TargetText;
            entity.TargetTextTimeout = this.TargetTextTimeout;
            entity.TargetAudio = this.TargetAudio;
            entity.TargetAudioTimeout = this.TargetAudioTimeout;
        }

        public override ExecCpallsItemModel ExecCpallsItemModel
        {
            get
            {
                base.ExecCpallsItemModel.Props.Add("TargetText", TargetText);
                base.ExecCpallsItemModel.Props.Add("TargetTextTimeout", TargetTextTimeout);
                base.ExecCpallsItemModel.Props.Add("TargetAudio", TargetAudio);
                base.ExecCpallsItemModel.Props.Add("TargetAudioTimeout", TargetAudioTimeout);
                base.ExecCpallsItemModel.Props.Add("Timeout", Timeout ?? 0);

                return base.ExecCpallsItemModel;
            }
        }
    }
}
