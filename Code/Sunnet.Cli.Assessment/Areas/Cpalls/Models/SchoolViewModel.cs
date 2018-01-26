using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace Sunnet.Cli.Assessment.Areas.Cpalls.Models
{
    public class SchoolViewModel
    {
        public int CommunityId { get; set; }

        [DisplayName("School")]
        public string SchoolName { get; set; }

        public int SchoolId { get; set; }

        public int Year { get; set; }

        public int Wave { get; set; }
    }
}