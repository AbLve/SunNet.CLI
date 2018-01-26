using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Sunnet.Framework.Core.Base;
using Sunnet.Cli.Core.Ade.Enums;

/**************************************************************************
 * Developer: 		JackZhang
 * Computer:		JACKZ
 * Domain:			Jackz
 * CreatedOn:		2014/8/11 0:59:45
 * Description:		Please input class summary
 * Version History:	Created,2014/8/11 0:59:45
 * 
 * 
 **************************************************************************/
namespace Sunnet.Cli.Core.Ade.Entities
{
    public class AnswerEntity : EntityBase<int>
    {
        public AnswerEntity()
        {
            Picture = string.Empty;
            Audio = string.Empty;
            Text = string.Empty;
            Value = string.Empty;
            Maps = string.Empty;
            ImageType = ImageType.Selectable;
            SequenceNumber = 0;
            ResponseAudio = string.Empty;
        }
        public int ItemId { get; set; }


        [StringLength(100)]
        public string Picture { get; set; }

        public int PictureTime { get; set; }

        //该字段用于TxkeaReceptiveItem时区分类型，默认为Stimulus
        public ImageType ImageType { get; set; }

        [StringLength(100)]
        public string Audio { get; set; }

        public int AudioTime { get; set; }

        [StringLength(100)]
        public string Text { get; set; }
        public int TextTime { get; set; }

        [StringLength(100)]
        public string Value { get; set; }

        /// <summary>
        /// (可能)用于处理多张图片的定位问题.
        /// </summary>
        [StringLength(1000)]
        public string Maps { get; set; }

        public bool IsCorrect { get; set; }

        public decimal Score { get; set; }

        public int SequenceNumber { get; set; }

        public string ResponseAudio { get; set; }

        [JsonIgnore]
        public virtual ItemBaseEntity Item { get; set; }

    }
}
