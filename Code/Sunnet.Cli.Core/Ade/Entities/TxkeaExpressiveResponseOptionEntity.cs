using Sunnet.Framework.Core.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Cli.Core.Ade.Entities
{
    public class TxkeaExpressiveResponseOptionEntity : EntityBase<int>
    {
        public int ResponseId { get; set; }

        /// <summary>
        /// 正确选项 (CPALLS+ 时借用，ade 不需要)
        /// </summary>
        public bool IsCorrect { get; set; }

        /// <summary>
        /// 该选项分值
        /// </summary>
        public decimal Score { get; set; }

        /// <summary>
        /// 选项名称
        /// </summary>
        public string Lable { get; set; }

        /// <summary>
        /// 是否出现 文本框
        /// </summary>
        public bool AddTextbox { get; set; }

        public bool IsDeleted { get; set; }

        public virtual TxkeaExpressiveResponseEntity Response { get; set; }
    }
}
