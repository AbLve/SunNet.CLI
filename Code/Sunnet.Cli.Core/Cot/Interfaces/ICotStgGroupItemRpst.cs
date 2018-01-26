using Sunnet.Cli.Core.Cot.Entities;
using Sunnet.Cli.Core.Cot.Models;
using Sunnet.Framework.Core.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Cli.Core.Cot.Interfaces
{
    public interface ICotStgGroupItemRpst : IRepository<CotStgGroupItemEntity, int>
    {
        List<CotStgGroupItemModel> GetCotStgGroupItems(int cotStgReportId);
    }
}
