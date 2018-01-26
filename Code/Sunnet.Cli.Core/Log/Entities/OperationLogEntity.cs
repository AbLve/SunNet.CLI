using Sunnet.Framework.Core.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Cli.Core.Log.Entities
{
    public class OperationLogEntity : EntityBase<Int64>
    {
        public OperationLogEntity()
        {

        }
        public OperationLogEntity(string ip, string email, int userId, string description = "login")
        {
            IP = ip;
            Model = "Login";
            CreatedBy = userId;
            Operation = OperationEnum.Login;
            Email = email;
            Description = description;
        }

        /// <summary>
        /// 日志类型.
        /// </summary>
        public string Model { get; set; }

        public OperationEnum Operation { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }

        public int CreatedBy { get; set; }

        public string IP { get; set; }

        public string Email { get; set; }
    }

    public enum OperationEnum : byte
    {
        Insert = 1,
        Update = 2,
        Delete = 3,
        Login = 4,
        StatusChanged = 5,
        ResetSendInvitation = 6,
    }
}
