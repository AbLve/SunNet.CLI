using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Web;
using System.Web.Mvc;
/**************************************************************************
 * Developer: 		Leason
 * Computer:		Leason-PC
 * Domain:			Leason-pc
 * CreatedOn:		2014/8/23 16:43:20
 * Description:		Create ClassController
 * Version History:	Created,2014/8/23 16:43:20
 * 
 * 
 **************************************************************************/
using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
using Sunnet.Cli.Business.Ade;
using Sunnet.Cli.Business.Classes;
using Sunnet.Cli.Business.Classrooms;
using Sunnet.Cli.Business.Communities;
using Sunnet.Cli.Business.Communities.Models;
using Sunnet.Cli.Business.Cpalls.Models;
using Sunnet.Cli.Business.MasterData;
using Sunnet.Cli.Business.Schools;
using Sunnet.Cli.Business.Users;
using Sunnet.Cli.Core.Classes.Entites;
using Sunnet.Cli.Core.Common.Enums;
using Sunnet.Cli.Core.Communities.Entities;
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

namespace Sunnet.Cli.MainSite.Areas.Class.Controllers
{
    public class ClassController : BaseController
    {

        #region Private Field
        private readonly ClassBusiness _classBusiness;
        private readonly MasterDataBusiness _masterDataBusiness;
        private readonly ClassroomBusiness _classroomBusiness;
        private readonly UserBusiness _userBusiness;
        private readonly SchoolBusiness _schoolBusiness;
        private readonly CommunityBusiness _communityBusiness;
        #endregion

        #region Public Contruction
        public ClassController()
        {
            _classBusiness = new ClassBusiness(UnitWorkContext);
            _masterDataBusiness = new MasterDataBusiness(UnitWorkContext);
            _classroomBusiness = new ClassroomBusiness(UnitWorkContext);
            _userBusiness = new UserBusiness(UnitWorkContext);
            _schoolBusiness = new SchoolBusiness(UnitWorkContext);
            _communityBusiness = new CommunityBusiness(UnitWorkContext);
        }
        #endregion

        #region Class Index/View/New/Edit

        /// <summary>
        /// community list
        /// </summary>
        /// <returns></returns>
        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.Class_Management, Anonymity = Anonymous.Verified)]
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

        /// <summary>
        /// view
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [CLIUrlAuthorizeAttribute(Account = Authority.View, PageId = PagesModel.Class_Management, Anonymity = Anonymous.Verified)]
        public ActionResult View(int id)
        {
            ClassRoleEntity roleEntity = _classBusiness.GetClassRole(UserInfo.Role);
            ViewBag.Role = JsonHelper.SerializeObject(roleEntity);
            var entity = _classBusiness.GetClass(id);
            List<int> comIds = UserInfo.UserCommunitySchools.Where(o => o.Community != null).Select(o => o.Community.BasicCommunityId).ToList();
            if (UserInfo.Role == Role.Community || UserInfo.Role == Role.District_Community_Specialist ||
                UserInfo.Role == Role.Statewide)
            {
                if (!(entity.UserClasses.Any(o => o.UserId == UserInfo.ID) || comIds.Contains(entity.School.BasicSchool.CommunityId))
                    )
                { Response.Redirect("Index"); }
            }

            InitViewParameters(entity);
            ViewBag.communityName = string.Join(", ", entity.School.CommunitySchoolRelations.Select(r => r.Community.Name));
            ViewBag.schoolName = entity.School.BasicSchool.Name ?? string.Empty;
            ViewBag.classroomName = string.Join(", ", entity.ClassroomClasses.Select(r => r.Classroom.Name));
            ViewBag.schoolType = entity.School.SchoolType.Name ?? string.Empty;

         var classLevelEntity = _classBusiness.GetClassLevels().FirstOrDefault(c => c.ID == entity.Classlevel);
            if (classLevelEntity != null)
            {
                ViewBag.ClassLevelName = classLevelEntity.Name;
            }
            else
            {
                ViewBag.ClassLevelName = "";
            }

            GetTeacherByClassId(id);
            #region Assessments

            var listCompanyId = entity.School.CommunitySchoolRelations.Select(r => r.CommunityId).ToList();
            var featureList = new List<CpallsAssessmentModel>();
            List<CommunityAssessmentRelationsEntity> assignedList = _communityBusiness.GetAssignedAssessments(listCompanyId).ToList();

            AdeBusiness adeBus = new AdeBusiness();
            if (assignedList.Count > 0)
            {
                assignedList =
                    assignedList.Where(c => ("," + c.ClassLevelIds + ",").Contains("," + entity.Classlevel.ToString() + ",")
                                         || ("," + c.ClassLevelIds + ",").Contains(",0,")).ToList();
                var assessmentIds = assignedList.Select(c => c.AssessmentId).ToList();
                featureList = adeBus.GetAssessmentList(c => assessmentIds.Contains(c.ID));
            }
            #endregion

            ViewBag.FeatureList = featureList;


            return View(entity);
        }

        #region Class New
        [CLIUrlAuthorizeAttribute(Account = Authority.Add, PageId = PagesModel.Class_Management, Anonymity = Anonymous.Verified)]
        public ActionResult New(int schoolId = 0, string schoolName = "", int LeadTeacherId = 0)
        {
            ClassRoleEntity roleEntity = _classBusiness.GetClassRole(UserInfo.Role);
            ViewBag.Role = JsonHelper.SerializeObject(roleEntity);
            ClassEntityModel entity = _classBusiness.NewClassModel();
            InitParameters();
            ViewBag.schoolId = "";
            ViewBag.schoolName = "";
            entity.LeadTeacherId = 0;
            if (schoolId > 0)
            {
                ViewBag.schoolId = schoolId;
                ViewBag.schoolName = schoolName;
                entity.LeadTeacherId = LeadTeacherId;
            }
            return View(entity);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CLIUrlAuthorizeAttribute(Account = Authority.Add, PageId = PagesModel.Class_Management, Anonymity = Anonymous.Verified)]
        public string New(ClassEntity entity, int[] languageSelectList, int classroomId = 0)
        {
            var response = new PostFormResponse();

            string code = UserInfo.Role == Role.Teacher ? UserInfo.TeacherInfo.TeacherId : string.Empty;
            OperationResult result = _classBusiness.InsertClass(entity, languageSelectList, UserInfo.Role, classroomId, UserInfo, code);
            if (result.ResultType == OperationResultType.Success && entity.LeadTeacherId > 0)
            {
                result = _classBusiness.AssignTeacherToClass(entity.LeadTeacherId, entity.ID);
            }
            response.Success = result.ResultType == OperationResultType.Success;
            response.Message = result.Message;
            return JsonConvert.SerializeObject(response);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.Class_Management, Anonymity = Anonymous.Logined)]
        public string IsClassExist(string name, int id = 0, int schoolId = 0)
        {
            return _classBusiness.IsClassExist(name, id, schoolId).ToString().ToLower();
        }

        #endregion

        #region Class Edit
        //eg: class/class/edit/16
        [CLIUrlAuthorizeAttribute(Account = Authority.Edit, PageId = PagesModel.Class_Management, Anonymity = Anonymous.Verified)]
        public ActionResult Edit(int id)
        {
            ClassRoleEntity roleEntity = _classBusiness.GetClassRole(UserInfo.Role);
            ViewBag.Role = JsonHelper.SerializeObject(roleEntity);
            ClassEntity entity = _classBusiness.GetClass(id);
            List<int> comIds = UserInfo.UserCommunitySchools.Where(o => o.Community != null).Select(o => o.Community.BasicCommunityId).ToList();
            if (UserInfo.Role == Role.Community || UserInfo.Role == Role.District_Community_Specialist ||
                UserInfo.Role == Role.Statewide)
            {
                if (!(entity.UserClasses.Any(o => o.UserId == UserInfo.ID) || comIds.Contains(entity.School.BasicSchool.CommunityId))
                    )
                { Response.Redirect("Index"); }
            }


            InitParameters();

            #region assignLanguage
            StringBuilder sbLanguage = new StringBuilder();
            //format ["1","2","-10"]
            sbLanguage.Append("[");
            if (entity.Languages.Any())
            {
                foreach (var lang in entity.Languages)
                {
                    sbLanguage.AppendFormat("'{0}',", lang.ID);
                }
                sbLanguage.Append("'-10'");
            }
            sbLanguage.Append("]");
            ViewBag.language = sbLanguage.ToString();
            #endregion


            ViewBag.communityName = string.Join(", ", entity.School.CommunitySchoolRelations.Select(r => r.Community.Name));
            ViewBag.schoolName = entity.School.BasicSchool.Name ?? string.Empty;
            ViewBag.classroomName = string.Join(", ", entity.ClassroomClasses.Select(r => r.Classroom.Name));
            GetTeacherByClassId(id);

            List<SelectItemModel> teacherList =
                entity.Teachers.Where(r => r.UserInfo.Status == EntityStatus.Active && r.UserInfo.IsDeleted == false)
                    .Select(
                        r =>
                            new SelectItemModel()
                            {
                                ID = r.ID,
                                Name = string.Format("{0} {1}", r.UserInfo.FirstName, r.UserInfo.LastName)
                            })
                    .ToList();


            ViewBag.TeacherOptions = teacherList.ToSelectList().AddDefaultItem(ViewTextHelper.DefaultPleaseSelectText, "-1").ToList();



            #region Assessments

            var listCompanyId = entity.School.CommunitySchoolRelations.Select(r => r.CommunityId).ToList();
            var featureList = new List<CpallsAssessmentModel>();
            List<CommunityAssessmentRelationsEntity> assignedList = _communityBusiness.GetAssignedAssessments(listCompanyId).ToList();
          
            AdeBusiness adeBus = new AdeBusiness();
            if (assignedList.Count > 0)
            {
                assignedList =
                    assignedList.Where(c => ("," + c.ClassLevelIds + ",").Contains("," + entity.Classlevel.ToString() + ",")
                       || ("," + c.ClassLevelIds + ",").Contains(",0,")).ToList();
                var assessmentIds = assignedList.Select(c => c.AssessmentId).ToList();
                featureList = adeBus.GetAssessmentList(c => assessmentIds.Contains(c.ID));
            }
            #endregion

            ViewBag.FeatureList = featureList;
            return View(entity);
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CLIUrlAuthorizeAttribute(Account = Authority.Edit, PageId = PagesModel.Class_Management, Anonymity = Anonymous.Verified)]
        public string Edit(int id, ClassEntity entity, int[] languageSelectList)
        {
            var response = new PostFormResponse();

            OperationResult result = _classBusiness.UpdateClass(entity, languageSelectList, UserInfo.Role);
            response.Success = result.ResultType == OperationResultType.Success;
            response.Message = result.Message;
            //response.Data = result;
            return JsonConvert.SerializeObject(response);
        }
        #endregion

        #endregion

        #region Teacher
        //Edit TeacherEmployBy
        [CLIUrlAuthorizeAttribute(Account = Authority.Assign, PageId = PagesModel.Class_Management, Anonymity = Anonymous.Verified)]
        public void GetTeacherByClassId(int classId)
        {
            ClassEntity classEntity = _classBusiness.GetClass(classId);
            List<string> list = new List<string>();
            if (classEntity.Teachers != null && classEntity.Teachers.Any(e => e.UserInfo.IsDeleted == false))
            {
                foreach (TeacherEntity teacher in classEntity.Teachers.Where(e => e.UserInfo.IsDeleted == false))
                {
                    if (!list.Any(m => teacher.EmployedBy == 0 || m.Contains(teacher.EmployedBy.ToDescription())))
                    {
                        list.Add(teacher.EmployedBy.ToDescription());
                    }
                }
                list.RemoveAll(o => o.Contains("0"));
            }
            ViewBag.TeacherList = list;
        }

        //Assign Teacher To Class
        [CLIUrlAuthorizeAttribute(Account = Authority.Assign, PageId = PagesModel.Class_Management, Anonymity = Anonymous.Verified)]
        public ActionResult AssignTeacher(int schoolId, int classId)
        {
            List<SelectItemModel> teacherList = _userBusiness.GetTeachersBySchoolId(schoolId);
            List<SelectListItem> leadTeacherList = new List<SelectListItem>();

            if (teacherList != null && teacherList.Any())
            {
                ClassEntity classEntity = _classBusiness.GetClass(classId);
                if (classEntity != null && classEntity.Teachers != null)
                {
                    foreach (var teacher in classEntity.Teachers.Where(e => e.UserInfo.IsDeleted == false))
                    {
                        SelectItemModel item = teacherList.Find(o => o.ID == teacher.ID);
                        if (item != null)
                            item.Selected = true;
                    }
                }

                foreach (var teacher in classEntity.Teachers.Where(e => e.UserInfo.IsDeleted == false))
                {
                    SelectItemModel item = teacherList.Find(o => o.ID == teacher.ID);

                    if (item != null)
                    {
                        if (classEntity.LeadTeacherId > 0 && classEntity.LeadTeacherId == teacher.ID)
                            leadTeacherList.Add(new SelectListItem() { Text = item.Name, Value = teacher.ID.ToString(), Selected = true });
                        else
                            leadTeacherList.Add(new SelectListItem() { Text = item.Name, Value = teacher.ID.ToString() });
                    }
                }
            }

            leadTeacherList.Insert(0, new SelectListItem() { Text = ViewTextHelper.DefaultPleaseSelectText, Value = "-1" });

            ViewBag.teacherList = teacherList;
            ViewBag.ddlLeadTeacher = leadTeacherList;
            ViewBag.classId = classId;

            bool accessAddTeacher = false;

            UserAuthorityModel userAuthority = new PermissionBusiness().GetUserAuthority(UserInfo, (int)PagesModel.Teacher);
            if (userAuthority != null)
            {
                if ((userAuthority.Authority & (int)Authority.Add) == (int)Authority.Add)
                {
                    accessAddTeacher = true;
                }
            }
            ViewBag.AccessAddTeacher = accessAddTeacher;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CLIUrlAuthorizeAttribute(Account = Authority.Assign, PageId = PagesModel.Class_Management, Anonymity = Anonymous.Verified)]
        public string AssignTeacher(int[] chkTeacher, int classId, int LeadTeacher)
        {
            var response = new PostFormResponse();
            var result = new OperationResult(OperationResultType.Error);
            ClassEntity classEntity = _classBusiness.GetClass(classId);
            if (classEntity != null)
            {
                result = _classBusiness.AssignTeacherToClass(chkTeacher, classId);
                if (result.ResultType == OperationResultType.Success)
                {
                    if (LeadTeacher <= 0)
                    {
                        classEntity.LeadTeacherId = 0;
                    }
                    else
                    {
                        if (LeadTeacher > 0 && chkTeacher != null && chkTeacher.Contains(LeadTeacher))
                            classEntity.LeadTeacherId = LeadTeacher;
                        else
                            classEntity.LeadTeacherId = 0;
                    }

                    result = _classBusiness.UpdateClass(classEntity);
                }
                response.Success = result.ResultType == OperationResultType.Success;
            }
            response.Message = result.Message;

            return JsonConvert.SerializeObject(response);
        }
        #endregion

        #region Public Function
        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.Administrative, Anonymity = Anonymous.Logined)]
        public String GetClassroomSelectList(int communityId = -1, int schoolId = -1)
        {
            var list = _classroomBusiness.GetClassroomSelectList(UserInfo, communityId, schoolId).ToList();
            return JsonHelper.SerializeObject(new SelectList(list, "ID", "Name").AddDefaultItem(ViewTextHelper.DefaultAllText, "-1"));
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.Class_Management, Anonymity = Anonymous.Verified)]
        [ValidateInput(false)]
        public string Search(int communityId = -1, string communityName = "", int schoolId = -1,
            string schoolName = "", int classroomId = -1, string classId = "", string className = "",
            int status = -1, string sort = "ClassId", string order = "Asc", int first = 0, int count = 10)
        {
            var users = new List<int>() { 0, 1 };
            var total = 0;
            var expression = PredicateHelper.True<ClassEntity>();
            if (communityId >= 1)
                expression = expression.And(r => r.School.CommunitySchoolRelations.Select(c => c.CommunityId).Contains(communityId));
            else if (communityName != null && communityName.Trim() != string.Empty)
                expression = expression.And(o => o.School.CommunitySchoolRelations.Count(r => r.Community.Name.Contains(communityName.Trim())) > 0);
            if (schoolId >= 1)
                expression = expression.And(o => o.SchoolId == schoolId);
            else if (schoolName != null && schoolName.Trim() != string.Empty)
                expression = expression.And(o => o.School.Name.Contains(schoolName.Trim()));
            if (classroomId >= 1)
                expression = expression.And(o => o.ClassroomClasses.Select(r => classroomId).Contains(classroomId));
            if (classId != null && classId.Trim() != string.Empty)
                expression = expression.And(o => o.ClassId.Contains(classId.Trim()));
            if (className != null && className.Trim() != string.Empty)
                expression = expression.And(o => o.Name.Contains(className.Trim()));
            if (status >= 0)
                expression = expression.And(o => (int)o.Status == status);
            var list = _classBusiness.SearchClasses(UserInfo, expression, sort, order, first, count, out total);

            var result = new { total = total, data = list };
            return JsonHelper.SerializeObject(result);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.Class_Management, Anonymity = Anonymous.Verified)]
        public string GetClassId(int communityId = 0, int schoolId = 0, int classroomId = 0)
        {
            var expression = PredicateHelper.True<ClassEntity>();
            //TODO:School CommunityId DEL
            //if (communityId > 0)
            //    expression = expression.And(r => r.School.CommunityId == communityId);                  // expression = expression.And(o => o.CommunityId == communityId);
            if (schoolId > 0)
                expression = expression.And(o => o.SchoolId == schoolId);
            if (classroomId > 0)
                expression = expression.And(o => o.ClassroomClasses.Select(r => classroomId).Contains(classroomId));
            var list = _classBusiness.GetClassId(expression);
            return JsonHelper.SerializeObject(list);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.Administrative, Anonymity = Anonymous.Logined)]
        public string GetClassName(int communityId = 0, int schoolId = 0, int classroomId = 0)
        {
            var expression = PredicateHelper.True<ClassEntity>();
            //TODO:School CommunityId DEL
            //if (communityId > 0)
            //    expression = expression.And(r => r.School.CommunityId == communityId);         // expression = expression.And(o => o.CommunityId == communityId);
            if (schoolId > 0)
                expression = expression.And(o => o.SchoolId == schoolId);
            if (classroomId > 0)
                expression = expression.And(o => o.ClassroomClasses.Select(r => r.ClassroomId).Contains(classroomId));
            var list = _classBusiness.GetClassSelectList(UserInfo, expression);
            return JsonHelper.SerializeObject(list);
        }
        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.Administrative, Anonymity = Anonymous.Logined)]
        public string GetClassNameForBES(int communityId = 0, int schoolId = 0, int classroomId = 0)
        {
            var expression = PredicateHelper.True<ClassEntity>();
            //TODO:School CommunityId DEL
            //if (communityId > 0)
            //    expression = expression.And(r => r.School.CommunityId == communityId);         // expression = expression.And(o => o.CommunityId == communityId);
            if (schoolId > 0)
                expression = expression.And(o => o.SchoolId == schoolId);
            if (classroomId > 0)
                expression = expression.And(o => o.ClassroomClasses.Select(r => r.ClassroomId).Contains(classroomId));
            var list = _classBusiness.GetClassSelectListByCreatedDESC(UserInfo, expression);
            return JsonHelper.SerializeObject(list);
        }
        #endregion

        #region Private Function
        /// <summary>
        /// generate ddl
        /// </summary>
        /// <param name="list"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        private IEnumerable<SelectListItem> ListToDDL(IEnumerable<object> list, string defaultValue = "")
        {
            return new SelectList(list, "ID", "Name").AddDefaultItem(ViewTextHelper.DefaultPleaseSelectText, defaultValue);
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
            bool accessAssignCommunitySpecialists = false;
            if (UserInfo != null)
            {
                UserAuthorityModel userAuthority = new PermissionBusiness().GetUserAuthority(UserInfo, (int)PagesModel.Class_Management);

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
                        if (UserInfo.Role == Role.Super_admin || UserInfo.Role == Role.Principal || UserInfo.Role == Role.Principal_Delegate)
                            accessAssignCommunitySpecialists = true;
                    }
                }
            }
            ViewBag.accessAdd = accessAdd;
            ViewBag.accessEdit = accessEdit;
            ViewBag.accessView = accessView;
            ViewBag.accessAssign = accessAssign;
            //ViewBag.accessAssignCommunitySpecialists = accessAssignCommunitySpecialists;
            ViewBag.AssignUserAccess = ((accessAssign && (UserInfo.Role == Role.Principal)) || UserInfo.Role == Role.Super_admin);
        }

        private void InitParameters()
        {
            ViewBag.CurriculumOptions = ListToDDL(_masterDataBusiness.GetCurriculumSelectList(),"0");
            ViewBag.MonitoringToolOptions = ListToDDL(_classBusiness.GetMonitoringToolSelectList());
            ViewBag.StatusOptions = EntityStatus.Active.ToSelectList();
            ViewBag.LanguageOptions = new SelectList(_masterDataBusiness.GetLanguageSelectList(), "ID", "Name");
            var classLevelList = _classBusiness.GetClassLevels();
            classLevelList.Insert(0,new ClassLevelEntity(){ID = 0,CreatedOn = DateTime.Now,Status = EntityStatus.Active,Name="Please select",UpdatedOn = DateTime.Now});
            ViewBag.ClassLevelOptions = new SelectList(classLevelList, "ID", "Name");

        }

        private void InitViewParameters(ClassEntity entity)
        {
            if (entity.Curriculum != null)
                ViewBag.Curriculum = entity.Curriculum.Name;
            if (entity.SupplementalCurriculum != null)
                ViewBag.SupplementalCurriculum = entity.SupplementalCurriculum.Name;
            if (entity.MonitoringTool != null)
                ViewBag.MonitoringTool = entity.MonitoringTool.Name;

            ViewBag.Language = entity.Languages.Select(o => o.Language);


            ViewBag.showNotes = true;
            switch (UserInfo.Role)
            {
                case Role.Community:
                case Role.District_Community_Delegate:
                case Role.District_Community_Specialist:
                case Role.Community_Specialist_Delegate:
                case Role.TRS_Specialist:
                case Role.TRS_Specialist_Delegate:
                case Role.School_Specialist:
                case Role.School_Specialist_Delegate:
                case Role.Principal:
                case Role.Principal_Delegate:
                case Role.Teacher:
                    ViewBag.showNotes = false;
                    break;
            }

        }
        #endregion

        #region Class Classroom  Relation

        [CLIUrlAuthorizeAttribute(Account = Authority.Assign, PageId = PagesModel.Class_Management, Anonymity = Anonymous.Verified)]
        public ActionResult AssignClassroom(int classId = 0)
        {
            ClassEntity classEntity = _classBusiness.GetClass(classId);
            if (classEntity.Status == EntityStatus.Inactive)
            {
                if (Request.QueryString["returnurl"] == null)
                    return View("index");
                else
                    return new RedirectResult(Request.QueryString["returnurl"]);
            }
            return View(classEntity);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.Assign, PageId = PagesModel.Class_Management, Anonymity = Anonymous.Verified)]
        public string SearchUnAssignedClassroom(int schoolId = 0, int classId = 0, string name = "",
            string sort = "Name", string order = "ASC", int first = 0, int count = 10)
        {
            int total = 0;
            var list = _classBusiness.GetUnAssignClassroom(UserInfo, schoolId, classId, name, sort, order, first, count, out total);
            var result = new { total = total, data = list };
            string s = JsonHelper.SerializeObject(result);
            return s;
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.Assign, PageId = PagesModel.Class_Management, Anonymity = Anonymous.Verified)]
        public string SearchAssignedClassroom(int classId = 0, string name = "",
            string sort = "Name", string order = "ASC", int first = 0, int count = 10)
        {
            int total = 0;
            var list = _classBusiness.GetAssignedClassroom(UserInfo,
                o => (o.ClassId == classId
                    && (o.Classroom.Name.Contains(name) || name.Trim() == "")), sort, order, first, count, out total);
            var result = new { total = total, data = list };
            return JsonHelper.SerializeObject(result);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.Assign, PageId = PagesModel.Class_Management, Anonymity = Anonymous.Verified)]
        public string AssignRelations(int classId, int[] classroomIds)
        {
            var response = new PostFormResponse();

            OperationResult result = new OperationResult(OperationResultType.Success);
            ClassEntity classEntity = _classBusiness.GetClass(classId);
            if (classEntity.Status == EntityStatus.Inactive)
            {
                response.Success = false;
                response.Message = GetInformation("HasInactive").Replace("{Entity}", "Class");
                return JsonHelper.SerializeObject(response);
            }
            result = _classBusiness.InserClassroomClassRelations(classId, classroomIds);
            response.Success = result.ResultType == OperationResultType.Success;
            response.Data = "";
            response.Message = result.Message;
            return JsonHelper.SerializeObject(response);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.Assign, PageId = PagesModel.Class_Management, Anonymity = Anonymous.Verified)]
        public string DeleteRelations(int classId, int[] classroomIds)
        {
            var response = new PostFormResponse();
            OperationResult result = new OperationResult(OperationResultType.Success);
            result = _classBusiness.DeleteClassroomClassRelations(classId, classroomIds);
            response.Success = result.ResultType == OperationResultType.Success;
            response.Data = "";
            response.Message = result.Message;
            return JsonHelper.SerializeObject(response);
        }
        #endregion
    }
}
