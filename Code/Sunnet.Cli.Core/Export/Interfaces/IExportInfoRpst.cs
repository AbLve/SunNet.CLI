using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Text;
using System.Threading.Tasks;

using Sunnet.Framework.Core.Base;
using Sunnet.Cli.Core.Export.Entities;
namespace Sunnet.Cli.Core.Export.Interfaces
{
    public interface IExportInfoRpst : IRepository<ExportInfoEntity, Int32>
    {
        DataSet ExecuteExportSql(string executeSql);
    }
}
