using Sunnet.Framework.Core.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Cli.Core.Trs.Entities
{
    public class TRSAnswerEntity : EntityBase<int>
    {
        public string Text { get; set; }
        public int Score { get; set; }

        public virtual ICollection<TRSItemAnswerEntity> ItemAnswers { get; set; }
    }
}