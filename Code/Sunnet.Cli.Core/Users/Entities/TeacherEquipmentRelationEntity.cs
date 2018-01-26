using Sunnet.Framework.Core.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Cli.Core.Users.Entities
{
    public class TeacherEquipmentRelationEntity : EntityBase<int>
    {
        public int TeacherId { get; set; }

        public int EquipmentId { get; set; }

        public string SerialNumber { get; set; }

        public string UTHealthTag { get; set; }

        public virtual TeacherEntity Teacher { get; set; }

    }
}
