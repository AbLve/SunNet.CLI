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
    public class EmailReplaceBodyEntity : EntityBase<int>
    {
        public EmailReplaceBodyEntity()
        {
            Subject = "";
            Title = "";
            AplicantFirstName = "";
            AplicantLastName = "";
            SuperiorFirstName = "";
            SuperiorLastName = "";
            HomeUrl = "";
            CliEngageUrl = "";
            AplicantEmail = "";
            AdminEmail = "";
        }
        public string Subject { get; set; }
        public string Title { get; set; }
        public string AplicantFirstName { get; set; }
        public string AplicantLastName { get; set; }
        public string SuperiorFirstName { get; set; }
        public string SuperiorLastName { get; set; }
        public string HomeUrl { get; set; }
        public string CliEngageUrl { get; set; }
        public string AplicantEmail { get; set; }
        public string AdminEmail { get; set; }

    }
}
