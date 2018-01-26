using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		Leason
 * Computer:		Leason-PC
 * Domain:			Leason-pc
 * CreatedOn:		2014/8/31 17:48:20
 * Description:		Create CommunityModel
 * Version History:	Created,2014/8/31 17:48:20
 * 
 * 
 **************************************************************************/
using Sunnet.Cli.Core.Common.Enums;

namespace Sunnet.Cli.Business.Communities.Models
{
    public class CommunityModel
    {
        public int ID { get; set; }
        public string CommunityId { get; set; }
        public string CommunityName { get; set; }
        public string FundingName { get; set; }
        public string DistrictNumber { get; set; }
        public EntityStatus Status { get; set; }
    }
}
