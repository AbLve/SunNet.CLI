using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		Damon
 * Computer:		Damon-PC
 * Domain:			Damon-pc
 * CreatedOn:		2014/8/8 10:58:18
 * Description:		Please input class summary
 * Version History:	Created,2014/8/8 10:58:18
 * 
 * 
 **************************************************************************/
using Sunnet.Cli.Core.Users.Entities;
using Sunnet.Cli.Core.Users.Interfaces;
using Sunnet.Framework.Core.Base;

namespace Sunnet.Cli.Impl.Users
{

    public class PrincipalRpst : EFRepositoryBase<PrincipalEntity, Int32>, IPrincipalRpst
    {
        public PrincipalRpst(IUnitOfWork unit)
        {
            this.UnitOfWork = unit;
        }
    }

}
