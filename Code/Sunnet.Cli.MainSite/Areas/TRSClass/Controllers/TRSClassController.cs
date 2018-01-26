using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
using Sunnet.Cli.Business.Classes;
using Sunnet.Cli.Business.Classrooms;
using Sunnet.Cli.Business.Communities;
using Sunnet.Cli.Business.MasterData;
using Sunnet.Cli.Business.Schools;
using Sunnet.Cli.Business.TRSClasses;
using Sunnet.Cli.Business.Users;
using Sunnet.Cli.Core.Classes.Entites;
using Sunnet.Cli.Core.Common.Enums;
using Sunnet.Cli.Core.TRSClasses.Entites;
using Sunnet.Cli.Core.TRSClasses.Enums;
using Sunnet.Cli.Core.Users.Entities;
using Sunnet.Cli.Core.Users.Enums;
using Sunnet.Cli.UIBase.Models;
using Sunnet.Framework.Core.Tool;
using Sunnet.Framework.Extensions;
using Sunnet.Cli.MainSite.Controllers;
using Sunnet.Framework.Helpers;
using Sunnet.Cli.Core.Permission.Entities;
using Sunnet.Cli.Business.Permission;
using Sunnet.Framework.Permission;
using Sunnet.Cli.UIBase.Filters;
using Sunnet.Cli.Business.Permission.Models;
using Sunnet.Cli.Core.Classes.Enums;
using Sunnet.Cli.Business.Schools.Models;
using Sunnet.Cli.Core.Schools.Enums;
using Sunnet.Cli.Business.Classes.Models;
using Sunnet.Cli.Business.Common;

namespace Sunnet.Cli.MainSite.Areas.TRSClass.Controllers
{
    public class TRSClassController : BaseController
    {
        private readonly TRSClassBusiness _trsClassBusiness;
        private readonly MasterDataBusiness _masterDataBusiness;
        private readonly UserBusiness _userBusiness;
        private readonly SchoolBusiness _schoolBusiness;
        private readonly CommunityBusiness _communityBusiness;

        public TRSClassController()
        {
            _trsClassBusiness = new TRSClassBusiness(UnitWorkContext);
            _masterDataBusiness = new MasterDataBusiness(UnitWorkContext);
            _userBusiness = new UserBusiness(UnitWorkContext);
            _schoolBusiness = new SchoolBusiness(UnitWorkContext);
            _communityBusiness = new CommunityBusiness(UnitWorkContext);
        }

        #region Class Index/View/New/Edit

        /// <summary>
        /// community list
        /// </summary>
        /// <returns></returns>
        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.TRSClass_Management, Anonymity = Anonymous.Verified)]
        public ActionResult Index()
        {
            InitAccessOperation();
            List<SelectListItem> list = new List<SelectListItem>();
            list.Add(new SelectListItem()
            {
                Text = ViewTextHelper.DefaultAllText,
                Value = "-1",
                Selected = true
            });
            ViewBag.ClassroomOptions = list;
            return View();
        }

        #region Class New
        [CLIUrlAuthorizeAttribute(Account = Authority.Add, PageId = PagesModel.TRSClass_Management, Anonymity = Anonymous.Verified)]
        public ActionResult New(int schoolId = 0, string schoolName = "", int LeadTeacherId = 0)
        {
            TRSClassEntity entity = _trsClassBusiness.NewTRSClassModel();
            ViewBag.StatusOptions = EntityStatus.Active.ToSelectList();
            ViewBag.schoolId = 0;
            ViewBag.schoolName = "";
            entity.HomeroomTeacherId = 0;
            if (schoolId > 0)
            {
                ViewBag.schoolId = schoolId;
                ViewBag.schoolName = schoolName;
                entity.HomeroomTeacherId = LeadTeacherId;
            }
            #region trs
            ViewBag.ClassroomTypeOptions = TRSClassofType.MAC.ToSelectList(true);
            var types = _trsClassBusiness.GetChildrenType();
            var trsAccess = _trsClassBusiness.TRSAccess(UserInfo.Role);

            //数据将会在页面选择 School 后，有变更
            var trsObject = new
            {
                typeOfClass = 0,
                readOnly = false,
                visible = false,  //控制 Age of Children ,Type of Classroom  是否显示
                multiple = false,  //Type of Classroom 多选，还是单选
                show = false,  //Type of Classroom 是否显示,
                childrenTypes = types.ToList(),
                numbers = GenerateNumberList()
            };
            ViewBag.TrsObject = JsonHelper.SerializeObject(trsObject);
            ViewBag.TRSAccess = trsAccess;
            ViewBag.CoachOptions = new List<SelectListItem>() { new SelectListItem { Text = ViewTextHelper.DefaultPleaseSelectText, Value = "" } };
            ViewBag.HomeroomTeacherOptions = new List<SelectListItem>() { new SelectListItem { Text = ViewTextHelper.DefaultPleaseSelectText, Value = "0" } };
            #endregion
            return View(entity);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CLIUrlAuthorizeAttribute(Account = Authority.Add, PageId = PagesModel.TRSClass_Management, Anonymity = Anonymous.Verified)]
        public string New(TRSClassEntity entity, int[] hiddenChild, int[] dpChildrenNumber, int[] txtCaregiversNo)
        {
            var response = new PostFormResponse();

            List<CHChildrenResultEntity> resultList = new List<CHChildrenResultEntity>();
            if (dpChildrenNumber != null && hiddenChild != null)
            {
                if (hiddenChild.Length > 0)
                {
                    for (int i = 0; i < hiddenChild.Length; i++)
                    {
                        CHChildrenResultEntity temp = new CHChildrenResultEntity();
                        temp.TRSClassId = entity.ID;
                        temp.CHChildrenId = hiddenChild[i];
                        temp.ChildrenNumber = dpChildrenNumber[temp.CHChildrenId - 1];
                        temp.CaregiversNumber = txtCaregiversNo[temp.CHChildrenId - 1];
                        resultList.Add(temp);
                    }
                }
            }
            entity.Name = "TRS-" + entity.Name;
            entity.TypeOfClass = 0;
            OperationResult result = _trsClassBusiness.InsertTRSClass(entity, resultList);
            response.Success = result.ResultType == OperationResultType.Success;
            response.Message = result.Message;
            return JsonConvert.SerializeObject(response);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.TRSClass_Management, Anonymity = Anonymous.Logined)]
        public string IsClassExist(string name, int id = 0, int schoolId = 0)
        {
            return _trsClassBusiness.IsTRSClassExist("TRS-" + name, id, schoolId).ToString().ToLower();
        }

        #endregion

        #region Class Edit
        //eg: trsclass/trsclass/edit/16
        [CLIUrlAuthorizeAttribute(Account = Authority.Edit, PageId = PagesModel.TRSClass_Management, Anonymity = Anonymous.Verified)]
        public ActionResult Edit(int id)
        {
            TRSClassEntity entity = _trsClassBusiness.GetTRSClass(id);
            List<int> comIds = UserInfo.UserCommunitySchools.Where(o => o.Community != null).Select(o => o.Community.BasicCommunityId).ToList();
            ViewBag.StatusOptions = EntityStatus.Active.ToSelectList();
            ViewBag.communityName = string.Join(", ", entity.School.CommunitySchoolRelations.Select(r => r.Community.Name));
            ViewBag.schoolName = entity.School.BasicSchool.Name ?? string.Empty;
            List<SelectListItem> homeroomTeacherList = _userBusiness.GetTeacherBySchoolId(entity.SchoolId).ToSelectList();
            ViewBag.TeacherOptions = homeroomTeacherList.AddDefaultItem(ViewTextHelper.DefaultPleaseSelectText, "-1").ToList();

            #region trs
            ViewBag.ClassroomTypeOptions = TRSClassofType.MAC.ToSelectList(true);
            List<ChildrenTypeModel> types = _trsClassBusiness.GetChildrenType();
            List<CHChildrenResultEntity> classTypes = _trsClassBusiness.GetResultList(entity.ID);

            types.ForEach(type =>
            {
                var existed = classTypes.FirstOrDefault(x => x.CHChildrenId == type.ID);
                if (existed != null)
                {
                    type.Selected = true;
                    type.Count = existed.ChildrenNumber;
                    type.CaregiversNumber = existed.CaregiversNumber;
                }
            });
            var trsAccess = _trsClassBusiness.TRSAccess(UserInfo.Role, entity.SchoolId);
            var showAgeofChildren = false;
            if (trsAccess == Business.Permission.Permission.Invisible)
                showAgeofChildren = _communityBusiness.CheckShowAgeofChildren(entity.School.CommunitySchoolRelations.Select(r => r.CommunityId).ToList());
            else
                showAgeofChildren = true;

            var facilityType = entity.School.FacilityType;
            var single = facilityType == FacilityType.LCCH || facilityType == FacilityType.RCCH;
            var multiple = facilityType == FacilityType.LCAA || facilityType == FacilityType.LCSA;
            var classType = entity.TypeOfClass > 0 ? entity.TypeOfClass : TRSClassofType.NMAC;
            var classChildrenType = "_";
            if (multiple && facilityType > 0)
                classChildrenType = classType.ToString().ToUpper();
            else if (single && facilityType > 0)
                classChildrenType = facilityType.ToString().ToUpper().Substring(0, 2);
            var trsObject = new
            {
                typeOfClass = (int)entity.TypeOfClass,
                readOnly = trsAccess == Business.Permission.Permission.Readonly,
                visible = showAgeofChildren,
                multiple = multiple,
                show = trsAccess != Business.Permission.Permission.Invisible,
                childrenTypes = types.ToList(),
                numbers = GenerateNumberList()
            };


            ViewBag.TrsObject = JsonHelper.SerializeObject(trsObject);
            ViewBag.TRSAccess = trsAccess;
            ViewBag.CoachOptions = _userBusiness.GetSchoolSpecialist(new int[] { entity.SchoolId })
                .ToSelectList(ViewTextHelper.DefaultPleaseSelectText, "");
            #endregion

            if (entity.Name.Length >= 4 && entity.Name.Substring(0, 4).ToUpper() == "TRS-")
                entity.Name = entity.Name.Remove(0, 4);
            return View(entity);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CLIUrlAuthorizeAttribute(Account = Authority.Edit, PageId = PagesModel.TRSClass_Management,
            Anonymity = Anonymous.Verified)]
        public string Edit(int id, TRSClassEntity entity, int[] hiddenChild, int[] dpChildrenNumber, int[] txtCaregiversNo)
        {
            entity.TrsMentorId = entity.TrsMentorId == null ? 0 : entity.TrsMentorId;
            var response = new PostFormResponse();

            List<CHChildrenResultEntity> resultList = new List<CHChildrenResultEntity>();
            if (dpChildrenNumber != null && hiddenChild != null)
            {
                if (hiddenChild.Length >= dpChildrenNumber.Length)
                {
                    for (int i = 0; i < dpChildrenNumber.Length; i++)
                    {
                        CHChildrenResultEntity temp = new CHChildrenResultEntity();
                        temp.TRSClassId = entity.ID;
                        temp.CHChildrenId = hiddenChild[i];
                        temp.ChildrenNumber = dpChildrenNumber[i];
                        temp.CaregiversNumber = txtCaregiversNo[i];
                        resultList.Add(temp);
                    }
                }
            }

            //   if (entity.Name.Length>=4 && entity.Name.Substring(0, 4).ToUpper() != "TRS-")
            entity.Name = "TRS-" + entity.Name;
            OperationResult result = _trsClassBusiness.UpdateTRSClass(entity, resultList);
            response.Success = result.ResultType == OperationResultType.Success;
            response.Message = result.Message;
            return JsonConvert.SerializeObject(response);
        }

        #endregion

        #region Class View
        [CLIUrlAuthorizeAttribute(Account = Authority.View, PageId = PagesModel.TRSClass_Management, Anonymity = Anonymous.Verified)]
        public ActionResult View(int id)
        {
            var entity = _trsClassBusiness.GetTRSClass(id);

            ViewBag.communityName = string.Join(", ", entity.School.CommunitySchoolRelations.Select(r => r.Community.Name));
            ViewBag.schoolName = entity.School.BasicSchool.Name ?? string.Empty;
            ViewBag.schoolType = entity.School.SchoolType.Name ?? string.Empty;

            #region TRS
            var trsAccess = _trsClassBusiness.TRSAccess(UserInfo.Role, entity.SchoolId);
            var showAgeofChildren = false;
            if (trsAccess == Business.Permission.Permission.Invisible)
                showAgeofChildren = _communityBusiness.CheckShowAgeofChildren(entity.School.CommunitySchoolRelations.Select(r => r.CommunityId).ToList());
            else
                showAgeofChildren = true;

            ViewBag.ShowAgeofChildren = showAgeofChildren;
            ViewBag.TRSAccess = trsAccess != Business.Permission.Permission.Invisible;

            ViewBag.LCorRCChilden = _trsClassBusiness.GetResultModelList(entity.ID);

            UsernameModel homeroomTeacher = _userBusiness.GetUsername(entity.HomeroomTeacherId);
            ViewBag.HomeroomTeacher = homeroomTeacher == null
                ? ""
                : (homeroomTeacher.Firstname + " " + homeroomTeacher.Lastname);

            List<int> list_userIds = new List<int>();
            if (entity.TrsAssessorId > 0)
                list_userIds.Add(entity.TrsAssessorId);
            if (entity.TrsMentorId > 0)
                list_userIds.Add(entity.TrsMentorId.Value);
            List<UsernameModel> list_names = _userBusiness.GetUsernames(list_userIds);


            ViewBag.AssessorName = list_names.Find(r => r.ID == entity.TrsAssessorId) == null ? "" :
                list_names.Find(r => r.ID == entity.TrsAssessorId).Firstname + " " + list_names.Find(r => r.ID == entity.TrsAssessorId).Lastname;
            ViewBag.MentorName = list_names.Find(r => r.ID == entity.TrsMentorId) == null ? "" :
                list_names.Find(r => r.ID == entity.TrsMentorId).Firstname + " " + list_names.Find(r => r.ID == entity.TrsMentorId).Lastname;
            #endregion

            return View(entity);
        }
        #endregion
        #endregion

        #region Public Function

        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.TRSClass_Management,
            Anonymity = Anonymous.Verified)]
        [ValidateInput(false)]
        public string Search(int communityId = -1, string communityName = "", int schoolId = -1,
            string schoolName = "", string trsClassId = "", string trsClassName = "",
            int status = -1, string sort = "TRSClassId", string order = "Asc", int first = 0, int count = 10)
        {
            var users = new List<int>() { 0, 1 };
            var total = 0;
            var expression = PredicateHelper.True<TRSClassEntity>();
            if (communityId >= 1)
                expression =
                    expression.And(
                        r => r.School.CommunitySchoolRelations.Select(c => c.CommunityId).Contains(communityId));
            else if (communityName != null && communityName.Trim() != string.Empty)
                expression =
                    expression.And(
                        o =>
                            o.School.CommunitySchoolRelations.Count(r => r.Community.Name.Contains(communityName.Trim())) >
                            0);
            if (schoolId >= 1)
                expression = expression.And(o => o.SchoolId == schoolId);
            else if (schoolName != null && schoolName.Trim() != string.Empty)
                expression = expression.And(o => o.School.Name.Contains(schoolName.Trim()));
            if (trsClassId != null && trsClassId.Trim() != string.Empty)
                expression = expression.And(o => o.TRSClassId.Contains(trsClassId.Trim()));
            if (trsClassName != null && trsClassName.Trim() != string.Empty)
                expression = expression.And(o => o.Name.Contains(trsClassName.Trim()));
            if (status >= 0)
                expression = expression.And(o => (int)o.Status == status);
            var list = _trsClassBusiness.SearchTRSClasses(UserInfo, expression, sort, order, first, count, out total);

            var result = new { total = total, data = list };
            return JsonHelper.SerializeObject(result);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.TRSClass_Management,
            Anonymity = Anonymous.Verified)]
        public string GetTRSClassId(int communityId = 0, int schoolId = 0)
        {
            var expression = PredicateHelper.True<TRSClassEntity>();
            if (schoolId > 0)
                expression = expression.And(o => o.SchoolId == schoolId);
            var list = _trsClassBusiness.GetTRSClassId(expression);
            return JsonHelper.SerializeObject(list);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.Administrative, Anonymity = Anonymous.Logined)]
        public string GetTRSClassName(int communityId = 0, int schoolId = 0)
        {
            var expression = PredicateHelper.True<TRSClassEntity>();
            if (schoolId > 0)
                expression = expression.And(o => o.SchoolId == schoolId);
            var list = _trsClassBusiness.GetTRSClassSelectList(UserInfo, expression);
            return JsonHelper.SerializeObject(list);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.TRSClass_Management, Anonymity = Anonymous.Verified)]
        public string GetTrsAccess(int schoolId, string communityId)
        {
            var trsAccess = _trsClassBusiness.TRSAccess(UserInfo.Role, schoolId);
            var response = new PostFormResponse();
            response.Success = trsAccess == Business.Permission.Permission.Editable;//是否有TRS 操作权限
            var show = false; //是否有 Age of Children 操作权限
            var typeofClassroom = string.Empty;

            if (trsAccess == Business.Permission.Permission.Editable)
            {
                show = true;
                typeofClassroom = _schoolBusiness.GetSchoolFacilityType(schoolId);
            }
            else
            {
                int tmpCommunityId;
                if (int.TryParse(communityId, out tmpCommunityId))
                    show = _communityBusiness.CheckShowAgeofChildren(new List<int> { tmpCommunityId });
                else
                    show = _schoolBusiness.CheckShowAgeofChildren(schoolId);
            }

            response.Data = new { TypeofClassroom = typeofClassroom, Show = show };
            return JsonHelper.SerializeObject(response);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.TRSClass_Management, Anonymity = Anonymous.Verified)]
        public string GetTrsAssessor(int schoolId)
        {
            List<SelectListItem> trsAssessors = _userBusiness.GetSchoolSpecialist(new int[] { schoolId }).ToSelectList();
            return JsonHelper.SerializeObject(trsAssessors);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.TRSClass_Management,
            Anonymity = Anonymous.Verified)]
        public string GetHomeroomTeacher(int schoolId)
        {
            List<SelectListItem> homeroomTeacherList = _userBusiness.GetTeacherBySchoolId(schoolId).ToSelectList();
            return JsonHelper.SerializeObject(homeroomTeacherList);
        }

        List<SelectListItem> GenerateNumberList()
        {
            List<SelectListItem> list = new List<SelectListItem>();
            for (int i = 0; i <= 31; i++)
            {
                list.Add(new SelectListItem() { Text = i.ToString(), Value = i.ToString() });
            }
            return list;
        }
        #endregion

        #region Private Function
        /// <summary>
        /// 控制按钮操作是否显示
        /// </summary>
        private void InitAccessOperation()
        {
            bool accessAdd = false;
            bool accessEdit = false;
            bool accessView = false;
            if (UserInfo != null)
            {
                UserAuthorityModel userAuthority = new PermissionBusiness().GetUserAuthority(UserInfo, (int)PagesModel.TRSClass_Management);

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
                }
            }
            ViewBag.accessAdd = accessAdd;
            ViewBag.accessEdit = accessEdit;
            ViewBag.accessView = accessView;
        }
        #endregion
    }
}
