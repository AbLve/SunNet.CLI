using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		Sam
 * Computer:		SAM-PC
 * Domain:			sam-pc
 * CreatedOn:		2014/8/5 16:10:18
 * Description:		Please input class summary
 * Version History:	Created,2014/8/5 16:10:18
 * 
 * 
 **************************************************************************/
using Sunnet.Framework.Core.Base;
using System.ComponentModel.DataAnnotations;
using Sunnet.Cli.Core.Users.Enums;
using System.ComponentModel;
using Sunnet.Framework.Mvc;
using Sunnet.Cli.Core.Students.Entities;
using Sunnet.Cli.Core.MasterData.Entities;

namespace Sunnet.Cli.Core.Users.Entities
{
    public class ParentEntity : EntityBase<int>
    {
        public ParentEntity()
        {
            CountryId = 0;
            StateId = 0;
        }
        [EensureEmptyIfNullAttribute]
        [StringLength(50)]
        [DisplayName("Parent Engage ID")]
        public string ParentId { get; set; }

        [StringLength(5)]
        [EensureEmptyIfNullAttribute]
        [DisplayName("School Year")]
        public string SchoolYear { get; set; }

        [EensureEmptyIfNullAttribute]
        [StringLength(600)]
        [DisplayName("Parent/Guardian Notes")]
        public string ParentNotes { get; set; }

        [EensureEmptyIfNullAttribute]
        [DisplayName("Status")]
        public ParentStatus ParentStatus { get; set; }

        public int CountryId { get; set; }
        public int StateId { get; set; }

        [Required]
        public virtual UserBaseEntity UserInfo { get; set; }

        [EensureEmptyIfNullAttribute]
        public string SettingIds { get; set; }

        [EensureEmptyIfNullAttribute]
        public string OtherSetting { get; set; }

        public virtual ICollection<ParentStudentRelationEntity> ParentStudents { get; set; }

        public virtual ICollection<ParentChildEntity> ParentChilds { get; set; }
    }

    public enum ParentStatus : byte
    {
        Active = 1,

        Invited = 2,

        [Description("Not applicable")]
        NotApplicable = 3
    }
}
