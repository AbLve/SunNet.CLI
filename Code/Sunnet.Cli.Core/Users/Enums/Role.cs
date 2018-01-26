using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Cli.Core.Users.Enums
{
    public enum Role : byte
    {
        [Description("Super Admin")]
        Super_admin = 1,

        [Description("Content Personnel")]
        Content_personnel = 5,

        [Description("Statisticians")]
        Statisticians = 10,

        [Description("Administrative Personnel")]
        Administrative_personnel = 15,

        [Description("Intervention Manager")]
        Intervention_manager = 20,

        /// <summary>
        /// 有 VideoCodings 扩展表
        /// </summary>
        [Description("Video Coding Analyst")]
        Video_coding_analyst = 25,

        [Description("Intervention Support Personnel")]
        Intervention_support_personnel = 30,

        /// <summary>
        /// 有 CoordCoachs 扩展表
        /// </summary>
        [Description("Coordinator")]
        Coordinator = 35,

        /// <summary>
        /// 有 CoordCoachs 扩展表
        /// </summary>
        [Description("Mentor/Coach")]
        Mentor_coach = 40,

//--------------------------------



        /// <summary>
        /// 有 Auditors 扩展表 没有分配Community
        /// </summary>
        [Description("Auditor")] 
        Auditor = 101,

        /// <summary>
        /// 有 StateWides 扩展表  有分配 Community
        /// </summary>
        [Description("Statewide")]
        Statewide = 105,

        /// <summary>
        /// 有 CommunityUsers 扩展表
        /// </summary>
        [Description("Community/District User")]
        Community = 110,

        /// <summary>
        /// 有 CommunityUsers 扩展表
        /// </summary>
        [Description("Community/District Delegate")]
        District_Community_Delegate = 120,

        /// <summary>
        /// 有 CommunityUsers 扩展表
        /// </summary>
        [Description("Community/District Specialist")]
        District_Community_Specialist = 115,

        /// <summary>
        /// 有 CommunityUsers 扩展表 
        /// </summary>
        [Description("Community/District Specialist Delegate")]
        Community_Specialist_Delegate = 140,


        /// <summary>
        /// 有 Principals 扩展表
        /// </summary>
        [Description("TRS Specialist")]
        TRS_Specialist = 130,

        /// <summary>
        /// 有 Principals 扩展表
        /// </summary>
        [Description("School Specialist")]
        School_Specialist = 133,

        /// <summary>
        /// 有 Principals 扩展表
        /// </summary>
        [Description("TRS Specialist Delegate")]
        TRS_Specialist_Delegate = 142,

        /// <summary>
        /// 有 Principals 扩展表
        /// </summary>
        [Description("School Specialist Delegate")]
        School_Specialist_Delegate = 143,

        /// <summary>
        /// 有 Principals 扩展表
        /// </summary>
        [Description("Principal/Director")]
        Principal = 125,

        /// <summary>
        /// 有 Principals 扩展表
        /// </summary>
        [Description("Principal/Director Delegate")]
        Principal_Delegate = 135,

        /// <summary>
        /// 有 Teachers 扩展表
        /// </summary>
        [Description("Teacher")]
        Teacher = 145,

        /// <summary>
        /// 有 Parents 扩展表
        /// </summary>
        [Description("Parent")]
        Parent = 150
    }
}
