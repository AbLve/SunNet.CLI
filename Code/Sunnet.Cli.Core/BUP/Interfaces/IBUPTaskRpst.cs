using Sunnet.Cli.Core.BUP.Entities;
using Sunnet.Framework.Core.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Cli.Core.BUP.Interfaces
{
    public interface IBUPTaskRpst : IRepository<BUPTaskEntity, Int32>
    {
        string ExecuteCommunitySql(string sql);

        int ExecuteSqlCommand(string sql);

        dynamic ExecuteSqlQuery(string sql, BUPType type);

        void Start(int id, int createdBy);
    }
}
