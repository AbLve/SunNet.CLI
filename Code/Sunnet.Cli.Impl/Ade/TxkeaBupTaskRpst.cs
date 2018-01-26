using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		Joe
 * Computer:		JOE-PC
 * Domain:			Joe
 * CreatedOn:		2015/12/16
 * Description:		
 * Version History:	Created,2015/12/16
 * 
 * 
 **************************************************************************/
using Sunnet.Framework.Core.Base;
using Sunnet.Cli.Core.Ade.Entities;
using Sunnet.Cli.Core.Ade.Interfaces;

namespace Sunnet.Cli.Impl.Ade
{
    public class TxkeaBupTaskRpst : EFRepositoryBase<TxkeaBupTaskEntity, int>, ITxkeaBupTaskRpst
    {
        public TxkeaBupTaskRpst(IUnitOfWork unit)
        {
            this.UnitOfWork = unit;
        }

        public string ExecuteQuerySql(string sql)
        {
            try
            {
                string v = this.EFContext.DbContext.Database.SqlQuery<string>(sql).FirstOrDefault();
                return v;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
