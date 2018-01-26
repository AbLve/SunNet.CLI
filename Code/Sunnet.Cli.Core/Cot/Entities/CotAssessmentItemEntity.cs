using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		JackZhang
 * Computer:		JACKZ
 * Domain:			Jackz
 * CreatedOn:		2014/12/16 9:12:10
 * Description:		Please input class summary
 * Version History:	Created,2014/12/16 9:12:10
 * 
 * 
 **************************************************************************/
using Sunnet.Cli.Core.Ade.Entities;
using Sunnet.Framework.Core.Base;

namespace Sunnet.Cli.Core.Cot.Entities
{
    public class CotAssessmentItemEntity : EntityBase<int>
    {
        public int ItemId { get; set; }
        public int CotAssessmentId { get; set; }

        public DateTime GoalSetDate { get; set; }
        public DateTime GoalMetDate { get; set; }
        public DateTime BoyObsDate { get; set; }
        public DateTime MoyObsDate { get; set; }
        public bool NeedSupport { get; set; }
        public DateTime CotUpdatedOn { get; set; }
        public bool WaitingGoalMet { get; set; }

        [DisplayName("Created By")]
        public int CreatedBy { get; set; }

        [DisplayName("Updated By")]
        public int UpdatedBy { get; set; }

        public virtual CotAssessmentEntity Assessment{ get; set; }
        public virtual ItemBaseEntity Item { get; set; }

    }
}
