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

namespace Sunnet.Cli.Impl.Practices
{
    public class PracticeStudentMeasureRpst : EFRepositoryBase<PracticeStudentMeasureEntity, int>, IPracticeStudentMeasureRpst
    {
        public PracticeStudentMeasureRpst(IUnitOfWork unit)
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
        public void RecalculateParentGoal(int saId, int parentMeasureId = 0)
        {
            string strSql = @"EXEC  [Recalculate_Parent_Goal_SA] @SaId,@parentMeasureId";

            PracticeUnitOfWorkContext context = this.UnitOfWork as PracticeUnitOfWorkContext;
            var result = context.DbContext.Database.ExecuteSqlCommand(strSql,
                new SqlParameter("SaId", saId), new SqlParameter("parentMeasureId", parentMeasureId));
        }


        public void RefreshClassroom(int assessmentId, Wave wave, int userId)
        {
            string strSql = @"EXEC [RefreshClassroom] @assessmentId,@wave,@UserId";

            PracticeUnitOfWorkContext context = this.UnitOfWork as PracticeUnitOfWorkContext;
            var result = context.DbContext.Database.ExecuteSqlCommand(strSql,
                new SqlParameter("AssessmentId", assessmentId), new SqlParameter("Wave", wave), new SqlParameter("UserId", userId));
        }

        public void CleanClassroom(int assessmentId)
        {
            string strSql = @"EXEC [CleanClassroom] @assessmentId";

            PracticeUnitOfWorkContext context = this.UnitOfWork as PracticeUnitOfWorkContext;
            var result = context.DbContext.Database.ExecuteSqlCommand(strSql,
                new SqlParameter("AssessmentId", assessmentId)

                );
        }


        #region 之前存在于CpallsSchoolRpst中的方法，因为Practice的相关数据与School没关系了，所以直接移到了Practice中
        public List<ReportMeasureHeaderModel> GetReportMeasureHeaders(int assessmentId)
        {
            //ParentScored
            string strSql = @"  DECLARE @Measures table(ID int,AssessmentId int,Name nvarchar(max),
                                ApplyToWave nvarchar(max),ParentId int,Sort int,TotalScored bit,TotalScore decimal(18,2),LightColor bit,HasCutOffScores bit,BOYHasCutOffScores bit,MOYHasCutOffScores bit,EOYHasCutOffScores bit,[Description] varchar(4000));
                                INSERT INTO @Measures
                                SELECT ID,AssessmentId ,Name ,ApplyToWave,ParentId ,Sort,TotalScored ,
								TotalScore,LightColor,HasCutOffScores,BOYHasCutOffScores,MOYHasCutOffScores,EOYHasCutOffScores,[Description] 
                                FROM CLI_ADE_Measures M 
                                WHERE AssessmentId  =@AssessmentID AND IsDeleted = 0 AND Status = 1 
  
                                SELECT M.ID,M.ParentName,M.Name,M.ApplyToWave,M.ParentId,M.[Count],
                                        M.TotalScored,ParentScored = ISNULL(ParentScored,0),TotalScore = ISNULL(TotalScore,0),M.LightColor,M.HasCutOffScores,M.BOYHasCutOffScores,M.MOYHasCutOffScores,M.EOYHasCutOffScores,[Description]  FROM (
                                SELECT MC.*,
                                [ParentName] = (CASE MC.ParentId WHEN 1 THEN MC.Name ELSE MP.Name END),
                                [ParentSort] = (CASE MC.ParentId WHEN 1 THEN MC.Sort ELSE MP.Sort END),
                                [Count] = (SELECT COUNT(ID) FROM @Measures WHERE ParentId = MC.ParentId AND MC.ParentId > 1),
								ParentScored = MP.TotalScored 
                                FROM @Measures MC LEFT OUTER JOIN @Measures MP ON MC.ParentId = MP.ID 
                                WHERE MC.ParentId > 1 OR NOT EXISTS(SELECT 1 FROM @Measures TEMP_M WHERE TEMP_M.ParentId = MC.ID)
                                )  M 
                                ORDER BY ParentSort ASC,Sort ASC ";

            var context = this.UnitOfWork as PracticeUnitOfWorkContext;
            return context.DbContext.Database.SqlQuery<ReportMeasureHeaderModel>(strSql,
                new SqlParameter("AssessmentId", assessmentId)).ToList();
        }

        /// <summary>
        /// Student Summary Report,Percentile Rank Report
        /// </summary>
        /// <param name="assessmentId"></param>
        /// <param name="schoolYear"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public List<StudentRecordModel> GetReportStudentRecords(int assessmentId, string schoolYear, DateTime startDate, DateTime endDate, IEnumerable<int> studentIds, int userId)
        {
            if (studentIds == null || !studentIds.Any())
                return null;
            var ids = "(" + string.Join(",", studentIds) + ",0)";
            string strSql = string.Format(@"
            DECLARE @Measures table(ID int,AssessmentId int,Name nvarchar(max),ApplyToWave nvarchar(max),ParentId int,Sort int,TotalScored bit,Children INT);
            INSERT INTO @Measures
            SELECT ID,AssessmentId ,Name ,ApplyToWave,ParentId ,Sort,TotalScored ,Children = (CASE ParentID WHEN 1 THEN (SELECT COUNT(ID) FROM CLI_ADE_Measures M2 WHERE M2.ParentId = M1.ID) ELSE 0 END)
            FROM CLI_ADE_Measures M1
            WHERE AssessmentId  = @AssessmentID AND IsDeleted = 0 AND Status = 1 ;

            SELECT  SA.StudentId, SA.Wave, SM.MeasureId,SM.Comment, Goal = CASE M.TotalScored WHEN 1 THEN SM.Goal ELSE 0 END,SM.Benchmark,SM.PercentileRank,SM.BenchmarkId,SM.LowerScore,SM.HigherScore  
            FROM [dbo].[PracticeStudentAssessments] SA LEFT JOIN [dbo].[V_StudentMeasures] SM ON SA.ID = SM.SAId 
            Inner Join CLI_ADE_Measures M on SM.MeasureId = M.ID 
            WHERE  SA.AssessmentId  = @AssessmentID 
            AND SchoolYear = @SchoolYear
            AND SM.UpdatedOn BETWEEN @StartDate AND @EndDate
            AND SM.Status = 3 AND (SA.CreatedBy={1} or SA.CreatedBy=0)
            AND SA.StudentId in {0}
            UNION ALL 
            SELECT SA.StudentId, SA.Wave, SM.MeasureId,SM.Comment, SM.Goal,SM.Benchmark,SM.PercentileRank,SM.BenchmarkId,SM.LowerScore,SM.HigherScore  
			FROM [dbo].[PracticeStudentAssessments] SA 
			LEFT JOIN [dbo].[V_StudentMeasures] SM ON SA.ID = SM.SAId 
            WHERE  AssessmentId  = @AssessmentID AND (SA.CreatedBy={1} or SA.CreatedBy=0)
            AND EXISTS (SELECT 1 FROM @Measures ALL_MEASURES WHERE SM.MeasureId = ALL_MEASURES.ID AND ALL_MEASURES.Children>0)
            AND SchoolYear = @SchoolYear
            AND SA.StudentId in {0}", ids, userId);
            var context = this.UnitOfWork as PracticeUnitOfWorkContext;
            return context.DbContext.Database.SqlQuery<StudentRecordModel>(strSql,
                new SqlParameter("AssessmentId", assessmentId),
                new SqlParameter("SchoolYear", schoolYear),
                new SqlParameter("StartDate", startDate),
                new SqlParameter("EndDate", endDate)).ToList();
        }

        public List<WaveFinishedOnModel> GetWaveFinishedDate(int assessmentId, int userId)
        {
            string strSql = "";
            strSql = @"            
                    SELECT SA.Wave,FinishedOn = MIN(SM.UpdatedOn),SA.StudentId
                    FROM [dbo].[PracticeStudentAssessments] SA 
                    INNER JOIN [dbo].[PracticeStudentMeasures] SM ON SA.ID = SM.SAId 
					INNER JOIN [dbo].[PracticeDemoStudents] S ON S.AssessmentId=SA.AssessmentId
                    WHERE SA.AssessmentId=@AssessmentId AND (SA.CreatedBy=@UserId or SA.CreatedBy=0)
                    GROUP BY SA.Wave,SA.StudentId";
            var context = this.UnitOfWork as PracticeUnitOfWorkContext;
            try
            {
                return context.DbContext.Database.SqlQuery<WaveFinishedOnModel>(strSql.ToString(),
                    new SqlParameter("AssessmentId", assessmentId), new SqlParameter("UserId", userId)
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
        }
        #endregion
    }
}