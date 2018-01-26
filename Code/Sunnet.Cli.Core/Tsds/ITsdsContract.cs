using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sunnet.Cli.Core.Tsds.Entities;
using Sunnet.Framework.Core.Tool;

namespace Sunnet.Cli.Core.Tsds
{
    public interface ITsdsContract
    {
        #region TsdsAssessmentFiles

        IQueryable<TsdsAssessmentFileEntity> TsdsAssessmentFiles { get; }

        OperationResult InsertTsdsAssessmentFile(TsdsAssessmentFileEntity entity, bool isSave = true);

        OperationResult DeleteTsdsAssessmentFile(int id);

        OperationResult UpdateTsdsAssessmentFile(TsdsAssessmentFileEntity entity, bool isSave = true);

        TsdsAssessmentFileEntity GetTsdsAssessmentFile(int id);

        #endregion

        #region TSDS
        IQueryable<TsdsEntity> Tsdses { get; }
        IQueryable<TsdsMapEntity> TsdsMaps { get; }

        OperationResult InsertTsds(List<TsdsEntity> entityList, bool isSave = true);
        OperationResult InsertTsds(TsdsEntity entity, bool isSave = true);
        OperationResult UpdateTsds(TsdsEntity entity, bool isSave = true);
        TsdsEntity GetTsds(int id);
        #endregion
    }
}
