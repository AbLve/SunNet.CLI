using System;
using System.Collections.Generic;
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
using Sunnet.Cli.Core.Users.Entities;
using Sunnet.Cli.Core.Users.Interfaces;
using Sunnet.Framework.Core.Base;
using Sunnet.Cli.Core;
using System.Data.SqlClient;
using Sunnet.Cli.Core.Common.Enums;

namespace Sunnet.Cli.Impl.Users
{
    public class UserBaseRpst : EFRepositoryBase<UserBaseEntity, Int32>, IUserBaseRpst
    {
        public UserBaseRpst(IUnitOfWork unit)
        {
            this.UnitOfWork = unit;
        }

        public void UpdateInvitationEmail(int expirationDay, int id)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("update Users set invitationEmail ={0}, EmailExpireTime=dateadd(D,{1},getdate()) where id = {2}", (int)InvitationEmailEnum.Sent, expirationDay, id);

            //不能在此Dispose掉context，因为执行过此方法后，还会用到context
            EFUnitOfWorkContext context = this.UnitOfWork as EFUnitOfWorkContext;
            context.DbContext.Database.ExecuteSqlCommand(sb.ToString());
        }

        public void InsertEmailLog(EmailLogEntity log)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("insert EmailLogs values(@UserID,getdate(),@Email,@EmailType)");

            EFUnitOfWorkContext context = this.UnitOfWork as EFUnitOfWorkContext;
            context.DbContext.Database.ExecuteSqlCommand(sb.ToString(),
                new SqlParameter("UserID", log.UserId),
                new SqlParameter("Email", log.Email),
                new SqlParameter("EmailType", (int)log.EmailType));
        }

        public void ResetEmail(int sentTime)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("Update users set invitationEmail={0} from users u ", (int)InvitationEmailEnum.Pending)
                .AppendFormat(" where status={0} and GoogleID='' and EmailExpireTime < getdate() and invitationEmail={1} "
                , (int)EntityStatus.Active, (int)InvitationEmailEnum.Sent)
                .AppendFormat(" and  (select count(*) from EmailLogs l where  l.Id = u.id and EmailType ={1} ) < {0} ", sentTime, (int)EmailLogType.Batch);

            EFUnitOfWorkContext context = this.UnitOfWork as EFUnitOfWorkContext;
            context.DbContext.Database.ExecuteSqlCommand(sb.ToString());
        }


        public void InsertUserMail(int userId, string email)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("Update users set gmail=@gmail where id = {0} ", userId);

            EFUnitOfWorkContext context = this.UnitOfWork as EFUnitOfWorkContext;
            context.DbContext.Database.ExecuteSqlCommand(sb.ToString(),
                new SqlParameter("gmail", email));
        }

        public long GetUserCode()
        {
             EFUnitOfWorkContext context = this.UnitOfWork as EFUnitOfWorkContext;
             return context.DbContext.Database.SqlQuery<Int64>("select next value for SeqUserCode;").ToList().FirstOrDefault();
        }
    }
}
