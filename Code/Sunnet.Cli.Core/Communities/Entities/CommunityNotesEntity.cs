using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

using Sunnet.Framework.Core.Base;
using Sunnet.Cli.Core.Common.Enums;

namespace Sunnet.Cli.Core.Communities.Entities
{
    public class CommunityNotesEntity : EntityBase<int>
    {
        [Required]
        public int CommunityId { get; set; }

        [Required]
        [DisplayName("Messages")]
        [StringLength(1500, ErrorMessage = "This field is limited to 1500 characters.")]
        public string Messages { get; set; }

        [Required]
        [DisplayName("Status")]
        public EntityStatus Status { get; set; }

        [Required]
        [DisplayName("Start On")]
        public DateTime StartOn { get; set; }

        [Required]
        [DisplayName("Stop On")]
        public DateTime StopOn { get; set; }

        [Required]
        [DisplayName("Display Logo")]
        public bool DisplayLogo { get; set; }

        [Required]
        public int CreatedBy { get; set; }

        [Required]
        public int UpdateBy { get; set; }

        public virtual CommunityEntity Community { get; set; }
    }
}
