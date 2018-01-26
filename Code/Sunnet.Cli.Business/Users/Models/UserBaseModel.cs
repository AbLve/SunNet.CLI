using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sunnet.Cli.Core.Users.Enums;

namespace Sunnet.Cli.Business.Users.Models
{
    public class UserBaseModel
    {
        public int UserId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string FullName
        {
            get { return string.Format("{0} {1}", FirstName, LastName); }
        }

        public override string ToString()
        {
            return FullName;
        }

        public Role UserRole { get; set; }

        public IEnumerable<string> CommunityNames { get; set; }

        public string CommunityNameStr
        {
            get
            {
                if (CommunityNames != null)
                {
                    if (CommunityNames.Any())
                    {
                        return String.Join(",", CommunityNames);
                    }
                    else
                    {
                        return "";
                    }
                }
                else
                {
                    return "";
                }
            }
        }
    }
}
