using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VirtoCommerce.DemoCustomerSegmentsModule.Data.Models;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Data.Infrastructure;

namespace VirtoCommerce.DemoCustomerSegmentsModule.Data.Repositories
{
    public class DemoCustomerSegmentRepository: DbContextRepositoryBase<DemoCustomerSegmentDbContext>, IDemoCustomerSegmentRepository
    {
        public DemoCustomerSegmentRepository(DemoCustomerSegmentDbContext dbContext) : base(dbContext)
        {
        }

        public IQueryable<DemoCustomerSegmentEntity> CustomerSegments => DbContext.Set<DemoCustomerSegmentEntity>();

        public IQueryable<DemoCustomerSegmentStoreEntity> CustomerSegmentStores => DbContext.Set<DemoCustomerSegmentStoreEntity>();

        public async Task<DemoCustomerSegmentEntity[]> GetByIdsAsync(string[] ids)
        {
            var result = Array.Empty<DemoCustomerSegmentEntity>();

            if (!ids.IsNullOrEmpty())
            {
                result = await CustomerSegments.Where(x => ids.Contains(x.Id)).ToArrayAsync();
                await CustomerSegmentStores.Where(x => ids.Contains(x.CustomerSegmentId)).ToArrayAsync();
            }

            return result;
        }
    }
}
