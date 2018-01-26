using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sunnet.Cli.Core.TRSClasses.Entites;
using Sunnet.Cli.Core.TRSClasses.Interfaces;
using Sunnet.Framework.Core.Base;

namespace Sunnet.Cli.Impl.TRSClasses
{
    public class CHChildrenRpst : EFRepositoryBase<CHChildrenEntity, Int32>, ICHChildrenRpst
    {
        public CHChildrenRpst(IUnitOfWork unit)
        {
            UnitOfWork = unit;
        }
    }
}
