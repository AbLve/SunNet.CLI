using StructureMap;
using Sunnet.Cli.Business.Common;
using Sunnet.Cli.Business.Log;
using Sunnet.Cli.Business.Permission;
using Sunnet.Cli.Business.Permission.Models;
using Sunnet.Cli.Business.Users;
using Sunnet.Cli.Core.Common.Enums;
using Sunnet.Cli.Core.Log.Entities;
using Sunnet.Cli.Core.Users.Entities;
using Sunnet.Cli.Core.Users.Enums;
using Sunnet.Cli.MainSite.Controllers;
using Sunnet.Cli.UIBase.Filters;
using Sunnet.Cli.UIBase.Models;
using Sunnet.Framework;
using Sunnet.Framework.Core.Tool;
using Sunnet.Framework.Encrypt;
using Sunnet.Framework.Helpers;
using Sunnet.Framework.Permission;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sunnet.Framework.Extensions;
using Sunnet.Cli.Business.Students;

namespace Sunnet.Cli.MainSite.Areas.Invitation.Controllers
{
    public class ParentController : BaseController
    {
        private readonly UserBusiness userBusiness;
        private readonly PermissionBusiness permissionBusiness;
        private readonly StudentBusiness studentBusiness;
        private readonly OperationLogBusiness operationLogBusiness;

        public ParentController()
        {
            userBusiness = new UserBusiness(UnitWorkContext);
            permissionBusiness = new PermissionBusiness(UnitWorkContext);
            studentBusiness = new StudentBusiness(UnitWorkContext);
            operationLogBusiness = new OperationLogBusiness(UnitWorkContext);
        }
        //
        // GET: /Invitation/Parent/
        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.Parent, Anonymity = Anonymous.Verified)]
        public ActionResult Index()
        {
            InitAccessOperation();
            return View();
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.Parent, Anonymity = Anonymous.Verified)]
        public string Search(string firstName, string lastName, int status = -1,
            string sort = "LastName", string order = "Asc", int first = 0, int count = 10)
        {
            var total = 0;
            var expression = PredicateHelper.True<ParentEntity>();
            if (status > 0)
                expression = expression.And(r => (int)r.UserInfo.Status == status);
            if (firstName.Trim() != string.Empty)
                expression = expression.And(r => r.UserInfo.FirstName.Contains(firstName));
            if (lastName.Trim() != string.Empty)
                expression = expression.And(r => r.UserInfo.LastName.Contains(lastName));

            var list = userBusiness.SearchParents(UserInfo, expression, sort, order, first, count, out total);
            var result = new { total = total, data = list };
            return JsonHelper.SerializeObject(result);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.Add, PageId = PagesModel.Parent, Anonymity = Anonymous.Verified)]
        public ActionResult New()
        {
            UserBaseEntity userEntity = new UserBaseEntity();
            ParentEntity parentEntity = new ParentEntity();
            parentEntity.UserInfo = userEntity;
            return View(parentEntity);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.Edit, PageId = PagesModel.Parent, Anonymity = Anonymous.Verified)]
        public ActionResult Edit(int id)
        {
            ParentEntity parentEntity = userBusiness.GetParent(id);
            List<int> studentIds = userBusiness.GetStudentIDbyParentId(parentEntity.ID);
            ViewBag.StudentList = studentBusiness.GetStudentsGetIds(studentIds);
            if ((int) UserInfo.Role > 100)
            {
                ViewBag.SettingReadOnly = true;
            }
            else
            {
                ViewBag.SettingReadOnly = false;
            }
            return View(parentEntity);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.View, PageId = PagesModel.Parent, Anonymity = Anonymous.Verified)]
        public ActionResult View(int id)
        {
            ParentEntity parentEntity = userBusiness.GetParent(id);
            List<int> studentIds = userBusiness.GetStudentIDbyParentId(parentEntity.ID);
            ViewBag.StudentList = studentBusiness.GetStudentsGetIds(studentIds);
            return View(parentEntity);
        }

        [System.Web.Mvc.HttpPost]
        [ValidateAntiForgeryToken]
        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.Administrative, Anonymity = Anonymous.Logined)]
        public string SaveParent(ParentEntity parent, string childFirstName, string childLastName, List<int> chkSetting, string OtherSetting)
        {
            var response = new PostFormResponse();
            if (parent.ID == 0)
            {
                parent.UserInfo.GoogleId = "";
                parent.UserInfo.Role = Role.Parent;
                parent.SchoolYear = CommonAgent.SchoolYear;
                parent.UserInfo.StatusDate = DateTime.Now;
                parent.UserInfo.Sponsor = UserInfo.ID;
                parent.UserInfo.InvitationEmail = InvitationEmailEnum.NotSend;
                parent.UserInfo.Notes = RegisterType.Invitation.ToDescription();
            }
            if (response.Success = ModelState.IsValid)
            {
                OperationResult result = new OperationResult(OperationResultType.Success);
                if (parent.ID > 0)
                {
                    var user = userBusiness.GetUser(parent.UserInfo.ID);
                    if (user.Status == EntityStatus.Inactive && parent.UserInfo.Status == EntityStatus.Active)
                    {
                        operationLogBusiness.InsertLog(OperationEnum.ResetSendInvitation,
                            parent.UserInfo.Role.ToDescription(),
                            "Status Changed:" + "Inactive to Active" +
                            ",UserId:" + parent.UserInfo.ID,
                            CommonHelper.GetIPAddress(Request), UserInfo);
                    }
                    if (chkSetting != null)
                    {
                        parent.SettingIds = string.Join(",", chkSetting);
                        if (chkSetting.Contains(9))
                            parent.OtherSetting = OtherSetting;
                        else
                        {
                            parent.OtherSetting = "";
                        }
                    }
                  


                    result = userBusiness.UpdateParent(parent);
                }
                response.Success = result.ResultType == OperationResultType.Success;
                response.Message = result.Message;
            }
            response.ModelState = ModelState;
            return JsonHelper.SerializeObject(response);
        }

        private void InitAccessOperation()
        {
            bool accessView = false;
            bool accessAdd = false;
            bool accessEdit = false;
            bool accessClass = false;
            bool accessPermission = false;
            bool accessTransaction = false;
            bool accessEquipment = false;
            if (UserInfo != null)
            {
                UserAuthorityModel userAuthority = new PermissionBusiness().GetUserAuthority(UserInfo, (int)PagesModel.Parent);

                if (userAuthority != null)
                {
                    if ((userAuthority.Authority & (int)Authority.View) == (int)Authority.View)
                    {
                        accessView = true;
                    }
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
                        accessClass = true;
                        accessPermission = true;
                    }
                    if ((userAuthority.Authority & (int)Authority.Transaction) == (int)Authority.Transaction)
                    {
                        accessTransaction = true;
                    }
                    if ((userAuthority.Authority & (int)Authority.AssessmentEquipment) == (int)Authority.AssessmentEquipment)
                    {
                        accessEquipment = true;
                    }
                }
            }
            ViewBag.accessView = accessView;
            ViewBag.accessAdd = accessAdd;
            ViewBag.accessEdit = accessEdit;
            ViewBag.accessClass = accessClass;
            ViewBag.accessPermission = accessPermission;
            ViewBag.accessTransaction = accessTransaction;
            ViewBag.accessEquipment = accessEquipment;
        }
    }
}