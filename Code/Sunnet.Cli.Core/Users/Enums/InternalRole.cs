using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Cli.Core.Users.Enums
{
    public enum InternalRole : byte
    {
        [Description("Super Admin")]
        Super_admin = 1,
//------------------
        [Description("Content Personnel")]
        Content_personnel = 5,

        [Description("Statisticians")]
        Statisticians = 10,

        [Description("Administrative Personnel")]
        Administrative_personnel = 15,

        [Description("Intervention Manager")]
        Intervention_manager = 20,
//--------------------
        [Description("Video Coding Analyst")]
        Video_coding_analyst = 25,

        [Description("Intervention Support Personnel")]
        Intervention_support_personnel = 30,

        [Description("Coordinator")]
        Coordinator = 35,
//---------
        [Description("Mentor/Coach")]
        Mentor_coach = 40
    }
}
