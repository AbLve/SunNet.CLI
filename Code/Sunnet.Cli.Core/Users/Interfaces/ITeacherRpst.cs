using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		Sam
 * Computer:		SAM-PC
 * Domain:			sam-pc
 * CreatedOn:		2014/8/5 16:22:44
 * Description:		Please input class summary
 * Version History:	Created,2014/8/5 16:22:44
 * 
 * 
 **************************************************************************/
using Sunnet.Cli.Core.Users.Entities;
using Sunnet.Framework.Core.Base;
using Sunnet.Cli.Core.Reports.Models;

namespace Sunnet.Cli.Core.Users.Interfaces
{
    public interface ITeacherRpst : IRepository<TeacherEntity, Int32>
    {
        List<Community_Mentor_TeacherModel> GetCommunity_Mentor_Teachers();
        List<PDReportModel> PDCompletionCourseReport(List<int> communityIds, List<int> schoolIds, string teacher, int status);
    }

}
