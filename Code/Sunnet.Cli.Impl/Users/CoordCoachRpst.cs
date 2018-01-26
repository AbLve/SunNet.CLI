using Sunnet.Cli.Core;
using Sunnet.Cli.Core.Reports.Entities;
using Sunnet.Cli.Core.Reports.Models;
using Sunnet.Cli.Core.Users.Entities;
using Sunnet.Cli.Core.Users.Interfaces;
using Sunnet.Framework;
using Sunnet.Framework.Core.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Cli.Impl.Users
{
    public class CoordCoachRpst : EFRepositoryBase<CoordCoachEntity, Int32>, ICoordCoachRpst
    {
        public CoordCoachRpst(IUnitOfWork unit)
        {
            UnitOfWork = unit;
        }

        public List<CoachReportModel> GetCoachReport(List<int> communityIds, int mentorCoach, string funding, int status)
        {
            List<int> excludeCommunityIds = SFConfig.ExcludedCommunityForReport;
            if (communityIds.Any(e => excludeCommunityIds.Contains(e)))
            {
                excludeCommunityIds = new List<int>();
            }
            string strSql = "exec CoachReport '" + string.Join(",", communityIds) + "','" +
                            string.Join(",", excludeCommunityIds) + "'," + mentorCoach + ",'" + funding +
                            "'," + status;

            EFUnitOfWorkContext context = this.UnitOfWork as EFUnitOfWorkContext;
            List<CoachReportModel> listCoachReport =
                context.DbContext.Database.SqlQuery<CoachReportModel>(strSql).ToList();
            return listCoachReport;
        }
    }
}
