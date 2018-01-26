using Sunnet.Cli.Core.Users.Entities;
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
    public class ApplicantSpecialistModel
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

        [Required]
        [DisplayName("Job Title")]
        public int PositionId { get; set; }

        public string PositionOther { get; set; }

        [DisplayName("Associate with")]
        public Role RoleType { get; set; }

        public int? CommunityId { get; set; }

        public int? SchoolId { get; set; }
    }
}
