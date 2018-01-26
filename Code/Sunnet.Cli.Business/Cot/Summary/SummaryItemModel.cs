using Sunnet.Cli.Business.Ade.Models;
using Sunnet.Cli.Business.Common;
using Sunnet.Cli.Core.Ade;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Cli.Business.Cot.Summary
{
    public class SummaryItemModel
    {
        public int Id { get; set; }

        public CotLevel Level { get; set; }

        public string CotItemNo { get; set; }

        /// <summary>
        /// Observation at BOY | Observation at MOY | COT Updates | Goals Met
        /// </summary>
        public int Count { get; set; }

        public string Description { get; set; }

        public DateTime SetDate { get; set; }

        public bool IsSet
        {
            get
            {
                return SetDate > CommonAgent.MinDate;
            }
        }

        public DateTime MetDate { get; set; }

        public bool IsMet
        {
            get
            {
                return MetDate > CommonAgent.MinDate;
            }
        }

        public bool IsBlank { get; set; }

        public List<AdeLinkModel> Links { get; set; }
    }
}
