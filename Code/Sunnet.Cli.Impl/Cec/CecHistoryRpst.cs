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
 * Description:		CecHistoryRpst
 * Version History:	Created,2014/12/1 11:20:00
 * 
 * 
 **************************************************************************/
using Sunnet.Cli.Core.Cec.Entities;
using Sunnet.Cli.Core.Cec.Interfaces;
using Sunnet.Framework.Core.Base;
using Sunnet.Cli.Core.Cpalls;
using Sunnet.Cli.Core;
using System.Data.SqlClient;
using Sunnet.Cli.Core.Cec.Models;
using Sunnet.Framework.Helpers;

namespace Sunnet.Cli.Impl.Cec
{
    public class CecHistoryRpst : EFRepositoryBase<CecHistoryEntity, Int32>, ICecHistoryRpst
    {
        public CecHistoryRpst(IUnitOfWork unit)
        {
            UnitOfWork = unit;
        }

        public void Result(int assessmentId, Wave wave, string schoolYear, int teacherId)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendFormat("delete from CecResults where CecHistoryId in (select id from CecHistories where AssessmentId ={0} and wave ={1} and schoolyear =@schoolyear and teacherid ={2} );"
                    , assessmentId, (byte)wave, teacherId)
                    .AppendFormat("delete from CecHistories where AssessmentId ={0} and wave ={1} and schoolyear =@schoolyear and teacherid ={2} ;"
                     , assessmentId, (byte)wave, teacherId);

                AdeUnitOfWorkContext context = this.UnitOfWork as AdeUnitOfWorkContext;
                context.DbContext.Database.ExecuteSqlCommand(sb.ToString(), new SqlParameter("schoolyear", schoolYear));
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public List<CECTeacherModel> GetCECTeacherList(int assessmentId, string schoolYear,
            int? coachId, List<int> communities, List<int> schoolIds, string firstname,
            string lastname, string teacherId,
            string sort, string order, int first, int count, out int total)
        {
            var strSql = new StringBuilder();
            var strSqlCount = new StringBuilder();

            bool showAllSchool = schoolIds == null;
            schoolIds = schoolIds ?? new List<int>() { };
            if (!schoolIds.Any()) schoolIds.Add(0);

            bool showAllCommunity = communities == null;
            communities = communities ?? new List<int>() { 0 };
            if (!communities.Any()) communities.Add(0);

            var where = new StringBuilder(" TEA.[Status] = 1 ");
            
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
                                        SELECT * ,[Row_Index]    = ROW_NUMBER() OVER(ORDER BY {1} {2}) 
                                        FROM (
                                        SELECT distinct TEA.* 
                                        FROM	(SELECT TTEA.*,
                                                    SchoolYear = @SchoolYear,
			                                        BOY = ISNULL(CONVERT(VARCHAR,(SELECT TOP 1 AssessmentDate FROM CecHistories CB 
					                                        WHERE CB.TeacherId = TTEA.ID AND CB.AssessmentId = @AssessmentId 
                                                            AND CB.Wave = 1 AND CB.SchoolYear = @SchoolYear ),101),''),
			                                        MOY = ISNULL(CONVERT(VARCHAR,(SELECT TOP 1 AssessmentDate FROM CecHistories CB 
					                                        WHERE CB.TeacherId = TTEA.ID AND CB.AssessmentId = @AssessmentId 
                                                             AND CB.Wave = 2 AND CB.SchoolYear = @SchoolYear),101),''),
			                                        EOY = ISNULL(CONVERT(VARCHAR,(SELECT TOP 1 AssessmentDate FROM CecHistories CB 
					                                        WHERE CB.TeacherId = TTEA.ID AND CB.AssessmentId = @AssessmentId 
                                                            AND CB.Wave = 3 AND CB.SchoolYear = @SchoolYear ),101),'') 
			                                        FROM V_Teachers TTEA 
					                                        ) TEA
                                        INNER JOIN (SELECT * FROM [dbo].[Cli_Engage__UserComSchRelations]) UCSR ON TEA.UserID = UCSR.UserId 
                                        WHERE {3}
                                        ) T_ALL
                                        ) list
                                        WHERE [Row_Index] > {4}", count, sort, order, where, first);

            strSqlCount.AppendFormat(@"SELECT Count(1) FROM (SELECT distinct TEA.*  FROM  V_Teachers TEA 
                                        INNER JOIN (SELECT * FROM    [dbo].[Cli_Engage__UserComSchRelations]) UCSR ON TEA.UserID = UCSR.UserId 
                                        WHERE {0} ) list", where);

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
                return context.DbContext.Database.SqlQuery<CECTeacherModel>(strSql.ToString(),
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
                throw ex;
            }
            finally
            {
                context.DbContext.Database.Connection.Close();
            }
            return null;
        }

        public List<TeacherMissingCECModel> GetTeacherMissingCEC(int cecAssessmentId, string schoolYear, Wave wave)
        {
            try
            {
                AdeUnitOfWorkContext context = this.UnitOfWork as AdeUnitOfWorkContext;
                return context.DbContext.Database.SqlQuery<TeacherMissingCECModel>(string.Format("exec TeacherMissingCEC {0} ,'{1}',{2}",
                    cecAssessmentId, schoolYear, (int)wave)).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// EOY CEC Assessment Completion Report
        /// </summary>
        /// <param name="cecAssessmentId"></param>
        /// <param name="schoolYear"></param>
        /// <returns></returns>
        public List<CECCompletionModel> GetEOYCECCompletion(int cecAssessmentId, string schoolYear)
        {
            try
            {
                AdeUnitOfWorkContext context = this.UnitOfWork as AdeUnitOfWorkContext;
                return context.DbContext.Database.SqlQuery<CECCompletionModel>(string.Format("exec EOYCECCompletion {0},'{1}'",
                    cecAssessmentId,schoolYear)).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
