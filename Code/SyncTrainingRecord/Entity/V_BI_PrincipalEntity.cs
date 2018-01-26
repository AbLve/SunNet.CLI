using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyncTrainingRecord.Entity
{
    public class V_BI_PrincipalEntity
    {
        public int ID { get; set; }
        public int Status { get; set; }
        public string PrimaryEmailAddress { get; set; }
        public string SecondaryEmailAddress { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string PreviousLastName { get; set; }
        public string GoogleId { get; set; }
        public string PrimaryPhoneNumber { get; set; }
        public int PrimaryNumberType { get; set; }
        public string SecondaryPhoneNumber { get; set; }
        public int SecondaryNumberType { get; set; }
        public string FaxNumber { get; set; }
        public string City { get; set; }
        public int StateId { get; set; }
        public int CountyId { get; set; }
        public string Zip { get; set; }
        public string PrimaryLanguageId { get; set; }
        public string PrimaryLanguageOther { get; set; }
        public string StateName { get; set; }
        public string CountyName { get; set; }
        public DateTime BirthDate { get; set; }
        public int Gender { get; set; }
        public string Address { get; set; }
        public string EngageId { get; set; }
    }
}
