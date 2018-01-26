using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		Sam
 * Computer:		SAM-PC
 * Domain:			sam-pc
 * CreatedOn:		2014/8/19 12:15:10
 * Description:		Please input class summary
 * Version History:	Created,2014/8/19 12:15:10
 * 
 * 
 **************************************************************************/
using Sunnet.Cli.Core;
using Sunnet.Cli.Core.Reports.Entities;
using Sunnet.Cli.Core.Reports.Models;
using Sunnet.Cli.Core.Schools.Entities;
using Sunnet.Cli.Core.Schools.Interfaces;
using Sunnet.Framework;
using Sunnet.Framework.Core.Base;

namespace Sunnet.Cli.Impl.Schools
{
    public class SchoolRpst : EFRepositoryBase<SchoolEntity, Int32>, ISchoolRpst
    {
        public SchoolRpst(IUnitOfWork unit)
        {
            this.UnitOfWork = unit;

        }
        public List<SchoolSelectItemEntity> GetSchoolNameList(int communityId, string keyword)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(@"select A.ID,
                        ISNULL(B.Name,A.Name) as Name,
                        ISNULL(B.City,A.City) as City,
                        ISNULL(ISNULL(D.Name,C.Name),'') AS County,
                        ISNULL(B.PhysicalAddress1,A.Address1) AS [Address],
                        ISNULL(B.CountyId,a.CountyId) as CountyId,
                        ISNULL(B.StateId,a.StateId) as StateId,
                        ISNULL(ISNULL(B.Zip,A.Zip),'') as Zip,
                        ISNULL(ISNULL(B.PhoneNumber,A.phone),'') as Phone,
                        ISNULL(A.SchoolNumber,'') as SchoolNumber,
						ISNULL(ISNULL(E.Name,F.Name),'') AS [State]

                         from BasicSchools A left join Schools B ON A.ID = B.BasicSchoolId
                         LEFT JOIN Counties C ON A.CountyId = C.ID
                         LEFT JOIN Counties D ON B.CountyId = D.ID
						 LEFT JOIN States  E ON E.ID = B.StateId
						 LEFT JOIN States  F ON F.ID = A.StateId
                        WHERE  (A.BasicCommunityID = {0} OR 0 = {0} or A.BasicCommunityID=0)  and (A.Name like '%{1}%' or '{1}' = '-1')
                        ", communityId, keyword);
            EFUnitOfWorkContext context = this.UnitOfWork as EFUnitOfWorkContext;
            return context.DbContext.Database.SqlQuery<SchoolSelectItemEntity>(sb.ToString(), communityId, keyword).ToList();
        }

        /// <summary>
        /// get school by City Zip Name
        /// </summary>
        /// <param name="communityId"></param>
        /// <param name="keyword"></param>
        /// <returns></returns>
        public List<SchoolSelectItemEntity> GetSchoolListByKey(int communityId, string keyword)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(@"select A.ID,ISNULL(A.Name,A.Name) as Name,
                          ISNULL(A.City,'') as City,
                        ISNULL(C.Name,'') AS County,
                        ISNULL(A.Address1,'') AS [Address],
                        ISNULL(a.CountyId,0) as CountyId,
                        ISNULL(a.StateId,0) as StateId,
                        ISNULL(A.Zip,'') as Zip,
                        ISNULL(A.phone,'') as Phone,
                        ISNULL(A.SchoolNumber,'') as SchoolNumber,
					    ISNULL(D.Name,'') AS State
                         from BasicSchools A  
                         left join BasicCommunities B on A.BasicCommunityID=b.ID 
						 left join Communities E on E.BasicCommunityId=B.ID
                         LEFT JOIN Counties C ON A.CountyId = C.ID 
                         LEFT JOIN States D ON A.StateId = D.ID 
                        WHERE (E.ID = {0} OR 0 = {0} or E.ID=0) 
                        and (
                                 A.Name like '%{1}%' 
                                 or '{1}' = '-1'
                                 or (A.City like '%{1}%' and A.City is not null) 
                                 or (A.Zip like '%{1}%' and A.Zip is not null)
    
                            )
                        ", communityId, keyword);
            EFUnitOfWorkContext context = this.UnitOfWork as EFUnitOfWorkContext;
            return context.DbContext.Database.SqlQuery<SchoolSelectItemEntity>(sb.ToString(), communityId, keyword).ToList();
        }

        #region Service Report
        public List<ServiceReportModel> GetServiceReport(List<int> communityIds, List<int> schoolIds)
        {
            List<int> excludeCommunityIds = SFConfig.ExcludedCommunityForReport;
            if (communityIds.Any(e => excludeCommunityIds.Contains(e)))
            {
                excludeCommunityIds = new List<int>();
            }
            string strSql = string.Format("exec ServiceReport '{0}','{1}','{2}'",
                string.Join(",", communityIds),
                string.Join(",", excludeCommunityIds),
                string.Join(",", schoolIds));

            EFUnitOfWorkContext context = this.UnitOfWork as EFUnitOfWorkContext;
            List<ServiceReportModel> listCurrentSchoolReport =
                context.DbContext.Database.SqlQuery<ServiceReportModel>(strSql).ToList();
            return listCurrentSchoolReport;
        }
        #endregion
    }
}
