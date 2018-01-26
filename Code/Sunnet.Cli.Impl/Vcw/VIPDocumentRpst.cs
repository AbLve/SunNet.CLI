using Sunnet.Cli.Core.Vcw.Entities;
using Sunnet.Cli.Core.Vcw.Interfaces;
using Sunnet.Framework.Core.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Cli.Impl.Vcw
{
    public class VIPDocumentRpst : EFRepositoryBase<VIPDocumentEntity, Int32>, IVIPDocumentRpst
    {
        public VIPDocumentRpst(IUnitOfWork unit)
        {
            this.UnitOfWork = unit;
        }
    }
}
