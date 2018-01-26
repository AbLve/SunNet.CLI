using Sunnet.Cli.Core.Ade.Entities;
using Sunnet.Cli.Core.Ade.Entities;
using Sunnet.Cli.Core.Ade.Interfaces;
using Sunnet.Framework.Core.Base;
using Sunnet.Framework.Core.Tool;
using Sunnet.Framework.EmailSender;
using Sunnet.Framework.Encrypt;
using Sunnet.Framework.File;
using Sunnet.Framework.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using Sunnet.Cli.Core.Common.Enums;

/**************************************************************************
 * Developer: 		JackZhang
 * Computer:		JACKZ
 * Domain:			Jackz
 * CreatedOn:		2014/8/11 19:58:16
 * Description:		Please input class summary
 * Version History:	Created,2014/8/11 19:58:16
 * 
 * 
 **************************************************************************/

namespace Sunnet.Cli.Core.Ade
{
    internal partial class AdeService : CoreServiceBase, IAdeContract
    {
        #region Ctor
        private readonly IAdeLinkRpst _adeLinkRpst;
        private readonly IAnswerRpst _answerRpst;
        private readonly IAssessmentRpst _assessmentRpst;
        private readonly ICutOffScoreRpst _cutOffScoreRpst;
        private readonly IItemBaseRpst _itemBaseRpst;
        private readonly IMeasureRpst _measureRpst;
        private readonly ITxkeaLayoutRpst _txkeaLayoutRpst;
        private readonly ITxkeaBupTaskRpst _txkeaBupTaskRpst;
        private readonly ITxkeaBupLogRpst _txkeaBupLogRpst;
        private readonly IWaveLogRpst _wavelogRpst;
        private readonly IBenchmarkRpst _benchmarkRpst;
        private readonly IAssessmentReportRpst _assessmentReportRpst;
        private readonly IPercentileRankLookupRpst _percentileRankRpst;
        private readonly IAssessmentLegendRpst _assessmentLegendRpst;


        private readonly IScoreRpst _scoreRpst;
        private readonly IScoreAgeOrWaveBandsRpst _scoreAgeOrWaveBandsRpst;
        private readonly IScoreMeasureOrDefineCoefficientsRpst _scoreMeasureOrDefineCoefficientsRpst;






        public AdeService(ISunnetLog log, IFile fileHelper, IEmailSender emailSender, IEncrypt encrypt,
            IAdeLinkRpst adeLinkRpst,
            IAnswerRpst answerRpst,
            IAssessmentRpst assessmentRpst,
            ICutOffScoreRpst cutOffScoreRpst,
            IItemBaseRpst itemBaseRpst,
            IMeasureRpst measureRpst,
            ITxkeaLayoutRpst txkeaLayoutRpst,
            ITxkeaBupTaskRpst txkeaBupTaskRpst,
            ITxkeaBupLogRpst txkeaBupLogRpst,
            IWaveLogRpst wavelogRpst,
            IBenchmarkRpst benchmarkRpst,
            IAssessmentReportRpst assessmentReportRpst,
            IPercentileRankLookupRpst percentileRankRpst,
            IAssessmentLegendRpst assessmentLegendRpst,
            
            IScoreRpst scoreRpst,
            IScoreAgeOrWaveBandsRpst scoreAgeOrWaveBandsRpst,
            IScoreMeasureOrDefineCoefficientsRpst scoreMeasureOrDefineCoefficientsRpst,

            IUnitOfWork unit)
            : base(log, fileHelper, emailSender, encrypt)
        {
            _adeLinkRpst = adeLinkRpst;
            _answerRpst = answerRpst;
            _assessmentRpst = assessmentRpst;
            _cutOffScoreRpst = cutOffScoreRpst;
            _itemBaseRpst = itemBaseRpst;
            _measureRpst = measureRpst;
            _txkeaLayoutRpst = txkeaLayoutRpst;
            _txkeaBupTaskRpst = txkeaBupTaskRpst;
            _txkeaBupLogRpst = txkeaBupLogRpst;
            _wavelogRpst = wavelogRpst;
            _benchmarkRpst = benchmarkRpst;
            _assessmentReportRpst = assessmentReportRpst;
            _percentileRankRpst = percentileRankRpst;
            _assessmentLegendRpst = assessmentLegendRpst;

            _scoreRpst = scoreRpst;
            _scoreAgeOrWaveBandsRpst = scoreAgeOrWaveBandsRpst;
            _scoreMeasureOrDefineCoefficientsRpst = scoreMeasureOrDefineCoefficientsRpst;

            UnitOfWork = unit;
        }

        #endregion

        #region IQueryables
        public IQueryable<AdeLinkEntity> AdeLinks
        {
            get { return _adeLinkRpst.Entities; }
        }

        public IQueryable<AnswerEntity> Answers
        {
            get { return _answerRpst.Entities; }
        }

        public IQueryable<AssessmentEntity> Assessments
        {
            get { return _assessmentRpst.Entities.Where(x => x.ID > 1); }
        }

        public IQueryable<CutOffScoreEntity> CutOffScores
        {
            get { return _cutOffScoreRpst.Entities; }
        }

        public IQueryable<ItemBaseEntity> Items
        {
            get { return _itemBaseRpst.Entities; }
        }

        public IQueryable<MeasureEntity> Measures
        {
            get { return _measureRpst.Entities; }
        }

        public IQueryable<TxkeaLayoutEntity> TxkeaLayouts
        {
            get { return _txkeaLayoutRpst.Entities; }
        }

        public IQueryable<TxkeaBupTaskEntity> TxkeaBupTasks
        {
            get { return _txkeaBupTaskRpst.Entities; }
        }

        public IQueryable<TxkeaBupLogEntity> TxkeaBupLogs
        {
            get { return _txkeaBupLogRpst.Entities; }
        }

        public IQueryable<BenchmarkEntity> Benchmarks
        {
            get { return _benchmarkRpst.Entities; }
        }

        public IQueryable<AssessmentReportEntity> AssessmentReports
        {
            get { return _assessmentReportRpst.Entities; }
        }

        public IQueryable<PercentileRankLookupEntity> PercentileRanks
        {
            get { return _percentileRankRpst.Entities; }
        }

        public IQueryable<AssessmentLegendEntity> AssessmentLegends
        {
            get { return _assessmentLegendRpst.Entities; }
        }

        public IQueryable<ScoreEntity> Scores
        {
            get
            {
                //throw new NotImplementedException();
                return _scoreRpst.Entities;
            }
        }

        public IQueryable<ScoreAgeOrWaveBandsEntity> ScoreAgeOrWaveBands
        {
            get
            {
                //throw new NotImplementedException();
                return _scoreAgeOrWaveBandsRpst.Entities;
            }
        }

        public IQueryable<ScoreMeasureOrDefineCoefficientsEntity> ScoreMeasureOrDefineCoefficients
        {
            get
            {
                //throw new NotImplementedException();
                return _scoreMeasureOrDefineCoefficientsRpst.Entities;
            }
        }
        #endregion

        #region Assessment
        public AssessmentEntity GetAssessment(int id)
        {
            return _assessmentRpst.GetByKey(id);
        }

        public bool IsAssessmentExist(int id)
        {
            return _assessmentRpst.Entities.Any(c => c.IsDeleted == false && c.Status == EntityStatus.Active && c.ID == id);
        }

        public OperationResult InsertAssessment(AssessmentEntity entity)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                _assessmentRpst.Insert(entity);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult UpdateAssessment(AssessmentEntity entity)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                _assessmentRpst.Update(entity);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }
        public OperationResult UnlockAssessment(int id)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                _assessmentRpst.Unlock(id);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }
        #endregion

        #region Measure
        public MeasureEntity GetMeasure(int id)
        {
            return _measureRpst.GetByKey(id);
        }

        public OperationResult InsertMeasure(MeasureEntity entity)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                _measureRpst.Insert(entity);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult UpdateMeasure(MeasureEntity entity)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                _measureRpst.Update(entity);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public int UpdateCutOffScoresChanged(int measureId, bool cutoffScoresChanged)
        {
            int count = 0;
            try
            {
                count = _measureRpst.UpdateCutOffScoresChanged(measureId, cutoffScoresChanged);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return count;
        }
        public OperationResult UpdateMeasures(List<MeasureEntity> entities)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                entities.ForEach(x => _measureRpst.Update(x, false));
                UnitOfWork.Commit();
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult DeleteMeasure(int id)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                _measureRpst.Delete(id);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult AdjustMeasureOrders(List<int> measureIds)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                result.ResultType = _measureRpst.AdjustOrder(measureIds)
                    ? OperationResultType.Success
                    : OperationResultType.Error;
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        /// <summary>
        /// Recalculates measures' total scores.
        /// </summary>
        public void RecalculateTotalScore()
        {
            try
            {
                _measureRpst.RecalculateTotalScore();
            }
            catch (Exception ex)
            {
                ResultError(ex);
            }
        }

        #endregion

        #region Item
        public ItemBaseEntity GetItemBase(int id)
        {
            return _itemBaseRpst.GetByKey(id);
        }

        public OperationResult InsertItem(ItemBaseEntity entity)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                _itemBaseRpst.Insert(entity);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult InsertItem(List<ItemBaseEntity> entities, bool isSave = true)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                _itemBaseRpst.Insert(entities, isSave);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult UpdateItem(ItemBaseEntity entity)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                _itemBaseRpst.Update(entity);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult DeleteItem(int id)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                _itemBaseRpst.Delete(id);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult AdjustItemsOrders(List<int> itemIds)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                result.ResultType = _itemBaseRpst.AdjustOrder(itemIds)
                    ? OperationResultType.Success
                    : OperationResultType.Error;
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        #endregion

        #region Answer, AdeLink , CutOffScores
        public OperationResult InsertAnswer(AnswerEntity entity)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                _answerRpst.Insert(entity);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult UpdateAnswer(AnswerEntity entity)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                _answerRpst.Update(entity);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult DeleteAnswers(int itemId)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                _answerRpst.Delete(x => x.ItemId == itemId);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult DeleteAnswers(ICollection<AnswerEntity> answers, bool isSave)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                _answerRpst.Delete(answers, isSave);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult InsertLink<T>(List<AdeLinkEntity> entities) where T : IAdeLinkProperties
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                var type = typeof(T).ToString();
                entities.ForEach(x => x.HostType = type);
                _adeLinkRpst.Insert(entities);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult UpdateLink<T>(List<AdeLinkEntity> entities) where T : IAdeLinkProperties
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                var type = typeof(T).ToString();
                entities.ForEach(x => x.HostType = type);
                entities.ForEach(x =>
                {
                    if (x.ID <= 0)
                        _adeLinkRpst.Insert(x, false);
                    else
                        _adeLinkRpst.Update(x, false);
                }
                    );
                UnitOfWork.Commit(true);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult DeleteLink<T>(int hostId, int linkType) where T : IAdeLinkProperties
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                var type = typeof(T).ToString();
                _adeLinkRpst.Delete(x => x.HostType == type && x.HostId == hostId && x.LinkType == linkType);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult InsertCutOffScore<T>(List<CutOffScoreEntity> entities) where T : ICutOffScoreProperties
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                var type = typeof(T).ToString();
                entities.ForEach(x => x.HostType = type);
                _cutOffScoreRpst.Insert(entities);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult UpdateCutOffScore<T>(List<CutOffScoreEntity> entities) where T : ICutOffScoreProperties
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                var type = typeof(T).ToString();
                entities.ForEach(x => x.HostType = type);
                entities.ForEach(x =>
                    {
                        if (x.ID <= 0)
                            _cutOffScoreRpst.Insert(x, false);
                        else
                            _cutOffScoreRpst.Update(x, false);
                    }
                    );
                UnitOfWork.Commit(true);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult DeleteCutOffScore<T>(int hostId) where T : ICutOffScoreProperties
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                var type = typeof(T).ToString();
                _cutOffScoreRpst.Delete(x => x.HostType == type && x.HostId == hostId);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }
        #endregion

        #region New Empty Entities
        public AssessmentEntity NewAssessmentEntity()
        {
            return _assessmentRpst.Create();
        }

        public MeasureEntity NewMeasureEntity()
        {
            return _measureRpst.Create();
        }

        public ItemBaseEntity NewItemBaseEntity(ItemType type)
        {
            var target = _itemBaseRpst.Create();
            switch (type)
            {
                case ItemType.Cec:
                    target = new CecItemEntity();
                    break;
                case ItemType.Checklist:
                    target = new ChecklistItemEntity();
                    break;
                case ItemType.Cot:
                    target = new CotItemEntity();
                    break;
                case ItemType.Direction:
                    target = new DirectionItemEntity();
                    break;
                case ItemType.MultipleChoices:
                    target = new MultipleChoicesItemEntity();
                    break;
                case ItemType.Pa:
                    target = new PaItemEntity();
                    break;
                case ItemType.Quadrant:
                    target = new QuadrantItemEntity();
                    break;
                case ItemType.RapidExpressive:
                    target = new RapidExpressiveItemEntity();
                    break;
                case ItemType.Receptive:
                    target = new ReceptiveItemEntity();
                    break;
                case ItemType.ReceptivePrompt:
                    target = new ReceptivePromptItemEntity();
                    break;
                case ItemType.TypedResponse:
                    target = new TypedResponseItemEntity();
                    break;
                case ItemType.ObservableChoice:
                    target = new ObservableChoiceEntity();
                    break;
                case ItemType.ObservableResponse:
                    target = new ObservableEntryEntity();
                    break;
                case ItemType.TxkeaReceptive:
                    target = new TxkeaReceptiveItemEntity();
                    break;
                case ItemType.TxkeaExpressive:
                    target = new TxkeaExpressiveItemEntity();
                    break;
                default:
                    break;
            }
            target.Type = type;
            return target;
        }

        public AnswerEntity NewAnswerEntity()
        {
            return _answerRpst.Create();
        }

        public AdeLinkEntity NewAdeLinkEntity()
        {
            return _adeLinkRpst.Create();
        }

        public CutOffScoreEntity NewCutOffScoreEntity()
        {
            return _cutOffScoreRpst.Create();
        }

        public BenchmarkEntity NewBenchmarkEntity()
        {
            return _benchmarkRpst.Create();
        }
        #endregion

        #region Wave log
        public OperationResult InsertWaveLog(WaveLogEntity entity)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                _wavelogRpst.Insert(entity);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult UpdateWaveLog(WaveLogEntity entity)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                _wavelogRpst.Update(entity);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public WaveLogEntity GetUserWavelog(int userId, int assessmentId)
        {
            return _wavelogRpst.Entities.FirstOrDefault(o => o.UserId == userId && o.AssessmentId == assessmentId);
        }
        #region Common
        public OperationResult ExecuteSql(string strSql)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                result.ResultType = _itemBaseRpst.ExecuteSql(strSql)
                    ? OperationResultType.Success
                    : OperationResultType.Error;
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }
        #endregion

        #region Layout
        public OperationResult InsertLayout(TxkeaLayoutEntity entity)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                _txkeaLayoutRpst.Insert(entity);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }
        public OperationResult UpdateLayout(TxkeaLayoutEntity entity)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                _txkeaLayoutRpst.Update(entity);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }
        public TxkeaLayoutEntity GetLayout(int id)
        {
            return _txkeaLayoutRpst.GetByKey(id);
        }
        #endregion

        #region TxkeaBup
        public TxkeaBupTaskEntity GetTask(int id)
        {
            return _txkeaBupTaskRpst.GetByKey(id);
        }

        public OperationResult InsertTask(TxkeaBupTaskEntity entity)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                _txkeaBupTaskRpst.Insert(entity);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult DeleteTask(TxkeaBupTaskEntity entity)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                entity.IsDeleted = true;
                _txkeaBupTaskRpst.Update(entity);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult UpdateTask(TxkeaBupTaskEntity entity)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                _txkeaBupTaskRpst.Update(entity);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public string ExecuteQuerySql(string strSql)
        {
            return _txkeaBupTaskRpst.ExecuteQuerySql(strSql);
        }


        public OperationResult InsertLogs(List<TxkeaBupLogEntity> entities, bool isSave = true)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                _txkeaBupLogRpst.Insert(entities, isSave);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult DeleteLogs(List<TxkeaBupLogEntity> entities, bool isSave = true)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                _txkeaBupLogRpst.Delete(entities, isSave);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }
        #endregion

        #endregion

        #region Benchmarks

        public OperationResult InsertBenchmark(BenchmarkEntity entity)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                _benchmarkRpst.Insert(entity);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }
        public OperationResult UpdateBenchmark(BenchmarkEntity entity)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                _benchmarkRpst.Update(entity);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult UpdateBenchmarks(List<BenchmarkEntity> entities)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                entities.ForEach(x => _benchmarkRpst.Update(x, false));
                UnitOfWork.Commit(true);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult DeleteBenchmarks(int assessmentId)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                _benchmarkRpst.Delete(x => x.AssessmentId == assessmentId);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult DeleteBenchmarks(List<int> benchmarkIds)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                _benchmarkRpst.Delete(x => benchmarkIds.Contains(x.ID));
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult InsertBenchmarks(List<BenchmarkEntity> entities)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                _benchmarkRpst.Insert(entities, true);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }
        #endregion

        #region Assessment Report
        public OperationResult InsertAssessmentReports(List<AssessmentReportEntity> lists)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                foreach (var assessmentReportEntity in lists)
                {
                    _assessmentReportRpst.Insert(assessmentReportEntity);
                }
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }
        public OperationResult DeleteAssessmentReports(List<AssessmentReportEntity> assessmentReports, bool isSave = true)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            try
            {
                _assessmentReportRpst.Delete(assessmentReports, isSave);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }
        #endregion

        #region Assessment Legend
        public OperationResult InsertAssessmentLegends(List<AssessmentLegendEntity> lists)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                foreach (var assessmentLegendEntity in lists)
                {
                    _assessmentLegendRpst.Insert(assessmentLegendEntity);
                }
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult InsertAssessmentLegend(AssessmentLegendEntity entity)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                _assessmentLegendRpst.Insert(entity);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult DeleteAssessmentLegends(List<AssessmentLegendEntity> assessmentLegends, bool isSave = true)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            try
            {
                _assessmentLegendRpst.Delete(assessmentLegends, isSave);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult UpdateAssessmentLegend(AssessmentLegendEntity entity)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                _assessmentLegendRpst.Update(entity);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }
        #endregion




        #region Scores added by Tony 20170602

        public ScoreEntity NewScoreEntity()
        {
            return _scoreRpst.Create();
        }

        public OperationResult InsertScore(ScoreEntity entity)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                _scoreRpst.Insert(entity);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult UpdateScore(ScoreEntity entity)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                _scoreRpst.Update(entity);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;

        }

        public ScoreEntity GetScore(int id)
        {
            return _scoreRpst.GetByKey(id);
        }



        public OperationResult DeleteScoreAgeOrWaveBands(int scoreId)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                _scoreAgeOrWaveBandsRpst.Delete(x => x.ScoreId == scoreId); ;
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }



        public OperationResult DeleteScoreMeasureOrDefineCoefficients(int scoreId)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                _scoreMeasureOrDefineCoefficientsRpst.Delete(x => x.ScoreId == scoreId); ;
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }



        public OperationResult InsertScoreMeasureOrDefineCoefficients(List<ScoreMeasureOrDefineCoefficientsEntity> lists)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                foreach (var scoreMeasureOrDefineCoefficientsEntity in lists)
                {
                    _scoreMeasureOrDefineCoefficientsRpst.Insert(scoreMeasureOrDefineCoefficientsEntity);
                }
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }



        public OperationResult InsertScoreAgeOrWaveBands(List<ScoreAgeOrWaveBandsEntity> lists)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                foreach (var scoreAgeOrWaveBandsEntity in lists)
                {
                    _scoreAgeOrWaveBandsRpst.Insert(scoreAgeOrWaveBandsEntity);
                }
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }


        #endregion



    }
}
