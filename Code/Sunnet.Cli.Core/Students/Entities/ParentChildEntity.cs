using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sunnet.Cli.Core.Users.Entities;
using Sunnet.Framework.Core.Base;

namespace Sunnet.Cli.Core.Students.Entities
{
    public class ParentChildEntity : EntityBase<int>
    {
        public int ParentId { get; set; }

        public int ChildId { get; set; }

        public virtual ParentEntity Parent { get; set; }

        public virtual ChildEntity Child { get; set; }

        //public virtual V_ChildEntity V_Child { get; set; }
    }
}
