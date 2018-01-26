using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sunnet.Cli.Core.Ade;
using System.ComponentModel;

namespace Sunnet.Cli.Business.Ade.Models
{
    /// <summary>
    /// 做Assessment时, 用来显示名称以及双语言版本的模型类
    /// </summary>
    public class BaseAssessmentModel
    {
        public int ID { get; set; }

        public string Name { get; set; }

        public AssessmentLanguage Language { get; set; }

        public int TheOtherId { get; set; }

        public AssessmentLanguage TheOtherLang { get; set; }

        [DisplayName("Parent Report Cover Sheet")]
        public string ParentReportCoverPath { get; set; }

        public bool DisplayPercentileRank { get; set; }
    }
}
