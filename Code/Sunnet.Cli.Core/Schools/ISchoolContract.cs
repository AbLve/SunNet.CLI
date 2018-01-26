using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		Sam
 * Computer:		SAM-PC
 * Domain:			sam-pc
 * CreatedOn:		2014/8/19 8:46:44
 * Description:		Please input class summary
 * Version History:	Created,2014/8/19 8:46:44
 * 
 * 
 **************************************************************************/
using Sunnet.Cli.Core.Schools.Entities;
using Sunnet.Cli.Core.Users.Enums;
using Sunnet.Framework.Core.Tool;

namespace Sunnet.Cli.Core.Schools
{
    public interface ISchoolContract
    {
        IQueryable<SchoolEntity> Schools { get; }

        IQueryable<BasicSchoolEntity> BasicSchools { get; }

        IQueryable<SchoolTypeEntity> SchoolTypes { get; }

        IQueryable<IspEntity> Isps { get; }

        IQueryable<TrsProviderEntity> TrsProviders { get; }

        IQueryable<ParentAgencyEntity> ParentAgencies { get; }

        IQueryable<PlaygroundEntity> Playgrounds { get; }

        IQueryable<SchoolStudentRelationEntity> SchoolStudentRelation { get; }

        OperationResult InsertSchool(SchoolEntity shcool);

        OperationResult UpdateSchool(SchoolEntity shcool);
        OperationResult DeleteSchool(SchoolEntity shcool);

        SchoolEntity GetSchool(int schoolId);

        BasicSchoolEntity GetBasicSchool(int id);

        OperationResult InsertBasicSchool(BasicSchoolEntity basicSchool);

        OperationResult UpdateBasicSchool(BasicSchoolEntity basicSchool);
        OperationResult DeleteBasicSchool(BasicSchoolEntity basicSchool);
        SchoolEntity NewSchoolEntity();

        List<SchoolSelectItemEntity> GetSchoolNameList(int communityId, string keyword);

        List<SchoolSelectItemEntity> GetSchoolListByKey(int communityId, string keyword);


        TrsProviderEntity NewTrsProviderEntity();

        OperationResult InsertTrsProvider(TrsProviderEntity entity);

        OperationResult UpdateTrsProvider(TrsProviderEntity entity);

        TrsProviderEntity GetTrsProvider(int id);


        ParentAgencyEntity NewParentAgencyEntity();

        OperationResult InsertParentAgency(ParentAgencyEntity entity);

        OperationResult UpdateParentAgency(ParentAgencyEntity entity);

        ParentAgencyEntity GetParentAgency(int id);


        IspEntity NewIspEntity();

        OperationResult InsertIsp(IspEntity entity);

        OperationResult UpdateIsp(IspEntity entity);

        IspEntity GetIsp(int id);


        SchoolTypeEntity NewSchoolTypeEntity();

        OperationResult InsertSchoolType(SchoolTypeEntity entity);

        OperationResult UpdateSchoolType(SchoolTypeEntity entity);

        SchoolTypeEntity GetSchoolType(int id);



        OperationResult InsertPlayground(PlaygroundEntity ground);
        OperationResult DeletePlayground(int Id);


        OperationResult InsertRelations(IList<CommunitySchoolRelationsEntity> list);
        OperationResult DelRelations(IList<CommunitySchoolRelationsEntity> list);
        IList<CommunitySchoolRelationsEntity> GetRelationsBySchoolId(int schoolId, int[] comIds);
        IList<CommunitySchoolRelationsEntity> GetRelationsByCommunityId(int comId, int[] schoolIds);
        int[] GetCommIds(int schoolId);
        int[] GetSchoolIds(int comId);
        List<int> GetSchoolIds(int[] communityIds);
        List<int> GetCommunityIds(int[] SchoolIds);
        IQueryable<CommunitySchoolRelationsEntity> CommunitySchoolRelations { get; }


        SchoolStudentRelationEntity NewSchoolStudentRelationEntity(int schoolId, int studentId);

        OperationResult InsertSchoolStudentRelation(SchoolStudentRelationEntity entity);

        OperationResult InsertSchoolStudentRelations(List<SchoolStudentRelationEntity> entities);

        OperationResult UpdateSchoolStudentRelation(SchoolStudentRelationEntity entity);

        OperationResult UpdateSchoolStudentRelations(List<SchoolStudentRelationEntity> entities);

        OperationResult DeleteSchoolStudentRelations(List<SchoolStudentRelationEntity> entities);

        SchoolRoleEntity GetSchoolRole(Role role);
    }
}
