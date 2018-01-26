using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Cli.Business.Export.Models
{
    public class FieldListModel
    {
        public int ID { get; set; }

        public string FieldName { get; set; }

        public string DisplayName { get; set; }

        public bool IsDisabled { get; set; }
    }
}
