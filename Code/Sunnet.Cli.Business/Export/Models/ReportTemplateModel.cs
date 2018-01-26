using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Sunnet.Cli.Core.Export.Entities;

namespace Sunnet.Cli.Business.Export.Models
{
    public class ReportTemplateModel
    {
        public int ID { get; set; }

        public int CommunityId { get; set; }

        public string CommunityFields { get; set; }

        public List<FieldMapEntity> SchoolFields { get; set; }

        public List<FieldMapEntity> ClassroomFields { get; set; }

        public List<FieldMapEntity> ClassFields { get; set; }

        public List<FieldMapEntity> StudentFields { get; set; }
    }
}
