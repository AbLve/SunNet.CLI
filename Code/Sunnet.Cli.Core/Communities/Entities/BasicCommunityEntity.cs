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
 * CreatedOn:		2014/8/22 17:33:00
 * Description:		Create BasicCommunityCnfg
 * Version History:	Created,2014/8/22 17:33:00
 * 
 * 
 **************************************************************************/
using Sunnet.Cli.Core.Common.Enums;
using Sunnet.Framework.Core.Base;

namespace Sunnet.Cli.Core.Communities.Entities
{
    public class BasicCommunityEntity:EntityBase<int>
    {
        [Required]
        public string Name { get; set; }
        public EntityStatus Status { get; set; }
        public string Type { get; set; }
        public string Address1 { get; set; }
        public string City { get; set; }
        public string Zip { get; set; }
        public string Phone { get; set; }
        public int CountyId { get; set; }
        public int StateId { get; set; }
        public string DistrictNumber { get; set; }

        public ICollection<CommunityEntity> Communities{ get; set; } 
    }
}
