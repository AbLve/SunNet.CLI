using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyncTrainingRecord.Entity
{
    public class TrainingAttendedEntity
    {
        public int ID { get; set; }
        /// <summary>
        /// UserID
        /// </summary>		
        public int UserID { get; set; }
        /// <summary>
        /// CompletionDate
        /// </summary>		
        public DateTime CompletionDate { get; set; }
        /// <summary>
        /// TrainingTitle
        /// </summary>		
        public string TrainingTitle { get; set; }
        /// <summary>
        /// enum:Core Competency Area
        /// </summary>		
        public int CoreCompetencyArea { get; set; }
        /// <summary>
        /// If user select a registered trainer ,it's the trainer's ID;else it's 0;
        /// </summary>		
        public int TrainerID { get; set; }
        /// <summary>
        /// If user select a registered trainer ,it's the trainer's name;else it's empty;
        /// </summary>		
        public string RegisteredTrainerName { get; set; }
        /// <summary>
        /// TrainerName
        /// </summary>		
        public string TrainerName { get; set; }
        /// <summary>
        /// ClockHours
        /// </summary>		
        public decimal ClockHours { get; set; }
        /// <summary>
        /// enums:Training Method 
        /// </summary>		
        public int TrainingMethod { get; set; }
        /// <summary>
        /// UploadCertificate
        /// </summary>		
        public string UploadCertificate { get; set; }
        /// <summary>
        /// CreatedDate
        /// </summary>		
        public DateTime CreatedDate { get; set; }
        /// <summary>
        /// For report
        /// /// CenterDirector = 2,
        ///Practitioner = 4,
        /// </summary>		
        public int Role { get; set; }
        /// <summary>
        /// whether the training is "Trainings attended in the Core Competency Areas for Practitioners or Administrators" or "Trainings attended in the Core Competency Areas for Trainers"
        /// CenterDirector = 2,
        ///Practitioner = 4,
        /// </summary>		
        public int TrainingFor { get; set; }
        /// <summary>
        /// people count of attends
        /// </summary>
        public int Attendees { get; set; }
        /// <summary>
        /// Attended or Presented
        /// </summary>
        public bool ISPresented { get; set; }

        public int IsValid { get; set; }

        public int ValidatorId { get; set; }
        public DateTime ValidTime { get; set; }
    }
}
