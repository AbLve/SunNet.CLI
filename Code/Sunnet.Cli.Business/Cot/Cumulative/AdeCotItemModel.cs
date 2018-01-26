/**************************************************************************
 * Developer: 		JackZhang
 * Computer:		JACKZ
 * Domain:			JackZ
 * CreatedOn:		2/7 2015 13:52:12
 * Description:		Please input class summary
 * Version History:	Created,2/7 2015 13:52:12
 * 
 * 
 **************************************************************************/
using Sunnet.Cli.Business.Ade.Models;
using Sunnet.Cli.Business.Common;
using Sunnet.Cli.Core.Ade;
using System;
using System.Collections.Generic;

namespace Sunnet.Cli.Business.Cot.Cumulative
{
    public class AdeCotItemModel
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

        public bool IsFillColor { get; set; }

        public bool NeedSupport { get; set; }

        public List<AdeLinkModel> Links { get; set; }
    }
}
