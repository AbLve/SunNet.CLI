using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sunnet.Cli.Core.Common.Enums;

namespace Sunnet.Cli.Business.Students.Models
{
    public class StudentClassModel
    {
        public int StudentId { get; set; }

        public EntityStatus StudentStatus { get; set; }

        public string Name { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public int ClassCount { get; set; }
    }
}
