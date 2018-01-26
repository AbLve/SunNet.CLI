using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Sunnet.Framework.Core.Base;

namespace Sunnet.Cli.Core.Ade.Entities
{
    public class AssessmentLegendEntity : EntityBase<int>
    {
        [Required]
        public int AssessmentId { get; set; }

        [Required]
        public LegendTypeEnum LegendType { get; set; }

        [Required]
        [DisplayName("Color")]
        public string ColorFilePath { get; set; }

        [Required]
        public string ColorFileName { get; set; }

        [Required(AllowEmptyStrings = true)]
        [DisplayName("Black White")]
        public string BlackWhiteFilePath { get; set; }

        [Required(AllowEmptyStrings = true)]
        public string BlackWhiteFileName { get; set; }

        [Required]
        public string Text { get; set; }

        /// <summary>
        /// Bottom or Top
        /// </summary>
        [Required]
        public string TextPosition { get; set; }

        public virtual AssessmentEntity Assessment { get; set; }
    }
}
