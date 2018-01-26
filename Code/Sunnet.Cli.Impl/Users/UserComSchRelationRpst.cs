using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		Sam
 * Computer:		SAM-PC
 * Domain:			sam-pc
 * CreatedOn:		2014/9/13 13:56:28
 * Description:		Please input class summary
 * Version History:	Created,2014/9/13 13:56:28
 * 
 * 
 **************************************************************************/
using Sunnet.Cli.Core.Users.Entities;
using Sunnet.Cli.Core.Users.Interfaces;
using Sunnet.Framework.Core.Base;

namespace Sunnet.Cli.Impl.Users
{
    public class UserComSchRelationRpst : EFRepositoryBase<UserComSchRelationEntity, int>, IUserComSchRelationRpst
    {
        public UserComSchRelationRpst(IUnitOfWork unit)
        {
            this.UnitOfWork = unit;
        }
    }
}
