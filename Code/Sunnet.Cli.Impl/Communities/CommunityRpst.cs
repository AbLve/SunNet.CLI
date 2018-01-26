using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		Leason
 * Computer:		Leason-PC
 * Domain:			Leason-pc
 * CreatedOn:		2014/8/18 16:27:20
 * Description:		Create CommunitiesRspt
 * Version History:	Created,2014/8/18 16:27:20
 * 
 * 
 **************************************************************************/
using Sunnet.Cli.Core;
using Sunnet.Cli.Core.Common.Enums;
using Sunnet.Cli.Core.Communities.Entities;
using Sunnet.Cli.Core.Communities.Interfaces;
using Sunnet.Framework.Core.Base;
using Sunnet.Cli.Core.Vcw.Entities;
using Sunnet.Framework.Core.Extensions;

namespace Sunnet.Cli.Impl.Communities
{
    public class CommunityRpst : EFRepositoryBase<CommunityEntity, Int32>, ICommunityRpst
    {
        public CommunityRpst(IUnitOfWork unit)
        {
            UnitOfWork = unit;
        }
        public bool InactiveEntities_old(ModelName model, int entityId, EntityStatus status, string fundingYear)
        {
            string sqlStr = "";
            switch (model)
            {
                case ModelName.Community:
                    sqlStr = string.Format(@"
                            update Users         set Status='{1}'  where id in (select userid from teachers where schoolid in (Select ID  from Schools  where CommunityId={0}) and SchoolYear='{2}')  
                            update students      set Status='{1}'  where ID in (Select studentId  from studentclassRelations  where classID   in (Select ID from classes where classroomID in (Select ID  from classrooms  where SchoolID in (Select ID from Schools where CommunityID={0}))))
                            update classes       set Status='{1}'  where classroomID  in (Select ID  from classrooms where SchoolID  in (Select ID from Schools where CommunityID={0}))  and SchoolYear='{2}'
                            update classrooms    set Status='{1}'  where SchoolID     in (Select ID from Schools where CommunityID={0})  and SchoolYear='{2}'
                            update Communities   set Status='{1}'  where ID={0} 
                            update schools       set Status='{1}'  where CommunityID={0}  and SchoolYear='{2}'",
                        entityId, (int)status, fundingYear);
                    if (status == EntityStatus.Inactive)
                    {
                        string deleteStr = String.Format(@"DELETE FROM TeacherClassRelations WHERE classId in
                                             (SELECT [ID] FROM [Classes] WHERE CommunityId ={0})", entityId);
                        sqlStr = sqlStr + deleteStr;
                    }
                    break;
                case ModelName.School:
                    sqlStr = string.Format(@"
                            update users         set Status='{1}'  where id in (select userid from teachers where SchoolID ={0}  and SchoolYear='{2}')
                            update students      set Status='{1}'  where ID in (Select studentId  from studentclassRelations  where classID   in (Select ID from classes where classroomID in (Select ID  from classrooms  where SchoolID ={0})))
                            update classes       set Status='{1}'  where classroomID  in (Select ID  from classrooms where SchoolID  ={0})  and SchoolYear='{2}'
                            update classrooms    set Status='{1}'  where SchoolID ={0} and SchoolYear='{2}'
                            update schools       set Status='{1}'  where ID={0}  and SchoolYear='{2}'
                        ",
                        entityId, (int)status, fundingYear);
                    if (status == EntityStatus.Inactive)
                    {
                        string deleteStr = String.Format(@"DELETE FROM TeacherClassRelations WHERE classId in
                                             (SELECT [ID] FROM [Classes] WHERE SchoolId ={0})", entityId);
                        sqlStr = sqlStr + deleteStr;
                    }
                    break;
                case ModelName.Classroom:
                    sqlStr = string.Format(@"
                            update students      set Status='{1}'  where ID in (Select studentId  from studentclassRelations  where classID in (Select ID from classes where classroomID ={0}))
                            update classes       set Status='{1}'  where classroomID  ={0}  and SchoolYear='{2}'
                            update classrooms    set Status='{1}'  where ID ={0} and SchoolYear='{2}'
                        ",
                        entityId, (int)status, fundingYear);
                    break;
                case ModelName.Class:
                    sqlStr = string.Format(@"
                            update students set Status='{1}'  where ID in (Select studentId  from studentclassRelations  where classID = {0})
                            update classes set Status='{1}'  where ID  ={0}  and SchoolYear='{2}'
                        ",
                        entityId, (int)status, fundingYear);
                    break;
                case ModelName.Teacher:
                    if (status == EntityStatus.Inactive)
                    {
                        string deleteStr = String.Format(@"DELETE FROM TeacherClassRelations WHERE [TeacherId] ={0}", entityId);
                        sqlStr = sqlStr + deleteStr;
                    }
                    break;
            }
            EFUnitOfWorkContext context = this.UnitOfWork as EFUnitOfWorkContext;
            //不能在此Dispose掉context，因为执行过此方法后，还会用到context
            return context.DbContext.Database.ExecuteSqlCommand(sqlStr) > 0;
        }

        public bool InactiveEntities(ModelName model, int entityId, EntityStatus status, string fundingYear)
        {
            string sqlStr = "";
            switch (model)
            {
                case ModelName.Community:
                    if (status == EntityStatus.Inactive)
                    {
                        sqlStr = "exec dbo.DeleleCommunityUserRelations " + entityId;
                    }
                    break;
                case ModelName.School:
                     if (status == EntityStatus.Inactive)
                    {
                        sqlStr = "exec dbo.DeleleSchoolUserRelations " + entityId;
                    }
                    break;
                case ModelName.Classroom:
                     if (status == EntityStatus.Inactive)
                    {
                        sqlStr = "exec dbo.DeleleClassroomClassRelations " + entityId;
                    }
                    break;
                case ModelName.Class:
                     if (status == EntityStatus.Inactive)
                    {
                        sqlStr = "exec dbo.DeleleStudentClassRelations " + entityId;
                    }
                    break;
                case ModelName.Teacher:
                    
                    break;
            }
            EFUnitOfWorkContext context = this.UnitOfWork as EFUnitOfWorkContext;
            //不能在此Dispose掉context，因为执行过此方法后，还会用到context
            return context.DbContext.Database.ExecuteSqlCommand(sqlStr)>0;
            //return context.DbContext.Database.ExecuteSqlCommand(sqlStr) > 0;
        }
    }
}
