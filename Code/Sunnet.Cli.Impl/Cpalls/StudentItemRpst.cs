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
 * CreatedOn:		2014/9/4 4:22:15
 * Description:		Please input class summary
 * Version History:	Created,2014/9/4 4:22:15
 * 
 * 
 **************************************************************************/
using Sunnet.Cli.Core;
using Sunnet.Cli.Core.Ade;
using Sunnet.Cli.Core.Cpalls;
using Sunnet.Cli.Core.Cpalls.Entities;
using Sunnet.Cli.Core.Cpalls.Interfaces;
using Sunnet.Cli.Core.Cpalls.Models.Report;
using Sunnet.Cli.Core.Reports.Models;
using Sunnet.Framework.Core.Base;

namespace Sunnet.Cli.Impl.Cpalls
{
    public class StudentItemRpst : EFRepositoryBase<StudentItemEntity, int>, IStudentItemRpst
    {
        public StudentItemRpst(IUnitOfWork unit)
        {
            this.UnitOfWork = unit;
        }

        public List<CircleDataExportStudentItemModel> GetCircleDataExportStudentItemModels(int communityId, string year, int schoolId, List<int> waves, List<int> measures, List<ItemType> types)
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

SELECT * FROM DBO.V_StudentItemDetail V
WHERE 
EXISTS( SELECT 1 FORM @StudentOfDistrict SOD WHERE SOD.StudentId = V.StudentId)
AND [SchoolYear] = @SchoolYear
AND [Wave] IN ({0}) 
AND ( [MeasureId] IN ({1}) 
    OR EXISTS (SELECT 1 FROM  [Measures] MEA WHERE MEA.ParentId in ({1}) AND MEA.ID = [MeasureId])
) 
AND [Type] IN ({2})", string.Join(",", waves), string.Join(",", measures), string.Join(",", types.Select(x => (byte)x)));
            var context = this.UnitOfWork as AdeUnitOfWorkContext;

            Decimal? decimalNull = null;
            try
            {
                context.DbContext.Database.Connection.Open();

                var command = context.DbContext.Database.Connection.CreateCommand();
                command.CommandText = strSql;
                command.Parameters.Add(new SqlParameter("CommunityId", communityId));
                command.Parameters.Add(new SqlParameter("SchoolYear", year));
                command.Parameters.Add(new SqlParameter("SchoolId", schoolId));
                command.CommandTimeout = 60 * 60; // 1H
                List<CircleDataExportStudentItemModel> list = null;
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (list == null)
                            list = new List<CircleDataExportStudentItemModel>();

                        list.Add(new CircleDataExportStudentItemModel()
                        {
                            AssessmentId = reader.GetInt32(0),
                            SchoolYear = reader.GetString(1),
                            Wave = (Wave)reader.GetByte(2),
                            CDId = reader.GetInt32(3),
                            SchoolId = reader.GetInt32(4),
                            StudentId = reader.GetInt32(5),
                            MeasureId = reader.GetInt32(6),
                            SMId = reader.GetInt32(7),
                            Description = reader.GetString(8),
                            Type = (ItemType)reader.GetByte(9),
                            Id = reader.GetInt32(10),
                            ItemId = reader.GetInt32(11),
                            Goal = reader["Goal"]== null? decimalNull : decimal.Parse(reader["ItemGoal"].ToString()),
                            Score = reader.GetDecimal(13),
                            Scored = reader.GetBoolean(14),
                            IsCorrect = reader.GetBoolean(15),
                            PauseTime = reader.GetInt32(16),
                            SelectedAnswers = reader.GetString(17)
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
            //return context.DbContext.Database.SqlQuery<CircleDataExportStudentItemModel>(strSql
            //    , new SqlParameter("CommunityId", communityId)
            //    , new SqlParameter("SchoolYear", year)
            //    , new SqlParameter("SchoolId", schoolId)
            //    ).ToList();
        }
    }
}
