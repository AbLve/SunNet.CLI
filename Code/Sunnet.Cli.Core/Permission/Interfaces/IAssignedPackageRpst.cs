using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		Joe
 * Computer:		Joe-PC
 * Domain:			Joe-pc
 * CreatedOn:		2014/9/5
 * Description:		Create CountyEntity
 * Version History:	Created,2014/9/5
 * 
 * 
 **************************************************************************/
using Sunnet.Framework.Core.Base;
using Sunnet.Cli.Core.Permission.Entities;

namespace Sunnet.Cli.Core.Permission.Interfaces
{
    public interface IAssignedPackageRpst : IRepository<AssignedPackageEntity, Int32>
    {
    }
}
