using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Sunnet.Cli.Assessment.Controllers;
using Sunnet.Cli.Business.Ade;
using Sunnet.Cli.Business.Classes;
using Sunnet.Cli.Business.Common;
using Sunnet.Cli.Business.Communities;
using Sunnet.Cli.Business.Cot;
using Sunnet.Cli.Business.Cpalls.Models;
using Sunnet.Cli.Business.Observable.Models;
using Sunnet.Cli.Business.Permission;
using Sunnet.Cli.Business.Schools;
using Sunnet.Cli.Business.Students;
using Sunnet.Cli.Core.Ade;
using Sunnet.Cli.Core.Classes.Entites;
using Sunnet.Cli.Core.Common.Enums;
using Sunnet.Cli.Core.Communities.Entities;
using Sunnet.Cli.Core.Observable.Entities;
using Sunnet.Cli.Core.Observable.Enums;
using Sunnet.Cli.Core.Schools.Entities;
using Sunnet.Cli.Core.Students.Entities;
using Sunnet.Cli.Core.Users.Enums;
using Sunnet.Cli.UIBase;
using Sunnet.Cli.UIBase.Filters;
using Sunnet.Cli.UIBase.Models;
using Sunnet.Framework;
using Sunnet.Framework.Core.Tool;
using Sunnet.Framework.Extensions;
using Sunnet.Framework.Helpers;
using Sunnet.Framework.PDF;
using Sunnet.Framework.Permission;
using Sunnet.Framework.Resources;

namespace Sunnet.Cli.Assessment.Areas.Observable.Controllers
{
    public class ObservableController : BaseController
    {
        AdeBusiness _adeBusiness;
        private StudentBusiness _studentBusiness;
        CommunityBusiness _communityBusiness;
        private SchoolBusiness _schoolBusiness;
        private ClassBusiness _classBusiness;
        private ObservableBusiness _observableBusiness;
        public ObservableController()
        {
            _adeBusiness = new AdeBusiness(AdeUnitWorkContext);
            _communityBusiness = new CommunityBusiness(UnitWorkContext);
            _studentBusiness = new StudentBusiness(UnitWorkContext);
            _schoolBusiness = new SchoolBusiness(UnitWorkContext);
            _classBusiness = new ClassBusiness(UnitWorkContext);
            _observableBusiness = new ObservableBusiness(AdeUnitWorkContext);
        }

        // GET: /Observable/Observable/
        [CLIUrlAuthorize(Account = Authority.All, PageId = PagesModel.TCD, Anonymity = Anonymous.Verified)]
        public ActionResult Index(string showmessage = "")
        {
            List<int> accountPageIds = new PermissionBusiness().CheckPage(UserInfo);

            ViewBag.ShowTCD = accountPageIds.Contains((int)PagesModel.TCD);
            List<int> pageIds = accountPageIds.FindAll(r => r > SFConfig.AssessmentPageStartId);

            HttpContext.Response.Cache.SetNoStore();
            List<CpallsAssessmentModel> list = _adeBusiness.GetAssessmentList(o => o.Type == AssessmentType.UpdateObservables);
            List<CpallsAssessmentModel> accessTCDList = list.FindAll(r => pageIds.Contains(r.ID + SFConfig.AssessmentPageStartId));
            ViewBag.List = accessTCDList;
            return View();
        }
        [CLIUrlAuthorize(Account = Authority.All, PageId = PagesModel.TCD, Anonymity = Anonymous.Verified)]
        public ActionResult ObservableList(int studentId = 0, int childId = 0)
        {
            List<int> accountPageIds = new PermissionBusiness().CheckPage(UserInfo);
            ViewBag.ShowTCD = accountPageIds.Contains((int)PagesModel.TCD);
            List<int> pageIds = accountPageIds.FindAll(r => r > SFConfig.AssessmentPageStartId);
            HttpContext.Response.Cache.SetNoStore();

            List<CpallsAssessmentModel> list = _adeBusiness.GetAssessmentList(o => o.Type == AssessmentType.UpdateObservables);
            List<CpallsAssessmentModel> accessTCDList = list.FindAll(r => pageIds.Contains(r.ID + SFConfig.AssessmentPageStartId));
            ViewBag.List = accessTCDList;
            ViewBag.StudentId = studentId;
            ViewBag.ChildId = childId;
            return View();
        }

        // GET: /Observable/Observable/
        [CLIUrlAuthorize(Account = Authority.All, PageId = PagesModel.TCD, Anonymity = Anonymous.Verified)]
        public ActionResult Detail(int assessmentId, int studentId = 0, int childId = 0)
        {
            var baseAssessment = _adeBusiness.GetBaseAssessmentModel(assessmentId);
            if (baseAssessment == null) return new EmptyResult();
            ViewBag.AssessmentName = baseAssessment.Name;

            if (!_adeBusiness.CanExecuteAssessment(assessmentId))
                return RedirectToAction("Index", "Dashboard", new { Area = "", showMessage = "assessment_unavaiable" });

            ViewBag.assessmentId = assessmentId;
            ViewBag.studentId = studentId;
            ViewBag.childId = childId;
            string objectName = "";
            if (studentId != 0)
            {
                var student = _studentBusiness.GetStudentById(studentId);
                objectName = student.FirstName + " " + student.LastName;
            }
            else if (childId != 0)
            {
                var child = _studentBusiness.GetChildById(childId, UserInfo);
                if (child != null)
                    objectName = child.FirstName + " " + child.LastName;
            }
            var assessment = _observableBusiness.GetAssessmentFromCache(assessmentId);
            var oldAnswers = _observableBusiness.GetAssessmentItemEntities(0, assessmentId, studentId, childId);
            foreach (var item in assessment.Items)
            {

                if (item.IsShown)
                {
                    var findItem = oldAnswers.FirstOrDefault(c => c.ItemId == item.ItemId);
                    if (findItem != null)
                    {
                        item.Res = findItem.Response;
                        item.Date = findItem.UpdatedOn.ToString("MM/dd/yyyy");
                    }
                }
            }
            ViewBag.objectName = objectName;
            ViewBag.Json = JsonHelper.SerializeObject(assessment);

            return View();
        }
        // GET: /Observable/Observable/


        // GET: /Observable/Observable/
        [CLIUrlAuthorize(Account = Authority.All, PageId = PagesModel.TCD, Anonymity = Anonymous.Verified)]
        public ActionResult AccessResults(int assessmentId)
        {
            HttpContext.Response.Cache.SetNoStore();
            _adeBusiness.LockAssessment(assessmentId);
            var list = _adeBusiness.GetAssessmentList(o => o.Type == AssessmentType.UpdateObservables);
            ViewBag.List = list;
            ViewBag.AssessmentId = assessmentId;
            ViewBag.AssessmentName = _adeBusiness.GetAssessment(assessmentId).Name;
            ViewBag.StatusOptions = ReportStatus.ShowAll.ToSelectList(true);
            ViewBag.UserRole = UserInfo.Role;
            return View();
        }

        [CLIUrlAuthorize(Account = Authority.All, PageId = PagesModel.TCD, Anonymity = Anonymous.Verified)]
        public ActionResult ReportHistory(int assessmentId, int studentId, int childId)
        {
            ViewBag.StudentId = studentId;
            ViewBag.ChildId = childId;
            ViewBag.AssessmentId = assessmentId;
            return View();
        }
        [CLIUrlAuthorize(Account = Authority.All, PageId = PagesModel.TCD, Anonymity = Anonymous.Verified)]
        [ValidateInput(false)]
        public string SearchHistory(int studentId, int childId, int assessmentId,
             string sort = "Name", string order = "Asc", int first = 0, int count = 10)
        {
            DateTime birthDate = CommonAgent.MinDate;
            if (studentId > 0)
            {
                var student = _studentBusiness.GetStudentEntity(studentId, UserInfo);
                if (student != null)
                {
                    birthDate = student.BirthDate;
                }
            }
            else
            {
                var child = _studentBusiness.GetChildById(childId, UserInfo);
                if (child != null)
                    birthDate = child.BirthDate;
            }
            var total = 0;
            IList<ObservableReportModel> list = new List<ObservableReportModel>();
            if (birthDate != CommonAgent.MinDate)
            {
                list = _observableBusiness.SearchObervableReportHistory(assessmentId, studentId, childId, birthDate, sort, order, first, count, out total);
            }
            var result = new { total = total, data = list };
            return JsonHelper.SerializeObject(result);
        }


        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.TCD, Anonymity = Anonymous.Logined)]
        [ValidateInput(false)]
        public string Search(int assessmentId = 0, int communityId = -1, string communityName = "", int schoolId = -1, string schoolName = "", string teacherName = "", int classId = -1, string className = "",
              string studentName = "", int status = 0, DateTime? reportBegin = null, DateTime? reportEnd = null,
              string sort = "Name", string order = "Asc", int first = 0, int count = 10)
        {
            var total = 0;
            List<int> studentIds = new List<int>();
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
            if (teacherName != "")
            {
                expression = expression.And(s => s.Classes.Any(c => c.Teachers.Any(t => t.UserInfo.FirstName.Contains(teacherName) || t.UserInfo.LastName.Contains(teacherName))));
            }
            if (classId >= 1)
                expression = expression.And(s => s.Classes.Count(o => o.ID == classId && o.IsDeleted == false) > 0);

            //当用户为SuperAdmin时，如果继续查询ClassId，会查询出几万条数据，在进行Contains处理时，SQL长度会超出限制
            if (UserInfo.Role == Role.Super_admin)
            {
                if (communityId > 0)
                {
                    IList<int> classIds = _classBusiness.GetClassIdsForReport(communityId, assessmentId);
                    expression = expression.And(s => s.Classes.Any(c => classIds.Contains(c.ID)));
                }
            }
            else
            {
                if (assessmentId > 0 && communityId > 0)
                {
                    IList<int> classIds = _classBusiness.GetClassIdsForReport(communityId, assessmentId);
                    expression = expression.And(s => s.Classes.Any(c => classIds.Contains(c.ID)));
                }
                else
                {
                    IList<int> classIds = _classBusiness.GetClassIdsForReport(UserInfo, assessmentId);
                    expression = expression.And(s => s.Classes.Any(c => classIds.Contains(c.ID)));
                }
            }


            if (studentName != null && studentName.Trim() != string.Empty)
            {
                studentName = studentName.Trim();
                expression = expression.And(s => s.FirstName.Contains(studentName)
                    || s.MiddleName.Contains(studentName)
                    || s.LastName.Contains(studentName));
            }
            if (reportBegin != null || reportEnd != null)
            {
                studentIds = _observableBusiness.GetAssessmentReportstudentIds(assessmentId, status, reportBegin, reportEnd);
                expression = expression.And(s => studentIds.Contains(s.ID));
            }
            var list = _studentBusiness.SearchStudents(UserInfo, expression, sort, order, first, count, out total);

            foreach (var studentModel in list)
            {
                studentModel.Reports = _observableBusiness.GetAssessmentRports(assessmentId, studentModel.ID, status, reportBegin, reportEnd);
            }
            var result = new { total = total, data = list };
            return JsonHelper.SerializeObject(result);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.TCD, Anonymity = Anonymous.Logined)]
        public String GetClassSelectList(int communityId = 0, int schoolId = 0, int assessmentId = 0)
        {
            var expression = PredicateHelper.True<ClassEntity>();
            if (communityId > 0)
                expression = expression.And(o => o.School
                    .CommunitySchoolRelations.Count(r => r.CommunityId == communityId) > 0);
            if (schoolId > 0)
                expression = expression.And(o => o.SchoolId == schoolId);
            if (assessmentId > 0)
            {
                IList<int> classIds = _classBusiness.GetClassIdsForReport(communityId, assessmentId);
                expression = expression.And(o => classIds.Contains(o.ID));
            }
            return JsonHelper.SerializeObject(
                ListToDDL(_classBusiness.GetClassSelectList(UserInfo, expression), ViewTextHelper.DefaultAllText, "-1"));
        }
        private IEnumerable<SelectListItem> ListToDDL(IEnumerable<SelectItemModel> list, string defaultText = "", string defaultValue = "")
        {
            if (defaultText == "") defaultText = ViewTextHelper.DefaultPleaseSelectText;
            return list.ToSelectList(defaultText, defaultValue);
        }

        #region 搜索条件
        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.TCD, Anonymity = Anonymous.Logined)]
        public string GetCommunitySelectListForSearch(string keyword, int communityId = -1, bool isActiveCommunity = true)
        {
            var expression = PredicateHelper.True<CommunityEntity>();
            if (communityId > 0)
                expression = expression.And(o => o.ID == communityId);
            var list = _communityBusiness.GetCommunitySelectList(UserInfo, expression, isActiveCommunity);
            return JsonHelper.SerializeObject(list);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.TCD, Anonymity = Anonymous.Logined)]
        public string GetSchoolSelectList(string keyword, int communityId = 0, string schoolName = "", bool isActive = true)
        {
            var expression = PredicateHelper.True<SchoolEntity>();

            if (communityId > 0)
                expression = expression.And(o => o.CommunitySchoolRelations.Count(r => r.CommunityId == communityId) > 0);
            if (schoolName != null && schoolName.Trim() != string.Empty)
                expression = expression.And(o => o.Name.Contains(schoolName));
            var schoolList = _schoolBusiness.GetSchoolsSelectList(UserInfo, expression, isActive);
            return JsonHelper.SerializeObject(schoolList);
        }
        #endregion

        [HttpPost]
        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.TCD, Anonymity = Anonymous.Logined)]
        public string SaveObservableAssessment(int assessmentId, int studentId, int childId, string hidAnswers, DateTime? AssessmentDate)
        {
            var response = new PostFormResponse();
            var res = new OperationResult(OperationResultType.Success);
            if (hidAnswers == "")
            {
                res.Message = "No Item changed.";
                return JsonHelper.SerializeObject(res);
            }
            var listAnswer = JsonHelper.DeserializeObject<List<ItemAnswer>>(hidAnswers);
            listAnswer = listAnswer.Where(c => c.Res != null).ToList();

            var needSaveList = new List<ObservableAssessmentItemEntity>();
            var assessmentModel = new ObservableAssessmentEntity();
            assessmentModel.StudentId = studentId;
            assessmentModel.ChildId = childId;
            assessmentModel.AssessmentId = assessmentId;
            assessmentModel.Status = EntityStatus.Active;
            assessmentModel.ReportUrl = "";
            assessmentModel.ReportName = "";
            assessmentModel.CreatedOn = AssessmentDate == null ? DateTime.Now : AssessmentDate.Value;
            assessmentModel.CreatedBy = UserInfo.ID;
            assessmentModel.UpdatedBy = UserInfo.ID;


            var oldAnswers = _observableBusiness.GetAssessmentItemEntities(0, assessmentId, studentId, childId);
            foreach (var itemAnswer in listAnswer)
            {
                var answer = new ObservableAssessmentItemEntity
                {
                    ItemId = itemAnswer.ItemId,
                    ObservableAssessmentId = assessmentModel.ID,
                    Response = itemAnswer.Res,
                    CreatedBy = UserInfo.ID,
                    UpdatedBy = UserInfo.ID,
                    CreatedOn = assessmentModel.CreatedOn,
                    UpdatedOn = assessmentModel.CreatedOn
                };
                var findItem = oldAnswers.FirstOrDefault(c => c.ItemId == answer.ItemId);
                if (findItem != null)
                {
                    if (findItem.Response != answer.Response)
                    {
                        findItem.Response = answer.Response;
                        findItem.ObservableAssessmentId = assessmentModel.ID;
                        findItem.UpdatedBy = answer.UpdatedBy;
                        findItem.UpdatedOn = answer.UpdatedOn;
                        needSaveList.Add(findItem);
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(answer.Response))
                        needSaveList.Add(answer);
                }
            }
            if (needSaveList.Count > 0)
            {
                res = _observableBusiness.SaveObservableAssessment(assessmentModel);
                if (res.ResultType == OperationResultType.Success)
                {
                    foreach (var item in needSaveList)
                    {
                        item.ObservableAssessmentId = assessmentModel.ID;
                        res = _observableBusiness.SaveAssessmentItems(item);
                    }
                }
            }
            else
            {
                res.ResultType = OperationResultType.Warning;
                res.Message = "No Item changed.";
            }
            res.AppendData = assessmentModel;
            response.Success = res.ResultType == OperationResultType.Success;
            response.Message = res.Message;
            response.Data = assessmentModel.ID;
            return JsonHelper.SerializeObject(response);
        }

        private string GetReportName(int studentId, DateTime createOn, string assessmentName)
        {
            return _observableBusiness.GetAssessmentReportName(studentId, createOn, assessmentName);
        }

        #region Report
        [CLIUrlAuthorize(Account = Authority.All, PageId = PagesModel.TCD, Anonymity = Anonymous.Verified)]
        public ActionResult View(int studentId, int childId, int assessmentId, int observableId)
        {
            var baseAssessment = _adeBusiness.GetBaseAssessmentModel(assessmentId);
            if (baseAssessment == null) return new EmptyResult();
            ViewBag.AssessmentName = baseAssessment.Name;
            if (!_adeBusiness.CanExecuteAssessment(assessmentId))
                return RedirectToAction("Index", "Dashboard", new { Area = "", showMessage = "assessment_unavaiable" });
            ViewBag.assessmentId = assessmentId;
            var assessment = _observableBusiness.GetAssessmentFromCache(assessmentId);
            var oldAnswers = _observableBusiness.GetAssessmentItemEntities(0, assessmentId, studentId, childId);
            var assessmentModel = _observableBusiness.GetAssessmentModel(observableId);
            foreach (var item in assessment.Items)
            {
                var findItem = oldAnswers.FirstOrDefault(c => c.ItemId == item.ItemId);
                if (findItem != null)
                {
                    item.Res = findItem.Response;
                    item.Date = findItem.UpdatedOn.ToString("MM/dd/yyyy");
                }
            }
            assessmentModel.ReportName = GetReportName(studentId, assessmentModel.CreatedOn, assessment.Name);
            ViewBag.AssessmentModel = assessment;
            ViewBag.ObservableAssessmentModel = assessmentModel;
            string filename = "Observables/" + assessmentId + "/" + studentId + "/" + assessmentModel.ReportName + ".pdf";
            var localFile = FileHelper.HasProtectedFile(filename);
            if (string.IsNullOrEmpty(localFile))
            {
                localFile = FileHelper.GetProtectedFilePhisycalPath(filename);
            }
            ViewBag.Json = JsonHelper.SerializeObject(assessment);
            ViewBag.Pdf = true;
            ViewBag.UserRole = UserInfo.Role;

            ViewBag.ObjectName = "";
            ViewBag.DOB = "";
            ViewBag.CommunityName = "";
            ViewBag.SchoolName = "";
            ViewBag.SchoolYear = "";
            ViewBag.ClassName = "";
            ViewBag.TeacherName = "";
            ViewBag.studentId = studentId;

            if (studentId > 0)
            {
                var student = _studentBusiness.GetStudentById(studentId);
                ViewBag.ObjectName = student.FirstName + " " + student.LastName;
                ViewBag.DOB = student.BirthDate.ToString("MM/dd/yyyy");

                List<int> schoolIdList = student.Classes.Select(c => c.SchoolId).ToList();
                if (schoolIdList != null)
                {
                    IEnumerable<string> communityNameList = _communityBusiness.GetCommunityNames(r => r.CommunitySchoolRelations.Any(s => schoolIdList.Contains(s.SchoolId)));
                    ViewBag.CommunityName = string.Join(", ", communityNameList);
                }

                List<string> schoolNameList = student.SchoolRelations.Select(r => r.School.Name).ToList();
                if (schoolNameList != null)
                {
                    ViewBag.SchoolName = string.Join(", ", schoolNameList);
                }
                ViewBag.SchoolYear = CommonAgent.ToFullSchoolYearString(student.SchoolYear);

                List<string> classNameList = student.Classes.Select(c => c.Name).ToList();
                if (classNameList != null)
                {
                    ViewBag.ClassName = string.Join(", ", classNameList);
                }

                List<string> teacherNameList = student.Classes.Where(c => c.LeadTeacher != null).Select(c => c.LeadTeacher.UserInfo.FirstName + " " + c.LeadTeacher.UserInfo.LastName).ToList();
                if (teacherNameList != null)
                {
                    ViewBag.TeacherName = string.Join(", ", teacherNameList);
                }
            }
            else
            {
                var child = _studentBusiness.GetChildById(childId, UserInfo);
                if (child != null)
                {
                    ViewBag.ObjectName = child.FirstName + " " + child.LastName;
                    ViewBag.DOB = child.BirthDate;
                    ViewBag.SchoolName = child.School == null ? "" : child.School.Name;
                }
            }

            ViewBag.PerformBy = UserInfo.FirstName + " " + UserInfo.LastName;

            GetPdf(GetViewHtml("View"), localFile);

            assessmentModel.ReportUrl = localFile;
            _observableBusiness.SaveObservableAssessment(assessmentModel);
            if (Request.QueryString["returnurl"] != null)
            {
                Response.Redirect(Server.UrlDecode(Request.QueryString["returnurl"]));
            }
            else
            {
                Response.Redirect("AccessResults?assessmentId=" + assessmentId.ToString());
            }
            return View();
        }



        private void GetPdf(string html, string fileName, PdfType type = PdfType.Observable)
        {
            PdfProvider pdfProvider = new PdfProvider(type, "", "", "", "#avoidPageBreak");
            pdfProvider.SavePdf(html, fileName);
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

        [CLIUrlAuthorize(Account = Authority.All, PageId = PagesModel.TCD, Anonymity = Anonymous.Verified)]
        public string DeleteReport(int ID)
        {
            var res = _observableBusiness.DeleteReport(ID);
            var response = new PostFormResponse();
            response.Success = res.ResultType == OperationResultType.Success;
            response.Message = res.Message;
            return JsonHelper.SerializeObject(response);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.TCD, Anonymity = Anonymous.Verified)]
        public ActionResult AssessmentResults(int id)
        {
            var model = _observableBusiness.GetAssessmentModel(id);
            if (model != null)
            {
                int total = 0;
                if (model.StudentId > 0)
                {
                    var condition = PredicateHelper.True<StudentEntity>();
                    condition = condition.And(c => c.ID == model.StudentId);
                    var students = _studentBusiness.SearchStudents(UserInfo, condition, "ID", "asc", 1, int.MaxValue, out total);
                }
                else
                {
                    var condition = PredicateHelper.True<ChildEntity>();
                    condition = condition.And(c => c.ID == model.ChildId && c.ParentChilds.Any(r => r.ParentId == UserInfo.Parent.ID));
                    var students = _studentBusiness.SearchChilds(condition, "ID", "asc", 1, int.MaxValue, out total);
                }
                if (total > 0)
                {
                    FileHelper.ResponseFile(model.ReportUrl, model.ReportName + ".pdf");
                    return new EmptyResult();
                }
                else
                {
                    return View("NotAvailable");
                }
            }
            else
            {
                return View("NotAvailable");
            }
        }

        #endregion
    }

    public class ItemAnswer
    {
        public int ItemId { get; set; }
        public string Res { get; set; }
    }
}