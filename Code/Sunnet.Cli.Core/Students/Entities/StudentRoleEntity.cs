using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sunnet.Framework.Core.Base;

namespace Sunnet.Cli.Core.Students.Entities
{
    /*字段权限的Excel表中没有TRS_Specialist(130)和School_Specialist(133)
     这两个角色的权限，数据库中这两个角色权限是从District_Community_Specialist(115)复制而来*/
    public class StudentRoleEntity : EntityBase<int>
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public string StudentId { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Status { get; set; }
        public string StatusDate { get; set; }
        public string SchoolYear { get; set; }
        public string BirthDate { get; set; }
        public string Gender { get; set; }
        public string Ethnicity { get; set; }
        public string PrimaryLanguageId { get; set; }
        public string SecondaryLanguageId { get; set; }
        public string IsSendParent { get; set; }
        public string IsMediaRelease { get; set; }
        public string Notes { get; set; }
    }
}
