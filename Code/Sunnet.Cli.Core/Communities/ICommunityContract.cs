using System;
using System.Collections.Generic;
using System.Linq;
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
using Sunnet.Cli.Core.Common.Enums;
using Sunnet.Cli.Core.Communities.Entities;
using Sunnet.Cli.Core.Users.Enums;
using Sunnet.Framework.Core.Tool;
using Sunnet.Cli.Core.Vcw.Entities;

namespace Sunnet.Cli.Core.Communities
{
    public interface ICommunityContract
    {
        IQueryable<CommunityEntity> Communities { get; }
        CommunityEntity NewCommunityEntity();
        OperationResult InsertCommunity(CommunityEntity entity);
        OperationResult DeleteCommunity(int id);
        OperationResult UpdateCommunity(CommunityEntity entity);
        CommunityEntity GetCommunity(int id);

        IQueryable<BasicCommunityEntity> BasicCommunities { get; }
        BasicCommunityEntity NewBasicCommunityEntity();
        OperationResult InsertBasicCommunity(BasicCommunityEntity entity);
        OperationResult DeleteBasicCommunity(int id);
        OperationResult UpdateBasicCommunity(BasicCommunityEntity entity);
        bool InactiveEnity(ModelName model, int entityId, EntityStatus status, string schoolYear);
        BasicCommunityEntity GetBasicCommunity(int id);

        void SendEmailToCli(string to, string subject, string emailBody);
        CommunityRoleEntity GetRole(Role role);


        #region Community Notes

        CommunityNotesEntity GetCommunityNotes(int id);

        IQueryable<CommunityNotesEntity> CommunityNotes { get; }

        OperationResult InsertCommunityNotes(CommunityNotesEntity entity);

        OperationResult UpdateComunityNotes(CommunityNotesEntity entity);

        #endregion

        #region Community Assessment relations

        IQueryable<CommunityAssessmentRelationsEntity> CommunityAssessmentRelations { get; }
        OperationResult InsertRelations(IList<CommunityAssessmentRelationsEntity> list);
        OperationResult DelRelations(IList<CommunityAssessmentRelationsEntity> list);
        IList<CommunityAssessmentRelationsEntity> GetRelationsByComId(int comId, int[] assessmentIds);
        IList<CommunityAssessmentRelationsEntity> GetRelationsByAssessmentId(int assessmentId,int[] comIds);
        int[] GetAssessmentIds(int comId);
        List<CommunityAssessmentRelationsEntity> GetAssessments(int comId);
        List<CommunityAssessmentRelationsEntity> GetAssessments(List<int> comIds);
        OperationResult UpdateCommunityAssessmentRelation(CommunityAssessmentRelationsEntity item);

        #endregion
    }
}
