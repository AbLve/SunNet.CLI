using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sunnet.Cli.Practice.Controllers
{
    public class ErrorController : Controller
    {
        // GET: Error
        public ActionResult Index(Exception error = null)
        {
            return View("Error");
        }

        public ActionResult NotFound(Exception error = null)
        {
            return View("404");
        }

        public ActionResult Error(Exception error = null)
        {
            return View("Error");
        }

        public ActionResult nonauthorized()
        {
            return View();
        }
    }
}