using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Cli.Core.Students.Model
{
    public   class StudentForCpallsModel
    {
        /// <summary>
        /// Student Id
        /// </summary>
        public int ID { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public DateTime BirthDate { get; set; }

        public int SchoolId { get; set; }

        public string SchoolName { get; set; }

        public int CommunityId { get; set; }
        public string ParentCode { get; set; }
    }
}
