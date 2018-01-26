using System.Data.Entity.Validation;
using System.Linq.Expressions;
using Sunnet.Cli.Core.Common.Enums;
using Sunnet.Cli.Core.Users.Entities;
using Sunnet.Cli.Core.Users.Enums;
using Sunnet.Cli.Core.Users.Interfaces;
using Sunnet.Framework.Core.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		Sam
 * Computer:		SAM-PC
 * Domain:			sam-pc
 * CreatedOn:		2014/8/5 16:36:16
 * Description:		Please input class summary
 * Version History:	Created,2014/8/5 16:36:16
 * 
 * 
 **************************************************************************/
using Sunnet.Framework.Core.Extensions;
using Sunnet.Framework.Core.Tool;
using Sunnet.Framework.EmailSender;
using Sunnet.Framework.Encrypt;
using Sunnet.Framework.File;
using Sunnet.Framework.Log;
using StructureMap;
using Sunnet.Framework.Helpers;

namespace Sunnet.Cli.Core.Users
{
    internal partial class UserService : CoreServiceBase, IUserContract
    {
        IUserBaseRpst UserRepository;
        ITeacherRpst TeacherRepository;
        IPrincipalRpst PrincipalRepository;
        IDisCommunityRpst DisCommunityRepository;
        IParentRpst ParentRepository;
        IParentStudentRelationRpst ParentStudentRelationRepository;
        IStateWideRpst StateWideRpst;
        IAuditorRpst AuditorRpst;
        IRegistorLogRpst RegistorLogRepository;
        IApplicantRpst ApplicantRpst;
        IApplicantContactRpst ApplicantContactRpst;
        IApplicantEmailRpst ApplicantEmailRpst;
        ITeacherAgeGroupRpst TeacherAgeGroupRpst;
        ITeacherEquipmentRelationRpst TeacherEquipmentRelationRpst;
        ITeacherTransactionRpst TeacherTransactionRpst;
        IAffiliationRpst AffiliationRpst;
        ICertificateRpst CertificateRpst;
        IPositionRpst PositionRpst;
        IYearsInProjectRpst YearsInProjectRpst;
        IProfessionalDevelopmentRpst ProfessionalDevelopmentRpst;
        IVideoCodingRpst VideoCodingRpst;
        ICoordCoachRpst CoordCoachRpst;
        ICoordCoachEquipmentRpst CoordCoachEquipmentRpst;
        ITeacherRoleRpst TeacherRoleRpst;
        IUserComSchRelationRpst UserComSchRelationRpst;
        IUserClassRelationRpst UserClassRelationRpst;
        IIntManaCoachRelationRpst IntManaCoachRelationRpst;
        IPrincipalRoleRpst PrincipalRoleRpst;
        ICoordCoachRoleRpst CoordCoachRoleRpst;

        public UserService(ISunnetLog log, IFile fileHelper, IEmailSender emailSender, IEncrypt encrypt,
            IUserBaseRpst userRepository,
            ITeacherRpst teacherRepository,
            IPrincipalRpst principalRepository,
            IDisCommunityRpst disCommunityRepository,
            IParentRpst parentRepository,
            IParentStudentRelationRpst parentStudentRelationRepository,
            IStateWideRpst stateWideRpst,
            IAuditorRpst auditorRpst,
            IRegistorLogRpst registorLogRepository,
            IApplicantRpst applicantRpst,
            IApplicantContactRpst applicantContactRpst,
            IApplicantEmailRpst applicantEmailRpst,
            ITeacherAgeGroupRpst teacherAgeGroupRpst,
            ITeacherEquipmentRelationRpst teacherEquipmentRelationRpst,
            ITeacherTransactionRpst teacherTransactionRpst,
            IAffiliationRpst affiliationRpst,
            ICertificateRpst certificateRpst,
            IPositionRpst positionRpst,
            IYearsInProjectRpst yearsInProjectRpst,
            IProfessionalDevelopmentRpst professionalDevelopmentRpst,
            IVideoCodingRpst videoCodingRpst,
            ICoordCoachRpst coordCoachRpst,
            ICoordCoachEquipmentRpst coordCoachEquipmentRpst,
            ITeacherRoleRpst teacherRoleRpst,
            IUserComSchRelationRpst userComSchRelationRpst,
            IUserClassRelationRpst userClassRelationRpst,
            IIntManaCoachRelationRpst intManaCoachRelationRpst,
            IPrincipalRoleRpst principalRoleRpst,
            ICoordCoachRoleRpst coordCoachRoleRpst,
            IUnitOfWork unit)
            : base(log, fileHelper, emailSender, encrypt)
        {
            UserRepository = userRepository;
            TeacherRepository = teacherRepository;
            PrincipalRepository = principalRepository;
            DisCommunityRepository = disCommunityRepository;
            ParentRepository = parentRepository;
            ParentStudentRelationRepository = parentStudentRelationRepository;
            StateWideRpst = stateWideRpst;
            AuditorRpst = auditorRpst;
            RegistorLogRepository = registorLogRepository;
            ApplicantRpst = applicantRpst;
            ApplicantContactRpst = applicantContactRpst;
            ApplicantEmailRpst = applicantEmailRpst;
            TeacherAgeGroupRpst = teacherAgeGroupRpst;
            TeacherEquipmentRelationRpst = teacherEquipmentRelationRpst;
            TeacherTransactionRpst = teacherTransactionRpst;
            AffiliationRpst = affiliationRpst;
            CertificateRpst = certificateRpst;
            PositionRpst = positionRpst;
            YearsInProjectRpst = yearsInProjectRpst;
            ProfessionalDevelopmentRpst = professionalDevelopmentRpst;
            VideoCodingRpst = videoCodingRpst;
            CoordCoachRpst = coordCoachRpst;
            CoordCoachEquipmentRpst = coordCoachEquipmentRpst;
            TeacherRoleRpst = teacherRoleRpst;
            UserComSchRelationRpst = userComSchRelationRpst;
            UserClassRelationRpst = userClassRelationRpst;
            IntManaCoachRelationRpst = intManaCoachRelationRpst;
            PrincipalRoleRpst = principalRoleRpst;
            CoordCoachRoleRpst = coordCoachRoleRpst;
            UnitOfWork = unit;
        }
        #region Get Methods

        public IQueryable<UserBaseEntity> BaseUsers
        {
            get { return UserRepository.Entities.Where(e => e.IsDeleted == false); }
        }

        public IQueryable<VideoCodingEntity> VideoCodings
        {
            get { return VideoCodingRpst.Entities; }
        }

        public IQueryable<CoordCoachEntity> CoordCoachs
        {
            get { return CoordCoachRpst.Entities; }
        }

        public IQueryable<TeacherEntity> Teachers
        {
            get { return TeacherRepository.Entities.Where(e => e.UserInfo.IsDeleted == false); }
        }

        public IQueryable<PrincipalEntity> PrincipalDirectors
        {
            get { return PrincipalRepository.Entities; }
        }

        public IQueryable<CommunityUserEntity> DistrictCommunitys
        {
            get { return DisCommunityRepository.Entities; }
        }

        public IQueryable<ParentEntity> Parents
        {
            get { return ParentRepository.Entities; }
        }

        public IQueryable<ParentStudentRelationEntity> ParentStudentRelations
        {
            get { return ParentStudentRelationRepository.Entities; }
        }

        public IQueryable<StateWideEntity> StateWides
        {
            get { return StateWideRpst.Entities; }
        }

        public IQueryable<AuditorEntity> Auditors
        {
            get { return AuditorRpst.Entities; }
        }

        public IQueryable<ApplicantEntity> Applicants
        {
            get { return ApplicantRpst.Entities; }
        }

        public IQueryable<TeacherAgeGroupEntity> TeacherAgeGroups
        {
            get { return TeacherAgeGroupRpst.Entities; }
        }

        public IQueryable<TeacherEquipmentRelationEntity> TeacherEquipmentRelations
        {
            get { return TeacherEquipmentRelationRpst.Entities; }
        }

        public IQueryable<TeacherTransactionEntity> TeacherTransactions
        {
            get { return TeacherTransactionRpst.Entities; }
        }

        public IQueryable<CoordCoachEquipmentEntity> CoordCoachEquipments
        {
            get { return CoordCoachEquipmentRpst.Entities; }
        }

        public IQueryable<UserComSchRelationEntity> UserComSchRelations
        {
            get { return UserComSchRelationRpst.Entities.Where(e => e.User.IsDeleted == false); }
        }

        public IQueryable<UserClassRelationEntity> UserClassRelations
        {
            get { return UserClassRelationRpst.Entities; }
        }

        public IQueryable<IntManaCoachRelationEntity> IntManaCoachRelations
        {
            get { return IntManaCoachRelationRpst.Entities; }
        }

        public ApplicantEntity GetAppliant(int id)
        {
            return ApplicantRpst.GetByKey(id);
        }

        public UserBaseEntity GetUser(int id)
        {
            return UserRepository.GetByKey(id);
        }

        public CoordCoachEntity GetCoordCoach(int id)
        {
            return CoordCoachRpst.GetByKey(id);
        }

        public VideoCodingEntity GetVideoCoding(int id)
        {
            return VideoCodingRpst.GetByKey(id);
        }

        public TeacherEntity GetTeacher(int id)
        {
            return TeacherRepository.GetByKey(id);
        }

        public PrincipalEntity GetPrincipal(int id)
        {
            return PrincipalRepository.GetByKey(id);
        }

        public CommunityUserEntity GetCommunity(int id)
        {
            return DisCommunityRepository.GetByKey(id);
        }

        public AuditorEntity GetAuditor(int id)
        {
            return AuditorRpst.GetByKey(id);
        }

        public StateWideEntity GetStateWide(int id)
        {
            return StateWideRpst.GetByKey(id);
        }
        #endregion

        #region Insert Methods
        public OperationResult InsertParent(ParentEntity parent)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            try
            {
                ParentRepository.Insert(parent);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult InsertParentStudentRelation(ParentStudentRelationEntity parentStudentRelation)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            try
            {
                ParentStudentRelationRepository.Insert(parentStudentRelation);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult InsertUser(UserBaseEntity user)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            try
            {
                UserRepository.Insert(user);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult InsertCoordCoach(CoordCoachEntity coordCoach)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            try
            {
                CoordCoachRpst.Insert(coordCoach);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult InsertTeacher(TeacherEntity teacher)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            try
            {
                TeacherRepository.Insert(teacher, true);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult InsertPrincipal(PrincipalEntity principalDirectors)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            try
            {
                PrincipalRepository.Insert(principalDirectors, true);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult InsertCommunity(CommunityUserEntity communityUser)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            try
            {
                DisCommunityRepository.Insert(communityUser, true);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult InsertStateWide(StateWideEntity stateWide)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            try
            {
                StateWideRpst.Insert(stateWide, true);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult InsertAuditor(AuditorEntity auditor)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            try
            {
                AuditorRpst.Insert(auditor, true);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult InsertRegistor(RegistorLogEntity registor)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            try
            {
                RegistorLogRepository.Insert(registor);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult InsertApplication(ApplicantEntity applicant)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            try
            {
                ApplicantRpst.Insert(applicant);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult InsertApplicantContact(ApplicantContactEntity applicantContact)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            try
            {
                ApplicantContactRpst.Insert(applicantContact);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult InsertApplicantEmail(ApplicantEmailEntity applicantEmail)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            try
            {
                ApplicantEmailRpst.Insert(applicantEmail);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult InsertIntManaCoachRelation(List<IntManaCoachRelationEntity> entities, bool isSave = true)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            try
            {
                IntManaCoachRelationRpst.Insert(entities);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }
        #endregion

        #region Update Methods

        public OperationResult UpdateUser(UserBaseEntity user, bool isSave = true, bool isSyncLms = true)
        {
            if (isSyncLms)
                user.IsSyncLms = true;
            OperationResult result = new OperationResult(OperationResultType.Success);
            try
            {
                UserRepository.Update(user, isSave);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult UpdateAppliant(ApplicantEntity applicant, bool isSave = true)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            try
            {
                ApplicantRpst.Update(applicant, isSave);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult UpdateVideoCoding(VideoCodingEntity videoCoding, bool isSave = true)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            try
            {
                VideoCodingRpst.Update(videoCoding, isSave);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult UpdateCoordCoach(CoordCoachEntity coordCoach, bool isSave = true)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            try
            {
                CoordCoachRpst.Update(coordCoach, isSave);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult UpdateParent(ParentEntity parent)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            try
            {
                ParentRepository.Update(parent, true);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult UpdateTeacher(TeacherEntity teacher, bool isSave = true)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            try
            {
                TeacherRepository.Update(teacher, isSave);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult UpdatePrincipal(PrincipalEntity principal, bool isSave = true)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            try
            {
                PrincipalRepository.Update(principal, isSave);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult UpdateCommunity(CommunityUserEntity communityUser, bool isSave = true)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            try
            {
                DisCommunityRepository.Update(communityUser, isSave);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult UpdateStateWide(StateWideEntity stateWide, bool isSave = true)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            try
            {
                StateWideRpst.Update(stateWide, isSave);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult UpdateAuditor(AuditorEntity auditor, bool isSave = true)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            try
            {
                AuditorRpst.Update(auditor, isSave);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult UpdateRegistor(RegistorLogEntity entity)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            try
            {
                RegistorLogRepository.Update(entity);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }
        #endregion

        #region Delete Methods
        public OperationResult DeleteTeacherAgeGroup(List<TeacherAgeGroupEntity> listTeacherAgeGroup, bool isSave = true)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            try
            {
                TeacherAgeGroupRpst.Delete(listTeacherAgeGroup, isSave);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult DeleteTeacherEquipment(List<TeacherEquipmentRelationEntity> listTeacherEquipment)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            try
            {
                TeacherEquipmentRelationRpst.Delete(listTeacherEquipment);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult DeleteCoordCoachEquipment(List<CoordCoachEquipmentEntity> listCoordCoachEquipment)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            try
            {
                CoordCoachEquipmentRpst.Delete(listCoordCoachEquipment);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult DeleteCoordCoach(int id, bool isSave = true)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            try
            {
                CoordCoachRpst.Delete(id);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult DeleteVideoCoding(int id, bool isSave = true)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            try
            {
                VideoCodingRpst.Delete(id);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult DeleteIntManaCoachRelations(List<IntManaCoachRelationEntity> entities, bool isSave = true)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            try
            {
                IntManaCoachRelationRpst.Delete(entities, isSave);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult DeleteParentStudent(ParentStudentRelationEntity parentStudent)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            try
            {
                ParentStudentRelationRepository.Delete(parentStudent);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }
        public OperationResult DeleteParentStudent(int parentId,int studentId)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            try
            {
                ParentStudentRelationRepository.Delete(c=>c.ParentId == parentId && c.StudentId == studentId);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        #endregion

        /// <summary>
        /// sendToUserEmail
        /// </summary>
        /// <param name="to">Who is sent to the E-mail address</param>
        /// <param name="emailbody">Replace Email content</param>
        /// <param name="emailTemplateName">Replace Email content</param>
        public void SendToUserEmail(string to, string subject, string emailBody)
        {
            var emailSender = ObjectFactory.GetInstance<IEmailSender>();
            new SendHandler(() => emailSender.SendMail(to, subject, emailBody)).BeginInvoke(null, null);
        }
        private delegate void SendHandler();

        #region Get User Role
        public TeacherRoleEntity GetTeacherRole(Role role)
        {
            if (TeacherRoleRpst.Entities != null)
            {
                return TeacherRoleRpst.Entities.FirstOrDefault(o => o.RoleId == (int)role);
            }
            return null;
        }

        public PrincipalRoleEntity GetPrincipalRole(Role role)
        {
            if (PrincipalRoleRpst.Entities != null)
            {
                return PrincipalRoleRpst.Entities.FirstOrDefault(o => o.RoleId == (int)role);
            }
            return null;
        }

        public CoordCoachRoleEntity GetCoordCoachRole(Role role)
        {
            if (CoordCoachRoleRpst.Entities != null)
            {
                return CoordCoachRoleRpst.Entities.FirstOrDefault(o => o.RoleId == (int)role);
            }
            return null;
        }
        #endregion

        /// <summary>
        /// 用户注册时，写入email
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="eamil"></param>
        public void InsertUserMail(int userId, string email)
        {
            UserRepository.InsertUserMail(userId, email);
        }

        public long GetUserCode()
        {
            return UserRepository.GetUserCode();
        }
    }
}
