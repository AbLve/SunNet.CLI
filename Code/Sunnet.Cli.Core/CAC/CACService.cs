using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sunnet.Cli.Core.CAC.Entities;
using Sunnet.Cli.Core.CAC.Interfaces;
using Sunnet.Cli.Core.Classes.Entites;
using Sunnet.Cli.Core.Classes.Interfaces;
using Sunnet.Cli.Core.Classrooms.Entites;
using Sunnet.Cli.Core.Classrooms.Interfaces;
using Sunnet.Cli.Core.Common.Enums;
using Sunnet.Cli.Core.Communities.Entities;
using Sunnet.Cli.Core.Schools.Entities;
using Sunnet.Cli.Core.Users.Enums;
using Sunnet.Framework.Core.Base;
using Sunnet.Framework.Core.Tool;
using Sunnet.Framework.EmailSender;
using Sunnet.Framework.Encrypt;
using Sunnet.Framework.File;
using Sunnet.Framework.Log;

namespace Sunnet.Cli.Core.CAC
{
    internal class CACService : CoreServiceBase, ICACContract
    {
        private readonly IMyActivityRpst _myactivityRpst;
        private readonly IActivityHistoryRpst _activityHistoryRpst;


        public CACService(ISunnetLog log, IFile fileHelper, IEmailSender emailSender, IEncrypt encrypt,
            IMyActivityRpst myactivityRpst, IActivityHistoryRpst activityHistoryRpst,
            IUnitOfWork unit)
            : base(log, fileHelper, emailSender, encrypt)
        {
            _myactivityRpst = myactivityRpst;
            _activityHistoryRpst = activityHistoryRpst;
            UnitOfWork = unit;
        }

        #region IQueryable

        public IQueryable<MyActivityEntity> MyActivities
        {
            get { return _myactivityRpst.Entities.Where(e => e.Status == EntityStatus.Active); }
        }

        public IQueryable<ActivityHistoryEntity> ActivityHistory
        {
            get { return _activityHistoryRpst.Entities; }
        }
        #endregion

        #region Activity
        public MyActivityEntity NewMyActivityEntity()
        {
            return _myactivityRpst.Create();
        }

        public OperationResult InsertMyActivity(MyActivityEntity entity, bool isSave = true)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                _myactivityRpst.Insert(entity, isSave);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult InsertActivityHistory(MyActivityEntity entity, bool isSave = true)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                _myactivityRpst.Insert(entity, isSave);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }


        public OperationResult UpdateMyActivity(MyActivityEntity entity, bool isSave = true)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                _myactivityRpst.Update(entity, isSave);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult DeleteMyActivity(int id)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                _myactivityRpst.Delete(id);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }
        public OperationResult DeleteMyActivity(MyActivityEntity entity)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                _myactivityRpst.Delete(entity);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }
        public bool IsMyActivity(int userId, int activityId, string name)
        {
            try
            {
                return _myactivityRpst.Entities.Any(c => c.ActivityId == activityId && c.UserId == userId && c.ActivityName == name);
            }
            catch (Exception ex)
            {
                return false;
            }
        }


        public ActivityHistoryEntity NewActivityHistoryEntity()
        {
            return _activityHistoryRpst.Create();
        }

        public OperationResult InsertActivityHistory(ActivityHistoryEntity entity, bool isSave = true)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                _activityHistoryRpst.Insert(entity, isSave);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }
        #endregion

    }

}
