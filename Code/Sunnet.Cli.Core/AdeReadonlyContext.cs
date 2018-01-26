using Sunnet.Framework.Core.Base;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Cli.Core
{
    public class AdeReadonlyContext : AdeUnitOfWorkContext
    {
        /// <summary>
        ///     获取 当前使用的数据访问上下文对象
        /// </summary>
        protected override DbContext Context
        {
            get
            {
                //return secondCachingEnabled ? EFCachingDbContext.Value : EFDbContext.Value;
                return _efDbContext.Value;
            }
        }
        private readonly Lazy<AdeDbContext> _efDbContext = new Lazy<AdeDbContext>(delegate { return new AdeDbContext("AdeReadonlyDbContext"); });
    }
}
