using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using StructureMap;
using Sunnet.Cli.Assessment.Areas.Cpalls.Models;
using Sunnet.Cli.Assessment.Controllers;
using Sunnet.Cli.Business;
using Sunnet.Cli.Business.Ade;
using Sunnet.Cli.Business.Common;
using Sunnet.Cli.Business.Cpalls;
using Sunnet.Cli.Business.Users;
using Sunnet.Cli.Core.Ade;
using Sunnet.Cli.Core.Ade.Entities;
using Sunnet.Cli.Core.Cpalls;
using Sunnet.Cli.Core.Cpalls.Entities;
using Sunnet.Cli.UIBase.Models;
using Sunnet.Cli.Business.Cpalls.Models;
using Sunnet.Cli.Business.Students;
using Sunnet.Cli.Core.Users.Enums;
using Sunnet.Cli.UIBase.Filters;
using Sunnet.Framework;
using Sunnet.Framework.Encrypt;
using Sunnet.Framework.Log;
using Sunnet.Framework.PDF;
using Sunnet.Framework.Permission;
using Sunnet.Framework.Helpers;

namespace Sunnet.Cli.Assessment.Areas.Cpalls.Controllers
{
    public class ExecuteController : BaseController
    {
        private AdeBusiness _adeBusiness;
        private CpallsBusiness _cpallsBusiness;
        private UserBusiness _userBuss = new UserBusiness();
        private StudentBusiness _studentBusiness;
        private ISunnetLog _logger;
        private IEncrypt encrypter;
        public ExecuteController()
        {
            _adeBusiness = new AdeBusiness(AdeUnitWorkContext);
            _cpallsBusiness = new CpallsBusiness(AdeUnitWorkContext);
            _studentBusiness = new StudentBusiness();

            _logger = ObjectFactory.GetInstance<ISunnetLog>();
            encrypter = ObjectFactory.GetInstance<IEncrypt>();
        }

        public string Init(int assessmentId = 0)
        {
            //if (assessmentId > 0)
            //{
            //    CpallsStudentModel student = _studentBusiness.GetStudentModel(11);
            //    return _cpallsBusiness.InsertAssessment(student, Wave.BOY, "14-15", assessmentId, UserInfo).ResultType.ToString();
            //}
            return "nodata";
        }

        //
        // GET: /Cpalls/Execute/
        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.CPALLS, Anonymity = Anonymous.Verified)]
        public ActionResult Index(int id, string measures, int classId)
        {
            List<int> measureIds = measures.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList();
            ExecCpallsAssessmentModel assessment = _cpallsBusiness.GetAssessment(id, measureIds, classId, UserInfo);
            if (assessment == null)
                return RedirectToAction("Index", "Dashboard", new { Area = "", showMessage = "assessment_unavaiable" });
            assessment.Measures =
                assessment.Measures.Where(x => x.Status == CpallsStatus.Initialised || x.Status == CpallsStatus.Paused)
                    .ToList();
            var assessmentModel = _adeBusiness.GetBaseAssessmentModel(assessment.AssessmentId);
            if (assessmentModel == null)
                return RedirectToAction("Index", "Dashboard", new { Area = "", showMessage = "assessment_unavaiable" });
            ViewBag.AssessmentId = id;
            ViewBag.AssessmentName = assessmentModel.Name;

            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.ExpiresAbsolute = DateTime.Now.AddDays(-1);
            Response.Cache.SetExpires(DateTime.Now.AddDays(-1));
            Response.CacheControl = "no-cache";
            Response.Expires = 0;
            if (assessment.StudentId == 0)
            {
                assessment.Student = new ExecCpallsStudentModel()
                {
                    ID = 0,
                    Name = ViewTextHelper.DemoStudentFirstName + " " + ViewTextHelper.DemoStudentLastName
                };
            }

            if (assessment.Class.ID == 0)
            {
                assessment.Class = new ExecCpallsClassModel()
                {
                    ID = 0,
                    Name = ViewTextHelper.DemoClassName
                };
            }

            ViewBag.Json = JsonHelper.SerializeObject(assessment);
            _logger.Info("Index：SAID:{0},Assessment:{1},Measures:{2},Items:{3}",
                assessment.ExecId,
                assessment.AssessmentId,
                assessment.Measures.Count,
                assessment.Measures.Sum(x => x.Items.Count()));
            if (assessment.Measures.Any(x => x.Status == CpallsStatus.Paused
          || (x.Status == CpallsStatus.Initialised && x.Items.Any())))
            {
                return View(assessment);
            }
            return View("View", assessment);

        }

        //
        // GET: /Cpalls/Execute/
        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.CPALLS, Anonymity = Anonymous.Verified)]
        public ActionResult View(int id, int measure, int classId)
        {
            var measureIds = new List<int>() { measure };
            var assessment = _cpallsBusiness.GetAssessment(id, measureIds, classId, UserInfo);
            assessment.InExecDate = (UserInfo.Role == Role.Teacher ? false : assessment.InExecDateTemp);
            ViewBag.assessmentId = assessment.AssessmentId;
            ViewBag.Json = JsonHelper.SerializeObject(assessment);
            assessment.Measures =
                assessment.Measures.FindAll(x => x.Status == CpallsStatus.Finished);
            if (!assessment.Measures.Any())
            {
                return RedirectToAction("Index", "Student", new
                {
                    classId = assessment.Class.ID,
                    assessmentId = assessment.AssessmentId
                });
            }
            return View(assessment);
        }


        //
        // GET: /Cpalls/Execute/
        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.CPALLS, Anonymity = Anonymous.Verified)]
        public ActionResult List(int id, int measure, int classId)
        {
            var measureIds = new List<int>() { measure };
            var assessment = _cpallsBusiness.GetAssessment(id, measureIds, classId, UserInfo);
            ViewBag.assessmentId = assessment.AssessmentId;
            ViewBag.Json = JsonHelper.SerializeObject(assessment);
            ViewBag.Model = assessment;
            if (assessment.Measures.Any(x => x.Status == CpallsStatus.Initialised || x.Status == CpallsStatus.Paused))
            {
                return View("Index", assessment);
            }
            return View(assessment);
        }

        //
        // GET: /Cpalls/Execute/
        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.CPALLS, Anonymity = Anonymous.Verified)]
        public ActionResult ListForExec()
        {
            ViewBag.Json = JsonHelper.SerializeObject(false);
            return View("List");
        }


        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.CPALLS, Anonymity = Anonymous.Verified)]
        public ActionResult ViewOffline()
        {
            ViewBag.Offline = true;
            ViewBag.assessmentId = 0;
            ViewBag.Json = "false";
            return View("View");
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.CPALLS, Anonymity = Anonymous.Verified)]
        public ActionResult Go()
        {


            return View();
        }

        /// <summary>
        /// reset
        /// </summary>
        /// <param name="measureId"></param>
        /// <param name="execAssessmentId"></param>
        /// <param name="schoolId"></param>
        /// <returns></returns>
        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.CPALLS, Anonymity = Anonymous.Verified)]
        public string Invalidate(int measureId, int execAssessmentId, int schoolId)
        {
            var response = new PostFormResponse();
            string ip = CommonHelper.GetIPAddress(Request);

            response.Update(_cpallsBusiness.CancelMeasure(measureId, execAssessmentId, schoolId, UserInfo, ip));
            return JsonHelper.SerializeObject(response);
        }


        [HttpPost]
        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.CPALLS, Anonymity = Anonymous.Verified)]
        public string Pause(int execAssessmentId, string measures, string items, int schoolId, DateTime studentBirthday,
            string schoolYear, Wave wave)
        {
            //_logger.Info("Online.A--> ExcuteController.Pause execAssessmentId:" + execAssessmentId + ",Measures:" + measures + ",Items:" + items);
            var studentMeasures = JsonHelper.DeserializeObject<List<StudentMeasureEntity>>(measures);
            var studentItems = JsonHelper.DeserializeObject<List<StudentItemEntity>>(items);
            var response = new PostFormResponse();
            response.Update(_cpallsBusiness.PauseMeasures(execAssessmentId, schoolId, studentMeasures, studentItems,
                studentBirthday, schoolYear, wave));
            return JsonHelper.SerializeObject(response);
        }


        /// <summary>
        /// save 操作
        /// </summary>
        /// <param name="execAssessmentId"></param>
        /// <param name="schoolId"></param>
        /// <param name="items"></param>
        /// <returns></returns>
        [HttpPost]
        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.CPALLS, Anonymity = Anonymous.Verified)]
        public string Items(int execAssessmentId, int schoolId, string measures, string items, DateTime studentBirthday,
            string schoolYear, Wave wave)
        {
            //_logger.Info("Online.A--> ExcuteController.Items execAssessmentId:" + execAssessmentId + ",studentBirthday:" + studentBirthday + ",SchoolId:" + schoolId + ",Year:" + schoolYear + ",Wave:" + ((int)wave).ToString() + ",Measures:" + measures + ",Items:" + items);

            var studentItems = JsonHelper.DeserializeObject<List<StudentItemEntity>>(items);
            var studentMeasures = JsonHelper.DeserializeObject<List<StudentMeasureEntity>>(measures);
            var response = new PostFormResponse();
            response.Update(_cpallsBusiness.UpdateItems(execAssessmentId, schoolId, studentMeasures, studentItems,
                studentBirthday, schoolYear, wave));
            return JsonHelper.SerializeObject(response);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.CPALLS, Anonymity = Anonymous.Logined)]
        public ActionResult Preview(int itemId = 0, int measureId = 0)
        {
            if (itemId < 1)
            {
                return Redirect("/Error/Error");
            }
            var assessment = _cpallsBusiness.GetAssessmentForPreview(itemId, measureId);

            ViewBag.Json = JsonHelper.SerializeObject(assessment);
            return View(assessment);
        }

        private int UserId
        {
            get
            {
                var userId = 0;
                if (UserInfo != null)
                {
                    userId = UserInfo.ID;
                }
                else
                {
                    int.TryParse(encrypter.Decrypt(Request.QueryString["UserId"]), out userId);
                }
                return userId;
            }
        }
        private string Key_ResultHtml
        {

            get
            {
                return "Result_Html:" + UserId;
            }
        }
        private string Key_ResultHtml_Assessment
        {
            get
            {
                return "Result_Html_Assessment:" + UserId;
            }
        }

        [ValidateInput(false)]
        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.CPALLS, Anonymity = Anonymous.Verified)]
        public ActionResult GetPdf(int id, int measure, int classId, string resultHtml, bool export = true)
        {
            var measureIds = new List<int>() { measure };
            var assessment = _cpallsBusiness.GetAssessment(id, measureIds, classId, UserInfo);
            if (assessment == null || string.IsNullOrEmpty(resultHtml))
            {
                return Content("There is an error occured, please try again.");
            }
            ViewBag.ResultHtml = resultHtml;
            ViewBag.Model = assessment;
            if (export)
            {
                GetPdf(GetViewHtml("GetPdf"), assessment.Name);
            }
            return View();
        }

        [ValidateInput(false)]
        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.CPALLS, Anonymity = Anonymous.Verified)]
        public ActionResult GetListPdf(string name, string body, bool export)
        {
            ViewBag.Body = body;
            ViewBag.Pdf = true;
            if (export)
            {
                GetPdf(GetViewHtml("GetListPdf"), name);
            }
            return View();
        }

        private void GetPdf(string html, string fileName)
        {
            PdfProvider pdfProvider = new PdfProvider();
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
                ViewContext viewContext = new ViewContext(ControllerContext, result.View, this.ViewData, this.TempData, Response.Output);
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
            string format = "The view '{0}' or its master was not found or no view engine supports the searched locations. The following locations were searched:{1}";
            StringBuilder builder = new StringBuilder();
            foreach (string str in searchedLocations)
            {
                builder.AppendLine();
                builder.Append(str);
            }
            return string.Format(CultureInfo.CurrentCulture, format, viewName, builder);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.CPALLS, Anonymity = Anonymous.Verified)]
        public ActionResult Comment(int id)
        {
            var measure = _cpallsBusiness.GetStudentMeasureModel(id);
            return View(measure);
        }

        /// <summary>
        /// Updates the measure comment of student.
        /// </summary>
        /// <param name="id">The identifier of Student Measure.</param>
        /// <param name="comment">The new comment.</param>
        /// <returns></returns>
        /// Author : JackZhang
        /// Date   : 7/8/2015 12:01:33
        [HttpPost]
        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.CPALLS, Anonymity = Anonymous.Verified)]
        public string Comment(int id, string comment)
        {
            var response = new PostFormResponse();
            response.Update(_cpallsBusiness.UpdateMeasureComment(id, comment));
            return JsonHelper.SerializeObject(response);
        }
    }
}