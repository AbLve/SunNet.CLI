using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		JackZhang
 * Computer:		JACKZ
 * Domain:			Jackz
 * CreatedOn:		2014/12/16 13:59:32
 * Description:		Please input class summary
 * Version History:	Created,2014/12/16 13:59:32
 * 
 * 
 **************************************************************************/
using Sunnet.Cli.Core;
using Sunnet.Cli.Core.Cot.Entities;
using Sunnet.Cli.Core.Cot.Interfaces;
using Sunnet.Cli.Core.Cot.Models;
using Sunnet.Cli.Core.Cpalls.Models.Report;
using Sunnet.Framework.Core.Base;

namespace Sunnet.Cli.Impl.Cot
{
    public class CotAssessmentRpst : EFRepositoryBase<CotAssessmentEntity, int>, ICotAssessmentRpst
    {
        public CotAssessmentRpst(IUnitOfWork unit)
        {
            this.UnitOfWork = unit;
        }

        public List<CotTeacherModel> GetTeachers(int assessmentId, string schoolYear, int? coachId, List<int> communities, List<int> schoolIds, string firstname,
            string lastname, string teacherId, bool searchExistingCot,
            string sort, string order, int first, int count, out int total)
        {
            var strSql = new StringBuilder();
            var strSqlCount = new StringBuilder();
            var joinType = searchExistingCot
                ? " INNER "
                : " LEFT ";
            //if (sort.StartsWith("CotWave", StringComparison.CurrentCultureIgnoreCase))
            //{
            //    sort = " CTEA." + sort;
            //}
            //else
            //{
            //    sort = " TEA." + sort;
            //}

            bool showAllSchool = schoolIds == null;
            schoolIds = schoolIds ?? new List<int>() { };
            if (!schoolIds.Any()) schoolIds.Add(0);

            bool showAllCommunity = communities == null;
            communities = communities ?? new List<int>() { 0 };
            if (!communities.Any()) communities.Add(0);

            var where = new StringBuilder(" TEA.[Status] = 1 ");
            if (searchExistingCot)
            {
                where.Append(" AND CTEA.[AssessmentID] = @AssessmentID ");
                where.Append(" AND CTEA.[SchoolYear] = @SchoolYear ");
            }
            //else
            //{
            //    where.Append(" AND (CTEA.[AssessmentID] = @AssessmentID OR CTEA.[AssessmentID] IS NULL)");
            //}

            if (coachId.HasValue)
                where.AppendFormat(" AND TEA.[CoachId] > 0 AND TEA.[CoachId] = @CoachId ");
            if (!showAllCommunity)
                where.AppendFormat(" AND UCSR.[CommunityId] IN ({0})", string.Join(",", communities));
            if (!showAllSchool)
                where.AppendFormat(" AND UCSR.[SchoolId] IN ({0})", string.Join(",", schoolIds));
            if (!string.IsNullOrEmpty(firstname))
                where.AppendFormat(" AND TEA.[Firstname] LIKE @Firstname");
            if (!string.IsNullOrEmpty(lastname))
                where.AppendFormat(" AND TEA.[Lastname] LIKE @Lastname");
            if (!string.IsNullOrEmpty(teacherId))
                where.AppendFormat(" AND TEA.[TeacherID] LIKE @TeacherID");

            strSql.AppendFormat(@"SELECT TOP {0} * FROM (
                                        SELECT * ,
                                        [Row_Index]    = ROW_NUMBER() OVER(ORDER BY {1} {2}) FROM (
                                        SELECT distinct TEA.* ,
                                        [AssessmentID] = ISNULL(CTEA.AssessmentID,0) ,
                                        [SchoolYear]   = ISNULL(CTEA.SchoolYear,''),
                                        CTEA.CotWave1,
                                        CTEA.CotWave2,
                                        CTEA.CotWaveStatus1,
                                        CTEA.CotWaveStatus2
                                        FROM V_Teachers TEA 
                                        INNER JOIN (SELECT * FROM [dbo].[Cli_Engage__UserComSchRelations]) UCSR ON TEA.UserID = UCSR.UserId 
                                        {3} JOIN [V_Cot_Teachers] CTEA ON TEA.ID = CTEA.ID AND SchoolYear = '{4}'  AND (CTEA.[AssessmentID] = @AssessmentID OR CTEA.[AssessmentID] IS NULL)
                                        WHERE {5}
                                        ) T_ALL
                                        ) DistList
                                        WHERE [Row_Index] > {6}", count, sort, order, joinType, schoolYear, where, first);

            strSqlCount.AppendFormat(@"SELECT Count(*) from (SELECT distinct TEA.* ,
                                        [AssessmentID] = ISNULL(CTEA.AssessmentID,0) ,
                                        [SchoolYear]   = ISNULL(CTEA.SchoolYear,''),
                                        CTEA.CotWave1,
                                        CTEA.CotWave2,
                                        CTEA.CotWaveStatus1,
                                        CTEA.CotWaveStatus2
                                        FROM  V_Teachers TEA 
                                        INNER JOIN (SELECT * FROM    [dbo].[Cli_Engage__UserComSchRelations]) UCSR ON TEA.UserID = UCSR.UserId 
                                        {0} JOIN [V_Cot_Teachers] CTEA ON TEA.ID = CTEA.ID AND SchoolYear = '{1}'  AND (CTEA.[AssessmentID] = @AssessmentID OR CTEA.[AssessmentID] IS NULL)
                                        WHERE {2} ) list", joinType, schoolYear, where);

            var context = this.UnitOfWork as AdeUnitOfWorkContext;
            total = 0;
            var paramses = new SqlParameter[]
            {
                new SqlParameter("AssessmentId", assessmentId),
                new SqlParameter("SchoolYear", schoolYear),
                new SqlParameter("CoachId", coachId.HasValue ? coachId.Value : 0),
                new SqlParameter("Firstname", string.Format("%{0}%", firstname.Trim())),
                new SqlParameter("Lastname", string.Format("%{0}%", lastname.Trim())),
                new SqlParameter("TeacherID", string.Format("%{0}%", teacherId.Trim()))
            };
            try
            {
                context.DbContext.Database.Connection.Open();

                var command = context.DbContext.Database.Connection.CreateCommand();
                command.CommandText = strSqlCount.ToString();
                command.Parameters.AddRange(paramses);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        total = reader.GetInt32(0);
                    }
                }
                return context.DbContext.Database.SqlQuery<CotTeacherModel>(strSql.ToString(),
                    new SqlParameter("AssessmentId", assessmentId),
                    new SqlParameter("SchoolYear", schoolYear),
                    new SqlParameter("CoachId", coachId.HasValue ? coachId.Value : 0),
                    new SqlParameter("Firstname", string.Format("%{0}%", firstname.Trim())),
                    new SqlParameter("Lastname", string.Format("%{0}%", lastname.Trim())),
                    new SqlParameter("TeacherID", string.Format("%{0}%", teacherId.Trim()))
                ).ToList();
            }
            catch (Exception ex)
            {
                return null;
                throw ex;
            }
            finally
            {
                context.DbContext.Database.Connection.Close();
            }
        }
    }
}
