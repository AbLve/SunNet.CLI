using Sunnet.Cli.Core.Classes.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sunnet.Cli.Core.TRSClasses.Enums;

namespace Sunnet.Cli.Business.Classes.Models
{
    public class ClassCountAgeModel
    {
        public ClassCountAgeModel() { }

        public TrsAgeArea AgeArea { get; set; }

        public int Number { get; set; }
    }
}
