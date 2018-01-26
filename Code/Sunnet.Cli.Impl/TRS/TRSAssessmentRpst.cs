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
    public class TRSAssessmentRpst : EFRepositoryBase<TRSAssessmentEntity, int>, ITRSAssessmentRpst
    {
        public TRSAssessmentRpst(IUnitOfWork unit)
        {
            UnitOfWork = unit;
        }

        public int DeleteOfflineAssessment(List<int> assessmentIds)
        {
            string Sql = "update TRSAssessments set IsDeleted=1 where ID in (";
            foreach (int item in assessmentIds)
            {
                Sql += item + ",";
            }
            if (Sql.EndsWith(","))
            {
                Sql = Sql.TrimEnd(',');
            }
            Sql += ")";
            AdeUnitOfWorkContext context = this.UnitOfWork as AdeUnitOfWorkContext;
            return context.DbContext.Database.ExecuteSqlCommand(Sql, "");
        }

        public int UpdateTrsAssessmentStar(int trsAssessmentId, byte newStar)
        {
            int count = 0;
            try
            {
                string strSql = "UPDATE  TRSAssessments SET  Star = @Star,UpdatedOn=@UpdatedOn WHERE ID = @ID";
                AdeUnitOfWorkContext context = this.UnitOfWork as AdeUnitOfWorkContext;
                count = context.DbContext.Database.ExecuteSqlCommand(strSql,
                    new SqlParameter("Star", newStar),
                    new SqlParameter("ID", trsAssessmentId),
                    new SqlParameter("UpdatedOn", DateTime.Now));
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return count;
        }
    }
}