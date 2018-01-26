using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
using Sunnet.Cli.Core;
using Sunnet.Cli.Core.Reports.Models;
using Sunnet.Framework.Core.Base;
using Sunnet.Framework.Extensions;
using Sunnet.Cli.Core.Cpalls.Models.Report;
using Sunnet.Cli.Core.Ade;
using Sunnet.Cli.Core.Cpalls;
using Sunnet.Cli.Core.Practices.Entities;
using Sunnet.Cli.Core.Practices.Interfaces;
using Sunnet.Cli.Core.Practices.Entites;

namespace Sunnet.Cli.Impl.Practices
{
    public class PracticeMeasureGroupRpst : EFRepositoryBase<PracticeMeasureGroupEntity, int>, IPracticeMeasureGroupRpst
    {
        public PracticeMeasureGroupRpst(IUnitOfWork unit)
        {
            this.UnitOfWork = unit;
        }
        public int InitMeasures(int userId, int assessmentId, string schoolYear, int studentId,
           DateTime studentBirthday, Wave wave, IEnumerable<int> measureIds)
        {
            string strSql = @"EXEC  [InitMeasures]  @UserId,@AssessmentId,@SchoolYear, @StudentId, 
                            @StudentBirthday, @Wave, @MeasureIds, @StudentAssessmentId OUTPUT";

            PracticeUnitOfWorkContext context = this.UnitOfWork as PracticeUnitOfWorkContext;
            var stuAssParam = new SqlParameter("@StudentAssessmentId", 0)
            {
                Direction = ParameterDirection.Output
            };
            var result = context.DbContext.Database.ExecuteSqlCommand(strSql,
                new SqlParameter("UserId", userId),
                new SqlParameter("AssessmentId", assessmentId),
                new SqlParameter("SchoolYear", schoolYear),
                new SqlParameter("StudentId", studentId),
                new SqlParameter("StudentBirthday", studentBirthday),
                new SqlParameter("Wave", (byte)wave),
                new SqlParameter("MeasureIds", string.Join(",", measureIds)),
                stuAssParam);
            var stuAssId = stuAssParam.Value.CastTo<int>();
            return stuAssId;
        }
        public void RecalculateParentGoal(int saId)
        {
            string strSql = @"EXEC  [Recalculate_Parent_Goal_SA] @SaId";

            AdeUnitOfWorkContext context = this.UnitOfWork as AdeUnitOfWorkContext;
            var result = context.DbContext.Database.ExecuteSqlCommand(strSql,
                new SqlParameter("SaId", saId));
        }
    }
}