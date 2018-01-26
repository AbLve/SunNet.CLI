using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		Sam
 * Computer:		SAM-PC
 * Domain:			sam-pc
 * CreatedOn:		2014/8/15 14:34:02
 * Description:		Please input class summary
 * Version History:	Created,2014/8/15 14:34:02
 * 
 * 
 **************************************************************************/
using DocumentFormat.OpenXml.Office2010.ExcelAc;
using LinqKit;
using StructureMap;
using Sunnet.Cli.Business.Common;
using Sunnet.Cli.Business.Common.Enum;
using Sunnet.Cli.Business.Communities;
using Sunnet.Cli.Business.Schools.Models;
using Sunnet.Cli.Core.Users.Entities;
using Sunnet.Framework.Core.Tool;
using Sunnet.Cli.Core;
using Sunnet.Cli.Core.MasterData.Entities;
using Sunnet.Cli.Core.MasterData;
using Sunnet.Framework.Encrypt;
using Sunnet.Framework.Extensions;
using Sunnet.Cli.Core.Users.Enums;
using Sunnet.Framework.Helpers;
using Sunnet.Cli.Core.Common.Enums;
using Sunnet.Cli.Business.Users.Models;
using Sunnet.Framework.Core.Extensions;
namespace Sunnet.Cli.Business.Users
{
    public partial class UserBusiness
    {
        private readonly IMasterDataContract masterDataServer = DomainFacade.CreateMasterDataServer();

        public IEnumerable<SelectItemModel> GetLanguages(bool isActive = true)
        {
            IQueryable<LanguageEntity> query = isActive
                ? masterDataServer.Languages.Where(o => o.Status == EntityStatus.Active)
                : masterDataServer.Languages;

            return query.Select(e => new SelectItemModel()
            {
                ID = e.ID,
                Name = e.Language
            });
        }

        public IEnumerable<SelectItemModel> GetFundings()
        {
            return masterDataServer.Fundings.Select(e => new SelectItemModel()
            {
                ID = e.ID,
                Name = e.Name
            });
        }

        public IEnumerable<SelectItemModel> GetCountries()
        {
            return masterDataServer.Counties.Select(e => new SelectItemModel()
            {
                ID = e.ID,
                Name = e.Name
            });
        }

        public IEnumerable<SelectItemModel> GetStates()
        {
            return masterDataServer.States.Select(e => new SelectItemModel()
            {
                ID = e.ID,
                Name = e.Name
            });
        }
        #region YearsInProject
        public IEnumerable<SelectItemModel> GetYearsInProjects(bool isActive = true)
        {
            IQueryable<YearsInProjectEntity> query = isActive
                ? userService.YearsInProjects.Where(o => o.Status == EntityStatus.Active)
                : userService.YearsInProjects;

            return query.Select(e => new SelectItemModel()
            {
                ID = e.ID,
                Name = e.YearsInProject
            });
        }

        public IEnumerable<SelectItemModelOther> GetYearsInProjectsOther()
        {
            return userService.YearsInProjects.Select(e => new SelectItemModelOther()
                {
                    ID = e.ID,
                    Name = e.YearsInProject,
                    Status = e.Status
                });
        }

        public string GetYearsInProjectText(int id)
        {
            return userService.YearsInProjects.Where(e => e.ID == id).Select(x => x.YearsInProject).FirstOrDefault();
        }

        public YearsInProjectEntity GetYearsInProject(int id)
        {
            return userService.YearsInProjects.Where(e => e.ID == id).FirstOrDefault();
        }

        public OperationResult UpdateYearsInProject(YearsInProjectEntity entity)
        {
            return userService.UpdateYearsInProject(entity);
        }

        public OperationResult InsertYearsInProject(YearsInProjectEntity entity)
        {
            return userService.InsertYearsInProject(entity);
        }
        #endregion

        #region Positon Function
        /// <summary>
        /// 
        /// </summary>
        /// <param name="userType">等于0时，返回全部；</param>
        /// <param name="isActive"></param>
        /// <returns></returns>
        public IEnumerable<SelectItemModel> GetPositions(int userType = 0, bool isActive = true)
        {
            return userService.Positions.Where(e => e.UserType == userType || userType == 0)
                .Where(o => o.Status == EntityStatus.Active || isActive == false).Select(e => new SelectItemModel()
            {
                ID = e.ID,
                Name = e.Title
            });
        }

        public IEnumerable<SelectItemModelOther> GetPositionsOther(int userType = 0)
        {
            return userService.Positions.Where(e => e.UserType == userType || userType == 0).ToList()
                .Select(e => new SelectItemModelOther()
                {
                    ID = e.ID,
                    Name = e.Title,
                    Status = e.Status,
                    Other = ((Role)e.UserType).ToDescription(),
                    OtherId = e.UserType
                });
        }

        public OperationResult UpdatePosition(PositionEntity entity)
        {
            return userService.UpdatePosition(entity);
        }

        public PositionEntity GetPosition(int id)
        {
            return userService.Positions.Where(e => e.ID == id).FirstOrDefault();
        }

        public OperationResult InsertPosition(PositionEntity entity)
        {
            return userService.InsertPosition(entity);
        }
        #endregion

        #region Affiliation
        public IEnumerable<SelectItemModel> GetAffiliations(bool isActive = true)
        {
            IQueryable<AffiliationEntity> query = isActive
                ? userService.Affiliations.Where(o => o.Status == EntityStatus.Active)
                : userService.Affiliations;

            return query.Select(e => new SelectItemModel()
            {
                ID = e.ID,
                Name = e.Affiliation
            });
        }

        public IEnumerable<SelectItemModelOther> GetAffiliationsOther()
        {
            return userService.Affiliations.Select(e => new SelectItemModelOther()
                {
                    ID = e.ID,
                    Name = e.Affiliation,
                    Status = e.Status
                });
        }

        public AffiliationEntity GetAffiliation(int id)
        {
            return userService.Affiliations.Where(e => e.ID == id).FirstOrDefault();
        }

        public OperationResult InsertAffiliation(AffiliationEntity entity)
        {
            return userService.InsertAffiliation(entity);
        }

        public OperationResult UpdateAffiliation(AffiliationEntity entity)
        {
            return userService.UpdateAffiliation(entity);
        }
        #endregion

        #region ProfessionalDevelopment
        public IEnumerable<SelectItemModel> GetPDs(bool isActive = true)
        {
            IQueryable<ProfessionalDevelopmentEntity> query = null;
            if (isActive)
                query = userService.ProfessionalDevelopments.Where(o => o.Status == EntityStatus.Active);
            else
                query = userService.ProfessionalDevelopments;

            return query.Select(e => new SelectItemModel()
            {
                ID = e.ID,
                Name = e.ProfessionalDevelopment
            });
        }

        public IEnumerable<SelectItemModelOther> GetPDsOther()
        {
            return userService.ProfessionalDevelopments.Select(e => new SelectItemModelOther()
                {
                    ID = e.ID,
                    Name = e.ProfessionalDevelopment,
                    Status = e.Status
                });
        }

        public ProfessionalDevelopmentEntity GetProfessionalDevelopment(int id)
        {
            return userService.GetProfessionalDevelopment(id);
        }

        public OperationResult InsertProfessionalDevelopment(ProfessionalDevelopmentEntity entity)
        {
            return userService.InsertProfessionalDevelopment(entity);
        }

        public OperationResult UpdateProfessionalDevelopment(ProfessionalDevelopmentEntity entity)
        {
            return userService.UpdateProfessionalDevelopment(entity);
        }
        #endregion

        #region Certificate
        public IEnumerable<SelectItemModel> GetCertificates(bool isActive = true)
        {
            IQueryable<CertificateEntity> query = isActive
                ? userService.Certificates.Where(o => o.Status == EntityStatus.Active && o.IsShow == true)
                : userService.Certificates.Where(o => o.IsShow == true);

            return query.Select(e => new SelectItemModel()
            {
                ID = e.ID,
                Name = e.Certificate,
                Selected = e.IsShow
            });
        }

        public IEnumerable<SelectItemModelOther> GetCertificatesOther()
        {
            return userService.Certificates.Where(o => o.IsShow == true).Select(e => new SelectItemModelOther()
                {
                    ID = e.ID,
                    Name = e.Certificate,
                    Selected = e.IsShow,
                    Status = e.Status
                });
        }

        public CertificateEntity GetCertificate(int id)
        {
            return userService.GetCertificate(id);
        }

        public OperationResult InsertCertificate(CertificateEntity entity)
        {
            entity.IsShow = true;
            userService.InsertCertificate(entity, false);
            CertificateEntity entityOther = new CertificateEntity();
            entityOther.Certificate = entity.Certificate;
            entityOther.IsShow = false;
            entityOther.Status = entity.Status;
            return userService.InsertCertificate(entityOther, true);

        }

        public OperationResult UpdateCertificate(CertificateEntity entity, string oldCertificate)
        {
            //查出同名的Certificate且ID大的一个,也就是isshow为false的certificate实体,然后更新名称
            CertificateEntity noShowCertificateEntity = userService.Certificates.Where(o => o.Status == EntityStatus.Active
                && o.Certificate == oldCertificate).OrderByDescending(e => e.ID).FirstOrDefault();
            noShowCertificateEntity.Certificate = entity.Certificate;
            noShowCertificateEntity.Status = entity.Status;
            noShowCertificateEntity.UpdatedOn = DateTime.Now;
            userService.UpdateCertificate(entity, false);
            return userService.UpdateCertificate(noShowCertificateEntity);
        }
        #endregion

        public List<SelectItemModel> GetTeachersBySchoolId(int schoolId)
        {
            return userService.Teachers.Where(e => e.UserInfo.UserCommunitySchools.Any(x => x.SchoolId == schoolId) &&
                                                   e.UserInfo.Status == EntityStatus.Active)
                .Select(e => new SelectItemModel()
                {
                    ID = e.ID,
                    Name = e.UserInfo.FirstName + " " + e.UserInfo.LastName,
                }).ToList();
        }

        public int GetMaxUser()
        {
            return userService.BaseUsers.Max(e => e.ID);
        }

        public UsernameModel GetUsername(int id)
        {
            return userService.BaseUsers.Where(e => e.ID == id).Select(e => new UsernameModel()
            {
                ID = e.ID,
                Firstname = e.FirstName,
                Lastname = e.LastName
            }).FirstOrDefault();
        }

        public bool IsExistsUserName(int id, string userName)
        {
            return userService.BaseUsers.Where(e => e.ID != id
                && e.GoogleId == userName
                && (int)e.Role <= (int)Role.Mentor_coach
                && e.IsDeleted == false).Count() > 0;
        }

        public List<UsernameModel> GetUsernames(IEnumerable<int> userIds)
        {
            return userService.BaseUsers.Where(e => userIds.Contains(e.ID)).Select(e => new UsernameModel()
            {
                ID = e.ID,
                Firstname = e.FirstName,
                Lastname = e.LastName
            }).ToList();
        }

        public List<int> SearchUserIds(string keyword)
        {
            if (string.IsNullOrEmpty(keyword))
                return null;
            if (string.IsNullOrEmpty(keyword.Trim()))
                return null;
            return userService.BaseUsers.Where(e =>
                keyword.Contains(e.FirstName)
                || keyword.Contains(e.LastName)
                || e.FirstName.Contains(keyword)
                || e.LastName.Contains(keyword)
                ).Select(e => e.ID).ToList();
        }

        public List<UserModel> GetTrsMentors(int communityId)
        {
            List<int> userIds = userService.UserComSchRelations.Where(o => o.CommunityId == communityId
                && o.Community.Status == EntityStatus.Active).Select(e => e.UserId).ToList<int>();
            return userService.CoordCoachs.Where(e => e.User.IsDeleted == false
                                                      && e.User.Status == EntityStatus.Active
                                                      &&
                                                      (e.User.Role == Role.Mentor_coach ||
                                                       e.User.Role == Role.Coordinator)
                                                      && userIds.Contains(e.User.ID))
                .Select(e => new UserModel()
                {
                    ID = e.User.ID,
                    FirstName = e.User.FirstName,
                    LastName = e.User.LastName,
                    Type = (InternalRole)e.User.Role
                }).ToList();

        }
        public List<SelectItemModel> GetCoordCoachs(List<int> communityIds)
        {
            List<int> userIds = userService.UserComSchRelations.Where(o => communityIds.Contains(o.CommunityId)
                && o.Community.Status == EntityStatus.Active).Select(e => e.UserId).ToList<int>();
            return userService.CoordCoachs.Where(e => e.User.IsDeleted == false
                && e.User.Status == EntityStatus.Active
                && (e.User.Role == Role.Mentor_coach || e.User.Role == Role.Coordinator)
                && userIds.Contains(e.User.ID))
                .Select(e => new UserModel()
                {
                    ID = e.User.ID,
                    FirstName = e.User.FirstName,
                    LastName = e.User.LastName,
                    Type = (InternalRole)e.User.Role
                }).ToList()
                .Select(e => new SelectItemModel()
                {
                    ID = e.ID,
                    Name = e.FirstName + " " + e.LastName + " (" + e.Type.ToDescription() + ")"
                }).ToList();
        }

        public List<SelectItemModel> GetProjectManagers()
        {
            return userService.BaseUsers.Where(e => e.IsDeleted == false
                && e.Status == EntityStatus.Active
                && e.Role == Role.Intervention_manager)
                .Select(e => new SelectItemModel()
                {
                    ID = e.ID,
                    Name = e.FirstName + " " + e.LastName
                }).ToList();
        }

        public List<UserModel> GetUsersByRole(Role role)
        {
            return userService.BaseUsers.Where(e => e.IsDeleted == false
                                                    && e.Status == EntityStatus.Active
                                                    && e.Role == role)
                .Select(e => new UserModel()
                {
                    UserId = e.ID,
                    FirstName = e.FirstName,
                    LastName = e.LastName,
                    GoogleId = e.GoogleId,
                    Email = e.PrimaryEmailAddress,
                }).ToList();
        }

        /// <summary>
        /// 查找状态为active的principal
        /// </summary>
        /// <param name="schoolId"></param>
        /// <returns></returns>
        public List<UserModel> GetPrincipalBySchoolId(int schoolId)
        {
            return
                userService.PrincipalDirectors.Where(
                    e => e.UserInfo.UserCommunitySchools.Any(x => x.SchoolId == schoolId)
                         && e.UserInfo.Role == Role.Principal
                         && e.UserInfo.IsDeleted == false
                         && e.SchoolLevelRequest
                         && e.UserInfo.Status == EntityStatus.Active)
                    .Select(e => new UserModel()
                    {
                        FirstName = e.UserInfo.FirstName,
                        LastName = e.UserInfo.LastName,
                        Email = e.UserInfo.PrimaryEmailAddress,
                        GoogleId = e.UserInfo.GoogleId,
                        UserId = e.UserInfo.ID
                    }).ToList();
        }

        /// <summary>
        /// 通过schoolId查找状态为active的communityuser
        /// </summary>
        /// <param name="schoolId"></param>
        /// <returns></returns>
        public List<UserModel> GetActiveCommunityBySchoolId(int schoolId)
        {
            return
                userService.DistrictCommunitys.Where(
                    e =>
                        e.UserInfo.UserCommunitySchools.Any(
                            o => o.Community.CommunitySchoolRelations.Any(p => p.SchoolId == schoolId))
                        && e.UserInfo.Role == Role.Community
                        && e.UserInfo.IsDeleted == false
                        && e.CommunityLevelRequest
                        && e.UserInfo.Status == EntityStatus.Active)
                    .Select(e => new UserModel()
                    {
                        FirstName = e.UserInfo.FirstName,
                        LastName = e.UserInfo.LastName,
                        Email = e.UserInfo.PrimaryEmailAddress,
                        GoogleId = e.UserInfo.GoogleId
                    }).ToList();
        }

        /// <summary>
        /// 查找状态为active的communityuser
        /// </summary>
        /// <param name="communityId"></param>
        /// <param name="isSchoolLevelRequest">如果是Teacher申请，当Teacher选择的学校没有Principal时，发送邮件给Community，
        /// 需要判断Community用户的SchoolLevelRequest的属性为选中状态，
        ///当Principal和School Specialist角色注册时，需要判断Community User的CommunityLevelRequest属性为选中状态</param>
        /// <returns></returns>
        public List<UserModel> GetActiveCommunityByCommunityId(int communityId, bool isSchoolLevelRequest = false)
        {
            List<int> userIds = GetUserIdsByCommunityIds(new List<int>() {communityId});
            return userService.DistrictCommunitys.Where(e =>
                userIds.Contains(e.UserInfo.ID)
                && e.UserInfo.Role == Role.Community
                && e.UserInfo.IsDeleted == false
                && (isSchoolLevelRequest ? e.SchoolLevelRequest : e.CommunityLevelRequest)
                && e.UserInfo.Status == EntityStatus.Active)
                .Select(e => new UserModel()
                {
                    FirstName = e.UserInfo.FirstName,
                    LastName = e.UserInfo.LastName,
                    Email = e.UserInfo.PrimaryEmailAddress,
                    GoogleId = e.UserInfo.GoogleId
                }).ToList();
        }

        public IEnumerable<SelectItemModel> GetMentor_Coachs()
        {
            return userService.BaseUsers.Where(e => e.Role == Role.Mentor_coach
                || e.Role == Role.Coordinator).Select(e => new SelectItemModel()
            {
                ID = e.ID,
                Name = e.FirstName + " " + e.LastName
            });
        }

        public IEnumerable<SelectItemModel> GetMentor_CoachsByUserId(int userId)
        {
            List<int> commIds = GetCommunities(userId);
            List<int> userIds = GetUserIdsByCommunityIds(commIds);
            return userService.BaseUsers.Where(e => (e.Role == Role.Mentor_coach
                                                     || e.Role == Role.Coordinator) && userIds.Contains(e.ID))
                .Select(e => new SelectItemModel()
                {
                    ID = e.ID,
                    Name = e.FirstName + " " + e.LastName
                });
        }

        public IEnumerable<SelectItemModel> GetCoachCoordByCommunity(int communityId)
        {
            List<int> commIds = new List<int>();
            commIds.Add(communityId);
            List<int> userIds = GetUserIdsByCommunityIds(commIds);
            return userService.BaseUsers.Where(e => (e.Role == Role.Mentor_coach
                                                     || e.Role == Role.Coordinator) && userIds.Contains(e.ID))
                .Select(e => new SelectItemModel()
                {
                    ID = e.ID,
                    Name = e.FirstName + " " + e.LastName
                });
        }

        #region get sub user bu googleid
        public List<int> GetSubUserByGoogleId(Role role, int userId)
        {
            List<int> userIds = new List<int>();
            switch (role)
            {
                case Role.Content_personnel:
                case Role.Statisticians:
                case Role.Administrative_personnel:
                case Role.Intervention_manager:
                case Role.Video_coding_analyst:
                case Role.Intervention_support_personnel:
                case Role.Coordinator:
                case Role.Mentor_coach:
                    UserBaseEntity user = userService.BaseUsers.Where(e =>
                        e.ID == userId && e.Status == EntityStatus.Active && e.IsDeleted == false).FirstOrDefault();
                    if (user != null)
                    {
                        List<int> assignCommunityIds =
                            userService.UserComSchRelations.Where(e => e.UserId == user.ID)
                                .Select(e => e.CommunityId).ToList<int>();
                        userIds =
                            userService.UserComSchRelations.Where(e => assignCommunityIds.Contains(e.CommunityId))
                                .Select(e => e.UserId).ToList<int>();

                        List<int> principalUserIds =
                            userService.PrincipalDirectors.Where(
                                m =>
                                    m.UserInfo.UserCommunitySchools.Any(
                                        n =>
                                            n.School.CommunitySchoolRelations.Any(
                                                o => o.Community.UserCommunitySchools.Any(p => p.UserId == user.ID))))
                                .Select(q => q.UserInfo.ID).ToList();

                        List<int> teacherUserIds =
                            userService.Teachers.Where(
                                e => e.UserInfo.UserCommunitySchools.Any(x => assignCommunityIds.Contains(x.CommunityId)))
                                .Select(e => e.UserInfo.ID).ToList<int>();
                        userIds.AddRange(principalUserIds);
                        userIds.AddRange(teacherUserIds);
                    }
                    break;
                case Role.Community:
                case Role.District_Community_Delegate:
                case Role.District_Community_Specialist:
                case Role.Community_Specialist_Delegate:
                    CommunityUserEntity communityUser = userService.DistrictCommunitys.Where(e =>
                        e.UserInfo.ID == userId && e.UserInfo.IsDeleted == false
                        && e.UserInfo.Status == EntityStatus.Active).FirstOrDefault();
                    if (communityUser != null)
                    {
                        userIds =
                            userService.PrincipalDirectors.Where(
                                m =>
                                    m.UserInfo.UserCommunitySchools.Any(
                                        n =>
                                            n.School.CommunitySchoolRelations.Any(
                                                o => o.Community.UserCommunitySchools.Any(p => p.UserId == userId))))
                                .Select(q => q.UserInfo.ID).ToList();

                        List<int> teacherUserIds1 =
                            userService.Teachers.Where(
                                m =>
                                    m.UserInfo.UserCommunitySchools.Any(
                                        n =>
                                            n.School.CommunitySchoolRelations.Any(
                                                o => o.Community.UserCommunitySchools.Any(p => p.UserId == userId))))
                                .Select(q => q.UserInfo.ID).ToList();
                        userIds.AddRange(teacherUserIds1);
                    }
                    break;
                case Role.Principal:
                case Role.Principal_Delegate:
                case Role.TRS_Specialist:
                case Role.TRS_Specialist_Delegate:
                case Role.School_Specialist:
                case Role.School_Specialist_Delegate:
                    List<int> teacherUserIds2 =
                        userService.Teachers.Where(
                            m =>
                                m.UserInfo.UserCommunitySchools.Any(
                                    n => n.School.UserCommunitySchools.Any(o => o.UserId == userId)))
                            .Select(o => o.UserInfo.ID).ToList();
                    userIds.AddRange(teacherUserIds2);
                    break;
            }
            return userIds;
        }

        #endregion

        #region get all users by lms permission

        public List<UserBaseEntity> GetAllUsersByLmsPermission(int pageId)
        {
            List<Role> listRole = permissionBusiness.GetUserTypeByPageId(pageId);
            List<UserBaseEntity> listUsers =
                userService.BaseUsers.Where(
                    e => e.IsSyncLms == true
                         && e.IsDeleted == false
                         && e.Status == EntityStatus.Active
                         && e.PrimaryEmailAddress != ""
                         && listRole.Contains(e.Role)).ToList();
            return listUsers;
        }

        /// <summary>
        /// general lms url 
        /// </summary>
        /// <param name="user"></param>
        /// <param name="lmsDomain"></param>
        /// <returns></returns>
        public string GeneralLmsUrl(UserBaseEntity user, string lmsDomain)
        {
            IEncrypt _encrypt = ObjectFactory.GetInstance<IEncrypt>();
            string lmsurl = string.Format(
                "{0}/auth/cliauth/cli_redirect.php?clirole={1}&useremail={2}&firstname={3}&lastname={4}&userid={5}&status={6}&roletext={7}",
                lmsDomain, _encrypt.Encrypt(((byte) user.Role).ToString()),
                _encrypt.Encrypt(user.PrimaryEmailAddress),
                _encrypt.Encrypt(user.FirstName),
                _encrypt.Encrypt(user.LastName),
                _encrypt.Encrypt(user.ID.ToString()),
                _encrypt.Encrypt(user.Status.ToString()),
                _encrypt.Encrypt(user.Role.ToDescription()));

            if (user.Role == Role.Community || user.Role == Role.District_Community_Delegate
                || user.Role == Role.District_Community_Specialist ||
                user.Role == Role.Community_Specialist_Delegate)
            {
                lmsurl = lmsurl + string.Format("&objectId={0}", _encrypt.Encrypt(user.CommunityUser.CommunityUserId.ToLower()));
            }
            else if (user.Role == Role.Principal || user.Role == Role.Principal_Delegate
                     || user.Role == Role.TRS_Specialist || user.Role == Role.TRS_Specialist_Delegate
                || user.Role == Role.School_Specialist || user.Role == Role.School_Specialist_Delegate)
            {
                lmsurl = lmsurl + string.Format("&objectId={0}", _encrypt.Encrypt(user.Principal.PrincipalId.ToLower()));
            }
            else if (user.Role == Role.Teacher)
            {
                lmsurl = lmsurl +
                         string.Format("&objectId={0}", _encrypt.Encrypt(user.TeacherInfo.TeacherId.ToLower()));
            }
            return lmsurl;
        }

        #endregion


        #region AssignCoordCoaches
        public List<SelectItemModel> GetUnAssignedCoordMentors(Expression<Func<CoordCoachEntity, bool>> condition,
           string sort, string order, int first, int count, out int total)
        {
            var query = userService.CoordCoachs.AsExpandable().Where(condition)
            .Select(r => new SelectItemModel()
            {
                ID = r.User.ID,
                Name = r.User.FirstName + " " + r.User.LastName
            });
            total = query.Count();
            var list = query.OrderBy(sort, order).Skip(first).Take(count);
            return list.ToList();
        }

        public List<SelectItemModel> GetAssignedCoordMentors(Expression<Func<IntManaCoachRelationEntity, bool>> condition,
           string sort, string order, int first, int count, out int total)
        {
            var query = userService.IntManaCoachRelations.AsExpandable().Where(condition)
                .Select(r => new SelectItemModel()
                {
                    ID = r.User.ID,
                    Name = r.User.FirstName + " " + r.User.LastName
                });
            total = query.Count();
            var list = query.OrderBy(sort, order).Skip(first).Take(count);
            return list.ToList();
        }

        public OperationResult AssignCoorCoachRelations(int userId, int operatorId, int[] coorIds)
        {
            if (coorIds != null && coorIds.Length > 0)
            {
                List<IntManaCoachRelationEntity> IntManaCoachRelations = new List<IntManaCoachRelationEntity>();
                foreach (int item in coorIds)
                {
                    IntManaCoachRelationEntity IntManaCoachRelation = new IntManaCoachRelationEntity();
                    IntManaCoachRelation.PMUserId = userId;
                    IntManaCoachRelation.CoordCoachUserId = item;
                    IntManaCoachRelations.Add(IntManaCoachRelation);
                }
                return userService.InsertIntManaCoachRelation(IntManaCoachRelations, true);
            }
            else
            {
                return new OperationResult(OperationResultType.Error);
            }
        }

        public OperationResult DeleteCoorCoachRelations(int userId, int[] coorIds)
        {
            if (coorIds != null && coorIds.Length > 0)
            {
                List<IntManaCoachRelationEntity> IntManaCoachRelations =
                    userService.IntManaCoachRelations.Where(r => r.PMUserId == userId && coorIds.Contains(r.CoordCoachUserId)).ToList();
                return userService.DeleteIntManaCoachRelations(IntManaCoachRelations, true);
            }
            else
            {
                return new OperationResult(OperationResultType.Error);
            }
        }

        public string GetPMByCoordCoach(UserBaseEntity user)
        {
            string pmName = "";
            List<int> pmIds = user.IntManaCoachRelations.Select(r => r.PMUserId).ToList();
            if (pmIds != null && pmIds.Count > 0)
            {
                List<string> pmNames = userService.BaseUsers
                    .Where(r => r.Role == Role.Intervention_manager && r.Status == EntityStatus.Active
                        && r.IsDeleted == false && pmIds.Contains(r.ID))
                    .Select(r => r.FirstName + " " + r.LastName).ToList();
                if (pmNames != null && pmNames.Count > 0)
                {
                    pmName = string.Join(", ", pmNames);
                }
            }
            return pmName;
        }

        #endregion

    }
}
