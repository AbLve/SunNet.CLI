using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Sunnet.Cli.MainSite.Controllers;
using Sunnet.Cli.Business.Permission;
using Sunnet.Cli.UIBase.Models;
using Sunnet.Cli.Core.Permission.Entities;
using Sunnet.Framework.Core.Tool;
using Sunnet.Cli.Business.Permission.Models;
using Sunnet.Framework.Helpers;
using Newtonsoft.Json;
using Sunnet.Framework.Extensions;
using Sunnet.Cli.Core.Users.Enums;
using System.Linq.Expressions;
using Sunnet.Framework.Permission;
using Sunnet.Cli.UIBase.Filters;
using Sunnet.Cli.Core.Common.Enums;

namespace Sunnet.Cli.MainSite.Areas.Permission.Controllers
{
    public class PackageController : BaseController
    {
        PermissionBusiness permissionBus;
        public PackageController()
        {
            permissionBus = new PermissionBusiness(UnitWorkContext);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.Permission_Management, Anonymity = Anonymous.Verified)]
        public ActionResult Index()
        {
            ViewBag.RoleType = UserInfo.Role;
            return View();
        }


        // GET: /Permission/Package/Default
        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.Permission_Management, Anonymity = Anonymous.Verified)]
        public ActionResult Default()
        {
            InitAccess();
            return View();
        }

        // GET: /Permission/Package/Custom
        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.Permission_Management, Anonymity = Anonymous.Verified)]
        public ActionResult Custom()
        {
            InitAccess();
            ViewBag.UserTypes = BuilderRoleSelectList();
            return View();
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.Add, PageId = PagesModel.Permission_Management, Anonymity = Anonymous.Verified)]
        public ActionResult New(int isSystem = 1)
        {
            ViewBag.StatusOptions = EntityStatus.Active.ToSelectList(true, new SelectOptions(false, null, ""));
            PermissionRoleEntity entity = permissionBus.NewRoleEntity();
            entity.IsDefault = isSystem == 1;
            if (entity.IsDefault)
                ViewBag.Title = "Add Default Package";
            else
                ViewBag.Title = "Add Custom Package";
            List<SelectListItem> roleSelectList = BuilderRoleSelectList();

            if (entity.IsDefault)
                ViewBag.UserTypeOptions = BuilderRoles(roleSelectList);
            else ViewBag.UserTypeOptions = roleSelectList;
            return View(entity);
        }


        List<SelectListItem> BuilderRoleSelectList()
        {
            List<SelectListItem> roleSelectList = Role.Administrative_personnel
                .ToSelectList(true, new SelectOptions(true, "-1", ViewTextHelper.DefaultPleaseSelectText)).ToList();

            //只显示外部用户
            roleSelectList = roleSelectList.Where(r => int.Parse(r.Value) > (int)Role.Mentor_coach || r.Value == "-1").ToList();

            //代理不进行权限设置
            roleSelectList.Remove(roleSelectList.Find(r => r.Value == ((int)Role.Community_Specialist_Delegate).ToString()));
            roleSelectList.Remove(roleSelectList.Find(r => r.Value == ((int)Role.District_Community_Delegate).ToString()));
            roleSelectList.Remove(roleSelectList.Find(r => r.Value == ((int)Role.Principal_Delegate).ToString()));
            roleSelectList.Remove(roleSelectList.Find(r => r.Value == ((int)Role.TRS_Specialist_Delegate).ToString()));
            roleSelectList.Remove(roleSelectList.Find(r => r.Value == ((int)Role.School_Specialist_Delegate).ToString()));

            return roleSelectList;
        }

        IEnumerable<SelectListItem> BuilderRoles(IEnumerable<SelectListItem> roleSelectList)
        {
            List<SelectListItem> showRoleSelectList = new List<SelectListItem>();
            List<Role> roleList = permissionBus.GetDefaultRoleUserType();
            foreach (SelectListItem item in roleSelectList)
            {
                if (roleList.IndexOf((Role)int.Parse(item.Value)) < 0)
                    showRoleSelectList.Add(item);
            }
            return showRoleSelectList;
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.Edit, PageId = PagesModel.Permission_Management, Anonymity = Anonymous.Verified)]
        public ActionResult Edit(int id)
        {
            ViewBag.StatusOptions = EntityStatus.Active.ToSelectList(true, new SelectOptions(false, null, ""));
            PermissionRoleEntity entity = permissionBus.GetRole(id);
            if (entity.IsDefault)
                ViewBag.Title = "Edit Default Package";
            else
                ViewBag.Title = "Edit Custom Package";
            List<SelectListItem> roleSelectList = Role.Super_admin.ToSelectList().ToList();

            ViewBag.UserTypeOptions = roleSelectList;

            UserRoleModel model = new UserRoleModel()
            {
                ID = entity.ID,
                Name = entity.Name,
                Descriptions = entity.Descriptions,
                Status = entity.Status,
                UserType = entity.UserType,
                IsDefault = entity.IsDefault
            };
            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [CLIUrlAuthorizeAttribute(Account = Authority.Add, PageId = PagesModel.Permission_Management, Anonymity = Anonymous.Verified)]
        public string New(PermissionRoleEntity model)
        {
            var response = new PostFormResponse();
            if (ModelState.IsValid)
            {
                if (model.Descriptions == null)
                    model.Descriptions = string.Empty;
                OperationResult result = permissionBus.AddRole(model);
                response.Success = result.ResultType == OperationResultType.Success;
                response.Data = new UserRoleModel()
                {
                    Name = model.Name,
                    ID = model.ID,
                    Descriptions = model.Descriptions
                    ,
                    Status = model.Status,
                    UserType = model.UserType,
                    IsDefault = model.IsDefault
                };
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
        [ValidateAntiForgeryToken]
        [CLIUrlAuthorizeAttribute(Account = Authority.Edit, PageId = PagesModel.Permission_Management, Anonymity = Anonymous.Verified)]
        public string Edit(UserRoleModel model)
        {
            var response = new PostFormResponse();
            if (ModelState.IsValid)
            {
                PermissionRoleEntity entity = permissionBus.GetRole(model.ID);
                entity.Name = model.Name;
                entity.Descriptions = model.Descriptions ?? string.Empty;
                entity.UpdatedOn = DateTime.Now;
                if (!model.IsDefault)
                {
                    entity.Status = model.Status;
                }
                OperationResult result = permissionBus.UpdateRole(entity);

                if (entity.Status == EntityStatus.Inactive) //如果设置状态为禁用，则删除所有已分配该权限的关系表
                {
                    permissionBus.DeleteUserRole(entity.ID);
                }

                response.Success = result.ResultType == OperationResultType.Success;
                response.Data = new UserRoleModel()
                {
                    Name = model.Name,
                    ID = model.ID,
                    Descriptions = model.Descriptions,
                    Status = entity.Status,
                    UserType = entity.UserType,
                    IsDefault = entity.IsDefault
                };
                response.Message = result.Message;
            }
            else
            {
                response.Success = false;
            }
            response.ModelState = ModelState;
            permissionBus.ClearCache();//清除缓存
            return JsonHelper.SerializeObject(response);
        }

        [HttpPost]
        [CLIUrlAuthorizeAttribute(Account = Authority.Delete, PageId = PagesModel.Permission_Management, Anonymity = Anonymous.Verified)]
        public string Delete(int id)
        {
            var response = new PostFormResponse();
            PermissionRoleEntity role = permissionBus.GetRole(id);
            if (role.IsDefault == false)
            {
                OperationResult result = permissionBus.DeleteRole(id);
                response.Success = result.ResultType == OperationResultType.Success;
                response.Message = result.Message;
            }
            else
                response.Success = false;
            return JsonHelper.SerializeObject(response);
        }

        /// <summary>
        /// 查找Default Package
        /// </summary>
        /// <param name="name"></param>
        /// <param name="usertype"></param>
        /// <param name="status"></param>
        /// <param name="sort"></param>
        /// <param name="order"></param>
        /// <param name="first"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.Permission_Management, Anonymity = Anonymous.Verified)]
        public string SearchDefault(string name, int usertype = -1, int status = -1, string sort = "Name", string order = "Asc",
           int first = 0, int count = 10)
        {
            var total = 0;

            var expression = PredicateHelper.True<PermissionRoleEntity>();

            expression = expression.And(r => r.IsDefault == true);

            if (name.Trim() != string.Empty)
            {
                expression = expression.And(r => r.Name.Contains(name));
            }

            if (usertype > 0)
            {
                expression = expression.And(r => (int)r.UserType == usertype);
            }

            if (status > 0)
            {
                expression = expression.And(r => (int)r.Status == status);
            }

            var list = permissionBus.GetRolesList(expression, sort, order, first, count, out total);
            var result = new { total = total, data = list };
            return JsonHelper.SerializeObject(result);
        }

        /// <summary>
        /// 查找Custom Package
        /// </summary>
        /// <param name="name"></param>
        /// <param name="usertype"></param>
        /// <param name="status"></param>
        /// <param name="sort"></param>
        /// <param name="order"></param>
        /// <param name="first"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.Permission_Management, Anonymity = Anonymous.Verified)]
        public string SearchCustom(string name, int usertype = -1, int status = -1, string sort = "Name", string order = "Asc",
           int first = 0, int count = 10)
        {
            var total = 0;

            var expression = PredicateHelper.True<PermissionRoleEntity>();

            //只显示外部用户,并过滤Delegate的自定义包
            expression = expression.And(r => r.IsDefault == false &&
                r.UserType > Role.Mentor_coach && r.UserType != Role.District_Community_Delegate
                && r.UserType != Role.Community_Specialist_Delegate && r.UserType != Role.Principal_Delegate
                && r.UserType != Role.TRS_Specialist_Delegate && r.UserType != Role.School_Specialist_Delegate);

            if (name.Trim() != string.Empty)
            {
                expression = expression.And(r => r.Name.Contains(name));
            }

            if (usertype > 0)
            {
                expression = expression.And(r => (int)r.UserType == usertype);
            }

            if (status > 0)
            {
                expression = expression.And(r => (int)r.Status == status);
            }

            var list = permissionBus.GetRolesList(expression, sort, order, first, count, out total);
            var result = new { total = total, data = list };
            return JsonHelper.SerializeObject(result);
        }


        private void InitAccess()
        {
            bool accessAdd = false;
            bool accessEdit = false;
            bool accessPermission = false;
            bool accessDelete = false;
            bool accessAssignScope = false;
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
                    if ((userAuthority.Authority & (int)Authority.Assign) == (int)Authority.Assign)
                    {
                        accessPermission = true;
                        accessAssignScope = true;
                    }
                    if ((userAuthority.Authority & (int)Authority.Delete) == (int)Authority.Delete)
                    {
                        accessDelete = true;
                    }
                }
            }
            ViewBag.accessAdd = accessAdd;
            ViewBag.accessEdit = accessEdit;
            ViewBag.accessPermission = accessPermission;
            ViewBag.accessDelete = accessDelete;
            ViewBag.accessAssignScope = accessAssignScope;
        }
    }
}