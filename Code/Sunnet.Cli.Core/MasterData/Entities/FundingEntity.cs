using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		Leason
 * Computer:		Leason-PC
 * Domain:			Leason-pc
 * CreatedOn:		2014/8/19 16:27:20
 * Description:		Create FundingRspt
 * Version History:	Created,2014/8/19 16:27:20
 * 
 * 
 **************************************************************************/
using Sunnet.Cli.Core.Classrooms.Entites;
using Sunnet.Cli.Core.Common.Enums;
using Sunnet.Cli.Core.Communities.Entities;
using Sunnet.Cli.Core.Schools.Entities;
using Sunnet.Framework.Core.Base;

namespace Sunnet.Cli.Core.MasterData.Entities
{
    public class FundingEntity : EntityBase<int>
    {
        [Required]
        [StringLength(32)]
        public string Name { get; set; }
        [Required]
        public EntityStatus Status { get; set; }

        public virtual ICollection<CommunityEntity> Communities { get; set; }
        public virtual ICollection<SchoolEntity> Schools { get; set; }
        public virtual ICollection<ClassroomEntity> Classrooms { get; set; }

    }
}
