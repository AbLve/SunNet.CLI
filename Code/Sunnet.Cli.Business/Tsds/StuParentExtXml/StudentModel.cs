using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Cli.Business.Tsds.StuParentExtXml
{
    public class StudentModel
    {
        public string Id { get; set; }

        public string StudentUniqueStateId { get; set; }

        public string StudentIdentificationCode { get; set; }

        public string Sex { get; set; }

        public string FirstName { get; set; }

        public string LastSurname { get; set; }

        public string BirthDate { get; set; } //Format: YYYY-mm-dd
        
    }
}
