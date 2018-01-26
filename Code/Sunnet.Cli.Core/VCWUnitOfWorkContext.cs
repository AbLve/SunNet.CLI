using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Sunnet.Framework.Core.Base;
using System.Data.Entity;

namespace Sunnet.Cli.Core
{
    public class VCWUnitOfWorkContext : UnitOfWorkContextBase
    {
        /// <summary>
        ///     获取 当前使用的数据访问上下文对象
        /// </summary>
        protected override DbContext Context
        {
            get
            {
                //return secondCachingEnabled ? EFCachingDbContext.Value : EFDbContext.Value;
                return EFDbContext.Value;
            }
        }
        private Lazy<VcwDbContext> EFDbContext = new Lazy<VcwDbContext>();
    }
}
