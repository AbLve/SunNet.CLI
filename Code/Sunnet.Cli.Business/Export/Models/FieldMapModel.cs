using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Cli.Business.Export.Models
{
    public class FieldMapModel
    {
        public int ID { get; set; }

        public string FieldName { get; set; }

        public string DisplayName { get; set; }

        public string AssociateSql { get; set; }

        public string SelectName { get; set; }
    }
}
