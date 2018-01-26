using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		Joe
 * Computer:		Joe-PC
 * Domain:			Joe-pc
 * CreatedOn:		2015/3/3
 * Description:		Create AuthorityEntity
 * Version History:	Created,2015/3/3
 * 
 * 
 **************************************************************************/
using Sunnet.Framework.Core.Base;
using System.ComponentModel.DataAnnotations;
using Sunnet.Cli.Core.Vcw.Enums;

namespace Sunnet.Cli.Core.Vcw.Entities
{
    public class FileContentEntity : EntityBase<int>
    {
        [Required]
        public int FileId { get; set; }

        [Required]
        public int ContentId { get; set; }

        public virtual Vcw_FileEntity File { get; set; }
    }
}
