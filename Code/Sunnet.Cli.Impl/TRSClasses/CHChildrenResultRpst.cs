using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sunnet.Cli.Core.TRSClasses.Entites;
using Sunnet.Cli.Core.TRSClasses.Interfaces;
using Sunnet.Framework.Core.Base;
using Sunnet.Cli.Core;
using System.Data;

namespace Sunnet.Cli.Impl.TRSClasses
{
    public class CHChildrenResultRpst : EFRepositoryBase<CHChildrenResultEntity, Int32>, ICHChildrenResultRpst
    {
        public CHChildrenResultRpst(IUnitOfWork unit)
        {
            UnitOfWork = unit;
        }

        public void DeleteResultBySchoolId(int schoolId)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("update TRSClasses set TypeOfClass = 1 where SchoolId = {0}", schoolId);

            EFUnitOfWorkContext context = this.UnitOfWork as EFUnitOfWorkContext;

            try
            {
                context.DbContext.Database.ExecuteSqlCommand(sb.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
