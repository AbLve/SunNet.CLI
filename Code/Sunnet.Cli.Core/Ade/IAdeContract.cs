using Sunnet.Cli.Core.Ade.Entities;
using Sunnet.Framework.Core.Tool;
using System.Collections.Generic;
using System.Linq;

/**************************************************************************
 * Developer: 		JackZhang
 * Computer:		JACKZ
 * Domain:			Jackz
 * CreatedOn:		2014/8/11 19:50:52
 * Description:		Please input class summary
 * Version History:	Created,2014/8/11 19:50:52
 * 
 * 
 **************************************************************************/

namespace Sunnet.Cli.Core.Ade
{
    public partial interface IAdeContract : IAdeDataContract
    {
        AssessmentEntity NewAssessmentEntity();
        AssessmentEntity GetAssessment(int id);
        bool IsAssessmentExist(int id);
        OperationResult InsertAssessment(AssessmentEntity entity);
        OperationResult UpdateAssessment(AssessmentEntity entity);

        OperationResult UnlockAssessment(int id);

        MeasureEntity NewMeasureEntity();
        MeasureEntity GetMeasure(int id);
        OperationResult InsertMeasure(MeasureEntity entity);
        OperationResult UpdateMeasure(MeasureEntity entity);

        int UpdateCutOffScoresChanged(int measureId, bool cutoffScoresChanged);
        OperationResult UpdateMeasures(List<MeasureEntity> entities);
        OperationResult DeleteMeasure(int id);
        OperationResult AdjustMeasureOrders(List<int> measureIds);
        void RecalculateTotalScore();

        ItemBaseEntity NewItemBaseEntity(ItemType type);
        ItemBaseEntity GetItemBase(int id);
        OperationResult InsertItem(ItemBaseEntity entity);
        OperationResult InsertItem(List<ItemBaseEntity> entities, bool isSave);
        OperationResult UpdateItem(ItemBaseEntity entity);
        OperationResult DeleteItem(int id);
        OperationResult AdjustItemsOrders(List<int> itemIds);

        AnswerEntity NewAnswerEntity();
        OperationResult InsertAnswer(AnswerEntity entity);
        OperationResult UpdateAnswer(AnswerEntity entity);
        OperationResult DeleteAnswers(int itemId);

        OperationResult DeleteAnswers(ICollection<AnswerEntity> answers, bool isSave);

        AdeLinkEntity NewAdeLinkEntity();
        OperationResult InsertLink<T>(List<AdeLinkEntity> entities) where T : IAdeLinkProperties;
        OperationResult UpdateLink<T>(List<AdeLinkEntity> entities) where T : IAdeLinkProperties;
        OperationResult DeleteLink<T>(int hostId, int linkType) where T : IAdeLinkProperties;

        CutOffScoreEntity NewCutOffScoreEntity();
        OperationResult InsertCutOffScore<T>(List<CutOffScoreEntity> entities) where T : ICutOffScoreProperties;
        OperationResult UpdateCutOffScore<T>(List<CutOffScoreEntity> entities) where T : ICutOffScoreProperties;
        OperationResult DeleteCutOffScore<T>(int hostId) where T : ICutOffScoreProperties;

        OperationResult ExecuteSql(string strSql);

        OperationResult InsertLayout(TxkeaLayoutEntity entity);
        OperationResult UpdateLayout(TxkeaLayoutEntity entity);
        TxkeaLayoutEntity GetLayout(int id);

        string ExecuteQuerySql(string sql);

        TxkeaBupTaskEntity GetTask(int id);
        OperationResult InsertTask(TxkeaBupTaskEntity entity);
        OperationResult DeleteTask(TxkeaBupTaskEntity entity);

        OperationResult UpdateTask(TxkeaBupTaskEntity entity);

        OperationResult InsertLogs(List<TxkeaBupLogEntity> entities, bool isSave);

        OperationResult DeleteLogs(List<TxkeaBupLogEntity> entities, bool isSave);

        OperationResult InsertWaveLog(WaveLogEntity entity);
        OperationResult UpdateWaveLog(WaveLogEntity entity);
        WaveLogEntity GetUserWavelog(int userId, int assessmentId);

        #region Benchmarks
        BenchmarkEntity NewBenchmarkEntity();
        OperationResult InsertBenchmark(BenchmarkEntity entity);
        OperationResult UpdateBenchmark(BenchmarkEntity entity);
        OperationResult UpdateBenchmarks(List<BenchmarkEntity> entities);
        OperationResult DeleteBenchmarks(int assessmentId);
        OperationResult DeleteBenchmarks(List<int> benchmarkIds);
        OperationResult InsertBenchmarks(List<BenchmarkEntity> entities);
        #endregion

        IQueryable<AssessmentReportEntity> AssessmentReports { get; }
        OperationResult InsertAssessmentReports(List<AssessmentReportEntity> lists);
        OperationResult DeleteAssessmentReports(List<AssessmentReportEntity> assessmentReports, bool isSave = true);

        IQueryable<PercentileRankLookupEntity> PercentileRanks { get; }

        IQueryable<AssessmentLegendEntity> AssessmentLegends { get; }
        OperationResult InsertAssessmentLegend(AssessmentLegendEntity entity);
        OperationResult InsertAssessmentLegends(List<AssessmentLegendEntity> lists);
        OperationResult DeleteAssessmentLegends(List<AssessmentLegendEntity> assessmentLegends, bool isSave = true);
        OperationResult UpdateAssessmentLegend(AssessmentLegendEntity entity);


        //Scores added by Tony 20170602

        ScoreEntity NewScoreEntity();

        OperationResult InsertScore(ScoreEntity entity);

        OperationResult UpdateScore(ScoreEntity entity);

        ScoreEntity GetScore(int id);

        OperationResult DeleteScoreAgeOrWaveBands(int scoreId);

        OperationResult DeleteScoreMeasureOrDefineCoefficients(int scoreId);

        OperationResult InsertScoreAgeOrWaveBands(List<ScoreAgeOrWaveBandsEntity> lists);

        OperationResult InsertScoreMeasureOrDefineCoefficients(List<ScoreMeasureOrDefineCoefficientsEntity> lists);


    }
}
