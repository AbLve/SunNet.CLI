using Sunnet.Cli.Core.Ade.Entities;
using Sunnet.Cli.Core.Ade.Interfaces;
using Sunnet.Framework.Core.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Cli.Impl.Ade
{
    public class BranchingScoreRpst : EFRepositoryBase<BranchingScoreEntity, int>, IBranchingScoreRpst
    {
        public BranchingScoreRpst(IUnitOfWork unit)
        {
            this.UnitOfWork = unit;
        }
    }
}