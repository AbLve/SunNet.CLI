using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
/**************************************************************************
 * Developer: 		Leason
 * Computer:		Leason-PC
 * Domain:			Leason-pc
 * CreatedOn:		2014/8/30 11:16:20
 * Description:		Create AssigenStudentClassModel
 * Version History:	Created,2014/8/30 11:16:20
 * 
 * 
 **************************************************************************/
using Sunnet.Cli.Core.Classes.Enums;

namespace Sunnet.Cli.Business.Classes
{
    /// <summary>
    /// Assigen Student to Class
    /// </summary>
    public class AssigenStudentClassModel
    {

        /// <summary>
        /// Class的主键
        /// </summary>
        public int ClassId { get; set; }

        public string ClassName { get; set; }

        public DayType DayType { get; set; }

        public string SchoolName { get; set; }

        [JsonIgnore]
        public IEnumerable<string> ClasroomNameList { get; set; }

        public string ClassroomName
        {
            get
            {
                if (ClasroomNameList == null) return string.Empty;
                return string.Join(", ", ClasroomNameList);
            }
        }

        public string ClassCode { get; set; }

    }
}
