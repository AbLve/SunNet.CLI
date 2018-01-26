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
    public class AuditorEntity : EntityBase<int>
    {
        [EensureEmptyIfNullAttribute]
        [StringLength(50)]
        [DisplayName("Auditor Engage ID")]
        public string AuditorId { get; set; }

        [EensureEmptyIfNullAttribute]
        [StringLength(5)]
        [DisplayName("School Year")]
        public string SchoolYear { get; set; }

        [Required]
        [DisplayName("Title/Role")]
        public int PositionId { get; set; }

        [EensureEmptyIfNullAttribute]
        [StringLength(100)]
        public string PositionOther { get; set; }

        [Required]
        [DisplayName("Affiliation")]
        public int AffiliationId { get; set; }

        [EensureEmptyIfNullAttribute]
        [StringLength(100)]
        public string AffiliationOther { get; set; }

        [EensureEmptyIfNullAttribute]
        [StringLength(600)]
        [DisplayName("Auditor Notes")]
        public string AuditorNotes { get; set; }

        [Required]
        public virtual UserBaseEntity UserInfo { get; set; }
    }
}
