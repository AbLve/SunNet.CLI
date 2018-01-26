using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Cli.Core.Users.Enums
{
    public enum EmployedBy : byte
    {
        [Description("Public School (ISD)")]
        Public_School = 1,

        [Description("Head Start (HS)")]
        Head_Start = 2,

        [Description("Child Care (CC)")]
        Child_Care = 3,

        [Description("Non Applicable")]
        NonApplicable = 4
    }
}
