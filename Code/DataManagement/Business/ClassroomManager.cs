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
    public class ClassroomManager:Base
    {
        ISunnetLog _log;
        BUPTaskBusiness _bupTaskBusiness;

        public ClassroomManager()
        {
            _log = ObjectFactory.GetInstance<ISunnetLog>();
            _bupTaskBusiness = new BUPTaskBusiness();
        }

        private delegate void ProcessHandler(int id, int createdBy);

        public bool PendingClassroom()
        {
            var pendingList = _bupTaskBusiness.GetList(BUPType.Classroom).Where(c => c.Status == BUPStatus.Pending);
            foreach (var pending in pendingList)
            {
                PendingClassroomByOne(pending);
            }

            return false;
        }

        private bool PendingClassroomByOne(BUPTaskEntity groupModel)
        {
            ProcessHandler handler = new ProcessHandler(BUPTaskBusiness.Start);
            handler.BeginInvoke(groupModel.ID, groupModel.UpdatedBy, null, null);
            return false;
        }
    }
}
