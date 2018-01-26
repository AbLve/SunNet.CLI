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
 * Description:		Create ClassroomService
 * Version History:	Created,2014/8/22 14:37:20
 * 
 * 
 **************************************************************************/
using Sunnet.Cli.Core.Classrooms.Entites;
using Sunnet.Cli.Core.Classrooms.Interfaces;
using Sunnet.Cli.Core.Users.Enums;
using Sunnet.Framework.Core.Base;
using Sunnet.Framework.Core.Tool;
using Sunnet.Framework.EmailSender;
using Sunnet.Framework.Encrypt;
using Sunnet.Framework.File;
using Sunnet.Framework.Log;

namespace Sunnet.Cli.Core.Classrooms
{
    internal class ClassroomService: CoreServiceBase, IClassroomContract
    {
        private readonly IClassroomRpst ClassroomRpst;
        private readonly IKitRpst KitRpst;
        private readonly IClassroomRoleRpst RoleRpst;

        public ClassroomService(ISunnetLog log, IFile fileHelper, IEmailSender emailSender, IEncrypt encrypt,
            IClassroomRpst classroomRpst,
            IKitRpst kitRpst,
            IClassroomRoleRpst roleRpst,
          
            IUnitOfWork unit):base(log, fileHelper, emailSender, encrypt)
        {
            UnitOfWork = unit;
            ClassroomRpst = classroomRpst;
            KitRpst = kitRpst;
            RoleRpst = roleRpst;         
        }

        #region Classroom

        public IQueryable<ClassroomEntity> Classrooms
        {
            get { return ClassroomRpst.Entities; }
        }

        public ClassroomEntity NewClassroomEntity()
        {
            return ClassroomRpst.Create();
        }

        public OperationResult InsertClassroom(ClassroomEntity entity)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                ClassroomRpst.Insert(entity);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;            
        }

        public OperationResult DeleteClassroom(int id)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                ClassroomRpst.Delete(id);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult UpdateClassroom(ClassroomEntity entity)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                ClassroomRpst.Update(entity);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public ClassroomEntity GetClassroom(int id)
        {
            return ClassroomRpst.GetByKey(id);
        }

        #endregion

        #region Kit

        public IQueryable<KitEntity> Kits
        {
            get { return KitRpst.Entities; }
        }

        public KitEntity NewKitEntity()
        {
            return KitRpst.Create();
        }

        public OperationResult InsertKit(KitEntity entity)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                KitRpst.Insert(entity);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult DeleteKit(int id)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                KitRpst.Delete(id);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult UpdateKit(KitEntity entity)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                KitRpst.Update(entity);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public KitEntity GetKit(int id)
        {
            return KitRpst.GetByKey(id);
        }

        #endregion

        #region Classroom Role
        public ClassroomRoleEntity GetRole(Role role)
        {
            return RoleRpst.Entities.FirstOrDefault(o => o.RoleId == (int)role);
        }
        #endregion

     
    }
}
