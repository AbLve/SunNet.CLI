using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sunnet.Framework.Core.Base;

namespace Sunnet.Cli.Core.Ade.Entities
{
    public class TypedResponseOptionEntity : EntityBase<int>
    {
        public TypedResponseType Type { get; set; }

        public string Keyword { get; set; }

        public decimal From { get; set; }

        public decimal To { get; set; }

        public decimal Score { get; set; }

        public int ResponseId { get; set; }

        public bool IsDeleted { get; set; }

        public virtual TypedResponseEntity Response { get; set; }
    }
}
