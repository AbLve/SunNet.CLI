using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sunnet.Cli.Core.Common.Enums;
using Sunnet.Cli.Core.Communities.Entities;
using Sunnet.Cli.Core.Users.Entities;
using Sunnet.Cli.Core.Vcw.Entities;
using Sunnet.Framework.Core.Base;

namespace Sunnet.Cli.Core.Tsds.Entities
{
    public class TsdsEntity : EntityBase<int>
    {
        public string FileName { get; set; }

        public int AssessmentId { get; set; }

        public int CommunityId { get; set; }

        public string SchoolIds { get; set; }

        public string MeasureIds { get; set; }
            
        public int DownloadBy { get; set; }
        public Nullable<DateTime> DOBStartDate { get; set; }
        public Nullable<DateTime> DOBEndDate { get; set; }

        public DateTime DownloadOn { get; set; }

        public TsdsStatus Status { get; set; }

        public string Comment { get; set; }

        public string SchoolNames { get; set; }
        public string MetaDataFile { get; set; }
        public string StudentParentFile { get; set; }
        public string ErrorFileName { get; set; }
        public virtual V_CommunityEntity Community { get; set; }

        public virtual V_UserEntity DownloadUser { get; set; }

    }
}
