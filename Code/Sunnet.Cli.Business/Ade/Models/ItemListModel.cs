using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		JackZhang
 * Computer:		JACKZ
 * Domain:			Jackz
 * CreatedOn:		2014/9/1 0:36:32
 * Description:		Please input class summary
 * Version History:	Created,2014/9/1 0:36:32
 * 
 * 
 **************************************************************************/
using Sunnet.Cli.Core.Ade;
using Sunnet.Cli.Core.Ade.Entities;
using Sunnet.Cli.Core.Common.Enums;

namespace Sunnet.Cli.Business.Ade.Models
{
    public class ItemListModel
    {
        private string _createdByName;
        private List<AdeLinkEntity> _links;
        public int ID { get; set; }

        public MeasureModel Measure { get; set; }
        public int MeasureId { get; set; }

        public bool Locked { get; set; }
        public string Label { get; set; }

        public ItemType Type { get; set; }
        public string Description { get; set; }

        public int CreatedBy { get; set; }
        [DisplayName("Created By")]
        public string CreatedByName
        {
            get { return _createdByName ?? (_createdByName = ""); }
            set { _createdByName = value; }
        }


        public EntityStatus Status { get; set; }

        [DisplayName("Updated")]
        public DateTime UpdatedOn { get; set; }

        public bool IsShown { get; set; }

        public List<AdeLinkEntity> Links
        {
            get { return _links ?? (_links = new List<AdeLinkEntity>()); }
            set { _links = value; }
        }

        public int BranchingScoresLength { get; set; }
    }
}
