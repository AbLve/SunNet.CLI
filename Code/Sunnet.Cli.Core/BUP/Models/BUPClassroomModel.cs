using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sunnet.Cli.Core.Common.Enums;

namespace Sunnet.Cli.Core.BUP.Models
{
    public  class BUPClassroomModel
    {
        public int ID { get; set; }

        public int TaskId { get; set; }

        public BUPAction Action { get; set; }

        public string CommunityName { get; set; }

        /// <summary>
        /// Community_Engage_ID
        /// </summary>
        public string CommunityEngageID { get; set; }

        public string SchoolName { get; set; }

        /// <summary>
        /// School_Engage_ID
        /// </summary>
        public string SchoolEngageID { get; set; }

        public string Name { get; set; }

        /// <summary>
        /// Classroom_Engage_ID
        /// </summary>
        public string ClassroomEngageId { get; set; }

        public string ClassroomInternalID { get; set; }

        public BUPStatus Status { get; set; }

        public string Remark { get; set; }

        public int LineNum { get; set; }
    }
}
