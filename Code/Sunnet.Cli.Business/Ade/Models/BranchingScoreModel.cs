using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Cli.Business.Ade.Models
{
    public class BranchingScoreModel
    {
        public BranchingScoreModel()
        {
        }

        public int ID { get; set; }

        public decimal From { get; set; }

        public decimal To { get; set; }
        
        /// <summary>
        /// 当前Itemd Id
        /// </summary>
        public int ItemId { get; set; }

        /// <summary>
        /// 需跳转到的itemId
        /// </summary>
        public int SkipItemId { get; set; }

        public bool IsDeleted { get; set; }
    }
}
