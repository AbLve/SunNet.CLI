using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		JackZhang
 * Computer:		JACKZ
 * Domain:			Jackz
 * CreatedOn:		2014/10/7 4:37:22
 * Description:		Please input class summary
 * Version History:	Created,2014/10/7 4:37:22
 * 
 * 
 **************************************************************************/
using Sunnet.Cli.Core.Cpalls.Entities;
using Sunnet.Cli.Core.Cpalls.Interfaces;
using Sunnet.Framework.Core.Base;

namespace Sunnet.Cli.Impl.Cpalls
{
    public class CpallsStudentGroupRpst : EFRepositoryBase<CpallsStudentGroupEntity, int>, ICpallsStudentGroupRpst
    {
        public CpallsStudentGroupRpst(IUnitOfWork unit)
        {
            this.UnitOfWork = unit;
        }
    }
}
