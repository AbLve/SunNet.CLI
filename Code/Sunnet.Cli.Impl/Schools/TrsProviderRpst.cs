using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		Sam
 * Computer:		SAM-PC
 * Domain:			sam-pc
 * CreatedOn:		2014/8/19 12:15:10
 * Description:		Please input class summary
 * Version History:	Created,2014/8/19 12:15:10
 * 
 * 
 **************************************************************************/
using Sunnet.Cli.Core.Schools.Entities;
using Sunnet.Cli.Core.Schools.Interfaces;
using Sunnet.Framework.Core.Base;

namespace Sunnet.Cli.Impl.Schools
{
    public class TrsProviderRpst : EFRepositoryBase<TrsProviderEntity, Int32>, ITrsProviderRpst
    {
        public TrsProviderRpst(IUnitOfWork unit)
        {
            this.UnitOfWork = unit;
        }
    }
}
