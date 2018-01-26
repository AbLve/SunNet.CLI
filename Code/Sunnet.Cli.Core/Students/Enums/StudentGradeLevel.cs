using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace Sunnet.Cli.Core.Students.Enums
{
    public enum StudentGradeLevel : byte
    {
        [Description("Pre-k")]
        Prek = 1,
        K = 2,

        [Description("1st grade")]
        One_grade = 3,

        [Description("2nd grade")]
        Two_grade = 4
    }
}
