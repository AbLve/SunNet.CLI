using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sunnet.Cli.Core.Common.Enums;

namespace Sunnet.Cli.Core.BUP.Models
{
    public class BUPCommunityModel
    {
        public int ID { get; set; }

        public BUPAction Action { get; set; }

        /// <summary>
        /// Community_Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Community_Engage_ID
        /// </summary>
        public string EngageID { get; set; }

        /// <summary>
        /// Community_Internal_ID
        /// </summary>
        public string InternalID { get; set; }

        /// <summary>
        /// Community_Physical_Address1
        /// </summary>
        public string Address1 { get; set; }

        /// <summary>
        /// Community_Physical_Address2
        /// </summary>
        public string Address2 { get; set; }

        /// <summary>
        /// Community_City
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// Community State
        /// </summary>
        public int State { get; set; }

        /// <summary>
        /// Community_ZIP
        /// </summary>
        public string Zip { get; set; }

        /// <summary>
        /// Community_Phone_Number
        /// </summary>
        public string PhoneNumber { get; set; }

        /// <summary>
        /// Community_Phone_Number_Type  PhoneType
        /// </summary>
        public byte PhoneNumberType { get; set; }

        /// <summary>
        /// Community_Primary_Contact_Salutation  UserSalutation
        /// </summary>
        public byte PrimarySalutation { get; set; }

        /// <summary>
        /// Community_Primary_Contact_Name
        /// </summary>
        public string PrimaryName { get; set; }

        /// <summary>
        /// Community_Primary_Contact_Title
        /// </summary>
        public int PrimaryTitleId { get; set; }

        /// <summary>
        /// Community_Primary_Contact_Phone_Number
        /// </summary>
        public string PrimaryPhone { get; set; }

        /// <summary>
        /// Community_Primary_Contact_Phone_Number_Type  PhoneType
        /// </summary>
        public byte PrimaryPhoneType { get; set; }

        /// <summary>
        /// Community_Primary_Contact_Email_Address
        /// </summary>
        public string PrimaryEmail { get; set; }

        /// <summary>
        /// Community_Secondary_Contact_Salutation  UserSalutation
        /// </summary>
        public byte SecondarySalutation { get; set; }

        /// <summary>
        /// Community_Secondary_Contact_Name
        /// </summary>
        public string SecondaryName { get; set; }

        /// <summary>
        /// Community_Secondary_Contact_Title
        /// </summary>
        public int SecondaryTitleId { get; set; }

        /// <summary>
        /// Community_Secondary_Contact_Phone_Number
        /// </summary>
        public string SecondaryPhone { get; set; }

        /// <summary>
        /// Community_Secondary_Contact_Phone_Number_Type  PhoneType
        /// </summary>
        public byte SecondaryPhoneType { get; set; }

        /// <summary>
        /// Community_Secondary_Contact_Email_Address
        /// </summary>
        public string SecondaryEmail { get; set; }

        /// <summary>
        /// Community_Web_Address
        /// </summary>
        public string WebAddress { get; set; }

        public BUPStatus Status { get; set; }

        public string Remark { get; set; }

        public int FundingId { get; set; }

        public int LineNum { get; set; }
    }
}
