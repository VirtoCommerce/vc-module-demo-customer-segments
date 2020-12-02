using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.DemoCustomerSegmentsModule.Data.Models;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.DemoCustomerSegmentsModule.Data.Repositories
{
    public interface IDemoCustomerSegmentRepository: IRepository
    {
        IQueryable<DemoCustomerSegmentEntity> CustomerSegments { get; }

        IQueryable<DemoCustomerSegmentStoreEntity> CustomerSegmentStores { get; }

        Task<DemoCustomerSegmentEntity[]> GetByIdsAsync(string[] ids);
    }
}
