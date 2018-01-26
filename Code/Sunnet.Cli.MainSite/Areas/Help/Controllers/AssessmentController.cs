using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sunnet.Cli.UIBase.Controllers;

namespace Sunnet.Cli.MainSite.Areas.Help.Controllers
{
    public class AssessmentController : GlobalController
    {
        // GET: Help/Assessment
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Offline()
        {
            return View();
        }
    }
}