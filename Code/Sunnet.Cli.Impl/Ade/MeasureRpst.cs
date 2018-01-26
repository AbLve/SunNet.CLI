using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sunnet.Cli.Core;
using Sunnet.Cli.Core.Ade.Entities;
using Sunnet.Cli.Core.Ade.Interfaces;
using Sunnet.Framework.Core.Base;
/**************************************************************************
 * Developer: 		Jack
 * Computer:		Jackz
 * Domain:			Jackz
 * CreatedOn:		08/11/2014 03:52:57
 * Description:		MeasureEntity's IRepository
 * Version History:	Created,08/11/2014 03:52:57
 * 
 * 
 **************************************************************************/
using Sunnet.Framework.Extensions;

namespace Sunnet.Cli.Impl.Ade
{
    /// <summary>
    /// MeasureEntity's Repository
    /// </summary>
    public class MeasureRpst : EFRepositoryBase<MeasureEntity, int>, IMeasureRpst
    {
        public MeasureRpst(IUnitOfWork unit)
        {
            this.UnitOfWork = unit;
        }

        public bool AdjustOrder(List<int> measureIds)
        {
            var strSql = new StringBuilder();
            var index = 0;
            measureIds.ForEach(x => strSql.AppendFormat("UPDATE [Measures] SET [Sort] = {0} WHERE ID = {1} ;", index++, x));
            return this.EFContext.DbContext.Database.ExecuteSqlCommand(strSql.ToString()) >= 0;
        }

        public void RecalculateTotalScore()
        {
            string strSql = @"Recalculate_Ade_Measure_TotalScore";
            var result = EFContext.DbContext.Database.ExecuteSqlCommand(strSql);
        }

        public int UpdateCutOffScoresChanged(int measureId, bool cutoffScoresChanged)
        {
            int count = 0;
            try
            {
                string strSql = "UPDATE dbo.Measures SET CutOffScoresChanged=@CutOffScoresChanged,UpdatedOn=@UpdatedOn WHERE ID=@ID";
                AdeUnitOfWorkContext context = this.UnitOfWork as AdeUnitOfWorkContext;
                count = context.DbContext.Database.ExecuteSqlCommand(strSql,
                    new SqlParameter("CutOffScoresChanged", cutoffScoresChanged),
                    new SqlParameter("ID", measureId),
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
