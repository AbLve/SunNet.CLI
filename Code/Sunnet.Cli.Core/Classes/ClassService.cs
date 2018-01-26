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
 * Description:		Create ClassCnfg
 * Version History:	Created,2014/8/22 14:37:20
 * 
 * 
 **************************************************************************/
using Sunnet.Cli.Core.Classes.Entites;
using Sunnet.Cli.Core.Classes.Interfaces;
using Sunnet.Cli.Core.Classrooms;
using Sunnet.Cli.Core.Classrooms.Entites;
using Sunnet.Cli.Core.Classrooms.Interfaces;
using Sunnet.Cli.Core.Common.Enums;
using Sunnet.Cli.Core.Communities.Entities;
using Sunnet.Cli.Core.Communities.Interfaces;
using Sunnet.Cli.Core.MasterData.Entities;
using Sunnet.Cli.Core.Schools.Entities;
using Sunnet.Cli.Core.Schools.Interfaces;
using Sunnet.Cli.Core.Users.Enums;
using Sunnet.Framework.Core.Base;
using Sunnet.Framework.Core.Tool;
using Sunnet.Framework.EmailSender;
using Sunnet.Framework.Encrypt;
using Sunnet.Framework.File;
using Sunnet.Framework.Log;

namespace Sunnet.Cli.Core.Classes
{
    internal class ClassService : CoreServiceBase, IClassContract
    {
        private readonly IClassRpst _classRpst;
        private readonly IClassLevelRpst _classLevelRpst;
        private readonly IMonitoringToolRpst _monitoringToolRpst;
        private readonly ICommunityRpst _communityRpst;
        private readonly ISchoolRpst _schoolRpst;
        private readonly IClassroomRpst _classroomRpst;
        private readonly IClassRoleRpst _classRoleRpst;
        private readonly IClassroomClassRpst _classroomClassRpst;


        public ClassService(ISunnetLog log, IFile fileHelper, IEmailSender emailSender, IEncrypt encrypt,
            IClassRpst classRpst, IMonitoringToolRpst monitoringToolRpst,
            ICommunityRpst communityRpst, ISchoolRpst schoolRpst, IClassroomRpst classroomRpst,
            IClassRoleRpst classRoleRpst,
            IClassroomClassRpst classroomClassRpst,
            IClassLevelRpst classLevelRpst,
            IUnitOfWork unit)
            : base(log, fileHelper, emailSender, encrypt)
        {
            _classRpst = classRpst;
            _monitoringToolRpst = monitoringToolRpst;
            _communityRpst = communityRpst;
            _schoolRpst = schoolRpst;
            _classroomRpst = classroomRpst;
            _classRoleRpst = classRoleRpst;
            _classroomClassRpst = classroomClassRpst;
            _classLevelRpst = classLevelRpst;
            UnitOfWork = unit;
        }

        #region IQueryable

        public IQueryable<ClassLevelEntity> ClasseLevels
        {
            get { return _classLevelRpst.Entities.Where(e => e.Status == EntityStatus.Active); }
        }


        public IQueryable<ClassEntity> Classes
        {
            get { return _classRpst.Entities.Where(e => e.IsDeleted == false); }
        }
        public IQueryable<MonitoringToolEntity> MonitoringTools
        {
            get { return _monitoringToolRpst.Entities; }
        }
        public IQueryable<CommunityEntity> Communities
        {
            get { return _communityRpst.Entities; }
        }
        public IQueryable<SchoolEntity> Schools
        {
            get { return _schoolRpst.Entities; }
        }
        public IQueryable<ClassroomEntity> Classrooms
        {
            get { return _classroomRpst.Entities; }
        }
        public IQueryable<ClassroomClassEntity> ClassroomClassRelations
        {
            get { return _classroomClassRpst.Entities.Where(e => e.Class.IsDeleted == false); }
        }

        #endregion

        #region Class
        public ClassEntity NewClassEntity()
        {
            return _classRpst.Create();
        }

        public OperationResult InsertClass(ClassEntity entity, bool isSave = true)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                _classRpst.Insert(entity, isSave);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult DeleteClass(int id)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                _classRpst.Delete(id);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }


        public OperationResult UpdateClass(ClassEntity entity, bool isSave = true)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                _classRpst.Update(entity, isSave);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public ClassEntity GetClass(int id)
        {
            return _classRpst.GetByKey(id);
        }

        public OperationResult UpdateClassPlayground(int playgroundId = 0, int[] classIds = null)
        {
            var result = new OperationResult(OperationResultType.Success);

            if (playgroundId > 0)
            {
                try
                {
                    _classRpst.UpdateClassPlaygrounds(playgroundId, classIds);
                }
                catch (Exception ex)
                {
                    result = ResultError(ex);
                }
            }
            return result;
        }

        #endregion

        #region ClassRole
        public ClassRoleEntity GetClassRole(Role role)
        {
            return _classRoleRpst.Entities.FirstOrDefault(o => o.RoleId == (int)role);
        }
        #endregion

        #region MonitoringTool

        public MonitoringToolEntity NewMonitoringToolEntity()
        {
            return _monitoringToolRpst.Create();
        }

        public OperationResult InsertMonitoringTool(MonitoringToolEntity entity)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                _monitoringToolRpst.Insert(entity);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult UpdateMonitoringTool(MonitoringToolEntity entity)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                _monitoringToolRpst.Update(entity);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public MonitoringToolEntity GetMonitoringTool(int id)
        {
            return _monitoringToolRpst.GetByKey(id);
        }
        #endregion

        #region Class Classroom  Relation
        public OperationResult InsertRelations(IList<ClassroomClassEntity> list)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            try
            {
                _classroomClassRpst.Insert(list, true);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }
        public OperationResult DeleteRelations(IList<ClassroomClassEntity> list)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            try
            {
                _classroomClassRpst.Delete(list);
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
