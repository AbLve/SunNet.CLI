using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		Sam
 * Computer:		SAM-PC
 * Domain:			sam-pc
 * CreatedOn:		2014/8/5 15:48:47
 * Description:		Please input class summary
 * Version History:	Created,2014/8/5 15:48:47
 * 
 * 
 **************************************************************************/
using Sunnet.Cli.Core.Users.Entities;
using Sunnet.Cli.Core.Users.Interfaces;
using Sunnet.Framework.Core.Base;

namespace Sunnet.Cli.Impl.Users
{
    public class ParentRpst : EFRepositoryBase<ParentEntity, Int32>, IParentRpst
    {
        public ParentRpst(IUnitOfWork unit)
        {
            this.UnitOfWork = unit;
        }
    }
}
