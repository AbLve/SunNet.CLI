using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		Joe
 * Computer:		Joe-PC
 * Domain:			Joe-pc
 * CreatedOn:		2015/7/22
 * Description:		Create CountyEntity
 * Version History:	Created,2015/7/22
 * 
 * 
 **************************************************************************/
using Sunnet.Framework.Core.Base;
using Sunnet.Cli.Core.Permission.Entities;
using Sunnet.Cli.Core.Permission.Interfaces;

namespace Sunnet.Cli.Impl.Permission
{
    public class DisabledUserRoleRpst : EFRepositoryBase<DisabledUserRoleEntity, Int32>, IDisabledUserRoleRpst
    {
        public DisabledUserRoleRpst(IUnitOfWork unit)
        {
            this.UnitOfWork = unit;
        }
    }
}
