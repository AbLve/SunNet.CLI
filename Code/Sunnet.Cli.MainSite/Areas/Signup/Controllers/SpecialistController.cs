using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
/**************************************************************************
 * Developer: 		Joe
 * Computer:		JOE-PC
 * Domain:			Joe-pc
 * CreatedOn:		2014/9/9 12:15:10
 * Description:		Please input class summary
 * Version History:	Created,2014/9/9 12:15:10
 * 
 * 
 **************************************************************************/
using Sunnet.Cli.MainSite.Controllers;
using Sunnet.Cli.Business.Users;
using Sunnet.Cli.Business.Schools;
using Sunnet.Cli.Business.Users.Models;
using Sunnet.Cli.UIBase.Models;
using Sunnet.Framework.Core.Tool;
using Sunnet.Cli.Core.Users.Enums;
using Sunnet.Cli.Business.Communities;
using Sunnet.Framework.Extensions;
using Sunnet.Cli.Core.Users.Entities;
using Sunnet.Cli.Business.Communities.Models;
using Sunnet.Cli.Business.Schools.Models;
using Sunnet.Framework.SSO;
using Sunnet.Cli.UIBase;

namespace Sunnet.Cli.MainSite.Areas.Signup.Controllers
{
    public class SpecialistController : BaseController
    {
        private readonly UserBusiness userBusiness;
        private readonly SchoolBusiness schoolBusiness;
        private readonly CommunityBusiness communityBusiness;

        public SpecialistController()
        {
            userBusiness = new UserBusiness(UnitWorkContext);
            schoolBusiness = new SchoolBusiness(UnitWorkContext);
            communityBusiness = new CommunityBusiness(UnitWorkContext);
        }

        //
        // GET: /Signup/Specialist/
        public ActionResult Index()
        {
            if (UserAuthentication != AuthenticationStatus.Login)
            {
                ViewBag.LoginUrl = BuilderLoginUrl();
                ViewBag.CLIUserLogin = BuilderLoginUrl(LoginUserType.UTACCESSMANAGER);
            }
            ApplicantSpecialistModel entity = new ApplicantSpecialistModel();
            ViewBag.Position = userBusiness.GetPositions((int)Role.District_Community_Specialist).
                ToSelectList(ViewTextHelper.DefaultPleaseSelectText, "");
            ViewBag.Position = userBusiness.GetPositions((int)Role.District_Community_Specialist).
                ToSelectList(ViewTextHelper.DefaultPleaseSelectText, "");
            ViewBag.Position = userBusiness.GetPositions((int)Role.District_Community_Specialist).
                ToSelectList(ViewTextHelper.DefaultPleaseSelectText, "");
            return View(entity);
        }

        public string GetPositions(int role)
        {
            var PositionsLists = userBusiness.GetPositions(role).ToList();
            return JsonHelper.SerializeObject(PositionsLists);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public string SpecialistApplicant(ApplicantSpecialistModel entity, string roleType, bool chkIsFindSchool)
        {
            bool isSchoolVerified = true;   //判断School是否验证通过
            bool isCommunityVerified = true;//判断Community是否验证通过
            var response = new PostFormResponse();
            if (response.Success = ModelState.IsValid)
            {
                OperationResult result = new OperationResult(OperationResultType.Success);
                if (!string.IsNullOrEmpty(roleType))
                {
                    if (roleType == "0")  //用户注册类型为地区专家
                    {
                        entity.RoleType = Role.District_Community_Specialist;
                        if (!chkIsFindSchool) //如果没有选中 未找到  选项，则进行判断是否验证通过
                        {
                            CommunityModel community = communityBusiness.IsVerified(entity.CommunityId.Value);
                            if (community == null)
                            {
                                isCommunityVerified = false;
                                chkIsFindSchool = true;//未通过验证
                                entity.CommunityId = 0;
                            }
                            else
                            {
                                entity.CommunityId = community.ID;
                            }
                        }

                    }
                    else if (roleType == "1")  //用户注册类型为TRS专家
                    {
                        entity.RoleType = Role.TRS_Specialist;
                        if (!chkIsFindSchool)
                        {
                            SchoolModel school = schoolBusiness.IsVerified(entity.SchoolId.Value);
                            if (school == null)
                            {
                                isSchoolVerified = false;   //未验证通过
                                chkIsFindSchool = true;     //如果没有验证通过，将没有找到学校设置为true 
                                entity.SchoolId = 0;
                            }
                            else
                            {
                                entity.SchoolId = school.ID;  //将schoolid设置为school表中的id
                            }
                        }
                    }
                    else if (roleType == "2")  //用户注册类型为学校专家
                    {
                        entity.RoleType = Role.School_Specialist;
                        if (!chkIsFindSchool)
                        {
                            SchoolModel school = schoolBusiness.IsVerified(entity.SchoolId.Value);
                            if (school == null)
                            {
                                isSchoolVerified = false;   //未验证通过
                                chkIsFindSchool = true;     //如果没有验证通过，将没有找到学校设置为true 
                                entity.SchoolId = 0;
                            }
                            else
                            {
                                entity.SchoolId = school.ID;  //将schoolid设置为school表中的id
                            }
                        }
                    }
                }
                result = userBusiness.SpecialistApplicant(entity, chkIsFindSchool);
                response.Success = result.ResultType == OperationResultType.Success;
                response.Data = new { entity = entity, isSchoolVerified = isSchoolVerified, isCommunityVerified = isCommunityVerified };
                response.Message = result.Message;
            }
            response.ModelState = ModelState;
            return JsonHelper.SerializeObject(response);
        }


        /// <summary>
        /// community 邀请注册视图
        /// </summary>
        /// <param name="applicantId"></param>
        /// <returns></returns>
        public ActionResult CommunityInvitedRegister(int applicantId, string basicCommunityId)
        {
            if (UserAuthentication != AuthenticationStatus.Login)
            {
                ViewBag.LoginUrl = BuilderLoginUrl();
                ViewBag.CLIUserLogin = BuilderLoginUrl(LoginUserType.UTACCESSMANAGER);
            }
            ViewBag.StateOptions = userBusiness.GetStates().ToSelectList(ViewTextHelper.DefaultStateText, "");
            ApplicantContactEntity applicantContact = new ApplicantContactEntity();
            applicantContact.ApplicantId = applicantId;
            applicantContact.CommunityId = 0;
            if (!string.IsNullOrEmpty(basicCommunityId))
            {
                BasicCommunityModel basicCommunity = communityBusiness.GetBasicCommunityModel(int.Parse(basicCommunityId));
                if (basicCommunity != null)
                {
                    applicantContact.CommunityId = basicCommunity.ID;
                    applicantContact.CommunityName = basicCommunity.Name;
                    applicantContact.Address = basicCommunity.Address1;
                    applicantContact.City = basicCommunity.City;
                    applicantContact.StateId = basicCommunity.StateId;
                    applicantContact.Zip = basicCommunity.Zip;
                }
            }
            return View(applicantContact);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public string InvitationCommunityRegister(ApplicantContactEntity applicantContact, string RoleType)
        {
            var response = new PostFormResponse();

            OperationResult result = new OperationResult(OperationResultType.Success);
            result = userBusiness.SaveApplicantContact(applicantContact, "TeacherApplicant_TBD_Template.xml");
            response.Success = result.ResultType == OperationResultType.Success;
            response.Message = result.Message;

            response.ModelState = ModelState;
            return JsonHelper.SerializeObject(response);
        }
    }
}