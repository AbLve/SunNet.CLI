using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Cli.Core.Reports.Models
{
    public class CoachingHoursbyCommunityModel
    {
        public int CommunityId { get; set; }

        public string CommunityName { get; set; }

        /// <summary>
        /// F2F Available Hours
        /// </summary>
        public decimal F2FAvailableHours { get; set; }

        /// <summary>
        /// Rem Available Cycles
        /// </summary>
        public decimal RemAvailableCycle { get; set; }

        /// <summary>
        /// Nmber of F2F teachers
        /// </summary>
        public int NmberofF2F { get; set; }

        /// <summary>
        /// Number of Rem Teachers
        /// </summary>
        public int NmberofRem { get; set; }

        /// <summary>
        /// Total Teacher
        /// </summary>
        public int TeacherTotal { get; set; }

        /// <summary>
        /// Req Hours
        /// </summary>
        public decimal ReqHours { get; set; }

        /// <summary>
        /// Req Cycles
        /// </summary>
        public decimal ReqCycles { get; set; }
    }
}
