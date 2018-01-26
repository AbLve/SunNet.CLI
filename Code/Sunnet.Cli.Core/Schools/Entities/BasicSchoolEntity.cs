using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		Sam
 * Computer:		SAM-PC
 * Domain:			sam-pc
 * CreatedOn:		2014/8/23 14:53:03
 * Description:		Please input class summary
 * Version History:	Created,2014/8/23 14:53:03
 * 
 * 
 **************************************************************************/
using Sunnet.Cli.Core.Common.Enums;
using Sunnet.Cli.Core.MasterData.Entities;
using Sunnet.Framework.Core.Base;

namespace Sunnet.Cli.Core.Schools.Entities
{
    public class BasicSchoolEntity : EntityBase<int>
    {
        public string Name { get; set; }
        public SchoolStatus Status { get; set; }
        public string Address1 { get; set; }
        public string City { get; set; }
        public string Zip { get; set; }
        public string Phone { get; set; }
        public int CountyId { get; set; }
        public int StateId { get; set; }
        public virtual CountyEntity County { get; set; }
        public virtual StateEntity State { get; set; }
        public int CommunityId { get; set; }
        public string SchoolNumber { get; set; }

    }
}
