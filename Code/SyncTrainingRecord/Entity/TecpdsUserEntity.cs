using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyncTrainingRecord.Entity
{
    public class TecpdsUserEntity
    {
        public int ID { get; set; }
        /// <summary>
        /// CreatedDate
        /// </summary>		
        public DateTime CreatedDate { get; set; }
        /// <summary>
        /// Active = 1,
        ///Pending = 2,
        ///Deferred = 3,
        ///Inactive = 4,
        ///[Description("Incomplete")]
        ///InComplete = 5
        /// </summary>		
        public int Status { get; set; }
        /// <summary>
        /// IsDeleted
        /// </summary>		
        public bool IsDeleted { get; set; }
        /// <summary>
        /// Dr.|MR.|Ms.|Mrs.
        /// </summary>		
        public string Title { get; set; }
        /// <summary>
        /// FirstName
        /// </summary>		
        public string FirstName { get; set; }
        /// <summary>
        /// MiddleInitial
        /// </summary>		
        public string MiddleInitial { get; set; }
        /// <summary>
        /// LastName
        /// </summary>		
        public string LastName { get; set; }
        /// <summary>
        /// PreviousLastName
        /// </summary>		
        public string PreviousLastName { get; set; }
        /// <summary>
        /// BirthDate
        /// </summary>		
        public DateTime BirthDate { get; set; }
        /// <summary>
        /// sex:Female-0,Male-1
        /// </summary>		
        public int Gender { get; set; }
        /// <summary>
        /// HomeMailingAddress
        /// </summary>		
        public string HomeMailingAddress { get; set; }
        /// <summary>
        /// City
        /// </summary>		
        public string City { get; set; }
        /// <summary>
        /// State abbreviation list
        /// </summary>		
        public int State { get; set; }
        /// <summary>
        /// ZipCode
        /// </summary>		
        public string ZipCode { get; set; }
        /// <summary>
        /// If Texas is selected for State, list of counties in Texas.  If state other than Texas is selected, this object is inactive.
        /// </summary>		
        public int County { get; set; }
        /// <summary>
        /// xxx-xxx-xxxx
        /// </summary>		
        public string PrimaryPhoneNumber { get; set; }
        /// <summary>
        /// HomeNumber = 1,
        ///CellNumber = 2,
        ///WorkNumber = 3
        /// </summary>		
        public int PrimaryNumberType { get; set; }
        /// <summary>
        /// xxx-xxx-xxxx
        /// </summary>		
        public string SecondaryPhoneNumber { get; set; }
        /// <summary>
        /// HomeNumber = 1,
        ///CellNumber = 2,
        ///WorkNumber = 3
        /// </summary>		
        public int SecondaryNumberType { get; set; }
        /// <summary>
        /// xxx-xxx-xxxx
        /// </summary>		
        public string FaxNumber { get; set; }
        /// <summary>
        /// WebAddress
        /// </summary>		
        public string WebAddress { get; set; }
        /// <summary>
        /// x@xxx.xxx
        /// </summary>		
        public string PrimaryEmailAddress { get; set; }
        /// <summary>
        /// x@xxx.xxx
        /// </summary>		
        public string SecondaryEmailAddress { get; set; }
        /// <summary>
        /// African American | Alaskan Native | American Indian | Asian | Caucasian | Hispanic | Multiracial | Other
        /// </summary>		
        public int RacialEthnicBackground { get; set; }
        /// <summary>
        /// English | Spanish | Vietnamese | Cantonese | Urdu | Korean | Other
        /// </summary>		
        public int PrimaryLanguage { get; set; }
        /// <summary>
        /// English | Spanish | Vietnamese | Cantonese | Urdu | Korean | Other
        /// </summary>		
        public int SecondaryLanguage { get; set; }
        /// <summary>
        /// ApplicationDate
        /// </summary>		
        public DateTime ApplicationDate { get; set; }

        /// <summary>
        /// LastPaymentDate
        /// </summary>		
        public DateTime LastPaymentDate { get; set; }
        /// <summary>
        /// Trainer only:Professional | Master Professional | Provisional | Registered | Master
        /// </summary>		
        public int Level { get; set; }
        /// <summary>
        /// RenewalDate
        /// </summary>		
        public DateTime RenewalDate { get; set; }
        /// <summary>
        /// ActiveDate
        /// </summary>		
        public DateTime ActiveDate { get; set; }
        /// <summary>
        /// User's Role
        /// Administrator = 1,
        /// Trainer = 3,
        /// CenterDirector = 2,
        /// Practitioner = 4,
        /// LWBAdministrator = 6,
        /// Validator = 7
        /// </summary>		
        public int Role { get; set; }

        /// <summary>
        /// Comments
        /// </summary>		
        public string Comments { get; set; }

        /// <summary>
        /// Highest Level of Education Achieved
        /// </summary>		
        public int Education { get; set; }

        /// <summary>
        /// Number of years working in the field of early childhood
        /// </summary>		
        public int WorkExperience { get; set; }

        /// <summary>
        /// Record user's registe status,default =0
        /// </summary>
        public int RegisteSteps { get; set; }

        public string Scoresheet { get; set; }

        public string TrainingLog { get; set; }
    }
}
