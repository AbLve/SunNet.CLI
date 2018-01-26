using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Sunnet.Cli.Core.Classes.Entites;
using Sunnet.Cli.Core.Classes.Interfaces;
using Sunnet.Framework.Core.Base;

namespace Sunnet.Cli.Impl.Classes
{
    public class ClassroomClassRpst : EFRepositoryBase<ClassroomClassEntity, int>, IClassroomClassRpst
    {
        public ClassroomClassRpst(IUnitOfWork unit)
        {
            UnitOfWork = unit;
        }
    }
}
