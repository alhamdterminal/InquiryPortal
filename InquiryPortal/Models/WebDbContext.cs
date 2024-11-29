

using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace InquiryPortal.Models
{
    public class WebDbContext : DbContext
    { // Constructor accepting DbContextOptions to configure the context
        public WebDbContext(DbContextOptions<WebDbContext> options)
            : base(options)
        {
        }

       

        public DbSet<ContainerIndex> ContainerIndex { get; set; }
       
        public DbSet<ContainerInfo> ContainerInfos { get; set; }



        // Configuring the model (optional)
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ContainerIndex>().HasNoKey();
            modelBuilder.Entity<ContainerInfo>().HasNoKey();
          
        }
    }
}
