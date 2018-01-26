using Sunnet.Framework.Core.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Cli.Core.Trs.Entities
{
    public class TRSItemAnswerEntity : EntityBase<int>
    {
        public int ItemId { get; set; }

        public int AnswerId { get; set; }

        public virtual TRSAnswerEntity Answer { get; set; }

        public virtual TRSItemEntity TRSItem { get; set; }
    }
}
