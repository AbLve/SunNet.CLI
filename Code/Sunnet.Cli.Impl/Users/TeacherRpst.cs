using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		Sam
 * Computer:		SAM-PC
 * Domain:			sam-pc
 * CreatedOn:		2014/8/5 15:48:47
 * Description:		Please input class summary
 * Version History:	Created,2014/8/5 15:48:47
 * 
 * 
 **************************************************************************/
using MySql.Data.MySqlClient;
using Sunnet.Cli.Core.Users.Entities;
using Sunnet.Cli.Core.Users.Interfaces;
using Sunnet.Framework;
using Sunnet.Framework.Core.Base;
using Sunnet.Cli.Core;
using Sunnet.Cli.Core.Reports.Models;

namespace Sunnet.Cli.Impl.Users
{
    public class TeacherRpst : EFRepositoryBase<TeacherEntity, Int32>, ITeacherRpst
    {
        public TeacherRpst(IUnitOfWork unit)
        {
            this.UnitOfWork = unit;
        }

        public List<Community_Mentor_TeacherModel> GetCommunity_Mentor_Teachers()
        {
            List<Community_Mentor_TeacherModel> list = new List<Community_Mentor_TeacherModel>();
            try
            {
                EFUnitOfWorkContext context = this.UnitOfWork as EFUnitOfWorkContext;
                return context.DbContext.Database.SqlQuery<Community_Mentor_TeacherModel>("exec Community_Mentor_Teachers").ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<PDReportModel> PDCompletionCourseReport(List<int> communityIds, List<int> schoolIds, string teacher, int status)
        {
            List<int> excludeCommunityIds = SFConfig.ExcludedCommunityForReport;
            if (communityIds.Any(e => excludeCommunityIds.Contains(e)))
            {
                excludeCommunityIds = new List<int>();
            }
            string _mysqlconnstring = ConfigurationManager.ConnectionStrings["LMSConnection"].ConnectionString;
            List<PDReportModel> listPD = new List<PDReportModel>();
            MySqlConnection connMySql = new MySqlConnection(_mysqlconnstring);
            try
            {
                string sql = "Call PDCompletionCourseReport('" + string.Join(",", communityIds) + "','" +
                             string.Join(",", excludeCommunityIds) + "','"
                             + string.Join(",", schoolIds) + "','" + teacher + "'," + status + ")";
                connMySql.Open();
                MySqlCommand command = new MySqlCommand(sql, connMySql);
                command.CommandTimeout = 120000;
                MySqlDataReader dr = command.ExecuteReader();
                DateTime? nullTime = null;
                while (dr.Read())
                {
                    listPD.Add(new PDReportModel()
                    {
                        CommunityDistrict = dr["CommunityDistrict"].ToString(),
                        SchoolName = dr["SchoolName"].ToString(),
                        TeacherFirstName = dr["firstname"].ToString(),
                        TeacherLastName = dr["lastname"].ToString(),
                        TeacherID = dr["teacherID"].ToString(),
                        TeacherEmail = dr["email"].ToString(),
                        CircleCourseId = Convert.ToInt32(dr["CircleCourse_id"].ToString()),
                        CircleCourse = dr["CircleCourse"].ToString(),
                        StartDate = dr["startdate"].ToString() == "" ? nullTime : Convert.ToDateTime(dr["startdate"].ToString()),
                        Status = dr["STATUS"].ToString(),
                        TimeSpentInCourse = dr["TimeSpentInCourse"].ToString(),
                        CountofPosts = Convert.ToInt32(dr["CountofPosts"].ToString()),
                        CourseViewed = Convert.ToInt32(dr["CourseViewed"].ToString())
                    });
                }
            }
            finally
            {
                if (connMySql != null && connMySql.State == ConnectionState.Open)
                {
                    connMySql.Close();
                    connMySql = null;
                }
            }
            return listPD;
        }
    }
}
