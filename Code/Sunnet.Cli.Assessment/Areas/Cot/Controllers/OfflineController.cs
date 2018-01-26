#region Using

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Optimization;
using StructureMap;
using Sunnet.Cli.Assessment.Controllers;
using Sunnet.Cli.Assessment.Models;
using Sunnet.Cli.Business.Cot;
using Sunnet.Cli.Business.Cot.Models;
using Sunnet.Cli.Core.Cot.Entities;
using Sunnet.Cli.Core.Cot.Models;
using Sunnet.Cli.UIBase;
using Sunnet.Cli.UIBase.Filters;
using Sunnet.Cli.UIBase.Models;
using Sunnet.Framework.Core.Tool;
using Sunnet.Framework.Log;
using Sunnet.Framework.Permission;

#endregion

namespace Sunnet.Cli.Assessment.Areas.Cot.Controllers
{
    public class OfflineController : BaseController
    {
        private readonly CotBusiness _cotBusiness;
        private ISunnetLog logger;

        public OfflineController()
        {
            _cotBusiness = new CotBusiness(AdeUnitWorkContext);
            logger = ObjectFactory.GetInstance<ISunnetLog>();
        }

        private static string ManifestContent
        {
            get
            {
                string hash = BundleConfig.UpdateKey;

                var fileList = new List<string>();
                ProcessStaticFiles(fileList);

                string cacheList = string.Join("\n", fileList);
                string fallback = "/ /Offline/Index/Offline";
                string network = "*";
                string content = string.Format(
                    "CACHE MANIFEST\n# hash:{0}\nCACHE:\n{1}\nFALLBACK:\n{2}\nNETWORK:\n{3}", hash,
                    cacheList, fallback, network);

                return content;
            }
        }

        private string GetKey(string prefix)
        {
            return string.Format("_User_{0}_{1}_", UserInfo.ID, prefix);
        }

        [CLIUrlAuthorize(Account = Authority.All, PageId = PagesModel.COT, Anonymity = Anonymous.Verified)]
        public ActionResult Index()
        {
            ViewBag.LoginUrl = BuilderLoginUrl(LoginUserType.GOOGLEACCOUNT, LoginIASID.COT_OFFLINE);
            ViewBag.CLIUserLogin = BuilderLoginUrl(LoginUserType.UTACCESSMANAGER, LoginIASID.COT_OFFLINE);

            ViewBag.Manifest = Url.Action("Manifest");

            return View();
        }

        [CLIUrlAuthorize(Account = Authority.All, PageId = PagesModel.COT, Anonymity = Anonymous.Verified)]
        public ActionResult Preparing(int assessmentId)
        {
            var teachers = Session["_Cot_Offline_Teachers"] as List<CotSchoolTeacherModel>;
            object result = _cotBusiness.GetOfflineTeachers(assessmentId, teachers);
            Session.Remove("_Cot_Offline_Teachers");
            ViewBag.Json = JsonHelper.SerializeObject(result);

            return View();
        }

        public ActionResult Assessment()
        {
            return View();
        }

        public ActionResult Teacher()
        {
            return View();
        }

        public ActionResult Report()
        {
            return View();
        }

        public ActionResult Stg()
        {
            return View();
        }

        public ActionResult StgResult()
        {
            return View();
        }

        [CLIUrlAuthorize(Account = Authority.All, PageId = PagesModel.COT, Anonymity = Anonymous.Verified)]
        public FileResult Manifest()
        {
            byte[] by = Encoding.UTF8.GetBytes(ManifestContent);
            return File(by, "text/cache-manifest");
        }

        private static void ProcessStaticFiles(List<string> fileList)
        {
            fileList.Add("/Cot/Offline/Assessment");
            fileList.Add("/Cot/Offline/Teacher");
            fileList.Add("/Cot/Offline/Report");
            fileList.Add("/Cot/Offline/Stg");
            fileList.Add("/Cot/Offline/StgResult");

            fileList.AddRange(OfflineHelper.GlobalResources);
            fileList.AddRange(OfflineHelper.CKEditorResources);
            fileList.AddRange(OfflineHelper.DatetimeBoxResources);


            var cotList = new List<string>
            {
                "~/scripts/modernizr/offline",
                "~/scripts/jquery/offline",
                "~/scripts/bootstrap/offline",
                "~/scripts/knockout/offline",
                "~/scripts/cli/offline",
                "~/scripts/cot/offline",
                "~/scripts/format/offline",
                "~/scripts/jquery_val/offline",
                "~/scripts/ckeditor/offline",
                "~/css/basic/offline"
            };

#if DEBUG
            cotList.ForEach(resource =>
            {
                string content = resource.StartsWith("~/scripts")
                    ? Scripts.Render(resource).ToString()
                    : Styles.Render(resource).ToString();
                fileList.AddRange(OfflineHelper.SplitResources(content));
            });
#endif
#if !DEBUG
            fileList.AddRange(cotList.Select(resource => resource.Replace("~", "") + "?v=" + BundleConfig.UpdateKey));
#endif
        }

        [HttpPost]
        [ValidateInput(false)]
        [CLIUrlAuthorize(Account = Authority.All, PageId = PagesModel.COT, Anonymity = Anonymous.Verified)]
        public string Sync(int teacherId, int assessmentId, string items, string tmpItems, string waves, string reports)
        {
            var response = new PostFormResponse();
            response.Success = true;
            try
            {
                var itemEntities = JsonHelper.DeserializeObject<List<CotAssessmentItemEntity>>(items);
                var tmpItemEntities = JsonHelper.DeserializeObject<List<CotAssessmentWaveItemEntity>>(tmpItems);
                var waveEntities = JsonHelper.DeserializeObject<List<CotWaveEntity>>(waves);
                var reportEntities = JsonHelper.DeserializeObject<List<CotStgReportEntity>>(reports);
                OperationResult result = _cotBusiness.Sync(teacherId, assessmentId, itemEntities, tmpItemEntities,
                    waveEntities, reportEntities, UserInfo);
                response.Update(result);
            }
            catch (Exception ex)
            {
                logger.Debug("Sunnet.Cli.Assessment.Areas.Cot.Controllers.OfflineController, Sync Error:", teacherId,
                    assessmentId, items, tmpItems, waves, reports);

                response.Message = ex.Message;
                response.Success = false;
            }
            return JsonHelper.SerializeObject(response);
        }

        public string Online()
        {
            var online = new
            {
                online = true,
                date = DateTime.Now.ToString("HH:mm:ss"),
                logged = UserInfo != null
            };
            return JsonHelper.SerializeObject(online);
        }

        public string Offline()
        {
            return "false";
        }
    }
}