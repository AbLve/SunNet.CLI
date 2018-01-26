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

namespace Sunnet.Cli.Core.Permission.Interfaces
{
    public interface IDisabledUserRoleRpst : IRepository<DisabledUserRoleEntity, Int32>
    {

    }
}
