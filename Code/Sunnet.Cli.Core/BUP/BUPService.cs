using Sunnet.Cli.Core.BUP.Entities;
using Sunnet.Cli.Core.BUP.Interfaces;
using Sunnet.Framework.Core.Base;
using Sunnet.Framework.Core.Tool;
using Sunnet.Framework.EmailSender;
using Sunnet.Framework.Encrypt;
using Sunnet.Framework.File;
using Sunnet.Framework.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Cli.Core.BUP
{
    internal class BUPService : CoreServiceBase, IBUPContract
    {
        private readonly IBUPTaskRpst _rpst;
        private readonly IAutomationSettingRpst _automationRpst;

        public BUPService(ISunnetLog log, IFile fileHelper, IEmailSender emailSender, IEncrypt encrypt,
            IBUPTaskRpst rpst, IAutomationSettingRpst automationRpst, IUnitOfWork unit)
            : base(log, fileHelper, emailSender, encrypt)
        {
            _rpst = rpst;
            _automationRpst = automationRpst;
            UnitOfWork = unit;
        }

        public IQueryable<BUPTaskEntity> Tasks
        {
            get { return _rpst.Entities; }
        }

        public OperationResult InsertTask(BUPTaskEntity entity)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                int aa = _rpst.Insert(entity);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }


        public OperationResult UpdateTask(BUPTaskEntity entity)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                _rpst.Update(entity);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public string ExecuteCommunitySql(string sql)
        {
            try
            {
                return _rpst.ExecuteCommunitySql(sql);
            }
            catch (Exception ex)
            {
                LoggerHelper.Info(sql);
                OperationResult result = ResultError(ex);
                return result.Message;
            }
        }

        public int ExecuteSqlCommand(string sql)
        {
            try
            {
                return _rpst.ExecuteSqlCommand(sql);
            }
            catch (Exception ex)
            {
                LoggerHelper.Info(sql);
                OperationResult result = ResultError(ex);
                return 0;
            }
        }

        public dynamic ExecuteSqlQuery(string sql, BUPType type)
        {
            try
            {
                return _rpst.ExecuteSqlQuery(sql, type);
            }
            catch (Exception ex)
            {
                LoggerHelper.Info(sql);
                OperationResult result = ResultError(ex);
                return 0;
            }
        }

        public IQueryable<AutomationSettingEntity> AutomationSettings
        {
            get { return _automationRpst.Entities; }
        }

        public OperationResult InsertAutomationSetting(AutomationSettingEntity entity)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                int aa = _automationRpst.Insert(entity);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }


        public OperationResult UpdateAutomationSetting(AutomationSettingEntity entity)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                _automationRpst.Update(entity);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public AutomationSettingEntity GetAutomationSetting(int Id)
        {
            return _automationRpst.GetByKey(Id);
        }

        public void Start(int id, int createdBy)
        {
            try
            {
                _rpst.Start(id, createdBy);
            }
            catch (Exception ex)
            {
                LoggerHelper.Debug(ex);
            }
        }
    }
}
