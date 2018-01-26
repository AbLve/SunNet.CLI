using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Sunnet.Framework.Core.Base;
using Sunnet.Cli.Core.Common.Enums;

namespace Sunnet.Cli.Core.Vcw.Entities
{
    /// <summary>
    /// 部分只读  CREATE SYNONYM Cli_Engage__Users FOR [主库名称].[dbo].[Users]
    /// </summary>
    public class V_UserEntity: EntityBase<int>
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public EntityStatus Status { get; set; }

        public virtual V_TeacherEntity TeacherInfo { get; set; }

        public virtual ICollection<AssignmentEntity> Assignments { get; set; }

        public virtual ICollection<Vcw_FileEntity> Files { get; set; }
    }
}
