using Sunnet.Framework.Core.Base;
using Sunnet.Framework.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Cli.Core.Users.Entities
{
    public class VideoCodingEntity : EntityBase<int>
    {
        [DisplayName("Primary Language")]
        public int PrimaryLanguageId { get; set; }

        [EensureEmptyIfNullAttribute]
        [StringLength(100)]
        public string PrimaryLanguageOther { get; set; }

        [DisplayName("Secondary Language")]
        public int SecondaryLanguageId { get; set; }

        [EensureEmptyIfNullAttribute]
        [StringLength(100)]
        public string SecondaryLanguageOther { get; set; }

        [Required]
        public virtual UserBaseEntity User { get; set; }
    }
}
