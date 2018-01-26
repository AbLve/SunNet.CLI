using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		Leason
 * Computer:		Leason-PC
 * Domain:			Leason-pc
 * CreatedOn:		2014/8/22 14:37:20
 * Description:		Create IClassContract
 * Version History:	Created,2014/8/22 14:37:20
 * 
 * 
 **************************************************************************/
using Sunnet.Cli.Core.Classes.Entites;
using Sunnet.Cli.Core.Classrooms.Entites;
using Sunnet.Cli.Core.Communities.Entities;
using Sunnet.Cli.Core.Schools.Entities;
using Sunnet.Cli.Core.Users.Enums;
using Sunnet.Framework.Core.Tool;

namespace Sunnet.Cli.Core.Classes
{
    public interface IClassContract
    {

        IQueryable<ClassEntity> Classes { get; }
        IQueryable<MonitoringToolEntity> MonitoringTools { get; }
        IQueryable<CommunityEntity> Communities { get; }
        IQueryable<SchoolEntity> Schools { get; }
        IQueryable<ClassroomEntity> Classrooms { get; }
        IQueryable<ClassroomClassEntity> ClassroomClassRelations { get; }
        IQueryable<ClassLevelEntity> ClasseLevels { get; }

        ClassEntity NewClassEntity();
        OperationResult InsertClass(ClassEntity entity, bool isSave = true);
        OperationResult DeleteClass(int id);
        OperationResult UpdateClass(ClassEntity entity, bool isSave = true);
        ClassEntity GetClass(int id);

        ClassRoleEntity GetClassRole(Role role);

        MonitoringToolEntity NewMonitoringToolEntity();
        OperationResult InsertMonitoringTool(MonitoringToolEntity entity);
        OperationResult UpdateMonitoringTool(MonitoringToolEntity entity);
        MonitoringToolEntity GetMonitoringTool(int id);

        OperationResult UpdateClassPlayground(int playgroundId, int[] classIds = null);

        #region Class Classroom  Relation
        OperationResult InsertRelations(IList<ClassroomClassEntity> list);

        OperationResult DeleteRelations(IList<ClassroomClassEntity> list);
        #endregion
    }
}
