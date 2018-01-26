using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StructureMap;
using Sunnet.Cli.Business.BUP;
using Sunnet.Cli.Core.BUP;
using Sunnet.Cli.Core.BUP.Entities;
using Sunnet.Framework.Log;

namespace DataManagement.Business
{
    public class AuditorManager : Base
    {
        ISunnetLog _log;
        BUPTaskBusiness _bupTaskBusiness;

        public AuditorManager()
        {
            _log = ObjectFactory.GetInstance<ISunnetLog>();
            _bupTaskBusiness = new BUPTaskBusiness();
        }

        private delegate void ProcessHandler(int id, int createdBy);

        public bool PendingAuditor()
        {
            var pendingList = _bupTaskBusiness.GetList(BUPType.Auditor).Where(c => c.Status == BUPStatus.Pending);
            foreach (var pending in pendingList)
            {
                PendingAuditorByOne(pending);
            }

            return false;
        }

        private bool PendingAuditorByOne(BUPTaskEntity groupModel)
        {
            ProcessHandler handler = new ProcessHandler(BUPTaskBusiness.Start);
            handler.BeginInvoke(groupModel.ID, groupModel.UpdatedBy, null, null);
            return false;
        }
    }
}
