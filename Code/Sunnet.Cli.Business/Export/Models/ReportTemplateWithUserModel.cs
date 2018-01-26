using Sunnet.Cli.Core.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Cli.Business.Export.Models
{
    public class ReportTemplateWithUserModel
    {
        public int ID { get; set; }

        public string Name { get; set; }

        public EntityStatus Status { get; set; }

        public int CreatedBy { get; set; }

        public DateTime CreatedOn { get; set; }

        public string CreatedUserName { get; set; }
    }
}
