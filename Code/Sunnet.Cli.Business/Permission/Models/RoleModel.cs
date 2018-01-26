using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		Joe
 * Computer:		Joe-PC
 * Domain:			Joe-pc
 * CreatedOn:		2014/9/5
 * Description:		Create RoleEntity
 * Version History:	Created,2014/9/5
 * 
 * 
 **************************************************************************/
using Sunnet.Cli.Core.Permission.Entities;
using Sunnet.Cli.Core.Users.Enums;
using System.ComponentModel.DataAnnotations;
using Sunnet.Cli.Core.Common.Enums;

namespace Sunnet.Cli.Business.Permission.Models
{
    public class RoleModel
    {
        public RoleModel()
        {
            DistrictsAndSchools = new List<AssignedPackageModel>();
        }

        public int ID { get; set; }

        public string Name { get; set; }

        public string Descriptions { get; set; }

        public EntityStatus Status { get; set; }

        [Display(Name = "User Type")]
        public Role UserType { get; set; }

        public bool IsDefault { get; set; }

        public ICollection<AssignedPackageModel> DistrictsAndSchools { get; set; }

    }
}
