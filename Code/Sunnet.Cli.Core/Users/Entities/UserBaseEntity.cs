using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		Sam
 * Computer:		SAM-PC
 * Domain:			sam-pc
 * CreatedOn:		2014/8/5 15:58:17
 * Description:		Please input class summary
 * Version History:	Created,2014/8/5 15:58:17
 * 
 * 
 **************************************************************************/
using Sunnet.Cli.Core.Classes.Entites;
using Sunnet.Cli.Core.Communities.Entities;
using Sunnet.Cli.Core.Tsds.Entities;
using Sunnet.Framework.Core.Base;
using Sunnet.Cli.Core.Common.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using Sunnet.Framework.Mvc;
using Sunnet.Cli.Core.Permission.Entities;
using Sunnet.Cli.Core.Users.Enums;
using Sunnet.Cli.Core.StatusTracking.Entities;

namespace Sunnet.Cli.Core.Users.Entities
{
    public class UserBaseEntity : EntityBase<int>
    {
        public UserBaseEntity()
        {
            Role = 0;
            GoogleId = "";
            FirstName = "";
            MiddleName = "";
            LastName = "";
            PreviousLastName = "";
            Status = EntityStatus.Active;
            StatusDate = DateTime.Parse("01/01/1753");
            PrimaryEmailAddress = "";
            SecondaryEmailAddress = "";
            PrimaryPhoneNumber = "";
            PrimaryNumberType = 0;
            SecondaryPhoneNumber = "";
            SecondaryNumberType = 0;
            FaxNumber = "";
            IsDeleted = false;
            Sponsor = 0;
            InvitationEmail = InvitationEmailEnum.Pending;
            EmailExpireTime = DateTime.Parse("01/01/1753");
            Notes = "";
            Gmail = "";
            IsSyncLms = true;
            InternalID = "";
        }
        /// <summary>
        /// ��Ӧ�� Sunnet.Cli.Core.Users.Enums.Role ;ͬʱ��Ӧ RoleEntity Role
        /// </summary>
        [Required]
        [DisplayName("User Type")]
        public Role Role { get; set; }

        [EensureEmptyIfNullAttribute]
        [StringLength(50)]
        public string GoogleId { get; set; }

        [StringLength(140)]
        [Required]
        [DisplayName("First Name")]
        public string FirstName { get; set; }

        [EensureEmptyIfNullAttribute]
        [StringLength(140)]
        [DisplayName("Middle Name")]
        public string MiddleName { get; set; }

        [StringLength(140)]
        [Required]
        [DisplayName("Last Name")]
        public string LastName { get; set; }

        [EensureEmptyIfNullAttribute]
        [StringLength(140)]
        [DisplayName("Previous Last Name")]
        public string PreviousLastName { get; set; }

        [DisplayName("Status")]
        public EntityStatus Status { get; set; }

        [Required]
        [DisplayName("Status Date")]
        public DateTime StatusDate { get; set; }

        [Required]
        [StringLength(50)]
        [DisplayName("Primary Email")]
        [DataType(DataType.EmailAddress)]
        public string PrimaryEmailAddress { get; set; }

        [EensureEmptyIfNullAttribute]
        [StringLength(50)]
        [DisplayName("Secondary Email")]
        [DataType(DataType.EmailAddress)]
        public string SecondaryEmailAddress { get; set; }

        [Required(AllowEmptyStrings = true)]
        [StringLength(50)]
        [DisplayName("Primary Phone Number")]
        public string PrimaryPhoneNumber { get; set; }

        [Required(AllowEmptyStrings = true)]
        [DisplayName("Primary Number Type")]
        public PhoneType PrimaryNumberType { get; set; }

        [EensureEmptyIfNullAttribute]
        [DisplayName("Secondary Phone Number")]
        [StringLength(50)]
        public string SecondaryPhoneNumber { get; set; }

        [DisplayName("Secondary Number Type")]
        public PhoneType SecondaryNumberType { get; set; }

        [EensureEmptyIfNullAttribute]
        [DisplayName("Fax Number")]
        [StringLength(50)]
        public string FaxNumber { get; set; }

        public bool IsDeleted { get; set; }

        public bool IsCLIUser
        {
            get
            {
                return Role <= Role.Mentor_coach;
            }
        }

        /// <summary>
        /// ��������ߣ�������ͬ���ߣ��������������ߵ�UserId
        /// </summary>
        public int Sponsor { get; set; }

        [DisplayName("Email Invitation")]
        public InvitationEmailEnum InvitationEmail { get; set; }

        /// <summary>
        /// ��� �������ӹ���ʱ��
        /// </summary>
        public DateTime EmailExpireTime { get; set; }

        /// <summary>
        /// ��ע�û�ע�᷽ʽ ��ӦRegisterType
        /// </summary> 
        [EensureEmptyIfNullAttribute]
        [StringLength(100)]
        public string Notes { get; set; }

        [EensureEmptyIfNullAttribute]
        public string Gmail { get; set; }

        [Required]
        [DisplayName("Sync Lms")]
        public bool IsSyncLms { get; set; }

        [EensureEmptyIfNull]
        [StringLength(32)]
        [DisplayName("Internal ID")]
        public string InternalID { get; set; }

        /// <summary>
        /// �ҳ�
        /// </summary>
        public virtual ParentEntity Parent { get; set; }

        public virtual TeacherEntity TeacherInfo { get; set; }

        public virtual PrincipalEntity Principal { get; set; }

        public virtual CommunityUserEntity CommunityUser { get; set; }

        public virtual StateWideEntity StateWide { get; set; }

        public virtual AuditorEntity Auditor { get; set; }

        public virtual VideoCodingEntity VideoCoding { get; set; }

        public virtual CoordCoachEntity CoordCoach { get; set; }

        public virtual ICollection<ProfessionalDevelopmentEntity> ProfessionalDevelopments { get; set; }

        public virtual ICollection<CertificateEntity> Certificates { get; set; }

      

        /// <summary>
        /// Relation User Role:��Super Admin����ڲ��û�, Community User�����û�, Principal�����û�, Teacher
        /// </summary>
        public virtual ICollection<UserComSchRelationEntity> UserCommunitySchools { get; set; }

        /// <summary>
        /// Relation User Role:School Specialist,Community Specialist
        /// </summary>
        public virtual ICollection<UserClassRelationEntity> UserClasses { get; set; }

        /// <summary>
        /// Relation User Role:Coach,Coordinator
        /// </summary>
        public virtual ICollection<IntManaCoachRelationEntity> IntManaCoachRelations { get; set; }

        //��ɫ���û� ����һ���м�� Permission_UserRole
        public virtual ICollection<PermissionRoleEntity> PermissionRoles { get; set; }

        public virtual ICollection<StatusTrackingEntity> StatusTrackings_Approvers { get; set; }

        public virtual ICollection<StatusTrackingEntity> StatusTrackings_Requestors { get; set; }

        public virtual ICollection<StatusTrackingEntity> StatusTracking_SupposedApprover { get; set; }

        public virtual ICollection<DisabledUserRoleEntity> DisabledUsrRoles { get; set; }
    }


    public enum InvitationEmailEnum : byte
    {
        /// <summary>
        /// Saveʱ��״̬��������������ʱ���õ�ֵ
        /// </summary>
        [Description("Not Send")]
        NotSend = 1,

        /// <summary>
        /// Batch ��������ʱ���õ�ֵ������֪ͨ�����ʼ�
        /// </summary>
        [Description("Pending")]
        Pending = 2,

        /// <summary>
        /// Sent   ��ʾ�ѷ����ʼ�
        /// </summary>  
        [Description("Sent")]
        Sent = 3
    }

    public enum RegisterType
    {
        Normal = 1,
        Invitation = 2,
        BatchImport = 3
    }
}
