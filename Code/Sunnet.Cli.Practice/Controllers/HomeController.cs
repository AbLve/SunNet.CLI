using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sunnet.Cli.Business.Ade;
using Sunnet.Cli.Business.Classes;
using Sunnet.Cli.Business.Communities;
using Sunnet.Cli.Business.Cpalls.Models;
using Sunnet.Cli.Business.Permission;
using Sunnet.Cli.Business.Practices;
using Sunnet.Cli.Business.Practices.Models;
using Sunnet.Cli.Business.Schools;
using Sunnet.Cli.Business.Users;
using Sunnet.Cli.Core.Ade;
using Sunnet.Cli.Core.Practices.Entites;
using Sunnet.Cli.Core.Practices.Entities;
using Sunnet.Cli.Core.Users.Entities;
using Sunnet.Cli.Core.Users.Enums;
using Sunnet.Cli.UIBase;
using Sunnet.Cli.UIBase.Filters;
using Sunnet.Cli.UIBase.Models;
using Sunnet.Framework;
using Sunnet.Framework.Core.Tool;
using Sunnet.Framework.Helpers;
using Sunnet.Framework.Permission;
using Sunnet.Framework.Resources;
using Sunnet.Framework.SSO;

namespace Sunnet.Cli.Practice.Controllers
{
    public class HomeController : BaseController
    {
        AdeBusiness _adeBusiness;
        CommunityBusiness _community;
        SchoolBusiness _schoolBusiness;
        ClassBusiness _classBusiness;
        PracticeBusiness _practiceBusiness;
        public HomeController()
        {
            _adeBusiness = new AdeBusiness();
           _community = new CommunityBusiness(UnitWorkContext);
            _schoolBusiness = new SchoolBusiness(UnitWorkContext);
         _classBusiness = new ClassBusiness(UnitWorkContext);
        _practiceBusiness = new PracticeBusiness(PracticeUnitWorkContext);
        }

        // GET: /Practice/
        [CLIUrlAuthorize(Account = Authority.All, PageId = PagesModel.Assessment, Anonymity = Anonymous.Verified)]
        public ActionResult Index(string showmessage = "")
        {
            if (UserAuthentication != AuthenticationStatus.Login)
                return new RedirectResult(string.Format("{0}home/Index?{1}", DomainHelper.SsoSiteDomain, BuilderLoginUrl("40")));

            List<int> accountPageId = new PermissionBusiness().CheckPage(UserInfo);
            var practiceList = new PermissionBusiness().CheckPracticePage(UserInfo);
            ViewBag.ShowCPALLS = accountPageId.Contains((int)PagesModel.CPALLS);
            List<int> pageIds = practiceList.FindAll(r => r > SFConfig.AssessmentPageStartId);
            List<int> featureAssessmentIds = _community.GetFeatureAssessmentIds(UserInfo);

            HttpContext.Response.Cache.SetNoStore();
            ViewBag.School = false;
            if (!string.IsNullOrEmpty(showmessage))
            {
                ViewBag.Message = ResourceHelper.GetRM().GetInformation(showmessage);
            }
            List<CpallsAssessmentModel> list = _adeBusiness.GetAssessmentCpallsList();
            List<CpallsAssessmentModel> accessList = list.FindAll(r => pageIds.Contains(r.ID + SFConfig.AssessmentPageStartId));
            if (UserInfo.Role > Role.Mentor_coach)
                accessList = accessList.FindAll(r => featureAssessmentIds.Contains(r.ID));

            List<CpallsAssessmentModel> English = accessList.FindAll(r => r.Language == AssessmentLanguage.English);
            List<CpallsAssessmentModel> newList = new List<CpallsAssessmentModel>();

            foreach (CpallsAssessmentModel model in accessList)
            {
                if (model.Language == AssessmentLanguage.Spanish)
                {
                    if (English.Find(r => r.Name == model.Name) != null)
                        continue;
                }
                newList.Add(model);
            }
            ViewBag.List = newList;
            return View();
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.ADE, Anonymity = Anonymous.Verified)]
        public ActionResult DemoRecord(int assessmentId)
        {
            ViewBag.assessmentId = assessmentId;
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.ADE, Anonymity = Anonymous.Verified)]
        public string UploadDemoRoster()
        {
            int assessmentId = 0;
            var res = new OperationResult(OperationResultType.Success);
            bool confirm = false;
            int.TryParse(Request.Params["assessmentId"], out assessmentId);
            bool.TryParse(Request.Params["confirm"], out confirm);
            HttpPostedFileBase postFileBase = Request.Files["dataFile"];
            var response = new PostFormResponse();
            //  OperationResult res = Upload(communityId, schoolId, teacherId, classId, classDayType, postFileBase, confirm);
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
        private OperationResult Upload(int assessmentId, HttpPostedFileBase postFileBase, bool isComfirm = false)
        {
            var res = new OperationResult(OperationResultType.Success);
            var response = new PostFormResponse();
            DataTable dt = new DataTable();
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
                IList<ExcelModel> excelList = new List<ExcelModel>();

                foreach (DataRow dr in dt.Rows)
                {
                    ExcelModel student = new ExcelModel();

                    if (GetData(dr["Measure"]) != string.Empty)
                    {
                        student.MeaureName = GetData(dr["Measure"]);
                    }
                    if (GetData(dr["Item label"]) != string.Empty)
                    {
                        student.ItemLabel = GetData(dr["Item label"]);
                    }
                    if (GetData(dr["Student name"]) != string.Empty)
                    {
                        student.StudentName = GetData(dr["Student name"]);
                    }
                    if (GetData(dr["Age years"]) != string.Empty)
                    {
                        student.AgeYear = GetNumber(dr["Age years"]);
                    }
                    if (GetData(dr["Age months"]) != string.Empty)
                    {
                        student.AgeMonth = GetNumber(dr["Age months"]);
                    }
                    if (GetData(dr["Item Value"]) != string.Empty)
                    {
                        student.ItemValue = GetNumber(dr["Item Value"]);
                    }
                    if (GetData(dr["Assesment language"]) != string.Empty)
                    {
                        student.AssessmentLanguage = GetAssessmentLanguage(dr["Assesment language"]);
                    }
                    excelList.Add(student);
                }


                //res = CheckStudents(excelList);
                //if (res.ResultType == OperationResultType.Success)
                //{
                //    foreach (var studentEntity in listStu)
                //    {
                //        res = SaveStudent(schoolId, findClass.ID, studentEntity);
                //        if (res.ResultType != OperationResultType.Success)
                //        {
                //            break;
                //        }
                //    }
                //}

            }
            return res;
        }
        public OperationResult InsertStudents(IList<ExcelModel> excelList, out List<DemoStudentEntity> studentList)
        {
            studentList = new List<DemoStudentEntity>();
            List<PracticeStudentAssessmentEntity> asList = new List<PracticeStudentAssessmentEntity>();
            List<PracticeStudentMeasureEntity> smList = new List<PracticeStudentMeasureEntity>();
            List<PracticeStudentItemEntity> stuItemList = new List<PracticeStudentItemEntity>();
            OperationResult res = new OperationResult(OperationResultType.Success);
            foreach (var excel in excelList)
            {
                if (!studentList.Any(c => c.StudentName == excel.StudentName && c.StudentAgeYear == excel.AgeYear && c.StudentAgeMonth == excel.AgeMonth))
                {
                    var demoStudent = new DemoStudentEntity
                    {
                        StudentName = excel.StudentName,
                        StudentDob = DateTime.MinValue,
                        StudentAgeYear = excel.AgeYear,
                        StudentAgeMonth = excel.AgeMonth,
                        AssessmentLanguage = excel.AssessmentLanguage,
                        DataFrom = "",
                        Remark = ""
                    };
                    studentList.Add(demoStudent);
                }
            }
            //   res = _practiceBusiness.InsertStudents(studentList);
            return res;
        }
        public OperationResult InsertStudentMeasures(IList<ExcelModel> excelList, out List<DemoStudentEntity> studentList)
        {
            studentList = new List<DemoStudentEntity>();
            List<PracticeStudentAssessmentEntity> asList = new List<PracticeStudentAssessmentEntity>();
            List<PracticeStudentMeasureEntity> smList = new List<PracticeStudentMeasureEntity>();
            List<PracticeStudentItemEntity> stuItemList = new List<PracticeStudentItemEntity>();
            OperationResult res = new OperationResult(OperationResultType.Success);
            foreach (var excel in excelList)
            {
                if (!studentList.Any(c => c.StudentName == excel.StudentName && c.StudentAgeYear == excel.AgeYear && c.StudentAgeMonth == excel.AgeMonth))
                {
                    var demoStudent = new DemoStudentEntity
                    {
                        StudentName = excel.StudentName,
                        StudentDob = DateTime.MinValue,
                        StudentAgeYear = excel.AgeYear,
                        StudentAgeMonth = excel.AgeMonth,
                        AssessmentLanguage = excel.AssessmentLanguage ,
                        DataFrom = "",
                        Remark = ""
                    };
                    studentList.Add(demoStudent);
                }
            }
            //   res = _practiceBusiness.InsertStudents(studentList);
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

            string strSQL = " SELECT * FROM [DRU$] ";

            OleDbDataAdapter cmd = new OleDbDataAdapter(strSQL, cnn);

            cmd.Fill(dt);
            cnn.Close();
            if (dt == null || dt.Rows.Count < 1 || dt.Columns.Count < 8)
            {
                res.Message = "Datasource error.";
                res.ResultType = OperationResultType.Error;
                return res;
            }

            var isValid = string.Equals(dt.Columns[0].ColumnName, "Measure", StringComparison.CurrentCultureIgnoreCase) == true
                     &&
                     string.Equals(dt.Columns[1].ColumnName, "Item label", StringComparison.CurrentCultureIgnoreCase) == true
                     &&
                     string.Equals(dt.Columns[2].ColumnName, "Student name", StringComparison.CurrentCultureIgnoreCase) == true
                      &&
                     string.Equals(dt.Columns[3].ColumnName, "Age years", StringComparison.CurrentCultureIgnoreCase) == true
                      &&
                     string.Equals(dt.Columns[4].ColumnName, "Age months", StringComparison.CurrentCultureIgnoreCase) == true
                        &&
                     string.Equals(dt.Columns[5].ColumnName, "Item Value", StringComparison.CurrentCultureIgnoreCase) == true
                        &&
                     string.Equals(dt.Columns[6].ColumnName, "Assesment language", StringComparison.CurrentCultureIgnoreCase) == true
                   ;
            if (!isValid)
            {
                res.Message = "Datasource error.";
                res.ResultType = OperationResultType.Error;
                return res;
            }
            return res;
        }
        private StudentAssessmentLanguage GetAssessmentLanguage(object o)
        {
            if (o is DBNull) return StudentAssessmentLanguage.English;
            var str = SqlHelper.ReplaceSqlChar(o.ToString().Trim());
            switch (str)
            {
                case "English":
                case "Eng":
                case "En":
                    return StudentAssessmentLanguage.English;
                case "Sp":
                case "Spanish":
                    return StudentAssessmentLanguage.Spanish;
                case "En and Sp":
                case "Bilingual":
                    return StudentAssessmentLanguage.Bilingual;
            }
            return StudentAssessmentLanguage.NonApplicable;
        }
        private string GetData(object o)
        {
            if (o is DBNull) return string.Empty;
            return SqlHelper.ReplaceSqlChar(o.ToString().Trim());
        }
        private int GetNumber(object o)
        {
            if (o is DBNull) return -1;
            string number = o.ToString().Trim();
            int tmpNum;
            if (int.TryParse(number, out tmpNum))
                return tmpNum;
            else return -1;
        }


        /// <summary>
        /// 专供ssol调用
        /// </summary>
        public ActionResult CallBack(string IASID = "", string TimeStamp = "", string Authenticator = "", string UserAccount = "")
        {
            if (!string.IsNullOrEmpty(IASID)
               && !string.IsNullOrEmpty(TimeStamp)
               && !string.IsNullOrEmpty(Authenticator)
               && !string.IsNullOrEmpty(UserAccount))
            {
                SSORequest ssoRequest = new SSORequest();
                ssoRequest.IASID = IASID;
                ssoRequest.TimeStamp = TimeStamp;
                ssoRequest.AppUrl = string.Format("{0}Home/CallBack", DomainHelper.VcwDomain.ToString());
                ssoRequest.UserAccount = UserAccount;//google Id
                ssoRequest.Authenticator = Authenticator;

                if (Authentication.ValidateCenterToken(ssoRequest))
                {
                    //检查是否过期
                    if (string.IsNullOrEmpty(TimeStamp))
                    {
                        CookieHelper.RemoveAll();
                        return new RedirectResult(string.Format("{0}home/logout", DomainHelper.SsoSiteDomain));
                    }
                    else
                    {
                        var sendTime = new DateTime(long.Parse(TimeStamp));
                        if (sendTime < DateTime.Now.AddMinutes(-2) || sendTime > DateTime.Now.AddMinutes(2))
                        {
                            CookieHelper.RemoveAll();
                            return new RedirectResult(string.Format("{0}home/logout", DomainHelper.SsoSiteDomain));
                        }
                    }
                    UserBaseEntity user = new UserBusiness(UnitWorkContext).UserLogin(UserAccount);
                    if (user != null)
                    {
                        LocalSignIn(user.ID, user.GoogleId, user.FirstName);
                        return new RedirectResult("/home/");
                    }
                }
            }
            return new RedirectResult(DomainHelper.MainSiteDomain.ToString());
        }

    }
}