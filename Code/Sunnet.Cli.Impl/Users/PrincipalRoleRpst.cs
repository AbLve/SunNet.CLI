using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sunnet.Cli.Core.Users.Entities;
using Sunnet.Cli.Core.Users.Interfaces;
using Sunnet.Framework.Core.Base;

namespace Sunnet.Cli.Impl.Users
{
    public class PrincipalRoleRpst : EFRepositoryBase<PrincipalRoleEntity, Int32>, IPrincipalRoleRpst
    {
        public PrincipalRoleRpst(IUnitOfWork unit)
        {
            this.UnitOfWork = unit;
        }
    }
}
