using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		Sam
 * Computer:		SAM-PC
 * Domain:			sam-pc
 * CreatedOn:		2014/8/15 14:34:02
 * Description:		Please input class summary
 * Version History:	Created,2014/8/15 14:34:02
 * 
 * 
 **************************************************************************/
using Sunnet.Cli.Business.Common;
using Sunnet.Cli.Core.Users.Entities;
using Sunnet.Framework.Core.Tool;
using Sunnet.Cli.Core;
using Sunnet.Cli.Core.MasterData.Entities;
using Sunnet.Cli.Core.MasterData;
using Sunnet.Framework.Extensions;
using Sunnet.Cli.Core.Users.Enums;
using Sunnet.Framework.Helpers;
using Sunnet.Cli.Core.Common.Enums;
using Sunnet.Cli.Business.Users.Models;
using Sunnet.Cli.Business.Schools.Models;
using Sunnet.Framework;
using System.Linq.Expressions;
using LinqKit;
using Sunnet.Framework.Core.Extensions;
using Sunnet.Cli.Core.Permission.Entities;
using Sunnet.Cli.Business.Permission;
using Sunnet.Cli.Business.Communities;
using Sunnet.Cli.Core.Communities.Entities;
using Sunnet.Cli.Core.Schools.Entities;
using Sunnet.Cli.Business.Schools;
using System.Data.Entity;

namespace Sunnet.Cli.Business.Users
{
    public partial class UserBusiness
    {
        #region

        // community/district 配置值
        int missingfunding = string.IsNullOrEmpty(SFConfig.MissingFunding) ? 0 : int.Parse(SFConfig.MissingFunding);

        public ApplicantEntity GetApplicant(int id)
        {
            return userService.GetAppliant(id);
        }

        public ApplicantEntity GetApplicantByInviteeId(int inviteeId)
        {
            return userService.Applicants.Where(e => e.InviteeId == inviteeId).FirstOrDefault();
        }

        public List<UserVerificationModel> SearchVertificationRequests(UserBaseEntity user, Expression<Func<ApplicantEntity, bool>> condition,
  string sort, string order, int first, int count, out int total)
        {
            var query = userService.Applicants.AsExpandable().Where(condition).Where(GetVericationRequestRoleCondition(user));

            total = query.Count();
            var list = query.Select(e => new UserVerificationModel()
            {
                ID = e.ID,
                FirstName = e.FirstName,
                LastName = e.LastName,
                Email = e.Email,
                RoleType = e.RoleType,
                RequestedOn = e.CreatedOn,
                Status = e.Status
            }).OrderBy(sort, order).AsQueryable().Skip(first).Take(count);
            return list.ToList();
        }

        private Expression<Func<ApplicantEntity, bool>> GetVericationRequestRoleCondition(UserBaseEntity userInfo)
        {
            Expression<Func<ApplicantEntity, bool>> condition = o => true;
            if (userInfo == null)
                return condition;
            UserBusiness userBusiness = new UserBusiness();
            switch (userInfo.Role)
            {
                case Role.Teacher:
                    condition = o => false;
                    break;
                case Role.Principal:
                    if (userInfo.Principal != null)
                    {
                        var schoolIds = userInfo.UserCommunitySchools.Select(x => x.SchoolId).ToList();
                        condition = PredicateBuilder.And(condition, o =>
                            (o.RoleType == Role.TRS_Specialist || o.RoleType == Role.School_Specialist ||
                             o.RoleType == Role.Teacher) && schoolIds.Any(x => x == o.SchoolId));
                    }
                    break;
                case Role.Principal_Delegate:
                    if (userInfo.Principal != null)
                    {
                        UserBaseEntity parentUser = userBusiness.GetUser(userInfo.Principal.ParentId);
                        var schoolIds = parentUser.UserCommunitySchools.Select(x => x.SchoolId).ToList();
                        condition = PredicateBuilder.And(condition, o =>
                            (o.RoleType == Role.TRS_Specialist || o.RoleType == Role.School_Specialist ||
                             o.RoleType == Role.Teacher) && schoolIds.Any(x => x == o.SchoolId));
                    }
                    break;
                case Role.TRS_Specialist:
                case Role.School_Specialist:
                    if (userInfo.Principal != null)
                    {
                        var schoolIds = userInfo.UserCommunitySchools.Select(x => x.SchoolId).ToList();
                        condition = PredicateBuilder.And(condition,
                            o => (o.RoleType == Role.Teacher)
                                && schoolIds.Any(x => x == o.SchoolId));
                    }
                    break;
                case Role.TRS_Specialist_Delegate:
                case Role.School_Specialist_Delegate:
                    if (userInfo.Principal != null)
                    {
                        UserBaseEntity parentUser = userBusiness.GetUser(userInfo.Principal.ParentId);
                        var schoolIds = parentUser.UserCommunitySchools.Select(x => x.SchoolId).ToList();
                        condition = PredicateBuilder.And(condition,
                            o => (o.RoleType == Role.Teacher)
                                && schoolIds.Any(x => x == o.SchoolId));
                    }
                    break;
                case Role.Statewide:
                case Role.Community:
                    if (userInfo.CommunityUser != null)
                    {
                        List<int> primarySchools =
                            schoolBusiness.GetPrimarySchoolsByComId(
                                userInfo.UserCommunitySchools.Select(e => e.CommunityId).ToList())
                                .Select(e => e.ID).ToList();
                        List<int> communityIds = GetCommunities(userInfo.ID);
                        condition = PredicateBuilder.And(condition,
                            o =>
                                (o.RoleType == Role.Community || o.RoleType == Role.District_Community_Specialist ||
                                 o.RoleType == Role.Principal || o.RoleType == Role.TRS_Specialist ||
                                 o.RoleType == Role.School_Specialist || o.RoleType == Role.Teacher) &&
                                (communityIds.Contains(o.CommunityId)) || primarySchools.Contains(o.SchoolId));
                    }
                    break;
                case Role.District_Community_Delegate:
                    if (userInfo.CommunityUser != null)
                    {
                        UserBaseEntity parentUser = userBusiness.GetUser(userInfo.CommunityUser.ParentId);
                        List<int> primarySchools =
                            schoolBusiness.GetPrimarySchoolsByComId(
                                parentUser.UserCommunitySchools.Select(e => e.CommunityId).ToList())
                                .Select(e => e.ID).ToList();
                        List<int> communityIds = GetCommunities(parentUser.ID);
                        condition = PredicateBuilder.And(condition,
                            o =>
                                (o.RoleType == Role.District_Community_Specialist ||
                                 o.RoleType == Role.Principal || o.RoleType == Role.TRS_Specialist ||
                                 o.RoleType == Role.School_Specialist || o.RoleType == Role.Teacher) &&
                                (communityIds.Contains(o.CommunityId)) || primarySchools.Contains(o.SchoolId));
                    }
                    break;
                case Role.District_Community_Specialist:
                    if (userInfo.CommunityUser != null)
                    {
                        List<int> primarySchools =
                            schoolBusiness.GetPrimarySchoolsByComId(
                                userInfo.UserCommunitySchools.Select(e => e.CommunityId).ToList())
                                .Select(e => e.ID).ToList();
                        List<int> communityIds = GetCommunities(userInfo.ID);
                        condition = PredicateBuilder.And(condition,
                            o =>
                                (o.RoleType == Role.Principal || o.RoleType == Role.TRS_Specialist ||
                                 o.RoleType == Role.School_Specialist || o.RoleType == Role.Teacher) &&
                                (communityIds.Contains(o.CommunityId)) || primarySchools.Contains(o.SchoolId));
                    }
                    break;
                case Role.Community_Specialist_Delegate:
                    if (userInfo.CommunityUser != null)
                    {
                        UserBaseEntity parentUser = userBusiness.GetUser(userInfo.CommunityUser.ParentId);
                        List<int> primarySchools =
                            schoolBusiness.GetPrimarySchoolsByComId(
                                parentUser.UserCommunitySchools.Select(e => e.CommunityId).ToList())
                                .Select(e => e.ID).ToList();
                        List<int> communityIds = GetCommunities(parentUser.ID);
                        condition = PredicateBuilder.And(condition,
                            o =>
                                (o.RoleType == Role.Principal || o.RoleType == Role.TRS_Specialist ||
                                 o.RoleType == Role.School_Specialist || o.RoleType == Role.Teacher) &&
                                (communityIds.Contains(o.CommunityId)) || primarySchools.Contains(o.SchoolId));
                    }
                    break;
                case Role.Content_personnel:
                case Role.Statisticians:
                case Role.Administrative_personnel:
                case Role.Intervention_manager:
                case Role.Video_coding_analyst:
                case Role.Intervention_support_personnel:
                case Role.Coordinator:
                case Role.Mentor_coach:
                    List<int> internal_communityIds = userInfo.UserCommunitySchools.Select(e => e.CommunityId).ToList();
                    List<int> internal_schoolIds = schoolBusiness.GetSchoolIds(internal_communityIds);
                    condition = PredicateBuilder.And(condition,
                        o => internal_communityIds.Contains(o.CommunityId) || internal_schoolIds.Contains(o.SchoolId));
                    break;
                case Role.Parent:
                    condition = o => false;
                    break;
            }
            return condition;
        }

        #endregion

        #region External User Sign Up Methods
        public OperationResult TeacherApplicant(ApplicantTeacherModel applicantTeacher, bool chkSchool)
        {
            ApplicantEntity applicant = new ApplicantEntity();
            applicant.Title = applicantTeacher.Title;
            applicant.FirstName = applicantTeacher.FirstName;
            applicant.LastName = applicantTeacher.LastName;
            applicant.Email = applicantTeacher.Email;
            applicant.RoleType = applicantTeacher.RoleType;
            applicant.SchoolId = applicantTeacher.SchoolId == null ? 0 : applicantTeacher.SchoolId.Value;
            applicant.CommunityId = applicantTeacher.CommunityId == null ? 0 : applicantTeacher.CommunityId.Value;

            //发送邮件给principal和community
            SendEmail_TeacherApplicant(applicantTeacher.SchoolId, chkSchool, applicant);

            OperationResult result = new OperationResult(OperationResultType.Success);
            result = userService.InsertApplication(applicant);
            applicantTeacher.ApplicantId = applicant.ID;
            return result;
        }

        private void SendEmail_TeacherApplicant(int? schoolId, bool chkSchool, ApplicantEntity applicant)
        {
            if (!chkSchool)
            {
                if (schoolId > 0)
                {
                    applicant.ApplicantEmails = new List<ApplicantEmailEntity>();

                    string emailTemplateName = ""; //发送到校长的邮件模板
                    string emialTemplateName_Community = "";//发送到community的邮件模板
                    EmailTemplete template = new EmailTemplete();
                    string emailBody = "";
                    string subject = "";
                    List<UserModel> userModelList = GetPrincipalBySchoolId(schoolId.Value); //根据schoolId查找出所有有效的校长
                    CommunityEntity community = null;
                    if (applicant.CommunityId > 0)
                    {
                        community = new CommunityBusiness().GetCommunity(applicant.CommunityId);
                    }
                    SchoolModel school = new SchoolBusiness().GetSchoolEntity(schoolId.Value, null);

                    bool isVerified = false;//用于判断校长是否全部通过验证

                    if (community != null)
                    {
                        if (community.FundingId == missingfunding)//是community ,发送4B邮件
                        {

                            emialTemplateName_Community = "TeacherApplicant_4B_Template.xml";
                        }
                        else   //是district,发送4A邮件
                        {

                            emialTemplateName_Community = "TeacherApplicant_4A_Template.xml";
                        }
                    }

                    if (school != null)
                    {
                        if (school.SchoolTypeId == 1)     //Public School,发送2A邮件
                        {
                            emailTemplateName = "TeacherApplicant_2A_Template.xml";
                        }
                        else  //非公立学校，发送2B邮件
                        {
                            emailTemplateName = "TeacherApplicant_2B_Template.xml";
                        }
                    }


                    if (!string.IsNullOrEmpty(emailTemplateName))
                    {
                        if (userModelList.Count > 0)
                        {
                            isVerified = true;

                            //发送给当前学校下有效的所有校长
                            foreach (var item in userModelList)
                            {
                                template = XmlHelper.GetEmailTemplete(emailTemplateName);
                                subject = template.Subject;
                                emailBody = template.Body.Replace("{SuperiorFirstName}", item.FirstName)
                                    .Replace("{SuperiorLastName}", item.LastName)
                                    .Replace("{ApplicantFirstName}", applicant.FirstName)
                                    .Replace("{ApplicantLastName}", applicant.LastName)
                                    .Replace("{ApplicantEmail}", applicant.Email)
                                    .Replace("{CliEngageUrl}", SFConfig.MainSiteDomain)
                                    .Replace("{StaticDomain}", SFConfig.StaticDomain);
                                SendEmail(item.Email, subject, emailBody);
                                ApplicantEmailEntity applicantEmailEntity = new ApplicantEmailEntity();
                                applicantEmailEntity.EmailContent = emailBody;
                                applicantEmailEntity.EmailAddress = item.Email;
                                applicant.ApplicantEmails.Add(applicantEmailEntity);
                            }
                        }
                    }


                    if (!string.IsNullOrEmpty(emialTemplateName_Community))
                    {
                        //如果principle或者director全部都没有通过验证，发送邮件到community/district contact
                        if (!isVerified)
                        {
                            //查找community下的所有联系人
                            List<UserModel> communityUsers = GetActiveCommunityByCommunityId(applicant.CommunityId, true);
                            if (communityUsers.Count > 0)
                            {
                                //发送给当前community下的所有人员
                                foreach (UserModel item_community in communityUsers)
                                {
                                    template = XmlHelper.GetEmailTemplete(emialTemplateName_Community);
                                    subject = template.Subject;
                                    emailBody = template.Body.Replace("{SuperiorFirstName}", item_community.FirstName)
                                    .Replace("{SuperiorLastName}", item_community.LastName)
                                .Replace("{Role}", applicant.RoleType.ToDescription().ToLower())
                                    .Replace("{ApplicantFirstName}", applicant.FirstName)
                                    .Replace("{ApplicantLastName}", applicant.LastName)
                                    .Replace("{ApplicantEmail}", applicant.Email)
                                    .Replace("{CliEngageUrl}", SFConfig.MainSiteDomain)
                                    .Replace("{StaticDomain}", SFConfig.StaticDomain);
                                    SendEmail(item_community.Email, subject, emailBody);
                                    ApplicantEmailEntity applicantEmailEntity = new ApplicantEmailEntity();
                                    applicantEmailEntity.EmailContent = emailBody;
                                    applicantEmailEntity.EmailAddress = item_community.Email;
                                    applicant.ApplicantEmails.Add(applicantEmailEntity);
                                }
                            }
                            else
                            {
                                SendEmailtoCLI(applicant);
                            }
                        }
                    }
                }
            }
        }


        /// <summary>
        /// 发送邮件给CLI用户
        /// </summary>
        /// <param name="applicant"></param>
        private void SendEmailtoCLI(ApplicantEntity applicant)
        {
            EmailTemplete template = new EmailTemplete();
            template = XmlHelper.GetEmailTemplete("CLI_Template.xml");
            string subject = template.Subject;
            string emailBody = template.Body
                            .Replace("{ApplicantFirstName}", applicant.FirstName)
                            .Replace("{ApplicantLastName}", applicant.LastName)
                            .Replace("{CliEngageUrl}", SFConfig.MainSiteDomain)
                            .Replace("{StaticDomain}", SFConfig.StaticDomain);
            //根据不同的角色替换不同的内容
            switch (applicant.RoleType)
            {
                case Role.Community:
                    emailBody = emailBody.Replace("{UserRole}", "COMMUNITY/DISTRICT USER");
                    break;
                case Role.District_Community_Specialist:
                    emailBody = emailBody.Replace("{UserRole}", "COMMUNITY/DISTRICT SPECIALIST");
                    break;
                case Role.Principal:
                    emailBody = emailBody.Replace("{UserRole}", "PRINCIPAL/DIRECTOR");
                    break;
                case Role.TRS_Specialist:
                    emailBody = emailBody.Replace("{UserRole}", "TRS SPECIALIST");
                    break;
                case Role.School_Specialist:
                    emailBody = emailBody.Replace("{UserRole}", "SCHOOL SPECIALIST");
                    break;
                case Role.Teacher:
                    emailBody = emailBody.Replace("{UserRole}", "TEACHER");
                    break;
            }
            SendEmail(SFConfig.CLIAdministratorEmail, subject, emailBody);
            ApplicantEmailEntity applicantEmailEntity = new ApplicantEmailEntity();
            applicantEmailEntity.EmailContent = emailBody;
            applicantEmailEntity.EmailAddress = SFConfig.CLIAdministratorEmail;
            applicant.ApplicantEmails.Add(applicantEmailEntity);
        }


        /// <summary>
        /// Principal 注册
        /// </summary>
        /// <param name="applicantTeacher"></param>
        /// <param name="chkNoDistrict"></param>
        /// <returns></returns>
        public OperationResult PrincipalApplicant(ApplicantTeacherModel applicantTeacher, bool chkNoDistrict)
        {
            ApplicantEntity applicant = new ApplicantEntity();
            applicant.Title = applicantTeacher.Title;
            applicant.FirstName = applicantTeacher.FirstName;
            applicant.LastName = applicantTeacher.LastName;
            applicant.Email = applicantTeacher.Email;
            applicant.RoleType = applicantTeacher.RoleType;
            applicant.CommunityId = applicantTeacher.CommunityId == null ? 0 : applicantTeacher.CommunityId.Value;
            if (!chkNoDistrict)
            {
                if (applicantTeacher.CommunityId > 0)
                {
                    applicant.ApplicantEmails = new List<ApplicantEmailEntity>();

                    string emailTemplateName = "";
                    EmailTemplete template = new EmailTemplete();
                    string emailBody = "";
                    string subject = "";
                    List<UserModel> userModelList = GetActiveCommunityByCommunityId(applicantTeacher.CommunityId.Value);

                    CommunityEntity community = new CommunityBusiness().GetCommunity(applicant.CommunityId);
                    if (community.FundingId == missingfunding)//是community ,发送4B邮件
                    {
                        emailTemplateName = "TeacherApplicant_4B_Template.xml";
                    }
                    else   //是district,发送4A邮件
                    {
                        emailTemplateName = "TeacherApplicant_4A_Template.xml";
                    }
                    if (userModelList.Count > 0)
                    {
                        foreach (var item in userModelList)
                        {
                            template = XmlHelper.GetEmailTemplete(emailTemplateName);
                            subject = template.Subject;
                            emailBody = template.Body.Replace("{SuperiorFirstName}", item.FirstName)
                                .Replace("{SuperiorLastName}", item.LastName)
                                .Replace("{Role}", applicant.RoleType.ToDescription().ToLower())
                                .Replace("{ApplicantFirstName}", applicant.FirstName)
                                .Replace("{ApplicantLastName}", applicant.LastName)
                                .Replace("{ApplicantEmail}", applicant.Email)
                                .Replace("{CliEngageUrl}", SFConfig.MainSiteDomain)
                                .Replace("{StaticDomain}", SFConfig.StaticDomain);
                            SendEmail(item.Email, subject, emailBody);
                            ApplicantEmailEntity applicantEmailEntity = new ApplicantEmailEntity();
                            applicantEmailEntity.EmailContent = emailBody;
                            applicantEmailEntity.EmailAddress = item.Email;
                            applicant.ApplicantEmails.Add(applicantEmailEntity);
                        }
                    }
                    else   //没有community user时发送邮件给CLI
                    {
                        SendEmailtoCLI(applicant);
                    }

                }
            }
            OperationResult result = new OperationResult(OperationResultType.Success);
            result = userService.InsertApplication(applicant);
            applicantTeacher.ApplicantId = applicant.ID;
            return result;
        }


        public OperationResult CommunityApplicant(ApplicantCommunityModel applicantCommunity, int basicCommunityId, string communityName)
        {
            ApplicantEntity applicant = new ApplicantEntity();
            applicant.CommunityId = applicantCommunity.CommunityId.Value;
            applicant.Title = applicantCommunity.Title;
            applicant.FirstName = applicantCommunity.FirstName;
            applicant.LastName = applicantCommunity.LastName;
            applicant.WorkPhone = applicantCommunity.WorkPhone;
            applicant.OtherPhone = applicantCommunity.OtherPhone;
            applicant.Email = applicantCommunity.Email;
            applicant.Address = applicantCommunity.Address;
            applicant.Address2 = applicantCommunity.Address2;
            applicant.City = applicantCommunity.City;
            applicant.StateId = applicantCommunity.StateId;
            applicant.Zip = applicantCommunity.Zip;
            applicant.RoleType = applicantCommunity.RoleType;
            applicant.ApplicantEmails = new List<ApplicantEmailEntity>();

            ApplicantContactEntity applicantContact = new ApplicantContactEntity();
            if (applicant.CommunityId == 0)
            {
                //将basic community Id插入到applicant Contact表中，以便CLI认证页面区分此id是community还是basicommunity
                applicantContact.CommunityId = basicCommunityId;
                applicantContact.CommunityName = communityName;
                applicant.ApplicantContacts.Add(applicantContact);
            }
            SendEmailtoCLI(applicant);

            OperationResult result = new OperationResult(OperationResultType.Success);
            result = userService.InsertApplication(applicant);
            applicantCommunity.ApplicantId = applicant.ID;
            return result;
        }

        /// <summary>
        /// SpecialistApplicant
        /// </summary>
        /// <param name="applicantSpecialist"></param>
        /// <param name="chkSchool"></param>
        /// <returns></returns>
        public OperationResult SpecialistApplicant(ApplicantSpecialistModel applicantSpecialist, bool chkSchool)
        {
            ApplicantEntity applicant = new ApplicantEntity();
            applicant.Title = applicantSpecialist.Title;
            applicant.FirstName = applicantSpecialist.FirstName;
            applicant.LastName = applicantSpecialist.LastName;
            applicant.Email = applicantSpecialist.Email;
            applicant.RoleType = applicantSpecialist.RoleType;
            applicant.PositionId = applicantSpecialist.PositionId;
            applicant.PositionOther = string.IsNullOrEmpty(applicantSpecialist.PositionOther) ? "" : applicantSpecialist.PositionOther;
            applicant.SchoolId = applicantSpecialist.SchoolId == null ? 0 : applicantSpecialist.SchoolId.Value;
            applicant.CommunityId = applicantSpecialist.CommunityId == null ? 0 : applicantSpecialist.CommunityId.Value;
            if (!chkSchool)
            {
                string emailTemplateName = "";
                EmailTemplete template = new EmailTemplete();
                string emailBody = "";
                string subject = "";

                if (applicantSpecialist.SchoolId > 0) //School Specialist     同principal
                {
                    applicant.ApplicantEmails = new List<ApplicantEmailEntity>();

                    CommunityEntity community = new CommunityEntity();
                    if (applicant.CommunityId > 0)
                    {
                        community = new CommunityBusiness().GetCommunity(applicant.CommunityId);
                    }
                    if (community.FundingId == missingfunding) //是community ,发送4B邮件
                    {
                        emailTemplateName = "TeacherApplicant_4B_Template.xml";
                    }
                    else //是district,发送4A邮件
                    {
                        emailTemplateName = "TeacherApplicant_4A_Template.xml";
                    }
                    List<UserModel> userModelList = GetActiveCommunityBySchoolId(applicant.SchoolId);
                    if (userModelList.Count > 0)
                    {
                        foreach (var item in userModelList)
                        {
                            template = XmlHelper.GetEmailTemplete(emailTemplateName);
                            subject = template.Subject;
                            emailBody = template.Body.Replace("{SuperiorFirstName}", item.FirstName)
                                .Replace("{SuperiorLastName}", item.LastName)
                                .Replace("{Role}", applicant.RoleType.ToDescription().ToLower())
                                .Replace("{ApplicantFirstName}", applicant.FirstName)
                                .Replace("{ApplicantLastName}", applicant.LastName)
                                .Replace("{ApplicantEmail}", applicant.Email)
                                .Replace("{CliEngageUrl}", SFConfig.MainSiteDomain)
                                .Replace("{StaticDomain}", SFConfig.StaticDomain);
                            SendEmail(item.Email, subject, emailBody);

                            ApplicantEmailEntity applicantEmailEntity = new ApplicantEmailEntity();
                            applicantEmailEntity.EmailContent = emailBody;
                            applicantEmailEntity.EmailAddress = item.Email;
                            applicant.ApplicantEmails.Add(applicantEmailEntity);
                        }
                    }
                    else
                    {
                        SendEmailtoCLI(applicant);
                    }
                }
                else //Community Specialist
                {
                    if (applicant.CommunityId > 0)
                    {
                        List<UserModel> userModelList = GetActiveCommunityByCommunityId(applicant.CommunityId);
                        if (userModelList.Count > 0)
                        {
                            emailTemplateName = "TeacherApplicant_4A_Template.xml";
                            foreach (var item in userModelList)
                            {
                                template = XmlHelper.GetEmailTemplete(emailTemplateName);
                                subject = template.Subject;
                                emailBody = template.Body.Replace("{SuperiorFirstName}", item.FirstName)
                                    .Replace("{SuperiorLastName}", item.LastName)
                                    .Replace("{Role}", applicant.RoleType.ToDescription().ToLower())
                                    .Replace("{ApplicantFirstName}", applicant.FirstName)
                                    .Replace("{ApplicantLastName}", applicant.LastName)
                                    .Replace("{ApplicantEmail}", applicant.Email)
                                    .Replace("{CliEngageUrl}", SFConfig.MainSiteDomain)
                                    .Replace("{StaticDomain}", SFConfig.StaticDomain);
                                SendEmail(item.Email, subject, emailBody);

                                ApplicantEmailEntity applicantEmailEntity = new ApplicantEmailEntity();
                                applicantEmailEntity.EmailContent = emailBody;
                                applicantEmailEntity.EmailAddress = item.Email;
                                applicant.ApplicantEmails.Add(applicantEmailEntity);
                            }
                        }
                        else
                        {
                            SendEmailtoCLI(applicant);
                        }
                    }
                }
            }
            OperationResult result = new OperationResult(OperationResultType.Success);
            result = userService.InsertApplication(applicant);
            applicantSpecialist.ApplicantId = applicant.ID;
            return result;
        }



        /// <summary>
        /// 保存ApplicantContact实体并发送邮件
        /// </summary>
        /// <param name="applicantContactEntity"></param>
        /// <param name="emailTemplateName"></param>
        /// <returns></returns>
        public OperationResult SaveApplicantContact(ApplicantContactEntity applicantContactEntity, string emailTemplateName)
        {
            ApplicantEntity applicant = userService.GetAppliant(applicantContactEntity.ApplicantId);
            string emailBody = "";
            if (!string.IsNullOrEmpty(emailTemplateName))
            {
                EmailTemplete template = XmlHelper.GetEmailTemplete(emailTemplateName);
                string subject = template.Subject;
                emailBody = template.Body
                            .Replace("{ApplicantFirstName}", applicantContactEntity.FirstName)
                            .Replace("{ApplicantLastName}", applicantContactEntity.LastName)
                            .Replace("{ApplicantEmail}", applicantContactEntity.Email)
                            .Replace("{CliEngageUrl}", SFConfig.MainSiteDomain)
                            .Replace("{StaticDomain}", SFConfig.StaticDomain);
                SendEmail(applicantContactEntity.Email, subject, emailBody);

                ApplicantEmailEntity applicantEmail = new ApplicantEmailEntity();
                applicantEmail.EmailAddress = applicantContactEntity.Email;
                applicantEmail.EmailContent = emailBody;
                applicant.ApplicantEmails.Add(applicantEmail);
            }
            else
            {
                SendEmailtoCLI(applicant);
            }
            OperationResult result = new OperationResult(OperationResultType.Success);
            applicant.ApplicantContacts = new List<ApplicantContactEntity>();
            applicant.ApplicantContacts.Add(applicantContactEntity);
            if (applicant.RoleType == Role.Teacher || applicant.RoleType == Role.TRS_Specialist ||
                applicant.RoleType == Role.School_Specialist)
                applicant.CommunityId = applicantContactEntity.CommunityId.Value;
            if (applicant.RoleType == Role.District_Community_Specialist)
            {
                applicant.Address = applicantContactEntity.Address;
                applicant.Address2 = applicantContactEntity.Address2;
                applicant.City = applicantContactEntity.City;
                applicant.StateId = applicantContactEntity.StateId;
                applicant.Zip = applicantContactEntity.Zip;
            }
            result = userService.UpdateAppliant(applicant);
            return result;
        }
        #endregion

        #region Sign Init Methods
        public OperationResult RegisterUser(ApplicantEntity applicant)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            result = userService.InsertApplication(applicant);
            return result;
        }

        public OperationResult RegisterUserEmail(ApplicantEmailEntity applicant)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            result = userService.InsertApplicantEmail(applicant);
            return result;
        }

        public OperationResult RegisterUserContact(ApplicantContactEntity applicantContact)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            result = userService.InsertApplicantContact(applicantContact);
            return result;
        }

        public OperationResult UpdateApplicant(ApplicantEntity applicant)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            result = userService.UpdateAppliant(applicant);
            return result;
        }
        #endregion

        #region User Verification
        public OperationResult UserVerification(ApplicantEntity applicant, int[] packageId, int currentUserId)
        {
            UserBaseEntity user = new UserBaseEntity();
            user.PermissionRoles = new List<PermissionRoleEntity>();
            if (packageId != null)
            {
                foreach (var item in packageId)
                {
                    PermissionRoleEntity permissionRole = new PermissionRoleEntity();
                    permissionRole = permissionBusiness.GetRole(item);
                    user.PermissionRoles.Add(permissionRole);
                }
            }
            switch (applicant.RoleType)
            {
                case Role.Community:
                case Role.District_Community_Specialist:
                    CommunityUserEntity communityUser = new CommunityUserEntity();
                    ApplicantToUser(applicant, user, currentUserId);
                    ApplicantToCommunity(applicant, communityUser);
                    user.CommunityUser = communityUser;
                    break;
                case Role.Principal:
                case Role.TRS_Specialist:
                case Role.School_Specialist:
                    PrincipalEntity principal = new PrincipalEntity();
                    ApplicantToUser(applicant, user, currentUserId);
                    ApplicantToPrincipal(applicant, principal);
                    user.Principal = principal;
                    break;
                case Role.Teacher:
                    TeacherEntity teacher = new TeacherEntity();
                    ApplicantToUser(applicant, user, currentUserId);
                    ApplicantToTeacher(applicant, teacher);
                    user.TeacherInfo = teacher;
                    break;
            }

            OperationResult result = new OperationResult(OperationResultType.Success);
            userService.InsertUser(user);
            applicant.InviteeId = user.ID;
            result = userService.UpdateAppliant(applicant);
            return result;
        }

        private void ApplicantToUser(ApplicantEntity applicant, UserBaseEntity user, int currentUserId)
        {
            user.FirstName = applicant.FirstName;
            user.LastName = applicant.LastName;
            user.PrimaryEmailAddress = applicant.Email;
            user.Status = EntityStatus.Active;
            user.Role = applicant.RoleType;
            user.Sponsor = applicant.SponsorId;
            user.InvitationEmail = InvitationEmailEnum.Sent;
            user.Notes = RegisterType.Normal.ToDescription();
            user.EmailExpireTime = DateTime.Now;

            //Principal只跟School有关系，所以需要将Community设置为0
            int communityId = 0;
            if (applicant.RoleType != Role.Principal && applicant.RoleType != Role.School_Specialist &&
                applicant.RoleType != Role.TRS_Specialist)
            {
                communityId = applicant.CommunityId;
            }
            user.UserCommunitySchools = new List<UserComSchRelationEntity>();
            user.UserCommunitySchools.Add(new UserComSchRelationEntity()
            {
                CommunityId = communityId,
                SchoolId = applicant.SchoolId,
                Status = EntityStatus.Active,
                CreatedBy = currentUserId,
                UpdatedBy = currentUserId,
            });
        }

        private void ApplicantToCommunity(ApplicantEntity applicant, CommunityUserEntity communityUser)
        {
            communityUser.CommunityUserId = CommunityUserCode();
            communityUser.SchoolYear = CommonAgent.SchoolYear;
            communityUser.Address = applicant.Address;
            communityUser.Address2 = applicant.Address2;
            communityUser.City = applicant.City;
            communityUser.StateId = applicant.StateId;
            communityUser.Zip = applicant.Zip;
            communityUser.PositionId = applicant.PositionId;
            communityUser.PositionOther = applicant.PositionOther;
            communityUser.BirthDate = DateTime.Parse("01/01/1753");
        }

        private void ApplicantToPrincipal(ApplicantEntity applicant, PrincipalEntity principal)
        {
            principal.PrincipalId = PrincipalCode();
            principal.SchoolYear = CommonAgent.SchoolYear;
            principal.Address = applicant.Address;
            principal.Address2 = applicant.Address2;
            principal.City = applicant.City;
            principal.StateId = applicant.StateId;
            principal.Zip = applicant.Zip;
            principal.PositionId = applicant.PositionId;
            principal.BirthDate = DateTime.Parse("01/01/1753");
        }

        private void ApplicantToTeacher(ApplicantEntity applicant, TeacherEntity teacher)
        {
            teacher.SchoolYear = CommonAgent.SchoolYear;
            teacher.HomeMailingAddress = applicant.Address;
            teacher.HomeMailingAddress2 = applicant.Address2;
            teacher.City = applicant.City;
            teacher.StateId = applicant.StateId;
            teacher.Zip = applicant.Zip;
            teacher.BirthDate = DateTime.Parse("01/01/1753");
        }
        #endregion
    }
}
