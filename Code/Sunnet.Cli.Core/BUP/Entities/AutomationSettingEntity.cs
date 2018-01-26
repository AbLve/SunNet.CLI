using Sunnet.Cli.Core.Common.Enums;
using Sunnet.Cli.Core.Communities.Entities;
using Sunnet.Framework.Core.Base;
using Sunnet.Framework.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Cli.Core.BUP.Entities
{
    public class AutomationSettingEntity : EntityBase<int>
    {
        [Required]
        [DisplayName("Community/District")]
        public int CommunityId { get; set; }

        [Required]
        [DisplayName("SFTP Host IP")]
        public string HostIp { get; set; }


        [Required]
        [DisplayName("SFTP Port")]
        public int Port { get; set; }

        [EensureEmptyIfNull]
        [StringLength(100)]
        [DisplayName("SFTP UserName")]
        public string UserName { get; set; }

        [EensureEmptyIfNull]
        [StringLength(100)]
        [DisplayName("SFTP Password")]
        public string PassWord { get; set; }

        [EensureEmptyIfNull]
        [StringLength(200)]
        [DisplayName("School File Path")]
        public string SchoolPath { get; set; }

        [EensureEmptyIfNull]
        [StringLength(200)]
        [DisplayName("Classroom File Path")]
        public string ClassroomPath { get; set; }

        [EensureEmptyIfNull]
        [StringLength(200)]
        [DisplayName("Class File Path")]
        public string ClassPath { get; set; }

        [EensureEmptyIfNull]
        [StringLength(200)]
        [DisplayName("Student File Path")]
        public string StudentPath { get; set; }

        [EensureEmptyIfNull]
        [StringLength(200)]
        [DisplayName("Community/District User File Path")]
        public string CommunityUserPath { get; set; }

        [EensureEmptyIfNull]
        [StringLength(200)]
        [DisplayName("Community/District Specialist File Path")]
        public string CommunitySpecialistPath { get; set; }

        [EensureEmptyIfNull]
        [StringLength(200)]
        [DisplayName("Principal/Director File Path")]
        public string PrincipalPath { get; set; }

        [EensureEmptyIfNull]
        [StringLength(200)]
        [DisplayName("School Specialist File Path")]
        public string SchoolSpecialistPath { get; set; }

        [EensureEmptyIfNull]
        [StringLength(200)]
        [DisplayName("Teacher File Path")]
        public string TeacherPath { get; set; }

        [EensureEmptyIfNull]
        [StringLength(200)]
        [DisplayName("Parent File Path")]
        public string ParentPath { get; set; }

        [Required]
        public EntityStatus Status { get; set; }

        public int CreatedBy { get; set; }

        public int UpdatedBy { get; set; }

        public virtual CommunityEntity Community { get; set; }
    }
}
