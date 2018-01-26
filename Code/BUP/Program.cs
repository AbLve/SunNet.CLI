using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Collections;
using Sunnet.Cli.Core.BUP;
using Sunnet.Cli.Business.BUP;
using Sunnet.Framework;
using Sunnet.Cli.Core.BUP.Enums;
using Sunnet.Cli.Core.BUP.Entities;
using Sunnet.Framework.Encrypt;
using StructureMap;
using Sunnet.Framework.Log;
using Sunnet.Cli.Business.BUP.Model;
using Sunnet.Cli.Business.Users;
using Sunnet.Cli.Core.Users.Entities;
using Sunnet.Framework.Helpers;
using Sunnet.Framework.EmailSender;
using Sunnet.Framework.SFTP;
using Sunnet.Cli.Core;

namespace BUP
{
    public class Program
    {
        static void Main(string[] args)
        {
            IoC.Init();

            string filePath = string.IsNullOrEmpty(ConfigurationManager.AppSettings["UploadFile"]) ?
           "C:/log/cli/Upload/data_Process/" : ConfigurationManager.AppSettings["UploadFile"];

            BUPProcessBusiness _bupProcessBusiness = new BUPProcessBusiness();
            BUPTaskBusiness _bupTaskBusiness = new BUPTaskBusiness();
            AutomationSettingBusiness _autoBusiness = new AutomationSettingBusiness();
            List<AutomationSettingModel> processingAutomations = _autoBusiness.GetProcessingAutomationSettings();
            UserBusiness _userBusiness = new UserBusiness();
            ISunnetLog LoggerHelper = ObjectFactory.GetInstance<ISunnetLog>();

            LoggerHelper.Info("start ......");
            Console.WriteLine("start ......");

            if (processingAutomations != null && processingAutomations.Count > 0)
            {
                List<UserBaseEntity> users = _userBusiness.GetUsers(processingAutomations.Select(r => r.CreatedBy).ToList());
                foreach (AutomationSettingModel item in processingAutomations)
                {
                    IEncrypt encrypt = ObjectFactory.GetInstance<IEncrypt>();
                    SFTPHelper sftp = new SFTPHelper(item.HostIp, item.Port, item.UserName, encrypt.Decrypt(item.PassWord), LoggerHelper);

                    bool bconn = sftp.Connect();
                    if (bconn)
                    {
                        try
                        {
                            UserBaseEntity user = users.Find(r => r.ID == item.CreatedBy);
                            if (user != null)
                            {
                                Dictionary<string, dynamic> dicType = GetDictionary(item);

                                foreach (var type in dicType)
                                {
                                    if (sftp.DirExist(type.Key))
                                    {
                                        string localPath = filePath + (item.CommunityName + "/" + type.Key.Replace("/", "").Replace("\\", ""))
                                            + "/" + DateTime.Now.ToString("MM-dd-yyyy") + "/";
                                        string failedPath = type.Key + "/Failed/" + DateTime.Now.ToString("MM-dd-yyyy") + "/";
                                        string successPath = type.Key + "/Processed/" + DateTime.Now.ToString("MM-dd-yyyy") + "/";

                                        //从sftp获取文件到本地目录
                                        List<string> objList = new List<string>();
                                        List<string> LocalFileList = new List<string>();
                                        objList = sftp.GetFileList(type.Key, new string[] { ".xls", ".xlsx" });
                                        if (objList.Count > 0)
                                        {
                                            Helper.CheckAndCreatePath(localPath);
                                            foreach (object obj in objList)
                                            {
                                                string fileUrl = obj.ToString();
                                                string newFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff") + "--" + fileUrl;
                                                sftp.Get(type.Key + "/" + fileUrl, localPath + "/" + newFileName);
                                                LocalFileList.Add(newFileName);
                                                sftp.DeleteFile(type.Key + "/" + fileUrl);
                                            }

                                            if (Directory.Exists(localPath))
                                            {
                                                DirectoryInfo dr = new DirectoryInfo(localPath);
                                                foreach (FileInfo file in dr.GetFiles()
                                                    .Where(r => r.Extension.ToLower() == ".xls" || r.Extension.ToLower() == ".xlsx"
                                                        && LocalFileList.Contains(r.Name)))
                                                {
                                                    Console.WriteLine(string.Format("Start processing {0}", file.FullName));
                                                    LoggerHelper.Info(string.Format("Start processing {0}", file.FullName));
                                                    //导入数据到BUP表
                                                    DataTable dt = new DataTable();
                                                    string errorMsg = "";
                                                    errorMsg = _bupProcessBusiness.InvalidateFile(file.FullName, type.Value.Count, type.Value.Type, out dt);
                                                    if (!string.IsNullOrEmpty(errorMsg))
                                                    {
                                                        Console.WriteLine(string.Format("Result: Error. Message: {0}", errorMsg));
                                                        LoggerHelper.Info(string.Format("Result: Error. Message: {0}", errorMsg));
                                                        WriteErrorMsg(sftp, localPath, failedPath, file, errorMsg);
                                                        continue;
                                                    }

                                                    int identity = 0;
                                                    string originFileName = file.Name.Substring(file.Name.IndexOf("--") + 2);
                                                    //Automation   默认发送邀请
                                                    switch ((BUPType)type.Value.Type)
                                                    {
                                                        case BUPType.School:
                                                            errorMsg = _bupProcessBusiness.ProcessSchool(dt, user.ID, originFileName, file.Name, user,
                                                                out identity, BUPProcessType.Automation,item.CommunityId);
                                                            break;
                                                        case BUPType.Classroom:
                                                            errorMsg = _bupProcessBusiness.ProcessClassroom(dt, user.ID, originFileName, file.Name, user,
                                                                item.CommunityId, out identity, BUPProcessType.Automation);
                                                            break;
                                                        case BUPType.Class:
                                                            errorMsg = _bupProcessBusiness.ProcessClass(dt, user.ID, originFileName, file.Name, user,
                                                                out identity, BUPProcessType.Automation, item.CommunityId);
                                                            break;
                                                        case BUPType.Teacher:
                                                            errorMsg = _bupProcessBusiness.ProcessTeacher(dt, user.ID, "1", originFileName, file.Name, user,
                                                                 out identity, BUPProcessType.Automation, item.CommunityId);
                                                            break;
                                                        case BUPType.Student:
                                                            errorMsg = _bupProcessBusiness.ProcessStudent(dt, user.ID, originFileName, file.Name, user,
                                                                 out identity, BUPProcessType.Automation, item.CommunityId);
                                                            break;
                                                        case BUPType.CommunityUser:
                                                            errorMsg = _bupProcessBusiness.ProcessCommunityUser(dt, user.ID, false, "1", originFileName, file.Name,
                                                                user, item.CommunityId, out identity, BUPProcessType.Automation);
                                                            break;
                                                        case BUPType.CommunitySpecialist:
                                                            errorMsg = _bupProcessBusiness.ProcessCommunityUser(dt, user.ID, true, "1", originFileName, file.Name,
                                                                user, item.CommunityId, out identity, BUPProcessType.Automation);
                                                            break;
                                                        case BUPType.Principal:
                                                            errorMsg = _bupProcessBusiness.ProcessPrincipal(dt, user.ID, false, originFileName, file.Name, "1",
                                                                user, item.CommunityId, out identity, BUPProcessType.Automation);
                                                            break;
                                                        case BUPType.SchoolSpecialist:
                                                            errorMsg = _bupProcessBusiness.ProcessPrincipal(dt, user.ID, true, originFileName, file.Name, "1",
                                                                user, item.CommunityId, out identity, BUPProcessType.Automation);
                                                            break;
                                                        case BUPType.Parent:
                                                            errorMsg = _bupProcessBusiness.ProcessParent(dt, user.ID, originFileName, file.Name,
                                                                out identity, BUPProcessType.Automation);
                                                            break;
                                                        default:
                                                            errorMsg = "Can not find this action: " + type.Value.Type;
                                                            break;
                                                    }
                                                    if (!string.IsNullOrEmpty(errorMsg))
                                                    {
                                                        Console.WriteLine(string.Format("Result: Error. Message: {0}", errorMsg));
                                                        LoggerHelper.Info(string.Format("Result: Error. Message: {0}", errorMsg));
                                                        WriteErrorMsg(sftp, localPath, failedPath, file, errorMsg);
                                                        continue;
                                                    }

                                                    //执行数据导入操作
                                                    try
                                                    {
                                                        if (identity > 0)
                                                        {
                                                            ProcessHandler handler = new ProcessHandler(BUPTaskBusiness.Start);
                                                            IAsyncResult asyncResult = handler.BeginInvoke(identity, user.ID, null, null);
                                                            handler.EndInvoke(asyncResult);  //此方法会等待异步执行完成后再往下执行
                                                            ProcessData(identity, user, LoggerHelper, sftp, successPath, file);
                                                        }
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        Console.WriteLine(string.Format("Result: Error. Message: {0}", ex.Message));
                                                        LoggerHelper.Info(string.Format("Result: Error. Message: {0}", errorMsg));
                                                        WriteErrorMsg(sftp, localPath, failedPath, file, ex.Message);
                                                        continue;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                sftp.Disconnect();
                            }
                            else
                            {
                                LoggerHelper.Debug("Can not find user with the ID: " + item.CreatedBy + ". DateTime: " + DateTime.Now.ToString());
                            }

                        }
                        catch (Exception ex)
                        {
                            if (sftp.IsConnected)
                                sftp.Disconnect();
                            Console.WriteLine(ex.Message);
                            LoggerHelper.Info(string.Format("Result: Error. Message: {0}", ex.Message));
                            LoggerHelper.Debug(ex);
                        }
                    }
                    else
                    {
                        LoggerHelper.Info("can not connect to the sftp server");
                        Console.WriteLine("can not connect to the sftp server");
                    }
                }
            }
            LoggerHelper.Info("end ......");
            Console.WriteLine("end ......");
        }

        private static Dictionary<string, dynamic> GetDictionary(AutomationSettingModel item)
        {
            Dictionary<string, dynamic> dicType = new Dictionary<string, dynamic>();
            if (!string.IsNullOrEmpty(item.SchoolPath.Trim()))
                dicType[item.SchoolPath.Trim()] = new { Type = BUPType.School, Count = 38 };
            if (!string.IsNullOrEmpty(item.ClassroomPath.Trim()))
                dicType[item.ClassroomPath.Trim()] = new { Type = BUPType.Classroom, Count = 9 };
            if (!string.IsNullOrEmpty(item.ClassPath.Trim()))
                dicType[item.ClassPath.Trim()] = new { Type = BUPType.Class, Count = 16 };
            if (!string.IsNullOrEmpty(item.StudentPath.Trim()))
                dicType[item.StudentPath.Trim()] = new { Type = BUPType.Student, Count = 22 };
            if (!string.IsNullOrEmpty(item.CommunityUserPath.Trim()))
                dicType[item.CommunityUserPath.Trim()] = new { Type = BUPType.CommunityUser, Count = 15 };
            if (!string.IsNullOrEmpty(item.CommunitySpecialistPath.Trim()))
                dicType[item.CommunitySpecialistPath.Trim()] = new { Type = BUPType.CommunitySpecialist, Count = 15 };
            if (!string.IsNullOrEmpty(item.PrincipalPath.Trim()))
                dicType[item.PrincipalPath.Trim()] = new { Type = BUPType.Principal, Count = 15 };
            if (!string.IsNullOrEmpty(item.SchoolSpecialistPath.Trim()))
                dicType[item.SchoolSpecialistPath.Trim()] = new { Type = BUPType.SchoolSpecialist, Count = 15 };
            if (!string.IsNullOrEmpty(item.TeacherPath.Trim()))
                dicType[item.TeacherPath.Trim()] = new { Type = BUPType.Teacher, Count = 21 };
            if (!string.IsNullOrEmpty(item.ParentPath.Trim()))
                dicType[item.ParentPath.Trim()] = new { Type = BUPType.Parent, Count = 15 };
            return dicType;
        }

        private static void WriteErrorMsg(SFTPHelper sftp, string localPath, string failedPath, FileInfo file, string errorMsg)
        {
            //在本地生成错误日志
            string loaclErrorLog = localPath + "/" + file.Name + ".log";
            Helper.WriteLog(loaclErrorLog, new ArrayList { errorMsg, "DateTime: " + DateTime.Now.ToString() });

            //将excel和错误日志写回sftp
            if (!sftp.DirExist(failedPath))
                sftp.MakeDir(failedPath);
            sftp.Put(loaclErrorLog, failedPath + "/" + file.Name + ".log");
            sftp.Put(file.FullName, failedPath + "/" + file.Name);
        }


        private static void ProcessData(int identity, UserBaseEntity user, ISunnetLog loggerHelper,
            SFTPHelper sftp, string successPath, FileInfo file)
        {
            //将excel写回sftp
            if (!sftp.DirExist(successPath))
                sftp.MakeDir(successPath);
            sftp.Put(file.FullName, successPath + "/" + file.Name);

            string tempPath = ConfigurationManager.AppSettings["TempPath"];
            EmailTemplete temp = XmlHelper.GetEmailTemplete(tempPath, "EmailTemplate.xml");

            string subject = temp.Subject;
            string body = string.Empty;
            body = temp.Body.Replace("{Name}", user.FirstName + " " + user.LastName);

            IEmailSender emailSender = ObjectFactory.GetInstance<IEmailSender>();
            emailSender.SendMail(user.PrimaryEmailAddress, subject, body);

            Console.WriteLine(string.Format("Result: Success. Identity: {0}", identity));
            loggerHelper.Info(string.Format("Result: Success. Identity: {0}", identity));
        }

        private delegate void ProcessHandler(int id, int createdBy);
    }
}
