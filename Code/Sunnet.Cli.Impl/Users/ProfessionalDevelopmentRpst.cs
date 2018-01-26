using Sunnet.Cli.Core.Users.Entities;
using Sunnet.Cli.Core.Users.Interfaces;
using Sunnet.Framework.Core.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Cli.Impl.Users
{
    public class ProfessionalDevelopmentRpst : EFRepositoryBase<ProfessionalDevelopmentEntity, Int32>, IProfessionalDevelopmentRpst
    {
        public ProfessionalDevelopmentRpst(IUnitOfWork unit)
        {
            UnitOfWork = unit;
        }
    }
}
