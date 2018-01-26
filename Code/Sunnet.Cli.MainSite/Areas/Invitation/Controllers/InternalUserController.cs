using Sunnet.Cli.Business.Communities;
using Sunnet.Cli.Business.Log;
using Sunnet.Cli.Business.Users;
using Sunnet.Cli.Core.Common.Enums;
using Sunnet.Cli.Core.Log.Entities;
using Sunnet.Cli.Core.Users.Enums;
using Sunnet.Cli.MainSite.Controllers;
using Sunnet.Cli.UIBase.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Sunnet.Framework.Extensions;
using Sunnet.Cli.Core.Users.Entities;
using Sunnet.Framework.Core.Tool;
using Sunnet.Cli.Business.Common;
using Sunnet.Framework.Helpers;
using Sunnet.Cli.Business.Permission;
using Sunnet.Framework.Permission;
using Sunnet.Cli.UIBase.Filters;
using Sunnet.Cli.Core.MasterData.Entities;
using Sunnet.Cli.Business.MasterData;
using Sunnet.Cli.Business.Permission.Models;

namespace Sunnet.Cli.MainSite.Areas.Invitation.Controllers
{
    public class InternalUserController : BaseController
    {
        private readonly UserBusiness userBusiness;
        private readonly CommunityBusiness communityBusiness;
        private readonly MasterDataBusiness masterDataBusiness;
        private readonly OperationLogBusiness operationLogBusiness;
        public InternalUserController()
        {
            userBusiness = new UserBusiness(UnitWorkContext);
            communityBusiness = new CommunityBusiness(UnitWorkContext);
            masterDataBusiness = new MasterDataBusiness(UnitWorkContext);
            operationLogBusiness = new OperationLogBusiness(UnitWorkContext);
        }
        //
        // GET: /Invitation/InternalUser/
        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.CLI_User_Management, Anonymity = Anonymous.Verified)]
        public ActionResult Index()
        {
            InitAccessOperation();
            return View();
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.Add, PageId = PagesModel.CLI_User_Management, Anonymity = Anonymous.Verified)]
        public ActionResult New()
        {
            InternalUserRole();
            UserBaseEntity userEntity = new UserBaseEntity();
            CoordCoachEntity coordCoach = new CoordCoachEntity();
            coordCoach.User = userEntity;
            ViewBag.ProjectManagerOptions = userBusiness.GetProjectManagers().ToSelectList(ViewTextHelper.DefaultPleaseSelectText, "0");
            ViewBag.Language = userBusiness.GetLanguages().ToSelectList(ViewTextHelper.DefaultPleaseSelectText, "0");
            ViewBag.Funding = userBusiness.GetFundings().ToSelectList(ViewTextHelper.DefaultPleaseSelectText, "0");
            ViewBag.State = userBusiness.GetStates().ToSelectList(ViewTextHelper.DefaultStateText, "");
            ViewBag.County = userBusiness.GetCountries().ToSelectList(ViewTextHelper.DefaultCountyText, "");
            ViewBag.CertificateList = userBusiness.GetCertificates();
            ViewBag.StatusAccess = StatusAccess();
            ViewBag.OtherAccess = OtherAccess();
            ViewBag.NoteAccess = NoteAccess();
            return View(coordCoach);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.Edit, PageId = PagesModel.CLI_User_Management, Anonymity = Anonymous.Verified)]
        public ActionResult Edit(int userId)
        {
            InternalUserRole();
            UserBaseEntity user = userBusiness.GetUser(userId);
            CoordCoachEntity coordCoachEntity = new CoordCoachEntity();

            if (user.Role == Role.Coordinator || user.Role == Role.Mentor_coach)
            {
                coordCoachEntity = user.CoordCoach;
                ViewBag.PMName = userBusiness.GetPMByCoordCoach(user);
            }
            else if (user.Role == Role.Video_coding_analyst)
            {
                coordCoachEntity.User = user;
                coordCoachEntity.ID = user.VideoCoding.ID;
                coordCoachEntity.PrimaryLanguageId = user.VideoCoding.PrimaryLanguageId;
                coordCoachEntity.PrimaryLanguageOther = user.VideoCoding.PrimaryLanguageOther;
                coordCoachEntity.SecondaryLanguageId = user.VideoCoding.SecondaryLanguageId;
                coordCoachEntity.SecondaryLanguageOther = user.VideoCoding.SecondaryLanguageOther;
            }
            else
            {
                coordCoachEntity.User = user;
            }

            string certificateText = "";
            foreach (var item in coordCoachEntity.User.Certificates)
            {
                certificateText += item.ID + ",";
            }
            ViewBag.CertificateText = certificateText;
            ViewBag.Language = userBusiness.GetLanguages().ToSelectList(ViewTextHelper.DefaultPleaseSelectText, "0");
            ViewBag.Funding = userBusiness.GetFundings().ToSelectList(ViewTextHelper.DefaultPleaseSelectText, "0");
            ViewBag.State = userBusiness.GetStates().ToSelectList(ViewTextHelper.DefaultStateText, "");
            ViewBag.County = userBusiness.GetCountries().ToSelectList(ViewTextHelper.DefaultCountyText, "");
            ViewBag.CertificateList = userBusiness.GetCertificates();
            ViewBag.StatusAccess = StatusAccess();
            ViewBag.OtherAccess = OtherAccess();
            ViewBag.NoteAccess = NoteAccess();


            return View(coordCoachEntity);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.View, PageId = PagesModel.CLI_User_Management, Anonymity = Anonymous.Verified)]
        public ActionResult View(int userId)
        {
            UserBaseEntity user = userBusiness.GetUser(userId);
            CoordCoachEntity coordCoachEntity = new CoordCoachEntity();

            if (user.Role == Role.Coordinator || user.Role == Role.Mentor_coach)
            {
                coordCoachEntity = user.CoordCoach;
            }
            else if (user.Role == Role.Video_coding_analyst)
            {
                coordCoachEntity.User = user;
                coordCoachEntity.ID = user.VideoCoding.ID;
                coordCoachEntity.PrimaryLanguageId = user.VideoCoding.PrimaryLanguageId;
                coordCoachEntity.PrimaryLanguageOther = user.VideoCoding.PrimaryLanguageOther;
                coordCoachEntity.SecondaryLanguageId = user.VideoCoding.SecondaryLanguageId;
                coordCoachEntity.SecondaryLanguageOther = user.VideoCoding.SecondaryLanguageOther;
            }
            else
            {
                coordCoachEntity.User = user;
            }
            if (coordCoachEntity.CLIFundingId > 0)
            {
                FundingEntity funding = masterDataBusiness.GetFunding(coordCoachEntity.CLIFundingId);
                if (funding != null)
                    ViewBag.Funding = funding.Name;
            }
            if (coordCoachEntity.HomeMailingStateId > 0)
            {
                StateEntity state = masterDataBusiness.GetState(coordCoachEntity.HomeMailingStateId);
                if (state != null)
                    ViewBag.HomeMailingState = state.Name;
            }
            if (coordCoachEntity.HomeMailingCountyId > 0)
            {
                CountyEntity county = masterDataBusiness.GetCounty(coordCoachEntity.HomeMailingCountyId);
                if (county != null)
                    ViewBag.HomeMailingCounty = county.Name;
            }
            if (coordCoachEntity.OfficeStateId > 0)
            {
                StateEntity state = masterDataBusiness.GetState(coordCoachEntity.OfficeStateId);
                if (state != null)
                    ViewBag.OfficeState = state.Name;
            }
            if (coordCoachEntity.OfficeCountyId > 0)
            {
                CountyEntity county = masterDataBusiness.GetCounty(coordCoachEntity.OfficeCountyId);
                if (county != null)
                    ViewBag.OfficeCounty = county.Name;
            }
            if (coordCoachEntity.PrimaryLanguageId > 0)
            {
                LanguageEntity language = masterDataBusiness.GetLanguage(coordCoachEntity.PrimaryLanguageId);
                if (language != null)
                    ViewBag.Language = language.Language;
            }
            if (coordCoachEntity.SecondaryLanguageId > 0)
            {
                LanguageEntity language = masterDataBusiness.GetLanguage(coordCoachEntity.SecondaryLanguageId);
                if (language != null)
                    ViewBag.Language2 = language.Language;
            }
            ViewBag.CertificateList = userBusiness.GetCertificates();
            ViewBag.PMName = userBusiness.GetPMByCoordCoach(user);
            return View(coordCoachEntity);
        }

        private void InternalUserRole()
        {
            int videocodinganalyst_id = (int)InternalRole.Video_coding_analyst;
            int interpersonnel_id = (int)InternalRole.Intervention_support_personnel;
            int coordinator_id = (int)InternalRole.Coordinator;
            int mentorcoach_id = (int)InternalRole.Mentor_coach;

            List<SelectListItem> AllItems = InternalRole.Super_admin.ToSelectList()
                .AddDefaultItem(ViewTextHelper.DefaultPleaseSelectText, "").ToList();
            switch (UserInfo.Role)
            {
                case Role.Super_admin:  //可以添加所有用户
                    ViewBag.InternalUserOptions = AllItems;
                    break;
                case Role.Content_personnel: //不能添加任何用户
                case Role.Video_coding_analyst:
                case Role.Intervention_support_personnel:
                case Role.Mentor_coach:
                    ViewBag.InternalUserOptions = new List<SelectListItem> {  new SelectListItem 
                    { 
                        Value = "", Text = ViewTextHelper.DefaultPleaseSelectText} 
                    };
                    break;
                case Role.Statisticians: //可以创建 Video Coding Analyst CLI
                    ViewBag.InternalUserOptions = new List<SelectListItem> { new SelectListItem 
                    { 
                        Value = videocodinganalyst_id.ToString(), Text = InternalRole.Video_coding_analyst.ToDescription() } 
                    }.AddDefaultItem(ViewTextHelper.DefaultPleaseSelectText, "").ToList();
                    break;
                case Role.Administrative_personnel://可以创建 Intervention Support Personel CLI
                    ViewBag.InternalUserOptions = new List<SelectListItem> { new SelectListItem 
                    { 
                        Value = interpersonnel_id.ToString(), Text = InternalRole.Intervention_support_personnel.ToDescription() } 
                    }.AddDefaultItem(ViewTextHelper.DefaultPleaseSelectText, "").ToList();
                    break;
                case Role.Intervention_manager://可以创建 Intervention Support Personel CLI、Coordinator CLI、Mentor/Coach CLI
                    ViewBag.InternalUserOptions = new List<SelectListItem> { 
                        new SelectListItem{ 
                        Value = interpersonnel_id.ToString(), Text = InternalRole.Intervention_support_personnel.ToDescription() },
                        new SelectListItem{
                        Value=coordinator_id.ToString(),Text=InternalRole.Coordinator.ToDescription()
                        },
                        new SelectListItem{
                        Value=mentorcoach_id.ToString(),Text=InternalRole.Mentor_coach.ToDescription()
                        }
                    }.AddDefaultItem(ViewTextHelper.DefaultPleaseSelectText, "").ToList();
                    break;

                case Role.Coordinator: //可以创建 Mentor/Coach CLI
                    ViewBag.InternalUserOptions = new List<SelectListItem> {                        
                        new SelectListItem{
                        Value=mentorcoach_id.ToString(),Text=InternalRole.Mentor_coach.ToDescription()
                        }
                    }.AddDefaultItem(ViewTextHelper.DefaultPleaseSelectText, "").ToList();
                    break;

                default: //不能添加任何用户
                    ViewBag.InternalUserOptions = new List<SelectListItem> {  new SelectListItem 
                    { 
                        Value = "", Text = ViewTextHelper.DefaultPleaseSelectText} 
                    }.AddDefaultItem(ViewTextHelper.DefaultPleaseSelectText, "").ToList();
                    break;
            }
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.AssessmentEquipment, PageId = PagesModel.CLI_User_Management, Anonymity = Anonymous.Verified)]
        public ActionResult CoordCoachEquipment(int userId)
        {
            UserBaseEntity user = userBusiness.GetUser(userId);
            ViewBag.CoordCoachId = user.CoordCoach.ID;
            ViewBag.IsAssessmentEquipment = user.CoordCoach.IsAssessmentEquipment;
            ViewBag.CoordCoachEquipments = user.CoordCoach.CoordCoachEquipments.ToList();
            ViewBag.Equipment = EquipmentEnum.Camera.ToSelectList();
            ViewBag.EquipmentAccess = EquipmentAccess();
            return View();
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.CLI_User_Management, Anonymity = Anonymous.Verified)]
        public string Search(string keyword, int roleId = -1, int status = -1, string sort = "LastName",
            string order = "Asc", int first = 0, int count = 10)
        {
            var total = 0;
            var expression = PredicateHelper.True<UserBaseEntity>();
            if (status > 0)
                expression = expression.And(r => (int)r.Status == status);
            if (keyword.Trim() != string.Empty)
                expression = expression.And(r => r.FirstName.Contains(keyword) || r.LastName.Contains(keyword)
                    || r.GoogleId.Contains(keyword) || r.PrimaryEmailAddress.Contains(keyword));
            if (roleId > 0)
                expression = expression.And(r => (int)r.Role == roleId);
            //is internal user
            expression = expression.And(r => (int)r.Role <= (int)InternalRole.Mentor_coach);
            expression = expression.And(r => r.IsDeleted == false);

            var list = userBusiness.SearchInternalUsers(UserInfo, expression, sort, order, first, count, out total);
            var result = new { total = total, data = list };
            return JsonHelper.SerializeObject(result);
        }

        [System.Web.Mvc.HttpPost]
        [CLIUrlAuthorizeAttribute(Account = Authority.Delete, PageId = PagesModel.CLI_User_Management, Anonymity = Anonymous.Verified)]
        public string Delete(int id)
        {
            var response = new PostFormResponse();
            var result = userBusiness.DeleteUser(id);
            response.Success = result.ResultType == OperationResultType.Success;
            response.Message = result.Message;
            return JsonHelper.SerializeObject(response);
        }

        [System.Web.Mvc.HttpPost]
        [ValidateAntiForgeryToken]
        [CLIUrlAuthorizeAttribute(Account = Authority.Edit, PageId = PagesModel.CLI_User_Management, Anonymity = Anonymous.Verified)]
        public string Save(CoordCoachEntity coordCoach, string certificates)
        {
            List<int> certificatesList = new List<int>();
            if (!certificates.IsNullOrEmpty())
            {
                foreach (var item in certificates.TrimEnd(',').Split(','))
                {
                    certificatesList.Add(Convert.ToInt32(item));
                }
            }
            var response = new PostFormResponse();
            OperationResult result = new OperationResult(OperationResultType.Success);
            if (userBusiness.IsExistsUserName(coordCoach.User.ID, coordCoach.User.GoogleId.Trim()))
            {
                result.ResultType = OperationResultType.Warning;
                result.Message = "User Name already exists";
            }
            else
            {
                if (coordCoach.User.ID > 0)
                {
                    var userExist = userBusiness.GetUser(coordCoach.User.ID);
                    if (userExist.Status == EntityStatus.Inactive && coordCoach.User.Status == EntityStatus.Active)
                    {
                        operationLogBusiness.InsertLog(OperationEnum.ResetSendInvitation,
                            coordCoach.User.Role.ToDescription(),
                            "Status Changed:" + "Inactive to Active" +
                            ",UserId:" + coordCoach.User.ID,
                            CommonHelper.GetIPAddress(Request), UserInfo);
                    }
                    if (coordCoach.User.Role == Role.Coordinator || coordCoach.User.Role == Role.Mentor_coach)
                    {
                        UserBaseEntity user = new UserBaseEntity();
                        user = coordCoach.User;
                        user.CoordCoach = coordCoach;
                        result = userBusiness.UpdateCoordCoach(user, certificatesList);
                    }
                    else if (coordCoach.User.Role == Role.Video_coding_analyst)
                    {
                        UserBaseEntity user = new UserBaseEntity();
                        VideoCodingEntity videoCoding = new VideoCodingEntity();
                        videoCoding.ID = coordCoach.ID;
                        videoCoding.PrimaryLanguageId = coordCoach.PrimaryLanguageId;
                        videoCoding.PrimaryLanguageOther = coordCoach.PrimaryLanguageOther;
                        videoCoding.SecondaryLanguageId = coordCoach.SecondaryLanguageId;
                        videoCoding.SecondaryLanguageOther = coordCoach.SecondaryLanguageOther;
                        user = coordCoach.User;
                        user.VideoCoding = videoCoding;
                        result = userBusiness.UpdateVideoCoding(user);
                    }
                    else
                    {
                        result = userBusiness.UpdateInternalUser(coordCoach.User);
                    }
                }
                else
                {
                    coordCoach.User.StatusDate = DateTime.Now;
                    coordCoach.User.Sponsor = UserInfo.ID;
                    coordCoach.User.InvitationEmail = InvitationEmailEnum.NotSend;
                    coordCoach.User.Notes = RegisterType.Invitation.ToDescription();
                    if (coordCoach.User.Role == Role.Coordinator || coordCoach.User.Role == Role.Mentor_coach)
                    {
                        coordCoach.SchoolYear = CommonAgent.SchoolYear;
                        result = userBusiness.InsertCoordCoach(coordCoach, certificatesList);
                    }
                    else if (coordCoach.User.Role == Role.Video_coding_analyst)
                    {
                        UserBaseEntity user = new UserBaseEntity();
                        VideoCodingEntity videoCoding = new VideoCodingEntity();
                        videoCoding.PrimaryLanguageId = coordCoach.PrimaryLanguageId;
                        videoCoding.PrimaryLanguageOther = coordCoach.PrimaryLanguageOther;
                        videoCoding.SecondaryLanguageId = coordCoach.SecondaryLanguageId;
                        videoCoding.SecondaryLanguageOther = coordCoach.SecondaryLanguageOther;
                        user = coordCoach.User;
                        user.VideoCoding = videoCoding;
                        result = userBusiness.InsertUser(user);
                        operationLogBusiness.InsertLog(OperationEnum.ResetSendInvitation,
                            user.Role.ToDescription(), "Created User,UserId:" + coordCoach.User.ID,
                            CommonHelper.GetIPAddress(Request), UserInfo);
                    }
                    else
                    {
                        result = userBusiness.InsertUser(coordCoach.User);
                        operationLogBusiness.InsertLog(OperationEnum.ResetSendInvitation,
                            coordCoach.User.Role.ToDescription(), "Created User,UserId:" + coordCoach.User.ID,
                            CommonHelper.GetIPAddress(Request), UserInfo);
                    }
                }
            }
            response.Success = result.ResultType == OperationResultType.Success;
            response.Message = result.Message;
            return JsonHelper.SerializeObject(response);
        }

        [System.Web.Mvc.HttpPost]
        [ValidateAntiForgeryToken]
        [CLIUrlAuthorizeAttribute(Account = Authority.AssessmentEquipment, PageId = PagesModel.CLI_User_Management, Anonymity = Anonymous.Verified)]
        public string SaveEquipment(int coordCoachId, string[] serialNumber, string[] uTHealthTag,
            int[] chkEquipment, int[] isAssessmentEquipment)
        {
            var response = new PostFormResponse();
            if (isAssessmentEquipment[0] > 0)
            {
                OperationResult result = new OperationResult(OperationResultType.Success);
                result = userBusiness.CoordCoachEquipment(coordCoachId, serialNumber, uTHealthTag,
                    chkEquipment, isAssessmentEquipment);
                response.Success = result.ResultType == OperationResultType.Success;
                response.Message = result.Message;
            }
            response.ModelState = ModelState;
            return JsonHelper.SerializeObject(response);
        }

        private void InitAccessOperation()
        {
            bool accessView = false;
            bool accessAdd = false;
            bool accessEdit = false;
            bool accessEquipment = false;
            bool accessCommunity = false;
            bool accessDelete = false;
            bool accessCoordCoach = false;
            if (UserInfo != null)
            {
                UserAuthorityModel userAuthority = new PermissionBusiness().GetUserAuthority(UserInfo, (int)PagesModel.CLI_User_Management);

                if (userAuthority != null)
                {
                    if ((userAuthority.Authority & (int)Authority.View) == (int)Authority.View)
                    {
                        accessView = true;
                    }
                    if ((userAuthority.Authority & (int)Authority.Add) == (int)Authority.Add)
                    {
                        accessAdd = true;
                    }
                    if ((userAuthority.Authority & (int)Authority.Edit) == (int)Authority.Edit)
                    {
                        accessEdit = true;
                    }
                    if ((userAuthority.Authority & (int)Authority.AssessmentEquipment) == (int)Authority.AssessmentEquipment)
                    {
                        accessEquipment = true;
                    }
                    if ((userAuthority.Authority & (int)Authority.Delete) == (int)Authority.Delete)
                    {
                        accessDelete = true;
                    }
                    if ((userAuthority.Authority & (int)Authority.Assign) == (int)Authority.Assign)
                    {
                        accessCommunity = true;
                        accessCoordCoach = true;
                    }
                }
            }
            ViewBag.accessView = accessView;
            ViewBag.accessAdd = accessAdd;
            ViewBag.accessEdit = accessEdit;
            ViewBag.accessEquipment = accessEquipment;
            ViewBag.accessDelete = accessDelete;
            ViewBag.accessCommunity = accessCommunity;
            ViewBag.accessCoordCoach = accessCoordCoach;
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.Assign, PageId = PagesModel.CLI_User_Management, Anonymity = Anonymous.Verified)]
        public ActionResult AssignCoaches(int userId, bool isCoord)
        {
            UserBaseEntity user = userBusiness.GetUser(userId);
            ViewBag.Name = user == null ? "" : user.FirstName + " " + user.LastName;
            ViewBag.IsCoord = isCoord;
            ViewBag.UserId = userId;
            if (isCoord)
                ViewBag.Title = "Assign Coordinator to Intervention Manager";
            else
                ViewBag.Title = "Assign Mentor/Coach to Intervention Manager";
            return View();
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.Assign, PageId = PagesModel.CLI_User_Management, Anonymity = Anonymous.Verified)]
        public string SearchUnassigedCoordCoaches(int userId, bool isCoord, string name = "", string sort = "ID",
            string order = "Asc", int first = 0, int count = 10)
        {
            int total = 0;
            var expression = PredicateHelper.True<CoordCoachEntity>();
            expression = expression.And(r => !(r.User.IntManaCoachRelations.Any(s => s.PMUserId == userId)));
            if (!string.IsNullOrEmpty(name.Trim()))
            {
                expression = expression.And(r => r.User.FirstName.Contains(name));
                expression = expression.Or(r => r.User.LastName.Contains(name));
            }
            expression = expression.And(r => r.User.Status == EntityStatus.Active && r.User.IsDeleted == false);
            if (isCoord)
                expression = expression.And(r => r.User.Role == Role.Coordinator);
            else
                expression = expression.And(r => r.User.Role == Role.Mentor_coach);

            var list = userBusiness.GetUnAssignedCoordMentors(expression, sort, order, first, count, out total);
            var result = new { total = total, data = list };
            return JsonHelper.SerializeObject(result);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.Assign, PageId = PagesModel.CLI_User_Management, Anonymity = Anonymous.Verified)]
        public string SearchAssignedCoordCoaches(int userId, bool isCoord, string name = "", string sort = "CommunityName",
            string order = "Asc", int first = 0, int count = 10)
        {
            int total = 0;
            var expression = PredicateHelper.True<IntManaCoachRelationEntity>();
            expression = expression.And(r => r.PMUserId == userId && r.User.Status == EntityStatus.Active && r.User.IsDeleted == false);
            if (!string.IsNullOrEmpty(name.Trim()))
            {
                expression = expression.And(r => r.User.FirstName.Contains(name));
                expression = expression.Or(r => r.User.LastName.Contains(name));
            }
            if (isCoord)
                expression = expression.And(r => r.User.Role == Role.Coordinator && r.User.CoordCoach != null);
            else
                expression = expression.And(r => r.User.Role == Role.Mentor_coach && r.User.CoordCoach != null);
            var list = userBusiness.GetAssignedCoordMentors(expression, sort, order, first, count, out total);
            var result = new { total = total, data = list };
            return JsonHelper.SerializeObject(result);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.Assign, PageId = PagesModel.CLI_User_Management, Anonymity = Anonymous.Verified)]
        public string AssignCoorCoaches(int userId, int[] coorIds)
        {
            var response = new PostFormResponse();
            OperationResult result = new OperationResult(OperationResultType.Success);
            result = userBusiness.AssignCoorCoachRelations(userId, UserInfo.ID, coorIds);
            response.Success = result.ResultType == OperationResultType.Success;
            response.Message = result.Message;
            return JsonHelper.SerializeObject(response);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.Assign, PageId = PagesModel.CLI_User_Management, Anonymity = Anonymous.Verified)]
        public string DeleteCoorCoaches(int userId, int[] coorIds)
        {
            var response = new PostFormResponse();
            OperationResult result = new OperationResult(OperationResultType.Success);
            result = userBusiness.DeleteCoorCoachRelations(userId, coorIds);
            response.Success = result.ResultType == OperationResultType.Success;
            response.Message = result.Message;
            return JsonHelper.SerializeObject(response);
        }

        #region page access
        private string StatusAccess()
        {
            switch (UserInfo.Role)
            {
                case Role.Coordinator:
                case Role.Mentor_coach:
                    return "R";
                case Role.Super_admin:
                case Role.Statisticians:
                case Role.Content_personnel:
                case Role.Intervention_manager:
                case Role.Intervention_support_personnel:
                case Role.Administrative_personnel:
                    return "RW";
                default:
                    return "R";
            }
        }

        private string NoteAccess()
        {
            switch (UserInfo.Role)
            {
                case Role.Video_coding_analyst:
                    return "R";
                case Role.Coordinator:
                case Role.Mentor_coach:
                case Role.Statewide:
                case Role.Community:
                case Role.TRS_Specialist:
                case Role.District_Community_Specialist:
                case Role.Principal:
                case Role.Teacher:
                case Role.Parent:
                    return "X";
                default:
                    return "RW";
            }
        }

        private string OtherAccess()
        {
            switch (UserInfo.Role)
            {
                case Role.Coordinator:
                case Role.Mentor_coach:
                    return "R";
                case Role.Statewide:
                case Role.Community:
                case Role.District_Community_Delegate:
                case Role.District_Community_Specialist:
                case Role.Community_Specialist_Delegate:
                case Role.Principal:
                case Role.Principal_Delegate:
                case Role.TRS_Specialist:
                case Role.TRS_Specialist_Delegate:
                case Role.School_Specialist:
                case Role.School_Specialist_Delegate:
                case Role.Teacher:
                case Role.Parent:
                    return "X";
                default:
                    return "RW";
            }
        }

        private string EquipmentAccess()
        {
            switch (UserInfo.Role)
            {
                case Role.Coordinator:
                case Role.Mentor_coach:
                case Role.Video_coding_analyst:
                    return "R";
                case Role.Statewide:
                case Role.Community:
                case Role.District_Community_Delegate:
                case Role.District_Community_Specialist:
                case Role.Community_Specialist_Delegate:
                case Role.Principal:
                case Role.Principal_Delegate:
                case Role.TRS_Specialist:
                case Role.TRS_Specialist_Delegate:
                case Role.School_Specialist:
                case Role.School_Specialist_Delegate:
                case Role.Teacher:
                case Role.Parent:
                    return "X";
                default:
                    return "RW";
            }
        }
        #endregion
    }
}