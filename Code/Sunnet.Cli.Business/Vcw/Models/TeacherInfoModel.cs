using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		Joe
 * Computer:		JOE-PC
 * Domain:			Joe-pc
 * CreatedOn:		2015/3/5 12:15:10
 * Description:		Please input class summary
 * Version History:	Created,2015/3/5 12:15:10
 * 
 * 
 **************************************************************************/

namespace Sunnet.Cli.Business.Vcw.Models
{
    public partial class TeacherInfoModel
    {
        public int UserId { get; set; }

        public int CommunityId { get; set; }

        public string CommunityName { get; set; }

        public int SchoolId { get; set; }

        public string SchoolName { get; set; }

        public string CoachName { get; set; }

        public string TeacherName { get; set; }

    }
}
