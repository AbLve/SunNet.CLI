using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		Joe
 * Computer:		Joe-PC
 * Domain:			Joe-pc
 * CreatedOn:		2015/7/13
 * Description:		Create AuthorityEntity
 * Version History:	Created,2015/7/13
 * 
 * 
 **************************************************************************/
using Sunnet.Framework.Core.Base;
using Sunnet.Cli.Core.Users.Entities;
using Sunnet.Cli.Core.Users.Interfaces;
namespace Sunnet.Cli.Impl.Users
{
    public class IntManaCoachRelationRpst : EFRepositoryBase<IntManaCoachRelationEntity, Int32>, IIntManaCoachRelationRpst
    {
        public IntManaCoachRelationRpst(IUnitOfWork unit)
        {
            this.UnitOfWork = unit;
        }
    }
}
