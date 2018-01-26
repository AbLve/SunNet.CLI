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
using Sunnet.Cli.Core.Permission.Entities;
using Sunnet.Cli.Business.Permission;
using Sunnet.Cli.MainSite.Controllers;
using Sunnet.Cli.Business.Permission.Models;
using System.Web.Script.Serialization;
using Sunnet.Framework.Core.Tool;
using Sunnet.Cli.UIBase.Models;
using Sunnet.Cli.UIBase.Filters;
using Sunnet.Framework.Permission;
using Sunnet.Cli.Business.Vcw.Models;

namespace Sunnet.Cli.MainSite.Areas.Permission.Controllers
{
    public class AssignedPermissionController : BaseController
    {
        PermissionBusiness permissionBusiness;

        public AssignedPermissionController()
        {
            permissionBusiness = new PermissionBusiness(UnitWorkContext);
        }


        [CLIUrlAuthorizeAttribute(Account = Authority.Assign, PageId = PagesModel.Permission_Management, Anonymity = Anonymous.Verified)]
        public ActionResult Assign(int packageId, bool isCustom = false)
        {
            //返回所有页面列表
            ViewBag.Pages = permissionBusiness.GetPagesRoleList(packageId);

            ViewBag.PackageId = packageId;

            ViewBag.PackagePageAuthorityList = permissionBusiness.BindData(packageId);


            //获取每个Menu下的Authority
            Dictionary<int, List<int>> MenuAuthority = new Dictionary<int, List<int>>();

            List<PageModel> list_Page = permissionBusiness.GetAllPages();




            foreach (PageModel item in list_Page.FindAll(p => p.ParentID == 0))//一级菜单
            {
                List<int> MenuAuthorityIds = new List<int>();
                List<PageModel> list_Page2 = list_Page.FindAll(p => p.ParentID == item.ID);
                foreach (PageModel item2 in list_Page2) //二级菜单
                {
                    MenuAuthorityIds.AddRange(item2.Authorities.Select(au => au.ID));

                    List<PageModel> list_Page3 = list_Page.FindAll(p => p.ParentID == item2.ID);
                    foreach (PageModel item3 in list_Page3) //三级菜单
                    {
                        MenuAuthorityIds.AddRange(item3.Authorities.Select(au => au.ID));
                    }
                }
                MenuAuthorityIds.Sort();
                MenuAuthority.Add(item.ID, MenuAuthorityIds.Distinct().ToList());
            }

            ViewBag.MenuAuthority = MenuAuthority;

            PermissionRoleEntity roleEntity = permissionBusiness.GetRole(packageId);

            RoleModel roleModel = new RoleModel
            {
                ID = roleEntity.ID,
                Name = roleEntity.Name,
                Descriptions = roleEntity.Descriptions,
                Status = roleEntity.Status,
                UserType = roleEntity.UserType,
                IsDefault = roleEntity.IsDefault
            };
            ViewBag.isCustom = isCustom;

            return View(roleModel);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">PackageId</param>
        /// <param name="pageAuthorities"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CLIUrlAuthorizeAttribute(Account = Authority.Assign, PageId = PagesModel.Permission_Management, Anonymity = Anonymous.Verified)]
        public string SaveInvitation(int id, string pageAuthorities)
        {
            var response = new PostFormResponse();
            if (ModelState.IsValid)
            {
                OperationResult result = new OperationResult(OperationResultType.Success);
                pageAuthorities = pageAuthorities.Replace(",]", "]");
                JavaScriptSerializer Serializer = new JavaScriptSerializer();
                List<PageAuthorities> PageAuthorityList = Serializer.Deserialize<List<PageAuthorities>>(pageAuthorities);

                //如果从页面传过来的值有权限，但是序列化时出现错误导致没权限时，不执行更新操作
                if (pageAuthorities.Length > 2 && PageAuthorityList.Count == 0)
                {
                    result.ResultType = OperationResultType.NoChanged;
                }
                else
                {
                    result = permissionBusiness.UpdateRolePageAuthority(id, PageAuthorityList);
                }

                response.Success = result.ResultType == OperationResultType.Success;
                response.Message = result.Message;
            }
            else
            {
                response.Success = false;
            }
            response.ModelState = ModelState;
            return JsonHelper.SerializeObject(response);
        }
    }
}