using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sunnet.Cli.Core.Common.Enums;
using System.ComponentModel;

namespace Sunnet.Cli.Business.Cec.Model
{
    public class CECSeachModel
    {
        /// <summary>
        /// teacher.id
        /// </summary>
        public int ID { get; set; }

        [DisplayName("Community/District")]
        public string CommunityName { get; set; }

        public int CommunityId { get; set; }

        [DisplayName("School Name")]
        public string SchoolName { get; set; }

        public int SchoolId { get; set; }

        [DisplayName("First Name")]
        public string FirstName { get; set; }

        [DisplayName("Last Name")]
        public string LastName { get; set; }

        public string Code { get; set; }

        public string SchoolYear { get; set; }
    }
}
