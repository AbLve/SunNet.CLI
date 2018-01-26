using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sunnet.Cli.Core.Students.Entities;
using Sunnet.Cli.Core.Students.Interfaces;
using Sunnet.Framework.Core.Base;

namespace Sunnet.Cli.Impl.Students
{
    public class ChildRpst : EFRepositoryBase<ChildEntity, Int32>, IChildRpst
    {
        public ChildRpst(IUnitOfWork unit)
        {
            this.UnitOfWork = unit;
        }
    }
}
