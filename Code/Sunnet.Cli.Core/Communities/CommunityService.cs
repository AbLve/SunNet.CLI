using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
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
using StructureMap;
using Sunnet.Cli.Core.Common.Enums;
using Sunnet.Cli.Core.Communities.Entities;
using Sunnet.Cli.Core.Communities.Interfaces;
using Sunnet.Cli.Core.Cpalls;
using Sunnet.Cli.Core.Schools.Interfaces;
using Sunnet.Cli.Core.Users.Enums;
using Sunnet.Framework.Core.Base;
using Sunnet.Framework.Core.Tool;
using Sunnet.Framework.EmailSender;
using Sunnet.Framework.Encrypt;
using Sunnet.Framework.File;
using Sunnet.Framework.Log;
using Sunnet.Cli.Core.Vcw.Entities;
using Sunnet.Cli.Core.Schools.Entities;

namespace Sunnet.Cli.Core.Communities
{
    internal class CommunityService : CoreServiceBase, ICommunityContract
    {
        private readonly ICommunityRpst CommunityRpst;
        private readonly IBasicCommunityRpst BasicCommunityRpst;
        private readonly ICommunityRoleRpst CommunityRoleRpst;
        private readonly ICommunityNotesRpst CommunityNotesRpst;
        private readonly ICommunityAssessmentRelationsRpst CommunityAssessmentRelationsRpst;

        public CommunityService(ISunnetLog log,
            IFile fileHelper,
            IEmailSender emailSender,
            IEncrypt encrypt,
            ICommunityRpst communityRpst,
            IBasicCommunityRpst basicCommunityRpst,
            ICommunityRoleRpst communityRoleRpst,
            ICommunityNotesRpst communityNotesRpst,
            ICommunityAssessmentRelationsRpst communityAssessmentRelationsRpst,
            IUnitOfWork unit)
            : base(log, fileHelper, emailSender, encrypt)
        {
            CommunityRpst = communityRpst;
            BasicCommunityRpst = basicCommunityRpst;
            UnitOfWork = unit;
            CommunityRoleRpst = communityRoleRpst;
            CommunityNotesRpst = communityNotesRpst;
            CommunityAssessmentRelationsRpst = communityAssessmentRelationsRpst;
        }

        #region Community

        public IQueryable<CommunityEntity> Communities
        {
            get { return CommunityRpst.Entities; }
        }

        public CommunityEntity NewCommunityEntity()
        {
            return CommunityRpst.Create();
        }

        public OperationResult InsertCommunity(CommunityEntity entity)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                CommunityRpst.Insert(entity);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult DeleteCommunity(int id)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                CommunityRpst.Delete(id);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult UpdateCommunity(CommunityEntity entity)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                CommunityRpst.Update(entity);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public CommunityEntity GetCommunity(int id)
        {
            return CommunityRpst.GetByKey(id);
        }

        public bool InactiveEnity(ModelName model, int entityId, EntityStatus status, string schoolYear)
        {
            return CommunityRpst.InactiveEntities(model, entityId, status, schoolYear);
        }

        #endregion

        #region BasicCommunity

        public IQueryable<BasicCommunityEntity> BasicCommunities
        {
            get { return BasicCommunityRpst.Entities; }
        }

        public BasicCommunityEntity NewBasicCommunityEntity()
        {
            return BasicCommunityRpst.Create();
        }

        public OperationResult InsertBasicCommunity(BasicCommunityEntity entity)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                BasicCommunityRpst.Insert(entity);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult DeleteBasicCommunity(int id)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                BasicCommunityRpst.Delete(id);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult UpdateBasicCommunity(BasicCommunityEntity entity)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                BasicCommunityRpst.Update(entity);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public BasicCommunityEntity GetBasicCommunity(int id)
        {
            return BasicCommunityRpst.GetByKey(id);
        }
        #endregion

        #region Email
        public void SendEmailToCli(string to, string subject, string emailBody)
        {
            var email = ObjectFactory.GetInstance<IEmailSender>();
            new SendHandler(() => email.SendMail(to, subject, emailBody)).BeginInvoke(null, null);
        }
        public delegate void SendHandler();
        #endregion

        #region Community Role
        public CommunityRoleEntity GetRole(Role role)
        {
            return CommunityRoleRpst.Entities.FirstOrDefault(o => o.RoleId == (int)role);
        }
        #endregion


        #region Community Notes

        public CommunityNotesEntity GetCommunityNotes(int id)
        {
            return CommunityNotesRpst.GetByKey(id);
        }

        public IQueryable<CommunityNotesEntity> CommunityNotes
        {
            get { return CommunityNotesRpst.Entities; }
        }

        public OperationResult InsertCommunityNotes(CommunityNotesEntity entity)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                CommunityNotesRpst.Insert(entity);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult UpdateComunityNotes(CommunityNotesEntity entity)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                CommunityNotesRpst.Update(entity);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }
        #endregion

        #region Community Assessment relations
        public IQueryable<CommunityAssessmentRelationsEntity> CommunityAssessmentRelations
        {
            get { return CommunityAssessmentRelationsRpst.Entities; }
        }

        public OperationResult InsertRelations(IList<CommunityAssessmentRelationsEntity> list)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            try
            {
                CommunityAssessmentRelationsRpst.Insert(list);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult DelRelations(IList<CommunityAssessmentRelationsEntity> list)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            try
            {
                CommunityAssessmentRelationsRpst.Delete(list);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public IList<CommunityAssessmentRelationsEntity> GetRelationsByComId(int comId, int[] assessmentIds)
        {
            return CommunityAssessmentRelationsRpst.Entities.Where(o => o.CommunityId == comId && assessmentIds.Contains(o.AssessmentId)).ToList();
        }
        public IList<CommunityAssessmentRelationsEntity> GetRelationsByAssessmentId(int assessmentId,int[] comIds)
        {
            return CommunityAssessmentRelationsRpst.Entities.Where(o => o.AssessmentId == assessmentId && comIds.Contains(o.CommunityId)).ToList();
        }
       
        public int[] GetAssessmentIds(int comId)
        {
            return CommunityAssessmentRelationsRpst.Entities.Where(o => o.CommunityId == comId).Select(e => e.AssessmentId).ToArray();
        }
        public List<CommunityAssessmentRelationsEntity> GetAssessments(int comId)
        {
            return CommunityAssessmentRelationsRpst.Entities.Where(o => o.CommunityId == comId).ToList();
        }
        public List<CommunityAssessmentRelationsEntity> GetAssessments(List<int> comIds)
        {
            return CommunityAssessmentRelationsRpst.Entities.Where(o => comIds.Contains(o.CommunityId)).ToList();
        }

        public OperationResult UpdateCommunityAssessmentRelation(CommunityAssessmentRelationsEntity item)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            try
            {
                CommunityAssessmentRelationsRpst.Update(item);
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
