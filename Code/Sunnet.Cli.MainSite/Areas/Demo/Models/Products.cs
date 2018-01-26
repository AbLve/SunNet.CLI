using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using Sunnet.Framework.Mvc;

namespace Sunnet.Cli.MainSite.Areas.Demo.Models
{
    public class Products : IEntity
    {
        [Key]
        public int ID { get; set; }
        [Required]
        [StringLength(64)]
        public string Name { get; set; }
        [Required]
        [StringLength(512)]
        [Display(Name = "Product Description")]
        public string Description { get; set; }
        [Required]
        [StringLength(64)]
        public string Logo { get; set; }
        [Required]
        [Range(0, 10000)]
        public decimal Price { get; set; }

        [DataType(DataType.Date)]
        [DisplayName("日期")]
        public DateTime CreatedOn { get; set; }

        [DataType("Time")]
        [DisplayName("时间")]
        public DateTime CreatedOnTime { get; set; }

        [DataType(DataType.DateTime)]
        [DisplayName("日期+时间")]
        public DateTime CreatedOnDateTime { get; set; }
    }
}