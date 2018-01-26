using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Cli.Business.Cpalls.Models
{
    public class ClassModel
    {
        public int ID { get; set; }

        public CpallsSchoolModel School { get; set; }

        public int ClassId { get; set; }
        public int ClassLevel { get; set; }
        public string ClassName { get; set; }
    }    
}
