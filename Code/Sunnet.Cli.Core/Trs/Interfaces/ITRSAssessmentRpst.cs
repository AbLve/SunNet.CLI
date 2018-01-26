using Sunnet.Cli.Core.Trs.Entities;
using Sunnet.Framework.Core.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Cli.Core.Trs.Interfaces
{
    public interface ITRSAssessmentRpst : IRepository<TRSAssessmentEntity, int>
    {
        int DeleteOfflineAssessment(List<int> assessmentIds);

        int UpdateTrsAssessmentStar(int trsAssessmentId, byte newStar);
    }
}
