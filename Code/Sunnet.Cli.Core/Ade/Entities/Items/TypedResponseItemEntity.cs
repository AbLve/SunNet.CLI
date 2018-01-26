using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sunnet.Framework.Mvc;

namespace Sunnet.Cli.Core.Ade.Entities
{
    public class TypedResponseItemEntity : ItemBaseEntity
    {
        private ICollection<TypedResponseEntity> _responses;

        [DisplayName("Time Out")]
        public int Timeout { get; set; }

        [DisplayName("Target Text")]
        public string TargetText { get; set; }

        [DisplayName("Time in (ms)")]
        public int TargetTextTimeout { get; set; }

        [DisplayName("Target Audio")]
        [EensureEmptyIfNull]
        public string TargetAudio { get; set; }

        [DisplayName("Time in (ms)")]
        public int TargetAudioTimeout { get; set; }

        public virtual ICollection<TypedResponseEntity> Responses
        {
            get { return _responses ?? (_responses = new List<TypedResponseEntity>()); }
            set { _responses = value; }
        }
    }
}
