using Sunnet.Cli.Core.Common.Enums;
using Sunnet.Framework.Core.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Cli.Core.DataProcess.Entities
{
    public class DataSchoolEntity : EntityBase<int>
    {
        public int GroupId { get; set; }

        public ProcessStatus Status { get; set; }

        public string Remark { get; set; }

        public string District_TEA_ID { get; set; }

        public string School_TEA_ID { get; set; }

        public string School_Name { get; set; }

        public string School_Physical_Address1 { get; set; }

        public string School_Physical_Address2 { get; set; }

        public string School_City { get; set; }

        public string School_County { get; set; }

        public string School_State { get; set; }

        public string School_ZIP { get; set; }

        public string School_Phone_Number { get; set; }

        public PhoneType School_Phone_Number_Type { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int School_Type { get; set; }

        public int School_Percent_At_Risk { get; set; }

        public int? School_Size { get; set; }

        public UserSalutation School_Primary_Contact_Salutation { get; set; }

        public string School_Primary_Contact_Name { get; set; }

        public string  School_Primary_Contact_Title { get; set; }

        public string School_Primary_Contact_Phone_Number { get; set; }

        public PhoneType School_Primary_Contact_Phone_Type { get; set; }

        public string School_Primary_Contact_Email_Address { get; set; }

        public string School_Secondary_Contact_Salutation { get; set; }

        public string School_Secondary_Contact_Name { get; set; }

        public string  School_Secondary_Contact_Title { get; set; }

        public string School_Secondary_Contact_Phone_Number { get; set; }

        public PhoneType School_Secondary_Contact_Phone_Type { get; set; }

        public string School_Secondary_Contact_Email_Address { get; set; }
    }
}
