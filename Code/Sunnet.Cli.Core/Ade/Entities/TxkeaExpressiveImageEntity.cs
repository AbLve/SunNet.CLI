using Sunnet.Framework.Core.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Cli.Core.Ade.Entities
{
    public class TxkeaExpressiveImageEntity : EntityBase<int>
    {
        public int ItemId { get; set; }

        [Description("Response Text")]
        public string TargetImage { get; set; }

        [Description("Mandatory")]
        public int ImageTimeDelay { get; set; }

        [Description("Response Text")]
        public string TargetAudio { get; set; }

        [Description("Mandatory")]
        public int AudioTimeDelay { get; set; }

        /// <summary>
        /// Same as Image Delay
        /// </summary>
        public bool SameasImageDelay { get; set; }

        public bool IsDeleted { get; set; }

        public virtual TxkeaExpressiveItemEntity Item { get; set; }
    }
}