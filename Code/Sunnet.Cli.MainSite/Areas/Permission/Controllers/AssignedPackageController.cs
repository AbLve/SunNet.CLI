using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
/**************************************************************************
 * Developer: 		Joe
 * Computer:		JOE-PC
 * Domain:			Joe-pc
 * CreatedOn:		2014/9/5 12:15:10
 * Description:		Please input class summary
 * Version History:	Created,2014/9/5 12:15:10
 * 
 * 
 **************************************************************************/
using Sunnet.Cli.MainSite.Controllers;
using Sunnet.Cli.Business.Permission;
using Sunnet.Cli.Business.Permission.Models;
using System.Web.Script.Serialization;
using Sunnet.Framework.Core.Tool;
using Sunnet.Cli.UIBase.Models;
using Sunnet.Cli.Core.Permission.Entities;
using Sunnet.Cli.UIBase.Filters;
using Sunnet.Framework.Permission;
using Sunnet.Cli.Core.Users.Enums;

namespace Sunnet.Cli.MainSite.Areas.Permission.Controllers
{
    public class AssignedPackageController : BaseController
    {
        List<int> DistrictAndSchoolids = new List<int>();  //该权限包已经分配过的District和School的ID集合
        PermissionBusiness permissionBusiness;
        public AssignedPackageController()
        {
            permissionBusiness = new PermissionBusiness(UnitWorkContext);
        }


        [CLIUrlAuthorizeAttribute(Account = Authority.Assign, PageId = PagesModel.Permission_Management, Anonymity = Anonymous.Verified)]
        public ActionResult Assign(int packageId, bool isCustom = false)
        {
            PermissionRoleEntity roleEntity = permissionBusiness.GetRole(packageId);

            RoleModel roleModel = new RoleModel
            {
                ID = roleEntity.ID,
                Name = roleEntity.Name,
                Descriptions = roleEntity.Descriptions,
                Status = roleEntity.Status,
                UserType = roleEntity.UserType,
                IsDefault = roleEntity.IsDefault,
                DistrictsAndSchools = roleEntity.DistrictsAndSchools.Select(r => new AssignedPackageModel
                {
                    ID = r.ID,
                    PackageId = r.PackageId,
                    PackageName = r.Package.Name,
                    PackageDescription = r.Package.Descriptions,
                    ScopeId = r.ScopeId,
                    Type = r.Type,
                    ScopeName = r.Type == AssignedType.District ? r.Community.Name : r.School.Name
                }).OrderBy(a => a.Type).ToList()
            };
            DistrictAndSchoolids = roleModel.DistrictsAndSchools.Select(a => a.ScopeId).ToList();
            ViewBag.isCustom = isCustom;

            return View(roleModel);
        }


        /// <summary>
        /// 添加package分配
        /// </summary>
        /// <param name="packageId"></param>
        /// <param name="assignedPackages"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CLIUrlAuthorizeAttribute(Account = Authority.Assign, PageId = PagesModel.Permission_Management, Anonymity = Anonymous.Verified)]
        public string SaveInvitation(int id, string assignedPackages)
        {
            var response = new PostFormResponse();
            if (ModelState.IsValid)
            {
                OperationResult result = new OperationResult(OperationResultType.Success);

                PermissionRoleEntity role = permissionBusiness.GetRole(id);
                DistrictAndSchoolids = role.DistrictsAndSchools.Select(a => a.ScopeId).ToList();

                assignedPackages = assignedPackages.Replace(",]", "]");
                //将传入的字符串解析成List<AssignedPackageModel>
                JavaScriptSerializer Serializer = new JavaScriptSerializer();
                List<AssignedPackageModel> AssignedPackages =
                    Serializer.Deserialize<List<AssignedPackageModel>>(assignedPackages);
                AssignedPackages.ForEach(a => a.PackageId = id);

                //先执行删除方法
                permissionBusiness.DeleteAssignedPackage(id);

                //再执行添加方法
                result = permissionBusiness.AddAssignedPackage(AssignedPackages);


                //重置Permission_UserRole表

                List<int> NewDistrictAndSchoolids = AssignedPackages.Select(a => a.ScopeId).ToList();//重新要添加的District和School的ID集合

                if (NewDistrictAndSchoolids.Count > 0)
                {
                    foreach (int item in DistrictAndSchoolids)
                    {
                        //如果之前被分配过的District和School的ID 不在 重新要添加的District和School的ID集合里，需要删除user_role表中的数据
                        if (!NewDistrictAndSchoolids.Contains(item))
                        {
                            permissionBusiness.DeleteUserRole_Removed(id, role.UserType);
                        }
                    }
                }
                else //该权限包没有分配给任何community或者school时，可直接删除该包所对应的user_role表中的数据
                {
                    permissionBusiness.DeleteUserRole(id);
                }


                response.Success = result.ResultType == OperationResultType.Success;
                response.Message = result.Message;
            }
            else
            {
                response.Success = false;
            }
            response.ModelState = ModelState;
            permissionBusiness.ClearCache();//清除缓存
            return JsonHelper.SerializeObject(response);
        }
    }
}