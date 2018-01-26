using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/**************************************************************************
 * Developer: 		Sam
 * Computer:		SAM-PC
 * Domain:			sam-pc
 * CreatedOn:		2014/9/13 15:23:19
 * Description:		Please input class summary
 * Version History:	Created,2014/9/13 15:23:19
 * 
 * 
 **************************************************************************/
namespace Sunnet.Cli.Business.Users.Models
{
   public  class UserCommunityRelationModel
    {
       public Int64 ID { get; set; }
       public int UserId { get; set; }
       public int CommunityId { get; set; }
       public string CommunityName { get; set; }
    }
}
