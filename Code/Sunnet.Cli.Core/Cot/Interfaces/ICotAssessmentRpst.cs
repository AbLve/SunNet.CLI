using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		JackZhang
 * Computer:		JACKZ
 * Domain:			Jackz
 * CreatedOn:		2014/12/16 9:26:35
 * Description:		Please input class summary
 * Version History:	Created,2014/12/16 9:26:35
 * 
 * 
 **************************************************************************/
using Sunnet.Cli.Core.Cot.Entities;
using Sunnet.Cli.Core.Cot.Models;
using Sunnet.Framework.Core.Base;

namespace Sunnet.Cli.Core.Cot.Interfaces
{
    public interface ICotAssessmentRpst : IRepository<CotAssessmentEntity, int>
    {
        List<CotTeacherModel> GetTeachers(int assessmentId, string schoolYear,
            int? coachId, List<int> communities, List<int> schoolIds, string firstname,
            string lastname, string teacherId, bool searchExistingCot,
            string sort,string order,int first,int count,out int total);
    }
}
