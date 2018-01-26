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
    public class TRSAssessmentItemRpst : EFRepositoryBase<TRSAssessmentItemEntity, int>, ITRSAssessmentItemRpst
    {
        public TRSAssessmentItemRpst(IUnitOfWork unit)
        {
            UnitOfWork = unit;
        }

        public int UpdateItemAnswer(int trsAssessmentItemId, int AnswerId)
        {
            int count = 0;
            try
            {
                string strSql = "UPDATE  TRSAssessmentItems SET  AnswerId = @AnswerId,UpdatedOn=@UpdatedOn WHERE ID = @ID";
                AdeUnitOfWorkContext context = this.UnitOfWork as AdeUnitOfWorkContext;
                count = context.DbContext.Database.ExecuteSqlCommand(strSql,
                    new SqlParameter("AnswerId", AnswerId),
                    new SqlParameter("ID", trsAssessmentItemId),
                    new SqlParameter("UpdatedOn", DateTime.Now));
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return count;
        }

        public int DelAssessmentItems(List<int> ids)
        {
            int count = 0;
            try
            {
                string strIds = string.Join(",", ids);
                string strSql = string.Format("DELETE FROM dbo.TRSAssessmentItems WHERE ID IN({0});", strIds);
                AdeUnitOfWorkContext context = this.UnitOfWork as AdeUnitOfWorkContext;
                count = context.DbContext.Database.ExecuteSqlCommand(strSql);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return count;
        }
    }
}