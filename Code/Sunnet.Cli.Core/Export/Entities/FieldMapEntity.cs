using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Sunnet.Framework.Core.Base;

namespace Sunnet.Cli.Core.Export.Entities
{
    public class FieldMapEntity : EntityBase<int>
    {
        [Required]
        public string FieldName { get; set; }

        [Required]
        public string DisplayName { get; set; }

        [Required]
        public string AssociateSql { get; set; }

        [Required]
        public string SelectName { get; set; }
    }
}
