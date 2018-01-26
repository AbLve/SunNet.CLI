using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Cli.Business.Tsds.AssessmentXml
{
    public class ItemXmlModel
    {
        /// <summary>
        /// AIID-Item ID
        /// </summary>
        public string ItemId { get; set; }

        /// <summary>
        /// Item ID
        /// </summary>
        public string IdentificationCode { get; set; }

        /// <summary>
        /// Measure display Name
        /// </summary>
        public string ItemCategory { get; set; }

        public string MaxRawScore { get; set; }

        /// <summary>
        /// Corrent Answer index(s)
        /// </summary>
        public string CorrectResponse { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string ContentStandardName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string LearningStandardIdentificationCode { get; set; }
    }
}
