using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		JackZhang
 * Computer:		JACKZ
 * Domain:			Jackz
 * CreatedOn:		2014/10/27 20:56:30
 * Description:		Please input class summary
 * Version History:	Created,2014/10/27 20:56:30
 * 
 * 
 **************************************************************************/
using Sunnet.Cli.Core.Ade.Entities;
using Sunnet.Cli.Core.Ade.Interfaces;
using Sunnet.Framework.Core.Base;
using Sunnet.Framework.EmailSender;
using Sunnet.Framework.Encrypt;
using Sunnet.Framework.File;
using Sunnet.Framework.Log;

namespace Sunnet.Cli.Core.Ade
{
    internal class AdeDataService : CoreServiceBase, IAdeDataContract
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
        private readonly IBenchmarkRpst _benchmarkRpst;
        private readonly IScoreRpst _scoreRpst;

        private readonly IScoreAgeOrWaveBandsRpst _scoreAgeOrWaveBandsRpst;
        private readonly IScoreMeasureOrDefineCoefficientsRpst _scoreMeasureOrDefineCoefficientsRpst;

        public AdeDataService(ISunnetLog log, IFile fileHelper, IEmailSender emailSender, IEncrypt encrypt,
            IAdeLinkRpst adeLinkRpst,
            IAnswerRpst answerRpst,
            IAssessmentRpst assessmentRpst,
            ICutOffScoreRpst cutOffScoreRpst,
            IItemBaseRpst itemBaseRpst,
            IMeasureRpst measureRpst,
            ITxkeaLayoutRpst txkeaLayoutRpst,
            ITxkeaBupTaskRpst txkeaBupTaskRpst,
            ITxkeaBupLogRpst txkeaBupLogRpst,
            IBenchmarkRpst benchmarkRpst,
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
            _benchmarkRpst = benchmarkRpst;
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

        public IQueryable<ScoreEntity> Scores
        {
            get
            {
                return _scoreRpst.Entities;
            }
        }

        public IQueryable<ScoreAgeOrWaveBandsEntity> ScoreAgeOrWaveBands
        {
            get
            {
                return _scoreAgeOrWaveBandsRpst.Entities;
            }
        }

        public IQueryable<ScoreMeasureOrDefineCoefficientsEntity> ScoreMeasureOrDefineCoefficients
        {
            get
            {
                return _scoreMeasureOrDefineCoefficientsRpst.Entities;
            }
        }

        #endregion

    }
}
