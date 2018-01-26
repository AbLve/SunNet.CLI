using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		Leason
 * Computer:		Leason-PC
 * Domain:			Leason-pc
 * CreatedOn:		2014/9/16 14:06:20
 * Description:		Create TeacherRoleEntity
 * Version History:	Created,2014/9/16 14:06:20
 * 
 * 
 **************************************************************************/
using Sunnet.Framework.Core.Base;

namespace Sunnet.Cli.Core.Users.Entities
{
    public class TeacherRoleEntity : EntityBase<int>
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public string UserInfo_GoogleId { get; set; }
        public string CommunityId { get; set; }
        public string SchoolId { get; set; }
        public string TeacherId { get; set; }
        public string SchoolYear { get; set; }
        public string UserInfo_FirstName { get; set; }
        public string UserInfo_MiddleName { get; set; }
        public string UserInfo_LastName { get; set; }
        public string UserInfo_PreviousLastName { get; set; }
        public string BirthDate { get; set; }
        public string Gender { get; set; }
        public string Ethnicity { get; set; }
        public string VendorCode { get; set; }
        public string PrimaryLanguageId { get; set; }
        public string EmployedBy { get; set; }
        public string SecondaryLanguageId { get; set; }
        public string CLIFundingId { get; set; }
        public string MediaRelease { get; set; }
        public string UserInfo_Status { get; set; }
        public string HomeMailingAddress { get; set; }
        public string HomeMailingAddress2 { get; set; }
        public string City { get; set; }
        public string CountyId { get; set; }
        public string StateId { get; set; }
        public string Zip { get; set; }
        public string UserInfo_PrimaryPhoneNumber { get; set; }
        public string UserInfo_PrimaryNumberType { get; set; }
        public string UserInfo_SecondaryPhoneNumber { get; set; }
        public string UserInfo_SecondaryNumberType { get; set; }
        public string UserInfo_FaxNumber { get; set; }
        public string UserInfo_PrimaryEmailAddress { get; set; }
        public string UserInfo_SecondaryEmailAddress { get; set; }
        public string TotalTeachingYears { get; set; }
        public string AgeGroup { get; set; }
        public string CurrentAgeGroupTeachingYears { get; set; }
        public string TeacherType { get; set; }
        public string UserPDRelations { get; set; }
        public string EducationLevel { get; set; }
        public string UserCertificateRelations { get; set; }
        public string CoachId { get; set; }
        public string CoachAssignmentWay { get; set; }
        public string ECIRCLEAssignmentWay { get; set; }
        public string CoachingHours { get; set; }
        public string ReqCycles { get; set; }
        public string YearsInProjectId { get; set; }
        public string CoreAndSupplemental { get; set; }
        public string CoreAndSupplemental2 { get; set; }
        public string CoreAndSupplemental3 { get; set; }
        public string CoreAndSupplemental4 { get; set; }
        public string Permission_UserRole { get; set; }
        public string TeacherNotes { get; set; }
        public string TeacherClassRelations { get; set; }
        public string TeacherTransactions { get; set; }
        public string TeacherTSDSID { get; set; }
    }
}
