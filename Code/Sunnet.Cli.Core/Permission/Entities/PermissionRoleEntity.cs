using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		Joe
 * Computer:		Joe-PC
 * Domain:			Joe-pc
 * CreatedOn:		2014/9/2
 * Description:		Create RoleEntity
 * Version History:	Created,2014/9/2
 * 
 * 
 **************************************************************************/
using Sunnet.Framework.Core.Base;
using System.ComponentModel.DataAnnotations;
using Sunnet.Cli.Core.Users.Entities;
using Sunnet.Cli.Core.Users.Enums;
using System.ComponentModel;
using Sunnet.Cli.Core.Common.Enums;

namespace Sunnet.Cli.Core.Permission.Entities
{
    public class PermissionRoleEntity : EntityBase<int>
    {
        [Required]
        [StringLength(100)]
        [DisplayName("Package Name")]
        public string Name { get; set; }

        [StringLength(200)]
        public string Descriptions { get; set; }

        public EntityStatus Status { get; set; }

        [DisplayName("User Role")]
        public Role UserType { get; set; }

        [Required]
        public bool IsDefault { get; set; }


        //角色和用户 生成一个中间表 Permission_UserRole
        public virtual ICollection<UserBaseEntity> Users { get; set; }

        //一个package可以分配给多个地区或学校
        public virtual ICollection<AssignedPackageEntity> DistrictsAndSchools { get; set; }
    }
}
