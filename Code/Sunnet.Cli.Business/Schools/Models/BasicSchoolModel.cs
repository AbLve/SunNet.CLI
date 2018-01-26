using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		JackZhang
 * Computer:		JACKZ
 * Domain:			JackZ
 * CreatedOn:		2/13 2015 13:43:23
 * Description:		Please input class summary
 * Version History:	Created,2/13 2015 13:43:23
 * 
 * 
 **************************************************************************/
using Newtonsoft.Json;

namespace Sunnet.Cli.Business.Schools.Models
{
    public class BasicSchoolModel
    {
        public int ID { get; set; }
        public string Name { get; set; }

        public string SchoolType { get; set; }


       [DisplayName("Community/District")]
        public IEnumerable<string> Communities { get; set; }

        [DisplayName("Community/District")]
        public string CommunitiesText
        {
            get
            {
                return Communities == null ? string.Empty : string.Join(", ", Communities);
            }
        }
    }
}
