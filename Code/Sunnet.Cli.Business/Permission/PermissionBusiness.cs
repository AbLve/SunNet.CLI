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
using Sunnet.Cli.Core.Permission;
using Sunnet.Cli.Core;
using Sunnet.Cli.Core.Permission.Entities;
using Sunnet.Cli.Business.Permission.Models;
using Sunnet.Framework.Extensions;
using Sunnet.Cli.Core.Users;
using Sunnet.Cli.Core.Users.Entities;
using Sunnet.Framework.Core.Tool;
using System.Linq.Expressions;
using Sunnet.Cli.Core.Users.Enums;
using Sunnet.Cli.Business.Common;
using Sunnet.Framework.Core.Extensions;
using LinqKit;
using Sunnet.Framework.Helpers;
using Sunnet.Cli.Business.Users;
using Sunnet.Framework.Permission;
using System.Web;
using Sunnet.Framework;
using System.Net;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using System.Web.Caching;
using System.Collections;
using Sunnet.Framework.Log;
using StructureMap;
using Sunnet.Cli.Core.Common.Enums;

namespace Sunnet.Cli.Business.Permission
{
    public class PermissionBusiness
    {
        private readonly IPermissionContract server;
        private readonly ISunnetLog _logger;

        public PermissionBusiness(EFUnitOfWorkContext unit = null)
        {
            server = DomainFacade.CreatePermissionService(unit);
            _logger = ObjectFactory.GetInstance<ISunnetLog>();
        }

        #region Page

        /// <summary>
        /// 返回所有页面数据(用于查询)
        /// </summary>
        /// <returns></returns>
        public IQueryable<PageModel> GetPagesList()
        {
            return server.Pages
                .Select(a => new PageModel
                {
                    ID = a.ID,
                    Name = a.Name,
                    IsPage = a.IsPage,
                    ParentID = a.ParentID,
                    Url = a.Url,
                    Sort = a.Sort,
                    IsShow = a.IsShow,
                    Descriptions = a.Descriptions
                });
        }

        /// <summary>
        /// 根据菜单结构返回页面数据
        /// </summary>
        /// <param name="order"></param>
        /// <param name="first"></param>
        /// <param name="count"></param>
        /// <param name="total"></param>
        /// <returns></returns>

        public List<PageModel> SearchPageList(Expression<Func<PageModel, bool>> condition, int firstMenu)
        {
            List<PageModel> ShowPages = new List<PageModel>();
            IQueryable<PageModel> AllPages = GetPagesList().OrderBy(a => a.Sort);

            //先根据菜单ID进行过滤
            IQueryable<PageModel> FindPages = null;

            if (firstMenu == 0)
                FindPages = AllPages.Where(a => a.ParentID == 0);
            else
                FindPages = AllPages.Where(a => a.ID == firstMenu);

            foreach (PageModel item in FindPages)
            {
                item.Name = "<div>" + item.Name + "</div>";
                ShowPages.Add(item);//添加本身
                ShowPages.AddRange(GetItemsByParent(AllPages, item.ID));//添加子项
            }

            ShowPages = ShowPages.AsQueryable().AsExpandable()
                        .Where(condition).ToList();

            return ShowPages;

        }


        /// <summary>
        /// 根据集合 和 菜单ID查找子项目
        /// </summary>
        /// <param name="allPages"></param>
        /// <param name="parentId"></param>
        /// <returns></returns>
        private List<PageModel> GetItemsByParent(IQueryable<PageModel> allPages, int parentId)
        {
            List<PageModel> FindPages = new List<PageModel>();

            foreach (PageModel item in allPages.Where(a => a.ParentID == parentId)) //查找第二级菜单和页面
            {
                item.Name = "<div style='padding-left:20px'>" + item.Name + "</div>";
                FindPages.Add(item);
                IQueryable<PageModel> SecondMenu = allPages.Where(a => a.ParentID == item.ID);
                foreach (PageModel item_SecondMenu in SecondMenu) //查找第三级页面
                {
                    item_SecondMenu.Name = "<div style='padding-left:40px'>" + item_SecondMenu.Name + "</div>";
                    FindPages.Add(item_SecondMenu);
                }
            }
            return FindPages;
        }

        /// <summary>
        /// 返回所属菜单下拉框
        /// </summary>
        /// <returns></returns>
        public List<SelectItemModel> GetPageDropDownList()
        {
            List<SelectItemModel> Menu = server.Pages.OrderBy(a => a.ID)
                .Where(a => a.IsPage == false && a.IsShow == true)
                .Select(a => new SelectItemModel { ID = a.ID, Name = a.Name })
                .ToList();
            Menu.Insert(0, new SelectItemModel { ID = 0, Name = "No", Selected = true });
            return Menu;
        }

        /// <summary>
        /// 添加页面及页面对应权限
        /// </summary>
        /// <param name="page">页面视图</param>
        /// <param name="Authorities">页面权限数组</param>
        public OperationResult AddPage(PageEntity page, string[] authorities, bool ispage)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);

            if (authorities != null)
            {
                if (authorities.Length > 0)
                {
                    page.Authorities = new List<AuthorityEntity>();
                    for (int i = 0; i < authorities.Length; i++)
                    {
                        page.Authorities.Add(server.GetAuthority(int.Parse(authorities[i])));
                    }
                }
            }
            //是否是页面
            page.IsPage = ispage;
            page.CreatedOn = DateTime.Now;
            page.UpdatedOn = DateTime.Now;
            result = server.AddPage(page);
            return result;
        }

        public OperationResult DeletePage(int id)
        {
            //先删除PageAuthorities表中的数据
            PageEntity entity = server.GetPage(id);
            while (entity.Authorities.Count > 0)
            {
                entity.Authorities.Remove(entity.Authorities.First());
            }

            //再删除page表
            return server.DeletePage(id);
        }

        public PageEntity GetPage(int id)
        {
            return server.GetPage(id);
        }


        /// <summary>
        /// 更新页面及页面对应权限
        /// </summary>
        /// <param name="page"></param>
        /// <param name="Authorities"></param>
        public OperationResult UpdatePage(PageEntity page, string[] authorities, bool ispage)
        {
            //针对一对多关系表操作时   先根据ID获取主表实体，删除子表中的数据  然后保存数据
            PageEntity entity = server.GetPage(page.ID);
            while (entity.Authorities.Count > 0)
            {
                entity.Authorities.Remove(entity.Authorities.First());
            }
            if (authorities != null)
            {
                if (authorities.Length > 0)
                {
                    entity.Authorities = new List<AuthorityEntity>();
                    foreach (string item in authorities)
                    {
                        entity.Authorities.Add(server.GetAuthority(int.Parse(item)));
                    }
                }
            }
            server.UpdatePage(entity);

            //再保存页面传过来的实体
            page.IsPage = ispage;
            page.UpdatedOn = DateTime.Now;
            return server.UpdatePage(page);
        }
        #endregion

        #region Authority
        public OperationResult AddAuthority(AuthorityEntity authority)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            result = server.AddAuthority(authority);
            return result;
        }

        public OperationResult DeleteAuthority(int id)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            result = server.DeleteAuthority(id);
            return result;
        }

        public AuthorityEntity GetAuthority(int id)
        {
            return server.GetAuthority(id);
        }

        public OperationResult UpdateAuthority(AuthorityEntity authority)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            result = server.UpdateAuthority(authority);
            return result;
        }
        #endregion

        #region Role
        public PermissionRoleEntity NewRoleEntity()
        {
            return server.NewRoleEntity();
        }

        /// <summary>
        /// 返回所有角色数据
        /// </summary>
        /// <returns></returns>
        public List<UserRoleModel> GetRolesList(Expression<Func<PermissionRoleEntity, bool>> condition)
        {
            return server.Roles.AsExpandable().Where(condition).Select(r => new UserRoleModel
            {
                ID = r.ID,
                Name = r.Name,
                Descriptions = r.Descriptions,
                UserType = r.UserType,
                IsDefault = r.IsDefault,
                Status = r.Status
            }).ToList();
        }

        /// <summary>
        /// 返回所有角色数据(分页)
        /// </summary>
        /// <returns></returns>
        public List<UserRoleModel> GetRolesList(Expression<Func<PermissionRoleEntity, bool>> condition,
           string sort, string order, int first, int count, out int total)
        {
            var query = server.Roles.AsExpandable().Where(condition);

            total = query.Count();

            return query.Select(r => new UserRoleModel
            {
                ID = r.ID,
                Name = r.Name,
                Descriptions = r.Descriptions,
                UserType = r.UserType,
                IsDefault = r.IsDefault,
                Status = r.Status
            }).OrderBy(sort, order).AsQueryable().Skip(first).Take(count).ToList();
        }


        public OperationResult AddRole(PermissionRoleEntity role)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            if (CheckPackageName(role.Name))
            {
                result.Message = CommonAgent.GetInformation("PackageSamename");
            }
            if (role.IsDefault)
            {
                if (GetDefaultRoleUserType().IndexOf(role.UserType) >= 0)
                {
                    result.Message = CommonAgent.GetInformation("DefautlPackage");
                }
            }

            if (result.ResultType == OperationResultType.Success)
                result = server.AddRole(role);

            return result;
        }

        public OperationResult DeleteRole(int id)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);

            //删除AssignedPackages
            IEnumerable<AssignedPackageEntity> AssignedPackages =
                server.AssignedPackages.Where(a => a.PackageId == id);
            server.DeleteAssignedPackage(AssignedPackages);

            //删除RolePageAuthorities
            IEnumerable<RolePageAuthorityEntity> RolePagesAuthorities =
                server.RolePageAuthorities.Where(a => a.RoleId == id);
            server.DeleteRolePageAuthority(RolePagesAuthorities, true);


            //删除Role
            PermissionRoleEntity entity = server.GetRole(id);
            if (entity.IsDefault)
            {
                result.Message = CommonAgent.GetInformation("DeleteDefaultPackage");
            }
            else
            {
                result = server.DeleteRole(id);
            }
            ClearCache();
            return result;
        }

        public PermissionRoleEntity GetRole(int id)
        {
            return server.GetRole(id);
        }

        public OperationResult UpdateRole(PermissionRoleEntity role)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            if (CheckPackageName(role.Name))
            {
                result.Message = CommonAgent.GetInformation("PackageSamename");
            }
            result = server.UpdateRole(role);
            return result;
        }

        /// <summary>
        /// 获取系统权限用过哪些 UserType
        /// </summary>
        /// <returns></returns>
        public List<Role> GetDefaultRoleUserType()
        {
            return server.Roles.Where(r => r.IsDefault).Select(r => r.UserType).ToList();
        }

        public bool CheckPackageName(string name, int id = 0)
        {
            return server.Roles.Where(r => r.Name == name && r.ID != id).Count() > 0;
        }
        #endregion




        /// <summary>
        /// 根据页面获取权限是否选中（用于编辑页面）
        /// </summary>
        /// <param name="PageId"></param>
        /// <returns></returns>
        public PageModel GetPagesAuthorityList(int pageId)
        {
            List<PageModel> Page = server.Pages.Where(a => a.ID == pageId)
                .Select(r => new PageModel
            {
                ID = r.ID,
                Name = r.Name,
                Descriptions = r.Descriptions,
                IsPage = r.IsPage,
                IsShow = r.IsShow,
                Url = r.Url,
                ParentID = r.ParentID,
                Sort = r.Sort,
                Authorities = r.Authorities

            }).ToList();
            return Page.FirstOrDefault();
        }


        /// <summary>
        /// 根据角色获取页面是否选中列表
        /// </summary>
        /// <param name="RoleId"></param>
        /// <param name="PageId"></param>
        /// <returns></returns>
        public List<PageModel> GetPagesRoleList(int roleId)
        {
            List<PageModel> pagesList = new List<PageModel>();
            List<RolePageAuthorityEntity> list_RolePageAuthority = server.RolePageAuthorities
                .Where(r => r.RoleId == roleId).ToList();
            foreach (PageEntity item in server.Pages.Where(a => a.IsShow == true).OrderBy(a => a.Sort))
            {
                PageModel page = new PageModel();
                page.ID = item.ID;
                page.Name = item.Name;
                page.IsPage = item.IsPage;
                page.ParentID = item.ParentID;
                page.Url = item.Url;
                page.Sort = item.Sort;
                page.IsShow = item.IsShow;
                page.Descriptions = item.Descriptions;
                page.IsSelected = list_RolePageAuthority.Find(a => a.PageId == page.ID) != null ? true : false;
                pagesList.Add(page);
            }

            return pagesList;
        }


        public List<PageModel> GetAllPages()
        {
            return server.Pages.Where(a => a.IsShow == true)
                      .Select(r => new PageModel
                      {
                          ID = r.ID,
                          ParentID = r.ParentID,
                          Authorities = r.Authorities
                      }).ToList();
        }




        /// <summary>
        /// 返回所有页面功能数据
        /// </summary>
        /// <returns></returns>
        public List<AuthorityModel> GetAuthoritiesList()
        {
            var Authorities = server.Authorities
                .Where(a => a.ID != (int)Authority.All)//排除所有选项
                .Select(a => new AuthorityModel { ID = a.ID, Authority = a.Authority, Descriptions = a.Descriptions });
            return Authorities.ToList();
        }



        /// <summary>
        /// 根据 RoleId 查询页面对应的权限并判断是否选中 
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public List<AuthorityWithSelectModel> BindData(int roleId)
        {
            List<PageModel> list_Page = server.Pages.Where(a => a.IsShow == true)
                .Select(r => new PageModel
            {
                ID = r.ID,
                Name = r.Name,
                Descriptions = r.Descriptions,
                IsPage = r.IsPage,
                IsShow = r.IsShow,
                Url = r.Url,
                ParentID = r.ParentID,
                Sort = r.Sort,
                Authorities = r.Authorities
            }).ToList();
            List<RolePageAuthorityEntity> list_RolePageAuthority = server.RolePageAuthorities.Where(r => r.RoleId == roleId).ToList();
            List<AuthorityWithSelectModel> list_AuthoritySelect = new List<AuthorityWithSelectModel>();
            foreach (PageModel page in list_Page)
            {
                foreach (AuthorityEntity action in page.Authorities)
                {
                    AuthorityWithSelectModel model = new AuthorityWithSelectModel();

                    model.ID = action.ID;
                    model.Authority = action.Authority;
                    model.PageId = page.ID;

                    model.IsSelected = list_RolePageAuthority.Find(r => r.PageId == page.ID
                        && CheckIfExist(r.PageAction, ';', action.ID.ToString())) != null ? true : false;

                    list_AuthoritySelect.Add(model);
                }
            }
            return list_AuthoritySelect;
        }


        /// <summary>
        /// 判断数组中是否包含元素
        /// </summary>
        /// <param name="checkString"></param>
        /// <param name="splitChar"></param>
        /// <param name="existString"></param>
        /// <returns></returns>
        private bool CheckIfExist(string checkString, char splitChar, string existString)
        {
            bool ifexists = false;
            string[] str_Check = checkString.Split(splitChar);
            for (int i = 0; i < str_Check.Length; i++)
            {
                if (existString == str_Check[i])
                {
                    ifexists = true;
                    break;
                }
            }
            return ifexists;
        }




        /// <summary>
        /// 返回所有角色页面功能数据
        /// </summary>
        /// <returns></returns>
        public IQueryable<RolePageAuthorityEntity> GetRolePageAuthority()
        {
            return server.RolePageAuthorities;
        }



        /// <summary>
        /// 添加RolePageAuthority表
        /// </summary>
        /// <param name="RoleId">角色ID</param>
        /// <param name="PageAuthorities">页面ID和权限的对应关系列表</param>
        public OperationResult AddRolePageAuthority(int roleId, List<PageAuthorities> pageAuthorityList)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            if (pageAuthorityList.Count > 0)
            {
                List<RolePageAuthorityEntity> RolePageAuthorities = new List<RolePageAuthorityEntity>();
                foreach (PageAuthorities item in pageAuthorityList)
                {
                    RolePageAuthorityEntity RolePageAuthority = new RolePageAuthorityEntity();

                    if (string.IsNullOrEmpty(item.PageAuthority))
                    {
                        PageEntity page = server.GetPage(int.Parse(item.PageId));
                        if (page != null)
                        {
                            if (page.IsPage == false) //菜单配置为all
                                RolePageAuthority.PageAction = (int)Authority.All + ";";
                        }
                    }
                    else
                    {
                        RolePageAuthority.PageAction = item.PageAuthority;
                    }

                    RolePageAuthority.RoleId = roleId;
                    RolePageAuthority.PageId = int.Parse(item.PageId);

                    RolePageAuthority.CreatedOn = DateTime.Now;
                    RolePageAuthority.UpdatedOn = DateTime.Now;
                    RolePageAuthorities.Add(RolePageAuthority);
                }

                result = server.AddRolePageAuthority(RolePageAuthorities);
                ClearCache();
            }
            return result;
        }

        public OperationResult DeleteRolePageAuthority(int id)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            result = server.DeleteRolePageAuthority(id);
            ClearCache();
            return result;
        }

        public RolePageAuthorityEntity GetRolePageAuthority(int id)
        {
            return server.GetRolePageAuthority(id);
        }

        /// <summary>
        /// 更改RolePageAuthority表
        /// </summary>
        /// <param name="RoleId"></param>
        /// <param name="PageAuthorityList"></param>
        public OperationResult UpdateRolePageAuthority(int roleId, List<PageAuthorities> pageAuthorityList)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            //先删除原角色所拥有的权限
            IEnumerable<RolePageAuthorityEntity> DeleteRolePageAuthorities = server.RolePageAuthorities
                .Where(a => a.RoleId == roleId);
            server.DeleteRolePageAuthority(DeleteRolePageAuthorities, false);

            //再添加角色现在拥有的权限
            result = AddRolePageAuthority(roleId, pageAuthorityList);
            ClearCache();
            return result;
        }


        /// <summary>
        /// 批量添加AssignedPackages表
        /// </summary>
        /// <param name="Entities"></param>
        /// <returns></returns>
        public OperationResult AddAssignedPackage(List<AssignedPackageModel> Entities)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            List<AssignedPackageEntity> list_AssignedPackage = new List<AssignedPackageEntity>();
            foreach (AssignedPackageModel item in Entities)
            {
                AssignedPackageEntity entity = new AssignedPackageEntity();
                entity.PackageId = item.PackageId;
                entity.ScopeId = item.ScopeId;
                entity.Type = item.Type;
                entity.CreatedOn = DateTime.Now;
                entity.UpdatedOn = DateTime.Now;
                list_AssignedPackage.Add(entity);
            }
            result = server.AddAssignedPackage(list_AssignedPackage);
            return result;
        }

        /// <summary>
        /// 根据packageid批量删除AssignedPackage
        /// </summary>
        /// <param name="packageId"></param>
        /// <returns></returns>
        public OperationResult DeleteAssignedPackage(int packageId)
        {
            IEnumerable<AssignedPackageEntity> Entities = server.AssignedPackages
                .Where(a => a.PackageId == packageId);
            OperationResult result = new OperationResult(OperationResultType.Success);
            result = server.DeleteAssignedPackage(Entities);
            return result;
        }



        /// <summary>
        /// 根据传入的字符串 去除重复项之后 求和  (并去除禁用的权限)
        /// </summary>
        /// <param name="str"></param>
        /// <param name="split"></param>
        /// <returns></returns>
        private int GetSum(string str, string disableStr)
        {
            int sum = 0;
            string[] strings = str.Split(";".ToArray(), StringSplitOptions.RemoveEmptyEntries);
            string[] disableStrings = disableStr.Split(";".ToArray(), StringSplitOptions.RemoveEmptyEntries); // 禁用的页面权限
            //此处分为三种情况处理
            //1.若默认包和扩展包权限为 Index : Index  或者 Index : Index/Add  ,则直接去除该Page的权限
            //2.若默认包和扩展包权限为 Index/Add : Index  ,则去除Index权限，保留Add权限
            //3.若默认包和扩展包权限为 Index/Add : Index/Edit  ,则保留Index和Add权限
            if (strings.Length == 1 && strings.Contains(Authority.Index.GetValue().ToString())
                && disableStrings.Contains(Authority.Index.GetValue().ToString())) //情况1
                sum = 0;
            else
            {
                foreach (string item in strings.AsQueryable().Distinct())
                {
                    if (!disableStrings.Contains(item))
                    {
                        sum += int.Parse(item);
                    }
                }
                if (strings.Contains(Authority.Index.GetValue().ToString()) && strings.Length > 1
                    && disableStrings.Contains(Authority.Index.GetValue().ToString()) && disableStrings.Length > 1
                    && strings.Any(r => !disableStrings.Contains(r)))   //情况 3
                {
                    sum += Authority.Index.GetValue();
                }
            }
            return sum;
        }

        public List<GroupPackageModel> GetAssignedPackages(int userType, List<int> schoolIds, List<int> communityIds)
        {
            IQueryable<AssignedPackageModel> AssignedPackages = from a in server.Roles
                                                                join b in server.AssignedPackages
                                                                on a.ID equals b.PackageId
                                                                where (int)a.UserType == userType && a.Status == EntityStatus.Active
                                                                select new AssignedPackageModel
                                                                {
                                                                    PackageId = b.PackageId,
                                                                    PackageName = a.Name,
                                                                    PackageDescription = a.Descriptions,
                                                                    ScopeId = b.ScopeId,
                                                                    ScopeName = b.Type == AssignedType.District ? b.Community.Name : b.School.Name,
                                                                    Type = b.Type
                                                                };

            if (schoolIds != null && communityIds == null)
            {
                AssignedPackages = AssignedPackages.Where(a => schoolIds.Contains(a.ScopeId) && a.Type == AssignedType.School);
            }

            if (schoolIds == null && communityIds != null)
            {
                AssignedPackages = AssignedPackages.Where(a => communityIds.Contains(a.ScopeId) && a.Type == AssignedType.District);
            }

            if (schoolIds != null && communityIds != null)
            {
                AssignedPackages =
                    AssignedPackages.Where(a => (schoolIds.Contains(a.ScopeId) && a.Type == AssignedType.School) ||
                                                (communityIds.Contains(a.ScopeId) && a.Type == AssignedType.District)
                        );
            }

            var GroupPackages = from a in AssignedPackages
                                group a by new { a.PackageId, a.PackageName, a.PackageDescription }
                                    into g
                                    select new GroupPackageModel
                                    {
                                        PackageId = g.Key.PackageId,
                                        PackageName = g.Key.PackageName,
                                        PackageDescription = g.Key.PackageDescription
                                    };
            return GroupPackages.ToList();

        }
        /// <summary>
        /// 根据usertype,schoolid,communityid查找所对应的权限列表
        /// </summary>
        /// <param name="userType"></param>
        /// <param name="schoolId"></param>
        /// <param name="communityId"></param>
        /// <returns></returns>
        public List<GroupPackageModel> GetAssignedPackages(int userType, int? schoolId, int? communityId)
        {
            IQueryable<AssignedPackageModel> AssignedPackages = from a in server.Roles
                                                                join b in server.AssignedPackages
                                                                on a.ID equals b.PackageId
                                                                where (int)a.UserType == userType && a.Status == EntityStatus.Active
                                                                select new AssignedPackageModel
                                                                {
                                                                    PackageId = b.PackageId,
                                                                    PackageName = a.Name,
                                                                    PackageDescription = a.Descriptions,
                                                                    ScopeId = b.ScopeId,
                                                                    ScopeName = b.Type == AssignedType.District ? b.Community.Name : b.School.Name,
                                                                    Type = b.Type
                                                                };

            if (schoolId != null && communityId == null)
            {
                AssignedPackages = AssignedPackages.Where(a => a.ScopeId == schoolId && a.Type == AssignedType.School);
            }

            if (schoolId == null && communityId != null)
            {
                AssignedPackages = AssignedPackages.Where(a => a.ScopeId == communityId && a.Type == AssignedType.District);
            }

            if (schoolId != null && communityId != null)
            {
                AssignedPackages = AssignedPackages.Where(
                    a => (a.ScopeId == schoolId && a.Type == AssignedType.School) ||
                        (a.ScopeId == communityId && a.Type == AssignedType.District)
                    );
            }

            var GroupPackages = from a in AssignedPackages
                                group a by new { a.PackageId, a.PackageName, a.PackageDescription }
                                    into g
                                    select new GroupPackageModel
                                    {
                                        PackageId = g.Key.PackageId,
                                        PackageName = g.Key.PackageName,
                                        PackageDescription = g.Key.PackageDescription
                                    };
            return GroupPackages.ToList();

        }





        /// <summary>
        /// 根据用户类别查找默认的role(package)
        /// </summary>
        /// <param name="userType"></param>
        /// <returns></returns>
        public PermissionRoleEntity GetDefaultPackage(Role userType)
        {
            return server.Roles
                .Where(a => a.UserType == userType && a.IsDefault == true)
                .FirstOrDefault();
        }

        /// <summary>
        /// 根据userType查找该类型的所有自定义包
        /// </summary>
        /// <param name="userType"></param>
        /// <returns></returns>
        public List<GroupPackageModel> GetCustomPackages(Role userType)
        {
            return server.Roles
                .Where(a => a.UserType == userType && a.IsDefault == false)
                .Select(a => new GroupPackageModel
                {
                    PackageId = a.ID,
                    PackageName = a.Name,
                    PackageDescription = a.Descriptions
                })
                .ToList();
        }


        /// <summary>
        /// 获取指定用户指定Page的权限
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="pageId"></param>
        /// <returns></returns>
        public UserAuthorityModel GetUserAuthority(UserBaseEntity userInfo, int pageId)
        {
            return GetUserAuthority(userInfo).Find(r => r.UserId == userInfo.ID && r.PageId == pageId);
        }

        public List<UserAuthorityModel> GetUserAuthorities(UserBaseEntity userInfo, List<int> pageIds)
        {
            return GetUserAuthority(userInfo).Where(r => r.UserId == userInfo.ID && pageIds.Contains(r.PageId)).ToList();
        }

        /// <summary>
        /// 默认页面没有权限，选一个有index权限的页面进行跳转
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="pageId"></param>
        /// <returns></returns>
        public UserAuthorityModel GetUserAuthority(UserBaseEntity userInfo, List<int> pageId)
        {
            return GetUserAuthority(userInfo).FindAll(r => r.UserId == userInfo.ID
                && pageId.Contains(r.PageId)
                && (r.Authority & (int)Authority.Index) == (int)Authority.Index)
                .OrderBy(r => r.PageId).FirstOrDefault();
        }

        /// <summary>
        /// 检验是否有菜单权限
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="menuId"></param>
        /// <returns></returns>
        public bool CheckMenu(UserBaseEntity userInfo, int menuId)
        {
            return GetUserAuthority(userInfo).Find(r => r.UserId == userInfo.ID && r.PageId == menuId && r.Authority == (int)Authority.All) != null;
        }

        /// <summary>
        /// 查找用户ID所能控制的Page和Menu列表
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="menuId"></param>
        /// <returns></returns>
        public List<int> CheckPage(UserBaseEntity userInfo)
        {
            return GetUserAuthority(userInfo).FindAll(a => a.UserId == userInfo.ID
                && ((a.Authority & (int)Authority.Index) == (int)Authority.Index
                || (a.Authority & (int)Authority.All) == (int)Authority.All)).Select(r => r.PageId).ToList();
        }
        public List<int> CheckPracticePage(UserBaseEntity userInfo)
        {
            return GetUserAuthority(userInfo).FindAll(a => a.UserId == userInfo.ID
                && ((a.Authority & (int)Authority.AssessmentPracticeArea) == (int)Authority.AssessmentPracticeArea)).Select(r => r.PageId).ToList();
        }

        /// <summary>
        /// 根据模块ID ，获取模块的下的页面 列表
        /// </summary>
        /// <param name="menuId"></param>
        /// <returns></returns>
        public List<PageModel> GetPagesByMenuId(int menuId)
        {
            return server.Pages.Where(r => r.ParentID == menuId)
                .Select(r => new PageModel()
                {
                    ID = r.ID,
                    Url = r.Url
                }).ToList();
        }


        /// <summary>
        /// 生成每个用户的最终权限
        /// </summary>
        /// <param name="userInfo"></param>
        /// <returns></returns>
        List<UserAuthorityModel> GetAuthotirty(UserBaseEntity userInfo)
        {
            List<PageModel> pages = GetPagesList().ToList();
            List<UserAuthorityModel> list = new List<UserAuthorityModel>();

            //找到用户对应的默认系统组
            PermissionRoleEntity defaultRole;
            Role tmpRole = userInfo.Role;
            switch (userInfo.Role)
            {
                case Role.District_Community_Delegate:
                    tmpRole = Role.Community;
                    break;
                case Role.Community_Specialist_Delegate:
                    tmpRole = Role.District_Community_Specialist;
                    break;
                case Role.Principal_Delegate:
                    tmpRole = Role.Principal;
                    break;
                case Role.TRS_Specialist_Delegate:
                    tmpRole = Role.TRS_Specialist;
                    break;
                case Role.School_Specialist_Delegate:
                    tmpRole = Role.School_Specialist;
                    break;
            }

            //找到用户对应的额外权限组
            List<int> roleIds = new List<int>();
            roleIds.AddRange(userInfo.PermissionRoles
              .Where(r => r.IsDefault == false && r.UserType == tmpRole && r.Status == EntityStatus.Active).Select(r => r.ID).ToList());
            //添加默认权限
            defaultRole = server.Roles.Where(r => r.IsDefault == true && r.Status == EntityStatus.Active && r.UserType == tmpRole).FirstOrDefault();
            roleIds.Add(defaultRole.ID);

            //删除禁用的权限组
            IEnumerable<int> disabledRoleIds = userInfo.DisabledUsrRoles.Select(r => r.RoleId);
            List<RolePageAuthorityEntity> disabledAuthorityList = server.RolePageAuthorities
                .Where(r => disabledRoleIds.Contains(r.RoleId)).ToList();

            ///获取所有的权限组对应的权限
            List<RolePageAuthorityEntity> rolePageAuthorityList = server.RolePageAuthorities.Where(r => roleIds.Contains(r.RoleId)).ToList();

            IEnumerable<int> pageids = rolePageAuthorityList.Select(a => a.PageId).Distinct();

            foreach (int pageid in pageids)
            {
                string authorities = "";
                string disabledAuthorities = "";
                foreach (RolePageAuthorityEntity item in rolePageAuthorityList.Where(a => a.PageId == pageid))
                {
                    authorities += item.PageAction;
                }
                foreach (var item in disabledAuthorityList.Where(a => a.PageId == pageid))
                {
                    disabledAuthorities += item.PageAction;
                }

                UserAuthorityModel userAuthority = new UserAuthorityModel();
                userAuthority.UserId = userInfo.ID;
                userAuthority.PageId = pageid;
                userAuthority.Authority = GetSum(authorities, disabledAuthorities);
                if (userAuthority.Authority > 0)
                    list.Add(userAuthority);
            }

            //若页面在最终的权限中，且有父级菜单，则要添加父级菜单权限
            foreach (int pageid in pageids)
            {
                int secondParentId = pages.Find(r => (r.ID == pageid) && (list.Find(x => x.PageId == r.ID) != null)) == null ?
                    0 : pages.Find(r => r.ID == pageid).ParentID; //查找二级菜单
                int firstParentId = 0; //查找一级菜单
                if (secondParentId > 0)
                    firstParentId = pages.Find(r => r.ID == secondParentId) == null ? 0 : pages.Find(r => r.ID == secondParentId).ParentID;

                if (secondParentId > 0 && list.Find(r => r.UserId == userInfo.ID && r.PageId == secondParentId) == null)
                {
                    list.Add(new UserAuthorityModel() { UserId = userInfo.ID, PageId = secondParentId, Authority = Authority.All.GetValue() });
                }
                if (firstParentId > 0 && list.Find(r => r.UserId == userInfo.ID && r.PageId == firstParentId) == null)
                {
                    list.Add(new UserAuthorityModel() { UserId = userInfo.ID, PageId = firstParentId, Authority = Authority.All.GetValue() });
                }
            }

            return list;
        }

        public List<UserAuthorityModel> GetUserAuthority(UserBaseEntity userInfo)
        {
          
            List<UserAuthorityModel> UserAuthorityList = CacheHelper.Get<List<UserAuthorityModel>>("PermissionRoleEntity" + userInfo.ID.ToString()) ;
            //HttpRuntime.Cache["PermissionRoleEntity" + userInfo.ID.ToString()] as List<UserAuthorityModel>;
            if (UserAuthorityList == null)
            {
                //if (File.Exists(SFConfig.CacheFileDependency_Permission) == false)
                //    File.Create(SFConfig.CacheFileDependency_Permission).Close();
                UserAuthorityList = GetAuthotirty(userInfo);
                // HttpRuntime.Cache.Insert("PermissionRoleEntity" + userInfo.ID.ToString(), UserAuthorityList, new CacheDependency(SFConfig.CacheFileDependency_Permission));
                CacheHelper.Add("PermissionRoleEntity" + userInfo.ID.ToString(), UserAuthorityList, CacheHelper.DefaultExpiredSeconds);
            }
            return UserAuthorityList;
        }

        /// <summary>
        /// 清除UserAuthorityModel 缓存
        /// </summary>
        public void ClearCache()
        {
           // CacheHelper.Remove();
            //if (File.Exists(SFConfig.CacheFileDependency_Permission))
            //    File.Delete(SFConfig.CacheFileDependency_Permission);
        }

        private bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            return true; //总是接受  
        }

        /// <summary>
        /// 根据RoleId删除Permission_UserRole表
        /// </summary>
        /// <param name="RoleId"></param>
        /// <returns></returns>
        public int DeleteUserRole(int RoleId)
        {
            return server.DeleteUserRole(RoleId);
        }

        /// <summary>
        /// 根据RoleId删除已移除的Permission_UserRole表
        /// </summary>
        /// <param name="RoleId"></param>
        /// <returns></returns>
        public int DeleteUserRole_Removed(int RoleId, Role role)
        {
            return server.DeleteUserRole_Removed(RoleId, role);
        }


        /// <summary>
        /// 根据PageId查找分配过该权限的用户类型集合
        /// </summary>
        /// <returns></returns>
        public List<Role> GetUserTypeByPageId(int PageId)
        {
            return server.RolePageAuthorities.Where(rp => rp.PageId == PageId)
                .Join(server.Roles, rp => rp.RoleId, r => r.ID, (rp, r) => r.UserType)
                .Distinct().ToList();
        }

        /// <summary>
        /// 新建 Assessment 时，将Assessment作为一个Page写入权限中 
        /// </summary>
        /// <param name="assessmentId"></param>
        /// <param name="name"></param>
        /// <param name="parentId"></param>
        public void AddAssessmentPage(int assessmentId, string label, int type)
        {
            int parentId = 2000;
            switch (type)
            {
                case 1: //cpalls+
                    parentId = 2100;
                    break;
                case 2: //cot
                    parentId = 2300;
                    break;
                case 3: //cec
                    parentId = 2200;
                    break;
                case 4: //Observables
                    parentId = 2400;
                    break;
            }
            server.AddAssessmentPage(assessmentId, label, parentId);
        }

        public void UpdateAssessmentPage(int assessmentId, string label, int type)
        {
            server.UpdateAssessmentPage(assessmentId, label);
        }

        /// <summary>
        /// 删除assessment时，要将 page移除
        /// </summary>
        /// <param name="assessmentId"></param>
        public void DeleteAssessmentPage(int assessmentId)
        {
            server.DeleteAssessmentPage(assessmentId);
        }

        public OperationResult DeleteDisabledUserRole(IEnumerable<DisabledUserRoleEntity> Entities, bool isSave)
        {
            return server.DeleteDisabledUserRole(Entities, isSave);
        }

        //public List<ParentButtonEntity> GetParentButtons()
        //{
        //    return server.ParentButtons.ToList();
        //}

        public List<PageModel> GetChildPagesList(List<int> allPageIds, int menuId)
        {
            List<int> pageIds = server.Pages.Where(e => e.ParentID == menuId).Select(e => e.ID).ToList();
            List<int> authPageIds = allPageIds.Where(e => pageIds.Contains(e)).ToList();
            return server.Pages.Where(e => authPageIds.Contains(e.ID)).Select(r => new PageModel()
            {
                ID = r.ID,
                Url = r.Url,
                Name = r.Name,
                Descriptions = r.Descriptions
            }).ToList();
        }
    }
}
