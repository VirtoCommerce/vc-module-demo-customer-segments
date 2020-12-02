using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using VirtoCommerce.DemoSolutionFeaturesModule.Core.Models;
using VirtoCommerce.DemoSolutionFeaturesModule.Core.Models.Search;
using VirtoCommerce.DemoSolutionFeaturesModule.Core.Services;
using VirtoCommerce.DemoSolutionFeaturesModule.Data.Caching;
using VirtoCommerce.DemoSolutionFeaturesModule.Data.Models;
using VirtoCommerce.DemoSolutionFeaturesModule.Data.Repositories;
using VirtoCommerce.Platform.Core.Caching;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Data.Infrastructure;

namespace VirtoCommerce.DemoSolutionFeaturesModule.Data.Services.CustomerSegment
{
    public class DemoCustomerSegmentSearchService: IDemoCustomerSegmentSearchService
    {
        private readonly Func<IDemoCustomerSegmentRepository> _customerSegmentRepositoryFactory;
        private readonly IPlatformMemoryCache _platformMemoryCache;
        private readonly IDemoCustomerSegmentService _customerSegmentService;

        public DemoCustomerSegmentSearchService(Func<IDemoCustomerSegmentRepository> customerSegmentRepositoryFactory, IPlatformMemoryCache platformMemoryCache, IDemoCustomerSegmentService customerSegmentService)
        {
            _customerSegmentRepositoryFactory = customerSegmentRepositoryFactory;
            _platformMemoryCache = platformMemoryCache;
            _customerSegmentService = customerSegmentService;
        }

        public virtual async Task<DemoCustomerSegmentSearchResult> SearchCustomerSegmentsAsync(DemoCustomerSegmentSearchCriteria criteria)
        {
            ValidateParameters(criteria);

            var cacheKey = CacheKey.With(GetType(), nameof(SearchCustomerSegmentsAsync), criteria.GetCacheKey());

            return await _platformMemoryCache.GetOrCreateExclusiveAsync(cacheKey, async cacheEntry =>
            {
                cacheEntry.AddExpirationToken(DemoCustomerSegmentSearchCacheRegion.CreateChangeToken());

                var result = AbstractTypeFactory<DemoCustomerSegmentSearchResult>.TryCreateInstance();

                using (var customerSegmentsRepositoryFactory = _customerSegmentRepositoryFactory())
                {
                    //Optimize performance and CPU usage
                    customerSegmentsRepositoryFactory.DisableChangesTracking();
                    var sortInfos = BuildSortExpression(criteria);
                    var query = BuildQuery(customerSegmentsRepositoryFactory, criteria);

                    result.TotalCount = await query.CountAsync();

                    if (criteria.Take > 0 && result.TotalCount > 0)
                    {
                        var ids = await query.OrderBySortInfos(sortInfos).ThenBy(x => x.Id)
                            .Select(x => x.Id)
                            .Skip(criteria.Skip).Take(criteria.Take)
                            .AsNoTracking()
                            .ToArrayAsync();

                        result.Results = (await _customerSegmentService.GetByIdsAsync(ids)).OrderBy(x => Array.IndexOf(ids, x.Id)).ToList();
                    }
                }

                return result;
            });
        }

        protected virtual IQueryable<DemoCustomerSegmentEntity> BuildQuery(IDemoCustomerSegmentRepository repository, DemoCustomerSegmentSearchCriteria criteria)
        {
            var query = repository.CustomerSegments;

            if (!string.IsNullOrEmpty(criteria.Keyword))
            {
                query = query.Where(x => x.Name.Contains(criteria.Keyword));
            }

            if (!criteria.StoreIds.IsNullOrEmpty())
            {
                query = query.Where(x => !x.Stores.Any() || x.Stores.Any(s => criteria.StoreIds.Contains(s.StoreId)));
            }

            if (criteria.IsActive != null)
            {
                var utcNow = DateTime.UtcNow;
                query = query.Where(x => x.IsActive == criteria.IsActive && (x.StartDate == null || utcNow >= x.StartDate) && (x.EndDate == null || x.EndDate >= utcNow));
            }

            return query;
        }

        protected virtual IList<SortInfo> BuildSortExpression(DemoCustomerSegmentSearchCriteria criteria)
        {
            var sortInfos = criteria.SortInfos;

            if (sortInfos.IsNullOrEmpty())
            {
                sortInfos = new[] { new SortInfo { SortColumn = nameof(DemoCustomerSegment.Name) } };
            }

            return sortInfos;
        }

        private static void ValidateParameters(DemoCustomerSegmentSearchCriteria criteria)
        {
            if (criteria == null)
            {
                throw new ArgumentNullException(nameof(criteria));
            }
        }
    }
}
