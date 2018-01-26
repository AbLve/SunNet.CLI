using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Cli.Business.Cpalls.Group
{
    public class MeasureGroupModel
    {
        private decimal _goal;
        public int StudentId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public decimal Goal
        {
            get
            {
                if (_goal < 0)
                    return 0;
                return _goal;
            }
            set { _goal = value; }
        }

        public int MeasureId { get; set; }
    }
}
