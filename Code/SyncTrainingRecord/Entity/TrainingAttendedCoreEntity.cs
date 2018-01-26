using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyncTrainingRecord.Entity
{
    public class TrainingAttendedCoreEntity
    {
        public int ID { get; set; }
        public int ParentID { get; set; }
        public int CoreCompetencyAreaID { get; set; }
        /// <summary>
        /// =1
        /// </summary>
        public int Type { get; set; }
    }
}
