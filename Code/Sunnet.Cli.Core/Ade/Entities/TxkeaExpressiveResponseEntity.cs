using Sunnet.Framework.Core.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Cli.Core.Ade.Entities
{
    public class TxkeaExpressiveResponseEntity : EntityBase<int>
    {
        public int ItemId { get; set; }

        [Description("Response Text")]
        public string Text { get; set; }

        [Description("Mandatory")]
        public bool Mandatory { get; set; }

        [Description("Response type")]
        public TypedResponseType Type { get; set; }

        /// <summary>
        /// number of check boxes or number of radio buttons 
        /// </summary>
        [Description("Number of Radio buttons")]
        public int Buttons { get; set; }

        public bool IsDeleted { get; set; }

        public virtual TxkeaExpressiveItemEntity Item { get; set; }

        private ICollection<TxkeaExpressiveResponseOptionEntity> _options;
        public virtual ICollection<TxkeaExpressiveResponseOptionEntity> Options
        {
            get { return _options ?? (_options = new List<TxkeaExpressiveResponseOptionEntity>()); }
            set { _options = value; }
        }
    }
}
