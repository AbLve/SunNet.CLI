using Sunnet.Cli.Core.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Cli.Core.Reports.Models
{
    public class CountbyFacilityTypeMode
    {
        public int SchoolTypeId { get; set; }

        public string SchoolType { get; set; }

        public int SchoolTotal { get; set; }

        public int ClassroomTotal { get; set; }

        public int TeacherTotal { get; set; }

        public int StudentTotal { get; set; }

        /// <summary>
        /// school type status
        /// </summary>
        public EntityStatus Status { get; set; }
    }
}
