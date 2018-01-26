using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Cli.Business.Vcw.Models
{
    public class SchoolCommunityModel
    {
        public int SchoolId { get; set; }

        public string SchoolName { get; set; }

        public int CommunityID { get; set; }

        public string CommunityName { get; set; }
    }


    /// <summary>
    /// 用于CoachTeachers(4.2.3)  、PM-Teachers(4.3.2)  和 PM-Coaches(4.3.3) 页面选中的传值
    /// </summary>
    public class PageSelected
    {
        //用于CoachTeachers(4.2.3)  、PM-Teachers(4.3.2)、Admin-Teachers 时值选项
        //TeacherVIP=1
        //TeacherGeneral=2
        //TeacherAssignment=3
        //TeacherSendAssignment=4


        //用于PM-Coaches(4.3.3)、Admin-Coaches时值选项
        //General=1
        //Assignments=2
        //SendAssignment=3
        public int PageId { get; set; }
    }
}
