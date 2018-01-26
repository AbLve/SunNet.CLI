using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		JackZhang
 * Computer:		JACKZ
 * Domain:			Jackz
 * CreatedOn:		2014/12/16 13:58:24
 * Description:		Please input class summary
 * Version History:	Created,2014/12/16 13:58:24
 * 
 * 
 **************************************************************************/
using Sunnet.Cli.Core.Cot.Entities;
using Sunnet.Cli.Core.Cot.Interfaces;
using Sunnet.Framework.Core.Base;

namespace Sunnet.Cli.Impl.Cot
{
    public class CotAssessmentWaveItemRpst : EFRepositoryBase<CotAssessmentWaveItemEntity, int>, ICotAssessmentWaveItemRpst
    {
        public CotAssessmentWaveItemRpst(IUnitOfWork unit)
        {
            this.UnitOfWork = unit;
        }
    }
}
