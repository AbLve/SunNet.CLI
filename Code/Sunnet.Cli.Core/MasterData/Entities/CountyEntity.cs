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
 * Description:		Create CountyEntity
 * Version History:	Created,2014/8/19 16:27:20
 * 
 * 
 **************************************************************************/
using Sunnet.Cli.Core.Communities.Entities;
using Sunnet.Cli.Core.Schools.Entities;
using Sunnet.Framework.Core.Base;

namespace Sunnet.Cli.Core.MasterData.Entities
{
    public class CountyEntity:EntityBase<int>
    {
        [Required]
        [StringLength(64)]
        public string Name { get; set; }
        public int StateId { get; set; }
        public virtual ICollection<CommunityEntity> Communities { get; set; }
        public virtual ICollection<BasicSchoolEntity> Schools { get; set; }

    }
}
