using Sunnet.Cli.Core.Cec.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Cli.Business.Cec.Model
{
    public class CECOfflineModel
    {
        public List<CECOfflineTeacherModel> TeacherList { get; set; }

        List<CecHistoryModel> list = new List<CecHistoryModel>();

        public List<CecHistoryModel> CECItemList { get; set; }
    }
}