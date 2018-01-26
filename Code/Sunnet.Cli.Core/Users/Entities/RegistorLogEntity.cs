using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		Sam
 * Computer:		SAM-PC
 * Domain:			sam-pc
 * CreatedOn:		2014/8/5 15:58:17
 * Description:		Please input class summary
 * Version History:	Created,2014/8/5 15:58:17
 * 
 * 
 **************************************************************************/
using Sunnet.Framework.Core.Base;

namespace Sunnet.Cli.Core.Users.Entities
{
    
    public class RegistorLogEntity : EntityBase<int>
    {
        public int UserId { get; set; }
        public int Status { get; set; }
        public int VerifyBy { get; set; }

       
        public virtual UserBaseEntity Applicant { get; set; }


        
        public virtual UserBaseEntity Verifier { get; set; }

    }
}
