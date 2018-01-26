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
using Sunnet.Cli.Core.Cpalls;
using Sunnet.Cli.Core.Cpalls.Entities;
using Sunnet.Cli.Core.Cpalls.Interfaces;
using Sunnet.Cli.Core.Cpalls.Models;
using Sunnet.Cli.Core.Reports.Models;
using Sunnet.Framework.Core.Base;
using Sunnet.Framework.Extensions;
using Sunnet.Cli.Core.Cpalls.Models.Report;
using Sunnet.Cli.Core.Ade;

namespace Sunnet.Cli.Impl.Cpalls
{
    public class StudentMeasureRpst : EFRepositoryBase<StudentMeasureEntity, int>, IStudentMeasureRpst
    {
        public StudentMeasureRpst(IUnitOfWork unit)
        {
            this.UnitOfWork = unit;
        }

        public int UpdateBenchmark(int studentMeasureId, int benchmarkId, decimal lowerScore, decimal higherScore, bool ShowOnGroup, bool benchmarkChanged)
        {
            int count = 0;
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.Append("UPDATE  StudentMeasures SET ");
                strSql.Append("BenchmarkId = @BenchmarkId, ");
                strSql.Append("LowerScore = @LowerScore, ");
                strSql.Append("HigherScore = @HigherScore, ");
                strSql.Append("ShowOnGroup = @ShowOnGroup, ");
                strSql.Append("BenchmarkChanged = @BenchmarkChanged ");
                //strSql.Append("UpdatedOn = @UpdatedOn "); //David 0401/2017, Request by Client, Cannot change the date on rescoring.
                strSql.Append("WHERE ID = @ID ");
                AdeUnitOfWorkContext context = this.UnitOfWork as AdeUnitOfWorkContext;
                count = context.DbContext.Database.ExecuteSqlCommand(strSql.ToString(),
                    new SqlParameter("BenchmarkId", benchmarkId),
                    new SqlParameter("LowerScore", lowerScore),
                    new SqlParameter("HigherScore", higherScore),
                    new SqlParameter("ShowOnGroup", ShowOnGroup),
                    new SqlParameter("BenchmarkChanged", benchmarkChanged),
                    new SqlParameter("ID", studentMeasureId));
                // new SqlParameter("UpdatedOn", DateTime.Now));
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return count;
        }
        public int UpdatePercentileRank(int studentMeasureId, string percentileRank)
        {
            int count = 0;
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.Append("UPDATE  StudentMeasures SET ");
                strSql.Append("PercentileRank = @PercentileRank ");
                //strSql.Append("UpdatedOn = @UpdatedOn "); //David 04/01/2017 Cannot change the assessment date when resocing
                strSql.Append("WHERE ID = @ID ");
                AdeUnitOfWorkContext context = this.UnitOfWork as AdeUnitOfWorkContext;
                count = context.DbContext.Database.ExecuteSqlCommand(strSql.ToString(),
                    new SqlParameter("PercentileRank", percentileRank),
                    new SqlParameter("ID", studentMeasureId));
                //new SqlParameter("UpdatedOn", DateTime.Now));
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return count;
        }

        public int UpdateBenchmarkChangedToFalse(int measureId)
        {
            int count = 0;
            try
            {  //David 04/01/2017 Cannot change the updated on
                string strSql = "UPDATE StudentMeasures SET BenchmarkChanged=0 WHERE MeasureId=@MeasureId AND BenchmarkChanged=1";
                AdeUnitOfWorkContext context = this.UnitOfWork as AdeUnitOfWorkContext;
                count = context.DbContext.Database.ExecuteSqlCommand(strSql,
                    new SqlParameter("MeasureId", measureId)
                    );
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return count;
        }
        public int InitMeasures(int userId, int assessmentId, string schoolYear, int studentId,
            DateTime studentBirthday, Wave wave, IEnumerable<int> measureIds)
        {
            string strSql = @"EXEC  [InitMeasures]  @UserId,@AssessmentId,@SchoolYear, @StudentId, 
                            @StudentBirthday, @Wave, @MeasureIds, @StudentAssessmentId OUTPUT";

            AdeUnitOfWorkContext context = this.UnitOfWork as AdeUnitOfWorkContext;
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

            AdeUnitOfWorkContext context = this.UnitOfWork as AdeUnitOfWorkContext;
            var result = context.DbContext.Database.ExecuteSqlCommand(strSql,
                new SqlParameter("SaId", saId), new SqlParameter("parentMeasureId", parentMeasureId));
        }

        /// <summary>
        /// communityId ==0 时，表示，按SchoolId 查询，只处理 双语的学生
        /// </summary>
        public List<CompletionMeasureModel> GetCompletionCombinedStudentMeasure(int communityId, int schoolId, int assessmentId, int otherAssessmentId
            , Wave wave, List<int> measureIdList, List<int> hasChilderMeasureId, string schoolYear, DateTime startDate, DateTime endDate, DateTime dobStartDate, DateTime dobEndDate, IList<int> classIds, out List<CompletionMeasureModel> otherList)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(" select Distinct sa.Wave,sm.MeasureId,m.Name as MeasureName,m.RelatedMeasureId,m.ParentId ,sm.Status ,m.Sort ,a.Language,sa.StudentId ,sm.Goal ")
                .Append("  into #list  from StudentMeasures sm ")
                .Append("  inner join Measures m on sm.MeasureId = m.ID ")
                .Append("  inner join Assessments a on a.ID = m.AssessmentId ")
                .Append("  inner join StudentAssessments sa on sa.ID = sm.SAId ")
                .Append("  inner join Cli_Engage__Students s on s.ID = sa.StudentId")
                .Append("  inner join Cli_Engage__SchoolStudentRelations ssr on ssr.StudentId = s.ID")
                .Append("  inner join Cli_Engage__Schools sch on sch.ID = ssr.SchoolId")
                 .Append("  inner join Cli_Engage__StudentClassRelations scr on scr.StudentId = s.ID");
            if (communityId > 0)
            {
                sb.Append("  left join Cli_Engage__SchoolTypes st on st.id = sch.SchoolTypeId");
                sb.Append("   inner join Cli_Engage__CommunitySchoolRelations csr on csr.SchoolId = ssr.SchoolId ");
            }

            sb.Append(" where 1=1 ")
            .AppendFormat(" and sm.UpdatedOn between '{0}' and '{1}' ", startDate, endDate)
            .AppendFormat(" and s.BirthDate between '{0}' and '{1}' ", dobStartDate, dobEndDate);
            if (communityId > 0)
                sb.Append(" and SUBSTRING(st.Name,1,4)!='Demo' ")
                    .AppendFormat(" and csr.CommunityId ={0} ", communityId);
            else
                sb.AppendFormat(" and ssr.SchoolId = {0} ", schoolId);

            sb.AppendFormat(" and a.id in({0},{1}) ", assessmentId, otherAssessmentId)
                .AppendFormat(" and sa.SchoolYear='{0}' ", schoolYear)
                .AppendFormat(" and sa.Wave={0} ", (byte)wave)
                .AppendFormat(" and s.Status = 1 and s.AssessmentLanguage = {0} ", (byte)StudentAssessmentLanguage.Bilingual)
                .AppendFormat(" and (m.id in ({0}) or m.RelatedMeasureId in ({0}))", string.Join(",", measureIdList))
                .AppendFormat(" and (scr.ClassId in ({0}))", string.Join(",", classIds));

            /// --处理没有Relation的
            sb.AppendFormat("; select * into #noRelated from #list where RelatedMeasureId = 0 and Status > {0}", (byte)CpallsStatus.Paused);
            // --处理英语完成的
            sb.AppendFormat("; select * into #finshEnglish from #list where status = {0} and Language = {1} and RelatedMeasureId > 0", (byte)CpallsStatus.Finished, (byte)StudentAssessmentLanguage.English);
            // --删除英语完成的相关measure
            sb.Append("; delete from #list  from #list l  inner join #finshEnglish e on e.StudentId = l.StudentId and e.MeasureId = l.RelatedMeasureId ");
            // --处理Spanish 完成的
            sb.AppendFormat("; select * into #finshSpanish from  #list where status = {0} and Language = {1} and RelatedMeasureId > 0 ; update #finshSpanish set  MeasureId =  RelatedMeasureId "
                , (byte)CpallsStatus.Finished, (byte)StudentAssessmentLanguage.Spanish);
            // --处理locked 状态的
            sb.Append("; select l.*  into #locked from #list l ")
            .AppendFormat(" inner join #list l2 on l.StudentId = l2.StudentId and l.MeasureId = l2.RelatedMeasureId and l2.Status={0}  where l.Status = {0} and l.Language = {1} "
            , (byte)CpallsStatus.Locked, (byte)StudentAssessmentLanguage.English);
            // --处理好的 数据
            sb.Append("; select * from #noRelated")
                .Append("  union all select * from #finshEnglish ")
                .Append("  union all select * from #finshSpanish ")
                .Append("  union all select * from #locked ");

            if (hasChilderMeasureId != null && hasChilderMeasureId.Count > 0)
            {
                // --需要额外处理的数据
                sb.Append("; select StudentId , MeasureId , RelatedMeasureId into #OtherLockParent from #list  ")
                    .AppendFormat("  where Goal > -1 and RelatedMeasureId in ({0}) group by StudentId , MeasureId , RelatedMeasureId"
                      , string.Join(",", hasChilderMeasureId))
                      .Append("; select parentId ,measureId into #OtherLockMeasure from  #list ")
                .AppendFormat(" where parentId in ({0}) and measureId != parentId and RelatedMeasureId = 0 group by parentId ,measureId ", string.Join(",", hasChilderMeasureId))
                .Append("; select p.StudentId,p.RelatedMeasureId,m.* into #OtherLockList from #OtherLockParent  p ")
                .Append("  left join  #OtherLockMeasure m on p.RelatedMeasureId = m.ParentId order by m.MeasureId ")
                 .AppendFormat(";select {0} as Wave,o.MeasureId,'' as MeasureName,o.RelatedMeasureId,1 as ParentId,{1} as Status,0 as Sort,1 as Language,o.StudentId,0 as Goal from #OtherLockList o ", (byte)wave, (byte)CpallsStatus.Locked)
                 .Append("  left join #list l  on l.MeasureId  = o.MeasureId and l.StudentId = o.StudentId and l.Status > 1  where l.MeasureId is null ");
            }

            AdeUnitOfWorkContext context = this.UnitOfWork as AdeUnitOfWorkContext;

            List<CompletionMeasureModel> list = new List<CompletionMeasureModel>();
            otherList = new List<CompletionMeasureModel>();

            try
            {
                if (context.DbContext.Database.Connection.State != ConnectionState.Open)
                    context.DbContext.Database.Connection.Open();

                var command = context.DbContext.Database.Connection.CreateCommand();
                command.CommandText = sb.ToString();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new CompletionMeasureModel(reader));
                    }
                    if (reader.NextResult())
                    {
                        while (reader.Read())
                            otherList.Add(new CompletionMeasureModel(reader));
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return list;
        }

        /// <summary>
        /// communityId ==0 时，表示，按SchoolId 查询，双语报表，处理只是英语或者西班牙语的学生
        /// </summary>
        public List<CompletionMeasureModel> GetCompletionEnglishAndSpanishStudentMeasure(int communityId, int schoolId, int assessmentId, int otherAssessmentId
            , Wave wave, List<int> measureIdList, string schoolYear, DateTime startDate, DateTime endDate, DateTime dobStartDate, DateTime dobEndDate, IList<int> classIds)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(" select Distinct sa.Wave,sm.MeasureId,m.Name as MeasureName,m.RelatedMeasureId,m.ParentId ,sm.Status ,m.Sort ,a.Language,sa.StudentId ,sm.Goal ")
                .Append("  from StudentMeasures sm ")
                .Append("  inner join Measures m on sm.MeasureId = m.ID ")
                .Append("  inner join Assessments a on a.ID = m.AssessmentId ")
                .Append("  inner join StudentAssessments sa on sa.ID = sm.SAId ")
                .Append("  inner join Cli_Engage__Students s on s.ID = sa.StudentId")
                .Append("  inner join Cli_Engage__SchoolStudentRelations ssr on ssr.StudentId = s.ID")
                .Append("  inner join Cli_Engage__Schools sch on sch.ID = ssr.SchoolId")
               .Append("  inner join Cli_Engage__StudentClassRelations scr on scr.StudentId = s.ID");

            if (communityId > 0)
                sb.Append("   inner join Cli_Engage__CommunitySchoolRelations csr on csr.SchoolId = ssr.SchoolId")
                    .Append("  left join Cli_Engage__SchoolTypes st on st.id = sch.SchoolTypeId ");

            sb.Append(" where 1=1 ")
                .AppendFormat(" and sm.UpdatedOn between '{0}' and '{1}' ", startDate, endDate)
                .AppendFormat(" and s.BirthDate between '{0}' and '{1}' ", dobStartDate, dobEndDate);

            if (communityId > 0)
                sb.AppendFormat(" and csr.CommunityId ={0} ", communityId)
                    .Append("  and SUBSTRING(st.Name,1,4)!='Demo' ");
            else
                sb.AppendFormat(" and ssr.SchoolId = {0} ", schoolId);
            sb.AppendFormat(" and a.id in({0},{1}) ", assessmentId, otherAssessmentId)
                .AppendFormat(" and sa.SchoolYear='{0}' ", schoolYear)
                .AppendFormat(" and sm.Status > {0} ", (byte)CpallsStatus.Paused)
                .AppendFormat(" and sa.Wave={0} ", (byte)wave)
                .AppendFormat(" and s.Status = 1 and s.AssessmentLanguage in({0},{1}) ", (byte)StudentAssessmentLanguage.English, (byte)StudentAssessmentLanguage.Spanish)
                .AppendFormat(" and (m.id in ({0}) or m.RelatedMeasureId in ({0}))", string.Join(",", measureIdList))
                .AppendFormat(" and (scr.ClassId in ({0}))", string.Join(",", classIds));


            AdeUnitOfWorkContext context = this.UnitOfWork as AdeUnitOfWorkContext;

            List<CompletionMeasureModel> list = new List<CompletionMeasureModel>();

            try
            {
                if (context.DbContext.Database.Connection.State != ConnectionState.Open)
                    context.DbContext.Database.Connection.Open();

                var command = context.DbContext.Database.Connection.CreateCommand();
                command.CommandText = sb.ToString();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new CompletionMeasureModel(reader));
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return list;
        }

        /// <summary>
        /// communityId ==0 时，表示，按SchoolId 查寻 ,完成报表，English 版或 Spanlish
        /// </summary>
        public List<CompletionMeasureModel> GetCompletionStudentMeasure(int communityId, int schoolId, int assessmentId
            , Wave wave, List<int> measureIdList, string schoolYear, DateTime startDate, DateTime endDate, DateTime dobStartDate, DateTime dobEndDate, StudentAssessmentLanguage language, IList<int> classIds)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(" select Distinct sa.Wave,sm.MeasureId,m.Name as MeasureName,m.RelatedMeasureId,m.ParentId ,sm.Status ,m.Sort ,a.Language,sa.StudentId  ,sm.Goal")
                .Append("    from StudentMeasures sm ")
                .Append("  inner join Measures m on sm.MeasureId = m.ID ")
                .Append("  inner join Assessments a on a.ID = m.AssessmentId ")
                .Append("  inner join StudentAssessments sa on sa.ID = sm.SAId ")
                .Append("  inner join Cli_Engage__Students s on s.ID = sa.StudentId")
                .Append("  inner join Cli_Engage__SchoolStudentRelations ssr on ssr.StudentId = s.Id")
                .Append("  inner join Cli_Engage__Schools sch on sch.ID = ssr.SchoolId")
                .Append("  inner join Cli_Engage__StudentClassRelations scr on scr.StudentId = s.ID");
            if (communityId > 0)
                sb.Append("   inner join Cli_Engage__CommunitySchoolRelations csr on csr.SchoolId = ssr.SchoolId")
                    .Append("  left join Cli_Engage__SchoolTypes st on st.id = sch.SchoolTypeId");

            sb.Append(" where 1=1 ")
                .AppendFormat(" and sm.UpdatedOn between '{0}' and '{1}' ", startDate, endDate)
                .AppendFormat(" and s.BirthDate between '{0}' and '{1}' ", dobStartDate, dobEndDate);

            if (communityId > 0)
                sb.AppendFormat(" and csr.CommunityId ={0} ", communityId)
                    .Append(" and SUBSTRING(st.Name,1,4)!='Demo' ");
            else
                sb.AppendFormat(" and ssr.SchoolId = {0} ", schoolId);

            sb.AppendFormat(" and a.id ={0} ", assessmentId)
                .AppendFormat(" and sm.Status > {0} ", (byte)CpallsStatus.Paused)
                .AppendFormat(" and sa.SchoolYear='{0}' ", schoolYear)
                .AppendFormat(" and sa.Wave={0} ", (byte)wave)
                .AppendFormat(" and s.Status = 1 ")
                .AppendFormat(" and s.AssessmentLanguage in ({0},{1}) ", (byte)language, (byte)StudentAssessmentLanguage.Bilingual)
                .AppendFormat(" and m.id in ({0})", string.Join(",", measureIdList))
                .AppendFormat(" and (scr.ClassId in ({0}))", string.Join(",", classIds));
            AdeUnitOfWorkContext context = this.UnitOfWork as AdeUnitOfWorkContext;

            List<CompletionMeasureModel> list = new List<CompletionMeasureModel>();

            try
            {
                context.DbContext.Database.Connection.Open();

                var command = context.DbContext.Database.Connection.CreateCommand();
                command.CommandText = sb.ToString();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new CompletionMeasureModel(reader));
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return list;
        }

        public List<CircleDataExportStudentMeasureModel> GetCircleDataExportStudentMeasureModels(int communityId, string year, int schoolId, List<int> waves, List<int> measures)
        {
            if (waves == null || measures == null || !waves.Any() || !measures.Any()) return null;
            var strSql = string.Format(@"

DECLARE @StudentOfDistrict TABLE(StudentId INT);
	INSERT INTO @StudentOfDistrict 
	SELECT STU.ID FROM [Cli_Engage__CommunitySchoolRelations] CS 
				INNER JOIN [dbo].[Cli_Engage__Schools] SCH ON CS.SchoolID = SCH.ID AND SCH.[Status] = 1
				INNER JOIN [Cli_Engage__SchoolStudentRelations] SS ON SCH.ID = SS.SchoolId  
				INNER JOIN [dbo].[Cli_Engage__Students] STU ON SS.StudentId = STU.ID AND STU.[Status] = 1
				WHERE CS.CommunityId = @DistrictId AND (@SchoolId = 0 OR CS.SchoolId = @SchoolId)

SELECT * FROM DBO.V_StudentMeasureDetail V
WHERE 

EXISTS( SELECT 1 FORM @StudentOfDistrict SOD WHERE SOD.StudentId = V.StudentId) 
AND [SchoolYear] = @SchoolYear
AND [Wave] IN ({0}) 
AND ([MeasureId] IN ({1}) 
    OR EXISTS (SELECT 1 FROM  [Measures] MEA WHERE MEA.ParentId in ({1}) AND MEA.ID = [MeasureId])
)", string.Join(",", waves), string.Join(",", measures));
            var context = this.UnitOfWork as AdeUnitOfWorkContext;
            try
            {
                context.DbContext.Database.Connection.Open();

                var command = context.DbContext.Database.Connection.CreateCommand();
                command.CommandText = strSql;
                command.Parameters.Add(new SqlParameter("CommunityId", communityId));
                command.Parameters.Add(new SqlParameter("SchoolYear", year));
                command.Parameters.Add(new SqlParameter("SchoolId", schoolId));
                command.CommandTimeout = 60 * 60; // 1H
                List<CircleDataExportStudentMeasureModel> list = null;
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (list == null)
                            list = new List<CircleDataExportStudentMeasureModel>();

                        list.Add(new CircleDataExportStudentMeasureModel()
                        {
                            AssessmentId = reader.GetInt32(0),
                            SchoolYear = reader.GetString(1),
                            Wave = (Wave)reader.GetByte(2),
                            StudentId = reader.GetInt32(3),
                            BirthDay = reader.GetDateTime(4),
                            ID = reader.GetInt32(5),
                            MeasureId = reader.GetInt32(6),
                            MeasureName = reader.GetString(7),
                            MeasureShortName = reader.GetString(8),
                            Benchmark = reader.GetDecimal(9),
                            TotalScore = reader.GetDecimal(10),
                            Goal = reader.GetDecimal(11),
                            BenchmarkId = reader.GetInt32(12),
                            LabelText = reader.GetString(13),
                            Color = reader.GetString(14),
                            BW = (BlackWhiteStyle)reader.GetByte(15)
                        });
                    }
                }
                return list;
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

        public List<CircleDataExportStudentMeasureModel> GetCircleDataExportStudentMeasureModelsWithItems(int communityId, string year, int schoolId, List<int> waves, List<int> measures, List<int> measuresIncludeItems, List<ItemType> types, DateTime startDate, DateTime endDate)
        {
            if (waves == null || measures == null || !waves.Any() || !measures.Any()) return null;
            if (measuresIncludeItems == null) measuresIncludeItems = new List<int>();
            measuresIncludeItems.Add(0);

            var strSql = string.Format(@"

DECLARE @StudentOfDistrict TABLE(StudentId INT);
	INSERT INTO @StudentOfDistrict 
	SELECT STU.ID FROM [Cli_Engage__CommunitySchoolRelations] CS 
				INNER JOIN [dbo].[Cli_Engage__Schools] SCH ON CS.SchoolID = SCH.ID AND SCH.[Status] = 1
				INNER JOIN [Cli_Engage__SchoolStudentRelations] SS ON SCH.ID = SS.SchoolId  
				INNER JOIN [dbo].[Cli_Engage__Students] STU ON SS.StudentId = STU.ID 
				WHERE CS.CommunityId = @CommunityId AND (@SchoolId = 0 OR CS.SchoolId = @SchoolId)


SELECT SMD.*, 
[ItemDescription] = SIT.[Description], 
SIT.[Type],
[SIId] = SIT.ID, 
SIT.ItemId, 
[ItemGoal] = SIT.Goal, 
[ItemScore] = SIT.Score, 
[ItemScored] = SIT.Scored,
SIT.IsCorrect,
[ItemPauseTime] = SIT.PauseTime,
SIT.SelectedAnswers
FROM 
 [dbo].[V_StudentMeasureDetail] SMD
 LEFT JOIN [dbo].[V_StudentItemDetail] SIT ON SMD.ID = SIT.SMId AND SIT.Type IN ({2}) 
    AND  EXISTS( SELECT 1 FROM Measures M WHERE SIT.MeasureId = M.Id AND (M.ID IN ({3}) OR M.ParentId IN ({3})) ) 
 WHERE 
 SMD.UpdatedOn between @StartDate and @EndDate
 AND EXISTS( SELECT 1 FROM @StudentOfDistrict SOD WHERE SOD.StudentId = SMD.StudentId) 
 AND SMD.[SchoolYear] = @SchoolYear
 AND SMD.Wave IN ({0})
 AND  (SMD.[MeasureId] IN ({1}) 
    OR EXISTS (SELECT 1 FROM  [Measures] MEA WHERE MEA.ParentId in ({1}) AND MEA.ID = SMD.[MeasureId])
) 
ORDER BY  SMD.ID ASC ",
  string.Join(",", waves),
  string.Join(",", measures),
  string.Join(",", types.Select(x => (int)x)),
  string.Join(",", measuresIncludeItems)
  );
            Decimal? decimalNull = null;
            var context = this.UnitOfWork as AdeUnitOfWorkContext;
            try
            {
                context.DbContext.Database.Connection.Open();

                var command = context.DbContext.Database.Connection.CreateCommand();
                command.CommandText = strSql;
                command.Parameters.Add(new SqlParameter("CommunityId", communityId));
                command.Parameters.Add(new SqlParameter("SchoolYear", year));
                command.Parameters.Add(new SqlParameter("SchoolId", schoolId));
                command.Parameters.Add(new SqlParameter("StartDate", startDate.Date));
                command.Parameters.Add(new SqlParameter("EndDate", endDate.Date));
                command.CommandTimeout = 60 * 60; // 1H
                List<CircleDataExportStudentMeasureModel> list = null;
                CircleDataExportStudentMeasureModel lastMeasure = null;
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (list == null)
                            list = new List<CircleDataExportStudentMeasureModel>();


                        var smId = int.Parse(reader["id"].ToString());
                        if (lastMeasure == null || lastMeasure.ID != smId)
                        {
                            lastMeasure = new CircleDataExportStudentMeasureModel();

                            lastMeasure.AssessmentId = int.Parse(reader["AssessmentId"].ToString());
                            lastMeasure.SchoolYear = reader["SchoolYear"].ToString();
                            lastMeasure.Wave = (Wave)byte.Parse(reader["Wave"].ToString());
                            lastMeasure.StudentId = int.Parse(reader["StudentId"].ToString());
                            lastMeasure.BirthDay = DateTime.Parse(reader["BirthDay"].ToString());
                            lastMeasure.ID = int.Parse(reader["ID"].ToString());
                            lastMeasure.MeasureId = int.Parse(reader["MeasureId"].ToString());
                            lastMeasure.MeasureName = reader["MeasureName"].ToString();
                            lastMeasure.MeasureShortName = reader["MeasureShortName"].ToString();
                            lastMeasure.Benchmark = decimal.Parse(reader["Benchmark"].ToString());
                            lastMeasure.TotalScore = decimal.Parse(reader["TotalScore"].ToString());
                            lastMeasure.Goal = decimal.Parse(reader["Goal"].ToString());
                            lastMeasure.BenchmarkId = int.Parse(reader["BenchmarkId"].ToString());
                            lastMeasure.LabelText = reader["LabelText"].ToString();
                            lastMeasure.Color = reader["Color"].ToString();
                            lastMeasure.BW = reader["BW"].ToString()==""? (BlackWhiteStyle) 0: (BlackWhiteStyle)byte.Parse(reader["BW"].ToString());
                            lastMeasure.UpdatedOn = DateTime.Parse(reader["UpdatedOn"].ToString());
                            list.Add(lastMeasure);
                        }

                        // Item Props start index;


                        // ItemId, Null => No Items
                        if (reader["ItemId"] != DBNull.Value)
                        {
                            var itemModel = new CircleDataExportStudentItemModel();
                            itemModel.AssessmentId = lastMeasure.AssessmentId;
                            itemModel.SchoolYear = lastMeasure.SchoolYear;
                            itemModel.Wave = lastMeasure.Wave;
                            itemModel.StudentId = lastMeasure.StudentId;
                            itemModel.MeasureId = lastMeasure.MeasureId;
                            itemModel.SMId = lastMeasure.ID;
                            itemModel.Description = reader["ItemDescription"] == DBNull.Value ? "" : reader["ItemDescription"].ToString();
                            itemModel.Type = reader["Type"] == DBNull.Value ? ItemType.Direction : (ItemType)byte.Parse(reader["Type"].ToString());
                            itemModel.Id = reader["SIId"] == DBNull.Value ? 0 : int.Parse(reader["SIId"].ToString());
                            itemModel.ItemId = reader["ItemId"] == DBNull.Value ? 0 : int.Parse(reader["ItemId"].ToString());
                            itemModel.Goal = reader["ItemGoal"] == DBNull.Value ? decimalNull : decimal.Parse(reader["ItemGoal"].ToString());
                            itemModel.Score = reader["ItemScore"] == DBNull.Value ? 0 : decimal.Parse(reader["ItemScore"].ToString());
                            itemModel.Scored = reader["ItemScored"] != DBNull.Value && bool.Parse(reader["ItemScored"].ToString());
                            itemModel.IsCorrect = reader["IsCorrect"] != DBNull.Value && bool.Parse(reader["IsCorrect"].ToString());
                            itemModel.PauseTime = reader["ItemPauseTime"] == DBNull.Value ? 0 : int.Parse(reader["ItemPauseTime"].ToString());
                            itemModel.SelectedAnswers = reader["SelectedAnswers"] == DBNull.Value ? "" : reader["SelectedAnswers"].ToString();
                            lastMeasure.Items.Add(itemModel);
                        }
                    }
                }
                return list;
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

        public List<WaveFinishedOnModel> GetWaveFinishedDate(QueryLevel level, int objectId)
        {
            string strSql = "";
            switch (level)
            {
                case QueryLevel.Community:
                    strSql = @"
                    SELECT SA.Wave,FinishedOn = MIN(SM.UpdatedOn) ,CSR.CommunityId
                    FROM [dbo].[StudentAssessments] SA 
                    INNER JOIN [dbo].[StudentMeasures] SM ON SA.ID = SM.SAId 

                    INNER JOIN [dbo].[Cli_Engage__SchoolStudentRelations] SSR ON SA.StudentId = SSR.StudentId 
                    INNER JOIN [dbo].[Cli_Engage__CommunitySchoolRelations] CSR ON SSR.SchoolId = CSR.SchoolId
                    WHERE CSR.CommunityId = @CommunityId 
                    GROUP BY SA.Wave,CSR.CommunityId 
                    ";
                    break;
                case QueryLevel.School:
                    strSql = @"            
                    SELECT SA.Wave,FinishedOn = MIN(SM.UpdatedOn) ,SSR.SchoolId
                    FROM [dbo].[StudentAssessments] SA 
                    INNER JOIN [dbo].[StudentMeasures] SM ON SA.ID = SM.SAId 
                    INNER JOIN [dbo].[Cli_Engage__SchoolStudentRelations] SSR ON SA.StudentId = SSR.StudentId 
					INNER JOIN [dbo].[Cli_Engage__CommunitySchoolRelations] CSR ON SSR.SchoolId = CSR.SchoolId 
                    WHERE CSR.CommunityId = @CommunityId 
                    GROUP BY SA.Wave,SSR.SchoolId";
                    break;
                case QueryLevel.Class:
                    strSql = @"            
                    SELECT SA.Wave,FinishedOn = MIN(SM.UpdatedOn) ,SCR.ClassId,SA.StudentId
                    FROM [dbo].[StudentAssessments] SA 
                    INNER JOIN [dbo].[StudentMeasures] SM ON SA.ID = SM.SAId 
                    INNER JOIN [dbo].[Cli_Engage__StudentClassRelations] SCR ON SCR.StudentId = SA.StudentId 
					INNER JOIN [dbo].[Cli_Engage__Classes] CLA ON SCR.ClassId  = CLA.ID 
					INNER JOIN [dbo].[Cli_Engage__BasicSchools] BSC ON CLA.SchoolId = BSC.ID
                    WHERE BSC.ID = @SchoolId
                    GROUP BY SA.Wave, SCR.ClassId,SA.StudentId";
                    break;
                case QueryLevel.Student:
                    strSql = @"
                    SELECT SA.Wave,FinishedOn = MIN(SM.UpdatedOn) ,SA.StudentId
                    FROM [dbo].[StudentAssessments] SA 
                    INNER JOIN [dbo].[StudentMeasures] SM ON SA.ID = SM.SAId 
					INNER JOIN [dbo].[Cli_Engage__StudentClassRelations] SCR ON SA.StudentId = SCR.StudentId
                    WHERE SCR.ClassId = @ClassId
                    GROUP BY SA.Wave,SA.StudentId ";
                    break;
            }
            var context = this.UnitOfWork as AdeUnitOfWorkContext;
            try
            {
                return context.DbContext.Database.SqlQuery<WaveFinishedOnModel>(strSql.ToString(),
                    new SqlParameter("CommunityId", objectId),
                    new SqlParameter("SchoolId", objectId),
                    new SqlParameter("ClassId", objectId),
                    new SqlParameter("StudentId", objectId)
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
    }
}