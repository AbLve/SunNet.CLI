using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Caching;
using System.Web;
using System.Web.Caching;
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
using Sunnet.Framework.Core.Base;
using Sunnet.Cli.Core.Permission.Interfaces;
using Sunnet.Cli.Core.Permission.Entities;
using Sunnet.Framework.Log;
using Sunnet.Framework.File;
using Sunnet.Framework.EmailSender;
using Sunnet.Framework.Encrypt;
using System.Collections;
using Sunnet.Framework.Core.Tool;
using Sunnet.Framework;
using System.Net;
using System.IO;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using Sunnet.Cli.Core.Users.Enums;

namespace Sunnet.Cli.Core.Permission
{
    internal class PermissionService : CoreServiceBase, IPermissionContract
    {
        #region 受保护的属性

        IAuthorityRpst AuthorityRpst;

        IPageRpst PageRpst;

        IPermissionRoleRpst RoleRpst;

        IRolePageAuthorityRpst RolePageAuthorityRpst;

        IAssignedPackageRpst AssignedPackageRpst;

        IDisabledUserRoleRpst DisabledUserRoleRpst;

        #endregion


        public PermissionService(
            ISunnetLog log,
            IFile fileHelper,
            IEmailSender emailSender,
            IEncrypt encrypt,
            IAuthorityRpst authorityRpst,
            IPageRpst pageRpst,
            IPermissionRoleRpst roleRpst,
            IRolePageAuthorityRpst rolePageAuthorityRpst,
            IAssignedPackageRpst assignedPackageRpst,
            IDisabledUserRoleRpst disabledUserRoleRpst,
            IUnitOfWork unit)
            : base(log, fileHelper, emailSender, encrypt)
        {
            AuthorityRpst = authorityRpst;
            PageRpst = pageRpst;
            RoleRpst = roleRpst;
            RolePageAuthorityRpst = rolePageAuthorityRpst;
            AssignedPackageRpst = assignedPackageRpst;
            DisabledUserRoleRpst = disabledUserRoleRpst;
            UnitOfWork = unit;
        }

        #region 公共属性

        public IQueryable<AuthorityEntity> Authorities { get { return AuthorityRpst.Entities; } }

        public IQueryable<PageEntity> Pages { get { return PageRpst.Entities; } }


        public IQueryable<PermissionRoleEntity> Roles
        {
            get
            {
                return RoleRpst.Entities;
            }
        }

        public IQueryable<RolePageAuthorityEntity> RolePageAuthorities { get { return RolePageAuthorityRpst.Entities; } }




        public IQueryable<AssignedPackageEntity> AssignedPackages { get { return AssignedPackageRpst.Entities; } }

        #endregion



        #region Create
        public PermissionRoleEntity NewRoleEntity()
        {
            return RoleRpst.Create();
        }
        #endregion

        #region AddMethods

        public OperationResult AddPage(PageEntity page)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            try
            {
                PageRpst.Insert(page);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult AddRole(PermissionRoleEntity Role)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            try
            {
                RoleRpst.Insert(Role);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult AddAuthority(AuthorityEntity Authority)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            try
            {
                AuthorityRpst.Insert(Authority);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult AddRolePageAuthority(RolePageAuthorityEntity RolePageAuthority)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            try
            {
                RolePageAuthorityRpst.Insert(RolePageAuthority);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult AddRolePageAuthority(IEnumerable<RolePageAuthorityEntity> Entities)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            try
            {
                RolePageAuthorityRpst.Insert(Entities);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }



        public OperationResult AddAssignedPackage(AssignedPackageEntity AssignedPackage)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            try
            {
                AssignedPackageRpst.Insert(AssignedPackage);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }

            return result;
        }

        public OperationResult AddAssignedPackage(IEnumerable<AssignedPackageEntity> Entities)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            try
            {
                AssignedPackageRpst.Insert(Entities);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        #endregion

        #region DeleteMethods

        public OperationResult DeletePage(int id)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            try
            {
                PageRpst.Delete(id);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult DeleteRole(int id)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            try
            {
                RoleRpst.Delete(id);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult DeleteAuthority(int id)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            try
            {
                AuthorityRpst.Delete(id);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult DeleteRolePageAuthority(int id)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            try
            {
                RolePageAuthorityRpst.Delete(id);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult DeleteRolePageAuthority(IEnumerable<RolePageAuthorityEntity> Entities, bool isSave)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            try
            {
                RolePageAuthorityRpst.Delete(Entities, isSave);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }


        public OperationResult DeleteAssignedPackage(int id)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            try
            {
                AssignedPackageRpst.Delete(id);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }

            return result;
        }

        public OperationResult DeleteAssignedPackage(IEnumerable<AssignedPackageEntity> Entities)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            try
            {
                AssignedPackageRpst.Delete(Entities);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult DeleteDisabledUserRole(IEnumerable<DisabledUserRoleEntity> Entities, bool isSave = true)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            try
            {
                DisabledUserRoleRpst.Delete(Entities, isSave);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }
        #endregion

        #region UpdateMethods

        public OperationResult UpdatePage(PageEntity page)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            try
            {
                PageRpst.Update(page);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult UpdateRole(PermissionRoleEntity role)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            try
            {
                RoleRpst.Update(role);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult UpdateAuthority(AuthorityEntity authority)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            try
            {
                AuthorityRpst.Update(authority);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult UpdateRolePageAuthority(RolePageAuthorityEntity rolePageAuthority)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            try
            {
                RolePageAuthorityRpst.Update(rolePageAuthority);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }


        public OperationResult UpdateAssignedPackage(AssignedPackageEntity assignedPackage)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            try
            {
                AssignedPackageRpst.Update(assignedPackage);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }

            return result;
        }

        #endregion

        #region GetMethods

        public PageEntity GetPage(int id)
        {
            return PageRpst.GetByKey(id);
        }

        public PermissionRoleEntity GetRole(int id)
        {
            return RoleRpst.GetByKey(id);
        }

        public AuthorityEntity GetAuthority(int id)
        {
            return AuthorityRpst.GetByKey(id);
        }

        public RolePageAuthorityEntity GetRolePageAuthority(int id)
        {
            return RolePageAuthorityRpst.GetByKey(id);
        }

        public AssignedPackageEntity GetAssignedPackage(int id)
        {
            return AssignedPackageRpst.GetByKey(id);
        }

        #endregion


        public int DeleteUserRole(int RoleId)
        {
            return RoleRpst.DeleteUserRole(RoleId);
        }

        public int DeleteUserRole_Removed(int RoleId, Role role)
        {
            return RoleRpst.DeleteUserRole_Removed(RoleId, role);
        }

        public void AddAssessmentPage(int assessmentId, string Label, int parentId)
        {
            try
            {
                PageRpst.AddAssessmentPage(assessmentId, Label, parentId);
            }
            catch (Exception ex)
            {
                ResultError(ex);
            }
        }

        public void UpdateAssessmentPage(int assessmentId, string Label)
        {
            try
            {
                PageRpst.UpdateAssessmentPage(assessmentId, Label);
            }
            catch (Exception ex)
            {
                ResultError(ex);
            }
        }

        public void DeleteAssessmentPage(int assessmentId)
        {
            try
            {
                PageRpst.DeleteAssessmentPage(assessmentId);
            }
            catch (Exception ex)
            {
                ResultError(ex);
            }
        }


    }
}