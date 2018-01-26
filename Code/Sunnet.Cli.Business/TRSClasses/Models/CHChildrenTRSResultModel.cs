using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Cli.Business.TRSClasses.Models
{
    public class CHChildrenTRSResultModel
    {
        public int ID { get; set; }

        public string TypeofChildren { get; set; }

        public int NumofChildren { get; set; }

        public bool Checked { get; set; }

        public int CaregiversNumber { get; set; }
    }
}
