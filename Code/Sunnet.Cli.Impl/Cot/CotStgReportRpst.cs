using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		JackZhang
 * Computer:		JACKZ
 * Domain:			Jackz
 * CreatedOn:		2014/12/16 14:00:33
 * Description:		Please input class summary
 * Version History:	Created,2014/12/16 14:00:33
 * 
 * 
 **************************************************************************/
using Sunnet.Cli.Core.Cot.Entities;
using Sunnet.Cli.Core.Cot.Interfaces;
using Sunnet.Framework.Core.Base;
using Sunnet.Cli.Core.Cot.Models;
using Sunnet.Cli.Core;

namespace Sunnet.Cli.Impl.Cot
{
    public class CotStgReportRpst : EFRepositoryBase<CotStgReportEntity, int>, ICotStgReportRpst
    {
        public CotStgReportRpst(IUnitOfWork unit)
        {
            this.UnitOfWork = unit;
        }

        public List<STGR_CompletionModel> GetSTGR_Completion_Report(int assessmentId)
        {
            try
            {
                AdeUnitOfWorkContext context = this.UnitOfWork as AdeUnitOfWorkContext;
                return context.DbContext.Database.SqlQuery<STGR_CompletionModel>(string.Format("exec STGR_Completion_Report {0}", assessmentId)).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public List<TeacherMissingSTGRModel> GetTeacher_Missing_STGR(int assessmentId)
        {
            try
            {
                AdeUnitOfWorkContext context = this.UnitOfWork as AdeUnitOfWorkContext;
                return context.DbContext.Database.SqlQuery<TeacherMissingSTGRModel>(string.Format( "exec Teacher_Missing_STGR {0}",assessmentId)).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
