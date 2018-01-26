using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sunnet.Cli.Core.Common.Enums;
using Sunnet.Framework.Mvc;

namespace Sunnet.Cli.Core.Ade.Models
{
    public class BaseItemModel
    {
        public int ID { get; set; }

        public string Label { get; set; }

        [StringLength(1000)]
        [EensureEmptyIfNull]
        public string Description { get; set; }

        public EntityStatus Status { get; set; }
    }
}
