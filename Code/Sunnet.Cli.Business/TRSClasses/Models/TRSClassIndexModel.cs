using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sunnet.Cli.Core.Common.Enums;
using Newtonsoft.Json;

namespace Sunnet.Cli.Business.TRSClasses.Models
{
    public class TRSClassIndexModel
    {
        public int ID { get; set; }
        public string TRSClassId { get; set; }
        public string TRSClassName { get; set; }
        public EntityStatus Status { get; set; }
        public int CommunityId { get; set; }
        public int SchoolId { get; set; }
        public int PlaygroundId { get; set; }
    }
}
