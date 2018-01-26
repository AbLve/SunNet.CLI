using Sunnet.Cli.Core.BUP.Enums;
using Sunnet.Framework.Core.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Cli.Core.BUP.Entities
{
    public class BUPTaskEntity : EntityBase<int>
    {
        public BUPType Type { get; set; }

        public BUPStatus Status { get; set; }

        public string Remark { get; set; }

        public int RecordCount { get; set; }

        public int FailCount { get; set; }

        public int SuccessCount { get; set; }

        public int CreatedBy { get; set; }

        public int UpdatedBy { get; set; }

        public BUPProcessType ProcessType { get; set; }

        public string OriginFileName { get; set; }

        public bool SendInvitation { get; set; }

        public string FilePath { get; set; }
    }
}
