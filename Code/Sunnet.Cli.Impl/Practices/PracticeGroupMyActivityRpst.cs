using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sunnet.Cli.Core.Practices.Entites;
using Sunnet.Cli.Core.Practices.Interfaces;
using Sunnet.Framework.Core.Base;

namespace Sunnet.Cli.Impl.Practices
{
    public class PracticeGroupMyActivityRpst : EFRepositoryBase<PracticeGroupMyActivityEntity, int>, IPracticeGroupMyActivityRpst
    {
        public PracticeGroupMyActivityRpst(IUnitOfWork unit)
        {
            this.UnitOfWork = unit;
        }
    }
}
