using StructureMap;
using Sunnet.Cli.Business.Export;
using Sunnet.Cli.Core.Export.Entities;
using Sunnet.Cli.Core.Export.Enums;
using Sunnet.Framework;
using Sunnet.Framework.EmailSender;
using Sunnet.Framework.Encrypt;
using Sunnet.Framework.Helpers;
using Sunnet.Framework.Log;
using Sunnet.Framework.StringZipper;
using Sunnet.Framework.SFTP;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataExport
{
    internal delegate void QueueEventHandler();

    internal class QueueManager
    {
        internal event QueueEventHandler BeforeProcessQueues;
        internal event QueueEventHandler AfterProcessQueues;
        private IEmailSender emailSender;
        private ISunnetLog LoggerHelper;
        private IEncrypt encrypt;
        private ExportBusiness _exportBusiness;

        internal QueueManager()
        {
            emailSender = ObjectFactory.GetInstance<IEmailSender>();
            LoggerHelper = ObjectFactory.GetInstance<ISunnetLog>();
            encrypt = ObjectFactory.GetInstance<IEncrypt>();
            _exportBusiness = new ExportBusiness();
        }


        /// <summary>
        /// 发送邮件带有下载链接
        /// </summary>
        /// <param name="exportInfos"></param>
        private void SendEmail(List<ExportInfoEntity> exportInfos)
        {
            string tempPath = ConfigurationManager.AppSettings["TempPath"];
            EmailTemplete temp = XmlHelper.GetEmailTemplete(tempPath, "ExportEmail.xml");
            ExportInfoEntity exportInfo = exportInfos.FirstOrDefault();

            string subject = temp.Subject;
            string body = string.Empty;
            body = temp.Body.Replace("{FirstName}", exportInfo.CreaterFirstName + " " + exportInfo.CreaterLastName)
                .Replace("{Link}", SFConfig.MainSiteDomain + exportInfo.DownloadUrl);

            emailSender.SendMail(exportInfo.CreaterMail, subject, body);
            ExportBusiness _exportBusiness = new ExportBusiness();
            _exportBusiness.UpdateStatus(exportInfos, ExportInfoStatus.Sent);
        }

        /// <summary>
        /// 发送邮件带有附件
        /// </summary>
        /// <param name="exportInfos"></param>
        private void SendEmailWithAttach(List<ExportInfoEntity> exportInfos)
        {
            string tempPath = ConfigurationManager.AppSettings["TempPath"];
            ExportInfoEntity exportInfo = exportInfos.FirstOrDefault();
            EmailTemplete temp = XmlHelper.GetEmailTemplete(tempPath, "EmailWithAttach.xml");

            string subject = temp.Subject;
            string body = string.Empty;
            body = temp.Body.Replace("{FirstName}", exportInfo.CreaterFirstName + " " + exportInfo.CreaterLastName);

            string protectedFilePath = ConfigurationManager.AppSettings["ProtectedFiles"];

            string attachFile = protectedFilePath + exportInfo.FileUrl;
            emailSender.SendMail(exportInfo.CreaterMail, "Cli Support", subject, body, attachFile);

            ExportBusiness _exportBusiness = new ExportBusiness();
            _exportBusiness.UpdateStatus(exportInfos, ExportInfoStatus.Sent);
        }

        private void PutToSFTP(List<ExportInfoEntity> exportInfos)
        {
            ExportBusiness _exportBusiness = new ExportBusiness();
            ExportInfoEntity exportInfo = exportInfos.FirstOrDefault();

            SFTPHelper sfptHelper = new SFTPHelper
                (exportInfo.FtpHostIp, exportInfo.FtpPort, exportInfo.FtpUserName, encrypt.Decrypt(exportInfo.FtpPassword), LoggerHelper);
            try
            {
                string protectedFilePath = ConfigurationManager.AppSettings["ProtectedFiles"];
                string remoteDir = exportInfo.FtpFilePath;
                bool bconn = sfptHelper.Connect();
                //连接失败
                if (!bconn)
                {
                    SFTPFailEmail(exportInfos);
                    LoggerHelper.Info(string.Format("-->Connect to sftp failed.This time already execute the query. Group of {0} / No:{1}", exportInfo.GroupName, string.Join(",", exportInfos.Select(r => r.ID))));
                    _exportBusiness.UpdateStatus(exportInfos, ExportInfoStatus.SftpError);
                    return;
                }
                if (!sfptHelper.DirExist(remoteDir))
                    sfptHelper.MakeDir(remoteDir);
                string remoteFile = remoteDir.TrimEnd('/') + "/" + exportInfo.GroupName + ".zip";
                string localFile = protectedFilePath + exportInfo.FileUrl;
                if (bconn)
                {
                    bool putFile = sfptHelper.Put(localFile, remoteFile);
                    if (putFile)
                    {
                        LoggerHelper.Info("-->Put files to sftp success");
                        _exportBusiness.UpdateStatus(exportInfos, ExportInfoStatus.Sent);
                        //发邮件通知
                        string tempPath = ConfigurationManager.AppSettings["TempPath"];
                        EmailTemplete emailTemplate = XmlHelper.GetEmailTemplete(tempPath, "UploadToSFTPSuccess.xml");
                        var subject = emailTemplate.Subject;
                        string body = string.Empty;
                        body = emailTemplate.Body.Replace("{FirstName}", exportInfo.CreaterFirstName)
                        .Replace("{SFTPHostIp}", exportInfo.FtpHostIp);
                        emailSender.SendMail(exportInfo.CreaterMail, subject, body);
                    }
                    else
                    {
                        LoggerHelper.Info(string.Format("-->Put file to sftp failed. Group of {0} / No:{1}", exportInfo.GroupName, string.Join(",", exportInfos.Select(r => r.ID))));
                        _exportBusiness.UpdateStatus(exportInfos, ExportInfoStatus.SftpError);
                    }
                    sfptHelper.Disconnect();


                }
            }
            catch (Exception ex)
            {
                sfptHelper.Disconnect();
                _exportBusiness.UpdateStatus(exportInfos, ExportInfoStatus.SftpError);
                LoggerHelper.Debug(ex);
            }
        }

        private void CheckNotSend(List<ExportInfoEntity> exportInfos)
        {
            List<ExportInfoEntity> notSendExportInfos = exportInfos
                           .Where(x => x.Status == ExportInfoStatus.Processed).ToList();
            if (notSendExportInfos.Count() > 0)
            {
                LoggerHelper.Info("-->sftp or send mail");
                List<ExportInfoEntity> uploadExportInfos = notSendExportInfos.
                    Where(x => x.ReceiveFileBy == ReceiveFileBy.SFTP).ToList();
                if (uploadExportInfos.Count() > 0)
                {
                    PutToSFTP(uploadExportInfos.ToList());
                }

                List<ExportInfoEntity> emailExportInfos = notSendExportInfos.
                    Where(x => x.ReceiveFileBy == ReceiveFileBy.DownloadLink).ToList();
                if (emailExportInfos.Count() > 0)
                {
                    SendEmail(notSendExportInfos.ToList());
                }
            }
        }

        private bool TestSFTPConnet(ExportInfoEntity exportInfo)
        {
            SFTPHelper sfptHelper = new SFTPHelper
                (exportInfo.FtpHostIp, exportInfo.FtpPort, exportInfo.FtpUserName, encrypt.Decrypt(exportInfo.FtpPassword), LoggerHelper);
            bool bconn = sfptHelper.Connect();
            //连接失败
            if (bconn)
                sfptHelper.Disconnect();
            return bconn;
        }

        private void SFTPFailEmail(List<ExportInfoEntity> exportInfos)
        {
            string tempPath = ConfigurationManager.AppSettings["TempPath"];
            ExportInfoEntity exportInfo = exportInfos.FirstOrDefault();
            EmailTemplete emailTemplate = XmlHelper.GetEmailTemplete(tempPath, "SFTPConnctFail.xml");
            var subject = emailTemplate.Subject;
            string body = string.Empty;

            body = emailTemplate.Body.Replace("{FirstName}", exportInfo.CreaterFirstName)
            .Replace("{SFTPHostIp}", exportInfo.FtpHostIp).Replace("{SFTPPort}", exportInfo.FtpPort.ToString())
            .Replace("{SFTPUserName}", exportInfo.FtpUserName);
            emailSender.SendMail(exportInfo.CreaterMail, subject, body);
        }

        private static void CreatePath(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        internal void Start()
        {
            BeforeProcessQueues?.Invoke();
            LoggerHelper.Info("-->Prepare");
            List<ExportInfoEntity> exportInfos = new List<ExportInfoEntity>();
            try
            {
                string logPath = ConfigurationManager.AppSettings["LogFile"] + "";
                string protectedFilePath = ConfigurationManager.AppSettings["ProtectedFiles"];
                string zipFilePath = protectedFilePath + "export/zip/";
                string csvFilePath = protectedFilePath + "export/csv/";
                CreatePath(csvFilePath);
                CreatePath(zipFilePath);

                LoggerHelper.Info("-->Read data from the database");
                List<ExportInfoEntity> notProcessExportInfos = _exportBusiness.GetNotProcessExportInfos();

                //已处理完成，但未发送邮件或者未上传到sftp的
                CheckNotSend(notProcessExportInfos);

                notProcessExportInfos = notProcessExportInfos
                    .Where(x => x.CreatedOn < DateTime.Now.AddMinutes(-1) && x.Status < ExportInfoStatus.Processed)
                    .ToList();

               
                if (notProcessExportInfos.Count() == 0)
                {
                    LoggerHelper.Info("-->There is no data need to export");
                    AfterProcessQueues?.Invoke();
                    return;
                }

                List<string> groupNames = notProcessExportInfos.Select(x => x.GroupName).Distinct().ToList();
                LoggerHelper.Info("-->A total of " + groupNames.Count + " groups of data need to export");
                foreach (string groupName in groupNames)
                {
                    string csvGroupFilePath = csvFilePath + groupName + "/";
                    CreatePath(csvGroupFilePath);
                    //分组，用于打包 
                    exportInfos = notProcessExportInfos.Where(x => x.GroupName == groupName).ToList();

                    //测试SFTP
                    if (exportInfos.Count(e => e.ReceiveFileBy == ReceiveFileBy.SFTP) >= 1)
                    {
                        if (!TestSFTPConnet(exportInfos.FirstOrDefault()))
                        {
                            SFTPFailEmail(exportInfos);
                            _exportBusiness.UpdateStatus(exportInfos, ExportInfoStatus.SftpError);
                            LoggerHelper.Info(
                                string.Format(
                                    "Connect to sftp failed. This time haven't execute the query. groupName:{0}",
                                    groupName));
                            continue;
                        }
                    }

                    LoggerHelper.Info(string.Format("-->Start export by group of {0} / No:{1}", groupName,
                        string.Join(",", exportInfos.Select(r => r.ID))));

                    _exportBusiness.UpdateStatus(exportInfos, ExportInfoStatus.Processing);

                    foreach (ExportInfoEntity exportInfo in exportInfos)
                    {
                        DataSet ds = _exportBusiness.GetQueryDataBySql(exportInfo.ExecuteSQL);
                        string fileFullPath = csvGroupFilePath + Guid.NewGuid().ToString() + ".csv";
                        _exportBusiness.ExportToCsv(exportInfo.FileType, ds.Tables[0], fileFullPath);
                    }

                    CSharpCodeStringZipper.CreateZip(zipFilePath, groupName + ".zip", csvGroupFilePath);

                    string downLoadLink = "Export/Export/DowloadFile/{id}";
                    exportInfos.ForEach(x =>
                    {
                        x.Status = ExportInfoStatus.Processed;
                        x.FileUrl = "export/zip/" + groupName + ".zip";
                        x.DownloadUrl = downLoadLink.Replace("{id}", encrypt.Encrypt(x.ID.ToString()));
                        x.UpdatedOn = DateTime.Now;
                    });
                    _exportBusiness.UpdateExportInfos(exportInfos);
                    LoggerHelper.Info("-->Group of " + groupName + " export success");

                    ExportInfoEntity exportEntity = exportInfos.FirstOrDefault();
                    switch (exportEntity.ReceiveFileBy)
                    {
                        case ReceiveFileBy.DownloadLink:
                            LoggerHelper.Info("-->Begin to send E-mail");
                            SendEmail(exportInfos);
                            LoggerHelper.Info("-->Send E-mail success");
                            break;
                        case ReceiveFileBy.SFTP:
                            LoggerHelper.Info("-->Begin upload files to sftp");
                            PutToSFTP(exportInfos);
                            break;
                    }
                }
                LoggerHelper.Info("-->Export end");
            }
            catch (Exception ex)
            {
                var subject = "An email with an exception about DataExport CLI.  ";
                if (exportInfos.Count>0)
                {
                     subject = "An email with an exception about DataExport CLI.The group name is " +
                             exportInfos[0].GroupName;
                }
               
                string body = ex.ToString();
                string toEmail = ConfigurationManager.AppSettings["ExceptionEmail"];
                emailSender.SendMail(toEmail, subject, body);

                _exportBusiness.UpdateStatus(exportInfos, ExportInfoStatus.Error);
                LoggerHelper.Debug(ex);
            }
            finally
            {
                AfterProcessQueues?.Invoke();
            }
        }
    }
}