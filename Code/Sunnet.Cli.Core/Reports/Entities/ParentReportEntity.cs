using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sunnet.Cli.Core.Common.Enums;
using Sunnet.Framework.Core.Base;

namespace Sunnet.Cli.Core.Reports.Entities
{
    public class ParentReportEntity : EntityBase<int>
    {
 
        public int AssessmentId { get; set; }
        public string AssessmentName { get; set; }
        public int SchoolId { get; set; }
        public int ClassId { get; set; }
        public int StudentId { get; set; }
        public string ReportName { get; set; }
        public EntityStatus Status { get; set; }
        public int CreatedBy { get; set; }
    }
}
