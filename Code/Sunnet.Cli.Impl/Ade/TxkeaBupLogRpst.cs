using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		Joe
 * Computer:		JOE-PC
 * Domain:			Joe
 * CreatedOn:		2015/12/22
 * Description:		
 * Version History:	Created,2015/12/22
 * 
 * 
 **************************************************************************/
using Sunnet.Framework.Core.Base;
using Sunnet.Cli.Core.Ade.Entities;
using Sunnet.Cli.Core.Ade.Interfaces;


namespace Sunnet.Cli.Impl.Ade
{
    public class TxkeaBupLogRpst : EFRepositoryBase<TxkeaBupLogEntity, int>, ITxkeaBupLogRpst
    {
        public TxkeaBupLogRpst(IUnitOfWork unit)
        {
            this.UnitOfWork = unit;
        }
    }
}
