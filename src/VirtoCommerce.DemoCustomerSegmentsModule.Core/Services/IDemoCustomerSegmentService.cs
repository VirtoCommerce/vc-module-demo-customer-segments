using System.Threading.Tasks;
using VirtoCommerce.DemoCustomerSegmentsModule.Core.Models;

namespace VirtoCommerce.DemoCustomerSegmentsModule.Core.Services
{
    public interface IDemoCustomerSegmentService
    {
        Task<DemoCustomerSegment[]> GetByIdsAsync(string[] ids);

        Task SaveChangesAsync(DemoCustomerSegment[] customerSegments);

        Task DeleteAsync(string[] ids);
    }
}
