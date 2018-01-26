using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Cli.Business.BUP.Model
{
    public class NameModel
    {
        public string Name { get; set; }

        public string InternalId { get; set; }

        public string EngageId { get; set; }
    }

    public class NameModelWithSchool
    {
        public string SchoolName { get; set; }

        public string SchoolEngageId { get; set; }

        public string Name { get; set; }

        public string InternalId { get; set; }

        public string EngageId { get; set; }
    }
}
