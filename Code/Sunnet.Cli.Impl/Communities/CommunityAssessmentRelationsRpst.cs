using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sunnet.Cli.Core.Communities.Entities;
using Sunnet.Cli.Core.Communities.Interfaces;
using Sunnet.Cli.Core.Schools.Entities;
using Sunnet.Cli.Core.Schools.Interfaces;
using Sunnet.Framework.Core.Base;

namespace Sunnet.Cli.Impl.Communities
{
    public class CommunityAssessmentRelationsRpst : EFRepositoryBase<CommunityAssessmentRelationsEntity, Int32>, ICommunityAssessmentRelationsRpst
    {
        public CommunityAssessmentRelationsRpst(IUnitOfWork unit)
        {
            this.UnitOfWork = unit;
        }
    }
}
