using System.Data;
using System.Globalization;
using System.IO;
using System.Linq.Expressions;
using System.Threading;
using Newtonsoft.Json;
using StructureMap.Query;
using Sunnet.Cli.Business.Common;
using Sunnet.Cli.Business.MasterData;
using Sunnet.Cli.Business.Reports;
using Sunnet.Cli.Business.Users;
using Sunnet.Cli.Business.Users.Models;
using Sunnet.Cli.Core.Classes.Enums;
using Sunnet.Cli.Core.Common.Enums;
using Sunnet.Cli.Core.Reports;
using Sunnet.Cli.Core.Reports.Entities;
using Sunnet.Cli.Core.Reports.Models;
using Sunnet.Cli.Core.Users.Entities;
using Sunnet.Cli.Core.Users.Enums;
using Sunnet.Cli.Impl.Classrooms;
using Sunnet.Cli.MainSite.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sunnet.Cli.UIBase;
using Sunnet.Cli.UIBase.Filters;
using Sunnet.Cli.UIBase.Models;
using Sunnet.Framework;
using Sunnet.Framework.Core.Tool;
using Sunnet.Framework.Extensions;
using Sunnet.Framework.Helpers;
using Sunnet.Framework.Permission;
using Sunnet.Framework.Resources;
using Sunnet.Cli.Business.Permission.Models;
using Sunnet.Cli.Business.Permission;
using Sunnet.Cli.Business.Communities;
using Sunnet.Cli.Business.Schools;

namespace Sunnet.Cli.MainSite.Areas.Report.Controllers
{
    public class IndexController : BaseController
    {
        private readonly ReportBusiness _reportBusiness;
        private readonly DataExportBusiness dataExportBusiness;
        private readonly UserBusiness userBusiness;
        private readonly MasterDataBusiness masterDataBusiness;

        public IndexController()
        {
            _reportBusiness = new ReportBusiness(UnitWorkContext);
            dataExportBusiness = new DataExportBusiness(UnitWorkContext);
            userBusiness = new UserBusiness(UnitWorkContext);
            masterDataBusiness = new MasterDataBusiness(UnitWorkContext);
        }


        #region view
        //
        // GET: /Report/
        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.Reports, Anonymity = Anonymous.Verified)]
        public ActionResult Index()
        {
            ViewBag.DataExport = false;
            ViewBag.CIRCLEDataExport = false;
            ViewBag.BeechReports = false;
            ViewBag.ParticipationCounts = false;
            ViewBag.MentorCoachReport = false;
            ViewBag.EverServiced = false;
            ViewBag.CurrentlyServing = false;
            ViewBag.ByESCRegion = false;
            ViewBag.PDReports = false;
            ViewBag.CoachingHoursbyCommunitys = false;
            ViewBag.TeacherTurnoverReport = false;
            ViewBag.TSRMediaConsentReports = false;

            if (UserInfo != null)
            {
                UserAuthorityModel userAuthorityDataExport = new PermissionBusiness().GetUserAuthority(UserInfo, (int)PagesModel.DataExport);
                if (userAuthorityDataExport != null)
                    ViewBag.DataExport = true;

                UserAuthorityModel userAuthorityCIRCLEDataExport = new PermissionBusiness().GetUserAuthority(UserInfo, (int)PagesModel.CIRCLEDataExport);
                if (userAuthorityCIRCLEDataExport != null)
                    ViewBag.CIRCLEDataExport = true;

                UserAuthorityModel userAuthorityBeechReports = new PermissionBusiness().GetUserAuthority(UserInfo, (int)PagesModel.BeechReports);
                if (userAuthorityBeechReports != null)
                    ViewBag.BeechReports = true;

                UserAuthorityModel userAuthorityParticipationCounts = new PermissionBusiness().GetUserAuthority(UserInfo, (int)PagesModel.ParticipationCounts);
                if (userAuthorityParticipationCounts != null)
                    ViewBag.ParticipationCounts = true;

                UserAuthorityModel userAuthorityMentorCoachReport = new PermissionBusiness().GetUserAuthority(UserInfo, (int)PagesModel.MentorCoachReport);
                if (userAuthorityMentorCoachReport != null)
                    ViewBag.MentorCoachReport = true;

                UserAuthorityModel userAuthorityEverServiced = new PermissionBusiness().GetUserAuthority(UserInfo, (int)PagesModel.EverServiced);
                if (userAuthorityEverServiced != null)
                    ViewBag.EverServiced = true;

                UserAuthorityModel userAuthorityCurrentlyServing = new PermissionBusiness().GetUserAuthority(UserInfo, (int)PagesModel.CurrentlyServing);
                if (userAuthorityCurrentlyServing != null)
                    ViewBag.CurrentlyServing = true;

                UserAuthorityModel userAuthorityByESCRegion = new PermissionBusiness().GetUserAuthority(UserInfo, (int)PagesModel.ByESCRegion);
                if (userAuthorityByESCRegion != null)
                    ViewBag.ByESCRegion = true;

                UserAuthorityModel userAuthorityPDReports = new PermissionBusiness().GetUserAuthority(UserInfo, (int)PagesModel.PDReports);
                if (userAuthorityPDReports != null)
                    ViewBag.PDReports = true;

                UserAuthorityModel userAuthorityCoachingHoursbyCommunitys = new PermissionBusiness().GetUserAuthority(UserInfo, (int)PagesModel.CoachingHoursbyCommunitys);
                if (userAuthorityCoachingHoursbyCommunitys != null)
                    ViewBag.CoachingHoursbyCommunitys = true;

                UserAuthorityModel userAuthorityTeacherTurnoverReport = new PermissionBusiness().GetUserAuthority(UserInfo, (int)PagesModel.TeacherTurnoverReport);
                if (userAuthorityTeacherTurnoverReport != null)
                    ViewBag.TeacherTurnoverReport = true;

                UserAuthorityModel userAuthorityTSRMediaConsentReports = new PermissionBusiness().GetUserAuthority(UserInfo, (int)PagesModel.TSRMediaConsentReports);
                if (userAuthorityTSRMediaConsentReports != null)
                    ViewBag.TSRMediaConsentReports = true;
            }

            return View();
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.BeechReports, Anonymity = Anonymous.Verified)]
        public ActionResult BeechReport()
        {
            return View();
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.MentorCoachReport, Anonymity = Anonymous.Verified)]
        public ActionResult CoachReport()
        {
            if (UserInfo.Role == Role.Super_admin)
            {
                ViewBag.MentorCoachs =
                    userBusiness.GetMentor_Coachs().ToSelectList(ViewTextHelper.DefaultAllText, "0");
            }
            else
            {
                ViewBag.MentorCoachs = userBusiness.GetMentor_CoachsByUserId(UserInfo.ID)
                    .ToSelectList(ViewTextHelper.DefaultAllText, "0");
            }

            ViewBag.Fundings = masterDataBusiness.GetFundingSelectList(false);
            return View();
        }

        [CLIUrlAuthorize(Account = Authority.All, PageId = PagesModel.Administrative, Anonymity = Anonymous.Logined)]
        public string GetCoachCoordByCommunity(int? communityId)
        {
            IEnumerable<SelectListItem> list = null;

            if (communityId == null)
            {
                if (UserInfo.Role == Role.Super_admin)
                {
                    list = userBusiness.GetMentor_Coachs().ToSelectList(ViewTextHelper.DefaultAllText, "0");
                }
                else
                {
                    list = userBusiness.GetMentor_CoachsByUserId(UserInfo.ID)
                        .ToSelectList(ViewTextHelper.DefaultAllText, "0");
                }
            }
            else
                list = userBusiness.GetCoachCoordByCommunity((int)communityId)
                .ToSelectList(ViewTextHelper.DefaultAllText, "0");

            return JsonHelper.SerializeObject(list);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.TSRMediaConsentReports, Anonymity = Anonymous.Verified)]
        public ActionResult MediaConsentReport()
        {
            return View();
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.ParticipationCounts, Anonymity = Anonymous.Verified)]
        public ActionResult QuarterlyReport()
        {
            ViewBag.Fundings = masterDataBusiness.GetFundingSelectList(false);
            return View();
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.PDReports, Anonymity = Anonymous.Verified)]
        public ActionResult PDReport()
        {
            return View();
        }


        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.EverServiced,
            Anonymity = Anonymous.Verified)]
        public ActionResult ServiceReport()
        {
            return View();
        }

        #endregion end view

        #region Media Consent Report

        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.TSRMediaConsentReports, Anonymity = Anonymous.Verified, IsSignUrl = false)]
        public void GetMediaConsentPercent(int? communityId, int? schoolId, string teacher)
        {
            List<int> communityIds = new List<int>();
            List<int> schoolIds = new List<int>();
            switch (UserInfo.Role)
            {
                case Role.Statewide:
                case Role.Community:
                case Role.District_Community_Specialist:
                    if (communityId == null)
                    {
                        communityIds = UserInfo.UserCommunitySchools.Select(e => e.CommunityId).ToList();
                    }
                    break;
                case Role.Community_Specialist_Delegate:
                case Role.District_Community_Delegate:
                    if (communityId == null)
                    {
                        var user = userBusiness.GetUser(UserInfo.ID);
                        communityIds = user.UserCommunitySchools.Select(e => e.CommunityId).ToList();
                    }
                    break;
                case Role.Principal:
                case Role.TRS_Specialist:
                case Role.School_Specialist:
                    if (schoolId == null)
                    {
                        schoolIds = UserInfo.UserCommunitySchools.Select(e => e.SchoolId).ToList();
                    }
                    break;
                case Role.Principal_Delegate:
                case Role.TRS_Specialist_Delegate:
                case Role.School_Specialist_Delegate:
                    if (schoolId == null)
                    {
                        var user = userBusiness.GetUser(UserInfo.CommunityUser.ParentId);
                        schoolIds = user.UserCommunitySchools.Select(e => e.SchoolId).ToList();
                    }
                    break;
                case Role.Teacher:
                    if (teacher == "")
                        teacher = UserInfo.FirstName + " " + UserInfo.LastName;
                    break;
                case Role.Parent:
                    communityIds.Add(0);
                    schoolIds.Add(0);
                    break;
            }
            if (communityId != null)
                communityIds.Add(communityId.Value);
            if (schoolId != null)
                schoolIds.Add(schoolId.Value);
            string path = "";
            _reportBusiness.GetMediaConsentPercent(out path, communityIds, schoolIds, teacher);
            FileStream fs = new FileStream(path, FileMode.Open);
            byte[] bytes = new byte[(int)fs.Length];
            fs.Read(bytes, 0, bytes.Length);
            fs.Close();
            if (System.IO.File.Exists(@path))
            {
                System.IO.File.Delete(@path);
            }
            Response.ContentType = "application/octet-stream";
            Response.AddHeader("Content-Disposition",
                "attachment;  filename=" +
                HttpUtility.UrlEncode("MediaConsent" + DateTime.Now.ToString("_yyyyMMdd_hhmmss") + ".xlsx",
                    System.Text.Encoding.UTF8));
            Response.BinaryWrite(bytes);
            Response.Flush();
            Response.End();
        }
        #endregion

        #region BEECH Report

        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.BeechReports, Anonymity = Anonymous.Verified, IsSignUrl = false)]
        public void GetBeechReport(int? communityId, int? schoolId, string teacher)
        {
            List<int> communityIds = new List<int>();
            List<int> schoolIds = new List<int>();
            switch (UserInfo.Role)
            {
                case Role.Statewide:
                case Role.Community:
                case Role.District_Community_Specialist:
                    if (communityId == null)
                        communityIds = UserInfo.UserCommunitySchools.Select(e => e.CommunityId).ToList();
                    break;
                case Role.Community_Specialist_Delegate:
                case Role.District_Community_Delegate:
                    if (communityId == null)
                    {
                        var user = userBusiness.GetUser(UserInfo.CommunityUser.ParentId);
                        communityIds = user.UserCommunitySchools.Select(e => e.CommunityId).ToList();
                    }
                    break;
                case Role.Principal:
                case Role.TRS_Specialist:
                case Role.School_Specialist:
                    if (schoolId == null)
                    {
                        schoolIds = UserInfo.UserCommunitySchools.Select(e => e.SchoolId).ToList();
                    }
                    break;
                case Role.Principal_Delegate:
                case Role.TRS_Specialist_Delegate:
                case Role.School_Specialist_Delegate:
                    if (schoolId == null)
                    {
                        var user = userBusiness.GetUser(UserInfo.CommunityUser.ParentId);
                        schoolIds = user.UserCommunitySchools.Select(e => e.SchoolId).ToList();
                    }
                    break;
                case Role.Teacher:
                    if (teacher == "")
                        teacher = UserInfo.FirstName + " " + UserInfo.LastName;
                    break;
                case Role.Parent:
                    communityIds.Add(0);
                    schoolIds.Add(0);
                    break;
            }
            if (communityId != null)
                communityIds.Add(communityId.Value);
            if (schoolId != null)
                schoolIds.Add(schoolId.Value);
            string path = "";
            _reportBusiness.GetBeechReport(out path, communityIds, schoolIds, teacher);
            FileStream fs = new FileStream(path, FileMode.Open);
            byte[] bytes = new byte[(int)fs.Length];
            fs.Read(bytes, 0, bytes.Length);
            fs.Close();
            if (System.IO.File.Exists(@path))
            {
                System.IO.File.Delete(@path);
            }
            Response.ContentType = "application/octet-stream";
            Response.AddHeader("Content-Disposition",
                "attachment;  filename=" +
                HttpUtility.UrlEncode("BEECHReport" + DateTime.Now.ToString("_yyyyMMdd_hhmmss") + ".xlsx",
                    System.Text.Encoding.UTF8));
            Response.BinaryWrite(bytes);
            Response.Flush();
            Response.End();
        }

        [CLIUrlAuthorize(Account = Authority.All, PageId = PagesModel.Administrative, Anonymity = Anonymous.Logined)]
        public string GetTeachers(int communityId = 0, int schoolId = 0, string keyword = "")
        {
            int t;
            Expression<Func<TeacherEntity, bool>> condition = x => true;
            if (communityId > 0)
                condition = condition.And(x => x.UserInfo.UserCommunitySchools.Any(y => y.CommunityId == communityId));
            if (schoolId > 0) condition = condition.And(x => x.UserInfo.UserCommunitySchools.Any(y => y.SchoolId == schoolId));
            var teachers = userBusiness.SearchTeachers(UserInfo, condition, "FirstName", "ASC", 0, int.MaxValue, out t)
                .Select(x => new SelectItemModel()
                {
                    ID = x.ID,
                    Name = x.FirstName + " " + x.LastName
                });
            return JsonHelper.SerializeObject(teachers);
        }

        #endregion

        #region Online Report
        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.PDReports,
            Anonymity = Anonymous.Verified, IsSignUrl = false)]
        public string SearchPdReport(int? communityId, int? schoolId, string teacher, int status)
        {
            List<int> communityIds = new List<int>();
            List<int> schoolIds = new List<int>();
            switch (UserInfo.Role)
            {
                case Role.Statewide:
                case Role.Community:
                case Role.District_Community_Specialist:
                    if (communityId == null)
                    {
                        communityIds = UserInfo.UserCommunitySchools.Select(e => e.CommunityId).ToList();
                    }
                    break;
                case Role.Community_Specialist_Delegate:
                case Role.District_Community_Delegate:
                    if (communityId == null)
                    {
                        communityIds = UserInfo.UserCommunitySchools.Select(e => e.CommunityId).ToList();
                    }
                    break;
                case Role.Principal:
                case Role.TRS_Specialist:
                case Role.School_Specialist:
                    if (communityId == null)
                    {

                    }
                    if (schoolId == null)
                    {
                        schoolIds = UserInfo.UserCommunitySchools.Select(e => e.SchoolId).ToList();
                    }
                    break;
                case Role.Principal_Delegate:
                case Role.TRS_Specialist_Delegate:
                case Role.School_Specialist_Delegate:
                    if (schoolId == null)
                    {
                        var user = userBusiness.GetUser(UserInfo.CommunityUser.ParentId);
                        schoolIds = user.UserCommunitySchools.Select(e => e.SchoolId).ToList();
                    }
                    break;
                case Role.Teacher:
                    if (teacher == "")
                        teacher = UserInfo.FirstName + " " + UserInfo.LastName;
                    break;
                case Role.Parent:
                    communityIds.Add(0);
                    schoolIds.Add(0);
                    break;
            }
            if (communityId != null)
                communityIds.Add(communityId.Value);
            if (schoolId != null)
                schoolIds.Add(schoolId.Value);
            List<PDReportModel> list = _reportBusiness.GetPdCompletionCourse(communityIds, schoolIds, teacher,
                status);
            List<PDReportModel> list1 = new List<PDReportModel>();
            for (int i = 0; i < list.Count(); i++)
            {
                PDReportModel model = list[i];
                list1.Add(model);
                if (list.Count == i + 1 || list[i].TeacherEmail != list[i + 1].TeacherEmail)
                {
                    PDReportModel model_1 = new PDReportModel();
                    model_1.CommunityDistrict = model.CommunityDistrict;
                    model_1.SchoolName = model.SchoolName;
                    model_1.TeacherFirstName = model.TeacherFirstName;
                    model_1.TeacherLastName = model.TeacherLastName;
                    model_1.TeacherID = model.TeacherID;
                    model_1.TeacherEmail = model.TeacherEmail;
                    model_1.CircleCourse = "Total";
                    model_1.Status = "null";
                    List<PDReportModel> timeSpentList =
                        list.Where(e =>
                            e.CommunityDistrict == model_1.CommunityDistrict &&
                            e.SchoolName == model_1.SchoolName &&
                            model_1.TeacherEmail == e.TeacherEmail).ToList();
                    TimeSpan ts = new TimeSpan();
                    foreach (var pdReport in timeSpentList)
                    {
                        if (pdReport.TimeSpentInCourse != "")
                            ts += DateTime.Parse(pdReport.TimeSpentInCourse) - DateTime.Now.Date;
                    }
                    model_1.TimeSpentInCourse = ts.ToString();

                    model_1.CountofPosts = list.Where(e => e.CommunityDistrict == model_1.CommunityDistrict
                                                           && e.SchoolName == model_1.SchoolName &&
                                                           e.TeacherEmail == model_1.TeacherEmail)
                        .Sum(e => e.CountofPosts);

                    model_1.CourseViewed = list.Where(e => e.CommunityDistrict == model_1.CommunityDistrict
                                                           && e.SchoolName == model_1.SchoolName &&
                                                           e.TeacherEmail == model_1.TeacherEmail)
                        .Sum(e => e.CourseViewed);
                    list1.Add(model_1);
                }
            }
            return JsonHelper.SerializeObject(list1);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.PDReports, Anonymity = Anonymous.Verified, IsSignUrl = false)]
        public string SearchSummaryReport(int? communityId, int? schoolId, string teacher, int status)
        {
            List<int> communityIds = new List<int>();
            List<int> schoolIds = new List<int>();
            switch (UserInfo.Role)
            {
                case Role.Statewide:
                case Role.Community:
                case Role.District_Community_Specialist:
                    if (communityId == null)
                    {
                        communityIds = UserInfo.UserCommunitySchools.Select(e => e.CommunityId).ToList();
                    }
                    break;
                case Role.Community_Specialist_Delegate:
                case Role.District_Community_Delegate:
                    if (communityId == null)
                    {
                        communityIds = UserInfo.UserCommunitySchools.Select(e => e.CommunityId).ToList();
                    }
                    break;
                case Role.Principal:
                case Role.TRS_Specialist:
                case Role.School_Specialist:
                    if (communityId == null)
                    {

                    }
                    if (schoolId == null)
                    {
                        schoolIds = UserInfo.UserCommunitySchools.Select(e => e.SchoolId).ToList();
                    }
                    break;
                case Role.Principal_Delegate:
                case Role.TRS_Specialist_Delegate:
                case Role.School_Specialist_Delegate:
                    if (schoolId == null)
                    {
                        var user = userBusiness.GetUser(UserInfo.CommunityUser.ParentId);
                        schoolIds = user.UserCommunitySchools.Select(e => e.SchoolId).ToList();
                    }
                    break;
                case Role.Teacher:
                    if (teacher == "")
                        teacher = UserInfo.FirstName + " " + UserInfo.LastName;
                    break;
                case Role.Parent:
                    communityIds.Add(0);
                    schoolIds.Add(0);
                    break;
            }
            if (communityId != null)
                communityIds.Add(communityId.Value);
            if (schoolId != null)
                schoolIds.Add(schoolId.Value);
            List<SummaryReportModel> list = _reportBusiness.GetSummaryReport(communityIds, schoolIds, teacher, status);
            return JsonHelper.SerializeObject(list);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.PDReports, Anonymity = Anonymous.Verified, IsSignUrl = false)]
        public void GetPdReport(int? communityId, int? schoolId, string teacher, int status)
        {
            List<int> communityIds = new List<int>();
            List<int> schoolIds = new List<int>();
            switch (UserInfo.Role)
            {
                case Role.Statewide:
                case Role.Community:
                case Role.District_Community_Specialist:
                    if (communityId == null)
                    {
                        communityIds = UserInfo.UserCommunitySchools.Select(e => e.CommunityId).ToList();
                    }
                    break;
                case Role.Community_Specialist_Delegate:
                case Role.District_Community_Delegate:
                    if (communityId == null)
                    {
                        communityIds = UserInfo.UserCommunitySchools.Select(e => e.CommunityId).ToList();
                    }
                    break;
                case Role.Principal:
                case Role.TRS_Specialist:
                case Role.School_Specialist:
                    if (communityId == null)
                    {

                    }
                    if (schoolId == null)
                    {
                        schoolIds = UserInfo.UserCommunitySchools.Select(e => e.SchoolId).ToList();
                    }
                    break;
                case Role.Principal_Delegate:
                case Role.TRS_Specialist_Delegate:
                case Role.School_Specialist_Delegate:
                    if (schoolId == null)
                    {
                        var user = userBusiness.GetUser(UserInfo.CommunityUser.ParentId);
                        schoolIds = user.UserCommunitySchools.Select(e => e.SchoolId).ToList();
                    }
                    break;
                case Role.Teacher:
                    if (teacher == "")
                        teacher = UserInfo.FirstName + " " + UserInfo.LastName;
                    break;
                case Role.Parent:
                    communityIds.Add(0);
                    schoolIds.Add(0);
                    break;
            }
            if (communityId != null)
                communityIds.Add(communityId.Value);
            if (schoolId != null)
                schoolIds.Add(schoolId.Value);
            string path = "";
            _reportBusiness.GetPdReport(out path, communityIds, schoolIds, teacher, status);
            FileStream fs = new FileStream(path, FileMode.Open);
            byte[] bytes = new byte[(int)fs.Length];
            fs.Read(bytes, 0, bytes.Length);
            fs.Close();
            if (System.IO.File.Exists(@path))
            {
                System.IO.File.Delete(@path);
            }
            Response.ContentType = "application/octet-stream";
            Response.AddHeader("Content-Disposition",
                "attachment;  filename=" +
                HttpUtility.UrlEncode("OnlineCoursesReport" + DateTime.Now.ToString("_yyyyMMdd_hhmmss") + ".xlsx",
                    System.Text.Encoding.UTF8));
            Response.BinaryWrite(bytes);
            Response.Flush();
            Response.End();
        }
        #endregion

        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.MentorCoachReport, Anonymity = Anonymous.Verified, IsSignUrl = false)]
        public void GetCoachReport(int? communityId, int mentorCoach, string funding, int status)
        {
            //控制权限到Community User，低于Community User的角色不应该看到该报表
            List<int> communityIds = new List<int>();
            switch (UserInfo.Role)
            {
                case Role.Statewide:
                case Role.Community:
                case Role.District_Community_Specialist:
                    if (communityId == null)
                        communityIds = UserInfo.UserCommunitySchools.Select(e => e.CommunityId).ToList();
                    break;
                case Role.Community_Specialist_Delegate:
                case Role.District_Community_Delegate:
                    if (communityId == null)
                    {
                        var user = userBusiness.GetUser(UserInfo.CommunityUser.ParentId);
                        communityIds = user.UserCommunitySchools.Select(e => e.CommunityId).ToList();
                    }
                    break;
                case Role.Principal:
                case Role.Principal_Delegate:
                case Role.TRS_Specialist:
                case Role.TRS_Specialist_Delegate:
                case Role.School_Specialist:
                case Role.School_Specialist_Delegate:
                case Role.Teacher:
                case Role.Parent:
                    communityIds.Add(0);
                    break;
            }
            if (communityId != null)
                communityIds.Add(communityId.Value);
            string path = "";
            _reportBusiness.GetCoachReport(out path, communityIds, mentorCoach, funding, status);
            FileStream fs = new FileStream(path, FileMode.Open);
            byte[] bytes = new byte[(int)fs.Length];
            fs.Read(bytes, 0, bytes.Length);
            fs.Close();
            if (System.IO.File.Exists(@path))
            {
                System.IO.File.Delete(@path);
            }
            Response.ContentType = "application/octet-stream";
            Response.AddHeader("Content-Disposition",
                "attachment;  filename=" +
                HttpUtility.UrlEncode("CoachReport" + DateTime.Now.ToString("_yyyyMMdd_hhmmss") + ".xlsx",
                    System.Text.Encoding.UTF8));
            Response.BinaryWrite(bytes);
            Response.Flush();
            Response.End();
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.EverServiced, Anonymity = Anonymous.Verified, IsSignUrl = false)]
        public void GetServiceReport(int? communityId, int? schoolId, int serviceType)
        {
            List<int> communityIds = new List<int>();
            List<int> schoolIds = new List<int>();
            switch (UserInfo.Role)
            {
                case Role.Statewide:
                case Role.Community:
                case Role.District_Community_Specialist:
                    if (communityId == null)
                    {
                        communityIds = UserInfo.UserCommunitySchools.Select(e => e.CommunityId).ToList();
                    }
                    break;
                case Role.Community_Specialist_Delegate:
                case Role.District_Community_Delegate:
                    if (communityId == null)
                    {
                        communityIds = UserInfo.UserCommunitySchools.Select(e => e.CommunityId).ToList();
                    }
                    break;
                case Role.Principal:
                case Role.TRS_Specialist:
                case Role.School_Specialist:
                    if (communityId == null)
                    {

                    }
                    if (schoolId == null)
                    {
                        schoolIds = UserInfo.UserCommunitySchools.Select(e => e.SchoolId).ToList();
                    }
                    break;
                case Role.Principal_Delegate:
                case Role.TRS_Specialist_Delegate:
                case Role.School_Specialist_Delegate:
                    if (schoolId == null)
                    {
                        var user = userBusiness.GetUser(UserInfo.CommunityUser.ParentId);
                        schoolIds = user.UserCommunitySchools.Select(e => e.SchoolId).ToList();
                    }
                    break;
                case Role.Teacher:
                case Role.Parent:
                    communityIds.Add(0);
                    schoolIds.Add(0);
                    break;
            }
            if (communityId != null)
                communityIds.Add(communityId.Value);
            if (schoolId != null)
                schoolIds.Add(schoolId.Value);
            string path = "";
            _reportBusiness.GetServiceReport(out path, communityIds, schoolIds, serviceType);
            FileStream fs = new FileStream(path, FileMode.Open);
            byte[] bytes = new byte[(int)fs.Length];
            fs.Read(bytes, 0, bytes.Length);
            fs.Close();
            if (System.IO.File.Exists(@path))
            {
                System.IO.File.Delete(@path);
            }
            Response.ContentType = "application/octet-stream";
            Response.AddHeader("Content-Disposition",
                "attachment;  filename=" +
                HttpUtility.UrlEncode("ServiceReport" + DateTime.Now.ToString("_yyyyMMdd_hhmmss") + ".xlsx",
                    System.Text.Encoding.UTF8));
            Response.BinaryWrite(bytes);
            Response.Flush();
            Response.End();
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.ParticipationCounts, Anonymity = Anonymous.Verified, IsSignUrl = false)]
        public void GeParticipationCountsReport(int? communityId, string funding, DateTime? startDate, DateTime? endDate,
          int status)
        {
            //控制权限到Community User，低于Community User的角色不应该看到该报表
            List<int> communityIds = new List<int>();
            switch (UserInfo.Role)
            {
                case Role.Statewide:
                case Role.Community:
                case Role.District_Community_Specialist:
                    if (communityId == null)
                    {
                        communityIds = UserInfo.UserCommunitySchools.Select(e => e.CommunityId).ToList();
                    }
                    break;
                case Role.Community_Specialist_Delegate:
                case Role.District_Community_Delegate:
                    if (communityId == null)
                    {
                        var user = userBusiness.GetUser(UserInfo.ID);
                        communityIds = user.UserCommunitySchools.Select(e => e.CommunityId).ToList();
                    }
                    break;
                case Role.Principal:
                case Role.Principal_Delegate:
                case Role.TRS_Specialist:
                case Role.TRS_Specialist_Delegate:
                case Role.School_Specialist:
                case Role.School_Specialist_Delegate:
                case Role.Teacher:
                case Role.Parent:
                    communityIds.Add(0);
                    break;
            }
            if (communityId != null)
                communityIds.Add(communityId.Value);
            if (startDate == null)
                startDate = DateTime.Now;
            if (endDate == null)
                endDate = DateTime.Now;
            string path = "";
            List<int> fundingList = new List<int>();
            if (funding.Trim() != string.Empty)
                fundingList = funding.Trim().Split(',').Select(r => r.ToInt32()).ToList();
            _reportBusiness.GetParticipationCountsReport(out path, communityIds, fundingList, startDate, endDate, status);
            FileStream fs = new FileStream(path, FileMode.Open);
            byte[] bytes = new byte[(int)fs.Length];
            fs.Read(bytes, 0, bytes.Length);
            fs.Close();
            if (System.IO.File.Exists(@path))
            {
                System.IO.File.Delete(@path);
            }
            Response.ContentType = "application/octet-stream";
            Response.AddHeader("Content-Disposition",
                "attachment;  filename=" +
                HttpUtility.UrlEncode("ParticipationCounts" + DateTime.Now.ToString("_yyyyMMdd_hhmmss") + ".xlsx",
                    System.Text.Encoding.UTF8));
            Response.BinaryWrite(bytes);
            Response.Flush();
            Response.End();
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.TeacherTurnoverReport, Anonymity = Anonymous.Verified)]
        public ActionResult SelectDate()
        {
            return View();
        }


        private IEnumerable<SelectListItem> WrapTurnoverCoachOptions(IEnumerable<SelectItemModel> list)
        {
            return list.ToSelectList(ViewTextHelper.DefaultAllText, "0")
                        .AddDefaultItem("No Coach", "-1")
                        .AddDefaultItem(ViewTextHelper.DefaultPleaseSelectText, "");
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.TeacherTurnoverReport, Anonymity = Anonymous.Verified, IsSignUrl = false)]
        public void TurnoverReport(DateTime? startDate, DateTime? endDate, int? communityId, string communityName)
        {
            if (startDate == null || startDate < CommonAgent.MinDate) startDate = DateTime.Now.Date;
            if (endDate == null || endDate < CommonAgent.MinDate) endDate = DateTime.Now.Date;

            Dictionary<string, object> searchCriteria = new Dictionary<string, object>();
            if (!string.IsNullOrEmpty(communityName))
                searchCriteria.Add("Community", communityName);

            List<int> schoolIds = null;
            List<int> communityIds = null;

            if (UserInfo.Role == Role.Super_admin)
            {
                // all communities
            }
            else if (UserInfo.Role <= Role.Mentor_coach || UserInfo.Role == Role.Statewide)
            {
                // 多个community
                communityIds = userBusiness.GetAssignedCommunities(UserInfo.ID).Select(r => r.ID).ToList();
            }
            else if (UserInfo.Role == Role.Community
                || UserInfo.Role == Role.District_Community_Specialist)
            {
                //可能多个community
                communityIds = userBusiness.GetAssignedCommunities(UserInfo.ID).Select(r => r.ID).ToList();
            }
            else if (UserInfo.Role == Role.TRS_Specialist
                     || UserInfo.Role == Role.School_Specialist
                     || UserInfo.Role == Role.Principal
                )
            {
                // Schoold
                schoolIds = UserInfo.UserCommunitySchools.Select(e => e.SchoolId).ToList();
            }
            else
            {
                schoolIds = new List<int>();
                communityIds = new List<int>();
            }

            if (communityId.HasValue)
            {
                communityIds = new List<int>() { communityId.Value };
            }

            string path = "";

            _reportBusiness.TurnoverReport(out path, startDate.Value, endDate.Value.AddDays(1), communityIds, schoolIds,
                searchCriteria);
            string downloadName = "TeacherTurnoverReport.xlsx";
            FileHelper.ResponseFile(path, downloadName.FormatReportName(DateTime.Now), "", true);
        }


        /// <summary>
        /// Coaching Load by Community  只有 CLI 内部用户才有权限
        /// </summary>
        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.CoachingHoursbyCommunitys, Anonymity = Anonymous.Verified, IsSignUrl = false)]
        public void CoachingHoursbyCommunity()
        {
            string path = "";
            _reportBusiness.CoachingHoursbyCommunity(UserInfo.Role <= Role.Mentor_coach, out path);
            FileStream fs = new FileStream(path, FileMode.Open);
            byte[] bytes = new byte[(int)fs.Length];
            fs.Read(bytes, 0, bytes.Length);
            fs.Close();
            if (System.IO.File.Exists(@path))
            {
                System.IO.File.Delete(@path);
            }
            Response.ContentType = "application/octet-stream";
            Response.AddHeader("Content-Disposition", "attachment;  filename=" +
HttpUtility.UrlEncode("CoachingLoad_byCommunity" + DateTime.Now.ToString("_yyyyMMdd_hhmmss") + ".xlsx",
                    System.Text.Encoding.UTF8));
            Response.BinaryWrite(bytes);
            Response.Flush();
            Response.End();
        }
    }
}