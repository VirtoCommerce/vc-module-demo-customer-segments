using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using VirtoCommerce.DemoSolutionFeaturesModule.Core.Events;
using VirtoCommerce.DemoSolutionFeaturesModule.Core.Models;
using VirtoCommerce.DemoSolutionFeaturesModule.Core.Services;
using VirtoCommerce.DemoSolutionFeaturesModule.Data.Caching;
using VirtoCommerce.DemoSolutionFeaturesModule.Data.Models;
using VirtoCommerce.DemoSolutionFeaturesModule.Data.Repositories;
using VirtoCommerce.Platform.Core.Caching;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Events;
using VirtoCommerce.Platform.Data.Infrastructure;

namespace VirtoCommerce.DemoSolutionFeaturesModule.Data.Services.CustomerSegment
{
    public class DemoCustomerSegmentService: IDemoCustomerSegmentService
    {
        private readonly Func<IDemoCustomerSegmentRepository> _customerSegmentRepositoryFactory;
        private readonly IPlatformMemoryCache _platformMemoryCache;
        private readonly IEventPublisher _eventPublisher;

        public DemoCustomerSegmentService(
            Func<IDemoCustomerSegmentRepository> customerSegmentRepositoryFactory,
            IPlatformMemoryCache platformMemoryCache,
            IEventPublisher eventPublisher)
        {
            _customerSegmentRepositoryFactory = customerSegmentRepositoryFactory;
            _platformMemoryCache = platformMemoryCache;
            _eventPublisher = eventPublisher;
        }

        public virtual async Task<DemoCustomerSegment[]> GetByIdsAsync(string[] ids)
        {
            var cacheKey = CacheKey.With(GetType(), nameof(GetByIdsAsync), string.Join("-", ids.OrderBy(x => x)));

            var result = await _platformMemoryCache.GetOrCreateExclusiveAsync(cacheKey, async cacheEntry =>
            {
                var rules = Array.Empty<DemoCustomerSegment>();

                if (!ids.IsNullOrEmpty())
                {
                    using var customerSegmentsRepository = _customerSegmentRepositoryFactory();

                    //Optimize performance and CPU usage
                    customerSegmentsRepository.DisableChangesTracking();

                    var entities = await customerSegmentsRepository.GetByIdsAsync(ids);

                    rules = entities
                        .Select(x => x.ToModel(AbstractTypeFactory<DemoCustomerSegment>.TryCreateInstance()))
                        .ToArray();

                    cacheEntry.AddExpirationToken(DemoCustomerSegmentCacheRegion.CreateChangeToken(ids));
                }

                return rules;
            });

            return result;
        }

        public virtual async Task SaveChangesAsync(DemoCustomerSegment[] customerSegments)
        {
            var pkMap = new PrimaryKeyResolvingMap();
            var changedEntries = new List<GenericChangedEntry<DemoCustomerSegment>>();

            using var customerSegmentsRepository = _customerSegmentRepositoryFactory();

            var ids = customerSegments.Where(x => !x.IsTransient()).Select(x => x.Id).ToArray();
            var dbExistProducts = await customerSegmentsRepository.GetByIdsAsync(ids);

            foreach (var customerSegment in customerSegments)
            {
                var modifiedEntity = AbstractTypeFactory<DemoCustomerSegmentEntity>.TryCreateInstance().FromModel(customerSegment, pkMap);
                var originalEntity = dbExistProducts.FirstOrDefault(x => x.Id == customerSegment.Id);

                if (originalEntity != null)
                {
                    changedEntries.Add(new GenericChangedEntry<DemoCustomerSegment>(customerSegment, originalEntity.ToModel(AbstractTypeFactory<DemoCustomerSegment>.TryCreateInstance()), EntryState.Modified));
                    modifiedEntity.Patch(originalEntity);
                }
                else
                {
                    customerSegmentsRepository.Add(modifiedEntity);
                    changedEntries.Add(new GenericChangedEntry<DemoCustomerSegment>(customerSegment, EntryState.Added));
                }
            }

            await _eventPublisher.Publish(new DemoCustomerSegmentChangingEvent(changedEntries));

            await customerSegmentsRepository.UnitOfWork.CommitAsync();
            pkMap.ResolvePrimaryKeys();

            ClearCache(customerSegments);

            await _eventPublisher.Publish(new DemoCustomerSegmentChangedEvent(changedEntries));
        }

        public virtual async Task DeleteAsync(string[] ids)
        {
            var items = await GetByIdsAsync(ids);
            var changedEntries = items
                .Select(x => new GenericChangedEntry<DemoCustomerSegment>(x, EntryState.Deleted))
                .ToArray();

            using var customerSegmentsRepositoryFactory = _customerSegmentRepositoryFactory();

            await _eventPublisher.Publish(new DemoCustomerSegmentChangingEvent(changedEntries));

            var customerSegmentEntities = await customerSegmentsRepositoryFactory.GetByIdsAsync(ids);

            foreach (var customerSegmentEntity in customerSegmentEntities)
            {
                customerSegmentsRepositoryFactory.Remove(customerSegmentEntity);
            }

            await customerSegmentsRepositoryFactory.UnitOfWork.CommitAsync();

            ClearCache(items);

            await _eventPublisher.Publish(new DemoCustomerSegmentChangedEvent(changedEntries));
        }


        protected virtual void ClearCache(IEnumerable<DemoCustomerSegment> customerSegments)
        {
            foreach (var customerSegment in customerSegments)
            {
                DemoCustomerSegmentCacheRegion.ExpireEntity(customerSegment);
            }

            DemoCustomerSegmentSearchCacheRegion.ExpireRegion();
        }
    }
}
