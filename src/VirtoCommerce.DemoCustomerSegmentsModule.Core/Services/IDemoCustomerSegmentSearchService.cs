using System.Threading.Tasks;
using VirtoCommerce.DemoSolutionFeaturesModule.Core.Models.Search;

namespace VirtoCommerce.DemoSolutionFeaturesModule.Core.Services
{
    public interface IDemoCustomerSegmentSearchService
    {
        Task<DemoCustomerSegmentSearchResult> SearchCustomerSegmentsAsync(DemoCustomerSegmentSearchCriteria criteria);
    }
}
