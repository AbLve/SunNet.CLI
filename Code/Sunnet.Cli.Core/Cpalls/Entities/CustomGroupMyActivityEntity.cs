using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		JackZhang
 * Computer:		JACKZ
 * Domain:			Jackz
 * CreatedOn:		2014/10/7 4:21:45
 * Description:		Please input class summary
 * Version History:	Created,2014/10/7 4:21:45
 * 
 * 
 **************************************************************************/
using Sunnet.Framework.Core.Base;
using Sunnet.Cli.Core.Ade;
using Sunnet.Cli.Core.Cpalls.Configurations;

namespace Sunnet.Cli.Core.Cpalls.Entities
{
    public class CustomGroupMyActivityEntity : EntityBase<int>
    {
        public int GroupId { get; set; }
        public int MyActivityId { get; set; }



        [DisplayName("Created By")]
        public int CreatedBy { get; set; }

        [DisplayName("Updated By")]
        public int UpdatedBy { get; set; }
        public virtual CpallsStudentGroupEntity Group { get; set; }

    }
}
