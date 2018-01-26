using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Net.Configuration;
using System.Web;
using System.Web.Mvc;
using Sunnet.Cli.Business.Classes;
using Sunnet.Cli.Business.Classrooms;
using Sunnet.Cli.Business.Common;
using Sunnet.Cli.Business.Communities;
using Sunnet.Cli.Business.Permission;
using Sunnet.Cli.Business.Permission.Models;
using Sunnet.Cli.Business.Students;
using Sunnet.Cli.Business.Users;
using Sunnet.Cli.Business.Users.Models;
using Sunnet.Cli.Core.Ade;
using Sunnet.Cli.Core.Classes.Entites;
using Sunnet.Cli.Core.Classes.Enums;
using Sunnet.Cli.Core.Classrooms.Entites;
using Sunnet.Cli.Core.Common.Enums;
using Sunnet.Cli.Core.Schools.Entities;
using Sunnet.Cli.Core.Students.Entities;
using Sunnet.Cli.Core.Students.Enums;
using Sunnet.Cli.Core.Users.Entities;
using Sunnet.Cli.Core.Users.Enums;
using Sunnet.Cli.MainSite.Controllers;
using Sunnet.Cli.UIBase;
using Sunnet.Cli.UIBase.Models;
using Sunnet.Framework.Core.Tool;
using Sunnet.Framework.Extensions;
using Sunnet.Framework.Helpers;
using Sunnet.Cli.UIBase.Filters;
using Sunnet.Framework.Permission;
using Sunnet.Cli.Business.Schools;

namespace Sunnet.Cli.MainSite.Areas.Roster.Controllers
{
    public class IndexController : BaseController
    {
        private readonly ClassroomBusiness classroomBusiness;

        private readonly CommunityBusiness communityBusiness;
        private readonly ClassBusiness classBusiness;
        private readonly StudentBusiness studentBusiness;
        private readonly UserBusiness userBusiness;
        private readonly SchoolBusiness schoolBusiness;

        public string SortStr = "";
        public string OrderDesc = "";
        public IndexController()
        {
            classroomBusiness = new ClassroomBusiness(UnitWorkContext);
            communityBusiness = new CommunityBusiness(UnitWorkContext);
            classBusiness = new ClassBusiness(UnitWorkContext);
            userBusiness = new UserBusiness(UnitWorkContext);
            studentBusiness = new StudentBusiness(UnitWorkContext);
            schoolBusiness = new SchoolBusiness(UnitWorkContext);
        }
        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.ClassRoster, Anonymity = Anonymous.Verified)]
        public ActionResult Index()
        {
            ViewBag.DayTypeOptions = DayType.Am.ToSelectList(true).AddDefaultItem(ViewTextHelper.DefaultAllText, 0);
            ViewBag.GradeLevelJson = JsonHelper.SerializeObject(StudentGradeLevel.Prek.ToList());
            ViewBag.EthnicityJson = JsonHelper.SerializeObject(Ethnicity.African_American.ToList());
            ViewBag.GenderJson = JsonHelper.SerializeObject(Gender.Female.ToList());
            ViewBag.AssessmentLanguageJson = JsonHelper.SerializeObject(StudentAssessmentLanguage.Bilingual.ToList());
            ViewBag.StatusJson = JsonHelper.SerializeObject(EntityStatus.Active.ToList());
            ViewBag.MinDate = DateTime.Now.AddYears(-12).ToString("MM/dd/yyyy");
            ViewBag.MaxDate = DateTime.Now.AddYears(-2).ToString("MM/dd/yyyy");
            ViewBag.CommunityId = 0;
            ViewBag.CommunityName = "";
            ViewBag.SchoolId = 0;
            ViewBag.SchoolName = "";
            var accessAddClass = false;
            UserAuthorityModel userAuthority = new PermissionBusiness().GetUserAuthority(UserInfo, (int)PagesModel.Class_Management);
            if (userAuthority != null)
            {
                if ((userAuthority.Authority & (int)Authority.Add) == (int)Authority.Add)
                {
                    accessAddClass = true;
                }
            }
            ViewBag.accessAddClass = accessAddClass;
            return View();
        }
        [CLIUrlAuthorizeAttribute(Account = Authority.Bes, PageId = PagesModel.ClassRoster, Anonymity = Anonymous.Verified)]
        public ActionResult UploadStudents(int comId = 0, int schoolId = 0, int teacherId = 0, int classId = 0, int classDayType = 0)
        {
            ViewBag.comId = comId;
            ViewBag.schoolId = schoolId;
            ViewBag.teacherId = teacherId;
            ViewBag.classId = classId;
            ViewBag.classDayType = classDayType;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CLIUrlAuthorizeAttribute(Account = Authority.Bes, PageId = PagesModel.ClassRoster, Anonymity = Anonymous.Verified)]
        public string UploadStudents()
        {
            int communityId = 0;
            int schoolId = 0;
            int teacherId = 0;
            int classId = 0;
            bool confirm = false;
            DayType classDayType = new DayType();
            int.TryParse(Request.Params["communityId"], out communityId);
            int.TryParse(Request.Params["teacherId"], out teacherId);
            int.TryParse(Request.Params["schoolId"], out schoolId);
            int.TryParse(Request.Params["classId"], out classId);

            bool.TryParse(Request.Params["confirm"], out confirm);

            DayType.TryParse(Request.Params["classDayType"], out classDayType);
            HttpPostedFileBase postFileBase = Request.Files["dataFile"];
            var response = new PostFormResponse();
            OperationResult res = Upload(communityId, schoolId, teacherId, classId, classDayType, postFileBase, confirm);
            if (res.ResultType == OperationResultType.Warning)
            {
                // 存在其他角色的用户提醒 Continue
                response.Success = true;
                response.Message = res.Message;
                response.Data = new
                {
                    type = "continue"
                };
            }
            else
            {
                response.Success = res.ResultType == OperationResultType.Success;
                response.Message = res.Message;
            }

            return JsonHelper.SerializeObject(response);
        }

        public string GetTeacherName(int classId)
        {

            var teacher = new TeacherModel();
            var findTeacher = new TeacherEntity();
            if (classId > 0)
            {
                ClassEntity findClass = classBusiness.GetClass(classId);
                if (findClass != null)
                {
                    findTeacher = userBusiness.GetTeacher(findClass.LeadTeacherId, UserInfo);
                }
                if (findTeacher != null)
                {
                    teacher.Id = findTeacher.ID;
                    teacher.FirstName = findTeacher.UserInfo.FirstName;
                    teacher.LastName = findTeacher.UserInfo.LastName;

                }
            }
            return JsonHelper.SerializeObject(teacher);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.ClassRoster, Anonymity = Anonymous.Verified)]
        public string Teachers(string keyword, int communityId = 0, int schoolId = 0)
        {
            List<SelectItemModel> items = new List<SelectItemModel>();
            if (communityId > 0 && schoolId > 0)
            {
                items =
               userBusiness.GetTeachers(
                   x => (x.UserInfo.UserCommunitySchools.Any(o => o.CommunityId == communityId))
                       && (x.UserInfo.UserCommunitySchools.Any(p => p.SchoolId == schoolId))
                       && x.UserInfo.Status == EntityStatus.Active
                       && (keyword == "-1" || x.UserInfo.FirstName.Contains(keyword) || x.UserInfo.LastName.Contains(keyword)));
            }
            else if (schoolId > 0)
            {
                items =
               userBusiness.GetTeachers(
                   x => (schoolId == 0 || x.UserInfo.UserCommunitySchools.Any(p => p.SchoolId == schoolId))
                       && x.UserInfo.Status == EntityStatus.Active
                       && (keyword == "-1" || x.UserInfo.FirstName.Contains(keyword) || x.UserInfo.LastName.Contains(keyword)));
            }
            else if (communityId > 0)
            {
                items =
               userBusiness.GetTeachers(
                   x => (x.UserInfo.UserCommunitySchools.Any(o => o.CommunityId == communityId))
                       && x.UserInfo.Status == EntityStatus.Active
                       && (keyword == "-1" || x.UserInfo.FirstName.Contains(keyword) || x.UserInfo.LastName.Contains(keyword)));
            }
            return JsonHelper.SerializeObject(items);
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

        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.ClassRoster, Anonymity = Anonymous.Verified)]
        public string Search(int communityId = 0, int schoolId = 0, int teacherId = 0, DayType classDayType = (DayType)0, int classId = 0, string className = "", string sort = "Name", string order = "Asc", int first = 0, int count = 10)
        {
            var condition = PredicateHelper.True<StudentEntity>();
            int total = 0;
            if (communityId > 0)
                condition = condition.And(s => s.SchoolRelations.Count
                    (r => r.School.CommunitySchoolRelations.Count
                        (c => c.CommunityId == communityId) > 0) > 0);
            if (schoolId > 0)
                condition = condition.And(s => s.SchoolRelations.Count(r => r.SchoolId == schoolId) > 0);

            if (teacherId > 0)
                condition = condition.And(o => o.Classes.Any(c => c.LeadTeacherId == teacherId && c.IsDeleted == false));
            if ((int)classDayType != 0)
                condition = condition.And(o => o.Classes.Any(c => c.DayType == classDayType && c.IsDeleted == false));
            if (classId > 0)
                condition = condition.And(o => o.Classes.Any(c => c.ID == classId && c.IsDeleted == false));

            var list = studentBusiness.SearchStudents(UserInfo, condition, sort, order, first, count, out total);
            var result = new { total = total, data = list };
            return JsonHelper.SerializeObject(result);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.ClassRoster, Anonymity = Anonymous.Verified)]
        public string Save(int communityId, int schoolId, int classId, string students)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            var listStu = JsonHelper.DeserializeObject<List<StudentEntity>>(students);
            var response = new PostFormResponse();
            ClassroomEntity classroom = null;
            ClassEntity findClass = null;
            TeacherEntity teacher = null;
            //if (teacherId > 0)
            //    teacher = userBusiness.GetTeacher(teacherId, UserInfo);
            if (classId > 0)
                findClass = classBusiness.GetClass(classId);

            if (classId <= 0)
            {
                result.ResultType = OperationResultType.Error;
                result.Message = "Class is required.";
            }
            // Check students
            result = CheckStudents(communityId, listStu, true);
            if (result.ResultType != OperationResultType.Success)
            {
                response.Success = result.ResultType == OperationResultType.Success;
                response.OtherMsg = "duplicate";

                response.Message = result.Message;
                return JsonHelper.SerializeObject(response);
            }
            if (result.ResultType == OperationResultType.Success)
            {
                var chkClass = new[] { findClass.ID };
                foreach (StudentEntity entity in listStu)
                {
                    try
                    {
                        result = SaveStudent(schoolId, findClass.ID, entity);
                        if (result.ResultType != OperationResultType.Success)
                            break;
                    }
                    catch (Exception ex)
                    {
                        result.ResultType = OperationResultType.Error;
                        result.Message += "error: " + entity.FirstName + " " + entity.LastName + " : " + ex.Message;
                        break;
                    }
                }
            }
            response.Success = result.ResultType == OperationResultType.Success;
            response.Message = result.Message;
            return JsonHelper.SerializeObject(response);
        }

        private OperationResult SaveStudent(int schoolId, int classId, StudentEntity entity)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            var chkClass = new[] { classId };
            if (entity.ID <= 0)
            {
                entity.SchoolYear = CommonAgent.SchoolYear;
                entity.StatusDate = DateTime.Now;
                entity.IsMediaRelease = MediaRelease.No;
                entity.Notes = "";
                entity.StudentId = string.Empty;
                entity.EthnicityOther = "";

                // entity.LocalStudentID = "";
                entity.ParentCode = "";
                entity.PrimaryLanguageOther = "";
                entity.SecondaryLanguageOther = "";
                entity.TSDSStudentID = "";
                result = studentBusiness.InsertStudent(UserInfo, entity, chkClass);
                if (result.ResultType == OperationResultType.Success)
                {
                    result = schoolBusiness.InserSchoolStudentRelations(new List<int>() { entity.ID }, schoolId, UserInfo);
                }
            }
            else
            {
                StudentEntity findStudent = studentBusiness.GetStudentEntity(entity.ID, UserInfo);
                if (findStudent != null)
                {
                    findStudent.FirstName = entity.FirstName;
                    findStudent.MiddleName = entity.MiddleName;
                    findStudent.LastName = entity.LastName;
                    findStudent.BirthDate = entity.BirthDate;
                    findStudent.Gender = entity.Gender;
                    findStudent.LocalStudentID = entity.LocalStudentID;
                    findStudent.GradeLevel = entity.GradeLevel;
                    //  findStudent.TSDSStudentID = entity.TSDSStudentID;
                    findStudent.Ethnicity = entity.Ethnicity;
                    findStudent.AssessmentLanguage = entity.AssessmentLanguage;
                    findStudent.Status = entity.Status;
                    findStudent.StatusDate = DateTime.Now;
                    IList<int> classList = new List<int>();
                    classList = findStudent.Classes.Where(e => e.IsDeleted == false).Select(o => o.ID).ToList();
                    if (!classList.Any(o => o.Equals(classId)))
                        classList.Add(classId);
                    result = studentBusiness.UpdateStudent(UserInfo, findStudent, classList.ToArray());
                }
            }

            return result;
        }

        private OperationResult Upload(int communityId, int schoolId, int teacherId, int classId, DayType classDayType, HttpPostedFileBase postFileBase, bool isComfirm = false)
        {
            var res = new OperationResult(OperationResultType.Success);
            var response = new PostFormResponse();
            ClassEntity findClass = new ClassEntity();
            DataTable dt = new DataTable();
            List<StudentEntity> listStu = new List<StudentEntity>();
            try
            {
                res = ValidateExcel(postFileBase, out dt);
            }
            catch (Exception ex)
            {
                res.Message = "Invalid excel file format.";
                res.ResultType = OperationResultType.Error;
            }
            if (res.ResultType == OperationResultType.Success)
            {
                if (classId <= 0)
                {
                    res.Message = "Class is required.";
                    res.ResultType = OperationResultType.Error;
                }
                else
                {
                    findClass = classBusiness.GetClass(classId);
                }
            }
            if (res.ResultType == OperationResultType.Success)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    StudentEntity student = studentBusiness.NewStudentEntity();

                    if (GetData(dr["First Name"]) != string.Empty)
                    {
                        student.FirstName = GetData(dr["First Name"]);
                    }
                    if (GetData(dr["Middle Name"]) != string.Empty)
                    {
                        student.MiddleName = GetData(dr["Middle Name"]);
                    }
                    if (GetData(dr["Last Name"]) != string.Empty)
                    {
                        student.LastName = GetData(dr["Last Name"]);
                    }
                    if (GetData(dr["Date of Birth(mm/dd/yyyy)"]) != string.Empty)
                    {
                        student.BirthDate = GetDateTime(dr["Date of Birth(mm/dd/yyyy)"]);
                    }
                    if (GetData(dr["Student Internal ID"]) != string.Empty)
                    {
                        student.LocalStudentID = GetData(dr["Student Internal ID"]);
                    }
                    if (GetData(dr["Ethnicity"]) != string.Empty)
                    {
                        var ethnicityStr = GetData(dr["Ethnicity"]).ToLower();
                        var ethnicity = new Ethnicity();
                        switch (ethnicityStr)
                        {
                            case "african american":
                                ethnicity = Ethnicity.African_American;
                                break;
                            case "alaskan":
                                ethnicity = Ethnicity.Alaskan;
                                break;
                            case "native_american":
                                ethnicity = Ethnicity.Native_American;
                                break;
                            case "indian":
                                ethnicity = Ethnicity.Indian;
                                break;
                            case "asian":
                                ethnicity = Ethnicity.Asian;
                                break;
                            case "white":
                                ethnicity = Ethnicity.White;
                                break;
                            case "hispanic":
                                ethnicity = Ethnicity.Hispanic;
                                break;
                            case "multiracial":
                                ethnicity = Ethnicity.Multiracial;
                                break;
                            case "other":
                                ethnicity = Ethnicity.Other;
                                break;
                            default:
                                ethnicity = Ethnicity.Other;
                                break;
                        }
                        student.Ethnicity = ethnicity;
                    }
                    if (GetData(dr["Gender(Male/Female)"]) != string.Empty)
                    {
                        student.Gender = GetData(dr["Gender(Male/Female)"]).ToLower() == "female" ? Gender.Female : Gender.Male;
                    }
                    student.Status = EntityStatus.Active;
                    if (GetData(dr["Status(Active/Inactive)"]) != string.Empty)
                    {
                        student.Status = GetData(dr["Status(Active/Inactive)"]).ToLower() == "active" ? EntityStatus.Active : EntityStatus.Inactive;
                    }
                    if (GetData(dr["Assessment Language"]) != string.Empty)
                    {
                        var languageStr = GetData(dr["Assessment Language"]).ToLower();
                        var language = new StudentAssessmentLanguage();

                        switch (languageStr)
                        {
                            case "english": language = StudentAssessmentLanguage.English; break;
                            case "spanish": language = StudentAssessmentLanguage.Spanish; break;
                            case "bilingual": language = StudentAssessmentLanguage.Bilingual; break;
                            case "nonapplicable": language = StudentAssessmentLanguage.NonApplicable; break;
                            default: language = StudentAssessmentLanguage.NonApplicable;
                                break;
                        }
                        student.AssessmentLanguage = language;
                    }
                    student.GradeLevel = StudentGradeLevel.K;
                    if (GetData(dr["Grade Level(Pre-k/K)"]) != string.Empty)
                    {
                        student.GradeLevel = GetData(dr["Grade Level(Pre-k/K)"]).ToLower() == "k" ? StudentGradeLevel.K : StudentGradeLevel.Prek;
                    }
                    listStu.Add(student);
                }
                res = CheckStudents(communityId, listStu, isComfirm);
                if (res.ResultType == OperationResultType.Success)
                {
                    foreach (var studentEntity in listStu)
                    {
                        res = SaveStudent(schoolId, findClass.ID, studentEntity);
                        if (res.ResultType != OperationResultType.Success)
                        {
                            break;
                        }
                    }
                }

            }
            return res;
        }

        private OperationResult CheckStudents(int communityId, List<StudentEntity> listStu, bool isComfirm = false)
        {
            var res = new OperationResult(OperationResultType.Success);
            var studentList = listStu.Where(t => t.ID == 0).GroupBy(o => new { o.FirstName, o.LastName, o.BirthDate }).ToList();
            studentList = studentList.Where(o => o.Count() > 1).ToList();
            if (studentList.Count > 0)
            {
                res.ResultType = OperationResultType.Error;
                res.Message = " More than one student with the same name and DOB in the list. ";
                return res;
            }
            if (!isComfirm)
            {
                foreach (var studentEntity in listStu)
                {
                    if (studentEntity.LocalStudentID == "")
                    {

                        res.ResultType = OperationResultType.Warning;
                        res.Message = "More than one student has no Student Internal ID , Continue?";
                        return res;
                    }

                }
            }
            foreach (StudentEntity studentEntity in listStu)
            {
                StudentEntity findEntity = studentBusiness.GetStudent(communityId, studentEntity.FirstName, studentEntity.LastName, studentEntity.BirthDate);
                if (findEntity != null && findEntity.ID != studentEntity.ID)
                {
                    res.ResultType = OperationResultType.IllegalOperation;
                    string msg = "";
                    string schoolName = "";
                    string className = "";
                    string teacherName = "";
                    if (findEntity.Classes.Any(e => e.IsDeleted == false))
                    {
                        className = findEntity.Classes.Where(e => e.IsDeleted == false).Select(o => o.Name).FirstOrDefault();
                        List<TeacherEntity> teachers =
                            findEntity.Classes.Where(e => e.IsDeleted == false)
                                .SelectMany(o => o.Teachers.Where(e => e.UserInfo.IsDeleted == false))
                                .ToList();
                        if (teachers.Count > 0)
                        {
                            teacherName = teachers.Select(t => t.UserInfo.FirstName +" "+t.UserInfo.LastName).FirstOrDefault();
                        }
                        var firstOrDefaultSchoolName = findEntity.Classes.FirstOrDefault(e => e.IsDeleted == false);
                        if (firstOrDefaultSchoolName != null)
                            schoolName = firstOrDefaultSchoolName.School.Name; 
                        //(c => c.School.Name);
                    }
                    string resStr = @"<table style='text-align:left;vertical-align: top; table-layout:fixed;word-break:break-word'>
                                          <tr><td colspan='2' style='color:#000000'>Your Class seems to have a duplicate Student at:</td></tr>
                                        <tr><td colspan='2'>&nbsp;</td></tr>
                                        <tr><td style='text-align:left;vertical-align: top;width:150px;'>School:</td><td>" + schoolName + @"</td></tr>
                                        <tr><td style='text-align:left;vertical-align: top;width:150px;'>Class:</td><td>" + className + @"</td></tr>
                                        <tr><td style='text-align:left;vertical-align: top;width:150px;'>Teacher:</td><td>" + teacherName + @"</td></tr>
                                        <tr><td style='text-align:left;vertical-align: top;width:150px;'>Student:</td><td>" + findEntity.FirstName + " " + findEntity.LastName + @"</td></tr>
                                         <tr><td style='text-align:left;vertical-align: top;width:170px;'>Student Engage ID:</td><td>" + findEntity.StudentId + @"</td></tr>    
  <tr><td colspan='2'>&nbsp;</td></tr>
                                             <tr><td colspan='2' style='color:#000000'>You may need to contact your District administrator to solve the duplicate student issue.</td></tr>                              
                                        </table>";
                    res.Message = resStr;
                    return res;
                }
            }
            return res;
        }

        private OperationResult ValidateExcel(HttpPostedFileBase postFileBase, out DataTable dt)
        {
            OperationResult res = new OperationResult(OperationResultType.Success);
            dt = new DataTable();
            if (postFileBase == null || postFileBase.ContentLength == 0)
            {
                res.Message = "Please select file.";
                res.ResultType = OperationResultType.Error;
                return res;
            }

            string fileType = string.Empty;
            if (postFileBase.ContentLength == 0)
            {
                res.Message = "Please select a valid file.";
                res.ResultType = OperationResultType.Error;
                return res;
            }

            string[] name = postFileBase.FileName.Split('.');
            fileType = name[name.Length - 1];
            if (string.IsNullOrEmpty(fileType) || (fileType.ToLower() != "xls" && fileType.ToLower() != "xlsx"))
            {
                res.Message = "Please select a valid file.";
                res.ResultType = OperationResultType.Error;
                return res;
            }

            string originFileName = Path.GetFileName(postFileBase.FileName);
            var virtualPath = FileHelper.SaveProtectedFile(postFileBase, "BES");
            string uploadPath = FileHelper.GetProtectedFilePhisycalPath(virtualPath);

            string strCNN = string.Empty;

            strCNN = "Provider=Microsoft.ACE.OleDb.12.0;Data Source = " + uploadPath + ";Extended Properties = 'Excel 12.0;HDR=Yes;IMEX=1;'";

            OleDbConnection cnn = new OleDbConnection(strCNN);
            try
            {
                cnn.Open();
            }
            catch (Exception ex)
            {
                res.Message = ex.ToString();
                res.ResultType = OperationResultType.Error;
                cnn.Dispose();
                return res;
            }

            DataTable schemaTable = cnn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] { null, null, null, "Table" });
            string tableName = string.Empty;
            if (schemaTable == null)
            {
                res.Message = "There are some errors in the file.";
                res.ResultType = OperationResultType.Error;
                return res;

            }

            string strSQL = " SELECT * FROM [Students$] ";

            OleDbDataAdapter cmd = new OleDbDataAdapter(strSQL, cnn);

            cmd.Fill(dt);
            cnn.Close();
            if (dt == null || dt.Rows.Count < 1 || dt.Columns.Count < 8)
            {
                res.Message = "Datasource error.";
                res.ResultType = OperationResultType.Error;
                return res;
            }

            var isValid = string.Equals(dt.Columns[0].ColumnName, "First Name", StringComparison.CurrentCultureIgnoreCase) == true
                     &&
                     string.Equals(dt.Columns[1].ColumnName, "Middle Name", StringComparison.CurrentCultureIgnoreCase) == true
                     &&
                     string.Equals(dt.Columns[2].ColumnName, "Last Name", StringComparison.CurrentCultureIgnoreCase) == true
                      &&
                     string.Equals(dt.Columns[3].ColumnName, "Date of Birth(mm/dd/yyyy)", StringComparison.CurrentCultureIgnoreCase) == true
                      &&
                     string.Equals(dt.Columns[4].ColumnName, "Gender(Male/Female)", StringComparison.CurrentCultureIgnoreCase) == true
                        &&
                     string.Equals(dt.Columns[5].ColumnName, "Student Internal ID", StringComparison.CurrentCultureIgnoreCase) == true
                        &&
                     string.Equals(dt.Columns[6].ColumnName, "Ethnicity", StringComparison.CurrentCultureIgnoreCase) == true
                        &&
                     string.Equals(dt.Columns[7].ColumnName, "Grade Level(Pre-k/K)", StringComparison.CurrentCultureIgnoreCase) == true
                      &&
                     string.Equals(dt.Columns[8].ColumnName, "Assessment Language", StringComparison.CurrentCultureIgnoreCase) == true
                            &&
                     string.Equals(dt.Columns[9].ColumnName, "Status(Active/Inactive)", StringComparison.CurrentCultureIgnoreCase) == true
                   ;
            if (!isValid)
            {
                res.Message = "Datasource error.";
                res.ResultType = OperationResultType.Error;
                return res;
            }
            return res;
        }

        private DateTime GetDateTime(object o)
        {
            if (o is DBNull) return CommonAgent.MinDate;
            string number = o.ToString().Trim();
            DateTime tmpDate;
            if (DateTime.TryParse(number, out tmpDate))
                return tmpDate;
            else return CommonAgent.MinDate;
        }

        private int GetNumber(object o)
        {
            if (o is DBNull) return 0;
            string number = o.ToString().Trim();
            int tmpNum;
            if (int.TryParse(number, out tmpNum))
                return tmpNum;
            else return 0;
        }

        private byte GetGender(object o)
        {
            if (o is DBNull) return 0;
            string gender = o.ToString().Trim();
            gender = gender.ToLower();
            if (gender == "female")
                return (byte)Gender.Female;
            else if (gender == "male")
                return (byte)Gender.Male;
            return 0;
        }

        private string GetData(object o)
        {
            if (o is DBNull) return string.Empty;
            return SqlHelper.ReplaceSqlChar(o.ToString().Trim());
        }
    }
}