using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		Sam
 * Computer:		SAM-PC
 * Domain:			sam-pc
 * CreatedOn:		2014/8/5 16:22:44
 * Description:		Please input class summary
 * Version History:	Created,2014/8/5 16:22:44
 * 
 * 
 **************************************************************************/
using Sunnet.Cli.Core.Users.Entities;
using Sunnet.Framework.Core.Base;

namespace Sunnet.Cli.Core.Users.Interfaces
{
    public interface IUserBaseRepository : IRepository<UserBaseEntity, Int32>
    {

    }

}
