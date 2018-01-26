using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sunnet.Cli.Core.Cpalls.Entities;
using Sunnet.Cli.Core.Cpalls.Models;
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
    //public interface IAdeLinkRpst : IRepository<AdeLinkEntity, int>
    public interface IStudentAssessmentRpst : IRepository<StudentAssessmentEntity, int>
    {
        List<SchoolMeasureGoalModel> GetSchoolMeasureGoal(List<int> schoolId, string schoolYear, Wave wave, int assessmentId,IList<int> classIds );
        List<StudentMeasureGoalModel> GetStudentMeasureGoal(List<int> studentIds, string schoolYear, Wave wave, int assessmentId);
        int GetStudentAssessmentIdForPlayMeasure(int assessmentId, int studentId, string schoolYear, int wave);
    }
}
