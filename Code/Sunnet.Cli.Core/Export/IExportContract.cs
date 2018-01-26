using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sunnet.Cli.Core.Export.Entities;
using Sunnet.Framework.Core.Tool;

namespace Sunnet.Cli.Core.Export
{
    public interface IExportContract
    {
        IQueryable<ReportTemplateEntity> ReportTemplates { get; }

        OperationResult InsertReportTemplate(ReportTemplateEntity entity);

        OperationResult UpdateReportTemplate(ReportTemplateEntity entity);

        OperationResult DeleteReportTemplate(int id);

        ReportTemplateEntity GetTemplate(int id);


        IQueryable<FieldMapEntity> FieldMaps { get; }


        IQueryable<ExportInfoEntity> ExportInfos { get; }

        OperationResult InsertExportInfo(ExportInfoEntity entity);

        OperationResult InsertExportInfoList(List<ExportInfoEntity> entities);

        OperationResult UpdateExportInfo(ExportInfoEntity entity);

        OperationResult UpdateExportInfos(List<ExportInfoEntity> entities);

        ExportInfoEntity GetExportInfo(int id);

        DataSet ExecuteExportSql(string executeSql);

    }
}
