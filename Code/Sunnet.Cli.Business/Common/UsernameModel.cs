using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


/**************************************************************************
 * Developer: 		JackZhang
 * Computer:		JACKZ
 * Domain:			Jackz
 * CreatedOn:		2014/9/5 5:09:40
 * Description:		Please input class summary
 * Version History:	Created,2014/9/5 5:09:40
 * 
 * 
 **************************************************************************/

namespace Sunnet.Cli.Business.Common
{
    public class UsernameModel
    {
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public int ID { get; set; }

        public string ShowName
        {
            get { return Firstname; }
        }
    }
}
