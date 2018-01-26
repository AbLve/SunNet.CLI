using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Cli.Core.Cec.Models
{
    public  class CECCompletionModel
    {
        public int CoachId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public int Total { get; set; }

        public int Complete { get; set; }
    }
}
