using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sunnet.Cli.Core.Common.Enums;
using Sunnet.Cli.Core.Reports.Entities;
using Sunnet.Cli.Core.Reports.Interfaces;
using Sunnet.Cli.Core.Students.Enums;
using Sunnet.Framework.Core.Base;
using Sunnet.Cli.Core.Reports.Models;
using Sunnet.Cli.Core;
using Sunnet.Cli.Core.Ade;
using Sunnet.Cli.Core.Users.Enums;
using Sunnet.Framework;

namespace Sunnet.Cli.Impl.Reports
{
    public class ReportQueueRpst : EFRepositoryBase<ReportQueueEntity, int>, IReportQueueRpst
    {
        public ReportQueueRpst(IUnitOfWork unit)
        {
            this.UnitOfWork = unit;
        }

        #region TSR Turnover Report / Teacher Turnover Report
        /// <summary>
        /// SchoolIds 为空 并且 communityIds 为空，表示管理员查看
        /// </summary>
        public List<TeacherSchoolTypeModel> GetTeacherBySchoolType(DateTime start, DateTime end, List<int> communityIds, List<int> schoolIds)
        {
            try
            {
                string condition = string.Empty;
                //非管理员
                if (communityIds != null || schoolIds != null)
                {
                    if (schoolIds != null)
                    {
                        if (schoolIds.Count == 0)
                            condition = " AND ucs.SchoolId =0 ";
                        else
                            condition = string.Format(" AND ucs.SchoolId in ({0}) ", string.Join(",", schoolIds));
                    }
                    else
                    {
                        if (communityIds.Count == 0)
                            condition = " AND ucs.CommunityId = 0 ";
                        else
                        {
                            if (communityIds.Count > 1)
                            {
                                condition = string.Format(" AND ucs.CommunityId in ({0}) ", string.Join(",", communityIds));
                                if (SFConfig.ExcludedCommunityForReport.Count > 0)
                                    condition += string.Format(" AND ucs.CommunityId not in ({0}) ", string.Join(",", SFConfig.ExcludedCommunityForReport));
                            }
                            else
                                condition = string.Format(" AND ucs.CommunityId = {0} ", communityIds[0]);
                        }
                    }
                }
                ///管理员
                if (communityIds == null)
                {
                    if (SFConfig.ExcludedCommunityForReport.Count > 0)
                        condition += string.Format(" AND ucs.CommunityId not in ({0}) ", string.Join(",", SFConfig.ExcludedCommunityForReport));
                }

                StringBuilder sql = new StringBuilder();

                sql.Append("with History as(")
                    .AppendFormat("  select * from TeacherStatusHistories where UpdatedOn >='{0}' and UpdatedOn < '{1}' )", start, end)
                    .Append(" ,maxIds as( ")
                    .AppendFormat(" select max(id) as Id from TeacherStatusHistories where UpdatedOn < '{0}' group by TeacherId )", start)
                    .Append(" , Actives as( ")
                    .Append(" select TeacherId,1 as Status from TeacherStatusHistories ")
                    .Append(" where id in (select Id from maxIds) and Status =1 ")
                    .Append(" and ID not in (select TeacherId from History)) ")
                    .Append(",newHistory as (select * from History ")
                    .Append(" union select 0,t.ID,2,u.CreatedOn from Teachers t inner join Users u on t.UserId = u.ID )")
                    .Append(" ,oneChange as( ")
                    .Append(" select max(id) as id from newHistory group by Teacherid having count(*) = 2) ")
                    .Append(" ,oneChangeList as (")
                    .Append(" select TeacherId,1 as Status from TeacherStatusHistories where id in (select id from oneChange) and Status = 2 ")
                    .Append(" union select TeacherId,2 as Status from TeacherStatusHistories where id in (select id from oneChange) and Status = 2 ")
                    .Append(" union select TeacherId,1 as Status from TeacherStatusHistories where id in (select id from oneChange) and Status = 1 )")
                    .Append(" , leastOneChange as(")
                    .Append(" 	select max(id) as id from newHistory group by Teacherid having count(*) > 2 ),")
                    .Append(" leastOneChangeList as (select TeacherId,1 as Status from TeacherStatusHistories where id in (select id from leastOneChange) and Status = 1 ")
                    .Append(" union select TeacherId,1 as Status from TeacherStatusHistories where id in (select id from leastOneChange) and Status = 2 ")
                    .Append(" union select TeacherId,2 as Status from TeacherStatusHistories where id in (select id from leastOneChange) and Status = 2 ) ")

                    .Append(" , List as ( ")
                    .Append(" select * from leastOneChangeList union select * from oneChangeList union select * from Actives )")
                    .Append(" select st.ID as SchoolTypeId ,st.Name as SchoolTypeName ,st.Status ,")
                    .Append(" ( select count(*) from Teachers t inner join UserComSchRelations ucs on ucs.UserId=t.UserId and ucs.SchoolId >0 inner join Users U on U.ID=t.UserId " +
                            "inner join Schools s on ucs.SchoolId = s.ID ")
                .AppendFormat(" where U.IsDeleted=0 and s.SchoolTypeId = st.ID and t.ID in (select TeacherId from List) {0} ) as Total,", condition)
                    .Append(" ( select count(*) from Teachers t inner join UserComSchRelations ucs on ucs.UserId=t.UserId and ucs.SchoolId >0 inner join Users U on U.ID=t.UserId " +
                            "inner join Schools s on ucs.SchoolId = s.ID ")
                .AppendFormat(" where U.IsDeleted=0 and s.SchoolTypeId = st.ID and t.ID in (select TeacherId from List where status = 1)  {0} ) as ActiveTotal,", condition)
                    .Append(" ( select count(*) from Teachers t inner join UserComSchRelations ucs on ucs.UserId=t.UserId and ucs.SchoolId >0 inner join Users U on U.ID=t.UserId " +
                            "inner join Schools s on ucs.SchoolId = s.ID ")
                .AppendFormat(" where U.IsDeleted=0 and s.SchoolTypeId = st.ID and t.ID in (select TeacherId from List where status = 2)  {0} ) as DroppedTotal ", condition)
                    .Append(" from SchoolTypes st where SUBSTRING(Name,1,4)!='Demo' and ParentId=0 order by Name ");

                EFUnitOfWorkContext context = this.UnitOfWork as EFUnitOfWorkContext;
                return context.DbContext.Database.SqlQuery<TeacherSchoolTypeModel>(sql.ToString()).ToList();
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }


        /// <summary>
        ///  SchoolIds 为空 并且 communityIds 为空，表示管理员查看
        /// </summary>
        public List<CountTeacherbyCommunityModel> GetCountTeacherbyCommunityList(DateTime start, DateTime end, List<int> communityIds, List<int> schoolIds)
        {
            try
            {
                StringBuilder sql = new StringBuilder();

                string condition = " 1=1 ";
                //非管理员
                if (communityIds != null || schoolIds != null)
                {
                    if (schoolIds != null)
                    {
                        if (schoolIds.Count == 0)
                            condition += " AND ucs.SchoolId = 0 ";
                        else
                            condition += string.Format(" AND ucs.SchoolId in ({0}) ", string.Join(",", schoolIds));
                    }
                    else
                    {
                        if (communityIds.Count == 0)
                            condition += " AND ucs.CommunityId = 0 ";
                        else
                        {
                            if (communityIds.Count == 1)
                                condition += string.Format(" AND ucs.CommunityId = {0} ", communityIds[0]);
                            else
                            {
                                condition += string.Format(" AND ucs.CommunityId in ({0}) ", string.Join(",", communityIds));
                                if (SFConfig.ExcludedCommunityForReport.Count > 0)
                                    condition += string.Format(" AND ucs.CommunityId not in ({0}) ", string.Join(",", SFConfig.ExcludedCommunityForReport));
                            }
                        }
                    }
                }
                if (communityIds == null)
                {
                    if (SFConfig.ExcludedCommunityForReport.Count > 0)
                        condition += string.Format(" AND  ucs.CommunityId not in ({0}) ", string.Join(",", SFConfig.ExcludedCommunityForReport));
                }

                sql.Append("with History as(")
                 .AppendFormat("  select * from TeacherStatusHistories where UpdatedOn >='{0}' and UpdatedOn < '{1}') ", start, end)
                 .Append(" ,maxIds as( ")
                 .AppendFormat(" select max(id) as Id from TeacherStatusHistories where UpdatedOn < '{0}' group by TeacherId )", start)
                 .Append(" , Actives as( ")
                 .Append(" select TeacherId,1 as Status from TeacherStatusHistories ")
                 .Append(" where id in (select Id from maxIds) and Status =1 ")
                 .Append(" and ID not in (select TeacherId from History)) ")
                 .Append(",newHistory as (select * from History ")
                 .Append(" union select 0,t.ID,2,u.CreatedOn from Teachers t 	inner join Users u on t.UserId = u.ID )")
                 .Append(" ,oneChange as( ")
                 .Append(" select max(id) as id from newHistory group by Teacherid having count(*) = 2) ")
                 .Append(" ,oneChangeList as (")
                 .Append(" select TeacherId,1 as Status from TeacherStatusHistories where id in (select id from oneChange) and Status = 2 ")
                 .Append(" union select TeacherId,2 as Status from TeacherStatusHistories where id in (select id from oneChange) and Status = 2 ")
                 .Append(" union select TeacherId,1 as Status from TeacherStatusHistories where id in (select id from oneChange) and Status = 1 )")
                 .Append(" , leastOneChange as(")
                 .Append(" 	select max(id) as id from newHistory group by Teacherid having count(*) > 2 ),")
                 .Append(" leastOneChangeList as (select TeacherId,1 as Status from TeacherStatusHistories where id in (select id from leastOneChange) and Status = 1 ")
                 .Append(" union select TeacherId,1 as Status from TeacherStatusHistories where id in (select id from leastOneChange) and Status = 2 ")
                 .Append(" union select TeacherId,2 as Status from TeacherStatusHistories where id in (select id from leastOneChange) and Status = 2 ) ")
                 .Append(", List as ( select * from leastOneChangeList union select * from oneChangeList union select * from Actives ) ")

                 .Append(" ,dataList as (select ucs.CommunityId , ISNULL(coachUser.ID,0 ) AS  CoachId ,CONVERT(tinyint, l.Status) as Status, COUNT(*) as Total  from list l ")
                 .Append(" inner join Teachers t on l.TeacherId = t.ID inner join UserComSchRelations ucs on ucs.UserId=t.UserId and ucs.SchoolId >0 inner join Users U on U.ID=t.UserId " +
                         "inner join Schools s on ucs.SchoolId = s.ID  ")
                 .Append(" left join SchoolTypes st on s.SchoolTypeId = st.ID")
                 .Append(" inner join Communities c on ucs.CommunityId = c.ID ")
                 .Append(" left join Users coachUser on t.CoachId = coachUser.ID and coachUser.Role  =35 ")
                 .Append(" and (select count(*) from UserComSchRelations where UserId=coachUser.ID and CommunityId=ucs.CommunityId) >0 ")
                 .AppendFormat(" where U.IsDeleted=0 and {0}  and SUBSTRING(st.Name,1,4)!='Demo' ", condition)
                 .Append(" group by ucs.CommunityId,coachUser.ID,l.Status )")

                 .Append(" select d.* ,bc.Name as CommunityName, ISNULL( u.FirstName,'') as CoachFirtName, ISNULL(u.LastName,'') as CoachLastName ")
                 .Append(" ,(select COUNT(*) from Teachers t inner join UserComSchRelations ucs on ucs.UserId=t.UserId and ucs.SchoolId >0 inner join Users U on U.ID=t.UserId " +
                         "inner join Schools s on ucs.SchoolId = s.ID ")
                 .Append(" left join SchoolTypes st on s.SchoolTypeId = st.ID ")
                 .Append(" left join Users cu on cu.ID = t.CoachId and cu.Role =35  ")
                 .Append(" and (select count(*) from UserComSchRelations where UserId=cu.ID and CommunityId=ucs.CommunityId) >0 ")
                 .Append(" where U.IsDeleted=0 and SUBSTRING(st.Name,1,4)!='Demo' and t.ID in (select TeacherId from list) and ucs.CommunityId = d.CommunityId and isnull(cu.id,0) = d.CoachId ) as AllTotal ")
                 .Append(" from dataList  d ")
                 .Append(" inner join Communities c on c.id = d.CommunityId  ")
                 .Append(" inner join BasicCommunities bc on bc.ID = c.BasicCommunityId  ")
                 .Append(" left join Users u on d.CoachId = u.ID ");

                EFUnitOfWorkContext context = this.UnitOfWork as EFUnitOfWorkContext;
                return context.DbContext.Database.SqlQuery<CountTeacherbyCommunityModel>(sql.ToString()).ToList();
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }
        #endregion

        /// <summary>
        /// TSR: Count by Facility Type
        /// </summary>
        /// <returns></returns>
        public List<CountbyFacilityTypeMode> GetCountbyFacilityTypeList(List<int> communityIds, List<int> fundingList,
            DateTime? startDate, DateTime? endDate, int status)
        {
            DateTime start = startDate ?? DateTime.Now;
            DateTime end = endDate ?? DateTime.Now;
            end = end.AddDays(1);
            try
            {
                StringBuilder sb = new StringBuilder();
                if (status > 0)
                {
                    //classroom 
                    sb.Append("with crStatusMax as (")
                        .AppendFormat("select max(id) as id from ClassroomStatusHistories where updatedon < '{0}' group by ClassroomId ", end.ToString("MM/dd/yyyy"))
                        .Append("                   ),")
                        .Append("crStatuslist as(")
                        .Append(" select h.* from ClassroomStatusHistories h ")
                        .Append(" inner join crStatusMax s on h.Id = s.ID ")
                        .Append(" ),")
                        .Append(" crInactiveList as ( ")
                        .AppendFormat(" select ClassroomId  from ClassroomStatusHistories where UpdatedOn >= '{0}' and UpdatedOn <'{1}' "
                        , start.ToString("MM/dd/yyyy"), end.ToString("MM/dd/yyyy"));
                    if (status == 1)
                        sb.Append(" and ClassroomId in (select ClassroomId from crStatuslist where Status =2) ");
                    else
                        sb.Append(" and ClassroomId in (select ClassroomId from crStatuslist where Status =1) ");
                    sb.Append(" and ClassroomId not in (select ClassroomId from ClassroomStatusHistories group by ClassroomId having count(*) = 1) ),")
                    .Append(" classroomIds as( ");
                    if (status == 1)
                        sb.Append(" select ClassroomId from crStatuslist where Status =1 ");
                    else
                        sb.Append(" select ClassroomId from crStatuslist where Status =2 ");
                    sb.Append(" union ")
                    .Append(" select classroomid from crInactiveList ")
                    .Append(" ), ClassroomTotals as ( ")
                    .Append(" select SchoolTypeId , Count(*) Total from Classrooms c inner join Schools s on c.SchoolId = s.ID " +
                            "inner join CommunitySchoolRelations csr on csr.SchoolId=s.ID ")
                            .Append(" where c.ID in (select * from classroomIds)  ");

                    if (fundingList.Count() > 0)
                        sb.AppendFormat(" and c.FundingId in ({0}) and s.FundingId in({0}) ", string.Join(",", fundingList));

                    if (communityIds.Count > 0)
                    {
                        if (communityIds.Count == 1)
                            sb.AppendFormat(" and csr.CommunityId = {0} ", communityIds[0]);
                        else
                            sb.AppendFormat(" and csr.CommunityId in ({0}) ", string.Join(",", communityIds.Except(SFConfig.ExcludedCommunityForReport)));
                    }
                    else
                    {
                        if (SFConfig.ExcludedCommunityForReport.Count > 0)
                            sb.AppendFormat(" and csr.CommunityId not in ({0}) ", string.Join(",", SFConfig.ExcludedCommunityForReport));
                    }
                    sb.Append(" group by s.SchoolTypeId  ");
                    sb.Append(")");

                    //Teacher
                    sb.Append(", tStatusMax as ( ")
                        .AppendFormat(" select max(id) as id from TeacherStatusHistories where updatedon < '{0}' group by TeacherId ", end.ToString("MM/dd/yyyy"))
                        .Append(" ), ")
                        .Append(" tStatuslist as( ")
                        .Append(" select h.* from TeacherStatusHistories h  ")
                        .Append(" inner join tStatusMax s on h.Id = s.ID ")
                        .Append(" ),")
                        .Append(" tInactiveList as ( ")
                        .AppendFormat(" select TeacherId  from TeacherStatusHistories where UpdatedOn >= '{0}' and UpdatedOn <'{1}' "
                        , start.ToString("MM/dd/yyyy"), end.ToString("MM/dd/yyyy"));
                    if (status == 1)
                        sb.Append(" and TeacherId in (select TeacherId from tStatuslist where Status =2)");
                    else
                        sb.Append(" and TeacherId in (select TeacherId from tStatuslist where Status =1)");
                    sb.Append(" and TeacherId not in (select TeacherId from TeacherStatusHistories group by TeacherId having count(*) = 1)), ")
                    .Append(" TeacherIds as( ");
                    if (status == 1)
                        sb.Append(" select TeacherId from tStatuslist where Status =1 ");
                    else
                        sb.Append(" select TeacherId from tStatuslist where Status =2 ");
                    sb.Append(" union ")
                    .Append(" select TeacherId from tInactiveList  ")
                    .Append(" ), ")
                    .Append(" TeacherTotals as( ")
                    .Append(" select SchoolTypeId , Count(*) Total from Teachers t inner join UserComSchRelations ucr " +
                            "on ucr.UserId=t.UserId and ucr.SchoolId>0 inner join Users U on U.ID=t.UserId inner join Schools s on ucr.schoolid =s.id ")
                    .Append("  where U.IsDeleted=0 and t.ID in (select * from TeacherIds) ");

                    if (fundingList.Count() > 0)
                        sb.AppendFormat(" and t.CLIFundingId in ({0}) and s.FundingId in({0}) ", string.Join(",", fundingList));

                    if (communityIds.Count > 0)
                    {
                        if (communityIds.Count == 1)
                            sb.AppendFormat(" and ucr.CommunityId = {0} ", communityIds[0]);
                        else
                            sb.AppendFormat(" and ucr.CommunityId in ({0}) ", string.Join(",", communityIds.Except(SFConfig.ExcludedCommunityForReport)));
                    }
                    else
                    {
                        if (SFConfig.ExcludedCommunityForReport.Count > 0)
                            sb.AppendFormat(" and ucr.CommunityId not in ({0}) ", string.Join(",", SFConfig.ExcludedCommunityForReport));
                    }

                    sb.Append("  group by s.SchoolTypeId ");
                    sb.Append(" ) ");

                    //Student
                    sb.Append(",sStatusMax as (")
                        .AppendFormat(" select max(id) as id from StudentStatusHistories where updatedon < '{0}' group by StudentId ", end.ToString("MM/dd/yyyy"))
                        .Append(" ),")
                        .Append(" sStatuslist as( ")
                        .Append(" select h.* from StudentStatusHistories h  ")
                        .Append(" inner join sStatusMax s on h.Id = s.ID ")
                        .Append(" ),")
                        .Append(" sInactiveList as (")
                        .AppendFormat(" select StudentId  from StudentStatusHistories  where UpdatedOn >= '{0}' and UpdatedOn <'{1}' "
                        , start.ToString("MM/dd/yyyy"), end.ToString("MM/dd/yyyy"));
                    if (status == 1)
                        sb.Append(" and StudentId in (select StudentId from sStatuslist where Status =2)");
                    else
                        sb.Append(" and StudentId in (select StudentId from sStatuslist where Status =1)");
                    sb.Append(" and studentid not in (select StudentId from StudentStatusHistories group by StudentId having count(*) = 1)),")
                    .Append(" StudentIds as(");
                    if (status == 1)
                        sb.Append(" select StudentId from sStatuslist where Status =1 ");
                    else
                        sb.Append(" select StudentId from sStatuslist where Status =2 ");
                    sb.Append(" union ")
                    .Append(" select StudentId from sInactiveList  ")
                    .Append(" ), ")
                    .Append(" StudentTotals as( ")
                    .Append(" select SchoolTypeId , Count(*) Total from Students t inner join SchoolStudentRelations ssr on ssr.StudentId=t.ID " +
                            "inner join Schools s on s.ID=ssr.SchoolId inner join CommunitySchoolRelations csr on csr.SchoolId=s.ID ")
                    .Append(" where t.IsDeleted=0 and t.ID in (select * from StudentIds) ");

                    if (fundingList.Count() > 0)
                        sb.AppendFormat(" and s.FundingId in({0}) ", string.Join(",", fundingList));
                    if (communityIds.Count > 0)
                    {
                        if (communityIds.Count == 1)
                            sb.AppendFormat(" and csr.CommunityId  = {0} ", communityIds[0]);
                        else
                            sb.AppendFormat(" and csr.CommunityId in ({0}) ", string.Join(",", communityIds.Except(SFConfig.ExcludedCommunityForReport)));
                    }
                    else
                    {
                        if (SFConfig.ExcludedCommunityForReport.Count > 0)
                            sb.AppendFormat(" and csr.CommunityId not in ({0}) ", string.Join(",", SFConfig.ExcludedCommunityForReport));
                    }

                    sb.Append("  group by s.SchoolTypeId ");

                    sb.Append(" ) ");

                    //School
                    sb.Append(",shStatusMax as (")
                        .AppendFormat(" select max(id) as id from SchoolStatusHistories where updatedon < '{0}' group by SchoolId ", end.ToString("MM/dd/yyyy"))
                        .Append(" ), ")
                        .Append(" shStatuslist as( ")
                        .Append(" select h.* from SchoolStatusHistories h ")
                        .Append(" inner join shStatusMax s on h.Id = s.ID ")
                        .Append(" ), ")
                        .Append(" shInactiveList as ( ")
                        .AppendFormat(" select SchoolId  from SchoolStatusHistories where UpdatedOn >= '{0}' and UpdatedOn <'{1}' "
                        , start.ToString("MM/dd/yyyy"), end.ToString("MM/dd/yyyy"));
                    if (status == 1)
                        sb.Append(" and SchoolId in (select SchoolId from shStatuslist where Status =2) ");
                    else
                        sb.Append(" and SchoolId in (select SchoolId from shStatuslist where Status =1) ");
                    sb.Append(" and SchoolId not in (select SchoolId from SchoolStatusHistories group by SchoolId having count(*) = 1)), ")
                    .Append(" SchoolIds as( ");
                    if (status == 1)
                        sb.Append(" select SchoolId from shStatuslist where Status =1 ");
                    else
                        sb.Append(" select SchoolId from shStatuslist where Status =2 ");
                    sb.Append(" union ")
                    .Append(" select SchoolId from shInactiveList  ")
                    .Append(" ), ")
                    .Append(" SchoolTotals as ( select SchoolTypeId , Count(*) Total from Schools s " +
                            "inner join CommunitySchoolRelations csr on csr.SchoolId=s.ID ")
                    .Append(" where s.ID in (select * from SchoolIds) ");

                    if (communityIds.Count > 0)
                    {
                        if (communityIds.Count == 1)
                            sb.AppendFormat(" and csr.CommunityId = {0} ", communityIds[0]);
                        else
                            sb.AppendFormat(" and csr.CommunityId in ({0}) ", string.Join(",", communityIds.Except(SFConfig.ExcludedCommunityForReport)));
                    }
                    else
                    {
                        if (SFConfig.ExcludedCommunityForReport.Count > 0)
                            sb.AppendFormat(" and csr.CommunityId not in ({0}) ", string.Join(",", SFConfig.ExcludedCommunityForReport));
                    }

                    if (fundingList.Count() > 0)
                        sb.AppendFormat(" and FundingId in({0}) ", string.Join(",", fundingList));
                    sb.Append("group by SchoolTypeId )")

                        //汇总
                        .Append(" select ID as SchoolTypeId,Name as SchoolType ,Status")
                        .Append(" ,isnull((select Total from ClassroomTotals where SchoolTypeId =st.ID),0) as ClassroomTotal ")
                        .Append(" ,isnull((select Total from TeacherTotals where SchoolTypeId =st.id),0) as TeacherTotal ")
                        .Append(" ,isnull((select Total from StudentTotals where SchoolTypeId = st.id),0) as StudentTotal ")
                        .Append(" ,isnull((select Total from SchoolTotals where SchoolTypeId = st.id),0) as SchoolTotal ")
                        .Append(" from SchoolTypes st ")
                        .Append(" where st.ParentId = 0 and SUBSTRING(Name,1,4)!='Demo' order by Name");
                }
                else
                {
                    sb.Append("select ID as SchoolTypeId,Name as SchoolType ")
                        .Append(" ,isnull((select count(*) from Classrooms  c inner join Schools s on c.schoolid =s.id inner join CommunitySchoolRelations csr " +
                                "on csr.SchoolId=s.ID where s.SchoolTypeId =st.ID ");

                    if (communityIds.Count > 0)
                    {
                        if (communityIds.Count == 1)
                            sb.AppendFormat(" and csr.CommunityId ={0} ", communityIds[0]);
                        else
                            sb.AppendFormat(" and csr.CommunityId in ({0}) ", string.Join(",", communityIds.Except(SFConfig.ExcludedCommunityForReport)));
                    }
                    else
                    {
                        if (SFConfig.ExcludedCommunityForReport.Count > 0)
                            sb.AppendFormat(" and csr.CommunityId not in ({0}) ", string.Join(",", SFConfig.ExcludedCommunityForReport));
                    }

                    if (fundingList.Count() > 0)
                        sb.AppendFormat(" and s.FundingId in({0}) ", string.Join(",", fundingList));

                    sb.Append(" ),0) as ClassroomTotal ")
                        .Append(" ,isnull((select count(*) from Teachers  c inner join UserComSchRelations ucr " +
                                "on ucr.UserId=c.UserId and ucr.SchoolId>0 inner join Users U on U.ID=c.UserId inner join Schools s on ucr.schoolid =s.id " +
                                "where U.IsDeleted and s.SchoolTypeId =st.ID ");

                    if (communityIds.Count > 0)
                    {
                        if (communityIds.Count == 1)
                            sb.AppendFormat(" and ucr.CommunityId ={0} ", communityIds[0]);
                        else
                            sb.AppendFormat(" and ucr.CommunityId in ({0}) ", string.Join(",", communityIds.Except(SFConfig.ExcludedCommunityForReport)));
                    }
                    else
                    {
                        if (SFConfig.ExcludedCommunityForReport.Count > 0)
                            sb.AppendFormat(" and ucr.CommunityId not in ({0}) ", string.Join(",", SFConfig.ExcludedCommunityForReport));
                    }

                    if (fundingList.Count() > 0)
                        sb.AppendFormat(" and s.FundingId in({0}) ", string.Join(",", fundingList));

                    sb.Append("),0) as TeacherTotal  ")
                        .Append(" ,isnull((select count(*) from Students c inner join SchoolStudentRelations ssr on ssr.StudentId=c.ID " +
                                "inner join Schools s on s.ID=ssr.SchoolId inner join CommunitySchoolRelations csr on csr.SchoolId=s.ID " +
                                "where c.IsDeleted=0 and s.SchoolTypeId =st.ID ");

                    if (communityIds.Count > 0)
                    {
                        if (communityIds.Count == 1)
                            sb.AppendFormat(" and csr.CommunityId ={0} ", communityIds[0]);
                        else
                            sb.AppendFormat(" and csr.CommunityId in ({0}) ", string.Join(",", communityIds.Except(SFConfig.ExcludedCommunityForReport)));
                    }
                    else
                    {
                        if (SFConfig.ExcludedCommunityForReport.Count > 0)
                            sb.AppendFormat(" and csr.CommunityId not in ({0}) ", string.Join(",", SFConfig.ExcludedCommunityForReport));
                    }

                    if (fundingList.Count() > 0)
                        sb.AppendFormat(" and s.FundingId in({0}) ", string.Join(",", fundingList));

                    sb.Append(" ),0) as StudentTotal ")
                        .Append(" ,isnull((select count(*) from Schools s inner join CommunitySchoolRelations csr on csr.SchoolId=s.ID where s.SchoolTypeId =st.ID ");

                    if (communityIds.Count > 0)
                    {
                        if (communityIds.Count == 1)
                            sb.AppendFormat(" and csr.CommunityId ={0} ", communityIds[0]);
                        else
                            sb.AppendFormat(" and csr.CommunityId in ({0}) ", string.Join(",", communityIds.Except(SFConfig.ExcludedCommunityForReport)));
                    }
                    else
                    {
                        if (SFConfig.ExcludedCommunityForReport.Count > 0)
                            sb.AppendFormat(" and csr.CommunityId not in ({0}) ", string.Join(",", SFConfig.ExcludedCommunityForReport));
                    }

                    if (fundingList.Count() > 0)
                        sb.AppendFormat(" and s.FundingId in({0}) ", string.Join(",", fundingList));

                    sb.Append(" ),0) as SchoolTotal  ,Status")
                         .Append(" from SchoolTypes st ")
                         .Append(" where st.ParentId = 0 and SUBSTRING(Name,1,4)!='Demo' order by Name");
                }

                EFUnitOfWorkContext context = this.UnitOfWork as EFUnitOfWorkContext;
                return context.DbContext.Database.SqlQuery<CountbyFacilityTypeMode>(sb.ToString()).ToList();
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }


        /// <summary>
        /// TSR: Count by Community
        /// </summary>
        /// <returns></returns>
        public List<CountbyCommunityModel> GetCountbyCommunityList(List<int> communityIds, List<int> fundingList,
            DateTime? startDate, DateTime? endDate, int status)
        {
            DateTime start = startDate ?? DateTime.Now;
            DateTime end = endDate ?? DateTime.Now;
            end = end.AddDays(1);
            try
            {
                StringBuilder sb = new StringBuilder();
                if (status > 0)
                {
                    //classroom 
                    sb.Append("with crStatusMax as (")
                        .AppendFormat("select max(id) as id from ClassroomStatusHistories where updatedon < '{0}' group by ClassroomId ", end.ToString("MM/dd/yyyy"))
                        .Append("                   ),")
                        .Append("crStatuslist as(")
                        .Append(" select h.* from ClassroomStatusHistories h ")
                        .Append(" inner join crStatusMax s on h.Id = s.ID ")
                        .Append(" ),")
                        .Append(" crInactiveList as ( ")
                        .AppendFormat(" select ClassroomId  from ClassroomStatusHistories where UpdatedOn >= '{0}' and UpdatedOn <'{1}' "
                        , start.ToString("MM/dd/yyyy"), end.ToString("MM/dd/yyyy"));
                    if (status == 1)
                        sb.Append(" and ClassroomId in (select ClassroomId from crStatuslist where Status =2) ");
                    else
                        sb.Append(" and ClassroomId in (select ClassroomId from crStatuslist where Status =1) ");
                    sb.Append(" and ClassroomId not in (select ClassroomId from ClassroomStatusHistories group by ClassroomId having count(*) = 1)),")
                    .Append(" classroomIds as( ");
                    if (status == 1)
                        sb.Append(" select ClassroomId from crStatuslist where Status =1 ");
                    else
                        sb.Append(" select ClassroomId from crStatuslist where Status =2 ");
                    sb.Append(" union ")
                    .Append(" select classroomid from crInactiveList ")
                    .Append(" ), ClassroomTotals as ( ")
                    .Append(" select csr.CommunityId ,cr.FundingId,count(*) as Total from Classrooms cr ")
                    .Append(" inner join Schools s on cr.SchoolId = s.ID inner join CommunitySchoolRelations csr on csr.SchoolId=s.ID ")
                    .Append(" left join SchoolTypes st on st.id = s.SchoolTypeId ")
                    .Append(" where SUBSTRING(st.Name,1,4)!='Demo'  and cr.id in (select * from classroomIds) ");

                    if (fundingList.Count() > 0)
                        sb.AppendFormat(" and cr.FundingId in ({0}) and s.FundingId in({0}) ", string.Join(",", fundingList));

                    if (communityIds.Count > 0)
                    {
                        if (communityIds.Count == 1)
                            sb.AppendFormat(" and csr.CommunityId = {0} ", communityIds[0]);
                        else
                        {
                            sb.AppendFormat(" and csr.CommunityId in ({0}) ", string.Join(",", communityIds.Except(SFConfig.ExcludedCommunityForReport)));
                        }
                    }
                    else
                    {
                        if (SFConfig.ExcludedCommunityForReport.Count > 0)
                            sb.AppendFormat(" and csr.CommunityId not in ({0}) ", string.Join(",", SFConfig.ExcludedCommunityForReport));
                    }

                    sb.Append(" group by csr.CommunityId, cr.FundingId ");
                    sb.Append(")");

                    //Teacher
                    sb.Append(", tStatusMax as ( ")
                        .AppendFormat(" select max(id) as id from TeacherStatusHistories where updatedon < '{0}' group by TeacherId ", end.ToString("MM/dd/yyyy"))
                        .Append(" ), ")
                        .Append(" tStatuslist as( ")
                        .Append(" select h.* from TeacherStatusHistories h  ")
                        .Append(" inner join tStatusMax s on h.Id = s.ID ")
                        .Append(" ),")
                        .Append(" tInactiveList as ( ")
                        .AppendFormat(" select TeacherId  from TeacherStatusHistories where UpdatedOn >= '{0}' and UpdatedOn <'{1}' "
                        , start.ToString("MM/dd/yyyy"), end.ToString("MM/dd/yyyy"));
                    if (status == 1)
                        sb.Append(" and TeacherId in (select TeacherId from tStatuslist where Status =2)");
                    else
                        sb.Append(" and TeacherId in (select TeacherId from tStatuslist where Status =1)");
                    sb.Append(" and TeacherId not in (select TeacherId from TeacherStatusHistories group by TeacherId having count(*) = 1)), ")
                    .Append(" TeacherIds as( ");
                    if (status == 1)
                        sb.Append(" select TeacherId from tStatuslist where Status =1 ");
                    else
                        sb.Append(" select TeacherId from tStatuslist where Status =2 ");
                    sb.Append(" union ")
                    .Append(" select TeacherId from tInactiveList  ")
                    .Append(" ), ")
                    .Append(" TeacherTotals as( ")
                    .Append(" select ucr.CommunityId, count(*) as Total from Teachers t ")
                    .Append(" inner join UserComSchRelations ucr on ucr.UserId=t.UserId and ucr.SchoolId>0 inner join Users U on U.ID=t.UserId " +
                            "inner join Schools s on ucr.schoolid =s.id ")
                    .Append(" left join SchoolTypes st on st.ID = s.SchoolTypeId ")
                    .Append(" where U.IsDeleted=0 and SUBSTRING(st.Name,1,4)!='Demo'  and t.id in (select * from TeacherIds) ");

                    if (fundingList.Count() > 0)
                        sb.AppendFormat(" and t.CLIFundingId in ({0}) and s.FundingId in({0}) ", string.Join(",", fundingList));
                    if (communityIds.Count > 0)
                    {
                        if (communityIds.Count == 1)
                            sb.AppendFormat(" and ucr.CommunityId = {0} ", communityIds[0]);
                        else
                        {
                            sb.AppendFormat(" and ucr.CommunityId in ({0}) ", string.Join(",", communityIds.Except(SFConfig.ExcludedCommunityForReport)));
                        }
                    }
                    else
                    {
                        if (SFConfig.ExcludedCommunityForReport.Count > 0)
                            sb.AppendFormat(" and ucr.CommunityId not in ({0}) ", string.Join(",", SFConfig.ExcludedCommunityForReport));
                    }

                    sb.Append(" group by ucr.CommunityId ");
                    sb.Append(" ) ");

                    //Student
                    sb.Append(",sStatusMax as (")
                        .AppendFormat(" select max(id) as id from StudentStatusHistories where updatedon < '{0}' group by StudentId ", end.ToString("MM/dd/yyyy"))
                        .Append(" ),")
                        .Append(" sStatuslist as( ")
                        .Append(" select h.* from StudentStatusHistories h  ")
                        .Append(" inner join sStatusMax s on h.Id = s.ID ")
                        .Append(" ),")
                        .Append(" sInactiveList as (")
                        .AppendFormat(" select StudentId  from StudentStatusHistories  where UpdatedOn >= '{0}' and UpdatedOn <'{1}' "
                        , start.ToString("MM/dd/yyyy"), end.ToString("MM/dd/yyyy"));
                    if (status == 1)
                        sb.Append(" and StudentId in (select StudentId from sStatuslist where Status =2)");
                    else
                        sb.Append(" and StudentId in (select StudentId from sStatuslist where Status =1)");
                    sb.Append(" and studentid not in (select StudentId from StudentStatusHistories group by StudentId having count(*) = 1)),")
                    .Append(" StudentIds as(");
                    if (status == 1)
                        sb.Append(" select StudentId from sStatuslist where Status =1 ");
                    else
                        sb.Append(" select StudentId from sStatuslist where Status =2 ");
                    sb.Append(" union ")
                    .Append(" select StudentId from sInactiveList  ")
                    .Append(" ), ")
                    .Append(" StudentTotals as( ")
                    .Append(" select csr.CommunityId, count(*) as Total from Students t ")
                    .Append(" inner join SchoolStudentRelations ssr on ssr.StudentId=t.ID inner join Schools s on s.ID=ssr.SchoolId " +
                            "inner join CommunitySchoolRelations csr on csr.SchoolId=s.ID ")
                    .Append(" left join SchoolTypes st on st.ID = s.SchoolTypeId ")
                    .Append(" where t.IsDeleted=0 and SUBSTRING(st.Name,1,4)!='Demo'  and t.id in (select * from StudentIds) ");

                    if (fundingList.Count() > 0)
                        sb.AppendFormat(" and s.FundingId in({0}) ", string.Join(",", fundingList));

                    if (communityIds.Count > 0)
                    {
                        if (communityIds.Count == 1)
                            sb.AppendFormat(" and csr.CommunityId = {0} ", communityIds[0]);
                        else
                        {
                            sb.AppendFormat(" and csr.CommunityId in ({0}) ", string.Join(",", communityIds.Except(SFConfig.ExcludedCommunityForReport)));
                        }
                    }
                    else
                    {
                        if (SFConfig.ExcludedCommunityForReport.Count > 0)
                            sb.AppendFormat(" and csr.CommunityId not in ({0}) ", string.Join(",", SFConfig.ExcludedCommunityForReport));
                    }

                    sb.Append(" group by csr.CommunityId ");

                    sb.Append(" ) ");

                    ///汇总

                    sb.Append(" select bc.Name as CommunityName , c.ID as CommunityId ")
                        .Append(" , isnull( t.Total,0) as TeacherTotal ")
                        .Append(" ,isnull(cr.Total,0) as ClassroomTotal ")
                        .Append(" ,isnull(s.Total,0) as StudentTotal ")
                        .Append(" ,ISNULL(cr.FundingId,0) as FundingId ")
                        .Append(" FROM Communities c  ")
                        .Append(" inner join BasicCommunities bc on c.BasicCommunityId = bc.ID  ")
                        .Append(" left join TeacherTotals t on t.CommunityId = c.ID  ")
                        .Append(" left join ClassroomTotals cr on cr.CommunityId = c.ID  ")
                        .Append(" left join StudentTotals s on s.CommunityId = c.ID  ");


                    if (communityIds.Count > 0)
                    {
                        if (communityIds.Count == 1)
                            sb.AppendFormat(" where c.ID = {0} ", communityIds[0]);
                        else
                        {
                            sb.AppendFormat(" where c.ID in ({0}) ", string.Join(",", communityIds.Except(SFConfig.ExcludedCommunityForReport)));
                        }
                    }
                    else
                    {
                        if (SFConfig.ExcludedCommunityForReport.Count > 0)
                            sb.AppendFormat(" where c.ID not in ({0}) ", string.Join(",", SFConfig.ExcludedCommunityForReport));
                    }
                }
                else
                {
                    //classroomTotals
                    sb.Append("with classroomTotals as ( ")
                        .Append(" select csr.CommunityId ,s.FundingId ,count(*) as Total from Classrooms c ")
                        .Append(" inner join Schools s on c.SchoolId = s.ID inner join CommunitySchoolRelations csr on csr.SchoolId=s.ID ")
                        .Append(" left join SchoolTypes st on st.id = s.SchoolTypeId ")
                        .Append(" where SUBSTRING(st.Name,1,4)!='Demo'  ");

                    if (fundingList.Count() > 0)
                        sb.AppendFormat(" and c.FundingId in ({0}) and s.FundingId in({0}) ", string.Join(",", fundingList));

                    if (communityIds.Count > 0)
                    {
                        if (communityIds.Count == 1)
                            sb.AppendFormat(" and csr.CommunityId = {0} ", communityIds[0]);
                        else
                        {
                            sb.AppendFormat(" and csr.CommunityId in ({0}) ", string.Join(",", communityIds.Except(SFConfig.ExcludedCommunityForReport)));
                        }
                    }
                    else
                    {
                        if (SFConfig.ExcludedCommunityForReport.Count > 0)
                            sb.AppendFormat(" and csr.CommunityId not in ({0}) ", string.Join(",", SFConfig.ExcludedCommunityForReport));
                    }

                    sb.Append(" group by csr.CommunityId ,s.FundingId ")
                    .Append(" ), ")

                    //TeacherTotals
                    .Append(" TeacherTotals as ( ")
                    .Append(" select ucr.CommunityId ,count(*) as Total from Teachers t ")
                    .Append(" inner join UserComSchRelations ucr on ucr.UserId=t.UserId and ucr.SchoolId>0 inner join Users U on U.ID=t.UserId " +
                            "inner join Schools s on ucr.schoolid =s.id ")
                    .Append(" left join SchoolTypes st on st.id = s.SchoolTypeId ")
                    .Append(" where U.IsDeleted=0 and SUBSTRING(st.Name,1,4)!='Demo' ");
                    if (fundingList.Count() > 0)
                        sb.AppendFormat(" and t.CLIFundingId in ({0}) and s.FundingId in({0}) ", string.Join(",", fundingList));

                    if (communityIds.Count > 0)
                    {
                        if (communityIds.Count == 1)
                            sb.AppendFormat(" and ucr.CommunityId = {0} ", communityIds[0]);
                        else
                        {
                            sb.AppendFormat(" and ucr.CommunityId in ({0}) ", string.Join(",", communityIds.Except(SFConfig.ExcludedCommunityForReport)));
                        }
                    }
                    else
                    {
                        if (SFConfig.ExcludedCommunityForReport.Count > 0)
                            sb.AppendFormat(" and ucr.CommunityId not in ({0}) ", string.Join(",", SFConfig.ExcludedCommunityForReport));
                    }

                    sb.Append(" group by ucr.CommunityId ")
                    .Append(" ),")

                    //StudentTotals
                    .Append(" StudentTotals as ( ")
                    .Append(" select csr.CommunityId ,count(*) as Total from Students t ")
                    .Append(" inner join SchoolStudentRelations ssr on ssr.StudentId=t.ID inner join Schools s on s.ID=ssr.SchoolId " +
                            "inner join CommunitySchoolRelations csr on csr.SchoolId=s.ID ")
                    .Append(" left join SchoolTypes st on st.id = s.SchoolTypeId ")
                    .Append(" where t.IsDeleted=0 and SUBSTRING(st.Name,1,4)!='Demo' ");

                    if (communityIds.Count > 0)
                    {
                        if (communityIds.Count == 1)
                            sb.AppendFormat(" and csr.CommunityId = {0} ", communityIds[0]);
                        else
                        {
                            sb.AppendFormat(" and csr.CommunityId in ({0}) ", string.Join(",", communityIds.Except(SFConfig.ExcludedCommunityForReport)));
                        }
                    }
                    else
                    {
                        if (SFConfig.ExcludedCommunityForReport.Count > 0)
                            sb.AppendFormat(" and csr.CommunityId not in ({0}) ", string.Join(",", SFConfig.ExcludedCommunityForReport));
                    }

                    sb.Append(" group by csr.CommunityId )");

                    //汇总
                    sb.Append(" select bc.Name as CommunityName , c.ID as CommunityId ")
                       .Append(" , isnull( t.Total,0) as TeacherTotal ")
                       .Append(" ,isnull(cr.Total,0) as ClassroomTotal ")
                       .Append(" ,isnull(s.Total,0) as StudentTotal ")
                       .Append(" ,ISNULL(cr.FundingId,0) as FundingId ")
                       .Append(" FROM Communities c  ")
                       .Append(" inner join BasicCommunities bc on c.BasicCommunityId = bc.ID  ")
                       .Append(" left join TeacherTotals t on t.CommunityId = c.ID  ")
                       .Append(" left join ClassroomTotals cr on cr.CommunityId = c.ID  ")
                       .Append(" left join StudentTotals s on s.CommunityId = c.ID  ");

                    if (communityIds.Count > 0)
                    {
                        if (communityIds.Count == 1)
                            sb.AppendFormat(" and c.ID = {0} ", communityIds[0]);
                        else
                        {
                            sb.AppendFormat(" and c.ID in ({0}) ", string.Join(",", communityIds.Except(SFConfig.ExcludedCommunityForReport)));
                        }
                    }
                    else
                    {
                        if (SFConfig.ExcludedCommunityForReport.Count > 0)
                            sb.AppendFormat(" and c.ID not in ({0}) ", string.Join(",", SFConfig.ExcludedCommunityForReport));
                    }
                }

                EFUnitOfWorkContext context = this.UnitOfWork as EFUnitOfWorkContext;
                return context.DbContext.Database.SqlQuery<CountbyCommunityModel>(sb.ToString()).ToList();
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }

        public List<CircleDataExportStudentModel> GetCircleDataExportStudentModels(int communityId, int schoolId, out int debugId)
        {
            var strSql = @"SELECT *   FROM  [V_Circle_Data_Export_Students] Where 
[CommunityId] = @CommunityId 
AND ([SchoolId] = @SchoolId OR @SchoolId = 0) ";
            var context = this.UnitOfWork as EFUnitOfWorkContext;
            debugId = 0;
            try
            {
                context.DbContext.Database.Connection.Open();
                var command = context.DbContext.Database.Connection.CreateCommand();
                command.CommandText = strSql;
                command.Parameters.Add(new SqlParameter("CommunityId", communityId));
                command.Parameters.Add(new SqlParameter("SchoolId", schoolId));
                command.CommandTimeout = 60 * 60; // 1H
                List<CircleDataExportStudentModel> list = new List<CircleDataExportStudentModel>();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (list == null)
                            list = new List<CircleDataExportStudentModel>();

                        var model = new CircleDataExportStudentModel();


                        debugId = int.Parse(reader["ID"].ToString());

                        model.CommunityId = int.Parse(reader["CommunityId"].ToString());
                        model.CommunityName = reader["CommunityName"].ToString();
                        model.CommunityIdentity = reader["CommunityIdentity"].ToString();
                        model.DistrictNumber = reader["DistrictNumber"].ToString();
                        model.SchoolId = int.Parse(reader["SchoolId"].ToString());
                        model.SchoolName = reader["SchoolName"].ToString();
                        model.SchoolIdentity = reader["SchoolIdentity"].ToString();

                        var schoolStatus = reader["SchoolStatus"].ToString();
                        if (schoolStatus == "0")
                            model.SchoolStatus = EntityStatus.Inactive;
                        else
                            model.SchoolStatus = (EntityStatus)byte.Parse(schoolStatus);

                        model.SchoolType = reader["SchoolType"].ToString();
                        model.SchoolNumber = reader["SchoolNumber"].ToString();
                        model.ID = int.Parse(reader["ID"].ToString());
                        model.FirstName = reader["FirstName"].ToString();
                        model.MiddleName = reader["MiddleName"].ToString();
                        model.LastName = reader["LastName"].ToString();
                        model.BirthDate = DateTime.Parse(reader["BirthDate"].ToString());

                        var studentStatus = reader["StudentStatus"].ToString();
                        if (studentStatus == "0")
                            model.StudentStatus = EntityStatus.Inactive;
                        else
                            model.StudentStatus = (EntityStatus)byte.Parse(studentStatus);

                        model.StudentIdentity = reader["StudentIdentity"].ToString();
                        model.LocalStudentID = reader["LocalStudentID"].ToString();
                        model.TSDSStudentID = reader["TSDSStudentID"].ToString();
                        model.AssessmengLanguage =
                            (StudentAssessmentLanguage) byte.Parse(reader["AssessmentLanguage"].ToString());
                        model.StudentGender =
                            (Gender)byte.Parse(reader["StudentGender"].ToString());
                        model.StudentEthnicity =
                            (Ethnicity)byte.Parse(reader["Ethnicity"].ToString());
                        model.StudentEthnicityOther = reader["EthnicityOther"].ToString();
                        model.TSDSStudentID = reader["TSDSStudentID"].ToString();
                        model.TSDSStudentID = reader["TSDSStudentID"].ToString();
                        model.TSDSStudentID = reader["TSDSStudentID"].ToString();

                        model.Teachers = reader["Teachers"] == DBNull.Value ? null : reader["Teachers"].ToString();
                        model.Classes = reader["Classes"] == DBNull.Value ? null : reader["Classes"].ToString();

                        var gradelevel = reader["GradeLevel"].ToString();
                        if (gradelevel == "0")
                            model.GradeLevel = StudentGradeLevel.Prek;
                        else
                            model.GradeLevel = (StudentGradeLevel)byte.Parse(gradelevel);

                        list.Add(model);
                    }
                }
                return list;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                context.DbContext.Database.Connection.Close();
            }
            //return context.DbContext.Database.SqlQuery<CircleDataExportStudentModel>(strSql
            //    , new SqlParameter("CommunityId", communityId)
            //    , new SqlParameter("SchoolId", schoolId)
            //    ).ToList();
        }

        /// <summary>
        ///TSR: Coaching Hours by Community 
        /// </summary>
        /// <returns></returns>
        public List<CoachingHoursbyCommunityModel> GetCoachingHoursbyCommunityModel()
        {
            try
            {
                EFUnitOfWorkContext context = this.UnitOfWork as EFUnitOfWorkContext;
                return context.DbContext.Database.SqlQuery<CoachingHoursbyCommunityModel>("exec CoachingHoursbyCommunity").ToList();
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }

        public List<TurnoverTeacherModel> GetTurnoverTeacherModels(DateTime start, DateTime end, List<int> communities,
            List<int> schools)
        {
            /* debug parameters
             DECLARE @startDate DATETIME ;
DECLARE @endDate DATETIME;
SET @startDate = '2014-1-1';
SET @endDate = '2015-9-9';
             */

            var communityCondition = "";
            if (communities != null)
            {
                if (communities.Count == 0)
                    communityCondition = " AND 1 = 0 ";
                else if (communities.Count == 1)
                    communityCondition = string.Format(" AND UCS.CommunityId = {0}", communities[0]);
                else

                    communityCondition = " AND UCS.CommunityId IN (" +
                                         string.Join(",", communities.Except(SFConfig.ExcludedCommunityForReport)) +
                                         " ) ";
            }
            else
            {
                if (SFConfig.ExcludedCommunityForReport.Count > 0)
                    communityCondition = string.Format(" AND UCS.CommunityId not in ({0}) ",
                        string.Join(",", SFConfig.ExcludedCommunityForReport));
            }

            var schoolCondition = "";
            if (schools != null)
            {
                schoolCondition = schools.Any()
                    ? " AND UCS.SchoolId IN (" + string.Join(",", schools) + " ) "
                    : " AND 1 = 0 ";
            }

            string strSql = string.Format(@"WITH Modified AS (
SELECT H1.* ,USR.CreatedOn FROM [TeacherStatusHistories] H1 
INNER JOIN DBO.Teachers TEA ON H1.TeacherId = TEA.ID 
inner join DBO.UserComSchRelations UCS on UCS.UserId=TEA.UserId and UCS.SchoolId>0
INNER JOIN DBO.Users USR ON TEA.UserId = USR.ID 
WHERE 1=1 {0} {1} AND H1.UpdatedOn BETWEEN @startDate AND @endDate AND DATEDIFF(SECOND, USR.CreatedOn, H1.UpdatedOn) > 60
)
 --SELECT * FROM Modified
SELECT  [CommunityName] = BCM.Name, 
[SchoolName] = BSH.Name, [SchoolType] = SCT.Name, [SchoolStatus] = SCH.[Status], 
USR.FirstName,USR.LastName ,T.TeacherId,T.TeacherType, T.TeacherTypeOther, T.CoachAssignmentWay, T.CoachAssignmentWayOther ,T.YearsInProjectId, YIP.YearsInProject,
SCH.PrimaryEmail , USR.PrimaryEmailAddress,[TeacherStatus] = USR.[Status], [InactiveDate] = D.EndDate,[CreationDate] = d.CreatedOn
FROM (
SELECT DISTINCT TeacherId
,StartStatus = (
SELECT TOP 1 [Status] FROM Modified M2 WHERE M2.TeacherId = M1.TeacherId ORDER BY ID ASC
)
,EndStatus = (
SELECT TOP 1 [Status] FROM Modified M2 WHERE M2.TeacherId = M1.TeacherId ORDER BY ID DESC
)
,EndDate = (
SELECT TOP 1 UpdatedOn FROM Modified M2 WHERE M2.TeacherId = M1.TeacherId ORDER BY ID DESC
)
,Times = (
SELECT COUNT(1) FROM Modified M3 WHERE M3.TeacherId = M1.TeacherId
),
m1.CreatedOn
FROM Modified M1
) D 
INNER JOIN DBO.Teachers T ON D.TeacherId = T.ID 
inner join UserComSchRelations UCS on T.UserId=UCS.UserId and UCS.SchoolId>0
INNER JOIN DBO.Users USR ON T.UserId = USR.ID
INNER JOIN DBO.Communities COM ON UCS.CommunityId = COM.ID
INNER JOIN DBO.BasicCommunities BCM ON COM.BasicCommunityId = BCM.ID
INNER JOIN DBO.Schools SCH ON UCS.SchoolId = SCH.ID 
INNER JOIN DBO.BasicSchools BSH ON SCH.BasicSchoolId = BSH.ID
LEFT JOIN DBO.SchoolTypes SCT ON SCH.SchoolTypeId = SCT.ID AND SCT.ID > 0
LEFT JOIN DBO.YearsInProjects YIP ON T.YearsInProjectId = YIP.ID

WHERE D.EndStatus = 2 and  SUBSTRING(SCT.Name,1,4)!='Demo' order by BSH.Name",
                communityCondition,
                schoolCondition
                );

            EFUnitOfWorkContext context = this.UnitOfWork as EFUnitOfWorkContext;
            return context.DbContext.Database.SqlQuery<TurnoverTeacherModel>(strSql,
                new SqlParameter("startDate", start),
                new SqlParameter("endDate", end)
                ).ToList();
        }
    }
}