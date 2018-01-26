using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sunnet.Framework.Core.Base;

namespace Sunnet.Cli.Core.Tsds.Entities
{
    public class TsdsAssessmentFileEntity : EntityBase<int>
    {
        public int AssessmentId { get; set; }

        public byte Status { get; set; }

        public string FileName { get; set; }

        public string FileSize { get; set; }

        public string DownloadUrl { get; set; }

        public int CreatedBy { get; set; }
    }
}
