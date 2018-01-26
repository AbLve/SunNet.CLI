using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		Joe
 * Computer:		Joe-PC
 * Domain:			Joe-pc
 * CreatedOn:		2014/9/5
 * Description:		Create CountyEntity
 * Version History:	Created,2014/9/5
 * 
 * 
 **************************************************************************/
using Sunnet.Framework.Core.Base;
using Sunnet.Cli.Core.Communities.Entities;
using Sunnet.Cli.Core.Schools.Entities;

namespace Sunnet.Cli.Core.Permission.Entities
{
    public class AssignedPackageEntity : EntityBase<int>
    {
        public int PackageId { get; set; }

        public int ScopeId { get; set; }

        public AssignedType Type { get; set; }

        //一个角色可以分配给多个地区或学校
        public virtual PermissionRoleEntity Package { get; set; }

        public virtual CommunityEntity Community { get; set; }

        public virtual SchoolEntity School { get; set; }
    }


    //用户类别枚举
    public enum AssignedType
    {
        District = 1,
        School = 2
    }
}
