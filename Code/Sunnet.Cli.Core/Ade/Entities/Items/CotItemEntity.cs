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
 * CreatedOn:		2014/8/11 1:10:51
 * Description:		Please input class summary
 * Version History:	Created,2014/8/11 1:10:51
 * 
 * 
 **************************************************************************/
namespace Sunnet.Cli.Core.Ade.Entities
{
    public class CotItemEntity : ItemBaseEntity
    {
        public CotLevel Level { get; set; }

        [StringLength(1000)]
        [Required]
        [DisplayName("Short Target Text")]
        public string ShortTargetText { get; set; }

        [Required]
        [DisplayName("Full Target Text")]
        public string FullTargetText { get; set; }

        [DisplayName("Prekindergarten Guidelines")]
        public string PrekindergartenGuidelines { get; set; }

        [DisplayName("Circle Manual")]
        public string CircleManual { get; set; }

        [DisplayName("Mentoring Guide")]
        public string MentoringGuide { get; set; }

        [Required]
        [DisplayName("Cot Item")]
        [StringLength(10)]
        public string CotItemId { get; set; }

    }
}
