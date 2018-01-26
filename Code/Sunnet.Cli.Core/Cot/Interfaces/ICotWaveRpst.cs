using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		JackZhang
 * Computer:		JACKZ
 * Domain:			Jackz
 * CreatedOn:		2014/12/16 9:30:43
 * Description:		Please input class summary
 * Version History:	Created,2014/12/16 9:30:43
 * 
 * 
 **************************************************************************/
using Sunnet.Cli.Core.Cot.Entities;
using Sunnet.Framework.Core.Base;
using Sunnet.Cli.Core.Cot.Models;

namespace Sunnet.Cli.Core.Cot.Interfaces
{
    public interface ICotWaveRpst : IRepository<CotWaveEntity, int>
    {
        /// <summary>
        /// BOY & MOY - COT & CEC Assessment Completion Report
        /// </summary>
        /// <param name="cotAssessmentId"></param>
        /// <param name="cecAssessmentId"></param>
        /// <param name="schoolYear"></param>
        /// <returns></returns>
        List<COTCECCompletionModel> GetCOTCECCompletion(int cotAssessmentId, int cecAssessmentId, string schoolYear);

        /// <summary>
        /// List of teachers with missing COT: 
        /// </summary>
        List<TeacherMissingCOTModel> GetTeacherMissingMOYCOT(int cotAssessmentId, string schoolYear);
    }
}
