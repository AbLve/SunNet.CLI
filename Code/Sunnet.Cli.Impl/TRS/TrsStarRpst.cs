using Sunnet.Cli.Core;
using Sunnet.Cli.Core.Trs.Entities;
using Sunnet.Cli.Core.Trs.Interfaces;
using Sunnet.Framework.Core.Base;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Cli.Impl.TRS
{
    public class TrsStarRpst : EFRepositoryBase<TrsStarEntity, int>, ITrsStarRpst
    {
        public TrsStarRpst(IUnitOfWork unit)
        {
            UnitOfWork = unit;
        }

        public int UpdateTrsStar(int trsStarId, byte newStar)
        {
            int count = 0;
            try
            {
                string strSql = "UPDATE  TrsStars SET  Star = @Star WHERE ID = @ID";
                AdeUnitOfWorkContext context = this.UnitOfWork as AdeUnitOfWorkContext;
                count = context.DbContext.Database.ExecuteSqlCommand(strSql,
                    new SqlParameter("Star", newStar),
                    new SqlParameter("ID", trsStarId));
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return count;
        }
    }
}