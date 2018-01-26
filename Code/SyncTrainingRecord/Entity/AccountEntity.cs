using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyncTrainingRecord.Entity
{
    public class AccountEntity
    {
        public int ID { get; set; }
        /// <summary>
        /// TrainerID
        /// </summary>		
        public int TrainerID { get; set; }
        /// <summary>
        /// PractitionerID
        /// </summary>		
        public int PractitionerID { get; set; }
        /// <summary>
        /// CenteDirectorID
        /// </summary>		
        public int CenteDirectorID { get; set; }
        /// <summary>
        /// GoogleID
        /// </summary>		
        public string GoogleID { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastLoginDate { get; set; }
        public int Status { get; set; }
        public string LoginAccount { get; set; }
        public string LoginPassword { get; set; }
        public int DefaultRole { get; set; }
    }
}
