using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sunnet.Cli.Core.Common.Enums;
using Sunnet.Cli.Core.Communities.Entities;
using Sunnet.Cli.Core.Students.Entities;
using Sunnet.Framework.Core.Base;

namespace Sunnet.Cli.Core.Schools.Entities
{
    public class SchoolStudentRelationEntity : EntityBase<int>
    {
        
        public int SchoolId { get; set; }
        
        public int StudentId { get; set; }

        public EntityStatus Status { get; set; }

        public int CreatedBy { get; set; }

        public int UpdatedBy { get; set; }

        public virtual SchoolEntity School { get; set; }

        public virtual StudentEntity Student { get; set; }
    }
}
