using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sunnet.Cli.Core.Users.Entities;
using Sunnet.Cli.Business.Users;
using Sunnet.Framework.Extensions;

namespace Sunnet.Cli.Sso.Areas.SignUp.Controllers
{
    public class TeacherController : Controller
    {
        UserBusiness userBusiness = new UserBusiness();

        // GET: /SignUp/Teacher/
        public ActionResult Index()
        {
            ApplicantEntity applicant = new ApplicantEntity();
            applicant.Title = "Mr";
            applicant.FirstName = "zhang";
            applicant.LastName = "san";
            applicant.Email = "123@123.com";
            applicant.RegistrantType = "1";
            applicant.Lock = "1";
            applicant.CreatedOn = DateTime.Now;
            applicant.UpdatedOn = DateTime.Now;
            //ViewBag.ListTitle = ListTitle.Mrs.ToSelectList();
            //List<SelectListItem> items = ListTitle.Dr.ToSelectList().ToList();
            //ViewBag.TitleItems = items;
            //ViewBag.TitleItems = new List<SelectListItem>() { new SelectListItem() { Text = "A" } };
            return View(applicant);
        }

        //Teacher
        public int Registor(ApplicantEntity applicant)
        {
            applicant.Title = "3";
            applicant.Lock = "1";
            applicant.CreatedOn = DateTime.Now;
            applicant.UpdatedOn = DateTime.Now;
            //insert
            int registerUser = userBusiness.RegisterUser(applicant);
            return registerUser;
        }

        //title       
        public enum ListTitle
        {
            Mr = 0,
            Mrs = 1,
            Ms = 2,
            Miss = 3,
            Dr = 4
        }

    }
}