using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using Sunnet.Cli.Core.Common.Enums;

namespace Sunnet.Cli.Business.Communities.Models
{
    public class CommunityNotesModel
    {
        public int ID { get; set; }

        public int CommunityId { get; set; }

        public string LogoUrl { get; set; }

        [Required]
        [DisplayName("Community Name")]
        public string CommunityName { get; set; }

        [Required]
        [DisplayName("Messages")]
        public string Messages { get; set; }

        [Required]
        [DisplayName("Status")]
        public CommunityNoteStatus Status { get; set; }

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

        
        public DateTime UpdateOn { get; set; }
    }
}
