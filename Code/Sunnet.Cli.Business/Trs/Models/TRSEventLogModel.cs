using Sunnet.Cli.Core.Trs.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Cli.Business.Trs.Models
{
    public class TRSEventLogModel
    {
        public int ID { get; set; }

        public int SchoolId { get; set; }

        public DateTime DateCreated { get; set; }

        public string CreatedBy { get; set; }

        public TrsEventType EventType { get; set; }

        public string Comment { get; set; }

        public DateTime ActionRequired { get; set; }

        public bool Notification { get; set; }

        public TrsAccreditation Accreditation { get; set; }

        public int RelatedId { get; set; }

        public DateTime CreatedOn { get; set; }

        public IEnumerable<TRSEventLogFileModel> Documents { get; set; }
    }

    public class TRSEventLogFileModel
    {
        public int ID { get; set; }

        public int EventLogId { get; set; }

        public string FileName { get; set; }

        public string FilePath { get; set; }

        public string FileServerPath { get; set; }

        public bool IsDelete { get; set; }

        public int CreatedBy { get; set; }

        public bool IsAllowDel { get; set; }
    }
}
