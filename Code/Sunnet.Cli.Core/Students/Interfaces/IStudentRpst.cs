using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		Damon
 * Computer:		Damon-PC
 * Domain:			Damon-pc
 * CreatedOn:		2014/8/23 17:36:23
 * Description:		Please input class summary
 * Version History:	Created,2014/8/23 17:36:23
 **************************************************************************/
using Sunnet.Cli.Core.Ade;
using Sunnet.Cli.Core.Reports.Models;
using Sunnet.Cli.Core.Students.Entities;
using Sunnet.Cli.Core.Students.Model;
using Sunnet.Framework.Core.Base;
using Sunnet.Cli.Core.Reports.Entities;

namespace Sunnet.Cli.Core.Students.Interfaces
{
    public interface IStudentRpst : IRepository<StudentEntity, Int32>
    {
        #region Media Consent Report
        List<MediaConsentPercentModel> GetMediaConsentPercent(List<int> communityIds, List<int> schoolIds, string teacher);
        List<MediaConsentDetailModel> GetMediaConsentDetail(List<int> communityIds, List<int> schoolIds, string teacher);
        #endregion
        #region BEECH Reports
        List<BeechReportModel> GetBeechReport(List<int> communityIds, List<int> schoolIds, string teacher);
        #endregion

        List<StudentForCpallsModel> GetCpallsStudentModel(List<int> ids, string sort, string order);
        StudentForCpallsModel GetStudentForPlayMeasure(int id);
    }
}
