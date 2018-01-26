using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sunnet.Cli.Core.Classes.Entites;
using Sunnet.Cli.Core.Classes.Interfaces;
using Sunnet.Cli.Core.TRSClasses.Entites;
using Sunnet.Cli.Core.TRSClasses.Interfaces;
using Sunnet.Framework.Core.Base;
using Sunnet.Framework.Core.Tool;
using Sunnet.Framework.EmailSender;
using Sunnet.Framework.Encrypt;
using Sunnet.Framework.File;
using Sunnet.Framework.Log;

namespace Sunnet.Cli.Core.TRSClasses
{
    internal class TRSClassService : CoreServiceBase, ITRSClassContract
    {
        private readonly ITRSClassRpst _trsClassRpst;
        private readonly ICHChildrenRpst _chChildrenRpst;
        private readonly ICHChildrenResultRpst _chChildrenResultRpst;

        public TRSClassService(ISunnetLog log, IFile fileHelper, IEmailSender emailSender, IEncrypt encrypt,
            ITRSClassRpst tRSClassRpst,
            ICHChildrenRpst cHChildrenRpst,
            ICHChildrenResultRpst cHChildrenResultRpst,
            IUnitOfWork unit)
            : base(log, fileHelper, emailSender, encrypt)
        {
            _trsClassRpst = tRSClassRpst;
            _chChildrenRpst = cHChildrenRpst;
            _chChildrenResultRpst = cHChildrenResultRpst;
            UnitOfWork = unit;
        }

        #region TRS Class
        public IQueryable<TRSClassEntity> TRSClasses
        {
            get { return _trsClassRpst.Entities; }
        }

        public OperationResult InsertTRSClass(TRSClassEntity entity, bool isSave = true)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                _trsClassRpst.Insert(entity, isSave);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult DeleteTRSClass(int id)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                _trsClassRpst.Delete(id);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult UpdateTRSClass(TRSClassEntity entity, bool isSave = true)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                _trsClassRpst.Update(entity, isSave);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public TRSClassEntity GetTRSClass(int id)
        {
            return _trsClassRpst.GetByKey(id);
        }

        public OperationResult UpdateTRSClassPlayground(int playgroundId = 0, int[] classIds = null)
        {
            var result = new OperationResult(OperationResultType.Success);

            if (playgroundId > 0)
            {
                try
                {
                    _trsClassRpst.UpdateTRSClassPlayground(playgroundId, classIds);
                }
                catch (Exception ex)
                {
                    result = ResultError(ex);
                }
            }
            return result;
        }

        #endregion

        #region TRS
        public IQueryable<CHChildrenEntity> CHChildrens
        {
            get { return _chChildrenRpst.Entities; }
        }

        public IQueryable<CHChildrenResultEntity> CHChildrenResults
        {
            get { return _chChildrenResultRpst.Entities; }
        }

        public OperationResult InsertResult(List<CHChildrenResultEntity> list, bool isSave = true)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                _chChildrenResultRpst.Insert(list, isSave);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult DeleteResult(int classId, bool isSave = true)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                _chChildrenResultRpst.Delete(o => o.TRSClassId == classId, isSave);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult DeleteResultBySchoolId(int schoolId, bool isSave = true)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                _chChildrenResultRpst.DeleteResultBySchoolId(schoolId);
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
