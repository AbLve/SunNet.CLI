using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sunnet.Cli.Core.Students.Entities;
using Sunnet.Cli.Core.Students.Interfaces;
using Sunnet.Framework.Core.Base;

namespace Sunnet.Cli.Impl.Students
{
    public class StudentRoleRpst : EFRepositoryBase<StudentRoleEntity, Int32>, IStudentRoleRpst
    {
        public StudentRoleRpst(IUnitOfWork unit)
        {
            this.UnitOfWork = unit;
        }
    }
}
