using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Sunnet.Cli.MainSite.Areas.Demo.Models
{
    public class OrderDbContext:DbContext
    {
        public OrderDbContext()
            : base("OrderDbContext")
        {
            this.Configuration.ProxyCreationEnabled = false;
        }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            //modelBuilder.Entity<OrderDetail>().HasKey(detail => new { detail.Order, detail.Product });
        }

        public DbSet<Products> Products { get; set; }
    }
}