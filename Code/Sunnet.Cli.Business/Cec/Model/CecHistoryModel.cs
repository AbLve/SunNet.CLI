using Sunnet.Cli.Business.Ade.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sunnet.Cli.Core.Ade.Entities;

namespace Sunnet.Cli.Business.Cec.Model
{
    public class CecHistoryModel
    {
        public int MeasureId { get; set; }

        public string MeasureName { get; set; }

        public List<CecHistoryModel> Childer { get; set; }

        public List<CecItemModel> List { get; set; }

        public List<AdeLinkEntity> Links { get; set; }
    }
}
