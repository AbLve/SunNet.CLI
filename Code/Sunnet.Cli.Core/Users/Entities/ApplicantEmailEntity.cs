using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		Damon
 * Computer:		Damon-PC
 * Domain:			Damon-pc
 * CreatedOn:		2014/8/13 11:59:23
 * Description:		Please input class summary
 * Version History:	Created,2014/8/13 11:44:23
 * 
 * 
 **************************************************************************/
using Sunnet.Framework.Core.Base;

namespace Sunnet.Cli.Core.Users.Entities
{
    public class ApplicantEmailEntity : EntityBase<int>
    {
        public ApplicantEmailEntity()
        {
            EmailContent = "";
            EmailAddress = "";
        }

        public int ApplicantId { get; set; }

        public string EmailContent { get; set; }

        public string EmailAddress { get; set; }

        public virtual ApplicantEntity Applicant { get; set; }
    }
}
