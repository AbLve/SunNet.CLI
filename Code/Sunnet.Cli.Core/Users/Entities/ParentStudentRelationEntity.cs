using Sunnet.Cli.Core.Students.Entities;
using Sunnet.Cli.Core.Users.Enums;
using Sunnet.Framework.Core.Base;
using Sunnet.Framework.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Cli.Core.Users.Entities
{
    public class ParentStudentRelationEntity : EntityBase<Int32>
    {
        [Required]
        public int ParentId { get; set; }

        [Required]
        public int StudentId { get; set; }

        [Required]
        [DisplayName("Relation")]
        public ParentRelation Relation { get; set; }

        [EensureEmptyIfNullAttribute]
        [StringLength(100)]
        public string RelationOther { get; set; }

        public virtual StudentEntity Student { get; set; }

        public virtual ParentEntity Parent { get; set; }
    }
}
