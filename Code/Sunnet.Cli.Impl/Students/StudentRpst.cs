using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		Damon
 * Computer:		Damon-PC
 * Domain:			Damon-pc
 * CreatedOn:		2014/8/25 13:42:23
 * Description:		Please input class summary
 * Version History:	Created,2014/8/25 13:42:23
 **************************************************************************/
using Sunnet.Cli.Core;
using Sunnet.Cli.Core.Ade;
using Sunnet.Cli.Core.Reports.Models;
using Sunnet.Cli.Core.Students.Entities;
using Sunnet.Cli.Core.Students.Interfaces;
using Sunnet.Cli.Core.Students.Model;
using Sunnet.Framework;
using Sunnet.Framework.Core.Base;
using Sunnet.Cli.Core.Reports.Entities;
using System.Data;
namespace Sunnet.Cli.Impl.Students
{
    public class StudentRpst : EFRepositoryBase<StudentEntity, Int32>, IStudentRpst
    {
        public StudentRpst(IUnitOfWork unit)
        {
            this.UnitOfWork = unit;
        }

        public List<StudentForCpallsModel> GetCpallsStudentModel(List<int> ids, string sort, string order)
        {
            List<StudentForCpallsModel> list = new List<StudentForCpallsModel>();
            if (ids == null || ids.Count == 0) return list;

            StringBuilder sb = new StringBuilder();
            sb.Append("select s.ID , FirstName , LastName,BirthDate ,s.SchoolId, s.CommunityId ,bsch.Name as schoolName ")
                .Append(" from students s ")
                .Append(" inner join Schools sch on  s.schoolId = sch.id ")
                .Append(" inner join BasicSchools bsch  on sch.BasicSchoolId = bsch.ID ")
                .AppendFormat(" where s.id in({0}) ", string.Join(",", ids))
                .Append(" order by LastName asc ");


            //不能在此Dispose掉context，因为执行过此方法后，还会用到context
            EFUnitOfWorkContext context = this.UnitOfWork as EFUnitOfWorkContext;


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
                            list.Add(new StudentForCpallsModel()
                            {
                                ID = (int)reader["ID"],
                                FirstName = (string)reader["FirstName"],
                                LastName = (string)reader["LastName"],
                                BirthDate = (DateTime)reader["BirthDate"],
                                SchoolId = (int)reader["SchoolId"],
                                SchoolName = (string)reader["SchoolName"],
                                CommunityId = (int)reader["CommunityId"]

                            });
                        }
                    }
                }
            }
            return list;
        }

        public StudentForCpallsModel GetStudentForPlayMeasure(int id)
        {
            StudentForCpallsModel student  = new StudentForCpallsModel();
           

            StringBuilder sb = new StringBuilder();
            sb.Append(
                "select top 1 Students.ID,FirstName,LastName,BirthDate,ParentCode,SchoolStudentRelations.SchoolId ")
                .AppendFormat(
                    " from Students left join SchoolStudentRelations on Students.ID = SchoolStudentRelations.StudentId where Students.ID = {0}  AND Students.Status=1  order by SchoolStudentRelations.ID",
                    id); 
            EFUnitOfWorkContext context = this.UnitOfWork as EFUnitOfWorkContext;
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
                            student.ID = (int) reader["ID"];
                            student.FirstName = (string) reader["FirstName"];
                            student.LastName = (string) reader["LastName"];
                            student.BirthDate = (DateTime) reader["BirthDate"];
                            student.ParentCode = (string)reader["ParentCode"];
                            if (reader["SchoolId"] == null)
                                student.SchoolId = 0;//(int)reader["SchoolId"],
                            else
                            {
                                student.SchoolId = (int) reader["SchoolId"];
                            }
                        }
                    }
                }
            }
            return student;
        }

        #region Beech Report

        public List<BeechReportModel> GetBeechReport(List<int> communityIds, List<int> schoolIds, string teacher)
        {
            List<int> excludeCommunityIds = SFConfig.ExcludedCommunityForReport;
            if (communityIds.Any(e => excludeCommunityIds.Contains(e)))
            {
                excludeCommunityIds = new List<int>();
            }
            string strSql = "exec BeechReport '" + string.Join(",", communityIds) + "','" +
                            string.Join(",", excludeCommunityIds) + "','" +
                            string.Join(",", schoolIds) + "','" + teacher + "'";
            EFUnitOfWorkContext context = this.UnitOfWork as EFUnitOfWorkContext;
            List<BeechReportModel> listBeechReport =
                context.DbContext.Database.SqlQuery<BeechReportModel>(strSql).ToList();
            return listBeechReport;
        }

        #endregion

        #region Media Consent Report
        public List<MediaConsentPercentModel> GetMediaConsentPercent(List<int> communityIds, List<int> schoolIds, string teacher)
        {
            List<int> excludeCommunityIds = SFConfig.ExcludedCommunityForReport;
            if (communityIds.Any(e => excludeCommunityIds.Contains(e)))
            {
                excludeCommunityIds = new List<int>();
            }
            string strSql = "exec MediaConsentPercent '" + string.Join(",", communityIds) + "','" +
                            string.Join(",", excludeCommunityIds) + "','" +
                            string.Join(",", schoolIds) + "','" + teacher + "'";
            EFUnitOfWorkContext context = this.UnitOfWork as EFUnitOfWorkContext;
            List<MediaConsentPercentModel> listMediaConsentPercent =
                context.DbContext.Database.SqlQuery<MediaConsentPercentModel>(strSql).ToList();
            return listMediaConsentPercent;
        }

        public List<MediaConsentDetailModel> GetMediaConsentDetail(List<int> communityIds, List<int> schoolIds, string teacher)
        {
            List<int> excludeCommunityIds = SFConfig.ExcludedCommunityForReport;
            if (communityIds.Any(e => excludeCommunityIds.Contains(e)))
            {
                excludeCommunityIds = new List<int>();
            }
            string strSql = "exec MediaConsentDetail '" + string.Join(",", communityIds) + "','" +
                            string.Join(",", excludeCommunityIds) + "','" +
                            string.Join(",", schoolIds) + "','" + teacher + "'";
            EFUnitOfWorkContext context = this.UnitOfWork as EFUnitOfWorkContext;
            List<MediaConsentDetailModel> listMediaConsentDetail =
                context.DbContext.Database.SqlQuery<MediaConsentDetailModel>(strSql).ToList();
            return listMediaConsentDetail;
        }

        #endregion
    }
}
