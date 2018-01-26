using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		Leason
 * Computer:		Leason-PC
 * Domain:			Leason
 * CreatedOn:		2014/12/1 11:20:00
 * Description:		CecHistoryEntity
 * Version History:	Created,2014/12/1 11:20:00
 * 
 * 
 **************************************************************************/
using Newtonsoft.Json;
using Sunnet.Framework.Core.Base;
using Sunnet.Framework.Mvc;
using Sunnet.Cli.Core.Cpalls;

namespace Sunnet.Cli.Core.Cec.Entities
{
    public class CecHistoryEntity:EntityBase<int>
    {
        [StringLength(5)]
        [DisplayName("School Year")]
        public string SchoolYear { get; set; }

        public Wave Wave { get; set; }

        [DisplayName("Assessment Date")]
        public DateTime AssessmentDate { get; set; }

        /// <summary>
        /// Teacher 表的 ID
        /// </summary>
        public int TeacherId { get; set; }

        public int AssessmentId { get; set; }

        [EensureEmptyIfNull]
        [DisplayName("Total Score")]
        public decimal TotalScore { get; set; }

        [EensureEmptyIfNull]
        [DisplayName("Created By")]

        public int CreatedBy { get; set; }
        [EensureEmptyIfNull]
        [DisplayName("Updated By")]
        public int UpdatedBy { get; set; }

        public virtual ICollection<CecResultEntity> CecResults { get; set; } 
    }
}
