using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		Sam
 * Computer:		SAM-PC
 * Domain:			sam-pc
 * CreatedOn:		2014/8/19 9:02:02
 * Description:		Please input class summary
 * Version History:	Created,2014/8/19 9:02:02
 * 
 * 
 **************************************************************************/
using Sunnet.Cli.Core.Reports.Entities;
using Sunnet.Cli.Core.Reports.Models;
using Sunnet.Cli.Core.Schools.Entities;
using Sunnet.Framework.Core.Base;

namespace Sunnet.Cli.Core.Schools.Interfaces
{
    public interface ISchoolTypeRpst : IRepository<SchoolTypeEntity, Int32>
    {
        #region Quarterly-like reports
        List<QuarterlyReportModel> GetYearsInProjectCountBySchoolType(List<int> communityIds, List<int> fundingList,
            DateTime? startDate, DateTime? endDate, int status);
        #endregion
    }
}
