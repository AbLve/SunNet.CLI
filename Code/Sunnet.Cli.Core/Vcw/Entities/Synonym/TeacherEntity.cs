using System.ComponentModel.DataAnnotations;
using Sunnet.Framework.Core.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Cli.Core.Vcw.Entities
{
    /// <summary>
    /// 部分只读  CREATE SYNONYM Cli_Engage__Teachers FOR [主库名称].[dbo].[Teachers]
    /// </summary>
    public class V_TeacherEntity : EntityBase<int>
    {
        public int CommunityId { get; set; }
        public int SchoolId { get; set; }
        public int CoachId { get; set; }

        [Required]
        public virtual V_UserEntity UserInfo { get; set; }

    }
}
