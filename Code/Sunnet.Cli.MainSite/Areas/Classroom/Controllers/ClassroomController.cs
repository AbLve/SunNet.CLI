using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web;
using System.Web.Mvc;
/**************************************************************************
 * Developer: 		Leason
 * Computer:		Leason-PC
 * Domain:			Leason-pc
 * CreatedOn:		2014/8/23 11:20:20
 * Description:		Create ClassroomController
 * Version History:	Created,2014/8/23 11:20:20
 * 
 * 
 **************************************************************************/
using Newtonsoft.Json;
using Sunnet.Cli.Business.Classes;
using Sunnet.Cli.Business.Classrooms;
using Sunnet.Cli.Business.Classrooms.Models;
using Sunnet.Cli.Business.Communities;
using Sunnet.Cli.Business.MasterData;
using Sunnet.Cli.Business.Schools;
using Sunnet.Cli.Business.Users;
using Sunnet.Cli.Core.Classrooms.Entites;
using Sunnet.Cli.Core.Classrooms.Enums;
using Sunnet.Cli.Core.Common.Enums;
using Sunnet.Cli.Core.MasterData.Entities;
using Sunnet.Cli.Core.Users.Entities;
using Sunnet.Cli.Core.Users.Enums;
using Sunnet.Cli.MainSite.Controllers;
using Sunnet.Cli.UIBase.Models;
using Sunnet.Framework.Core.Base;
using Sunnet.Framework.Core.Tool;
using Sunnet.Framework.Extensions;
using Sunnet.Framework.Helpers;
using Sunnet.Cli.Core.Permission.Entities;
using Sunnet.Cli.Business.Permission;
using Sunnet.Framework.Permission;
using Sunnet.Cli.UIBase.Filters;
using Sunnet.Cli.Business.Permission.Models;
using Sunnet.Cli.Core.Classes.Entites;


namespace Sunnet.Cli.MainSite.Areas.Classroom.Controllers
{
    public class ClassroomController : BaseController
    {
        private readonly ClassroomBusiness _classroomBusiness;
        private readonly MasterDataBusiness _masterBusiness;
        private readonly CommunityBusiness communityBusiness;
        private readonly ClassBusiness _classBusiness;
        private UserBusiness userBusiness;

        public ClassroomController()
        {
            _classroomBusiness = new ClassroomBusiness(UnitWorkContext);
            _masterBusiness = new MasterDataBusiness(UnitWorkContext);
            communityBusiness = new CommunityBusiness(UnitWorkContext);
            _classBusiness = new ClassBusiness(UnitWorkContext);
            userBusiness = new UserBusiness(UnitWorkContext);
        }

        #region Index New Edit View
        //
        // GET: /Classroom/Classroom/
        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.Classroom_Management, Anonymity = Anonymous.Verified)]
        public ActionResult Index()
        {
            InitAccessOperation();
            ViewBag.StatusOptions = EntityStatus.Active.ToSelectList(true, new SelectOptions(true, "-1", ViewTextHelper.DefaultAllText));

            return View();
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.Add, PageId = PagesModel.Classroom_Management, Anonymity = Anonymous.Verified)]
        public ActionResult New()
        {
            InitControls();
            ClassroomRoleEntity roleEntity = _classroomBusiness.GetClassroomRoleEntity(GetRole());
            if (roleEntity == null)
                return RedirectToAction("index");
            ViewBag.Role = JsonHelper.SerializeObject(roleEntity);
            ClassroomEntity classroom = _classroomBusiness.NewClassroomEntity();
            return View(classroom);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.Edit, PageId = PagesModel.Classroom_Management, Anonymity = Anonymous.Verified)]
        public ActionResult Edit(int id)
        {
            InitControls();

            ClassroomRoleEntity roleEntity = _classroomBusiness.GetClassroomRoleEntity(GetRole());
            if (roleEntity == null)
                return RedirectToAction("index");
            ViewBag.Role = JsonHelper.SerializeObject(roleEntity);
            ClassroomModel classroom = _classroomBusiness.GetClassroom(id, UserInfo);
            return View(classroom);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.View, PageId = PagesModel.Classroom_Management, Anonymity = Anonymous.Verified)]
        public ActionResult View(int id)
        {
            //   InitControls();
            ClassroomRoleEntity roleEntity = _classroomBusiness.GetClassroomRoleEntity(GetRole());
            if (roleEntity == null)
                return RedirectToAction("index");
            ViewBag.Role = JsonHelper.SerializeObject(roleEntity);

            ClassroomModel classroom = _classroomBusiness.GetClassroom(id, UserInfo);

            //InterventionStatus
            if (classroom.InterventionStatus == (InterventionStatus)100 || (classroom.InterventionStatus == (InterventionStatus)0))
                ViewBag.InterventionStatus = "";
            else if (classroom.InterventionStatus == InterventionStatus.Other)
                ViewBag.InterventionStatus = classroom.InterventionOther;
            else
                ViewBag.InterventionStatus = classroom.InterventionStatus.ToDescription();

            //Funding
            IEnumerable<SelectItemModel> fundingList = _masterBusiness.GetFundingSelectList();
            {
                ViewBag.FundingName = fundingList.Where(o => o.ID == classroom.FundingId).Select(o => o.Name).FirstOrDefault();

                ViewBag.FCfundingName = fundingList.Where(o => o.ID == classroom.FcFundingId).Select(o => o.Name).FirstOrDefault();

                ViewBag.part1FundingName = fundingList.Where(o => o.ID == classroom.Part1FundingId).Select(o => o.Name).FirstOrDefault();

                ViewBag.part2FundingName = fundingList.Where(o => o.ID == classroom.Part2FundingId).Select(o => o.Name).FirstOrDefault();

                ViewBag.StartupFundingName = fundingList.Where(o => o.ID == classroom.StartupKitFundingId).Select(o => o.Name).FirstOrDefault();

                ViewBag.CurriculumFundingName = fundingList.Where(o => o.ID == classroom.CurriculumFundingId).Select(o => o.Name).FirstOrDefault();

                ViewBag.TalkerFundingName = fundingList.Where(o => o.ID == classroom.DevelopingTalkerKitFundingId).Select(o => o.Name).FirstOrDefault();

                ViewBag.FCCFundingName = fundingList.Where(o => o.ID == classroom.FccKitFundingId).Select(o => o.Name).FirstOrDefault();

            }
            //Kits
            IEnumerable<SelectItemModel> kitList = _classroomBusiness.GetKitsSelectList(true, false);
            IEnumerable<SelectItemModel> currList = _masterBusiness.GetCurriculumSelectListForSchool();
            {
                ViewBag.kitName = kitList.Where(o => o.ID == classroom.KitId).Select(o => o.Name).FirstOrDefault();
                ViewBag.kitNeedName = kitList.Where(o => o.ID == classroom.FcNeedKitId).Select(o => o.Name).FirstOrDefault();

                ViewBag.part1KitName = kitList.Where(o => o.ID == classroom.Part1KitId).Select(o => o.Name).FirstOrDefault();
                ViewBag.part1NeedKitName = kitList.Where(o => o.ID == classroom.Part1NeedKitId).Select(o => o.Name).FirstOrDefault();

                ViewBag.part2KitName = kitList.Where(o => o.ID == classroom.Part2KitId).Select(o => o.Name).FirstOrDefault();

                ViewBag.part2NeedKitName = kitList.Where(o => o.ID == classroom.Part2NeedKitId).Select(o => o.Name).FirstOrDefault();

                ViewBag.StartupKitName = kitList.Where(o => o.ID == classroom.StartupKitId).Select(o => o.Name).FirstOrDefault();
                ViewBag.StartupNeedKitName = kitList.Where(o => o.ID == classroom.StartupNeedKitId).Select(o => o.Name).FirstOrDefault();

                ViewBag.CurriculumKitName = currList.Where(o => o.ID == classroom.CurriculumId).Select(o => o.Name).FirstOrDefault();
                ViewBag.CurriculumNeedKitName = currList.Where(o => o.ID == classroom.NeedCurriculumId).Select(o => o.Name).FirstOrDefault();

                ViewBag.TalkerKitName = kitList.Where(o => o.ID == classroom.DevelopingTalkersKitId).Select(o => o.Name).FirstOrDefault();
                ViewBag.TalkerNeedKitName = kitList.Where(o => o.ID == classroom.DevelopingTalkersNeedKitId).Select(o => o.Name).FirstOrDefault();

                ViewBag.FCCKitName = kitList.Where(o => o.ID == classroom.FccKitId).Select(o => o.Name).FirstOrDefault();
                ViewBag.FCCNeedKitName = kitList.Where(o => o.ID == classroom.FccNeedKitId).Select(o => o.Name).FirstOrDefault();

            }

            return View(classroom);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.Assign, PageId = PagesModel.Classroom_Management, Anonymity = Anonymous.Verified)]
        public ActionResult AssignedClasses(int id)
        {
            ClassroomModel classroom = _classroomBusiness.GetClassroom(id, UserInfo);
            return View(classroom);
        }
        #endregion

        #region Ajax Methods
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CLIUrlAuthorizeAttribute(Account = Authority.Edit, PageId = PagesModel.Classroom_Management, Anonymity = Anonymous.Verified)]
        public string Update(ClassroomEntity classroom)
        {
            var response = new PostFormResponse();
            OperationResult result = _classroomBusiness.UpdateClassroom(classroom, GetRole());
            response.Success = result.ResultType == OperationResultType.Success;
            response.Data = classroom;
            response.Message = result.Message;
            return JsonConvert.SerializeObject(response);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CLIUrlAuthorizeAttribute(Account = Authority.Add, PageId = PagesModel.Classroom_Management, Anonymity = Anonymous.Verified)]
        public string New(ClassroomEntity classroom)
        {
            var response = new PostFormResponse();
            OperationResult result = _classroomBusiness.InsertClassroom(classroom, GetRole());
            response.Success = result.ResultType == OperationResultType.Success;
            response.Data = classroom;
            response.Message = result.Message;
            return JsonConvert.SerializeObject(response);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.Administrative, Anonymity = Anonymous.Logined)]
        public string IsClassroomExist(string name, int id = 0, int schoolId = 0)
        {
            return _classroomBusiness.IsClassroomExist(name, id, schoolId).ToString().ToLower();
        }

        #endregion

        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.Classroom_Management, Anonymity = Anonymous.Verified)]
        public string SearchClasses(int classroomId = -1, string sort = "Name", string order = "Asc", int first = 0, int count = 10)
        {
            IEnumerable<AssignClassModel> classList = _classBusiness.GetClassesByClassroomId(classroomId, UserInfo);
            var result = new { total = 100, data = classList };
            return JsonHelper.SerializeObject(result);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.Classroom_Management, Anonymity = Anonymous.Verified)]
        [ValidateInput(false)]
        public string Search(int communityId = -1, string communityName = "", int schoolId = -1, string schoolName = "", string classroomName = "", string classroomId = "", int status = -1,
            string sort = "Name", string order = "Asc", int first = 0, int count = 10)
        {
            var total = 0;
            var expression = PredicateHelper.True<ClassroomEntity>();
            if (communityId >= 1)
                expression = expression.And(o => o.School.CommunitySchoolRelations.Count
                    (r => r.CommunityId == communityId) > 0);
            else if (communityName != null && communityName.Trim() != string.Empty)
            {
                communityName = communityName.Trim();
                expression = expression.And(o => o.School.CommunitySchoolRelations.Count(r => r.Community.Name.Contains(communityName)) > 0);
            }

            if (schoolId >= 1)
                expression = expression.And(o => o.SchoolId.Equals(schoolId));
            else if (schoolName != null && schoolName.Trim() != string.Empty)
                expression = expression.And(o => o.School.BasicSchool.Name.Contains(schoolName));

            if (classroomName != null && classroomName.Trim() != string.Empty)
                expression = expression.And(o => o.Name.Contains(classroomName.Trim()));

            if (classroomId != null && classroomId.Trim() != string.Empty)
                expression = expression.And(o => o.ClassroomId.Contains(classroomId.Trim()));

            if (status >= 0)
                expression = expression.And(o => (int)o.Status == status);

            var list = _classroomBusiness.SearchClassrooms(UserInfo, expression, sort, order, first, count, out total);
            var result = new { total = total, data = list };
            return JsonHelper.SerializeObject(result);
        }

        private void InitControls()
        {

            ViewBag.StatusOptions = EntityStatus.Active.ToSelectList(true);
            ViewBag.InterventionStatusOptions = EnumToDDL(InterventionStatus.EngageOnly);
            ViewBag.ClassroomFundingOptions = ListToDDL(_masterBusiness.GetFundingSelectList(), "", "");
            ViewBag.FundingOptions = ListToDDL(_masterBusiness.GetFundingSelectList(), "", "0");
            ViewBag.KitOptions = ListToDDL(_classroomBusiness.GetKitsSelectList(), "", "0");
            ViewBag.NeedKitOptions = ListToDDL(_classroomBusiness.GetKitsSelectList(false), "", "0");
            ViewBag.CurriculumOptions = ListToDDL(_masterBusiness.GetCurriculumSelectListForSchool(), "", "0");
            ViewBag.CurriculumNeedOptions = ListToDDL(_masterBusiness.GetCurriculumSelectListForSchool(false), "", "0");
            ViewBag.InternetSpeedOptions = EnumToDDL(InternetSpeed.LessThan15, "0");
            ViewBag.InternetTypeOptions = EnumToDDL(InternetType.None);
            ViewBag.WirelessTypeOptions = EnumToDDL(WireLessType.WirelessA);
            ViewBag.CommunityOptions = ListToDDL(communityBusiness.GetCommunitySelectList(UserInfo));
        }


        private IEnumerable<SelectListItem> EnumToDDL(Enum eEnum, string defaultVal = "")
        {
            return eEnum.ToSelectList(true, new SelectOptions(true, defaultVal, ViewTextHelper.DefaultPleaseSelectText));
        }

        private IEnumerable<SelectListItem> ListToDDL(IEnumerable<SelectItemModel> list, string defaultText = "", string defaultValue = "")
        {
            if (defaultText == "") defaultText = ViewTextHelper.DefaultPleaseSelectText;
            return list.ToSelectList(defaultText, defaultValue);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.Administrative, Anonymity = Anonymous.Logined)]
        public string GetClassroomSelectList(string keyword, int communityId = -1, int schoolId = -1)
        {
            IEnumerable<SelectItemModel> list = _classroomBusiness.GetClassroomSelectList(UserInfo, communityId, schoolId, keyword);
            return JsonHelper.SerializeObject(list);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.Administrative, Anonymity = Anonymous.Logined)]
        public string GetClassroomNameSelectList(int communityId = -1, int schoolId = -1, string classroomName = "", bool isActive = true)
        {
            var expression = PredicateHelper.True<ClassroomEntity>();
            if (communityId > 0)
                expression = expression.And(o => o.School.
                    CommunitySchoolRelations.Count(r => r.CommunityId == communityId) > 0);
            if (schoolId > 0)
                expression = expression.And(o => o.SchoolId == schoolId);
            if (classroomName != null && classroomName.Trim() == string.Empty)
                expression = expression.And(o => o.Name.Contains(classroomName));

            var list = _classroomBusiness.GetClassroomNameSelectList(UserInfo, expression, isActive);
            return JsonHelper.SerializeObject(list);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.Administrative, Anonymity = Anonymous.Logined)]
        public string GetClassroomIdSelectList(int communityId = -1, int schoolId = -1, string classroomId = "", bool isActiveClassroomId = true)
        {
            var expression = PredicateHelper.True<ClassroomEntity>();
            if (schoolId > 0)
                expression = expression.And(o => o.SchoolId == schoolId);
            else if (communityId > 0)
                expression = expression.And(o => o.School.CommunitySchoolRelations.Count(r => r.CommunityId == communityId) > 0);

            if (classroomId != null && classroomId.Trim() == string.Empty)
                expression = expression.And(o => o.Name.Contains(classroomId));

            var list = _classroomBusiness.GetClassroomIdSelectList(UserInfo, expression, isActiveClassroomId);
            return JsonHelper.SerializeObject(list);
        }

        private Role GetRole()
        {
            Role newRole = UserInfo.Role;
            switch (UserInfo.Role)
            {
                case Role.District_Community_Delegate:
                    newRole = Role.Community;
                    break;
                case Role.Principal_Delegate:
                    newRole = Role.Principal;
                    break;
                case Role.TRS_Specialist_Delegate:
                    newRole = Role.TRS_Specialist;
                    break;
                case Role.School_Specialist_Delegate:
                    newRole = Role.School_Specialist;
                    break;
                case Role.Community_Specialist_Delegate:
                    newRole = Role.District_Community_Specialist;
                    break;
                default:
                    newRole = UserInfo.Role;
                    break;
            }
            return newRole;
        }

        /// <summary>
        /// 控制按钮操作是否显示
        /// </summary>
        private void InitAccessOperation()
        {
            bool accessAdd = false;
            bool accessEdit = false;
            bool accessView = false;
            bool accessAssign = false;
            if (UserInfo != null)
            {
                UserAuthorityModel userAuthority = new PermissionBusiness().GetUserAuthority(UserInfo, (int)PagesModel.Classroom_Management);

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
                    if ((userAuthority.Authority & (int)Authority.View) == (int)Authority.View)
                    {
                        accessView = true;
                    }
                    if ((userAuthority.Authority & (int)Authority.Assign) == (int)Authority.Assign)
                    {
                        accessAssign = true;
                    }
                }
            }
            ViewBag.accessAdd = accessAdd;
            ViewBag.accessEdit = accessEdit;
            ViewBag.accessView = accessView;
            ViewBag.accessAssign = accessAssign;
        }


        #region Assign Classes
        [CLIUrlAuthorizeAttribute(Account = Authority.Assign, PageId = PagesModel.Classroom_Management, Anonymity = Anonymous.Verified)]
        public ActionResult AssignClasses(int classroomId = 0)
        {
            ClassroomEntity classroomEntity = _classroomBusiness.GetClassroom(classroomId);
            return View(classroomEntity);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.Assign, PageId = PagesModel.Classroom_Management, Anonymity = Anonymous.Verified)]
        public string SearchAssignedClass(int classroomId = 0, string name = "",
            string sort = "Name", string order = "ASC", int first = 0, int count = 10)
        {
            int total = 0;
            var expression = PredicateHelper.True<ClassroomClassEntity>();

            expression = expression.And(r => r.ClassroomId == classroomId);

            if (name.Trim() != string.Empty)
            {
                var className = name.Trim();
                expression = expression.And(r => r.Class.Name.Contains(className) && r.Class.IsDeleted == false);
            }

            var list = _classBusiness.GetAssignedClass(UserInfo, expression, sort, order, first, count, out total);
            var result = new { total = total, data = list };
            return JsonHelper.SerializeObject(result);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.Assign, PageId = PagesModel.Classroom_Management, Anonymity = Anonymous.Verified)]
        public string SearchUnAssignedClass(int schoolId = 0, int classroomId = 0, string name = "",
            string sort = "Name", string order = "ASC", int first = 0, int count = 10)
        {
            int total = 0;
            var list = _classBusiness.GetUnAssignClass(UserInfo, schoolId, classroomId, name.Trim(), sort, order, first, count, out total);
            var result = new { total = total, data = list };
            string s = JsonHelper.SerializeObject(result);
            return s;
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.Assign, PageId = PagesModel.Classroom_Management, Anonymity = Anonymous.Verified)]
        public string AssignRelations(int classroomId, int[] classIds)
        {
            var response = new PostFormResponse();
            ClassroomEntity classroomEntity = _classroomBusiness.GetClassroom(classroomId);
            OperationResult result = new OperationResult(OperationResultType.Success);
            result = _classBusiness.InserClassroomClassRelationsOnClassroom(classroomId, classIds);
            response.Success = result.ResultType == OperationResultType.Success;
            response.Data = "";
            response.Message = result.Message;
            return JsonHelper.SerializeObject(response);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.Assign, PageId = PagesModel.Classroom_Management, Anonymity = Anonymous.Verified)]
        public string DeleteRelations(int classroomId, int[] classIds)
        {
            var response = new PostFormResponse();
            OperationResult result = new OperationResult(OperationResultType.Success);
            result = _classBusiness.DeleteClassroomClassRelationsOnClassroom(classroomId, classIds);
            response.Success = result.ResultType == OperationResultType.Success;
            response.Data = "";
            response.Message = result.Message;
            return JsonHelper.SerializeObject(response);
        }
        #endregion
    }
}