using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sunnet.Cli.Core.Common.Enums;
using Sunnet.Framework.Core.Base;

namespace Sunnet.Cli.Core.TRSClasses.Entites
{
    public class CHChildrenResultEntity : EntityBase<int>
    {
        public int TRSClassId { get; set; }
        public int CHChildrenId { get; set; }
        public int ChildrenNumber { get; set; }
        public int CaregiversNumber { get; set; }

        public virtual CHChildrenEntity CHChildren { get; set; }
    }
}
