using System.Threading.Tasks;
using VirtoCommerce.DemoSolutionFeaturesModule.Core.Models;

namespace VirtoCommerce.DemoSolutionFeaturesModule.Core.Services
{
    public interface IDemoCustomerSegmentService
    {
        Task<DemoCustomerSegment[]> GetByIdsAsync(string[] ids);

        Task SaveChangesAsync(DemoCustomerSegment[] customerSegments);

        Task DeleteAsync(string[] ids);
    }
}
