using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		Joe
 * Computer:		JOE-PC
 * Domain:			Joe-pc
 * CreatedOn:		2014/9/2 12:15:10
 * Description:		Please input class summary
 * Version History:	Created,2014/9/2 12:15:10
 * 
 * 
 **************************************************************************/
using Sunnet.Framework.Core.Base;
using Sunnet.Cli.Core.Permission.Entities;
using Sunnet.Cli.Core.Permission.Interfaces;

namespace Sunnet.Cli.Impl.Permission
{
    public class RolePageAuthorityRpst : EFRepositoryBase<RolePageAuthorityEntity, Int32>, IRolePageAuthorityRpst
    {
        public RolePageAuthorityRpst(IUnitOfWork unit)
        {
            this.UnitOfWork = unit;
        }
    }
}
