using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sunnet.Framework.Core.Base;
using Sunnet.Cli.Core.Ade;

namespace Sunnet.Cli.Core.Cpalls.Entities
{
    public class MeasureClassGroupEntity : EntityBase<int>
    {
        [Required]
        public int MeasureId { get; set; }

        [Required]
        public int ClassId { get; set; }

        [Required]
        public int Year { get; set; }

        [Required]
        public int Wave { get; set; }

        [DisplayName("Note")]
        public string Note { get; set; }

        public int CreatedBy { get; set; }

        public int UpdatedBy { get; set; }
    }
}
