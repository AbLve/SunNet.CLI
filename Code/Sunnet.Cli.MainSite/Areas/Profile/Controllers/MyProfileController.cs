using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

/**************************************************************************
 * Developer: 		Joe
 * Computer:		JOE-PC
 * Domain:			Joe-pc
 * CreatedOn:		2014/9/25   12:15:10
 * Description:		Please input class summary
 * Version History:	Created,2014/9/25 12:15:10
 * 
 * 
 **************************************************************************/
using Sunnet.Cli.MainSite.Controllers;
using Sunnet.Cli.Business.Users;
using Sunnet.Cli.Core.Users.Enums;
using Sunnet.Cli.Core.Users.Entities;
using Sunnet.Framework.Extensions;
using Sunnet.Cli.UIBase.Models;
using Sunnet.Framework.Core.Tool;
using Sunnet.Cli.Business.Common;
using Sunnet.Cli.Business.Permission;
using Sunnet.Cli.Business.Permission.Models;
using Sunnet.Cli.MainSite.Areas.Invitation.Controllers;
using Sunnet.Cli.Core.Common.Enums;
using Sunnet.Cli.MainSite.Areas.School.Controllers;
using Sunnet.Cli.Business.Schools.Models;
using Sunnet.Cli.Business.Schools;
using Sunnet.Cli.UIBase.Filters;
using Sunnet.Framework.Permission;
using Sunnet.Cli.Business.Students;

namespace Sunnet.Cli.MainSite.Areas.Profile.Controllers
{
    public class MyProfileController : BaseController
    {
        private readonly UserBusiness userBusiness;
        private readonly PermissionBusiness permissionBusiness;
        private readonly SchoolController schoolController;
        private readonly SchoolBusiness schoolBusiness;
        private readonly StudentBusiness studentBusiness;

        public MyProfileController()
        {
            userBusiness = new UserBusiness(UnitWorkContext);
            permissionBusiness = new PermissionBusiness(UnitWorkContext);
            schoolBusiness = new SchoolBusiness(UnitWorkContext);
            schoolController = new SchoolController();
            studentBusiness = new StudentBusiness(UnitWorkContext);
        }
        //
        // GET: /Profile/Profile/
        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.Administrative, Anonymity = Anonymous.Logined)]
        public ActionResult Index(int first = 0)
        {
            bool isFirst = false;
            if (first == 1)  //第一次进入profile时，更改后跳转到Dashbord页面
                isFirst = true;

            switch (UserInfo.Role)
            {
                //内部用户
                case Role.Super_admin:
                case Role.Content_personnel:
                case Role.Statisticians:
                case Role.Administrative_personnel:
                case Role.Intervention_manager:
                case Role.Intervention_support_personnel:
                    return View("EditInternalUser", GetInternalUser(UserInfo));

                case Role.Video_coding_analyst:
                    return View("VideoCodingEdit", GetVideoCoding(UserInfo));

                case Role.Mentor_coach:
                case Role.Coordinator:
                    return View("CoordCoachEdit", GetCoordCoach(UserInfo));

                //外部用户

                case Role.Auditor:
                    return View("AuditorEdit", GetAuditor(UserInfo, isFirst));

                case Role.Statewide:
                    return View("StatewideEdit", GetStatewide(UserInfo, isFirst));

                case Role.Community:
                case Role.District_Community_Delegate:
                    return View("CommunityEdit", GetCommunity(UserInfo, isFirst));

                case Role.District_Community_Specialist:
                case Role.Community_Specialist_Delegate:
                    return View("CommunitySpecialistEdit", GetCommunitySpecialist(UserInfo, isFirst));

                case Role.Principal:
                case Role.Principal_Delegate:
                    return View("PrincipalEdit", GetPrincipal(UserInfo, isFirst));

                case Role.TRS_Specialist:
                case Role.TRS_Specialist_Delegate:
                    return View("TRSSpecialistEdit", GetTRSSpecialist(UserInfo, isFirst));

                case Role.School_Specialist:
                case Role.School_Specialist_Delegate:
                    return View("SchoolSpecialistEdit", GetSchoolSpecialist(UserInfo, isFirst));

                case Role.Teacher:
                    return View("TeacherEdit", GetTeacher(UserInfo, isFirst));

                case Role.Parent:
                    ViewBag.SettingReadOnly = false;
                    return View("ParentEdit", GetParent(UserInfo, isFirst));

                default:
                    return View();
            }
        }

        //内部角色

        #region  其他内部角色

        /// <summary>
        /// 其他内部角色编辑页面
        /// </summary>
        /// <param name="userinfo"></param>
        /// <returns></returns>
        private UserBaseEntity GetInternalUser(UserBaseEntity userinfo)
        {
            UserBaseEntity user = userBusiness.GetUser(userinfo.ID);
            return user;
        }

        /// <summary>
        /// 其他内部角色编辑操作
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [System.Web.Mvc.HttpPost]
        [ValidateAntiForgeryToken]
        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.Administrative, Anonymity = Anonymous.Logined)]
        public string SaveInternalUser(UserBaseEntity user)
        {
            var response = new PostFormResponse();
            if (response.Success = ModelState.IsValid)
            {
                OperationResult result = new OperationResult(OperationResultType.Success);
                if (userBusiness.IsExistsUserName(user.ID, user.GoogleId.Trim()))
                {
                    result.ResultType = OperationResultType.Warning;
                    result.Message = "User Name already exists";
                }
                else
                {
                    if (user.ID > 0)
                        result = userBusiness.UpdateUser(user);
                    else
                    {
                        user.StatusDate = DateTime.Now;
                        result = userBusiness.InsertUser(user);
                    }
                }
                response.Success = result.ResultType == OperationResultType.Success;
                response.Data = user;
                response.Message = result.Message;
            }
            response.ModelState = ModelState;
            return JsonHelper.SerializeObject(response);
        }

        #endregion

        #region Video_coding_analyst
        /// <summary>
        /// Video_coding_analyst 角色编辑页面
        /// </summary>
        /// <param name="userinfo"></param>
        /// <returns></returns>
        private UserBaseEntity GetVideoCoding(UserBaseEntity userinfo)
        {
            ViewBag.LanguagePrimaryOptions = userBusiness.GetLanguages().ToSelectList(ViewTextHelper.DefaultPleaseSelectText, "0");
            ViewBag.LanguageOptions = userBusiness.GetLanguages().ToSelectList(ViewTextHelper.DefaultPleaseSelectText, "0");
            UserBaseEntity user = userBusiness.GetUser(userinfo.ID);
            return user;
        }

        /// <summary>
        /// Video_coding_analyst 角色编辑操作
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.Administrative, Anonymity = Anonymous.Logined)]
        public string SaveVideoCoding(UserBaseEntity user)
        {
            var response = new PostFormResponse();
            if (response.Success = ModelState.IsValid)
            {
                OperationResult result = new OperationResult(OperationResultType.Success);
                if (userBusiness.IsExistsUserName(user.ID, user.GoogleId.Trim()))
                {
                    result.ResultType = OperationResultType.Warning;
                    result.Message = "User Name already exists";
                }
                else
                {
                    if (user.ID > 0)
                        result = userBusiness.UpdateVideoCoding(user);
                    else
                    {
                        user.StatusDate = DateTime.Now;
                        result = userBusiness.InsertUser(user);
                    }
                }
                response.Success = result.ResultType == OperationResultType.Success;
                response.Message = result.Message;
            }
            response.ModelState = ModelState;
            return JsonHelper.SerializeObject(response);
        }

        #endregion

        #region Mentor_coach、Coordinator

        /// <summary>
        /// Mentor_coach、Coordinator 角色编辑页面
        /// </summary>
        /// <param name="userinfo"></param>
        /// <returns></returns>
        private CoordCoachEntity GetCoordCoach(UserBaseEntity userinfo)
        {
            UserBaseEntity user = userBusiness.GetUser(userinfo.ID);
            CoordCoachEntity coordCoachEntity = new CoordCoachEntity();
            coordCoachEntity = user.CoordCoach;
            string certificateText = "";
            foreach (var item in coordCoachEntity.User.Certificates)
            {
                certificateText += item.ID + ",";
            }
            ViewBag.PMName = userBusiness.GetPMByCoordCoach(user);
            ViewBag.CertificateText = certificateText;
            ViewBag.ProjectManagerOptions = userBusiness.GetProjectManagers().ToSelectList(ViewTextHelper.DefaultPleaseSelectText, "0");
            ViewBag.Language = new UserBusiness().GetLanguages().ToSelectList(ViewTextHelper.DefaultPleaseSelectText, "0");
            ViewBag.Funding = new UserBusiness().GetFundings().ToSelectList(ViewTextHelper.DefaultPleaseSelectText, "0");
            ViewBag.State = new UserBusiness().GetStates().ToSelectList(ViewTextHelper.DefaultStateText, "");
            ViewBag.County = new UserBusiness().GetCountries().ToSelectList(ViewTextHelper.DefaultCountyText, "");
            ViewBag.CertificateList = new UserBusiness().GetCertificates();
            return coordCoachEntity;
        }


        /// <summary>
        /// Mentor_coach、Coordinator 角色编辑操作
        /// </summary>
        /// <param name="coordCoach"></param>
        /// <param name="certificates"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.Administrative, Anonymity = Anonymous.Logined)]
        public string SaveCoordCoach(CoordCoachEntity coordCoach, string certificates)
        {
            var response = new PostFormResponse();
            List<int> certificatesList = new List<int>();
            if (!certificates.IsNullOrEmpty())
            {
                foreach (var item in certificates.TrimEnd(',').Split(','))
                {
                    certificatesList.Add(Convert.ToInt32(item));
                }
            }
            if (response.Success = ModelState.IsValid)
            {
                OperationResult result = new OperationResult(OperationResultType.Success);
                if (userBusiness.IsExistsUserName(coordCoach.User.ID, coordCoach.User.GoogleId.Trim()))
                {
                    result.ResultType = OperationResultType.Warning;
                    result.Message = "User Name already exists";
                }
                else
                {
                    if (coordCoach.ID > 0)
                    {
                        UserBaseEntity user = new UserBaseEntity();
                        user = coordCoach.User;
                        user.CoordCoach = coordCoach;
                        result = userBusiness.UpdateCoordCoach(user, certificatesList);
                    }
                    else
                    {
                        coordCoach.User.StatusDate = DateTime.Now;
                        coordCoach.SchoolYear = CommonAgent.SchoolYear;
                        result = userBusiness.InsertCoordCoach(coordCoach, certificatesList);
                    }
                }
                response.Success = result.ResultType == OperationResultType.Success;
                response.Message = result.Message;
            }
            response.ModelState = ModelState;
            return JsonHelper.SerializeObject(response);
        }

        #endregion

        //外部角色

        #region Auditor

        private AuditorEntity GetAuditor(UserBaseEntity userinfo, bool isFirst)
        {
            AuditorEntity auditorEntity = userBusiness.GetAuditor(userinfo.Auditor.ID);
            ViewBag.PositionOptions = userBusiness.GetPositions((int)Role.Auditor).
                ToSelectList(ViewTextHelper.DefaultPleaseSelectText, "");
            ViewBag.AffiliationOptions = userBusiness.GetAffiliations().
                ToSelectList(ViewTextHelper.DefaultPleaseSelectText, "");
            ViewBag.GroupPackages = permissionBusiness.GetCustomPackages(Role.Auditor);
            ViewBag.GroupPackageSelected = auditorEntity.UserInfo.PermissionRoles.Select(e =>
                new GroupPackageModel()
                {
                    PackageId = e.ID,
                    PackageName = e.Name,
                    PackageDescription = e.Descriptions
                }).ToList();
            ViewBag.IsFirst = isFirst;
            return auditorEntity;
        }



        [System.Web.Mvc.HttpPost]
        [ValidateAntiForgeryToken]
        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.Administrative, Anonymity = Anonymous.Logined)]
        public string SaveAuditor(AuditorEntity auditor)
        {
            var response = new PostFormResponse();
            if (response.Success = ModelState.IsValid)
            {
                OperationResult result = new OperationResult(OperationResultType.Success);
                if (auditor.ID > 0)
                    result = userBusiness.UpdateAuditor(auditor);

                response.Success = result.ResultType == OperationResultType.Success;
                response.Message = result.Message;
            }
            response.ModelState = ModelState;
            return JsonHelper.SerializeObject(response);
        }
        #endregion

        #region Statewide

        private StateWideEntity GetStatewide(UserBaseEntity userinfo, bool isFirst)
        {
            StateWideEntity stateWideEntity = userBusiness.GetStateWide(userinfo.StateWide.ID);
            ViewBag.GroupPackages = permissionBusiness.GetCustomPackages(Role.Statewide);
            ViewBag.GroupPackageSelected = stateWideEntity.UserInfo.PermissionRoles.Select(e =>
                new GroupPackageModel()
                {
                    PackageId = e.ID,
                    PackageName = e.Name,
                    PackageDescription = e.Descriptions
                }).ToList();
            ViewBag.PositionOptions = userBusiness.GetPositions((int)Role.Statewide).
                ToSelectList(ViewTextHelper.DefaultPleaseSelectText, "");
            ViewBag.IsFirst = isFirst;
            return stateWideEntity;
        }


        [System.Web.Mvc.HttpPost]
        [ValidateAntiForgeryToken]
        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.Administrative, Anonymity = Anonymous.Logined)]
        public string SaveStatewide(StateWideEntity stateWide, bool confirm = false)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            var response = new PostFormResponse();
            if (confirm == false)
            {
                if (userBusiness.CheckUserExistedStatus(stateWide.UserInfo.ID,
                    stateWide.UserInfo.FirstName,
                    stateWide.UserInfo.LastName, stateWide.UserInfo.PrimaryEmailAddress,
                    stateWide.UserInfo.Role, out result))
                {
                    response.Success = true;
                    response.Message = result.Message;
                    response.Data = result.AppendData;
                    return JsonHelper.SerializeObject(response);
                }
            }
            if (response.Success = ModelState.IsValid)
            {
                if (stateWide.ID > 0)
                    result = userBusiness.UpdateStateWide(stateWide);
                response.Success = result.ResultType == OperationResultType.Success;
                response.Message = result.Message;
            }
            response.ModelState = ModelState;
            return JsonHelper.SerializeObject(response);
        }

        #endregion

        #region Community 、District_Community_Delegate

        private CommunityUserEntity GetCommunity(UserBaseEntity userinfo, bool isFirst)
        {
            CommunityUserEntity communityUserEntity = userBusiness.GetCommunity(userinfo.CommunityUser.ID, UserInfo, userinfo.Role, true);
            string certificateText = "";
            foreach (var item in communityUserEntity.UserInfo.Certificates)
            {
                certificateText += item.ID + ",";
            }
            ViewBag.CertificateText = certificateText;
            ViewBag.Language = new UserBusiness().GetLanguages().ToSelectList(ViewTextHelper.DefaultPleaseSelectText, "0");
            ViewBag.State = new UserBusiness().GetStates().ToSelectList(ViewTextHelper.DefaultStateText, "");
            ViewBag.County = new UserBusiness().GetCountries().ToSelectList(ViewTextHelper.DefaultCountyText, "");
            ViewBag.PositionOptions = userBusiness.GetPositions((int)Role.Community).ToSelectList(ViewTextHelper.DefaultPleaseSelectText, "");
            ViewBag.CertificateList = new UserBusiness().GetCertificates();

            ViewBag.RoleType = userinfo.Role;
            ViewBag.GroupPackages = permissionBusiness.GetAssignedPackages(
                (int)Role.Community, null,
                communityUserEntity.UserInfo.UserCommunitySchools.Select(e => e.CommunityId).ToList());
            ViewBag.GroupPackageSelected = communityUserEntity.UserInfo.PermissionRoles.Select(e =>
               new GroupPackageModel()
               {
                   PackageId = e.ID,
                   PackageName = e.Name,
                   PackageDescription = e.Descriptions
               }).ToList();
            ViewBag.IsFirst = isFirst;
            if ((Role) userinfo.Role == Role.District_Community_Delegate)
            {
                UserBaseEntity parentUser = userBusiness.GetUser(userinfo.CommunityUser.ParentId);
                ViewBag.ParentCommunityNames =
                    parentUser.UserCommunitySchools.Where(e => e.CommunityId > 0)
                        .Select(x => x.Community.Name)
                        .ToArray();
            }
            return communityUserEntity;
        }


        [System.Web.Mvc.HttpPost]
        [ValidateAntiForgeryToken]
        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.Administrative, Anonymity = Anonymous.Logined)]
        public string SaveCommunity(CommunityUserEntity communityUser, string certificates, bool confirm = false)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            var response = new PostFormResponse();
            if (confirm == false)
            {
                if (userBusiness.CheckUserExistedStatus(communityUser.UserInfo.ID,
                    communityUser.UserInfo.FirstName,
                    communityUser.UserInfo.LastName, communityUser.UserInfo.PrimaryEmailAddress,
                    communityUser.UserInfo.Role, out result))
                {
                    response.Success = true;
                    response.Message = result.Message;
                    response.Data = result.AppendData;
                    return JsonHelper.SerializeObject(response);
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
            if (response.Success = ModelState.IsValid)
            {
                if (communityUser.ID > 0)
                    result = userBusiness.UpdateCommunityUser(communityUser, certificatesList);

                response.Success = result.ResultType == OperationResultType.Success;
                response.Message = result.Message;
            }
            response.ModelState = ModelState;
            return JsonHelper.SerializeObject(response);
        }

        #endregion

        #region Community_Specialist  、  Community_Specialist_Delegate


        private CommunityUserEntity GetCommunitySpecialist(UserBaseEntity userinfo, bool isFirst)
        {
            new CommunitySpecialistController().InternalAccess();
            CommunityUserEntity communityUserEntity = userBusiness.GetCommunity(userinfo.CommunityUser.ID, UserInfo, userinfo.Role, false);
            string certificateText = "";
            foreach (var item in communityUserEntity.UserInfo.Certificates)
            {
                certificateText += item.ID + ",";
            }
            ViewBag.CertificateText = certificateText;
            ViewBag.Language = new UserBusiness().GetLanguages().ToSelectList(ViewTextHelper.DefaultPleaseSelectText, "0");
            ViewBag.State = new UserBusiness().GetStates().ToSelectList(ViewTextHelper.DefaultStateText, "");
            ViewBag.County = new UserBusiness().GetCountries().ToSelectList(ViewTextHelper.DefaultCountyText, "");
            ViewBag.PositionOptions = userBusiness.GetPositions((int)Role.District_Community_Specialist).ToSelectList(ViewTextHelper.DefaultPleaseSelectText, "");
            ViewBag.CertificateList = new UserBusiness().GetCertificates();

            ViewBag.GroupPackages = permissionBusiness.GetAssignedPackages(
                (int)Role.District_Community_Specialist, null,
                communityUserEntity.UserInfo.UserCommunitySchools.Select(e => e.CommunityId).ToList());
            ViewBag.GroupPackageSelected = communityUserEntity.UserInfo.PermissionRoles.Select(e =>
               new GroupPackageModel()
               {
                   PackageId = e.ID,
                   PackageName = e.Name,
                   PackageDescription = e.Descriptions
               }).ToList();
            ViewBag.RoleType = userinfo.Role;
            ViewBag.IsFirst = isFirst;
            if ((Role) userinfo.Role == Role.Community_Specialist_Delegate)
            {
                UserBaseEntity parentUser = userBusiness.GetUser(userinfo.CommunityUser.ParentId);
                ViewBag.ParentCommunityNames =
                    parentUser.UserCommunitySchools.Where(e => e.CommunityId > 0)
                        .Select(x => x.Community.Name)
                        .ToArray();
            }
            return communityUserEntity;
        }


        [System.Web.Mvc.HttpPost]
        [ValidateAntiForgeryToken]
        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.Administrative, Anonymity = Anonymous.Logined)]
        public string SaveCommunitySpecialist(CommunityUserEntity communityUser, string certificates, bool confirm = false)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            var response = new PostFormResponse();
            if (confirm == false)
            {
                if (userBusiness.CheckUserExistedStatus(communityUser.UserInfo.ID,
                    communityUser.UserInfo.FirstName,
                    communityUser.UserInfo.LastName, communityUser.UserInfo.PrimaryEmailAddress,
                    communityUser.UserInfo.Role, out result))
                {
                    response.Success = true;
                    response.Message = result.Message;
                    response.Data = result.AppendData;
                    return JsonHelper.SerializeObject(response);
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

            if (response.Success = ModelState.IsValid)
            {
                if (communityUser.ID > 0)
                    result = userBusiness.UpdateCommunityUser(communityUser, certificatesList);
                response.Success = result.ResultType == OperationResultType.Success;
                response.Message = result.Message;
            }
            response.ModelState = ModelState;
            return JsonHelper.SerializeObject(response);
        }

        #endregion

        #region Principal 、 Principal_Delegate

        private PrincipalEntity GetPrincipal(UserBaseEntity userinfo, bool isFirst)
        {
            PrincipalEntity principalEntity = userBusiness.GetPrincipal(userinfo.Principal.ID, UserInfo, userinfo.Role);
            string certificateText = "";
            foreach (var item in principalEntity.UserInfo.Certificates)
            {
                certificateText += item.ID + ",";
            }
            ViewBag.CertificateText = certificateText;
            ViewBag.Language = new UserBusiness().GetLanguages().ToSelectList(ViewTextHelper.DefaultPleaseSelectText, "0");
            ViewBag.State = new UserBusiness().GetStates().ToSelectList(ViewTextHelper.DefaultStateText, "");
            ViewBag.County = new UserBusiness().GetCountries().ToSelectList(ViewTextHelper.DefaultCountyText, "");
            ViewBag.PositionOptions = userBusiness.GetPositions((int)Role.Principal).
                ToSelectList(ViewTextHelper.DefaultPleaseSelectText, "");
            ViewBag.PDList = new UserBusiness().GetPDs();
            ViewBag.CertificateList = new UserBusiness().GetCertificates();

            ViewBag.RoleType = userinfo.Role;
            ViewBag.GroupPackages = permissionBusiness.GetAssignedPackages(
                (int)Role.Principal, principalEntity.UserInfo.UserCommunitySchools.Select(x => x.SchoolId).ToList(),
                principalEntity.UserInfo.UserCommunitySchools.Select(x => x.CommunityId).ToList());
            ViewBag.GroupPackageSelected = principalEntity.UserInfo.PermissionRoles.Select(e =>
               new GroupPackageModel()
               {
                   PackageId = e.ID,
                   PackageName = e.Name,
                   PackageDescription = e.Descriptions
               }).ToList();
            ViewBag.IsFirst = isFirst;
            if ((Role) userinfo.Role == Role.Principal_Delegate)
            {
                UserBaseEntity parentUser = userBusiness.GetUser(userinfo.Principal.ParentId);
                ViewBag.ParentSchoolNames = parentUser.UserCommunitySchools.Select(x => x.School.Name).ToArray();
            }
            return principalEntity;
        }


        [System.Web.Mvc.HttpPost]
        [ValidateAntiForgeryToken]
        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.Administrative, Anonymity = Anonymous.Logined)]
        public string SavePrincipal(PrincipalEntity principal, int[] chkPDs, string certificates, bool confirm = false)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            PostFormResponse response = new PostFormResponse();
            if (confirm == false && principal.UserInfo.Role == Role.Principal)
            {
                if (userBusiness.CheckUserExistedStatus(principal.UserInfo.ID,
                    principal.UserInfo.FirstName,
                    principal.UserInfo.LastName, principal.UserInfo.PrimaryEmailAddress,
                    principal.UserInfo.Role, out result))
                {
                    response.Success = true;
                    response.Message = result.Message;
                    response.Data = result.AppendData;
                    return JsonHelper.SerializeObject(response);
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
            if (response.Success = ModelState.IsValid)
            {
                if (principal.ID > 0)
                    result = userBusiness.UpdatePrincipal(principal, chkPDs, certificatesList);

                response.Success = result.ResultType == OperationResultType.Success;
                response.Message = result.Message;
            }
            response.ModelState = ModelState;
            return JsonHelper.SerializeObject(response);
        }

        #endregion


        #region TRS_Specialist 、  TRS_Specialist_Delegate

        private PrincipalEntity GetTRSSpecialist(UserBaseEntity userinfo, bool isFirst)
        {
            new TRSSpecialistController().InternalAccess();
            PrincipalEntity principalEntity = userBusiness.GetPrincipal(userinfo.Principal.ID, UserInfo, userinfo.Role);
            string certificateText = "";
            foreach (var item in principalEntity.UserInfo.Certificates)
            {
                certificateText += item.ID + ",";
            }
            ViewBag.CertificateText = certificateText;
            ViewBag.Language = new UserBusiness().GetLanguages().ToSelectList(ViewTextHelper.DefaultPleaseSelectText, "0");
            ViewBag.State = new UserBusiness().GetStates().ToSelectList(ViewTextHelper.DefaultStateText, "");
            ViewBag.County = new UserBusiness().GetCountries().ToSelectList(ViewTextHelper.DefaultCountyText, "");
            ViewBag.PositionOptions = userBusiness.GetPositions((int)Role.TRS_Specialist).
                ToSelectList(ViewTextHelper.DefaultPleaseSelectText, "");
            ViewBag.PDList = new UserBusiness().GetPDs();
            ViewBag.CertificateList = new UserBusiness().GetCertificates();

            ViewBag.RoleType = userinfo.Role;

            ViewBag.GroupPackages = permissionBusiness.GetAssignedPackages(
                (int)Role.TRS_Specialist, principalEntity.UserInfo.UserCommunitySchools.Select(x => x.SchoolId).ToList(),
                principalEntity.UserInfo.UserCommunitySchools.Select(x => x.CommunityId).ToList());
            ViewBag.GroupPackageSelected = principalEntity.UserInfo.PermissionRoles.Select(e =>
               new GroupPackageModel()
               {
                   PackageId = e.ID,
                   PackageName = e.Name,
                   PackageDescription = e.Descriptions
               }).ToList();
            ViewBag.IsFirst = isFirst;
            if ((Role) userinfo.Role == Role.TRS_Specialist_Delegate)
            {
                UserBaseEntity parentUser = userBusiness.GetUser(userinfo.Principal.ParentId);
                ViewBag.ParentSchoolNames = parentUser.UserCommunitySchools.Select(x => x.School.Name).ToArray();
            }
            return principalEntity;
        }

        [System.Web.Mvc.HttpPost]
        [ValidateAntiForgeryToken]
        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.Administrative, Anonymity = Anonymous.Logined)]
        public string SaveTRSSpecialist(PrincipalEntity principal, int[] chkPDs, string certificates, bool confirm = false)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            PostFormResponse response = new PostFormResponse();
            if (confirm == false && principal.UserInfo.Role == Role.Principal)
            {
                if (userBusiness.CheckUserExistedStatus(principal.UserInfo.ID,
                    principal.UserInfo.FirstName,
                    principal.UserInfo.LastName, principal.UserInfo.PrimaryEmailAddress,
                    principal.UserInfo.Role, out result))
                {
                    response.Success = true;
                    response.Message = result.Message;
                    response.Data = result.AppendData;
                    return JsonHelper.SerializeObject(response);
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
            if (response.Success = ModelState.IsValid)
            {
                if (principal.ID > 0)
                    result = userBusiness.UpdatePrincipal(principal, chkPDs, certificatesList);
                response.Success = result.ResultType == OperationResultType.Success;
                response.Message = result.Message;
            }
            response.ModelState = ModelState;
            return JsonHelper.SerializeObject(response);
        }

        #endregion

        #region School_Specialist 、  School_Specialist_Delegate

        private PrincipalEntity GetSchoolSpecialist(UserBaseEntity userinfo, bool isFirst)
        {
            new TRSSpecialistController().InternalAccess();
            PrincipalEntity principalEntity = userBusiness.GetPrincipal(userinfo.Principal.ID, UserInfo, userinfo.Role);
            string certificateText = "";
            foreach (var item in principalEntity.UserInfo.Certificates)
            {
                certificateText += item.ID + ",";
            }
            ViewBag.CertificateText = certificateText;
            ViewBag.Language = new UserBusiness().GetLanguages().ToSelectList(ViewTextHelper.DefaultPleaseSelectText, "0");
            ViewBag.State = new UserBusiness().GetStates().ToSelectList(ViewTextHelper.DefaultStateText, "");
            ViewBag.County = new UserBusiness().GetCountries().ToSelectList(ViewTextHelper.DefaultCountyText, "");
            ViewBag.PositionOptions = userBusiness.GetPositions((int)Role.School_Specialist).
                ToSelectList(ViewTextHelper.DefaultPleaseSelectText, "");
            ViewBag.PDList = new UserBusiness().GetPDs();
            ViewBag.CertificateList = new UserBusiness().GetCertificates();

            ViewBag.RoleType = userinfo.Role;

            ViewBag.GroupPackages = permissionBusiness.GetAssignedPackages(
                (int)Role.School_Specialist, principalEntity.UserInfo.UserCommunitySchools.Select(x => x.SchoolId).ToList(),
                principalEntity.UserInfo.UserCommunitySchools.Select(x => x.CommunityId).ToList());
            ViewBag.GroupPackageSelected = principalEntity.UserInfo.PermissionRoles.Select(e =>
               new GroupPackageModel()
               {
                   PackageId = e.ID,
                   PackageName = e.Name,
                   PackageDescription = e.Descriptions
               }).ToList();
            ViewBag.IsFirst = isFirst;
            if ((Role)userinfo.Role == Role.School_Specialist_Delegate)
            {
                UserBaseEntity parentUser = userBusiness.GetUser(userinfo.Principal.ParentId);
                ViewBag.ParentSchoolNames = parentUser.UserCommunitySchools.Select(x => x.School.Name).ToArray();
            }
            return principalEntity;
        }

        [System.Web.Mvc.HttpPost]
        [ValidateAntiForgeryToken]
        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.Administrative, Anonymity = Anonymous.Logined)]
        public string SaveSchoolSpecialist(PrincipalEntity principal, int[] chkPDs, string certificates, bool confirm = false)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            PostFormResponse response = new PostFormResponse();
            if (confirm == false && principal.UserInfo.Role == Role.Principal)
            {
                if (userBusiness.CheckUserExistedStatus(principal.UserInfo.ID,
                    principal.UserInfo.FirstName,
                    principal.UserInfo.LastName, principal.UserInfo.PrimaryEmailAddress,
                    principal.UserInfo.Role, out result))
                {
                    response.Success = true;
                    response.Message = result.Message;
                    response.Data = result.AppendData;
                    return JsonHelper.SerializeObject(response);
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
            if (response.Success = ModelState.IsValid)
            {
                if (principal.ID > 0)
                    result = userBusiness.UpdatePrincipal(principal, chkPDs, certificatesList);
                response.Success = result.ResultType == OperationResultType.Success;
                response.Message = result.Message;
            }
            response.ModelState = ModelState;
            return JsonHelper.SerializeObject(response);
        }

        #endregion

        #region Teacher

        private TeacherEntity GetTeacher(UserBaseEntity userinfo, bool isFirst)
        {
            TeacherRoleEntity roleEntity = userBusiness.GetTeacherRoleEntity(UserInfo.Role);
            if (roleEntity != null)
                ViewBag.Role = JsonHelper.SerializeObject(roleEntity);

            TeacherEntity teacherEntity = userBusiness.GetTeacher(userinfo.TeacherInfo.ID, UserInfo);

            string certificateText = "";
            foreach (var item in teacherEntity.UserInfo.Certificates)
            {
                certificateText += item.ID + ",";
            }
            ViewBag.CertificateText = certificateText;

            ViewBag.AgeGroupJson = JsonHelper.SerializeObject(teacherEntity.TeacherAgeGroups.Select(
                e => new { AgeGroup = e.AgeGroup, AgeGroupOther = e.AgeGroupOther }));
            ViewBag.AgeGroup = new SelectList(AgeGroup.Infant.ToSelectList(), "Value", "Text");
            ViewBag.CoachOptions = userBusiness.GetCoordCoachs(teacherEntity.UserInfo.UserCommunitySchools.Select(x=>x.CommunityId).ToList())
                .ToSelectList(ViewTextHelper.DefaultPleaseSelectText, "0");
            ViewBag.Language = new UserBusiness().GetLanguages().ToSelectList(ViewTextHelper.DefaultPleaseSelectText, "0");
            ViewBag.Funding = new UserBusiness().GetFundings().ToSelectList(ViewTextHelper.DefaultPleaseSelectText, "0");
            ViewBag.State = userBusiness.GetStates().ToSelectList("State", "0");
            ViewBag.County = userBusiness.GetCountries().ToSelectList("County", "0");
            ViewBag.YearsInProject = new UserBusiness().GetYearsInProjects().ToSelectList(ViewTextHelper.DefaultPleaseSelectText, "0");
            ViewBag.PDList = new UserBusiness().GetPDs();
            ViewBag.CertificateList = new UserBusiness().GetCertificates();

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
            ViewBag.IsFirst = isFirst;
            ViewBag.IsCLIUser = UserInfo.IsCLIUser;
            return teacherEntity;
        }

        [System.Web.Mvc.HttpPost]
        [ValidateAntiForgeryToken]
        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.Administrative, Anonymity = Anonymous.Logined)]
        public string SaveTeacher(TeacherEntity teacher, int[] ageGroups, int[] chkPDs, string certificates, bool confirm = false)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            PostFormResponse response = new PostFormResponse();
            if (confirm == false)
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
            List<int> certificatesList = new List<int>();
            if (!certificates.IsNullOrEmpty())
            {
                foreach (var item in certificates.TrimEnd(',').Split(','))
                {
                    certificatesList.Add(Convert.ToInt32(item));
                }
            }
            if (response.Success = ModelState.IsValid)
            {
                teacher.BirthDate = teacher.BirthDate ?? Convert.ToDateTime("1753-01-01");

                if (teacher.ID > 0)
                    result = userBusiness.UpdateTeacher(teacher, ageGroups, chkPDs, certificatesList, UserInfo.Role);

                response.Success = result.ResultType == OperationResultType.Success;
                response.Message = result.Message;
            }
            response.ModelState = ModelState;
            return JsonHelper.SerializeObject(response);
        }

        #endregion

        #region Parent

        private ParentEntity GetParent(UserBaseEntity userinfo, bool isFirst)
        {
            ParentEntity parent = userBusiness.GetParent(userinfo.Parent.ID);
            List<int> studentIds = userBusiness.GetStudentIDbyParentId(parent.ID);
            ViewBag.StudentList = studentBusiness.GetStudentsGetIds(studentIds);
            ViewBag.IsFirst = isFirst;
            if (!string.IsNullOrEmpty(parent.SettingIds))
            {
                List<string> settingList = parent.SettingIds.Split(',').ToList();
                ViewBag.SettingList = settingList;
            }
          
            return parent;
        }


        [System.Web.Mvc.HttpPost]
        [ValidateAntiForgeryToken]
        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.Administrative, Anonymity = Anonymous.Logined)]
        public string SaveParent(ParentEntity parent, List<int> chkSetting, string OtherSetting)
        {
            var response = new PostFormResponse();
            if (response.Success = ModelState.IsValid)
            {
                parent.SettingIds = string.Join(",", chkSetting);
                if(chkSetting.Contains(9))
                parent.OtherSetting = OtherSetting;
                else
                {
                    parent.OtherSetting = "";
                }
                OperationResult result = new OperationResult(OperationResultType.Success);
                if (parent.ID > 0)
                    result = userBusiness.UpdateParent(parent);
                response.Success = result.ResultType == OperationResultType.Success;
                response.Message = result.Message;
            }
            response.ModelState = ModelState;
            return JsonHelper.SerializeObject(response);
        }
        #endregion
    }
}