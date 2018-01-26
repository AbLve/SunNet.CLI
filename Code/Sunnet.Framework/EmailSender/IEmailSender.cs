using Sunnet.Framework.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;

namespace Sunnet.Framework.EmailSender
{
    public interface IEmailSender
    {

        ISunnetLog Logger
        {
            get;
            set;
        }

        bool SendMail(MailAddressCollection To, MailAddressCollection ToCc, MailAddressCollection ToBcc, string displayName, string subject
            , string body, string attachFiles, bool IsBodyHtml, MailPriority priority);

        bool SendMail(string to, string toCc, string toBcc, string replyTo, string from, string displayName, string subject, string body, string attachFiles, bool isBodyHtml, MailPriority priority);

        /// <summary>
        /// 发送邮件: 以系统统一的发件人身份发送.
        /// </summary>
        /// <param name="to">To.</param>
        /// <param name="subject">The subject.</param>
        /// <param name="body">The body.</param>
        /// <param name="isBodyHtml">if set to <c>true</c> [is body HTML].</param>
        /// <returns></returns>
        bool SendMail(string to, string subject, string body, bool isBodyHtml = true);


        /// <summary>
        /// 发送邮件: 自定义发件人姓名发送.
        /// </summary>
        /// <param name="to">To.</param>
        /// <param name="from">From.</param>
        /// <param name="displayName">The display name.</param>
        /// <param name="subject">The subject.</param>
        /// <param name="body">The body.</param>
        /// <param name="attachfiles">The attachfiles.</param>
        /// <param name="isBodyHtml">if set to <c>true</c> [is body HTML].</param>
        /// <returns></returns>
        bool SendMail(string to, string displayName, string subject, string body,
            string attachfiles, bool isBodyHtml = true);

        bool SendMail(string to,string bcc, string subject, string body, bool isBodyHtml = true);
    }
}
