using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyncTrainingRecord.Entity
{
    public class ValidatorEntity
    {
        /// <summary>
        /// ID
        /// </summary>	
        public int ID { get; set; }
        /// <summary>
        /// First Name
        /// </summary>		
        public string FirstName { get; set; }
        /// <summary>
        /// Last Name
        /// </summary>		
        public string LastName { get; set; }
        /// <summary>
        /// Google ID: @gmail.com email address
        /// </summary>	
        public string GoogleGmail { get; set; }
        /// <summary>
        /// Status
        /// </summary>		
        public int Status { get; set; }

        /// <summary>
        /// Created Date
        /// </summary>		
        public DateTime CreatedDate { get; set; }
        /// <summary>
        /// Updated Date
        /// </summary>		
        public DateTime UpdatedDate { get; set; }
        /// <summary>
        /// Comments
        /// </summary>
        public string Comments { get; set; }
        /// <summary>
        /// Scoresheet
        /// </summary>
        public string Scoresheet { get; set; }
        /// <summary>
        /// LastLoginDate
        /// </summary>
        public DateTime LastLoginDate { get; set; }
    }
}
