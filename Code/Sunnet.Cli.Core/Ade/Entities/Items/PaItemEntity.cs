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
 * CreatedOn:		2014/8/11 1:09:31
 * Description:		Please input class summary
 * Version History:	Created,2014/8/11 1:09:31
 * 
 * 
 **************************************************************************/
using Sunnet.Framework.Mvc;

namespace Sunnet.Cli.Core.Ade.Entities
{
    public class PaItemEntity : ItemBaseEntity
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

        public bool IsMultiChoice { get; set; }
    }
}
