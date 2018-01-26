using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		Leason
 * Computer:		Leason-PC
 * Domain:			Leason-pc
 * CreatedOn:		2014/8/23 12:20:20
 * Description:		Create ClassRpst
 * Version History:	Created,2014/8/23 12:20:20
 * 
 * 
 **************************************************************************/
using Sunnet.Cli.Core;
using Sunnet.Cli.Core.Classes.Entites;
using Sunnet.Cli.Core.Classes.Interfaces;
using Sunnet.Framework.Core.Base;

namespace Sunnet.Cli.Impl.Classes
{
    public class ClassRpst : EFRepositoryBase<ClassEntity, Int32>, IClassRpst
    {
        public ClassRpst(IUnitOfWork unit)
        {
            UnitOfWork = unit;
        }
        public bool UpdateClassPlaygrounds(int playgroundId, int[] classIds = null)
        {
            StringBuilder sb = new StringBuilder();
            EFUnitOfWorkContext context = this.UnitOfWork as EFUnitOfWorkContext;
            if (classIds != null && classIds.Length > 0)
            {
                string ids = string.Join(",", classIds);
                sb.AppendFormat(
                    @" UPDATE Classes SET playgroundId =0 where playgroundId ={1};UPDATE Classes SET playgroundId ={1} where ID in ({0})",
                    ids, playgroundId);
            }
            else
            {
                sb.AppendFormat(@" UPDATE Classes SET playgroundId =0 where playgroundId ={0};", playgroundId);
            }
            return context.DbContext.Database.ExecuteSqlCommand(sb.ToString()) > 0;
        }
    }
}