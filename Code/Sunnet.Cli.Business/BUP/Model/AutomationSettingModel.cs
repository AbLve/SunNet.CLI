using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Sunnet.Cli.Core.BUP.Entities;
using Sunnet.Cli.Core.Common.Enums;

namespace Sunnet.Cli.Business.BUP.Model
{
    public class AutomationSettingModel : AutomationSettingEntity
    {
        [DisplayName("Community/District Name")]
        public string CommunityName { get; set; }
    }
}
