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
using Sunnet.Cli.Core.Common.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using Sunnet.Framework.Mvc; 

namespace Sunnet.Cli.Core.Users.Entities
{
    public class Sy_UserBaseEntity : EntityBase<int>
    {
        public Sy_UserBaseEntity()
        {
            FirstName = "";
            MiddleName = "";
            LastName = "";
            PreviousLastName = "";
            Status = EntityStatus.Active;
           
        }
        /// <summary>
        /// 对应的 Sunnet.Cli.Core.Users.Enums.Role ;同时对应 RoleEntity Role
        /// </summary>
        
        [StringLength(140)]
        [Required]
        [DisplayName("First Name")]
        public string FirstName { get; set; }

        [EensureEmptyIfNullAttribute]
        [StringLength(140)]
        [DisplayName("Middle Name")]
        public string MiddleName { get; set; }

        [StringLength(140)]
        [Required]
        [DisplayName("Last Name")]
        public string LastName { get; set; }

        [EensureEmptyIfNullAttribute]
        [StringLength(140)]
        [DisplayName("Previous Last Name")]
        public string PreviousLastName { get; set; }

        [DisplayName("Status")]
        public EntityStatus Status { get; set; }

        
    }

     
}
