using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StructureMap;
using Sunnet.Cli.Business.Students;
using Sunnet.Cli.Business.Cpalls;
using Sunnet.Cli.Business.Common;
using Sunnet.Cli.Core.Students.Entities;
using Sunnet.Cli.Core.Cpalls.Entities;
using Sunnet.Framework.EmailSender;
using Sunnet.Framework.Encrypt;
using Sunnet.Framework.Log;
using Sunnet.Cli.Business.Ade;
using Sunnet.Cli.Core.Ade.Entities;
using Sunnet.Cli.Core.Cpalls;
using Sunnet.Cli.Core.Students.Enums;
using Sunnet.Framework.Core.Tool;

namespace CpallsScore
{
    internal delegate void QueueEventHandler();
    internal class QueueManager
    {
        internal event QueueEventHandler BeforeProcessQueues;
        internal event QueueEventHandler AfterProcessQueues;
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
            BeforeProcessQueues?.Invoke();
            _studentBussiness = new StudentBusiness();

            //Student Age Changed
            List<StudentDOBEntity> dobEntityList = null;
            try
            {
                dobEntityList = _studentBussiness.GetStudentDobsbyStatus(StudentDOBStatus.Save);
            }
            catch (Exception ex)
            {
                logHelper.Debug("An error in GetStudentDobsbyStatus(..),error message:" + ex.Message);
            }

            if (dobEntityList != null && dobEntityList.Count() > 0)
            {
                logHelper.Info("-->Begin process data about student DOB changed");
                foreach (StudentDOBEntity dobEntity in dobEntityList)
                {
                    int stuId = dobEntity.StudentId;
                    try
                    {
                        dobEntity.Status = StudentDOBStatus.Processing;
                        _studentBussiness.UpdateStudentDOB(dobEntity);
                        logHelper.Info("-->Processing studentId " + stuId);
                        _cpallsBusiness = new CpallsBusiness();
                        List<StudentMeasureEntity> stuMeasures = _cpallsBusiness.GetStuMeasuresByStuId(stuId);
                        stuMeasures = stuMeasures.Where(c => c.Status != CpallsStatus.Locked).ToList();
                        List<CutOffScoreEntity> cutOffScoreEntities = _adeBusiness.GetCutOffScores<MeasureEntity>(stuMeasures.Select(m => m.MeasureId));

                        foreach (StudentMeasureEntity measure in stuMeasures)
                        {
                            DateTime birthday = dobEntityList.Where(d => d.StudentId == measure.Assessment.StudentId).Select(d => d.NewDOB).FirstOrDefault();
                            string schoolyear = dobEntity.SchoolYear;

                            string newPercentileRank = _adeBusiness.PercentileRankLookup(measure.MeasureId, birthday,
                                measure.Goal, measure.UpdatedOn);
                            logHelper.Info("-->stuEntity.id " + stuId + ",stuMeasure.PercentileRank = " + measure.PercentileRank + ", New_PercentileRank=" + newPercentileRank);
                            if (measure.PercentileRank != newPercentileRank)
                            {
                                int stuMeasureId = measure.ID;
                                logHelper.Info("-->Begin update StudentMeasure -->StudentMeasure.ID = " + stuMeasureId);
                                int count = _cpallsBusiness.UpdateStudentMeasurePercentileRank(stuMeasureId, newPercentileRank);
                                if (count > 0)
                                    logHelper.Info("-->Update StudentMeasure PercentileRank -->StudentMeasure.ID = " + stuMeasureId + " success");
                                else
                                    logHelper.Debug("-->Update StudentMeasure PercentileRank -->StudentMeasure.ID = " + stuMeasureId + " failed");
                            }

                            List<CutOffScoreEntity> cutOffScores = cutOffScoreEntities.Where(c => c.HostId == measure.MeasureId && c.Wave == measure.Assessment.Wave).ToList();
                            if (!cutOffScores.Any())
                                continue;

                            CutOffScoreEntity cutOffScoreEntity = _adeBusiness.GetCutOffScore<MeasureEntity>(cutOffScores, schoolyear, birthday, measure.Goal);
                            if (cutOffScoreEntity == null)
                            {
                                logHelper.Info("-->stuEntity.id " + stuId + ",Measure.Id " + measure.MeasureId + ",Did not find matching cutpoint.");
                                int stuMeasureId = measure.ID;
                                int count = _cpallsBusiness.UpdateStudentMeasureBenchmark(stuMeasureId, 0, -1, -1, false, false);
                                if (count > 0)
                                    logHelper.Info("-->Init StudentMeasure -->StudentMeasure.ID = " + stuMeasureId + " success");
                                else
                                    logHelper.Debug("-->Init StudentMeasure -->StudentMeasure.ID = " + stuMeasureId + " failed");
                                continue;
                            }
                            logHelper.Info("-->stuEntity.id " + stuId + ",stuMeasure.BenchmarkId = " + measure.BenchmarkId + ", New_benchmark ID=" + cutOffScoreEntity.BenchmarkId);
                            if (measure.BenchmarkId != cutOffScoreEntity.BenchmarkId
                                || measure.HigherScore != cutOffScoreEntity.HigherScore
                                || measure.LowerScore != cutOffScoreEntity.LowerScore
                                || measure.ShowOnGroup != cutOffScoreEntity.ShowOnGroup)
                            {
                                int stuMeasureId = measure.ID;
                                logHelper.Info("-->Begin update StudentMeasure -->StudentMeasure.ID = " + stuMeasureId);
                                int count = _cpallsBusiness.UpdateStudentMeasureBenchmark(stuMeasureId,
                                    cutOffScoreEntity.BenchmarkId,
                                    cutOffScoreEntity.LowerScore,
                                    cutOffScoreEntity.HigherScore,
                                    cutOffScoreEntity.ShowOnGroup, false);//Change to only update the related fields.   
                                if (count > 0)
                                    logHelper.Info("-->Update StudentMeasure -->StudentMeasure.ID = " + stuMeasureId + " success");
                                else
                                    logHelper.Debug("-->Update StudentMeasure -->StudentMeasure.ID = " + stuMeasureId + " failed");
                            }
                        }
                        dobEntity.Status = StudentDOBStatus.Processed;
                        _studentBussiness.UpdateStudentDOB(dobEntity);
                        logHelper.Info("-->Processed studentId " + stuId);
                    }
                    catch (Exception ex)
                    {
                        dobEntity.Status = StudentDOBStatus.Error;
                        _studentBussiness.UpdateStudentDOB(dobEntity);

                        logHelper.Debug("An error in change cpalls score,studentId:"
                            + stuId + ",OldDOB:" + dobEntity.OldDOB + ",NewDOB:" + dobEntity.NewDOB + ",error message:" + ex.Message);
                        AfterProcessQueues?.Invoke();
                    }
                }
            }
            else
            {
                logHelper.Info("-->There no student change DOB.");
            }

            #region Measure CutOffScore Changed  David disable it on 01/30/2017 It affects the database performance.
            /*
            _adeBusiness = new AdeBusiness();
            try{
            List<MeasureEntity> measureList = _adeBusiness.GetCutOffChangedMeasures();
            } 
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
                       List<StudentMeasureEntity> stuMeasureList = _cpallsBusiness.GetStuMeasuresByMeasureId(measureId, measure.UpdatedOn);
                        logHelper.Info("-->End GetStuMeasuresByMeasureId: " + measureId + ",stuMeasureList.count=" + stuMeasureList.Count);

                        List<StudentMeasureEntity> updateStudentMeasureList = new List<StudentMeasureEntity>();
                        foreach (StudentMeasureEntity stuMeasure in stuMeasureList)
                        {
                            logHelper.Info("-->A. stuMeasure.id = " + stuMeasure.ID + ",stuMeasure.SAId=" + stuMeasure.SAId);
                            temCutOffScoreList = cutOffScoreList.Where(c => c.Wave == stuMeasure.Assessment.Wave).ToList();
                            logHelper.Info("-->AA. stuMeasure.Assessment.StudentId = " + stuMeasure.Assessment.StudentId);

                            if (stuMeasure.Assessment.StudentId <= 0)
                                continue;

                            StudentEntity stuEntity = _studentBussiness.GetStudent(stuMeasure.Assessment.StudentId);
                            DateTime birthday = stuEntity.BirthDate;
                            string schoolyear = stuEntity.SchoolYear;

                            decimal benchmark = CpallsBusiness.CalculateBenchmark(temCutOffScoreList, birthday, schoolyear);
                            logHelper.Info("-->B.stuEntity.id" + stuEntity.ID + ",stuMeasure.Benchmark = " + stuMeasure.Benchmark + ", New_benchmark=" + benchmark);

                            if (stuMeasure.Benchmark != benchmark)
                            {
                                int stuMeasureId = stuMeasure.ID;
                                logHelper.Info("-->C. Begin update StudentMeasure -->StudentMeasure.ID = " + stuMeasure.ID);
                                int count = _cpallsBusiness.UpdateStudentMeasureBenchmark(stuMeasureId, benchmark, true);//Change to only update the related fields.   
                                if (count > 0)
                                    logHelper.Info("-->CC.Update StudentMeasure -->StudentMeasure.ID = " + stuMeasureId + "success");
                                else
                                    logHelper.Debug("-->C.Update StudentMeasure -->StudentMeasure.ID = " + stuMeasureId + " failed");
                            }
                        }

                        logHelper.Info("-->Change measure's cutoffscorechanged field to false,measure ID:" + measureId);

                        measure.CutOffScoresChanged = false;

                        // UpdateMeasure(MesureID,CutOffScoresChanged)  update Measures set CutOffScoresChanged=@xx where ID=@)
                        int updateResult = _adeBusiness.UpdateCutOffScoresChanged(measureId, false);
                        if (updateResult > 0)
                            logHelper.Info("-->Update measure success,measure ID:" + measureId);
                        else
                            logHelper.Debug("-->Update measure failed,measure ID:" + measureId);


                        logHelper.Info("-->Begin--Update All BenchmarkChanged to 0, measure ID:" + measureId);
                        //SQL = update StudentMeasures set BenchmarkChanged=0 where MeasureId=@ID and BenchmarkChanged=1
                        int result = _cpallsBusiness.UpdateBenchmarkChangedToFalse(measureId);
                        if (result > 0)
                            logHelper.Info("-->End-->Update All BenchmarkChanged to 0 success, measure ID:" + measureId + " count:" + result);
                        else
                            logHelper.Debug("-->End-->Update All BenchmarkChanged to 0 failed, measure ID:" + measureId);
                    }
                    catch (Exception ex)
                    {
                        logHelper.Debug("An error in change cpalls score by measure cutoffscores, measure ID:" + measureId
                            + ",error message:" + ex.Message);
                        if (AfterProcessQueues != null) AfterProcessQueues();
                    }
                }
            }
            else
            {
                logHelper.Info("-->There no measure change cutoff score.");
            }*/
            #endregion for Measure Cut Poing Change

            AfterProcessQueues?.Invoke();
        }
    }
}
