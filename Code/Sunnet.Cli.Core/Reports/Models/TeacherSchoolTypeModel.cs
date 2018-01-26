using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sunnet.Cli.Core.Users.Enums;
using Sunnet.Cli.Core.Common.Enums;

namespace Sunnet.Cli.Core.Reports.Models
{
    public class TeacherSchoolTypeModel
    {
        public int SchoolTypeId { get; set; }

        public string SchoolTypeName { get; set; }

        public int ActiveTotal { get; set; }

        public int DroppedTotal { get; set; }

        public int Total { get; set; }

        public EntityStatus Status { get; set; }
    }
}
