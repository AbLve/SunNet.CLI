using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		Joe
 * Computer:		JOE-PC
 * Domain:			Joe-pc
 * CreatedOn:		2014/9/26   12:15:10
 * Description:		Please input class summary
 * Version History:	Created,2014/9/26 12:15:10
 * 
 * 
 **************************************************************************/
using Sunnet.Framework.Core.Tool;
using Sunnet.Cli.Core.Users.Entities;
using Sunnet.Cli.Core.Users.Enums;
using Sunnet.Cli.Core.Common.Enums;

namespace Sunnet.Cli.Business.Users
{
    /// <summary>
    /// 用于Profile的基本信息修改
    /// </summary>
    public partial class UserBusiness
    {
        public OperationResult UpdateAuditor(AuditorEntity auditor)
        {           
            OperationResult result = new OperationResult(OperationResultType.Success);
            userService.UpdateUser(auditor.UserInfo, false);
            result = userService.UpdateAuditor(auditor);
            return result;
        }


        public OperationResult UpdateStateWide(StateWideEntity stateWide)
        {          
            OperationResult result = new OperationResult(OperationResultType.Success);
            userService.UpdateUser(stateWide.UserInfo, false);
            result = userService.UpdateStateWide(stateWide);
            return result;
        }

        public OperationResult UpdateCommunityUser(CommunityUserEntity communityUser, List<int> certificates)
        {
            CommunityUserEntity communityUserEntity = userService.GetCommunity(communityUser.ID);
           
            while (communityUserEntity.UserInfo.Certificates.Count > 0)
            {
                communityUserEntity.UserInfo.Certificates.Remove(communityUserEntity.UserInfo.Certificates.First());
            }
            CertificateEntity certificate = new CertificateEntity();
            if (certificates != null)
            {
                foreach (var item in certificates)
                {
                    certificate = userService.GetCertificate(item);
                    communityUserEntity.UserInfo.Certificates.Add(certificate);
                }
            }

            OperationResult result = new OperationResult(OperationResultType.Success);
            userService.UpdateUser(communityUser.UserInfo, false);
            result = userService.UpdateCommunity(communityUser);            
            return result;
        }

        public OperationResult UpdatePrincipal(PrincipalEntity principal, int[] chkPDs, List<int> certificates)
        {
            PrincipalEntity principalEntity = userService.GetPrincipal(principal.ID);
            
            while (principalEntity.UserInfo.Certificates.Count > 0)
            {
                principalEntity.UserInfo.Certificates.Remove(principalEntity.UserInfo.Certificates.First());
            }
            while (principalEntity.UserInfo.ProfessionalDevelopments.Count > 0)
            {
                principalEntity.UserInfo.ProfessionalDevelopments.Remove(principalEntity.UserInfo.ProfessionalDevelopments.First());
            }
            ProfessionalDevelopmentEntity professionalDevelopment = new ProfessionalDevelopmentEntity();
            CertificateEntity certificate = new CertificateEntity();
            if (chkPDs != null)
            {
                principalEntity.UserInfo.ProfessionalDevelopments = new List<ProfessionalDevelopmentEntity>();
                foreach (var item in chkPDs)
                {
                    professionalDevelopment = userService.GetProfessionalDevelopment(item);
                    principalEntity.UserInfo.ProfessionalDevelopments.Add(professionalDevelopment);
                }
            }
            if (certificates != null)
            {
                foreach (var item in certificates)
                {
                    certificate = userService.GetCertificate(item);
                    principalEntity.UserInfo.Certificates.Add(certificate);
                }
            }

            OperationResult result = new OperationResult(OperationResultType.Success);
            userService.UpdateUser(principal.UserInfo, false);
            result = userService.UpdatePrincipal(principal);
           
            return result;
        }

        public OperationResult UpdateTeacher(TeacherEntity teacher, int[] ageGroups, int[] chkPDs,
            List<int> certificates, Role role)
        {
            TeacherRoleEntity teacherRoleEntity = GetTeacherRoleEntity(role);
            InitByRole(teacher, teacherRoleEntity);
            TeacherEntity teacherEntity = GetTeacher(teacher.ID, null);
                     
            while (teacherEntity.UserInfo.Certificates.Count > 0)
            {
                teacherEntity.UserInfo.Certificates.Remove(teacherEntity.UserInfo.Certificates.First());
            }
            while (teacherEntity.UserInfo.ProfessionalDevelopments.Count > 0)
            {
                teacherEntity.UserInfo.ProfessionalDevelopments.Remove(teacherEntity.UserInfo.ProfessionalDevelopments.First());
            }

            userService.DeleteTeacherAgeGroup(teacherEntity.TeacherAgeGroups.ToList(), false);

            ProfessionalDevelopmentEntity professionalDevelopment = new ProfessionalDevelopmentEntity();
            CertificateEntity certificate = new CertificateEntity();
            if (ageGroups != null)
            {
                teacherEntity.TeacherAgeGroups = new List<TeacherAgeGroupEntity>();
                foreach (var item in ageGroups)
                {
                    TeacherAgeGroupEntity teacherAgeGroup = new TeacherAgeGroupEntity();
                    teacherAgeGroup.AgeGroup = item;
                    teacherAgeGroup.AgeGroupOther = "";
                    if ((AgeGroup)item == AgeGroup.Other)
                        teacherAgeGroup.AgeGroupOther = teacher.AgeGroupOther;
                    teacherEntity.TeacherAgeGroups.Add(teacherAgeGroup);
                }
            }
            if (chkPDs != null)
            {
                teacherEntity.UserInfo.ProfessionalDevelopments = new List<ProfessionalDevelopmentEntity>();
                foreach (var item in chkPDs)
                {
                    professionalDevelopment = userService.GetProfessionalDevelopment(item);
                    teacherEntity.UserInfo.ProfessionalDevelopments.Add(professionalDevelopment);
                }
            }
            if (certificates != null)
            {
                foreach (var item in certificates)
                {
                    certificate = userService.GetCertificate(item);
                    teacherEntity.UserInfo.Certificates.Add(certificate);
                }
            }

            OperationResult result = new OperationResult(OperationResultType.Success);
            userService.UpdateUser(teacher.UserInfo, false);
            result = userService.UpdateTeacher(teacher);
            return result;
        }
    }
}
