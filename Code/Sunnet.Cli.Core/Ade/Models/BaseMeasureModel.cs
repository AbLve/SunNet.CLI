using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sunnet.Cli.Core.Common.Enums;

namespace Sunnet.Cli.Core.Ade.Models
{
    public class BaseMeasureModel
    {
        public int ID { get; set; }

        public string Name { get; set; }

        public string Label { get; set; }

        public string ShortName { get; set; }

        public int ParentId { get; set; }

        public AssessmentLanguage AssessmentLanguage { get; set; }

        public EntityStatus Status { get; set; }

        public int ParentSort { get; set; }

        public int Sort { get; set; }

        public bool HasCutPoint { get; set; }

    }
}
