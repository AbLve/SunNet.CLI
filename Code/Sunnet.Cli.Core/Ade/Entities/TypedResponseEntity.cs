using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sunnet.Framework.Core.Base;

namespace Sunnet.Cli.Core.Ade.Entities
{
    public class TypedResponseEntity : EntityBase<int>
    {
        private ICollection<TypedResponseOptionEntity> _options;
        public int ItemId { get; set; }

        public bool Required { get; set; }

        public TypedResponseType Type { get; set; }

        public int Length { get; set; }

        public string Text { get; set; }

        public string Picture { get; set; }

        public int TextTimeIn { get; set; }

        public int PictureTimeIn { get; set; }

        public bool IsDeleted { get; set; }

        public virtual TypedResponseItemEntity Item { get; set; }

        public virtual ICollection<TypedResponseOptionEntity> Options
        {
            get { return _options ?? (_options = new List<TypedResponseOptionEntity>()); }
            set { _options = value; }
        }
    }
}
