using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
/**************************************************************************
 * Developer: 		Sam
 * Computer:		SAM-PC
 * Domain:			sam-pc
 * CreatedOn:		2014/8/30 17:44:03
 * Description:		Please input class summary
 * Version History:	Created,2014/8/30 17:44:03
 * 
 * 
 **************************************************************************/
using Sunnet.Cli.Core.Classrooms.Enums;
using Sunnet.Cli.Core.Common.Enums;
using Sunnet.Cli.Core.Schools.Entities;
using Sunnet.Framework.Mvc;

namespace Sunnet.Cli.Business.Classrooms.Models
{
    public class ClassroomModel
    {
        [Required]
        public int ID { get; set; }
        [DisplayName("Classroom Engage ID")]
        public string ClassroomId { get; set; }

        public string ClassroomInternalID { get; set; }

        [Required]
        [DisplayName("Community/District")]
        public int CommunityId { get; set; }

        [JsonIgnore]
        public IEnumerable<string> CommunityNameList { get; set; }

        public string CommunityName
        {
            get
            {
                if (CommunityNameList == null) return string.Empty;
                return string.Join(", ", CommunityNameList);
            }
        }

        [Required]
        [DisplayName("School")]
        public int SchoolId { get; set; }
        [DisplayName("School Name")]
        public string SchoolName { get; set; }

        public SchoolTypeEntity SchoolType { get; set; }

        [Required]
        [StringLength(50)]
        [DisplayName("Classroom Name")]
        public string Name { get; set; }
        [Required]
        [DisplayName("Status")]
        public EntityStatus Status { get; set; }
        [Required]

        [DisplayName("Classroom Status Date")]
        public DateTime StatusDate { get; set; }
        [Required]
        [StringLength(5)]
        [DisplayName("School Year")]
        public string SchoolYear { get; set; }
        [Required]
        [DisplayName("Intervention Status")]
        public InterventionStatus InterventionStatus { get; set; }
        [EensureEmptyIfNull]
        [StringLength(100)]
        [DisplayName("Intervention Other")]
        public string InterventionOther { get; set; }
        [Required]
        [DisplayName("Classroom Funding")]
        public int FundingId { get; set; }
        [Required]
        [DisplayName("Current School Readiness Kit - Full/Complete")]
        public int KitId { get; set; }
        [Required]
        [DisplayName("Last Modified Date")]
        public DateTime KitUpdatedOn { get; set; }
        [Required]
        [DisplayName("School Readiness Kit - FC Needed? ENG/SPA")]
        public int FcNeedKitId { get; set; }
        [Required]
        [DisplayName("School Readiness Kit - FC Funding")]
        public int FcFundingId { get; set; }
        [Required]
        [DisplayName("Current School Readiness Kit - Part 1")]
        public int Part1KitId { get; set; }

        [DisplayName("Last Modified Date")]
        public DateTime Part1KitUpdatedOn { get; set; }
        [Required]
        [DisplayName("School Readiness Kit - Part 1 Needed? ENG/SPA")]
        public int Part1NeedKitId { get; set; }
        [Required]
        [DisplayName("School Readiness Kit - Part 1 Funding")]
        public int Part1FundingId { get; set; }
        [Required]
        [DisplayName("Current School Readiness Kit - Part 2")]
        public int Part2KitId { get; set; }
        [DisplayName("Last Modified Date")]
        public DateTime Part2KitUpdatedOn { get; set; }
        [Required]
        [DisplayName("School Readiness Kit - Part 2 Needed? ENG/SPA")]
        public int Part2NeedKitId { get; set; }
        [Required]
        [DisplayName("School Readiness Kit - Part 2 Funding")]
        public int Part2FundingId { get; set; }
        [Required]
        [DisplayName("Current Classroom Startup Kit")]
        public int StartupKitId { get; set; }
        [DisplayName("Last Modified Date")]
        public DateTime StartupKitUpdatedOn { get; set; }
        [Required]
        [DisplayName("Classroom Startup Kit Needed? ENG/SPA")]
        public int StartupNeedKitId { get; set; }
        [Required]
        [DisplayName("Classroom Startup Kit Funding")]
        public int StartupKitFundingId { get; set; }
        [Required]
        [DisplayName("Current Curriculum Kit")]
        public int CurriculumId { get; set; }
        [DisplayName("Last Modified Date")]
        public DateTime CurriculumUpdatedOn { get; set; }
        [Required]
        [DisplayName("Curriculum Kit Needed?")]
        public int NeedCurriculumId { get; set; }
        [DisplayName("Last Modified Date")]
        public DateTime NeedCurriculumUpdatedOn { get; set; }
        [Required]
        [DisplayName("Curriculum Kit Funding")]
        public int CurriculumFundingId { get; set; }
        [Required]
        [DisplayName("Current Developing Talkers Kit")]
        public int DevelopingTalkersKitId { get; set; }
        [DisplayName("Last Modified Date")]
        public DateTime DevelopingTalkersKitUpdatedOn { get; set; }
        [Required]
        [DisplayName("Developing Talkers Kit Needed? ENG /SPA")]
        public int DevelopingTalkersNeedKitId { get; set; }
        [Required]
        [DisplayName("Developing Talkers Kit Funding")]
        public int DevelopingTalkerKitFundingId { get; set; }
        [Required]
        [DisplayName("Current Family Child Care (FCC) Kit")]
        public int FccKitId { get; set; }
        [Required]
        [DisplayName("Last Modified Date")]
        public DateTime FccKitUpdatedOn { get; set; }
        [Required]
        [DisplayName("Family Child Care (FCC) Kit Needed? ENG /SPA")]
        public int FccNeedKitId { get; set; }
        [Required]
        [DisplayName("Family Child Care (FCC) Kit Funding")]
        public int FccKitFundingId { get; set; }
        [DisplayName("Internet Speed (in Classroom)")]
        public InternetSpeed InternetSpeed { get; set; }
        [Required]
        [DisplayName("Internet Type")]
        public InternetType InternetType { get; set; }
        [DisplayName("Type of Wireless")]
        public WireLessType WirelessType { get; set; }
        [DisplayName("Staff use computer/tablet in classroom")]
        public bool IsUsingInClassroom { get; set; }
        [DisplayName("# of computer(s)/tablet(s) accessible to children")]
        [Range(0, 999)]
        public int ComputerNumber { get; set; }
        [DisplayName("Interactive whiteboard in classroom (e.g. Smartboard)")]
        public bool IsInteractiveWhiteboard { get; set; }
        [EensureEmptyIfNull]
        [StringLength(600, ErrorMessage = "This field is limited to 600 characters.")]
        [DisplayName("Classroom Materials Notes")]
        public string MaterialsNotes { get; set; }
        [EensureEmptyIfNull]
        [StringLength(600, ErrorMessage = "This field is limited to 600 characters.")]
        [DisplayName("Technology Notes")]
        public string TechnologyNotes { get; set; }

        [DisplayName("# Of Children")]
        public int LCchildrenNumber { get; set; }

        [DisplayName("Type Of Children")]
        public int LCchildrenTypeId { get; set; }

        public DateTime UpdatedOn { get; set; }
        public DateTime CreatedOn { get; set; }

        public string RefrenceData { get; set; }
    }
}
