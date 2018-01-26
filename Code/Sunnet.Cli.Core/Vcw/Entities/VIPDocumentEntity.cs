using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		Joe
 * Computer:		Joe-PC
 * Domain:			Joe-pc
 * CreatedOn:		2014/11/3
 * Description:		Create AuthorityEntity
 * Version History:	Created,2014/11/3
 * 
 * 
 **************************************************************************/
using Sunnet.Framework.Core.Base;
using System.ComponentModel.DataAnnotations;
using Sunnet.Framework.Mvc;

namespace Sunnet.Cli.Core.Vcw.Entities
{
    public class VIPDocumentEntity : EntityBase<int>
    {
        [Required]
        public int UserId { get; set; }

        [StringLength(200)]
        [EensureEmptyIfNullAttribute]
        public string FileName { get; set; }

        [StringLength(500)]
        [EensureEmptyIfNullAttribute]
        public string FilePath { get; set; }
    }
}
