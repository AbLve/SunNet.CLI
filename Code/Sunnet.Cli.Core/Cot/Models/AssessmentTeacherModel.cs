using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		JackZhang
 * Computer:		JACKZ
 * Domain:			JackZ
 * CreatedOn:		7/2/2015 16:00:07
 * Description:		Please input class summary
 * Version History:	Created,7/2/2015 16:00:07
 *
 *
 **************************************************************************/
using Sunnet.Cli.Core.Common.Enums;
using Sunnet.Cli.Core.Users.Enums;


namespace Sunnet.Cli.Core.Cot.Models
{
    public class AssessmentTeacherModel
    {
        public int ID { get; set; }

        public int UserID { get; set; }

        [DisplayName("Teacher ID")]
        public string TeacherID { get; set; }

        [DisplayName("First Name")]
        public string FirstName { get; set; }

        [DisplayName("Last Name")]
        public string LastName { get; set; }

        public TeacherType TeacherType { get; set; }

        public int CLIFundingId { get; set; }

        public int YearsInProjectId { get; set; }

        [DisplayName("Years in Project")]
        public string YearsInProject { get; set; }

        public EntityStatus Status { get; set; }

        public int? CoachId { get; set; }

        [DisplayName("Coach Name")]
        public string CoachFirstName { get; set; }

        [DisplayName("Coach Name")]
        public string CoachLastName { get; set; }

    }
}
