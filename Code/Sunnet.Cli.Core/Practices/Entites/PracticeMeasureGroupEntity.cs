using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Sunnet.Framework.Core.Base;

namespace Sunnet.Cli.Core.Practices.Entites
{
    public class PracticeMeasureGroupEntity : EntityBase<int>
    {
        [Required]
        public int MeasureId { get; set; }

        [Required]
        public int AssessmentId { get; set; }

        [Required]
        public int Year { get; set; }

        [Required]
        public int Wave { get; set; }

        [DisplayName("Note")]
        public string Note { get; set; }

        public int CreatedBy { get; set; }

        public int UpdatedBy { get; set; }
    }
}