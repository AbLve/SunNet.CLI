using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		Leason
 * Computer:		Leason-PC
 * Domain:			Leason-pc
 * CreatedOn:		2014/9/10 16:21:20
 * Description:		Create ClassRolesEntity
 * Version History:	Created,2014/9/10 16:21:20
 * 
 * 
 **************************************************************************/
using Sunnet.Framework.Core.Base;

namespace Sunnet.Cli.Core.Classes.Entites
{
    public class ClassRoleEntity : EntityBase<int>
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public string CommunityId { get; set; }
        public string SchoolId { get; set; }
        public string ClassroomId { get; set; }
        public string ClassId { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public string StatusDate { get; set; }
        public string SchoolYear { get; set; }
        public string AtRiskPercent { get; set; }
        public string DayType { get; set; }
        public string LanguageOfInstruction { get; set; }
        public string CurriculumId { get; set; }
        public string SupplementalCurriculumId { get; set; }
        public string MonitoringToolId { get; set; }
        public string UsedEquipment { get; set; }
        public string ClassType { get; set; }
        public string ClassCount { get; set; }
        public string ReferenceData { get; set; }
        public string Notes { get; set; }
        public string Classlevel { get; set; }
        public string ClassInternalID { get; set; }
    }
}
