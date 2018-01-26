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
    public class CommunitySpecialistManager : Base
    {
        ISunnetLog _log;
        BUPTaskBusiness _bupTaskBusiness;

        public CommunitySpecialistManager()
        {
            _log = ObjectFactory.GetInstance<ISunnetLog>();
            _bupTaskBusiness = new BUPTaskBusiness();
        }

        private delegate void ProcessHandler(int id, int createdBy);

        public bool PendingCommunitySpecialist()
        {
            var pendingList = _bupTaskBusiness.GetList(BUPType.CommunitySpecialist).Where(c => c.Status == BUPStatus.Pending);
            foreach (var pending in pendingList)
            {
                PendingCommunitySpecialistByOne(pending);
            }

            return false;
        }

        private bool PendingCommunitySpecialistByOne(BUPTaskEntity groupModel)
        {
            ProcessHandler handler = new ProcessHandler(BUPTaskBusiness.Start);
            handler.BeginInvoke(groupModel.ID, groupModel.UpdatedBy, null, null);
            return false;
        }
    }
}
