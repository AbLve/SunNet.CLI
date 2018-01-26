using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/**************************************************************************
 * Developer: 		JackZhang
 * Computer:		JACKZ
 * Domain:			Jackz
 * CreatedOn:		2014/8/11 1:12:19
 * Description:		Please input class summary
 * Version History:	Created,2014/8/11 1:12:19
 * 
 * 
 **************************************************************************/
namespace Sunnet.Cli.Core.Ade.Entities
{
    public class CecItemEntity : ItemBaseEntity
    {
        

        [StringLength(4000)]
        [DisplayName("Target Text")]
        public string TargetText { get; set; }

        public bool IsMultiChoice { get; set; }

        [DisplayName("Response #")]
        public int ResponseCount { get; set; }

        [Range(1, 2, ErrorMessage = "Choose one, please.")]
        public CecItemsDirection Direction { get; set; }

        [DisplayName("Required")]
        public bool IsRequired { get; set; }
    }
}
