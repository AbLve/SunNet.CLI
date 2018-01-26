using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sunnet.Cli.Core.TRSClasses.Entites;
using Sunnet.Framework.Core.Base;
using Sunnet.Cli.Core.Classes.Entites;

namespace Sunnet.Cli.Core.TRSClasses.Interfaces
{
    public interface ICHChildrenResultRpst : IRepository<CHChildrenResultEntity, Int32>
    {
        void DeleteResultBySchoolId(int schoolId);
    }
}
