using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Sunnet.Framework.Core.Base;
using Sunnet.Cli.Core.Communities.Entities;
using Sunnet.Cli.Core.Communities.Interfaces;

namespace Sunnet.Cli.Impl.Communities
{
    public class CommunityNotesRpst : EFRepositoryBase<CommunityNotesEntity, Int32>, ICommunityNotesRpst
    {
        public CommunityNotesRpst(IUnitOfWork unit)
        {
            UnitOfWork = unit;
        }
    }
}
