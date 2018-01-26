using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		Joe
 * Computer:		Joe-PC
 * Domain:			Joe-pc
 * CreatedOn:		2015/3/12
 * Description:		Create AuthorityEntity
 * Version History:	Created,2015/3/12
 * 
 * 
 **************************************************************************/
using Sunnet.Framework.Core.Base;
using System.ComponentModel.DataAnnotations;
using Sunnet.Cli.Core.Vcw.Enums;

namespace Sunnet.Cli.Core.Vcw.Entities
{
    public class AssignmentReportEntity : EntityBase<int>
    {
        [Required]
        public int AssignmentId { get; set; }

        [Required]
        public int ReportId { get; set; }

        [Required]
        public DateTime ReportCreatedOn { get; set; }

        public virtual AssignmentEntity Assignment { get; set; }
    }
}
