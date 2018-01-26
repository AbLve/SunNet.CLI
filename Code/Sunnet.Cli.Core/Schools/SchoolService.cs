using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		Sam
 * Computer:		SAM-PC
 * Domain:			sam-pc
 * CreatedOn:		2014/8/19 8:45:43
 * Description:		Please input class summary
 * Version History:	Created,2014/8/19 8:45:43
 * 
 * 
 **************************************************************************/
using Sunnet.Cli.Core.Common.Enums;
using Sunnet.Cli.Core.Schools.Entities;
using Sunnet.Cli.Core.Schools.Interfaces;
using Sunnet.Cli.Core.Users.Enums;
using Sunnet.Framework.Core.Base;
using Sunnet.Framework.Core.Tool;
using Sunnet.Framework.EmailSender;
using Sunnet.Framework.Encrypt;
using Sunnet.Framework.File;
using Sunnet.Framework.Log;

namespace Sunnet.Cli.Core.Schools
{
    internal class SchoolService : CoreServiceBase, ISchoolContract
    {
        readonly ISchoolRpst _schoolRpst;
        readonly IBasicSchoolRpst _basicSchoolRpst;
        readonly ISchoolTypeRpst _schoolTypeRpst;
        readonly IIspRpst _ispRpst;
        readonly ITrsProviderRpst _trsProviderRpst;
        readonly IParentAgencyRpst _parentAgencyRpst;
        readonly IPlaygroundRpst _playgroundRpst;
        readonly ICommunitySchoolRelationsRpst _communitySchoolRelationsRpst;
        readonly ISchoolStudentRelationRpst _schoolStudentRelationRpst;
        readonly ISchoolRoleRpst _schoolRoleRpst;

        public SchoolService(ISunnetLog log,
            IFile fileHelper,
            IEmailSender emailSender,
            IEncrypt encrypt,
            ISchoolRpst schoolRpst,
            IBasicSchoolRpst basicShoolRpst,
            ISchoolTypeRpst schoolTypeRpst,
            IIspRpst ispRpst,
            ITrsProviderRpst trsProviderRpst,
            IParentAgencyRpst parentAgencyRpst,
            IPlaygroundRpst playgroundRpst,
            ICommunitySchoolRelationsRpst communitySchoolRelationsRpst,
            ISchoolStudentRelationRpst schoolStudentRelationRpst,
            ISchoolRoleRpst schoolRoleRpst,
            IUnitOfWork unit)
            : base(log, fileHelper, emailSender, encrypt)
        {
            _schoolRpst = schoolRpst;
            _basicSchoolRpst = basicShoolRpst;
            _schoolTypeRpst = schoolTypeRpst;
            _ispRpst = ispRpst;
            _trsProviderRpst = trsProviderRpst;
            _parentAgencyRpst = parentAgencyRpst;
            _playgroundRpst = playgroundRpst;
            _communitySchoolRelationsRpst = communitySchoolRelationsRpst;
            _schoolStudentRelationRpst = schoolStudentRelationRpst;
            _schoolRoleRpst = schoolRoleRpst;

            UnitOfWork = unit;
        }

        public IQueryable<SchoolEntity> Schools
        {
            get { return _schoolRpst.Entities; }
        }

        public IQueryable<SchoolTypeEntity> SchoolTypes
        {
            get { return _schoolTypeRpst.Entities; }
        }

        public IQueryable<IspEntity> Isps
        {
            get { return _ispRpst.Entities; }
        }

        public IQueryable<TrsProviderEntity> TrsProviders
        {
            get { return _trsProviderRpst.Entities; }
        }

        public IQueryable<ParentAgencyEntity> ParentAgencies
        {
            get { return _parentAgencyRpst.Entities; }
        }

        public IQueryable<PlaygroundEntity> Playgrounds
        {
            get { return _playgroundRpst.Entities; }
        }

        public IQueryable<SchoolStudentRelationEntity> SchoolStudentRelation
        {
            get { return _schoolStudentRelationRpst.Entities; }
        }

        #region School Function

        public OperationResult InsertSchool(SchoolEntity shcool)
        {
            shcool.SchoolId = string.Empty;
            OperationResult result = new OperationResult(OperationResultType.Success);
            try
            {
                _schoolRpst.Insert(shcool);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult UpdateSchool(SchoolEntity school)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            try
            {
                _schoolRpst.Update(school);

            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult DeleteSchool(SchoolEntity shcool)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            try
            {
                _schoolRpst.Delete(shcool);

            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public SchoolEntity GetSchool(int schoolId)
        {
            return _schoolRpst.GetByKey(schoolId);
        }

        public SchoolEntity NewSchoolEntity()
        {
            return _schoolRpst.Create();
        }
         
        #endregion

        #region Basic School Info

        public IQueryable<BasicSchoolEntity> BasicSchools
        {
            get { return _basicSchoolRpst.Entities; }
        }

        public BasicSchoolEntity GetBasicSchool(int id)
        {
            return _basicSchoolRpst.GetByKey(id);
        }

        public OperationResult InsertBasicSchool(BasicSchoolEntity basicSchool)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            try
            {
                _basicSchoolRpst.Insert(basicSchool);
            }
            catch (Exception ex)
            {
                result.ResultType = OperationResultType.Error;
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult UpdateBasicSchool(BasicSchoolEntity basicSchool)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            try
            {
                _basicSchoolRpst.Update(basicSchool);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult DeleteBasicSchool(BasicSchoolEntity basicSchool)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            try
            {
                _basicSchoolRpst.Delete(basicSchool);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public List<SchoolSelectItemEntity> GetSchoolNameList(int communityId, string keyword)
        {
            return _schoolRpst.GetSchoolNameList(communityId, keyword);
        }

        public List<SchoolSelectItemEntity> GetSchoolListByKey(int communityId, string keyword)
        {
            return _schoolRpst.GetSchoolListByKey(communityId, keyword);
        }

        #endregion

        #region TrsProvider
        public TrsProviderEntity NewTrsProviderEntity()
        {
            return _trsProviderRpst.Create();
        }

        public OperationResult InsertTrsProvider(TrsProviderEntity entity)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            try
            {
                _trsProviderRpst.Insert(entity);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult UpdateTrsProvider(TrsProviderEntity entity)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            try
            {
                _trsProviderRpst.Update(entity);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public TrsProviderEntity GetTrsProvider(int id)
        {
            return _trsProviderRpst.GetByKey(id);
        }
        #endregion

        #region ParentAgency
        public ParentAgencyEntity NewParentAgencyEntity()
        {
            return _parentAgencyRpst.Create();
        }

        public OperationResult InsertParentAgency(ParentAgencyEntity entity)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            try
            {
                _parentAgencyRpst.Insert(entity);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult UpdateParentAgency(ParentAgencyEntity entity)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            try
            {
                _parentAgencyRpst.Update(entity);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public ParentAgencyEntity GetParentAgency(int id)
        {
            return _parentAgencyRpst.GetByKey(id);
        }
        #endregion

        #region Isp
        public IspEntity NewIspEntity()
        {
            return _ispRpst.Create();
        }

        public OperationResult InsertIsp(IspEntity entity)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            try
            {
                _ispRpst.Insert(entity);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult UpdateIsp(IspEntity entity)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            try
            {
                _ispRpst.Update(entity);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public IspEntity GetIsp(int id)
        {
            return _ispRpst.GetByKey(id);
        }
        #endregion

        #region SchoolType
        public SchoolTypeEntity NewSchoolTypeEntity()
        {
            return _schoolTypeRpst.Create();
        }

        public OperationResult InsertSchoolType(SchoolTypeEntity entity)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            try
            {
                _schoolTypeRpst.Insert(entity);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult UpdateSchoolType(SchoolTypeEntity entity)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            try
            {
                _schoolTypeRpst.Update(entity);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public SchoolTypeEntity GetSchoolType(int id)
        {
            return _schoolTypeRpst.GetByKey(id);
        }
        #endregion

        #region Playgrounds
        public OperationResult InsertPlayground(PlaygroundEntity ground)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            try
            {
                _playgroundRpst.Insert(ground);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult DeletePlayground(int Id)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            try
            {
                _playgroundRpst.Delete(o => o.ID == Id);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }
        #endregion

        #region Community and School  relations
        public IQueryable<CommunitySchoolRelationsEntity> CommunitySchoolRelations
        {
            get { return _communitySchoolRelationsRpst.Entities; }
        }
        public OperationResult InsertRelations(IList<CommunitySchoolRelationsEntity> list)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            try
            {
                _communitySchoolRelationsRpst.Insert(list);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult DelRelations(IList<CommunitySchoolRelationsEntity> list)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            try
            {
                _communitySchoolRelationsRpst.Delete(list);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public IList<CommunitySchoolRelationsEntity> GetRelationsBySchoolId(int schoolId, int[] comIds)
        {
            return _communitySchoolRelationsRpst.Entities.Where(o => o.SchoolId == schoolId && comIds.Contains(o.CommunityId)).ToList();
        }
        public IList<CommunitySchoolRelationsEntity> GetRelationsByCommunityId(int comId, int[] schoolIds)
        {
            return _communitySchoolRelationsRpst.Entities.Where(o => o.CommunityId == comId && schoolIds.Contains(o.SchoolId)).ToList();
        }
        public int[] GetCommIds(int schoolId)
        {
            return _communitySchoolRelationsRpst.Entities.Where(o => o.SchoolId == schoolId && o.Community.Status == EntityStatus.Active).Select(e => e.CommunityId).ToArray();
        }

        public int[] GetSchoolIds(int comId)
        {
            return _communitySchoolRelationsRpst.Entities.Where(o => o.CommunityId == comId && o.School.Status == SchoolStatus.Active).Select(e => e.SchoolId).ToArray();
        }

        public List<int> GetSchoolIds(int[] communityIds)
        {
            return
                _communitySchoolRelationsRpst.Entities.Where(
                    o => communityIds.Contains(o.CommunityId) && o.Community.Status == EntityStatus.Active)
                    .Select(e => e.SchoolId)
                    .ToList<int>();
        }
        public List<int> GetCommunityIds(int[] SchoolIds)
        {
            return
                _communitySchoolRelationsRpst.Entities.Where(
                    o => SchoolIds.Contains(o.SchoolId) && o.School.Status == SchoolStatus.Active)
                    .Select(e => e.CommunityId)
                    .ToList<int>();
        }
        #endregion

        #region School Student Relation

        public SchoolStudentRelationEntity NewSchoolStudentRelationEntity(int schoolId, int studentId)
        {
            var entity = _schoolStudentRelationRpst.Create();
            entity.SchoolId = schoolId;
            entity.StudentId = studentId;
            return entity;
        }

        public OperationResult InsertSchoolStudentRelation(SchoolStudentRelationEntity entity)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                _schoolStudentRelationRpst.Insert(entity);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult UpdateSchoolStudentRelation(SchoolStudentRelationEntity entity)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                _schoolStudentRelationRpst.Update(entity);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult InsertSchoolStudentRelations(List<SchoolStudentRelationEntity> entities)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                entities.ForEach(x => _schoolStudentRelationRpst.Insert(x, false));
                UnitOfWork.Commit();
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult UpdateSchoolStudentRelations(List<SchoolStudentRelationEntity> entities)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                entities.ForEach(x => _schoolStudentRelationRpst.Update(x, false));
                UnitOfWork.Commit();
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult DeleteSchoolStudentRelations(List<SchoolStudentRelationEntity> entities)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                entities.ForEach(x => _schoolStudentRelationRpst.Delete(x, false));
                UnitOfWork.Commit();
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }
        #endregion

        #region School Role
        public SchoolRoleEntity GetSchoolRole(Role role)
        {
            return _schoolRoleRpst.Entities.FirstOrDefault(o => o.RoleId == (int)role);
        }
        #endregion
    }
}
