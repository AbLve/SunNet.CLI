using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		JackZhang
 * Computer:		JACKZ
 * Domain:			Jackz
 * CreatedOn:		2014/12/16 14:01:34
 * Description:		Please input class summary
 * Version History:	Created,2014/12/16 14:01:34
 * 
 * 
 **************************************************************************/
using Sunnet.Cli.Core.Cot.Entities;
using Sunnet.Cli.Core.Cot.Interfaces;
using Sunnet.Framework.Core.Base;
using Sunnet.Cli.Core.Cot.Models;
using Sunnet.Cli.Core;
using System.Data.SqlClient;

namespace Sunnet.Cli.Impl.Cot
{
    public class CotWaveRpst : EFRepositoryBase<CotWaveEntity, int>, ICotWaveRpst
    {
        public CotWaveRpst(IUnitOfWork unit)
        {
            this.UnitOfWork = unit;
        }

        public List<COTCECCompletionModel> GetCOTCECCompletion(int cotAssessmentId,int cecAssessmentId,string schoolYear)
        {
            try
            {
                AdeUnitOfWorkContext context = this.UnitOfWork as AdeUnitOfWorkContext;
                return context.DbContext.Database.SqlQuery<COTCECCompletionModel>(string.Format("exec COTCECCompletionReport {0} , {1} ,'{2}'"
                    , cotAssessmentId, cecAssessmentId, schoolYear
                    ))
                    .ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public List<TeacherMissingCOTModel> GetTeacherMissingMOYCOT(int cotAssessmentId, string schoolYear)
        {
            try
            {
                AdeUnitOfWorkContext context = this.UnitOfWork as AdeUnitOfWorkContext;
                return context.DbContext.Database.SqlQuery<TeacherMissingCOTModel>(string.Format("exec TeacherMissingMOYCOT {0},'{1}'",cotAssessmentId,schoolYear)).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
