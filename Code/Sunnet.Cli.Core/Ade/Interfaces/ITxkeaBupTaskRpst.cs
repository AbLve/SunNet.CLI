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


namespace Sunnet.Cli.Core.Ade.Interfaces
{
    public interface ITxkeaBupTaskRpst : IRepository<TxkeaBupTaskEntity, int>
    {
        string ExecuteQuerySql(string sql);
    }
}
