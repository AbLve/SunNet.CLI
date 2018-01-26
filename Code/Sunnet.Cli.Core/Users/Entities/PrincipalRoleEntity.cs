using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sunnet.Framework.Core.Base;

namespace Sunnet.Cli.Core.Users.Entities
{
    /*字段权限的Excel表中没有TRS_Specialist(130)和School_Specialist(133)
     这两个角色的权限，数据库中这两个角色权限是从District_Community_Specialist(115)复制而来*/
    public class PrincipalRoleEntity : EntityBase<int>
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public string PrincipalId { get; set; }
        public string SchoolYear { get; set; }
        public string BirthDate { get; set; }
        public string Gender { get; set; }
        public string Address { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string CountyId { get; set; }
        public string StateId { get; set; }
        public string Zip { get; set; }
        public string Ethnicity { get; set; }
        public string PrimaryLanguageId { get; set; }
        public string TotalYearCurrentRole { get; set; }
        public string EducationLevel { get; set; }
        public string PrincipalNotes { get; set; }
    }
}
