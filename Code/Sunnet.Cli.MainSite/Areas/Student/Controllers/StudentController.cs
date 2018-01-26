using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
/**************************************************************************
 * Developer: 		Damon
 * Computer:		Damon-PC
 * Domain:			Damon-pc
 * CreatedOn:		2014/8/31 18:54:23
 * Description:		Please input class summary
 * Version History:	Created,2014/8/31 18:54:23
 **************************************************************************/
using Newtonsoft.Json;
using Sunnet.Cli.Business;
using Sunnet.Cli.Business.Ade;
using Sunnet.Cli.Business.Classes;
using Sunnet.Cli.Business.Common;
using Sunnet.Cli.Business.Communities;
using Sunnet.Cli.Business.Cpalls;
using Sunnet.Cli.Business.Cpalls.Models;
using Sunnet.Cli.Business.Cpalls.Models.Report;
using Sunnet.Cli.Business.Students;
using Sunnet.Cli.Business.Students.Models;
using Sunnet.Cli.Business.Users;
using Sunnet.Cli.Core.Ade;
using Sunnet.Cli.Core.Classes.Entites;
using Sunnet.Cli.Core.Common.Enums;
using Sunnet.Cli.Core.Cpalls;
using Sunnet.Cli.Core.Students.Entities;
using Sunnet.Cli.Core.Students.Enums;
using Sunnet.Cli.Core.Users.Entities;
using Sunnet.Cli.Core.Users.Enums;
using Sunnet.Cli.MainSite.Areas.Student.Models;
using Sunnet.Cli.MainSite.Controllers;
using Sunnet.Cli.UIBase.Models;
using Sunnet.Framework.Core.Tool;
using Sunnet.Framework.Extensions;
using Sunnet.Framework.Helpers;
using Sunnet.Framework.PDF;
using Sunnet.Cli.UIBase.Filters;
using Sunnet.Framework.Permission;
using Sunnet.Cli.Core.Permission.Entities;
using Sunnet.Cli.Business.Permission;
using Sunnet.Cli.Business.Permission.Models;
using Sunnet.Cli.Core.Schools.Entities;
using Sunnet.Cli.Business.Schools;

namespace Sunnet.Cli.MainSite.Areas.Student.Controllers
{
    public class StudentController : BaseController
    {

        private readonly StudentBusiness _studentBusiness;
        private readonly UserBusiness _userBusiness;
        private readonly ClassBusiness _classBusiness;
        private UserBusiness userBusiness;
        private readonly SchoolBusiness _schoolBusiness;
        private readonly AdeBusiness _adeBusiness;
        private CpallsBusiness _cpallsBusiness;
        public StudentController()
        {
            _studentBusiness = new StudentBusiness(UnitWorkContext);
            _userBusiness = new UserBusiness(UnitWorkContext);
            _classBusiness = new ClassBusiness(UnitWorkContext);
            userBusiness = new UserBusiness(UnitWorkContext);
            _schoolBusiness = new SchoolBusiness(UnitWorkContext);
            _adeBusiness = new AdeBusiness(AdeUnitWorkContext);
            _cpallsBusiness = new CpallsBusiness(AdeUnitWorkContext);
        }

        // GET: /Student/Student/
        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.Student_Management, Anonymity = Anonymous.Verified)]
        public ActionResult Index()
        {
            InitAccessOperation();
            ViewBag.ClassesOptions = ListToDDL(_classBusiness.GetClassSelectList(UserInfo, o => true), ViewTextHelper.DefaultAllText, "-1");
            ViewBag.StatusOptions = EntityStatus.Active.ToSelectList(true, new SelectOptions(true, "-1", ViewTextHelper.DefaultAllText));

            return View();
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.Administrative, Anonymity = Anonymous.Logined)]
        public String GetClassSelectList(int communityId = 0, int schoolId = 0)
        {
            var expression = PredicateHelper.True<ClassEntity>();
            if (communityId > 0)
                expression = expression.And(o => o.School
                    .CommunitySchoolRelations.Count(r => r.CommunityId == communityId) > 0);
            if (schoolId > 0)
                expression = expression.And(o => o.SchoolId == schoolId);
            return JsonHelper.SerializeObject(
                ListToDDL(_classBusiness.GetClassSelectList(UserInfo, expression), ViewTextHelper.DefaultAllText, "-1"));
        }
        // GET: /Student/Student/New

        [CLIUrlAuthorizeAttribute(Account = Authority.Add, PageId = PagesModel.Student_Management, Anonymity = Anonymous.Verified)]
        public ActionResult New()
        {
            InitAccessOperation();
            ViewBag.StatusOptions = EntityStatus.Active.ToSelectList(true);
            ViewBag.Language = _userBusiness.GetLanguages().ToSelectList(ViewTextHelper.DefaultPleaseSelectText, "0");
            ViewBag.AssessmentLanguages = StudentAssessmentLanguage.English.ToSelectList(true, new SelectOptions(true, "", ViewTextHelper.DefaultPleaseSelectText));
            ViewBag.GradeLevelOptions = StudentGradeLevel.Prek.ToSelectList(true, new SelectOptions(true, "", ViewTextHelper.DefaultPleaseSelectText));
            ViewBag.NoteAccess = NoteAccess();
            ViewBag.MediaAccess = MediaAccess();
            ViewBag.Role = UserInfo.Role;
            StudentModel student = _studentBusiness.NewStudentModel();
            return View(student);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.Add, PageId = PagesModel.Student_Management, Anonymity = Anonymous.Logined)]
        public string IsStudentExist(string firstName, string lastName, DateTime birthDate, int communityId)
        {
            return _studentBusiness.IsStudentExist(firstName, lastName, birthDate, communityId).ToString().ToLower();
        }

        // POST: /Student/Student/New
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CLIUrlAuthorizeAttribute(Account = Authority.Add, PageId = PagesModel.Student_Management, Anonymity = Anonymous.Verified)]
        public string New(StudentModel studentModel, int[] chkClasses)
        {
            var response = new PostFormResponse();

            StudentEntity entity = _studentBusiness.GetEntityByModel(studentModel);
            OperationResult result = _studentBusiness.InsertStudent(UserInfo, entity, chkClasses);
            response.Success = result.ResultType == OperationResultType.Success;
            response.Message = result.Message;

            //add student school relation
            if (response.Success)
            {
                int schoolId = studentModel.SchoolId;
                List<int> studentIds = new List<int>() { entity.ID };
                _schoolBusiness.InserSchoolStudentRelations(studentIds, schoolId, UserInfo);
            }

            if (response.Success && studentModel.IsSendParent)
                GeneratePdf(entity.ID);
            return JsonConvert.SerializeObject(response);
        }

        // GET:/Student/Student/Edit
        [CLIUrlAuthorizeAttribute(Account = Authority.Edit, PageId = PagesModel.Student_Management, Anonymity = Anonymous.Verified)]
        public ActionResult Edit(int id)
        {
            InitAccessOperation();
            ViewBag.Language = new UserBusiness().GetLanguages().ToSelectList(ViewTextHelper.DefaultPleaseSelectText, "0");
            ViewBag.StatusOptions = EntityStatus.Active.ToSelectList(true);
            ViewBag.AssessmentLanguages = StudentAssessmentLanguage.English.ToSelectList(true, new SelectOptions(true, "", ViewTextHelper.DefaultPleaseSelectText));
            ViewBag.GradeLevelOptions = StudentGradeLevel.Prek.ToSelectList(true, new SelectOptions(true, "", ViewTextHelper.DefaultPleaseSelectText));
            StudentModel studentModel = _studentBusiness.GetStudent(id, UserInfo);

            if (studentModel != null)
            {
                studentModel.IsSendParent = false;//不记住状态


            }
            ViewBag.NoteAccess = NoteAccess();
            ViewBag.MediaAccess = MediaAccess();
            return View(studentModel);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.View, PageId = PagesModel.Student_Management, Anonymity = Anonymous.Verified)]
        public ActionResult View(int id)
        {
            ViewBag.NoteAccess = NoteAccess();
            ViewBag.MediaAccess = MediaAccess();
            ViewBag.Role = UserInfo.Role;
            List<SelectItemModel> languageList = new UserBusiness().GetLanguages().ToList();
            ViewBag.StatusOptions = EntityStatus.Active.ToSelectList(true);
            ViewBag.AssessmentLanguages = StudentAssessmentLanguage.English.ToSelectList(true, new SelectOptions(true, "", ViewTextHelper.DefaultPleaseSelectText));
            StudentModel studentEntity = _studentBusiness.GetStudent(id, UserInfo);
            ViewBag.listClasses = JsonHelper.SerializeObject(studentEntity.Classes.Where(e => e.IsDeleted == false).Select(o =>
                                        new AssigenStudentClassModel
                                        {
                                            ClassId = o.ID,
                                            ClasroomNameList = o.ClassroomClasses.Select(r => r.Classroom.Name),
                                            ClassName = o.Name,
                                            DayType = o.DayType
                                        }));

            string primaryName =
                       languageList.Where(o => o.ID == studentEntity.PrimaryLanguageId)
                           .Select(o => o.Name)
                           .FirstOrDefault();
            if (primaryName != "Other")
                ViewBag.primaryLaguage = primaryName;
            else
                ViewBag.primaryLaguage = studentEntity.PrimaryLanguageOther;

            string secondaryName =
                 languageList.Where(o => o.ID == studentEntity.SecondaryLanguageId)
                     .Select(o => o.Name)
                     .FirstOrDefault();
            if (secondaryName != "Other")
                ViewBag.secondaryLaguage = secondaryName;
            else
                ViewBag.secondaryLaguage = studentEntity.SecondaryLanguageOther;

            return View(studentEntity);
        }

        private string MediaAccess()
        {
            switch (UserInfo.Role)
            {
                case Role.Super_admin:
                case Role.Statisticians:
                case Role.Content_personnel:
                case Role.Intervention_manager:
                case Role.Intervention_support_personnel:
                case Role.Administrative_personnel:
                    return "RW";
                default:
                    return "R";
            }
        }

        private string NoteAccess()
        {
            switch (UserInfo.Role)
            {
                case Role.Video_coding_analyst:
                    return "R";
                case Role.Statewide:
                case Role.Community:
                case Role.TRS_Specialist:
                case Role.District_Community_Specialist:
                case Role.Principal:
                case Role.Teacher:
                case Role.Parent:
                case Role.Auditor:
                    return "X";
                default:
                    return "RW";
            }
        }


        [CLIUrlAuthorizeAttribute(Account = Authority.ParentInvitation, PageId = PagesModel.Student_Management, Anonymity = Anonymous.Verified)]
        public ActionResult ParentInvitations()
        {
            InitAccessOperation();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CLIUrlAuthorizeAttribute(Account = Authority.Edit, PageId = PagesModel.Student_Management, Anonymity = Anonymous.Verified)]
        public string Edit(StudentModel studentModel, int[] chkClasses)
        {
            var response = new PostFormResponse();

            StudentEntity studentEntity = _studentBusiness.GetEntityByModel(studentModel);
            studentEntity.ID = studentModel.ID;
            if (ModelState.IsValid)
            {
                OperationResult result = _studentBusiness.UpdateStudent(UserInfo, studentEntity, chkClasses);
                response.Success = result.ResultType == OperationResultType.Success;
                response.Message = result.Message;
            }

            if (response.Success)
                _studentBusiness.CheckStuSchool(studentModel.ID);

            if (response.Success && studentEntity.IsSendParent)
                GeneratePdf(studentEntity.ID);
            return JsonConvert.SerializeObject(response);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.Student_Management, Anonymity = Anonymous.Verified)]
        [ValidateInput(false)]
        public string Search(int communityId = -1, string communityName = "", int schoolId = -1, string schoolName = "", int classId = -1,
            string studentId = "", string studentName = "", int status = -1,
           string sort = "Name", string order = "Asc", int first = 0, int count = 10)
        {
            var total = 0;
            var expression = PredicateHelper.True<StudentEntity>();
            if (communityId >= 1)
                expression = expression.And(s => s.SchoolRelations.Count
                    (r => r.School.CommunitySchoolRelations.Count
                        (c => c.CommunityId == communityId) > 0) > 0);
            else if (communityName != null && communityName.Trim() != string.Empty)
            {
                communityName = communityName.Trim();
                expression = expression.And(s => s.SchoolRelations
                    .Count(r => r.School.CommunitySchoolRelations
                        .Count(c => c.Community.Name.Contains(communityName)) > 0) > 0);
            }

            if (schoolId >= 1)
                expression = expression.And(s => s.SchoolRelations.Count(r => r.SchoolId == schoolId) > 0);
            else if (schoolName != null && schoolName.Trim() != string.Empty)
            {
                schoolName = schoolName.Trim();
                expression = expression.And(s => s.SchoolRelations
                    .Count(r => r.School.Name.Contains(schoolName)) > 0);
            }

            if (classId >= 1)
                expression = expression.And(s => s.Classes.Count(o => o.ID == classId && o.IsDeleted == false) > 0);

            if (studentId != null && studentId.Trim() != string.Empty)
            {
                studentId = studentId.Trim();
                expression = expression.And(s => s.StudentId.Contains(studentId));
            }

            if (studentName != null && studentName.Trim() != string.Empty)
            {
                studentName = studentName.Trim();
                expression = expression.And(s => s.FirstName.Contains(studentName)
                    || s.MiddleName.Contains(studentName)
                    || s.LastName.Contains(studentName));
            }

            if (status >= 1)
                expression = expression.And(s => (int)s.Status == status);

            var list = _studentBusiness.SearchStudents(UserInfo, expression, sort, order, first, count, out total);

            var result = new { total = total, data = list };
            return JsonHelper.SerializeObject(result);
        }


        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.Administrative, Anonymity = Anonymous.Logined)]
        public string ParentSearch(string sort = "Name", string order = "Asc", int first = 0, int count = 10)
        {
            var total = 0;
            var list = _studentBusiness.SearchStudents(UserInfo.Parent.ID, sort, order, first, count, out total);


            var result = new { total = total, data = list };
            return JsonHelper.SerializeObject(result);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.Student_Management, Anonymity = Anonymous.Verified)]
        public string GetClassesBySchoolId(int schoolId = -1)
        {
            List<int> schoolIds = new List<int>();
            schoolIds.Add(schoolId);
            var classList = _classBusiness.GetClassesBySchoolId(schoolIds, UserInfo);
            var result = new { total = 100, data = classList };
            return JsonHelper.SerializeObject(result);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.Student_Management, Anonymity = Anonymous.Verified)]
        public string GetClassesByStudentId(int studentId)
        {
            IList<int> classIds = _studentBusiness.GetClassIdsByStudentId(studentId);
            IList<int> schoolIds = _studentBusiness.GetSchoolIdsByStudentId(studentId);
            var classList = _classBusiness.GetClassesBySchoolIdClassId(schoolIds, classIds, UserInfo);
            var result = new { total = 100, data = classList };
            return JsonHelper.SerializeObject(result);
        }

        private IEnumerable<SelectListItem> ListToDDL(IEnumerable<SelectItemModel> list, string defaultText = "", string defaultValue = "")
        {
            if (defaultText == "") defaultText = ViewTextHelper.DefaultPleaseSelectText;
            return list.ToSelectList(defaultText, defaultValue);
        }

        private IEnumerable<SelectListItem> EnumToDDL(Enum eEnum, string defaultVal = "")
        {
            return eEnum.ToSelectList(true, new SelectOptions(true, defaultVal, ViewTextHelper.DefaultPleaseSelectText));
        }


        [CLIUrlAuthorizeAttribute(Account = Authority.ParentInvitation, PageId = PagesModel.Student_Management, Anonymity = Anonymous.Verified)]
        public string GeneratePdf(int id)
        {
            OperationResult result = _studentBusiness.GeneratePDF(id, UserInfo);
            return JsonConvert.SerializeObject(result);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.ParentInvitation, PageId = PagesModel.Student_Management, Anonymity = Anonymous.Verified)]
        public void ExportPdf(string studentList)
        {
            List<List<string>> list = JsonHelper.DeserializeObject<List<List<string>>>(studentList);
            _studentBusiness.GenerateListPDF(list);

        }

        /// <summary>
        /// 控制按钮操作是否显示
        /// </summary>
        private void InitAccessOperation()
        {
            bool accessAdd = false;
            bool accessEdit = false;
            bool accessView = false;
            bool accessParentInvitation = false;
            bool accessAddClass = false;
            bool accessBES = false;
            if (UserInfo != null)
            {
                UserAuthorityModel userAuthority = new PermissionBusiness().GetUserAuthority(UserInfo, (int)PagesModel.Student_Management);
                UserAuthorityModel rosterAuthority = new PermissionBusiness().GetUserAuthority(UserInfo, (int)PagesModel.ClassRoster);
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
                    if ((userAuthority.Authority & (int)Authority.ParentInvitation) == (int)Authority.ParentInvitation)
                    {
                        accessParentInvitation = true;
                    }
                    if (rosterAuthority != null)
                    {
                        if ((rosterAuthority.Authority & (int)Authority.Index) == (int)Authority.Index)
                        {
                            accessBES = true;
                        }
                    }
                }

                userAuthority = new PermissionBusiness().GetUserAuthority(UserInfo, (int)PagesModel.Class_Management);
                if (userAuthority != null)
                {
                    if ((userAuthority.Authority & (int)Authority.Add) == (int)Authority.Add)
                    {
                        accessAddClass = true;
                    }
                }
            }
            ViewBag.accessAdd = accessAdd;
            ViewBag.accessEdit = accessEdit;
            ViewBag.accessView = accessView;
            ViewBag.accessParentInvitation = accessParentInvitation;
            ViewBag.AddClass = accessAddClass;
            ViewBag.accessBES = accessBES;
        }


        #region My Children
        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.Administrative, Anonymity = Anonymous.Logined)]
        public ActionResult ProfileIndex()
        {
            var assessmentList = _adeBusiness.GetAssessmentCpallsList();

            foreach (var model in assessmentList)
            {
                model.DisplayName = model.Name;
                if (assessmentList.Count(c => c.Name == model.Name) > 1)
                {
                    if ((int)model.Language > 0)
                    {
                        model.DisplayName = model.Name + "(" + model.Language.ToString() + ")";
                    }
                }
            }
            ViewBag.assessmentList = assessmentList;
            return View();
        }


        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.Administrative, Anonymity = Anonymous.Logined)]
        public ActionResult AddChild()
        {
            ParentChildModel parentChild = new ParentChildModel();
            parentChild.ChildLastName = UserInfo.LastName;
            ViewBag.RelationList = ParentRelation.Father.ToSelectList();
            return View(parentChild);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.Administrative, Anonymity = Anonymous.Logined)]
        public string SaveParentChild(ParentChildModel parentChild)
        {
            var response = new PostFormResponse();

            if (ModelState.IsValid)
            {
                //通过parentid、firstname、lastname查找student实体
                StudentEntity student = _studentBusiness.GetStudentByCode(parentChild.ParentId, parentChild.ChildFirstName, parentChild.ChildLastName);
                if (student != null)
                {
                    //是否已经存在该关系
                    bool ifexist = _userBusiness.IsExistStudent(UserInfo.Parent.ID, student.ID);
                    if (!ifexist) //不存在时
                    {
                        ParentStudentRelationEntity parentStudent = new ParentStudentRelationEntity();
                        parentStudent.ParentId = UserInfo.Parent.ID;
                        parentStudent.StudentId = student.ID;
                        parentStudent.Relation = parentChild.Relation;
                        parentStudent.RelationOther = parentChild.RelationOther;
                        OperationResult result = _userBusiness.InsertParentStudentRelation(parentStudent);
                        response.Success = result.ResultType == OperationResultType.Success;
                        response.Message = result.Message;
                    }
                    else
                    {
                        response.Success = false;
                        response.Message = "The student has existed in your list!";
                    }
                }
                else
                {
                    response.Success = false;
                    response.Message = "I'm sorry! The student does not exist!";
                }
            }
            return JsonConvert.SerializeObject(response);
        }


        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.Administrative, Anonymity = Anonymous.Logined)]
        public ActionResult ProfileView(int id)
        {
            ViewBag.NoteAccess = NoteAccess();
            ViewBag.MediaAccess = MediaAccess();
            ViewBag.Role = UserInfo.Role;
            List<SelectItemModel> languageList = new UserBusiness().GetLanguages().ToList();
            ViewBag.StatusOptions = EntityStatus.Active.ToSelectList(true);
            ViewBag.AssessmentLanguages = StudentAssessmentLanguage.English.ToSelectList(true, new SelectOptions(true, "", ViewTextHelper.DefaultPleaseSelectText));
            ViewBag.GradeLevelOptions = StudentGradeLevel.Prek.ToSelectList(true, new SelectOptions(true, "", ViewTextHelper.DefaultPleaseSelectText));
            StudentModel studentEntity = _studentBusiness.GetStudent(id, null);
            ViewBag.listClasses = JsonHelper.SerializeObject(studentEntity.Classes.Where(e => e.IsDeleted == false).Select(o =>
                                        new
                                        {
                                            ClassId = o.ID,
                                            //TODO:Classroom and Class has many to many
                                            //ClassroomName = o.Classroom.Name,
                                            ClassName = o.Name,
                                            DayType = (o.DayType == 0 ? "" : o.DayType.ToDescription())
                                        }));

            string primaryName =
                       languageList.Where(o => o.ID == studentEntity.PrimaryLanguageId)
                           .Select(o => o.Name)
                           .FirstOrDefault();
            if (primaryName != "Other")
                ViewBag.primaryLaguage = primaryName;
            else
                ViewBag.primaryLaguage = studentEntity.PrimaryLanguageOther;

            string secondaryName =
                 languageList.Where(o => o.ID == studentEntity.SecondaryLanguageId)
                     .Select(o => o.Name)
                     .FirstOrDefault();
            if (secondaryName != "Other")
                ViewBag.secondaryLaguage = secondaryName;
            else
                ViewBag.secondaryLaguage = studentEntity.SecondaryLanguageOther;

            return View(studentEntity);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.Administrative, Anonymity = Anonymous.Logined)]
        public ActionResult StudentSummaryReport(int assessmentId, int id)
        {
            var classId = 0;
            var classList = _studentBusiness.GetClassIdsByStudentId(id);
            if (classList != null)
            {
                classId = classList.FirstOrDefault();
            }

            var year = CommonAgent.Year;
            var assessment = _adeBusiness.GetBaseAssessmentModel(assessmentId);
            if (assessment == null) return View("NoData");
            ViewBag.AssessmentName = assessment.Name;
            var class1 = _classBusiness.GetClassForCpalls(classId);

            ViewBag.Title = "Class Scores";
            ViewBag.District = class1.School.CommunitiesText;
            ViewBag.School = class1.School.Name;
            ViewBag.Class = class1.ClassName;
            ViewBag.Teacher = string.Join(", ", _classBusiness.GetTeachers(classId));
            ViewBag.Language = assessment.Language.ToDescription();
            ViewBag.ScoolYear = year.ToFullSchoolYearString();
            ViewBag.ReportFirstColumn = "Student";

            ViewBag.Student = "";
            var waveMeasures = WaveMesures(assessmentId, year);
            var studentEntity = _studentBusiness.GetStudentEntity(id, UserInfo);
            if (studentEntity != null)
            {
                ViewBag.Student = studentEntity.FirstName + " " + studentEntity.LastName;
                Dictionary<object, List<ReportRowModel>> datas = _cpallsBusiness.GetStudentReport(assessmentId, studentEntity.AssessmentLanguage,
              true, UserInfo, year, class1.School.ID, classId, id, waveMeasures, null, null);
                ViewBag.Datas = datas.ToDictionary(x => (int)x.Key, x => x.Value);
            }

            ViewBag.Pdf = true;
            string startPdfFilePath = assessment.Language == AssessmentLanguage.English ? "~/resource/Pdf/Parent_Report_English.pdf" : "~/resource/Pdf/Parent_Report_SPA.pdf";
            GetPdf(GetViewHtml("StudentSummaryReport"), "Student_Summary", PdfType.Assessment_Portrait, startPdfFilePath);
            return View();
        }

        public Dictionary<Wave, IEnumerable<int>> WaveMesures(int assessmentId, int year)
        {
            var list = new Dictionary<Wave, IEnumerable<int>>();
            List<MeasureHeaderModel> measures;
            List<MeasureHeaderModel> parentMeasures;
            _cpallsBusiness.BuilderHeader(assessmentId, year, Wave.BOY, out measures, out parentMeasures, true);

            foreach (Wave v in Wave.BOY.ToList())
            {
                var value = Convert.ToInt32(v);
                var items = new List<int>();


                items = measures.Where(o => o.ApplyToWave != null && o.ApplyToWave.Contains((value).ToString())).Select(c => c.MeasureId).ToList();
                list.Add(v, items);
            }
            return list;
        }
        private void GetPdf(string html, string fileName, PdfType type = PdfType.Assessment_Landscape, string startPdfFilePath = "")
        {
            startPdfFilePath = Server.MapPath(startPdfFilePath);
            PdfProvider pdfProvider = new PdfProvider(type, "", startPdfFilePath);
            pdfProvider.GeneratePDF(html, fileName);
        }
        private string GetViewHtml(string viewName)
        {
            ViewBag.Pdf = true;
            var resultHtml = "";
            ViewEngineResult result = ViewEngines.Engines.FindView(ControllerContext, viewName, null);
            if (null == result.View)
            {
                throw new InvalidOperationException(FormatErrorMessage(viewName, result.SearchedLocations));
            }
            try
            {
                ViewContext viewContext = new ViewContext(ControllerContext, result.View, this.ViewData, this.TempData,
                    Response.Output);
                var textWriter = new StringWriter();
                result.View.Render(viewContext, textWriter);
                resultHtml = textWriter.ToString();
            }
            finally
            {
                result.ViewEngine.ReleaseView(ControllerContext, result.View);
            }
            return resultHtml;
        }
        private string FormatErrorMessage(string viewName, IEnumerable<string> searchedLocations)
        {
            string format =
                "The view '{0}' or its master was not found or no view engine supports the searched locations. The following locations were searched:{1}";
            StringBuilder builder = new StringBuilder();
            foreach (string str in searchedLocations)
            {
                builder.AppendLine();
                builder.Append(str);
            }
            return string.Format(CultureInfo.CurrentCulture, format, viewName, builder);
        }



        #endregion
    }
}
