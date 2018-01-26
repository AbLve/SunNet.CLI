using Sunnet.Cli.Core.Practices.Entites;
using Sunnet.Cli.Core.Practices.Interfaces;
using Sunnet.Framework.Core.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Cli.Impl.Practices
{
    public class DemoStudentRpst : EFRepositoryBase<DemoStudentEntity, int>, IDemoStudentRpst
    {
        public DemoStudentRpst(IUnitOfWork unit)
        {
            this.UnitOfWork = unit;
        }
    }
}
