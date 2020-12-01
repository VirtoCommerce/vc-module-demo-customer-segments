using EntityFrameworkCore.Triggers;
using Microsoft.EntityFrameworkCore;

namespace VirtoCommerce.DemoCustomerSegmentsModule.Data.Repositories
{
    public class VirtoCommerceDemoCustomerSegmentsModuleDbContext : DbContextWithTriggers
    {
        public VirtoCommerceDemoCustomerSegmentsModuleDbContext(DbContextOptions<VirtoCommerceDemoCustomerSegmentsModuleDbContext> options)
          : base(options)
        {
        }

        protected VirtoCommerceDemoCustomerSegmentsModuleDbContext(DbContextOptions options)
            : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //        modelBuilder.Entity<MyModuleEntity>().ToTable("MyModule").HasKey(x => x.Id);
            //        modelBuilder.Entity<MyModuleEntity>().Property(x => x.Id).HasMaxLength(128);
            //        base.OnModelCreating(modelBuilder);
        }
    }
}

