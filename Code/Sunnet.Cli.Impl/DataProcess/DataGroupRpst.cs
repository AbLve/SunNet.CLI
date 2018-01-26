using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Sunnet.Framework.Core.Base;
using Sunnet.Cli.Core.DataProcess.Entities;
using Sunnet.Cli.Core.DataProcess.Interfaces;
using Sunnet.Cli.Core;
using System.Data.SqlClient;
using System.Data;
using Sunnet.Cli.Core.DataProcess.Models;

namespace Sunnet.Cli.Impl.DataProcess
{
    public class DataGroupRpst : EFRepositoryBase<DataGroupEntity, Int32>, IDataGroupRpst
    {
        public DataGroupRpst(IUnitOfWork unit)
        {
            UnitOfWork = unit;
        }

        public string ImportData(string sql)
        {
            //不能在此Dispose掉context，因为执行过此方法后，还会用到context
            EFUnitOfWorkContext context = this.UnitOfWork as EFUnitOfWorkContext;
            try
            {
                return context.DbContext.Database.SqlQuery<string>(sql).FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }

        public void Start(int id, int createdBy)
        {
            //不能在此Dispose掉context，因为执行过此方法后，还会用到context
            EFUnitOfWorkContext context = this.UnitOfWork as EFUnitOfWorkContext;

            try
            {
                using (SqlConnection conn = new SqlConnection(context.DbContext.Database.Connection.ConnectionString))
                {
                    if (conn.State != ConnectionState.Open)
                        conn.Open();

                    SqlCommand cmd = new SqlCommand("DataProcessGroup", conn);
                    SqlParameter para = new SqlParameter("@ID", id);
                    cmd.Parameters.Add(para);
                    para = new SqlParameter("@CreatedBy", createdBy);
                    cmd.Parameters.Add(para);

                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 40 * 60;  //Second
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            
            //context.DbContext.Database.ExecuteSqlCommand("exec ImportDataBatch  @ID", parms);
        }

        public void Verify(int id)
        {
            //不能在此Dispose掉context，因为执行过此方法后，还会用到context
            EFUnitOfWorkContext context = this.UnitOfWork as EFUnitOfWorkContext;


            using (SqlConnection conn = new SqlConnection(context.DbContext.Database.Connection.ConnectionString))
            {
                if (conn.State != ConnectionState.Open)
                    conn.Open();

                SqlCommand cmd = new SqlCommand("ImportDataVerify", conn);
                SqlParameter para = new SqlParameter("@ID", id);
                cmd.Parameters.Add(para);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 20 * 60;  //Second
                cmd.ExecuteNonQuery();
            }
        }

        public List<RecordRemarkModel> GetRemarks(string sql)
        {
            //不能在此Dispose掉context，因为执行过此方法后，还会用到context
            EFUnitOfWorkContext context = this.UnitOfWork as EFUnitOfWorkContext;
            try
            {
                return context.DbContext.Database.SqlQuery<RecordRemarkModel>(sql).ToList();
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }
    }
}
