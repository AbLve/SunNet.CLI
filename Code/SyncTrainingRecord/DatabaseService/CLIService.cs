using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SyncTrainingRecord.Entity;

namespace SyncTrainingRecord.DatabaseService
{
    public class CLIService
    {
        private readonly string connString = ConfigurationManager.ConnectionStrings["CLIDbContext"].ToString();

        public List<V_BI_TeacherEntity> GetTeachers(List<string> engageIds)
        {
            string engageIdStrings = "";
            foreach (var engageId in engageIds)
            {
                engageIdStrings = engageIdStrings + "'" + engageId + "'" + ",";
            }
            engageIdStrings = string.IsNullOrEmpty(engageIdStrings) ? "''" : engageIdStrings.TrimEnd(',');
            //string ids = string.Join(",", userIds);
            using (SqlConnection conn = new SqlConnection())
            {
                conn.ConnectionString = connString;
                conn.Open();

                string sql =
                    "select U.[ID], U.[Status],[PrimaryEmailAddress],[SecondaryEmailAddress],[FirstName],[LastName],[PreviousLastName]," +
                    "[MiddleName],[City], T.StateId,[Zip],[CountyId],[PrimaryPhoneNumber],[PrimaryNumberType]," +
                    "[SecondaryPhoneNumber],[SecondaryNumberType],[FaxNumber],[PrimaryLanguageId],[PrimaryLanguageOther]," +
                    "[SecondaryLanguageId],[SecondaryLanguageOther],[GoogleId],[Role],[gmail],[HomeMailingAddress],States.Name as StateName," +
                    "Counties.Name as CountyName,[BirthDate],[Gender],T.TeacherId as EngageId " +
                    "from Users U " +
                    "inner join Teachers T on U.ID=T.UserId " +
                    "LEFT JOIN States ON T.StateId = States.ID " +
                    "LEFT JOIN Counties ON T.CountyId = Counties.ID " +
                    "where U.Role= 145 and T.TeacherId in(" + engageIdStrings + ")";
                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataReader dataReader = cmd.ExecuteReader();
                List<V_BI_TeacherEntity> teachers = HelperFillEntity<V_BI_TeacherEntity>.FillEntityList(dataReader);

                conn.Close();
                return teachers;
            }
        }

        public List<V_BI_PrincipalEntity> GetPrincipals(List<string> engageIds)
        {
            string engageIdStrings = "";
            foreach (var engageId in engageIds)
            {
                engageIdStrings = engageIdStrings + "'" + engageId + "'" + ",";
            }
            engageIdStrings = string.IsNullOrEmpty(engageIdStrings) ? "''" : engageIdStrings.TrimEnd(',');
            //string ids = string.Join(",", userIds);
            using (SqlConnection conn = new SqlConnection())
            {
                conn.ConnectionString = connString;
                conn.Open();

                String sql =
                    "select U.[ID], [PrimaryEmailAddress],[SecondaryEmailAddress],[FirstName],[MiddleName],[LastName],[PreviousLastName],[Status]," +
                    "[GoogleId],[PrimaryPhoneNumber],[PrimaryNumberType],[SecondaryPhoneNumber],[SecondaryNumberType],[FaxNumber],[Address]," +
                    "[City], P.[CountyId],P.[StateId],[Zip],[PrimaryLanguageId],[PrimaryLanguageOther],States.Name as [StateName]," +
                    "Counties.Name as [CountyName],[BirthDate],[Gender],P.PrincipalId as EngageId " +
                    "from Users U " +
                    "inner join Principals P on U.ID=P.UserId " +
                    "LEFT JOIN States ON P.StateId = States.ID " +
                    "LEFT JOIN Counties ON P.CountyId = Counties.ID " +
                    "where U.Role= 125 and P.PrincipalId in(" + engageIdStrings + ")";
                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataReader dataReader = cmd.ExecuteReader();
                List<V_BI_PrincipalEntity> principals = HelperFillEntity<V_BI_PrincipalEntity>.FillEntityList(dataReader);

                conn.Close();
                return principals;
            }
        }
    }
}
