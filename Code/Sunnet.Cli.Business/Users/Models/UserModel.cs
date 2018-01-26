using Newtonsoft.Json;
using Sunnet.Cli.Business.Communities.Models;
using Sunnet.Cli.Core.Common.Enums;
using Sunnet.Cli.Core.Users.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Cli.Business.Users.Models
{
    public class UserModel
    {
        /// <summary>
        /// 具体对象ID; 如： TeacherID ,AuditorID 等
        /// </summary>
        public int ID { get; set; }

        public int UserId { get; set; }

        [DisplayName("Google ID")]
        public string GoogleId { get; set; }

        [DisplayName("Teacher Code")]
        public string Code { get; set; }

        [DisplayName("Keyword")]
        public string Keyword { get; set; }

        [DisplayName("First Name")]
        public string FirstName { get; set; }


        [DisplayName("Middle Name")]
        public string MiddleName { get; set; }

        [DisplayName("Last Name")]
        public string LastName { get; set; }

        [DisplayName("Teacher Number")]
        public string TeacherNumber { get; set; }

        [DisplayName("Community/District")]
        public string CommunityName { get; set; }

        [DisplayName("School")]
        public string SchoolName { get; set; }

        [DisplayName("Type")]
        public TeacherType TeacherType { get; set; }

        [DisplayName("Status")]
        public EntityStatus Status { get; set; }

        [DisplayName("User Type")]
        public InternalRole Type { get; set; }

        [DisplayName("Email")]
        public string Email { get; set; }

        [DisplayName("Google Account")]
        public string Gmail { get; set; }

        public AccessType AccessType { get; set; }

        public IEnumerable<int> CommunityIds { get; set; }
        public IEnumerable<int> SchoolIds { get; set; }

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

        public string FirstCommunityName { get; set; }

        public string CommunityNameText
        {
            get
            {
                if (CommunityNames != null)
                    return CommunityNames.Aggregate<string, string>(null, (current, next) => (current == null ? "" : current + ", ") + next);
                return "";
            }
        }
    }
}
