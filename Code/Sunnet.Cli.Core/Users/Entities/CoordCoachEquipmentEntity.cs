using Sunnet.Framework.Core.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Cli.Core.Users.Entities
{
    public class CoordCoachEquipmentEntity : EntityBase<int>
    {
        public int CoordCoachId { get; set; }

        public int EquipmentId { get; set; }

        public string SerialNumber { get; set; }

        public string UTHealthTag { get; set; }

        public virtual CoordCoachEntity CoordCoach { get; set; }
    }
}
