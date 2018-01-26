using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		JackZhang
 * Computer:		JACKZ
 * Domain:			Jackz
 * CreatedOn:		2014/10/7 4:21:45
 * Description:		Please input class summary
 * Version History:	Created,2014/10/7 4:21:45
 * 
 * 
 **************************************************************************/
using Sunnet.Framework.Core.Base;
using Sunnet.Cli.Core.Ade;

namespace Sunnet.Cli.Core.Cpalls.Entities
{
    public class CpallsStudentGroupEntity : EntityBase<int>
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        public int ClassId { get; set; }

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

        public virtual ICollection<CustomGroupMyActivityEntity> Activities { get; set; }
    }
}
