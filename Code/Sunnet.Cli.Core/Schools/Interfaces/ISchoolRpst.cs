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
    public interface ISchoolRpst : IRepository<SchoolEntity, Int32>
    {
        List<SchoolSelectItemEntity> GetSchoolNameList(int communityId, string keyword);

        List<SchoolSelectItemEntity> GetSchoolListByKey(int communityId, string keyword);

        List<ServiceReportModel> GetServiceReport(List<int> communityIds, List<int> schoolIds);
    }
}
