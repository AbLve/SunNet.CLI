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
    public class ClassIndexModel
    {
        public int ID { get; set; }
        public string ClassId { get; set; }
        public string ClassName { get; set; }
        public EntityStatus Status { get; set; }

        [JsonIgnore]
        public IEnumerable<string> ClasroomNameList { get; set; }

        public string ClassroomName {
            get
            {
                if (ClasroomNameList.Count() == 0) return string.Empty;
                return string.Join(", ", ClasroomNameList);
            }
        }



        public int CommunityId { get; set; }
        public int SchoolId { get; set; }
        public int ClassroomId { get; set; }
    }
}
