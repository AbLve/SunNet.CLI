/**************************************************************************
 * Developer: 		JackZhang
 * Computer:		JACKZ
 * Domain:			JackZ
 * CreatedOn:		1/28 2015 15:06:14
 * Description:		Please input class summary
 * Version History:	Created,1/28 2015 15:06:14
 * 
 * 
 **************************************************************************/

using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Newtonsoft.Json;
using Sunnet.Cli.Business.Communities.Models;
using Sunnet.Cli.Business.Schools.Models;
using Sunnet.Cli.Core.Users.Enums;

namespace Sunnet.Cli.Business.Users.Models
{
    public class TeacherModel
    {
        private UserBaseModel _coach;

        public int Id { get; set; }

        [DisplayName("Teacher")]
        public string FirstName { get; set; }

        [DisplayName("Teacher")]
        public string LastName { get; set; }

        public int CoachId { get; set; }

        [DisplayName("Mentor")]
        public UserBaseModel Coach
        {
            get { return _coach ?? (_coach = new UserBaseModel()); }
            set { _coach = value; }
        }

        public int YearsInProjectId { get; set; }

        public AssignmentType CoachingModel { get; set; }

        public AssignmentType ECircle { get; set; }

        public string FullName
        {
            get { return string.Format("{0} {1}", FirstName, LastName); }
        }

        [JsonIgnore]
        public IEnumerable<string> SchoolNames { get; set; }

        public string SchoolNameText
        {
            get
            {
                if (SchoolNames != null)
                    return SchoolNames.Aggregate<string, string>(null, (current, next) => (current == null ? "" : current + ", ") + next);
                return "";
            }
        }

        [JsonIgnore]
        public IEnumerable<string> CommunityNames { get; set; }

        public string CommunityNameText
        {
            get
            {
                if (CommunityNames != null)
                    return CommunityNames.Aggregate<string, string>(null, (current, next) => (current == null ? "" : current + ", ") + next);
                return "";
            }
        }

        [DisplayName("Community/District")]
        public IEnumerable<CommunityModel> Communities { get; set; }

        [DisplayName("School")]
        public IEnumerable<BasicSchoolModel> Schools { get; set; }
    }
}
