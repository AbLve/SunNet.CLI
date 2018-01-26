using System;
using System.Collections.Generic;
using System.Linq;
using StructureMap;
using Sunnet.Framework.EmailSender;
using Sunnet.Framework.Encrypt;
using Sunnet.Framework.Log;
using Sunnet.Cli.Business.Tsds;
using Sunnet.Cli.Core.Tsds.Entities;
using Sunnet.Cli.Business.Students;
using Sunnet.Cli.Business.Cpalls;
using Sunnet.Cli.Business.Tsds.Models;
using System.Configuration;
using System.IO;
using Sunnet.Framework.StringZipper;
using Sunnet.Cli.Core.Common.Enums;
using Sunnet.Cli.Core.Students.Entities;
using Sunnet.Framework.Csv;
using Sunnet.Cli.Core.Communities.Entities;
using Sunnet.Cli.Business.Communities;

namespace TsdsXmlFile
{
    internal delegate void QueueEventHandler();
    internal class QueueManager
    {
        internal event QueueEventHandler BeforeProcessQueues;
        internal event QueueEventHandler AfterProcessQueues;
        private IEmailSender emailSender;
        private ISunnetLog logHelper;
        private IEncrypt encrypt;
        private TsdsBusiness _tsdsBusiness;

        internal QueueManager()
        {
            emailSender = ObjectFactory.GetInstance<IEmailSender>();
            logHelper = ObjectFactory.GetInstance<ISunnetLog>();
            encrypt = ObjectFactory.GetInstance<IEncrypt>();
            _tsdsBusiness = new TsdsBusiness();
        }

        internal void Start()
        {
            BeforeProcessQueues?.Invoke();
            TsdsEntity processingTsds = new TsdsEntity();
            try
            {

                var tsdsIdStr = ConfigurationManager.AppSettings["TSDSIDs"]; // seperate by ,
                logHelper.Info("-->Running with TDSD IDS: " + tsdsIdStr);

                List<TsdsEntity> tsdsEntities = null;
                if (tsdsIdStr == null || tsdsIdStr == "")
                {
                    logHelper.Info("-->No Tsds ID in config file, Running with All");
                    tsdsEntities = _tsdsBusiness.GetPendingTsds();
                }
                else
                {

                    List<int> tsdsIDs = new List<int>();
                    for (int i = 0; i < tsdsIdStr.Split(',').Length; i++)
                    {

                        tsdsIDs.Add(Convert.ToInt32(tsdsIdStr.Split(',')[i]));
                    }
                    tsdsEntities = _tsdsBusiness.GetPendingTsds(tsdsIDs);
                }

                if (tsdsEntities.Any())
                {
                    logHelper.Info("-->Prepare process data.");
                    string filePath = ConfigurationManager.AppSettings["ProtectedFiles"];

                    string zipFilePath = filePath + "TSDS";
                    string missingIDPath = filePath + "TSDS/MissingID";
                    CreatePath(zipFilePath);
                    CreatePath(missingIDPath);
                    foreach (TsdsEntity tsds in tsdsEntities)
                    {
                        string xmlFilePath = filePath + "TSDS/XML/" + tsds.ID;
                        CreatePath(xmlFilePath);
                        processingTsds = tsds;
                        logHelper.Info("-->Prepare process tsds ID: " + tsds.ID);
                        tsds.Status = TsdsStatus.Processing;
                        tsds.UpdatedOn = DateTime.Now;
                        _tsdsBusiness.UpdateTsds(tsds);

                        int communityId = tsds.CommunityId;
                        logHelper.Info("-->" + tsds.ID + ".AAAAAAAAAAA");

                        CommunityEntity community = new CommunityBusiness().GetCommunity(communityId);
                        string communityNumber = community.DistrictNumber;
                        logHelper.Info("-->" + tsds.ID + ".BBBBBBBBBBBBBBB");

                        int assessmentId = tsds.AssessmentId;
                        string schoolIds = tsds.SchoolIds.ToString();
                        int[] schoolIdList = schoolIds.Split(',').Select(x => int.Parse(x.ToString())).ToArray();
                        logHelper.Info("-->" + tsds.ID + ".CCCCCCCCCCCCCCCCC");

                        List<int> stuIdListInSchools = new StudentBusiness().GetStudentsForTSDSBySchoolIds(tsds.DOBStartDate, tsds.DOBEndDate, schoolIdList);
                        logHelper.Info("-->" + tsds.ID + ".DDDDDDDDDDDDDDDDDDDDDD");

                        List<int> measureIds = tsds.MeasureIds.Split(',').Select(x => int.Parse(x.ToString())).ToList();
                        logHelper.Info("-->" + tsds.ID + ".EEEEEEEEEEEEEEEEE");

                        List<int> stuIdList = new CpallsBusiness().GetExistAssessmentStuIds(assessmentId, stuIdListInSchools, measureIds);
                        logHelper.Info("-->" + tsds.ID + ".FFFFFFFFFFFFFFFFFFFFFFFFFFF");

                        if (!stuIdList.Any())
                        {
                            logHelper.Info("-->" + tsds.ID + ".GGGGGGGGGGGGGGGGGGG");

                            tsds.Status = TsdsStatus.Succeed;
                            tsds.UpdatedOn = DateTime.Now;
                            _tsdsBusiness.UpdateTsds(tsds);
                            continue;
                        }
                        logHelper.Info("-->" + tsds.ID + ".HHHHHHHHHHHHHHHHHHHH");

                        List<StudentEntity> errStudents = new List<StudentEntity>();
                        StuAssessmentXmlModel stuAssessmentXmlModel = new TsdsBusiness().CreateStuAssessmentModel(assessmentId, stuIdList, measureIds, out errStudents);
                        logHelper.Info("-->" + tsds.ID + ".IIIIIIIIIIIIIIIIIIIIIIIIII");
                        if (!stuAssessmentXmlModel.studentAssessments.Any())
                        {
                            logHelper.Info("-->" + tsds.ID + ".JJJJJJJJJJJJJJJJJJJJJ");

                            tsds.Status = TsdsStatus.Succeed;
                            tsds.UpdatedOn = DateTime.Now;
                            _tsdsBusiness.UpdateTsds(tsds);
                            continue;
                        }

                        logHelper.Info("-->" + tsds.ID + ".KKKKKKKKKKKKKKKKK");
                        string stuAssXmlFileName = new TsdsBusiness().CreatStuAssessmentXml(stuAssessmentXmlModel, communityNumber, assessmentId, xmlFilePath + "/");
                        logHelper.Info("-->" + tsds.ID + ".LLLLLLLLLLLLLLLLLLLLLLL");

                        string metaXmlFileName = new TsdsBusiness().CreateTsdsAssessmentFile(assessmentId, communityNumber, xmlFilePath + "/");
                        logHelper.Info("-->" + tsds.ID + ".MMMMMMMMMMMMMMMMMMMMMM");

                        #region David 06/08/2017 to create the InterchangeStudentParentExtension.xml file
                        string stuParentExtXmlFileName = new TsdsBusiness().CreateStudentParentExtensionXml(stuIdList, communityNumber, xmlFilePath + "/");
                        logHelper.Info("-->" + tsds.ID + ".NNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNN");

                        #endregion

                        //string zipFileName = stuAssXmlFileName.Substring(0, stuAssXmlFileName.Length - 4) + ".zip";
                        /*Since the 3 files are now delivered in a zip folder, we need to change the name of the zip folder; it currently has the name of one of the files:
                            125901_000_2017TSDS_201706191113_InterchangeStudentAssessment
                            It will make more sense to name the zip file with the following elements:
                             Community / District Name_Language_Date
                            I.e.:
                                Alice ISD_English_201706191113
                         */
                        string language = "English";
                        if (assessmentId == 10) //Only support assessmientID(9,10)
                            language = "Spanish";
                        string zipFileName = community.Name + "_" + language + "_" + DateTime.Now.ToString("yyyyMMddhhmm") + ".zip";

                        string errorFileName = community.Name + "_MissingTSDSIDs_" + DateTime.Now.ToString("yyyyMMddhhmm") + ".csv";
                        if (errStudents != null && errStudents.Count > 0)
                        {
                            CreateCsv(errStudents, errorFileName, missingIDPath + "/");
                        }
                        else
                        {
                            errorFileName = "";
                        }
                        logHelper.Info("-->" + tsds.ID + ".OOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOO");

                        CSharpCodeStringZipper.CreateZip(zipFilePath, zipFileName, xmlFilePath);
                        logHelper.Info("-->" + tsds.ID + ".PPPPPPPPPPPPPPPPPPPPPPPPPPPPPPP");

                        DeleteDirectory(xmlFilePath);
                        tsds.Status = TsdsStatus.Succeed;
                        tsds.FileName = zipFileName;
                        tsds.MetaDataFile = metaXmlFileName;
                        tsds.ErrorFileName = errorFileName;
                        tsds.StudentParentFile = stuParentExtXmlFileName;//David 06/08/2017
                        logHelper.Info("-->" + tsds.ID + ".QQQQQQQQQQQQQQQQQQQQQQQQQQQQQ");
                        tsds.UpdatedOn = DateTime.Now;
                        _tsdsBusiness.UpdateTsds(tsds);
                        logHelper.Info("-->Process tsds ID:" + tsds.ID + "success.");
                    }
                }
            }
            catch (Exception ex)
            {
                logHelper.Debug(ex);
                if (processingTsds.ID > 0)
                {
                    processingTsds.Status = TsdsStatus.Error;
                    processingTsds.UpdatedOn = DateTime.Now;
                    _tsdsBusiness.UpdateTsds(processingTsds);
                }
            }
            AfterProcessQueues?.Invoke();
        }

        private static void CreatePath(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        static void DeleteDirectory(string dir)
        {
            if (Directory.GetDirectories(dir).Length == 0 && Directory.GetFiles(dir).Length == 0)
            {
                Directory.Delete(dir);//删除文件夹，若不删除文件夹则不需要 Directory.Delete(dir)
                return;
            }
            foreach (string var in Directory.GetDirectories(dir))
            {
                DeleteDirectory(var);
            }
            foreach (string var in Directory.GetFiles(dir))
            {
                File.Delete(var);
            }
            Directory.Delete(dir);//删除文件夹，若不删除文件夹则不需要 Directory.Delete(dir)
        }

        public static bool CreateCsv(List<StudentEntity> students, string fileName, string fileDir)
        {


            string fileFullPath = fileDir + fileName;
            if (!Directory.Exists(fileDir))
            {
                Directory.CreateDirectory(fileDir);
            }
            CsvFileWriter csvWriter = new CsvFileWriter(fileFullPath);
            CsvRow csvTitle = new CsvRow();
            csvTitle.Add("Community");
            csvTitle.Add("Community Engage ID");

            csvTitle.Add("School");
            csvTitle.Add("School Engage ID");

            csvTitle.Add("First Name");
            csvTitle.Add("Middle Name");
            csvTitle.Add("Last Name");
            csvTitle.Add("DOB");
            csvTitle.Add("Engage ID");
            csvTitle.Add("Local ID");
            csvTitle.Add("TSDS ID");
            csvTitle.Add("Status");

            csvWriter.WriteRow(csvTitle);

            foreach (var student in students)
            {
                List<string> communityNames = new List<string>();
                List<string> schoolNames = new List<string>();
                List<string> communityEngageIds = new List<string>();
                List<string> schoolEngageIds = new List<string>();

                CsvRow csvRow = new CsvRow();
                foreach (var schoolRelation in student.SchoolRelations)
                {
                    var sch = schoolRelation.School;
                    foreach (var communitySchoolRelationsEntity in sch.CommunitySchoolRelations)
                    {
                        var com = communitySchoolRelationsEntity.Community;
                        communityNames.Add(com.Name);
                        communityEngageIds.Add(com.CommunityId);
                    }
                    schoolNames.Add(sch.Name);
                    schoolEngageIds.Add(sch.SchoolId);
                }
                csvRow.Add(String.Join(",", communityNames));
                csvRow.Add(String.Join(",", communityEngageIds));
                csvRow.Add(String.Join(",", schoolNames));
                csvRow.Add(String.Join(",", schoolEngageIds));
                csvRow.Add(student.FirstName);
                csvRow.Add(student.MiddleName);
                csvRow.Add(student.LastName);
                csvRow.Add(student.BirthDate.ToShortDateString());
                csvRow.Add(student.StudentId);
                csvRow.Add(student.LocalStudentID);
                csvRow.Add(student.TSDSStudentID);
                csvRow.Add(student.Status.ToString());
                csvWriter.WriteRow(csvRow);
            }
            csvWriter.Close();
            return true;


        }


    }
}
