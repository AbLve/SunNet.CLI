using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		JackZhang
 * Computer:		JACKZ
 * Domain:			Jackz
 * CreatedOn:		2014/12/16 9:07:21
 * Description:		Please input class summary
 * Version History:	Created,2014/12/16 9:07:21
 * 
 * 
 **************************************************************************/
using Sunnet.Framework.Core.Base;

namespace Sunnet.Cli.Core.Cot.Entities
{
    public class CotWaveEntity:EntityBase<int>
    {
        public int CotAssessmentId { get; set; }
        public CotWave Wave { get; set; }

        [DisplayName("Date of Visit")]
        public DateTime VisitDate { get; set; }
        [Required]
        [DisplayName("Length of Visit (hours)")]
        public string SpentTime { get; set; }
        public CotWaveStatus Status { get; set; }

        [DisplayName("Created By")]
        public int CreatedBy { get; set; }

        [DisplayName("Updated By")]
        public int UpdatedBy { get; set; }
        public DateTime FinalizedOn { get; set; }
        public virtual CotAssessmentEntity Assessment { get; set; }
    }
}
