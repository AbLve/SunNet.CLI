using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		Leason
 * Computer:		Leason-PC
 * Domain:			Leason-pc
 * CreatedOn:		2014/8/22 14:37:20
 * Description:		Create ClassEntity
 * Version History:	Created,2014/8/22 14:37:20
 * 
 * 
 **************************************************************************/
using Sunnet.Cli.Core.Classes.Enums;
using Sunnet.Cli.Core.Classrooms.Entites;
using Sunnet.Cli.Core.Common.Enums;
using Sunnet.Cli.Core.Communities.Entities;
using Sunnet.Cli.Core.MasterData.Entities;
using Sunnet.Cli.Core.Schools.Entities;
using Sunnet.Cli.Core.Students.Entities;
using Sunnet.Cli.Core.TRSClasses.Enums;
using Sunnet.Framework.Core.Base;
using Sunnet.Framework.Mvc;
using Sunnet.Cli.Core.Users.Entities;
using Sunnet.Cli.Core.Ade;

namespace Sunnet.Cli.Core.Practices.Entites
{
    public class DemoStudentEntity : EntityBase<int>
    { 
        public int StudentId { get; set; }
        [DisplayName("Student Name")]
        public string StudentName { get; set; }
        public DateTime StudentDob { get; set; }
        [DisplayName("Student Age Year")]
        public int StudentAgeYear { get; set; }
        [DisplayName("Student Age Month")]
        public int StudentAgeMonth { get; set; }

        [DisplayName("Assessment Language")]
        public StudentAssessmentLanguage AssessmentLanguage { get; set; }
        public string DataFrom { get; set; }
        public string Remark { get; set; }
        public int AssessmentId { get; set; }
        public EntityStatus Status { get; set; }

        [DisplayName("File Name")]
        public string FileName { get; set; }

        [DisplayName("Source")]
        public string Source { get; set; }

    }
}
