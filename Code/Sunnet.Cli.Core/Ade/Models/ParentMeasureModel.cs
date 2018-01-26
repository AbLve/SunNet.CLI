using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Cli.Core.Ade.Models
{
    public class ParentMeasureModel : ChildMeasureModel
    {
        public IEnumerable<ChildMeasureModel> Measures { get; set; }
    }
}
