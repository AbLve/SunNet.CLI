using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Sunnet.Framework.Core.Base
{
    public abstract class EntityBase<TKey>
    {
        /// <summary>
        /// Default Constructor.
        /// </summary>
        protected EntityBase()
        {
            this.CreatedOn = DateTime.Now;
            this.UpdatedOn = DateTime.Now;
        }

        [Key]
        public TKey ID { get; set; }


        /// <summary>
        /// 获取或设置 添加时间
        /// </summary>
        [DataType(DataType.DateTime)]
        [Range(typeof(DateTime), "1753-1-1", "2100-1-1")]
        [DisplayName("Created On")]
        public DateTime CreatedOn { get; set; }

        /// <summary>
        /// 获取或设置　修改时间；也可以用来进行　版本控制标识，用于处理并发
        /// </summary>
        [DataType(DataType.DateTime)]
        [DisplayName("Updated On")]
        [Range(typeof(DateTime), "1753-1-1", "2100-1-1")]
        public DateTime UpdatedOn { get; set; }
    }
}
