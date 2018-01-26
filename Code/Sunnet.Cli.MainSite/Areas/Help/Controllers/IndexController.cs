using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sunnet.Cli.UIBase.Controllers;

namespace Sunnet.Cli.MainSite.Areas.Help.Controllers
{
    public class IndexController : GlobalController
    {
        //
        // GET: /Help/Index/
        public ActionResult Index()
        {
            return View();
        }
	}
}