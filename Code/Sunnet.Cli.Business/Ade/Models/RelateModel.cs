using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Cli.Business.Ade.Models
{
    public class RelateModel
    {
        public int ID { get; set; }
        public int ParentId { get; set; }
        public int ParentRelationId { get; set; }
        public string Name { get; set; }
    }
}
