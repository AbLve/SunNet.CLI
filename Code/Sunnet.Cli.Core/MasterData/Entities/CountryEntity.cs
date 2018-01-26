using Sunnet.Framework.Core.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Cli.Core.MasterData.Entities
{
    public class CountryEntity : EntityBase<int>
    {
        [Required]
        public string Name { get; set; }
    }
}
