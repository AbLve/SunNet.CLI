using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


/**************************************************************************
 * Developer: 		JackZhang
 * Computer:		JACKZ
 * Domain:			JackZ
 * CreatedOn:		2/14 2015 10:24:16
 * Description:		Please input class summary
 * Version History:	Created,2/14 2015 10:24:16
 * 
 * 
 **************************************************************************/

namespace Sunnet.Cli.Business.Users
{
    public enum UserExistedStatus
    {
        /// <summary>
        /// 系统中不存在用户
        /// </summary>
        NotExisted = 1,
        /// <summary>
        /// 系统存在用户
        /// </summary>
        UserExisted = 10,
        /// <summary>
        /// 系统存在用户，并且用户与指定的Community有关联
        /// </summary>
        ExistedInCommunity = 20,
        /// <summary>
        /// 系统存在用户，并且用户与指定的School有关联
        /// </summary>
        ExistedInSchool = 30
    }
}
