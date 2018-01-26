using Newtonsoft.Json;
using Sunnet.Framework.Core.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Cli.Core.Users.Entities
{
    public class TeacherAgeGroupEntity : EntityBase<int>
    {
        [Required]
        public int TeacherId { get; set; }

        [Required]
        public int AgeGroup { get; set; }

        public string AgeGroupOther { get; set; }

        public virtual TeacherEntity Teacher { get; set; }
    }
}
