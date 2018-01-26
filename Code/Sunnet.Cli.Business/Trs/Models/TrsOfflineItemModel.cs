using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Sunnet.Cli.Core.Trs;
using Sunnet.Cli.Core.Trs.Enums;

namespace Sunnet.Cli.Business.Trs.Models
{
    public class TrsOfflineItemModel
    {
        //TRSAssessmentItem.id
        public int Id { get; set; }

        //TRSItems.id
        public int ItemId { get; set; }

        public int AnswerId { get; set; }

        public int ClassId { get; set; }

        public string Comments { get; set; }

        public int SubCategoryId { get; set; }

        public bool IfCanAccess { get; set; }
    }
}
