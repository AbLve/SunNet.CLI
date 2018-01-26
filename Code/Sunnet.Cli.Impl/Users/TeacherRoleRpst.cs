using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		Leason
 * Computer:		Leason-PC
 * Domain:			Leason-pc
 * CreatedOn:		2014/9/16 14:41:20
 * Description:		Create TeacherRoleRpst
 * Version History:	Created,2014/9/16 14:41:20
 * 
 * 
 **************************************************************************/
using Sunnet.Cli.Core.Users.Entities;
using Sunnet.Cli.Core.Users.Interfaces;
using Sunnet.Framework.Core.Base;

namespace Sunnet.Cli.Impl.Users
{
    public class TeacherRoleRpst : EFRepositoryBase<TeacherRoleEntity, Int32>, ITeacherRoleRpst
    {
        public TeacherRoleRpst(IUnitOfWork unit)
        {
            UnitOfWork = unit;
        }
    }
}
