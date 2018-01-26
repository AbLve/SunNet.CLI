using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sunnet.Cli.Core.Trs;

namespace Sunnet.Cli.Business.Trs.Models
{
    public class TrsRecentStarModel
    {
        public Dictionary<TRSCategoryEnum, TRSStarDisplayEnum> CurrentStars { get; set; }

        public Dictionary<TRSCategoryEnum, TRSStarDisplayEnum> RecentStars { get; set; }

        public bool HasRecentStar { get; set; }

        public TRSStarEnum RecentVerifiedStar { get; set; }
    }
}
