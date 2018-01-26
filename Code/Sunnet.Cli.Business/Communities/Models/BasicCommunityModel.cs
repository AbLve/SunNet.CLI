using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		Leason
 * Computer:		Leason-PC
 * Domain:			Leason-pc
 * CreatedOn:		2014/9/15 11:36:50
 * Description:		Create BasicCommunityModel
 * Version History:	Created,2014/9/15 11:36:50
 * 
 * 
 **************************************************************************/
using Sunnet.Framework.Extensions;

namespace Sunnet.Cli.Business.Communities.Models
{
    public class BasicCommunityModel : SelectItemModel
    {
        public string Type { get; set; }
        public string Address1 { get; set; }
        public string City { get; set; }
        public string Zip { get; set; }
        public string Phone { get; set; }
        public int CountyId { get; set; }
        public int StateId { get; set; }
        public string DistrictNumber { get; set; }
    }
}
