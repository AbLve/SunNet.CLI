using Sunnet.Cli.Business.Communities;
using Sunnet.Cli.Business.Log;
using Sunnet.Cli.Business.MasterData;
using Sunnet.Cli.Business.Schools;
using Sunnet.Cli.Business.StatusTracking;
using Sunnet.Cli.Core.Communities.Entities;
using Sunnet.Cli.Core.Log.Entities;
using Sunnet.Cli.Core.Schools.Entities;
using Sunnet.Cli.Core.StatusTracking.Entities;
using Sunnet.Cli.Core.StatusTracking.Enums;
using Sunnet.Cli.Core.Users.Entities;
using Sunnet.Cli.Core.Users.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Sunnet.Cli.MainSite.Models;
using Sunnet.Framework.Extensions;
using Sunnet.Cli.Business.Users;
using Sunnet.Cli.Core.Common.Enums;
using Sunnet.Cli.UIBase.Models;
using Sunnet.Framework.Core.Tool;
using Sunnet.Cli.Business.Common;
using Sunnet.Cli.MainSite.Controllers;
using Sunnet.Cli.Business.Classes;
using Sunnet.Framework.Helpers;
using Sunnet.Framework;
using StructureMap;
using Sunnet.Framework.Encrypt;
using Sunnet.Cli.Business.Permission.Models;
using Sunnet.Cli.Business.Permission;
using Sunnet.Cli.Core.Students.Enums;
using Sunnet.Framework.Permission;
using Sunnet.Cli.UIBase.Filters;
using Sunnet.Cli.Business.Users.Models;
using Sunnet.Cli.Core.MasterData.Entities;
using Sunnet.Framework.Resources;

namespace Sunnet.Cli.MainSite.Areas.Invitation.Controllers
{
    public class TeacherController : BaseController
    {
        private readonly UserBusiness userBusiness;
        private readonly ClassBusiness classBusiness;
        private readonly PermissionBusiness permissionBusiness;
        private readonly MasterDataBusiness masterDataBusiness;
        private readonly StatusTrackingBusiness statusTrackingBusiness;
        private readonly OperationLogBusiness operationLogBusiness;
        private readonly SchoolBusiness schoolBusiness;
        private readonly CommunityBusiness communityBusiness;
        public TeacherController()
        {
            userBusiness = new UserBusiness(UnitWorkContext);
            classBusiness = new ClassBusiness(UnitWorkContext);
            permissionBusiness = new PermissionBusiness(UnitWorkContext);
            masterDataBusiness = new MasterDataBusiness(UnitWorkContext);
            statusTrackingBusiness = new StatusTrackingBusiness(UnitWorkContext);
            operationLogBusiness = new OperationLogBusiness(UnitWorkContext);
            schoolBusiness = new SchoolBusiness(UnitWorkContext);
            communityBusiness = new CommunityBusiness(UnitWorkContext);
        }
        //
        // GET: /Invitation/Teacher/
        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.Teacher, Anonymity = Anonymous.Verified)]
        public ActionResult Index()
        {
            InitAccessOperation();
            UserModel userModel = new UserModel();
            return View(userModel);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.Teacher, Anonymity = Anonymous.Verified)]
        public string Search(string txtCommunity, string txtSchool, string teacherCode, string firstName, string lastName, int communityId = 0,
            int schoolId = 0, int status = -1, string sort = "LastName", string order = "Asc", int first = 0, int count = 10)
        {
            var total = 0;
            var expression = PredicateHelper.True<TeacherEntity>();
            if (communityId > 0)
                expression = expression.And(r => r.UserInfo.UserCommunitySchools.Any(s => s.CommunityId == communityId));
            if (txtCommunity.Trim() != string.Empty)
                expression =
                    expression.And(r => r.UserInfo.UserCommunitySchools.Any(s => s.Community.Name.Contains(txtCommunity)));
            if (schoolId > 0)
                expression = expression.And(r => r.UserInfo.UserCommunitySchools.Any(s => s.SchoolId == schoolId));
            else if (txtSchool.Trim() != string.Empty)
                expression = expression.And(r => r.UserInfo.UserCommunitySchools.Any(s => s.School.Name.Contains(txtSchool)));
            if (status > 0)
                expression = expression.And(r => (int)r.UserInfo.Status == status);
            if (teacherCode.Trim() != string.Empty)
                expression = expression.And(r => r.TeacherId.Contains(teacherCode));
            if (firstName.Trim() != string.Empty)
                expression = expression.And(r => r.UserInfo.FirstName.Contains(firstName));
            if (lastName.Trim() != string.Empty)
                expression = expression.And(r => r.UserInfo.LastName.Contains(lastName));

            var list = userBusiness.SearchTeachers(UserInfo, expression, sort, order, first, count, out total);
            var result = new { total = total, data = list };
            return JsonHelper.SerializeObject(result);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.Transaction, PageId = PagesModel.Teacher, Anonymity = Anonymous.Verified)]
        public string SearchTeacherTransaction(int teacherId, string sort = "CreatedOn", string order = "Desc")
        {
            var list = userBusiness.SearchTeacherTransactions(x => x.TeacherId == teacherId, sort, order);
            var result = new { data = list };
            return JsonHelper.SerializeObject(result);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.Add, PageId = PagesModel.Teacher, Anonymity = Anonymous.Verified)]
        public ActionResult New()
        {
            TeacherRoleEntity roleEntity = userBusiness.GetTeacherRoleEntity(UserInfo.Role);
            if (roleEntity != null)
                ViewBag.Role = JsonHelper.SerializeObject(roleEntity);

            UserBaseEntity userEntity = new UserBaseEntity();
            TeacherEntity teacherEntity = new TeacherEntity();
            teacherEntity.MediaRelease = MediaRelease.No;
            teacherEntity.UserInfo = userEntity;
            ViewBag.AgeGroup = new SelectList(AgeGroup.Infant.ToSelectList(), "Value", "Text");
            ViewBag.CoachOptions = new List<SelectItemModel>().ToSelectList(ViewTextHelper.DefaultPleaseSelectText, "0");
            ViewBag.Language = userBusiness.GetLanguages().ToSelectList(ViewTextHelper.DefaultPleaseSelectText, "0");
            ViewBag.Funding = userBusiness.GetFundings().ToSelectList(ViewTextHelper.DefaultPleaseSelectText, "0");
            ViewBag.State = userBusiness.GetStates().ToSelectList("State", "0");
            ViewBag.County = userBusiness.GetCountries().ToSelectList("County", "0");
            ViewBag.YearsInProject = userBusiness.GetYearsInProjects().ToSelectList(ViewTextHelper.DefaultPleaseSelectText, "0");
            ViewBag.PDList = userBusiness.GetPDs();
            ViewBag.CertificateList = userBusiness.GetCertificates();
            ViewBag.RoleType = UserInfo.Role;
            ViewBag.IsCLIUser = UserInfo.IsCLIUser;
            return View(teacherEntity);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.Edit, PageId = PagesModel.Teacher, Anonymity = Anonymous.Verified)]
        public ActionResult Edit(int id)
        {
            TeacherRoleEntity roleEntity = userBusiness.GetTeacherRoleEntity(UserInfo.Role);
            if (roleEntity != null)
                ViewBag.Role = JsonHelper.SerializeObject(roleEntity);

            TeacherEntity teacherEntity = userBusiness.GetTeacher(id, UserInfo);

            string certificateText = "";
            foreach (var item in teacherEntity.UserInfo.Certificates)
            {
                certificateText += item.ID + ",";
            }
            ViewBag.CertificateText = certificateText;

            ViewBag.AgeGroupJson = JsonHelper.SerializeObject(teacherEntity.TeacherAgeGroups.Select(
                e => new { AgeGroup = e.AgeGroup, AgeGroupOther = e.AgeGroupOther }));
            ViewBag.AgeGroup = new SelectList(AgeGroup.Infant.ToSelectList(), "Value", "Text");
            ViewBag.CoachOptions = userBusiness.GetCoordCoachs(teacherEntity.UserInfo.UserCommunitySchools.Select(x => x.CommunityId).ToList())
                .ToSelectList(ViewTextHelper.DefaultPleaseSelectText, "0");
            ViewBag.Language = userBusiness.GetLanguages().ToSelectList(ViewTextHelper.DefaultPleaseSelectText, "0");
            ViewBag.Funding = userBusiness.GetFundings().ToSelectList(ViewTextHelper.DefaultPleaseSelectText, "0");
            ViewBag.State = userBusiness.GetStates().ToSelectList("State", "0");
            ViewBag.County = userBusiness.GetCountries().ToSelectList("County", "0");
            ViewBag.YearsInProject = userBusiness.GetYearsInProjects().ToSelectList(ViewTextHelper.DefaultPleaseSelectText, "0");
            ViewBag.PDList = userBusiness.GetPDs();
            ViewBag.CertificateList = userBusiness.GetCertificates();

            ViewBag.GroupPackages = permissionBusiness.GetAssignedPackages(
                (int)Role.Teacher, teacherEntity.UserInfo.UserCommunitySchools.Select(x => x.SchoolId).ToList(),
                teacherEntity.UserInfo.UserCommunitySchools.Select(x => x.CommunityId).ToList());
            ViewBag.GroupPackageSelected = teacherEntity.UserInfo.PermissionRoles.Select(e =>
               new GroupPackageModel()
               {
                   PackageId = e.ID,
                   PackageName = e.Name,
                   PackageDescription = e.Descriptions
               }).ToList();
            ViewBag.IsCLIUser = UserInfo.IsCLIUser;
            return View(teacherEntity);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.View, PageId = PagesModel.Teacher, Anonymity = Anonymous.Verified)]
        public ActionResult View(int id)
        {
            TeacherRoleEntity roleEntity = userBusiness.GetTeacherRoleEntity(UserInfo.Role);
            if (roleEntity != null)
                ViewBag.Role = JsonHelper.SerializeObject(roleEntity);
            TeacherEntity teacherEntity = userBusiness.GetTeacher(id, UserInfo);
            ViewBag.AgeGroup = teacherEntity.TeacherAgeGroups.Select(e => ((AgeGroup)e.AgeGroup).ToDescription()).ToList();
            if (teacherEntity.CoachId > 0)
            {
                UserBaseEntity userCoach = userBusiness.GetUser(teacherEntity.CoachId);
                if (userCoach != null)
                    ViewBag.Coach = userCoach.FirstName + " " + userCoach.LastName + " (" + userCoach.Role.ToDescription() + ")";
            }
            if (teacherEntity.CLIFundingID > 0)
            {
                FundingEntity funding = masterDataBusiness.GetFunding(teacherEntity.CLIFundingID);
                if (funding != null)
                    ViewBag.Funding = funding.Name;
            }
            if (teacherEntity.StateId > 0)
            {
                StateEntity state = masterDataBusiness.GetState(teacherEntity.StateId);
                if (state != null)
                    ViewBag.State = state.Name;
            }
            if (teacherEntity.CountyId > 0)
            {
                CountyEntity county = masterDataBusiness.GetCounty(teacherEntity.CountyId);
                if (county != null)
                    ViewBag.County = county.Name;
            }
            if (teacherEntity.PrimaryLanguageId > 0)
            {
                LanguageEntity language = masterDataBusiness.GetLanguage(teacherEntity.PrimaryLanguageId);
                if (language != null)
                    ViewBag.Language = language.Language;
            }
            if (teacherEntity.SecondaryLanguageId > 0)
            {
                LanguageEntity language = masterDataBusiness.GetLanguage(teacherEntity.SecondaryLanguageId);
                if (language != null)
                    ViewBag.Language2 = language.Language;
            }
            if (teacherEntity.YearsInProjectId > 0)
            {
                YearsInProjectEntity yearsInProject = userBusiness.GetYearsInProject(teacherEntity.YearsInProjectId);
                if (yearsInProject != null)
                    ViewBag.YearsInProject = yearsInProject.YearsInProject;
            }
            ViewBag.PD = teacherEntity.UserInfo.ProfessionalDevelopments.Select(e => e.ProfessionalDevelopment).ToList();
            ViewBag.CertificateList = userBusiness.GetCertificates();

            ViewBag.GroupPackageSelected = teacherEntity.UserInfo.PermissionRoles.Where(e => e.IsDefault == false).Select(e =>
               new GroupPackageModel()
               {
                   PackageName = e.Name
               }).ToList().Select(x => x.PackageName).ToList();
            ViewBag.IsCLIUser = UserInfo.IsCLIUser;
            return View(teacherEntity);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.AssessmentEquipment, PageId = PagesModel.Teacher, Anonymity = Anonymous.Verified)]
        public ActionResult Equipment(int id)
        {
            TeacherRoleEntity roleEntity = userBusiness.GetTeacherRoleEntity(UserInfo.Role);
            if (roleEntity != null)
                ViewBag.Role = JsonHelper.SerializeObject(roleEntity);

            ViewBag.TeacherId = id;
            TeacherEntity teacher = userBusiness.GetTeacher(id, null);
            ViewBag.IsAssessmentEquipment = teacher.IsAssessmentEquipment;
            ViewBag.TeacherEquipments = teacher.TeacherEquipmentRelations.ToList();
            ViewBag.Equipment = EquipmentEnum.Camera.ToSelectList();
            return View();
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.Transaction, PageId = PagesModel.Teacher, Anonymity = Anonymous.Verified)]
        public ActionResult Transaction(int id)
        {
            TeacherRoleEntity roleEntity = userBusiness.GetTeacherRoleEntity(UserInfo.Role);
            if (roleEntity != null)
                ViewBag.Role = JsonHelper.SerializeObject(roleEntity);

            TeacherEntity teacher = userBusiness.GetTeacher(id, null);
            ViewBag.FundingOptions = userBusiness.GetFundings().ToSelectList(ViewTextHelper.DefaultPleaseSelectText, "0");
            TeacherTransactionEntity transaction = new TeacherTransactionEntity();
            transaction.FundingYear = CommonAgent.SchoolYear;
            transaction.Teacher = teacher;
            transaction.TeacherId = id;
            return View(transaction);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.Assign, PageId = PagesModel.Teacher, Anonymity = Anonymous.Verified)]
        public ActionResult AssignClass(int id)
        {
            TeacherRoleEntity roleEntity = userBusiness.GetTeacherRoleEntity(UserInfo.Role);
            if (roleEntity != null)
                ViewBag.Role = JsonHelper.SerializeObject(roleEntity);

            TeacherEntity teacher = userBusiness.GetTeacher(id, null);
            ViewBag.TeacherId = id;
            ViewBag.ClassSelected = teacher.Classes.Where(e => e.IsDeleted == false).Select(e =>
                new AssigenStudentClassModel()
                {
                    ClassId = e.ID,
                    ClassName = e.Name,
                    DayType = e.DayType,
                    //TODO:Classroom and Class has many to many
                    ClasroomNameList = e.ClassroomClasses.Select(x => x.Classroom.Name),
                    ClassCode = e.ClassId
                }).ToList();
            ViewBag.Classs =
                classBusiness.GetClassesBySchoolId(teacher.UserInfo.UserCommunitySchools.Select(x => x.SchoolId).ToList(),
                    UserInfo);
            return View();
        }

        [System.Web.Mvc.HttpPost]
        [ValidateAntiForgeryToken]
        [CLIUrlAuthorizeAttribute(Account = Authority.Edit, PageId = PagesModel.Teacher, Anonymity = Anonymous.Verified)]
        public string SaveInvitation(TeacherEntity teacher, int[] ageGroups, int[] chkPDs,
            string certificates, bool? isInvite, int[] chkPackages, int communityId, int? schoolId, bool confirm = false)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            PostFormResponse response = new PostFormResponse();
            if (confirm == false)
            {
                if (teacher.ID == 0)
                {
                    if (CheckExisted(teacher, communityId, schoolId == null ? 0 : schoolId.Value, response))
                        return JsonHelper.SerializeObject(response);
                }
                else
                {
                    if (userBusiness.CheckUserExistedStatus(teacher.UserInfo.ID,
                        teacher.UserInfo.FirstName,
                        teacher.UserInfo.LastName, teacher.UserInfo.PrimaryEmailAddress,
                        teacher.UserInfo.Role, out result))
                    {
                        response.Success = true;
                        response.Message = result.Message;
                        response.Data = result.AppendData;
                        return JsonHelper.SerializeObject(response);
                    }
                }
            }
            List<int> certificatesList = new List<int>();
            if (!certificates.IsNullOrEmpty())
            {
                foreach (var item in certificates.TrimEnd(',').Split(','))
                {
                    certificatesList.Add(Convert.ToInt32(item));
                }
            }
            if (teacher.ID == 0)
            {
                teacher.UserInfo.GoogleId = "";
                teacher.UserInfo.Role = Role.Teacher;
                teacher.SchoolYear = CommonAgent.SchoolYear;
                teacher.UserInfo.StatusDate = DateTime.Now;
                teacher.UserInfo.Sponsor = UserInfo.ID;
                teacher.UserInfo.InvitationEmail = InvitationEmailEnum.NotSend;
                teacher.UserInfo.Notes = RegisterType.Invitation.ToDescription();
            }
            if (isInvite == true)
            {
                teacher.UserInfo.EmailExpireTime = DateTime.Now.AddDays(SFConfig.ExpirationTime);
                teacher.UserInfo.InvitationEmail = InvitationEmailEnum.Sent;
            }
            teacher.UserInfo.PrimaryPhoneNumber = (teacher.UserInfo.PrimaryPhoneNumber) == null
                ? ""
                : teacher.UserInfo.PrimaryPhoneNumber;
            ModelState.Remove("UserInfo.PrimaryPhoneNumber");
            if (response.Success = ModelState.IsValid)
            {
                teacher.BirthDate = teacher.BirthDate ?? Convert.ToDateTime("1753-01-01");

                if (teacher.ID > 0)
                {
                    var user = userBusiness.GetUser(teacher.UserInfo.ID);
                    if (user.Status == EntityStatus.Inactive && teacher.UserInfo.Status == EntityStatus.Active)
                    {
                        operationLogBusiness.InsertLog(OperationEnum.ResetSendInvitation,
                            teacher.UserInfo.Role.ToDescription(),
                            "Status Changed:" + "Inactive to Active" +
                            ",UserId:" + teacher.UserInfo.ID,
                            CommonHelper.GetIPAddress(Request), UserInfo);
                    }
                    result = userBusiness.UpdateTeacher(teacher, ageGroups, chkPDs, certificatesList, chkPackages,
                            UserInfo.Role);
                }
                else
                {
                    result = userBusiness.InsertTeacher(teacher, ageGroups, chkPDs, certificatesList, chkPackages,
                        communityId, schoolId == null ? 0 : schoolId.Value, UserInfo.ID);
                    operationLogBusiness.InsertLog(OperationEnum.ResetSendInvitation,
                        teacher.UserInfo.Role.ToDescription(), "Created User,UserId:" + teacher.UserInfo.ID,
                        CommonHelper.GetIPAddress(Request), UserInfo);
                }

                if (isInvite == true)
                {
                    EmailTemplete template = XmlHelper.GetEmailTemplete("TeacherInvitation_Template.xml");
                    string param = teacher.UserInfo.ID.ToString() + "," + DateTime.Now.ToString();
                    string encryptParam = ObjectFactory.GetInstance<IEncrypt>().Encrypt(param);
                    string link = SFConfig.MainSiteDomain + "Home/InviteVerification/"
                        + System.Web.HttpContext.Current.Server.UrlEncode(encryptParam);
                    string emailBody = template.Body.Replace("{FirstName}", teacher.UserInfo.FirstName)
                    .Replace("{LastName}", teacher.UserInfo.LastName)
                    .Replace("{InviteLink}", "<a style='text-decoration: underline; cursor:pointer; color: #008000;' href='" + link + "'>Click here</a>")
                    .Replace("{StaticDomain}", SFConfig.StaticDomain)
                    .Replace("{CliEngageUrl}", SFConfig.MainSiteDomain);
                    userBusiness.SendEmail(teacher.UserInfo.PrimaryEmailAddress, template.Subject, emailBody);
                    EmailLogEntity emailLog = new EmailLogEntity(teacher.UserInfo.ID,
                        teacher.UserInfo.PrimaryEmailAddress, EmailLogType.Invitation);
                    userBusiness.InsertEmailLog(emailLog);
                    operationLogBusiness.InsertLog(OperationEnum.ResetSendInvitation,
                        teacher.UserInfo.Role.ToDescription(), "Send Invitation,UserId:" + teacher.UserInfo.ID,
                        CommonHelper.GetIPAddress(Request), UserInfo);
                }
                response.Success = result.ResultType == OperationResultType.Success;
                response.Message = result.Message;
            }
            response.ModelState = ModelState;
            return JsonHelper.SerializeObject(response);
        }

        private bool CheckExisted(TeacherEntity teacher, int communityId, int schoolId, PostFormResponse response)
        {
            teacher.UserInfo.Role = Role.Teacher;
            var existedUserId = 0;
            Role existedUserRole;
            var existedStatus = userBusiness.CheckTeacherUserExistedStatus(teacher.UserInfo.ID, teacher.UserInfo.FirstName,
                teacher.UserInfo.LastName, teacher.UserInfo.PrimaryEmailAddress, teacher.UserInfo.Role,
                communityId, schoolId, out existedUserId, out existedUserRole);
            if (existedStatus == UserExistedStatus.ExistedInSchool)
            {
                // do nothing
                response.Success = true;
                response.Message = ResourceHelper.GetRM().GetInformation("UserExistedInSchool")
                            .Replace("{Role}", existedUserRole.ToDescription().ToLower());
                response.Data = "waiting";
                return true;
            }
            else if (existedStatus == UserExistedStatus.ExistedInCommunity)
            {
                response.Success = true;
                response.Message = ResourceHelper.GetRM().GetInformation("UserExistedInCommunity")
                            .Replace("{Role}", existedUserRole.ToDescription().ToLower());
                response.Data = "waiting";
                return true;
            }
            else if (existedStatus == UserExistedStatus.UserExisted)
            {
                if (existedUserRole == Role.Teacher)
                {
                    if (UserInfo.Role == Role.Community || UserInfo.Role == Role.District_Community_Specialist
                        || UserInfo.Role == Role.Statewide)
                    {
                        //如果当前用户所属的Community包含查询出的用户所属的Community，则直接跳转到分配页面，否则需要发送邮件请求
                        if (
                            UserInfo.UserCommunitySchools.Where(e => e.SchoolId > 0).Any(
                                e => e.School.UserCommunitySchools.Any(o => o.UserId == existedUserId)))
                        {
                            response.Success = true;
                            response.Message = ResourceHelper.GetRM().GetInformation("UserExistedInCurrentRole")
                                .Replace("{Role}", existedUserRole.ToDescription().ToLower());
                            response.Data = new
                            {
                                type = "confirmAssign",
                                url =
                                    Url.Action(schoolId > 0 ? "TeacherAssignSchool" : "AssignCommunity", "Public",
                                        new { userId = existedUserId, roleType = teacher.UserInfo.Role })
                            };
                            return true;
                        }
                        else
                        {
                            response.Success = true;
                            response.Message =
                                ResourceHelper.GetRM().GetInformation("UserExistedInSystemSendEmail")
                                    .Replace("{Role}", existedUserRole.ToDescription().ToLower());
                            response.Data = new
                            {
                                type = "confirmEmail",
                                userId = existedUserId,
                                invitationType = schoolId > 0 ? "school" : "community"
                            };
                            return true;
                        }
                    }
                    else if (UserInfo.Role == Role.District_Community_Delegate || UserInfo.Role == Role.Community_Specialist_Delegate)
                    {
                        UserBaseEntity parentUser = userBusiness.GetUser(UserInfo.ID);
                        //如果当前Delegate用户的父级所属的Community包含查询出的用户所属的Community，则直接跳转到分配页面，否则需要发送邮件请求
                        if (
                            parentUser.UserCommunitySchools.Where(e => e.SchoolId > 0).Any(
                                e => e.School.UserCommunitySchools.Any(o => o.UserId == existedUserId)))
                        {
                            response.Success = true;
                            response.Message = ResourceHelper.GetRM().GetInformation("UserExistedInCurrentRole")
                                .Replace("{Role}", existedUserRole.ToDescription().ToLower());
                            response.Data = new
                            {
                                type = "confirmAssign",
                                url =
                                    Url.Action(schoolId > 0 ? "TeacherAssignSchool" : "AssignCommunity", "Public",
                                        new { userId = existedUserId, roleType = teacher.UserInfo.Role })
                            };
                            return true;
                        }
                        else
                        {
                            response.Success = true;
                            response.Message =
                                ResourceHelper.GetRM().GetInformation("UserExistedInSystemSendEmail")
                                    .Replace("{Role}", existedUserRole.ToDescription().ToLower());
                            response.Data = new
                            {
                                type = "confirmEmail",
                                userId = existedUserId,
                                invitationType = schoolId > 0 ? "school" : "community"
                            };
                            return true;
                        }
                    }
                    else if (UserInfo.Role == Role.Principal
                             || UserInfo.Role == Role.TRS_Specialist
                             || UserInfo.Role == Role.School_Specialist)
                    {
                        //如果当前用户所属的School包含查询出的用户所属的School，则直接跳转到分配页面，否则需要发送邮件请求
                        if (
                            UserInfo.UserCommunitySchools.Any(
                                m => m.School.UserCommunitySchools.Any(n => n.UserId == existedUserId)))
                        {
                            response.Success = true;
                            response.Message = ResourceHelper.GetRM().GetInformation("UserExistedInCurrentRole")
                                .Replace("{Role}", existedUserRole.ToDescription().ToLower());
                            response.Data = new
                            {
                                type = "confirmAssign",
                                url =
                                    Url.Action("TeacherAssignSchool", "Public",
                                        new { userId = existedUserId, roleType = teacher.UserInfo.Role })
                            };
                            return true;
                        }
                        else
                        {
                            response.Success = true;
                            response.Message =
                                ResourceHelper.GetRM().GetInformation("UserExistedInSystemSendEmail")
                                    .Replace("{Role}", existedUserRole.ToDescription().ToLower());
                            response.Data = new
                            {
                                type = "confirmEmail",
                                userId = existedUserId,
                                invitationType = "school"
                            };
                            return true;
                        }
                    }
                    else if (UserInfo.Role == Role.Principal_Delegate
                             || UserInfo.Role == Role.TRS_Specialist_Delegate
                             || UserInfo.Role == Role.School_Specialist_Delegate)
                    {
                        UserBaseEntity parentUser = userBusiness.GetUser(UserInfo.ID);
                        //如果当前Delegate用户的父级所属的School包含查询出的用户所属的School，则直接跳转到分配页面，否则需要发送邮件请求
                        if (
                            parentUser.UserCommunitySchools.Any(
                                m => m.School.UserCommunitySchools.Any(n => n.UserId == existedUserId)))
                        {
                            response.Success = true;
                            response.Message = ResourceHelper.GetRM().GetInformation("UserExistedInCurrentRole")
                                .Replace("{Role}", existedUserRole.ToDescription().ToLower());
                            response.Data = new
                            {
                                type = "confirmAssign",
                                url =
                                    Url.Action("TeacherAssignSchool", "Public",
                                        new { userId = existedUserId, roleType = teacher.UserInfo.Role })
                            };
                            return true;
                        }
                        else
                        {
                            response.Success = true;
                            response.Message =
                                ResourceHelper.GetRM().GetInformation("UserExistedInSystemSendEmail")
                                    .Replace("{Role}", existedUserRole.ToDescription().ToLower());
                            response.Data = new
                            {
                                type = "confirmEmail",
                                userId = existedUserId,
                                invitationType = "school"
                            };
                            return true;
                        }
                    }
                    else if (UserInfo.Role <= Role.Mentor_coach)
                    {
                        // 内部用户提醒分配 
                        response.Success = true;
                        response.Message = ResourceHelper.GetRM().GetInformation("UserExistedInCurrentRole")
                            .Replace("{Role}", existedUserRole.ToDescription().ToLower());
                        response.Data = new
                        {
                            type = "confirmAssign",
                            url =
                                Url.Action(schoolId > 0 ? "TeacherAssignSchool" : "AssignCommunity", "Public",
                                    new { userId = existedUserId, roleType = teacher.UserInfo.Role })
                        };
                        return true;
                    }
                    else
                    {
                        // 已存在Teacher 角色的并且FirstName,LastName,Email相同的记录,但是操作者没有权限(意外勾选了不属于操作者的权限)
                        response.Success = true;
                        response.Message = ResourceHelper.GetRM()
                            .GetInformation("UserExistedInCurrentRole")
                            .Replace("{Role}", existedUserRole.ToDescription().ToLower());
                        response.Data = new
                        {
                            type = "noauthority"
                        };
                        return true;
                    }
                }
                else
                {
                    // 存在其他角色的用户提醒 Continue
                    response.Success = true;
                    response.Message = ResourceHelper.GetRM()
                        .GetInformation("UserExistedInSystem")
                        .Replace("{Role}", existedUserRole.ToDescription().ToLower());
                    response.Data = new
                    {
                        type = "continue"
                    };
                    return true;
                }
            }
            return false;
        }

        [HttpPost]
        [CLIUrlAuthorizeAttribute(Account = Authority.Index, PageId = PagesModel.Teacher, Anonymity = Anonymous.Verified)]
        public string SendEmail(string invitationType, int communityId, string txtCommunity, int? schoolId,
            string txtSchool, string email, int userId)
        {
            if (schoolId == null)
                schoolId = 0;
            EmailTemplete template = XmlHelper.GetEmailTemplete("NoPermission_Invite_Template.xml");
            var user = userBusiness.GetUser(userId);
            var recipient = user.FirstName + " " + user.LastName;

            var approveLink = new LinkModel()
            {
                RoleType = Role.Teacher,
                Host = SFConfig.MainSiteDomain,
                Path = "Approve/",
                Sender = UserInfo.ID,
                Recipient = userId
            };
            approveLink.Others.Add("CommunityId", communityId);
            approveLink.Others.Add("SchoolId", schoolId);

            var denyLink = new LinkModel()
            {
                RoleType = Role.Teacher,
                Host = SFConfig.MainSiteDomain,
                Path = "Deny/",
                Sender = UserInfo.ID,
                Recipient = userId
            };
            denyLink.Others.Add("CommunityId", communityId);
            denyLink.Others.Add("SchoolId", schoolId);

            string systemObject = "";
            if (invitationType == "community")
                systemObject = txtCommunity;
            else
                systemObject = txtSchool;
            string emailBody = template.Body.Replace("{StaticDomain}", SFConfig.StaticDomain)
                .Replace("{Recipient}", recipient)
                .Replace("{Sender}", UserInfo.FirstName + " " + UserInfo.LastName)
                .Replace("{SystemObject}", systemObject)
                .Replace("{Approve}", approveLink.ToString())
                .Replace("{Deny}", denyLink.ToString());
            userBusiness.SendEmail(email, template.Subject, emailBody);

            StatusTrackingEntity statusTrackingEntity = statusTrackingBusiness.GetExistingTracking(UserInfo.ID,
                userId, communityId, schoolId.Value);
            if (statusTrackingEntity == null)
            {
                statusTrackingEntity = new StatusTrackingEntity();
                statusTrackingEntity.RequestorId = UserInfo.ID;
                statusTrackingEntity.SupposedApproverId = userId;
                statusTrackingEntity.Status = StatusEnum.Pending;
                statusTrackingEntity.ExpiredTime = DateTime.Now.AddDays(Convert.ToInt32(SFConfig.ExpirationTime));
                statusTrackingEntity.CommunityId = communityId;
                statusTrackingEntity.SchoolId = schoolId.Value;
                statusTrackingEntity.CreatedBy = UserInfo.ID;
                statusTrackingEntity.Type = StatusType.Invitation;
                statusTrackingBusiness.AddTracking(statusTrackingEntity);
            }
            else
            {
                statusTrackingBusiness.Resend(statusTrackingEntity.ID, UserInfo.ID);
            }
            return JsonHelper.SerializeObject(new PostFormResponse()
            {
                Success = true
            });
        }

        [System.Web.Mvc.HttpPost]
        [ValidateAntiForgeryToken]
        [CLIUrlAuthorizeAttribute(Account = Authority.AssessmentEquipment, PageId = PagesModel.Teacher, Anonymity = Anonymous.Verified)]
        public string SaveEquipment(int teacherId, string[] serialNumber, string[] uTHealthTag,
            int[] chkEquipment, int[] isAssessmentEquipment)
        {
            var response = new PostFormResponse();
            if (isAssessmentEquipment[0] > 0)
            {
                OperationResult result = new OperationResult(OperationResultType.Success);
                result = userBusiness.SaveEquipment(teacherId, serialNumber, uTHealthTag, chkEquipment, isAssessmentEquipment);
                response.Success = result.ResultType == OperationResultType.Success;
                response.Message = result.Message;
            }
            response.ModelState = ModelState;
            return JsonHelper.SerializeObject(response);
        }

        [System.Web.Mvc.HttpPost]
        [ValidateAntiForgeryToken]
        [CLIUrlAuthorizeAttribute(Account = Authority.Transaction, PageId = PagesModel.Teacher, Anonymity = Anonymous.Verified)]
        public string SaveTransaction(TeacherTransactionEntity transaction)
        {
            transaction.Teacher = null;
            var response = new PostFormResponse();
            if (response.Success = ModelState.IsValid)
            {
                OperationResult result = new OperationResult(OperationResultType.Success);
                result = userBusiness.SaveTransaction(transaction);
                response.Success = result.ResultType == OperationResultType.Success;
                response.Message = result.Message;
            }
            response.ModelState = ModelState;
            return JsonHelper.SerializeObject(response);
        }

        [System.Web.Mvc.HttpPost]
        [ValidateAntiForgeryToken]
        [CLIUrlAuthorizeAttribute(Account = Authority.Assign, PageId = PagesModel.Teacher, Anonymity = Anonymous.Verified)]
        public string AssignClass(int teacherId, int[] chkClasses)
        {
            var response = new PostFormResponse();
            if (response.Success = ModelState.IsValid)
            {
                OperationResult result = new OperationResult(OperationResultType.Success);
                result = userBusiness.AssignClass(teacherId, chkClasses);
                response.Success = result.ResultType == OperationResultType.Success;
                response.Message = result.Message;
            }
            response.ModelState = ModelState;
            return JsonHelper.SerializeObject(response);
        }

        public string GetCoordCoachsByCommunityId(int communityId)
        {
            List<int> communityIds = new List<int>();
            communityIds.Add(communityId);
            var classList = userBusiness.GetCoordCoachs(communityIds)
                .ToSelectList(ViewTextHelper.DefaultPleaseSelectText, "0");
            return JsonHelper.SerializeObject(classList);
        }

        private void InitAccessOperation()
        {
            bool accessView = false;
            bool accessAdd = false;
            bool accessEdit = false;
            bool accessClass = false;
            bool accessPermission = false;
            bool accessTransaction = false;
            bool accessEquipment = false;
            bool accessAssignSchool = false;
            bool accessAssignCommunity = false;
            bool accessBES = false;
            if (UserInfo != null)
            {
                UserAuthorityModel userAuthority = new PermissionBusiness().GetUserAuthority(UserInfo,
                    (int)PagesModel.Teacher);

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
                    if ((userAuthority.Authority & (int)Authority.Assign) == (int)Authority.Assign)
                    {
                        accessClass = true;
                        accessPermission = true;
                    }
                    if ((userAuthority.Authority & (int)Authority.Transaction) == (int)Authority.Transaction)
                    {
                        accessTransaction = true;
                    }
                    if ((userAuthority.Authority & (int)Authority.AssessmentEquipment) ==
                        (int)Authority.AssessmentEquipment)
                    {
                        accessEquipment = true;
                    }
                    if ((userAuthority.Authority & (int)Authority.Bes) == (int)Authority.Bes)
                    {
                        accessBES = true;
                    }
                }
                if (UserInfo.IsCLIUser || UserInfo.Role == Role.Community || UserInfo.Role == Role.Statewide)
                {
                    accessAssignCommunity = true;
                }
                if (UserInfo.IsCLIUser || UserInfo.Role == Role.Community ||
                    UserInfo.Role == Role.District_Community_Delegate ||
                    UserInfo.Role == Role.District_Community_Specialist ||
                    UserInfo.Role == Role.Community_Specialist_Delegate ||
                    UserInfo.Role == Role.Statewide ||
                    UserInfo.Role == Role.Principal ||
                    UserInfo.Role == Role.Principal_Delegate ||
                    UserInfo.Role == Role.TRS_Specialist ||
                    UserInfo.Role == Role.TRS_Specialist_Delegate ||
                    UserInfo.Role == Role.School_Specialist ||
                    UserInfo.Role == Role.School_Specialist_Delegate)
                {
                    accessAssignSchool = true;
                    accessAssignCommunity = true;
                }
            }
            ViewBag.accessView = accessView;
            ViewBag.accessAdd = accessAdd;
            ViewBag.accessEdit = accessEdit;
            ViewBag.accessClass = accessClass;
            ViewBag.accessPermission = accessPermission;
            ViewBag.accessTransaction = accessTransaction;
            ViewBag.accessEquipment = accessEquipment;
            ViewBag.accessAssignCommunity = accessAssignCommunity;
            ViewBag.accessAssignSchool = accessAssignSchool;
            ViewBag.accessBES = accessBES;
        }

        #region BES
        [CLIUrlAuthorizeAttribute(Account = Authority.Bes, PageId = PagesModel.Teacher, Anonymity = Anonymous.Verified)]
        public ActionResult TeacherBES()
        {
            InitAccessOperation();
            ViewBag.StatusJson = JsonHelper.SerializeObject(EntityStatus.Active.ToList());
            ViewBag.PositionOptions = JsonHelper.SerializeObject(userBusiness.GetPositions((int)Role.Principal).ToSelectList(ViewTextHelper.DefaultPleaseSelectText, ""));
            //  ViewBag.GendersJson = JsonHelper.SerializeObject(Gender.Female.ToList());
            //   ViewBag.Language = JsonHelper.SerializeObject(userBusiness.GetLanguages().ToSelectList(ViewTextHelper.DefaultPleaseSelectText, "0"));
            //  ViewBag.PhoneTypeOptions = JsonHelper.SerializeObject(PhoneType.HomeNumber.ToList());
            // ViewBag.EthnicityOptions = JsonHelper.SerializeObject(Ethnicity.African_American.ToList());
            ViewBag.EmployedByOptions = JsonHelper.SerializeObject(EmployedBy.Child_Care.ToList());

            TeacherUserModel userModel = new TeacherUserModel();
            return View(userModel);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.Bes, PageId = PagesModel.Teacher, Anonymity = Anonymous.Verified)]
        public string BESSearch(string txtCommunity, string txtSchool, string teacherCode, string firstName, string lastName, int communityId = 0,
            int schoolId = 0, int status = -1, string sort = "LastName", string order = "Asc", int first = 0, int count = 10)
        {
            var total = 0;
            var expression = PredicateHelper.True<TeacherEntity>();
            if (communityId > 0)
                expression = expression.And(r => r.UserInfo.UserCommunitySchools.
                    Any(m => m.School.CommunitySchoolRelations.Any(n => n.CommunityId == communityId)));
            else if (txtCommunity.Trim() != string.Empty)
                expression = expression.And(r => r.UserInfo.UserCommunitySchools.
                    Any(m => m.School.CommunitySchoolRelations.Any(n => n.Community.Name.Contains(txtCommunity))));
            if (schoolId > 0)
                expression = expression.And(r => r.UserInfo.UserCommunitySchools.Any(e => e.SchoolId == schoolId));
            else if (txtSchool.Trim() != string.Empty)
                expression = expression.And(r => r.UserInfo.UserCommunitySchools.Any(e => e.School.Name.Contains(txtSchool)));
            if (status > 0)
                expression = expression.And(r => (int)r.UserInfo.Status == status);
            if (teacherCode.Trim() != string.Empty)
                expression = expression.And(r => r.TeacherId.Contains(teacherCode));
            if (firstName.Trim() != string.Empty)
                expression = expression.And(r => r.UserInfo.FirstName.Contains(firstName));
            if (lastName.Trim() != string.Empty)
                expression = expression.And(r => r.UserInfo.LastName.Contains(lastName));

            var list = userBusiness.SearchTeachersForBES(UserInfo, expression, sort, order, first, count, out total);
            var result = new { total = total, data = list };
            return JsonHelper.SerializeObject(result);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.Bes, PageId = PagesModel.Teacher, Anonymity = Anonymous.Verified)]
        public string BESSave(string Teachers, bool confirm = false)
        {
            var response = new PostFormResponse();
            OperationResult res = new OperationResult(OperationResultType.Success);
            List<TeacherUserModel> listTeachers = JsonHelper.DeserializeObject<List<TeacherUserModel>>(Teachers);
            if (!confirm)
            {
                if (CheckUsersList(listTeachers, response))
                    return JsonHelper.SerializeObject(response);
            }
            foreach (TeacherUserModel model in listTeachers)
            {
                TeacherEntity item = null;
                if (model.ID <= 0)
                {
                    item = new TeacherEntity();
                    item.UserInfo = new UserBaseEntity();
                    item.UserInfo.Role = Role.Teacher;
                    item.SchoolYear = CommonAgent.SchoolYear;
                }
                else
                {
                    item = userBusiness.GetTeacher(model.ID, UserInfo);
                }
                int schoolId = model.SchoolId;
                int communityId = model.CommunityId;

                item.UserInfo.FirstName = model.FirstName;
                item.UserInfo.MiddleName = model.MiddleName ?? "";
                item.UserInfo.LastName = model.LastName;
                item.UserInfo.PreviousLastName = "";
                item.BirthDate = CommonAgent.MinDate;
                item.Gender = (Gender)0;
                item.Ethnicity = (Ethnicity)0;
                //   item.VendorCode = model.VendorCode;
                item.PrimaryLanguageId = 0;
                item.SecondaryLanguageId = 0;
                item.EmployedBy = model.EmployedBy;
                //   item.CLIFundingID = model.CLIFundingID;
                // item.MediaRelease = model.MediaRelease;
                item.UserInfo.Status = model.Status;
                item.UserInfo.InternalID = model.TeacherNumber;
                // item.HomeMailingAddress = model.HomeMailingAddress;

                item.UserInfo.PrimaryPhoneNumber = model.PrimaryPhoneNumber;
                //  item.UserInfo.PrimaryNumberType = model.PrimaryNumberType;
                //  item.UserInfo.SecondaryPhoneNumber = model.SecondPhoneNumber;
                //     item.UserInfo.SecondaryNumberType = model.SecondNumberType;
                item.UserInfo.FaxNumber = model.FaxNumber;
                item.UserInfo.PrimaryEmailAddress = model.PrimaryEmail;


                if (item.ID <= 0)
                {
                    res = userBusiness.InsertTeacher(item, new int[0], new int[0], new List<int>(), new int[0], communityId, schoolId, UserInfo.ID);
                }
                else
                {
                    res = userBusiness.UpdateTeacher(item, new int[0], new int[0], new List<int>(), new int[0], UserInfo.Role);
                }
                if (res.ResultType == OperationResultType.Error)
                    break;
                if (model.IsInvitation)
                {
                    try
                    {
                        SendInvitation(item);
                    }
                    catch (Exception ex)
                    {
                        res.ResultType = OperationResultType.Error;
                        res.Message = ex.ToString();
                        break;
                    }
                }
            }
            response.Success = res.ResultType == OperationResultType.Success;
            response.Message = res.Message;
            return JsonHelper.SerializeObject(response);
        }

        private void SendInvitation(TeacherEntity teacher)
        {
            EmailTemplete template = XmlHelper.GetEmailTemplete("TeacherInvitation_Template.xml");
            string param = teacher.UserInfo.ID.ToString() + "," + DateTime.Now.ToString();
            string encryptParam = ObjectFactory.GetInstance<IEncrypt>().Encrypt(param);
            string link = SFConfig.MainSiteDomain + "Home/InviteVerification/" + System.Web.HttpContext.Current.Server.UrlEncode(encryptParam);
            string emailBody = template.Body.Replace("{FirstName}", teacher.UserInfo.FirstName)
            .Replace("{LastName}", teacher.UserInfo.LastName)
            .Replace("{InviteLink}", "<a style='text-decoration: underline; cursor:pointer; color: #008000;' href='" + link + "'>Click here</a>")
            .Replace("{StaticDomain}", SFConfig.StaticDomain)
            .Replace("{CliEngageUrl}", SFConfig.MainSiteDomain);
            userBusiness.SendEmail(teacher.UserInfo.PrimaryEmailAddress, template.Subject, emailBody);
            EmailLogEntity emailLog = new EmailLogEntity(teacher.UserInfo.ID, teacher.UserInfo.PrimaryEmailAddress, EmailLogType.Invitation);
            userBusiness.InsertEmailLog(emailLog);
            operationLogBusiness.InsertLog(OperationEnum.ResetSendInvitation, teacher.UserInfo.Role.ToDescription(), "Send Invitation",
                                             CommonHelper.GetIPAddress(Request), UserInfo);
        }

        private bool CheckUsersList(List<TeacherUserModel> listTeachers, PostFormResponse response)
        {
            var userList = listTeachers.Where(t => t.ID == 0).GroupBy(o => new { o.FirstName, o.LastName, o.PrimaryEmail }).ToList();
            userList = userList.Where(o => o.Count() > 1).ToList();

            TeacherEntity teacher = new TeacherEntity();
            teacher.UserInfo = new UserBaseEntity();

            if (userList.Count > 0)
            {
                teacher.UserInfo.Role = Role.Teacher;

                teacher.UserInfo.FirstName = userList[0].Key.FirstName;
                teacher.UserInfo.LastName = userList[0].Key.LastName;
                teacher.UserInfo.PrimaryEmailAddress = userList[0].Key.PrimaryEmail;
                if (CheckExisted3(teacher, UserExistedStatus.UserExisted, 0, response))
                {
                    return true;
                }
            }
            foreach (TeacherUserModel model in listTeachers)
            {
                TeacherEntity item = null;
                if (model.ID <= 0)
                {
                    item = new TeacherEntity();
                    item.UserInfo = new UserBaseEntity();
                    item.UserInfo.Role = Role.Teacher;
                }
                else
                {
                    item = userBusiness.GetTeacher(model.ID, UserInfo);
                }
                int communityId = model.CommunityId;
                int schoolId = model.SchoolId;
                item.UserInfo.FirstName = model.FirstName;
                item.UserInfo.LastName = model.LastName;
                item.UserInfo.Status = model.Status;
                item.UserInfo.PrimaryEmailAddress = model.PrimaryEmail;

                if (CheckExisted2(item, communityId, schoolId, response))
                {
                    return true;
                }
            }
            return false;
        }

        private bool CheckExisted2(TeacherEntity teacher, int communityId, int schoolId, PostFormResponse response)
        {
            teacher.UserInfo.Role = Role.Teacher;
            var existedUserId = 0;
            Role existedUserRole;
            var existedStatus = userBusiness.CheckTeacherUserExistedStatus(teacher.UserInfo.ID, teacher.UserInfo.FirstName,
                teacher.UserInfo.LastName, teacher.UserInfo.PrimaryEmailAddress, teacher.UserInfo.Role,
                communityId, schoolId, out existedUserId, out existedUserRole);

            if (existedStatus == UserExistedStatus.ExistedInSchool)
            {
                SchoolEntity school = schoolBusiness.GetSchool(schoolId);
                // do nothing
                response.Success = true;
                response.Message = "More than one teacher with the same first name, last name, and primary email already exists in CLI Engage. "
                    + teacher.UserInfo.FirstName + " " + teacher.UserInfo.LastName + " " + " is currently active at " +
                  school.Name;
                response.Data = "waiting";
                return true;
            }
            else if (existedStatus == UserExistedStatus.ExistedInCommunity)
            {
                CommunityEntity community = communityBusiness.GetCommunity(communityId);
                response.Success = true;
                response.Message = "More than one teacher with the same first name, last name, and primary email already exists in CLI Engage. "
                    + teacher.UserInfo.FirstName + " " + teacher.UserInfo.LastName + " " + " is currently active at " +
                  community.Name;

                response.Data = "waiting";
                return true;
            }
            else if (existedStatus == UserExistedStatus.UserExisted)
            {
                if (existedUserRole == Role.Teacher)
                {
                    response.Success = true;
                    response.Message = "More than one teacher with the same first name, last name, and primary email already exists in Cli Engage: "
                    + teacher.UserInfo.FirstName + " " + teacher.UserInfo.LastName + " " + teacher.UserInfo.PrimaryEmailAddress;
                    response.Data = "waiting";
                    return true;
                }
                else
                {
                    // 存在其他角色的用户提醒 Continue
                    response.Success = true;
                    response.Message = "More than one user with the same first name, last name, and primary email already exists in Cli Engage: "
                    + teacher.UserInfo.FirstName + " " +
                    teacher.UserInfo.LastName
                    + " (" + existedUserRole.ToDescription() + ") " + teacher.UserInfo.PrimaryEmailAddress + ", Continue?";

                    response.Data = new
                    {
                        type = "continue"
                    };
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 判断当前页面中有可能会重复的记录
        /// </summary>
        /// <param name="teacher"></param>
        /// <param name="existedStatus"></param>
        /// <param name="schoolId"></param>
        /// <param name="response"></param>
        /// <returns></returns>
        private bool CheckExisted3(TeacherEntity teacher, UserExistedStatus existedStatus, int schoolId, PostFormResponse response)
        {
            if (existedStatus == UserExistedStatus.UserExisted)
            {
                // 已存在Teacher 角色的并且FirstName,LastName,Email相同的记录,但是操作者没有权限(意外勾选了不属于操作者的权限)
                response.Success = true;
                response.Message = "More than one teacher with the same first name, last name, and primary email already exists in this list: "
                     + teacher.UserInfo.FirstName + " " + teacher.UserInfo.LastName + " " + teacher.UserInfo.PrimaryEmailAddress;
                response.Data = "waiting"; ;
                response.Data = "waiting";
                return true;
            }
            return false;
        }

        #endregion
    }
}