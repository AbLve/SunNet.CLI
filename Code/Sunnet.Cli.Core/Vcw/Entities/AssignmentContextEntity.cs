using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		Lee
 * Computer:		Lee-PC
 * Domain:			Lee-pc
 * CreatedOn:		2014/10/21
 * Description:		Create AuthorityEntity
 * Version History:	Created,2014/10/21
 * 
 * 
 **************************************************************************/
using Sunnet.Framework.Core.Base;
using System.ComponentModel.DataAnnotations;
using Sunnet.Cli.Core.Vcw.Enums;

namespace Sunnet.Cli.Core.Vcw.Entities
{
    public class AssignmentContextEntity : EntityBase<int>
    {
        [Required]
        public int AssignmentId { get; set; }

        [Required]
        public int ContextId { get; set; }

        public virtual AssignmentEntity Assignment { get; set; }
    }
}
