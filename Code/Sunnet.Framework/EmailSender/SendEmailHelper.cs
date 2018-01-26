using Sunnet.Framework.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;

namespace Sunnet.Framework.EmailSender
{
    public class SendEmailHelper
    {
        ISunnetLog logger;
        public SendEmailHelper(ISunnetLog logger)
        {
            this.logger = logger;
        }

        public  bool Send(MailMessage mail)
        {
            try
            {
                SmtpClientEmailSender sces = new SmtpClientEmailSender(logger);
                string attachments = "";
                if (mail.Attachments.Count > 0)
                {
                    for (int i = 0; i < mail.Attachments.Count; i++)
                    {
                        attachments += mail.Attachments[i].ToString() + ";";
                    }
                    if (attachments.EndsWith(";"))
                    {
                        attachments = attachments.Substring(0, attachments.Length - 1);
                    }
                }
                return sces.SendMail
                     (mail.To, mail.CC, mail.Bcc, "", mail.Subject, mail.Body, attachments,
                     true, System.Net.Mail.MailPriority.Normal);
            }
            catch (Exception ex)
            {
                logger.Debug(ex);
                return false;
            }
        }
    }
}
