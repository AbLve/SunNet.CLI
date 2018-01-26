using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using SyncTrainingRecord.DatabaseService;
using SyncTrainingRecord.Entity;
using Sunnet.Framework.EmailSender;
using Sunnet.Framework.Log;
using StructureMap;
using System.Configuration;

namespace SyncTrainingRecord
{
    internal delegate void QueueEventHandler();

    internal class QueueManager
    {
        internal event QueueEventHandler BeforeProcessQueues;
        internal event QueueEventHandler AfterProcessQueues;
        private readonly CLIService cliService = null;
        private readonly TECPDSService tecpdsService = null;
        private ISunnetLog logHelper;
        private IEmailSender emailSender;

        internal QueueManager()
        {
            cliService = new CLIService();
            tecpdsService = new TECPDSService();
            emailSender = ObjectFactory.GetInstance<IEmailSender>();
            //logHelper = ObjectFactory.GetInstance<ISunnetLog>();
        }

        internal void Start()
        {

            /* JSON Sample
             {"cert_code":"wZz21Y9A9rBjlSi","cert_type":"CPE","cert_timeissued":"31471200","learner_cliengageid":"ete1723638","learner_name":"LEARNER1 TEST","instructor_cliengageid":"","instructor_name":"","course_id":"1","course_name":"TECPDS TEST1","course_idnumber":"tecpds_test_1","course_clockhours":"0","course_deliverymethod":"Self-instructional","cert_file":"\\\\uthsch-nas.uthouston.edu\\ms_cli\\LMS\\DEV2\/certificates\/w\/Z\/z\/wZz21Y9A9rBjlSi_CPE.pdf","course_strategies":"this (1), that (2), and again (3)","course_strategy_ids":[1,2,3]}
              {"cert_code":"wZz21Y9A9rBjlSi","cert_type":"CLOCKHOUR","cert_timeissued":"31471200","learner_cliengageid":"ete1723638","learner_name":"LEARNER1 TEST","instructor_cliengageid":"","instructor_name":"","course_id":"1","course_name":"TECPDS TEST1","course_idnumber":"tecpds_test_1","course_clockhours":"0","course_deliverymethod":"Self-instructional","cert_file":"\\\\uthsch-nas.uthouston.edu\\ms_cli\\LMS\\DEV2\/certificates\/w\/Z\/z\/wZz21Y9A9rBjlSi_CLOCKHOUR.pdf","course_competencies":"this (1), that (2), and again (3)","course_competency_ids":[1,2,3]}
*/
            BeforeProcessQueues?.Invoke();
            Config.Instance.Logger.Info("Start");

            //if (true == true)
            //{
            //    List<TrainingAttendRecordEntity> jlist = new List<TrainingAttendRecordEntity>();

            //    TrainingAttendRecordEntity jsonRecord = new TrainingAttendRecordEntity();
            //    jsonRecord.course_competencies = "test1";
            //    List<int> templist = new List<int>();
            //    templist.Add(1);
            //    templist.Add(2);
            //    templist.Add(3);
                
            //    jsonRecord.course_strategy_ids = templist;

            //    TrainingAttendRecordEntity jsonRecord2 = new TrainingAttendRecordEntity();
            //    jsonRecord2.course_competencies="test2";
            //    jsonRecord2.course_competency_ids = templist;
            //    jlist.Add(jsonRecord);
            //    jlist.Add(jsonRecord2);

            //    string oneLine = JsonConvert.SerializeObject(jlist); //Convert back to json for log
            //    Console.WriteLine(oneLine);
            //    return;
            //}
            string logfilePath = "";
            string logfileName = "";
            var validatorID = 0;
            string toEmail = ConfigurationManager.AppSettings["ExceptionEmail"];
            #region Get the default validator id for all records
            try
            {
                validatorID = getDefaultValidatorID();
            }
            catch (Exception ex )
            {
                Config.Instance.Logger.Debug(ex);
                emailSender.SendMail(toEmail, "LMS/TECPDS Exception", ex.ToString());
                return;
            }
            #endregion


            try
            {
                Config.Instance.Logger.Info("Read all log files from data directory begin.");
                DirectoryInfo dir = new DirectoryInfo(Config.Instance.DataPath + "tobeprocessed\\");
                FileInfo[] logFiles = dir.GetFiles();
                foreach (FileInfo logFile in logFiles)
                {
                    Config.Instance.Logger.Info("Begin File Name:" + logFile.Name);
                    if (logFile.Extension.Equals(".log"))
                    {
                        logfilePath = logFile.FullName;
                        logfileName = logFile.Name;
                        string json = "";

                        FileStream fs = new FileStream(logFile.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                        StreamReader sr = new StreamReader(fs, System.Text.Encoding.Default);

                        StringBuilder sb = new StringBuilder();
                        while (!sr.EndOfStream)
                        {
                            //David 10/19/2017 The file is designed to split lines based on line feeds. Works quite well from PHP. Tested and verified.
                            sb.AppendLine(sr.ReadLine()+","); 
                        }
                        sr.ReadToEnd();
                        sr.Dispose();
                      

                        json = "[" + sb.ToString() + "]";

                        Config.Instance.Logger.Info("Begin Sync Data.File Name:" + logFile.Name);
                        string errorMessage = SyncOperate(json, validatorID);
                        Config.Instance.Logger.Info("End Sync Data.File Name:" + logFile.Name + "-->errorMessage:" + errorMessage);
                        //Error Resources
                        WritetoFile(Config.Instance.DataPath + "error\\" + logfileName+".error", errorMessage);
                        //移动文件位置到processed目录中
                        File.Move(logfilePath, Config.Instance.DataPath + "processed\\" + logFile.Name);
                        emailSender.SendMail(toEmail, "LMS/TECPDS Processed Successsfully", "The File ["+ logFile.Name +"] has been moved to processed floder.<br><br> Error Messages:<br>"+ errorMessage);
                        // File.Copy(logfilePath, Config.Instance.DataPath + "processed\\" + logFile.Name, true);
                        // File.Delete(logfilePath);
                    }
                    Config.Instance.Logger.Info("End File Name:" + logFile.Name);
                }
                Config.Instance.Logger.Info("Read all log files from data directory end.");
            }
            catch (Exception ex)
            {
                //代码出现异常，移动文件位置到error目录中
                File.Move(logfilePath, Config.Instance.DataPath + "error\\" + logfileName);
                //File.Copy(logfilePath, Config.Instance.DataPath + "error\\" + logfileName);
                //File.Delete(logfilePath);
                emailSender.SendMail(toEmail, "LMS/TECPDS Exception", "The File [" + logfileName + "] has been moved to error floder. Exception Detail:" + ex.ToString());
                Config.Instance.Logger.Debug(ex);
                Config.Instance.Logger.Info("End by Exception");
            }
            finally
            {
                AfterProcessQueues?.Invoke();
            }
        }

        private string SyncOperate(string json,int validatorID)
        {
            Config.Instance.Logger.Info("SyncOperate()-->Begin Json...."+json);
            var list = JsonConvert.DeserializeObject<List<TrainingAttendRecordEntity>>(json);
            list = list.Where(e => e != null && e.cert_type.ToUpper() == "CLOCKHOUR").ToList(); //Only for CLOCKHOUR
            Config.Instance.Logger.Info("SyncOperate()-->Total CLOCKHOUR records in this json:" + list.Count);
            StringBuilder errorMessage = new StringBuilder();
            if (!list.Any())
            {
                errorMessage.Append("No CLOCKHOUR records found in this file.");
                return errorMessage.ToString();
            }
            
            #region 下面是将部分数据先查询到内存中，避免循环时多次访问数据库

            var cliEngageIds = list.Select(e => e.learner_cliengageid).ToList();
            Config.Instance.Logger.Info("SyncOperate()->cliEngageIds :" + string.Join(",", cliEngageIds));
            List<V_BI_TeacherEntity> teachers = new CLIService().GetTeachers(cliEngageIds);
            List<V_BI_PrincipalEntity> principals = new CLIService().GetPrincipals(cliEngageIds);
            Config.Instance.Logger.Info("SyncOperate()->teachers:" + teachers.Count + " ,principals:" + principals.Count);

            List<CoreCompetencyAreaEntity> allCoreCompetency = tecpdsService.GetAllCoreCompetencyAreas();

            var cliStateNames = teachers.Select(e => e.StateName).ToList();
            cliStateNames.AddRange(principals.Select(e => e.StateName).ToList());
            Config.Instance.Logger.Info("SyncOperate()->cliStateNames :" + string.Join(",", cliStateNames));

            var tecpdsStates = tecpdsService.GetStates(cliStateNames);

            var cliCountyNames = teachers.Select(e => e.CountyName).ToList();
            cliCountyNames.AddRange(principals.Select(e => e.CountyName).ToList());
            Config.Instance.Logger.Info("SyncOperate()->cliCountyNames :" + string.Join(",", cliCountyNames));

            var tecpdsCounties = tecpdsService.GetCounties(cliCountyNames);

            var googleIds = teachers.Select(e => e.GoogleId).ToList();
            googleIds.AddRange(principals.Select(e => e.GoogleId).ToList());
            Config.Instance.Logger.Info("SyncOperate()->googleIds :" + string.Join(",", googleIds));

            #endregion

            Config.Instance.Logger.Info("SyncOperate()-->Start to process "+list.Count+ " jsonRecord(s)");
            foreach (var jsonRecord in list)
            {
                string oneLine = JsonConvert.SerializeObject(jsonRecord); //Convert back to json for log
                try
                {
                    string rstMsg = String.Empty;
                    bool result = Process(jsonRecord, teachers, principals, tecpdsStates, tecpdsCounties, validatorID, allCoreCompetency, out rstMsg);
                    if (!result)
                    {
                        errorMessage.Append(oneLine + "|Error:" + rstMsg + Environment.NewLine);
                    }
                    Config.Instance.Logger.Info(oneLine + "Result Msg:" + rstMsg);
                }
                catch (Exception ex)
                {
                    Config.Instance.Logger.Debug(JsonConvert.SerializeObject(jsonRecord) + " Exception:" + ex.ToString());
                    errorMessage.Append(oneLine + "|Exception:" + ex.Message+Environment.NewLine);
                }

            }//foreach (var trainingAttendRecord in list) 

            Config.Instance.Logger.Info("SyncOperate()-->End Insert TrainingAttendRecord");
            return errorMessage.ToString();

        }
        private bool Process(TrainingAttendRecordEntity jsonRecord, List<V_BI_TeacherEntity> teachers,
            List<V_BI_PrincipalEntity> principals, List<StateEntity> tecpdsStates,
             List<CountyEntity> tecpdsCounties,int validatorID,List<CoreCompetencyAreaEntity> allCoreCompetency,out string rstMsg)
        {
            Config.Instance.Logger.Info("Process()-->TrainingAttendRecord:" + JsonConvert.SerializeObject(jsonRecord));
            int trainingAttendUserId = 0;
            int stateId = 0;
            int countyId = 0;
            var teacher = new V_BI_TeacherEntity();
            var principal = new V_BI_PrincipalEntity();
            int role = 4;//CenterDirector = 2,Practitioner = 4,
            #region Teacher->Practitioner begin
            if (teachers.Any(e => e.EngageId.ToUpper() == jsonRecord.learner_cliengageid.ToUpper()))
            {
                teacher = teachers.FirstOrDefault(e => e.EngageId.ToUpper() == jsonRecord.learner_cliengageid.ToUpper());
                role = 4;
                var state = tecpdsStates.FirstOrDefault(e => e.Name.ToUpper() == teacher.StateName.ToUpper());
                stateId = state == null ? 0 : state.ID;
                var county = tecpdsCounties.FirstOrDefault(e => e.Name.ToUpper() == teacher.CountyName.ToUpper());
                countyId = county == null ? 0 : county.ID;
                var account = tecpdsService.GetAccount(teacher.GoogleId);
                if (account.ID > 0)
                {
                   // Config.Instance.Logger.Info("Process()-->Account Exist:" + account.ID);
                    if (account.PractitionerID > 0)
                    {
                      //  Config.Instance.Logger.Info("Process()-->account.PractitionerID>0,account.PractitionerID:" + account.PractitionerID);
                        trainingAttendUserId = account.PractitionerID;
                    }
                    else
                    {
                       // Config.Instance.Logger.Info("Process()-->account.PractitionerID=0,account.CenterDirector:" + account.CenteDirectorID);
                        TecpdsUserEntity tecpdsUser = new TecpdsUserEntity();
                        InitPractitionerUser(tecpdsUser, teacher, stateId, countyId, role);
                        trainingAttendUserId = tecpdsService.InsertUser(tecpdsUser);
                        account.PractitionerID = trainingAttendUserId;
                        tecpdsService.UpdateAccount(account);
                       // Config.Instance.Logger.Info("Process()-->update account.PractitionerID:" + account.PractitionerID);
                    }
                }
                else // if (account.ID > 0)
                {
                   // Config.Instance.Logger.Info("Process()-->Account Not Exist");
                    TecpdsUserEntity tecpdsUser = new TecpdsUserEntity();
                    InitPractitionerUser(tecpdsUser, teacher, stateId, countyId, role);
                   // Config.Instance.Logger.Info("Process()-->insert user begin");
                    trainingAttendUserId = tecpdsService.InsertUser(tecpdsUser);
                    //Config.Instance.Logger.Info("Process()-->insert Practitioner successful.User ID=" + trainingAttendUserId);
                    AccountEntity newAccount = new AccountEntity();
                    InitAccount(newAccount, trainingAttendUserId, role, teacher.GoogleId);
                    int accountId = tecpdsService.InsertAccount(newAccount);
                   // Config.Instance.Logger.Info("Process()-->insert account end.Account ID=" + accountId);
                }
            }
            #endregion Teacher->Practitioner end
            #region principal -> CenterDirector begin
            else if (principals.Any(e => e.EngageId.ToUpper() == jsonRecord.learner_cliengageid.ToUpper()))
            {
                principal = principals.FirstOrDefault(e => e.EngageId.ToUpper() == jsonRecord.learner_cliengageid.ToUpper());
                //Config.Instance.Logger.Info("Process()-->Principal ID:" + principal.ID);
                role = 2;
                var state = tecpdsStates.FirstOrDefault(e => e.Name.ToUpper() == principal.StateName.ToUpper());
                stateId = state == null ? 0 : state.ID;
                //Config.Instance.Logger.Info("Process()-->stateId:" + stateId);
                var county = tecpdsCounties.FirstOrDefault(e => e.Name.ToUpper() == principal.CountyName.ToUpper());
                countyId = county == null ? 0 : county.ID;
                //Config.Instance.Logger.Info("Process()-->countyId:" + countyId);

                var account = tecpdsService.GetAccount(principal.GoogleId);
                if (account.ID > 0)
                {
                    if (account.CenteDirectorID > 0)
                    {
                        trainingAttendUserId = account.CenteDirectorID;
                       // Config.Instance.Logger.Info("Process()-->account.CenteDirectorID>0,account.CenteDirectorID:" + account.CenteDirectorID);
                    }
                    else
                    {
                       // Config.Instance.Logger.Info("Process()-->account.PractitionerID=0,account.CenterDirector:" + account.CenteDirectorID);
                        TecpdsUserEntity tecpdsUser = new TecpdsUserEntity();
                        InitCenterDirectorUser(tecpdsUser, principal, stateId, countyId, role);
                       // Config.Instance.Logger.Info("Process()-->insert user begin");
                        trainingAttendUserId = tecpdsService.InsertUser(tecpdsUser);
                       //Config.Instance.Logger.Info("Process()-->insert CenterDirector.User ID=" + trainingAttendUserId);
                        account.CenteDirectorID = trainingAttendUserId;
                        tecpdsService.UpdateAccount(account);
                        //Config.Instance.Logger.Info("Process()-->update account.PractitionerID:" + account.PractitionerID);
                    }
                }
                else
                {
                    TecpdsUserEntity tecpdsUser = new TecpdsUserEntity();
                    InitCenterDirectorUser(tecpdsUser, principal, stateId, countyId, role);
                    trainingAttendUserId = tecpdsService.InsertUser(tecpdsUser);
                    //Config.Instance.Logger.Info("Process()-->insert CenterDirector successful.User ID=" + trainingAttendUserId);
                    AccountEntity newAccount = new AccountEntity();
                    InitAccount(newAccount, trainingAttendUserId, role, principal.GoogleId);
                    int accountId = tecpdsService.InsertAccount(newAccount);
                   // Config.Instance.Logger.Info("Process()-->insert Account successful.Account ID=" + accountId);
                }
            }
            #endregion principal->CenterDirector
            else
            {
                rstMsg = "No teacher nor principal found";
                return false;
            }
            Config.Instance.Logger.Info("Process()-->init trainingattended entity");
            TrainingAttendedEntity trainingAttendEntity = new TrainingAttendedEntity();
            trainingAttendEntity.UserID = trainingAttendUserId; //通过engageid查询出相对应的tecpds用户ID
            trainingAttendEntity.CompletionDate = GetTime(jsonRecord.cert_timeissued);
            trainingAttendEntity.TrainingTitle = jsonRecord.course_name;
            trainingAttendEntity.CoreCompetencyArea = 0; //关系存储到关系表，此字段无实际意义
            trainingAttendEntity.TrainerID = 0; //有可能需要根据instructor_cliengageid查询出具体的TECPDS User ID
            trainingAttendEntity.RegisteredTrainerName = jsonRecord.instructor_name;
            trainingAttendEntity.TrainerName = jsonRecord.instructor_name;
            trainingAttendEntity.ClockHours = jsonRecord.course_clockhours;
            trainingAttendEntity.TrainingMethod = 2;//1: Face to Face, 2: Online, 3: Distance 4: Blened
            if (jsonRecord.cert_file.Contains(Config.Instance.UploadFile))
            {
                jsonRecord.cert_file =
                    jsonRecord.cert_file.Substring(Config.Instance.UploadFile.Length);
            }
            //string certFileExt = trainingAttendRecord.cert_file.Substring(trainingAttendRecord.cert_file.LastIndexOf("."));
            //trainingAttendRecord.cert_file =
            //    jsonRecord.cert_file.Replace("&", "").Replace("#", "").Replace(certFileExt, "")
            //    + DateTime.Now.ToString("yyMMddHHmmssffff") + trainingAttendUserId.ToString().PadLeft(5, '0') +
            //    Guid.NewGuid().ToString().Substring(0, 5) + certFileExt;
            trainingAttendEntity.UploadCertificate = jsonRecord.cert_file; //只需要记录文件名即可，文件需要存储到配置目录
            trainingAttendEntity.CreatedDate = DateTime.Now;
            trainingAttendEntity.Role = role; //根据上面查询出的tecpds userid确认
            trainingAttendEntity.TrainingFor = 4; //TECPDS 代码中写的是Role.Practitioner(4)或Role.Administrator(1)
            trainingAttendEntity.Attendees = 0;
            trainingAttendEntity.ISPresented = false;
            trainingAttendEntity.IsValid = 1;//EducationValidStatus.Verified
            trainingAttendEntity.ValidatorId = validatorID; //查询出Validator用户FirstName为Engage的Id
            trainingAttendEntity.ValidTime = DateTime.Now;
            Config.Instance.Logger.Info("Process()-->insert TrainingAttend Begin.");
            int trainingAttendId = tecpdsService.InsertTrainingAttend(trainingAttendEntity);
            Config.Instance.Logger.Info("Process()-->insert TrainingAttend End.TrainingAttend ID=" + trainingAttendId);

            #region Core Competency Areas

            if(jsonRecord.course_competency_ids.Any())
            {
                foreach (var compency_id in jsonRecord.course_competency_ids)
                {
         
                    var coreCompetency = allCoreCompetency.Where(e => e.ID == compency_id).FirstOrDefault();
                    if (coreCompetency != null)
                    {
                        Config.Instance.Logger.Info("Process()--> Insert CoreCompetency ID=" + coreCompetency.ID + ",name=" + coreCompetency.Name);
                        TrainingAttendedCoreEntity trainingAttendedCoreEntity = new TrainingAttendedCoreEntity();
                        trainingAttendedCoreEntity.ParentID = trainingAttendId;
                        trainingAttendedCoreEntity.CoreCompetencyAreaID = compency_id;
                        trainingAttendedCoreEntity.Type = 1;//表示TrainingAttendCoreType.TrainingAttend
                        int trainingAttendedCoreId = tecpdsService.InsertTrainingAttendCore(trainingAttendedCoreEntity);
                    }
                }
            }


            #endregion core Competency Areas
            rstMsg = "Successed";
            return true;
        }


        private void InitPractitionerUser(TecpdsUserEntity tecpdsUser, V_BI_TeacherEntity teacher, int stateId, int countyId, int role)
        {
            //插入用户，插入Account
            //TecpdsUserEntity tecpdsUser = new TecpdsUserEntity();
            tecpdsUser.CreatedDate = DateTime.Now;
            tecpdsUser.Status = teacher.Status;
            tecpdsUser.IsDeleted = false;
            tecpdsUser.Title = "";
            tecpdsUser.FirstName = teacher.FirstName;
            tecpdsUser.MiddleInitial = teacher.MiddleName;
            tecpdsUser.LastName = teacher.LastName;
            tecpdsUser.PreviousLastName = teacher.PreviousLastName;
            tecpdsUser.BirthDate = teacher.BirthDate;
            tecpdsUser.Gender = teacher.Gender;
            tecpdsUser.HomeMailingAddress = teacher.HomeMailingAddress;
            tecpdsUser.City = teacher.City;
            tecpdsUser.State = stateId;
            tecpdsUser.ZipCode = teacher.Zip;
            tecpdsUser.County = countyId;
            tecpdsUser.PrimaryPhoneNumber = teacher.PrimaryPhoneNumber;
            tecpdsUser.PrimaryNumberType = teacher.PrimaryNumberType;
            tecpdsUser.SecondaryPhoneNumber = teacher.SecondaryPhoneNumber;
            tecpdsUser.SecondaryNumberType = teacher.SecondaryNumberType;
            tecpdsUser.FaxNumber = teacher.FaxNumber;
            tecpdsUser.WebAddress = "";
            tecpdsUser.PrimaryEmailAddress = teacher.PrimaryEmailAddress;
            tecpdsUser.SecondaryEmailAddress = teacher.SecondaryEmailAddress;
            tecpdsUser.RacialEthnicBackground = 0;
            tecpdsUser.PrimaryLanguage = teacher.PrimaryLanguageId;
            tecpdsUser.SecondaryLanguage = teacher.SecondaryLanguageId;
            tecpdsUser.ApplicationDate = DateTime.Now;
            tecpdsUser.LastPaymentDate = DateTime.Now;
            tecpdsUser.Level = 0;
            tecpdsUser.RenewalDate = DateTime.Now.AddYears(3);//For TSR teachers 3 years from created date.  For Higher Ed, one year from created date.
            tecpdsUser.ActiveDate = DateTime.Now;
            tecpdsUser.Role = role;
            tecpdsUser.Comments = "Opt-in from Engage";
            tecpdsUser.Education = 0;
            tecpdsUser.WorkExperience = 0;
            tecpdsUser.RegisteSteps = 0;
            tecpdsUser.Scoresheet = "";
            tecpdsUser.TrainingLog = "";
        }

        private void InitCenterDirectorUser(TecpdsUserEntity tecpdsUser, V_BI_PrincipalEntity principal, int stateId, int countyId, int role)
        {
            tecpdsUser.CreatedDate = DateTime.Now;
            tecpdsUser.Status = principal.Status;
            tecpdsUser.IsDeleted = false;
            tecpdsUser.Title = "";
            tecpdsUser.FirstName = principal.FirstName;
            tecpdsUser.MiddleInitial = principal.MiddleName;
            tecpdsUser.LastName = principal.LastName;
            tecpdsUser.PreviousLastName = principal.PreviousLastName;
            tecpdsUser.BirthDate = principal.BirthDate;
            tecpdsUser.Gender = principal.Gender;
            tecpdsUser.HomeMailingAddress = principal.Address;
            tecpdsUser.City = principal.City;
            tecpdsUser.State = stateId;
            tecpdsUser.ZipCode = principal.Zip;
            tecpdsUser.County = countyId;
            tecpdsUser.PrimaryPhoneNumber = principal.PrimaryPhoneNumber;
            tecpdsUser.PrimaryNumberType = principal.PrimaryNumberType;
            tecpdsUser.SecondaryPhoneNumber = principal.SecondaryPhoneNumber;
            tecpdsUser.SecondaryNumberType = principal.SecondaryNumberType;
            tecpdsUser.FaxNumber = principal.FaxNumber;
            tecpdsUser.WebAddress = "";
            tecpdsUser.PrimaryEmailAddress = principal.PrimaryEmailAddress;
            tecpdsUser.SecondaryEmailAddress = principal.SecondaryEmailAddress;
            tecpdsUser.RacialEthnicBackground = 0;
            tecpdsUser.PrimaryLanguage = 0;
            tecpdsUser.SecondaryLanguage = 0;
            tecpdsUser.ApplicationDate = DateTime.Now;
            tecpdsUser.LastPaymentDate = DateTime.Now;
            tecpdsUser.Level = 0;
            tecpdsUser.RenewalDate = DateTime.Now.AddYears(3);//For TSR teachers 3 years from created date.  For Higher Ed, one year from created date.
            tecpdsUser.ActiveDate = DateTime.Now;
            tecpdsUser.Role = role;
            tecpdsUser.Comments = "Opt-in form Engage";
            tecpdsUser.Education = 0;
            tecpdsUser.WorkExperience = 0;
            tecpdsUser.RegisteSteps = 0;
            tecpdsUser.Scoresheet = "";
            tecpdsUser.TrainingLog = "";
        }
        private void InitAccount(AccountEntity account, int userId, int role, string googleId)
        {
            account.TrainerID = 0;
            if (role == 2)
            {
                account.PractitionerID = 0;
                account.CenteDirectorID = userId;
            }
            else if (role == 4)
            {
                account.PractitionerID = userId;
                account.CenteDirectorID = 0;
            }
            account.GoogleID = googleId;
            account.CreatedDate = DateTime.Now;
            account.LastLoginDate = DateTime.Now;
            account.Status = 1;
            account.LoginAccount = "";
            account.LoginPassword = "";
            account.DefaultRole = role;
        }
        private int getDefaultValidatorID()
        {
            var validatorId = tecpdsService.GetValidateId();
            if (validatorId == 0)
            {
                ValidatorEntity validator = new ValidatorEntity();
                validator.FirstName = "Engage";
                validator.LastName = "";
                validator.GoogleGmail = "";
                validator.Status = 1;
                validator.Comments = "Only for Engage system";
                validator.Scoresheet = "";
                validator.CreatedDate = DateTime.Now;
                validator.UpdatedDate = DateTime.Now;
                validator.LastLoginDate = DateTime.Now;
                validatorId = tecpdsService.InsertValidator(validator);
            }
            return validatorId;
        }
        public static DateTime GetTime(string timeStamp)
        {
            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            long lTime = long.Parse(timeStamp + "0000000");
            TimeSpan toNow = new TimeSpan(lTime);
            return dtStart.Add(toNow);
        }

        private  void WritetoFile(string fileName, string message)
        {
            using (StreamWriter sw = File.AppendText(fileName))
            {
                sw.WriteLine(message);
                sw.Flush();
                sw.Close();
            }
        }
    }
}
