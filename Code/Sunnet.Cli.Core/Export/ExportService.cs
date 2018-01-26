using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;

using Sunnet.Cli.Core.Export.Entities;
using Sunnet.Framework.Core.Tool;
using Sunnet.Cli.Core.Export;
using Sunnet.Cli.Core.Export.Interfaces;
using Sunnet.Framework.Log;
using Sunnet.Framework.File;
using Sunnet.Framework.EmailSender;
using Sunnet.Framework.Encrypt;
using Sunnet.Framework.Core.Base;

namespace Sunnet.Cli.Core.Export
{
    internal class ExportService : CoreServiceBase, IExportContract
    {
        private readonly IReportTemplateRpst ReportTemplateRpst;
        private readonly IFieldMapRpst FieldMapRpst;
        private readonly IExportInfoRpst ExportInfoRpst;

        public ExportService(ISunnetLog log, IFile fileHelper, IEmailSender emailSender, IEncrypt encrypt,
            IReportTemplateRpst reportTemplateRpst,
            IFieldMapRpst fieldMapRpst,
            IExportInfoRpst exportInfoRpst,
            IUnitOfWork unit)
            : base(log, fileHelper, emailSender, encrypt)
        {
            ReportTemplateRpst = reportTemplateRpst;
            FieldMapRpst = fieldMapRpst;
            ExportInfoRpst = exportInfoRpst;
            UnitOfWork = unit;
        }

        #region ReportTemplate

        public IQueryable<ReportTemplateEntity> ReportTemplates
        {
            get { return ReportTemplateRpst.Entities; }
        }

        public OperationResult InsertReportTemplate(ReportTemplateEntity entity)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                ReportTemplateRpst.Insert(entity);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult UpdateReportTemplate(ReportTemplateEntity entity)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                ReportTemplateRpst.Update(entity);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult DeleteReportTemplate(int id)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                ReportTemplateRpst.Delete(id);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public ReportTemplateEntity GetTemplate(int id)
        {
            return ReportTemplateRpst.GetByKey(id);
        }

        #endregion

        #region FieldMap

        public IQueryable<FieldMapEntity> FieldMaps
        {
            get { return FieldMapRpst.Entities; }
        }

        #endregion

        #region ExportInfo

        public IQueryable<ExportInfoEntity> ExportInfos
        {
            get { return ExportInfoRpst.Entities; }
        }

        public OperationResult InsertExportInfo(ExportInfoEntity entity)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                ExportInfoRpst.Insert(entity);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult InsertExportInfoList(List<ExportInfoEntity> entities)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                ExportInfoRpst.Insert(entities);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult UpdateExportInfo(ExportInfoEntity entity)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                ExportInfoRpst.Update(entity);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult UpdateExportInfos(List<ExportInfoEntity> entities)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                entities.ForEach(x => ExportInfoRpst.Update(x, false));
                UnitOfWork.Commit();
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public ExportInfoEntity GetExportInfo(int id)
        {
            return ExportInfoRpst.GetByKey(id);
        }

        public DataSet ExecuteExportSql(string executeSql)
        {
            return ExportInfoRpst.ExecuteExportSql(executeSql);
        }
        #endregion
    }
}
