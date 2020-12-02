using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.DemoSolutionFeaturesModule.Data.Models;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.DemoSolutionFeaturesModule.Data.Repositories
{
    public interface IDemoCustomerSegmentRepository: IRepository
    {
        IQueryable<DemoCustomerSegmentEntity> CustomerSegments { get; }

        IQueryable<DemoCustomerSegmentStoreEntity> CustomerSegmentStores { get; }

        Task<DemoCustomerSegmentEntity[]> GetByIdsAsync(string[] ids);
    }
}
