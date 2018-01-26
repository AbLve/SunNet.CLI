using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sunnet.Cli.Core.Students.Enums;
using Sunnet.Framework.Core.Base;

namespace Sunnet.Cli.Core.Students.Entities
{
    public class StudentDOBEntity : EntityBase<int>
    {
        public int StudentId { get; set; }

        public StudentDOBStatus Status { get; set; }

        public DateTime OldDOB { get; set; }

        public DateTime NewDOB { get; set; }

        public string SchoolYear { get; set; }
    }
}
