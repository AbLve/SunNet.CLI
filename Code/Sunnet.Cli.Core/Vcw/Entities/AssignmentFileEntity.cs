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
using Sunnet.Framework.Mvc;

namespace Sunnet.Cli.Core.Vcw.Entities
{
    public class AssignmentFileEntity : EntityBase<int>
    {
        [Required]
        public int AssignmentId { get; set; }

        [StringLength(200)]
        [EensureEmptyIfNullAttribute]
        public string FileName { get; set; }

        [StringLength(500)]
        [EensureEmptyIfNullAttribute]
        public string FilePath { get; set; }

        public virtual AssignmentEntity Assignment { get; set; }
    }
}
