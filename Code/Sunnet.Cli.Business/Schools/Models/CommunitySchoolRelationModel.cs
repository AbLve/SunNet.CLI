using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Cli.Business.Schools.Models
{
    public class CommunitySchoolRelationModel
    {
        public Int64 ID { get; set; }
        public int SchoolId { get; set; }
        public int CommunityId { get; set; }
        public string CommunityName { get; set; }
        public string SchoolName { get; set; }

    }
}
