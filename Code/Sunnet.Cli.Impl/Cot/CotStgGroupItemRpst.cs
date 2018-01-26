using Sunnet.Cli.Core;
using Sunnet.Cli.Core.Cot.Entities;
using Sunnet.Cli.Core.Cot.Interfaces;
using Sunnet.Cli.Core.Cot.Models;
using Sunnet.Framework.Core.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Cli.Impl.Cot
{
    public class CotStgGroupItemRpst : EFRepositoryBase<CotStgGroupItemEntity, int>, ICotStgGroupItemRpst
    {
        public CotStgGroupItemRpst(IUnitOfWork unit)
        {
            this.UnitOfWork = unit;
        }

        public List<CotStgGroupItemModel> GetCotStgGroupItems(int cotStgReportId)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("SELECT ");
                sb.Append("CSG.CotStgReportId , ");
                sb.Append("CSG.ID AS CotStgGroupId , ");
                sb.Append("CSG.GroupName , ");
                sb.Append("CSGI.ItemId, ");
                sb.Append("CI.Level, ");
                sb.Append("CI.ShortTargetText, ");
                sb.Append("CI.FullTargetText, ");
                sb.Append("CI.CotItemId, ");
                sb.Append("CSGI.Sort ");
                sb.Append("FROM ");
                sb.Append("dbo.CotStgReports AS CSR ");
                sb.Append("LEFT JOIN dbo.CotStgGroups AS CSG ON CSR.ID = CSG.CotStgReportId ");
                sb.Append("LEFT JOIN dbo.CotStgGroupItems AS CSGI ON CSG.ID = CSGI.CotStgGroupId ");
                sb.Append("LEFT JOIN dbo.COTAssessmentItems AS CAI ON CSGI.ItemId = CAI.ID ");
                sb.Append("LEFT JOIN dbo.CotItems AS CI ON CAI.ItemId = CI.ID ");
                sb.Append("WHERE ");
                sb.Append("CSG.CotStgReportId={0} ");
                sb.Append("AND CSG.IsDelete=0;");
                AdeUnitOfWorkContext context = this.UnitOfWork as AdeUnitOfWorkContext;
                return context.DbContext.Database.SqlQuery<CotStgGroupItemModel>(string.Format(sb.ToString(), cotStgReportId)).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
