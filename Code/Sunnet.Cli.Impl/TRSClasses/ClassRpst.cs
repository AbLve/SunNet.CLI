using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sunnet.Cli.Core;
using Sunnet.Cli.Core.TRSClasses.Entites;
using Sunnet.Cli.Core.TRSClasses.Interfaces;
using Sunnet.Framework.Core.Base;

namespace Sunnet.Cli.Impl.TRSClasses
{
    public class TRSClassRpst : EFRepositoryBase<TRSClassEntity, Int32>, ITRSClassRpst
    {
        public TRSClassRpst(IUnitOfWork unit)
        {
            UnitOfWork = unit;
        }

        public bool UpdateTRSClassPlayground(int playgroundId, int[] trsClassIds = null)
        {
            StringBuilder sb = new StringBuilder();
            EFUnitOfWorkContext context = this.UnitOfWork as EFUnitOfWorkContext;
            if (trsClassIds != null && trsClassIds.Length > 0)
            {
                string ids = string.Join(",", trsClassIds);
                sb.AppendFormat(
                    @" UPDATE TRSClasses SET playgroundId =0 where playgroundId ={1};UPDATE TRSClasses SET playgroundId ={1} where ID in ({0})",
                    ids, playgroundId);
            }
            else
            {
                sb.AppendFormat(@" UPDATE TRSClasses SET playgroundId =0 where playgroundId ={0};", playgroundId);
            }
            return context.DbContext.Database.ExecuteSqlCommand(sb.ToString()) > 0;
        }
    }
}