using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		JackZhang
 * Computer:		JACKZ
 * Domain:			JackZ
 * CreatedOn:		1/27 2015 14:54:33
 * Description:		Please input class summary
 * Version History:	Created,1/27 2015 14:54:33
 * 
 * 
 **************************************************************************/
using Sunnet.Framework.Core.Base;

namespace Sunnet.Cli.Core.Cot.Entities
{
    public class CotStgReportItemEntity : EntityBase<int>
    {
        public int CotStgReportId { get; set; }

        /// <summary>
        /// COT Assessment Item Id.
        /// </summary>
        public int ItemId { get; set; }

        public int Sort { get; set; }

        public DateTime GoalMetDate { get; set; }

        public virtual CotStgReportEntity Report { get; set; }

        public virtual CotAssessmentItemEntity Item { get; set; }

    }
}
