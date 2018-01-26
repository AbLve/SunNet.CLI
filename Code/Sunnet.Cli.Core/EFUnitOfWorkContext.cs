using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Sunnet.Framework.Core.Base;


/**************************************************************************
 * Developer: 		JackZhang
 * Computer:		JACKZ
 * Domain:			Jackz
 * CreatedOn:		2014/8/1 0:54:12
 * Description:		Please input class summary
 * Version History:	Created,2014/8/1 0:54:12
 * 
 * 
 **************************************************************************/


namespace Sunnet.Cli.Core
{
    /// <summary>
    /// MainDbContext Lazy init
    /// </summary>
    public class EFUnitOfWorkContext : UnitOfWorkContextBase
    {
        /// <summary>
        ///     ��ȡ ��ǰʹ�õ����ݷ��������Ķ���
        /// </summary>
        protected override DbContext Context
        {
            get
            {
                //return secondCachingEnabled ? EFCachingDbContext.Value : EFDbContext.Value;
                return EFDbContext.Value;
            }
        }
        private Lazy<MainDbContext> EFDbContext = new Lazy<MainDbContext>();
    }
}
