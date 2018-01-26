using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Linq.Expressions;
using Sunnet.Cli.Core.Vcw.Entities;
using Sunnet.Cli.Core.Users.Enums;
using Sunnet.Framework.Helpers;
using Sunnet.Cli.Core.Vcw.Enums;
using Sunnet.Cli.Business.Users;
using Sunnet.Cli.Business.Vcw;
using Sunnet.Framework.Extensions;
using Sunnet.Cli.Core.Users.Entities;

namespace Sunnet.Cli.Vcw.Models
{
    public static class GetExpression
    {
        //Coach查看TeacherVIPAssignment和TeacherCoachingAssignment
        public static Expression<Func<AssignmentEntity, bool>> GetAssignmentExpression(int? community, int? school, int? teacher,
            int? status, AssignmentTypeEnum? assignmentType, UserBaseEntity userInfo)
        {
            int userId = userInfo.ID;
            Role role = userInfo.Role;
            var expression = PredicateHelper.True<AssignmentEntity>();

            if (status != null && status > 0)
                expression = expression.And(o => o.Status == (AssignmentStatus)status);
            if (community != null && community > 0)
            {
                List<int> teacherIds = new UserBusiness().GetTeacheridsByCommunity(community.Value);
                expression = expression.And(o => teacherIds.Contains(o.ReceiveUserId));
            }
            if (school != null && school > 0)
            {
                List<int> teacherIds = new UserBusiness().GetTeacheridsBySchool(school.Value);
                expression = expression.And(o => teacherIds.Contains(o.ReceiveUserId));
            }
            if (teacher != null && teacher > 0)
                expression = expression.And(o => o.ReceiveUserId == teacher);
            if (assignmentType != null)
                expression = expression.And(o => o.AssignmentType == assignmentType);

            List<int> teacherids;
            if (role == Role.Coordinator)
                teacherids = new UserBusiness().GetTeacherIdsByPM(userId);
            else if (role == Role.Mentor_coach)
                teacherids = new UserBusiness().GetAssignedTeacherIdsByCoach(userId);
            else
                teacherids = new UserBusiness().GetTeacherIdsByExternalUser(userInfo);

            expression = expression.And(o => teacherids.Contains(o.ReceiveUserId) && o.IsDelete == false);
            return expression;
        }

        //获取TeacherGeneral时
        public static Expression<Func<Vcw_FileEntity, bool>> GetTeacherGeneralExpression(int? community, int? school, int? coach, int? teacher,
             string number, UserBaseEntity userInfo)
        {
            int userId = userInfo.ID;
            Role role = userInfo.Role;
            var expression = PredicateHelper.True<Vcw_FileEntity>();
            if (community != null && community > 0)
            {
                List<int> teacherIds = new UserBusiness().GetTeacheridsByCommunity(community.Value);
                expression = expression.And(o => teacherIds.Contains(o.OwnerId));
            }
            if (school != null && school > 0)
            {
                List<int> teacherIds = new UserBusiness().GetTeacheridsBySchool(school.Value);
                expression = expression.And(o => teacherIds.Contains(o.OwnerId));
            }
            if (coach != null && coach > 0)
                expression = expression.And(o => o.UserInfo.TeacherInfo.CoachId == coach);
            if (teacher != null && teacher > 0)
                expression = expression.And(o => o.OwnerId == teacher);
            if (!string.IsNullOrEmpty(number))
                expression = expression.And(GetDropDownItems.GetNumberExpression(number));

            List<int> teacherids;
            if (role == Role.Coordinator || role == Role.Intervention_manager)
                teacherids = new UserBusiness().GetTeacherIdsByPM(userId);
            else if (role == Role.Mentor_coach)
                teacherids = new UserBusiness().GetAssignedTeacherIdsByCoach(userId);
            else
                teacherids = new UserBusiness().GetTeacherIdsByExternalUser(userInfo);

            expression = expression.And(o => teacherids.Contains(o.OwnerId)
                && o.VideoType == FileTypeEnum.TeacherGeneral && o.IsDelete == false);
            return expression;
        }

        //获取TeacherSummary
        public static Expression<Func<Vcw_FileEntity, bool>> GetTeacherSummaryExpression(int? community,
            int? school, int? teacher, int? uploadby, int? videotype, string number, UserBaseEntity userInfo)
        {
            int userId = userInfo.ID;
            Role role = userInfo.Role;
            var expression = PredicateHelper.True<Vcw_FileEntity>();
            if (community != null && community > 0)
            {
                List<int> teacherIds = new UserBusiness().GetTeacheridsByCommunity(community.Value);
                expression = expression.And(o => teacherIds.Contains(o.OwnerId));
            }
            if (school != null && school > 0)
            {
                List<int> teacherIds = new UserBusiness().GetTeacheridsBySchool(school.Value);
                expression = expression.And(o => teacherIds.Contains(o.OwnerId));
            }
            if (teacher != null && teacher > 0)
                expression = expression.And(o => o.OwnerId == teacher);
            if (uploadby == UploadUserTypeEnum.Coach.GetValue())
                expression = expression.And(o => (o.UploadUserType == UploadUserTypeEnum.Coach
                    || o.UploadUserType == UploadUserTypeEnum.PM || o.UploadUserType == UploadUserTypeEnum.Admin));
            if (uploadby == UploadUserTypeEnum.Teacher.GetValue())//Teacher自己上传的文件
                expression = expression.And(o => o.UploadUserType == UploadUserTypeEnum.Teacher);
            if (videotype != null && videotype > 0)
                expression = expression.And(o => o.VideoType == (FileTypeEnum)videotype);
            else
                expression = expression.And(o => o.VideoType == FileTypeEnum.TeacherGeneral
                    || o.VideoType == FileTypeEnum.TeacherVIP || o.VideoType == FileTypeEnum.TeacherAssignment);
            if (!string.IsNullOrEmpty(number))
                expression = expression.And(GetDropDownItems.GetNumberExpression(number));

            List<int> teacherids;
            if (role == Role.Coordinator)
                teacherids = new UserBusiness().GetTeacherIdsByPM(userId);
            else if (role == Role.Mentor_coach)
                teacherids = new UserBusiness().GetAssignedTeacherIdsByCoach(userId);
            else
                teacherids = new UserBusiness().GetTeacherIdsByExternalUser(userInfo);
            expression = expression.And(o => teacherids.Contains(o.OwnerId) && o.IsDelete == false);
            return expression;
        }


        //PM查看TeacherAssignment
        public static Expression<Func<AssignmentEntity, bool>> GetPMTeacherAssignmentExpression(int? community, int? school, int? coach,
            int? teacher, int? status, AssignmentTypeEnum? assignmentType, bool isAdmin, int userId)
        {
            var expression = PredicateHelper.True<AssignmentEntity>();
            List<int> teacherids = new List<int>();

            if (status != null && status > 0)
                expression = expression.And(o => o.Status == (AssignmentStatus)status);
            if (community != null && community > 0)
            {
                List<int> teacherIds = new UserBusiness().GetTeacheridsByCommunity(community.Value);
                expression = expression.And(o => teacherIds.Contains(o.ReceiveUserId));
            }
            if (school != null && school > 0)
            {
                List<int> teacherIds = new UserBusiness().GetTeacheridsBySchool(school.Value);
                expression = expression.And(o => teacherIds.Contains(o.ReceiveUserId));
            }
            if (teacher != null && teacher > 0)
                expression = expression.And(o => o.ReceiveUserId == teacher);
            if (coach > 0)
                expression = expression.And(o => o.UserInfo.TeacherInfo.CoachId == coach);
            if (assignmentType != null)
                expression = expression.And(o => o.AssignmentType == assignmentType);
            //if (isAdmin)
            //    teacherids = new UserBusiness().GetAllTeachers().Select(a => a.ID).ToList();
            if (!isAdmin)
            {
                teacherids = new UserBusiness().GetTeacherIdsByPM(userId);
                expression = expression.And(o => teacherids.Contains(o.ReceiveUserId) && o.IsDelete == false);
            }
           
            expression = expression.And(o =>  o.IsDelete == false);
            return expression;
        }

        //PM查看Coach Summary
        public static Expression<Func<Vcw_FileEntity, bool>> GetPMCoachSummaryExpression(int community, int coach,
            int uploadby, int videotype, string number, bool isAdmin, int userId)
        {
            List<int> Coache_id;
            if (isAdmin)
                Coache_id = new UserBusiness().GetCoach().Select(a => a.ID).ToList();
            else
                Coache_id = new UserBusiness().GetCoachByPM(userId).Select(a => a.ID).ToList();
            Expression<Func<Vcw_FileEntity, bool>> fileContition = PredicateHelper.True<Vcw_FileEntity>();
            fileContition = fileContition.And(r => Coache_id.Contains(r.OwnerId));
            fileContition = fileContition.And(r => r.IsDelete == false);

            if (community > 0)
            {
                List<int> user_Ids = new UserBusiness().GetAssignedUsersByCommunity(community).Select(a => a.UserId).ToList();
                fileContition = fileContition.And(r => user_Ids.Contains(r.OwnerId));
            }

            if (coach > 0)
                fileContition = fileContition.And(r => r.OwnerId == coach);

            if (uploadby == UploadUserTypeEnum.Coach.GetValue()) //Coach上传
                fileContition = fileContition.And(r => r.UploadUserType == UploadUserTypeEnum.Coach);

            if (uploadby == UploadUserTypeEnum.PM.GetValue()) //PM上传
                fileContition = fileContition.And(r => r.UploadUserType == UploadUserTypeEnum.PM);

            if (uploadby == UploadUserTypeEnum.Admin.GetValue()) //Admin上传
                fileContition = fileContition.And(r => r.UploadUserType == UploadUserTypeEnum.Admin);

            if (videotype > 0)
                fileContition = fileContition.And(r => r.VideoType == (FileTypeEnum)videotype);
            else
                fileContition = fileContition.And(r => (r.VideoType == FileTypeEnum.CoachGeneral || r.VideoType == FileTypeEnum.CoachAssignment));

            if (!string.IsNullOrEmpty(number))
                fileContition = fileContition.And(GetDropDownItems.GetNumberExpression(number));

            return fileContition;
        }

        //PM查看Teacher Summary
        public static Expression<Func<Vcw_FileEntity, bool>> GetPMTeacherSummaryExpression(int community, int school,
            int teacher, int uploadby, int videotype, string number, int userId)
        {
            List<int> teacherIds = new UserBusiness().GetTeacherIdsByPM(userId);
            var expression = PredicateHelper.True<Vcw_FileEntity>();
            if (community > 0)
            {
                List<int> comuser_Ids = new UserBusiness().GetAssignedUsersByCommunity(community).Select(a => a.UserId).ToList();
                expression = expression.And(r => comuser_Ids.Contains(r.OwnerId));
            }
            if (school > 0)
            {
                List<int> schooluser_Ids = new UserBusiness().GetTeacheridsBySchool(school);
                expression = expression.And(r => schooluser_Ids.Contains(r.OwnerId));
            }
            if (teacher > 0)
                expression = expression.And(o => o.OwnerId == teacher);

            if (uploadby == UploadUserTypeEnum.Teacher.GetValue())
                expression = expression.And(o => o.UploadUserType == UploadUserTypeEnum.Teacher);
            if (uploadby == UploadUserTypeEnum.Coach.GetValue())
                expression = expression.And(o => o.UploadUserType == UploadUserTypeEnum.Coach);
            if (uploadby == UploadUserTypeEnum.PM.GetValue())
                expression = expression.And(o => o.UploadUserType == UploadUserTypeEnum.PM);
            if (uploadby == UploadUserTypeEnum.Admin.GetValue())
                expression = expression.And(o => o.UploadUserType == UploadUserTypeEnum.Admin);

            if (videotype > 0)
                expression = expression.And(o => o.VideoType == (FileTypeEnum)videotype);
            else
                expression = expression.And(o => o.VideoType == FileTypeEnum.TeacherGeneral
                    || o.VideoType == FileTypeEnum.TeacherVIP || o.VideoType == FileTypeEnum.TeacherAssignment);

            if (!string.IsNullOrEmpty(number))
                expression = expression.And(GetDropDownItems.GetNumberExpression(number));

            expression = expression.And(o => teacherIds.Contains(o.OwnerId) && o.IsDelete == false);

            return expression;
        }


        public static Expression<Func<Vcw_FileEntity, bool>> GetAdminTeacherSummaryExpression(int community, int school,
           int teacher, int uploadby, int videotype, string number)
        {
            List<int> teacherIds = new UserBusiness().GetAllTeachers()
                    .Select(a => a.ID).ToList();
            var expression = PredicateHelper.True<Vcw_FileEntity>();
            if (community > 0)
            {
                List<int> comuser_Ids = new UserBusiness().GetAssignedUsersByCommunity(community).Select(a => a.UserId).ToList();
                expression = expression.And(r => comuser_Ids.Contains(r.OwnerId));
            }
            if (school > 0)
            {
                List<int> schooluser_Ids = new UserBusiness().GetTeacheridsBySchool(school);
                expression = expression.And(r => schooluser_Ids.Contains(r.OwnerId));
            }
            if (teacher > 0)
                expression = expression.And(o => o.OwnerId == teacher);

            if (uploadby == UploadUserTypeEnum.Teacher.GetValue())
                expression = expression.And(o => o.UploadUserType == UploadUserTypeEnum.Teacher);
            if (uploadby == UploadUserTypeEnum.Coach.GetValue())
                expression = expression.And(o => o.UploadUserType == UploadUserTypeEnum.Coach);
            if (uploadby == UploadUserTypeEnum.PM.GetValue())
                expression = expression.And(o => o.UploadUserType == UploadUserTypeEnum.PM);
            if (uploadby == UploadUserTypeEnum.Admin.GetValue())
                expression = expression.And(o => o.UploadUserType == UploadUserTypeEnum.Admin);

            if (videotype > 0)
                expression = expression.And(o => o.VideoType == (FileTypeEnum)videotype);
            else
                expression = expression.And(o => o.VideoType == FileTypeEnum.TeacherAssignment ||
                    o.VideoType == FileTypeEnum.TeacherGeneral || o.VideoType == FileTypeEnum.TeacherVIP);
            if (!string.IsNullOrEmpty(number))
                expression = expression.And(GetDropDownItems.GetNumberExpression(number));

            expression = expression.And(o => teacherIds.Contains(o.OwnerId) && o.IsDelete == false);

            return expression;
        }
    }
}