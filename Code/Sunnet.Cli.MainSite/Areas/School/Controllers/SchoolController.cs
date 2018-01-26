using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;
using Newtonsoft.Json;
using StructureMap;
using Sunnet.Cli.Business.Classes;
using Sunnet.Cli.Business.Classes.Models;
using Sunnet.Cli.Business.Communities;
using Sunnet.Cli.Business.Communities.Enums;
using Sunnet.Cli.Business.MasterData;
using Sunnet.Cli.Business.Schools;
using Sunnet.Cli.Business.Schools.Models;
using Sunnet.Cli.Business.StatusTracking;
using Sunnet.Cli.Business.TRSClasses;
using Sunnet.Cli.Business.TRSClasses.Models;
using Sunnet.Cli.Business.Users;
using Sunnet.Cli.Core.Classes.Enums;
using Sunnet.Cli.Core.Common.Enums;
using Sunnet.Cli.Core.Communities.Entities;
using Sunnet.Cli.Core.Schools.Entities;
using Sunnet.Cli.Core.Schools.Enums;
using Sunnet.Cli.Core.StatusTracking.Entities;
using Sunnet.Cli.Core.StatusTracking.Enums;
using Sunnet.Cli.Core.Trs;
using Sunnet.Cli.Core.TRSClasses.Enums;
using Sunnet.Cli.Core.Users.Entities;
using Sunnet.Cli.Core.Users.Enums;
using Sunnet.Cli.MainSite.Controllers;
using Sunnet.Cli.UIBase.Models;
using Sunnet.Framework.Core.Tool;
using Sunnet.Framework.Encrypt;
using Sunnet.Framework.Extensions;
using Sunnet.Framework.Helpers;
using Sunnet.Framework.Resources;
using Sunnet.Cli.UIBase.Filters;
using Sunnet.Framework.Permission;
using Sunnet.Cli.Business.Permission;
using Sunnet.Cli.Business.Permission.Models;


namespace Sunnet.Cli.MainSite.Areas.School.Controllers
{
    public class SchoolController : BaseController
    {
        //
        // GET: /School/Index/
        private readonly SchoolBusiness _schoolBusiness;
        private TRSClassBusiness _trsClassBusiness;
        private readonly MasterDataBusiness _masterBusiness;
        private readonly CommunityBusiness _communityBusiness;
        private UserBusiness _userBusiness;

        public SchoolController()
        {
            _schoolBusiness = new SchoolBusiness(UnitWorkContext);
            _masterBusiness = new MasterDataBusiness(UnitWorkContext);
            _communityBusiness = new CommunityBusiness(UnitWorkContext);
            _userBusiness = new UserBusiness(UnitWorkContext);
            _trsClassBusiness = new TRSClassBusiness(UnitWorkContext);
        }

        #region View Action

        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.School_Management, Anonymity = Anonymous.Verified)]
        public ActionResult Index()
        {
            InitAccessOperation();
            SchoolModel model = new SchoolModel();
            ViewBag.CommunityOptions = GetCommunityList(ViewTextHelper.DefaultAllText, "-1");
            ViewBag.SchoolTypeOptions = GetSchoolTypesList(ViewTextHelper.DefaultAllText, "-1", 0);
            ViewBag.StatusOptions = EntityStatus.Active.ToSelectList(true, new SelectOptions(true, "-1", ViewTextHelper.DefaultAllText));


            return View(model);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.Add, PageId = PagesModel.School_Management, Anonymity = Anonymous.Verified)]
        public ActionResult New()
        {
            ViewBag.EnableAutoAssign4StarTips = ResourceHelper.GetRM().GetInformation("EnableAutoAssign4StarTips");

            SchoolEntity school = _schoolBusiness.NewSchoolEntity();
            SchoolModel model = new SchoolModel();

            model = _schoolBusiness.EntityToModel(school, model);
            model = _schoolBusiness.InitSchoolByRole(model, UserInfo.Role);
            InitControls();

            ViewBag.CoachOptions =
               _userBusiness.GetSchoolSpecialist(new[] { school.ID })
              .ToSelectList(ViewTextHelper.DefaultPleaseSelectText, "0");
            ViewBag.Infants = "";
            ViewBag.Toddlers = "";
            ViewBag.Preschool = "";
            ViewBag.SchoolAge = "";

            bool changingStarRating = false;
            if (UserInfo.Role == Role.Super_admin || UserInfo.Role == Role.TRS_Specialist || UserInfo.Role == Role.TRS_Specialist_Delegate)
                changingStarRating = true;
            ViewBag.ChangingStarRating = changingStarRating;

            return View(model);
        }

        /// <summary>
        /// Is this user can assign community to school ？
        /// </summary>
        /// <returns></returns>
        public bool AssignAccess(SchoolEntity school)
        {
            bool res = false;
            if (UserInfo.IsCLIUser)
            {
                res = true;
            }
            else if (UserInfo.CommunityUser != null)
            {
                List<int> schoolCommunityIds = school.CommunitySchoolRelations.Select(o => o.CommunityId).ToList();
                List<int> userCommunityIds = UserInfo.UserCommunitySchools.Select(o => o.CommunityId).ToList();
                if (userCommunityIds.Any(schoolCommunityIds.Contains))
                {
                    res = true;
                }
            }
            return res;
        }


        [CLIUrlAuthorizeAttribute(Account = Authority.Edit, PageId = PagesModel.School_Management, Anonymity = Anonymous.Verified)]
        public ActionResult Edit(int id)
        {
            ViewBag.EnableAutoAssign4StarTips = ResourceHelper.GetRM().GetInformation("EnableAutoAssign4StarTips");
            SchoolModel school = _schoolBusiness.GetSchoolEntity(id, UserInfo);
            ViewBag.CoachOptions = _userBusiness.GetSchoolSpecialist(new[] { school.ID }).ToSelectList(ViewTextHelper.DefaultPleaseSelectText, "0");
            InitControls(id);
            ViewBag.IsShowTRS = _communityBusiness.CheckIfTrsBySchool(school.ID);
            ViewBag.Grounds = JsonHelper.SerializeObject(_schoolBusiness.GetPlaygrounds(id));
            ViewBag.TrsTaStatusOption = GetTrsTaOptions(school.TrsTaStatus);
            List<ClassCountAgeModel> trsClassCount = _trsClassBusiness.GetTRSClassCountAge(id);
            ViewBag.Infants = GetClassCount(trsClassCount, TrsAgeArea.Infants);
            ViewBag.Toddlers = GetClassCount(trsClassCount, TrsAgeArea.Toddlers);
            ViewBag.Preschool = GetClassCount(trsClassCount, TrsAgeArea.Preschool);
            ViewBag.SchoolAge = GetClassCount(trsClassCount, TrsAgeArea.SchoolAge);
            ViewBag.isSuperAdmin = (UserInfo.Role == Role.Super_admin);

            bool isSecondaryCommunityEditTrs = false;
            if (UserInfo.Role == Role.Community || UserInfo.Role == Role.District_Community_Specialist)
            {
                if (
                    UserInfo.UserCommunitySchools.Any(
                        e =>
                            e.Community.CommunityAssessmentRelations.Any(
                                s => ((LocalAssessment)s.AssessmentId == LocalAssessment.TexasRisingStar))))
                {
                    isSecondaryCommunityEditTrs = true;
                }
            }
            else if (UserInfo.Role == Role.School_Specialist)
            {
                if (
                    UserInfo.UserCommunitySchools.Any(
                        e =>
                            e.School.CommunitySchoolRelations.Any(
                                t =>
                                    t.Community.CommunityAssessmentRelations.Any(
                                        s => ((LocalAssessment)s.AssessmentId == LocalAssessment.TexasRisingStar)))))
                {
                    isSecondaryCommunityEditTrs = true;
                }
            }
            if (school.SchoolAccess == AccessType.ReadOnly && !isSecondaryCommunityEditTrs)
            {
                Response.Redirect("Index");
            }

            bool changingStarRating = false;
            if (UserInfo.Role == Role.Super_admin || UserInfo.Role == Role.TRS_Specialist || UserInfo.Role == Role.TRS_Specialist_Delegate)
                changingStarRating = true;
            ViewBag.ChangingStarRating = changingStarRating;

            return View(school);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.Edit, PageId = PagesModel.School_Management, Anonymity = Anonymous.Verified)]
        public ActionResult EditSchoolName(int id)
        {
            SchoolModel school = _schoolBusiness.GetSchoolEntity(id, UserInfo);
            return View(school);
        }

        private int GetClassCount(List<ClassCountAgeModel> list, TrsAgeArea area)
        {
            int result = 0;
            if (list.Count > 0)
            {
                if (list.FirstOrDefault(r => r.AgeArea == area) != null)
                {
                    result = list.FirstOrDefault(r => r.AgeArea == area).Number;
                }
            }
            return result;
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.View, PageId = PagesModel.School_Management, Anonymity = Anonymous.Verified)]
        public ActionResult View(int id)
        {
            ViewBag.EnableAutoAssign4StarTips = ResourceHelper.GetRM().GetInformation("EnableAutoAssign4StarTips");

            SchoolModel school = _schoolBusiness.GetSchoolEntity(id, UserInfo);
            InitControls();
            ViewBag.ParentAgencyName =
               _schoolBusiness.GetParentAgencyList(false).Where(o => o.ID == school.ParentAgencyId).
               Select(o => o.Name).FirstOrDefault();

            string typeName = _schoolBusiness.GetSchoolTypeName(school.SchoolTypeId);
            ViewBag.SchoolTypeName = typeName;

            if (school.SubTypeId != 0)
                ViewBag.SubSchoolTypeName = _schoolBusiness.GetSchoolTypeName(school.SubTypeId);

            if (school.EscName != 0)
                ViewBag.EscName = school.EscName.ToString();
            else
                ViewBag.EscName = "";
            ViewBag.IsShowTRS = _communityBusiness.CheckIfTrsBySchool(school.ID);
            if (school.FundingId != 0)
                if (_masterBusiness.GetFunding(school.FundingId) != null)
                    ViewBag.fundingName = _masterBusiness.GetFunding(school.FundingId).Name;

            //if (school.TrsProviderId != 0)
            //    ViewBag.TrsName = schoolBusiness.GetTrsName(school.TrsProviderId);

            if (school.IspId != 0)
            {
                string ispName = _schoolBusiness.GetIspName(school.IspId);
                if (ispName != "Other")
                    ViewBag.IspName = ispName;
                else
                    ViewBag.IspName = school.IspOther;
            }

            if (school.PrimaryTitleId != 0)
                ViewBag.primaryTitle = _masterBusiness.GetTitle(school.PrimaryTitleId).Name;


            if (school.SecondaryTitleId != 0)
                ViewBag.secondaryTitle = _masterBusiness.GetTitle(school.SecondaryTitleId).Name;

            string stateName = "";
            if (school.StateId != 0)
                stateName = _masterBusiness.GetState(school.StateId).Name;
            string countyName = "";
            if (school.CountyId != 0)
                countyName = _masterBusiness.GetCounty(school.CountyId).Name;
            string FullAddr = school.City + ", " + (countyName == "" ? "" : countyName + ", ") + stateName + " " + school.Zip;
            ViewBag.FullAddr = FullAddr;

            string mailStateName = "";
            string mailCountyName = "";
            if (school.MailingStateId != 0)
                mailStateName = _masterBusiness.GetState(school.MailingStateId).Name;
            if (school.MailingCountyId != 0)
                mailCountyName = _masterBusiness.GetCounty(school.MailingCountyId).Name;

            string FullMailAddr = school.MailingCity + ", " + (mailCountyName == "" ? "" : mailCountyName + ", ") + mailStateName + " " + school.MailingZip;
            ViewBag.FullMailAddr = FullMailAddr;

            ViewBag.TrsAssessor = "";
            if (school.TrsAssessorId > 0)
            {
                var TrsAssessor = _userBusiness.GetUserBaseModel(school.TrsAssessorId);
                if (TrsAssessor != null)
                    ViewBag.TrsAssessor = TrsAssessor.FullName;
            }
            ViewBag.TrsTaStatusOption = GetTrsTaOptions(school.TrsTaStatus);
            List<ClassCountAgeModel> trsClassCount = _trsClassBusiness.GetTRSClassCountAge(id);

            ViewBag.Infants = GetClassCount(trsClassCount, TrsAgeArea.Infants);
            ViewBag.Toddlers = GetClassCount(trsClassCount, TrsAgeArea.Toddlers);
            ViewBag.Preschool = GetClassCount(trsClassCount, TrsAgeArea.Preschool);
            ViewBag.SchoolAge = GetClassCount(trsClassCount, TrsAgeArea.SchoolAge);

            return View(school);
        }



        [CLIUrlAuthorizeAttribute(Account = Authority.Edit, PageId = PagesModel.School_Management, Anonymity = Anonymous.Verified)]
        public ActionResult Playgrounds(int schoolId)
        {
            SchoolModel school = _schoolBusiness.GetSchoolEntity(schoolId, UserInfo);
            if (school.SchoolAccess == AccessType.ReadOnly)
                Response.Redirect("Index");

            return View(school);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.Assign, PageId = PagesModel.School_Management, Anonymity = Anonymous.Verified)]
        public ActionResult AssignCommunity(int schoolId)
        {
            SchoolModel school = _schoolBusiness.GetSchoolEntity(schoolId, UserInfo);
            if (school.Status == SchoolStatus.Inactive)
            {
                Response.Redirect("Index");
            }
            if (school.SchoolAccess == AccessType.ReadOnly)
                Response.Redirect("Index");

            return View(school);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.Edit, PageId = PagesModel.School_Management, Anonymity = Anonymous.Verified)]
        public ActionResult VerifyBasicSchool(string msg)
        {
            bool isValid = true, ViewRequest = false;
            var encrypt = ObjectFactory.GetInstance<IEncrypt>();
            string basicSchoolIdStr = encrypt.Decrypt(msg);
            string[] fields = basicSchoolIdStr.Split('&');//basicSchoolId & creatorId & Expiration Time
            int communityId = 0, basicSchoolId = 0, createrId = 0;
            DateTime endTime = DateTime.MaxValue;
            BasicSchoolEntity basicSchool = new BasicSchoolEntity();
            if (fields.Length != 4)
            {
                isValid = false;
            }
            else
            {
                int.TryParse(fields[0], out communityId);
                int.TryParse(fields[1], out basicSchoolId);
                int.TryParse(fields[2], out createrId);
                DateTime.TryParse(fields[3], out endTime);
            }
            if (communityId == 0
                || basicSchoolId == 0
                || createrId == 0
               )
            {
                isValid = false;
            }
            else
            {
                basicSchool = _schoolBusiness.GetBasicSchoolById(basicSchoolId);
                if (basicSchool == null)
                {
                    isValid = false;
                    basicSchool = new BasicSchoolEntity();
                }
            }
            if (isValid)
            {
                if ((int)UserInfo.Role > 100 && createrId != UserInfo.ID)
                {
                    ViewBag.Content = "Access denied .";
                    ViewBag.msg = "";
                    isValid = false;
                }
                else if (basicSchool.Status != SchoolStatus.Pending)
                {
                    ViewBag.Content = "This link is invalid .";
                    ViewBag.msg = "";
                    isValid = false;
                }
                else if (endTime < DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd 00:00:00")))
                {
                    if (createrId == UserInfo.ID)
                    {
                        ViewRequest = true;
                    }
                    else
                    {
                        ViewBag.Content = "This link is expired.";
                        ViewBag.msg = "";
                        isValid = false;
                    }
                }
                else
                {
                    ViewBag.Content = "This basic school information is currently awaiting approval. ";
                    ViewBag.msg = msg;
                    if (createrId == UserInfo.ID)
                    {
                        ViewRequest = true;
                    }
                }

            }
            else
            {
                ViewBag.Content = "This link is invalid .";

            }

            ViewBag.isValid = isValid;
            ViewBag.msg = msg;
            ViewBag.ViewRequest = ViewRequest;
            return View(basicSchool);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.Edit, PageId = PagesModel.School_Management, Anonymity = Anonymous.Verified)]
        public ActionResult VerifySelectedSchool(string msg)
        {
            bool isValid = true, ViewRequest = false; ;
            var encrypt = ObjectFactory.GetInstance<IEncrypt>();
            string schoolIdStr = encrypt.Decrypt(msg);
            string[] fields = schoolIdStr.Split('&');//basicSchoolId & creatorId & Expiration Time
            int schoolId = 0, selectedCommunityId = 0, createrId = 0;
            DateTime endTime = DateTime.MaxValue;
            SchoolModel school = new SchoolModel();
            CommunityEntity community = new CommunityEntity();
            if (fields.Length != 4)
            {
                isValid = false;
            }
            else
            {
                int.TryParse(fields[0], out schoolId);
                int.TryParse(fields[1], out selectedCommunityId);
                int.TryParse(fields[2], out createrId);
                DateTime.TryParse(fields[3], out endTime);
                if (createrId == UserInfo.ID)
                {
                    school = _schoolBusiness.GetActiveSchoolEntity(schoolId);
                }
                else
                {
                    school = _schoolBusiness.GetSchoolEntity(schoolId, UserInfo);
                }
                community = _communityBusiness.GetCommunity(selectedCommunityId);
            }
            if (school == null
                || community == null
                || createrId == 0
               )
            {
                ViewBag.Content = "This link is invalid, only Supper Admin and assigned Principal can visit this link.";
                isValid = false;
            }
            else if (endTime < DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd 00:00:00")))
            {
                if (createrId == UserInfo.ID)
                {
                    ViewRequest = true;
                }
                else
                {
                    ViewBag.Content = "This link is expired.";
                    isValid = false;
                }

            }
            if (isValid)
            {
                if (_schoolBusiness.GetAssignedCommIds(schoolId).Contains(selectedCommunityId))
                {
                    isValid = false;
                    ViewBag.Content = "The school and community relationship exsits.";
                }
                else
                {
                    ViewBag.community = community;
                    ViewBag.Content = "The following school request is awaiting approval: Add " + school.SchoolName + " to " + community.Name;
                    ViewBag.msg = msg;
                    if (createrId == UserInfo.ID)
                    {
                        ViewRequest = true;
                    }
                }
            }
            else
            {
                //ViewBag.Content = "This link is invalid or expired.";
                ViewBag.msg = "";
            }
            ViewBag.isValid = isValid;
            ViewBag.ViewRequest = ViewRequest;
            return View(school);
        }

        #endregion

        #region Ajax Methods
        [CLIUrlAuthorizeAttribute(Account = Authority.Add, PageId = PagesModel.School_Management, Anonymity = Anonymous.Verified)]
        public string IsSchoolExists(int basicSchoolId = 0, int selectCommunityId = 0)
        {

            SchoolEntity school = _schoolBusiness.SchoolExists(basicSchoolId);
            if (school != null)
            {
                StatusTrackingBusiness _statusBusiness = new StatusTrackingBusiness();
                StatusTrackingEntity track = _statusBusiness.GetExistingTracking(selectCommunityId, school.ID, StatusType.AddSchool);

                if (school.CommunitySchoolRelations.Select(o => o.CommunityId).Contains(selectCommunityId))
                {
                    return school.ID.ToString() + "_" + "exsit";
                }
                if (track != null && track.Status == StatusEnum.Pending)
                {
                    return school.ID.ToString() + "_" + "pending";
                }
                if (AssignAccess(school))
                {
                    return school.ID.ToString() + "_" + "true";
                }
                else
                {
                    return school.ID.ToString() + "_" + "false";
                }
            }
            return "";
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        [CLIUrlAuthorizeAttribute(Account = Authority.Add, PageId = PagesModel.School_Management, Anonymity = Anonymous.Verified)]
        public string New(SchoolModel school, string txtBasicSchool)
        {
            var response = new PostFormResponse();
            OperationResult result = new OperationResult(OperationResultType.Success);
            school.SchoolName = txtBasicSchool;

            school.CreateBy = UserInfo.ID;
            school.UpdateBy = 0;
            school.CreateFrom = "SchoolPage";
            school.UpdateFrom = "";
            result = _schoolBusiness.AddNewSchool(school.CommunityId, school, UserInfo);
            response.Success = result.ResultType == OperationResultType.Success;
            response.Data = school;
            response.Message = result.Message;
            return JsonConvert.SerializeObject(response);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CLIUrlAuthorizeAttribute(Account = Authority.Edit, PageId = PagesModel.School_Management, Anonymity = Anonymous.Verified)]
        public string Update(SchoolModel school)
        {
            var response = new PostFormResponse();

            school.UpdateBy = UserInfo.ID;
            school.UpdateFrom = "SchoolPage";
            OperationResult result = _schoolBusiness.UpdateSchool(school, UserInfo.Role);
            response.Success = result.ResultType == OperationResultType.Success;
            response.Data = school;
            response.Message = result.Message;
            return JsonConvert.SerializeObject(response);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.View, PageId = PagesModel.School_Management, Anonymity = Anonymous.Verified)]
        public string GetSchool(int schoolId)
        {
            SchoolModel school = _schoolBusiness.GetSchoolEntity(schoolId, UserInfo);
            return JsonHelper.SerializeObject(school);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CLIUrlAuthorizeAttribute(Account = Authority.Edit, PageId = PagesModel.School_Management, Anonymity = Anonymous.Verified)]
        public string EditSchoolName()
        {
            int schoolId = 0;
            string newSchoolName = "";
            OperationResult result = new OperationResult(OperationResultType.Success);
            int.TryParse(Request.Params["ID"], out schoolId);
            if (Request.Params["SchoolName"] != null)
                newSchoolName = Request.Params["SchoolName"];
            SchoolEntity findSchool = new SchoolEntity();
            if (newSchoolName == "" || schoolId <= 0)
            {
                result.ResultType = OperationResultType.Error;
                result.Message = "School Name is required.";
            }
            else
            {
                findSchool = _schoolBusiness.GetSchool(schoolId);
                if (findSchool != null)
                {
                    bool IsSchoolNameExsits = _schoolBusiness.IsSchoolNameExsits(newSchoolName, schoolId);
                    bool isBasicSchoolNameExsits = _schoolBusiness.IsBasicSchoolNameExsits(newSchoolName, findSchool.BasicSchoolId);
                    if (IsSchoolNameExsits || isBasicSchoolNameExsits)
                    {
                        result.ResultType = OperationResultType.Error;
                        result.Message = "School Name Exsits.";
                    }
                }
                else
                {
                    result.ResultType = OperationResultType.Error;
                    result.Message = "School is not exsit.";
                }
            }
            if (result.ResultType == OperationResultType.Success)
            {
                if (findSchool != null)
                {
                    findSchool.Name = newSchoolName;
                    result = _schoolBusiness.UpdateSchool(findSchool);
                    if (result.ResultType == OperationResultType.Success)
                    {
                        BasicSchoolEntity findBasicSchool = _schoolBusiness.GetBasicSchoolById(findSchool.BasicSchoolId);
                        findBasicSchool.Name = newSchoolName;
                        result = _schoolBusiness.UpdateBasicSchool(findBasicSchool);
                    }
                }
            }
            var response = new PostFormResponse();
            response.Data = newSchoolName;
            response.Success = result.ResultType == OperationResultType.Success;
            response.Message = result.Message;
            return JsonConvert.SerializeObject(response);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.School_Management, Anonymity = Anonymous.Verified)]
        public string Search(string communityName = "", string schoolName = "", string schoolId = "", int schoolTypeId = -1
            , int communityId = -1, int status = -1, string sort = "SchoolName", string order = "Asc", int first = 0, int count = 10)
        {
            var users = new List<int>() { 0, 1 };
            var total = 0;

            var expression = PredicateHelper.True<SchoolEntity>();

            if (communityId >= 1)
                expression = expression.And(o => o.CommunitySchoolRelations.Count(r => r.CommunityId == communityId) > 0);
            else if (communityName.Trim() != string.Empty)
                expression = expression.And(o => o.CommunitySchoolRelations.Count(r => r.Community.Name.Contains(communityName)) > 0);


            if (schoolName.Trim() != string.Empty)
                expression = expression.And(o => o.BasicSchool.Name.Contains(schoolName));
            if (schoolTypeId >= 1)
                expression = expression.And(o => o.SchoolTypeId.Equals(schoolTypeId));
            if (schoolId.Trim() != string.Empty)
                expression = expression.And(o => o.SchoolId.Contains(schoolId));
            if (status >= 0)
                expression = expression.And(o => (int)o.Status == status);

            expression = expression.And(o => o.Status != SchoolStatus.Pending);



            if (sort == "CommunityName")
                sort = "CommunityNames";
            var list = _schoolBusiness.SearchSchools(UserInfo, expression, sort, order, first, count, out total);
            var result = new { total = total, data = list };
            return JsonHelper.SerializeObject(result);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.Administrative, Anonymity = Anonymous.Logined)]
        public string GetSchoolSelectList(string keyword, int communityId = 0, string schoolName = "", bool isActive = true)
        {
            var expression = PredicateHelper.True<SchoolEntity>();

            if (communityId > 0)
                expression = expression.And(o => o.CommunitySchoolRelations.Count(r => r.CommunityId == communityId) > 0);
            if (schoolName != null && schoolName.Trim() != string.Empty)
                expression = expression.And(o => o.Name.Contains(schoolName));
            var schoolList = _schoolBusiness.GetSchoolsSelectList(UserInfo, expression, isActive);
            return JsonHelper.SerializeObject(schoolList);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.Administrative, Anonymity = Anonymous.Logined)]
        public string GetPrimarySchoolSelectList(string keyword, int communityId = 0, string schoolName = "", bool isActive = true)
        {
            var expression = PredicateHelper.True<SchoolEntity>();

            if (communityId > 0)
                expression = expression.And(o => o.CommunitySchoolRelations.Count(r => r.CommunityId == communityId) > 0);
            if (schoolName != null && schoolName.Trim() != string.Empty)
                expression = expression.And(o => o.Name.Contains(schoolName));
            var schoolList = _schoolBusiness.GetPrimarySchoolsSelectList(UserInfo, expression, isActive);
            return JsonHelper.SerializeObject(schoolList);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.Administrative, Anonymity = Anonymous.Logined)]
        public string GetAllSchoolSelectList(string keyword, int communityId = 0, string schoolName = "", bool isActive = true)
        {
            var expression = PredicateHelper.True<SchoolEntity>();

            if (communityId > 0)
                expression = expression.And(o => o.CommunitySchoolRelations.Count(r => r.CommunityId == communityId) > 0);
            if (schoolName != null && schoolName.Trim() != string.Empty)
                expression = expression.And(o => o.Name.Contains(schoolName));
            var schoolList = _schoolBusiness.GetAllSchoolsSelectList(expression, isActive);
            return JsonHelper.SerializeObject(schoolList);
        }

        /// <summary>
        /// GET SCHOOLS BY  CITY  ZIP  NAME
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="communityId"></param>
        /// <param name="schoolName"></param>
        /// <param name="isActive"></param>
        /// <returns></returns>
        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.Administrative, Anonymity = Anonymous.Logined)]
        public string GetSchoolSelectListByKey(string keyword, int communityId = 0, string schoolName = "", bool isActive = true)
        {
            var expression = PredicateHelper.True<SchoolEntity>();
            //TODO:School CommunityId DEL
            if (communityId > 0)
                expression = expression.And(o => o.CommunitySchoolRelations.Count(r => r.CommunityId == communityId) > 0);
            if (schoolName != null && schoolName.Trim() != string.Empty)
            {
                expression = expression.And(
                    o => (
                        o.BasicSchool.Name.Contains(schoolName)
                         || o.Zip.Contains(schoolName)
                         || o.City.Contains(schoolName)
                         )
                        );
            }
            var schoolList = _schoolBusiness.GetSchoolsSelectList(UserInfo, expression, isActive);
            return JsonHelper.SerializeObject(schoolList);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.Administrative, Anonymity = Anonymous.Logined)]
        public string GetSchoolIdSelectList(string keyword, int communityId = 0, string schoolId = "", bool isActiveSchoolId = true)
        {
            var expression = PredicateHelper.True<SchoolEntity>();
            //TODO:School CommunityId DEL
            //if (communityId > 0)
            //    expression = expression.And(o => o.CommunityId == communityId);
            if (schoolId != null && schoolId.Trim() != string.Empty)
                expression = expression.And(o => o.SchoolId.Contains(schoolId));
            var schoolList = _schoolBusiness.GetSchoolsSelectList(UserInfo, expression, isActiveSchoolId);
            return JsonHelper.SerializeObject(schoolList);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.Administrative, Anonymity = Anonymous.Logined)]
        public string GetBasicSchoolSelectList(string keyword)
        {
            IEnumerable<SchoolSelectItemModel> list = _schoolBusiness.GetBasicSchoolSelectList(keyword);
            return JsonHelper.SerializeObject(list);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.Administrative, Anonymity = Anonymous.Logined)]
        public int GetAtRiskPercent(int schoolId)
        {
            return _schoolBusiness.GetAtRiskPercent(schoolId);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.Administrative, Anonymity = Anonymous.Logined)]
        public string GetSchoolType(int schoolId)
        {
            return _schoolBusiness.GetSchoolType(schoolId);
        }


        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.Administrative, Anonymity = Anonymous.Logined)]
        public string GetCountiesByStateId(int stateId = 0)
        {
            var classList = _masterBusiness.GetCountySelectList(stateId).ToSelectList("County*", "0");

            return JsonHelper.SerializeObject(classList);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.Administrative, Anonymity = Anonymous.Logined)]
        public string GetSchoolFacilityType(int schoolId)
        {
            return _schoolBusiness.GetSchoolFacilityType(schoolId);
        }

        public bool IsShowTRS(int communityId)
        {
            return _communityBusiness.CheckIfTrsByCommunity(communityId);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.Administrative, Anonymity = Anonymous.Logined)]
        public string ApproveBasicSchool(string msg, string urlStr)
        {
            var response = new PostFormResponse();
            OperationResult result = new OperationResult(OperationResultType.Success);
            result = _schoolBusiness.ApproveBasicSchool(msg, urlStr, UserInfo.ID);
            response.Message = result.Message;
            response.Success = result.ResultType == OperationResultType.Success;
            response.Data = "";
            response.Message = result.Message;
            return JsonConvert.SerializeObject(response);
        }

        //重新发送请求
        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.Administrative, Anonymity = Anonymous.Logined)]
        public string ResendBasicSchool(string msg)
        {
            var response = new PostFormResponse();
            OperationResult result = new OperationResult(OperationResultType.Success);
            result = _schoolBusiness.ResendBasicSchool(msg, UserInfo);
            response.Message = result.Message;
            response.Success = result.ResultType == OperationResultType.Success;
            response.Data = "";
            response.Message = result.Message;
            return JsonConvert.SerializeObject(response);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.Administrative, Anonymity = Anonymous.Logined)]
        public string DenyBasicSchool(string msg, string urlStr)
        {
            var response = new PostFormResponse();
            OperationResult result = new OperationResult(OperationResultType.Success);
            result = _schoolBusiness.DenyBasicSchool(msg, urlStr, UserInfo.ID);
            response.Message = result.Message;
            response.Success = result.ResultType == OperationResultType.Success;
            response.Data = "";
            response.Message = result.Message;
            return JsonConvert.SerializeObject(response);

        }

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.Administrative, Anonymity = Anonymous.Logined)]
        public string ApproveSelectedSchool(string msg, string urlStr)
        {
            var response = new PostFormResponse();
            OperationResult result = new OperationResult(OperationResultType.Success);
            result = _schoolBusiness.ApproveSelectedSchool(msg, UserInfo, urlStr);
            response.Message = result.Message;
            response.Success = result.ResultType == OperationResultType.Success;
            response.Data = "";
            response.Message = result.Message;
            return JsonConvert.SerializeObject(response);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.Administrative, Anonymity = Anonymous.Logined)]
        public string DenySelectedSchool(string msg, string urlStr)
        {
            var response = new PostFormResponse();
            OperationResult result = new OperationResult(OperationResultType.Success);
            result = _schoolBusiness.DenySelectedSchool(msg, UserInfo, urlStr);
            response.Message = result.Message;
            response.Success = result.ResultType == OperationResultType.Success;
            response.Data = "";
            response.Message = result.Message;
            return JsonConvert.SerializeObject(response);

        }

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.Administrative, Anonymity = Anonymous.Logined)]
        public string SendSchoolRequest(int schoolId, int communityId)
        {
            var response = new PostFormResponse();
            OperationResult result = new OperationResult(OperationResultType.Success);
            result = _schoolBusiness.SendRequest(UserInfo, schoolId, communityId);
            response.Message = result.Message;
            response.Success = result.ResultType == OperationResultType.Success;
            response.Data = "";
            response.Message = result.Message;
            return JsonConvert.SerializeObject(response);
        }
        //重新发送请求
        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.Administrative, Anonymity = Anonymous.Logined)]
        public string ResendSelectedSchool(string msg)
        {
            var response = new PostFormResponse();
            OperationResult result = new OperationResult(OperationResultType.Success);
            result = _schoolBusiness.ResendSelectedSchool(msg, UserInfo);
            response.Message = result.Message;
            response.Success = result.ResultType == OperationResultType.Success;
            response.Data = "";
            response.Message = result.Message;
            return JsonConvert.SerializeObject(response);
        }


        #region  Playground


        [CLIUrlAuthorizeAttribute(Account = Authority.Edit, PageId = PagesModel.School_Management, Anonymity = Anonymous.Verified)]
        public ActionResult PlaygroundPopup(int shoolId)
        {
            SchoolModel school = _schoolBusiness.GetSchoolEntity(shoolId, UserInfo);
            return View(school);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.View, PageId = PagesModel.School_Management, Anonymity = Anonymous.Verified)]
        public ActionResult PlaygroundPopupView(int shoolId)
        {
            SchoolModel school = _schoolBusiness.GetSchoolEntity(shoolId, UserInfo);
            return View(school);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.View, PageId = PagesModel.School_Management, Anonymity = Anonymous.Verified)]
        public string GetPlaygrounds(int schoolId, string sort = "Name", string order = "Asc", int first = 0, int count = 10)
        {
            List<PlaygroundEntity> list = _schoolBusiness.GetPlaygrounds(schoolId);
            var result = new { total = list.Count, data = list };
            return JsonHelper.SerializeObject(result);
        }

        [HttpPost]
        [CLIUrlAuthorizeAttribute(Account = Authority.Edit, PageId = PagesModel.School_Management, Anonymity = Anonymous.Verified)]
        public string SavePlayground(int schoolId, string name)
        {
            PlaygroundEntity playgroundEntity = new PlaygroundEntity();
            playgroundEntity.Name = name;
            playgroundEntity.SchoolId = schoolId;
            OperationResult result = _schoolBusiness.InsertPlayground(playgroundEntity);

            var response = new PostFormResponse();
            response.Success = result.ResultType == OperationResultType.Success;
            response.Data = "";
            response.Message = result.Message;

            return JsonHelper.SerializeObject(response);
        }
        [HttpPost]
        [CLIUrlAuthorizeAttribute(Account = Authority.Edit, PageId = PagesModel.School_Management, Anonymity = Anonymous.Verified)]
        public string DeletePlayground(int id)
        {
            OperationResult result = _schoolBusiness.DeletePlayground(id);
            var response = new PostFormResponse();
            response.Success = result.ResultType == OperationResultType.Success;
            response.Data = "";
            response.Message = result.Message;
            if (response.Success)
            {
                UpdateTRSClassPlayground(id, null);
            }
            return JsonHelper.SerializeObject(response);
        }


        [CLIUrlAuthorizeAttribute(Account = Authority.View, PageId = PagesModel.School_Management,
            Anonymity = Anonymous.Verified)]
        public string GetTRSClasses(int schoolId, int playgroundId, string name, string sort = "TRSClassName",
            string order = "Asc", int first = 0, int count = 10)
        {
            int totalCount = 0;
            List<TRSClassIndexModel> list =
                _trsClassBusiness.SearchTRSClasses(UserInfo,
                    o => o.SchoolId == schoolId && o.Status == EntityStatus.Active
                         && (o.PlaygroundId == 0 || o.PlaygroundId == playgroundId) && o.Name.Contains(name)
                    , sort, order, first, count, out totalCount).ToList();
            var result = new { total = list.Count, data = list };
            return JsonHelper.SerializeObject(result);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.View, PageId = PagesModel.School_Management,
            Anonymity = Anonymous.Verified)]
        public string GetAssignedTRSClasses(int schoolId, int playgroundId, string name, string sort = "TRSClassName",
            string order = "Asc", int first = 0, int count = 10)
        {
            int totalCount = 0;
            List<TRSClassIndexModel> list =
                _trsClassBusiness.SearchTRSClasses(UserInfo,
                    o =>
                        o.SchoolId == schoolId && (o.PlaygroundId == playgroundId || playgroundId == 0) &&
                        o.Name.Contains(name)
                    , sort, order, first, count, out totalCount).ToList();
            var result = new { total = list.Count, data = list };
            return JsonHelper.SerializeObject(result);
        }


        [CLIUrlAuthorizeAttribute(Account = Authority.View, PageId = PagesModel.School_Management, Anonymity = Anonymous.Verified)]
        public string UpdateTRSClassPlayground(int playgroundId = 0, int[] trsClassIds = null)
        {
            var response = new PostFormResponse();
            if (playgroundId > 0)
            {
                OperationResult result = _trsClassBusiness.UpdateTRSClassPlayground(playgroundId, trsClassIds);

                response.Success = result.ResultType == OperationResultType.Success;
                response.Data = "";
                response.Message = result.Message;
            }
            else
                response.Success = true;

            return JsonHelper.SerializeObject(response);
        }

        #endregion

        #region  Community School Relations
        [CLIUrlAuthorizeAttribute(Account = Authority.Assign, PageId = PagesModel.School_Management, Anonymity = Anonymous.Verified)]
        public string SearchUnassigedCommunities(int schoolId, string name = "", string sort = "CommunityName", string order = "Asc", int first = 0, int count = 10)
        {
            int total = 0;
            int[] assignedIds = _schoolBusiness.GetAssignedCommIds(schoolId);
            var list = _communityBusiness.SearchCommunities(UserInfo,
                o => (o.BasicCommunity.Name.Contains(name) || name.Trim() == "") && (!assignedIds.Contains(o.ID) && o.Status == EntityStatus.Active), sort, order, first, count, out total);
            var result = new { total = total, data = list };
            return JsonHelper.SerializeObject(result);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.Assign, PageId = PagesModel.School_Management, Anonymity = Anonymous.Verified)]
        public string InsertCommunitySchoolRelations(int schoolId, int[] comIds)
        {
            var response = new PostFormResponse();
            OperationResult result = new OperationResult(OperationResultType.Success);
            result = _schoolBusiness.InsertCommunitySchoolRelations(UserInfo.ID, schoolId, comIds);
            response.Success = result.ResultType == OperationResultType.Success;
            response.Data = "";
            response.Message = result.Message;
            return JsonHelper.SerializeObject(response);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.Assign, PageId = PagesModel.School_Management, Anonymity = Anonymous.Verified)]
        public string DeleteCommunitySchoolRelations(int schoolId, int[] comIds)
        {
            var response = new PostFormResponse();
            OperationResult result = new OperationResult(OperationResultType.Success);

            SchoolModel model = _schoolBusiness.GetSchoolEntity(schoolId, UserInfo);
            if (model != null)
            {
                if (comIds.Contains(model.PrimaryCommunityId))
                {
                    result.ResultType = OperationResultType.Error;
                    result.Message = "Primary community can not be unassigned.";
                }
                else
                {
                    result = _schoolBusiness.DeleteCommunitySchoolRelations2(schoolId, comIds);
                }
            }
            else
            {
                result.ResultType = OperationResultType.Error;
                result.Message = "School information error.";

            }
            //int[] assignedIds = _schoolBusiness.GetAssignedCommIds(schoolId);
            //if (comIds.Except(assignedIds).ToList().Count == 0 && assignedIds.Except(comIds).ToList().Count == 0)
            //{
            //    result.ResultType = OperationResultType.Error;
            //    result.Message = "One school must have a community";
            //}


            response.Success = result.ResultType == OperationResultType.Success;
            response.Data = "";
            response.Message = result.Message;
            return JsonHelper.SerializeObject(response);
        }
        [CLIUrlAuthorizeAttribute(Account = Authority.Assign, PageId = PagesModel.School_Management, Anonymity = Anonymous.Verified)]
        public string SearchAssignedCommunities(long schoolId = 0, string name = "", string sort = "CommunityName", string order = "Asc", int first = 0, int count = 10)
        {
            int total = 0;
            var list = _schoolBusiness.GetAssignedCommunities(UserInfo,
                o => (o.SchoolId == schoolId && (o.Community.BasicCommunity.Name.Contains(name) || name.Trim() == "")), sort, order, first, count, out total);
            var result = new { total = total, data = list };
            return JsonHelper.SerializeObject(result);
        }
        #endregion

        #endregion



        #region Tools Methods

        private IEnumerable<SelectListItem> GetIntegerList(int minValue, int MaxValue, bool isNull = true)
        {
            IList<SelectListItem> list = new List<SelectListItem>();
            for (int i = minValue; i <= MaxValue; i++)
            {
                list.Add(new SelectListItem() { Text = i.ToString(), Value = i.ToString() });
            }
            if (isNull)
                list.AddDefaultItem(ViewTextHelper.DefaultPleaseSelectText, "0");
            else
            {
                list.AddDefaultItem(ViewTextHelper.DefaultPleaseSelectText, "");
            }
            return list;
        }

        //profile功能需要用此方法，所以将权限修改为登陆即可
        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.Administrative, Anonymity = Anonymous.Logined)]
        public IEnumerable<SelectListItem> GetSchoolTypesList(string defaultText = "", string defaultVal = "", int parentId = 0)
        {
            if (defaultText == "") defaultText = ViewTextHelper.DefaultPleaseSelectText;
            var list = _schoolBusiness.GetSchoolTypeList(parentId).Select(o => new SelectListItem
            {
                Text = o.Name,
                Value = o.ID.ToString()
            }).AddDefaultItem(defaultText, defaultVal);
            return list;
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.Administrative, Anonymity = Anonymous.Logined)]
        public string GetSchoolTypesJson(string defaultText = "", string defaultVal = "", int parentId = 0)
        {
            return JsonHelper.SerializeObject(GetSchoolTypesList(defaultText, defaultVal, parentId).ToList());
        }

        private IEnumerable<SelectListItem> GetCommunityList(string defaultText, string defaultVal = "")
        {
            if (defaultText == "") defaultText = ViewTextHelper.DefaultPleaseSelectText;
            var list = _communityBusiness.GetCommunitySelectList(UserInfo).Select(o => new SelectListItem
            {
                Text = o.Name,
                Value = o.ID.ToString()
            }).AddDefaultItem(defaultText, defaultVal);
            return list;
        }

        private IEnumerable<SelectListItem> ListToDDL(IEnumerable<SelectItemModel> list, string defaultValue = "")
        {
            return list.ToSelectList(ViewTextHelper.DefaultPleaseSelectText, defaultValue);
        }

        #endregion

        #region Init Methods
        private void InitPartial(int schoolId = 0)
        {
            string partialUrl = "";
            string viewPartialUrl = "";
            ViewBag.AccessTRSClassManagement = false;
            UserAuthorityModel userAuthority = new PermissionBusiness().GetUserAuthority(UserInfo, (int)PagesModel.TRSClass_Management);
            switch (UserInfo.Role)
            {
                case Role.Super_admin:
                case Role.Statisticians:
                case Role.Intervention_manager:
                case Role.Intervention_support_personnel:
                case Role.Administrative_personnel:
                    partialUrl = "~/areas/school/views/school/partials/FullAccess.cshtml";
                    viewPartialUrl = "~/areas/school/views/school/partials/views/FullAccess.cshtml";
                    break;
                case Role.Community:
                case Role.District_Community_Delegate:
                    if (userAuthority != null)
                        ViewBag.AccessTRSClassManagement = true;
                    partialUrl = "~/areas/school/views/school/partials/CommunityUser.cshtml";
                    if (schoolId > 0)
                    {
                        var primarySchoolIds =
                            _schoolBusiness.GetPrimarySchoolsByComId(
                                UserInfo.UserCommunitySchools.Select(e => e.CommunityId).ToList())
                                .Select(e => e.ID);
                        if (!primarySchoolIds.Contains(schoolId))
                        {
                            partialUrl = "~/areas/school/views/school/partials/SecondaryCommunityEditTrs.cshtml";
                        }
                    }
                    viewPartialUrl = "~/areas/school/views/school/partials/views/CommunityUser.cshtml";
                    break;
                case Role.Coordinator:
                    partialUrl = "~/areas/school/views/school/partials/Coordinator.cshtml";
                    viewPartialUrl = "~/areas/school/views/school/partials/views/Coordinator.cshtml";
                    break;
                case Role.Mentor_coach:
                    partialUrl = "~/areas/school/views/school/partials/Mentor.cshtml";
                    viewPartialUrl = "~/areas/school/views/school/partials/views/Mentor.cshtml";
                    break;
                case Role.Principal:
                case Role.Principal_Delegate:
                    partialUrl = "~/areas/school/views/school/partials/Principal.cshtml";
                    viewPartialUrl = "~/areas/school/views/school/partials/views/Principal.cshtml";
                    break;
                case Role.TRS_Specialist:
                case Role.TRS_Specialist_Delegate:
                    partialUrl = "~/areas/school/views/school/partials/Specialist.cshtml";
                    viewPartialUrl = "~/areas/school/views/school/partials/views/Specialist.cshtml";
                    break;
                case Role.School_Specialist:
                case Role.School_Specialist_Delegate:
                    partialUrl = "~/areas/school/views/school/partials/Specialist.cshtml";
                    viewPartialUrl = "~/areas/school/views/school/partials/views/Specialist.cshtml";
                    break;
                case Role.District_Community_Specialist:
                case Role.Community_Specialist_Delegate:
                    if (userAuthority != null)
                        ViewBag.AccessTRSClassManagement = true;
                    if (
                        UserInfo.UserCommunitySchools.Any(
                            e =>
                                e.Community.CommunityAssessmentRelations.Any(
                                    s => (LocalAssessment)s.AssessmentId == LocalAssessment.TexasRisingStar)))
                    {
                        partialUrl = "~/areas/school/views/school/partials/SecondaryCommunityEditTrs.cshtml";
                    }
                    else
                    {
                        partialUrl = "~/areas/school/views/school/partials/CommunitySpecialist.cshtml";
                    }
                    viewPartialUrl = "~/areas/school/views/school/partials/views/CommunitySpecialist.cshtml";
                    break;
                case Role.Teacher:
                    partialUrl = "~/areas/school/views/school/partials/Teacher.cshtml";
                    viewPartialUrl = "~/areas/school/views/school/partials/views/Teacher.cshtml";
                    break;
                case Role.Content_personnel:
                case Role.Video_coding_analyst:
                    partialUrl = "~/areas/school/views/school/partials/ReadOnly.cshtml";
                    viewPartialUrl = "~/areas/school/views/school/partials/views/ReadOnly.cshtml";
                    break;
                case Role.Statewide:
                case Role.Auditor:
                    partialUrl = "~/areas/school/views/school/partials/ReadOnlyNotNotes.cshtml";
                    viewPartialUrl = "~/areas/school/views/school/partials/views/ReadOnlyNotNotes.cshtml";
                    break;

            }
            ViewBag.partialUrl = partialUrl;
            ViewBag.viewPartialUrl = viewPartialUrl;
        }

        public void InitControls(int schoolId=0)
        {
            //List<SelectListItem> Counties = new List<SelectListItem>();
            //Counties.Add(new SelectListItem() { Text = "County*", Value = "" });

            var SchoolNames = ListToDDL(_schoolBusiness.GetBasicSchoolsList());
            var Fundings = ListToDDL(_masterBusiness.GetFundingSelectList(), "0");
            var Counties = _masterBusiness.GetCountySelectList().ToSelectList("County*", "");
            var States = _masterBusiness.GetStateSelectList().ToSelectList("State*", "");

            var Titles = ListToDDL(_masterBusiness.GetTitleSelectList(3));
            var Titles2 = ListToDDL(_masterBusiness.GetTitleSelectList(4), "0");
            var ParentAgencies = ListToDDL(_schoolBusiness.GetParentAgencyList());



            ViewBag.SchoolTypeOptions = GetSchoolTypesList("", "", 0);
            ViewBag.ChildCareSchoolTypeOptions = GetSchoolTypesList("", "0", 3);
            ViewBag.FCCSchoolTypeOptions = GetSchoolTypesList("", "0", 4);
            ViewBag.ParentAgencyOptions = ParentAgencies;
            ViewBag.SchoolNameOptions = SchoolNames;
            ViewBag.StatusOptions = EntityStatus.Active.ToSelectList(true);
            ViewBag.EscNameOptions = GetIntegerList(1, 20);
            ViewBag.PhoneTypeOptions = PhoneType.HomeNumber.ToSelectList(true, new SelectOptions(true, "", ViewTextHelper.DefaultPleaseSelectText));
            ViewBag.PhoneTypeOptions2 = PhoneType.HomeNumber.ToSelectList(true, new SelectOptions(true, "0", ViewTextHelper.DefaultPleaseSelectText));

            // ViewBag.AtRiskPercentOptions = GetIntegerList(1, 100, false);
            ViewBag.PrimarySalutationOptions = UserSalutation.Dr.ToSelectList(true, new SelectOptions(true, "0", ViewTextHelper.DefaultPleaseSelectText));
            ViewBag.InternetSpeedOptions = InternetSpeed.LessThan15.ToSelectList(true, new SelectOptions(true, "0", ViewTextHelper.DefaultPleaseSelectText));
            ViewBag.InternetTypeOptions = InternetType.None.ToSelectList(true, new SelectOptions(true, "0", ViewTextHelper.DefaultPleaseSelectText));
            ViewBag.WirelessTypeOptions = WireLessType.WirelessA.ToSelectList(true, new SelectOptions(true, "0", ViewTextHelper.DefaultPleaseSelectText));
            ViewBag.CommunityOptions = GetCommunityList(ViewTextHelper.DefaultPleaseSelectText, "-1");
            ViewBag.TitleOptions = Titles;
            ViewBag.TitleOptions2 = Titles2;
            ViewBag.CountyOptions = Counties;
            ViewBag.StateOptions = States;
            ViewBag.FundingOptions = Fundings;
            ViewBag.TrsProviderOptions = ListToDDL(_schoolBusiness.GetTrsProviderList());
            ViewBag.IspOptions = ListToDDL(_schoolBusiness.GetIspList(), "0");
            ViewBag.VSDesignationOptions = TRSStarEnum.One.ToSelectList(true, new SelectOptions(true, "0", ViewTextHelper.DefaultPleaseSelectText));
            ViewBag.FourStart = (int)TRSStarEnum.Four;
            #region TRS
            ViewBag.FacilityTypeOptions = FacilityType.LCAA.ToSelectList(true, new SelectOptions(true, "", ViewTextHelper.DefaultPleaseSelectText));

            ViewBag.RegulatingEntityOptions = Regulating.StateOfTX.ToSelectList(true, new SelectOptions(true, "", ViewTextHelper.DefaultPleaseSelectText));
            #endregion
            InitPartial(schoolId);
        }

        #endregion

        /// <summary>
        /// 控制按钮操作是否显示
        /// </summary>
        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.Administrative, Anonymity = Anonymous.Logined)]
        public string GetCoordCoachsByCommunityId(int communityId)
        {
            var classList = _userBusiness.GetSchoolSpecialist(new[] { communityId })//GetCoordCoachs(communityId)
                .ToSelectList(ViewTextHelper.DefaultPleaseSelectText, "0");
            return JsonHelper.SerializeObject(classList);
        }

        #region Profile
        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.Administrative, Anonymity = Anonymous.Logined)]
        public ActionResult SchoolProfile(int id = 0)
        {
            ViewBag.EnableAutoAssign4StarTips = ResourceHelper.GetRM().GetInformation("EnableAutoAssign4StarTips");
            SchoolModel school = null;
            var list = new List<SchoolModel>();
            switch (UserInfo.Role)
            {
                case Role.Principal:
                case Role.TRS_Specialist:
                case Role.School_Specialist:
                    if (UserInfo.Principal != null)
                    {
                        list = UserInfo.UserCommunitySchools.Select(o => new SchoolModel
                        {
                            ID = o.SchoolId,
                            SchoolName = o.School.Name
                        }).ToList();

                        ViewBag.SchoolList = list;

                        if (id == 0)
                        {
                            school = _schoolBusiness.GetSchoolEntity(list.Select(o => o.ID).FirstOrDefault(), UserInfo);
                        }
                        else
                        {
                            school = _schoolBusiness.GetSchoolEntity(id, UserInfo);
                        }
                    }
                    break;
                case Role.Principal_Delegate:
                case Role.TRS_Specialist_Delegate:
                case Role.School_Specialist_Delegate:
                    if (UserInfo.Principal != null)
                    {
                        var user = _userBusiness.GetUser(UserInfo.Principal.ParentId);
                        list = user.UserCommunitySchools.Select(o => new SchoolModel
                        {
                            ID = o.SchoolId,
                            SchoolName = o.School.Name
                        }).ToList();

                        ViewBag.SchoolList = list;

                        if (id == 0)
                        {
                            school = _schoolBusiness.GetSchoolEntity(list.Select(o => o.ID).FirstOrDefault(), UserInfo);
                        }
                        else
                        {
                            school = _schoolBusiness.GetSchoolEntity(id, UserInfo);
                        }
                    }
                    break;
                case Role.Teacher:
                    if (UserInfo.TeacherInfo != null)
                    {
                        list = UserInfo.UserCommunitySchools.Where(e => e.SchoolId > 0).Select(o => new SchoolModel
                        {
                            ID = o.SchoolId,
                            SchoolName = o.School.Name
                        }).ToList();

                        ViewBag.SchoolList = list;

                        if (id == 0)
                        {
                            if (list.Any())
                                school = _schoolBusiness.GetSchoolEntity(list.Select(o => o.ID).FirstOrDefault(),
                                    UserInfo);
                        }
                        else
                        {
                            school = _schoolBusiness.GetSchoolEntity(id, UserInfo);
                        }
                    }
                    break;
                default:
                    break;
            }
            if (school == null)
                school = new SchoolModel();
            #region TRS
            //TODO:School CommunityId DEL
            //ViewBag.CoachOptions = _userBusiness.GetSchoolSpecialist(school.CommunityId)
            //.ToSelectList(ViewTextHelper.DefaultPleaseSelectText, "");

            List<ClassCountAgeModel> trsClassCount = _trsClassBusiness.GetTRSClassCountAge(school.ID);

            ViewBag.Infants = GetClassCount(trsClassCount, TrsAgeArea.Infants);
            ViewBag.Toddlers = GetClassCount(trsClassCount, TrsAgeArea.Toddlers);
            ViewBag.Preschool = GetClassCount(trsClassCount, TrsAgeArea.Preschool);
            ViewBag.SchoolAge = GetClassCount(trsClassCount, TrsAgeArea.SchoolAge);
            ViewBag.CoachOptions =
               _userBusiness.GetSchoolSpecialist(new[] { school.ID })
              .ToSelectList(ViewTextHelper.DefaultPleaseSelectText, "0");

            if (school.TrsAssessorId > 0)
            {
                var TrsAssessor = _userBusiness.GetUserBaseModel(school.TrsAssessorId);
                if (TrsAssessor != null)
                    ViewBag.TrsAssessor = TrsAssessor.FullName;
            }
            ViewBag.TrsTaStatusOption = GetTrsTaOptions(school.TrsTaStatus);
            #endregion
            ViewBag.IsShowTRS = _communityBusiness.CheckIfTrsBySchool(school.ID);

            InitControls();
            ViewBag.UserRole = UserInfo.Role;
            ViewBag.ParentAgencyName =
               _schoolBusiness.GetParentAgencyList().Where(o => o.ID == school.ParentAgencyId).
               Select(o => o.Name).FirstOrDefault();

            string typeName = _schoolBusiness.GetSchoolTypeName(school.SchoolTypeId);
            ViewBag.SchoolTypeName = typeName;

            if (school.SubTypeId != 0)
                ViewBag.SubSchoolTypeName = _schoolBusiness.GetSchoolTypeName(school.SubTypeId);

            if (school.FundingId != 0)
                if (_masterBusiness.GetFunding(school.FundingId) != null)
                    ViewBag.fundingName = _masterBusiness.GetFunding(school.FundingId).Name;



            if (school.IspId != 0)
            {
                string ispName = _schoolBusiness.GetIspName(school.IspId);
                if (ispName != "Other")
                    ViewBag.IspName = ispName;
                else
                    ViewBag.IspName = school.IspOther;
            }

            if (school.PrimaryTitleId != 0)
                ViewBag.primaryTitle = _masterBusiness.GetTitle(school.PrimaryTitleId).Name;


            if (school.SecondaryTitleId != 0)
                ViewBag.secondaryTitle = _masterBusiness.GetTitle(school.SecondaryTitleId).Name;

            string stateName = "";
            if (school.StateId != 0)
                stateName = _masterBusiness.GetState(school.StateId).Name;
            string countyName = "";
            if (school.CountyId != 0)
                countyName = _masterBusiness.GetCounty(school.CountyId).Name;
            string FullAddr = school.City + ", " + (countyName == "" ? "" : countyName + ", ") + stateName + " " + school.Zip;
            ViewBag.FullAddr = FullAddr;

            string mailStateName = "";
            string mailCountyName = "";
            if (school.MailingStateId != 0)
                mailStateName = _masterBusiness.GetState(school.MailingStateId).Name;
            if (school.MailingCountyId != 0)
                mailCountyName = _masterBusiness.GetCounty(school.MailingCountyId).Name;

            string FullMailAddr = school.MailingCity + ", " + (mailCountyName == "" ? "" : mailCountyName + ", ") + mailStateName + " " + school.MailingZip;
            ViewBag.FullMailAddr = FullMailAddr;

            bool changingStarRating = false;
            if (UserInfo.Role == Role.Super_admin || UserInfo.Role == Role.TRS_Specialist || UserInfo.Role == Role.TRS_Specialist_Delegate)
                changingStarRating = true;
            ViewBag.ChangingStarRating = changingStarRating;

            return View(school);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.Administrative, Anonymity = Anonymous.Logined)]
        public string ProfileUpdate(SchoolModel school)
        {
            var response = new PostFormResponse();
            OperationResult result = _schoolBusiness.UpdateSchool(school, UserInfo.Role);
            response.Success = result.ResultType == OperationResultType.Success;
            response.Data = school;
            response.Message = result.Message;
            return JsonConvert.SerializeObject(response);
        }
        #endregion

        private void InitAccessOperation()
        {
            bool accessAdd = false;
            bool accessEdit = false;
            bool accessView = false;
            bool accessAssign = false;
            if (UserInfo != null)
            {
                UserAuthorityModel userAuthority = new PermissionBusiness().GetUserAuthority(UserInfo, (int)PagesModel.School_Management);

                if (userAuthority != null)
                {
                    if ((userAuthority.Authority & (int)Authority.Add) == (int)Authority.Add)
                    {
                        accessAdd = true;
                    }
                    if ((userAuthority.Authority & (int)Authority.Edit) == (int)Authority.Edit)
                    {
                        accessEdit = true;
                    }
                    if ((userAuthority.Authority & (int)Authority.View) == (int)Authority.View)
                    {
                        accessView = true;
                    }
                    if ((userAuthority.Authority & (int)Authority.Assign) == (int)Authority.Assign)
                    {
                        accessAssign = true;
                    }
                }
            }
            ViewBag.accessAdd = accessAdd;
            ViewBag.accessEdit = accessEdit;
            ViewBag.accessView = accessView;
            ViewBag.accessAssign = accessAssign;
            ViewBag.AssignUserAccess = ((accessAssign && (UserInfo.Role == Role.Principal)) || UserInfo.Role == Role.Super_admin);
        }

        private string GetTrsTaOptions(string Ids)
        {
            List<string> nameList = new List<string>();
            string[] idArray = Ids.Split(',');
            foreach (string idStr in idArray)
            {
                if (idStr != "")
                {
                    int id = 0;
                    int.TryParse(idStr, out id);
                    if (id > 0)
                    {
                        nameList.Add(((TrsTaStatus)id).ToDescription());
                    }
                }

            }
            return string.Join(",", nameList);
        }

        #region assign Users to this School

        [CLIUrlAuthorizeAttribute(Account = Authority.Edit, PageId = PagesModel.School_Management, Anonymity = Anonymous.Verified)]
        public ActionResult AssignUsersToSchool(int schoolId)
        {
            SchoolModel school = _schoolBusiness.GetSchoolEntity(schoolId, UserInfo);
            var list = new List<SelectListItem>();
            list.Add(new SelectListItem() { Text = "Please select...", Value = "0" });
            list.Add(new SelectListItem() { Text = Role.Community.ToDescription(), Value = Role.Community.GetValue().ToString() });
            list.Add(new SelectListItem() { Text = Role.District_Community_Specialist.ToDescription(), Value = Role.District_Community_Specialist.GetValue().ToString() });
            list.Add(new SelectListItem() { Text = Role.Statewide.ToDescription(), Value = Role.Statewide.GetValue().ToString() });
            ViewBag.RoleOptions = list;

            if (school.SchoolAccess == AccessType.ReadOnly)
                Response.Redirect("Index");

            return View(school);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.Edit, PageId = PagesModel.School_Management, Anonymity = Anonymous.Verified)]
        public string SearchUnAssignedUsers(int communityId = 0, string communityName = "", int schoolId = 0, int classId = 0, Role role = (Role)0, string name = "",
            string sort = "FirstName", string order = "ASC", int first = 0, int count = 10)
        {
            int total = 0;
            Expression<Func<UserBaseEntity, bool>> condition = PredicateHelper.True<UserBaseEntity>();

            if (name.Trim() != string.Empty)
                condition = condition.And(r => r.FirstName.Contains(name.Trim()) || r.LastName.Contains(name.Trim()));
            if (communityId > 0)
            {
                condition = condition.And(r => r.UserCommunitySchools.Any(c => c.Community.CommunitySchoolRelations.Any(s => s.CommunityId == communityId)));
            }
            if (communityName.Trim() != "")
            {
                condition = condition.And(r => r.UserCommunitySchools.Any(c => c.Community.CommunitySchoolRelations.Any(s => s.Community.Name.IndexOf(communityName) >= 0)));
            }
            condition = condition.And(r => r.UserCommunitySchools.Any(c => c.Community.CommunitySchoolRelations.Any(s => s.SchoolId == schoolId)));
            condition = condition.And(r => !(r.UserCommunitySchools.Any(c => c.SchoolId == schoolId)));
            if (role != (Role)0)
            {
                condition = condition.And(r => r.Role == role);
            }
            else
            {
                condition = condition.And(r => (r.Role == Role.Community || r.Role == Role.District_Community_Specialist || r.Role == Role.Statewide));
            }
            var list = _userBusiness.GetUsers(UserInfo, condition, sort, order, first, count, out total);

            var result = new { total = total, data = list };
            return JsonHelper.SerializeObject(result);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.Edit, PageId = PagesModel.School_Management, Anonymity = Anonymous.Verified)]
        public string AssignUsers(int schoolId, int[] userIds)
        {
            var response = new PostFormResponse();
            OperationResult result = new OperationResult(OperationResultType.Success);
            result = _userBusiness.InsertUserSchoolRelations(userIds, UserInfo.ID, schoolId);
            response.Success = result.ResultType == OperationResultType.Success;
            response.Data = "";
            response.Message = result.Message;
            return JsonHelper.SerializeObject(response);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.Edit, PageId = PagesModel.School_Management, Anonymity = Anonymous.Verified)]
        public string UnsignUsers(int schoolId, int[] userIds)
        {
            var response = new PostFormResponse();
            OperationResult result = new OperationResult(OperationResultType.Success);
            result = _userBusiness.RemoveUserSchoolRelations(userIds, schoolId);
            response.Success = result.ResultType == OperationResultType.Success;
            response.Data = "";
            response.Message = result.Message;
            return JsonHelper.SerializeObject(response);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.Edit, PageId = PagesModel.School_Management, Anonymity = Anonymous.Verified)]
        public string SearchAssignedUsers(int communityId = 0, string communityName = "", int schoolId = 0, int classId = 0, Role role = (Role)0, string name = "",
            string sort = "FirstName", string order = "ASC", int first = 0, int count = 10)
        {
            int total = 0;
            Expression<Func<UserBaseEntity, bool>> condition = PredicateHelper.True<UserBaseEntity>();
            if (communityId > 0)
            {
                condition = condition.And(r => r.UserCommunitySchools.Any(c => c.Community.CommunitySchoolRelations.Any(s => s.CommunityId == communityId)));
            }
            if (communityName.Trim() != "")
            {
                condition = condition.And(r => r.UserCommunitySchools.Any(c => c.Community.CommunitySchoolRelations.Any(s => s.Community.Name.IndexOf(communityName) >= 0)));
            }
            if (name.Trim() != string.Empty)
                condition = condition.And(r => r.FirstName.Contains(name.Trim()) || r.LastName.Contains(name.Trim()));

            condition = condition.And(r => (r.UserCommunitySchools.Any(c => c.SchoolId == schoolId)));
            if (role != (Role)0)
            {
                condition = condition.And(r => r.Role == role);
            }
            var list = _userBusiness.GetUsers(UserInfo, condition, sort, order, first, count, out total);

            var result = new { total = total, data = list };
            return JsonHelper.SerializeObject(result);
        }

        #endregion


    }
}