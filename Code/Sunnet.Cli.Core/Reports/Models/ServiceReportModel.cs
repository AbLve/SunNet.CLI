using Sunnet.Cli.Core.Common.Enums;
using Sunnet.Cli.Core.Students.Enums;
using Sunnet.Framework.Core.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Cli.Core.Reports.Models
{
    public class ServiceReportModel
    {
        public string CommunityName { get; set; }

        public string SchoolName { get; set; }

        public int ESCName { get; set; }

        public EntityStatus Status { get; set; }

        public string SchoolCode { get; set; }

        public string PhysicalAddress1 { get; set; }

        public string PhysicalAddress2 { get; set; }

        public string City { get; set; }

        public string County { get; set; }

        public string State { get; set; }

        public string Zip { get; set; }

        public string Phone { get; set; }

        public string SchoolType { get; set; }

        public string SchoolYear { get; set; }

        public string ChildCareLicense { get; set; }

        public string Funding { get; set; }

        public int ClassroomCount { get; set; }

        public int TeacherCount { get; set; }

        public int StudentCount { get; set; }
    }
}
