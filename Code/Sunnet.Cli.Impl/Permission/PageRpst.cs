using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		Joe
 * Computer:		JOE-PC
 * Domain:			Joe-pc
 * CreatedOn:		2014/9/2 12:15:10
 * Description:		Please input class summary
 * Version History:	Created,2014/9/2 12:15:10
 * 
 * 
 **************************************************************************/
using Sunnet.Framework.Core.Base;
using Sunnet.Cli.Core.Permission.Entities;
using Sunnet.Cli.Core.Permission.Interfaces;
using Sunnet.Cli.Core;
using Sunnet.Framework.Permission;
using Sunnet.Framework;

namespace Sunnet.Cli.Impl.Permission
{
    public class PageRpst : EFRepositoryBase<PageEntity, Int32>, IPageRpst
    {
        public PageRpst(IUnitOfWork unit)
        {
            this.UnitOfWork = unit;
        }

        public void AddAssessmentPage(int assessmentId, string label, int parentId)
        {
            try
            {
                //不能在此Dispose掉context，因为执行过此方法后，还会用到context
                EFUnitOfWorkContext context = this.UnitOfWork as EFUnitOfWorkContext;

                StringBuilder sql = new StringBuilder();
                sql.Append("SET IDENTITY_INSERT Permission_Pages ON;")
                 .Append("insert Permission_Pages (ID, Name,Ispage,ParentID,Url,Sort,IsShow,Descriptions,CreatedOn,UpdatedOn) ")
                  .AppendFormat(" values({0}, '{1}',1,{2},'',1,1,'',getdate(),getdate());"
                    , SFConfig.AssessmentPageStartId + assessmentId, label, parentId)
                    .Append("SET IDENTITY_INSERT Permission_Pages OFF")
                    .Append("; insert Permission_PageAuthorities (PageId,ActionId) ")
                    .AppendFormat(" values({0},{1});", SFConfig.AssessmentPageStartId + assessmentId, (int)Authority.Index)
                    .Append(" insert Permission_PageAuthorities (PageId,ActionId) ")
                    .AppendFormat(" values({0},{1});", SFConfig.AssessmentPageStartId + assessmentId, (int)Authority.Offline)
                        .Append(" insert Permission_PageAuthorities (PageId,ActionId) ")
                    .AppendFormat(" values({0},{1});", SFConfig.AssessmentPageStartId + assessmentId, (int)Authority.AssessmentPracticeArea);

                context.DbContext.Database.ExecuteSqlCommand(sql.ToString());
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }


        public void UpdateAssessmentPage(int assessmentId, string label)
        {
            try
            {
                //不能在此Dispose掉context，因为执行过此方法后，还会用到context
                EFUnitOfWorkContext context = this.UnitOfWork as EFUnitOfWorkContext;

                StringBuilder sql = new StringBuilder();
                sql.AppendFormat("update Permission_Pages set  Name = '{0}' where id = {1} "
                    , label, SFConfig.AssessmentPageStartId + assessmentId);

                context.DbContext.Database.ExecuteSqlCommand(sql.ToString());
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }

        public void DeleteAssessmentPage(int assessmentId)
        {
            try
            {
                //不能在此Dispose掉context，因为执行过此方法后，还会用到context
                EFUnitOfWorkContext context = this.UnitOfWork as EFUnitOfWorkContext;

                StringBuilder sql = new StringBuilder();
                sql.AppendFormat("delete [dbo].[Permission_Pages] where id = {0};"
                    , SFConfig.AssessmentPageStartId + assessmentId)
                    .AppendFormat("delete [dbo].[Permission_PageAuthorities]  where PageId =  {0};"
                    , SFConfig.AssessmentPageStartId + assessmentId)
                    .AppendFormat("delete Permission_RolePageAuthorities where PageId = {0};"
                    , SFConfig.AssessmentPageStartId + assessmentId);

                context.DbContext.Database.ExecuteSqlCommand(sql.ToString());
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }
    }
}
