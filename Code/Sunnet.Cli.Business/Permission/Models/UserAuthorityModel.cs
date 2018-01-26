using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		Joe
 * Computer:		JOE-PC
 * Domain:			Joe-pc
 * CreatedOn:		2014/9/2 12:15:10
 * Description:		Please input class summary
 * Version History:	Created,2014/9/2 12:15:10
 * 
 * 
 **************************************************************************/

namespace Sunnet.Cli.Business.Permission.Models
{
    public class UserAuthorityModel
    {
        public int UserId { get; set; }

        public int PageId { get; set; }

        public string Authorities { get; set; }

        public int Authority { get; set; }
    }
}
