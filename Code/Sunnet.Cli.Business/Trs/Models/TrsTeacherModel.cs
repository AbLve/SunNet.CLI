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
 * CreatedOn:		1/21 2015 15:28:24
 * Description:		Please input class summary
 * Version History:	Created,1/21 2015 15:28:24
 * 
 * 
 **************************************************************************/
using Sunnet.Cli.Business.Users.Models;

namespace Sunnet.Cli.Business.Trs.Models
{
    public class TrsTeacherModel:UserBaseModel
    {

        public bool IsLeadTeacher { get; set; }

        [DisplayName("Middle Name")]
        public string MiddleName { get; set; }
    }
}
