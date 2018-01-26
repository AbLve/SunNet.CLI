using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Diagnostics.PerformanceData;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SyncTrainingRecord.Entity;

namespace SyncTrainingRecord.DatabaseService
{
    public class TECPDSService
    {
        private readonly string connString = ConfigurationManager.ConnectionStrings["TECPDSDbContext"].ToString();
        private readonly string validatorName = "Engage";

        #region User Method
        public int InsertUser(TecpdsUserEntity tecpdsUser)
        {
            using (SqlConnection conn = new SqlConnection())
            {
                conn.ConnectionString = connString;
                conn.Open();

                string sql =
                    "insert into Users values(@CreatedDate,@Status,@IsDeleted,@Title,@FirstName,@MiddleInitial,@LastName,@PreviousLastName,@BirthDate," +
                    "@Gender,@HomeMailingAddress,@City,@State,@ZipCode,@County,@PrimaryPhoneNumber,@PrimaryNumberType,@SecondaryPhoneNumber," +
                    "@SecondaryNumberType,@FaxNumber,@WebAddress,@PrimaryEmailAddress,@SecondaryEmailAddress,@RacialEthnicBackground," +
                    "@PrimaryLanguage,@SecondaryLanguage,@ApplicationDate,@LastPaymentDate,@Level,@RenewalDate,@ActiveDate,@Role,@Comments,@Education," +
                    "@WorkExperience,@RegisteSteps,@Scoresheet,@TrainingLog);select @@identity";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("CreatedDate", tecpdsUser.CreatedDate);
                cmd.Parameters.AddWithValue("Status", tecpdsUser.Status);
                cmd.Parameters.AddWithValue("IsDeleted", tecpdsUser.IsDeleted);
                cmd.Parameters.AddWithValue("Title", tecpdsUser.Title);
                cmd.Parameters.AddWithValue("FirstName", tecpdsUser.FirstName);
                cmd.Parameters.AddWithValue("MiddleInitial", tecpdsUser.MiddleInitial);
                cmd.Parameters.AddWithValue("LastName", tecpdsUser.LastName);
                cmd.Parameters.AddWithValue("PreviousLastName", tecpdsUser.PreviousLastName);
                cmd.Parameters.AddWithValue("BirthDate", tecpdsUser.BirthDate);

                cmd.Parameters.AddWithValue("Gender", tecpdsUser.Gender);
                cmd.Parameters.AddWithValue("HomeMailingAddress", tecpdsUser.HomeMailingAddress);
                cmd.Parameters.AddWithValue("City", tecpdsUser.City);
                cmd.Parameters.AddWithValue("State", tecpdsUser.State);
                cmd.Parameters.AddWithValue("ZipCode", tecpdsUser.ZipCode);
                cmd.Parameters.AddWithValue("County", tecpdsUser.County);
                cmd.Parameters.AddWithValue("PrimaryPhoneNumber", tecpdsUser.PrimaryPhoneNumber);
                cmd.Parameters.AddWithValue("PrimaryNumberType", tecpdsUser.PrimaryNumberType);
                cmd.Parameters.AddWithValue("SecondaryPhoneNumber", tecpdsUser.SecondaryPhoneNumber);

                cmd.Parameters.AddWithValue("SecondaryNumberType", tecpdsUser.SecondaryNumberType);
                cmd.Parameters.AddWithValue("FaxNumber", tecpdsUser.FaxNumber);
                cmd.Parameters.AddWithValue("WebAddress", tecpdsUser.WebAddress);
                cmd.Parameters.AddWithValue("PrimaryEmailAddress", tecpdsUser.PrimaryEmailAddress);
                cmd.Parameters.AddWithValue("SecondaryEmailAddress", tecpdsUser.SecondaryEmailAddress);
                cmd.Parameters.AddWithValue("RacialEthnicBackground", tecpdsUser.RacialEthnicBackground);

                cmd.Parameters.AddWithValue("PrimaryLanguage", tecpdsUser.PrimaryLanguage);
                cmd.Parameters.AddWithValue("SecondaryLanguage", tecpdsUser.SecondaryLanguage);
                cmd.Parameters.AddWithValue("ApplicationDate", tecpdsUser.ApplicationDate);
                cmd.Parameters.AddWithValue("LastPaymentDate", tecpdsUser.LastPaymentDate);
                cmd.Parameters.AddWithValue("Level", tecpdsUser.Level);
                cmd.Parameters.AddWithValue("RenewalDate", tecpdsUser.RenewalDate);
                cmd.Parameters.AddWithValue("ActiveDate", tecpdsUser.ActiveDate);
                cmd.Parameters.AddWithValue("Role", tecpdsUser.Role);
                cmd.Parameters.AddWithValue("Comments", tecpdsUser.Comments);
                cmd.Parameters.AddWithValue("Education", tecpdsUser.Education);

                cmd.Parameters.AddWithValue("WorkExperience", tecpdsUser.WorkExperience);
                cmd.Parameters.AddWithValue("RegisteSteps", tecpdsUser.RegisteSteps);
                cmd.Parameters.AddWithValue("Scoresheet", tecpdsUser.Scoresheet);
                cmd.Parameters.AddWithValue("TrainingLog", tecpdsUser.TrainingLog);

                int count = Convert.ToInt32(cmd.ExecuteScalar());
                conn.Close();
                return count;
            }
        }
        #endregion

        #region Account Method
        public AccountEntity GetAccount(string googleId)
        {
            using (SqlConnection conn = new SqlConnection())
            {
                conn.ConnectionString = connString;
                conn.Open();

                string sql =
                    "select ID,TrainerID,PractitionerID,CenteDirectorID,GoogleID,CreatedDate from Accounts where GoogleId ='" +
                    googleId + "'";
                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataReader dataReader = cmd.ExecuteReader();
                AccountEntity account = HelperFillEntity<AccountEntity>.FillEntity(dataReader);

                conn.Close();
                return account;
            }
        }

        public int InsertAccount(AccountEntity account)
        {
            using (SqlConnection conn = new SqlConnection())
            {
                conn.ConnectionString = connString;
                conn.Open();

                string sql =
                    "insert into Accounts values(@TrainerID,@PractitionerID,@CenteDirectorID,@GoogleID,@CreatedDate,@LastLoginDate,@Status,@LoginAccount," +
                    "@LoginPassword,@DefaultRole);select @@identity";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("TrainerID", account.TrainerID);
                cmd.Parameters.AddWithValue("PractitionerID", account.PractitionerID);
                cmd.Parameters.AddWithValue("CenteDirectorID", account.CenteDirectorID);
                cmd.Parameters.AddWithValue("GoogleID", account.GoogleID);
                cmd.Parameters.AddWithValue("CreatedDate", DateTime.Now);
                cmd.Parameters.AddWithValue("LastLoginDate", DateTime.Now);
                cmd.Parameters.AddWithValue("Status", 1);
                cmd.Parameters.AddWithValue("LoginAccount", "");
                cmd.Parameters.AddWithValue("LoginPassword", "");
                cmd.Parameters.AddWithValue("DefaultRole", account.CenteDirectorID > 0 ? 2 : 4);

                int count = Convert.ToInt32(cmd.ExecuteScalar());
                conn.Close();
                return count;
            }
        }

        public int UpdateAccount(AccountEntity account)
        {
            using (SqlConnection conn = new SqlConnection())
            {
                conn.ConnectionString = connString;
                conn.Open();

                string sql =
                    "update Accounts set PractitionerID=@PractitionerID,CenteDirectorID=@CenteDirectorID where ID=@ID";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("PractitionerID", account.PractitionerID);
                cmd.Parameters.AddWithValue("CenteDirectorID", account.CenteDirectorID);
                cmd.Parameters.AddWithValue("ID", account.ID);

                int count = cmd.ExecuteNonQuery();
                conn.Close();
                return count;
            }
        }
        #endregion

        #region Training Attend Method
        public int InsertTrainingAttend(TrainingAttendedEntity trainingAttendEntity)
        {
            using (SqlConnection conn = new SqlConnection())
            {
                conn.ConnectionString = connString;
                conn.Open();
                string sql =
                    "insert into TrainingAttendeds values(@UserID,@CompletionDate,@TrainingTitle,@CoreCompetencyArea,@TrainerID,@RegisteredTrainerName," +
                    "@TrainerName,@ClockHours,@TrainingMethod,@UploadCertificate,@CreatedDate,@Role,@TrainingFor,@Attendees," +
                    "@ISPresented,@IsValid,@ValidatorId,@ValidTime);select @@identity";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("UserID", trainingAttendEntity.UserID);
                cmd.Parameters.AddWithValue("CompletionDate", trainingAttendEntity.CompletionDate);
                cmd.Parameters.AddWithValue("TrainingTitle", trainingAttendEntity.TrainingTitle);
                cmd.Parameters.AddWithValue("CoreCompetencyArea", trainingAttendEntity.CoreCompetencyArea);
                cmd.Parameters.AddWithValue("TrainerID", trainingAttendEntity.TrainerID);
                cmd.Parameters.AddWithValue("RegisteredTrainerName", trainingAttendEntity.RegisteredTrainerName);
                cmd.Parameters.AddWithValue("TrainerName", trainingAttendEntity.TrainerName);
                cmd.Parameters.AddWithValue("ClockHours", trainingAttendEntity.ClockHours);
                cmd.Parameters.AddWithValue("TrainingMethod", trainingAttendEntity.TrainingMethod);
                cmd.Parameters.AddWithValue("UploadCertificate", trainingAttendEntity.UploadCertificate);
                cmd.Parameters.AddWithValue("CreatedDate", trainingAttendEntity.CreatedDate);
                cmd.Parameters.AddWithValue("Role", trainingAttendEntity.Role);
                cmd.Parameters.AddWithValue("TrainingFor", trainingAttendEntity.TrainingFor);
                cmd.Parameters.AddWithValue("Attendees", trainingAttendEntity.Attendees);
                cmd.Parameters.AddWithValue("ISPresented", trainingAttendEntity.ISPresented);
                cmd.Parameters.AddWithValue("IsValid", trainingAttendEntity.IsValid);
                cmd.Parameters.AddWithValue("ValidatorId", trainingAttendEntity.ValidatorId);
                cmd.Parameters.AddWithValue("ValidTime", trainingAttendEntity.ValidTime);

                int count = Convert.ToInt32(cmd.ExecuteScalar());
                conn.Close();
                return count;
            }
        }
       

        public int InsertTrainingAttendCore(TrainingAttendedCoreEntity trainingAttendedCore)
        {
            using (SqlConnection conn = new SqlConnection())
            {
                conn.ConnectionString = connString;
                conn.Open();
                string sql =
                    "insert into TrainingAttendedCore values(@ParentID,@CoreCompetencyAreaID,@Type)";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("ParentID", trainingAttendedCore.ParentID);
                cmd.Parameters.AddWithValue("CoreCompetencyAreaID", trainingAttendedCore.CoreCompetencyAreaID);
                cmd.Parameters.AddWithValue("Type", trainingAttendedCore.Type);

                int count = cmd.ExecuteNonQuery();
                conn.Close();
                return count;
            }
        }

        public List<CoreCompetencyAreaEntity> GetAllCoreCompetencyAreas()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection())
                {
                    conn.ConnectionString = connString;
                    conn.Open();

                    String sql =
                        "select ID,Name from CoreCompetencyAreaEnum";
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    SqlDataReader dataReader = cmd.ExecuteReader();
                    List<CoreCompetencyAreaEntity> coreCompetencyAreas = HelperFillEntity<CoreCompetencyAreaEntity>.FillEntityList(dataReader);
                    conn.Close();
                    return coreCompetencyAreas;
                }
            }
            catch (Exception ex)
            {
                Config.Instance.Logger.Debug(ex);
                return new List<CoreCompetencyAreaEntity>();
            }
        }

        public List<CoreCompetencyAreaEntity> GetCoreCompetencyAreas(List<string> coreCompetencyAreaNames,int role)
        {
            try
            {
                string coreCompetencyAreaNameStrings = "";
                foreach (var name in coreCompetencyAreaNames)
                {
                    coreCompetencyAreaNameStrings = coreCompetencyAreaNameStrings + "'" + name.Trim() + "'" + ",";
                }
                coreCompetencyAreaNameStrings = string.IsNullOrEmpty(coreCompetencyAreaNameStrings) ? "''" : coreCompetencyAreaNameStrings.TrimEnd(',');
                using (SqlConnection conn = new SqlConnection())
                {
                    conn.ConnectionString = connString;
                    conn.Open();

                    String sql =
                        "select ID,Name from CoreCompetencyAreaEnum where [for]=@Role and Name in (" + coreCompetencyAreaNameStrings + ")";
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("Role", role);
                    SqlDataReader dataReader = cmd.ExecuteReader();
                    List<CoreCompetencyAreaEntity> coreCompetencyAreas = HelperFillEntity<CoreCompetencyAreaEntity>.FillEntityList(dataReader);
                    conn.Close();
                    return coreCompetencyAreas;
                }
            }
            catch (Exception ex)
            {
                Config.Instance.Logger.Debug(ex);
                return new List<CoreCompetencyAreaEntity>();
            }
        }
        #endregion

        #region Validator Method
        public int GetValidateId()
        {
            using (SqlConnection conn = new SqlConnection())
            {
                conn.ConnectionString = connString;
                conn.Open();

                String sql =
                    "select top 1 ID from Validators where FirstName='" + validatorName + "'";
                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataReader dataReader = cmd.ExecuteReader();
                int validatorId = 0;
                while (dataReader.Read())
                {
                    validatorId = Convert.ToInt32(dataReader["ID"]);
                }
                conn.Close();
                return validatorId;
            }
        }

        public int InsertValidator(ValidatorEntity validator)
        {
            using (SqlConnection conn = new SqlConnection())
            {
                conn.ConnectionString = connString;
                conn.Open();
                string sql =
                    "insert into Validators values(@FirstName,@LastName,@GoogleGmail,@Status,@Comments,@Scoresheet,@CreatedDate,@UpdatedDate,@LastLoginDate);select @@identity";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add(new SqlParameter("FirstName", validator.FirstName));
                cmd.Parameters.Add(new SqlParameter("LastName", validator.LastName));
                cmd.Parameters.Add(new SqlParameter("GoogleGmail", validator.GoogleGmail));
                cmd.Parameters.Add(new SqlParameter("Status", validator.Status));
                cmd.Parameters.Add(new SqlParameter("Comments", validator.Comments));
                cmd.Parameters.Add(new SqlParameter("Scoresheet", validator.Scoresheet));
                cmd.Parameters.Add(new SqlParameter("CreatedDate", validator.CreatedDate));
                cmd.Parameters.Add(new SqlParameter("UpdatedDate", validator.UpdatedDate));
                cmd.Parameters.Add(new SqlParameter("LastLoginDate", validator.LastLoginDate));
                int count = Convert.ToInt32(cmd.ExecuteScalar());
                conn.Close();
                return count;
            }
        }
        #endregion

        #region State&County

        public List<StateEntity> GetStates(List<string> stateNames)
        {
            string stateNameStrings = "";
            foreach (var stateName in stateNames)
            {
                stateNameStrings = stateNameStrings + "'" + stateName + "'" + ",";
            }
            stateNameStrings = string.IsNullOrEmpty(stateNameStrings) ? "''" : stateNameStrings.TrimEnd(',');
            using (SqlConnection conn = new SqlConnection())
            {
                conn.ConnectionString = connString;
                conn.Open();

                String sql =
                    "select ID,Name from StateEnum where Name in (" + stateNameStrings + ")";
                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataReader dataReader = cmd.ExecuteReader();
                List<StateEntity> states = HelperFillEntity<StateEntity>.FillEntityList(dataReader);
                conn.Close();
                return states;
            }
        }

        public List<CountyEntity> GetCounties(List<string> countyNames)
        {
            string countyNameStrings = "";
            foreach (var countyName in countyNames)
            {
                countyNameStrings = countyNameStrings + "'" + countyName + "'" + ",";
            }
            countyNameStrings = string.IsNullOrEmpty(countyNameStrings) ? "''" : countyNameStrings.TrimEnd(',');
            using (SqlConnection conn = new SqlConnection())
            {
                conn.ConnectionString = connString;
                conn.Open();

                String sql =
                    "select ID,Name from CountyEnum where Name in (" + countyNameStrings + ")";
                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataReader dataReader = cmd.ExecuteReader();
                List<CountyEntity> counties = HelperFillEntity<CountyEntity>.FillEntityList(dataReader);
                conn.Close();
                return counties;
            }
        }
        #endregion
    }
}
