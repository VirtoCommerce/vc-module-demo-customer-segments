using EntityFrameworkCore.Triggers;
using Microsoft.EntityFrameworkCore;
using VirtoCommerce.DemoSolutionFeaturesModule.Data.Models;

namespace VirtoCommerce.DemoSolutionFeaturesModule.Data.Repositories
{
    public class DemoCustomerSegmentDbContext: DbContextWithTriggers
    {
        public DemoCustomerSegmentDbContext(DbContextOptions<DemoCustomerSegmentDbContext> options): base(options)
        {
        }

        protected DemoCustomerSegmentDbContext(DbContextOptions options): base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DemoCustomerSegmentEntity>().ToTable("DemoCustomerSegments").HasKey(x => x.Id);
            modelBuilder.Entity<DemoCustomerSegmentEntity>().Property(x => x.Id).HasMaxLength(128).ValueGeneratedOnAdd();

            modelBuilder.Entity<DemoCustomerSegmentStoreEntity>().ToTable("DemoCustomerSegmentStore");
            modelBuilder.Entity<DemoCustomerSegmentStoreEntity>().HasKey(x => x.Id);
            modelBuilder.Entity<DemoCustomerSegmentStoreEntity>().Property(x => x.Id).HasMaxLength(128).ValueGeneratedOnAdd();
            modelBuilder.Entity<DemoCustomerSegmentStoreEntity>().HasOne(x => x.CustomerSegment)
                .WithMany(x => x.Stores).HasForeignKey(x => x.CustomerSegmentId)
                .OnDelete(DeleteBehavior.Cascade).IsRequired();
            modelBuilder.Entity<DemoCustomerSegmentStoreEntity>().HasIndex(i => i.StoreId);

            base.OnModelCreating(modelBuilder);
        }
    }
}
