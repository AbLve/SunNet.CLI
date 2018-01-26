using Sunnet.Framework.Core.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Cli.Core.Users.Entities
{
    /// <summary>
    /// 邮件日志记录
    /// </summary>
    public class EmailLogEntity
    {
        /// <summary>
        /// 构建邮件日志
        /// </summary>
        /// <param name="userId">发送者</param>
        /// <param name="email">邮件接收者</param>
        /// <param name="type">邮件类型</param>
        public EmailLogEntity(int userId,string email, EmailLogType type )
        {
            UserId = userId;
            SendDate = DateTime.Now;
            Email = email;
            EmailType = type;
        }

        /// <summary>
        /// 谁发送的
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// 发送时间
        /// </summary>
        public DateTime SendDate { get; set; }

        /// <summary>
        /// 邮件接收者
        /// </summary>
        public string Email { get; set; }


        public EmailLogType EmailType { get; set; }
    }

    public enum EmailLogType : short
    {
        Invitation = 1,
        Notification = 2,
        Batch = 3,
        Other = 4
    }
}
