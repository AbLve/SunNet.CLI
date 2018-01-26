using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		JackZhang
 * Computer:		JACKZ
 * Domain:			Jackz
 * CreatedOn:		2014/10/27 20:55:32
 * Description:		Please input class summary
 * Version History:	Created,2014/10/27 20:55:32
 * 
 * 
 **************************************************************************/
using Sunnet.Cli.Core.Ade.Entities;

namespace Sunnet.Cli.Core.Ade
{
    public interface IAdeDataContract
    {
        #region IQueryables

        IQueryable<AdeLinkEntity> AdeLinks { get; }
        IQueryable<AnswerEntity> Answers { get; }
        IQueryable<AssessmentEntity> Assessments { get; }
        IQueryable<CutOffScoreEntity> CutOffScores { get; }
        IQueryable<ItemBaseEntity> Items { get; }
        IQueryable<MeasureEntity> Measures { get; }
        IQueryable<TxkeaLayoutEntity> TxkeaLayouts { get; }
        IQueryable<TxkeaBupTaskEntity> TxkeaBupTasks { get; }
        IQueryable<TxkeaBupLogEntity> TxkeaBupLogs { get; }
        IQueryable<BenchmarkEntity> Benchmarks { get; }

        /// <summary>
        /// added by tony
        /// </summary>
        IQueryable<ScoreEntity> Scores { get; }
        IQueryable<ScoreAgeOrWaveBandsEntity> ScoreAgeOrWaveBands { get; }

        IQueryable<ScoreMeasureOrDefineCoefficientsEntity> ScoreMeasureOrDefineCoefficients { get; }

        #endregion
    }
}
