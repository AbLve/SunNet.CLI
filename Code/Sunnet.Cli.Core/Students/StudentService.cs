using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
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
using StructureMap;
using Sunnet.Cli.Core.Ade;
using Sunnet.Cli.Core.Students.Configurations;
using Sunnet.Cli.Core.Students;
using Sunnet.Cli.Core.Students.Entities;
using Sunnet.Cli.Core.Students.Interfaces;
using Sunnet.Cli.Core.Students.Model;
using Sunnet.Framework.Core.Base;
using Sunnet.Framework.Core.Tool;
using Sunnet.Framework.EmailSender;
using Sunnet.Framework.Encrypt;
using Sunnet.Framework.File;
using Sunnet.Framework.Log;
using Sunnet.Cli.Core.Users.Enums;


namespace Sunnet.Cli.Core.Students
{
    internal class StudentService : CoreServiceBase, IStudentContract
    {
        private readonly IStudentRpst _studentRpst;
        private readonly IStudentRoleRpst _studentRoleRpst;
        private readonly IStudentDOBRpst _studentDOBRpst;

        private readonly IChildRpst _childRpst;
        //private readonly IV_ChildRpst _v_ChildRpst;
        private readonly IParentChildRpst _parentChildRpst;

        public StudentService(
            ISunnetLog log,
            IFile fileHelper,
            IEmailSender emailSender,
            IEncrypt encrypt,
            IStudentRpst studentRpst,
            IStudentRoleRpst studentRoleRpst,
            IStudentDOBRpst studentDOBRpst,
            IChildRpst childRpst,
            //IV_ChildRpst v_ChildRpst,
            IParentChildRpst parentChildRpst,
            IUnitOfWork unit
            )
            : base(log, fileHelper, emailSender, encrypt)
        {
            _studentRpst = studentRpst;
            _studentRoleRpst = studentRoleRpst;
            _studentDOBRpst = studentDOBRpst;
            _childRpst = childRpst;
            //_v_ChildRpst = v_ChildRpst;
            _parentChildRpst = parentChildRpst;
            UnitOfWork = unit;
        }

        public IQueryable<StudentEntity> Students
        {
            get { return _studentRpst.Entities.Where(e => e.IsDeleted == false); }
        }

        public StudentEntity NewStudentEntity()
        {
            return _studentRpst.Create();
        }

        public OperationResult InsertStudent(StudentEntity entity)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                _studentRpst.Insert(entity);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult DeleteStudent(int id)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                _studentRpst.Delete(id);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult UpdateStudent(StudentEntity entity)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                _studentRpst.Update(entity);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult UpdateStudents(List<StudentEntity> entities)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                entities.ForEach(e => _studentRpst.Update(e, false));
                UnitOfWork.Commit();
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public StudentEntity GetStudent(int id)
        {
            return _studentRpst.GetByKey(id);
        }
        public OperationResult InvitationEmail(string to, string from, string subject, string emailBody, string attFile)
        {

            var result = new OperationResult(OperationResultType.Success);
            try
            {
                var emailSender = ObjectFactory.GetInstance<IEmailSender>();
                new SendHandler(() => emailSender.SendMail(to, from, subject, emailBody, attFile)).BeginInvoke(null, null);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }
        private delegate void SendHandler();

        public List<StudentForCpallsModel> GetCpallsStudentModel(List<int> ids, string sort, string order)
        {
            return _studentRpst.GetCpallsStudentModel(ids, sort, order);
        }
        public StudentForCpallsModel GetCpallsStudentModelForPlayMeasure(int id)
        {
            return _studentRpst.GetStudentForPlayMeasure(id);
        }
        public StudentRoleEntity GetStudentRole(Role role)
        {
            return _studentRoleRpst.Entities.FirstOrDefault(r => r.RoleId == (int)role);
        }

        #region StudentDOB
        public IQueryable<StudentDOBEntity> StudentDOBs
        {
            get
            {
                return _studentDOBRpst.Entities;
            }
        }

        public OperationResult UpdateStudentDOB(StudentDOBEntity entity)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                _studentDOBRpst.Update(entity);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult UpdateStudentDOBs(List<StudentDOBEntity> entities)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                entities.ForEach(e => _studentDOBRpst.Update(e, false));
                UnitOfWork.Commit();
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }
        #endregion

        #region Child
        public IQueryable<ChildEntity> Children
        {
            get
            {
                return _childRpst.Entities.Where(c=>c.IsDeleted==false);
            }
        }

        //public IQueryable<V_ChildEntity>V_Children
        //{
        //    get
        //    {
        //        return _v_ChildRpst.Entities;
        //    }
        //}
        public ChildEntity GetChild(int id)
        {
            var child = _childRpst.GetByKey(id);
            if (child != null && child.IsDeleted)
                return null;
            else
                return child;
        }

        public OperationResult InsertChild(ChildEntity entity)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                _childRpst.Insert(entity);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult UpdateChild(ChildEntity entity)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                _childRpst.Update(entity);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }
        #endregion

        #region  Parent Child Relation
        public IQueryable<ParentChildEntity> ParentChilds
        {
            get
            {
                return _parentChildRpst.Entities.Where(c=>c.Child.IsDeleted == false);
            }
        }

        public OperationResult InsertParentChild(ParentChildEntity entity)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                _parentChildRpst.Insert(entity);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }
        #endregion
    }
}
