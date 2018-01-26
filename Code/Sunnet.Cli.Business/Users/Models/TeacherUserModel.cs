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
 * CreatedOn:		2/13 2015 12:20:41
 * Description:		Please input class summary
 * Version History:	Created,2/13 2015 12:20:41
 * 
 * 
 **************************************************************************/
using Newtonsoft.Json;
using Sunnet.Cli.Business.Schools.Models;
using Sunnet.Cli.Core.Ade;
using Sunnet.Cli.Core.Common.Enums;
using Sunnet.Cli.Core.Students.Enums;
using Sunnet.Cli.Core.Users.Entities;
using Sunnet.Cli.Core.Users.Enums;

namespace Sunnet.Cli.Business.Users.Models
{
    public class TeacherUserModel : UserModel
    {
        public TeacherUserModel()
        {
            CommunityId = 0;
            SchoolId = 0;
        }

        public bool IsInvitation { get; set; }
        public int CommunityId { get; set; }
        public int SchoolId { get; set; }
        public string TeacherId { get; set; }
        public string SchoolYear { get; set; }
        public string PreviousLastName { get; set; }
        public DateTime? BirthDate { get; set; }
        public Gender Gender { get; set; }
        public Ethnicity Ethnicity { get; set; }
        public int VendorCode { get; set; }
        public int PrimaryLanguageId { get; set; }
        public int SecondLanguageId { get; set; }
        public EmployedBy EmployedBy { get; set; }
        public int CLIFundingID { get; set; }
        public MediaRelease MediaRelease { get; set; }
        public string TeacherNumber { get; set; }
        public string HomeMailingAddress { get; set; }
        public string PrimaryPhoneNumber { get; set; }
        public PhoneType PrimaryNumberType { get; set; }
        public string SecondPhoneNumber { get; set; }
        public PhoneType SecondNumberType { get; set; }
        public string FaxNumber { get; set; }
        public string PrimaryEmail { get; set; }
        public string SecondEmail { get; set; }
          
    }
}
