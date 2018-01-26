using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Cli.Business.Tsds.Models
{
    public class MultiselectGroupModel
    {
        public string label { get; set; }

        public List<MultiselectModel> children { get; set; }


    }
}
