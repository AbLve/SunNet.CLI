using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using StructureMap;
using Sunnet.Cli.Business.Ade;
using Sunnet.Cli.Business.Cpalls;
using Sunnet.Cli.Business.Cpalls.Models;
using Sunnet.Cli.Business.Practices;
using Sunnet.Cli.Business.Students;
using Sunnet.Cli.Business.Users;
using Sunnet.Cli.Core.Cpalls;
using Sunnet.Cli.Core.Cpalls.Entities;
using Sunnet.Cli.Core.Practices.Entities;
using Sunnet.Cli.Practice.Controllers;
using Sunnet.Cli.UIBase.Filters;
using Sunnet.Cli.UIBase.Models;
using Sunnet.Framework.Core.Tool;
using Sunnet.Framework.Encrypt;
using Sunnet.Framework.Helpers;
using Sunnet.Framework.Log;
using Sunnet.Framework.PDF;
using Sunnet.Framework.Permission;

namespace Sunnet.Cli.Practice.Areas.Cpalls.Controllers
{
    public class ExecuteController : BaseController
    {
        private AdeBusiness _adeBusiness;
        private PracticeBusiness _practiceBusiness;
        private CpallsBusiness _cpallsBusiness;
        private UserBusiness _userBuss = new UserBusiness();
        private StudentBusiness _studentBusiness;
        private ISunnetLog _logger;
        private IEncrypt encrypter;
        public ExecuteController()
        {
            _adeBusiness = new AdeBusiness();
            _cpallsBusiness = new CpallsBusiness();
            _studentBusiness = new StudentBusiness();
            _practiceBusiness = new PracticeBusiness(PracticeUnitWorkContext);
            _logger = ObjectFactory.GetInstance<ISunnetLog>();
            encrypter = ObjectFactory.GetInstance<IEncrypt>();
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.Practice, Anonymity = Anonymous.Verified)]
        public ActionResult Index(int id, string measures)
        {
            List<int> measureIds = measures.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList();
            ExecCpallsAssessmentModel assessment = _practiceBusiness.GetAssessment(id, measureIds,  UserInfo);
            if (assessment == null)
                return RedirectToAction("Index", "Student", new { Area = "", showMessage = "assessment_unavaiable" });
            assessment.Measures =
                assessment.Measures.Where(x => x.Status == CpallsStatus.Initialised || x.Status == CpallsStatus.Paused)
                    .ToList();
            var assessmentModel = _adeBusiness.GetBaseAssessmentModel(assessment.AssessmentId);
            if (assessmentModel == null)
                return RedirectToAction("Index", "Student", new { Area = "", showMessage = "assessment_unavaiable" });
            ViewBag.AssessmentId = id;
            ViewBag.AssessmentName = assessmentModel.Name;

            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.ExpiresAbsolute = DateTime.Now.AddDays(-1);
            Response.Cache.SetExpires(DateTime.Now.AddDays(-1));
            Response.CacheControl = "no-cache";
            Response.Expires = 0;
           
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

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.Practice, Anonymity = Anonymous.Verified)]
        [HttpPost]
        public string Pause(int execAssessmentId, string measures, string items, DateTime studentBirthday,
            string schoolYear, Wave wave)
        {
            var studentMeasures = JsonHelper.DeserializeObject<List<PracticeStudentMeasureEntity>>(measures);
            var studentItems = JsonHelper.DeserializeObject<List<PracticeStudentItemEntity>>(items);
            var response = new PostFormResponse();
            response.Update(_practiceBusiness.PauseMeasures(execAssessmentId, studentMeasures, studentItems,
                studentBirthday, schoolYear, wave));
            return JsonHelper.SerializeObject(response);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.Practice, Anonymity = Anonymous.Verified)]
        public ActionResult View(int id, int measure)
        {
            var measureIds = new List<int>() { measure };
            var assessment = _practiceBusiness.GetAssessment(id, measureIds, UserInfo);
            ViewBag.assessmentId = assessment.AssessmentId;
            ViewBag.Json = JsonHelper.SerializeObject(assessment);
            assessment.Measures =
                assessment.Measures.FindAll(x => x.Status == CpallsStatus.Finished);
            if (!assessment.Measures.Any())
            {
                return RedirectToAction("Index", "Student", new
                {
                    assessmentId = assessment.AssessmentId
                });
            }
            return View(assessment);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.Practice, Anonymity = Anonymous.Verified)]
        public ActionResult Comment(int id)
        {
            var measure = _practiceBusiness.GetStudentMeasureModel(id);
            return View(measure);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.Practice, Anonymity = Anonymous.Verified)]
        [HttpPost]
        public string Comment(int id, string comment)
        {
            var response = new PostFormResponse();
            response.Update(_practiceBusiness.UpdateMeasureComment(id, comment));
            return JsonHelper.SerializeObject(response);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.Practice, Anonymity = Anonymous.Verified)]
        public string Invalidate(int measureId, int execAssessmentId)
        {
            var response = new PostFormResponse();
            string ip = CommonHelper.GetIPAddress(Request);

            response.Update(_practiceBusiness.CancelMeasure(measureId, execAssessmentId, UserInfo, ip));
            return JsonHelper.SerializeObject(response);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.Practice, Anonymity = Anonymous.Verified)]
        public ActionResult Go()
        {

            return View();
        }
        /// <summary>
        /// save 操作
        /// </summary>
        /// <param name="execAssessmentId"></param>
        /// <param name="schoolId"></param>
        /// <param name="items"></param>
        /// <returns></returns>
        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.Practice, Anonymity = Anonymous.Verified)]
        [HttpPost]
        public string Items(int execAssessmentId, int schoolId, string measures, string items, DateTime studentBirthday,
            string schoolYear, Wave wave)
        {
            var studentItems = JsonHelper.DeserializeObject<List<PracticeStudentItemEntity>>(items);
            var studentMeasures = JsonHelper.DeserializeObject<List<PracticeStudentMeasureEntity>>(measures);
            var response = new PostFormResponse();
            response.Update(_practiceBusiness.UpdateItems(execAssessmentId, studentMeasures, studentItems,
                studentBirthday, schoolYear, wave));
            return JsonHelper.SerializeObject(response);
        }


        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.Practice, Anonymity = Anonymous.Verified)]
        public ActionResult ViewOffline()
        {
            ViewBag.Offline = true;
            ViewBag.assessmentId = 0;
            ViewBag.Json = "false";
            return View("View");
        }

        [HttpPost]
        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.Practice, Anonymity = Anonymous.Verified)]
        public string RefreshClassroom(int assessmentId, Wave wave)
        {
            var response = new PostFormResponse();
            OperationResult res = new OperationResult(OperationResultType.Success);
            res = _practiceBusiness.RefreshClassroom(assessmentId, wave,UserInfo.ID);
            response.Update(res);
            return JsonHelper.SerializeObject(response);
        }
    }
}