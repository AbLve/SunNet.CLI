using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		Leason
 * Computer:		Leason-PC
 * Domain:			Leason-pc
 * CreatedOn:		2014/9/10 16:32:20
 * Description:		Create ClassRoleRpst
 * Version History:	Created,2014/9/10 16:32:20
 * 
 * 
 **************************************************************************/
using Sunnet.Cli.Core.Classes.Entites;
using Sunnet.Cli.Core.Classes.Interfaces;
using Sunnet.Framework.Core.Base;

namespace Sunnet.Cli.Impl.Classes
{
    class ClassRoleRpst : EFRepositoryBase<ClassRoleEntity, Int32>, IClassRoleRpst
    {
        public ClassRoleRpst(IUnitOfWork unit)
        {
            UnitOfWork = unit;
        }
    }
}
