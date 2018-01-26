using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Newtonsoft.Json;
using Sunnet.Cli.Core.Common.Enums;
using Sunnet.Cli.Core.Users.Entities;
using System.ComponentModel;
using Sunnet.Cli.Core.Users.Enums;
using System;

namespace Sunnet.Cli.Business.Users.Models
{
    public class DelegateModel
    {
        public int ID { get; set; }

        public int ObjectId { get; set; }

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

        [DisplayName("Delegate Type")]
        public Role RoleType { get; set; }

        [DisplayName("Requested On")]
        public DateTime RequestedOn { get; set; }

        [DisplayName("Google Account")]
        public string GoogleAccount { get; set; }

        [DisplayName("Status")]
        public EntityStatus Status { get; set; }

        public int ParentId { get; set; }

        [JsonIgnore]
        public IEnumerable<string> CommunityNames { get; set; }

        [DisplayName("Community/District")]
        public string CommunityNameText
        {
            get
            {
                if (CommunityNames != null)
                    return CommunityNames.Aggregate<string, string>(null,
                        (current, next) => (current == null ? "" : current + ", ") + next);
                return "";
            }
        }
    }
}
