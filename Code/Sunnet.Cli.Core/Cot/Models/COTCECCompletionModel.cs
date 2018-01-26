using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Cli.Core.Cot.Models
{
    public  class COTCECCompletionModel
    {
        public int CoachId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public int Total { get; set; }

        public int COTCompletion { get; set; }

        public int CECCompletion { get; set; }
    }
}
