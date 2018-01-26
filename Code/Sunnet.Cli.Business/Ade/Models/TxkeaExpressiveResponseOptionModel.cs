using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sunnet.Cli.Core.Ade;


namespace Sunnet.Cli.Business.Ade.Models
{
    public class TxkeaExpressiveResponseOptionModel
    {
        public TxkeaExpressiveResponseOptionModel()
        {

        }

        public int ID { get; set; }

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
        /// 是否添加文本框
        /// </summary>
        public bool AddTextbox { get; set; }

        public bool IsDeleted { get; set; }
    }
}
