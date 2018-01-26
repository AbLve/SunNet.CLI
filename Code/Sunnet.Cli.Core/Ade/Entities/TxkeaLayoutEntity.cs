using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		Joe
 * Computer:		JOE-PC
 * Domain:			Joe
 * CreatedOn:		2015/11/8
 * Description:		Add TxkeaReceptive Item
 * Version History:	Created,2015/11/8
 * 
 * 
 **************************************************************************/
using Sunnet.Framework.Core.Base;
using Sunnet.Cli.Core.Ade.Enums;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Sunnet.Framework.Mvc;

namespace Sunnet.Cli.Core.Ade.Entities
{
    public class TxkeaLayoutEntity : EntityBase<int>
    {
        [DisplayName("Layout Name")]
        [StringLength(200)]
        [Required]
        public string Name { get; set; }

        [DisplayName("Number of images")]
        [Required]
        public int NumberOfImages { get; set; }

        [DisplayName("Choose background color fill or image")]
        [StringLength(200)]
        [EensureEmptyIfNull]
        public string BackgroundFill { get; set; }

        public BackgroundFillType BackgroundFillType { get; set; }

        public bool IsDeleted { get; set; }

        [EensureEmptyIfNull]
        public string Layout { get; set; }

        public decimal ScreenWidth { get; set; }

        public decimal ScreenHeight { get; set; }

        [DisplayName("Created By")]
        public int CreatedBy { get; set; }

        [DisplayName("Last Updated By")]
        public int UpdatedBy { get; set; }

        public virtual ICollection<TxkeaReceptiveItemEntity> TxkeaReceptiveItems { get; set; }
        public virtual ICollection<TxkeaExpressiveItemEntity> TxkeaExpressiveItems { get; set; }
    }
}
