using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		Joe
 * Computer:		JOE-PC
 * Domain:			Joe-pc
 * CreatedOn:		2014/9/2 12:15:10
 * Description:		Please input class summary
 * Version History:	Created,2014/9/2 12:15:10
 * 
 * 
 **************************************************************************/
using Sunnet.Framework.Core.Base;
using Sunnet.Cli.Core.Permission.Entities;
using Sunnet.Cli.Core.Permission.Interfaces;
using Sunnet.Cli.Core;
using Sunnet.Cli.Core.Users.Enums;


namespace Sunnet.Cli.Impl.Permission
{
    public class PermissionRoleRpst : EFRepositoryBase<PermissionRoleEntity, Int32>, IPermissionRoleRpst
    {
        public PermissionRoleRpst(IUnitOfWork unit)
        {
            this.UnitOfWork = unit;
        }

        /// <summary>
        /// 根据RoleId删除Permission_UserRole表
        /// </summary>
        /// <param name="Scopids"></param>
        /// <returns></returns>
        public int DeleteUserRole(int RoleId)
        {
            string Sql = "delete Permission_UserRole where RoleId=" + RoleId;
            EFUnitOfWorkContext context = this.UnitOfWork as EFUnitOfWorkContext;
            return context.DbContext.Database.ExecuteSqlCommand(Sql, "");
        }

        /// <summary>
        /// 根据RoleId删除已移除的Permission_UserRole表
        /// </summary>
        /// <param name="RoleId"></param>
        /// <returns></returns>
        public int DeleteUserRole_Removed(int RoleId, Role role)
        {
            StringBuilder sb = new StringBuilder();

            //删除Community的用户权限
            sb.Append(@"delete Permission_UserRole from Permission_UserRole pu ")
                .Append(" where pu.UserId not in ( ")
                .Append(" select u.ID from UserComSchRelations ucs inner join Users u  ")
                .AppendFormat(" on ucs.UserId = u.ID and ucs.CommunityId > 0 and ucs.SchoolId = 0 and u.Role={0} ", (int)role)
                .AppendFormat("  where CommunityId in (select ScopeId from Permission_AssignedPackages where PackageId={0} and [TYPE] =1) )", RoleId)

                //删除School的用户权限
                .Append(" and pu.UserId not in ( ")
                .Append(" select u.ID from UserComSchRelations ucs inner join Users u ")
                .AppendFormat(" on ucs.UserId = u.ID and ucs.CommunityId = 0 and ucs.SchoolId > 0 and u.Role={0} ", (int)role)
                .AppendFormat("  where SchoolId in (select ScopeId from Permission_AssignedPackages where PackageId={0} and [TYPE] =2) )", RoleId)
                 .AppendFormat("  and pu.RoleId={0} ", RoleId);

            EFUnitOfWorkContext context = this.UnitOfWork as EFUnitOfWorkContext;
            return context.DbContext.Database.ExecuteSqlCommand(sb.ToString(), "");
        }
    }


}
