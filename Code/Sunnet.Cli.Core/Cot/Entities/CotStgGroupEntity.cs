using Sunnet.Framework.Core.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Cli.Core.Cot.Entities
{
    public class CotStgGroupEntity : EntityBase<int>
    {
        public int CotStgReportId { get; set; }
        public string GroupName { get; set; }
        public string OnMyOwn { get; set; }
        public string WithSupport { get; set; }
        public bool IsDelete { get; set; }

        public virtual CotStgReportEntity CotStgReport { get; set; }

        private ICollection<CotStgGroupItemEntity> _cotStgGroupItems;

        public virtual ICollection<CotStgGroupItemEntity> CotStgGroupItems
        {
            get { return _cotStgGroupItems ?? (_cotStgGroupItems = new Collection<CotStgGroupItemEntity>()); }
            set { _cotStgGroupItems = value; }
        }
    }
}
