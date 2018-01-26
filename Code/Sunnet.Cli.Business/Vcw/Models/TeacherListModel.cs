using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		Joe
 * Computer:		JOE-PC
 * Domain:			Joe-pc
 * CreatedOn:		2014/10/28 12:15:10
 * Description:		Please input class summary
 * Version History:	Created,2014/10/28 12:15:10
 * 
 * 
 **************************************************************************/
using Sunnet.Cli.Core.Communities.Entities;
using Sunnet.Cli.Core.Schools.Entities;
using Sunnet.Cli.Core.Users.Entities;
using Sunnet.Framework.Extensions;

namespace Sunnet.Cli.Business.Vcw.Models
{
    public class TeacherListModel
    {
        public int TeacherId { get; set; }

        public int TeacherUserId { get; set; }

        public int CoachId { get; set; }

        public string TeacherName { get; set; }

        public string CoachName { get; set; }

        public IEnumerable<SelectItemModel> Schools { get; set; }

        public IEnumerable<string> CommunityNames { get; set; }

        public IEnumerable<string> SchoolNames { get; set; }

        public string CommunityName
        {
            get
            {
                if (CommunityNames != null && CommunityNames.Count() > 0)
                {
                    List<string> community_Sort = CommunityNames.ToList();
                    community_Sort.Sort();
                    return string.Join(", ", community_Sort);
                }
                else
                    return "";
            }
        }

        public string SchoolName
        {
            get
            {
                if (SchoolNames != null && SchoolNames.Count() > 0)
                {
                    List<string> school_Sort = SchoolNames.ToList();
                    school_Sort.Sort();
                    return string.Join(", ", school_Sort);
                }
                else
                    return "";
            }
        }
    }
}
