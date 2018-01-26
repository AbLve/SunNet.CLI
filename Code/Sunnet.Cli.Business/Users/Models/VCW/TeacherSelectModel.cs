using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Cli.Business.Users.Models.VCW
{
    public class TeacherSelectModel
    {
        /// <summary>
        /// UserInfo.ID
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Teacher.ID
        /// </summary>
        public int TeacherId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }
    }
}
