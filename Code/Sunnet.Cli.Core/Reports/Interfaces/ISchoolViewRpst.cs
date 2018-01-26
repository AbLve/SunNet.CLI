using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		JackZhang
 * Computer:		JACKZ
 * Domain:			JackZ
 * CreatedOn:		2/3 2015 17:24:35
 * Description:		Please input class summary
 * Version History:	Created,2/3 2015 17:24:35
 * 
 * 
 **************************************************************************/
using Sunnet.Cli.Core.Reports.Entities;
using Sunnet.Framework.Core.Base;

namespace Sunnet.Cli.Core.Reports.Interfaces
{
    /// <summary>
    /// SchoolView数据接口
    /// 数据库中对应的是视图，不能执行Insert,Update,Delete方法
    /// </summary>
    public interface ISchoolViewRpst : IRepository<SchoolViewEntity, int>
    {

    }
}
