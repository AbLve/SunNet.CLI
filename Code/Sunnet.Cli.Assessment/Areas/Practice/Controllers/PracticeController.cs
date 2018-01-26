using Sunnet.Cli.Assessment.Controllers;
using Sunnet.Cli.Business.Ade;
using Sunnet.Cli.Business.Classes;
using Sunnet.Cli.Business.Communities;
using Sunnet.Cli.Business.Cpalls.Models;
using Sunnet.Cli.Business.Permission;
using Sunnet.Cli.Business.Practices;
using Sunnet.Cli.Business.Practices.Models;
using Sunnet.Cli.Business.Schools;
using Sunnet.Cli.Core.Ade;
using Sunnet.Cli.Core.Practices.Entites;
using Sunnet.Cli.Core.Practices.Entities;
using Sunnet.Cli.Core.Users.Enums;
using Sunnet.Cli.UIBase;
using Sunnet.Cli.UIBase.Filters;
using Sunnet.Cli.UIBase.Models;
using Sunnet.Framework;
using Sunnet.Framework.Core.Tool;
using Sunnet.Framework.Helpers;
using Sunnet.Framework.Permission;
using Sunnet.Framework.Resources;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sunnet.Cli.Business.Common;
using Sunnet.Cli.Core.Ade.Entities;
using Sunnet.Cli.Core.Cpalls;

namespace Sunnet.Cli.Assessment.Areas.Practice.Controllers
{
    public class PracticeController : BaseController
    {
        AdeBusiness _adeBusiness;
        PracticeBusiness _practiceBusiness;
        public PracticeController()
        {
            _adeBusiness = new AdeBusiness(AdeUnitWorkContext);
            _practiceBusiness = new PracticeBusiness(PracticeUnitWorkContext);
        }

        // GET: /Practice/
        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.Assessment, Anonymity = Anonymous.Verified)]
        public ActionResult Index(int assessmentId)
        {
            ViewBag.assessmentId = assessmentId;

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
            res = Upload(assessmentId, postFileBase, confirm);
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
            string sourceName = "";
            string fileName = "";
            try
            {
                res = ValidateExcel(postFileBase, out dt, out sourceName, out fileName);
            }
            catch (Exception ex)
            {
                res.Message = "Invalid excel file format.";
                res.ResultType = OperationResultType.Error;
            }
            if (res.ResultType == OperationResultType.Success)
            {
                string uploadPath = FileHelper.GetProtectedFilePhisycalPath(sourceName);
                IList<ExcelModel> excelList = new List<ExcelModel>();
                var findAssessment = _adeBusiness.GetAssessment(assessmentId);
                var otherAssessment = _adeBusiness.GetTheOtherLanguageAssessment(findAssessment.ID, findAssessment.Name);
                List<DemoStudentEntity> studentList; List<DemoStudentEntity> otherStudentList;
                var rowIndex = 1;
                foreach (DataRow dr in dt.Rows)
                {
                    rowIndex++;
                    ExcelModel student = new ExcelModel();

                    if (GetData(dr["Measure"]) != string.Empty)
                    {
                        student.MeaureName = GetData(dr["Measure"]); 
                    }
                    if (GetWaveData(dr["Wave"]) != null)
                    {
                        student.Wave = GetWaveData(dr["Wave"]).Value;
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
                    else
                    {
                        student.ItemValue = -1;
                    }
                    if (GetData(dr["Assessment language"]) != string.Empty)
                    {
                        student.AssessmentLanguage = GetAssessmentLanguage(dr["Assessment language"]);
                    }
                    if (!string.IsNullOrEmpty(student.StudentName))
                    {
                        //判断数据合法性
                        if ((string.IsNullOrEmpty(student.MeaureName) && !string.IsNullOrEmpty(student.ItemLabel)))
                        {
                            res.Message = "Row " + rowIndex + ": Measure cannot be null.";
                            res.ResultType = OperationResultType.Error;
                            DeleteFile(uploadPath);
                            return res;
                        }
                        if (!string.IsNullOrEmpty(student.MeaureName) && string.IsNullOrEmpty(student.ItemLabel))
                        {
                            res.Message = "Row " + rowIndex + ": Item cannot be null.";
                            res.ResultType = OperationResultType.Error;
                            DeleteFile(uploadPath);
                            return res;
                        }
                        if (!string.IsNullOrEmpty(student.MeaureName))
                        {
                            var isMeasureFind = IsMeasureExsit(findAssessment, otherAssessment, student.AssessmentLanguage,
                                student.MeaureName);
                            if (!isMeasureFind)
                            {
                                res.Message = "Row " + rowIndex + ": Cannot locate this Measure in the Assessment.";
                                res.ResultType = OperationResultType.Error;
                                DeleteFile(uploadPath);
                                return res;
                            }
                            var isItemFind = IsItemExsit(findAssessment, otherAssessment, student.AssessmentLanguage,
                                student.MeaureName, student.ItemLabel);
                            if (!isItemFind)
                            {
                                res.Message = "Row " + rowIndex + ": Cannot locate this Item in the Measure.";
                                res.ResultType = OperationResultType.Error;
                                DeleteFile(uploadPath);
                                return res;
                            }
                        }
                        excelList.Add(student);
                    }

                }

                res = _practiceBusiness.InsertStudents(findAssessment, otherAssessment, excelList, "", fileName, sourceName, out studentList, out otherStudentList);
                if (res.ResultType == OperationResultType.Success)
                {
                    if (studentList.Count > 0)
                    {
                        var targetAssessment = findAssessment;
                        if (otherAssessment != null && studentList[0].AssessmentId == otherAssessment.ID)
                            targetAssessment = otherAssessment;

                        res = _practiceBusiness.InsertStudentAssessment(UserInfo, excelList, targetAssessment, studentList, "FromExcel");
                    }
                    if (res.ResultType == OperationResultType.Success && otherAssessment != null)
                    {
                        var targetAssessment = otherAssessment;
                        if (studentList[0].AssessmentId == otherAssessment.ID)
                            targetAssessment = otherAssessment;
                        res = _practiceBusiness.InsertStudentAssessment(UserInfo, excelList, targetAssessment, otherStudentList, "FromExcel");
                    }
                }
            }
            return res;
        }

        private void DeleteFile(string fileName)
        {
            try
            {
                var file = new FileInfo(fileName);
                file.Delete();
            }
            catch (Exception)
            {

            }
        }

        private bool IsMeasureExsit(AssessmentEntity findAssessment, AssessmentEntity otherAssessment, StudentAssessmentLanguage language, string measureName)
        {
            var isMeasureFind = false;
            var hasMeasureFromAssessment = false;
            var hasMeasureFromOtherAssessment = false;
            hasMeasureFromAssessment = findAssessment.Measures.Any(c => c.Label == measureName);
            if (otherAssessment != null)
                hasMeasureFromOtherAssessment = otherAssessment.Measures.Any(c => c.Label == measureName);
            if ((int)findAssessment.Language == (int)language)
            {
                isMeasureFind = hasMeasureFromAssessment;
            }
            else if (otherAssessment != null && (int)otherAssessment.Language == (int)language)
            {
                isMeasureFind = hasMeasureFromOtherAssessment;
            }
            else if (language == StudentAssessmentLanguage.Bilingual)
            {
                isMeasureFind = (hasMeasureFromAssessment || hasMeasureFromOtherAssessment);
            }
            return isMeasureFind;
        }
        private bool IsItemExsit(AssessmentEntity findAssessment, AssessmentEntity otherAssessment, StudentAssessmentLanguage language, string measureName, string itemLabel)
        {
            bool isItemExsit = false;
            MeasureEntity targetMeasure = null;
            MeasureEntity measure = null;
            MeasureEntity otherMeasure = null;
            measure = findAssessment.Measures.FirstOrDefault(c => c.Label == measureName);
            if (otherAssessment != null)
                otherMeasure = otherAssessment.Measures.FirstOrDefault(c => c.Label == measureName);
            if ((int)findAssessment.Language == (int)language)
            {
                targetMeasure = measure;
            }
            else if (otherAssessment != null && (int)otherAssessment.Language == (int)language)
            {
                targetMeasure = otherMeasure;
            }
            else if (language == StudentAssessmentLanguage.Bilingual)
            {
                targetMeasure = (measure == null ? otherMeasure : measure);
            }
            if (targetMeasure != null)
            {
                isItemExsit = targetMeasure.Items.Any(c => c.Label == itemLabel);
            }
            return isItemExsit;
        }
        private OperationResult ValidateExcel(HttpPostedFileBase postFileBase, out DataTable dt, out string sourceName, out string fileName)
        {
            OperationResult res = new OperationResult(OperationResultType.Success);
            dt = new DataTable();
            sourceName = "";
            fileName = "";
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
            fileName = originFileName;
            var virtualPath = FileHelper.SaveProtectedFile(postFileBase, "DemoStudents");
            sourceName = virtualPath;
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

            var isValid = string.Equals(dt.Columns[0].ColumnName.Trim(), "Measure", StringComparison.CurrentCultureIgnoreCase) == true
                     &&
                      string.Equals(dt.Columns[1].ColumnName.Trim(), "Wave", StringComparison.CurrentCultureIgnoreCase) == true
                     &&
                     string.Equals(dt.Columns[2].ColumnName.Trim(), "Item label", StringComparison.CurrentCultureIgnoreCase) == true
                     &&
                     string.Equals(dt.Columns[3].ColumnName.Trim(), "Student name", StringComparison.CurrentCultureIgnoreCase) == true
                      &&
                     string.Equals(dt.Columns[4].ColumnName.Trim(), "Age years", StringComparison.CurrentCultureIgnoreCase) == true
                      &&
                     string.Equals(dt.Columns[5].ColumnName.Trim(), "Age months", StringComparison.CurrentCultureIgnoreCase) == true
                        &&
                     string.Equals(dt.Columns[6].ColumnName.Trim(), "Item Value", StringComparison.CurrentCultureIgnoreCase) == true
                        &&
                     string.Equals(dt.Columns[7].ColumnName.Trim(), "Assessment language", StringComparison.CurrentCultureIgnoreCase) == true
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

        private Wave? GetWaveData(object o)
        {
            if (o is DBNull) return null;
            switch (o.ToString().Trim().ToLower())
            {
                case "b":
                case "boy":
                case "1": return Wave.BOY;
                case "m":
                case "moy":
                case "2": return Wave.MOY;
                case "e":
                case "eoy":
                case "3": return Wave.EOY;
            }
            return null;
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

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.ADE, Anonymity = Anonymous.Verified)]
        [ValidateInput(false)]
        public string Search(int assessmentId, string studentName = "", string sort = "StudentName", string order = "Asc", int first = 0, int count = 10)
        {
            var total = 0;
            var expression = PredicateHelper.True<DemoStudentEntity>();
            expression = expression.And(c => c.AssessmentId == assessmentId);
            if (studentName != null && studentName.Trim() != string.Empty)
            {
                studentName = studentName.Trim();
                expression = expression.And(s => s.StudentName.Contains(studentName));
            }
            var list = _practiceBusiness.GetStudents(expression, sort, order, first, count, out total);

            var result = new { total = total, data = list };
            return JsonHelper.SerializeObject(result);
        }


        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.TCD, Anonymity = Anonymous.Verified)]
        public ActionResult DownloadDemoStudents(int studentId)
        {
            var demostudent = _practiceBusiness.GetStudent(studentId);
            if (demostudent != null)
            {
                var filePath = FileHelper.GetProtectedFilePhisycalPath(demostudent.Source);
                FileHelper.ResponseFile(filePath, demostudent.FileName);
            }
            return new EmptyResult();
        }
    }
}