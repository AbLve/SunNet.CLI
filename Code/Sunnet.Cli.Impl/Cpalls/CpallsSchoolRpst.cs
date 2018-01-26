using System.Data;
using System.Data.SqlClient;
using Sunnet.Cli.Core;
using Sunnet.Cli.Core.Cpalls;
using Sunnet.Cli.Core.Cpalls.Entities;
using Sunnet.Cli.Core.Cpalls.Interfaces;
using Sunnet.Cli.Core.Cpalls.Models.Report;
using Sunnet.Framework.Core.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Cli.Impl.Cpalls
{
    public class CpallsSchoolRpst : EFRepositoryBase<CpallsSchoolEntity, int>, ICpallsSchoolRpst
    {
        public CpallsSchoolRpst(IUnitOfWork unit)
        {
            this.UnitOfWork = unit;
        }

        public List<ReportMeasureHeaderModel> GetReportMeasureHeaders(int assessmentId)
        {
            //ParentScored
            string strSql = @"  DECLARE @Measures table(ID int,AssessmentId int,Name nvarchar(max),
                                ApplyToWave nvarchar(max),ParentId int,Sort int,TotalScored bit,TotalScore decimal(18,2),LightColor bit,HasCutOffScores bit,BOYHasCutOffScores bit,MOYHasCutOffScores bit,EOYHasCutOffScores bit,[Description] varchar(4000));
                                INSERT INTO @Measures
                                SELECT ID,AssessmentId ,Name ,ApplyToWave,ParentId ,Sort,TotalScored ,
								TotalScore,LightColor,HasCutOffScores,BOYHasCutOffScores,MOYHasCutOffScores,EOYHasCutOffScores,[Description] 
                                FROM Measures M 
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

            var context = this.UnitOfWork as AdeUnitOfWorkContext;
            return context.DbContext.Database.SqlQuery<ReportMeasureHeaderModel>(strSql,
                new SqlParameter("AssessmentId", assessmentId)).ToList();
        }
        public List<ReportMeasureHeaderModel> GetReportAllMeasureHeaders(int assessmentId)
        {
            //ParentScored
            string strSql = @"  DECLARE @Measures table(ID int,AssessmentId int,Name nvarchar(max),
                                ApplyToWave nvarchar(max),ParentId int,Sort int,TotalScored bit,TotalScore decimal(18,2),LightColor bit,HasCutOffScores bit,BOYHasCutOffScores bit,MOYHasCutOffScores bit,EOYHasCutOffScores bit,[Description] varchar(4000));
                                INSERT INTO @Measures
                                SELECT ID,AssessmentId ,Name ,ApplyToWave,ParentId ,Sort,TotalScored ,
								TotalScore,LightColor,HasCutOffScores,BOYHasCutOffScores,MOYHasCutOffScores,EOYHasCutOffScores,[Description] 
                                FROM Measures M 
                                WHERE AssessmentId  =@AssessmentID AND IsDeleted = 0 AND Status = 1 
  
                                SELECT M.ID,M.ParentName,M.Name,M.ApplyToWave,M.ParentId,M.[Count],
                                        M.TotalScored,ParentScored = ISNULL(ParentScored,0),TotalScore = ISNULL(TotalScore,0),M.LightColor,M.HasCutOffScores,M.BOYHasCutOffScores,M.MOYHasCutOffScores,M.EOYHasCutOffScores,[Description]  FROM (
                                SELECT MC.*,
                                [ParentName] = (CASE MC.ParentId WHEN 1 THEN MC.Name ELSE MP.Name END),
                                [ParentSort] = (CASE MC.ParentId WHEN 1 THEN MC.Sort ELSE MP.Sort END),
                                [Count] = (SELECT COUNT(ID) FROM @Measures WHERE ParentId = MC.ParentId AND MC.ParentId > 1),
								ParentScored = MP.TotalScored 
                                FROM @Measures MC LEFT OUTER JOIN @Measures MP ON MC.ParentId = MP.ID 
                                )  M 
                                ORDER BY ParentSort ASC,Sort ASC ";

            var context = this.UnitOfWork as AdeUnitOfWorkContext;
            return context.DbContext.Database.SqlQuery<ReportMeasureHeaderModel>(strSql,
                new SqlParameter("AssessmentId", assessmentId)).ToList();
        }

        public List<SchoolRecordModel> GetReportSchoolRecords(int assessmentId, string schoolYear, string schools, DateTime startDate, DateTime endDate,
            DateTime dobStartDate, DateTime dobEndDate, IList<int> classIds)
        {
            /*
                DECLARE @AssessmentID INT;
                DECLARE @SchoolYear NVARCHAR(10);
                DECLARE @SchoolId INT;
                DECLARE @Schools  NVARCHAR(10);
                DECLARE @StartDate DATETIME;
                DECLARE @EndDate DATETIME;

                SET @AssessmentID = 17;
                SET @SchoolYear = '14-15';
                SET @SchoolId = 52
                SET @Schools = '52,';
                SET @StartDate = '2010-9-9';
                SET @EndDate = '2015-7-22';
             */

            var ids = "(" + string.Join(",", classIds) + ",0)";
            if (classIds.Count == 0)
                ids = "(0)";
            string strSql = string.Format(@"
            SELECT  SchoolId,MeasureId,Wave, TotalScore = SUM(Goal),[Count] = COUNT(SchoolId)       
                FROM( SELECT DISTINCT SSR.SchoolId, SA.Wave, SM.MeasureId,SSR.StudentId,Goal = CASE M.TotalScored WHEN 1 THEN SM.Goal ELSE 0 END,SM.Benchmark
	            FROM [StudentAssessments] SA INNER JOIN [V_StudentMeasures] SM ON SA.ID = SM.SAId
                inner join Cli_Engage__Students s on s.ID = sa.StudentId
	            INNER JOIN [dbo].[Cli_Engage__SchoolStudentRelations] SSR ON SA.StudentId = SSR.StudentId
                Inner Join Measures M on SM.MeasureId = M.ID
                Inner Join [dbo].[Cli_Engage__StudentClassRelations] SCR ON SCR.StudentId =SA.StudentId 
                WHERE SA.AssessmentId  = @AssessmentID 
                AND SA.SchoolYear = @SchoolYear
                AND CHARINDEX(CAST(SSR.SchoolId AS NVARCHAR)+',',@Schools) > 0
                AND SM.Status = 3 
                AND SCR.ClassId IN {0}  
                AND SM.UpdatedOn BETWEEN @StartDate AND @EndDate
                AND s.BirthDate BETWEEN @DOBStartDate AND @DOBEndDate
                ) R1 
                GROUP BY R1.SchoolId,R1.MeasureId,R1.Wave", ids);
            if (!schools.EndsWith(","))
                schools += ",";
            var context = this.UnitOfWork as AdeUnitOfWorkContext;
            return context.DbContext.Database.SqlQuery<SchoolRecordModel>(strSql,
                new SqlParameter("AssessmentId", assessmentId),
                new SqlParameter("SchoolYear", schoolYear),
                new SqlParameter("Schools", schools),
                new SqlParameter("StartDate", startDate),
                new SqlParameter("EndDate", endDate),
                new SqlParameter("DOBStartDate", dobStartDate),
                new SqlParameter("DOBEndDate", dobEndDate)
                ).ToList();
        }
        public List<SchoolPercentileRankModel> GetReportSchoolPercentileRankRecords(int assessmentId, string schoolYear, string schools, DateTime startDate, DateTime endDate,
            DateTime dobStartDate, DateTime dobEndDate, IList<int> classIds)
        {
            var ids = "(" + string.Join(",", classIds) + ",0)";
            if (classIds.Count == 0)
                ids = "(0)";
            string strSql = string.Format(@"
            SELECT DISTINCT SSR.SchoolId, SA.Wave, SM.MeasureId,SSR.StudentId,
                SM.PercentileRank
	            FROM [StudentAssessments] SA INNER JOIN [V_StudentMeasures] SM ON SA.ID = SM.SAId
                inner join Cli_Engage__Students s on s.ID = sa.StudentId
	            INNER JOIN [dbo].[Cli_Engage__SchoolStudentRelations] SSR ON SA.StudentId = SSR.StudentId
                Inner Join Measures M on SM.MeasureId = M.ID
                Inner Join [dbo].[Cli_Engage__StudentClassRelations] SCR ON SCR.StudentId =SA.StudentId 
                WHERE SA.AssessmentId  = @AssessmentID 
                AND SA.SchoolYear = @SchoolYear
                AND CHARINDEX(CAST(SSR.SchoolId AS NVARCHAR)+',',@Schools) > 0
                AND (SM.Status = 3 or SM.Status = 1) 
                AND SCR.ClassId IN {0}  
                AND SM.UpdatedOn BETWEEN @StartDate AND @EndDate
                AND s.BirthDate BETWEEN @DOBStartDate AND @DOBEndDate", ids);
            if (!schools.EndsWith(","))
                schools += ",";
            var context = this.UnitOfWork as AdeUnitOfWorkContext;
            return context.DbContext.Database.SqlQuery<SchoolPercentileRankModel>(strSql,
                new SqlParameter("AssessmentId", assessmentId),
                new SqlParameter("SchoolYear", schoolYear),
                new SqlParameter("Schools", schools),
                new SqlParameter("StartDate", startDate),
                new SqlParameter("EndDate", endDate),
                new SqlParameter("DOBStartDate", dobStartDate),
                new SqlParameter("DOBEndDate", dobEndDate)
                ).ToList();
        }

        public List<StudentRecordModel> GetReportStudentRecords(int assessmentId, string schoolYear, int schoolId, DateTime startDate, DateTime endDate, IList<int> classIds)
        {
            /*
             DECLARE @AssessmentID INT;
DECLARE @SchoolYear NVARCHAR(10);
DECLARE @SchoolId INT;

SET @AssessmentID = 17;
SET @SchoolYear = '14-15';
SET @SchoolId = 52
             */
            if (classIds == null || !classIds.Any())
                return null;
            var ids = "(" + string.Join(",", classIds) + ",0)";
            string strSql = string.Format(@"
            DECLARE @Measures table(ID int,AssessmentId int,Name nvarchar(max),ApplyToWave nvarchar(max),ParentId int,Sort int,TotalScored bit,Children INT);
            INSERT INTO @Measures
            SELECT ID,AssessmentId ,Name ,ApplyToWave,ParentId ,Sort,TotalScored ,Children = (CASE ParentID WHEN 1 THEN (SELECT COUNT(ID) FROM Measures M2 WHERE M2.ParentId = M1.ID) ELSE 0 END)
            FROM Measures M1
            WHERE AssessmentId  = @AssessmentID AND IsDeleted = 0 AND Status = 1 ;

            SELECT  DISTINCT SSR.SchoolId,SA.StudentId, SA.Wave, SM.MeasureId,SM.Comment, Goal = CASE M.TotalScored WHEN 1 THEN SM.Goal ELSE 0 END,SM.Benchmark,SM.BenchmarkId,SM.LowerScore,SM.HigherScore  
            FROM [dbo].[StudentAssessments] SA LEFT JOIN [dbo].[V_StudentMeasures] SM ON SA.ID = SM.SAId 
			INNER JOIN [dbo].[Cli_Engage__SchoolStudentRelations] SSR ON SA.StudentId = SSR.StudentId 
            Inner Join Measures M on SM.MeasureId = M.ID 
            Inner Join [dbo].[Cli_Engage__StudentClassRelations] SCR ON SCR.StudentId =SA.StudentId 
            WHERE  SA.AssessmentId  = @AssessmentID 
            AND SchoolYear = @SchoolYear
            AND SSR.SchoolId = @SchoolId
            AND SM.UpdatedOn BETWEEN @StartDate AND @EndDate
            AND SM.Status = 3
            AND SCR.ClassId IN {0}  
            UNION ALL 
            SELECT DISTINCT SSR.SchoolId,SA.StudentId, SA.Wave, SM.MeasureId,SM.Comment, SM.Goal,SM.Benchmark,SM.BenchmarkId,SM.LowerScore,SM.HigherScore  
			FROM [dbo].[StudentAssessments] SA 
			INNER JOIN [dbo].[Cli_Engage__SchoolStudentRelations] SSR ON SA.StudentId = SSR.StudentId
             Inner Join [dbo].[Cli_Engage__StudentClassRelations] SCR ON SCR.StudentId =SA.StudentId 
			LEFT JOIN [dbo].[V_StudentMeasures] SM ON SA.ID = SM.SAId 
            WHERE  AssessmentId  = @AssessmentID 
            AND EXISTS (SELECT 1 FROM @Measures ALL_MEASURES WHERE SM.MeasureId = ALL_MEASURES.ID AND ALL_MEASURES.Children>0)
            AND SchoolYear = @SchoolYear
            AND SSR.SchoolId = @SchoolId
           AND SCR.ClassId IN {0}  
", ids);
            var context = this.UnitOfWork as AdeUnitOfWorkContext;
            return context.DbContext.Database.SqlQuery<StudentRecordModel>(strSql,
                new SqlParameter("AssessmentId", assessmentId),
                new SqlParameter("SchoolYear", schoolYear),
                new SqlParameter("SchoolId", schoolId),
                new SqlParameter("StartDate", startDate),
                new SqlParameter("EndDate", endDate)).ToList();
        }

        public List<StudentRecordModel> GetReportStudentRecords(int assessmentId, string schoolYear, int schoolId, IEnumerable<int> studentIds, DateTime startDate, DateTime endDate)
        {
            if (studentIds == null || !studentIds.Any())
                return null;
            var ids = "(" + string.Join(",", studentIds) + ",0)";
            string strSql = string.Format(@"
            DECLARE @Measures 
			table(ID int,AssessmentId int,Name nvarchar(max),ApplyToWave nvarchar(max),ParentId int,Sort int,TotalScored bit,Children INT);
            INSERT INTO @Measures
            SELECT ID,AssessmentId ,Name ,ApplyToWave,ParentId ,Sort,TotalScored ,
			Children = (CASE ParentID WHEN 1 THEN (SELECT COUNT(ID) FROM Measures M2 WHERE M2.ParentId = M1.ID) ELSE 0 END)
            FROM Measures M1
            WHERE AssessmentId  = @AssessmentID AND IsDeleted = 0 AND Status = 1 ;

            SELECT SSR.SchoolId,SA.StudentId, SA.Wave, SM.MeasureId,SM.Comment, Goal = CASE M.TotalScored WHEN 1 THEN SM.Goal ELSE 0 END,SM.Benchmark,
            SM.PercentileRank,SM.BenchmarkId,SM.LowerScore,SM.HigherScore  
			FROM [dbo].[StudentAssessments] SA 
			INNER JOIN [dbo].[V_StudentMeasures] SM ON SA.ID = SM.SAId 
			INNER JOIN [dbo].[Cli_Engage__SchoolStudentRelations] SSR ON SA.StudentId = SSR.StudentId
            Inner Join Measures M on SM.MeasureId = M.ID 
            WHERE  SA.AssessmentId  = @AssessmentID 
			AND SSR.SchoolId = @SchoolId
            AND SchoolYear = @SchoolYear
            AND SM.Status = 3
            AND SM.UpdatedOn BETWEEN @StartDate AND @EndDate
            AND SA.StudentId in {0}
UNION ALL
            SELECT  SSR.SchoolId,SA.StudentId, SA.Wave, SM.MeasureId,SM.Comment, SM.Goal,SM.Benchmark,SM.PercentileRank,SM.BenchmarkId,SM.LowerScore,SM.HigherScore  
			FROM [dbo].[StudentAssessments] SA 
			INNER JOIN [dbo].[V_StudentMeasures] SM ON SA.ID = SM.SAId
			INNER JOIN [dbo].[Cli_Engage__SchoolStudentRelations] SSR ON SA.StudentId = SSR.StudentId
            WHERE  AssessmentId  = @AssessmentID 
			AND SSR.SchoolId = @SchoolId
            AND EXISTS (SELECT 1 FROM @Measures ALL_MEASURES WHERE SM.MeasureId = ALL_MEASURES.ID AND ALL_MEASURES.Children>0)
            AND SchoolYear = @SchoolYear
            AND SA.StudentId in {0}
", ids);

            var context = this.UnitOfWork as AdeUnitOfWorkContext;
            return context.DbContext.Database.SqlQuery<StudentRecordModel>(strSql,
                new SqlParameter("AssessmentId", assessmentId),
                new SqlParameter("SchoolYear", schoolYear),
                new SqlParameter("SchoolId", schoolId),
                new SqlParameter("StartDate", startDate),
                new SqlParameter("EndDate", endDate)).ToList();
        }

        public List<StudentRecordModel> GetReportStudentRecords(int assessmentId, string schoolYear, IEnumerable<int> schoolIds, DateTime startDate, DateTime endDate,
            DateTime dobStartDate, DateTime dobEndDate, IList<int> listClassId)
        {
            var ids = "(" + string.Join(",", schoolIds) + ",-1)";
            var classIds = "(" + string.Join(",", listClassId) + ")";
            string strSql = string.Format(@"
            DECLARE @Measures 
			table(ID int,AssessmentId int,Name nvarchar(max),ApplyToWave nvarchar(max),ParentId int,Sort int,TotalScored bit,Children INT);
            
			INSERT INTO @Measures
            SELECT ID,AssessmentId ,Name ,ApplyToWave,ParentId ,Sort,TotalScored ,Children = (CASE ParentID WHEN 1 THEN (SELECT COUNT(ID) FROM Measures M2 WHERE M2.ParentId = M1.ID) ELSE 0 END)
            FROM Measures M1
            WHERE AssessmentId  = @AssessmentID AND IsDeleted = 0 AND Status = 1 ;

            SELECT SSR.SchoolId,SA.StudentId, SA.Wave, SM.MeasureId,SM.Comment, Goal = CASE M.TotalScored WHEN 1 THEN SM.Goal ELSE 0 END,SM.Benchmark  ,
			[SchoolIds] = (SELECT CAST(SSR2.SchoolId AS VARCHAR) + ',' FROM [dbo].[Cli_Engage__SchoolStudentRelations] SSR2 WHERE SSR2.StudentId = SA.StudentId FOR XML PATH('')),
            SM.BenchmarkId,SM.LowerScore,SM.HigherScore
			FROM [dbo].[StudentAssessments] SA 
            inner join Cli_Engage__Students S on S.ID = SA.StudentId
			INNER JOIN [dbo].[Cli_Engage__SchoolStudentRelations] SSR ON SA.StudentId = SSR.StudentId
			INNER JOIN [dbo].[V_StudentMeasures] SM ON SA.ID = SM.SAId 
            Inner Join Measures M on SM.MeasureId = M.ID 
            Inner Join [dbo].[Cli_Engage__StudentClassRelations] SCR ON SCR.StudentId =SA.StudentId 
            WHERE  SA.AssessmentId  = @AssessmentID 
            AND SA.SchoolYear = @SchoolYear
            AND SSR.SchoolId in {0}
            AND SM.Status = 3
            AND SM.UpdatedOn BETWEEN @StartDate AND @EndDate
            AND S.BirthDate BETWEEN @DOBStartDate AND @DOBEndDate
            AND SCR.ClassId IN {1}  

            UNION ALL
            SELECT  SSR.SchoolId,SA.StudentId, SA.Wave, SM.MeasureId,SM.Comment, SM.Goal,SM.Benchmark  ,
			[SchoolIds] = (SELECT CAST(SSR2.SchoolId AS VARCHAR) + ',' FROM [dbo].[Cli_Engage__SchoolStudentRelations] SSR2 WHERE SSR2.StudentId = SA.StudentId FOR XML PATH('')),
            SM.BenchmarkId,SM.LowerScore,SM.HigherScore
			FROM [dbo].[StudentAssessments] SA 
            inner join Cli_Engage__Students S on S.ID = SA.StudentId
			INNER JOIN [dbo].[Cli_Engage__SchoolStudentRelations] SSR ON SA.StudentId = SSR.StudentId
			INNER JOIN [dbo].[V_StudentMeasures] SM ON SA.ID = SM.SAId
            Inner Join [dbo].[Cli_Engage__StudentClassRelations] SCR ON SCR.StudentId =SA.StudentId 
            WHERE  AssessmentId  = @AssessmentID 
            AND EXISTS (SELECT 1 FROM @Measures ALL_MEASURES WHERE SM.MeasureId = ALL_MEASURES.ID AND ALL_MEASURES.Children>0)
            AND SA.SchoolYear = @SchoolYear
            AND SSR.SchoolId in {0}
            AND S.BirthDate BETWEEN @DOBStartDate AND @DOBEndDate
            AND SCR.ClassId IN {1}  ", ids, classIds);

            var context = this.UnitOfWork as AdeUnitOfWorkContext;
            try
            {
                context.DbContext.Database.Connection.Open();

                var command = context.DbContext.Database.Connection.CreateCommand();
                command.CommandText = strSql;
                command.Parameters.Add(new SqlParameter("AssessmentId", assessmentId));
                command.Parameters.Add(new SqlParameter("SchoolYear", schoolYear));
                command.Parameters.Add(new SqlParameter("StartDate", startDate));
                command.Parameters.Add(new SqlParameter("EndDate", endDate));
                command.Parameters.Add(new SqlParameter("DOBStartDate", dobStartDate));
                command.Parameters.Add(new SqlParameter("DOBEndDate", dobEndDate));
                command.CommandTimeout = 20 * 60;
                List<StudentRecordModel> list = null;
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (list == null)
                            list = new List<StudentRecordModel>();
                        var index = 0;
                        list.Add(new StudentRecordModel()
                        {
                            SchoolId = reader.GetInt32(index++),
                            StudentId = reader.GetInt32(index++),
                            Wave = (Wave)reader.GetByte(index++),
                            MeasureId = reader.GetInt32(index++),
                            Comment = reader.GetString(index++),
                            Goal = reader.GetDecimal(index++),
                            Benchmark = reader.GetDecimal(index++),
                            SchoolIds =
                                reader.GetString(index++)
                                    .Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                                    .Select(int.Parse)
                                    .ToList(),
                            BenchmarkId = reader.GetInt32(index++),
                            LowerScore = reader.GetDecimal(index++),
                            HigherScore = reader.GetDecimal(index++)
                        });
                    }
                }
                return list;
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

        public List<StudentRecordModel> GetReportStudentRecordsByDistrict(int assessmentId, string schoolYear, int districtId,
            List<Wave> waves)
        {
            /*
            DECLARE @DistrictId INT;
            DECLARE @AssessmentID INT;
            DECLARE @SchoolYear NVARCHAR(10);
            DECLARE @SchoolId INT;
            DECLARE @Waves NVARCHAR(10);

            SET @DistrictId = 67
            SET @AssessmentID = 17;
            SET @SchoolYear = '14-15';
            SET @SchoolId = 52
            SET @Waves = '1,2,3';
             */

            string strSql = @"
            DECLARE @Measures table(ID int,AssessmentId int,Name nvarchar(max),ApplyToWave nvarchar(max),ParentId int,Sort int,TotalScored bit,Children INT);
            INSERT INTO @Measures
            SELECT ID,AssessmentId ,Name ,ApplyToWave,ParentId ,Sort,TotalScored ,Children = (CASE ParentID WHEN 1 THEN (SELECT COUNT(ID) FROM Measures M2 WHERE M2.ParentId = M1.ID) ELSE 0 END)
            FROM Measures M1
            WHERE AssessmentId  = @AssessmentId AND IsDeleted = 0 AND Status = 1 ;

	        DECLARE @StudentOfDistrict TABLE(StudentId INT);
	        INSERT INTO @StudentOfDistrict
	        SELECT STU.ID FROM [Cli_Engage__CommunitySchoolRelations] CS
				        INNER JOIN [dbo].[Cli_Engage__Schools] SCH ON CS.SchoolID = SCH.ID AND SCH.[Status] = 1
				        INNER JOIN [Cli_Engage__SchoolStudentRelations] SS ON SCH.ID = SS.SchoolId
				        INNER JOIN [dbo].[Cli_Engage__Students] STU ON SS.StudentId = STU.ID AND STU.[Status] = 1
				        WHERE CS.CommunityId = @DistrictId

            SELECT  SA.StudentId, SA.Wave, SM.MeasureId, Goal = CASE M.TotalScored WHEN 1 THEN SM.Goal ELSE 0 END,SM.Benchmark,
			[SchoolIds] = (SELECT CAST(SSR.SchoolId AS VARCHAR) + ',' FROM [dbo].[Cli_Engage__SchoolStudentRelations] SSR WHERE SSR.StudentId = SA.StudentId FOR XML PATH(''))
			FROM [dbo].[V_StudentMeasures] SM LEFT JOIN [dbo].[StudentAssessments] SA ON SA.ID = SM.SAId
            Inner Join Measures M on SM.MeasureId = M.ID
            WHERE  SA.AssessmentId  = @AssessmentId
			AND SA.SchoolYear = @SchoolYear
			AND CHARINDEX(CAST(SA.Wave AS VARCHAR),@Waves,0) > 0
			AND EXISTS (SELECT 1 FROM @StudentOfDistrict ALLSTU WHERE ALLSTU.StudentId = SA.StudentId )
            AND SM.Status = 3

            UNION ALL
            SELECT  SA.StudentId, SA.Wave, SM.MeasureId, SM.Goal,SM.Benchmark,
			[SchoolIds] = (SELECT CAST(SSR.SchoolId AS VARCHAR) + ',' FROM [dbo].[Cli_Engage__SchoolStudentRelations] SSR WHERE SSR.StudentId = SA.StudentId FOR XML PATH(''))
			FROM  [dbo].[V_StudentMeasures] SM LEFT JOIN [dbo].[StudentAssessments] SA ON SA.ID = SM.SAId
            WHERE  AssessmentId  = @AssessmentId
            AND EXISTS (SELECT 1 FROM @Measures ALL_MEASURES WHERE SM.MeasureId = ALL_MEASURES.ID AND ALL_MEASURES.Children>0)
            AND SA.SchoolYear = @SchoolYear
			AND CHARINDEX(CAST(SA.Wave AS VARCHAR),@Waves,0) > 0
			AND EXISTS (SELECT 1 FROM @StudentOfDistrict ALLSTU WHERE ALLSTU.StudentId = SA.StudentId );
";
            var context = this.UnitOfWork as AdeUnitOfWorkContext;
            try
            {
                context.DbContext.Database.Connection.Open();

                var command = context.DbContext.Database.Connection.CreateCommand();
                command.CommandText = strSql;
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("AssessmentId", assessmentId));
                command.Parameters.Add(new SqlParameter("SchoolYear", schoolYear));
                command.Parameters.Add(new SqlParameter("DistrictId", districtId));
                command.Parameters.Add(new SqlParameter("Waves", waves == null ? "" :
                    string.Join(",", waves.Select(x => (byte)x).ToList())));
                command.CommandTimeout = 20 * 60;
                List<StudentRecordModel> list = null;
                using (var reader = command.ExecuteReader())
                {
                    list = new List<StudentRecordModel>();
                    while (reader.Read())
                    {
                        var index = 0;
                        list.Add(new StudentRecordModel()
                        {
                            StudentId = reader.GetInt32(index++),
                            Wave = (Wave)reader.GetByte(index++),
                            MeasureId = reader.GetInt32(index++),
                            Goal = reader.GetDecimal(index++),
                            Benchmark = reader.GetDecimal(index++),
                            SchoolIds =
                                reader.GetString(index++)
                                    .Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                                    .Select(int.Parse)
                                    .ToList()
                        });
                    }
                }
                return list;
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
