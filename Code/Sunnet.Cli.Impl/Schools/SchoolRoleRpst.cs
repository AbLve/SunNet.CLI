using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sunnet.Cli.Core.Schools.Entities;
using Sunnet.Cli.Core.Schools.Interfaces;
using Sunnet.Framework.Core.Base;

namespace Sunnet.Cli.Impl.Schools
{
    public class SchoolRoleRpst : EFRepositoryBase<SchoolRoleEntity, Int32>, ISchoolRoleRpst
    {
        public SchoolRoleRpst(IUnitOfWork unit)
        {
            this.UnitOfWork = unit;
        }
    }
}
