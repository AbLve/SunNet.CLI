using Sunnet.Cli.Core.Users.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Cli.Core.BUP.Models
{
   public class BUPCommunityUserModel
    {
        public int TaskId { get; set; }

        public BUPAction Action { get; set; }

        public string CommunityName { get; set; }

        public string CommunityEngageID { get; set; }

        public string SchoolName { get; set; }

        public string SchoolEngageId { get; set; }

        public string FirstName { get; set; }

        public string MiddleName { get; set; }

        public string LastName { get; set; }

        public string EngageId { get; set; }

        public string InternalID { get; set; }

        public string PrimaryPhoneNumber { get; set; }

        public byte PrimaryNumberType { get; set; }

        public string PrimaryEmailAddress { get; set; }

        public string SecondaryEmailAddress { get; set; }

        public BUPStatus Status { get; set; }

        public string Remark { get; set; }

        public Role Role { get; set; }

        public int LineNum { get; set; }
   }
}