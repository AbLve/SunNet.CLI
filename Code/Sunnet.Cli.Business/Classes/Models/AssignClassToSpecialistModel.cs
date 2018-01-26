using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		Leason
 * Computer:		Leason-PC
 * Domain:			Leason-pc
 * CreatedOn:		2014/8/31 19:24:20
 * Description:		Create ClassIndexModel
 * Version History:	Created,2014/8/31 19:24:20
 * 
 * 
 **************************************************************************/
using Sunnet.Cli.Core.Common.Enums;
using Newtonsoft.Json;

namespace Sunnet.Cli.Business.Classes.Models
{
    public class AssignClassToSpecialistModel
    {
        public int ID { get; set; }
        public string ClassId { get; set; }
        public string ClassName { get; set; }
        public EntityStatus Status { get; set; }
        public string SchoolName { get; set; }
        [JsonIgnore]
        public IEnumerable<string> CommunityNameList { get; set; }

        public string CommunityName
        {
            get
            {
                if (!CommunityNameList.Any()) return string.Empty;
                return string.Join(", ", CommunityNameList);
            }
        }

    }
}
