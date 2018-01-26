using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyncTrainingRecord.Entity
{
    public class TrainingAttendRecordEntity
    {
        public string cert_code { get; set; }
        public string cert_type { get; set; }
        //[CompletionDate]
        public string cert_timeissued { get; set; }
        //用来查询Engage用户信息，比如GoogleID
        public string learner_cliengageid { get; set; }

        public string learner_name { get; set; }
        public string instructor_cliengageid { get; set; }

        //Trainer Name
        public string instructor_name { get; set; }
        //[TrainingTitle]
        public int course_id { get; set; }
        public string course_name { get; set; }
        public string course_idnumber { get; set; }
        //[ClockHours]
        public decimal course_clockhours { get; set; }

        public string course_deliverymethod { get; set; }
        //[UploadCertificate]
        public string cert_file { get; set; }

        //compentiny area 值，对应的[dbo].[CoreCompetencyAreaEnum]的数据
        public string course_competencies { get; set; }
        public IList<Int32> course_competency_ids { get; set; }
        public string course_strategies { get; set; }
        public IList<Int32> course_strategy_ids { get; set; }
    }
}
