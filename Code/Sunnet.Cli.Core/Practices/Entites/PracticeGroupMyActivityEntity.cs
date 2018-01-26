using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sunnet.Framework.Core.Base;

namespace Sunnet.Cli.Core.Practices.Entites
{
    public class PracticeGroupMyActivityEntity : EntityBase<int>
    {
        public int GroupId { get; set; }
        public int MyActivityId { get; set; }

        [DisplayName("Created By")]
        public int CreatedBy { get; set; }

        [DisplayName("Updated By")]
        public int UpdatedBy { get; set; }
        public virtual PracticeStudentGroupEntity Group { get; set; }

    }
}
