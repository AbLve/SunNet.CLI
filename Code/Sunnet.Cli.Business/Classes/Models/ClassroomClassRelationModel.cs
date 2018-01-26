using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Cli.Business.Classes.Models
{
    public class ClassroomClassRelationModel
    {
        public int ID { get; set; }
        public int ClassId { get; set; }
        public int ClassroomId { get; set; }
        public string ClassroomName { get; set; }
    }
}
