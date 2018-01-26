using StructureMap;
using StructureMap.Pipeline;
using Sunnet.Framework.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;

namespace Sunnet.Framework.EmailSender
{
    internal class SmtpClientEmailSender : IEmailSender
    {
        public ISunnetLog Logger
        {
            get;
            set;
        }

        public SmtpClientEmailSender(ISunnetLog logger)
        {
            Logger = logger;
        }


        public bool SendMail(MailAddressCollection To, MailAddressCollection ToCc, MailAddressCollection ToBcc, string displayName
           , string subject, string body, string attachFiles, bool IsBodyHtml, System.Net.Mail.MailPriority priority)
        {
            if (To.Count() == 0)
            {
                //errorMessage = "Missing \"To\" address.";
                return false;
            }

            //errorMessage = "";
            bool flag = true;
            MailMessage mail = new MailMessage();
            mail.IsBodyHtml = IsBodyHtml;
            mail.BodyEncoding = Encoding.UTF8;
            mail.Priority = priority;
            foreach (var v in To)
            {
                mail.To.Add(v);
            }
            foreach (var v in ToCc)
                mail.CC.Add(v);
            //mail.From = new MailAddress(from, displayName);
            if (string.IsNullOrEmpty(displayName))
            {
                if (!string.IsNullOrEmpty(SFConfig.EmailDisplayName))
                    displayName = SFConfig.EmailDisplayName;
            }
            mail.From = new MailAddress(SFConfig.FromEmailAddress, displayName);

            foreach (var v in ToBcc)
                mail.Bcc.Add(v);

            mail.Subject = subject;
            mail.Body = body;
            if (!string.IsNullOrEmpty(attachFiles))
            {
                foreach (string s in attachFiles.Split(';'))
                {
                    if (string.IsNullOrEmpty(s)) continue;
                    Attachment attach = new Attachment(s);
                    mail.Attachments.Add(attach);
                }
            }
            SmtpClient client = new SmtpClient();

            client.EnableSsl = SFConfig.EmailSSL;
            try
            {
                client.Send(mail);
                if (SFConfig.TestMode)
                {
                    Logger.Info(string.Format("To:{0}  On:{1}  Sent Success", To, DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss")));
                    Logger.Info(string.Format("Body: {0}", body));
                }
            }
            catch (Exception ex)
            {
                if (SFConfig.TestMode)
                {
                    Logger.Info(string.Format("To:{0}  On:{1}  Sent Fail", To, DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss")));
                    Logger.Info(string.Format("Body: {0}", body));
                }
                Logger.Debug(ex);
                flag = false;
            }
            finally
            {
                mail.Dispose();
            }
            return flag;
        }

        private string ListToString(IEnumerable<string> listInput)
        {
            string output = "";
            foreach (string s in listInput)
            {
                output += s + ";";
            }
            return output;
        }

        public bool SendMail(string to, string toCc, string toBcc, string replyTo, string from, string displayName
            , string subject, string body, string attachFiles, bool isBodyHtml, System.Net.Mail.MailPriority priority)
        {
            if (from == null) from = "";


            to = to.ToLower().Replace(";" + from.ToLower(), "").Replace(from.ToLower() + ";", "");
            if (to.Length == 0 && toCc.Length == 0 && toBcc.Length == 0)
            {
                return false;
            }
            if (from.Length == 0)
            {
                return false;
            }
            bool flag = true;
            MailMessage mail = new MailMessage();
            mail.IsBodyHtml = isBodyHtml;
            mail.BodyEncoding = Encoding.UTF8;
            mail.Priority = priority;
            if (to.Length != 0)
            {
                foreach (string s in to.Split(';'))
                {
                    if (string.IsNullOrEmpty(s)) continue;
                    mail.To.Add(s);
                }
            }
            if (toCc.Length != 0)
            {
                foreach (string s in toCc.Split(';'))
                {
                    if (string.IsNullOrEmpty(s)) continue;
                    mail.CC.Add(s);
                }
            }
            mail.From = new MailAddress(from, displayName);
            if (toBcc.Length != 0)
            {
                foreach (string s in toBcc.Split(';'))
                {
                    if (string.IsNullOrEmpty(s)) continue;
                    mail.Bcc.Add(s);
                }
            }
            mail.Subject = subject;
            mail.Sender = mail.From;
            mail.Body = body;
            //mail.ReplyToList.Add(replyTo);
            if (attachFiles.Length != 0)
            {
                foreach (string s in attachFiles.Split(';'))
                {
                    if (string.IsNullOrEmpty(s)) continue;
                    Attachment attach = new Attachment(s);
                    if (s.IndexOf("/") > 0)
                    {
                        int startIndex = s.LastIndexOf("/") + 1;
                        attach.Name = s.Substring(startIndex);
                    }
                  
                    mail.Attachments.Add(attach);
                }
            }
            SmtpClient client = new SmtpClient();
            client.EnableSsl = SFConfig.EmailSSL; 
            try
            {
                if (SFConfig.TestMode && SFConfig.TestModeEmail != null && SFConfig.TestModeEmail.Trim().Length > 0)
                {
                    body = string.Format("{0}\r\n<br/> This is an email send from test website,it's really send to {1},bcc {2},cc{3}\r\n",
                        body,
                        string.Join(",", mail.To.Select(x => x.Address).ToArray()),
                        string.Join(",", mail.CC.Select(x => x.Address).ToArray()),
                        string.Join(",", mail.Bcc.Select(x => x.Address).ToArray()));
                    mail.Body = body;
                    mail.To.Clear();
                    var testReceivers = SFConfig.TestModeEmail.Split(";".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                    foreach (string s in testReceivers)
                        mail.To.Add(s);
                    mail.CC.Clear();
                    mail.Bcc.Clear();
                }
                client.Send(mail);
                if (SFConfig.TestMode)
                {
                    Logger.Info(string.Format("To:{0}  On:{1}  Sent Success", to, DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss")));
                    Logger.Info(string.Format("Body: {0}", body));
                }
            }
            catch (Exception ex)
            {
                if (SFConfig.TestMode)
                {
                    Logger.Info(string.Format("To:{0}  On:{1}  Sent Fail", to, DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss")));
                    Logger.Info(string.Format("Body: {0}", body));
                }

                Logger.Debug(ex);
                flag = false;
            }
            finally
            {
                mail.Dispose();
            }
            return flag;
        }

        public bool SendMail(string to, string subject, string body, bool isBodyHtml = true)
        {
            return SendMail(to, "", "", "", SFConfig.FromEmailAddress, SFConfig.EmailDisplayName, subject, body, "", isBodyHtml, MailPriority.Normal);
        }

        public bool SendMail(string to, string displayName, string subject, string body, string attachfiles, bool isBodyHtml = true)
        {
            return SendMail(to, "", "", "", SFConfig.FromEmailAddress, displayName, subject, body, attachfiles, isBodyHtml, MailPriority.Normal);
        }

        public bool SendMail(string to,string bcc,string subject, string body, bool isBodyHtml = true)
        {
            return SendMail(to, "", bcc, "", SFConfig.FromEmailAddress, SFConfig.EmailDisplayName, subject, body, "", isBodyHtml, MailPriority.Normal);
        }
    }
}
