using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

using Sunnet.Framework.Core.Base;
using Sunnet.Cli.Core.Export.Entities;
using Sunnet.Cli.Core.Export.Interfaces;
using Sunnet.Cli.Core;

namespace Sunnet.Cli.Impl.Export
{
    public class ExportInfoRpst : EFRepositoryBase<ExportInfoEntity, Int32>, IExportInfoRpst
    {
        public ExportInfoRpst(IUnitOfWork unit)
        {
            UnitOfWork = unit;
        }

        public DataSet ExecuteExportSql(string executeSql)
        {
            EFUnitOfWorkContext context = this.UnitOfWork as EFUnitOfWorkContext;
            using (SqlConnection conn = new SqlConnection(context.DbContext.Database.Connection.ConnectionString))
            {
                if (conn.State != ConnectionState.Open)
                    conn.Open();

                SqlCommand cmd = new SqlCommand(executeSql, conn);
                cmd.CommandTimeout = 20 * 60;  //Second
                cmd.ExecuteNonQuery();
                SqlDataAdapter sdr = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                sdr.Fill(ds);
                return ds;
            }
        }
    }
}
