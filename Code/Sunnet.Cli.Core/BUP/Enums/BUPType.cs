using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Cli.Core.BUP
{
    public enum BUPType : byte
    {
        Community =1,

        School =2,

        Classroom =3,

        Class = 4,

        Teacher = 5,

        Student = 6,

        CommunityUser = 7,

        CommunitySpecialist =8,

        Principal = 9,

        SchoolSpecialist = 10,

        Parent = 11,

        StatewideAgency  =12,

        Auditor = 13

    }
}
