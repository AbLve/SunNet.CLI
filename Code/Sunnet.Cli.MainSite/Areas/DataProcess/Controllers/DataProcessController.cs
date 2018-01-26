using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Linq;
using Sunnet.Cli.Business.Classes;
using Sunnet.Cli.Business.DataProcess;
using Sunnet.Cli.Business.Students;
using Sunnet.Cli.Core.Classes.Entites;
using Sunnet.Cli.Core.DataProcess;
using Sunnet.Cli.MainSite.Controllers;
using Sunnet.Cli.UIBase;
using Sunnet.Cli.UIBase.Models;
using Sunnet.Framework.Helpers;
using Sunnet.Cli.Core.Communities.Entities;
using Sunnet.Cli.Business.Communities;
using System.IO;
using System.Data.OleDb;
using System.Data;
using Sunnet.Framework.Log;
using StructureMap;
using Sunnet.Cli.Core.DataProcess.Entities;
using Sunnet.Framework.Core.Tool;
using System.Text;
using Sunnet.Cli.Core.Common.Enums;
using Sunnet.Cli.Core.Classes.Enums;
using Sunnet.Cli.Business.Common;
using Sunnet.Cli.UIBase.Filters;
using Sunnet.Framework.Permission;
using Newtonsoft.Json;
using Sunnet.Cli.Business.BUP;
using Sunnet.Cli.Business.DataProcess.Models;
using Sunnet.Cli.Core.Students.Enums;
using Sunnet.Cli.Core.Ade;
using Sunnet.Cli.Core.Users.Enums;
using Sunnet.Cli.Core.BUP;
using System.Linq.Expressions;
using Sunnet.Cli.Business.Schools;
using Sunnet.Cli.Business.BUP.Model;
using Sunnet.Cli.Core.Students.Entities;
using Sunnet.Framework.Extensions;
using ProcessStatus = Sunnet.Cli.Core.DataProcess.Entities.ProcessStatus;

namespace Sunnet.Cli.MainSite.Areas.DataProcess.Controllers
{
    public class DataProcessController : BaseController
    {
        CommunityBusiness _communityBusiness;
        SchoolBusiness _schoolBusiness;
        DataProcessBusiness _processBus;
        BUPTaskBusiness _bupTaskBusiness;
        private ClassBusiness _classBusiness;
        private StudentBusiness _studentBusiness;
        ISunnetLog _log = ObjectFactory.GetInstance<ISunnetLog>();

        public DataProcessController()
        {
            _communityBusiness = new CommunityBusiness();
            _schoolBusiness = new SchoolBusiness();
            _bupTaskBusiness = new BUPTaskBusiness();
            _processBus = new DataProcessBusiness(UnitWorkContext);
            _classBusiness = new ClassBusiness();
            _studentBusiness = new StudentBusiness();
        }

        // GET: /DataProcess/DataProcess/
        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.DataProcess, Anonymity = Anonymous.Verified)]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.DataProcess, Anonymity = Anonymous.Verified)]
        public string Index_Spare(int communityId, string invitation, string createClassroom)
        {
            var response = new PostFormResponse();
            CommunityEntity communityEntity = _communityBusiness.GetCommunity(communityId);
            IList<ClassLevelEntity> classLevels = _classBusiness.GetClassLevels();
            if (communityEntity == null || communityEntity.Status == Core.Common.Enums.EntityStatus.Inactive)
            {
                response.Success = false;
                response.Data = "warning";
                response.Message = "Community does not exist.";
                return JsonConvert.SerializeObject(response);
            }

            HttpPostedFileBase postFileBase = Request.Files["dataFile"];

            if (postFileBase == null || postFileBase.ContentLength == 0)
            {
                response.Success = false;
                response.Data = "warning";
                response.Message = "<p>The file cannot be submitted.</p><p>Invalid format</p>";
                return JsonConvert.SerializeObject(response);
            }

            string fileType = string.Empty;
            string[] name = postFileBase.FileName.Split('.');
            fileType = name[name.Length - 1];
            if (string.IsNullOrEmpty(fileType) || (fileType.ToLower() == "xls" && fileType.ToLower() == "xlsx"))
            {
                response.Success = false;
                response.Data = "warning";
                response.Message = "<p>The file cannot be submitted.</p><p>Invalid format</p>";
                return JsonConvert.SerializeObject(response);
            }

            string originFileName = Path.GetFileName(postFileBase.FileName);
            var virtualPath = FileHelper.SaveProtectedFile(postFileBase, "data_Process");
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
                cnn.Dispose();
                _log.Debug(ex);

                response.Success = false;
                response.Data = "warning";
                response.Message = "<p>The file cannot be submitted.</p><p>Invalid format</p>";
                return JsonConvert.SerializeObject(response);
            }

            DataTable schemaTable = cnn.GetOleDbSchemaTable(System.Data.OleDb.OleDbSchemaGuid.Tables, new object[] { null, null, null, "Table" });

            string tableName = string.Empty;
            foreach (DataRow dr in schemaTable.Rows)
            {
                tableName = dr[2].ToString().Trim();
                if (tableName.IndexOf("_FilterDatabase") < 0)
                    break;
            }
            if (tableName.StartsWith("'"))
            {
                tableName = tableName.Replace("'", "");
                tableName = tableName.EndsWith("$") == false ? tableName + "$" : tableName;
                tableName = "'" + tableName + "'";
            }
            else
            {
                tableName = tableName.EndsWith("$") == false ? tableName + "$" : tableName;
            }

            string strSQL = " SELECT * FROM [" + tableName + "]";

            OleDbDataAdapter cmd = new OleDbDataAdapter(strSQL, cnn);
            DataTable dt = new DataTable();
            cmd.Fill(dt);
            cnn.Close();


            if (dt == null || dt.Rows.Count < 1 || dt.Columns.Count < 24)
            {
                response.Success = false;
                response.Data = "warning";
                response.Message = "<p>The file cannot be submitted.</p><p>Invalid format</p>";
                return JsonConvert.SerializeObject(response);
            }

            try
            {
                if (ValidateExcel(dt))
                {
                    response.Success = false;
                    response.Data = "warning";
                    response.Message = "<p>The file cannot be submitted.</p><p>Invalid format</p>";
                    return JsonConvert.SerializeObject(response);
                }
            }
            catch (Exception ex)
            {
                _log.Debug(ex);
                response.Success = false;
                response.Data = "warning";
                response.Message = "<p>The file cannot be submitted.</p><p>Invalid format</p>";
                return JsonConvert.SerializeObject(response);
            }

            

            DataGroupEntity groupEntity = new DataGroupEntity();
            groupEntity.FilePath = virtualPath;
            groupEntity.OriginFileName = originFileName;
            groupEntity.CreatedBy = UserInfo.ID;
            groupEntity.UpdatedBy = UserInfo.ID;
            groupEntity.SendInvitation = (invitation == "1");
            groupEntity.RecordCount = 0;
            groupEntity.Status = Sunnet.Cli.Core.DataProcess.Entities.ProcessStatus.Pending;
            groupEntity.SchoolTotal = 0;
            groupEntity.Remark = "";
            groupEntity.CommunityId = communityId;
            groupEntity.CreateClassroom = (createClassroom == "1");

            OperationResult result = _processBus.InsertGroup(groupEntity);
            

            response.Success = result.ResultType == OperationResultType.Success;
            response.Message = "<p>The file was successfully submitted. </p>" +
                                "The task will be executed automatically later. Before the task is executed, you can delete the file by clicking one of the icons in the action column. </p>";
            return JsonConvert.SerializeObject(response);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.DataProcess, Anonymity = Anonymous.Verified)]
        public string Index(int communityId, string invitation, string createClassroom)
        {
            var response = new PostFormResponse();
            CommunityEntity communityEntity = _communityBusiness.GetCommunity(communityId);
            IList<ClassLevelEntity> classLevels = _classBusiness.GetClassLevels();
            if (communityEntity == null || communityEntity.Status == Core.Common.Enums.EntityStatus.Inactive)
            {
                response.Success = false;
                response.Data = "warning";
                response.Message = "Community does not exist.";
                return JsonConvert.SerializeObject(response);
            }

            HttpPostedFileBase postFileBase = Request.Files["dataFile"];

            if (postFileBase == null || postFileBase.ContentLength == 0)
            {
                response.Success = false;
                response.Data = "warning";
                response.Message = "<p>The file cannot be submitted.</p><p>Invalid format</p>";
                return JsonConvert.SerializeObject(response);
            }

            string fileType = string.Empty;
            string[] name = postFileBase.FileName.Split('.');
            fileType = name[name.Length - 1];
            if (string.IsNullOrEmpty(fileType) || (fileType.ToLower() == "xls" && fileType.ToLower() == "xlsx"))
            {
                response.Success = false;
                response.Data = "warning";
                response.Message = "<p>The file cannot be submitted.</p><p>Invalid format</p>";
                return JsonConvert.SerializeObject(response);
            }

            string originFileName = Path.GetFileName(postFileBase.FileName);
            var virtualPath = FileHelper.SaveProtectedFile(postFileBase, "data_Process");
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
                cnn.Dispose();
                _log.Debug(ex);

                response.Success = false;
                response.Data = "warning";
                response.Message = "<p>The file cannot be submitted.</p><p>Invalid format</p>";
                return JsonConvert.SerializeObject(response);
            }

            DataTable schemaTable = cnn.GetOleDbSchemaTable(System.Data.OleDb.OleDbSchemaGuid.Tables, new object[] { null, null, null, "Table" });

            string tableName = string.Empty;
            foreach (DataRow dr in schemaTable.Rows)
            {
                tableName = dr[2].ToString().Trim();
                if (tableName.IndexOf("_FilterDatabase") < 0)
                    break;
            }
            if (tableName.StartsWith("'"))
            {
                tableName = tableName.Replace("'", "");
                tableName = tableName.EndsWith("$") == false ? tableName + "$" : tableName;
                tableName = "'" + tableName + "'";
            }
            else
            {
                tableName = tableName.EndsWith("$") == false ? tableName + "$" : tableName;
            }

            string strSQL = " SELECT * FROM [" + tableName + "]";

            OleDbDataAdapter cmd = new OleDbDataAdapter(strSQL, cnn);
            DataTable dt = new DataTable();
            cmd.Fill(dt);
            cnn.Close();


            if (dt == null || dt.Rows.Count < 1 || dt.Columns.Count < 24)
            {
                response.Success = false;
                response.Data = "warning";
                response.Message = "<p>The file cannot be submitted.</p><p>Invalid format</p>";
                return JsonConvert.SerializeObject(response);
            }

            try
            {
                if (ValidateExcel(dt))
                {
                    response.Success = false;
                    response.Data = "warning";
                    response.Message = "<p>The file cannot be submitted.</p><p>Invalid format</p>";
                    return JsonConvert.SerializeObject(response);
                }
            }
            catch (Exception ex)
            {
                _log.Debug(ex);
                response.Success = false;
                response.Data = "warning";
                response.Message = "<p>The file cannot be submitted.</p><p>Invalid format</p>";
                return JsonConvert.SerializeObject(response);
            }

            List<DataProcessModel> list = new List<DataProcessModel>();

            List<NameModel> allSchools = new List<NameModel>();
            List<NameModel> associatedSchools = new List<NameModel>();
            allSchools = _schoolBusiness.GetAllSchools();
            associatedSchools = _schoolBusiness.GetSchools(UserInfo, communityId);
            List<StudentEntity> allStudents =
                _studentBusiness.GetStudentsBySchoolIds(associatedSchools.Select(e => e.EngageId).ToList());
            List<NameModel> communitys = _communityBusiness.GetCommunitiesByUser(UserInfo);
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                StringBuilder sbRemark = new StringBuilder();
                HelpSolve help = new HelpSolve(dt.Rows, i);

                DataProcessModel model = new DataProcessModel();
                string action = help.NextData().ToLower().Trim();
                model.CommunityName = help.NextData();
                model.CommunityInternalID = help.NextData();
                model.SchoolName = help.NextData();
                model.SchoolInternalID = help.NextData();

                //若主要字段都为空，则忽略此条记录（用于防止添加数据后再删除时，读取空行的记录）
                if (string.IsNullOrEmpty(action) && string.IsNullOrEmpty(model.CommunityName) &&
                    string.IsNullOrEmpty(model.CommunityInternalID)
                    && string.IsNullOrEmpty(model.SchoolName) && string.IsNullOrEmpty(model.SchoolInternalID))
                {
                    continue;
                }

                if (action != "i" && action != "u")
                {
                    sbRemark.Append("<br />Action should be upper case i or u.");
                }
                else
                {
                    if (action == "i")
                    {
                        model.Action = DataProcessAction.Insert;
                    }
                    else if (action == "u")
                    {
                        model.Action = DataProcessAction.Update;
                    }
                }

                if (!(communitys.Any(e => e.Name.ToLower() == model.CommunityName.ToLower())))
                {
                    sbRemark.Append("<br />Verify that Community is active in Engage.");
                }
                if (!(allSchools.Any(e => e.Name.ToLower() == model.SchoolName.ToLower())))
                {
                    sbRemark.Append("<br />Verify that School is active in Engage.");
                }
                var conditionSelectedCommunity = PredicateHelper.True<CommunityEntity>();
                conditionSelectedCommunity =
                    conditionSelectedCommunity.And(
                        r => r.Name.ToLower() == model.CommunityName.ToLower() && (r.DistrictNumber.ToLower() == model.CommunityInternalID.ToLower() || r.CommunityId.ToLower() == model.CommunityInternalID.ToLower()));
                if (!(communitys.Any(e => e.Name.ToLower() == model.CommunityName.ToLower() && (e.InternalId.ToLower() == model.CommunityInternalID.ToLower() || e.EngageId.ToLower() == model.CommunityInternalID.ToLower()))))
                {
                    sbRemark.Append("<br />Community and Community ID mismatch.");
                }
                if (!(communitys.Any(e => e.Name.ToLower() == model.CommunityName.ToLower() && (e.InternalId.ToLower() == model.CommunityInternalID.ToLower() || e.EngageId.ToLower() == model.CommunityInternalID.ToLower()) && e.Name.ToLower() == communityEntity.Name.ToLower())))
                {
                    sbRemark.Append("<br />The Districts that you typed and selected do not match.");
                }
                if (!(allSchools.Any(e => e.Name.ToLower() == model.SchoolName.ToLower() && (e.InternalId.ToLower() == model.SchoolInternalID.ToLower() || e.EngageId.ToLower() == model.SchoolInternalID.ToLower()))))
                {
                    sbRemark.Append("<br />School and Internal ID mismatch.");
                }

                if (allSchools.Find(r => (r.InternalId.ToLower() == model.SchoolInternalID.ToLower() || r.EngageId.ToLower() == model.SchoolInternalID.ToLower()) && r.Name.ToLower() == model.SchoolName.ToLower()) !=
                    null
                    &&
                    associatedSchools.Find(r => (r.InternalId.ToLower() == model.SchoolInternalID.ToLower() || r.EngageId.ToLower() == model.SchoolInternalID.ToLower()) && r.Name.ToLower() == model.SchoolName.ToLower()) ==
                    null)
                {
                    sbRemark.Append("<br />The School does not belong to the Community or District.");
                }

                model.Teacher_FirstName = help.NextData();
                if (string.IsNullOrEmpty(model.Teacher_FirstName))
                {
                    sbRemark.Append("<br />Missing Teacher name.");
                }

                model.Teacher_MiddleName = help.NextData();

                model.Teacher_LastName = help.NextData();
                if (string.IsNullOrEmpty(model.Teacher_LastName))
                {
                    sbRemark.Append("<br />Missing Teacher Last name.");
                }

                model.Teacher_InternalID = help.NextData();
                if (string.IsNullOrEmpty(model.Teacher_InternalID))
                {
                    sbRemark.Append("<br />Missing Teacher Internal ID.");
                }

                model.Teacher_PhoneNumber = help.NextData();
                if (!string.IsNullOrEmpty(model.Teacher_PhoneNumber))
                {
                    if (!(Regex.IsMatch(model.Teacher_PhoneNumber, @"\d{3}-\d{3}-\d{4}") || Regex.IsMatch(model.Teacher_PhoneNumber, @"\(\d{3}\)\d{3}-\d{4}")))
                    {
                        sbRemark.Append("<br />Invalid phone number.");
                    }
                }

                string teacherPhoneType = help.NextData().ToLower().Trim();
                switch (teacherPhoneType)
                {
                    case "work":
                    case "w":
                        model.Teacher_PhoneType = (byte)PhoneType.WorkNumber;
                        break;
                    case "home":
                    case "h":
                        model.Teacher_PhoneType = (byte)PhoneType.HomeNumber;
                        break;
                    case "cell":
                    case "c":
                        model.Teacher_PhoneType = (byte)PhoneType.CellNumber;
                        break;
                    default:
                        model.Teacher_PhoneType = 0;
                        if (!string.IsNullOrEmpty(teacherPhoneType))
                            sbRemark.Append("<br />Invalid phone type.");
                        break;
                }

                model.Teacher_PrimaryEmail = help.NextData();
                if (string.IsNullOrEmpty(model.Teacher_PrimaryEmail)
                    || !CommonAgent.IsEmail(model.Teacher_PrimaryEmail))
                {
                    sbRemark.Append("<br />Missing or Invalid email.");
                }

                string classDayType = help.NextData().ToLower().Trim();
                switch (classDayType)
                {
                    case "full day":
                    case "fd":
                        model.Class_DayType = (byte)DayType.FullDay;
                        break;
                    case "am":
                        model.Class_DayType = (byte)DayType.Am;
                        break;
                    case "pm":
                        model.Class_DayType = (byte)DayType.Pm;
                        break;
                    default:
                        model.Class_DayType = 0;
                        break;
                }
                if (string.IsNullOrEmpty(classDayType) || model.Class_DayType <= 0)
                {
                    sbRemark.Append("<br />Missing or invalid Class Day Type.");
                }

                string classLevel = help.NextData().ToLower().Trim();

                var classLevelName = classLevel.ToUpper();
                var findClassLevel = classLevels.FirstOrDefault(c => c.Name == classLevelName);
                if (findClassLevel != null)
                    model.Class_Level = findClassLevel.ID;
                if (string.IsNullOrEmpty(classLevel) || findClassLevel == null)
                {
                    sbRemark.Append("<br />Missing or invalid Class Level.");
                }

                model.Student_FirstName = help.NextData();
                if (string.IsNullOrEmpty(model.Student_FirstName))
                {
                    sbRemark.Append("<br />Missing Student name.");
                }

                model.Student_MiddleName = help.NextData();

                model.Student_LastName = help.NextData();
                if (string.IsNullOrEmpty(model.Student_LastName))
                {
                    sbRemark.Append("<br />Missing Student Last name.");
                }

                model.Student_InternalID = help.NextData();
                model.Student_TsdsID = help.NextData();

                string studentGradeLevel = help.NextData().ToLower().Trim();
                switch (studentGradeLevel)
                {
                    case "pk":
                        model.Student_GradeLevel = (byte)StudentGradeLevel.Prek;
                        break;
                    case "k":
                        model.Student_GradeLevel = (byte)StudentGradeLevel.K;
                        break;
                    default:
                        model.Student_GradeLevel = 0;
                        break;
                }
                if (string.IsNullOrEmpty(studentGradeLevel) || model.Student_GradeLevel <= 0)
                {
                    sbRemark.Append("<br />Missing or invalid Student Grade Level.");
                }

                string studentLanguage = help.NextData().ToLower().Trim();
                switch (studentLanguage)
                {
                    case "bilingual":
                    case "both":
                    case "en and sp":
                        model.Student_AssessmentLanguage = (byte) StudentAssessmentLanguage.Bilingual;
                        break;
                    case "english":
                        model.Student_AssessmentLanguage = (byte) StudentAssessmentLanguage.English;
                        break;
                    case "spanish":
                        model.Student_AssessmentLanguage = (byte) StudentAssessmentLanguage.Spanish;
                        break;
                    default:
                        if (!string.IsNullOrEmpty(studentLanguage))
                            sbRemark.Append("<br />Invalid Assessment language.");
                        else
                        {
                            if (model.Action == DataProcessAction.Update)
                                model.Student_AssessmentLanguage = 0;
                            else
                                model.Student_AssessmentLanguage = (byte) StudentAssessmentLanguage.Bilingual;
                        }
                        break;
                }

                string birthDate = help.NextData();
                DateTime date;
                if (DateTime.TryParse(birthDate, out date))
                {
                    //系统中日期小于1900-1-1时，页面显示为空    和   为了防止只输入时间时，读取为当前日期
                    if (date >= DateTime.Parse("1900-1-1") && date.Date < DateTime.Now.Date)
                        model.Student_BirthDate = date;
                    else
                    {
                        model.Student_BirthDate = CommonAgent.MinDate;
                        sbRemark.Append("<br />Missing or invalid DOB.");
                    }
                }
                else
                {
                    model.Student_BirthDate = CommonAgent.MinDate;
                    sbRemark.Append("<br />Missing or invalid DOB.");
                }

                string studentGender = help.NextData().ToLower().Trim();
                switch (studentGender)
                {
                    case "male":
                    case "m":
                        model.Student_Gender = (byte)Gender.Male;
                        break;
                    case "female":
                    case "f":
                        model.Student_Gender = (byte)Gender.Female;
                        break;
                    default:
                        if (model.Action == DataProcessAction.Update)
                        {
                            if (!string.IsNullOrEmpty(studentGender))
                            {
                                sbRemark.Append("<br />Invalid gender.");
                            }
                        }
                        else if (!string.IsNullOrEmpty(studentLanguage))
                            sbRemark.Append("<br />Invalid gender.");
                        model.Student_Gender = 0;
                        break;
                }

                string studentEthnicity = help.NextData().ToLower().Trim();
                switch (studentEthnicity)
                {
                    case "african american":
                        model.Student_Ethnicity = (byte)Ethnicity.African_American;
                        break;
                    case "alaskan":
                        model.Student_Ethnicity = (byte)Ethnicity.Alaskan;
                        break;
                    case "native american":
                        model.Student_Ethnicity = (byte)Ethnicity.Native_American;
                        break;
                    case "indian":
                        model.Student_Ethnicity = (byte)Ethnicity.Indian;
                        break;
                    case "asian":
                        model.Student_Ethnicity = (byte)Ethnicity.Asian;
                        break;
                    case "white":
                        model.Student_Ethnicity = (byte)Ethnicity.White;
                        break;
                    case "hispanic":
                        model.Student_Ethnicity = (byte)Ethnicity.Hispanic;
                        break;
                    case "multiracial":
                        model.Student_Ethnicity = (byte)Ethnicity.Multiracial;
                        break;
                    case "other":
                        model.Student_Ethnicity = (byte)Ethnicity.Other;
                        break;
                    default:
                        if (model.Action == DataProcessAction.Update)
                        {
                            if (!string.IsNullOrEmpty(studentEthnicity))
                            {
                                sbRemark.Append("<br />Invalid ethnicity.");
                            }
                        }
                        else if (!string.IsNullOrEmpty(studentEthnicity))
                            sbRemark.Append("<br />Invalid ethnicity.");
                        model.Student_Ethnicity = 0;
                        break;
                }

                if (model.Action == DataProcessAction.Insert)
                {
                    if (
                        list.Where(w => w.Action == DataProcessAction.Insert).Any(
                            e =>
                                e.Student_FirstName.ToLower() == model.Student_FirstName.ToLower() &&
                                e.Student_LastName.ToLower() == model.Student_LastName.ToLower()
                                && e.Student_BirthDate == model.Student_BirthDate &&
                                e.Student_InternalID.ToLower() == model.Student_InternalID.ToLower()))
                    {
                        sbRemark.Append("<br />Duplicate Student found in the file.");
                    }
                    var student =
                        allStudents.FirstOrDefault(
                            e => e.FirstName.ToLower() == model.Student_FirstName.ToLower() && e.LastName.ToLower() == model.Student_LastName.ToLower()
                                 && e.BirthDate == model.Student_BirthDate &&
                                 e.LocalStudentID.ToLower() == model.Student_InternalID.ToLower());
                    if (student != null)
                    {
                        sbRemark.Append(student.SchoolRelations.Any(
                            e => e.School.CommunitySchoolRelations.Any(s => s.CommunityId == communityId))
                            ? "<br />Student already exists in the Community."
                            : "<br />Student already exists in another Community.");
                    }
                }
                else if (model.Action == DataProcessAction.Update)
                {
                    var student = allStudents.FirstOrDefault(e => e.LocalStudentID.ToLower() == model.Student_InternalID.ToLower());
                    if (student == null || string.IsNullOrEmpty(model.Student_InternalID))
                    {
                        sbRemark.Append("<br />Student not found in Engage.");
                    }
                }

                model.LineNum = i + 2;
                if (sbRemark.Length > 0)
                {
                    model.Remark = string.Format("#{0}:" + sbRemark.ToString(), i + 2);
                    model.RemarkType = 4;
                }
                list.Add(model);
            }

            //总的school个数
            int schoolCounts = list.GroupBy(r => new { r.SchoolInternalID, r.SchoolName }).Count();
            //总的teacher个数
            int teacherCounts = list.GroupBy(r => new { r.Teacher_InternalID, r.Teacher_FirstName, r.Teacher_LastName, r.Teacher_PrimaryEmail }).Count();
            //总的student个数
            int studentCounts = list.GroupBy(r => new { r.Student_FirstName, r.Student_LastName, r.Student_InternalID }).Count();

            StringBuilder sb = new StringBuilder();

            DataGroupEntity groupEntity = new DataGroupEntity();
            groupEntity.FilePath = virtualPath;
            groupEntity.OriginFileName = originFileName;
            groupEntity.CreatedBy = UserInfo.ID;
            groupEntity.UpdatedBy = UserInfo.ID;
            groupEntity.SendInvitation = (invitation == "1");
            groupEntity.RecordCount = list.Count;
            groupEntity.Status = Sunnet.Cli.Core.DataProcess.Entities.ProcessStatus.Pending;
            groupEntity.SchoolTotal = 0;
            groupEntity.Remark = "";
            groupEntity.CommunityId = communityId;
            groupEntity.CreateClassroom = (createClassroom == "1");

            OperationResult result = _processBus.InsertGroup(groupEntity);
            if (result.ResultType == OperationResultType.Success)
            {
                sb.Append("BEGIN TRANSACTION;");
                sb.Append("BEGIN TRY ");
                foreach (DataProcessModel model in list)
                {
                    sb.Append(";INSERT INTO dbo.DataProcesses(GroupId, Status, Remark, RemarkType, CommunityName, CommunityInternalId")
                .Append(", SchoolName, SchoolInternalId, TeacherFirstName, TeacherMiddleName, TeacherLastName")
                .Append(",TeacherInternalId, TeacherPhoneNumber, TeacherPhoneType, TeacherPrimaryEmail, ClassDayType, ClassLevel")
                .Append(",StudentFirstName, StudentMiddleName, StudentLastName, StudentInternalId, StudentTsdsId")
                .Append(",StudentGradeLevel, StudentLanguage, StudentBirthdate, StudentGender, StudentEthnicity, LineNum")
                .Append(",Action)")
                .Append(" VALUES (")
                .AppendFormat("{0},{1},'{2}',{3},'{4}','{5}'", groupEntity.ID,
                model.RemarkType == 4 ? (byte)BUPStatus.DataError : (byte)BUPStatus.Queued, model.Remark, model.RemarkType,
                model.CommunityName.ReplaceSqlChar(), model.CommunityInternalID.ReplaceSqlChar())
                .AppendFormat(",'{0}','{1}','{2}','{3}','{4}'", model.SchoolName.ReplaceSqlChar(), model.SchoolInternalID.ReplaceSqlChar(),
                model.Teacher_FirstName.ReplaceSqlChar(), model.Teacher_MiddleName.ReplaceSqlChar(), model.Teacher_LastName.ReplaceSqlChar())
                .AppendFormat(",'{0}','{1}',{2},'{3}',{4},{5}", model.Teacher_InternalID.ReplaceSqlChar(), model.Teacher_PhoneNumber.ReplaceSqlChar(),
                model.Teacher_PhoneType, model.Teacher_PrimaryEmail.ReplaceSqlChar(), model.Class_DayType, model.Class_Level)
                .AppendFormat(",'{0}','{1}','{2}','{3}','{4}'", model.Student_FirstName.ReplaceSqlChar(), model.Student_MiddleName.ReplaceSqlChar(),
                model.Student_LastName.ReplaceSqlChar(), model.Student_InternalID.ReplaceSqlChar(), model.Student_TsdsID.ReplaceSqlChar())
                .AppendFormat(",{0},{1},'{2}',{3},{4},{5},{6}", model.Student_GradeLevel, model.Student_AssessmentLanguage, model.Student_BirthDate,
                model.Student_Gender, model.Student_Ethnicity, model.LineNum, (int)model.Action)
                .Append(" ) ");
                }

                sb.AppendFormat(";UPDATE dbo.DataGroups SET ")
                    .AppendFormat(" TeacherTotal = {0}, StudentTotal={1} ,SchoolTotal = {2}", teacherCounts, studentCounts, schoolCounts)
                    .AppendFormat(" WHERE ID={0} ", groupEntity.ID);

                sb.Append(";SELECT '1' ;COMMIT TRANSACTION;   ")
                     .Append(" END TRY ")
                     .Append(" BEGIN CATCH ;  ")
                     .Append("IF @@TRANCOUNT > 0  BEGIN ROLLBACK TRANSACTION; END")
                     .Append(" SELECT ERROR_MESSAGE() ;")
                     .Append(" END CATCH;");

                string message = _processBus.ImportData(sb.ToString());
                if (message != "1")
                {
                    groupEntity.Status = Core.DataProcess.Entities.ProcessStatus.Error_Dup;
                    groupEntity.Remark = message;
                    _processBus.UpdateGrop(groupEntity);
                }
            }

            response.Success = result.ResultType == OperationResultType.Success;
            response.Message = "<p>The file was successfully submitted. </p>" +
                               "<p>You can process or delete the file by clicking one of the icons under the Action Column. </p>";
            return JsonConvert.SerializeObject(response);
        }

        private bool ValidateExcel(DataTable dt)
        {
            GetColumnData help = new GetColumnData(dt);
            return string.Equals(help.NextColumn(), "Action", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "Community_Name", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "Community_ID", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "School_Name", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "School_ID", StringComparison.CurrentCultureIgnoreCase) == false

                   || string.Equals(help.NextColumn(), "Teacher_First_Name", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "Teacher_Middle_Name", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "Teacher_Last_Name", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "Teacher_Internal_ID", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "Teacher_Phone_Number", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "Teacher_Phone_Type", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "Teacher_Primary_Email", StringComparison.CurrentCultureIgnoreCase) == false

                   || string.Equals(help.NextColumn(), "Class_Day_Type", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "Class_Level", StringComparison.CurrentCultureIgnoreCase) == false

                   || string.Equals(help.NextColumn(), "Student_First_Name", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "Student_Middle_Name", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "Student_Last_Name", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "Student_Internal_ID", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "Student_TSDS_ID", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "Student_Grade_Level", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "Student_Assessment_Language", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "Student_Birth_Date", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "Student_Gender", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "Student_Ethnicity", StringComparison.CurrentCultureIgnoreCase) == false;
        }


        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.DataProcess, Anonymity = Anonymous.Verified)]
        public string SearchDefault(string sort = "CreatedOn", string order = "Desc",
          int first = 0, int count = 10)
        {
            var total = 0;

            var list = _processBus.GetGroupList(UserInfo, sort, order, first, count, out total);
            var result = new { total = total, data = list };
            return JsonHelper.SerializeObject(result);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.DataProcess, Anonymity = Anonymous.Verified)]
        public string GetCommunitySelectListForSearch(string keyword, int communityId = -1, bool isActiveCommunity = true)
        {
            var expression = PredicateHelper.True<CommunityEntity>();
            if (communityId > 0)
                expression = expression.And(o => o.ID == communityId);
            var list = _communityBusiness.GetCommunitySelectList(UserInfo, expression, isActiveCommunity);
            return JsonHelper.SerializeObject(list);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.DataProcess, Anonymity = Anonymous.Verified)]
        public string process(int id)
        {
            var entity = _processBus.GetDataGroupEntity(id);
            if (entity != null)
            {
                entity.Status=ProcessStatus.Queued;
                _processBus.UpdateGrop(entity);
            }
            return string.Empty;
        }

        public delegate void ProcessHandler(int id, int createdBy);

        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.DataProcess, Anonymity = Anonymous.Verified)]
        public string Delete(int id)
        {
            var response = new PostFormResponse();
            StringBuilder sql = new StringBuilder();
            sql.AppendFormat("delete DataGroups where ID = {0};", id)
                .AppendFormat(" delete DataProcesses where GroupId = {0};", id);
            _bupTaskBusiness.ExecuteSqlCommand(sql.ToString());
            response.Success = true;
            return JsonConvert.SerializeObject(response);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.DataProcess, Anonymity = Anonymous.Verified)]
        public ActionResult ViewLog(int id)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("select * from DataProcesses where GroupId = {0} and status in (4,5) order by LineNum", id);
            ViewBag.Entities = _processBus.GetRemarks(sb.ToString());
            return View();
        }
    }
}