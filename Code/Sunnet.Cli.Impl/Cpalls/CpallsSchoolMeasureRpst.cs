using Sunnet.Cli.Core.Cpalls.Entities;
using Sunnet.Cli.Core.Cpalls.Interfaces;
using Sunnet.Framework.Core.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Cli.Impl.Cpalls
{
    public class CpallsSchoolMeasureRpst : EFRepositoryBase<CpallsSchoolMeasureEntity, int>, ICpallsSchoolMeasureRpst
    {
        public CpallsSchoolMeasureRpst(IUnitOfWork unit)
        {
            this.UnitOfWork = unit;
        }
    }
}
