using Sunnet.Cli.Business.BUP;
using Sunnet.Cli.Core.BUP.Entities;
using Sunnet.Cli.MainSite.Controllers;
using Sunnet.Cli.UIBase.Filters;
using Sunnet.Cli.UIBase.Models;
using Sunnet.Framework.Permission;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Sunnet.Cli.Core.BUP;
using Sunnet.Cli.Core.BUP.Models;
using Sunnet.Cli.Core.Users.Enums;
using Sunnet.Framework.Helpers;
using Sunnet.Cli.Core.Communities.Entities;
using Sunnet.Cli.Business.Communities;

namespace Sunnet.Cli.MainSite.Areas.BUP.Controllers
{
    public class PublicController : BaseController
    {
        private readonly BUPTaskBusiness _bupTaskBusiness;
        private readonly CommunityBusiness _communityBusiness;
        public PublicController()
        {
            _bupTaskBusiness = new BUPTaskBusiness(UnitWorkContext);
            _communityBusiness = new CommunityBusiness(UnitWorkContext);
        }

        public delegate void ProcessHandler(int id, int createdBy);

        #region
        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.BUP, Anonymity = Anonymous.Verified)]
        public void Process(int ID)
        {
            var bupTask = _bupTaskBusiness.GetBupTask(ID);
            if (bupTask != null)
            {
                bupTask.Status=BUPStatus.Queued;
                _bupTaskBusiness.Update(bupTask);
            }
            //ProcessHandler handler = new ProcessHandler(BUPTaskBusiness.Start);
            //handler.BeginInvoke(ID, UserInfo.ID, null, null);
        }
        #endregion
        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.BUP, Anonymity = Anonymous.Verified)]
        public ActionResult ViewLog(int id, byte type)
        {
            StringBuilder sb = new StringBuilder();
            switch (type)
            {
                case (byte)BUPType.Community:
                    sb.AppendFormat("select * from BUP_Communities where taskid = {0} and status in (4,5) order by LineNum", id);
                    ViewBag.Entities = _bupTaskBusiness.ExecuteSqlQuery(sb.ToString(), BUPType.Community);
                    return View("Community");
                case (byte)BUPType.School:
                    sb.AppendFormat("select * from BUP_Schools where taskid = {0} and status in (4,5) order by LineNum", id);
                    ViewBag.Entities = _bupTaskBusiness.ExecuteSqlQuery(sb.ToString(), BUPType.School);
                    return View("School");
                case (byte)BUPType.Classroom:
                    sb.AppendFormat("select * from BUP_Classrooms where taskid = {0} and status in (4,5) order by LineNum", id);
                    ViewBag.Entities = _bupTaskBusiness.ExecuteSqlQuery(sb.ToString(), BUPType.Classroom);
                    return View("Classroom");
                case (byte)BUPType.Class:
                    sb.AppendFormat("select * from BUP_Classes where taskid = {0} and status in (4,5) order by LineNum", id);
                    List<BUPClassModel> entitiesClass = _bupTaskBusiness.ExecuteSqlQuery(sb.ToString(), BUPType.Class);
                    ViewBag.BatchId = id;
                    ViewBag.Entities = entitiesClass;
                    return View("Class");
                case (byte)BUPType.Teacher:
                    sb.AppendFormat("select * from BUP_Teachers where taskid = {0} and status in (4,5) order by LineNum", id);
                    List<BUPTeacherModel> entitiesTeacher = _bupTaskBusiness.ExecuteSqlQuery(sb.ToString(), BUPType.Teacher);
                    ViewBag.BatchId = id;
                    ViewBag.Entities = entitiesTeacher;
                    return View("Teacher");
                case (byte)BUPType.Student:
                    sb.AppendFormat("select * from BUP_Students where taskid = {0} and status in (4,5) order by LineNum", id);
                    List<BUPStudentModel> entitiesStudent = _bupTaskBusiness.ExecuteSqlQuery(sb.ToString(), BUPType.Student);
                    ViewBag.BatchId = id;
                    ViewBag.Entities = entitiesStudent;
                    return View("Student");
                case (byte)BUPType.CommunityUser:
                    sb.AppendFormat("select * from BUP_CommunityUsers where taskid = {0} and role={1} and status in (4,5) order by LineNum", id, (byte)Role.Community);
                    ViewBag.Entities = _bupTaskBusiness.ExecuteSqlQuery(sb.ToString(), BUPType.CommunityUser);
                    ViewBag.Title = "Community/District User";
                    return View("CommunityUser");
                case (byte)BUPType.CommunitySpecialist:
                    sb.AppendFormat("select * from BUP_CommunityUsers where taskid = {0} and role={1} and status in (4,5) order by LineNum"
                        , id, (byte)Role.District_Community_Specialist);
                    ViewBag.Entities = _bupTaskBusiness.ExecuteSqlQuery(sb.ToString(), BUPType.CommunitySpecialist);
                    ViewBag.Title = "Community/District Specialist";
                    return View("CommunityUser");
                case (byte)BUPType.Principal:
                    sb.AppendFormat("select * from BUP_Principals where taskid = {0} and role={1} and status in (4,5) order by LineNum"
                        , id, (byte)Role.Principal);
                    ViewBag.Entities = _bupTaskBusiness.ExecuteSqlQuery(sb.ToString(), BUPType.Principal);
                    ViewBag.Title = "Principal";
                    return View("Principal");
                case (byte)BUPType.SchoolSpecialist:
                    sb.AppendFormat("select * from BUP_Principals where taskid = {0} and role={1} and status in (4,5) order by LineNum"
                        , id, (byte)Role.School_Specialist);
                    ViewBag.Entities = _bupTaskBusiness.ExecuteSqlQuery(sb.ToString(), BUPType.SchoolSpecialist);
                    ViewBag.Title = "School Specialist";
                    return View("Principal");
                case (byte)BUPType.Parent:
                    sb.AppendFormat("select * from BUP_Parents where taskid = {0} and status in (4,5) order by LineNum", id);
                    ViewBag.Entities = _bupTaskBusiness.ExecuteSqlQuery(sb.ToString(), BUPType.Parent);
                    return View("Parent");
                case (byte)BUPType.StatewideAgency:
                    sb.AppendFormat("select * from BUP_Statewides where taskid = {0} and status in (4,5) order by LineNum", id);
                    ViewBag.Entities = _bupTaskBusiness.ExecuteSqlQuery(sb.ToString(), BUPType.StatewideAgency);
                    return View("Statewide");
                case (byte)BUPType.Auditor:
                    sb.AppendFormat("select * from BUP_Auditors where taskid = {0} and status in (4,5) order by LineNum", id);
                    ViewBag.Entities = _bupTaskBusiness.ExecuteSqlQuery(sb.ToString(), BUPType.Auditor);
                    return View("Auditor");
                default:
                    return View("Error");
            }
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.BUP, Anonymity = Anonymous.Verified)]
        public string GetCommunitySelectListForSearch(string keyword, int communityId = -1, bool isActiveCommunity = true)
        {
            var expression = PredicateHelper.True<CommunityEntity>();
            if (communityId > 0)
                expression = expression.And(o => o.ID == communityId);
            var list = _communityBusiness.GetCommunitySelectList(UserInfo, expression, isActiveCommunity);
            return JsonHelper.SerializeObject(list);
        }
    }
}