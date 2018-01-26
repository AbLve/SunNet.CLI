using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		Leason
 * Computer:		Leason-PC
 * Domain:			Leason-pc
 * CreatedOn:		2014/8/22 14:37:20
 * Description:		Create ClassEntity
 * Version History:	Created,2014/10/23 13:37:20
 * 
 * 
 **************************************************************************/
namespace Sunnet.Cli.Business.Students.Models
{
    public class SchoolBrief
    {
        public int SchoolId { get; set; }
        public string SchoolName { get; set; }
        public int StudentCount { get; set; }
    }
}
