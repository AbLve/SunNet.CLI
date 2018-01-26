using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Sunnet.Framework.Core.Base;
using Sunnet.Cli.Core.Communities.Entities;

namespace Sunnet.Cli.Core.Communities.Interfaces
{
    public interface ICommunityNotesRpst : IRepository<CommunityNotesEntity, Int32>
    {
    }
}
