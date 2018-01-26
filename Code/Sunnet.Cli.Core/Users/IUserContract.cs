using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		Sam
 * Computer:		SAM-PC
 * Domain:			sam-pc
 * CreatedOn:		2014/8/5 16:33:29
 * Description:		Please input class summary
 * Version History:	Created,2014/8/5 16:33:29
 * 
 * 
 **************************************************************************/
using Sunnet.Cli.Core.Users.Entities;
using Sunnet.Cli.Core.Users.Enums;
using Sunnet.Framework.Core.Tool;

namespace Sunnet.Cli.Core.Users
{
    public interface IUserContract
    {
        #region Get Methods
        IQueryable<UserBaseEntity> BaseUsers { get; }

        IQueryable<VideoCodingEntity> VideoCodings { get; }

        IQueryable<CoordCoachEntity> CoordCoachs { get; }

        IQueryable<TeacherEntity> Teachers { get; }

        IQueryable<PrincipalEntity> PrincipalDirectors { get; }

        IQueryable<CommunityUserEntity> DistrictCommunitys { get; }

        IQueryable<ParentEntity> Parents { get; }

        IQueryable<ParentStudentRelationEntity> ParentStudentRelations { get; }

        IQueryable<StateWideEntity> StateWides { get; }

        IQueryable<AuditorEntity> Auditors { get; }

        IQueryable<ApplicantEntity> Applicants { get; }

        IQueryable<TeacherAgeGroupEntity> TeacherAgeGroups { get; }

        IQueryable<TeacherEquipmentRelationEntity> TeacherEquipmentRelations { get; }

        IQueryable<TeacherTransactionEntity> TeacherTransactions { get; }

        IQueryable<AffiliationEntity> Affiliations { get; }

        IQueryable<CertificateEntity> Certificates { get; }

        IQueryable<PositionEntity> Positions { get; }

        IQueryable<YearsInProjectEntity> YearsInProjects { get; }

        IQueryable<ProfessionalDevelopmentEntity> ProfessionalDevelopments { get; }

        IQueryable<UserComSchRelationEntity> UserComSchRelations { get; }

        IQueryable<UserClassRelationEntity> UserClassRelations { get; }

        IQueryable<IntManaCoachRelationEntity> IntManaCoachRelations { get; }

        ProfessionalDevelopmentEntity GetProfessionalDevelopment(int id);

        CertificateEntity GetCertificate(int id);

        ApplicantEntity GetAppliant(int id);

        UserBaseEntity GetUser(int id);

        VideoCodingEntity GetVideoCoding(int id);

        CoordCoachEntity GetCoordCoach(int id);

        TeacherEntity GetTeacher(int teacherId);

        PrincipalEntity GetPrincipal(int id);

        CommunityUserEntity GetCommunity(int id);

        AuditorEntity GetAuditor(int id);

        StateWideEntity GetStateWide(int id);


        #endregion

        #region Insert Methods
        OperationResult InsertParent(ParentEntity parent);

        OperationResult InsertParentStudentRelation(ParentStudentRelationEntity parentStudentRelation);

        OperationResult InsertUser(UserBaseEntity user);

        OperationResult InsertCoordCoach(CoordCoachEntity coordCoach);

        OperationResult InsertTeacher(TeacherEntity teacher);

        OperationResult InsertPrincipal(PrincipalEntity principalDirectors);

        OperationResult InsertCommunity(CommunityUserEntity communityUser);

        OperationResult InsertStateWide(StateWideEntity stateWide);

        OperationResult InsertAuditor(AuditorEntity auditor);

        OperationResult InsertRegistor(RegistorLogEntity registor);

        OperationResult InsertApplication(ApplicantEntity applicant);

        OperationResult InsertApplicantContact(ApplicantContactEntity applicantContact);

        OperationResult InsertApplicantEmail(ApplicantEmailEntity applicantEmail);

        OperationResult InsertPosition(PositionEntity entity);

        OperationResult InsertYearsInProject(YearsInProjectEntity entity);

        OperationResult InsertCertificate(CertificateEntity entity, bool isSave = true);

        OperationResult InsertAffiliation(AffiliationEntity entity);

        OperationResult InsertProfessionalDevelopment(ProfessionalDevelopmentEntity entity);

        OperationResult InsertIntManaCoachRelation(List<IntManaCoachRelationEntity> entities, bool isSave);

        #endregion

        #region Update Methods
        OperationResult UpdateUser(UserBaseEntity user, bool isSave = true, bool isSyncLms = true);

        OperationResult UpdateAppliant(ApplicantEntity applicant, bool isSave = true);

        OperationResult UpdateVideoCoding(VideoCodingEntity videoCoding, bool isSave = true);

        OperationResult UpdateCoordCoach(CoordCoachEntity coordCoach, bool isSave = true);

        OperationResult UpdateParent(ParentEntity parent);

        OperationResult UpdateTeacher(TeacherEntity teacher, bool isSave = true);

        OperationResult UpdatePrincipal(PrincipalEntity principal, bool isSave = true);

        OperationResult UpdateCommunity(CommunityUserEntity communityUser, bool isSave = true);

        OperationResult UpdateStateWide(StateWideEntity stateWide, bool isSave = true);

        OperationResult UpdateAuditor(AuditorEntity auditor, bool isSave = true);

        OperationResult UpdateRegistor(RegistorLogEntity entity);

        OperationResult UpdatePosition(PositionEntity entity);

        OperationResult UpdateYearsInProject(YearsInProjectEntity entity);

        OperationResult UpdateCertificate(CertificateEntity entity, bool isSave = true);

        OperationResult UpdateAffiliation(AffiliationEntity entity);

        OperationResult UpdateProfessionalDevelopment(ProfessionalDevelopmentEntity entity);

        #endregion

        #region   Delete Methods
        OperationResult DeletePDs(List<ProfessionalDevelopmentEntity> listPD);

        OperationResult DeleteCertificates(List<CertificateEntity> listCertificate);

        OperationResult DeleteTeacherAgeGroup(List<TeacherAgeGroupEntity> listTeacherAgeGroup, bool isSave = true);

        OperationResult DeleteTeacherEquipment(List<TeacherEquipmentRelationEntity> listTeacherEquipment);

        OperationResult DeleteCoordCoachEquipment(List<CoordCoachEquipmentEntity> listCoordCoachEquipment);

        OperationResult DeleteCoordCoach(int id, bool isSave = true);

        OperationResult DeleteVideoCoding(int id, bool isSave = true);

        OperationResult DeleteIntManaCoachRelations(List<IntManaCoachRelationEntity> entities, bool isSave);

        OperationResult DeleteParentStudent(ParentStudentRelationEntity parentStudent);
        OperationResult DeleteParentStudent(int parentId, int studentId);
        #endregion

        void SendToUserEmail(string to, string subject, string emailBody);

        #region User CommunitySchool Relations

        OperationResult InsertUserCommunitySchoolRelations(IList<UserComSchRelationEntity> list);

        OperationResult DelUserCommunitySchoolRelations(IList<UserComSchRelationEntity> list);

        #endregion

        #region User Class Relations
        OperationResult InsertUserClassRelations(IList<UserClassRelationEntity> list);

        OperationResult DelUserClassRelations(IList<UserClassRelationEntity> list);
        #endregion

        #region Get User Role
        TeacherRoleEntity GetTeacherRole(Role role);

        PrincipalRoleEntity GetPrincipalRole(Role role);

        CoordCoachRoleEntity GetCoordCoachRole(Role role);
        #endregion

        /// <summary>
        /// 专供批量发送邮件程序使用
        /// 标记已发送邮件
        /// </summary>
        /// <param name="expireTimeDay"></param>
        void UpdateInvitationEmail(int expirationDay, int id);

        /// <summary>
        /// 记录发送的Email记录
        /// </summary>
        /// <param name="log"></param>
        void InsertEmailLog(EmailLogEntity log);

        /// <summary>
        /// 专供批量发送邮件程序使用
        /// </summary>
        /// <param name="sentTime">最多发送次数</param>
        void ResetEmail(int sentTime);

        /// <summary>
        /// 用户注册时，写入email
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="eamil"></param>
        void InsertUserMail(int userId, string eamil);

        long GetUserCode();
    }
}
