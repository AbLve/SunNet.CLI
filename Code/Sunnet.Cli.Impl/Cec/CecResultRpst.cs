using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		Leason
 * Computer:		Leason-PC
 * Domain:			Leason
 * CreatedOn:		2014/12/1 11:20:00
 * Description:		CecResultRpst
 * Version History:	Created,2014/12/1 11:20:00
 * 
 * 
 **************************************************************************/
using Sunnet.Cli.Core.Cec.Entities;
using Sunnet.Cli.Core.Cec.Interfaces;
using Sunnet.Framework.Core.Base;

namespace Sunnet.Cli.Impl.Cec
{
    public class CecResultRpst:EFRepositoryBase<CecResultEntity, Int32>, ICecResultRpst
    {
        public CecResultRpst(IUnitOfWork unit)
        {
            UnitOfWork = unit;
        }
    }
}
