using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		Joe
 * Computer:		JOE-PC
 * Domain:			Joe-pc
 * CreatedOn:		2014/9/2 12:15:10
 * Description:		Please input class summary
 * Version History:	Created,2014/9/2 12:15:10
 * 
 * 
 **************************************************************************/
using Sunnet.Cli.Core.Permission.Entities;
using System.ComponentModel.DataAnnotations;
using Sunnet.Framework.Mvc;
using Sunnet.Cli.Core.Users.Enums;
using System.ComponentModel;
using Sunnet.Cli.Core.Common.Enums;

namespace Sunnet.Cli.Business.Permission.Models
{
    public class UserRoleModel
    {
        [Required]
        public int ID { get; set; }

        [Required]
        [StringLength(100)]
        [DisplayName("Package Name")]
        public string Name { get; set; }

        [EensureEmptyIfNull]
        [StringLength(600)]
        public string Descriptions { get; set; }

        [Required]
        public EntityStatus Status { get; set; }

        [Required]
        [DisplayName("User Role")]
        public Role UserType { get; set; }

        public bool IsSelected { get; set; }

        [Required]
        public bool IsDefault { get; set; }

        public ICollection<PermissionRoleEntity> Roles { get; set; }
    }
}
