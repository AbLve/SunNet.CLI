using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sunnet.Cli.Core.Users.Enums;

namespace Sunnet.Cli.Business.Trs.Models
{
    public class NotificationUserModel
    {
        public int UserId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public Role Role { get; set; }

        public bool Selected { get; set; }
    }
}
