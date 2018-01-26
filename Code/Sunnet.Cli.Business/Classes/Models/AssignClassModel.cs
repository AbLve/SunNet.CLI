using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		Leason
 * Computer:		Leason-PC
 * Domain:			Leason-pc
 * CreatedOn:		2014/8/31 14:16:20
 * Description:		Create AssignClassModel
 * Version History:	Created,2014/8/31 14:16:20
 * 
 * 
 **************************************************************************/
using Sunnet.Cli.Core.Classes.Enums;

namespace Sunnet.Cli.Business.Classes
{
    public class AssignClassModel
    {
        public int ID { get; set; }
        public string ClassId { get; set; }
        public string ClassName { get; set; }
        public ClassType ClassType { get; set; }
        public int StudentNumber
        {
            get
            {
                if (tmpCount == null) return 0;
                else return (int)tmpCount;
            }
        }
        public int? tmpCount { get; set; }
    }
}
