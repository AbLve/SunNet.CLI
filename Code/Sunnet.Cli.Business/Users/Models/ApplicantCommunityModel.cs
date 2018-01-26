using Sunnet.Cli.Core.Users.Entities;
using Sunnet.Cli.Core.Users.Enums;
using Sunnet.Framework.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Cli.Business.Users.Models
{
    public class ApplicantCommunityModel
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
        [StringLength(20)]
        [DisplayName("Work Phone")]
        public string WorkPhone { get; set; }

        [EensureEmptyIfNullAttribute]
        [StringLength(20)]
        [DisplayName("Other Phone")]
        public string OtherPhone { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required]
        [DisplayName("Community/District")]
        public int? CommunityId { get; set; }

        [Required]
        [StringLength(200)]
        public string Address { get; set; }

        [EensureEmptyIfNullAttribute]
        [StringLength(200)]
        public string Address2 { get; set; }

        [Required]
        [StringLength(50)]
        public string City { get; set; }

        [Required]
        public int StateId { get; set; }

        [Required]
        [StringLength(5)]
        public string Zip { get; set; }

        public Role RoleType { get; set; }
    }
}
