using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sunnet.Cli.Core.Classrooms.Entites;
using Sunnet.Cli.Core.Common.Enums;
using Sunnet.Framework.Core.Base;
namespace Sunnet.Cli.Core.Classes.Entites
{
    public class ClassroomClassEntity : EntityBase<int>
    {
        public int ClassId { get; set; }

        public int ClassroomId { get; set; }

        public EntityStatus Status { get; set; }

        public int CreatedBy { get; set; }

        public int UpdatedBy { get; set; }

        public virtual ClassroomEntity Classroom { get; set; }

        public virtual ClassEntity Class { get; set; }
    }
}
