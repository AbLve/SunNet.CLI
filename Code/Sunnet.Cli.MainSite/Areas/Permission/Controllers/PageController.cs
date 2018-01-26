using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
/**************************************************************************
 * Developer: 		Joe
 * Computer:		JOE-PC
 * Domain:			Joe-pc
 * CreatedOn:		2014/9/3 12:15:10
 * Description:		Please input class summary
 * Version History:	Created,2014/9/3 12:15:10
 * 
 * 
 **************************************************************************/
using Sunnet.Cli.Business.Permission;
using Sunnet.Cli.UIBase.Models;
using Sunnet.Cli.Core.Permission.Entities;
using Sunnet.Framework.Core.Tool;
using Sunnet.Cli.Business.Permission.Models;
using Sunnet.Cli.MainSite.Controllers;
using System.Linq.Expressions;
using Sunnet.Framework.Helpers;
using Sunnet.Cli.Business.Users;
using Sunnet.Framework.Permission;
using Sunnet.Cli.UIBase.Filters;

namespace Sunnet.Cli.MainSite.Areas.Permission.Controllers
{
    public class PageController : BaseController
    {
        private readonly PermissionBusiness permissionBusiness;

        public PageController()
        {
            permissionBusiness = new PermissionBusiness(UnitWorkContext);
        }


        // GET: /Permission/Page/
        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.Permission_Management, Anonymity = Anonymous.Verified)]
        public ActionResult Index()
        {           
            //所属菜单下拉框
            ViewBag.Menu = new SelectList(permissionBusiness.GetPageDropDownList(), "ID", "Name");
            InitAccessOperation();
            return View();
        }


        // 获取所有页面列表
        //GET:/Permission/Search/
        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.Permission_Management, Anonymity = Anonymous.Verified)]
        public string Search(string name, string type, string url, string isShow, string belongsMenu)
        {
            int firstMenu = string.IsNullOrEmpty(belongsMenu) ? 0 : int.Parse(belongsMenu);//该菜单所包含的子项

            
            var expression = PredicateHelper.True<PageModel>();
            expression = expression.And(a => a.Name.Contains(name) && a.Url.Contains(url));

            if (isShow == "1")
                expression = expression.And(a=>a.IsShow==true);
            if (isShow == "0")
                expression = expression.And(a => a.IsShow == false);

            if (type == "1")
                expression = expression.And(a => a.IsPage == true);
            if (type == "0")
                expression = expression.And(a => a.IsPage == false);

            var list = permissionBusiness.SearchPageList(expression, firstMenu);
            var result = new { total = 0, data = list };
            return JsonHelper.SerializeObject(result);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.Add, PageId = PagesModel.Permission_Management, Anonymity = Anonymous.Verified)]
        public ActionResult New()
        {
            PageEntity page = new PageEntity();
            page.IsShow = true;
            //所属菜单下拉框
            ViewBag.Menu = new SelectList(permissionBusiness.GetPageDropDownList(), "ID", "Name");
            return View(page);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [CLIUrlAuthorizeAttribute(Account = Authority.Add, PageId = PagesModel.Permission_Management, Anonymity = Anonymous.Verified)]
        public string SaveInvitation(PageEntity page, bool isPage, string authorityValues)
        {
            var response = new PostFormResponse();
            if (ModelState.IsValid)
            {
                OperationResult result = new OperationResult(OperationResultType.Success);
                //获取选中的页面权限
                string[] authorities_arr = null;
                if (!string.IsNullOrEmpty(authorityValues))
                {
                    authorities_arr = authorityValues.Split(',');
                }
                result = permissionBusiness.AddPage(page, authorities_arr, isPage);
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


        [CLIUrlAuthorizeAttribute(Account = Authority.Edit, PageId = PagesModel.Permission_Management, Anonymity = Anonymous.Verified)]
        public ActionResult Edit(int id)
        {
            //所属菜单下拉框
            ViewBag.Menu = new SelectList(permissionBusiness.GetPageDropDownList(), "ID", "Name");

            PageModel pageInfo = permissionBusiness.GetPagesAuthorityList(id);

            return View(pageInfo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CLIUrlAuthorizeAttribute(Account = Authority.Edit, PageId = PagesModel.Permission_Management, Anonymity = Anonymous.Verified)]
        public string EditInvitation(PageEntity page, bool isPage, string authorityValues)
        {
            var response = new PostFormResponse();
            if (ModelState.IsValid)
            {
                OperationResult result = new OperationResult(OperationResultType.Success);
                //获取选中的页面权限
                string[] authorities_arr = null;
                if (!string.IsNullOrEmpty(authorityValues))
                {
                    authorities_arr = authorityValues.Split(',');
                }
                result = permissionBusiness.UpdatePage(page, authorities_arr, isPage);
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

        [HttpPost]
        [CLIUrlAuthorizeAttribute(Account = Authority.Delete, PageId = PagesModel.Permission_Management, Anonymity = Anonymous.Verified)]
        public string Delete(int id)
        {
            var response = new PostFormResponse();
            var result = permissionBusiness.DeletePage(id);
            response.Success = result.ResultType == OperationResultType.Success;
            response.Message = result.Message;
            return JsonHelper.SerializeObject(response);
        }

        //测试用添加用户最终权限
        public string AddUserAuthority()
        {
            List<int> RoleId = permissionBusiness.GetRolesList(r=>true).Select(r => r.ID).ToList();

            return "OK，Don't repeat !";
        }


        private void InitAccessOperation()
        {
            bool accessAdd = false;
            bool accessEdit = false;            
            if (UserInfo != null)
            {
                UserAuthorityModel userAuthority = new PermissionBusiness().GetUserAuthority(UserInfo, (int)PagesModel.Permission_Management);

                if (userAuthority != null)
                {
                    if ((userAuthority.Authority & (int)Authority.Add) == (int)Authority.Add)
                    {
                        accessAdd = true;
                    }
                    if ((userAuthority.Authority & (int)Authority.Edit) == (int)Authority.Edit)
                    {
                        accessEdit = true;
                    }                   
                }
            }
            ViewBag.accessAdd = accessAdd;
            ViewBag.accessEdit = accessEdit;           
        }
    }
}