using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		Leason
 * Computer:		Leason-PC
 * Domain:			Leason-pc
 * CreatedOn:		2014/8/23 12:20:20
 * Description:		Create ClassroomRpst
 * Version History:	Created,2014/8/23 12:20:20
 * 
 * 
 **************************************************************************/
using Sunnet.Cli.Core.Classrooms.Entites;
using Sunnet.Cli.Core.Classrooms.Interfaces;
using Sunnet.Framework.Core.Base;

namespace Sunnet.Cli.Impl.Classrooms
{
    public class ClassroomRoleRpst:EFRepositoryBase<ClassroomRoleEntity,Int32>,IClassroomRoleRpst
    {
        public ClassroomRoleRpst(IUnitOfWork unit)
        {
            UnitOfWork = unit;
        }
    }
}
