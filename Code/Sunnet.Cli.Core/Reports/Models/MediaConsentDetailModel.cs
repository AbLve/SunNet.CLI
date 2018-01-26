using Sunnet.Cli.Core.Common.Enums;
using Sunnet.Cli.Core.Students.Enums;
using Sunnet.Framework.Core.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Cli.Core.Reports.Models
{
    public class MediaConsentDetailModel
    {
        public string CommunityName { get; set; }

        public string SchoolName { get; set; }

        public string SchoolCode { get; set; }

        public string TeacherName { get; set; }

        public string TeacherCode { get; set; }

        public string TargetStatus { get; set; }

        public string CoachName { get; set; }

        public string StudentName { get; set; }

        public string StudentCode { get; set; }

        public EntityStatus StudentStatus { get; set; }

        public MediaRelease StudentMediaRelease { get; set; }
    }
}
