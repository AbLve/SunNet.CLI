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
 * CreatedOn:		2014/9/4 4:21:56
 * Description:		Please input class summary
 * Version History:	Created,2014/9/4 4:21:56
 * 
 * 
 **************************************************************************/
using Sunnet.Cli.Core.Cpalls;
using Sunnet.Cli.Core.Cpalls.Entities;
using Sunnet.Cli.Core.Cpalls.Interfaces;
using Sunnet.Cli.Core.Cpalls.Models;
using Sunnet.Framework.Core.Base;
using Sunnet.Cli.Core;

namespace Sunnet.Cli.Impl.Cpalls
{
    public class StudentAssessmentRpst : EFRepositoryBase<StudentAssessmentEntity, int>, IStudentAssessmentRpst
    {
        public StudentAssessmentRpst(IUnitOfWork unit)
        {
            this.UnitOfWork = unit;
        }

        public List<SchoolMeasureGoalModel> GetSchoolMeasureGoal(List<int> schoolId, string schoolYear, Wave wave, int assessmentId,IList<int> classIds )
        {
            if (schoolId == null || schoolId.Count == 0 || classIds.Count==0)
                return new List<SchoolMeasureGoalModel>();
            /*
             * DEBUG PARAMS
             
                DECLARE @FinishedStatus TINYINT;
                SET @FinishedStatus = 3;
                DECLARE @SchoolYear NVARCHAR(10);
                SET @SchoolYear = '14-15';
                DECLARE @Wave TINYINT;
                SET @Wave = 3;
                DECLARE @AssessmentId INT;
                SET @AssessmentId = 17
             */
            string strSql = string.Format(@"
                DECLARE @Students TABLE(StudentId INT,SchoolId INT);
INSERT INTO @Students 
SELECT [StudentId] = STU.ID ,SSTU.SchoolId
FROM [dbo].[Cli_Engage__Schools] SCH 
INNER JOIN [Cli_Engage__SchoolStudentRelations] SSTU ON SCH.ID = SSTU.SchoolId AND SSTU.[Status] = 1 
INNER JOIN [dbo].[Cli_Engage__Students] STU ON SSTU.StudentId = STU.ID AND STU.[Status] = 1
   Inner Join [dbo].[Cli_Engage__StudentClassRelations] SCR ON SCR.StudentId =STU.ID 
WHERE SCH.ID IN ({0})  AND SCR.ClassId IN ({1})

SELECT SchoolId, MeasureId,Goal = SUM(Goal),Amount = SUM(Amount),TotalScored FROM (
	SELECT STU.SchoolId, MeasureId, Goal = (Case Goal when -1 then 0 else Goal end),
	Amount = CASE (SM.[Status]) When @FinishedStatus THEN 1 ELSE 0 END,
	M.TotalScored
	FROM V_StudentMeasures SM LEFT JOIN Measures M ON SM.MeasureId = M.ID
	INNER JOIN StudentAssessments SA on SM.SAId = SA.ID  
	FULL JOIN @Students STU ON SA.StudentId = STU.StudentId
	WHERE 
	EXISTS(SELECT 1 FROM @Students STU WHERE STU.StudentId = SA.StudentId) 
	AND SchoolYear = @SchoolYear AND Wave = @Wave AND SA.AssessmentId = @AssessmentId 
    AND (SM.Status = @FinishedStatus OR Exists (Select 1 from Measures M2 where M2.ParentId = SM.MeasureID AND M2.ParentId>0))
) StuGoals GROUP BY SchoolId, MeasureId,TotalScored 
                 ", string.Join(",", schoolId.ToArray()), string.Join(",", classIds.ToArray()));

            //不能在此Dispose掉context，因为执行过此方法后，还会用到context
            AdeUnitOfWorkContext context = this.UnitOfWork as AdeUnitOfWorkContext;
            return context.DbContext.Database.SqlQuery<SchoolMeasureGoalModel>(strSql,
                new SqlParameter("SchoolYear", schoolYear),
                new SqlParameter("Wave", (int)wave),
                new SqlParameter("AssessmentId", assessmentId),
                new SqlParameter("FinishedStatus", (int)CpallsStatus.Finished)
                ).ToList();
        }

        public List<StudentMeasureGoalModel> GetStudentMeasureGoal(List<int> studentIds, string schoolYear, Wave wave, int assessmentId)
        {
            if (studentIds == null || studentIds.Count == 0)
                return new List<StudentMeasureGoalModel>();
            string strSql = string.Format(@"SELECT SA.StudentId ,MeasureId, Goal,
                                            Amount = CASE (SM.[Status]) When 3 THEN 1 ELSE 0 END,M.TotalScored
                                            FROM V_StudentMeasures SM  LEFT JOIN Measures M ON SM.MeasureId = M.ID
                                            INNER JOIN StudentAssessments SA on SM.SAId = SA.ID  
                                            WHERE StudentId in ({0})  and SchoolYear = @SchoolYear and Wave = @Wave and SA.AssessmentId = @AssessmentId  and (SM.Status = 3 OR Exists(Select 1 from Measures M2 where M2.ParentId = SM.MeasureID and M2.ParentId>0))",
                string.Join(",", studentIds));
            //不能在此Dispose掉context，因为执行过此方法后，还会用到context
            AdeUnitOfWorkContext context = this.UnitOfWork as AdeUnitOfWorkContext;
            return context.DbContext.Database.SqlQuery<StudentMeasureGoalModel>(strSql,
                new SqlParameter("SchoolYear", schoolYear),
                new SqlParameter("Wave", (int)wave),
                new SqlParameter("AssessmentId", assessmentId),
                new SqlParameter("ActiveStatus", (int)CpallsStatus.Finished)).ToList();
        }

        public int GetStudentAssessmentIdForPlayMeasure(int assessmentId,int studentId,string schoolYear,int wave)
        {
            int saId = -1;
            StringBuilder sb = new StringBuilder();
            sb.Append( "select ID from StudentAssessments where ")
                .AppendFormat(
                    "AssessmentId = {0} and StudentId = {1} and SchoolYear ='{2}' and Wave = {3}",
                    assessmentId,studentId,schoolYear,wave);
            AdeUnitOfWorkContext context = this.UnitOfWork as AdeUnitOfWorkContext;
            using (SqlConnection conn = new SqlConnection(context.DbContext.Database.Connection.ConnectionString))
            {
                if (conn.State != ConnectionState.Open)
                    conn.Open();

                using (SqlCommand cmd = new SqlCommand(sb.ToString(), conn))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandTimeout = 20 * 60;  //Second
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            saId = (int)reader["ID"];
                           
                        }
                    }
                }
            }
            return saId;
        }
    }
}


