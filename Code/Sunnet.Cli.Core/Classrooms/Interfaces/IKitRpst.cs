using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		Leason
 * Computer:		Leason-PC
 * Domain:			Leason-pc
 * CreatedOn:		2014/8/23 14:23:20
 * Description:		Create CurriculumRpst
 * Version History:	Created,2014/8/23 14:23:20
 * 
 * 
 **************************************************************************/
using Sunnet.Cli.Core.Classrooms.Entites;
using Sunnet.Framework.Core.Base;

namespace Sunnet.Cli.Core.Classrooms.Interfaces
{
    public interface IKitRpst:IRepository<KitEntity,Int32>
    {
    }
}
