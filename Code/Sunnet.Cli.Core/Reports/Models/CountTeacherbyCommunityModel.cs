using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sunnet.Cli.Core.Common.Enums;

namespace Sunnet.Cli.Core.Reports.Models
{
    public class CountTeacherbyCommunityModel
    {
        public int CoachId { get; set; }

        public string CoachFirtName { get; set; }

        public string CoachLastName { get; set; }

        public string CoachName {
            get
            {
                return string.Format("{0} {1}", CoachFirtName, CoachLastName);
            }
        }

        public int CommunityId { get; set; }

        public string CommunityName { get; set; }

        public EntityStatus Status { get; set; }

        public int Total { get; set; }

        public int AllTotal { get; set; }
    }
}

