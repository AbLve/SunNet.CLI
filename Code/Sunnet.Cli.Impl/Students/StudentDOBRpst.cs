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
    public class StudentDOBRpst : EFRepositoryBase<StudentDOBEntity, Int32>, IStudentDOBRpst
    {
        public StudentDOBRpst(IUnitOfWork unit)
        {
            this.UnitOfWork = unit;
        }
    }
}
