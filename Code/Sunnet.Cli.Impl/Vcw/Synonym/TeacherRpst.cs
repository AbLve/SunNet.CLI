using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Sunnet.Framework.Core.Base;
using Sunnet.Cli.Core.Vcw.Entities;
using Sunnet.Cli.Core.Vcw.Interfaces;

namespace Sunnet.Cli.Impl.Vcw
{
    public class TeacherRpst : EFRepositoryBase<V_TeacherEntity, int>, ITeacherRpst_V
    {
        public TeacherRpst(IUnitOfWork unit)
        {
            this.UnitOfWork = unit;
        }
    }
}
