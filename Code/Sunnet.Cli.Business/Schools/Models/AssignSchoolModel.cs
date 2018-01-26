using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		Sam
 * Computer:		SAM-PC
 * Domain:			sam-pc
 * CreatedOn:		2014/8/28 18:26:10
 * Description:		Please input class summary
 * Version History:	Created,2014/8/28 18:26:10
 * 
 * 
 **************************************************************************/
using Newtonsoft.Json;
using Sunnet.Cli.Business.Common;
using Sunnet.Cli.Core.Common.Enums;
using Sunnet.Cli.Core.Schools.Entities;
using Sunnet.Cli.Core.Schools.Enums;
using Sunnet.Cli.Core.Trs;
using Sunnet.Cli.Core.Users.Entities;
using Sunnet.Framework.Mvc;

namespace Sunnet.Cli.Business.Schools.Models
{
    public class AssignSchoolModel
    {
        public int ID { get; set; }

        public int SchoolId { get; set; }

        [DisplayName("School Name")]
        public string SchoolName { get; set; }

        public int CommunityId { get; set; }

        [DisplayName("Communities ")]
        public string CommunityName { get; set; }

        public DateTime CreatedOn { get; set; }

        [DisplayName("Communities ")]
        [JsonIgnore]
        public IEnumerable<string> CommunityNames { get; set; }

        public string CommunityNameText
        {
            get
            {
                if (CommunityNames != null && CommunityNames.Any())
                    return CommunityNames.Aggregate<string, string>(null, (current, next) => (current == null ? "" : current + ", ") + next);
                return "";
            }
        }
    }
}
