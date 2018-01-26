using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Cli.Business.Ade.Models
{
    public class TxkeaExpressiveImageModel
    {
        public TxkeaExpressiveImageModel()
        {
            TargetImage = string.Empty;
            TargetAudio = string.Empty;
            IsDeleted = false;
        }

        public int ID { get; set; }

        public int ItemId { get; set; }

        public string TargetImage { get; set; }

        public int ImageTimeDelay { get; set; }

        public string TargetAudio { get; set; }

        public int AudioTimeDelay { get; set; }

        /// <summary>
        /// Same as Image Delay
        /// </summary>
        public bool SameasImageDelay { get; set; }

        public bool IsDeleted { get; set; }

    }
}
