using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		Joe
 * Computer:		Joe-PC
 * Domain:			Joe-pc
 * CreatedOn:		2015/3/16
 * Description:		Create AuthorityEntity
 * Version History:	Created,2015/3/16
 * 
 * 
 **************************************************************************/
using Sunnet.Framework.Core.Base;
using System.ComponentModel.DataAnnotations;
using Sunnet.Framework.Mvc;
using Sunnet.Cli.Core.Common.Enums;

namespace Sunnet.Cli.Core.Vcw.Entities
{
    public class UploadTypeEntity : EntityBase<int>
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; }


        [Required]
        public EntityStatus Status { get; set; }
    }
}
