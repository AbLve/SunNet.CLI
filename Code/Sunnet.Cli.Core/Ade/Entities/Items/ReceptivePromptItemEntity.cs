using System;
using System.Collections.Generic;
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
 * CreatedOn:		2014/8/11 1:05:07
 * Description:		Please input class summary
 * Version History:	Created,2014/8/11 1:05:07
 * 
 * 
 **************************************************************************/
using Sunnet.Framework.Mvc;

namespace Sunnet.Cli.Core.Ade.Entities
{
    public class ReceptivePromptItemEntity : ItemBaseEntity
    {

        [DisplayName("Target Text")]
        [StringLength(1000)]
        public string TargetText { get; set; }
        [DisplayName("Time in (ms)")]
        public int TargetTextTimeout { get; set; }

        [DisplayName("Target Audio")]
        [StringLength(100)]
        [EensureEmptyIfNull]
        public string TargetAudio { get; set; }
        [DisplayName("Time in (ms)")]
        public int TargetAudioTimeout { get; set; }

        [StringLength(100)]
        [Required]
        [DisplayName("Prompt Picture")]
        public string PromptPicture { get; set; }
        [DisplayName("Time in (ms)")]
        public int PromptPictureTimeout { get; set; }

        [StringLength(1000)]
        [EensureEmptyIfNull]
        [DisplayName("Prompt Text")]
        public string PromptText { get; set; }
        [DisplayName("Time in (ms)")]
        public int PromptTextTimeout { get; set; }

        [StringLength(1000)]
        [EensureEmptyIfNull]
        [DisplayName("Prompt Audio")]
        public string PromptAudio { get; set; }
        [DisplayName("Time in (ms)")]
        public int PromptAudioTimeout { get; set; }

    }
}
