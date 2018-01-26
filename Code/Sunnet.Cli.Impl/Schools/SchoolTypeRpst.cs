using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		Sam
 * Computer:		SAM-PC
 * Domain:			sam-pc
 * CreatedOn:		2014/8/19 12:15:10
 * Description:		Please input class summary
 * Version History:	Created,2014/8/19 12:15:10
 * 
 * 
 **************************************************************************/
using Sunnet.Cli.Core;
using Sunnet.Cli.Core.Reports.Entities;
using Sunnet.Cli.Core.Reports.Models;
using Sunnet.Cli.Core.Schools.Entities;
using Sunnet.Cli.Core.Schools.Interfaces;
using Sunnet.Framework;
using Sunnet.Framework.Core.Base;

namespace Sunnet.Cli.Impl.Schools
{
    public class SchoolTypeRpst : EFRepositoryBase<SchoolTypeEntity, Int32>, ISchoolTypeRpst
    {
        public SchoolTypeRpst(IUnitOfWork unit)
        {
            this.UnitOfWork = unit;
        }

        #region Querterly-like reports

        public List<QuarterlyReportModel> GetYearsInProjectCountBySchoolType(List<int> communityIds, List<int> fundingList,
            DateTime? startDate, DateTime? endDate, int status)
        {
            List<int> excludeCommunityIds = SFConfig.ExcludedCommunityForReport;
            if (communityIds.Any(e => excludeCommunityIds.Contains(e)))
            {
                excludeCommunityIds = new List<int>();
            }
            string sql = string.Format("exec CountYearsInProject '{0}','{1}','{2}','{3}','{4}',{5}",
                string.Join(",", communityIds),
                string.Join(",", excludeCommunityIds),
                string.Join(",", fundingList),
                startDate.Value.ToString("MM/dd/yyyy"),
                endDate.Value.ToString("MM/dd/yyyy"), status);

            EFUnitOfWorkContext context = this.UnitOfWork as EFUnitOfWorkContext;
            List<QuarterlyReportModel> listQuarterlyReport =
                context.DbContext.Database.SqlQuery<QuarterlyReportModel>(sql).ToList();
            return listQuarterlyReport;
        }

        #endregion
    }
}
