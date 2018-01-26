using LinqKit;
using Sunnet.Cli.Business.Ade.Models;
using Sunnet.Cli.Core.Ade.Entities;
using Sunnet.Cli.Core.Ade.Interfaces;
using Sunnet.Cli.Core.Common.Enums;
using Sunnet.Cli.Core.Cpalls;
using Sunnet.Cli.Core.Cpalls.Entities;
using Sunnet.Framework.Core.Extensions;
using Sunnet.Framework.Core.Tool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Cli.Business.Ade
{
    public partial class AdeBusiness
    {
        public ScoreEntity GetScoreEntity(int id)
        {
            return _adeContract.GetScore(id);
        }

        /// <summary>
        /// Get all Scores
        /// </summary>
        /// <returns></returns>
        public List<ScoreModel> GetScoreList(Expression<Func<ScoreEntity, bool>> condition, out int total,
            string sort = "ID", string order = "asc", int first = 0, int count = 10)
        {
            var query = _adeContract.Scores.AsExpandable()
               .Where(r => r.IsDeleted == false).OfType<ScoreEntity>()
               .Where(condition).OrderBy(sort, order)
               .Select(SelectScoreEntityToModel);
            total = query.Count();
            var result = query.Skip(first).Take(count).ToList();
            return result;

        }

        public OperationResult InsertScore(ScoreModel model)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            ScoreEntity entity = new ScoreEntity();

            entity.AssessmentId = model.AssessmentId;
            entity.ScoreName = model.ScoreName;
            entity.ScoreDomain = model.ScoreDomain;
            entity.MeanAdjustment = model.MeanAdjustment;
            entity.SDAdjustment = model.SDAdjustment;
            entity.TargetMean = model.TargetMean;
            entity.TargetSD = model.TargetSD;
            entity.TargetRound = model.TargetRound;
            entity.IsDeleted = false;
            entity.Description = model.Description;
            entity.GroupByLabel = model.GroupByLabel;

            entity.ScoreAgeOrWaveBands = model.ScoreAgeOrWave;
            entity.ScoreMeasureOrDefineCoefficients = model.ScoreMeasureOrDefineCoefficients;
            result = _adeContract.InsertScore(entity);

            //插入CutOffScores信息到表中
            if (model.CutOffScores.Count > 0)
            {
                model.CutOffScores.ForEach(x => x.HostId = entity.ID);
                result = _adeContract.InsertCutOffScore<ScoreEntity>(model.CutOffScores);
            }
            return result;
        }


        public OperationResult DeleteScore(int id)
        {
            var result = new OperationResult(OperationResultType.Success);
            var entity = _adeContract.GetScore(id);
            if (entity == null)
            {
                return result;
            }
            entity.IsDeleted = true;
            result = _adeContract.UpdateScore(entity);
            return result;
        }


        public OperationResult EditSaveScore(ScoreModel model)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            ScoreEntity entity = _adeContract.GetScore(model.ID);
            entity.ID = model.ID;
            entity.AssessmentId = model.AssessmentId;
            entity.ScoreName = model.ScoreName;
            entity.ScoreDomain = model.ScoreDomain;
            entity.MeanAdjustment = model.MeanAdjustment;
            entity.SDAdjustment = model.SDAdjustment;
            entity.TargetMean = model.TargetMean;
            entity.TargetSD = model.TargetSD;
            entity.TargetRound = model.TargetRound;
            entity.Description = model.Description;
            entity.GroupByLabel = model.GroupByLabel;
            entity.UpdatedOn = DateTime.Now;
            result = _adeContract.UpdateScore(entity);


            bool coefficientChanged = false;
            List<ScoreMeasureOrDefineCoefficientsEntity> oldCoefficients = entity.ScoreMeasureOrDefineCoefficients.ToList();
            List<ScoreMeasureOrDefineCoefficientsEntity> newCoefficients = model.ScoreMeasureOrDefineCoefficients;
            if (oldCoefficients.Count != newCoefficients.Count)
            {
                coefficientChanged = true;
            }
            else
            {
                bool isExist = true;
                foreach (ScoreMeasureOrDefineCoefficientsEntity newModel in newCoefficients)
                {
                    isExist = oldCoefficients.Exists(r => r.Wave == newModel.Wave && r.Measure == newModel.Measure
                        && r.Coefficient == newModel.Coefficient);
                    if (isExist == false)
                    {
                        coefficientChanged = true;
                        break;
                    }
                }
            }
            if (newCoefficients.Count > 0 && coefficientChanged)
            {
                if (result.ResultType == OperationResultType.Success)
                {
                    result = _adeContract.DeleteScoreMeasureOrDefineCoefficients(model.ID);
                }
                if (result.ResultType == OperationResultType.Success)
                {
                    model.ScoreMeasureOrDefineCoefficients.ForEach(x => x.ScoreId = model.ID);
                    result = _adeContract.InsertScoreMeasureOrDefineCoefficients(model.ScoreMeasureOrDefineCoefficients);
                }
            }

            bool bandChanged = false;
            List<ScoreAgeOrWaveBandsEntity> oldBands = entity.ScoreAgeOrWaveBands.ToList();
            List<ScoreAgeOrWaveBandsEntity> newBands = model.ScoreAgeOrWave;
            if (oldBands.Count != newBands.Count)
            {
                bandChanged = true;
            }
            else
            {
                bool isExist = true;
                foreach (ScoreAgeOrWaveBandsEntity newModel in newBands)
                {
                    isExist = oldBands.Exists(r => r.Wave == newModel.Wave && r.AgeMin == newModel.AgeMin
                        && r.AgeMax == newModel.AgeMax && r.AgeOrWaveMean == newModel.AgeOrWaveMean
                        && r.AgeOrWave == newModel.AgeOrWave);
                    if (isExist == false)
                    {
                        bandChanged = true;
                        break;
                    }
                }
            }
            if (newBands.Count > 0 && bandChanged)
            {
                if (result.ResultType == OperationResultType.Success)
                {
                    result = _adeContract.DeleteScoreAgeOrWaveBands(model.ID);
                }
                if (result.ResultType == OperationResultType.Success)
                {
                    model.ScoreAgeOrWave.ForEach(x => x.ScoreId = model.ID);
                    result = _adeContract.InsertScoreAgeOrWaveBands(model.ScoreAgeOrWave);
                }
            }

            bool cutOffScoresChanged = false;
            List<CutOffScoreEntity> OldCutOffScores = GetCutOffScores<ScoreEntity>(entity.ID);
            List<CutOffScoreEntity> NewCutOffScores = model.CutOffScores;
            if (OldCutOffScores.Count != NewCutOffScores.Count)
                cutOffScoresChanged = true;
            else
            {
                bool isExist = true;
                foreach (CutOffScoreEntity newModel in NewCutOffScores)
                {
                    isExist = OldCutOffScores.Exists(r => r.FromYear == newModel.FromYear && r.FromMonth == newModel.FromMonth
                        && r.ToYear == newModel.ToYear && r.ToMonth == newModel.ToMonth
                        && r.Wave == newModel.Wave && r.BenchmarkId == newModel.BenchmarkId
                        && r.LowerScore == newModel.LowerScore && r.HigherScore == newModel.HigherScore
                        && r.ShowOnGroup == newModel.ShowOnGroup);
                    if (isExist == false)
                    {
                        cutOffScoresChanged = true;
                        break;
                    }
                }
            }
            if (model.CutOffScores.Count > 0 && cutOffScoresChanged)
            {
                if (result.ResultType == OperationResultType.Success)
                {
                    result = _adeContract.DeleteCutOffScore<ScoreEntity>(model.ID);
                }
                if (result.ResultType == OperationResultType.Success)
                {
                    model.CutOffScores.ForEach(x => x.HostId = model.ID);
                    result = _adeContract.InsertCutOffScore<ScoreEntity>(model.CutOffScores);
                }
            }
            return result;
        }



        public ScoreModel GetScore(int id)
        {
            var model = _adeContract.Scores.Select(SelectScoreEntityToModel).FirstOrDefault(x => x.ID == id);
            if (model != null)
            {
                model.ScoreAgeOrWave = GetScoreAgeOrWaveBandsList(id);
                model.ScoreMeasureOrDefineCoefficients = GetScoreMeasureOrDefineCoefficientsList(id);
            }
            return model;
        }

        public List<ScoreEntity> GetScoreReports(List<int> scoreIds)
        {
            return _adeContract.Scores.Where(x => scoreIds.Contains(x.ID)).ToList();
        }

        public List<ScoreModel> GetScores(List<int> scoreIds)
        {
            return _adeContract.Scores.Select(SelectScoreEntityToModel).Where(x => scoreIds.Contains(x.ID)).ToList();
        }


        private static Expression<Func<ScoreEntity, ScoreModel>> SelectScoreEntityToModel
        {
            get
            {
                return x => new ScoreModel()
                {
                    ID = x.ID,
                    AssessmentId = x.AssessmentId,
                    ScoreName = x.ScoreName,
                    ScoreDomain = x.ScoreDomain,
                    MeanAdjustment = x.MeanAdjustment,
                    SDAdjustment = x.SDAdjustment,
                    TargetMean = x.TargetMean,
                    TargetSD = x.TargetSD,
                    TargetRound = x.TargetRound,
                    IsDeleted = x.IsDeleted,
                    Description = x.Description,
                    GroupByLabel = x.GroupByLabel
                };
            }
        }



        /// <summary>
        /// Get ALL ScoreAgeOrWaveBands
        /// </summary>
        /// <returns></returns>
        public List<ScoreAgeOrWaveBandsEntity> GetScoreAgeOrWaveBandsList(int scoreId)
        {
            return _adeContract.ScoreAgeOrWaveBands.Where(r => r.ScoreId == scoreId).ToList();

        }
        public List<ScoreAgeOrWaveBandsEntity> GetScoreAgeOrWaveBandsList(List<int> scoreIds)
        {
            return _adeContract.ScoreAgeOrWaveBands.Where(r => scoreIds.Contains(r.ScoreId)).ToList();
        }

        /// <summary>
        /// Get ALL ScoreMeasureOrDefineCoefficients
        /// </summary>
        /// <returns></returns>
        public List<ScoreMeasureOrDefineCoefficientsEntity> GetScoreMeasureOrDefineCoefficientsList(int scoreId)
        {
            return _adeContract.ScoreMeasureOrDefineCoefficients.Where(r => r.ScoreId == scoreId).ToList();
        }
        public List<ScoreMeasureOrDefineCoefficientsEntity> GetScoreMeasureOrDefineCoefficientsList(List<int> scoreIds)
        {
            return _adeContract.ScoreMeasureOrDefineCoefficients.Where(r => scoreIds.Contains(r.ScoreId) && r.Measure > 0).ToList();
        }


        public Object GetMeasureByAssessmentId(int assessmentId)
        {
            var query = _adeContract.Measures.Where(x =>
            x.AssessmentId == assessmentId
            && x.IsDeleted == false
            && x.Status == EntityStatus.Active).Select(x => new
            {
                ID = x.ID,
                ParentId = x.ParentId,
                Label = x.Label
            }).ToList();
            return query;
        }

        public bool HasScores(int assessmentID)
        {
            ////Tony, Please complete 
            ////SQL: select * from scores where AssessmentId=xxx
            //return false;

            List<ScoreEntity> list = _adeContract.Scores.Where(a => a.AssessmentId == assessmentID).ToList();

            if (list == null || list.Count == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public int GetOneSaid(int studentID, int assessmentID, int wave)
        {
            //////  said = select id from StudentAssessments where studentid = @studentID and AssessmentId = @assessmentID and wave = @wave
            //return 1;//said

            StudentAssessmentEntity stuA = _cpallsContract.Assessments.FirstOrDefault(a => a.StudentId == studentID &&
                                       a.AssessmentId == assessmentID && a.Wave == (Wave)wave && a.Measures.Any(e => e.Goal > -1 && e.Status == CpallsStatus.Finished));
            if (stuA != null)
            {
                return stuA.ID;
            }
            else
            {
                return 0;
            }

        }

        public List<int> GetSaids(List<int> studentIDs, int assessmentID, List<int> waves)
        {
            ////  saids = select id from StudentAssessments where studentid in(@studentID) and AssessmentId = 9 and wave = @waves)
            //return new List<int>();//saids

            List<int> list = new List<int>();
            List<StudentAssessmentEntity> stuAList = _cpallsContract.Assessments.Where(a => a.AssessmentId == assessmentID).ToList();

            foreach (var item in stuAList)
            {
                if (studentIDs.Contains(item.ID) && waves.Contains((int)item.Wave))
                {
                    list.Add(item.ID);
                }
            }

            return list;
        }

        /// <summary>
        /// 此方法为Student Result Report使用,与GetAllFinalResult的区别是：这个方法需要查询出每个score下每个measure得到的goal值
        /// </summary>
        /// <param name="studentIds"></param>
        /// <param name="assessmentId"></param>
        /// <param name="waves"></param>
        /// <param name="scoreIds"></param>
        /// <returns></returns>
        public List<ScoreReportModel> GetStudentResultReportFinalResult(List<int> studentIds, List<Wave> waves, string schoolYear,
            List<int> scoreIds, DateTime startDate, DateTime endDate)
        {
            List<ScoreReportModel> scoreReports = new List<ScoreReportModel>();
            #region 这里是将需要的数据先取出来放在内存中，避免下面循环中重复查询
            var scores = _adeContract.Scores.Where(e => scoreIds.Contains(e.ID)).ToList();
            if (!scores.Any())
                return scoreReports;
            List<int> assessmentIds = scores.Select(e => e.AssessmentId).Distinct().ToList();
            List<MeasureEntity> measures = _adeContract.Measures.Where(e => assessmentIds.Contains(e.AssessmentId)).ToList();
            List<StudentAssessmentEntity> studentAssessments =
                _cpallsContract.Assessments.Where(
                    a => waves.Contains(a.Wave) && assessmentIds.Contains(a.AssessmentId) && studentIds.Contains(a.StudentId) && waves.Contains(a.Wave)
                         && a.Measures.Any(e => e.UpdatedOn >= startDate && e.UpdatedOn <= endDate && e.Goal > -1 && e.Status == CpallsStatus.Finished))
                    .ToList();
            List<int> saIds = studentAssessments.Select(e => e.ID).ToList();
            List<StudentMeasureEntity> studentMeasures =
                _cpallsContract.Measures.Where(m => saIds.Contains(m.SAId) && m.UpdatedOn >= startDate && m.UpdatedOn <= endDate && m.Goal > -1).ToList();
            var students = StudentBusiness.GetStudentsGetIds(studentIds);
            List<ScoreAgeOrWaveBandsEntity> scoreAgeOrWaveBandsAll = GetScoreAgeOrWaveBandsList(scoreIds);
            List<ScoreMeasureOrDefineCoefficientsEntity> measureCoefficients = GetScoreMeasureOrDefineCoefficientsList(scoreIds);

            List<CutOffScoreEntity> cutOffScores = GetCutOffScores<ScoreEntity>(scoreIds);
            #endregion

            //下面代码是循环wave，score，student，将计算出的所有finalscore存到集合中，在报表和其他地方再过滤每个student和score的finalscore值
            foreach (var wave in waves)
            {
                foreach (var scoreId in scoreIds)
                {
                    foreach (var studentId in studentIds)
                    {
                        ScoreEntity score = scores.FirstOrDefault(a => a.ID == scoreId);
                        var studentAssessment =
                            studentAssessments.FirstOrDefault(
                                e => e.StudentId == studentId && e.AssessmentId == score.AssessmentId && e.Wave == wave);

                        ScoreReportModel scoreReportModel = new ScoreReportModel();
                        scoreReportModel.AssessmentId = 0;
                        scoreReportModel.ScoreId = scoreId;
                        scoreReportModel.ScoreDomain = score.ScoreDomain;
                        scoreReportModel.ScoreDescription = score.Description;
                        var student = students.FirstOrDefault(e => e.ID == studentId);
                        if (student != null)
                            scoreReportModel.StudentName = student.StudentName;
                        scoreReportModel.StudentId = studentId;
                        scoreReportModel.Wave = wave;
                        scoreReportModel.TargetRound = score.TargetRound;
                        scoreReportModel.FinalScore = null;

                        #region Student Result Report show Measure goal
                        var coefficientsScoreAllWaves = measureCoefficients.Where(e => e.ScoreId == scoreId && waves.Contains(e.Wave)).ToList();
                        List<ScoreMeasureModel> scoreMeasures = new List<ScoreMeasureModel>();
                        //将Score下所有的Wave下的所有Measure存入集合中，在报表中展示所有的Measure
                        foreach (var coefficient in coefficientsScoreAllWaves)
                        {
                            ScoreMeasureModel scoreMeasureModel = new ScoreMeasureModel();
                            scoreMeasureModel.MeasureId = coefficient.Measure;
                            scoreMeasureModel.MeasureName = coefficient.MeasureObject.Name;
                            StudentMeasureEntity studentMeasure = null;
                            if (studentAssessment != null)
                            {
                                studentMeasure = studentMeasures.FirstOrDefault(m => m.SAId == studentAssessment.ID
                              && m.UpdatedOn >= startDate && m.UpdatedOn <= endDate && m.MeasureId == coefficient.Measure);
                            }
                            if (studentMeasure == null)
                                scoreMeasureModel.Goal = null;
                            else
                                scoreMeasureModel.Goal = studentMeasure.Goal;
                            scoreMeasureModel.Wave = wave;
                            scoreMeasures.Add(scoreMeasureModel);
                        }
                        scoreReportModel.ScoreMeasures = scoreMeasures;
                        #endregion

                        var coefficients = measureCoefficients.Where(e => e.ScoreId == scoreId && e.Wave == wave).ToList();
                        if (studentAssessment != null && coefficients.Any())
                        {
                            //将score下当前wave下的measure coefficients查询出来
                            #region 下面这段代码是判断每个custom score下当前wave下的Measure/Define Coefficients和Cpalls里做完题目的measure对比
                            List<StudentMeasureEntity> stuMeasureList = studentMeasures.Where(m => m.SAId == studentAssessment.ID
                            && m.UpdatedOn >= startDate && m.UpdatedOn <= endDate).ToList();

                            var studentMeasureIds = stuMeasureList.Where(e => e.Goal > -1).Select(e => e.MeasureId).Distinct().ToList();
                            var scoreMeasureIds = coefficients.Select(e => e.Measure).Distinct().ToList();
                            //当前score和wave下的measure在Cpalls里这些measure没有全部做完题目
                            if (!scoreMeasureIds.All(e => studentMeasureIds.Contains(e)))
                            {
                                scoreReportModel.FinalScore = null;
                                scoreReports.Add(scoreReportModel);
                                continue;
                            }
                            #endregion
                            //当前score和wave下的measure在Cpalls里全部做完了题目，这时需要计算出FinalScore值
                            int saId = studentAssessment.ID;
                            decimal CoeffAndMeasure = 0;
                            //将所有符合条件的StudentMeasures中的Goal值相加,便于后面FinalScore值的计算
                            foreach (var measureCoe in coefficients)
                            {
                                foreach (var stuMeasure in stuMeasureList)
                                {
                                    if (measureCoe.Measure == stuMeasure.MeasureId)
                                    {
                                        CoeffAndMeasure += measureCoe.Coefficient * stuMeasure.Goal;
                                    }
                                }
                            }

                            ScoreAgeOrWaveBandsEntity scoreAgeOrWaveBands =
                                scoreAgeOrWaveBandsAll.FirstOrDefault(s => s.Wave == (Wave)wave && s.ScoreId == scoreId);
                            if (scoreAgeOrWaveBands == null)
                                scoreAgeOrWaveBands = new ScoreAgeOrWaveBandsEntity();

                            decimal Base_CS = score.MeanAdjustment / score.SDAdjustment + CoeffAndMeasure;

                            decimal Age_Wave_CS = 0M;
                            if (scoreAgeOrWaveBands.AgeOrWave > 0)
                                Age_Wave_CS = (Base_CS - scoreAgeOrWaveBands.AgeOrWaveMean) / scoreAgeOrWaveBands.AgeOrWave;

                            //decimal Target_CS = Math.Round(score.TargetMean + Age_Wave_CS * score.TargetSD, (int)score.TargetRound);

                            decimal finalResult = 0M;
                            if (scoreAgeOrWaveBands.AgeOrWave > 0)
                            {
                                finalResult =
                                    Math.Round(
                                        score.TargetMean +
                                        ((score.MeanAdjustment / score.SDAdjustment + CoeffAndMeasure / score.SDAdjustment -
                                          scoreAgeOrWaveBands.AgeOrWaveMean) / scoreAgeOrWaveBands.AgeOrWave) * score.TargetSD, score.TargetRound);
                            }
                            scoreReportModel.FinalScore = finalResult;
                        }
                        scoreReports.Add(scoreReportModel);
                    }
                }
            }

            return scoreReports;
        }

        /// <summary>
        /// 此方法为Cpalls Custom Score Report使用，需要根据日期过滤
        /// </summary>
        /// <param name="studentIds"></param>
        /// <param name="assessmentId"></param>
        /// <param name="waves"></param>
        /// <param name="scoreIds"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public List<ScoreReportModel> GetAllFinalResult(List<int> studentIds, int assessmentId, List<Wave> waves, string schoolYear,
            List<int> scoreIds, DateTime startDate, DateTime endDate)
        {
            List<ScoreReportModel> scoreReports = new List<ScoreReportModel>();
            #region 这里是将需要的数据先取出来放在内存中，避免下面循环中重复查询
            var scores = _adeContract.Scores.Where(e => e.AssessmentId == assessmentId && scoreIds.Contains(e.ID)).ToList();
            if (!scores.Any())
                return scoreReports;
            List<MeasureEntity> measures = _adeContract.Measures.Where(e => e.AssessmentId == assessmentId).ToList();
            List<StudentAssessmentEntity> studentAssessments =
                _cpallsContract.Assessments.Where(
                    a => waves.Contains(a.Wave) && a.AssessmentId == assessmentId && studentIds.Contains(a.StudentId) && waves.Contains(a.Wave)
                         && a.Measures.Any(e => e.UpdatedOn >= startDate && e.UpdatedOn <= endDate && e.Goal > -1 && e.Status == CpallsStatus.Finished))
                    .ToList();
            List<int> saIds = studentAssessments.Select(e => e.ID).ToList();
            List<StudentMeasureEntity> studentMeasures =
                _cpallsContract.Measures.Where(m => saIds.Contains(m.SAId) && m.UpdatedOn >= startDate && m.UpdatedOn <= endDate && m.Goal > -1).ToList();
            var students = StudentBusiness.GetStudentsGetIds(studentIds);
            List<ScoreAgeOrWaveBandsEntity> scoreAgeOrWaveBandsAll = GetScoreAgeOrWaveBandsList(scoreIds);
            List<ScoreMeasureOrDefineCoefficientsEntity> measureCoefficients = GetScoreMeasureOrDefineCoefficientsList(scoreIds);

            List<CutOffScoreEntity> cutOffScores = GetCutOffScores<ScoreEntity>(scoreIds);
            List<BenchmarkModel> benchmarks = GetIEnumBenchmarks(assessmentId).ToList();
            #endregion

            //下面代码是循环wave，score，student，将计算出的所有finalscore存到集合中，在报表和其他地方再过滤每个student和score的finalscore值
            foreach (var wave in waves)
            {
                foreach (var scoreId in scoreIds)
                {
                    foreach (var studentId in studentIds)
                    {
                        var studentAssessment =
                            studentAssessments.FirstOrDefault(
                                e => e.StudentId == studentId && e.AssessmentId == assessmentId && e.Wave == wave);

                        ScoreEntity score = scores.FirstOrDefault(a => a.ID == scoreId);

                        #region Init ScoreReportModel
                        ScoreReportModel scoreReportModel = new ScoreReportModel();
                        scoreReportModel.AssessmentId = assessmentId;
                        scoreReportModel.ScoreId = scoreId;
                        scoreReportModel.ScoreDomain = score.ScoreDomain;
                        scoreReportModel.ScoreDescription = score.Description;
                        scoreReportModel.TargetRound = score.TargetRound;
                        var student = students.FirstOrDefault(e => e.ID == studentId);
                        if (student != null)
                            scoreReportModel.StudentName = student.StudentName;
                        scoreReportModel.StudentId = studentId;
                        scoreReportModel.Wave = wave;
                        scoreReportModel.FinalScore = null;
                        #endregion

                        //将score下当前wave下的measure coefficients查询出来
                        var coefficients = measureCoefficients.Where(e => e.ScoreId == scoreId && e.Wave == wave).ToList();
                        if (studentAssessment != null && coefficients.Any())
                        {
                            #region 下面这段代码是判断每个custom score下当前wave下的Measure/Define Coefficients和Cpalls里做完题目的measure对比
                            List<StudentMeasureEntity> stuMeasureList = studentMeasures.Where(m => m.SAId == studentAssessment.ID
                            && m.UpdatedOn >= startDate && m.UpdatedOn <= endDate).ToList();
                            var studentMeasureIds = stuMeasureList.Where(e => e.Goal > -1).Select(e => e.MeasureId).Distinct().ToList();
                            var scoreMeasureIds = coefficients.Select(e => e.Measure).Distinct().ToList();
                            //当前score和wave下的measure在Cpalls里这些measure没有全部做完题目
                            if (!scoreMeasureIds.All(e => studentMeasureIds.Contains(e)))
                            {
                                scoreReportModel.FinalScore = null;
                                scoreReports.Add(scoreReportModel);
                                continue;
                            }
                            #endregion
                            //当前score和wave下的measure在Cpalls里全部做完了题目，这时需要计算出FinalScore值
                            int saId = studentAssessment.ID;
                            decimal CoeffAndMeasure = 0;
                            //将所有符合条件的StudentMeasures中的Goal值相加,便于后面FinalScore值的计算
                            foreach (var measureCoe in coefficients)
                            {
                                foreach (var stuMeasure in stuMeasureList)
                                {
                                    if (measureCoe.Measure == stuMeasure.MeasureId)
                                    {
                                        CoeffAndMeasure += measureCoe.Coefficient * stuMeasure.Goal;
                                    }
                                }
                            }

                            ScoreAgeOrWaveBandsEntity scoreAgeOrWaveBands =
                                scoreAgeOrWaveBandsAll.FirstOrDefault(s => s.Wave == (Wave)wave && s.ScoreId == scoreId);
                            if (scoreAgeOrWaveBands == null)
                                scoreAgeOrWaveBands = new ScoreAgeOrWaveBandsEntity();

                            decimal Base_CS = score.MeanAdjustment / score.SDAdjustment + CoeffAndMeasure;

                            decimal Age_Wave_CS = 0M;
                            if (scoreAgeOrWaveBands.AgeOrWave > 0)
                                Age_Wave_CS = (Base_CS - scoreAgeOrWaveBands.AgeOrWaveMean) / scoreAgeOrWaveBands.AgeOrWave;

                            //decimal Target_CS = Math.Round(score.TargetMean + Age_Wave_CS * score.TargetSD, (int)score.TargetRound);

                            decimal finalResult = 0M;
                            if (scoreAgeOrWaveBands.AgeOrWave > 0)
                            {
                                finalResult =
                                    Math.Round(
                                        score.TargetMean +
                                        ((score.MeanAdjustment / score.SDAdjustment + CoeffAndMeasure / score.SDAdjustment -
                                          scoreAgeOrWaveBands.AgeOrWaveMean) / scoreAgeOrWaveBands.AgeOrWave) * score.TargetSD, score.TargetRound);
                            }
                            scoreReportModel.FinalScore = finalResult;

                            #region benchmark calc
                            List<CutOffScoreEntity> currentCutOffScores = cutOffScores.Where(e => e.HostId == scoreId && e.Wave == wave
                            && e.LowerScore <= finalResult && e.HigherScore >= finalResult).ToList();
                            var currentYear = Common.CommonAgent.GetStartDateForAge(schoolYear);
                            foreach (var currentCutOffScore in currentCutOffScores)
                            {
                                var currentEndDate = currentYear.AddYears(0 - currentCutOffScore.FromYear);
                                currentEndDate = currentEndDate.AddMonths(0 - currentCutOffScore.FromMonth);
                                var currentStartDate = currentYear.AddYears(0 - currentCutOffScore.ToYear);
                                currentStartDate = currentStartDate.AddMonths(0 - currentCutOffScore.ToMonth);

                                //logger.Info(@"E--> AdeBusiness.GetCutOffScore,CutOffScore ID:" + entity.ID+" CurrentStartDate:"+ currentStartDate+",CurrentEndDate:"+currentEndDate+",birthDate:"+birthday);
                                if (student.BirthDate > currentStartDate && student.BirthDate <= currentEndDate)
                                {
                                    //匹配到具体的Benchmark，然后将需要的值赋给相应的字段
                                    var matchingBenchmark = benchmarks.FirstOrDefault(e => e.ID == currentCutOffScore.BenchmarkId);
                                    scoreReportModel.BenchmarkId = matchingBenchmark.ID;
                                    scoreReportModel.Color = matchingBenchmark.Color;
                                    scoreReportModel.LabelText = matchingBenchmark.LabelText;
                                    scoreReportModel.BlackWhite = matchingBenchmark.BlackWhite.ToString().ToLower();
                                }
                            }

                            #endregion
                        }
                        scoreReports.Add(scoreReportModel);
                    }
                }
            }


            return scoreReports;
        }

        /// <summary>
        /// 计算出所选Score的最大finalscore值,在报表中使用
        /// </summary>
        /// <param name="assessmentId"></param>
        /// <param name="waves"></param>
        /// <param name="scoreIds"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public List<ScoreInitModel> GetScoreInits(int assessmentId, Wave wave,
            List<int> scoreIds)
        {
            List<ScoreInitModel> scoreInits = new List<ScoreInitModel>();
            var scores = _adeContract.Scores.Where(e => e.AssessmentId == assessmentId && scoreIds.Contains(e.ID)).ToList();
            if (!scores.Any())
                return scoreInits;
            List<ScoreAgeOrWaveBandsEntity> scoreAgeOrWaveBandsAll = GetScoreAgeOrWaveBandsList(scoreIds);
            List<ScoreMeasureOrDefineCoefficientsEntity> measureCoefficients = GetScoreMeasureOrDefineCoefficientsList(scoreIds);
            List<CutOffScoreEntity> cutOffScores = GetCutOffScores<ScoreEntity>(scoreIds);
            List<BenchmarkModel> benchmarks = GetIEnumBenchmarks(assessmentId).ToList();

            foreach (var scoreId in scoreIds)
            {
                ScoreInitModel scoreInitModel = new ScoreInitModel();
                ScoreEntity score = scores.FirstOrDefault(a => a.ID == scoreId);
                var coefficients = measureCoefficients.Where(e => e.ScoreId == scoreId && e.Wave == wave && e.Measure > 0).ToList();
                //报表的最底部需要列出每个Score下的Measure数据
                List<ScoreMeasureModel> scoreMeasures = new List<ScoreMeasureModel>();
                scoreMeasures = coefficients.ToList().Select(e => new ScoreMeasureModel() { MeasureId = e.Measure, MeasureName = e.MeasureObject.Name }).ToList();
                scoreInitModel.ScoreMeasures = scoreMeasures;

                //将每个Score下的Benchmark查询出来，在报表中使用
                List<CutOffScoreEntity> currentCutOffScores = cutOffScores.Where(e => e.HostId == scoreId && e.Wave == wave).ToList();
                var matchingBenchmark = benchmarks.Where(e => currentCutOffScores.Select(r => r.BenchmarkId).Contains(e.ID));
                List<ScoreBenchmarkModel> scoreBenchmarks = new List<ScoreBenchmarkModel>();
                scoreBenchmarks = matchingBenchmark.ToList().Select(e => new ScoreBenchmarkModel() { BenchmarkId = e.ID, BenchmarkLabel = e.LabelText }).ToList();
                scoreInitModel.ScoreBenchmarks = scoreBenchmarks;

                decimal CoeffAndMeasure = 0;
                //将Measure的TotalScore值相加,便于后面计算FinalScore最大值
                foreach (var measureCoe in coefficients)
                {
                    CoeffAndMeasure += measureCoe.Coefficient * measureCoe.MeasureObject.TotalScore;
                }

                ScoreAgeOrWaveBandsEntity scoreAgeOrWaveBands =
                    scoreAgeOrWaveBandsAll.FirstOrDefault(s => s.Wave == (Wave)wave && s.ScoreId == scoreId);
                if (scoreAgeOrWaveBands == null)
                    scoreAgeOrWaveBands = new ScoreAgeOrWaveBandsEntity();

                decimal finalResult = 0M;
                if (scoreAgeOrWaveBands.AgeOrWave > 0)
                {
                    finalResult =
                        Math.Round(
                            score.TargetMean +
                            ((score.MeanAdjustment / score.SDAdjustment + CoeffAndMeasure / score.SDAdjustment -
                              scoreAgeOrWaveBands.AgeOrWaveMean) / scoreAgeOrWaveBands.AgeOrWave) * score.TargetSD, score.TargetRound);
                }
                scoreInitModel.FinalScore = finalResult;
                scoreInitModel.ScoreId = scoreId;
                scoreInitModel.ScoreDomain = score.ScoreDomain;
                scoreInitModel.TargetRound = score.TargetRound;
                scoreInits.Add(scoreInitModel);
            }
            return scoreInits;
        }

        /// <summary>
        /// 外面来调它的时候，一般是根据Assessment,Wave来给某个学生来
        /// said=select id from StudentAssessments where studentid=1 and AssessmentId=9 and wave=1
        /// </summary>
        /// <param name="said">StudentAssessments的主键</param>
        /// <param name="scoreId">当前Score的主键id</param>
        /// <returns></returns>

        public decimal GetFinalResult(int said, int scoreId)
        {
            //1.根据SAID获取AssessmentID和Wave
            int assessmentid = 0;
            int wave = 0;
            StudentAssessmentEntity stuA = _cpallsContract.Assessments.Where(a => a.ID == said).SingleOrDefault();
            if (stuA == null)
            {
                return 0M;
            }
            else
            {
                assessmentid = stuA.AssessmentId;
                wave = (int)stuA.Wave;
            }

            //2.判断对的assessmentID是否有Score,或许把它放外面去判断
            if (!HasScores(assessmentid))
            {
                return 0M;//错误，没有任何score设定，退出
            }

            //3.查询当前Score下面的 Measure Coeffients，获了MeasureID及 Coefficients (列表)
            //疑问：一个Assessment现在可以对应多个Score,所以把这个方法又新添加了一个参数scoreId获取到当前的Score,
            //但是在使用这个方法的时候从哪里传递当前Score的id主键值呢?

            ScoreEntity score = _adeContract.Scores.Where(a => a.ID == scoreId).SingleOrDefault();

            List<ScoreMeasureOrDefineCoefficientsEntity> measureCoefficients = new List<ScoreMeasureOrDefineCoefficientsEntity>();

            if (score == null)
            {
                return 0M;
            }
            else
            {
                measureCoefficients = score.ScoreMeasureOrDefineCoefficients.Distinct().ToList();
            }

            // //4. 根据第三步查出的MeasureID，找到SAID下所有的记录,找到Student 对应的Measure的得分(Goal)，注意，对于子Measure，要状态=3(完成)，父Measure，不管状态(因为它永远都是1)
            // //父Measure的定义：select * from Measures where ParentId=@measureID，如果存在,则@measureID是父measure,不管状态
            // //子Measure的定义: select * from Measures where ParentId=@measureID  如果不存在,则@measureID是子measure,状态Status=3
            // //select measureid, goal from StudentMeasures where said=xxx and measureid in(xxx) and status=3  (measure.parentID=0) or ..
            // //得到MeasureID及Goal的结果集


            List<ScoreMeasureOrDefineCoefficientsEntity> measureCoefficientsParent = new List<ScoreMeasureOrDefineCoefficientsEntity>();
            List<ScoreMeasureOrDefineCoefficientsEntity> measureCoefficientsChild = new List<ScoreMeasureOrDefineCoefficientsEntity>();
            MeasureEntity measureParent = new MeasureEntity();
            //根据获取的当前Socre下的所有Measure Coeffients信息（里面有父measureID和子measureID），然后根据判断条件分别放到父集合和子集合中
            foreach (var item in measureCoefficients)
            {
                measureParent = _adeContract.Measures.Where(m => m.ParentId == item.Measure).SingleOrDefault();
                if (measureParent != null)
                {
                    measureCoefficientsParent.Add(item);
                }
                else
                {
                    measureCoefficientsChild.Add(item);
                }
            }

            //分别遍历上面的集合,获取父StudentMeasureEntity集合信息和子StudentMeasureEntity集合信息
            List<StudentMeasureEntity> stuMeasureParentList = new List<StudentMeasureEntity>();
            foreach (var item in measureCoefficientsParent)
            {
                stuMeasureParentList = _cpallsContract.Measures.Where(m => m.SAId == said && m.MeasureId == item.Measure).ToList();
            }

            List<StudentMeasureEntity> stuMeasureChildList = new List<StudentMeasureEntity>();
            foreach (var item in measureCoefficientsChild)
            {
                stuMeasureChildList = _cpallsContract.Measures.Where(m => m.SAId == said && m.MeasureId == item.Measure && m.Status == CpallsStatus.Finished).ToList();
                stuMeasureParentList.AddRange(stuMeasureChildList);//合并父子StudentMeasure集合
            }


            decimal CoeffAndMeasure = 0;


            //if (list == null)
            //{
            //    list = new List<MeasureAndCoefficient>();
            //}
            //else if (list.Count > 0)
            //{
            //    foreach (var item in list)
            //    {
            //        CoeffAndMeasure += item.Coefficient * get;
            //    }
            //}


            foreach (var measureCoe in measureCoefficients)
            {
                foreach (var stuMeasure in stuMeasureParentList)
                {
                    if (measureCoe.Measure == stuMeasure.MeasureId)
                    {
                        CoeffAndMeasure += measureCoe.Coefficient * stuMeasure.Goal;
                    }
                }
            }

            ////(1)Base_CS=A2/B2 + (C2*D2 + E2*F2) / B2 （即G2 = A2/B2 + (C2*D2 + E2*F2) / B2）
            ////decimal Base_CS = Mean_Adjustment/ SD_Adjustment + (Coefficient1* Measure1+ Coefficient2* Measure2);
            //decimal Base_CS = score.MeanAdjustment / score.SDAdjustment + CoeffAndMeasure;

            ////（2）Age_Wave_CS = (G2 - I2) / J2（即K2 = (G2 - I2) / J2）
            //decimal Age_Wave_CS = (Base_CS - scoreAgeOrWaveBands.AgeOrWaveMean) / scoreAgeOrWaveBands.AgeOrWave;

            ////（3）Target_CS = ROUND(M2 + K2 * N2, 0) 即O2 = ROUND(M2 + K2 * N2, 0)
            //decimal Target_CS = Math.Round(score.TargetMean + Age_Wave_CS * score.TargetSD, (int)score.TargetRound);

            ////（4）Final = ROUND(M2 + ((A2/B2 + (C2*D2 + E2*F2)/B2 - I2) / J2)*N2,0)  （即Q2 = ROUND(M2 + ((A2/B2 + (C2*D2 + E2*F2)/B2 - I2) / J2)*N2,0)）
            ////decimal Final = Math.Round(Target_Mean + ((Mean_Adjustment / SD_Adjustment + (Coefficient1 * Measure1 + Coefficient2 * Measure2) / SD_Adjustment - 
            //                                                            //Age_Wave_group_mean) / Age_wave_Group_SD) * Target_SD, 0);
            //decimal Final = Math.Round(score.TargetMean + ((score.MeanAdjustment / score.SDAdjustment + CoeffAndMeasure / score.SDAdjustment -
            //                                                           scoreAgeOrWaveBands.AgeOrWaveMean) / scoreAgeOrWaveBands.AgeOrWave) * score.TargetSD, 0);
            //return Final;


            //获取具体wave下的数据
            ScoreAgeOrWaveBandsEntity scoreAgeOrWaveBands = score.ScoreAgeOrWaveBands.Where(s => s.Wave == (Wave)wave).FirstOrDefault();
            if (scoreAgeOrWaveBands == null)
                scoreAgeOrWaveBands = new ScoreAgeOrWaveBandsEntity();

            //(1)Base_CS=A2/B2 + (C2*D2 + E2*F2) / B2 （即G2 = A2/B2 + (C2*D2 + E2*F2) / B2）
            //decimal Base_CS = Mean_Adjustment/ SD_Adjustment + (Coefficient1* Measure1+ Coefficient2* Measure2);
            decimal Base_CS = score.MeanAdjustment / score.SDAdjustment + CoeffAndMeasure;

            //（2）Age_Wave_CS = (G2 - I2) / J2（即K2 = (G2 - I2) / J2）
            decimal Age_Wave_CS = 0M;
            if (scoreAgeOrWaveBands.AgeOrWave > 0)
                Age_Wave_CS = (Base_CS - scoreAgeOrWaveBands.AgeOrWaveMean) / scoreAgeOrWaveBands.AgeOrWave;

            //（3）Target_CS = ROUND(M2 + K2 * N2, 0) 即O2 = ROUND(M2 + K2 * N2, 0)
            //decimal Target_CS = Math.Round(score.TargetMean + Age_Wave_CS * score.TargetSD, (int)score.TargetRound);

            //（4）Final = ROUND(M2 + ((A2/B2 + (C2*D2 + E2*F2)/B2 - I2) / J2)*N2,0)  （即Q2 = ROUND(M2 + ((A2/B2 + (C2*D2 + E2*F2)/B2 - I2) / J2)*N2,0)）
            //decimal Final = Math.Round(Target_Mean + ((Mean_Adjustment / SD_Adjustment + (Coefficient1 * Measure1 + Coefficient2 * Measure2) / SD_Adjustment - 
            //Age_Wave_group_mean) / Age_wave_Group_SD) * Target_SD, 0);

            decimal finalResult = 0M;
            if (scoreAgeOrWaveBands.AgeOrWave > 0)
                finalResult =
                    Math.Round(
                        score.TargetMean +
                        ((score.MeanAdjustment / score.SDAdjustment + CoeffAndMeasure / score.SDAdjustment -
                          scoreAgeOrWaveBands.AgeOrWaveMean) / scoreAgeOrWaveBands.AgeOrWave) * score.TargetSD, score.TargetRound);
            return finalResult;


        }

        public List<ScoreEntity> GetScoresByAssessmentId(int assessmentId)
        {
            return _adeContract.Scores.Where(e => e.AssessmentId == assessmentId && e.IsDeleted == false).ToList();
        }

        public List<ScoreSelectModel> GetScoreSelectModels(int assessmentId)
        {
            return _adeContract.Scores.Where(e => e.AssessmentId == assessmentId && e.IsDeleted == false).
                Select(x => new ScoreSelectModel
                {
                    AssessmentId = x.AssessmentId,
                    ScoreId = x.ID,
                    ScoreName = x.ScoreName,
                    ScoreDomain = x.ScoreDomain
                }).OrderByDescending(x => x.ScoreId).ToList();
        }
    }
}

