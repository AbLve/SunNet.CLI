using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sunnet.Cli.Core.Classes.Enums;
using Sunnet.Cli.Core.Common.Enums;
using Sunnet.Cli.Core.TRSClasses.Enums;

namespace Sunnet.Cli.Business.TRSClasses.Models
{
    public class ChildrenTypeModel
    {
        public int ID { get; set; }

        public string Name { get; set; }

       // public string Type { get; set; }

        /// <summary>
        /// 年龄段排序
        /// </summary>
        public int AgeSort { get; set; }

        public EntityStatus Status { get; set; }

        public TrsAgeArea AgeArea { get; set; }

        /// <summary>
        /// 是否选中的项（已存在的Class，编辑时才有意义）
        /// </summary>
        public bool Selected { get; set; }

        /// <summary>
        /// # of Children （已存在的Class，编辑时才有意义）
        /// </summary>
        public int Count { get; set; }
    }
}
