using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Sunnet.Cli.Core.DataProcess.Entities;
using Sunnet.Framework.Core.Base;
using Sunnet.Cli.Core.DataProcess.Models;

namespace Sunnet.Cli.Core.DataProcess.Interfaces
{
    public interface IDataGroupRpst : IRepository<DataGroupEntity,int>
    {
        string ImportData(string sql);

        void Start(int id, int createdBy);

        void Verify(int id);

        List<RecordRemarkModel> GetRemarks(string sql);
    }
}
