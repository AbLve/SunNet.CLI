using System.Collections.Generic;
using System.Web.Mvc;
using System.Linq;
using Sunnet.Cli.Core.Users.Entities;
using Sunnet.Cli.Business.Users;
using Sunnet.Cli.Business.Users.Models;
using Sunnet.Framework.Extensions;
using Sunnet.Framework.Core.Tool;
using Newtonsoft.Json;
using Sunnet.Cli.Core.Common.Enums;
using Sunnet.Cli.MainSite.Controllers;
using Sunnet.Cli.Core.Users.Enums;
using Sunnet.Cli.UIBase.Models;
using Sunnet.Cli.Business.Schools;
using Sunnet.Framework.Helpers;
using Sunnet.Cli.Core.Communities.Entities;
using Sunnet.Cli.Business.Communities;
using Sunnet.Cli.Business.Schools.Models;
using Sunnet.Framework.SSO;
using Sunnet.Cli.UIBase;

namespace Sunnet.Cli.MainSite.Areas.Signup.Controllers
{
    public class TeacherController : BaseController
    {
        private readonly UserBusiness userBusiness;
        private readonly SchoolBusiness schoolBusiness;
        private readonly CommunityBusiness communityBusiness;
        public TeacherController()
        {
            userBusiness = new UserBusiness(UnitWorkContext);
            schoolBusiness = new SchoolBusiness(UnitWorkContext);
            communityBusiness = new CommunityBusiness();
        }

        // GET: /SignUp/Teacher/

        public ActionResult Confirmation()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public string TeacherApplicant(ApplicantTeacherModel applicantTeacher, bool chkIsFindSchool)
        {
            bool isSchoolVerified = true;
            var response = new PostFormResponse();
            if (response.Success = ModelState.IsValid)
            {
                OperationResult result = new OperationResult(OperationResultType.Success);
                applicantTeacher.RoleType = Role.Teacher;
                SchoolModel school = schoolBusiness.IsVerified(applicantTeacher.SchoolId == null ? 0
                    : applicantTeacher.SchoolId.Value);
                if (school == null)   //如果选择的school没有认证,则不发送邮件，且跳转到确认页面
                {
                    chkIsFindSchool = true;
                    isSchoolVerified = false;
                    applicantTeacher.SchoolId = 0;
                }
                else
                {
                    applicantTeacher.SchoolId = school.ID;
                }

                result = userBusiness.TeacherApplicant(applicantTeacher, chkIsFindSchool);
                response.Success = result.ResultType == OperationResultType.Success;
                response.Data = new { entity = applicantTeacher, isSchoolVerified = isSchoolVerified };
                response.Message = result.Message;
            }
            response.ModelState = ModelState;
            return JsonHelper.SerializeObject(response);
        }

        public ActionResult PrincipalInvitedRegister(int applicantId, string basicSchoolId, bool isSpecialist = false,
            string communityName="",int communityId = 0)
        {
            if (UserAuthentication != AuthenticationStatus.Login)
            {
                ViewBag.LoginUrl = BuilderLoginUrl();
                ViewBag.CLIUserLogin = BuilderLoginUrl(LoginUserType.UTACCESSMANAGER);
            }
            ViewBag.SchoolTypeOptions = schoolBusiness.GetSchoolTypeList(0).Select(o => new SelectListItem
            {
                Text = o.Name,
                Value = o.ID.ToString()
            }).AddDefaultItem(ViewTextHelper.DefaultPleaseSelectText, "");
            ViewBag.StateOptions = userBusiness.GetStates().ToSelectList(ViewTextHelper.DefaultStateText, "");
            ApplicantContactEntity applicantContact = new ApplicantContactEntity();
            applicantContact.ApplicantId = applicantId;
            applicantContact.CommunityId = communityId;
            applicantContact.CommunityName = communityName;
            if (!string.IsNullOrEmpty(basicSchoolId))
            {
                SchoolSelectItemModel basicSchool = schoolBusiness.GetBasicSchoolByBasicId(int.Parse(basicSchoolId));
                if (basicSchool != null)
                {
                    applicantContact.SchoolName = basicSchool.Name;
                    applicantContact.Address = basicSchool.Address;
                    applicantContact.StateId = basicSchool.StateId;
                    applicantContact.City = basicSchool.City;
                    applicantContact.Zip = basicSchool.Zip;
                }
            }

            ViewBag.isSpecialist = isSpecialist;
            return View(applicantContact);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public string InvitationPrincipalRegister(ApplicantContactEntity applicantContact, bool isSpecialist = false)
        {
            string emailTemplateName = "";
            var response = new PostFormResponse();
            if (response.Success = ModelState.IsValid)
            {
                OperationResult result = new OperationResult(OperationResultType.Success);

                if (isSpecialist) //是否是school secialist ,若是，则同principal
                {
                    if (applicantContact.SchoolTypeId == 1)//类型为 public 发送5A邮件
                    {
                        emailTemplateName = "PrincipalApplicant_5A_Template.xml";
                    }
                    //Head Start or Child Care Center-Based 发送5B邮件，其余类别不发送邮件
                    if (applicantContact.SchoolTypeId == 2 || applicantContact.SchoolTypeId == 3)
                    {
                        emailTemplateName = "PrincipalApplicant_5B_Template.xml";
                    }
                }

                else //teacher 注册
                {
                    //根据不同的学校类别发送不同模板的邮件
                    switch (applicantContact.SchoolTypeId)
                    {
                        case 1:  //Public School
                            emailTemplateName = "TeacherApplicant_1A_Template.xml";
                            break;
                        case 2:  //Head Start
                        case 3:   //Child Care Center-based
                            emailTemplateName = "TeacherApplicant_1B_Template.xml";
                            break;
                        case 4: //Family Child Care (FCC)
                            emailTemplateName = "TeacherApplicant_TBD_Template.xml";
                            break;
                    }
                }

                result = userBusiness.SaveApplicantContact(applicantContact, emailTemplateName);
                response.Success = result.ResultType == OperationResultType.Success;
                //response.Data = applicantContact;
                response.Message = result.Message;
            }
            response.ModelState = ModelState;
            return JsonHelper.SerializeObject(response);
        }

        public string GetBasicSchoolSelectList(string keyword, int communityId = 0)
        {
            IEnumerable<SchoolSelectItemModel> list = schoolBusiness.GetBasicSchoolSelectListByKey(keyword, communityId);
            return JsonHelper.SerializeObject(list);
        }

        public string GetBasicCommunitySelectList(string keyword)
        {
            var communitySelectList = communityBusiness.GetBasicCommunitySelectList(
                o => keyword == "-1" || o.Name.Contains(keyword), false, true);
            return JsonHelper.SerializeObject(communitySelectList);
        }

        public string GetCommunitySelectList(string keyword, string communityName = "")
        {
            var expression = PredicateHelper.True<CommunityEntity>();
            expression = string.IsNullOrEmpty(communityName) || communityName.Trim() == string.Empty
                ? expression.And(o => true)
                : expression.And(o => o.BasicCommunity.Name.Contains(communityName));
            var communitySelectList = communityBusiness.GetCommunitySelectList(null, expression, true);
            return JsonHelper.SerializeObject(communitySelectList);
        }



        /// <summary>
        /// 发送测试邮件
        /// </summary>
        /// <param name="name"></param>
        [HttpPost]
        public string TestSendEmail(string emailName)
        {
            var response = new PostFormResponse();
            OperationResult result = new OperationResult(OperationResultType.Success);
            if (!string.IsNullOrEmpty(emailName))
            {
                EmailTemplete template = XmlHelper.GetEmailTemplete(emailName);
                string subject = template.Subject;
                string emailBody = template.Body
                            .Replace("{ApplicantFirstName}", "ApplicantFirstName")
                            .Replace("{ApplicantLastName}", "ApplicantLastName")
                            .Replace("{ApplicantEmail}", "ApplicantEmail")
                            .Replace("{CliEngageUrl}", "CliEngageUr")
                            .Replace("{StaticDomain}", "http://static.sunnet.cc/Content/images/cli_logo.png");
                if (!string.IsNullOrEmpty(emailBody))
                {
                    userBusiness.SendEmail("joex@sunnet.us", subject, emailBody);
                }
            }
            response.Success = result.ResultType == OperationResultType.Success;
            response.Message = result.Message;
            response.ModelState = ModelState;
            return JsonHelper.SerializeObject(response);
        }

        //测试发送邮件页面
        public ActionResult TestSendEmailView()
        {
            return View();
        }
    }
}