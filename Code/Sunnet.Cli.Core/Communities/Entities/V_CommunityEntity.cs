using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		Leason
 * Computer:		Leason-PC
 * Domain:			Leason-pc
 * CreatedOn:		2014/8/18 16:27:20
 * Description:		Create CommunitiesRspt
 * Version History:	Created,2014/8/18 16:27:20
 * 
 * 
 **************************************************************************/
using Sunnet.Cli.Core.Classes.Entites;
using Sunnet.Cli.Core.Common.Enums;
using Sunnet.Cli.Core.MasterData.Entities;
using Sunnet.Cli.Core.Schools.Entities;
using Sunnet.Cli.Core.Students.Entities;
using Sunnet.Framework.Core.Base;
using Sunnet.Framework.Mvc;
using Sunnet.Cli.Core.Users.Entities;
using Sunnet.Cli.Core.Permission.Entities;
using Sunnet.Cli.Core.BUP.Entities;

namespace Sunnet.Cli.Core.Communities.Entities
{
    public class V_CommunityEntity : EntityBase<int>
    {
        /// <summary>
        /// 触发生成
        /// </summary>
        [EensureEmptyIfNull]
        [StringLength(32)]
        [DisplayName("Engage ID")]
        public string CommunityId { get; set; }

        [Required]
        [DisplayName("Community Name")]
        public string Name { get; set; }

        [Required]
        [DisplayName("Name")]
        public int BasicCommunityId { get; set; }

        [Required]
        [DisplayName("Status")]
        public EntityStatus Status { get; set; }

    }
     
}
