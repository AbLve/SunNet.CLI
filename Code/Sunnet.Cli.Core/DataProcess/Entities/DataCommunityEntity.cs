using Sunnet.Cli.Core.Common.Enums;
using Sunnet.Framework.Core.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Cli.Core.DataProcess.Entities
{
    public class DataCommunityEntity : EntityBase<int>
    {
        public int GroupId { get; set; }

        public int CommunityId { get; set; }

        public string Transaction_Type { get; set; }

        public string District_TEA_ID { get; set; }

        public string District_Name { get; set; }

        public string District_Physical_Address1 { get; set; }

        public string District_Physical_Address2 { get; set; }

        public string District_City { get; set; }

        public string District_ZIP { get; set; }

        public string District_Phone_Number { get; set; }

        public byte District_Phone_Number_Type { get; set; }

        public UserSalutation District_Primary_Contact_Salutation { get; set; }

        public string  District_Primary_Contact_Title { get; set; }

        public string District_Primary_Contact_Phone_Number { get; set; }

        public byte District_Primary_Contact_Phone_Number_Type { get; set; }

        public string District_Primary_Contact_Email_Address { get; set; }

        public UserSalutation District_Secondary_Contact_Salutation { get; set; }

        public string District_Secondary_Contact_Name { get; set; }

        public string  District_Secondary_Contact_Title { get; set; }

        public string District_Secondary_Contact_Phone_Number { get; set; }

        public byte District_Secondary_Contact_Phone_Number_Type { get; set; }

        public string District_Secondary_Contact_Email_Address { get; set; }

        public string District_Web_Address { get; set; }
    }
}
