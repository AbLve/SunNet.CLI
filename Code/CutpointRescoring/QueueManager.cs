using System;
using System.Collections.Generic;
using System.Linq;
using StructureMap;
using Sunnet.Cli.Business.Students;
using Sunnet.Cli.Business.Cpalls;
using Sunnet.Cli.Core.Cpalls.Entities;
using Sunnet.Framework.EmailSender;
using Sunnet.Framework.Encrypt;
using Sunnet.Framework.Log;
using Sunnet.Cli.Business.Ade;
using Sunnet.Cli.Core.Ade.Entities;
using Sunnet.Framework;
using Sunnet.Cli.Business.Students.Models;
using System.Configuration;
using Sunnet.Cli.Core.Cpalls;

namespace CutpointRescoring
{
    internal delegate void QueueEventHandler();
    internal class QueueManager
    {
        private IEmailSender emailSender;
        private ISunnetLog logHelper;
        private IEncrypt encrypt;
        private StudentBusiness _studentBussiness;
        private CpallsBusiness _cpallsBusiness;
        private AdeBusiness _adeBusiness;

        internal QueueManager()
        {
            emailSender = ObjectFactory.GetInstance<IEmailSender>();
            logHelper = ObjectFactory.GetInstance<ISunnetLog>();
            encrypt = ObjectFactory.GetInstance<IEncrypt>();
            _studentBussiness = new StudentBusiness();
            _cpallsBusiness = new CpallsBusiness();
            _adeBusiness = new AdeBusiness();
        }

        internal void Start()
        {
            #region Measure CutOffScore Changed  David disable it on 01/30/2017 It affects the database performance.
            _studentBussiness = new StudentBusiness();
            _adeBusiness = new AdeBusiness();

            var runningMeasureStr = SFConfig.MeasureId;
            int runningMeasureId = 0;
            int.TryParse(runningMeasureStr, out runningMeasureId);
            var LastRunTimeStr = ConfigurationManager.AppSettings["LastRunTime"];
            DateTime lastRunTime = DateTime.MinValue;
            bool HasLastRunTime = DateTime.TryParse(LastRunTimeStr, out lastRunTime);

            List<MeasureEntity> measureList = null;
            Console.WriteLine("running MeasureId from web.config is " + runningMeasureId);
            Console.WriteLine("running LastRunTime from web.config is " + LastRunTimeStr);
            Console.WriteLine("running Parse LastRunTime Result is " + HasLastRunTime + ", " + lastRunTime.ToString());


            if (runningMeasureId == -1)
            {
                Console.WriteLine("Running with All changed Measures");

                measureList = _adeBusiness.GetCutOffChangedMeasures(); //Running with All
            }
            else
            {
                Console.WriteLine("Running with specfic Measure " + runningMeasureId);
                measureList = _adeBusiness.GetCutOffChangedMeasures(runningMeasureId); //Runing with specfic Measure ID
            }

            string toEmail = ConfigurationManager.AppSettings["ExceptionEmail"];

            if (measureList.Count > 0)
            {
                logHelper.Info("-->Begin process data about measure cutoffscores changed");
                foreach (MeasureEntity measure in measureList)
                {
                    int measureId = measure.ID;
                    try
                    {
                        //update to measure cutoffchanged's status to  status 2(processing).. to do
                        _cpallsBusiness = new CpallsBusiness();
                        logHelper.Info("-->Processing measure with measure ID: " + measureId);
                        List<CutOffScoreEntity> cutOffScoreList = _adeBusiness.GetCutOffScores<MeasureEntity>(measureId);
                        List<CutOffScoreEntity> temCutOffScoreList = new List<CutOffScoreEntity>();

                        logHelper.Info("-->Begin GetStuMeasuresByMeasureId: " + measureId);
                        List<StudentMeasureEntity> stuMeasureList = null;
                        //stuMeasureList = _cpallsBusiness.GetStuMeasuresByMeasureId(measureId, measure.UpdatedOn);
                        if (HasLastRunTime)
                        {
                            stuMeasureList = _cpallsBusiness.GetStuMeasuresAfterLastRunTime(measureId, lastRunTime);
                        }
                        else
                        {
                            stuMeasureList = _cpallsBusiness.GetStuMeasuresByMeasureId(measureId);
                        }
                        //stuMeasureList = stuMeasureList.Where(c => c.Status != CpallsStatus.Locked).ToList(); No need.

                        logHelper.Info("-->End GetStuMeasuresByMeasureId: " + measureId + ",stuMeasureList.count=" + stuMeasureList.Count);
                        emailSender.SendMail(toEmail, "Cutoff Score Recoring --Started...", "Processing measure ID:" + measureId + ",Records:" + stuMeasureList.Count);

                        List<StudentMeasureEntity> updateStudentMeasureList = new List<StudentMeasureEntity>();
                        int i = 0;
                        foreach (StudentMeasureEntity stuMeasure in stuMeasureList)
                        {
                            i++;
                            if (i % 10 == 0)
                            {
                                Console.WriteLine(i);
                            }

                            logHelper.Info(i + "-->A. stuMeasure.id = " + stuMeasure.ID + ",stuMeasure.SAId=" + stuMeasure.SAId);
                            temCutOffScoreList = cutOffScoreList.Where(c => c.Wave == stuMeasure.Assessment.Wave).ToList();
                            logHelper.Info(i + "-->AA. stuMeasure.Assessment.StudentId = " + stuMeasure.Assessment.StudentId);

                            if (stuMeasure.Assessment.StudentId <= 0)
                                continue;

                            StudentModelForQueue stuEntity = _studentBussiness.GetStudentForQueue(stuMeasure.Assessment.StudentId);
                            if (stuEntity == null)
                                continue;
                            DateTime birthday = stuEntity.BirthDate;
                            string schoolyear = stuEntity.SchoolYear;

                            CutOffScoreEntity cutOffScoreEntity = _adeBusiness.GetCutOffScore<MeasureEntity>(temCutOffScoreList, schoolyear, birthday, stuMeasure.Goal);
                            if (cutOffScoreEntity == null)
                            {
                                logHelper.Info(i + "-->B.stuEntity.id " + stuEntity.ID + ",Measure.Id " + measureId + ",Did not find matching cutpoint.");
                                int stuMeasureId = stuMeasure.ID;
                                int count = _cpallsBusiness.UpdateStudentMeasureBenchmark(stuMeasureId, 0, -1, -1, false, false);
                                if (count > 0)
                                    logHelper.Info(i + "-->CC.Init StudentMeasure -->StudentMeasure.ID = " + stuMeasureId + " success");
                                else
                                    logHelper.Debug(i + "-->CC.Init StudentMeasure -->StudentMeasure.ID = " + stuMeasureId + " failed");
                                continue;
                            }
                            logHelper.Info(i + "-->B.stuEntity.id " + stuEntity.ID + ",stuMeasure.BenchmarkId = " + stuMeasure.BenchmarkId + ", New_benchmark ID=" + cutOffScoreEntity.BenchmarkId);
                            if (stuMeasure.BenchmarkId != cutOffScoreEntity.BenchmarkId
                               || stuMeasure.HigherScore != cutOffScoreEntity.HigherScore
                               || stuMeasure.LowerScore != cutOffScoreEntity.LowerScore
                               || stuMeasure.ShowOnGroup != cutOffScoreEntity.ShowOnGroup)
                            {
                                int stuMeasureId = stuMeasure.ID;
                                logHelper.Info(i + "-->C. Begin update StudentMeasure -->StudentMeasure.ID = " + stuMeasureId);
                                int count = _cpallsBusiness.UpdateStudentMeasureBenchmark(stuMeasureId,
                                    cutOffScoreEntity.BenchmarkId,
                                    cutOffScoreEntity.LowerScore,
                                    cutOffScoreEntity.HigherScore,
                                    cutOffScoreEntity.ShowOnGroup, false);//Change to only update the related fields.   
                                if (count > 0)
                                    logHelper.Info(i + "-->CC.Update StudentMeasure -->StudentMeasure.ID = " + stuMeasureId + " success");
                                else
                                    logHelper.Debug(i + "-->CC.Update StudentMeasure -->StudentMeasure.ID = " + stuMeasureId + " failed");
                            }
                        }

                        logHelper.Info("-->D.Change measure's cutoffscorechanged field to false,measure ID:" + measureId);

                        measure.CutOffScoresChanged = false;

                        // UpdateMeasure(MesureID,CutOffScoresChanged)  update Measures set CutOffScoresChanged=@xx where ID=@)
                        int updateResult = _adeBusiness.UpdateCutOffScoresChanged(measureId, false);
                        if (updateResult > 0)
                            logHelper.Info("-->DD.Update measure success,measure ID:" + measureId);
                        else
                            logHelper.Debug("-->DD. Update measure failed,measure ID:" + measureId);


                        logHelper.Info("-->E. Begin--Update All BenchmarkChanged to 0, measure ID:" + measureId);
                        //SQL = update StudentMeasures set BenchmarkChanged=0 where MeasureId=@ID and BenchmarkChanged=1
                        int result = _cpallsBusiness.UpdateBenchmarkChangedToFalse(measureId);
                        if (result > 0)
                            logHelper.Info("-->EE. End-->Update All BenchmarkChanged to 0 success, measure ID:" + measureId + " count:" + result);
                        else
                            logHelper.Debug("-->EE.End-->Update All BenchmarkChanged to 0 failed, measure ID:" + measureId);
                        //Send out email
                        emailSender.SendMail(toEmail, "Cutoff Score Recoring -Completed", "Processed measure ID:" + measureId + " successfully...");
                    }
                    catch (Exception ex)
                    {
                        logHelper.Debug("An error in change cpalls score by measure cutoffscores, measure ID:" + measureId
                            + ",error message:" + ex.Message);
                    }
                }
            }
            else
            {
                logHelper.Info("-->There no measure change cutoff score.");
            }
            #endregion for Measure Cut Poing Change



        }
    }
}
