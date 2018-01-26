using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinqKit;
using System.Threading.Tasks;

using Sunnet.Cli.Core.BUP;
using Sunnet.Cli.Core;
using Sunnet.Cli.Core.BUP.Entities;
using Sunnet.Cli.Core.Users.Entities;
using Sunnet.Cli.Core.BUP.Interfaces;
using System.Linq.Expressions;
using Sunnet.Framework.Core.Extensions;
using Sunnet.Cli.Core.Users.Enums;
using Sunnet.Cli.Business.BUP.Model;
using Sunnet.Framework.Core.Tool;
using Sunnet.Cli.Core.Common.Enums;
using Sunnet.Cli.Business.Communities.Enums;

namespace Sunnet.Cli.Business.BUP
{
    public class AutomationSettingBusiness
    {
        private readonly IBUPContract _contract;
        public AutomationSettingBusiness(EFUnitOfWorkContext unit = null)
        {
            _contract = DomainFacade.CreateBUPService(unit);
        }

        public List<AutomationSettingModel> GetAutomationSettings(UserBaseEntity user, Expression<Func<AutomationSettingEntity, bool>> condition,
            string sort, string order, int first, int count, out int total)
        {
            var query = _contract.AutomationSettings.AsExpandable()
                .Where(condition).Where(GetRoleCondition(user))
                .Select(r => new AutomationSettingModel()
                {
                    ID = r.ID,
                    CommunityId = r.CommunityId,
                    HostIp = r.HostIp,
                    Port = r.Port,
                    UserName = r.UserName,
                    PassWord = r.PassWord,
                    CommunityName = r.Community == null ? "" : r.Community.Name,
                    Status = r.Status
                });
            total = query.Count();
            var list = query.OrderBy(sort, order).Skip(first).Take(count);
            return list.ToList();
        }

        private Expression<Func<AutomationSettingEntity, bool>> GetRoleCondition(UserBaseEntity userInfo)
        {
            Expression<Func<AutomationSettingEntity, bool>> condition = o => false;
            if (userInfo.Role == Role.Super_admin)
                condition = o => true;
            if (userInfo.Role == Role.Community)
                condition = o => o.CreatedBy == userInfo.ID;
            return condition;
        }

        public OperationResult Insert(AutomationSettingEntity entity)
        {
            return _contract.InsertAutomationSetting(entity);
        }

        public AutomationSettingEntity GetEntity(int Id)
        {
            return _contract.GetAutomationSetting(Id);
        }

        public OperationResult Update(AutomationSettingEntity entity)
        {
            return _contract.UpdateAutomationSetting(entity);
        }

        public List<AutomationSettingModel> GetProcessingAutomationSettings()
        {
            return _contract.AutomationSettings
                .Where(r => r.Status == EntityStatus.Active
                    && r.Community.CommunityAssessmentRelations.Any(c => c.AssessmentId == ((int)LocalAssessment.Automation))
                && r.Community.Status == EntityStatus.Active)
                .Select(r => new AutomationSettingModel
                {
                    ID = r.ID,
                    CommunityId = r.CommunityId,
                    HostIp = r.HostIp,
                    Port = r.Port,
                    UserName = r.UserName,
                    PassWord = r.PassWord,
                    SchoolPath = r.SchoolPath,
                    ClassroomPath = r.ClassroomPath,
                    ClassPath = r.ClassPath,
                    StudentPath = r.StudentPath,
                    CommunityUserPath = r.CommunityUserPath,
                    CommunitySpecialistPath = r.CommunitySpecialistPath,
                    PrincipalPath = r.PrincipalPath,
                    SchoolSpecialistPath = r.SchoolSpecialistPath,
                    TeacherPath = r.TeacherPath,
                    ParentPath = r.ParentPath,
                    Status = r.Status,
                    CommunityName = r.Community == null ? "" : r.Community.Name,
                    CreatedBy = r.CreatedBy
                }).ToList();
        }
    }
}
