using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sunnet.Cli.Core.Ade.Entities;
using Sunnet.Cli.Core.Common.Enums;
using Sunnet.Framework.Core.Base;

namespace Sunnet.Cli.Core.Communities.Entities
{
    public class CommunityAssessmentRelationsEntity : EntityBase<int>
    {
        public CommunityAssessmentRelationsEntity()
        {
            AssessmentName = "";
            Comment = "";
        }

         public int CommunityId { get; set; }
        public int AssessmentId { get; set; }
        public EntityStatus Status { get; set; }
        public int CreatedBy { get; set; }
        public int UpdatedBy { get; set; }
        public string Comment { get; set; }
        public bool Isrequest { get; set; }
        public string  AssessmentName { get; set; }
        public string ClassLevelIds { get; set; }
        public virtual CommunityEntity Community { get; set; }
    }
}
