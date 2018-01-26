using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		JackZhang
 * Computer:		JACKZ
 * Domain:			Jackz
 * CreatedOn:		2014/12/16 9:30:09
 * Description:		Please input class summary
 * Version History:	Created,2014/12/16 9:30:09
 * 
 * 
 **************************************************************************/
using Sunnet.Cli.Core.Cot.Entities;
using Sunnet.Framework.Core.Base;
using Sunnet.Cli.Core.Cot.Models;

namespace Sunnet.Cli.Core.Cot.Interfaces
{
    public interface ICotStgReportRpst : IRepository<CotStgReportEntity, int>
    {
        List<STGR_CompletionModel> GetSTGR_Completion_Report(int assessmentId);

        List<TeacherMissingSTGRModel> GetTeacher_Missing_STGR(int assessmentId);
    }
}
