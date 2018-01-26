using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Cli.Business.Tsds.Models
{
    public class StudentReferenceModel
    {
        public string ReferenceId { get; set; }

        public string StudentUniqueStateId { get; set; }

        public string FirstName { get; set; }

        public string LastSurname { get; set; }

        public string BirthDate { get; set; }
    }
}
