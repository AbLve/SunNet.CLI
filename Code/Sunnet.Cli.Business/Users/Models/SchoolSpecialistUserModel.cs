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
using Sunnet.Cli.Core.Users.Entities;

namespace Sunnet.Cli.Business.Users.Models
{
    public class SchoolSpecialistUserModel : UserModel
    {
        public SchoolSpecialistUserModel()
        {
            CommunityId = 0;
            SchoolId = 0;
            PositionId =0;
        }

        [JsonIgnore]
        public IEnumerable<string> SchoolNames { get; set; }

        public int CommunityId { get; set; }
        public int SchoolId { get; set; }
        public int PositionId { get; set; }
        public string PreviousLastName { get; set; }
        public DateTime? BirthDate { get; set; }
        public Gender Gender { get; set; }
        public int PrimaryLanguageId  { get; set; }
        public bool IsSameAddress { get; set; }
        public string Address { get; set; }
        public string PrimaryPhoneNumber { get;set; }
        public PhoneType PrimaryNumberType { get; set; }

        public string SecondPhoneNumber { get; set; }
        public PhoneType SecondNumberType { get; set; }
        public string FaxNumber { get; set; }
        public string PrimaryEmail { get; set; } 

        public string SecondEmail { get; set; }
        public int[] SchoolIds { get; set; }
        public bool IsInvitation { get; set; }

        public string SchoolNameText
        {
            get
            {
                if (SchoolNames != null)
                    return SchoolNames.Aggregate<string, string>(null, (current, next) => (current == null ? "" : current + ", ") + next);
                return "";
            }
        } 
    }
}
