using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Sunnet.Cli.Core.Trs.Enums;
using Sunnet.Framework.Core.Base;

namespace Sunnet.Cli.Core.Trs.Entities
{
    public class TRSEventLogEntity : EntityBase<int>
    {
        public int SchoolId { get; set; }

        public DateTime DateCreated { get; set; }

        public string CreatedBy { get; set; }

        public TrsEventType EventType { get; set; }

        public string Comment { get; set; }

        public DateTime ActionRequired { get; set; }

        public bool Notification { get; set; }

        public int CreateBy { get; set; }

        public int UpdateBy { get; set; }

        public TrsAccreditation Accreditation { get; set; }

        public int RelatedId { get; set; }

        [JsonIgnore]
        public virtual ICollection<TRSNotificationEntity> Notifications { get; set; }

        [JsonIgnore]
        public virtual ICollection<TRSEventLogFileEntity> EventLogFiles { get; set; }
    }
}
