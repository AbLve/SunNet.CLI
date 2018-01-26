using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Cli.Business.Vcw.Models
{
    public class CoachesListModel
    {
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

        public string CommunityIds { get; set; }

        public int CoachUserId { get; set; }

        public string CoachName { get; set; }

        public IEnumerable<string> CommunityNames { get; set; }

    }


    /// <summary>
    /// 用于查找Coach用户列表
    /// </summary>
    public class CoachListModel
    {
        public int UserId { get; set; }

        public string Name { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }
    }
}
