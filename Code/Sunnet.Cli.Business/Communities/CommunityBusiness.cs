using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		Leason
 * Computer:		Leason-PC
 * Domain:			Leason-pc
 * CreatedOn:		2014/8/20 15:27:20
 * Description:		Create CommunitiesRspt
 * Version History:	Created,2014/8/20 15:27:20
 * 
 * 
 **************************************************************************/
using Sunnet.Cli.Business.Ade;
using Sunnet.Cli.Business.Common;
using Sunnet.Cli.Business.Communities.Enums;
using Sunnet.Cli.Business.Communities.Models;
using Sunnet.Cli.Business.Cpalls.Models;
using Sunnet.Cli.Business.Schools.Models;
using Sunnet.Cli.Business.Users;
using Sunnet.Cli.Core;
using Sunnet.Cli.Core.Common.Enums;
using Sunnet.Cli.Core.Communities;
using Sunnet.Cli.Core.Communities.Entities;
using Sunnet.Cli.Core.Schools;
using Sunnet.Cli.Core.Schools.Entities;
using Sunnet.Cli.Core.Users.Entities;
using Sunnet.Cli.Core.Users.Enums;
using Sunnet.Framework;
using Sunnet.Framework.Core.Tool;
using Sunnet.Framework.EmailSender;
using Sunnet.Framework.Extensions;
using Sunnet.Framework.Core.Extensions;
using Sunnet.Framework.Helpers;
using LinqKit;
using Sunnet.Cli.Core.Vcw.Entities;
using Sunnet.Cli.Business.Schools;
using Sunnet.Cli.Business.BUP.Model;

namespace Sunnet.Cli.Business.Communities
{
    public class CommunityBusiness
    {
        private readonly ICommunityContract _contract;
        private SchoolBusiness _schoolBus
        {
            get { return new SchoolBusiness(); }
        }
        private AdeBusiness _adeBus
        {
            get { return new AdeBusiness(); }
        }
        public CommunityBusiness(EFUnitOfWorkContext unit = null)
        {
            _contract = DomainFacade.CreateCommunityService(unit);
        }

        #region Community
        public CommunityEntity NewCommunityEntity()
        {
            CommunityEntity entity = _contract.NewCommunityEntity();
            entity.SchoolYear = CommonAgent.SchoolYear;
            entity.StatusDate = DateTime.Now;
            entity.MouStatus = true;
            //entity.ECircle = entity.ECircle ?? string.Empty;
            //entity.Beech = entity.Beech ?? string.Empty;
            //entity.Coaching = entity.Coaching ?? string.Empty;
            //entity.Training = entity.Training ?? string.Empty;
            //entity.Cpalls = entity.Cpalls ?? string.Empty;
            //entity.Materials = entity.Materials ?? string.Empty;
            entity.DistrictNumber = entity.DistrictNumber ?? string.Empty;
            return entity;
        }

        public OperationResult InsertCommunity(CommunityEntity entity, Role role)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            entity.CommunityId = string.Empty;
            BasicCommunityEntity basicCommunityEntity = _contract.GetBasicCommunity(entity.BasicCommunityId);

            if (basicCommunityEntity == null)
            {
                result.Message = CommonAgent.GetInformation("CommunityNameError");
                result.ResultType = OperationResultType.Error;
            }

            else
                if (basicCommunityEntity.Status == EntityStatus.Inactive)
                {
                    result.Message = CommonAgent.GetInformation("CommunityNameHasBeenUsed");
                    result.ResultType = OperationResultType.Error;
                }
                else
                {
                    CommunityRoleEntity roleEntity = GetCommunityRoleEntity(role);
                    entity = InitByRole(entity, roleEntity);
                    //entity.ECircle = entity.ECircle ?? string.Empty;
                    //entity.Beech = entity.Beech ?? string.Empty;
                    //entity.Coaching = entity.Coaching ?? string.Empty;
                    //entity.Training = entity.Training ?? string.Empty;
                    //entity.Cpalls = entity.Cpalls ?? string.Empty;
                    //entity.Materials = entity.Materials ?? string.Empty;
                    entity.DistrictNumber = entity.DistrictNumber ?? string.Empty;
                    entity.Name = basicCommunityEntity.Name;
                    result = _contract.InsertCommunity(entity);
                    if (result.ResultType == OperationResultType.Success)
                    {
                        basicCommunityEntity.Status = EntityStatus.Inactive;
                        result = UpdateBasicCommunity(entity, basicCommunityEntity);
                    }
                }

            return result;
        }


        /// <summary>
        /// Update Community
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public OperationResult UpdateCommunity(CommunityEntity entity, Role role)
        {
            if (entity.MouStatus == false)
            {
                entity.MouDocument = "";
            }
            CommunityRoleEntity roleEntity = GetCommunityRoleEntity(role);
            entity = InitByRole(entity, roleEntity);
            //status is inactive
            if (entity.Status == EntityStatus.Inactive)
                InactiveEnity(ModelName.Community, entity.ID, EntityStatus.Inactive, CommonAgent.SchoolYear);
            //entity.ECircle = entity.ECircle ?? string.Empty;
            //entity.Beech = entity.Beech ?? string.Empty;
            //entity.Coaching = entity.Coaching ?? string.Empty;
            //entity.Training = entity.Training ?? string.Empty;
            //entity.Cpalls = entity.Cpalls ?? string.Empty;
            //entity.Materials = entity.Materials ?? string.Empty;
            entity.DistrictNumber = entity.DistrictNumber ?? string.Empty;
            entity.UpdatedOn = DateTime.Now;

            OperationResult result = new OperationResult(OperationResultType.Success);
            result = _contract.UpdateCommunity(entity);
            if (result.ResultType == OperationResultType.Success)
            {
                BasicCommunityEntity basicCommunityEntity = _contract.GetBasicCommunity(entity.BasicCommunityId);
                result = UpdateBasicCommunity(entity, basicCommunityEntity);
            }
            return result;
        }

        public CommunityEntity GetCommunity(int id)
        {
            return _contract.GetCommunity(id);
        }

        public CommunityEntity GetCommunity(string communityName)
        {
            return _contract.Communities.FirstOrDefault(e => e.Name == communityName);
        }

        public CommunityEntity GetCommunityByEngageId(string engageId)
        {
            return
                _contract.Communities.FirstOrDefault(e => e.CommunityId == engageId && e.Status == EntityStatus.Active);
        }

        public bool CheckIfTrsBySchool(int schoolId)
        {
            int trsId = (int)LocalAssessment.TexasRisingStar;
            return _contract.Communities.Any(r => r.CommunitySchoolRelations.Any(
                x => x.SchoolId == schoolId && x.Community.CommunityAssessmentRelations.Any(
                   y => y.AssessmentId == trsId)));
        }

        public bool CheckIfTrsByCommunity(int communityId)
        {
            int trsId = (int)LocalAssessment.TexasRisingStar;
            return _contract.Communities.Where(r => r.ID == communityId)
                .Any(x => x.CommunityAssessmentRelations.Any(
                   y => y.AssessmentId == trsId));
        }

        public IList<CommunityEntity> GetCommunityListByBasicId(int basicId)
        {
            return _contract.Communities.Where(o => o.BasicCommunityId == basicId).ToList();
        }

        public IEnumerable<SelectItemModel> GetCommunitySelectList(UserBaseEntity user)
        {
            return _contract.Communities.AsExpandable().Where(o => o.Status == EntityStatus.Active).Where(GetRoleCondition(user)).Select(e => new SelectItemModel()
            {
                ID = e.ID,
                Name = e.BasicCommunity.Name
            });
        }

        /// <summary>
        /// 返回Community下拉框的值
        /// </summary>
        /// <param name="exception"></param>
        /// <param name="activeOnly">True,返回有效状态的值；False，返回全部的值</param>
        /// <returns></returns>
        public IEnumerable<CommunitySelectItemModel> GetCommunitySelectList(UserBaseEntity user, Expression<Func<CommunityEntity, bool>> exception, bool activeOnly = true)
        {
            return _contract.Communities.AsExpandable().Where(exception).Where(o => (o.Status == EntityStatus.Active || activeOnly == false))
                .Where(GetRoleCondition(user))
                .Select(e => new CommunitySelectItemModel()
            {
                ID = e.ID,
                Name = e.BasicCommunity.Name,
                City = " " + e.City,
                StateId = e.StateId,
                State = e.State.Name,
                CountyId = e.CountyId,
                Address = e.PhysicalAddress1,
                Address2 = e.PhysicalAddress2,
                Zip = e.Zip
            }).OrderBy(c => c.Name);
        }

        /// <summary>
        /// 读取缓存中的 community 信息，如果不存在缓存，则从数据库读取
        /// </summary> 
        /// <returns></returns>
        public IEnumerable<CommunitySelectItemModel> GetCommunitySelectListForCache(UserBaseEntity user, int communityId, bool activeOnly = true)
        {
            var key = "AllCommunitySelectList";
            var allCommunities = CacheHelper.Get<List<CommunitySelectItemModel>>(key);
            if (allCommunities == null)
            {
                lock (CacheHelper.Synchronize)
                {
                    allCommunities = CacheHelper.Get<List<CommunitySelectItemModel>>(key);
                    if (allCommunities == null)
                    {
                        allCommunities = _contract.Communities.AsExpandable().Where(o => (o.Status == EntityStatus.Active || activeOnly == false))
                                                           .Select(e => new CommunitySelectItemModel()
                                                           {
                                                               ID = e.ID,
                                                               Name = e.BasicCommunity.Name,
                                                               City = " " + e.City,
                                                               StateId = e.StateId,
                                                               State = e.State.Name,
                                                               CountyId = e.CountyId,
                                                               Address = e.PhysicalAddress1,
                                                               Address2 = e.PhysicalAddress2,
                                                               Zip = e.Zip
                                                           }).OrderBy(c => c.Name).ToList();


                        CacheHelper.Add(key, allCommunities, CacheHelper.DefaultExpiredSeconds);
                    }
                }
            }
            if (allCommunities != null)
            {
                var list = allCommunities.ToList();
                if (user.Role != Role.Super_admin)
                {
                    var userComIds = CacheHelper.Get<List<int>>("UserComIds" + user.ID);
                    if (userComIds == null)
                    {
                        userComIds = CacheHelper.Get<List<int>>(key);
                        if (userComIds == null)
                        {
                            userComIds = _contract.Communities.AsExpandable().Where(GetRoleCondition(user)).Select(o => o.ID).ToList();
                            CacheHelper.Add(key, userComIds, CacheHelper.DefaultExpiredSeconds);
                        }
                    }
                    list = list.Where(c => userComIds.Contains(c.ID)).ToList();
                }

                if (communityId > 0)
                    allCommunities.Where(c => c.ID == communityId).ToList();
                return list;
            }
            else
            {
                return new List<CommunitySelectItemModel>();
            }

        }

        /// <summary>
        /// 返回BasicCommunity下拉框的值
        /// </summary>
        /// <param name="exception"></param>
        /// <param name="isActive">True时,返回有效的值；False时，返回所有的值；</param>
        /// <returns></returns>
        public IEnumerable<SelectItemModel> GetBasicCommunitySelectList(Expression<Func<BasicCommunityEntity, bool>> exception,
            bool isActive = true, bool isBasicCommunityModel = false)
        {
            if (isBasicCommunityModel)
            {
                return _contract.BasicCommunities.Where(exception).Where(o => o.Status == EntityStatus.Active || isActive == false)
                    .Select(e => new BasicCommunityModel()
                {
                    ID = e.ID,
                    Name = e.Name,
                    Type = e.Type,
                    Address1 = e.Address1,
                    City = e.City,
                    CountyId = e.CountyId,
                    Phone = e.Phone,
                    Selected = false,
                    StateId = e.StateId,
                    Zip = e.Zip,
                    DistrictNumber = e.DistrictNumber
                });
            }
            else
            {
                return _contract.BasicCommunities.Where(exception).Where(o => o.Status == EntityStatus.Active || isActive == false)
                    .Select(e => new SelectItemModel()
                {
                    ID = e.ID,
                    Name = e.Name
                });
            }
        }

        public BasicCommunityModel GetBasicCommunityModel(int basicCommunityId)
        {
            if (basicCommunityId <= 0)
            {
                return null;
            }
            var basicCommunityEntity = _contract.GetBasicCommunity(basicCommunityId);
            if (basicCommunityEntity != null)
            {
                var basicCommuntiyModel = new BasicCommunityModel();
                basicCommuntiyModel.ID = basicCommunityEntity.ID;
                basicCommuntiyModel.Name = basicCommunityEntity.Name;
                basicCommuntiyModel.Type = basicCommunityEntity.Type;
                basicCommuntiyModel.Address1 = basicCommunityEntity.Address1;
                basicCommuntiyModel.City = basicCommunityEntity.City;
                basicCommuntiyModel.Zip = basicCommunityEntity.Zip;
                basicCommuntiyModel.Phone = basicCommunityEntity.Phone;
                basicCommuntiyModel.CountyId = basicCommunityEntity.CountyId;
                basicCommuntiyModel.StateId = basicCommunityEntity.StateId;
                return basicCommuntiyModel;
            }
            return null;
        }

        public OperationResult UpdateBasicCommunity(CommunityEntity entity, BasicCommunityEntity basicCommunityEntity)
        {
            basicCommunityEntity.UpdatedOn = entity.UpdatedOn;
            basicCommunityEntity.Address1 = entity.PhysicalAddress1;
            basicCommunityEntity.City = entity.City;
            basicCommunityEntity.Zip = entity.Zip;
            basicCommunityEntity.Phone = entity.PhoneNumber;
            basicCommunityEntity.CountyId = entity.CountyId;
            basicCommunityEntity.StateId = entity.StateId;
            basicCommunityEntity.DistrictNumber = entity.DistrictNumber;
            return _contract.UpdateBasicCommunity(basicCommunityEntity);
        }
        #endregion

        public List<CommunityModel> SearchCommunities(UserBaseEntity user, Expression<Func<CommunityEntity, bool>> condition,
           string sort, string order, int first, int count, out int total)
        {
            var query = _contract.Communities.AsExpandable().Where(condition).Where(GetRoleCondition(user))
                .Select(o => new CommunityModel()
            {
                ID = o.ID,
                CommunityId = o.CommunityId,
                CommunityName = o.BasicCommunity.Name,
                FundingName = o.Funding.Name,
                DistrictNumber = o.DistrictNumber,
                Status = o.Status
            });
            total = query.Count();
            var list = query.OrderBy(sort, order).Skip(first).Take(count);
            return list.ToList();
        }


        public CommunityModel IsVerified(int basicCommunityId = 0)
        {
            // var result = false;
            return _contract.Communities.Where(o => o.BasicCommunityId == basicCommunityId
                && o.Status == EntityStatus.Active).Select(e => new CommunityModel()
            {
                ID = e.ID,
                CommunityName = e.BasicCommunity.Name
            }).FirstOrDefault();
        }

        public bool IsSameCommunity(int selectedCommunityId, BasicSchoolEntity basicSchool)
        {
            IList<CommunityEntity> communityList = new List<CommunityEntity>();
            communityList = GetCommunityLists(basicSchool);
            return (communityList.Any(o => o.ID == selectedCommunityId));
        }

        public IList<CommunityEntity> GetCommunityLists(BasicSchoolEntity basicSchool)
        {
            IList<CommunityEntity> communityList = new List<CommunityEntity>();
            if (basicSchool != null)
            {
                communityList = _contract.Communities.Where(o => o.BasicCommunityId == basicSchool.CommunityId).ToList();
            }
            return communityList;
        }

        public List<string> GetCommunityNames(Expression<Func<CommunityEntity, bool>> condition)
        {
            return _contract.Communities.AsExpandable().Where(condition).Select(r => r.Name).ToList();
        }

        #region CommunityFeature
        /// <summary>
        /// 根据communityId获取CommunityFeatureModel
        /// </summary>
        /// <param name="communityId">Community主键</param>
        /// <returns></returns>
        public CommunityFeatureModel GetFeatureModel(int communityId)
        {
            CommunityEntity community = _contract.GetCommunity(communityId);
            CommunityFeatureModel feature = new CommunityFeatureModel();
            if (community != null)
            {
                feature.ID = community.ID;
                feature.Name = community.BasicCommunity.Name;
            }
            return feature;
        }

        public List<int> GetFeatureAssessmentIds(UserBaseEntity userInfo)
        {
            List<int> communityIds = _contract.Communities.AsExpandable()
                  .Where(x => x.Status == EntityStatus.Active)
                  .Where(GetRoleCondition(userInfo))
                  .Select(com => com.ID).ToList();
            return _contract.CommunityAssessmentRelations.Where(r => r.Isrequest == false && communityIds.Contains(r.CommunityId) && r.AssessmentId > 0)
                .Select(r => r.AssessmentId).ToList();
        }



        public void SendEmailToCli(string adminFirstName, string communityFirstLastName,
            string feature, string communityName, string webSiteUrl, string to)
        {
            EmailTemplete templete = XmlHelper.GetEmailTemplete("CommunityFeature_Template.xml");
            string subject = "Community Features Request";
            string emailBody = templete.Body
                .Replace("[AdminUserFirstName]", adminFirstName)
                .Replace("[CommnityUserFirstNameLastName]", communityFirstLastName)
                .Replace("[Features]", feature)
                .Replace("[CommunityName]", communityName)
                .Replace("{StaticDomain}", SFConfig.StaticDomain)
                .Replace("[WebsiteURL]", webSiteUrl);
            _contract.SendEmailToCli(to, subject, emailBody);
        }


        public bool CheckShowAgeofChildren(List<int> communityIds)
        {
            int trsId = (int)LocalAssessment.TexasRisingStar;
            int bEECH = (int)LocalAssessment.BEECH;
            return _contract.CommunityAssessmentRelations.Any(r => communityIds.Contains(r.CommunityId) && (r.AssessmentId == trsId || r.AssessmentId == bEECH));
        }

        #endregion

        #region RoleManage
        public CommunityEntity InitByRole(CommunityEntity community, CommunityRoleEntity role)
        {
            CommunityEntity oldEntity = NewCommunityEntity();

            if (community.ID > 0)
                oldEntity = _contract.GetCommunity(community.ID);

            Type r = role.GetType();
            Type c = community.GetType();
            Type o = oldEntity.GetType();
            foreach (PropertyInfo pi in r.GetProperties())
            {
                string name = pi.Name;
                string value1 = pi.GetValue(role, null).ToString();
                if (value1 != "RW")
                {
                    if (o.GetProperty(name) != null)
                    {
                        object oldValue = o.GetProperty(name).GetValue(oldEntity);
                        c.GetProperty(name).SetValue(community, oldValue);
                    }
                }
            }
            return community;
        }

        public CommunityRoleEntity GetCommunityRoleEntity(Role role)
        {
            Role newRole = role;
            switch (role)
            {
                case Role.District_Community_Delegate:
                    newRole = Role.Community;
                    break;
                case Role.Principal_Delegate:
                    newRole = Role.Principal;
                    break;
                case Role.TRS_Specialist_Delegate:
                    newRole = Role.TRS_Specialist;
                    break;
                case Role.School_Specialist_Delegate:
                    newRole = Role.School_Specialist;
                    break;
                case Role.Community_Specialist_Delegate:
                    newRole = Role.District_Community_Specialist;
                    break;
                default:
                    newRole = role;
                    break;
            }
            return _contract.GetRole(newRole);
        }
        #endregion

        /// <summary>
        /// 改变状态时，相关联的数据也需要修改状态
        /// </summary>
        /// <param name="model"></param>
        /// <param name="entityId"></param>
        /// <param name="status"></param>
        /// <param name="schoolYear"></param>
        /// <returns></returns>
        public bool InactiveEnity(ModelName model, int entityId, EntityStatus status, string schoolYear)
        {
            return _contract.InactiveEnity(model, entityId, status, schoolYear);
        }

        private Expression<Func<CommunityEntity, bool>> GetRoleCondition(UserBaseEntity userInfo)
        {
            Expression<Func<CommunityEntity, bool>> condition = o => true;
            if (userInfo == null)
                return condition;
            UserBusiness userBusiness = new UserBusiness();
            UserBaseEntity baseUser = userBusiness.GetUser(userInfo.ID);
            switch (userInfo.Role)
            {
                case Role.Content_personnel:
                case Role.Statisticians:
                case Role.Administrative_personnel:
                case Role.Intervention_manager:
                case Role.Video_coding_analyst:
                case Role.Intervention_support_personnel:
                case Role.Coordinator:
                case Role.Mentor_coach:
                    condition = PredicateBuilder.And(condition,
                          o => o.UserCommunitySchools.Any(p => p.UserId == userInfo.ID));
                    break;
                case Role.Statewide:
                case Role.Community: //当前community下的所有school的所有 classroom
                case Role.District_Community_Specialist:
                    if (baseUser.CommunityUser != null || baseUser.StateWide != null)
                        condition = PredicateBuilder.And(condition,
                            o => o.UserCommunitySchools.Any(p => p.UserId == userInfo.ID));
                    break;
                case Role.District_Community_Delegate:
                case Role.Community_Specialist_Delegate:
                    if (baseUser.CommunityUser != null)
                        condition = PredicateBuilder.And(condition,
                            o => o.UserCommunitySchools.Any(p => p.UserId == userInfo.CommunityUser.ParentId));
                    break;
                case Role.Parent:
                case Role.Teacher:
                    condition = PredicateBuilder.And(condition,
                        o => o.UserCommunitySchools.Any(p => p.UserId == userInfo.ID));
                    break;
                case Role.Principal:
                case Role.School_Specialist:
                case Role.TRS_Specialist:
                    condition = PredicateBuilder.And(condition,
                        o =>
                            o.UserCommunitySchools.Any(
                                p =>
                                    p.Community.CommunitySchoolRelations.Any(
                                        q =>
                                            q.School.UserCommunitySchools.Any(
                                                r => r.UserId == userInfo.ID))));
                    break;
                case Role.Principal_Delegate:
                case Role.School_Specialist_Delegate:
                case Role.TRS_Specialist_Delegate:
                    condition = PredicateBuilder.And(condition,
                        o =>
                            o.UserCommunitySchools.Any(
                                p =>
                                    p.Community.CommunitySchoolRelations.Any(
                                        q =>
                                            q.School.UserCommunitySchools.Any(
                                                r => r.UserId == userInfo.Principal.ParentId))));
                    break;
            }
            return condition;
        }


        /// <summary>
        /// 根据Communityid集合查找对应的community名称，VCW中PM查找发送任务列表中使用
        /// </summary>
        /// <param name="communityids"></param>
        /// <returns></returns>
        public List<CommunityModel> GetCommunityNames(List<int> communityids)
        {
            return _contract.Communities.Where(a => communityids.Contains(a.ID))
                .Select(a => new CommunityModel
                {
                    ID = a.ID,
                    CommunityName = a.BasicCommunity.Name
                }).Distinct().ToList();
        }

        public List<int> GetBasicComIdByIds(List<int> comIds)
        {
            return _contract.Communities.Where(o => comIds.Contains(o.ID)).Select(c => c.BasicCommunityId).ToList();
        }


        public List<CpallsCommunityModel> GetCommunities(Expression<Func<CommunityEntity, bool>> condition)
        {
            return
                _contract.Communities.AsExpandable()
                    .Where(x => x.Status == EntityStatus.Active)
                    .Where(condition)
                    .Select(com => new CpallsCommunityModel
                    {
                        ID = com.ID,
                        Name = com.BasicCommunity.Name,
                        SchoolCount = com.CommunitySchoolRelations.Count(s => s.Status == EntityStatus.Active)
                    }).ToList();
        }

        public List<int> GetCommunityId(Expression<Func<CommunityEntity, bool>> condition, UserBaseEntity userInfo)
        {
            return
                _contract.Communities.AsExpandable()
                    .Where(x => x.Status == EntityStatus.Active)
                    .Where(condition)
                    .Where(GetRoleCondition(userInfo))
                    .Select(com => com.ID).ToList();
        }

        #region User Community Relations

        public OperationResult InsertCommunitySchoolRelations(int currentUserId, int comId, int[] schoolIds)
        {
            List<CommunitySchoolRelationsEntity> list = new List<CommunitySchoolRelationsEntity>();
            foreach (int schoolId in schoolIds)
            {
                CommunitySchoolRelationsEntity entity = new CommunitySchoolRelationsEntity();
                entity.SchoolId = schoolId;
                entity.CommunityId = comId;
                entity.Status = EntityStatus.Active;
                entity.CreatedBy = currentUserId;
                entity.CreatedOn = DateTime.Now;
                entity.UpdatedOn = DateTime.Now;
                entity.UpdatedBy = currentUserId;
                list.Add(entity);
            }
            return _schoolBus.InsertCommunitySchoolRelations(list);
        }


        #endregion

        #region Community Notes

        public CommunityNotesEntity GetNotesForView(Expression<Func<CommunityNotesEntity, bool>> expression)
        {
            var notes = _contract.CommunityNotes.AsExpandable().Where(expression).ToList();
            if (notes.Count > 0)
                return notes[0];
            else
            {
                CommunityNotesEntity notesEntity = new CommunityNotesEntity();
                return notesEntity;
            }
        }
        public List<CommunityNotesModel> GetCommunityNotes(Expression<Func<CommunityNotesEntity, bool>> expression)
        {
            var notes = _contract.CommunityNotes.AsExpandable().Where(expression).OrderByDescending(o => o.UpdatedOn).Select(n => new CommunityNotesModel
            {
                ID = n.ID,
                CommunityId = n.CommunityId,
                CommunityName = n.Community.Name,
                LogoUrl = n.Community.LogoUrl,
                Messages = n.Messages,
                DisplayLogo = n.DisplayLogo,
                StartOn = n.StartOn,
                StopOn = n.StopOn,
                Status = (CommunityNoteStatus)n.Status
            }).ToList();
            return notes;
        }
        public CommunityNotesEntity GetNote(int noteId)
        {
            return _contract.CommunityNotes.FirstOrDefault(o => o.ID == noteId);

        }
        public CommunityNotesEntity GetCommunityNotes(int communityId)
        {
            if (_contract.CommunityNotes.Count(n => n.CommunityId == communityId) > 0)
            {
                var notes = _contract.CommunityNotes
                    .Where(n => n.CommunityId == communityId).OrderBy(n => n.CreatedBy);
                return notes.ToList()[0];
            }
            else
            {
                CommunityNotesEntity notesEntity = new CommunityNotesEntity();
                notesEntity.CommunityId = communityId;
                return notesEntity;
            }
        }

        public OperationResult NewNote(UserBaseEntity user, CommunityNotesEntity entity)
        {
            entity.CreatedBy = user.ID;
            return _contract.InsertCommunityNotes(entity);
        }


        public OperationResult EditNotes(UserBaseEntity user, CommunityNotesEntity entity)
        {
            entity.UpdateBy = user.ID;
            entity.UpdatedOn = DateTime.Now;
            return _contract.UpdateComunityNotes(entity);
        }

        public bool IsShowCommunityNotes(UserBaseEntity user)
        {
            if (GetCommunityNotes(user).Count() > 0)
                return true;
            else
                return false;
        }

        public List<CommunityNotesModel> GetCommunityNotes(UserBaseEntity user)
        {
            var communities = _contract.Communities.AsExpandable().Where(GetRoleCondition(user));
            List<int> communityIds = communities.Select(c => c.ID).ToList();
            List<CommunityNotesModel> resList = new List<CommunityNotesModel>();
            DateTime todayStart = DateTime.Now.Date;
            DateTime todayEnd = DateTime.Now.AddDays(1).Date.AddSeconds(-1);

            var list = _contract.CommunityNotes
                .Where(n => communityIds.Contains(n.CommunityId)
                    && n.Status == EntityStatus.Active
                    && n.StartOn <= todayEnd
                    && n.StopOn >= todayStart)
                  .Select(n => new CommunityNotesModel
                  {
                      CommunityId = n.CommunityId,
                      CommunityName = n.Community.Name,
                      LogoUrl = n.Community.LogoUrl,
                      Messages = n.Messages,
                      DisplayLogo = n.DisplayLogo,
                      StartOn = n.StartOn,
                      StopOn = n.StopOn,
                      Status = (CommunityNoteStatus)n.Status,
                      UpdateOn = n.UpdatedOn
                  }).ToList();
            foreach (int communityId in communityIds)
            {
                var firstItem = list.Where(o => o.CommunityId == communityId).OrderByDescending(o => o.UpdateOn).FirstOrDefault();
                if (firstItem != null)
                    resList.Add(firstItem);
            }
            resList = resList.OrderBy(o => o.CommunityName).ToList();
            return resList;
        }
        #endregion

        #region Community Assessment relations
        /// <summary>
        /// 
        /// </summary>
        /// <param name="currentUserId"></param>
        /// <param name="communityId"></param>
        /// <param name="features"></param>
        /// <param name="isRequest">"true" means external requets, "false" means internal users</param>
        /// <returns></returns>
        public OperationResult InsertCommunityAssessmentRelations(int currentUserId, int communityId, List<AssessmentFeatureClassLevel> features, bool isRequest)
        {
            List<CommunityAssessmentRelationsEntity> assigneds = GetAssignedAssessments(communityId);
            List<CommunityAssessmentRelationsEntity> list = new List<CommunityAssessmentRelationsEntity>();
            OperationResult res = new OperationResult(OperationResultType.Success);
            //Delete all unchecked items
            List<int> deletedIds = new List<int>();
            var assessmentIds = new int[0];
            if (!isRequest)
            {
                int[] approvedIds = GetAssignedApprovedAssessmentIds(communityId);
                deletedIds = approvedIds.ToList(); //初始值为全部数据
                if (features != null)
                {
                    assessmentIds = features.Select(c => c.AssessmentId).ToArray();
                    deletedIds = approvedIds.Except(assessmentIds).ToList();
                }
                if (deletedIds.Count > 0)
                    res = DeleteCommunityAssessmentRelations(communityId, deletedIds.ToArray());
                if (res.ResultType == OperationResultType.Error)
                    return res;
            }
            else
            {
                int[] requestIds = GetAssignedRequestAssessmentIds(communityId);
                deletedIds = requestIds.ToList(); //初始值为全部数据
                if (features != null)
                {
                    assessmentIds = features.Select(c => c.AssessmentId).ToArray();
                    deletedIds = requestIds.Except(assessmentIds).ToList();
                }
                if (deletedIds.Count > 0)
                    res = DeleteCommunityAssessmentRelations(communityId, deletedIds.ToArray());
                if (res.ResultType == OperationResultType.Error)
                    return res;
            }

            if (features != null)
            {
                var featureList = _adeBus.GetAssessmentList(o => o.IsDeleted == false);
                var localAssessments = LocalAssessment.ADE.ToList().Select(x => (LocalAssessment)x).ToList();
                foreach (AssessmentFeatureClassLevel feature in features)
                {
                    var currentFeature = featureList.FirstOrDefault(o => o.ID == feature.AssessmentId);
                    if (currentFeature != null)
                    {
                        CommunityAssessmentRelationsEntity entity = new CommunityAssessmentRelationsEntity();
                        entity.AssessmentId = feature.AssessmentId;
                        entity.CommunityId = communityId;
                        entity.Status = EntityStatus.Active;
                        entity.CreatedBy = currentUserId;
                        entity.CreatedOn = DateTime.Now;
                        entity.UpdatedOn = DateTime.Now;
                        entity.UpdatedBy = currentUserId;
                        entity.Comment = "";
                        entity.Isrequest = isRequest;
                        entity.ClassLevelIds = feature.ClassLevels != null ? string.Join(",", feature.ClassLevels.OrderBy(d => d).ToList()) : "";
                        CommunityAssessmentRelationsEntity temp = assigneds.FirstOrDefault(o => o.AssessmentId == entity.AssessmentId);
                        if (temp == null)
                        {
                            list.Add(entity);
                        }
                        else
                        {
                            if (temp.ClassLevelIds != entity.ClassLevelIds)
                            {
                                temp.ClassLevelIds = entity.ClassLevelIds;
                            }
                            if (!isRequest)
                            {
                                temp.Isrequest = false;
                            }

                            res = UpdateCommunityAssessmentRelation(temp);
                            if (res.ResultType == OperationResultType.Error)
                            {
                                return res;
                            }
                        }
                        //  Another feature with the same name with current one.
                        var nextOne = featureList.FirstOrDefault(o => o.Name == currentFeature.Name && o.ID != currentFeature.ID);
                        if (nextOne != null)
                        {
                            CommunityAssessmentRelationsEntity nextRelation = new CommunityAssessmentRelationsEntity();
                            nextRelation.AssessmentId = nextOne.ID;
                            nextRelation.CommunityId = communityId;
                            nextRelation.Status = EntityStatus.Active;
                            nextRelation.CreatedBy = currentUserId;
                            nextRelation.CreatedOn = DateTime.Now;
                            nextRelation.UpdatedOn = DateTime.Now;
                            nextRelation.UpdatedBy = currentUserId;
                            nextRelation.Comment = "";
                            nextRelation.Isrequest = isRequest;
                            nextRelation.ClassLevelIds = feature.ClassLevels != null ? string.Join(",", feature.ClassLevels.OrderBy(d => d).ToList()) : "";
                            CommunityAssessmentRelationsEntity nextOnetemp = assigneds.FirstOrDefault(o => o.AssessmentId == nextOne.ID);
                            if (nextOnetemp == null)
                            {
                                list.Add(nextRelation);
                            }
                            else if (deletedIds.Contains(nextOnetemp.AssessmentId))
                            {
                                list.Add(nextRelation);
                            }
                            else
                            {
                                if (nextOnetemp.ClassLevelIds != nextRelation.ClassLevelIds)
                                {
                                    nextOnetemp.ClassLevelIds = nextRelation.ClassLevelIds;
                                }
                                if (!isRequest)
                                {
                                    nextOnetemp.Isrequest = false;
                                }
                                res = UpdateCommunityAssessmentRelation(nextOnetemp);
                                if (res.ResultType == OperationResultType.Error)
                                {
                                    return res;
                                }
                            }

                        }
                    }
                    else if (feature.AssessmentId < 0)
                    {
                        CommunityAssessmentRelationsEntity entity = new CommunityAssessmentRelationsEntity();
                        entity.AssessmentId = feature.AssessmentId;
                        entity.CommunityId = communityId;
                        entity.Status = EntityStatus.Active;
                        entity.CreatedBy = currentUserId;
                        entity.CreatedOn = DateTime.Now;
                        entity.UpdatedOn = DateTime.Now;
                        entity.UpdatedBy = currentUserId;
                        entity.Comment = "";
                        entity.Isrequest = isRequest;
                        entity.ClassLevelIds = feature.ClassLevels != null ? string.Join(",", feature.ClassLevels.OrderBy(d => d).ToList()) : "";
                        CommunityAssessmentRelationsEntity temp = assigneds.FirstOrDefault(o => o.AssessmentId == entity.AssessmentId);
                        if (temp == null)
                        {
                            list.Add(entity);
                        }
                        else
                        {
                            temp = _contract.CommunityAssessmentRelations.FirstOrDefault(o => o.ID == temp.ID);
                            if (entity.ClassLevelIds != temp.ClassLevelIds)
                            {
                                temp.ClassLevelIds = entity.ClassLevelIds;
                            }
                            if (!isRequest)
                            {
                                temp.Isrequest = false;
                            }
                            res = UpdateCommunityAssessmentRelation(temp);
                            if (res.ResultType == OperationResultType.Error)
                            {
                                return res;
                            }
                        }
                    }
                }
            }
            if (list.Count > 0)
                return _contract.InsertRelations(list);
            else
            {
                return res;
            }
        }

        public OperationResult UpdateCommunityAssessmentRelation(CommunityAssessmentRelationsEntity item)
        {

            return _contract.UpdateCommunityAssessmentRelation(item);
        }

        public OperationResult InsertCommunityAssessmentRelations(int currentUserId, int communityId, List<AssessmentSimpleModel> assessments)
        {
            int[] assignedIds = GetAssignedRequestAssessmentIds(communityId);
            OperationResult res = new OperationResult(OperationResultType.Success);
            //Delete all unchecked items
            List<int> deletedIds = assignedIds.Except(assessments.Select(o => o.AssessmentId).ToArray()).ToList();
            if (deletedIds.Count > 0)
                res = DeleteCommunityAssessmentRelations(communityId, deletedIds.ToArray());
            if (res.ResultType == OperationResultType.Error)
                return res;

            var featureList = _adeBus.GetAssessmentList(o => o.IsDeleted == false);
            List<CommunityAssessmentRelationsEntity> list = new List<CommunityAssessmentRelationsEntity>();
            foreach (AssessmentSimpleModel ass in assessments)
            {
                var currentFeature = featureList.FirstOrDefault(o => o.ID == ass.AssessmentId);
                if (currentFeature != null)
                {
                    CommunityAssessmentRelationsEntity entity = new CommunityAssessmentRelationsEntity();
                    entity.AssessmentId = ass.AssessmentId;
                    entity.CommunityId = communityId;
                    entity.Status = EntityStatus.Active;
                    entity.CreatedBy = currentUserId;
                    entity.CreatedOn = DateTime.Now;
                    entity.UpdatedOn = DateTime.Now;
                    entity.UpdatedBy = currentUserId;
                    entity.Comment = ass.Comment;
                    entity.Isrequest = true;
                    if (!assignedIds.Contains(ass.AssessmentId))
                        list.Add(entity);
                }

                //  Another feature with the same name with current one.
                var nextOne = featureList.FirstOrDefault(o => o.Name == currentFeature.Name && o.ID != currentFeature.ID);
                if (nextOne != null)
                {
                    CommunityAssessmentRelationsEntity nextRelation = new CommunityAssessmentRelationsEntity();
                    nextRelation.AssessmentId = nextOne.ID;
                    nextRelation.CommunityId = communityId;
                    nextRelation.Status = EntityStatus.Active;
                    nextRelation.CreatedBy = currentUserId;
                    nextRelation.CreatedOn = DateTime.Now;
                    nextRelation.UpdatedOn = DateTime.Now;
                    nextRelation.UpdatedBy = currentUserId;
                    nextRelation.Comment = ass.Comment;
                    nextRelation.Isrequest = true;
                    if (!assignedIds.Contains(nextOne.ID))
                    {
                        list.Add(nextRelation);
                    }
                }
            }
            return _contract.InsertRelations(list);
        }

        public OperationResult DeleteCommunityAssessmentRelations(int comId, int[] assessmentIds)
        {
            IList<CommunityAssessmentRelationsEntity> list = _contract.GetRelationsByComId(comId, assessmentIds);
            return _contract.DelRelations(list);
        }

        public int[] GetAssignedAssessmentIds(int comId)
        {
            return _contract.GetAssessmentIds(comId);
        }

        public List<int> GetAssignedClassLevelsByComId(int assessmentId, int comId)
        {

            var relationList = _contract.GetRelationsByComId(comId, new[] { assessmentId });
            List<int> ClassLevelIds = new List<int>();
            foreach (var item in relationList)
            {
                var classLevelStr = item.ClassLevelIds.Split(',').ToList();
                foreach (var level in classLevelStr)
                {
                    if (level != "")
                    {
                        int levelId = 0;
                        int.TryParse(level, out levelId);
                        if (!ClassLevelIds.Contains(levelId))
                            ClassLevelIds.Add(levelId);
                    }
                }
            }
            return ClassLevelIds;
        }
        public List<int> GetAssignedClassLevels(int assessmentId, int schoolId)
        {
            var comIds = _schoolBus.GetAssignedCommIds(schoolId);
            var relationList = _contract.GetRelationsByAssessmentId(assessmentId, comIds);
            List<int> ClassLevelIds = new List<int>();
            foreach (var item in relationList)
            {
                var classLevelStr = item.ClassLevelIds.Split(',').ToList();
                foreach (var level in classLevelStr)
                {
                    if (level != "")
                    {
                        int levelId = 0;
                        int.TryParse(level, out levelId);
                        if (!ClassLevelIds.Contains(levelId))
                            ClassLevelIds.Add(levelId);
                    }
                }
            }
            return ClassLevelIds;
        }
        public List<int> GetAssignedClassLevels(int assessmentId, UserBaseEntity userInfo)
        {
            var comIds = GetCommunityIdsByUser(userInfo);
            if (comIds.Count == 0)
                return new List<int>();
            var relationList = _contract.GetRelationsByAssessmentId(assessmentId, comIds.ToArray());
            List<int> ClassLevelIds = new List<int>();
            foreach (var item in relationList)
            {
                var classLevelStr = item.ClassLevelIds.Split(',').ToList();
                foreach (var level in classLevelStr)
                {
                    if (level != "")
                    {
                        int levelId = 0;
                        int.TryParse(level, out levelId);
                        if (!ClassLevelIds.Contains(levelId))
                            ClassLevelIds.Add(levelId);
                    }
                }
            }
            return ClassLevelIds;
        }
        public List<int> GetAssignedClassLevels(int assessmentId, IList<int> schoolIds)
        {
            var comIds = _schoolBus.GetAssignedCommIds(schoolIds);
            if (comIds.Count == 0)
                return new List<int>();
            var relationList = _contract.GetRelationsByAssessmentId(assessmentId, comIds.ToArray());
            List<int> ClassLevelIds = new List<int>();
            foreach (var item in relationList)
            {
                var classLevelStr = item.ClassLevelIds.Split(',').ToList();
                foreach (var level in classLevelStr)
                {
                    if (level != "")
                    {
                        int levelId = 0;
                        int.TryParse(level, out levelId);
                        if (!ClassLevelIds.Contains(levelId))
                            ClassLevelIds.Add(levelId);
                    }
                }
            }
            return ClassLevelIds;
        }
        public int[] GetAssignedRequestAssessmentIds(int comId)
        {
            return
                _contract.CommunityAssessmentRelations.Where(o => o.CommunityId == comId && o.Isrequest)
                    .Select(o => o.AssessmentId)
                    .ToArray();
        }

        public int[] GetAssignedApprovedAssessmentIds(int comId)
        {
            return
                _contract.CommunityAssessmentRelations.Where(o => o.CommunityId == comId && o.Isrequest == false)
                    .Select(o => o.AssessmentId)
                    .ToArray();
        }
        public List<CommunityAssessmentRelationsEntity> GetAssignedAssessments(int comId)
        {
            List<CommunityAssessmentRelationsEntity> list = _contract.GetAssessments(comId);
            return list;
        }
        public List<CommunityAssessmentRelationsEntity> GetAssignedAssessments(List<int> comIds)
        {
            List<CommunityAssessmentRelationsEntity> list = _contract.GetAssessments(comIds);
            return list;
        }

        #endregion




        #region BUP

        //根据用户获取对应的Community
        public List<NameModel> GetCommunitiesByUser(UserBaseEntity user)
        {
            return _contract.Communities.AsExpandable()
                       .Where(x => x.Status == EntityStatus.Active)
                       .Where(GetRoleCondition(user))
                       .Select(com => new NameModel { EngageId = com.CommunityId, InternalId = com.DistrictNumber, Name = com.Name }).ToList();
        }
        public List<int> GetCommunityIdsByUser(UserBaseEntity user)
        {
            return _contract.Communities.AsExpandable()
                       .Where(x => x.Status == EntityStatus.Active)
                       .Where(GetRoleCondition(user))
                       .Select(com => com.ID).ToList();
        }
        public IEnumerable<SelectItemModel> GetCommunitySelectList(UserBaseEntity user, Expression<Func<CommunityEntity, bool>> condition)
        {
            return _contract.Communities.AsExpandable()
                .Where(condition)
                .Where(GetRoleCondition(user)).Select(e => new SelectItemModel()
            {
                ID = e.ID,
                Name = e.BasicCommunity.Name
            });
        }

        #endregion

    }
    public class CommunitySelectItemModel : SelectItemModel
    {
        public string City { get; set; }
        public string Address { get; set; }
        public string Address2 { get; set; }
        public int CountyId { get; set; }
        public int StateId { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }


    }

}
