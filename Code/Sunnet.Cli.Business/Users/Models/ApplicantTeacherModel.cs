using System.ComponentModel.DataAnnotations;
using Sunnet.Cli.Core.Users.Entities;
using System.ComponentModel;
using Sunnet.Cli.Core.Users.Enums;

namespace Sunnet.Cli.Business.Users.Models
{
    public class ApplicantTeacherModel
    {
        public int ApplicantId { get; set; }

        [Required]
        [DisplayName("Title")]
        public UserTitle Title { get; set; }

        [Required]
        [StringLength(140)]
        [DisplayName("First Name")]
        public string FirstName { get; set; }

        [Required]
        [StringLength(140)]
        [DisplayName("Last Name")]
        public string LastName { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        public int? CommunityId { get; set; }

        public int? SchoolId { get; set; }

        public Role RoleType { get; set; }
    }
}
