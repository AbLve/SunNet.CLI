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
    public class ScoreAgeOrWaveBandsRpst : EFRepositoryBase<ScoreAgeOrWaveBandsEntity, int>, IScoreAgeOrWaveBandsRpst
    {
        public ScoreAgeOrWaveBandsRpst(IUnitOfWork unit)
        {
            this.UnitOfWork = unit;
        }

    }
}
