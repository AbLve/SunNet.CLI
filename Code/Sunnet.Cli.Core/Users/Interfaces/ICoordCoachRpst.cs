using Sunnet.Cli.Core.Reports.Entities;
using Sunnet.Cli.Core.Reports.Models;
using Sunnet.Cli.Core.Users.Entities;
using Sunnet.Framework.Core.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Cli.Core.Users.Interfaces
{
    public interface ICoordCoachRpst : IRepository<CoordCoachEntity, Int32>
    {
        List<CoachReportModel> GetCoachReport(List<int> communityIds, int mentorCoach, string funding, int status);
    }
}
