using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sunnet.Cli.Core.Users.Entities;
using Sunnet.Framework.Core.Base;

namespace Sunnet.Cli.Core.Trs.Entities
{
    public class TRSNotificationEntity: EntityBase<int>
    {
        public int UserId { get; set; }

        public int EventLogId { get; set; }

        public virtual TRSEventLogEntity EventLog { get; set; }
    }
}
