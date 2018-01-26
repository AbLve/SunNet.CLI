using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace Sunnet.Cli.MainSite.Models
{
    public class ContactUsModel
    {
        public string Name { get; set; }

        [DisplayName("School District")]
        public string District { get; set; }

        [DisplayName("Email Address")]
        public string Email { get; set; }

        [DisplayName("Phone Number")]
        public string Phone { get; set; }
        public string Subject { get; set; }

        [DisplayName("How Can We Help?")]
        public string Content { get; set; }

    }
}