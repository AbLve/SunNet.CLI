using System.ComponentModel.DataAnnotations;
using Sunnet.Cli.Core.Users.Entities;
using System.ComponentModel;
using Sunnet.Cli.Core.Users.Enums;
using System;

namespace Sunnet.Cli.Business.Users.Models
{
    public class UserVerificationModel
    {
        public int ID { get; set; }

        [DisplayName("Keyword")]
        public string Keyword { get; set; }

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

        [DisplayName("Type")]
        public Role RoleType { get; set; }

        [DisplayName("Requested On")]
        public DateTime RequestedOn { get; set; }

        [DisplayName("Status")]
        public ApplicantStatus Status { get; set; }

    }
}
