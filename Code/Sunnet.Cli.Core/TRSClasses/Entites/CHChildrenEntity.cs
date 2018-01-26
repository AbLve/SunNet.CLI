using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sunnet.Cli.Core.Common.Enums;
using Sunnet.Cli.Core.TRSClasses.Enums;
using Sunnet.Framework.Core.Base;

namespace Sunnet.Cli.Core.TRSClasses.Entites
{
    public class CHChildrenEntity : EntityBase<int>
    {
        public string Name { get; set; }
        
        /// <summary>
        /// 年龄段排序
        /// </summary>
        public int AgeSort { get; set; }
        public EntityStatus Status { get; set; }
        public TrsAgeArea AgeArea { get; set; }
    }
}
