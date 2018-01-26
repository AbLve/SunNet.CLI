using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		Leason
 * Computer:		Leason-PC
 * Domain:			Leason-pc
 * CreatedOn:		2014/8/22 14:37:20
 * Description:		Create ClassEntity
 * Version History:	Created,2014/8/22 14:37:20
 * 
 * 
 **************************************************************************/
using Sunnet.Cli.Core.Classes.Enums;
using Sunnet.Cli.Core.Classrooms.Entites;
using Sunnet.Cli.Core.Common.Enums;
using Sunnet.Cli.Core.Communities.Entities;
using Sunnet.Cli.Core.MasterData.Entities;
using Sunnet.Cli.Core.Schools.Entities;
using Sunnet.Cli.Core.Students.Entities;
using Sunnet.Cli.Core.TRSClasses.Enums;
using Sunnet.Framework.Core.Base;
using Sunnet.Framework.Mvc;
using Sunnet.Cli.Core.Users.Entities;

namespace Sunnet.Cli.Core.Classes.Entites
{
    public class ClassEntity : EntityBase<int>
    {
        /// <summary>
        /// 该字段对应Community表里的主键（ID）
        /// </summary>

        [Required]
        [DisplayName("School Name")]
        public int SchoolId { get; set; }


        /// <summary>
        /// 使用触发器来处理值
        /// </summary>
        [EensureEmptyIfNull]
        [StringLength(32)]
        [DisplayName("Class Engage ID")]
        public string ClassId { get; set; }

        [EensureEmptyIfNull]
        [StringLength(150)]
        [DisplayName("Class Internal ID")]
        public string ClassInternalID { get; set; }

        [Required]
        [StringLength(150)]
        [DisplayName("Class Name")]
        public string Name { get; set; }

        [Required]
        [DisplayName("Class Status")]
        public EntityStatus Status { get; set; }

        [EensureEmptyIfNull]
        [DisplayName("Class Status Date")]
        public DateTime StatusDate { get; set; }

        [EensureEmptyIfNull]
        [StringLength(5)]
        [DisplayName("School Year")]
        public string SchoolYear { get; set; }

        [Required]
        [DisplayName("Same as school")]
        public bool IsSameAsSchool { get; set; }

        [DisplayName("% of At-Risk students in class (Approximately)")]
        public int AtRiskPercent { get; set; }

     
        [DisplayName("Day Type")]
        public DayType DayType { get; set; }

        //[Required]
        [DisplayName("Core curriculum used")]
        public int CurriculumId { get; set; }

        [EensureEmptyIfNull]
        [StringLength(150)]
        [DisplayName("Curriculum Other")]
        public string CurriculumOther { get; set; }

        //[Required]
        [DisplayName("Supplemental curriculum used")]
        public int SupplementalCurriculumId { get; set; }

        [EensureEmptyIfNull]
        [StringLength(150)]
        [DisplayName("Supplemental Curriculum Other")]
        public string SupplementalCurriculumOther { get; set; }

        //[Required]
        [DisplayName("Assessment/Progress Monitoring tool used")]
        public int MonitoringToolId { get; set; }

        [StringLength(150)]
        [EensureEmptyIfNull]
        [DisplayName("Monitoring Tool Curriculum Other")]
        public string MonitoringToolOther { get; set; }

        [EensureEmptyIfNull]
        [DisplayName("Equipment Used to Administer CPALLS+")]
        public EquipmentType UsedEquipment { get; set; }

        [EensureEmptyIfNull]
        [StringLength(150)]
        public string EquipmentNumber { get; set; }

        //[Required]
        [DisplayName("Class Type")]
        public ClassType ClassType { get; set; }

        [EensureEmptyIfNull]
        [StringLength(600)]
        [DisplayName("Class Notes")]
        public string Notes { get; set; }

        /// <summary>
        /// 标记Class 是哪个教师创建的，如果不是教师创建，标记为string.Empty
        /// </summary>
        [EensureEmptyIfNull]
        public string Previous_Teacher_TEA_ID { get; set; }

        [DisplayName("Class Level")]
        public int Classlevel { get; set; }

        /// <summary>
        /// Teacher.Id 
        /// </summary>
        [DisplayName("Homeroom Teacher")]
        public int LeadTeacherId { get; set; }

        public bool IsDeleted { get; set; }


        public virtual SchoolEntity School { get; set; }

        public virtual CurriculumEntity Curriculum { get; set; }

        public virtual CurriculumEntity SupplementalCurriculum { get; set; }

        public virtual MonitoringToolEntity MonitoringTool { get; set; }

        //table Class与Language 一对多
        public virtual ICollection<LanguageEntity> Languages { get; set; }


        public virtual ICollection<StudentEntity> Students { get; set; }

        public virtual ICollection<TeacherEntity> Teachers { get; set; }

        public virtual ICollection<UserClassRelationEntity> UserClasses { get; set; }

       
        public virtual TeacherEntity LeadTeacher { get; set; }

        //class与classroomclass 一对多
        public virtual ICollection<ClassroomClassEntity> ClassroomClasses { get; set; }
    }
}
