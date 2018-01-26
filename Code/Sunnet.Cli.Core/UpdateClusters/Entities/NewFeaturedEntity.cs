using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sunnet.Framework.Core.Base;

namespace Sunnet.Cli.Core.UpdateClusters.Entities
{
    public class NewFeaturedEntity : EntityBase<int>
    {
        public string Title { get; set; }

        public string Description { get; set; }

        [Required]
        public string HyperLink { get; set; }

        public string ThumbnailPath { get; set; }

        public string ThumbnailName { get; set; }

        public int CreatedBy { get; set; }

        public int UpdatedBy { get; set; }
    }
}
