using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		Leason
 * Computer:		Leason-PC
 * Domain:			Leason-pc
 * CreatedOn:		2014/9/10 16:21:20
 * Description:		Create ClassRolesEntity
 * Version History:	Created,2014/9/10 16:21:20
 * 
 * 
 **************************************************************************/
using Sunnet.Cli.Core.Common.Enums;
using Sunnet.Framework.Core.Base;

namespace Sunnet.Cli.Core.Classes.Entites
{
    public class ClassLevelEntity : EntityBase<int>
    {
        public string Name { get; set; }
        public EntityStatus Status { get; set; }

    }
}
