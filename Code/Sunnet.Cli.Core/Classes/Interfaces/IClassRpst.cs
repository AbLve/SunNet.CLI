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
 * Description:		Create ClassroomCnfg
 * Version History:	Created,2014/8/22 14:37:20
 * 
 * 
 **************************************************************************/
using Sunnet.Cli.Core.Classes.Entites;
using Sunnet.Framework.Core.Base;

namespace Sunnet.Cli.Core.Classes.Interfaces
{
    public interface IClassRpst:IRepository<ClassEntity,int>
    {
        bool UpdateClassPlaygrounds(int playgroundId, int[] classIds = null);
    }
}
