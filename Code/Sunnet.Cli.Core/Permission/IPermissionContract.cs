using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		Joe
 * Computer:		Joe-pc
 * Domain:			Joe-pc
 * CreatedOn:		2014/9/2 8:46:44
 * Description:		Please input class summary
 * Version History:	Created,2014/9/2 8:46:44
 * 
 * 
 **************************************************************************/
using Sunnet.Cli.Core.Permission.Entities;
using Sunnet.Framework.Core.Tool;
using Sunnet.Cli.Core.Users.Enums;

namespace Sunnet.Cli.Core.Permission
{
    public interface IPermissionContract
    {
        #region  属性

        IQueryable<PageEntity> Pages { get; }

        IQueryable<PermissionRoleEntity> Roles { get; }

        IQueryable<AuthorityEntity> Authorities { get; }

        IQueryable<RolePageAuthorityEntity> RolePageAuthorities { get; }


        IQueryable<AssignedPackageEntity> AssignedPackages { get; }

        #endregion


        OperationResult AddPage(PageEntity page);

        OperationResult DeletePage(int id);

        PageEntity GetPage(int id);

        OperationResult UpdatePage(PageEntity page);


        PermissionRoleEntity NewRoleEntity();

        OperationResult AddRole(PermissionRoleEntity role);

        OperationResult DeleteRole(int id);

        PermissionRoleEntity GetRole(int id);

        OperationResult UpdateRole(PermissionRoleEntity role);


        OperationResult AddAuthority(AuthorityEntity authority);

        OperationResult DeleteAuthority(int id);

        AuthorityEntity GetAuthority(int id);

        OperationResult UpdateAuthority(AuthorityEntity authority);


        OperationResult AddRolePageAuthority(RolePageAuthorityEntity rolePageAuthority);

        OperationResult AddRolePageAuthority(IEnumerable<RolePageAuthorityEntity> entities);

        OperationResult DeleteRolePageAuthority(int id);

        OperationResult DeleteRolePageAuthority(IEnumerable<RolePageAuthorityEntity> entities,bool isSave);

        RolePageAuthorityEntity GetRolePageAuthority(int id);

        OperationResult UpdateRolePageAuthority(RolePageAuthorityEntity rolePageAuthority);

        OperationResult DeleteDisabledUserRole(IEnumerable<DisabledUserRoleEntity> Entities, bool isSave);

             
      
        OperationResult AddAssignedPackage(AssignedPackageEntity AssignedPackage);

        OperationResult AddAssignedPackage(IEnumerable<AssignedPackageEntity> Entities);

        OperationResult DeleteAssignedPackage(int id);

        OperationResult DeleteAssignedPackage(IEnumerable<AssignedPackageEntity> Entities);

        OperationResult UpdateAssignedPackage(AssignedPackageEntity assignedPackage);

        AssignedPackageEntity GetAssignedPackage(int id);

        int DeleteUserRole(int RoleId);

        int DeleteUserRole_Removed(int RoleId, Role role);

        void AddAssessmentPage(int assessmentId, string Label, int parentId);

        void UpdateAssessmentPage(int assessmentId, string Label);        

        void DeleteAssessmentPage(int assessmentId);       
    }
}
