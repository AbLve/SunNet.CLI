using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		Damon
 * Computer:		Damon-PC
 * Domain:			Damon-pc
 * CreatedOn:		2014/8/25 14:36:23
 * Description:		Please input class summary
 * Version History:	Created,2014/8/25 14:36:23
 **************************************************************************/
using Sunnet.Cli.Core.Ade;
using Sunnet.Cli.Core.Students.Entities;
using Sunnet.Cli.Core.Students.Model;
using Sunnet.Cli.Core.Users.Enums;
using Sunnet.Framework.Core.Tool;

namespace Sunnet.Cli.Core.Students
{
    public interface IStudentContract
    {
        StudentEntity NewStudentEntity();
        OperationResult InsertStudent(StudentEntity entity);
        OperationResult DeleteStudent(int id);
        OperationResult UpdateStudent(StudentEntity entity);
        OperationResult UpdateStudents(List<StudentEntity> entities);
        StudentEntity GetStudent(int id);
        IQueryable<StudentEntity> Students { get; }
        OperationResult InvitationEmail(string to, string from, string subject, string emailBody, string attFile);

        List<StudentForCpallsModel> GetCpallsStudentModel(List<int> ids, string sort, string order);
        StudentForCpallsModel GetCpallsStudentModelForPlayMeasure(int id);
        StudentRoleEntity GetStudentRole(Role role);

        #region StudentDOB
        IQueryable<StudentDOBEntity> StudentDOBs { get; }
        OperationResult UpdateStudentDOB(StudentDOBEntity entity);
        OperationResult UpdateStudentDOBs(List<StudentDOBEntity> entities);
        #endregion

        #region Child
        IQueryable<ChildEntity> Children { get; }
        //IQueryable<V_ChildEntity> V_Children { get; }
        ChildEntity GetChild(int id);
        OperationResult InsertChild(ChildEntity entity);
        OperationResult UpdateChild(ChildEntity entity);
        #endregion

        #region  Parent Child Relation
        IQueryable<ParentChildEntity> ParentChilds { get; }
        OperationResult InsertParentChild(ParentChildEntity entity);
        #endregion
    }
}
