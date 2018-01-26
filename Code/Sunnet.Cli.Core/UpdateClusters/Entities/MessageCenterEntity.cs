using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sunnet.Framework.Core.Base;

namespace Sunnet.Cli.Core.UpdateClusters.Entities
{
    public class MessageCenterEntity : EntityBase<int>
    {
        [Required]
        public DateTime Date { get; set; }

        [Required]
        public string Description { get; set; }

        [Required(AllowEmptyStrings = true)]
        public string HyperLink { get; set; }

        public int CreatedBy { get; set; }

        public int UpdatedBy { get; set; }
    }
}
