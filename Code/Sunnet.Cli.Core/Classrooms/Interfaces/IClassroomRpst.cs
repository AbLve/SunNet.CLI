using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		Leason
 * Computer:		Leason-PC
 * Domain:			Leason-pc
 * CreatedOn:		2014/8/22 14:37:20
 * Description:		Create IClassroomRpst
 * Version History:	Created,2014/8/22 14:37:20
 * 
 * 
 **************************************************************************/
using Sunnet.Cli.Core.Classrooms.Entites;
using Sunnet.Framework.Core.Base;

namespace Sunnet.Cli.Core.Classrooms.Interfaces
{
    public interface IClassroomRpst : IRepository<ClassroomEntity, Int32>
    {
    }
}
