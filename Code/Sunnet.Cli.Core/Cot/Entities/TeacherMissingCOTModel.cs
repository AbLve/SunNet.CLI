using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Cli.Core.Cot.Entities
{
    public class TeacherMissingCOTModel
    {
        public int ID { get; set; }

        public int UserId { get; set; }

        public string Teacher_FirstName { get; set; }

        public string Teacher_LastName { get; set; }

        public string Coach_FirstName { get; set; }

        public string Coach_LastName { get; set; }

        public string CommunityName { get; set; }

        public string SchoolName { get; set; }
    }
}
