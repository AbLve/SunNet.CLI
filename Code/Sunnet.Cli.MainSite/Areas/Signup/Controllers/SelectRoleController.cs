using Sunnet.Cli.UIBase.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sunnet.Cli.MainSite.Areas.Signup.Controllers
{
    public class SelectRoleController : GlobalController
    {
        //
        // GET: /SignUp/SelectRole/
        public ActionResult Index()
        {
            return View();
        }

        //to I'm a teacher
        public ActionResult Teacher()
        {
            return View();
        }

        //to I'm a parent
        public ActionResult parent()
        {
            return View();
        }

        //to I'm a principal/director
        public ActionResult principal()
        {
            return View();
        }

        //to I'm a district/community user
        public ActionResult district()
        {
            return View();
        }

        //to Statewide user and Auditor
        public ActionResult statewide()
        {
            return View();
        }

        //to I'm a Specialist
        public ActionResult specialist()
        {
            return View();
        }

	}
}