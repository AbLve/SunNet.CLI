using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sunnet.Framework.Core.Base;

namespace Sunnet.Cli.Core.Users.Entities
{
    public class CoordCoachRoleEntity : EntityBase<int>
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public string SchoolYear { get; set; }
        public string BirthDate { get; set; }
        public string Gender { get; set; }
        public string HomeMailingAddress1 { get; set; }
        public string HomeMailingAddress2 { get; set; }
        public string HomeMailingCity { get; set; }
        public string HomeMailingCountyId { get; set; }
        public string HomeMailingStateId { get; set; }
        public string HomeMailingZip { get; set; }
        public string OfficeAddress1 { get; set; }
        public string OfficeAddress2 { get; set; }
        public string OfficeCity { get; set; }
        public string OfficeCountyId { get; set; }
        public string OfficeStateId { get; set; }
        public string OfficeZip { get; set; }
        public string Ethnicity { get; set; }
        public string PrimaryLanguageId { get; set; }
        public string SecondaryLanguageId { get; set; }
        public string TotalYearsCoaching { get; set; }
        public string EducationLevel { get; set; }
        public string CoachingType { get; set; }
        public string VendorCode { get; set; }
        public string FTE { get; set; }
        public string CLIFundingId { get; set; }
        public string FundedThrough { get; set; }
        public string CoordCoachNotes { get; set; }
    }
}
