using System.Threading.Tasks;
using VirtoCommerce.DemoCustomerSegmentsModule.Core.Models.Search;

namespace VirtoCommerce.DemoCustomerSegmentsModule.Core.Services
{
    public interface IDemoCustomerSegmentSearchService
    {
        Task<DemoCustomerSegmentSearchResult> SearchCustomerSegmentsAsync(DemoCustomerSegmentSearchCriteria criteria);
    }
}
