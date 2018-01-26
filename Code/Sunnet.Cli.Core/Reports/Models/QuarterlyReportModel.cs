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
    public class QuarterlyReportModel
    {
        public int SchoolTypeId { get; set; }

        public string SchoolType { get; set; }

        public int Count { get; set; }

        public int YearsInProjectId { get; set; }
    }

    public class YearsInProjectModel
    {
        public int SchoolTypeId { get; set; }
        public string SchoolType { get; set; }
        public int Year1Count { get; set; }
        public int Year2Count { get; set; }
        public int Year3Count { get; set; }
        public int MissingCount { get; set; }
    }
}
