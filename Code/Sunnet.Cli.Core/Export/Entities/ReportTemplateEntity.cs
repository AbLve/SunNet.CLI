using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

using Sunnet.Framework.Core.Base;
using Sunnet.Cli.Core.Common.Enums;
using Sunnet.Framework.Mvc;


namespace Sunnet.Cli.Core.Export.Entities
{
    public class ReportTemplateEntity : EntityBase<int>
    {
        [Required]
        [DisplayName("Report Template Name")]
        public string Name { get; set; }

        [DisplayName("Report Template Status")]
        public EntityStatus Status { get; set; }

        [EensureEmptyIfNull]
        [DisplayName("Fields")]
        public string Fields { get; set; }

        [Required]
        public int CreatedBy { get; set; }

        [Required]
        public int UpdatedBy { get; set; }
    }
}
