using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MMSite6.Areas.Identity.Data;
using MMSite6.Models;

namespace MMSite6.Models
{
    public class MMSite6Context : IdentityDbContext<MMSite6User>
    {
        public MMSite6Context(DbContextOptions<MMSite6Context> options)
            : base(options)
        {
        }

        public DbSet<MMSite6User> user { get; set; }
        public DbSet<Order> order { get; set; }
        public DbSet<Item> item { get; set; }
        public DbSet<Document> document { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<MMSite6User>().ToTable("ASPNetUsers");
            builder.Entity<Order>().ToTable("Order");
            builder.Entity<Item>().ToTable("Item");
            builder.Entity<Document>().ToTable("Documents");
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
        }
    }
}
