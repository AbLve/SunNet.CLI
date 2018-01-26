using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sunnet.Framework.Core.Base;

namespace Sunnet.Cli.Core.Schools.Entities
{
    public class SchoolRoleEntity : EntityBase<int>
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public string SchoolId { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public string StatusDate { get; set; }
        public string SchoolYear { get; set; }
        public string ESCName { get; set; }
        public string ParentAgencyId { get; set; }
        public string PhysicalAddress1 { get; set; }
        public string PhysicalAddress2 { get; set; }
        public string City { get; set; }
        public string CountyId { get; set; }
        public string StateId { get; set; }
        public string Zip { get; set; }
        public string PhoneNumber { get; set; }
        public string PhoneType { get; set; }
        public string SchoolTypeId { get; set; }
        public string AtRiskPercent { get; set; }
        public string FundingId { get; set; }
        public string PrimarySalutation { get; set; }
        public string PrimaryName { get; set; }
        public string PrimaryTitleId { get; set; }
        public string PrimaryPhone { get; set; }
        public string PrimaryPhoneType { get; set; }
        public string PrimaryEmail { get; set; }
        public string SecondarySalutation { get; set; }
        public string SecondaryName { get; set; }
        public string SecondaryTitleId { get; set; }
        public string SecondaryPhoneNumber { get; set; }
        public string SecondaryPhoneType { get; set; }
        public string SecondaryEmail { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string IsSamePhysicalAddress { get; set; }
        public string MailingAddress1 { get; set; }
        public string MailingAddress2 { get; set; }
        public string MailingCity { get; set; }
        public string MailingCountyId { get; set; }
        public string MailingStateId { get; set; }
        public string MailingZip { get; set; }
        public string SchoolSize { get; set; }
        public string IspId { get; set; }
        public string InternetSpeed { get; set; }
        public string InternetType { get; set; }
        public string WirelessType { get; set; }
        public string Notes { get; set; }
    }
}
