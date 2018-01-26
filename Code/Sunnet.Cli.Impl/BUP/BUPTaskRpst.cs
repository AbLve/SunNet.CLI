using Sunnet.Cli.Core;
using Sunnet.Cli.Core.BUP;
using Sunnet.Cli.Core.BUP.Entities;
using Sunnet.Cli.Core.BUP.Interfaces;
using Sunnet.Cli.Core.BUP.Models;
using Sunnet.Framework.Core.Base;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Cli.Impl.BUP
{
    public class BUPTaskRpst : EFRepositoryBase<BUPTaskEntity, Int32>, IBUPTaskRpst
    {
        public BUPTaskRpst(IUnitOfWork unit)
        {
            UnitOfWork = unit;
        }

        public string ExecuteCommunitySql(string sql)
        {
            EFUnitOfWorkContext context = this.UnitOfWork as EFUnitOfWorkContext;
            try
            {
                string v = context.DbContext.Database.SqlQuery<string>(sql).FirstOrDefault();
                return v;
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }

        public int ExecuteSqlCommand(string sql)
        {
            EFUnitOfWorkContext context = this.UnitOfWork as EFUnitOfWorkContext;
            try
            { 
                int v = context.DbContext.Database.ExecuteSqlCommand(sql);
                return v;
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }

        public dynamic ExecuteSqlQuery(string sql, BUPType type)
        {
            EFUnitOfWorkContext context = this.UnitOfWork as EFUnitOfWorkContext;
            switch (type)
            {
                case BUPType.Community:
                    return context.DbContext.Database.SqlQuery<BUPCommunityModel>(sql).ToList();
                case BUPType.School:
                    return context.DbContext.Database.SqlQuery<BUPSchoolModel>(sql).ToList();
                case BUPType.Classroom:
                    return context.DbContext.Database.SqlQuery<BUPClassroomModel>(sql).ToList();
                case BUPType.Class:
                    return context.DbContext.Database.SqlQuery<BUPClassModel>(sql).ToList();
                case BUPType.Teacher:
                    return context.DbContext.Database.SqlQuery<BUPTeacherModel>(sql).ToList();
                case BUPType.Student:
                    return context.DbContext.Database.SqlQuery<BUPStudentModel>(sql).ToList();
                case BUPType.CommunityUser:
                case BUPType.CommunitySpecialist:
                    return context.DbContext.Database.SqlQuery<BUPCommunityUserModel>(sql).ToList();
                case BUPType.Principal:
                case BUPType.SchoolSpecialist:
                    return context.DbContext.Database.SqlQuery<BUPPrincipalModel>(sql).ToList();
                case BUPType.Parent:
                    return context.DbContext.Database.SqlQuery<BUPParentModel>(sql).ToList();
                case BUPType.StatewideAgency:
                    return context.DbContext.Database.SqlQuery<BUPStatewideModel>(sql).ToList();
                case BUPType.Auditor:
                    return context.DbContext.Database.SqlQuery<BUPAuditorModel>(sql).ToList();
                default:
                    return "";
            }
        }

        public void Start(int id, int createdBy)
        {
            //不能在此Dispose掉context，因为执行过此方法后，还会用到context
            EFUnitOfWorkContext context = this.UnitOfWork as EFUnitOfWorkContext;

            try
            {
                using (SqlConnection conn = new SqlConnection(context.DbContext.Database.Connection.ConnectionString))
                {
                    if (conn.State != ConnectionState.Open)
                        conn.Open();

                    SqlCommand cmd = new SqlCommand("BUP_Task", conn);
                    SqlParameter para = new SqlParameter("@ID", id);
                    cmd.Parameters.Add(para);
                    para = new SqlParameter("@UpdatedBy", createdBy);
                    cmd.Parameters.Add(para);

                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 1200 * 60;  //Second
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }


            //context.DbContext.Database.ExecuteSqlCommand("exec ImportDataBatch  @ID", parms);
        }
    }
}