using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		Leason
 * Computer:		Leason-PC
 * Domain:			Leason
 * CreatedOn:		2014/12/1 11:20:00
 * Description:		ICecHistoryRpst
 * Version History:	Created,2014/12/1 11:20:00
 * 
 * 
 **************************************************************************/
using Sunnet.Cli.Core.Cec.Entities;
using Sunnet.Framework.Core.Base;
using Sunnet.Cli.Core.Cpalls;
using Sunnet.Cli.Core.Cec.Models;

namespace Sunnet.Cli.Core.Cec.Interfaces
{
    public interface ICecHistoryRpst : IRepository<CecHistoryEntity, Int32>
    {
        void Result(int assessmentId, Wave wave, string schoolYear, int teacherId);

        List<CECTeacherModel> GetCECTeacherList(int assessmentId, string schoolYear,
            int? coachId, List<int> communities, List<int> schoolIds, string firstname,
            string lastname, string teacherId, 
            string sort, string order, int first, int count, out int total);

        /// <summary>
        /// List of teachers with missing CEC: 
        /// </summary>
        /// <param name="cecAssessmentId"></param>
        /// <param name="schoolYear"></param>
        /// <param name="wave"></param>
        /// <returns></returns>
        List<TeacherMissingCECModel> GetTeacherMissingCEC(int cecAssessmentId, string schoolYear, Wave wave);

        /// <summary>
        /// EOY CEC Assessment Completion Report
        /// </summary>
        /// <param name="cecAssessmentId"></param>
        /// <param name="schoolYear"></param>
        /// <returns></returns>
        List<CECCompletionModel> GetEOYCECCompletion(int cecAssessmentId, string schoolYear);
    }
}
