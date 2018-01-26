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
 * Description:		Create IClassroomContract
 * Version History:	Created,2014/8/22 14:37:20
 * 
 * 
 **************************************************************************/
using Sunnet.Cli.Core.Classrooms.Entites;
using Sunnet.Cli.Core.Users.Enums;
using Sunnet.Framework.Core.Tool;

namespace Sunnet.Cli.Core.Classrooms
{
    public interface IClassroomContract
    {

        IQueryable<ClassroomEntity> Classrooms { get; }
        ClassroomEntity NewClassroomEntity();
        OperationResult InsertClassroom(ClassroomEntity entity);
        OperationResult DeleteClassroom(int id);
        OperationResult UpdateClassroom(ClassroomEntity entity);
        ClassroomEntity GetClassroom(int id);

        IQueryable<KitEntity> Kits { get; }
        KitEntity NewKitEntity();
        OperationResult InsertKit(KitEntity entity);
        OperationResult DeleteKit(int id);
        OperationResult UpdateKit(KitEntity entity);
        KitEntity GetKit(int id);
        ClassroomRoleEntity GetRole(Role role);
    }
}
