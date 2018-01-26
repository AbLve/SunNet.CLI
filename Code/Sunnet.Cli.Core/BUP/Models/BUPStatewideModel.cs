using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sunnet.Cli.Core.Common.Enums;

namespace Sunnet.Cli.Core.BUP.Models
{
    public class BUPStatewideModel
    {
        public int TaskId { get; set; }

        public BUPAction Action { get; set; }

        public string CommunityName { get; set; }

        public string CommunityEngageID { get; set; }

        public string SchoolName { get; set; }

        public string SchoolEngageID { get; set; }

        public string FirstName { get; set; }

        public string MiddleName { get; set; }

        public string LastName { get; set; }

        public string EngageId { get; set; }

        public string InternalID { get; set; }

        public string PhoneNumber { get; set; }

        public byte PhoneType { get; set; }

        public string PrimaryEmail { get; set; }

        public string SecondEmail { get; set; }

        public BUPStatus Status { get; set; }

        public string Remark { get; set; }

        public int LineNum { get; set; }
    }
}
