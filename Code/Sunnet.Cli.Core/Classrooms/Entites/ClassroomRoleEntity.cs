using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		Leason
 * Computer:		Leason-PC
 * Domain:			Leason-pc
 * CreatedOn:		2014/8/22 14:37:20
 * Description:		Create ClassroomEntity
 * Version History:	Created,2014/8/22 14:37:20
 * 
 * 
 **************************************************************************/
using Sunnet.Cli.Core.Classes.Entites;
using Sunnet.Cli.Core.Classrooms.Enums;
using Sunnet.Cli.Core.Common.Enums;
using Sunnet.Cli.Core.MasterData.Entities;
using Sunnet.Cli.Core.Schools.Entities;
using Sunnet.Framework.Core.Base;
using Sunnet.Framework.Mvc;

namespace Sunnet.Cli.Core.Classrooms.Entites
{
    public class ClassroomRoleEntity : EntityBase<int>
    { 
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public string ClassroomId { get; set; }
        public string CommunityId { get; set; }
        public string SchoolId { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public string StatusDate { get; set; }
        public string SchoolYear { get; set; }
        public string InterventionStatus { get; set; }
        public string InterventionOther { get; set; }
        public string FundingId { get; set; }
        public string KitId { get; set; }
        public string KitUpdatedOn { get; set; }
        public string FcNeedKitId { get; set; }
        public string FcFundingId { get; set; }
        public string Part1KitId { get; set; }
        public string Part1KitUpdatedOn { get; set; }
        public string Part1NeedKitId { get; set; }
        public string Part1FundingId { get; set; }
        public string Part2KitId { get; set; }
        public string Part2KitUpdatedOn { get; set; }
        public string Part2NeedKitId { get; set; }
        public string Part2FundingId { get; set; }
        public string StartupKitId { get; set; }
        public string StartupKitUpdatedOn { get; set; }
        public string StartupNeedKitId { get; set; }
        public string StartupKitFundingId { get; set; }
        public string CurriculumId { get; set; }
        public string CurriculumUpdatedOn { get; set; }
        public string NeedCurriculumId { get; set; }
        public string NeedCurriculumUpdatedOn { get; set; }
        public string CurriculumFundingId { get; set; }
        public string DevelopingTalkersKitId { get; set; }
        public string DevelopingTalkersKitUpdatedOn { get; set; }
        public string DevelopingTalkersNeedKitId { get; set; }
        public string DevelopingTalkerKitFundingId { get; set; }
        public string FccKitId { get; set; }
        public string FccKitUpdatedOn { get; set; }
        public string FccNeedKitId { get; set; }
        public string FccKitFundingId { get; set; }
        public string InternetSpeed { get; set; }
        public string InternetType { get; set; }
        public string WirelessType { get; set; }
        public string IsUsingInClassroom { get; set; }
        public string ComputerNumber { get; set; }
        public string IsInteractiveWhiteboard { get; set; }
        public string MaterialsNotes { get; set; }
        public string TechnologyNotes { get; set; }

    }
}
