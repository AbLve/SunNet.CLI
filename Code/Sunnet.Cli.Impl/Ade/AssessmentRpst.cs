using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sunnet.Cli.Core;
using Sunnet.Cli.Core.Ade.Entities;
using Sunnet.Cli.Core.Ade.Interfaces;
using Sunnet.Framework.Core.Base;

/**************************************************************************
 * Developer: 		Jack
 * Computer:		Jackz
 * Domain:			Jackz
 * CreatedOn:		08/11/2014 03:52:57
 * Description:		AssessmentEntity's IRepository
 * Version History:	Created,08/11/2014 03:52:57
 * 
 * 
 **************************************************************************/
namespace Sunnet.Cli.Impl.Ade
{
    /// <summary>
    /// AssessmentEntity's Repository
    /// </summary>
    public class AssessmentRpst : EFRepositoryBase<AssessmentEntity, int>, IAssessmentRpst
    {
        public AssessmentRpst(IUnitOfWork unit)
        {
            this.UnitOfWork = unit;
        }

        public int Unlock(int id)
        {
            string strSql = @" 
                declare @SAId table([ID] int);
                insert into  @SAId
                select ID from dbo.StudentAssessments 
                where [AssessmentId] = @ID order by id desc ;

                delete from [dbo].[StudentItems] 
                where [SMId] in 
                (select id from [dbo].[StudentMeasures] where SAId in (select ID from @SAId)) ;

                delete from [dbo].[StudentMeasures] where [SAId] in (select ID from @SAId) ;

                delete FROM [dbo].[StudentAssessments] where [AssessmentId] = @ID ;

                update Assessments set Locked = 0,UpdatedOn = GETDATE() where ID = @ID";
            //不能在此Dispose掉context，因为执行过此方法后，还会用到context
            AdeUnitOfWorkContext context = this.UnitOfWork as AdeUnitOfWorkContext;
            return context.DbContext.Database.ExecuteSqlCommand(strSql, new[] { new SqlParameter("ID", id) });
        }
    }
}
