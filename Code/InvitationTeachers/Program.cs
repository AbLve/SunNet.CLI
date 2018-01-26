using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Configuration;

using Sunnet.Cli.Core;
using Sunnet.Cli.Core.Common.Enums;
using Sunnet.Cli.Core.Users;
using Sunnet.Cli.Core.Users.Entities;
using Sunnet.Framework.Helpers;
using Sunnet.Framework.Encrypt;
using StructureMap;
using Sunnet.Framework;
using Sunnet.Framework.EmailSender;
using Sunnet.Cli.Core.Users.Enums;
using Sunnet.Framework.Log;

namespace InvitationTeachers
{
    class Program
    {
        static void Main(string[] args)
        {
            IoC.Init();

            ISunnetLog LoggerHelper = ObjectFactory.GetInstance<ISunnetLog>();

            LoggerHelper.Info("Start-------------------");

            string templatePath = ConfigurationManager.AppSettings["TemplatePath"] + "";


            if (templatePath.Trim() == string.Empty)
            {
                LoggerHelper.Debug("TemplatePath is empty");
                Console.WriteLine("TemplatePath is empty");
                return;
            }

            IUserContract userService = DomainFacade.CreateUserService();

            string ResetSendEmail = ConfigurationManager.AppSettings["ResetSendEmail"] + "";
            if (ResetSendEmail == "1")//重新发送
            {
                string SentTime = ConfigurationManager.AppSettings["SentTime"] + "";
                userService.ResetEmail(int.Parse(SentTime));
            }

            //发送 statewide,auditor,community user,community specialist,principal,school specialist,teacher
            List<TeacherModel> list = userService.BaseUsers.Where(r =>
                (r.Role == Role.Statewide || r.Role == Role.Auditor || r.Role == Role.Community || r.Role == Role.District_Community_Specialist
                || r.Role == Role.Principal || r.Role == Role.School_Specialist || r.Role == Role.Teacher)
                && r.Status == EntityStatus.Active
                && r.GoogleId == string.Empty && r.InvitationEmail == InvitationEmailEnum.Pending)
                .Select(r =>
                new TeacherModel()
                {
                    ID = r.ID,
                    UserId = r.ID,
                    FirstName = r.FirstName,
                    LastName = r.LastName,
                    Eamil = r.PrimaryEmailAddress
                }).ToList();

            try
            {
                EmailTemplete template = XmlHelper.GetEmailTemplete(templatePath, "TeacherInvitation_Template.xml");

                foreach (TeacherModel teacher in list)
                {
                    string param = teacher.UserId.ToString() + "," + DateTime.Now.ToString();
                    string encryptParam = ObjectFactory.GetInstance<IEncrypt>().Encrypt(param);
                    string link = SFConfig.MainSiteDomain + "Home/InviteVerification/"
                        + System.Web.HttpUtility.UrlEncode(encryptParam);

                    string emailBody = template.Body.Replace("{FirstName}", teacher.FirstName)
                    .Replace("{LastName}", teacher.LastName)
                    .Replace("{InviteLink}", "<a style='text-decoration: underline; cursor:pointer; color: #008000;' href='" + link + "'>Click here</a>")
                    .Replace("{StaticDomain}", SFConfig.StaticDomain)
                    .Replace("{CliEngageUrl}", SFConfig.MainSiteDomain);

                    if (System.Text.RegularExpressions.Regex.IsMatch(teacher.Eamil, @"\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"))
                    {
                        try
                        {
                            var emailSender = ObjectFactory.GetInstance<IEmailSender>();
                            emailSender.SendMail(teacher.Eamil, template.Subject, emailBody);

                            string ExpirationTime = ConfigurationManager.AppSettings["ExpirationTime"] + "";

                            userService.UpdateInvitationEmail(int.Parse(ExpirationTime), teacher.ID);
                            userService.InsertEmailLog(new EmailLogEntity(teacher.ID, teacher.Eamil, EmailLogType.Batch));
                        }
                        catch (Exception mailEx)
                        {
                            LoggerHelper.Debug(mailEx);
                        }
                    }
                    else
                        LoggerHelper.Debug(string.Format("{0}  email error", teacher.Eamil));
                }
            }
            catch (Exception ex)
            {
                LoggerHelper.Debug(ex);
                Console.WriteLine(ex);
            }

            LoggerHelper.Info("stop ------------------");
        }
    }

    class TeacherModel
    {
        public TeacherModel()
        {

        }

        public int ID { get; set; }
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Eamil { get; set; }
    }
}
