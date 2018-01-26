using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		Joe
 * Computer:		Joe-PC
 * Domain:			Joe-pc
 * CreatedOn:		2014/9/2 
 * Description:		Please input class summary
 * Version History:	Created,2014/9/2 9:02:02
 * 
 * 
 **************************************************************************/
using Sunnet.Framework.Core.Base;
using Sunnet.Cli.Core.Permission.Entities;
using Sunnet.Cli.Core.Users.Enums;

namespace Sunnet.Cli.Core.Permission.Interfaces
{
    public interface IPermissionRoleRpst : IRepository<PermissionRoleEntity, Int32>
    {
        int DeleteUserRole(int RoleId);

        int DeleteUserRole_Removed(int RoleId, Role role);
    }
}
