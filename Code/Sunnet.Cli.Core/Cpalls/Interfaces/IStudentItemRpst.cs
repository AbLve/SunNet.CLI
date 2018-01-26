using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sunnet.Cli.Core.Ade;
using Sunnet.Cli.Core.Cpalls.Entities;
using Sunnet.Cli.Core.Reports.Models;
using Sunnet.Framework.Core.Base;

/**************************************************************************
 * Developer: 		JackZhang
 * Computer:		JACKZ
 * Domain:			Jackz
 * CreatedOn:		2014/9/4 4:22:23
 * Description:		Please input class summary
 * Version History:	Created,2014/9/4 4:22:23
 * 
 * 
 **************************************************************************/


namespace Sunnet.Cli.Core.Cpalls.Interfaces
{
    public interface IStudentItemRpst : IRepository<StudentItemEntity, int>
    {

        List<CircleDataExportStudentItemModel> GetCircleDataExportStudentItemModels(int communityId, string year,
            int schoolId, List<int> waves, List<int> measures, List<ItemType> types);
    }
}
