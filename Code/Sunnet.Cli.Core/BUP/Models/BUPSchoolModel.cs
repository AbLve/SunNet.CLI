using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sunnet.Cli.Core.Common.Enums;

namespace Sunnet.Cli.Core.BUP.Models
{
    public class BUPSchoolModel
    {
        public int ID { get; set; }

        public int TaskId { get; set; }

        public BUPAction Action { get; set; }

        public string CommunityName { get; set; }

        public string CommunityEngageID { get; set; }

        public string Name { get; set; }

        public string EngageID { get; set; }

        public string InternalID { get; set; }

        public string PhysicalAddress1 { get; set; }

        public string PhysicalAddress2 { get; set; }

        public string City { get; set; }

        public int CountyId { get; set; }

        public int StateId { get; set; }

        public string Zip { get; set; }

        public string MailingAddress1 { get; set; }

        public string MailingAddress2 { get; set; }

        public string MailingCity { get; set; }

        public int MailingCountyId { get; set; }

        public int MailingStateId { get; set; }

        public string MailingZip { get; set; }

        public string PhoneNumber { get; set; }

        /// <summary>
        /// PhoneType
        /// </summary>
        public byte PhoneType { get; set; }

        /// <summary>
        /// _schoolBusiness.GetSchoolTypeList
        /// </summary>
        public int SchoolTypeId { get; set; }

        /// <summary>
        /// enter
        /// </summary>
        public int AtRiskPercent { get; set; }

        /// <summary>
        /// enter
        /// </summary>
        public int SchoolSize { get; set; }

        /// <summary>
        /// UserSalutation
        /// </summary>
        public byte PrimarySalutation { get; set; }

        public string PrimaryName { get; set; }

        /// <summary>
        /// _masterBusiness.GetTitleSelectList(3)
        /// </summary>
        public int PrimaryTitleId { get; set; }

        public string PrimaryPhone { get; set; }
        
        /// <summary>
        /// PhoneType
        /// </summary>
        public byte PrimaryPhoneType { get; set; }

        public string PrimaryEmail { get; set; }

        /// <summary>
        /// UserSalutation
        /// </summary>
        public byte SecondarySalutation { get; set; }

        public string SecondaryName { get; set; }

        /// <summary>
        /// _masterBusiness.GetTitleSelectList(4)
        /// </summary>
        public int SecondaryTitleId { get; set; }

        public string SecondaryPhoneNumber { get; set; }

        /// <summary>
        /// PhoneType
        /// </summary>
        public byte SecondaryPhoneType { get; set; }

        public string SecondaryEmail { get; set; }

        public string Latitude { get; set; }

        public string Longitude { get; set; }

        public BUPStatus Status { get; set; }

        public string Remark { get; set; }

        public int LineNum { get; set; }
    }
}
