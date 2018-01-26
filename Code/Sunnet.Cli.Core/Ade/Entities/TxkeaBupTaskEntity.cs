using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		Joe
 * Computer:		JOE-PC
 * Domain:			Joe
 * CreatedOn:		2015/12/16
 * Description:		
 * Version History:	Created,2015/12/16
 * 
 * 
 **************************************************************************/
using Sunnet.Framework.Core.Base;
using Sunnet.Cli.Core.Ade.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Sunnet.Cli.Core.Ade.Entities
{
    public class TxkeaBupTaskEntity : EntityBase<int>
    {
        public int AssessmentId { get; set; }

        public int MeasureId { get; set; }

        public TxkeaBupType Type { get; set; }

        public TxkeaBupStatus Status { get; set; }

        public string OriginFileName { get; set; }

        public string FilePath { get; set; }

        public bool IsDeleted { get; set; }

        [Required]
        [DisplayName("Resource Path")]
        [StringLength(500)]
        public string ResourcePath { get; set; }

        [StringLength(500)]
        public string Remark { get; set; }

        public int NumberOfItems { get; set; }

        public int CreatedBy { get; set; }

        public int UpdatedBy { get; set; }

        public virtual ICollection<TxkeaBupLogEntity> Logs { get; set; }
    }
}
