using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Sunnet.Cli.Core.DataProcess.Interfaces;
using Sunnet.Framework.Log;
using Sunnet.Framework.File;
using Sunnet.Framework.EmailSender;
using Sunnet.Framework.Encrypt;
using Sunnet.Framework.Core.Base;
using Sunnet.Cli.Core.DataProcess.Entities;
using Sunnet.Framework.Core.Tool;
using Sunnet.Cli.Core.DataProcess.Models;

namespace Sunnet.Cli.Core.DataProcess
{
    internal class DataProcessService : CoreServiceBase, IDataProcessContract
    {
        IDataGroupRpst _groupRpst;
        IDataCommunityRpst _communityRpst;
        IDataSchoolRpst _schoolRpst;
        IDataStudentRpst _studentRpst;

        public DataProcessService(ISunnetLog log,
            IFile fileHelper,
            IEmailSender emailSender,
            IEncrypt encrypt,
            IDataGroupRpst groupRpst,
            IDataCommunityRpst communityRpst,
            IDataSchoolRpst schoolRpst,
            IDataStudentRpst studentRpst,
            IUnitOfWork unit)
            : base(log, fileHelper, emailSender, encrypt)
        {
            _groupRpst = groupRpst;
            _communityRpst = communityRpst;
            _schoolRpst = schoolRpst;
            _studentRpst = studentRpst;
            UnitOfWork = unit;
        }

        public IQueryable<DataGroupEntity> Groups
        {
            get { return _groupRpst.Entities; }
        }

        public IQueryable<DataCommunityEntity> Communities
        {
            get { return _communityRpst.Entities; }
        }

        public IQueryable<DataSchoolEntity> Schools
        {
            get { return _schoolRpst.Entities; }
        }

        public IQueryable<DataStudentEntity> Students
        {
            get { return _studentRpst.Entities; }
        }



        public OperationResult InsertGroup(DataGroupEntity entity)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            try
            {
                _groupRpst.Insert(entity);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult UpdateGroup(DataGroupEntity entity)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            try
            {
                _groupRpst.Update(entity);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult InsertCommunity(DataCommunityEntity entity)
        {
            throw new NotImplementedException();
        }

        public OperationResult InsertSchool(DataSchoolEntity entity)
        {
            throw new NotImplementedException();
        }

        public OperationResult InsertStudent(DataStudentEntity entity)
        {
            throw new NotImplementedException();
        }

        public bool DeleteGroup(int id)
        {
            return _groupRpst.Delete(id) > 0;
        }

        public string ImportData(string sql)
        {
            try
            {
                return _groupRpst.ImportData(sql);
            }
            catch (Exception ex)
            {                
                LoggerHelper.Debug(ex);
                return ex.Message;
            }
        }

        public void Start(int id, int createdBy)
        {
            try
            {
                _groupRpst.Start(id, createdBy);
            }
            catch (Exception ex)
            {
                LoggerHelper.Debug(ex);
            }
        }

        public List<RecordRemarkModel> GetRemarks(string sql)
        {
            return _groupRpst.GetRemarks(sql);
        }
    }
}