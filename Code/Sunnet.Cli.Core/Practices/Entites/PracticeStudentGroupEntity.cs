using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Sunnet.Cli.Core.Ade;
using Sunnet.Cli.Core.Cpalls;
using Sunnet.Framework.Core.Base;

namespace Sunnet.Cli.Core.Practices.Entites
{
    public class PracticeStudentGroupEntity : EntityBase<int>
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; }

      

        /// <summary>
        /// 1,2,3,4 格式
        /// </summary>
        public string StudentIds { get; set; }

        public string SchoolYear { get; set; }

        public Wave Wave { get; set; }

        public AssessmentLanguage Language { get; set; }

        [DisplayName("Created By")]
        public int CreatedBy { get; set; }

        [DisplayName("Updated By")]
        public int UpdatedBy { get; set; }


        [DisplayName("Note")]
        public string Note { get; set; }

        public int AssessmentId { get; set; }

        public virtual ICollection<PracticeGroupMyActivityEntity> Activities { get; set; }
    }
}