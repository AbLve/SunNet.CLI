using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		Sam
 * Computer:		SAM-PC
 * Domain:			sam-pc
 * CreatedOn:		2014/8/19 9:01:35
 * Description:		Please input class summary
 * Version History:	Created,2014/8/19 9:01:35
 * 
 * 
 **************************************************************************/
using Sunnet.Framework.Core.Base;

namespace Sunnet.Cli.Core.Schools.Entities
{
    public class PlaygroundEntity : EntityBase<int>
    {
        [StringLength(50)]
        public string Name { get; set; }

        public int SchoolId { get; set; }
    }
}
