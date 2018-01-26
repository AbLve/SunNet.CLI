using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sunnet.Cli.Core.Common.Enums;

namespace Sunnet.Cli.Core.BUP.Models
{
    public class BUPClassModel
    {
        public int ID { get; set; }

        public int TaskId { get; set; }

        public BUPAction Action { get; set; }

        public string CommunityName { get; set; }

        public string CommunityEngageID { get; set; }

        public string SchoolName { get; set; }

        public string SchoolEngageID { get; set; }

        public string Name { get; set; }

        public string ClassEngageID { get; set; }

        public string ClassInternalID { get; set; }

        public byte DayType { get; set; }

        public string ClassroomName { get; set; }

        public string ClassroomEngageID { get; set; }

        public BUPStatus Status { get; set; }

        public string Remark { get; set; }

        public int LineNum { get; set; }

        public string HomeroomTeacherFirst { get; set; }

        public string HomeroomTeacherLast { get; set; }

        public string HomeroomTeacherEngageId { get; set; }

        public BUPEntityStatus ClassStatus { get; set; }
    }
}
