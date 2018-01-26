using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sunnet.Cli.Core.Ade.Entities;
using Sunnet.Framework.Core.Base;
using Sunnet.Cli.Core.Students.Entities;
using Sunnet.Cli.Core.Cpalls;
using Sunnet.Cli.Core.Vcw.Entities;

/**************************************************************************
 * Developer: 		JackZhang
 * Computer:		JACKZ
 * Domain:			Jackz
 * CreatedOn:		2014/9/4 4:22:23
 * Description:		Please input class summary
 * Version History:	Created,2014/9/4 4:22:23
 * 
 * 
 **************************************************************************/


namespace Sunnet.Cli.Core.Practices.Entities
{
    public class PracticeStudentAssessmentEntity : EntityBase<int>
    {

        public int StudentId { get; set; }

        public int AssessmentId { get; set; }

        public CpallsStatus Status { get; set; }

        public string SchoolYear { get; set; }
          
        public decimal TotalGoal { get; set; }

        [DisplayName("Created By")]
        public int CreatedBy { get; set; }

        [DisplayName("Updated By")]
        public int UpdatedBy { get; set; }

        public Wave Wave { get; set; }

    

        public virtual ICollection<PracticeStudentMeasureEntity> Measures { get; set; }

        public virtual CliAdeAssessmentEntity Assessment { get; set; }
    }
}
