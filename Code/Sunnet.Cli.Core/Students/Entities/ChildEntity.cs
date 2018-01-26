using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sunnet.Cli.Core.Schools.Entities;
using Sunnet.Framework.Core.Base;
using Sunnet.Framework.Mvc;

namespace Sunnet.Cli.Core.Students.Entities
{
    public class ChildEntity : EntityBase<int>
    {
        public ChildEntity()
        {
            IsDeleted = false;
        }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public DateTime BirthDate { get; set; }

        [EensureEmptyIfNull]
        public string SchoolCity { get; set; }

        [EensureEmptyIfNullAttribute]
        public string SchoolZip { get; set; }

        public int SchoolId { get; set; }

        [EensureEmptyIfNullAttribute]
        public string PINCode { get; set; }

        public int StudentId { get; set; }

        public bool IsDeleted { get; set; }
        public virtual StudentEntity Student { get; set; }

        public virtual SchoolEntity School { get; set; }

        public virtual ICollection<ParentChildEntity> ParentChilds { get; set; }
    }
}
